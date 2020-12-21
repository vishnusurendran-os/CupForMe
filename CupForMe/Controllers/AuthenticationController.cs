using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CupForMe.Models;
using CupForMe.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CupForMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private UserManager<UserIdentity> _userManager;
         private readonly ApplicationSettings _appSettings;
        RoleManager<ApplicationRole> _roleManager;
        SignInManager<UserIdentity> _signInManager;

        public AuthenticationController(IOptions<ApplicationSettings> appSettings, UserManager<UserIdentity> userManager,
            RoleManager<ApplicationRole> roleManager, SignInManager<UserIdentity> signInManager, ILogger<AuthenticationController> logger)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: true);
            UserIdentity user = await _userManager.FindByNameAsync(model.UserName);

            if (result.Succeeded)
            {
                if (!_userManager.IsLockedOutAsync(user).Result && user.IsActive)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64)

                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");

                    claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                    var tokenDescriptor = new SecurityTokenDescriptor();
                    tokenDescriptor.Subject = claimsIdentity;
                    tokenDescriptor.Expires = DateTime.UtcNow.AddHours(_appSettings.LoginClaimExpirationMinutes);
                    tokenDescriptor.SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSecret)), SecurityAlgorithms.HmacSha256Signature);


                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);

                    UserModel userModel = new UserModel
                    {
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Id = user.Id.ToString(),
                        IsActive = user.IsActive,
                        IsLocked = _userManager.IsLockedOutAsync(user).Result,
                        Token = token
                    };

                    userModel.Roles = roles;

                    return Ok(new { userModel });
                }
                else
                {
                    return BadRequest(new { message = "There was a problem signing you in. Please ensure that your user name and password are correct, and that your account is not locked." });
                }
            }
            else if (result.IsLockedOut)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddMinutes(10));
                return BadRequest(new { message = "The user account is locked." });
            }
            else
                return BadRequest(new { message = "Username or password is incorrect." });
        }
    }
}
