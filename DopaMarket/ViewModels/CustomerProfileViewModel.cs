﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class CustomerProfileViewModel
    {
        public Models.Customer Customer { get; set; }
        public string Email { get; set; }
    }
}