using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class ItemReview
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public Item Item { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public int Note { get; set; }
    }
}