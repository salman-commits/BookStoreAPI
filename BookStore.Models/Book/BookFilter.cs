using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.Book
{
    public class BookFilter
    {
        public int BookCategoryId { get; set; }
        public string SearchString { get; set; }
        public string SortColumn { get; set; }
        public bool SortTypeDesc { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
