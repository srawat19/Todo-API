using Todo.Domain.Interfaces;
using Todo_API.Controllers;
using Moq;
using Todo.Domain.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Todo.Domain.Entities;
using System.Security.Claims;

namespace Todo.Test
{
    public class CreateTodoTest
    {
        private readonly TodoController _todoController;
        private readonly Mock<ITodoItemService> _mockTodoService;
        public CreateTodoTest()
        {
            _mockTodoService = new Mock<ITodoItemService>();
            _todoController = new TodoController(_mockTodoService.Object);

        }


        [Fact]
        public async Task CreateTodo_ReturnsBadRequest_WhenInvalidModelState()
        {
            //Arrange 
            var invalidDTO = new TodoDTO();
            invalidDTO.Description = "Sample Todo Item1";
            TestHelper.ValidateModel(invalidDTO, _todoController);

            //Act 
            var result = await _todoController.CreateTodoItem(invalidDTO);

            //Assert            
            var res = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, res.StatusCode);
            Assert.False(_todoController.ModelState.IsValid);
            Assert.True(_todoController.ModelState.ContainsKey("Title"));

            _mockTodoService.Verify(x => x.CreateTodo(It.IsAny<TodoItem>()), Times.Never);
        }


        [Fact]
        public async Task CreateTodo_ReturnsCreatedAction_WhenValidUserAndTodo()
        {
            //Arrange 
            var todoItem = new TodoDTO() { Title = "sample", Description = "This is a sample Todo" };

            TestHelper.ValidateModel(todoItem, _todoController);

            TestHelper.SetAuthenticatedUser(_todoController, new Dictionary<string, string>()
            {
                {"oid","123445-tyyuy" },
                {ClaimTypes.Role,"user" }
            });
            _mockTodoService.Setup(s => s.CreateTodo(It.IsAny<TodoItem>())).ReturnsAsync(new TodoItem { Title = "sample", Description = "This is a sample Todo", Id = 1 });

            //Act 
            var res = await _todoController.CreateTodoItem(todoItem);


            //Assert 

            var createdResult = Assert.IsType<CreatedAtActionResult>(res);
            Assert.Equal(201, createdResult.StatusCode);

            Assert.NotNull(createdResult.RouteValues);

            Assert.Equal(1, createdResult.RouteValues["id"]);

            Assert.Equal("GetTodoItem", createdResult.ActionName);

            var createdValue = Assert.IsType<TodoDTO>(createdResult.Value);
            Assert.NotNull(createdValue);

            Assert.Equal("sample", createdValue.Title);
            Assert.Equal("This is a sample Todo", createdValue.Description);


        }


        [Fact]
        public async void CreateTodo_ReturnsUnauthorized_WhenInvalidUser()
        {
            //Arrange 
            var validDto = new TodoDTO()
            {
                Title = "Sample",
                Description = "This is a sample data"
            };
            TestHelper.ValidateModel(validDto, _todoController);
            TestHelper.SetAuthenticatedUser(_todoController, new Dictionary<string, string>()
            {
                { "oid","" }
            }
            );

            //Act 
            var result = await _todoController.CreateTodoItem(validDto);


            //Assert
            var unAuthResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unAuthResult.StatusCode);
            Assert.Contains("User Identity is missing or invalid", unAuthResult.Value.ToString());

        }
    }
}
