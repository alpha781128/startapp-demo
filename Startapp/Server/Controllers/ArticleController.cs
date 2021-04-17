using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Startapp.Shared.Core;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using Startapp.Shared.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Startapp.Server.Controllers
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDataStore _dataStore;

        public ArticleController(IMapper mapper, IDataStore dataStore)
        {
            _mapper = mapper;
            _dataStore = dataStore;
        }

        [HttpGet("articles/{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Article>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            //if (!(await _authorizationService.AuthorizeAsync(this.User, id, AccountManagementOperations.Read)).Succeeded)
            //    return new ChallengeResult();
            var result = await _dataStore.GetArticleDetailsAsync(id);
            if (result.Succeeded)
            {
                var json = new JsonResponse
                {
                    Json = result.Article
                };
                return Ok(json);
            }
            else
                return NotFound($"{id}");
        }

        [HttpPost("articles")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Article>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Add([FromBody] Article article)
        {

            
            if (article == null)
                return BadRequest($"{nameof(article)} cannot be null");
            string msg = string.Empty;
            if (!ModelState.IsValid)
                return BadRequest("Complete and correct all required fields and try again!");
            if (ModelState.IsValid)
            {
                var result = await _dataStore.AddArticleAsync(article);
                msg = result.Message;
                if (result.Succeeded)
                {
                    var json = new JsonResponse
                    {
                        Json = result.Article,
                        Message = msg
                    };
                    return Ok(json);
                }
            }
            return BadRequest(msg);
        }

        [HttpPut("articles")]
        [ProducesResponseType(201, Type = typeof(ApiResponse<Article>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] Article article)
        {
            if (article == null)
                return BadRequest($"{nameof(article)} cannot be null");
            string msg = string.Empty;
            if (!ModelState.IsValid)
                return BadRequest("Complete and correct all required fields and try again!");
            if (ModelState.IsValid)
            {
                var result = await _dataStore.GetArticleDetailsAsync(article.Id);
                if (!result.Succeeded)
                {
                    return NotFound(result.Message);
                }
                var oldArticle = result.Article;
                _mapper.Map<Article, Article>(article, oldArticle);
                result = await _dataStore.UpdateArticleAsync(oldArticle);
                msg = result.Message;
                if (result.Succeeded)
                {
                    var json = new JsonResponse
                    {
                        Json = result.Article,
                        Message = msg
                    };
                    return Ok(json);
                }
            }
            return BadRequest(msg);
        }

        [HttpDelete("articles/{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Article>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _dataStore.DeleteArticleAsync(id);
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

        //for all users "anonymous users"
        [HttpGet("public/articles/{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Article>))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        public async Task<IActionResult> GetPubArticle(int id)
        {
            var result = await GetArticleViewModelHelper(id);
            if (result != null)
            {
                var json = new JsonResponse
                {
                    Json = result
                };
                return Ok(json);
            }
            else
                return NotFound($"{id}");
        }


        [HttpGet("articles")]
        //[Authorize(Authorization.Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(ApiResponse<PagedList<Article>>))]
        public async Task<IActionResult> Get([FromQuery] PagingParameters pagingParameters)
        {
            string userId = Utilities.GetUserId(this.User);
            PagedList<Article> articles;
            if (User.IsInRole("administrator"))
                articles = await _dataStore.GetNewestArticlesAsync(pagingParameters);
            else articles = await _dataStore.GetRelatedArticlesAsync(pagingParameters, userId);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(articles.MetaData));

            if (articles != null)
            {
                var json = new JsonResponse
                {
                    Json = articles
                };
                return Ok(json);
            }
            return BadRequest("error");
        }

        [HttpGet("articles/newest")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<PagedList<ArticleViewModel>>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewestArticles([FromQuery] PagingParameters pagingParameters)
        {
            var articles = await _dataStore.GetNewestArticlesAsync(pagingParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(articles.MetaData));
            var articlesVM = _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleViewModel>>(articles);

            if (articles != null)
            {
                var json = new JsonResponse
                {
                    Json = articlesVM
                };
                return Ok(json);
            }
            return BadRequest("error");
        }

        [HttpGet("articles/topviewed")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<PagedList<ArticleViewModel>>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopViewedArticles([FromQuery] PagingParameters pagingParameters)
        {
            var articles = await _dataStore.GetTopViewedAsync(pagingParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(articles.MetaData));
            var articlesVM = _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleViewModel>>(articles);

            if (articles != null)
            {
                var json = new JsonResponse
                {
                    Json = articlesVM
                };
                return Ok(json);
            }
            return BadRequest("error");
        }

        [HttpGet("articles/topselled")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<PagedList<ArticleViewModel>>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopSelledArticles([FromQuery] PagingParameters pagingParameters)
        {
            var articles = await _dataStore.GetTopSelledAsync(pagingParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(articles.MetaData));
            var articlesVM = _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleViewModel>>(articles);

            if (articles != null)
            {
                var json = new JsonResponse
                {
                    Json = articlesVM
                };
                return Ok(json);
            }
            return BadRequest("error");
        }

        [HttpGet("articles/random")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<PagedList<ArticleViewModel>>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetRandomArticles([FromQuery] PagingParameters pagingParameters)
        {
            var articles = await _dataStore.GetRandomArticlesAsync(pagingParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(articles.MetaData));
            var articlesVM = _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleViewModel>>(articles);

            if (articles != null)
            {
                var json = new JsonResponse
                {
                    Json = articlesVM
                };
                return Ok(json);
            }
            return BadRequest("error");
        }

        [HttpGet("articles/related/{userId}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<PagedList<ArticleViewModel>>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetRelatedArticles([FromQuery] PagingParameters pagingParameters, string userId)
        {
            var articles = await _dataStore.GetRelatedArticlesAsync(pagingParameters, userId);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(articles.MetaData));
            var articlesVM = _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleViewModel>>(articles);

            if (articles != null)
            {
                var json = new JsonResponse
                {
                    Json = articlesVM
                };
                return Ok(json);
            }
            return BadRequest("error");
        }
        [HttpGet("articles/relatedcategory")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<PagedList<ArticleViewModel>>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetRelatedCategoryArticles([FromQuery] PagingParameters pagingParameters)
        {
            var articles = await _dataStore.GetRelatedCategoryAsync(pagingParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(articles.MetaData));
            var articlesVM = _mapper.Map<IEnumerable<Article>, IEnumerable<ArticleViewModel>>(articles);

            if (articles != null)
            {
                var json = new JsonResponse
                {
                    Json = articlesVM
                };
                return Ok(json);
            }
            return BadRequest("error");
        }

        //complement
        private async Task<ArticleViewModel> GetArticleViewModelHelper(int id)
        {
            var result = await _dataStore.GetArticleDetailsAsync(id);
            if (!result.Succeeded)
                return null;

            var articleVM = _mapper.Map<ArticleViewModel>(result.Article);

            return articleVM;
        }


    }
}
