using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct StopwatchRequestDetailsLogger : IDisposable
	{
		public StopwatchRequestDetailsLogger(RequestDetailsLogger logger, Enum marker)
		{
			ArgumentValidator.ThrowIfNull("logger", logger);
			ArgumentValidator.ThrowIfNull("marker", marker);
			this.logger = logger;
			this.marker = marker;
			this.stopwatch = Stopwatch.StartNew();
		}

		public void Dispose()
		{
			this.stopwatch.Stop();
			this.logger.Set(this.marker, this.stopwatch.ElapsedMilliseconds);
		}

		private readonly Enum marker;

		private readonly RequestDetailsLogger logger;

		private readonly Stopwatch stopwatch;
	}
}
