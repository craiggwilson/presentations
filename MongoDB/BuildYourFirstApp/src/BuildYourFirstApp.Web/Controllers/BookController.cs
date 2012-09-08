using BuildYourFirstApp.Web.Models;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildYourFirstApp.Web.Controllers
{
    public class BookController : MongoController
    {
        public ActionResult Index()
        {
            var model = new BookListViewModel
            {
                Books = Books.AsQueryable().Select(b => new BookViewModel { Id = b.Id, Title = b.Title })
            };
            return View(model);
        }
        
    }
}