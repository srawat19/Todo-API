using Microsoft.AspNetCore.Mvc;
using Moq;
using Todo.Domain.DTOs;
using Todo.Domain.Interfaces;
using Todo_API.Controllers;

namespace Todo.Test
{
    public class GetTodoTest
    {
        private readonly TodoController _todoController;
        private readonly Mock<ITodoItemService> _mockTodoItemService;
        public GetTodoTest()
        {
            _mockTodoItemService = new Mock<ITodoItemService>();
            _todoController = new TodoController(_mockTodoItemService.Object);
        }

        [Fact]
        public async void GetTodo_WhenPageAndLimitIsZero_ReturnsBadRequest()
        {
            //Arrange
            int page = 0; int limit = 0;

            //Act
            var res = await _todoController.GetTodos(page, limit);


            //Assert
            var badReqResult = Assert.IsType<BadRequestObjectResult>(res);
            Assert.Equal(400, badReqResult.StatusCode);
            Assert.Equal("Invalid Page or Limit value provided.", badReqResult.Value);
            _mockTodoItemService.Verify(x => x.GetTodosForUser(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async void DeleteTodo_WhenUserIsInvalid_ReturnsUserIdentityMissing()
        {
            //Arrange
            TestHelper.SetAuthenticatedUser(_todoController, new Dictionary<string, string>()
            {
                { "oid","" }
            });


            // Act
            var res = await _todoController.GetTodos(1, 10);

            //Assert
            var unAuthRes = Assert.IsType<UnauthorizedObjectResult>(res);
            Assert.Equal(401, unAuthRes.StatusCode);
            Assert.Contains("User Identity is missing", unAuthRes.Value.ToString());
            _mockTodoItemService.Verify(x => x.GetTodosForUser(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);


        }

        [Fact]
        public async void GetTodo_WhenPageandLimitAreValid_ReturnsOkWithCurrentUserTodos()
        {
            //Arrange
            TestHelper.SetAuthenticatedUser(_todoController, new Dictionary<string, string>()
            {
                { "oid","12345-8989-qbioh" }
            });

            _mockTodoItemService.Setup(x => x.GetTodosForUser(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Domain.DTOs.PaginatedResponseDTO<Domain.DTOs.TodoDTO>()
            {
                Data = new List<TodoDTO>()
                {
                    new TodoDTO()
                    {
                        Id=1, Title="sample", Description="This is first record",
                    },
                    new TodoDTO()
                    {

                        Id=2, Title="Sample2", Description="This is second record",
                    },
                      new TodoDTO()
                    {

                        Id=3, Title="Sample3", Description="This is third record",
                    }
                },
                TotalRecords = 6,
                TotalPages = 2,
                Limit = 3,
                Page = 1

            });

            // Act
            var res = await _todoController.GetTodos(1, 3);

            //Assert
            var okRes = Assert.IsType<OkObjectResult>(res);
            Assert.Equal(200, okRes.StatusCode);
            var paginatedData = Assert.IsType<PaginatedResponseDTO<TodoDTO>>(okRes.Value);

            Assert.Equal(6, paginatedData.TotalRecords);

        }




    }
}
