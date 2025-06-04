using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain.DTOs;
using Todo.Domain.Interfaces;
using Todo_API.Controllers;

namespace Todo.Test
{
    public class AdminGetTodoTest
    {

        private readonly AdminController _adminController;
        private readonly Mock<ITodoItemService> _mockTodoItemService;

        public AdminGetTodoTest()
        {
            _mockTodoItemService = new Mock<ITodoItemService>();
            _adminController = new AdminController(_mockTodoItemService.Object);
        }




        [Fact]
        public async void GetTodo_WhenPageandLimitAreValid_ReturnsOkWithTodos()
        {
            //Arrange
            TestHelper.SetAuthenticatedUser(_adminController, new Dictionary<string, string>()
            {
                { "oid","12345-8989-qbioh" },
                {ClaimTypes.Role,"admin1" }
            });

            int totalPages = 2;
            int currentPage = 1;

            _mockTodoItemService.Setup
                (x => x.GetTodosForAdmin(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Domain.DTOs.PaginatedResponseDTO<Domain.DTOs.TodoDTO>()
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
                    TotalPages = totalPages,
                    Limit = 3,
                    Page = currentPage


                });



            var urlHelper = new Mock<IUrlHelper>();

            urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns<UrlActionContext>(x =>
            {
                var page = x.Values.GetType().GetProperty("page")?.GetValue(x.Values);
                //var obj = x.Values;
                if (page != null)
                {
                    if ((int)page == 2)
                        return "http://localhost:5279/api/admin/todos?page=2&limit=3";
                    if ((int)page == 1)
                        return "http://localhost:5279/api/admin/todos?page=1&limit=3";
                }

                return null;
            }
             );

            _adminController.Url = urlHelper.Object;
            // Act
            var res = await _adminController.GetAllTodos(1, 3);

            //Assert
            var okRes = Assert.IsType<OkObjectResult>(res);
            Assert.Equal(200, okRes.StatusCode);
            var paginatedData = Assert.IsType<PaginatedResponseDTO<TodoDTO>>(okRes.Value);

            Assert.Equal(6, paginatedData.TotalRecords);
            Assert.Equal("http://localhost:5279/api/admin/todos?page=2&limit=3", paginatedData.NextPageUrl);

        }
    }
}
