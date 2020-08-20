using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CrossChat.Domain.Common;
using CrossChat.Domain.DBModel;

namespace CrossChat.Services.JwtService
{
    public class JwtService : IJwtService
    {
 
        public string Generate(User user)
        {
            var secretKey = Encoding.UTF8.GetBytes(JwtSettings.SecretKey); // longer that 16 character
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptionkey = Encoding.UTF8.GetBytes(JwtSettings.Encryptkey); //must be 16 character
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var claims = _getClaims(user);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = JwtSettings.Issuer,
                Audience = JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddMinutes(JwtSettings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(descriptor);

            var encryptedJwt = tokenHandler.WriteToken(securityToken);

            return encryptedJwt;

        }

        private List<Claim> _getClaims(User user)
        {
            var list = new List<Claim>
            {
                //new Claim(ClaimTypes.Name, user.Username),
               // new Claim(ClaimTypes.Email,user.Email),
                new Claim("UserId",user.Id.ToString())// user.ID.ToString())
                //new Claim(ClaimTypes.GivenName, user.Name)
            };

            return list;
        }

    }
}
