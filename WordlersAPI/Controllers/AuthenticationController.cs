using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Request;
using WordlersAPI.Models.Response;

namespace WordlersAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse<LoginResponseModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GeneralResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequestModel)
        {
            var response = await authenticationService.LoginAsync(loginRequestModel);
            if (!response.IsSuccess)
            {
                return BadRequest(response);    
            }
            return Ok(response);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GeneralResponse))]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel registerRequestModel)
        {
            var response = await authenticationService.RegisterAsync(registerRequestModel);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
