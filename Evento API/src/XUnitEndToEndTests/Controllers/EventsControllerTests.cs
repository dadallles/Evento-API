using Evento.Api;
using Evento.Infrastructure.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace XUnitEndToEndTests.Controllers
{
    public class EventsControllerTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public EventsControllerTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task fetching_events_should_return_not_empty_collection()
        {
            var response = await _client.GetAsync("events");
            var content = await response.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<IEnumerable<EventDto>>(content);

            response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.OK);
            events.Should().NotBeEmpty();
        }
    }

}
