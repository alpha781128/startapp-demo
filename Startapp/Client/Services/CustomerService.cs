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
    public interface ICustomerService
    {
        Task<PagingResponse<Customer>> GetPageAsync(PagingParameters pagingParameters);
        Task<Customer> GetAsync(string id);
        Task<Customer> AddAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task<Customer> DeleteAsync(int id);

        Task<AppUser> GetUserAsync(string id);
        Task<UserEditViewModel> GetUserAsync();
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<UserEditViewModel> UpdateUserAsync(UserEditViewModel user);

        Task<ClientInfo> GetClientInfoAsync(string id);
        Task<ClientInfo> UpdateClientInfoAsync(ClientInfo clientInfo);

    }

    public class CustomerService : ICustomerService
    {
        private readonly IGenericRepository _genericRepository;
        public CustomerService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<PagingResponse<Customer>> GetPageAsync(PagingParameters pagingParameters)
        {
            try
            {
                var response = await _genericRepository.GetPageAsync<Customer, PagingResponse<Customer>>($"api/customer/customers", pagingParameters);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new PagingResponse<Customer>();
            }
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            try
            {
                return await _genericRepository.GetAsync<List<string>>($"api/account/userroles/{userId}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new List<string>();
            }
        }

        public async Task<AppUser> GetUserAsync(string userId)
        {
            try
            {
                return await _genericRepository.GetAsync<AppUser>($"api/account/users/{userId}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new AppUser();
            }
        }

        public async Task<UserEditViewModel> GetUserAsync()
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

        public async Task<UserEditViewModel> UpdateUserAsync(UserEditViewModel user)
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

        public async Task<Customer> GetAsync(string userId)
        {        
            try
            {
                return await _genericRepository.GetAsync<Customer>($"api/customer/customers/{userId}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new Customer();
            }
        }
        
        public async Task<Customer> AddAsync(Customer customer)
        {
            try
            {
                var response = await _genericRepository.PostAsync<Customer>($"api/customer/customers", customer);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return customer;
            }
        }       

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            try
            {
                var response = await _genericRepository.PutAsync<Customer>($"api/customer/customers", customer);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return customer;
            }
        }
        public async Task<Customer> DeleteAsync(int id)
        {
            try
            {
                return await _genericRepository.DeleteAsync<Customer>($"api/customer/customers/{id}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return null;
            }
        }

        public async Task<ClientInfo> GetClientInfoAsync(string userId)
        {
            try
            {
                return await _genericRepository.GetAsync<ClientInfo>($"api/customer/customer/{userId}");
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return new ClientInfo();
            }
        }

        public async Task<ClientInfo> UpdateClientInfoAsync(ClientInfo clientInfo)
        {
            try
            {
                var response = await _genericRepository.PutAsync<ClientInfo>($"api/customer/customer", clientInfo);
                return response;
            }
            catch (HttpRequestExceptionEx e)
            {
                Debug.WriteLine(e.HttpCode);
                return clientInfo;
            }
        }
    }

   
}
