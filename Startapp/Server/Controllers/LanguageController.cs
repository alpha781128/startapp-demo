using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Startapp.Shared.Core;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Startapp.Server.Controllers
{
    [Route("api/[controller]")]
    public class LanguageController : ControllerBase
    {
        private readonly IMapper _mapper;      
        private readonly IDataStore _dataStore;

        public LanguageController(IMapper mapper, IDataStore dataStore)
        {
            _mapper = mapper;
            _dataStore = dataStore;
        }

        [HttpGet("languages")]
        [ProducesResponseType(200, Type = typeof(List<Language>))]
        public IActionResult Get()
        {
            var languages =  _dataStore.GetLanguagesAsync();
            if (languages != null)
            {              
                return Ok(languages);
            }
            return BadRequest("error loading languages!");
        }

        [HttpGet("languages/paging")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<PagedList<Language>>))]
        public async Task<IActionResult> Get([FromQuery] PagingParameters pagingParameters)
        {
            var languages = await _dataStore.GetLanguagesAsync(pagingParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(languages.MetaData));

            if (languages != null)
            {
                var json = new JsonResponse
                {
                    Json = languages
                    //Message = "Successfully loaded!"
                };
                return Ok(json);
            }
            return BadRequest("error loading languages!");
        }

        [HttpGet("languages/{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Language>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _dataStore.GetLanguageAsync(id);
            if (result.Succeeded)
            {
                var json = new JsonResponse
                {
                    Json = result.Language
                };
                return Ok(json);
            }
            else
                return NotFound($"No language with that id: {id}!");
        }       

        [HttpPost("languages")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Language>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Add([FromBody] Language language)
        {
            if (language == null)
                return BadRequest($"{nameof(language)} cannot be null");
            string msg = string.Empty;
            if (!ModelState.IsValid)
                return BadRequest("Complete and correct all required fields and try again!");
            if (ModelState.IsValid)
            {
                var result = await _dataStore.AddLanguageAsync(language);
                msg = result.Message;
                if (result.Succeeded)
                {
                    var json = new JsonResponse
                    {
                        Json = result.Language,
                        Message = msg
                    };
                    return Ok(json);
                }
            }
            return BadRequest(msg);
        }

        [HttpPut("languages")]
        [ProducesResponseType(201, Type = typeof(ApiResponse<Language>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] Language language)
        {
            if (language == null)
                return BadRequest($"{nameof(language)} cannot be null");            
            string msg = string.Empty;
            if (!ModelState.IsValid)
                return BadRequest("Complete and correct all required fields and try again!");
            if (ModelState.IsValid)
            {
                var result = await _dataStore.GetLanguageAsync(language.Id);
                if (!result.Succeeded)
                {
                    return NotFound(result.Message);
                }
                var old = result.Language;
                _mapper.Map<Language, Language>(language, old);
                result = await _dataStore.UpdateLanguageAsync(old);
                msg = result.Message;
                if (result.Succeeded)
                {
                    var json = new JsonResponse
                    {
                        Json = result.Language,
                        Message = msg
                    };
                    return Ok(json);
                }
            }
            return BadRequest(msg);
        }

        [HttpDelete("languages/{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Language>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _dataStore.DeleteLanguageAsync(id);
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
       
      

    }
}
