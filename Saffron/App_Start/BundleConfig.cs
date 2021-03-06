﻿using System.Web;
using System.Web.Optimization;

namespace Saffron
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Vendor scripts
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.1.1.min.js"));

            // jQuery Validation
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            "~/Scripts/jquery.validate.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            // Inspinia script
            bundles.Add(new ScriptBundle("~/bundles/inspinia").Include(
                      "~/Scripts/app/inspinia.js"
                      , "~/Scripts/chartist.min.js"
                      , "~/Scripts/DataTables/jquery.dataTables.min.js"
                      , "~/Scripts/DataTables/dataTables.bootstrap.min.js"
                      , "~/Scripts/bootstrap-datepicker.min.js"
                      , "~/Scripts/locales/bootstrap-datepicker.en-GB.min.js"));

            // SlimScroll
            bundles.Add(new ScriptBundle("~/plugins/slimScroll").Include(
                      "~/Scripts/plugins/slimScroll/jquery.slimscroll.min.js"));

            // jQuery plugins
            bundles.Add(new ScriptBundle("~/plugins/metsiMenu").Include(
                      "~/Scripts/plugins/metisMenu/metisMenu.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/pace").Include(
                      "~/Scripts/plugins/pace/pace.min.js"));

            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/animate.css",
                      "~/Content/style.css"
                      ,"~/Content/chartist.min.css"
                      ,"~/Content/DataTables/jquery.dataTables.min.css"
                      , "~/Content/bootstrap-datepicker3.min.css"
                      , "~/Content/bootstrap-datepicker.min.css"
                      , "~/Content/Site.css"));

            // Font Awesome icons
            bundles.Add(new StyleBundle("~/font-awesome/css").Include(
                      "~/fonts/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));

            // Editable Datatables
            bundles.Add(new StyleBundle("~/DataTablesEditable").Include(
                       "~/Content/Datatables/css/buttons.dataTables.min.css"));
        }
    }
}
