using Startapp.Client.Exceptions;
using Startapp.Client.Repository;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Startapp.Client.Services
{
    public interface IRoleService
    {
        Task<PagingResponse<RoleViewModel>> GetPageAsync(PagingParameters pagingParameters, string url, string userId = null);
        Task<List<AppRole>> GetRolesAsync(string url);
        Task<RoleViewModel> GetAsync(string id);
        Task<RoleViewModel> AddAsync(RoleViewModel role);
        Task<RoleViewModel> UpdateAsync(string id, RoleViewModel role);
        Task<RoleViewModel> DeleteAsync(string id);
    }

    public class RoleService : IRoleService
    {
        private readonly IGenericRepository _genericRepository;
        public RoleService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<PagingResponse<RoleViewModel>> GetPageAsync(PagingParameters pagingParameters, string url, string userId = null)
        {
            try
            {
                var response = await _genericRepository.GetPageAsync<RoleViewModel, PagingResponse<RoleViewModel>>(url, pagingParameters, userId);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new PagingResponse<RoleViewModel>();
            }
        }

        public async Task<List<AppRole>> GetRolesAsync(string url)
        {
            try
            {
                var response = await _genericRepository.GetAsync<List<AppRole>>(url);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new List<AppRole>();
            }
        }

        public async Task<RoleViewModel> GetAsync(string id)
        {
            try
            {
                return await _genericRepository.GetAsync<RoleViewModel>($"api/account/roles/{id}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new RoleViewModel();
            }
        }
        public async Task<RoleViewModel> AddAsync(RoleViewModel role)
        {
            try
            {
                var response = await _genericRepository.PostAsync<RoleViewModel>($"api/account/roles", role);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return role;
            }
        }

        public async Task<RoleViewModel> UpdateAsync(string id, RoleViewModel role)
        {
            try
            {
                var response = await _genericRepository.PutAsync<RoleViewModel>($"api/account/roles/{id}", role);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return role;
            }
        }
        public async Task<RoleViewModel> DeleteAsync(string id)
        {
            try
            {
                return await _genericRepository.DeleteAsync<RoleViewModel>($"api/account/roles/{id}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return null;
            }
        }
    }
}
