using System.Web;
using System.Web.Optimization;

namespace DryAgentSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));



            bundles.Add(new ScriptBundle("~/bundles/jqgrid").Include(
                "~/Scripts/jquery.jqGrid.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                      "~/Scripts/umd/popper.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
                     "~/Content/bootstrap.css"));

           bundles.Add(new StyleBundle("~/Content/sitecss").Include(
                      "~/Content/site.css"));

            //Create bundle for jQueryUI  
            //js  
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                      "~/Scripts/jquery-ui-{version}.js",
                      "~/Scripts/i18n/grid.locale-en.js"));

            //css  
            bundles.Add(new StyleBundle("~/Content/cssjqryUi").Include(
                    "~/Content/themes/base/jquery-ui.css",
                    "~/Content/themes/base/core.css",
                    "~/Content/themes/base/jquery-ui.structure.css",
                    "~/Content/themes/base/jquery-ui.theme.css",
                    "~/Content/jquery.jqGrid/ui.jqgrid.css"));

            //Bootstrap Multiselect
            bundles.Add(new ScriptBundle("~/bundles/BootstrapMultiJs").Include(
                "~/Scripts/bootstrap-multiselect.js",
                "~/Scripts/bootstrap-multiselect-collapsible-groups.js"));

            bundles.Add(new StyleBundle("~/Content/BootstrapMultiCss").Include(
                "~/Content/bootstrap-multiselect.css"));

            //Datetimepicker
            bundles.Add(new ScriptBundle("~/bundles/JQueryDatetimejs").Include(
                //"~/Scripts/jquery.datetimepicker.js",
                "~/Scripts/jquery.datetimepicker.full.min.js"));

            bundles.Add(new StyleBundle("~/Content/JQueryDatetimecss").Include(
                "~/Content/css/jquery.datetimepicker.min.css"));
        }
    }
}
