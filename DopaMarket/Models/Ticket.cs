using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public enum TicketStatus
    {
        Open,
        Close
    }

    public enum TicketType
    {
        WebsiteProblem,
        InfoInquiry,
        Complaint
    }

    public enum TicketPriority
    {
        Low,
        Medium,
        Hight,
        Urgent
    }

    public class Ticket
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public int PersonInChargeId { get; set; }

        public Customer PersonInCharge { get; set; }

        public DateTime Date { get; set; }

        public TicketStatus Status { get; set; }

        public TicketType Type { get; set; }

        public TicketPriority Priority { get; set; }

        public string Subject { get; set; }
    }
}