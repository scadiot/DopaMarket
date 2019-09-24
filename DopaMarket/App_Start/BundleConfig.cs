﻿using System.Web;
using System.Web.Optimization;

namespace DopaMarket
{
    public class BundleConfig
    {
        // Pour plus d'informations sur le regroupement, visitez https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilisez la version de développement de Modernizr pour le développement et l'apprentissage. Puis, une fois
            // prêt pour la production, utilisez l'outil de génération à l'adresse https://modernizr.com pour sélectionner uniquement les tests dont vous avez besoin.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/js")
                       .Include("~/Scripts/jquery-{version}.js")
                       .Include("~/Scripts/vendor.min.js")
                       .Include("~/Scripts/modernizr.min.js")
                       .Include("~/Scripts/card.min.js")
                       .Include("~/Scripts/scripts.min.js")
                       .Include("~/Scripts/dopamarket-scripts.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/styles.min.css",
                      "~/Content/vendor.min.css"));
        }
    }
}
