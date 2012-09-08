using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace BuildYourFirstApp.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Assets/css").Include(
                "~/Assets/bootstrap/css/bootstrap.css",
                "~/Assets/css/styles.css",
                "~/Assets/bootstrap/css/bootstrap-responsive.css"));

            bundles.Add(new ScriptBundle("~/Assets/js").Include(
                "~/Assets/bootstrap/js/jquery.js",
                "~/Assets/bootstrap/js/basic/bootstrap-transition.js",
                "~/Assets/bootstrap/js/basic/bootstrap-alert.js",
                "~/Assets/bootstrap/js/basic/bootstrap-modal.js",
                "~/Assets/bootstrap/js/basic/bootstrap-dropdown.js",
                "~/Assets/bootstrap/js/basic/bootstrap-scrollspy.js",
                "~/Assets/bootstrap/js/basic/bootstrap-tab.js",
                "~/Assets/bootstrap/js/basic/bootstrap-tooltip.js",
                "~/Assets/bootstrap/js/basic/bootstrap-popover.js",
                "~/Assets/bootstrap/js/basic/bootstrap-button.js",
                "~/Assets/bootstrap/js/basic/bootstrap-collapse.js",
                "~/Assets/bootstrap/js/basic/bootstrap-carousel.js",
                "~/Assets/bootstrap/js/basic/bootstrap-typeahead.js"));
        }
    }
}