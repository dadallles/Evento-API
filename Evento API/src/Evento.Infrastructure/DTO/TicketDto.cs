using System;
using System.Collections.Generic;
using System.Text;

namespace Evento.Infrastructure.DTO
{
    public class TicketDto
    {
        public Guid Id { get;  set; }

        public int Seating { get;  set; }

        public decimal Price { get;  set; }

        public DateTime? PurchasedAt { get;  set; }

        public bool Purchased { get;  set; }
    }
}
