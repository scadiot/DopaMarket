﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class ItemBasket
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int Count { get; set; }
    }
}