using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MultiLanguages.Translator;
using Startapp.Client.Exceptions;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Startapp.Client.Repository
{
    public class GenericRepository : IGenericRepository 
    {
        private readonly HttpClient _httpClient;
        private LoginResponse LoginResponse = new LoginResponse();
        private string _token = "";
        private static IJSRuntime JsRuntime;
        public ILanguageContainerService Translate { get; set; }


        public GenericRepository(HttpClient httpClient, IJSRuntime JSRuntime, ILanguageContainerService Translate)
        {
            _httpClient = httpClient;
            JsRuntime = JSRuntime;
            this.Translate = Translate;
        }
        public void SetResponse(LoginResponse loginResponse)
        {
            if (!string.IsNullOrEmpty(loginResponse.AccessToken))
            {
                LoginResponse = loginResponse;
                _token = LoginResponse.AccessToken;
            }
        }
       
        public async Task<T> GetAsync<T>(string uri)
        {
            return await CallAPI<T>(HttpMethod.Get, uri, null);
        }       

        public async Task<T> PostAsync<T>(string uri, T data)
        {
            return await CallAPI<T>(HttpMethod.Post, uri, data);
        }

        public async Task<TR> PostAsync<T, TR>(string uri, T data)
        {
            return await CallAPI<TR>(HttpMethod.Post, uri, data);
        }

        public async Task<T> PutAsync<T>(string uri, T data)
        {
            return await CallAPI<T>(HttpMethod.Put, uri, data);
        }

        public async Task<T> DeleteAsync<T>(string uri)
        {
            return await CallAPI<T>(HttpMethod.Delete, uri, null);
        }

        private async Task<T> CallAPI<T>(HttpMethod method, string uri, object data)
        {
            try
            {
                string jsonResult = string.Empty;
                var req = new HttpRequestMessage(method, uri);
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (data != null)
                {
                    req.Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                }
                if (!string.IsNullOrEmpty(_token))
                {
                    req.Headers.Add("Authorization", $"Bearer {_token}");
                }
                var response = await _httpClient.SendAsync(req);
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    jsonResult = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonResponse>(jsonResult, options);
                    ShowNotification(method, "success", jsonResponse.Message);
                    var json = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(jsonResponse.Json), options);

                    return json;
                }
                if (response.StatusCode.ToString() == "BadRequest" || response.StatusCode.ToString() == "NotFound")
                {
                    jsonResult = await response.Content.ReadAsStringAsync();
                    jsonResult = JsonSerializer.Deserialize<string>(jsonResult);
                    ShowNotification(method, "error", jsonResult);
                }
                throw new HttpRequestExceptionEx(response.StatusCode, jsonResult);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.GetType().Name} : {e.Message}");
                throw;
            }
        }

        public async Task<TR> GetStreamAsync<T, TR>(string uri)
        {
            try
            {
                string jsonResult = string.Empty;
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));              
                var response = await _httpClient.SendAsync(req);           
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    var content = await response.Content.ReadAsStringAsync();
                    var json = JsonSerializer.Deserialize<TR>(content, options);                   
                    return json;
                }
                if (response.StatusCode.ToString() == "BadRequest" || response.StatusCode.ToString() == "NotFound")
                {
                    jsonResult = "Error loading list";
                    ShowNotification(HttpMethod.Get, "error", jsonResult);
                }
                throw new HttpRequestExceptionEx(response.StatusCode, jsonResult);

            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.GetType().Name} : {e.Message}");
                throw;
            }
        }

        public async Task<TR> GetPageAsync<T, TR>(string uri, PagingParameters pagingParameters, string userId = null)
        {
            try
            {
                string errorMessage = string.Empty;
                var queryStringParam = new Dictionary<string, string>
                {
                    ["pageNumber"] = pagingParameters.PageNumber.ToString(),
                    ["pageSize"] = pagingParameters.PageSize.ToString(),
                    ["searchTerm"] = pagingParameters.SearchTerm,
                    ["cat"] = pagingParameters.Cat.ToString()
                };
                if (!string.IsNullOrEmpty(userId))
                {
                    queryStringParam.Add("userId", userId);
                }
                var url = QueryHelpers.AddQueryString(uri, queryStringParam);
                var req = new HttpRequestMessage(HttpMethod.Get, url);

                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(_token))
                {
                    req.Headers.Add("Authorization", $"Bearer {_token}");
                }
                var response = await _httpClient.SendAsync(req);
                var content = await response.Content.ReadAsStringAsync();               
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    var jsonResponse = JsonSerializer.Deserialize<JsonResponse>(content, options);

                    var pagingResponse = new
                    {
                        Items = jsonResponse.Json,
                        MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), options)
                    };
                    return JsonSerializer.Deserialize<TR>(JsonSerializer.Serialize(pagingResponse), options);
                }
                if (response.StatusCode.ToString() == "BadRequest" || response.StatusCode.ToString() == "NotFound")
                {
                    errorMessage = await response.Content.ReadAsStringAsync();
                    errorMessage = JsonSerializer.Deserialize<string>(errorMessage);
                    ShowNotification(HttpMethod.Get, "error", errorMessage);
                }
                throw new HttpRequestExceptionEx(response.StatusCode, errorMessage);

            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.GetType().Name} : {e.Message}");
                throw;
            }
        }

        public async Task<string> UploadImage(MultipartFormDataContent content, string id = null, string table = null)
        {
            var postResult = await _httpClient.PostAsync($"api/picture/upload/{id}/{table}", content);
            var jsonResponse = await postResult.Content.ReadAsStringAsync();
            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(jsonResponse);
            }
            else
            {              
                return jsonResponse;
            }
        }

        private async void ShowNotification(HttpMethod method, string type, string message)
        {
            string key ;
            if (type == "error")
            {
                key = (method.Method.ToString()) switch
                {
                    "POST" => "FailedToAdd",
                    "PUT" => "FailedToUpdate",
                    "DELETE" => "FailedToDelete",
                    _ => "success",
                };
            }
            else
            {
                key = (method.Method.ToString()) switch
                {
                    "POST" => "AddSuccess",
                    "PUT" => "UpdateSuccess",
                    "DELETE" => "DeleteSuccess",
                    _ => "success",
                };
            }          
            
            if (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Delete)
            {
                await JsRuntime.InvokeAsync<object>("ShowToaster", type, $"{message}, {Translate.Keys[key]}");
            };          
        }

      

      

        //private string GetUri(string uri) => $"{uri}";
    }
}
