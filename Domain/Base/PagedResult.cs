using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base
{
    public class PagedResult<T>   where T : BaseEntity
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageItems { get; set; }

        public int TotalItems { get; set; }

        public ICollection<T> Items { get; set; }

    }

   
}
