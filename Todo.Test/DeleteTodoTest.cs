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
    public class DeleteTodoTest
    {
        private readonly TodoController _todoController;
        private readonly ITodoItemService _todoService;
        private readonly Mock<ITodoItemService> _todoServiceMock;
        public DeleteTodoTest()
        {
            _todoServiceMock = new Mock<ITodoItemService>();
            _todoService = _todoServiceMock.Object;
            _todoController = new TodoController(_todoService);
        }


        [Fact]
        public async void DeleteTodo_WhenIdToUpdateIsZeroOrNegative_ReturnsBadRequest()
        {
            //Arrange
            int id = It.Is<int>(x => x == 0) | It.Is<int>(x => x == -1);

            //Act
            var res = await _todoController.DeleteTodoItem(id);


            //Assert
            var badReqResult = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal(400, badReqResult.StatusCode);
            Assert.Equal("Id to delete can't be 0 or a negative value.", badReqResult.Value);
            _todoServiceMock.Verify(x => x.DeleteTodo(It.IsAny<int>(), It.IsAny<string>()), Times.Never);


        }

        [Fact]
        public async void DeleteTodo_WhenUserIsInvaid_ReturnsUserIdentityMissing()
        {
            //Arrange
            int id = It.Is<int>(x => x == 0) | It.Is<int>(x => x == -1);


            TestHelper.SetAuthenticatedUser(_todoController, new Dictionary<string, string>()
            {
                { "oid","" }
            });


            // Act
            var res = await _todoController.DeleteTodoItem(1);

            //Assert
            var unAuthRes = Assert.IsType<UnauthorizedObjectResult>(res);
            Assert.Equal(401, unAuthRes.StatusCode);
            Assert.Contains("User Identity is missing", unAuthRes.Value.ToString());
            _todoServiceMock.Verify(x => x.DeleteTodo(It.IsAny<int>(), It.IsAny<string>()), Times.Never);


        }

        [Fact]
        public async void DeleteTodo_WhenIdToUpdateNotExists_ReturnsNotFound()
        {
            //Arrange 
            int id = 6;

            TestHelper.SetAuthenticatedUser(_todoController, new Dictionary<string, string>()
            {
                {"oid","123445-tyyuy" }
            });

            _todoServiceMock.Setup(x => x.DeleteTodo(It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(new Exception($"No existing Todo with Id:{id}"));


            //Act 
            var res = await _todoController.DeleteTodoItem(id);


            //Assert 
            var notFoundRes = Assert.IsType<NotFoundResult>(res);
            Assert.Equal(404, notFoundRes.StatusCode);


        }

        [Fact]
        public async void DeleteTodo_WhenIdIsValid_ReturnsNoContent()
        {
            //Arrange 
           
            TestHelper.SetAuthenticatedUser(_todoController, new Dictionary<string, string>()
            {
                { "oid","123445-tyyuy"}
            });

             _todoServiceMock.Setup(x => x.DeleteTodo(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.CompletedTask);
          

            //Act 
            var res = await _todoController.DeleteTodoItem(1);

            //Assert 
            var noContentResult = Assert.IsType<NoContentResult>(res);
            Assert.Equal(204, noContentResult.StatusCode);
      


        }
    }
}

