using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain.DTOs;
using Todo.Domain.Entities;

namespace Todo.Domain.MapperExtensions
{
    public static class Mapper
    {
        public static TodoItem ToDomainEntity(this TodoDTO dto)
        {
            return new TodoItem
            {
                Title = dto.Title,
                Description = dto.Description,
            };
        }



        public static TodoDTO ToDTO(this TodoItem dto)
        {
            return new TodoDTO
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
            };
        }
    }
}
