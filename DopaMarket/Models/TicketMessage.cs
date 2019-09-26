using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class TicketMessage
    {
        public int Id { get; set; }

        public int ticketId { get; set; }
        public Ticket Ticket { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }
    }
}