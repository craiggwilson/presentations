using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BuildYourFirstApp.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "NewBook",
                url: "books/new",
                defaults: new { controller = "Books", action="New" });

            routes.MapRoute(
                name: "NewBookReview",
                url: "books/{title}/notes/new",
                defaults: new { controller = "Books", action = "NewReview" });

            routes.MapRoute(
                name: "ViewBook",
                url: "books/{title}",
                defaults: new { controller = "Books", action="View" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}