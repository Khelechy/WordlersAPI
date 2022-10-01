using WordlersAPI.Models.Core;
using WordlersAPI.Models.Dto;

namespace WordlersAPI.Models.Response
{
    public class LoginResponseModel
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
    }
}
