using LamDT_SE172525.Repository.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace LamDT_SE172525.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto user)
        {
            var entity = await _accountRepository.GetBranchAccount(user.Username, user.Password);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        [HttpPost("get-role")]
        public IActionResult GetRole([FromBody] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Token is required.");
            }

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

                if (roleClaim != null)
                {
                    return Ok(new { Role = roleClaim.Value });
                }
                else
                {
                    return NotFound("Role claim not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid token: {ex.Message}");
            }
        }
    }

    public class LoginDto
    {
        public required String Username { get; set; }
        public required String Password { get; set; }
    }
}
