using System;
using System.ComponentModel.DataAnnotations;

namespace DopaMarket.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required]
        public string Street { get; set; }

        public string Street2 { get; set; }

        [Required]
        public string City { get; set; }

        public string State { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Country { get; set; }
    }
}