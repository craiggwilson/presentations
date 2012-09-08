using BuildYourFirstApp.Web.Data;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildYourFirstApp.Web.Models
{
    public class BookListViewModel
    {
        public IEnumerable<Book> Books { get; set; }
    }
}