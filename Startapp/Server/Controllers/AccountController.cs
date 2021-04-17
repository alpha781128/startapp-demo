using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Startapp.Server.Helpers;
using Startapp.Shared.Authorization;
using Startapp.Shared.Core;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Startapp.Server.Controllers
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEmailSender _emailSender;
        private readonly string _publicRoleName;
        private const string GetRoleByIdActionName = "GetRoleById";

        public AccountController(IMapper mapper, IAccountManager accountManager, IAuthorizationService authorizationService, IEmailSender emailSender, IOptions<AppSettings> appSettings )
        {
            _mapper = mapper;
            _accountManager = accountManager;
            _authorizationService = authorizationService;
            _emailSender = emailSender;
            _publicRoleName = appSettings.Value.DefaultUserRole;
        }

        [HttpGet("users/infos/{Id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<UserDTO>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserInfo(string Id)
        {
            var user = await _accountManager.GetUserByIdAsync(Id);
            if (user != null)
            {
                var userDTO = _mapper.Map<UserDTO>(user);
                var json = new JsonResponse
                {
                    Json = userDTO,
                    Message = $"User {userDTO.FullName} authenticated!"
                };
                return Ok(json);
            }
            return BadRequest("error loading user!");
        }

        [HttpGet("users/{userId}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<AppUser>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _accountManager.GetUserAsync(userId);
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
             

        [HttpGet("users/haspassword/{id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserHasPasswordById(string id)
        {
            if (!(await _authorizationService.AuthorizeAsync(this.User, id, AccountManagementOperations.Read)).Succeeded)
                return new ChallengeResult();

            var appUser = await _accountManager.GetUserByIdAsync(id);
            if (appUser != null)
            {
                var userHasPassword = await _accountManager.GetUserHasPasswordAsync(appUser);
                var json = new JsonResponse
                {
                    Json = userHasPassword
                };
                return Ok(json);
            }
            else
                return NotFound($"No user with that id: {id}!");
        }


        [HttpGet("users/username/{userName}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<UserViewModel>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByUserName(string userName)
        {
            AppUser appUser = await _accountManager.GetUserByUserNameAsync(userName);

            if (!(await _authorizationService.AuthorizeAsync(this.User, appUser?.Id ?? "", AccountManagementOperations.Read)).Succeeded)
                return new ChallengeResult();

            if (appUser != null)
            {
                var user = await GetUserViewModelHelper(appUser.Id);
                var json = new JsonResponse
                {
                    Json = user
                };
                return Ok(json);
            }
            else
                return NotFound($"No user with that user name: {userName}!");
        }      
              
     
        [HttpPost("users")]
        [AllowAnonymous]
        [ProducesResponseType(201, Type = typeof(UserViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (user == null)
                    return BadRequest($"{nameof(user)} cannot be null");

                AppUser appUser = _mapper.Map<AppUser>(user);

                var result = await _accountManager.CreateUserAsync(appUser, new string[] { _publicRoleName }, user.NewPassword);
                if (result.Succeeded)
                {
                    await SendVerificationEmail(appUser);                    

                    UserViewModel userVM = await GetUserViewModelHelper(appUser.Id);
                    if (userVM != null)
                    {
                        var json = new JsonResponse
                        {
                            Json = userVM
                        };
                        return Ok(json);
                    }
                    else
                        return NotFound($"errer!");
                }
                else
                    return BadRequest($"errer adding user !");
            }
            return BadRequest($"errer, model not valid !");
        }


        [HttpPost("users/me/sendconfirmemail")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SendConfirmEmail()
        {
            var userId = Utilities.GetUserId(this.User);
            AppUser appUser = await _accountManager.GetUserByIdAsync(userId);

            if (appUser.EmailConfirmed)
                return BadRequest("User email has already been confirmed");

            await SendVerificationEmail(appUser);

            var json = new JsonResponse
            {
                Message = "Successfully confirmed!"
            };
            return Ok(json);
        }


        [HttpPut("confirmemail")]
        [AllowAnonymous]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            AppUser appUser = await _accountManager.GetUserByIdAsync(userId);

            if (appUser == null)
                return NotFound(userId);

            var result = await _accountManager.ConfirmEmailAsync(appUser, code);
            if (!result.Succeeded)
                return BadRequest($"Confirming email failed for user \"{appUser.UserName}\". Errors: {string.Join(", ", result.Errors)}");

            return Accepted();
        }


        [HttpPost("recoverpassword")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserPasswordRecovery))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RecoverPassword([FromBody] UserPasswordRecovery recoveryInfo)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = null;

                if (recoveryInfo.UsernameOrEmail.Contains("@"))
                    appUser = await _accountManager.GetUserByEmailAsync(recoveryInfo.UsernameOrEmail);

                if (appUser == null)
                    appUser = await _accountManager.GetUserByUserNameAsync(recoveryInfo.UsernameOrEmail);

                if (appUser == null || !(await _accountManager.IsEmailConfirmedAsync(appUser)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return NotFound($"Don't reveal that the user does not exist or is not confirmed!");                  
                }

                string code = await _accountManager.GeneratePasswordResetTokenAsync(appUser);
                string callbackUrl = $"{Request.Scheme}://{Request.Host}/ResetPassword?code={code}";
                string message = EmailTemplates.GetResetPasswordEmail(appUser.UserName, HtmlEncoder.Default.Encode(callbackUrl));

                await _emailSender.SendEmailAsync(appUser.UserName, appUser.Email, "Reset Password", message);

                var json = new JsonResponse
                {
                    Json = recoveryInfo,
                };
                return Ok(json);
            }

            return BadRequest(ModelState);
        }

        [HttpPut("resetpassword")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(UserPasswordReset))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResetPassword([FromBody] UserPasswordReset resetInfo)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = null;

                if (resetInfo.UsernameOrEmail.Contains("@"))
                    appUser = await _accountManager.GetUserByEmailAsync(resetInfo.UsernameOrEmail);

                if (appUser == null)
                    appUser = await _accountManager.GetUserByUserNameAsync(resetInfo.UsernameOrEmail);

                if (appUser == null)
                {
                    // Don't reveal that the user does not exist
                    return NotFound();
                }

                var result = await _accountManager.ResetPasswordAsync(appUser, resetInfo.ResetCode, resetInfo.Password);
                if (!result.Succeeded)
                    return BadRequest($"Resetting password failed for user \"{appUser.UserName}\". Errors: {string.Join(", ", result.Errors)}");

                var json = new JsonResponse
                {
                    Json = resetInfo,
                };
                return Ok(json);
            }

            return BadRequest(ModelState);
        }
               

        [HttpPut("users/unblock/{id}")]
        [Authorize(Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UnblockUser(string id)
        {
            AppUser appUser = await _accountManager.GetUserByIdAsync(id);

            if (appUser == null)
                return NotFound(id);

            appUser.LockoutEnd = null;
            var result = await _accountManager.UpdateUserAsync(appUser);
            if (!result.Succeeded)
                throw new Exception("The following errors occurred whilst unblocking user: " + string.Join(", ", result.Errors));


            return NoContent();
        }


       
        //Roles

        [HttpGet("userroles/{userId}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<List<string>>))]
        public async Task<IActionResult> GetUserRolesAsync(string userId)
        {
            var user = await _accountManager.GetUserAndRolesAsync(userId);

            var roles = user.Value.Roles.ToList();

            if (roles != null)
            {
                var json = new JsonResponse
                {
                    Json = roles
                };
                return Ok(json);
            }
            return BadRequest("error");
        }

        [HttpGet("roles/{id}", Name = GetRoleByIdActionName)]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var appRole = await _accountManager.GetRoleByIdAsync(id);

            if (!(await _authorizationService.AuthorizeAsync(this.User, appRole?.Name ?? "", Policies.ViewRoleByRoleNamePolicy)).Succeeded)
                return new ChallengeResult();

            if (appRole == null)
                return NotFound(id);

            return await GetRoleByName(appRole.Name);
        }


        [HttpGet("roles/name/{name}")]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleByName(string name)
        {
            if (!(await _authorizationService.AuthorizeAsync(this.User, name, Policies.ViewRoleByRoleNamePolicy)).Succeeded)
                return new ChallengeResult();

            RoleViewModel roleVM = await GetRoleViewModelHelper(name);

            if (roleVM == null)
                return NotFound(name);

            var json = new JsonResponse
            {
                Json = roleVM
            };
            return Ok(json);

        }
              

        [HttpGet("roles")]
        [Authorize(Policies.ViewAllRolesPolicy)]
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


        [HttpPut("roles/{id}")]
        //[Authorize(Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(201, Type = typeof(RoleViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                if (role == null)
                    return BadRequest($"{nameof(role)} cannot be null");

                if (!string.IsNullOrWhiteSpace(role.Id) && id != role.Id)
                    return BadRequest("Conflicting role id in parameter and model data");


                AppRole appRole = await _accountManager.GetRoleByIdAsync(id);

                if (appRole == null)
                    return NotFound(id);

                _mapper.Map<RoleViewModel, AppRole>(role, appRole);

                var result = await _accountManager.UpdateRoleAsync(appRole, role.Permissions?.Select(p => p.Value).ToArray());
                if (result.Succeeded)
                {
                    var json = new JsonResponse
                    {
                        Json = role,
                        Message = appRole.Name
                    };
                    return Ok(json);
                }
                else
                    return BadRequest($"error");

            }

            return BadRequest(ModelState);
        }


        [HttpPost("roles")]
        //[Authorize(Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(201, Type = typeof(RoleViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddRole([FromBody] RoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                if (role == null)
                    return BadRequest($"{nameof(role)} cannot be null");


                AppRole appRole = _mapper.Map<AppRole>(role);

                var result = await _accountManager.CreateRoleAsync(appRole, role.Permissions?.Select(p => p.Value).ToArray());
                if (result.Succeeded)
                {
                    RoleViewModel roleVM = await GetRoleViewModelHelper(appRole.Name);
                    //return CreatedAtAction(GetRoleByIdActionName, new { id = roleVM.Id }, roleVM);
                    var json = new JsonResponse
                    {
                        Json = roleVM,
                        Message = appRole.Name
                    };
                    return Ok(json);
                }

                AddError(result.Errors);
            }

            return BadRequest(ModelState);
        }


        [HttpDelete("roles/{id}")]
        [Authorize(Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            AppRole appRole = await _accountManager.GetRoleByIdAsync(id);

            if (appRole == null)
                return NotFound(id);

            if (!await _accountManager.TestCanDeleteRoleAsync(id))
                return BadRequest("Role cannot be deleted. Remove all users from this role and try again");

            if (appRole.Name.Equals(_publicRoleName, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Default public role cannot be deleted");


            RoleViewModel roleVM = await GetRoleViewModelHelper(appRole.Name);

            var result = await _accountManager.DeleteRoleAsync(appRole);
            if (!result.Succeeded)
                throw new Exception("The following errors occurred whilst deleting role: " + string.Join(", ", result.Errors));

            var json = new JsonResponse
            {
                Json = roleVM,
                Message = appRole.Name
            };
            return Ok(json);

        }


        [HttpGet("permissions")]
        [Authorize(Policies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PermissionViewModel>))]
        public IActionResult GetAllPermissions()
        {
            return Ok(_mapper.Map<List<PermissionViewModel>>(ApplicationPermissions.AllPermissions));
        }

        private async Task<UserViewModel> GetUserViewModelHelper(string userId)
        {
            var userAndRoles = await _accountManager.GetUserAndRolesAsync(userId);
            if (userAndRoles == null)
                return null;

            var userVM = _mapper.Map<UserViewModel>(userAndRoles.Value.User);
            userVM.Roles = userAndRoles.Value.Roles;

            return userVM;
        }


        private async Task<RoleViewModel> GetRoleViewModelHelper(string roleName)
        {
            var role = await _accountManager.GetRoleLoadRelatedAsync(roleName);
            if (role != null)
                return _mapper.Map<RoleViewModel>(role);


            return null;
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
