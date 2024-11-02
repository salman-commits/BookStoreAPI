using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.Book
{
    public class BookCategory
    {
        public int BookCategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string BookCategoryName { get; set; }
    }
}
