using Evento.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Evento.Infrastructure.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDetailsDto>> GetForUserAsync(Guid userId);
        Task<TicketDto> GetAsync(Guid userId, Guid eventId, Guid ticketId);
        Task PurchaseAsync(Guid userId, Guid eventId, int amount);
        Task CancelAsync(Guid userId, Guid eventId, int amount);
    }
}
