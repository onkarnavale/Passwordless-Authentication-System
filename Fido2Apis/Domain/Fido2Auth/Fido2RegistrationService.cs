using Fido2Apis.Application.Apis.Fido2Auth;
using Fido2Apis.Application.Response;
using Fido2Apis.Infra.Db;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace Fido2Apis.Domain.Fido2Auth
{
    public class Fido2RegistrationService : IFido2RegistrationService
    {
        private readonly IFido2 _fido2;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<Fido2RegistrationService> _logger;

        public Fido2RegistrationService(IFido2 fido2, ApplicationDbContext applicationDbContext, IMemoryCache memoryCache, ILogger<Fido2RegistrationService> logger)
        {
            _fido2 = fido2;
            _applicationDbContext = applicationDbContext;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public ResponseObject CreateCredentialsOptions(Fido2RegistrationRequest fido2RegistrationRequest)
        {
            ResponseObject responseObject = new ResponseObject();
            try
            {
                _logger.LogInformation("Received data: " + fido2RegistrationRequest);

                Fido2User fido2User = new Fido2User
                {
                    Name = fido2RegistrationRequest.Username,
                    Id = Encoding.UTF8.GetBytes(fido2RegistrationRequest.Username),
                    DisplayName = fido2RegistrationRequest.Username
                };

                var existingFido2Credentials = _applicationDbContext.fido2_credentials.Where(x => x.UserName.Equals(fido2RegistrationRequest.Username)).Select(x => x.Descriptor).ToList();

                AuthenticatorSelection authenticatorSelection = new AuthenticatorSelection();
                //authenticatorSelection.AuthenticatorAttachment = AuthenticatorAttachment.CrossPlatform;
                authenticatorSelection.UserVerification = UserVerificationRequirement.Preferred;
                authenticatorSelection.RequireResidentKey = true;

                AttestationConveyancePreference attestationConveyancePreference = AttestationConveyancePreference.Direct;

                var credentialOptions = _fido2.RequestNewCredential(fido2User, existingFido2Credentials, authenticatorSelection, attestationConveyancePreference);

                if (credentialOptions != null )
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
                        SlidingExpiration = TimeSpan.FromSeconds(30),
                        Priority = CacheItemPriority.High,
                        Size = 1024
                    };

                    _memoryCache.Set("credentialOptions", credentialOptions.ToJson(), cacheOptions);

                    var cachedOptions = _memoryCache.Get("credentialOptions").ToString();
                    var credentialOptions2 = CredentialCreateOptions.FromJson(cachedOptions);

                    Console.WriteLine("type  of options from and converted: " + credentialOptions2.GetType());

                    responseObject.Success = true;
                    responseObject.Message = "Credential options created sucessfully";
                    responseObject.Data = credentialOptions;
                    responseObject.code = 200;

                    _logger.LogInformation("Credential options created sucessfully");

                    return responseObject;
                }
                else
                {
                    responseObject.Success = false;
                    responseObject.Message = "Failed to create credential options";
                    responseObject.code = 400;

                    _logger.LogInformation("Failed to create credential options");

                    return responseObject;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                responseObject.Success = false;
                responseObject.Message = "Internal Server Error: " + ex.Message;
                responseObject.code = 500;

                _logger.LogError("Internal Server Error: " + ex.Message);

                return responseObject;
            }
        }

        public async Task<ResponseObject> CreateCredentials(AuthenticatorAttestationRawResponse authenticatorAttestationRawResponse)
        {
            ResponseObject responseObject = new ResponseObject();
            try
            {
                var cachedOptions = _memoryCache.Get("credentialOptions").ToString();
                var credentialOptions = CredentialCreateOptions.FromJson(cachedOptions);

                if (credentialOptions != null)
                {
                    var createCredentials = await _fido2.MakeNewCredentialAsync(authenticatorAttestationRawResponse, credentialOptions, IsCredentialUnique);

                    if (createCredentials != null)
                    {
                        Fido2Credential fido2Credential = new Fido2Credential
                        {
                            AaGuid = createCredentials.Result.Aaguid,
                            CredType = createCredentials.Result.CredType,
                            Descriptor = new PublicKeyCredentialDescriptor(createCredentials.Result.CredentialId),
                            PublicKey = createCredentials.Result.PublicKey,
                            RegDate = DateTime.Now.ToUniversalTime(),
                            SignatureCounter = createCredentials.Result.Counter,
                            UserHandle = createCredentials.Result.User.Id,
                            UserId = credentialOptions.User.Id,
                            UserName = credentialOptions.User.Name
                        };

                        _applicationDbContext.fido2_credentials.Add(fido2Credential);

                        int result = _applicationDbContext.SaveChanges();

                        if (result > 0)
                        {
                            responseObject.Success = true;
                            responseObject.Message = "Registration Successful";
                            responseObject.code = 200;

                            _logger.LogInformation("Registration Successful");

                            return responseObject;
                        }
                        else
                        {
                            responseObject.Success = false;
                            responseObject.Message = "Registration Failed";
                            responseObject.code = 400;

                            _logger.LogInformation("Registration Failed, due to failure while saving entry in database table");

                            return responseObject;
                        }
                    }
                    else
                    {
                        responseObject.Success = false;
                        responseObject.Message = "Registration failed, due to failure in creating credentials";
                        responseObject.code = 400;

                        _logger.LogInformation("Registration failed, due to failure in creating credentials");

                        return responseObject;
                    }
                }
                else
                {
                    responseObject.Success = false;
                    responseObject.Message = "Registration failed, due to credential options not found in cache";
                    responseObject.code = 400;

                    _logger.LogInformation("Registration failed, due to credential options not found in cache");

                    return responseObject;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                responseObject.Success = false;
                responseObject.Message = "Internal Server Error: " + ex.Message;
                responseObject.code = 500;

                _logger.LogError("Internal Server Error: " + ex.Message);

                return responseObject;
            }

            throw new NotImplementedException();
        }

        private async Task<bool> IsCredentialUnique(IsCredentialIdUniqueToUserParams credentialIdUserParams, CancellationToken cancellationToken)
        {
            var credentialIdString = Base64Url.Encode(credentialIdUserParams.CredentialId);

            var credential = _applicationDbContext.fido2_credentials.Where(c => c.DescriptorJson.Contains(credentialIdString)).FirstOrDefault();

            if (credential == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
