using System;
using Todo.Domain.Interfaces;
using Todo_API.Controllers;
using Moq;
using Todo.Application.Services;
using Todo.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Todo.Domain.Entities;

namespace Todo.Test
{
    public class UpdateTodoTest
    {
        private readonly TodoController _todoController;
        private readonly ITodoItemService _todoService;
        private readonly Mock<ITodoItemService> _todoServiceMock;
        public UpdateTodoTest()
        {
            _todoServiceMock = new Mock<ITodoItemService>();
            _todoService = _todoServiceMock.Object;
            _todoController = new TodoController(_todoService);
        }


        [Fact]
        public async void UpdateTodo_WhenModelIsInvaid_ReturnsBadRequest()
        {
            //Arrange
            var invalidTodo = new TodoDTO() { Title = "sample", Description = "" };

            TestHelper.ValidateModel(invalidTodo, _todoController);

            //Act
            var res = await _todoController.UpdateTodoItem(invalidTodo, 1);


            //Assert
            var badReqResult = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal(400, badReqResult.StatusCode);
            _todoServiceMock.Verify(x => x.UpdateTodo(It.IsAny<TodoItem>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);


        }

        [Fact]
        public async void UpdateTodo_WhenUserIsInvaid_ReturnsUserIdentityMissing()
        {
            //Arrange
            var validDTO = new TodoDTO() { Title = "sampleUpdt", Description = "This is an update" };

            TestHelper.ValidateModel(validDTO, _todoController);
            TestHelper.SetAuthenticatedUser(_todoController, new Dictionary<string, string>()
            {
                { "oid","" }
            });


            // Act
            var res = await _todoController.UpdateTodoItem(validDTO, 1);

            //Assert
            var unAuthRes = Assert.IsType<UnauthorizedObjectResult>(res);
            Assert.Equal(401, unAuthRes.StatusCode);
            Assert.Contains("User Identity is missing", unAuthRes.Value.ToString());
            _todoServiceMock.Verify(x => x.UpdateTodo(It.IsAny<TodoItem>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);


        }

        [Fact]
        public async void UpdateTodo_WhenIdToUpdateNotExists_ReturnsNotFound()
        {
            //Arrange 
            var validDTO = new TodoDTO() { Title = "sampleUpdt", Description = "This is an update", Id = 6 };
            TestHelper.ValidateModel(validDTO, _todoController);
            TestHelper.SetAuthenticatedUser(_todoController, new Dictionary<string, string>()
            {
                {"oid","123445-tyyuy" }
            });

            _todoServiceMock.Setup(x => x.UpdateTodo(It.IsAny<TodoItem>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((TodoItem?)null);


            //Act 
            var res = await _todoController.UpdateTodoItem(validDTO, 6);


            //Assert 
            var notFoundRes = Assert.IsType<NotFoundObjectResult>(res);
            Assert.Equal(404, notFoundRes.StatusCode);
            Assert.Contains("No TodoItem was updated", notFoundRes.Value.ToString());


        }

        [Fact]
        public async void UpdateTodo_WhenIdIsValid_ReturnsOkWithUpdatedData()
        {
            //Arrange 
            var validDTO = new TodoDTO()
            {
                Title = "sampleUpdt",
                Description = "This is an update",
                Id = 1
            };
            TestHelper.ValidateModel(validDTO, _todoController);
            TestHelper.SetAuthenticatedUser(_todoController, new Dictionary<string, string>()
            {
                { "oid","123445-tyyuy"}
            });

            _todoServiceMock.Setup(x => x.UpdateTodo(It.IsAny<TodoItem>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new TodoItem()
            {
                Id = 1,
                Title = "sampleUpdt",
                Description = "This is an update",
            });

            //Act 
            var res = await _todoController.UpdateTodoItem(validDTO, 1);

            //Assert 
            var okResult = Assert.IsType<OkObjectResult>(res);
            Assert.Equal(200, okResult.StatusCode);
            var updatedResult = Assert.IsType<TodoDTO>(okResult.Value);
            Assert.Equal("This is an update", updatedResult.Description);


        }
    }
}
