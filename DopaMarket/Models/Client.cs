using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        public string IdentityUserId { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime? Birthday { get; set; }

        public string PhoneNumber { get; set; }

        public int? AddressId { get; set; }
    }
}