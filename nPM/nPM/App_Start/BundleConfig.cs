using System.Web;
using System.Web.Optimization;

namespace nPM
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));  

            #region CSS

            bundles.Add(new StyleBundle("~/Content/CustomCss/css").Include(
                        "~/Content/CustomCss/Common.css",
                        "~/Content/CustomCss/Home.css",
                        "~/Content/CustomCss/Login.css"));

            bundles.Add(new StyleBundle("~/Content/HelperCss/css").Include(
                        "~/Content/HelperCss/site.css",
                        "~/Content/HelperCss/toastr.css",
                        "~/Content/HelperCss/jquery.dataTables.css",
                        "~/Content/HelperCss/dropit.css"
                        //"~/Content/HelperCss/bootstrap.css",
                        //"~/Content/HelperCss/bootstrap.min.css",
                        //"~/Content/HelperCss/bootstrap-responsive.css",
                        //"~/Content/HelperCss/bootstrap-responsive.min.css",
                        //"~/Content/HelperCss/bootstrap-wysihtml5.css",
                        //"~/Content/HelperCss/bootstrap-wysihtml5-0.0.2.css",
                        //"~/Content/HelperCss/wysiwyg-color.css"
                        ));

            #endregion

            #region JS

            bundles.Add(new ScriptBundle("~/bundles/Scripts/CustomScripts").Include(
                    "~/Scripts/CustomScripts/Login.js",
                    "~/Scripts/CustomScripts/Home.js",
                    "~/Scripts/CustomScripts/Common.js",
                    "~/Scripts/CustomScripts/errorHandler.js"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/Scripts/HelperScripts").Include(
                   "~/Scripts/HelperScripts/toastr.js",
                   "~/Scripts/HelperScripts/jquery.dataTables.js",
                    "~/Scripts/HelperScripts/spin.js",
                    "~/Scripts/HelperScripts/pdfobject.js",
                    "~/Scripts/HelperScripts/Chart.js",
                    "~/Scripts/HelperScripts/highcharts.js",
                    "~/Scripts/HelperScripts/dopit.js",
                    "~/Scripts/HelperScripts/binaryajax.js",
                    "~/Scripts/HelperScripts/exif.js"
                    //"~/Scripts/HelperScripts/bootstrap.js",
                    //"~/Scripts/HelperScripts/bootstrap.min.js",
                    //"~/Scripts/HelperScripts/bootstrap-button.js",
                    //"~/Scripts/HelperScripts/bootstrap-wysihtml5.js",
                    //"~/Scripts/HelperScripts/prettify.js",
                    //"~/Scripts/HelperScripts/wysihtml5-0.3.0.js",
                    //"~/Scripts/HelperScripts/wysihtml5-0.3.0.min.js"
                    ));

            #region jquery ui

            bundles.Add(new ScriptBundle("~/bundles/Scripts/jquery-ui").Include(
                "~/Scripts/jquery-ui/jquery-ui.custom.js",
                "~/Scripts/jquery-ui/jquery.ui.accordion.js",
                "~/Scripts/jquery-ui/jquery.ui.autocomplete.js",
                "~/Scripts/jquery-ui/jquery.ui.core.js",
                "~/Scripts/jquery-ui/jquery.ui.datepicker.js",
                "~/Scripts/jquery-ui/jquery.ui.dialog.js",
                "~/Scripts/jquery-ui/jquery.ui.draggable.js",
                "~/Scripts/jquery-ui/jquery.ui.droppable.js",
                "~/Scripts/jquery-ui/jquery.ui.effect-blind.js",
                "~/Scripts/jquery-ui/jquery.ui.effect-bounce.js",
                "~/Scripts/jquery-ui/jquery.ui.effect-clip.js",
                "~/Scripts/jquery-ui/jquery.ui.effect-drop.js",
                "~/Scripts/jquery-ui/jquery.ui.effect-explode.js",
                "~/Scripts/jquery-ui/jquery.ui.effect-fade.js",
                "~/Scripts/jquery-ui/jquery.ui.effect-fold.js",
                "~/Scripts/jquery-ui/jquery.ui.effect-highlight.js",
                "~/Scripts/jquery-ui/jquery.ui.pulsate.js",
                "~/Scripts/jquery-ui/jquery.ui.effect-scale.js",
                "~/Scripts/jquery-ui/jquery.ui.effect-slide.js",
                "~/Scripts/jquery-ui/jquery.ui.efect-transfer.js",
                "~/Scripts/jquery-ui/jquery.ui.effect.js",
                "~/Scripts/jquery-ui/jquery.ui.menu.js",
                "~/Scripts/jquery-ui/jquery.ui.mouse.js",
                "~/Scripts/jquery-ui/jquery.ui.position.js",
                "~/Scripts/jquery-ui/jquery.ui.progressbar.js",
                "~/Scripts/jquery-ui/jquery.ui.resizable.js",
                "~/Scripts/jquery-ui/jquery.ui.selectable.js",
                "~/Scripts/jquery-ui/jquery.ui.slide.js",
                "~/Scripts/jquery-ui/jquery.ui.sortable.js",
                "~/Scripts/jquery-ui/jquery.ui.spinner.js",
                "~/Scripts/jquery-ui/jquery.ui.tabs.js",
                "~/Scripts/jquery-ui/jquery.ui.tooltip.js",
                "~/Scripts/jquery-ui/jquery.ui.widget.js",
                "~/Scripts/jquery-ui/jquery-1.9.1.js",
                "~/Scripts/jquery-ui/jquery-ui-1.10.3.custom.js",
                "~/Scripts/jquery-ui/jquery-ui-1.10.3.custom.min.js"
                ));

            #endregion

            #endregion

        }
    }
}