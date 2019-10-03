using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class CompareGroupSpecification
    {
        public int Id { get; set; }

        public int CompareGroupId { get; set; }
        public CompareGroup CompareGroup { get; set; }

        public int SpecificationId { get; set; }
        public Specification Specification { get; set; }
    }
}