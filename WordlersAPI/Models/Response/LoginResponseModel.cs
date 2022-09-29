using WordlersAPI.Models.Core;

namespace WordlersAPI.Models.Response
{
    public class LoginResponseModel
    {
        public User User { get; set; }
        public string Token { get; set; }
    }
}
