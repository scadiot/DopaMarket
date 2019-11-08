using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DopaMarket.Controllers.Api
{
    [AllowCrossSiteJson]
    public class ItemController : ApiController
    {
        ApplicationDbContext _context;

        public ItemController()
        {
            _context = new ApplicationDbContext();
        }

        public class ItemLight
        {
            public string Name { get; set; }
            public string LinkName { get; set; }
        }

        public IEnumerable<ItemLight> GetAllItems()
        {
            return _context.Items.ToArray().Select(i => new ItemLight() { Name = i.Name, LinkName = i.LinkName});
        }
    }
}
