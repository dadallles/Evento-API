using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evento.Core.Domain
{
    public class Event : Entity
    {
        private ISet<Ticket> _tickets = new HashSet<Ticket>();

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public DateTime StartDate { get; protected set; }

        public DateTime EndDate { get; protected set; }

        public DateTime UpdateAt { get; protected set; }

        public IEnumerable<Ticket> Tickets => _tickets;

        public IEnumerable<Ticket> PurchasedTickets => _tickets.Where(x => x.Purchased);

        public IEnumerable<Ticket> AvailableTickets => _tickets.Where(x => !x.Purchased);


        protected Event()
        {

        }

        public Event(Guid id, string name, string description, DateTime startDate, DateTime endDate)
        {
            Id = id;
            SetName(name);
            SetDescription(description);
            SetDates(startDate, endDate);
            CreatedAt = DateTime.UtcNow;
            UpdateAt = DateTime.UtcNow;
        }

        public void SetDates(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                throw new Exception($"Wydarzenie o id: {Id} ma złe daty.");
            }
            StartDate = startDate;
            EndDate = endDate;
        }

        public void SetName(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"Nazwa nie moze byc pusta");
            }
            Name = name;
            UpdateAt = DateTime.UtcNow;
        }

        public void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new Exception($"Opis nie moze byc pusty");
            }
            Description = description;
            UpdateAt = DateTime.UtcNow;
        }

        public void AddTickets(int amount, decimal price)
        {
            for(int i=0; i<amount; ++i)
            {
                var seating = _tickets.Count + 1;

                _tickets.Add(new Ticket(this, seating, price));
            }
        }

        public void PurchaseTickets(User user, int amount)
        {
            if(AvailableTickets.Count() < amount)
            {
                throw new Exception($"Nie ma dostepnych ({amount}) sztuk biletow!");
            }
            var tickets = AvailableTickets.Take(amount);
            foreach(var ticket in tickets)
            {
                ticket.Purchase(user);
            }
        }

        public void CancelPurchasedTickets(User user, int amount)
        {
            var tickets = GetTicketsPurchasedByUser(user);
            if(tickets.Count() < amount)
            {
                throw new Exception($"Nie wystarczajaca ilosc zakupionych biletow (({amount})) przez '{user.Name}'.");
            }
            foreach (var ticket in tickets.Take(amount))
            {
                ticket.Cancel();
            }
        }

        public IEnumerable<Ticket> GetTicketsPurchasedByUser(User user)
            => PurchasedTickets.Where(x => x.UserId == user.Id);

    }
}
