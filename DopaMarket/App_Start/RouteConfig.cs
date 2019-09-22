using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DopaMarket
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ItemPushReview",
                url: "ItemAction/PushReview",
                defaults: new { controller = "Item", action = "PushReview" });

            routes.MapRoute(
                name: "ItemDetail",
                url: "Item/{linkName}",
                defaults: new { controller = "Item", action = "Detail" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
