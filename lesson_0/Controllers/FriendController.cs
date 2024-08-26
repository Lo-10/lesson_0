using Autofac;
using lesson_0.Models;
using lesson_0.Models.Requests.Friend;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Net.Mime;
using System.Security.Claims;

namespace lesson_0.Controllers
{
    [Produces("application/json")]
    public class FriendController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FriendController> _logger;
        public FriendController(ILifetimeScope scope)
        {
            _mediator = scope.Resolve<IMediator>();
            _logger = scope.Resolve<ILogger<FriendController>>();
        }
        /// <summary>
        /// Создание поста
        /// </summary>
        /// <response code="200">Успешно создан пост</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="401">Неавторизованный доступ</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpPut("/friend/set/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> AddFriend(string id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity.FindFirst("userId").Value, out var userId)) return Unauthorized();

            var result = await _mediator.Send(new FriendAddRequest() { UserId = userId.ToString(), FriendId = id });
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Ошибка сервера"
                });
            }
            else if (result.UserId == default || result.FriendId == default) return BadRequest();
            else return Ok();
        }

        /// <summary>
        /// Удаление друга
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <response code="200">Пользователь успешно удалил из друзей пользователя</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="401">Неавторизованный доступ</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpPut("/friend/delete/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity.FindFirst("userId").Value, out var userId)) return Unauthorized();

            var result = await _mediator.Send(new FriendDeleteRequest() { UserId = userId.ToString(), FriendId = id });
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Ошибка сервера"
                });
            }
            else if (!result.Value) return BadRequest();
            else return Ok();
        }
    }
}
