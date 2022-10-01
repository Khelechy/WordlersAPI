using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WordlersAPI.Data;
using WordlersAPI.Enums;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Core;
using WordlersAPI.Models.Dto;
using WordlersAPI.Models.Helper;
using WordlersAPI.Models.Request;
using WordlersAPI.Models.Response;

namespace WordlersAPI.Services
{
    public class AuthenticationService : IAuthenticationService
	{
        private readonly UserManager<User> userManager;
        private readonly GameDbContext context;
        private readonly JwtSettings _configuration;
		private readonly IMapper _mapper;


		private readonly string _issuer;
		private readonly string _audience;
		private readonly string _securityKey;

		public AuthenticationService(ILogger<AuthenticationService> logger, GameDbContext context, UserManager<User> userManager, IOptions<JwtSettings> configuration,
			 IMapper mapper)
        {
			this.userManager = userManager;
			this.context = context;
			_configuration = configuration.Value;
			_issuer = _configuration.Issuer;	
			_audience = _configuration.Audience;
			_securityKey = _configuration.SecretKey;
			_mapper = mapper;

		}

		public async Task<string> GenerateToken(User user)
		{
			var userRole = await userManager.GetRolesAsync(user);
			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, $"{user.FirstName} {" "} {user.LastName}"),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
			};

			foreach (var role in userRole)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _issuer,
				audience: _audience,
				claims: claims,
				expires: DateTime.Now.AddHours(1),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

        public async Task<GeneralResponse<LoginResponseModel>> LoginAsync(LoginRequestModel requestModel)
        {
			var userByEmail = await userManager.FindByEmailAsync(requestModel.Email);
			var userByUserName = await context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == requestModel.Email.ToLower());
			User user = new User();

			if (userByEmail == null && userByUserName == null)
			{
				return new GeneralResponse<LoginResponseModel>
				{
					Message = "There is no account exisiting with this email or username",
					IsSuccess = false
				};
			}

			if(userByEmail != null)
            {
				user = userByEmail;
            }
            else
            {
				user = userByUserName;	
            }

			var result = await userManager.CheckPasswordAsync(user, requestModel.Password);


			if (!result)
			{
				return new GeneralResponse<LoginResponseModel>
				{
					Message = "Incorrect password",
					IsSuccess = false,
				};
			}

			user.LastLoggedInAt = DateTime.Now;
			await userManager.UpdateAsync(user);
			var token = await GenerateToken(user);

			var userDto = _mapper.Map<UserDto>(user);

			var loginResponse = new LoginResponseModel
			{
				User = userDto,	
				Token = token,	
			};

			return new GeneralResponse<LoginResponseModel>
			{
				Message = "Login Successful",
				IsSuccess = true,
				Data = loginResponse
			};
		}

        public async Task<GeneralResponse> RegisterAsync(RegisterRequestModel requestModel, bool isAdmin = false)
        {
			var existinguser = await userManager.FindByEmailAsync(requestModel.Email);
			if (existinguser != null)
            {
				return new GeneralResponse
				{
					Message = "An account with this email already exists",
					IsSuccess = false,
				};
			}
				

			if (requestModel.Password != requestModel.ConfirmPassword)
            {
				return new GeneralResponse
				{
					Message = "Passwords do not match",
					IsSuccess = false,
				};
			}
				


			var user = new User
			{
				Email = requestModel.Email,
				UserName = requestModel.UserName,
				LastName = requestModel.LastName,
				FirstName = requestModel.FirstName,
				CreatedAt = DateTimeOffset.Now,	
			};

			var result = await userManager.CreateAsync(user, requestModel.Password);

			if (result.Succeeded)
			{
				if (isAdmin)
				{
					await userManager.AddToRoleAsync(user, RolesEnum.Admin.ToString());
				}
				else
				{
					await userManager.AddToRoleAsync(user, RolesEnum.User.ToString());
				}

				return new GeneralResponse
				{
					IsSuccess = true,
					Message = "User created successfully",
				};
			}
			else
			{
				return new GeneralResponse
				{
					IsSuccess = false,
					Message = result.Errors.First().Description.ToString()
				};
			}
		}
    }
}
