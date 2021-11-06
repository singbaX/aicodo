using AiCodo.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AiCodo.Web.Services
{
    public class TokenService : ITokenService
    {
        private IOptions<JwtConfig> _options;
        public TokenService(IOptions<JwtConfig> options)
        {
            _options = options;
        }

        public Token CreateToken(IUser user)
        {
            Claim[] claims = {
                new Claim(ClaimTypes.NameIdentifier, user.UserID),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            return CreateToken(claims);
        }

        private Token CreateToken(Claim[] claims)
        {
            var now = DateTime.Now; 
            var expires = now.Add(TimeSpan.FromMinutes(_options.Value.ExpiresMinutes));
            var token = new JwtSecurityToken(
                issuer: _options.Value.Issuer,
                audience: _options.Value.Audience,
                claims: claims,
                notBefore: now.AddSeconds(-1),
                expires: expires,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecurityKey)), SecurityAlgorithms.HmacSha256));
            return new Token
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                TokenType = "Bearer",
                ExpiresAt = expires
            };
        }
    }

    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("profile")]
        public DynamicEntity Profile { get; set; }
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
