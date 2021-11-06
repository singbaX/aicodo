using AiCodo.Data;
using AiCodo.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AiCodo.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IUserService _UserService;
        ITokenService _TokenService;

        public AccountController(IUserService userService, ITokenService tokenService)
        {
            _UserService = userService;
            _TokenService = tokenService;
        }

        [HttpGet]
        [Route("user")]
        public IActionResult GetUser()
        {
            return Ok(true);
        }

        #region cookies
        [HttpPost]
        [Route("login")]
        public IActionResult Login()
        {
            try
            {
                DynamicEntity data = Request.Body.ReadToEnd();
                var userName = data.GetString("UserName");
                var password = data.GetString("Password");
                var user = _UserService.Login(userName, password);
                if (user == null)
                {
                    return Unauthorized();
                }

                var userID = user.UserID;
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim("UserID", userID.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, userName));

                var principal = new ClaimsPrincipal(identity);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.Now.AddMinutes(30)
                    }).Wait();
                return Ok(user.ToJson());
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }

        [Route("loginout")]
        public IActionResult LoginOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return Ok();
        }
        #endregion

        #region token
        [HttpPost("token")]
        public IActionResult RequestToken()
        {
            DynamicEntity data = Request.Body.ReadToEnd();
            var userName = data.GetString("UserName");
            var password = data.GetString("Password");
            var user = _UserService.Login(userName, password);
            if (user == null)
            {
                return Unauthorized();
            }

            var token = _TokenService.CreateToken(user);
            return Ok(token.ToJson());
        }
        #endregion
    }
}
