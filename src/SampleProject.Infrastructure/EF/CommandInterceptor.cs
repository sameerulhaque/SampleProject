using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Infrastructure.EF
{
    public class CommandInterceptor : DbCommandInterceptor
    {
        private const int ThresholdMilliseconds = 500;

        public override async ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Duration.TotalMilliseconds > ThresholdMilliseconds)
            {
                Debug.WriteLine($"Duration: {eventData.Duration.TotalMilliseconds} ms");
                Debug.WriteLine($"SQL: {command.CommandText}");
                foreach (DbParameter p in command.Parameters)
                    Debug.WriteLine($"{p.ParameterName} = {p.Value}");
            }

            return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }
    }
}
