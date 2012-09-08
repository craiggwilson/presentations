using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildYourFirstApp.Web.Data
{
    public class Book
    {
        public ObjectId Id { get; set; }

        public string Title { get; set; }

        public string Isbn { get; set; }

        public int Pages { get; set; }

        public BookPublisher Publisher { get; set; }

        public bool Available { get; set; }

        public ObjectId Author { get; set; }

        public List<BookNote> Notes { get; set; }
    }

    public class BookPublisher
    {
        public string PublisherName { get; set; }

        public string PublisherCity { get; set; }
    }

    public class BookNote
    {
        public string Username { get; set; }

        public string Note { get; set; }
    }
}