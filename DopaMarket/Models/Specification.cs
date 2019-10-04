using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public enum SpecificationType
    {
        Boolean,
        Interger,
        String,
        Decimal
    }

    public class Specification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SpecificationType Type { get; set; }
        public string Unity { get; set; }
        public string LongName { get; set; }
    }
}