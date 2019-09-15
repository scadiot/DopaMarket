using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class ClientFormViewModel
    {
        public Models.Client Client { get; set; }
        public Models.Address Address { get; set; }
    }
}