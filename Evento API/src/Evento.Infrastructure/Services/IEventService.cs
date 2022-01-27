using Evento.Core.Domain;
using Evento.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Evento.Infrastructure.Services
{
    public interface IEventService
    {
        Task<EventDetailsDto> GetAsync(Guid id);
        Task<EventDetailsDto> GetAsync(string name);
        Task<IEnumerable<EventDto>> BrowseAsync(string name = null);
        Task CreateAsync(Guid id, string name, string description, DateTime startDate, DateTime endDate);
        Task AddTicketAsync(Guid eventId, int amount, decimal price);
        Task UpdateAsync(Guid id, string name, string desription);
        Task DeleteAsync(Guid id);
    }
}
