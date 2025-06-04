using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain.Entities;

namespace Todo.Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepositoryBase<User, string>
    {

    }
}
