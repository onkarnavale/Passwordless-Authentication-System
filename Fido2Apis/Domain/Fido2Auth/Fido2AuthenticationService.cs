using Fido2Apis.Application.Apis.Fido2Auth;
using Fido2Apis.Application.Response;
using Fido2Apis.Infra.Db;
using Fido2Apis.Infra.Jwt;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Fido2Apis.Domain.Fido2Auth
{
    public class Fido2AuthenticationService : IFido2AuthenticationService
    {
        private readonly IFido2 _fido2;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<Fido2AuthenticationService> _logger;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public Fido2AuthenticationService(IFido2 fido2, ApplicationDbContext applicationDbContext, IMemoryCache memoryCache, ILogger<Fido2AuthenticationService> logger, JwtTokenGenerator jwtTokenGenerator)
        {
            _fido2 = fido2;
            _applicationDbContext = applicationDbContext;
            _memoryCache = memoryCache;
            _logger = logger;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public ResponseObject GetAssertionOptions(Fido2AuthenticationRequest fido2AuthenticationRequest)
        {
            ResponseObject responseObject = new ResponseObject();
            try
            {
                Console.WriteLine("name in authentication: " + fido2AuthenticationRequest.Username);

                var allowedCredentials = new List<PublicKeyCredentialDescriptor>();

                if (!string.IsNullOrEmpty(fido2AuthenticationRequest.Username))
                {
                    allowedCredentials = _applicationDbContext.fido2_credentials.Where(x => x.UserName == fido2AuthenticationRequest.Username).Select(x => x.Descriptor).ToList();
                }

                var asssertionOptions = _fido2.GetAssertionOptions(allowedCredentials, UserVerificationRequirement.Preferred);

                if (asssertionOptions != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
                        SlidingExpiration = TimeSpan.FromSeconds(30),
                        Priority = CacheItemPriority.High,
                        Size = 1024
                    };

                    _memoryCache.Set("asssertionOptions", asssertionOptions.ToJson(), cacheOptions);

                    responseObject.Success = true;
                    responseObject.Message = "Assertion options created sucessfully";
                    responseObject.Data = asssertionOptions;
                    responseObject.code = 200;

                    _logger.LogInformation("Assertion options created successfully");

                    return responseObject;
                }
                else
                {
                    responseObject.Success = false;
                    responseObject.Message = "Failed to get assertion options";
                    responseObject.code = 400;

                    _logger.LogInformation("Failed to get assertion options");

                    return responseObject;
                }
            }
            catch (Exception ex)
            {
                responseObject.Success = false;
                responseObject.Message = "Internal Server Error: " + ex.Message;
                responseObject.code = 500;

                _logger.LogError("Internal Server Error: " + ex.Message);
            
                return responseObject;
            }
        }

        public async Task<ResponseObject> MakeAssertion(AuthenticatorAssertionRawResponse authenticatorAssertionRawResponse)
        {
            ResponseObject responseObject = new ResponseObject();
            try
            {
                var cachedOptions = _memoryCache.Get("asssertionOptions").ToString();
                var asssertionOptions = AssertionOptions.FromJson(cachedOptions);
                var credentialIdString = Base64Url.Encode(authenticatorAssertionRawResponse.Id);

                if (asssertionOptions != null)
                {
                    var existingFido2Credentials = _applicationDbContext.fido2_credentials.Where(x => x.DescriptorJson.Contains(credentialIdString)).FirstOrDefault();

                    if (existingFido2Credentials != null)
                    {
                        var assertionResult = await _fido2.MakeAssertionAsync(authenticatorAssertionRawResponse, asssertionOptions, existingFido2Credentials.PublicKey, existingFido2Credentials.SignatureCounter, isUserHandleOwnerOfCredentialIdCallback);
                    
                        if (assertionResult != null)
                        {
                            existingFido2Credentials.SignatureCounter = assertionResult.Counter;

                            _applicationDbContext.fido2_credentials.Update(existingFido2Credentials);

                            int result = _applicationDbContext.SaveChanges();

                            if (result > 0)
                            {
                                JwtPayLoadData jwtPayLoadData = new JwtPayLoadData()
                                {
                                    Name = existingFido2Credentials.UserName
                                };

                                var tokenString = _jwtTokenGenerator.GenerateJwtToken(jwtPayLoadData);

                                responseObject.Success = true;
                                responseObject.Message = "Authentication successful";
                                responseObject.Data = new { name = existingFido2Credentials.UserName, accessToken = tokenString};
                                responseObject.code = 200;

                                _logger.LogInformation("Authentication successful");

                                return responseObject;
                            }
                            else
                            {
                                responseObject.Success = false;
                                responseObject.Message = "Authentication failed";
                                responseObject.code = 400;

                                _logger.LogInformation("Authentication failed, due to failure in updating counter in database table");

                                return responseObject;
                            }
                        }
                        else
                        {
                            responseObject.Success = false;
                            responseObject.Message = "Authencation failed";
                            responseObject.code = 400;

                            _logger.LogInformation("Authentication failed, due to failure in making assertion");

                            return responseObject;
                        }
                    }
                    else
                    {
                        responseObject.Success = false;
                        responseObject.Message = "Authentication failed, due to credentials not found";
                        responseObject.code = 400;

                        _logger.LogInformation("Authentication failed, due to credentials not found in database table");

                        return responseObject;
                    }
                }
                else
                {
                    responseObject.Success = false;
                    responseObject.Message = "Authentication failed, due to assertions options not found in cache";
                    responseObject.code = 400;

                    _logger.LogInformation("Authentication failed, due to assertions options not found in cache");

                    return responseObject;
                }
            }
            catch (Exception ex )
            {
                responseObject.Success = false;
                responseObject.Message = "Internal Server Error: " + ex.Message;
                responseObject.code = 500;

                _logger.LogError("Internal Server Error: " + ex.Message);

                return responseObject;
            }

            throw new NotImplementedException();
        }

        private async Task<bool> isUserHandleOwnerOfCredentialIdCallback(IsUserHandleOwnerOfCredentialIdParams args, CancellationToken cancellationToken)
        {
            var storedCreds = _applicationDbContext.fido2_credentials.Where(c => c.UserHandle.SequenceEqual(args.UserHandle)).ToList();

            var credentialIdExists = storedCreds.Exists(c => c.Descriptor.Id.SequenceEqual(args.CredentialId));

            return credentialIdExists;
        }

        public async Task<ResponseObject> getProfiles(ClaimsPrincipal claimsPrincipal)
        {
            ResponseObject responseObject = new ResponseObject();
            try
            {
                Fido2Credential existingFido2Credentials;

                var currentUser = claimsPrincipal;

                var userNameFromToken = currentUser.Claims.FirstOrDefault(x => x.Type == "name")?.Value;

                existingFido2Credentials = _applicationDbContext.fido2_credentials.Where(x => x.UserName == userNameFromToken).FirstOrDefault();

                responseObject.Success = false;
                responseObject.Message = "User credentials";
                responseObject.Data = new { Id = existingFido2Credentials.Id, userName = existingFido2Credentials.UserName, aaguid = existingFido2Credentials.AaGuid, date = existingFido2Credentials.RegDate};
                responseObject.code = 200;

                return responseObject;
            }
            catch (Exception ex)
            {
                responseObject.Success = false;
                responseObject.Message = "Internal Server Error: " + ex.Message;
                responseObject.code = 500;

                _logger.LogInformation("Internal Server Error: " + ex.Message);

                return responseObject;
            }
        }
    }
}
