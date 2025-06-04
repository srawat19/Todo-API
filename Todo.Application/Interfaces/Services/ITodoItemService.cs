using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain.DTOs;
using Todo.Domain.Entities;

namespace Todo.Domain.Interfaces
{
    public interface ITodoItemService
    {
        public Task<TodoItem> CreateTodo(TodoItem todoItem);

        public Task<TodoItem?> UpdateTodo(TodoItem todoItem, int id, string currentUserId);

        public Task<TodoItem?> GetTodo(int id);
        public Task DeleteTodo(int id, string currentUserId);
        public Task<PaginatedResponseDTO<TodoDTO>> GetTodosForUser(string currentUserId, int page, int limit);

        public Task<PaginatedResponseDTO<TodoDTO>> GetTodosForAdmin(int page, int limit);

    }
}
