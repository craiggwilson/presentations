using BuildYourFirstApp.Web.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildYourFirstApp.Web.Controllers
{
    public class MongoController : Controller
    {
        protected MongoCollection<Author> Authors { get; set; }

        protected MongoCollection<Book> Books { get; set; }

        protected MongoController()
        {
            var connectionString = ConfigurationManager.AppSettings["MongoDB"];
            var server = MongoServer.Create(connectionString);
            var database = server.GetDatabase("library");

            Authors = database.GetCollection<Author>("authors");
            Books = database.GetCollection<Book>("books");
        }
    }
}