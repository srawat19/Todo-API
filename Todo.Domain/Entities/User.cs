using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Domain.Entities
{
    public class User
    {
        public string UserObjectId { get; set; } //this will come from Azure AD, claim name "oid"

        public string DisplayName { get; set; }

        public string Email { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
