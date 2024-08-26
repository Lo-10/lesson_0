using Autofac;
using lesson_0.Models;
using lesson_0.Models.Requests.Post;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.Metrics;
using System.Net.Mime;
using System.Security.Claims;

namespace lesson_0.Controllers
{
    [Produces("application/json")]
    public class PostController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PostController> _logger;

        public PostController(ILifetimeScope scope)
        {
            _mediator = scope.Resolve<IMediator>();
            _logger = scope.Resolve<ILogger<PostController>>();
        }
        /// <summary>
        /// Создание поста
        /// </summary>
        /// <returns> Идентификатор поста </returns>
        /// <response code="200">Успешно создан пост</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="401">Неавторизованный доступ</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpPost("/post/create")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> CreatePost([FromBody] PostCreateRequest post)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity.FindFirst("userId").Value, out var userId)) return Unauthorized();

            post.UserId = userId;

            var result = await _mediator.Send(post);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Ошибка сервера"
                });
            }
            else if (result.PostId == default) return BadRequest();
            else return Ok(result.PostId);
        }

        /// <summary>
        /// Изменение поста
        /// </summary>
        /// <response code="200">Успешно изменен пост</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="401">Неавторизованный доступ</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpPut("/post/update")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> UpdatePost([FromBody] PostUpdateRequest request)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity.FindFirst("userId").Value, out var userId)) return Unauthorized();

            request.UserId = userId;

            var result = await _mediator.Send(request);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Ошибка сервера"
                });
            }
            else return Ok();
        }

        /// <summary>
        /// Получение поста
        /// </summary>
        /// <param name="id" example="dd5feafb-166e-48de-a3cf-bb115b3827e9">Идентификатор поста</param>
        /// <response code="200">Успешно получен пост</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="401">Неавторизованный доступ</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpGet("/post/get/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(PostGetResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> GetPost(Guid id)
        {
            var result = await _mediator.Send(new PostGetRequest { PostId = id });
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Ошибка сервера"
                });
            }
            else return Ok(result);
        }

        /// <summary>
        /// Удаление поста
        /// </summary>
        /// <param name="id" example="dd5feafb-166e-48de-a3cf-bb115b3827e9">Идентификатор поста</param>
        /// <response code="200">Успешно удален пост</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="401">Неавторизованный доступ</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpPut("/post/delete/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var result = await _mediator.Send(new PostDeleteRequest { PostId = id });
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Ошибка сервера"
                });
            }
            else return Ok();
        }

        /// <summary>
        /// Получение постов друзей
        /// </summary>
        /// <param name="offset" example="100"></param>
        /// <param name="limit" example="10"></param>
        /// <response code="200">Успешно получены посты друзей</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="401">Неавторизованный доступ</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpGet("/post/feed")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(PostGetResponse[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> GetPostFeed(int offset = 0, int limit = 10)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity.FindFirst("userId").Value, out var userId)) return Unauthorized();

            var request = new PostFeedGetRequest { UserId = userId, Offset = offset, Limit = limit };

            var result = await _mediator.Send(request);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Ошибка сервера"
                });
            }
            else return Ok(result);
        }
    }
}
