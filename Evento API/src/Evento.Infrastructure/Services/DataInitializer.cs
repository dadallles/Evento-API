using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Evento.Infrastructure.Services
{
    public class DataInitializer : IDataInitializer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IUserService _userService;
        private readonly IEventService _eventService;

        public DataInitializer(IUserService userService, IEventService eventService)
        {
            _userService = userService;
            _eventService = eventService;
        }

        public async Task SeedAsync()
        {
            Logger.Info("Inicjalizacja danych...");
            var tasks = new List<Task>();
            tasks.Add(_userService.RegisterAsync(Guid.NewGuid(), "user@email.com", "default", "secret"));
            tasks.Add(_userService.RegisterAsync(Guid.NewGuid(), "admin@email.com", "default", "secret", "admin"));
            Logger.Info("Utworzeni: user, admin");
            for (var i = 0; i < 11; i++)
            {
                var eventId = Guid.NewGuid();
                var name = $"Wydarzenie {i}";
                var description = $"{name} opis.";
                var startDate = DateTime.UtcNow.AddHours(3);
                var endDate = startDate.AddHours(2);
                tasks.Add(_eventService.CreateAsync(eventId, name, description, startDate, endDate));
                tasks.Add(_eventService.AddTicketAsync(eventId, 1000, 100));
                Logger.Info($"Utworzono: {name}");
            }
            await Task.WhenAll(tasks);
            Logger.Info("Zainicjalzowano dane wstepne.");
        }
    }
}
