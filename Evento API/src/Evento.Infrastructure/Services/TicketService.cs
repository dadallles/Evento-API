using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Evento.Core.Repositories;
using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using NLog;

namespace Evento.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private IUserRepository _userRepository;
        private IEventRepository _eventRepository;
        private IMapper _mapper;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public TicketService(IUserRepository userRepository, IEventRepository eventRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketDetailsDto>> GetForUserAsync(Guid userId)
        {
            Logger.Info("Catch ticket fo user");
            var user = await _userRepository.GetOrFailAsync(userId);
            var events = await _eventRepository.BrowseAsync();
            var alltickets = new List<TicketDetailsDto>();
            foreach (var @event in events)
            {
                var tickets = _mapper.Map<IEnumerable<TicketDetailsDto>>(@event.GetTicketsPurchasedByUser(user)).ToList();
                tickets.ForEach(x =>
                {
                    x.EventId = @event.Id;
                    x.EventName = @event.Name;
                });
                alltickets.AddRange(tickets);
            }

            return alltickets;

        }

        public async Task<TicketDto> GetAsync(Guid userId, Guid eventId, Guid ticketId)
        {
            Logger.Info("Catch ticket");
            var user = await _userRepository.GetOrFailAsync(userId);
            var ticket = await _eventRepository.GetTicketOrFailAsync(eventId, ticketId);

            return _mapper.Map<TicketDto>(ticket);
        }

        public async Task PurchaseAsync(Guid userId, Guid eventId, int amount)
        {
            Logger.Info("Purchase tickets");
            var user = await _userRepository.GetOrFailAsync(userId);
            var @event = await _eventRepository.GetOrFailAsync(eventId);
            @event.PurchaseTickets(user, amount);
            await _eventRepository.UpdateAsync(@event);
        }

        public async Task CancelAsync(Guid userId, Guid eventId, int amount)
        {
            Logger.Info("Cancel tickets");
            var user = await _userRepository.GetOrFailAsync(userId);
            var @event = await _eventRepository.GetOrFailAsync(eventId);
            @event.CancelPurchasedTickets(user, amount);
            await Task.CompletedTask;
        }

    }
}
