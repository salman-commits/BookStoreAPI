using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.Book
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public Guid ISBN { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AuthorName { get; set; }
        public decimal Price { get; set; }
        public Nullable<DateTime> PublishedDate { get; set; }
        public Nullable<int> Rating { get; set; }
        public string ImageURL { get; set; }
        public string ImageThumbnailURL { get; set; }
        public Nullable<int> Quantity { get; set; }
        public int BookCategoryId { get; set; }
        public List<BookCategory> Categories { get; set; }
    }
}
