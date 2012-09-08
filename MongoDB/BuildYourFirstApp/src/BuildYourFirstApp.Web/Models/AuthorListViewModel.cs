using BuildYourFirstApp.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildYourFirstApp.Web.Models
{
    public class AuthorListViewModel
    {
        public IEnumerable<Author> Authors { get; set; }
    }
}