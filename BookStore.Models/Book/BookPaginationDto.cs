using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.Book
{
    public class BookPaginationDto
    {
        public int TotalItems { get; set; }
        public List<Book> Books { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
