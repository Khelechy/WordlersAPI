using WordlersAPI.Models.Request;
using WordlersAPI.Models.Response;

namespace WordlersAPI.Interfaces
{
    public interface IAuthenticationService
    {
        Task<GeneralResponse<LoginResponseModel>> LoginAsync(LoginRequestModel requestModel);
        Task<GeneralResponse> RegisterAsync(RegisterRequestModel requestModel, bool isAdmin = false);
    }
}
