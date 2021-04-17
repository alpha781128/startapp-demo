using Startapp.Client.Exceptions;
using Startapp.Client.Repository;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Startapp.Client.Services
{
    public interface IUserService
    {
        Task<PagingResponse<AppUser>> GetPageAsync(PagingParameters pagingParameters, string url, string userId = null);
        Task<UserEditViewModel> AddAsync(UserEditViewModel user);
        Task<UserEditViewModel> UpdateAsync(string userId, UserEditViewModel user);
        Task<UserEditViewModel> UpdateCurrentUserAsync(UserEditViewModel user);
        Task<UserEditViewModel> DeleteAsync(string userId);
    }

    public class UserService : IUserService
    {
        private readonly IGenericRepository _genericRepository;
        public UserService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<PagingResponse<AppUser>> GetPageAsync(PagingParameters pagingParameters, string url, string userId = null)
        {
            try
            {
                var response = await _genericRepository.GetPageAsync<AppUser, PagingResponse<AppUser>>(url, pagingParameters, userId);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new PagingResponse<AppUser>();
            }
        }    

        public async Task<UserEditViewModel> AddAsync(UserEditViewModel user)
        {
            try
            {
                var response = await _genericRepository.PostAsync<UserEditViewModel>($"api/admin/users", user);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return user;
            }
        }

        public async Task<UserEditViewModel> UpdateAsync(string userId, UserEditViewModel user)
        {
            try
            {
                var response = await _genericRepository.PutAsync<UserEditViewModel>($"api/admin/users/{userId}", user);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return user;
            }
        }

        public async Task<UserEditViewModel> UpdateCurrentUserAsync(UserEditViewModel user)
        {
            try
            {
                var response = await _genericRepository.PutAsync<UserEditViewModel>($"api/admin/users/me", user);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return user;
            }
        }

        public async Task<UserEditViewModel> DeleteAsync(string userId)
        {
            try
            {
                return await _genericRepository.DeleteAsync<UserEditViewModel>($"api/admin/users/{userId}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return null;
            }
        }
    }
}
