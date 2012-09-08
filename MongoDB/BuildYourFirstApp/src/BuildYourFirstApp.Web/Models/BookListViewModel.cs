using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildYourFirstApp.Web.Models
{
    public class BookListViewModel
    {
        public IEnumerable<BookViewModel> Books { get; set; }
    }
}