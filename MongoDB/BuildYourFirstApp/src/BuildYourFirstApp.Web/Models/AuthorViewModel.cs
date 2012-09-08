using BuildYourFirstApp.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildYourFirstApp.Web.Models
{
    public class AuthorViewModel
    {
        public Author Author { get; set; }

        public List<Book> Books { get; set; }
    }
}