using System;
using System.Collections.Generic;
using System.Text;

namespace Evento.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToTimestamp(this DateTime datetime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var time = datetime.Subtract(new TimeSpan(epoch.Ticks));

            return time.Ticks / 1000;
        }
    }
}
