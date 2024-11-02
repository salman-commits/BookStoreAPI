using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Book
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ISBN is required")]
        public Guid ISBN { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot be more than 200 characters")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Author Name is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Author Name should contains alphabet only without any special characters.")]
        [StringLength(50)]
        public string AuthorName { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency)]
        [Range(0.01, 500, ErrorMessage = "Price can be between 0.01 to 500")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Published Date is required")]
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; }
        public int? Rating { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        public string ImageURL { get; set; }

        public string ImageThumbnailURL { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1,100, ErrorMessage = "Quantity should be between 1 to 100")]
        public int Quantity { get; set; }

        [Range(1, 50, ErrorMessage = "Book Category is required.")]
        public int BookCategoryId { get; set; }
        public BookCategory BookCategory { get; set; }

        [Required(ErrorMessage = "Created Date is required")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Created By is required")]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        public Nullable<DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
