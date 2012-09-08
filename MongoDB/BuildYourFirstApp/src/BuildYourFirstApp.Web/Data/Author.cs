using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildYourFirstApp.Web.Data
{
    public class Author
    {
        public ObjectId Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}