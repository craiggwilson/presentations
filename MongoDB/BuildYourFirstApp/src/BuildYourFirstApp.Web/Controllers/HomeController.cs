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
            var pipeline = new List<BsonDocument>();
            pipeline.Add(new BsonDocument("$project", new BsonDocument("author", 1)));
            pipeline.Add(new BsonDocument("$group",
                new BsonDocument
                {
                    { "_id", "$author" },
                    { "book_count", new BsonDocument("$sum", 1) }
                }));

            var authors = Authors.Aggregate(pipeline);
            return authors.ResultDocuments.Count();
        }
    }
}