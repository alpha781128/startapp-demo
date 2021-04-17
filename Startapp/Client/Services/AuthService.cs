using IdentityModel.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MultiLanguages.Translator;
using Startapp.Client.Exceptions;
using Startapp.Client.Repository;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace Startapp.Client.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LogWithPasswordAsync(LoginVM user);
        Task<LoginResponse> RefreshTokenAsync(string refreshToken);
        Task<LoginResponse> LogWithExternalProviderAsync(ExternalTokenRequest externalParams);
        Task<LoginResponse> RegisterUserAsync(RegisterViewModel user);
        Task<UserPasswordRecovery> RecoverPasswordAsync(UserPasswordRecovery passwordRecovery);
        Task<UserPasswordReset> ResetPasswordAsync(UserPasswordReset passwordReset);
        Task LogoutUserAsync();
        void SetTimer(int interval);
    }


    public class AuthService : IAuthService
    {
        private readonly string clientId = "startapp_spa";
        private string baseUrl = "";
        private readonly string scope = "openid email phone profile offline_access roles startapp_api";

        private readonly IGenericRepository _genericRepository;
        private readonly AuthenticationStateProvider _authProvider;
        private readonly HttpClient _httpClient;
        private static IJSRuntime JsRuntime;
        private readonly ILanguageContainerService _translate;
        public DiscoveryCache Discovery { get; set; }

        private static Timer aTimer;

        public AuthService(IGenericRepository genericRepository,
            AuthenticationStateProvider authProvider,
            HttpClient httpClient, IJSRuntime JSRuntime, ILanguageContainerService translator)
        {
            _genericRepository = genericRepository;
            _authProvider = authProvider;
            _httpClient = httpClient;
            baseUrl = _httpClient.BaseAddress.AbsoluteUri;
            Discovery = new DiscoveryCache(_httpClient.BaseAddress.AbsoluteUri);
            JsRuntime = JSRuntime;
            _translate = translator;
        }



        public async Task<LoginResponse> MapToLoginResponse(TokenResponse response)
        {
            var lr = new LoginResponse();
            if (response.IsError)
            {
                string error = response.ErrorDescription;
                if (!string.IsNullOrEmpty(error))
                {
                    lr.Message = $"- Error: {response.ErrorDescription.Replace("_", " ")}";
                }
                else lr.Message = $"- Error, occur to authenticate local server!";
            }
            else
            {
                lr.UserId = Utilities.GetClaimValueFromToken(response.AccessToken, "sub");
                lr.AccessToken = response.AccessToken;
                lr.RefreshToken = response.RefreshToken;
                lr.ExpiresIn = DateTime.UtcNow.AddMinutes(response.ExpiresIn / 60);
                lr.TokenType = response.TokenType;
                lr.Message = "Successfully log in";
                if (!string.IsNullOrEmpty(lr.AccessToken))
                {
                    await ((ApiAuthenticationStateProvider)_authProvider).MarkUserAsAuthenticated(lr);
                }
            }
            ShowNotification(lr.Message);
            ShowEmailNotification(lr.AccessToken);
            return lr;
        }

        public async Task<LoginResponse> LogWithPasswordAsync(LoginVM user)
        {
            var lr = new LoginResponse();
            var disco = await Discovery.GetAsync();
            // discover endpoints from metadata
            //var disco = await _httpClient.GetDiscoveryDocumentAsync();
            if (disco.IsError)
            {
                lr.Message = disco.Error.ToString();
                return lr;
            }
            // request token
            var response = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                //Address = $"{baseUrl}connect/token",
                UserName = user.UserName,
                Password = user.Password,
                ClientId = clientId,
                GrantType = "password",
                Scope = scope
            });

            lr = await MapToLoginResponse(response);

            return lr;

        }

        public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
        {
            var lr = new LoginResponse();
            var disco = await Discovery.GetAsync();
            if (disco.IsError)
            {
                lr.Message = disco.Error.ToString();
                return lr;
            }
            var response = await _httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                //Address = $"{baseUrl}connect/token",
                RefreshToken = refreshToken,
                ClientId = clientId,
                GrantType = "refresh_token"
                //Scope = scope
            });
            lr = await MapToLoginResponse(response);

            return lr;
        }

        public async Task<LoginResponse> LogWithExternalProviderAsync(ExternalTokenRequest parameters)
        {
            var lr = new LoginResponse();
            var disco = await Discovery.GetAsync();
            //var disco = await _httpClient.GetDiscoveryDocumentAsync();
            if (disco.IsError)
            {
                lr.Message = disco.Error.ToString();
                return lr;
            }
            var response = await _httpClient.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                //Address = $"{baseUrl}connect/token",                
                ClientId = clientId,
                GrantType = "delegation",
                Parameters = new Dictionary<string, string> { { "token", parameters.Token }, { "scope", scope }, { "provider", parameters.Provider } }
            });
            lr = await MapToLoginResponse(response);
            return lr;
        }

        public async Task<LoginResponse> RegisterUserAsync(RegisterViewModel user)
        {
            var lr = new LoginResponse();
            try
            {
                var ru = await _genericRepository.PostAsync<RegisterViewModel, UserViewModel>("api/account/users", user);

                if (ru.IsEnabled)
                {
                    LoginVM loguser = new LoginVM
                    {
                        UserName = user.UserName,
                        Password = user.NewPassword
                    };
                    lr = await LogWithPasswordAsync(loguser);
                }
                lr.Message = ru.ErrorMessage;
                return lr;
            }
            catch (HttpRequestExceptionEx e)
            {
                lr.Message = e.Message;
                Debug.WriteLine(e.HttpCode);
                return lr;
            }
        }

        public async Task<UserPasswordRecovery> RecoverPasswordAsync(UserPasswordRecovery passwordRecovery)
        {
            try
            {
                var response = await _genericRepository.PostAsync<UserPasswordRecovery>("api/account/recoverpassword", passwordRecovery);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new UserPasswordRecovery();
            }
        }
        public async Task<UserPasswordReset> ResetPasswordAsync(UserPasswordReset passwordReset)
        {
            try
            {
                var response = await _genericRepository.PutAsync<UserPasswordReset>("api/account/resetpassword", passwordReset);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new UserPasswordReset();
            }
        }

        public async Task LogoutUserAsync()
        {
            var msg = await ((ApiAuthenticationStateProvider)_authProvider).MarkUserAsLoggedOut();
            ShowNotification(msg);
        }

        private async void ShowNotification(string message)
        {
            if (message.Contains("Error"))
            {
                await JsRuntime.InvokeAsync<object>("ShowToaster", "error", message);
                return;
            }
            await JsRuntime.InvokeAsync<object>("ShowToaster", "success", message);
        }

        private async void ShowEmailNotification(string accessToken)
        {
            var claims = JwtParserHelper.ParseClaimsFromJwt(accessToken);
            var otac = claims.FirstOrDefault(x => x.Type == "otac");
            if (otac != null)
            {
                string ModalParams;
                var m = new ModalSuccess
                {
                    Title = _translate.Keys["ConfirmEmail"],
                    Message = _translate.Keys["EmailConfirmation"]
                };
                ModalParams = JsonSerializer.Serialize(m);

                await JsRuntime.InvokeVoidAsync("showModalDialog", ModalParams);
            }
          
        }

        public void SetTimer(int interval)
        {
            // Create a timer with a two second interval.
            aTimer = new Timer(interval * 1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var refreshToken = await JsRuntime.InvokeAsync<string>("GetFromLocalStorege", "refreshToken");

            await RefreshTokenAsync(refreshToken);
            //ShowNotification("access token refresh success: " + refreshToken);
            aTimer.Enabled = false;
        }


    }

}
