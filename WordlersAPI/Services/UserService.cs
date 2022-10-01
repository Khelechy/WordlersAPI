using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Core;
using WordlersAPI.Models.Request;
using WordlersAPI.Models.Response;

namespace WordlersAPI.Services
{
    public class UserService : IUserService
    {
        private UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        public UserService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<GeneralResponse> ChangePassword(ChangePasswordRequestModel requestModel)
        {
			var user = await GetUser();
			if (user == null)
            {
				return new GeneralResponse
				{
					Message = "There is account does not exist",
					IsSuccess = false,
				};
			}
				
			var isPasswordCorrect = await userManager.CheckPasswordAsync(user, requestModel.OldPassword);

			if (!isPasswordCorrect)
            {
				return new GeneralResponse
				{
					Message = "Incorrect Password",
					IsSuccess = false,
				};
			}else if (requestModel.NewPassword != requestModel.ConfirmPassword)
            {
				return new GeneralResponse
				{
					Message = "New Passwords do not match",
					IsSuccess = false,
				};
			}
				

			var result = await userManager.ChangePasswordAsync(user, requestModel.OldPassword, requestModel.NewPassword);

			if (result.Succeeded)
			{
				user.LastPasswordChangedAt = DateTimeOffset.Now;
				await userManager.UpdateAsync(user);
				return new GeneralResponse
				{
					Message = "Password changed successfully",
					IsSuccess = true,
				};
			}
            else
            {
				return new GeneralResponse
				{
					Message = result.Errors.Select(e => e.Description).ToString(),
					IsSuccess = false,
				};
			}
				
		}


        public async Task<User> GetUser()
        {
            var userID = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID != null)
                return await userManager.FindByIdAsync(userID);
            return null;
        }
    }
}
