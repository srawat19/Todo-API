using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Application.Interfaces.Repositories;
using Todo.Domain.DTOs;
using Todo.Domain.Entities;
using Todo.Domain.Interfaces;
using Todo.Domain.MapperExtensions;



namespace Todo.Application.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly ITodoItemRepository _todoRepo;
        public TodoItemService(ITodoItemRepository todoRepo)
        {
            _todoRepo = todoRepo;
        }

        //create todo
        public async Task<TodoItem> CreateTodo(TodoItem todoItem)
        {
            return await _todoRepo.AddAsync(todoItem);
        }

        //update todo
        public async Task<TodoItem?> UpdateTodo(TodoItem todoItem, int id, string currUserObjId)
        {
            //check User is authorized for update 
            var todo = await _todoRepo.GetByIdAsync(id);

            if (todo == null)
                return null;

            if (!CheckValidUser(currUserObjId, todo))
                throw new Exception("Current User is not authorized to update this todo item.");


            if (todo != null && CheckValidUser(currUserObjId, todo))

            {
                return await _todoRepo.UpdateAsync(todoItem);
            }

            return null;
        }

        private bool CheckValidUser(string currUserObjId, TodoItem todo)
        {
            return currUserObjId.Equals(todo.UserId, StringComparison.OrdinalIgnoreCase);
        }


        //get todo
        public async Task<TodoItem?> GetTodo(int id)
        {
            return await _todoRepo.GetByIdAsync(id);
        }


        //delete todo
        public async Task DeleteTodo(int id, string currentUserId)
        {
            //get the Todo Item based on id provided for delete
            TodoItem? existingTodo = await _todoRepo.GetByIdAsync(id);
            if (existingTodo == null)
            {
                throw new Exception($"No existing Todo with Id:{id}");
            }

            //check user trying to delete is authorized
            if (!CheckValidUser(currentUserId, existingTodo))
                throw new Exception("Current User is not authorized to update this todo item.");


            await _todoRepo.DeleteAsync(existingTodo);
        }


        //Paginated results
        public async Task<PaginatedResponseDTO<TodoDTO>> GetTodosForUser(string currentUserId, int page, int limit)
        {
            var todos = await _todoRepo.GetAllAsync();

            var currUserTodos = todos.Where(t => t.UserId == currentUserId).ToList();
            int total = currUserTodos.Count();

            if (total == 0)
            {
                return new PaginatedResponseDTO<TodoDTO>()
                {
                    Data = Enumerable.Empty<TodoDTO>(),
                    Page = page,
                    Limit = limit,
                    TotalRecords = total,
                    TotalPages = 0
                };
            }

            var data = currUserTodos.Skip((page - 1) * limit).Take(limit).ToList();

            var dataList = data.Select(x => x.ToDTO());

            return new PaginatedResponseDTO<TodoDTO>()
            {
                Data = dataList,
                Page = page,
                Limit = limit,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling((double)total / limit)
            };

        }

        //Admin Paginated Results with next and prev urls
        public async Task<PaginatedResponseDTO<TodoDTO>> GetTodosForAdmin(int page, int limit)
        {
            var todos = await _todoRepo.GetAllAsync();

            var data = todos.Skip((page - 1) * limit).Take(limit).ToList();
            var todoPagedData = data.Select(x => x.ToDTO()).ToList();
            var todoPagedDataTotal = todos.Count();

            if (todoPagedDataTotal == 0)
            {
                return new PaginatedResponseDTO<TodoDTO>()
                {
                    Data = Enumerable.Empty<TodoDTO>(),
                    Page = page,
                    Limit = limit,
                    TotalRecords = todoPagedDataTotal,
                    TotalPages = 0
                };
            }

            return new PaginatedResponseDTO<TodoDTO>
            {
                Data = todoPagedData,
                Page = page,
                Limit = limit,
                TotalRecords = todoPagedDataTotal,
                TotalPages = (int)Math.Ceiling((double)todos.Count() / limit)
            };

        }
    }
}
