﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildYourFirstApp.Web.Models
{
    public class BookViewModel
    {
        public ObjectId Id { get; set; }

        public string Title { get; set; }
    }
}