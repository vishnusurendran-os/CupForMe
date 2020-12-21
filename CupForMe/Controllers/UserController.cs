using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CupForMe.Models;
using CupForMe.StaticClass;
using CupForMe.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CupForMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserManager<UserIdentity> _userManager;
        RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<UserIdentity> userManager, RoleManager<ApplicationRole> roleManager, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdmUserRoleName)]
        [Route("Register")] // api/user/register
        public async Task<Object> RegisterUser(UserModel user)
        {
            UserIdentity userIdentity = new UserIdentity()
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive
            };

            List<string> roles = new List<string>();
            roles.Add(user.PrimaryRole);

            if (user.DataAccessRole != null)
            {
                roles.Add(user.DataAccessRole);
            }

            try
            {
                IdentityResult result = await _userManager.CreateAsync(userIdentity, user.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(userIdentity, roles);
                }
                else
                    return BadRequest(result.Errors);

                user.Roles = roles;

                return Ok(user);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, exc.Message, user);
                throw exc; // TODO: need to add logging of some sort
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdmUserRoleName)]
        [Route("List")]
        public async Task<Object> ListUsers(SearchStateModel searchStateModels)
        {
            try
            {
                // the plumbing is here to send in grid state, in the event we want to do it server side. However, the only way that will really
                // work to help with performance is to find a way to orderby async (orderby not supported in tolistasync but maybe a custom comparer) or to move to a proc
                // also, Kendo grid seems to not recognize a separate total (they'll let you set it but it gets ignored) so the only way to get the right total for paging
                // seems to be to send all records - would like to research more but not time right now!

                List<UserIdentity> users = await _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToListAsync();
                List<UserModel> exportList;
                List<UserModel> returnUsersForGrid = new List<UserModel>();
                List<UserModel> takeList;
                foreach (UserIdentity user in users)
                {

                    var roles = await _userManager.GetRolesAsync(user);

                    UserModel userModel = new UserModel
                    {
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Id = user.Id.ToString(),
                        IsActive = user.IsActive,
                        IsLocked = _userManager.IsLockedOutAsync(user).Result,

                    };

                    userModel.Roles = roles;

                    returnUsersForGrid.Add(userModel);
                }

                if (searchStateModels.Sort != null && searchStateModels.SortField != null)
                {
                    exportList = StaticMethods.OrderByDynamic<UserModel>(returnUsersForGrid.AsQueryable(), searchStateModels).ToList();
                    takeList = exportList.Skip(searchStateModels.Skip).Take(searchStateModels.Take).ToList();
                }
                else
                {
                    exportList = returnUsersForGrid.Skip(searchStateModels.Skip).ToList();
                    takeList = exportList.Take(searchStateModels.Take).ToList();
                }
                return this.Ok(new { total = returnUsersForGrid.Count, data = takeList, exportData = exportList });
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, exc.Message, searchStateModels);
                throw exc; // TODO: need to add logging of some sort
            }
        }
    }
}
