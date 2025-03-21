using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUser user;
        public AuthenticationController(IUser _user)
        {
            user = _user;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(AppUserDto appUserDto)
        {
            var result = await user.Register(appUserDto);

            return result.Falg ?Ok(result) :BadRequest(result);
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            var result = await user.Login(loginDTO);

            return result.Falg ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<GetUserDTO>> GetUser(int id)
        {
            var users = await user.GetUser(id);

            return users.Id > 0 ? Ok(users) : NotFound(users);
        }
    }
}
