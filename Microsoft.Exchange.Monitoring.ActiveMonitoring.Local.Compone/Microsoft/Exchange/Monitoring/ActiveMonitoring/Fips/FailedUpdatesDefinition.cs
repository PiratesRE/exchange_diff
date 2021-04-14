using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips
{
	public class FailedUpdatesDefinition
	{
		public int NumberOfFailedEngine { get; set; }

		public int ConsecutiveFailures { get; set; }
	}
}
