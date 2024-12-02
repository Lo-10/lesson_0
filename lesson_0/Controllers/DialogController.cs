using Autofac;
using lesson_0.Models;
using lesson_0.Models.Requests.Dialog;
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
    public class DialogController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DialogController> _logger;

        public DialogController(ILifetimeScope scope)
        {
            _mediator = scope.Resolve<IMediator>();
            _logger = scope.Resolve<ILogger<DialogController>>();
        }
        /// <summary>
        /// Отправка сообщения пользователю
        /// </summary>
        /// <response code="200">Успешно отправлено сообщение</response>
        /// <response code="400">Невалидные данные ввода</response>
        /// <response code="401">Неавторизованный доступ</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpPost("api/v1.0/dialog/{userId}/send")]
        [HttpPost("api/v2.0/dialog/{userId}/send")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> CreatePost(Guid userId, [FromBody] SendMessageRequest request)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity.FindFirst("userId").Value, out var fromUserId)) return Unauthorized();

            request.FromUserId = fromUserId;
            request.ToUserId = userId;
            request.RequestId = HttpContext.TraceIdentifier;

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
        /// Получение диалога
        /// </summary>
        /// <param name="userId" example="dd5feafb-166e-48de-a3cf-bb115b3827e9">Идентификатор пользователя-получателя</param>
        /// <response code="200">Успешно получен диалог</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="401">Неавторизованный доступ</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpGet("api/v1.0/dialog/{userId}/list")]
        [HttpGet("api/v2.0/dialog/{userId}/list")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(DialogMessageModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> GetPost(Guid userId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity.FindFirst("userId").Value, out var fromUserId)) return Unauthorized();

            var result = await _mediator.Send(new DialogGetRequest {
                FromUserId = fromUserId,
                ToUserId = userId,
                RequestId = HttpContext.TraceIdentifier
            });

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
        /// Получение количества непрочитанных сообщений
        /// </summary>
        /// <param name="userId" example="dd5feafb-166e-48de-a3cf-bb115b3827e9">Идентификатор пользователя-получателя</param>
        /// <response code="200">Успешно получен диалог</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="401">Неавторизованный доступ</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpGet("api/v2.0/dialogs/unreaded")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> GetUnreaded()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity.FindFirst("userId").Value, out var userId)) return Unauthorized();

            var result = await _mediator.Send(new UnreadMessageCountGetRequest
            {
                UserId = userId,
                RequestId = HttpContext.TraceIdentifier
            });

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
