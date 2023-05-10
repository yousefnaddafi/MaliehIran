using MaliehIran.Models;
using MaliehIran.Services.Common;
using MaliehIran.Services.UserTypeServices;
using MaliehIran.Settings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MaliehIran.Services.JWTServices
{
    public class GenerateJwtService : IGenerateJwtService
    {
        private readonly IJwtSetting _jwtSetting;
        private readonly IUserTypeService _userTypeService;

        public GenerateJwtService(IJwtSetting jwtSetting, IUserTypeService userTypeService)
        {
            _jwtSetting = jwtSetting;
            _userTypeService = userTypeService;
        }
        public async Task<IServiceResult<object>> GenerateAsync(User user)
        {
            var issuer = _jwtSetting.Issuer;
            var audienceId = _jwtSetting.Audience;
            var audienceSecret = Encoding.UTF8.GetBytes(_jwtSetting.SecretKey);
            var encryptionkey = Encoding.UTF8.GetBytes(_jwtSetting.Encryptkey); //must be 16 character
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var secretKey = audienceSecret; // longer that 16 character
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);
            var role = await _userTypeService.Get(user.Type);


            var claims = _getClaimsAsync(user, role);

            if (claims is null)
                return null;

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audienceId,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_jwtSetting.NotBeforeMinutes),
                Expires = DateTime.Now.AddMinutes(_jwtSetting.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(descriptor);

            return new ServiceResult<object>().Ok(new
            {
                Token = tokenHandler.WriteToken(securityToken),
                User = new
                {
                    user.UserId,
                    user.Name,
                    user.FamilyName,
                    user.Mobile,
                    Role = role.Name
                }
            });
        }
        private IEnumerable<Claim> _getClaimsAsync(User user, UserType role)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Mobile),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            };

            claims.Add(new Claim(ClaimTypes.Role, role.Name));
            return claims;
        }
    }
}
