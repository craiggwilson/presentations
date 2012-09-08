using BuildYourFirstApp.Web.Data;
using BuildYourFirstApp.Web.Models;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildYourFirstApp.Web.Controllers
{
    public class AuthorsController : MongoController
    {
        public ActionResult Index()
        {
            var model = new AuthorListViewModel
            {
                Authors = Authors.FindAll()
            };
            return View(model);
        }

        public ActionResult View(string name)
        {
            var lastSpace = name.LastIndexOf(' ');
            var firstName = name.Substring(0, lastSpace);
            var lastName = name.Substring(lastSpace + 1);

            var author = Authors.FindOne(Query<Author>.Where(a => a.FirstName == firstName && a.LastName == lastName));

            if (author == null)
            {
                return RedirectToAction("Index");
            }

            var books = Books.Find(Query<Book>.Where(b => b.Author == author.Id));

            var model = new AuthorViewModel
            {
                Author = author,
                Books = books.ToList()
            };

            return View(model);
        }
    }
}