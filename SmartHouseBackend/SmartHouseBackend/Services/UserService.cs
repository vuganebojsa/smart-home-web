using AutoMapper;
using FluentResults;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;
using SmartHouse.Core.Interfaces.Services;
using SmartHouse.Core.Messages;
using SmartHouse.Core.Model;
using SmartHouse.DTOS;
using SmartHouse.Extensions;
using SmartHouse.Infrastructure.DTOS;
using SmartHouse.Infrastructure.Interfaces.Repositories;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartHouse.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepositoryBase<User> _userRepository;
        private readonly ISmartPropertyRepository _smartPropertyRepository;
        private readonly IMapper _mapper;

        public UserService(IConfiguration configuration, IRepositoryBase<User> userRepository, ISmartPropertyRepository smartPropertyRepository, IMapper mapper)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _smartPropertyRepository = smartPropertyRepository;
            _mapper = mapper;

        }
        public string GenerateJwtToken(string email, Guid id, Role role)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("bivuja")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("email", email),
                new Claim("id", id.ToString()),
                new Claim("role", role.ToString()),
                new Claim(ClaimTypes.Role, role.ToString()),
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(360),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<Result<TokenDTO>> Login(string email, string password)
        {
            var user = await _userRepository.FindSingleByCondition(user => user.Email == email || user.UserName == email);

            if (user == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "email or password")); }

            var hashedPassword = HashPassword(password, Convert.FromBase64String(user.Salt));

            var hashed64 = Convert.ToBase64String(hashedPassword);

            if (hashed64 != user.Password) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "email or password")); }
            if (!user.IsVerified) { return ResultExtensions.FailBadRequest(ErrorMessages.EmailNotConfirmed()); }

            if (user.IsPasswordChanged == false) return Result.Ok(new TokenDTO { IsChangedPassword = false });

            var token = GenerateJwtToken(user.Email, user.Id, user.Role);

            return Result.Ok(new TokenDTO { Token = token });
        }

        public async Task<Result<UserDTO>> Register(RegisterUserDTO newUser)
        {
            if (newUser.Password != newUser.RepeatPassword) { return ResultExtensions.FailBadRequest(ErrorMessages.PasswordsDoNotMatch()); }

            var verifiedUser = await _userRepository.FindSingleByCondition(user => user.IsVerified == false && user.Email == newUser.Email && user.UserName == newUser.UserName);
            if (verifiedUser != null) { return ResultExtensions.FailBadRequest(ErrorMessages.ExitingAccountNotVerified()); }

            var existingUser = await _userRepository.FindSingleByCondition(user => user.Email == newUser.Email || user.UserName == newUser.UserName);
            if (existingUser != null) { return ResultExtensions.FailBadRequest(ErrorMessages.ExistingEmailOrUsername()); }

            byte[] salt = GenerateSalt();
            byte[] hashedPassword = HashPassword(newUser.Password, salt);



            var user = new User
            {
                Email = newUser.Email,
                UserName = newUser.UserName,
                Name = newUser.Name,
                LastName = newUser.LastName,
                IsVerified = false,
                Password = Convert.ToBase64String(hashedPassword),
                Salt = Convert.ToBase64String(salt),
                Role = Role.USER
            };

            _userRepository.Create(user);
            var imageFileName = "images/users/" + user.Id + "." + newUser.ImageType;

            user.ProfilePicturePath = imageFileName;
            await _userRepository.SaveChanges();

            byte[] imageBytes = Convert.FromBase64String(newUser.Image);


            await File.WriteAllBytesAsync(imageFileName, imageBytes);

            var apiKey = Environment.GetEnvironmentVariable("sendgridapikey");
            var client = new SendGridClient(apiKey);
            await SendEmail(client, user);

            UserDTO returnUser = new() { Id = user.Id, UserName = user.UserName };


            return Result.Ok(returnUser);
        }

        public async Task<Result<UserDTO>> RegisterAdmin(RegisterUserDTO newAdmin)
        {
            if (newAdmin.Password != newAdmin.RepeatPassword) { return ResultExtensions.FailBadRequest(ErrorMessages.PasswordsDoNotMatch()); }

            var existingUser = await _userRepository.FindSingleByCondition(user => user.Email == newAdmin.Email || user.UserName == newAdmin.UserName);
            if (existingUser != null) { return ResultExtensions.FailBadRequest(ErrorMessages.ExistingEmailOrUsername()); }


            byte[] salt = GenerateSalt();
            byte[] hashedPassword = HashPassword(newAdmin.Password, salt);



            var apiKey = Environment.GetEnvironmentVariable("sendgridapikey");
            var client = new SendGridClient(apiKey);



            var user = new User
            {
                Email = newAdmin.Email,
                UserName = newAdmin.UserName,
                Name = newAdmin.Name,
                LastName = newAdmin.LastName,
                IsVerified = true,
                Password = Convert.ToBase64String(hashedPassword),
                Salt = Convert.ToBase64String(salt),
                Role = Role.ADMIN,
            };

            _userRepository.Create(user);


            var imageFileName = "images/users/" + user.Id + "_profile." + newAdmin.ImageType;
            user.ProfilePicturePath = imageFileName;

            await _userRepository.SaveChanges();
            byte[] imageBytes = Convert.FromBase64String(newAdmin.Image);

            await File.WriteAllBytesAsync(imageFileName, imageBytes);

            UserDTO returnUser = new() { Id = user.Id, UserName = user.UserName };


            return Result.Ok(returnUser);
        }


        private static byte[] HashPassword(string password, byte[] salt)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(32); // 32 bytes is the size of the SHA-256 hash
            }
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[32];
            using (RNGCryptoServiceProvider rng = new())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static async Task SendEmail(SendGridClient client, User user)
        {
            string verifyURL = "https://localhost:7217/api/v1/users/activate?userEmail=" + user.Email;
            var from = new EmailAddress("vuga.sv53.2020@uns.ac.rs", "Nebojsa Vuga");
            var to = new EmailAddress(user.Email);
            var subject = "Profile activation link for smarthome";
            var plainContent = "Dear " + user.UserName + ", to complete the registration process and activate your profile, please click on the following link:: " + verifyURL;
            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString();
            var content = "<html><head><style>body { font-family: 'Arial', sans-serif; }.blue{ color: #073e66;}p{color: white;} " +
                ".timestamp{float:right; color:#073e66; margin-top:1px;} " +
                ".border {width: 40%; border: 2px solid #073e66; border-radius: 8px; text-align: center; padding: 20px; } " +
                ".novalite-button { color: white; background-color: #073e66; border: 2px solid white; margin-top: 7px; padding: 16px 26px; border-radius: 8px; font-weight: 500; font-size: 18px; font-weight: bold; text-transform: capitalize; transition: .3s ease-in-out; }" +
                                   " img { max-width: 10%; height: auto; display: block; margin: 0 auto; } </style></head><body><div class='border'><img src='https://cdn-icons-png.flaticon.com/512/3354/3354557.png' alt='Image' /><h1 class = 'blue'>Smarthome</h1><div class='novalite-button'><p>Dear " + user.UserName + ",<br> To complete the registration process and activate your profile, please click on the following link: <br>" + verifyURL + "</p> </div><div class='timestamp'>" + formattedDate + "</div></div></body></html>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainContent, content);
            await client.SendEmailAsync(msg);
        }

        public async Task<Result<string>> ActivateUser(string userEmail)
        {
            var user = await _userRepository.FindSingleByCondition(user => user.Email == userEmail);
            if (user == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "email")); }

            user.IsVerified = true;
            await _userRepository.SaveChanges();


            return Result.Ok("Succesfully verified your account.");

        }

        public async Task<Result<PagedList<UserPropertyDTO>>> GetUserProperties(Guid id, Page page)
        {
            var properties = await _smartPropertyRepository.FindByCondition(property => property.User.Id == id, page);

            var propertiesDTO = _mapper.Map<PagedList<UserPropertyDTO>>(properties);


            return Result.Ok(propertiesDTO);

        }

        public async Task<Result<PagedList<PropertyDTO>>> GetPendingProperties(Page page)
        {
            var pendingProperties = await _smartPropertyRepository.FindPendingWithUser(page);
            var pendingPropertiesDTO = _mapper.Map<PagedList<PropertyDTO>>(pendingProperties);

            return Result.Ok(pendingPropertiesDTO);
        }

        public async Task<Result<string>> ProccesPendingPropertyRequest(Guid id, ProcessPropertyRequestDTO request)
        {
            var apiKey = Environment.GetEnvironmentVariable("sendgridapikey");
            var client = new SendGridClient(apiKey);

            var pendingProperty = await _smartPropertyRepository.FindByConditionWithUserSingle(sp => sp.Id == id);

            if (pendingProperty == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id")); }
            if (pendingProperty.IsAccepted != Activation.Pending) { return ResultExtensions.FailBadRequest(ErrorMessages.AlreadyProcessedProperty()); }
            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString();
            if (request.Accept)
            {
                pendingProperty.IsAccepted = Activation.Accepted;
                var from = new EmailAddress("vuga.sv53.2020@uns.ac.rs", "Nebojsa Vuga");
                var to = new EmailAddress(pendingProperty.User.Email);
                var subject = "Acceptance of property";
                var plainContent = "Your property named: " + pendingProperty.Name + " has been accepted.";
                var content = "<html><head><style>body { font-family: 'Arial', sans-serif; }.blue{ color: #073e66;}p{color: white;} .timestamp{float:right; color:#073e66; margin-top:1px;} .border {width: 40%; border: 2px solid #073e66; border-radius: 8px; text-align: center; padding: 20px; } .novalite-button { color: #white; background-color: #073e66; border: 2px solid white; margin-top: 7px; padding: 16px 26px; border-radius: 8px; font-weight: 500; font-size: 18px; font-weight: bold; text-transform: capitalize; transition: .3s ease-in-out; }" +
                                   " img { max-width: 10%; height: auto; display: block; margin: 0 auto; } </style></head><body><div class='border'><img src='https://cdn-icons-png.flaticon.com/512/3354/3354557.png' alt='Image' /><h1 class = 'blue'>Smarthome</h1><div class='novalite-button'><p>Your property named: " + pendingProperty.Name + " has been accepted.</p> </div><div class='timestamp'>" + formattedDate + "</div></div></body></html>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainContent, content);

                await client.SendEmailAsync(msg);
                await _smartPropertyRepository.SaveChanges();
                return Result.Ok("Succesfully accepted property.");
            }
            else
            {
                pendingProperty.IsAccepted = Activation.Rejected;
                pendingProperty.Reason = request.Reason;
                var from = new EmailAddress("vuga.sv53.2020@uns.ac.rs", "Nebojsa Vuga");
                var to = new EmailAddress(pendingProperty.User.Email);
                var subject = "Rejection of property";

                var plainContent = "Your property named: " + pendingProperty.Name + " has been rejected.\n Reason: " + request.Reason;
                var content = "<html><head><style>body { font-family: 'Arial', sans-serif; }.blue{ color: #073e66;}.timestamp{float:right; color:#073e66; margin-top:1px;} .border {width: 40%; border: 2px solid #073e66; border-radius: 8px; text-align: center; padding: 20px; } .novalite-button { color: #073e66; background-color: #FF6347; border: 2px solid white; margin-top: 7px; padding: 16px 26px; border-radius: 8px; font-weight: 500; font-size: 18px; font-weight: bold; text-transform: capitalize; transition: .3s ease-in-out; }" +
                    " img { max-width: 10%; height: auto; display: block; margin: 0 auto; } </style></head><body><div class='border'><img src='https://cdn-icons-png.flaticon.com/512/3354/3354557.png' alt='Image' /><h1 class = 'blue'>Smarthome</h1><div class='novalite-button'>Your property named: " + pendingProperty.Name + " has been rejected. <hr> Reason: " + request.Reason + " </div><div class='timestamp'>" + formattedDate + "</div></div></body></html>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainContent, content);
                await client.SendEmailAsync(msg);
                await _smartPropertyRepository.SaveChanges();
                return Result.Ok("Succesfully rejected property.");
            }
        }


        static string ImageToBase64(string imagePath)
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
        public async Task<Result<UserProfileDTO>> GetUserProfile(Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }
            string imageBase64 = "";
            var profilePicturePath = user.ProfilePicturePath;
            try
            {
                imageBase64 = ImageToBase64(profilePicturePath);
            }
            catch (Exception)
            {
            }
            UserProfileDTO profile = new()
            {
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.UserName,
                ProfilePicturePath = profilePicturePath,
                Role = user.Role,
                ProfilePicture = imageBase64
            };
            return Result.Ok(profile);
        }

        public async Task<Result<EditUserDTO>> EditProfile(EditUserDTO newUserInfo, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }
            var existingUser = await _userRepository.FindSingleByCondition(usr => usr.Id != userId && usr.UserName == newUserInfo.Username);
            if (existingUser != null)
            {
                return ResultExtensions.FailBadRequest(ErrorMessages.ExistingEmailOrUsername());
            }
            existingUser = await _userRepository.FindSingleByCondition(usr => usr.Id != userId && usr.Email == newUserInfo.Email);
            if (existingUser != null)
            {
                return ResultExtensions.FailBadRequest(ErrorMessages.ExistingEmailOrUsername());
            }
            user.Name = newUserInfo.Name;
            user.LastName = newUserInfo.LastName;
            user.UserName = newUserInfo.Username;
            user.Email = newUserInfo.Email;
            try
            {
                File.Delete(user.ProfilePicturePath);
            }
            catch (Exception)
            {

            }

            var imageFileName = "images/users/" + user.Id + "." + newUserInfo.TypeOfImage;
            user.ProfilePicturePath = imageFileName;
            await _userRepository.SaveChanges();

            byte[] imageBytes = Convert.FromBase64String(newUserInfo.ProfilePicture);

            await File.WriteAllBytesAsync(imageFileName, imageBytes);
            var newUser = new EditUserDTO()
            {
                Username = user.UserName,
                Name = user.Name,
                LastName = user.LastName,
                ProfilePicture = newUserInfo.ProfilePicture,
                TypeOfImage = newUserInfo.TypeOfImage,
                Email = newUserInfo.Email,
            };
            var apiKey = Environment.GetEnvironmentVariable("sendgridapikey");
            var client = new SendGridClient(apiKey);
            await SendUpdateProfileEmail(client, user);
            return Result.Ok(newUser);


        }

        private async Task SendUpdateProfileEmail(SendGridClient client, User user)
        {
            var from = new EmailAddress("vuga.sv53.2020@uns.ac.rs", "Nebojsa Vuga");
            var to = new EmailAddress(user.Email);
            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString();
            var subject = "Your Profile has been edited!";
            var plainContent = $"Your username: {user.UserName}\nYour email: {user.Email}\nYour name: {user.Name} {user.LastName}";
            var content = "<html><head><style>body { font-family: 'Arial', sans-serif; }.blue{ color: #073e66;}p{color: white;} .timestamp{float:right; color:#073e66; margin-top:1px;}" +
                " .border {width: 40%; border: 2px solid #073e66; border-radius: 8px; text-align: center; padding: 20px; } " +
                ".novalite-button {color: white; background-color: #073e66; border: 2px solid white; margin-top: 7px; padding: 16px 26px; border-radius: 8px; font-weight: 500; font-size: 18px; font-weight: bold; text-transform: capitalize; transition: .3s ease-in-out; }" +
                                   " img { max-width: 10%; height: auto; display: block; margin: 0 auto; } " +
                                   "</style></head><body><div class='border'><img src='https://cdn-icons-png.flaticon.com/512/3354/3354557.png' alt='Image' /><h1 class = 'blue'>Smarthome</h1><div class='novalite-button'> <p> Your profile has been edited! <hr> <i> Username: </i> " + user.UserName + "  <br> <i>Email: </i>" + user.Email + " <br> <i> Name: </i> " + user.Name + " " + user.LastName + " </p> </div><div class='timestamp'>" + formattedDate + "</div></div></body></html>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainContent, content);
            await client.SendEmailAsync(msg);
        }

        public async Task<Result<bool>> EditPassword(EditPasswordDTO passwordInfo, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }

            var oldPassword = passwordInfo.OldPassword;

            if (oldPassword == passwordInfo.NewPassword)
            {
                return ResultExtensions.FailBadRequest(ErrorMessages.OldAndNewPasswordAreTheSame());
            }

            if (passwordInfo.NewPassword != passwordInfo.RepeatNewPassword)
            {
                return ResultExtensions.FailBadRequest(ErrorMessages.PasswordsDoNotMatch());
            }

            byte[] salt = Convert.FromBase64String(user.Salt);
            var hashedPw = user.Password;
            byte[] hashedNewPassword = HashPassword(passwordInfo.NewPassword, salt);
            if (StructuralComparisons.StructuralEqualityComparer.Equals(Convert.ToBase64String(hashedNewPassword), hashedPw))
            {
                return ResultExtensions.FailBadRequest(ErrorMessages.OldAndNewPasswordAreTheSame());
            }
            hashedNewPassword = HashPassword(oldPassword, salt);
            if (!StructuralComparisons.StructuralEqualityComparer.Equals(Convert.ToBase64String(hashedNewPassword), hashedPw))
            {
                return ResultExtensions.FailBadRequest(ErrorMessages.OldPasswordIsIncorrect());
            }
            salt = GenerateSalt();
            hashedNewPassword = HashPassword(passwordInfo.NewPassword, salt);
            user.Salt = Convert.ToBase64String(salt);
            user.Password = Convert.ToBase64String(hashedNewPassword);
            await _userRepository.SaveChanges();

            return true;
        }


        public async Task<Result<TokenDTO>> EditFirstPassword(EditPasswordDTO passwordInfo, string email)
        {
            var user = await _userRepository.FindSingleByCondition(user => user.Email == email || user.UserName == email);
            if (user == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "email or username"));
            }

            var oldPassword = passwordInfo.OldPassword;

            if (oldPassword == passwordInfo.NewPassword)
            {
                return ResultExtensions.FailBadRequest(ErrorMessages.OldAndNewPasswordAreTheSame());
            }

            if (passwordInfo.NewPassword != passwordInfo.RepeatNewPassword)
            {
                return ResultExtensions.FailBadRequest(ErrorMessages.PasswordsDoNotMatch());
            }

            byte[] salt = Convert.FromBase64String(user.Salt);
            var hashedPw = user.Password;
            byte[] hashedNewPassword = HashPassword(passwordInfo.NewPassword, salt);
            if (StructuralComparisons.StructuralEqualityComparer.Equals(Convert.ToBase64String(hashedNewPassword), hashedPw))
            {
                return ResultExtensions.FailBadRequest(ErrorMessages.OldAndNewPasswordAreTheSame());
            }
            hashedNewPassword = HashPassword(oldPassword, salt);
            if (!StructuralComparisons.StructuralEqualityComparer.Equals(Convert.ToBase64String(hashedNewPassword), hashedPw))
            {
                return ResultExtensions.FailBadRequest(ErrorMessages.OldPasswordIsIncorrect());
            }
            salt = GenerateSalt();
            hashedNewPassword = HashPassword(passwordInfo.NewPassword, salt);
            user.Salt = Convert.ToBase64String(salt);
            user.Password = Convert.ToBase64String(hashedNewPassword);
            user.IsPasswordChanged = true;
            await _userRepository.SaveChanges();


            var token = GenerateJwtToken(user.Email, user.Id, user.Role);
            return Result.Ok(new TokenDTO { Token = token });
        }
    }
}
