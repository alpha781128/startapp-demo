using Microsoft.AspNetCore.Components.Authorization;
using Startapp.Client.Exceptions;
using Startapp.Client.Repository;
using Startapp.Shared;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Startapp.Client.Services
{
    public interface IClientUserService
    {
        event EventHandler<UserAuthenticatedArgs> UserAuthenticatedEvent;

        Task<UserDTO> GetUserInfo(string Id);
        Task<UserEditViewModel> GetCurrentUser();

        Task<WeatherForecast[]> GetForecasts(); // just to demonstrate how to consume array of data

    }

    public class ClientUserService : IClientUserService
    {
        private readonly IGenericRepository _genericRepository;
        private readonly AuthenticationStateProvider _authProvider;

        public event EventHandler<UserAuthenticatedArgs> UserAuthenticatedEvent;
        public ClientUserService(IGenericRepository genericRepository,
            AuthenticationStateProvider authProvider)
        {
            _genericRepository = genericRepository;
            _authProvider = authProvider;
            _authProvider.AuthenticationStateChanged += _authProvider_AuthenticationStateChanged;
        }

        private void _authProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            if (task.Result.User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)task.Result.User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                UserAuthenticatedEvent?.Invoke(this, new UserAuthenticatedArgs(userId));
            }
            else
            {
                UserAuthenticatedEvent?.Invoke(this, new UserAuthenticatedArgs(""));
            }
        }

        public async Task<UserEditViewModel> GetCurrentUser()
        {
            try
            {
                return await _genericRepository.GetAsync<UserEditViewModel>($"api/admin/users/me");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new UserEditViewModel();
            }
        }

        public async Task<UserDTO> GetUserInfo(string Id)
        {
            try
            {
                return await _genericRepository.GetAsync<UserDTO>($"api/account/users/infos/{Id}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new UserDTO();
            }
        }

        public async Task<WeatherForecast[]> GetForecasts()
        {
            return await _genericRepository.GetAsync<WeatherForecast[]>("WeatherForecast");
        }
             

    }
}
