using AutoMapper;
using Fido2Apis.Application.Apis.User;
using Fido2Apis.Application.Response;
using Fido2Apis.Infra.Db;
using Fido2Apis.Infra.Jwt;
using Microsoft.AspNetCore.Identity;

namespace Fido2Apis.Domain.User
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly ILogger<UserAuthenticationService> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public UserAuthenticationService(ILogger<UserAuthenticationService> logger, ApplicationDbContext applicationDbContext, IPasswordHasher<User> passwordHasher, IMapper mapper, JwtTokenGenerator jwtTokenGenerator)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public ResponseObject Authenticate(UserAuthenticationRequest userAuthenticationRequest)
        {
            ResponseObject responseObject = new ResponseObject();
            try
            {
                _logger.LogInformation("Received data: " + userAuthenticationRequest);

                User? user = _applicationDbContext.users.Where(e => e.Username.Equals(userAuthenticationRequest.Username)).FirstOrDefault();

                if (user != null)
                {
                    var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, userAuthenticationRequest.Password);

                    if (passwordVerificationResult == PasswordVerificationResult.Success)
                    {
                        JwtPayLoadData jwtPayLoadData = new JwtPayLoadData()
                        {
                            Name = user.Username
                        };

                        var tokenString = _jwtTokenGenerator.GenerateJwtToken(jwtPayLoadData);

                        responseObject.Success = true;
                        responseObject.Message = "Authentication successful";
                        responseObject.Data = new { username =  user.Username, accessTokenPasswordAuth = tokenString };
                        responseObject.code = 200;

                        _logger.LogInformation("Authentication successful");

                        return responseObject;
                    }
                    else
                    {
                        responseObject.Success = false;
                        responseObject.Message = "Authentication failed, Password does not match";
                        responseObject.code = 400;

                        _logger.LogInformation("Authentication failed, Password does not match");

                        return responseObject;
                    }
                }
                else
                {
                    responseObject.Success = false;
                    responseObject.Message = "Authentication failed, user not registered";
                    responseObject.code = 400;

                    _logger.LogInformation("Authentication failed, user not registered");

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
    }
}
