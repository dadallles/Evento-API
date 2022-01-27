using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Evento.Infrastructure.Services
{
    public interface IDataInitializer
    {
        Task SeedAsync();
    }
}
