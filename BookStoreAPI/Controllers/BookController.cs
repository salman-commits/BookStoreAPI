using BookStore.Models.Book;
using BookStore.Repository.Repository;
using BookStore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Cors;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using BookStoreAPI.Filters;
using System.Data.Entity.Validation;
using System.Text;

namespace BookStoreAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/BookStore")]
    [ValidateModel]
    public class BookController : ApiController
    {
        readonly GenericRepository<Book> bookRepo = new GenericRepository<Book>();
        readonly BookService bookService = new BookService();

        [Authorize]
        [Route("GetAllBook")]
        [HttpPost]
        //api/BookStore/GetAllBook
        public HttpResponseMessage GetAllBook([FromBody] BookFilter bookFilter)
        {
            int bookTotalCount = 0;
            try
            {
                List<Book> books = bookService.GetAllBooks(bookFilter, ref bookTotalCount);

                BookPaginationDto bookResult = new BookPaginationDto
                {
                    TotalItems = bookTotalCount,
                    Books = books,
                    TotalPages = bookFilter.PageSize,
                    CurrentPage = bookFilter.PageNumber
                };
                return Request.CreateResponse(HttpStatusCode.OK, bookResult, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Manager, Admin")]
        [Route("CreateUpdateBook")]
        [HttpPost]
        public HttpResponseMessage CreateUpdateBook()
        {
            Book newBook = null;
            var requestFormPair = new Dictionary<string, string>();

            foreach (string key in HttpContext.Current.Request.Form.AllKeys)
            {
                string value = HttpContext.Current.Request.Form[key];
                if (key == "Id" && string.IsNullOrEmpty(value))
                    value = "0";

                requestFormPair.Add(key, value);
            }
            var jsSerializer = new JavaScriptSerializer();
            var serializedJson = jsSerializer.Serialize(requestFormPair);
            newBook = JsonConvert.DeserializeObject<Book>(serializedJson);

            try
            {
                if (ModelState.IsValid)
                {
                    var bookImageFile = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

                    if (bookImageFile != null && bookImageFile.ContentLength > 0)
                        SaveImage(newBook, bookImageFile);

                    if (newBook.Id > 0)
                    {
                        // delete old image before updating
                        string folderName = HttpContext.Current.Server.MapPath("~/Images/");
                        Book existingBook = bookService.GetBookById(newBook.Id);
                        DeleteFile(folderName + existingBook.ImageURL);
                        DeleteFile(folderName + existingBook.ImageThumbnailURL);

                        newBook.ModifiedBy = newBook.CreatedBy;
                        newBook.ModifiedDate = DateTime.Now;
                        newBook.CreatedDate = existingBook.CreatedDate;
                        bookRepo.Update(newBook);
                    }
                    else
                    {
                        newBook.CreatedDate = DateTime.Now;
                        bookRepo.Insert(newBook);
                    }
                }
            }
            catch (Exception ex)
            {
                var innerException = ex as DbEntityValidationException;
                if (innerException != null)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine();
                    sb.AppendLine();
                    foreach (var eve in innerException.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().FullName, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                ve.PropertyName,
                                eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                ve.ErrorMessage));
                        }
                    }
                    sb.AppendLine();

                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, sb.ToString());
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
            return Request.CreateResponse(HttpStatusCode.Created, newBook, Configuration.Formatters.JsonFormatter);
        }

        [Authorize(Roles = "Manager, Admin,User")]
        [Route("GetBookById/{id}")]
        [HttpGet]
        //api/BookStore/GetBookById/10
        public HttpResponseMessage GetBookById(int id)
        {
            try
            {
                Book book = bookService.GetBookById(id);

                if (book == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Book not found");

                book.ImageURL = @"Images\" + book.ImageURL;
                book.ImageThumbnailURL = @"Images\" + book.ImageThumbnailURL;
                return Request.CreateResponse(HttpStatusCode.OK, book, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Manager, Admin")]
        [Route("EditBookById/{id}")]
        [HttpGet]
        //api/BookStore/EditBookById/10
        public HttpResponseMessage EditBookById(int id)
        {
            try
            {
                BookViewModel book = bookService.GetBookViewModelForEditById(id);
                return Request.CreateResponse(HttpStatusCode.OK, book, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Manager, Admin")]
        [Route("GetBookViewModelForCreate")]
        [HttpGet]
        //api/BookStore/GetBookViewModelForCreate
        public HttpResponseMessage GetBookViewModelForCreate()
        {
            try
            {
                BookViewModel book = bookService.GetBookViewModelForCreate();
                return Request.CreateResponse(HttpStatusCode.OK, book, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("DeleteBookById/{id}")]
        [HttpDelete]
        //api/BookStore/DeleteBookById/10
        public HttpResponseMessage DeleteBookById(int id)
        {
            try
            {
                //delete image if exist
                Book book = bookService.GetBookById(id);
                if (book != null)
                {
                    string folderName = HttpContext.Current.Server.MapPath("~/Images/");
                    DeleteFile(folderName + book.ImageURL);
                    DeleteFile(folderName + book.ImageThumbnailURL);
                    bookRepo.Delete(id);
                }
                return Request.CreateResponse(HttpStatusCode.OK, book, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Manager, Admin, User")]
        [Route("GetImageById/{id}")]
        [HttpGet]
        //api/BookStore/GetImageById/10
        public HttpResponseMessage GetImageById(int id)
        {
            try
            {
                Book book = bookService.GetBookById(id);
                var folderPath = HttpContext.Current.Server.MapPath("~/Images");
                var filePath = folderPath + "/" + book.ImageURL;
                FileInfo fileImage = new FileInfo(filePath);
                if (fileImage.Exists)
                {
                    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                    var stream = new FileStream(filePath, FileMode.Open);
                    result.Content = new StreamContent(stream);
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(filePath);
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue(Path.GetExtension(filePath) == "jpg" ? "image/jpeg" : "image/png");
                    result.Content.Headers.ContentLength = stream.Length;

                    return result;
                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Book Image Not Found");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [Authorize(Roles = "Manager, Admin, User")]
        [Route("GetBookCategories")]
        [HttpGet]
        //api/BookStore/GetBookCategories
        public HttpResponseMessage GetBookCategories()
        {
            try
            {
                var bookCategories = bookService.GetBookCategoryList();
                return Request.CreateResponse(HttpStatusCode.OK, bookCategories, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        Book SaveImage(Book book, HttpPostedFile bookEditFile)
        {
            try
            {
                var allowedFileExtensions = new[] { ".png", ".jpg" };

                var fileExtension = Path.GetExtension(bookEditFile.FileName); //getting the extension(ex-.jpg)  
                if (allowedFileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase)) //check what type of extension
                {
                    string folderName = HttpContext.Current.Server.MapPath("~/Images/");
                    string name = Path.GetFileNameWithoutExtension(bookEditFile.FileName);
                    string fileName = name + "_" + DateTime.Now.ToString("MMddyyy_hhmmss");

                    string fileWithExtension = fileName + fileExtension;
                    string thumbfileWithExtension = "thumbnail_" + fileName + fileExtension;
                    var path = folderName + fileWithExtension;
                    bookEditFile.SaveAs(path);

                    // create an image object, using the filename we just retrieved
                    System.Drawing.Image image = System.Drawing.Image.FromFile(path);

                    // create the actual thumbnail image
                    System.Drawing.Image thumbnailImage = image.GetThumbnailImage(100, 100, null, IntPtr.Zero);

                    // put the image into folder
                    thumbnailImage.Save(folderName + thumbfileWithExtension);

                    book.ImageURL = fileWithExtension;
                    book.ImageThumbnailURL = thumbfileWithExtension;

                    thumbnailImage.Dispose();
                    image.Dispose();
                }
                return book;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void DeleteFile(string filePath)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                if (fi.Exists)
                    fi.Delete();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}