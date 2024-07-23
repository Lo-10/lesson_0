using Autofac;
using lesson_0.Accession;
using lesson_0.Models;
using lesson_0.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using XAct.Users;

namespace lesson_0.Controllers
{
    [Produces("application/json")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;
        public UserController(ILifetimeScope scope)
        {
            _mediator = scope.Resolve<IMediator>();
            _logger = scope.Resolve<ILogger<UserController>>();
        }
        /// <summary>
        /// Упрощенный процесс аутентификации путем передачи идентификатор пользователя и получения токена для дальнейшего прохождения авторизации
        /// </summary>
        /// <returns> Bearer токен </returns>
        /// <response code="200">Успешная аутентификация</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="404">Пользователь не найден</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [HttpPost("/login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Token([FromBody] LoginRequest credentials)
        {
            var result = await _mediator.Send(credentials);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Ошибка сервера"
                });
            }
            else if (result.Token == null)
            {
                return BadRequest();
            }
            else return Ok(result);
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <response code="200">Успешная регистрация</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="404">Пользователь не найден</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [HttpPost("/user/register")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserRegisterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest user)
        {
            var result = await _mediator.Send(user);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Ошибка сервера"
                });
            }
            else if (result.UserId == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Имя пользователя занято"
                });
            }
            else return Ok(result);
        }

        /// <summary>
        /// Получение анкеты пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <response code="200">Успешное получение анкеты пользователя</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="404">Анкета не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpGet("/user/get/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserGetRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> GetUser(string id)
        {
            var result = await _mediator.Send(new UserGetRequest() { UserId = id });
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Ошибка сервера"
                });
            }
            else if (result.UserId == default) return NotFound();
            else return Ok(result);
        }

        /// <summary>
        /// Поиск анкет
        /// </summary>
        /// <param name="firstName">Условие поиска по имени</param>
        /// <param name="lastName">Условие поиска по фамилии</param>
        /// <response code="200">Успешные поиск пользователя</response>
        /// <response code="400">Невалидные данные</response>
        /// <response code="500">Ошибка сервера</response>
        /// <response code="503">Ошибка сервера</response>
        [Authorize]
        [HttpGet("/user/search")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserGetRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        [ProducesResponseType(typeof(ServerErrorModel), StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(StatusCodes.Status500InternalServerError, "Retry-After", "integer", "Время, через которое еще раз нужно сделать запрос")]
        public async Task<IActionResult> SearchUser(string firstName, string lastName)
        {
            var result = await _mediator.Send(new UserSearchRequest() { FirstName = firstName, LastName = lastName });
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerErrorModel()
                {
                    Code = 500,
                    RequestId = HttpContext.Connection.Id,
                    Message = "Ошибка сервера"
                });
            }
            else if (result.UserId == default) return NotFound();
            else return Ok(result);
        }
    }
}
