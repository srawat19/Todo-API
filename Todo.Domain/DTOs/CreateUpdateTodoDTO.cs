using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Domain.DTOs
{
    public class CreateUpdateTodoDTO
    {

        [Required]
        public string Title { get; set; }


        [Required]
        public string Description { get; set; }
    }
}
