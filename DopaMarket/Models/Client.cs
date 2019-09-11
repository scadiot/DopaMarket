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
    }
}