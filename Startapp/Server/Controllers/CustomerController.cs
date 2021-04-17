using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Startapp.Shared.Core;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Startapp.Server.Controllers
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IDataStore _dataStore;

        public CustomerController(IMapper mapper, IAccountManager accountManager, IDataStore dataStore)
        {
            _mapper = mapper;
            _accountManager = accountManager;
            _dataStore = dataStore;
        }

        [HttpGet("customers/{userId}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Customer>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAsync(string userId)
        {
            //if (!(await _authorizationService.AuthorizeAsync(this.User, id, AccountManagementOperations.Read)).Succeeded)
            //    return new ChallengeResult();
            var result = await _dataStore.GetCustomerAsync();
            if (result.Succeeded)
            {
                var json = new JsonResponse
                {
                    Json = result.Customer
                };
                return Ok(json);
            }
            else
                return NotFound($"No customer with that user id: {userId}!");
        }
        
        [HttpGet("customers")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(ApiResponse<PagedList<Customer>>))]
        public async Task<IActionResult> GetAsync([FromQuery] PagingParameters pagingParameters, string searchTerm = null)
        {
            Thread.Sleep(2000);
            var customers = await _dataStore.GetCustomersAsync(pagingParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(customers.MetaData));

            if (customers != null)
            {
                var json = new JsonResponse
                {
                    Json = customers
                };
                return Ok(json);
            }
            return BadRequest("error loading customers!");
        }

        [HttpPost("customers")]
        [ProducesResponseType(201, Type = typeof(ApiResponse<Customer>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddAsync([FromBody] Customer customer)
        {
            if (customer == null)
                return BadRequest($"{nameof(customer)} cannot be null");
            string msg = string.Empty;
            if (!ModelState.IsValid)
                return BadRequest("Complete and correct all required fields and try again!");
            if (ModelState.IsValid)
            {
                var result = await _dataStore.AddCustomerAsync(customer);
                msg = result.Message;
                if (result.Succeeded)
                {
                    var json = new JsonResponse
                    {
                        Json = result.Customer,
                        Message = msg
                    };
                    return Ok(json);
                }
            }
            return BadRequest(msg);
        }

        [HttpPut("customers")]
        [ProducesResponseType(201, Type = typeof(ApiResponse<Customer>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateAsync([FromBody] Customer customer)
        {
            if (customer == null)
                return BadRequest($"{nameof(customer)} cannot be null");
            string msg = string.Empty;
            if (!ModelState.IsValid)
                return BadRequest("Complete and correct all required fields and try again!");
            if (ModelState.IsValid)
            {
                var result = await _dataStore.GetCustomerAsync();
                if (!result.Succeeded)
                {
                    result = await _dataStore.AddCustomerAsync(customer);
                    msg = result.Message;
                    if (result.Succeeded)
                    {
                        var json = new JsonResponse
                        {
                            Json = result.Customer,
                            Message = msg
                        };
                        return Ok(json);
                    }
                }
                var oldCustomer = result.Customer;
                customer.Id = oldCustomer.Id;
                _mapper.Map<Customer, Customer>(customer, oldCustomer);
                result = await _dataStore.UpdateCustomerAsync(oldCustomer);
                msg = result.Message;
                if (result.Succeeded)
                {
                    var json = new JsonResponse
                    {
                        Json = result.Customer,
                        Message = msg
                    };
                    return Ok(json);
                }
            }
            return BadRequest(msg);
        }

        [HttpDelete("customers/{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Customer>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _dataStore.DeleteCustomerAsync(id);
            if (result.Succeeded)
            {
                var json = new JsonResponse
                {
                    Json = result,
                    Message = result.Message
                };
                return Ok(json);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("customer/{userId}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<ClientInfo>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetClientAsync(string userId)
        {
            //if (!(await _authorizationService.AuthorizeAsync(this.User, id, AccountManagementOperations.Read)).Succeeded)
            //    return new ChallengeResult();
            var clientInfo = new ClientInfo();
            var user = await _accountManager.GetUserByIdAsync(userId);
            if (user!= null)
            {
                clientInfo.UserId = user.Id;
                clientInfo.FirstName = user.FirstName;
                clientInfo.LastName = user.LastName;
            }
            var result = await _dataStore.GetCustomerAsync();
            if (result.Succeeded)
            {               
                _mapper.Map<Customer, ClientInfo>(result.Customer, clientInfo);                
            }
            var json = new JsonResponse
            {
                Json = clientInfo
            };
            return Ok(json);
        }

        [HttpPut("customer")]
        [ProducesResponseType(201, Type = typeof(ApiResponse<ClientInfo>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateClientAsync([FromBody] ClientInfo clientInfo)
        {
            if (clientInfo == null)
                return BadRequest($"{nameof(clientInfo)} cannot be null");
            string msg = string.Empty;
            if (!ModelState.IsValid)
                return BadRequest("Complete and correct all required fields and try again!");
            if (ModelState.IsValid)
            {
                var customer = new Customer();
                _mapper.Map<ClientInfo, Customer>(clientInfo, customer);
                await UpdateAsync(customer);
                var user = await _accountManager.GetUserByIdAsync(clientInfo.UserId);
                if (user != null)
                {
                    user.FirstName = clientInfo.FirstName;
                    user.LastName = clientInfo.LastName;
                }
                var result = await _accountManager.UpdateUserAsync(user);
                if (result.Succeeded)
                {
                    var json = new JsonResponse
                    {
                        Json = clientInfo
                    };
                    return Ok(json);
                }
            }
            return BadRequest(msg);
        }

    }
}
