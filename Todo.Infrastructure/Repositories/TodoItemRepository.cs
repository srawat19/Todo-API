using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Application.Interfaces.Repositories;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Repositories
{
    public class TodoItemRepository : RepositoryBase<TodoItem, int>, ITodoItemRepository
    {
        public TodoItemRepository(TodoAppDbContext dbContext) : base(dbContext) { }

    }
}
