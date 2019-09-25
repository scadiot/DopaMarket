using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class CustomerFormViewModel
    {
        public Models.Customer Customer { get; set; }
        public Models.Address Address { get; set; }
    }
}