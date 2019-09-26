using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class CustomerTicketsViewModel
    {
        public IEnumerable<Models.Ticket> Tickets { get; set; }
    }
}