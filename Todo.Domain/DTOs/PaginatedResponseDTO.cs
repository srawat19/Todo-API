using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain.Entities;

namespace Todo.Domain.DTOs
{
    public class PaginatedResponseDTO<T>
    {

        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }


        public string? NextPageUrl { get; set; }
        public string? PrevPageUrl { get; set; }


    }
}
