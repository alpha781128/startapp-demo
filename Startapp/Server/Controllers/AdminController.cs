using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Startapp.Server.Helpers;
using Startapp.Shared.Authorization;
using Startapp.Shared.Core;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Startapp.Server.Controllers
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEmailSender _emailSender;

        private const string GetUserByIdActionName = "GetUserById";

        public AdminController(IMapper mapper, IAccountManager accountManager, IAuthorizationService authorizationService, IEmailSender emailSender)
        {
            _mapper = mapper;
            _accountManager = accountManager;
            _authorizationService = authorizationService;
            _emailSender = emailSender;
        }

        [HttpGet("users")]
        [Authorize(Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(ApiResponse<PagedList<AppUser>>))]
        public async Task<IActionResult> GetUsers([FromQuery] PagingParameters pg)
        {
            var users = await _accountManager.GetUsersAsync(pg);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(users.MetaData));
            //var usersVM = _mapper.Map<IEnumerable<AppUser>, IEnumerable<UserViewModel>>(users);          

            if (users != null)
            {
                var json = new JsonResponse
                {
                    Json = users
                };
                return Ok(json);
            }
            return BadRequest("error");
        }

        [HttpGet("users/me")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<UserViewModel>))]
        public async Task<IActionResult> GetCurrentUser()
        {
            return await GetUserById(Utilities.GetUserId(this.User));
        }

        [HttpGet("users/{userId}", Name = GetUserByIdActionName)]
        [ProducesResponseType(200, Type = typeof(ApiResponse<UserEditViewModel>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(string userId)
        {
            if (!(await _authorizationService.AuthorizeAsync(this.User, userId, AccountManagementOperations.Read)).Succeeded)
                return new ChallengeResult();

            var user = await GetUserViewModelHelper(userId);
            if (user != null)
            {               
                var json = new JsonResponse
                {
                    Json = user
                };
                return Ok(json);
            }
            else
                return NotFound($"No user with that id: {userId}!");
        }

        [HttpPost("users")]
        //[Authorize(Authorization.Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(201, Type = typeof(UserEditViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> AddUser([FromBody] UserEditViewModel user)
        {
            if (!(await _authorizationService.AuthorizeAsync(this.User, (user.Roles, new string[] { }), Policies.AssignAllowedRolesPolicy)).Succeeded)
                return new ChallengeResult();

            if (ModelState.IsValid)
            {
                if (user == null)
                    return BadRequest($"{nameof(user)} cannot be null");

                AppUser appUser = _mapper.Map<AppUser>(user);

                var result = await _accountManager.CreateUserAsync(appUser, user.Roles, user.NewPassword);
                if (result.Succeeded)
                {
                    await SendVerificationEmail(appUser);

                    UserEditViewModel userVM = await GetUserViewModelHelper(appUser.Id);
                    return CreatedAtAction(GetUserByIdActionName, new { userId = userVM.Id }, userVM);
                }
                else
                    return BadRequest($"errer adding user !");
            }
            return BadRequest($"errer adding user !");
        }

        [HttpPut("users/me")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserEditViewModel user)
        {
            return await UpdateUser(Utilities.GetUserId(this.User), user);
        }

        [HttpPut("users/{userId}")]
        [ProducesResponseType(200, Type = typeof(UserEditViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserEditViewModel user)
        {
            AppUser appUser = await _accountManager.GetUserByIdAsync(userId);
            string[] currentRoles = appUser != null ? (await _accountManager.GetUserRolesAsync(appUser)).ToArray() : null;

            var manageUsersPolicy = _authorizationService.AuthorizeAsync(this.User, userId, AccountManagementOperations.Update);
            var assignRolePolicy = _authorizationService.AuthorizeAsync(this.User, (user.Roles, currentRoles), Policies.AssignAllowedRolesPolicy);

            if ((await Task.WhenAll(manageUsersPolicy, assignRolePolicy)).Any(r => !r.Succeeded))
                return new ChallengeResult();

            if (ModelState.IsValid)
            {
                if (user == null)
                    return BadRequest($"{nameof(user)} cannot be null");

                if (!string.IsNullOrWhiteSpace(user.Id) && userId != user.Id)
                    return BadRequest("Conflicting user id in parameter and model data");

                if (appUser == null)
                    return NotFound(userId);

                bool isPasswordChanged = !string.IsNullOrWhiteSpace(user.NewPassword);
                bool isUserNameChanged = !appUser.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase);
                bool isConfirmedEmailChanged = !appUser.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase) && appUser.EmailConfirmed;

                bool userHasPassword = await _accountManager.GetUserHasPasswordAsync(appUser);

                if (userHasPassword && Utilities.GetUserId(this.User) == userId)
                {
                    if (string.IsNullOrWhiteSpace(user.CurrentPassword))
                    {
                        if (isPasswordChanged)
                            AddError("Current password is required when changing your own password", "Password");

                        if (isUserNameChanged)
                            AddError("Current password is required when changing your own username", "Username");

                        if (isConfirmedEmailChanged)
                            AddError("Current password is required when changing your own email address", "Email");
                    }
                    else if (isPasswordChanged || isUserNameChanged || isConfirmedEmailChanged)
                    {
                        if (!await _accountManager.CheckPasswordAsync(appUser, user.CurrentPassword))
                            AddError("The username/password couple is invalid.");
                    }
                }

                if (ModelState.IsValid)
                {
                    _mapper.Map<UserEditViewModel, AppUser>(user, appUser);
                    appUser.EmailConfirmed = isConfirmedEmailChanged ? false : appUser.EmailConfirmed;

                    var result = await _accountManager.UpdateUserAsync(appUser, user.Roles);
                    if (result.Succeeded)
                    {
                        if (isConfirmedEmailChanged)
                            await SendVerificationEmail(appUser);

                        if (isPasswordChanged)
                        {
                            if (userHasPassword && !string.IsNullOrWhiteSpace(user.CurrentPassword))
                                result = await _accountManager.UpdatePasswordAsync(appUser, user.CurrentPassword, user.NewPassword);
                            else
                                result = await _accountManager.ResetPasswordAsync(appUser, user.NewPassword);
                        }

                        if (result.Succeeded)
                        {
                            var json = new JsonResponse
                            {
                                Json = user,
                                Message = appUser.FullName
                            };
                            return Ok(json);
                        }
                        else
                            return NotFound($"errer");
                    }

                    AddError(result.Errors);
                }
            }

            return BadRequest("error updating user!");
        }

        [HttpDelete("users/{userId}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<UserViewModel>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (!(await _authorizationService.AuthorizeAsync(this.User, userId, AccountManagementOperations.Delete)).Succeeded)
                return new ChallengeResult();

            AppUser appUser = await _accountManager.GetUserByIdAsync(userId);

            if (appUser == null)
                return NotFound(userId);

            if (!await _accountManager.TestCanDeleteUserAsync(userId))
                return BadRequest("User cannot be deleted. Delete all orders associated with this user and try again");


            UserEditViewModel userVM = await GetUserViewModelHelper(appUser.Id);

            var result = await _accountManager.DeleteUserAsync(appUser);
            if (!result.Succeeded)
                throw new Exception("The following errors occurred whilst deleting user: " + string.Join(", ", result.Errors));

            var json = new JsonResponse
            {
                Json = userVM,                      
                Message = string.IsNullOrEmpty(appUser.FullName)? appUser.UserName : appUser.FullName
            };
            return Ok(json);
        }

        
        private async Task<UserEditViewModel> GetUserViewModelHelper(string userId)
        {
            var userAndRoles = await _accountManager.GetUserAndRolesAsync(userId);
            if (userAndRoles == null)
                return null;

            var userVM = _mapper.Map<UserEditViewModel>(userAndRoles.Value.User);
            userVM.Roles = userAndRoles.Value.Roles;

            return userVM;
        }


        [HttpGet("roles")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(ApiResponse<PagedList<RoleViewModel>>))]
        public async Task<IActionResult> GetRolesAsync([FromQuery] PagingParameters pg)
        {
            var roles = await _accountManager.GetRolesAsync(pg);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(roles.MetaData));
            var rolesVM = _mapper.Map<IEnumerable<AppRole>, IEnumerable<RoleViewModel>>(roles);          

            if (roles != null)
            {
                var json = new JsonResponse
                {
                    Json = rolesVM
                };
                return Ok(json);
            }
            return BadRequest("error");
        }

        [HttpGet("all/roles")]
        //[Authorize(Authorization.Policies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(ApiResponse<List<AppRole>>))]
        public async Task<IActionResult> GetRolesAsync()
        {
            var roles = await _accountManager.GetRolesAsync();
            if (roles != null)
            {
                var json = new JsonResponse
                {
                    Json = roles,
                };
                return Ok(json);
            }
            return BadRequest("error");
        }

              
        
        
        private async Task SendVerificationEmail(AppUser appUser)
        {
            string code = await _accountManager.GenerateEmailConfirmationTokenAsync(appUser);
            string callbackUrl = EmailTemplates.GetConfirmEmailCallbackUrl(Request, appUser.Id, code);
            string message = EmailTemplates.GetConfirmAccountEmail(appUser.UserName, callbackUrl);

            await _emailSender.SendEmailAsync(appUser.UserName, appUser.Email, "Confirm your email", message);
        }

        private void AddError(IEnumerable<string> errors, string key = "")
        {
            foreach (var error in errors)
            {
                AddError(error, key);
            }
        }

        private void AddError(string error, string key = "")
        {
            ModelState.AddModelError(key, error);
        }

    }
}
