using BuildYourFirstApp.Web.Data;
using BuildYourFirstApp.Web.Models;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildYourFirstApp.Web.Controllers
{
    public class BooksController : MongoController
    {
        public ActionResult Index()
        {
            var model = new BookListViewModel
            {
                Books = Books.FindAll()
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(NewBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var author = Authors.FindOne(Query<Author>.Where(a => a.FirstName == model.AuthorFirstName && a.LastName == model.AuthorLastName));

            if (author == null)
            {
                author = new Author
                {
                    FirstName = model.AuthorFirstName,
                    LastName = model.AuthorLastName,
                };

                Authors.Save(author);
            }

            var book = Books.FindOne(Query<Book>.Where(b => b.Isbn == model.Isbn));
            if (book == null)
            {
                book = new Book();
            }
            
            book.Author = author.Id;
            book.Available = model.Available;
            book.Isbn = model.Isbn;
            book.Pages = model.Pages;
            book.Publisher = new BookPublisher
            {
                PublisherName = model.PublisherName,
                PublisherCity = model.PublisherCity
            };
            book.Title = model.Title;

            Books.Save(book);

            return RedirectToAction("View", new { title = book.Title });
        }

        [HttpPost]
        public ActionResult NewNote(string title, string note)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                return RedirectToAction("Index");
            }
            if(string.IsNullOrWhiteSpace(note))
            {
                return RedirectToAction("View", new { title = title });
            }

            var query = Query<Book>.Where(b => b.Title == title);
            var update = Update<Book>.Push(x => x.Notes, new BookNote { Username = User.Identity.Name, Note = note });

            Books.Update(query, update);

            return RedirectToAction("View", new { title = title });
        }

        public ActionResult View(string title)
        {
            var book = Books.FindOne(Query<Book>.Where(b => b.Title == title));
            if (book == null)
            {
                return RedirectToAction("Index");
            }

            var author = Authors.FindOne(Query<Author>.Where(a => a.Id == book.Author));

            return View(new BookViewModel
            {
                Book = book,
                Author = author
            });
        }
    }
}