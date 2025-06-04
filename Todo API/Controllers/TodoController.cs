using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Todo.Domain.DTOs;
using Todo.Domain.Entities;
using Todo.Domain.Interfaces;
using Todo.Domain.MapperExtensions;

namespace Todo_API.Controllers
{
    [Authorize]
    [Route("api/todos")]
    [ApiController]
    [EnableRateLimiting("RoleBasedPolicy")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoItemService _todoItemService;
        public TodoController(ITodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }


        /// <summary>
        /// Creates a new ToDo item
        /// </summary>
        /// <param name="todoDTO">Details for ToDo to be created</param>
        /// <returns> </returns>
        /// <remarks>Accessible to users with Role - 'User','Admin'</remarks>
        ///
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(Tags = ["User/Admin"])]
        public async Task<IActionResult> CreateTodoItem([FromBody] TodoDTO todoDTO)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("ModelState", "To-do Model state is not valid");
                return BadRequest(ModelState);
            }

            string? oid = this.User.FindFirst("oid")?.Value;

            if (string.IsNullOrEmpty(oid))
            {
                return Unauthorized(new { message = "User Identity is missing or invalid." });
            }

            var todoEntity = todoDTO.ToDomainEntity();
            todoEntity.UserId = oid;

            TodoItem newTodo = await _todoItemService.CreateTodo(todoEntity);


            return CreatedAtAction(nameof(GetTodoItem), new { id = newTodo.Id }, newTodo.ToDTO());

        }

        /// <summary>
        /// Retrieves a specific ToDo item based on Id.
        /// </summary>
        /// <param name="id">ToDo item id to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [SwaggerOperation(Tags = ["User/Admin"])]

        public async Task<IActionResult> GetTodoItem(int id)
        {
            if (id == 0)
                return BadRequest();

            TodoItem? todoItem = await _todoItemService.GetTodo(id); //convert to DTO here
            if (todoItem == null)
                return NotFound();

            return Ok(todoItem);

        }

        /// <summary>
        /// Updates an existing ToDo item
        /// </summary>
        /// <param name="todoDto">Updated ToDo item</param>
        /// <param name="id">ToDo item id to be updated</param>
        /// <returns></returns>

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Tags = ["User/Admin"])]

        public async Task<IActionResult?> UpdateTodoItem([FromBody] TodoDTO todoDto, int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("ModelState", "Model State invalid");

                    return BadRequest(ModelState);
                }

                var currentUserId = this.User.FindFirst("oid")?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new { message = "User Identity is missing or invalid." });
                }

                TodoItem? todo = await _todoItemService.UpdateTodo(todoDto.ToDomainEntity(), id, currentUserId);

                if (todo == null)
                    return NotFound(new { message = "No TodoItem was updated." });

                return Ok(todo.ToDTO());
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not authorized"))
                    return Forbid(ex.Message);

                return BadRequest(ex);
            }
        }



        /// <summary>
        /// Deletes existing ToDo item
        /// </summary>
        /// <param name="id">ToDo item id to be deleted</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpDelete("{id:int}")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(Tags = ["User/Admin"])]

        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            try
            {
                if (id == 0 || id < 0)
                    return BadRequest("Id to delete can't be 0 or a negative value.");


                var currentUserId = this.User.FindFirst("oid")?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new { message = "User Identity is missing or invalid." });
                }
                await _todoItemService.DeleteTodo(id, currentUserId);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("No existing"))
                    return NotFound();
                else if (ex.Message.Contains("not authorized"))
                    return Forbid(ex.Message);
                else
                    return BadRequest(ex);
            }
        }

        /// <summary>
        /// Retrieves all ToDos belonging to the current user
        /// </summary>
        /// <param name="page">Optional. Specific page number to be retrieved.</param>
        /// <param name="limit">Number of ToDos to be displayed in a page</param>
        /// <returns></returns>
        [Authorize(Roles = "user,admin")]
        [HttpGet("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(Tags = ["User/Admin"])]


        public async Task<IActionResult> GetTodos([FromQuery] int page, [FromQuery] int limit)
        {
            if (page == 0 || limit == 0)
                return BadRequest("Invalid Page or Limit value provided.");

            var currentUserId = this.User.FindFirst("oid")?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new { message = "User Identity is missing or invalid." });
            }

            var todos = await _todoItemService.GetTodosForUser(currentUserId, page, limit);

            if (todos.TotalRecords == 0)
                return NoContent();

            return Ok(todos);
        }






    }
}
