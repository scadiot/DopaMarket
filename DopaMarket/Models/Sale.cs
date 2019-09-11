using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class Sale
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public DateTime InsertDate { get; set; }
    }
}