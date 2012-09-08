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
    }
}