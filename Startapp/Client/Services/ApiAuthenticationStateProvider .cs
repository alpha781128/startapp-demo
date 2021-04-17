using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Startapp.Client.Helpers;
using Startapp.Client.Repository;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Startapp.Client.Services
{
    public class SavedToken
    {
        public IEnumerable<Claim> Claims { get; set; }
        public LoginResponse SavedLR { get; set; } = new LoginResponse();
    }
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IGenericRepository _genericRepository;
        private readonly IJSRuntime _jsRuntime;

        private bool firstTimeThrough = true;

        public ApiAuthenticationStateProvider(ILocalStorageService localStorage,
            IGenericRepository genericRepository, IJSRuntime jsRuntime)
        {
            _localStorage = localStorage;
            _genericRepository = genericRepository;
            _jsRuntime = jsRuntime;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            SavedToken savedToken = await GetTokenAsync();

            if (string.IsNullOrWhiteSpace(savedToken.SavedLR.AccessToken))
            {
                firstTimeThrough = false;
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            if (firstTimeThrough)
            {
                firstTimeThrough = false;
                await MarkUserAsAuthenticated(savedToken);
            }
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(savedToken.Claims, "apiauth")));
        }
        //Public interface...no need for claims to be exposed
        public async Task MarkUserAsAuthenticated(LoginResponse lr)
        {
            SavedToken st = ParseToken(lr);
            await MarkUserAsAuthenticated(st);
        }
        private async Task MarkUserAsAuthenticated(SavedToken savedToken)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, savedToken.SavedLR.UserId) }, "apiauth"));
            authenticatedUser.AddIdentity(new ClaimsIdentity(JwtParserHelper.ParseClaimsFromJwt(savedToken.SavedLR.AccessToken), "apiauth"));

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            await _localStorage.SetItemAsync("authToken", savedToken.SavedLR.AccessToken);
            await _localStorage.SetItemAsync("refreshToken", savedToken.SavedLR.RefreshToken);
            await _localStorage.SetItemAsync("expireIn", savedToken.SavedLR.ExpiresIn);
            //await _jsRuntime.InvokeAsync<object>("RemoveUnusedParams");
            await _jsRuntime.InvokeMethod("RemoveUnusedParams");
            _genericRepository.SetResponse(savedToken.SavedLR);
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task<string> MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("refreshToken");
            await _localStorage.RemoveItemAsync("expireIn");      
            _genericRepository.SetResponse(new LoginResponse());
            NotifyAuthenticationStateChanged(authState);
            return "Successfully logged out!";
        }

        private async Task<SavedToken> GetTokenAsync()
        {
            var accessToken = await _localStorage.GetItemAsync<string>("authToken");
            var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");
            var expireDate = await _localStorage.GetItemAsync<DateTime>("expireIn");
            return ParseToken(new LoginResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expireDate
            });
        }

        private SavedToken ParseToken(LoginResponse lr)
        {
            if (string.IsNullOrWhiteSpace(lr.AccessToken))
            {
                return new SavedToken();
            }
           
            var claims = JwtParserHelper.ParseClaimsFromJwt(lr.AccessToken);
            string userId = claims.Where(x => x.Type == "sub").Select(x => x.Value).FirstOrDefault();
            return new SavedToken()
            {
                Claims = claims,
                SavedLR = new LoginResponse()
                {
                    UserId = userId,
                    AccessToken = lr.AccessToken,
                    ExpiresIn = lr.ExpiresIn,
                    RefreshToken = lr.RefreshToken
                }
            };
        }       


    }
}
