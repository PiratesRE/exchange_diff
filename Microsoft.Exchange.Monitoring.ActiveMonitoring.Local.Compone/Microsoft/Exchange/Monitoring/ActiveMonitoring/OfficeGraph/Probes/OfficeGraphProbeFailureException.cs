using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OfficeGraph.Probes
{
	public class OfficeGraphProbeFailureException : Exception
	{
		public OfficeGraphProbeFailureException(string message) : base(message)
		{
		}

		public OfficeGraphProbeFailureException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
