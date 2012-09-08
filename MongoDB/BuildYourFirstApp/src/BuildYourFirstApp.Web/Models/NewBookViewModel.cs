using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildYourFirstApp.Web.Models
{
    public class NewBookViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Isbn { get; set; }

        public int Pages { get; set; }

        [Required]
        public string AuthorFirstName { get; set; }

        [Required]
        public string AuthorLastName { get; set; }

        [Required]
        public string PublisherCity { get; set; }

        [Required]
        public string PublisherName { get; set; }

        public bool Available { get; set; }
    }
}