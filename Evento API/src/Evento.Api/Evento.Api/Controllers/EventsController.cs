using Evento.Infrastructure.Commands.Events;
using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evento.Api.Controllers
{
    [Route("[controller]")]
    public class EventsController : ApiControllerBaseController
    {
        private readonly IEventService _eventService;
        private readonly IMemoryCache _cache;

        public EventsController(IEventService eventService, IMemoryCache cache)
        {
            _eventService = eventService;
            _cache = cache;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string name)
        {
            //throw new ArgumentException("err");
            var events = _cache.Get<IEnumerable<EventDto>>("events");
            if (events == null)
            {
                events = await _eventService.BrowseAsync(name);
                _cache.Set("events", events, TimeSpan.FromMinutes(1));
            }

            return Json(events);
        }

        [HttpGet("{eventId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Guid eventId)
        {
            var @event = await _eventService.GetAsync(eventId);
            if(@event == null)
            {
                return NotFound();
            }

            return Json(@event);
        }

        [HttpPost]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<IActionResult> Post([FromBody]CreateEvent command)
        {
            command.EventId = Guid.NewGuid();
            await _eventService.CreateAsync(command.EventId, command.Name, command.Description, command.StartDate, command.EndDate);
            await _eventService.AddTicketAsync(command.EventId, command.Tickets, command.Price);

            return Created($"/events/{command.EventId}", null);
        }

        [HttpPut("{eventId}")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<IActionResult> Put(Guid eventId, [FromBody]UpdateEvent command)
        {
            command.EventId = Guid.NewGuid();
            await _eventService.UpdateAsync(command.EventId, command.Name, command.Description);

            return NoContent();
        }

        [HttpDelete("{eventId}")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            await _eventService.DeleteAsync(eventId);

            return NoContent();
        }

    }
}
