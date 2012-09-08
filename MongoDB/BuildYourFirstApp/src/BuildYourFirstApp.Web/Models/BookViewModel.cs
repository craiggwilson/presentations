using BuildYourFirstApp.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildYourFirstApp.Web.Models
{
    public class BookViewModel
    {
        public Book Book { get; set; }

        public Author Author { get; set; }
    }
}