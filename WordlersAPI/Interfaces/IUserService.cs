using WordlersAPI.Models.Request;
using WordlersAPI.Models.Response;

namespace WordlersAPI.Interfaces
{
    public interface IUserService
    {
        Task<GeneralResponse> ChangePassword(ChangePasswordRequestModel requestModel);
    }
}
