using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Domain.DTOs
{
    public class UserDto
    {
        [Required]
        public string Id { get; set; } //unique user obj id

        [Required]
        public string DisplayName { get; set; }


        [Required]
        public string Email { get; set; }
    }
}
