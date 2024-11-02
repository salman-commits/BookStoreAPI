using BookStore.Models.Book;
using BookStore.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Services
{
    public class BookService
    {
        GenericRepository<Book> bookRepo = new GenericRepository<Book>();
        GenericRepository<BookCategory> bookCategoryRepo = new GenericRepository<BookCategory>();
        public List<Book> GetAllBooks(dynamic validFilter)
        {
            int skip = (validFilter.PageNumber - 1) * validFilter.PageSize;
            List<Book> books = bookRepo.GetAll().Skip(skip).Take((int)validFilter.PageSize).ToList();

            return books;
        }

        public List<Book> GetAllBooks(BookFilter bookFilter, ref int bookTotalCount)
        {
            List<Book> lstBook = null;

            if (!string.IsNullOrEmpty(bookFilter.SearchString) && bookFilter.BookCategoryId > 0)
                lstBook = bookRepo.Gets
                    (filter: s => s.Title.ToLower().Contains(bookFilter.SearchString.ToLower()) && s.BookCategoryId == bookFilter.BookCategoryId)
                    .ToList();

            else if (string.IsNullOrEmpty(bookFilter.SearchString) && bookFilter.BookCategoryId > 0)
                lstBook = bookRepo.Gets(filter: s => s.BookCategoryId == bookFilter.BookCategoryId).ToList();

            else if (!string.IsNullOrEmpty(bookFilter.SearchString) && bookFilter.BookCategoryId == 0)
                lstBook = bookRepo.Gets(filter: s => s.Title.ToLower().Contains(bookFilter.SearchString.ToLower())).ToList();

            else
                lstBook = bookRepo.GetAll().ToList();

            bookTotalCount = lstBook.Count();

            if (!string.IsNullOrEmpty(bookFilter.SortColumn))
            {
                var propertyInfo = typeof(Book).GetProperty(bookFilter.SortColumn);
                IOrderedEnumerable<Book> orderedBooks = null;
                if (bookFilter.SortTypeDesc)
                {
                    orderedBooks = lstBook.OrderByDescending(x => propertyInfo.GetValue(x, null));
                }
                else
                {
                    orderedBooks = lstBook.OrderBy(x => propertyInfo.GetValue(x, null));
                }

                return GetBookDetails(orderedBooks.ToList(), bookFilter);
            }
            return GetBookDetails(lstBook, bookFilter);
        }

        public List<Book> GetBookDetails(List<Book> books, BookFilter bookFilter)
        {
            bookFilter.PageNumber = bookFilter.PageNumber < 1 ? 1 : bookFilter.PageNumber;
            bookFilter.PageSize = bookFilter.PageSize > 10 ? 10 : bookFilter.PageSize;

            int skip = (bookFilter.PageNumber - 1) * bookFilter.PageSize;

            foreach (var book in books)
            {
                book.ImageURL = @"Images\" + book.ImageURL;
                book.ImageThumbnailURL = @"Images\" + book.ImageThumbnailURL;
            }

            return books.Skip(skip).Take(bookFilter.PageSize).ToList();
        }

        public Book GetBookById(int id)
        {
            return bookRepo.Get(b => b.Id == id, null, "BookCategory");
        }

        public BookViewModel GetBookViewModelForEditById(int id)
        {
            Book existingBook = bookRepo.GetById(id);
            var book = new BookViewModel()
            {
                Id = existingBook.Id,
                ISBN = existingBook.ISBN,
                Title = existingBook.Title,
                Description = existingBook.Description,
                AuthorName = existingBook.AuthorName,
                Rating = existingBook.Rating,
                PublishedDate = existingBook.PublishedDate,
                Price = existingBook.Price,
                ImageURL = existingBook.ImageURL,
                ImageThumbnailURL = existingBook.ImageThumbnailURL,
                Quantity = existingBook.Quantity,
                BookCategoryId = existingBook.BookCategoryId,
                Categories = GetBookCategoryList()
            };
            return book;
        }
        public BookViewModel GetBookViewModelForCreate()
        {
            BookViewModel existingBook = new BookViewModel();
            existingBook.ISBN = Guid.NewGuid();
            existingBook.Categories = GetBookCategoryList();

            return existingBook;
        }
        public List<BookCategory> GetBookCategoryList()
        {
            List<BookCategory> categories =
                bookCategoryRepo.GetAll().OrderBy(n => n.BookCategoryName).ToList();

            return categories;
        }
    }
}