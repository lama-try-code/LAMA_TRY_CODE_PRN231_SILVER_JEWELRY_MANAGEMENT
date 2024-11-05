using LamDT_SE172525.BOs;
using LamDT_SE172525.DAO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LamDT_SE172525.Repository.Account
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IConfiguration _configuration;
        public AccountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<String> GetBranchAccount(string email, string password)
        {
            var account = await AccountsDAO.Instance.GetBranchAccount(email, password);
            if (account == null)
            {
                return "Login failed";
            }

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Role, account.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(300),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
