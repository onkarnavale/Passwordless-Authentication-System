using AutoMapper;
using Fido2Apis.Application.Apis.User;
using Fido2Apis.Application.Response;
using Fido2Apis.Infra.Db;
using Microsoft.AspNetCore.Identity;

namespace Fido2Apis.Domain.User
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly ILogger<UserRegistrationService> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;

        public UserRegistrationService(ILogger<UserRegistrationService> logger, ApplicationDbContext applicationDbContext, IPasswordHasher<User> passwordHasher, IMapper mapper)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public ResponseObject Register(UserRegistrationRequest userRegistrationRequest)
        {
            ResponseObject responseObject = new ResponseObject();
            try
            {
                _logger.LogInformation("Received data: " + userRegistrationRequest);

                User? existingUser = _applicationDbContext.users.Where(e => e.Username.Equals(userRegistrationRequest.Username)).FirstOrDefault();

                if (existingUser != null)
                {
                    responseObject.Success = false;
                    responseObject.Message = "User already registered";
                    responseObject.Data = 409;

                    _logger.LogInformation("User already registered");

                    return responseObject;
                }

                User user = new User();
                user.Username = userRegistrationRequest.Username;
                user.Password = _passwordHasher.HashPassword(user, userRegistrationRequest.Password);

                _applicationDbContext.users.Add(user);

                int result = _applicationDbContext.SaveChanges();

                if (result > 0)
                {
                    responseObject.Success = true;
                    responseObject.Message = "Registration successful";
                    responseObject.Data = _mapper.Map<UserDto>(user);
                    responseObject.code = 201;

                    _logger.LogInformation("Registration successful");

                    return responseObject;
                }
                else
                {
                    responseObject.Success = false;
                    responseObject.Message = "Registration failed";
                    responseObject.code = 400;

                    _logger.LogInformation("Registration failed, due to failure while storing entry in database table");

                    return responseObject;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                responseObject.Success = false;
                responseObject.Message = "Internal Server Error: " + ex.Message;
                responseObject.code = 500;

                _logger.LogInformation("Internal Server Error: " + ex.Message);

                return responseObject;
            }
        }
    }
}
