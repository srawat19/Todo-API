using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Todo.Domain.DTOs;
using Todo.Domain.Interfaces;

namespace Todo_API.Controllers
{
    /// <summary>
    /// Admin controller handles admin tasks. As of now, admin can view all users ToDos.
    /// </summary>
   

    [ApiController]
    [Route("api/admin/todos")]
    [Authorize(Roles = "admin")]
    [EnableRateLimiting("RoleBasedPolicy")]

    public class AdminController : ControllerBase
    {

        private readonly ITodoItemService _todoItemService;
        public AdminController(ITodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }


        /// <summary>
        /// Retrives all ToDos available, irrespective of which user created them.
        /// </summary>
        /// <param name="page">Optional. Page number to be retrieved. Provide value only when requesting a specific page </param>
        /// <param name="limit">Number of ToDos to be displayed in a page</param>
        /// <returns>HTTP Status Code 200 : ToDos were successfully retrived.
        /// HTTP Status Code 204 : No ToDos were found.
        /// HTTP Status Code 400 : One or more input param were invalid. 
        /// HTTP Status Code 401 : User trying to access does not have 'Admin' role.
        /// </returns>
        /// <remarks>
        /// Accessible only to users with 'Admin' role
        /// </remarks>
        /// 

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponseDTO<TodoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(Tags = ["Admin"])]

        public async Task<ActionResult> GetAllTodos([FromQuery] int page, [FromQuery] int limit)
        {
            if (page == 0 || limit == 0)
                return BadRequest("Invalid Page or Limit value provided.");

            var currentUserId = this.User.FindFirst("oid")?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new { message = "User Identity is missing or invalid." });
            }

            var todos = await _todoItemService.GetTodosForAdmin(page, limit);

            if (todos.TotalRecords == 0)
                return NoContent();

            //https://localhostxxxx/api/admin/todos

            todos.NextPageUrl = page < todos.TotalPages ? Url.Action("GetAllTodos", "Admin", new { page = page + 1, limit }, Request.Scheme) : null;

            todos.PrevPageUrl = page > 1 ? Url.Action("GetAllTodos", "Admin", new { page = page - 1, limit }, Request.Scheme) : null;

            return Ok(todos);
        }

    }
}
