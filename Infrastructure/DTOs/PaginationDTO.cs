using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int pageSize = 10;
        private readonly int maxPageSize = 50;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > maxPageSize ? maxPageSize : value); }
        }
    }
}
