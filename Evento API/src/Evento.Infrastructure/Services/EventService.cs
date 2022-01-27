using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Evento.Core.Domain;
using Evento.Core.Repositories;
using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using NLog;

namespace Evento.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public EventService(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<EventDetailsDto> GetAsync(Guid id)
        {
            Logger.Info("Catch event");
            var @event = await _eventRepository.GetAsync(id);
            return _mapper.Map<EventDetailsDto>(@event);
        }

        public async Task<EventDetailsDto> GetAsync(string name)
        {
            Logger.Info("Catch event");
            var @event = await _eventRepository.GetAsync(name);
            return _mapper.Map<EventDetailsDto>(@event);
        }

        public async Task<IEnumerable<EventDto>> BrowseAsync(string name = null)
        {
            Logger.Info("Catching events");
            var events = await _eventRepository.BrowseAsync(name);
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task CreateAsync(Guid id, string name, string description, DateTime startDate, DateTime endDate)
        {
            Logger.Info("Create event");
            var @event = await _eventRepository.IsExistsAsync(name);
            @event = new Event(id, name, description, startDate, endDate);
            await _eventRepository.AddAsync(@event);
        }

        public async Task AddTicketAsync(Guid eventId, int amount, decimal price)
        {
            Logger.Info("Add ticket");
            var @event = await _eventRepository.GetOrFailAsync(@eventId);
            @event.AddTickets(amount, price);
            await _eventRepository.UpdateAsync(@event);
        }

        public async Task UpdateAsync(Guid id, string name, string desription)
        {
            Logger.Info("Update event");
            var @event = await _eventRepository.IsExistsAsync(name);
            @event = await _eventRepository.GetOrFailAsync(id);
            @event.SetName(name);
            @event.SetDescription(desription);
            await _eventRepository.UpdateAsync(@event);

        }

        public async Task DeleteAsync(Guid id)
        {
            Logger.Info("Delete events");
            var @event = await _eventRepository.GetOrFailAsync(id);
            await _eventRepository.DeleteAsync(@event);
        }

    }
}
