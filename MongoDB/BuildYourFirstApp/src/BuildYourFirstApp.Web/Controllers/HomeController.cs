using BuildYourFirstApp.Web.Data;
using BuildYourFirstApp.Web.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildYourFirstApp.Web.Controllers
{
    public class HomeController : MongoController
    {
        public ActionResult Index()
        {
            var libraryModel = new LibraryViewModel
            {
                BookCount = GetBookCount(),
                UniqueAuthorCount = GetUniqueAuthorCount()
            };

            return View(libraryModel);
        }

        private int GetBookCount()
        {
            return (int)Books.Count();
        }

        private int GetUniqueAuthorCount()
        {
            return (int)Authors.Count();
        }
    }
}