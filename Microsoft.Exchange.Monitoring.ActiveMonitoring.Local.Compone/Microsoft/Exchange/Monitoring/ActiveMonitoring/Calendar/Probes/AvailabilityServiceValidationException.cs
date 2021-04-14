using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Calendar.Probes
{
	public class AvailabilityServiceValidationException : Exception
	{
		public AvailabilityServiceValidationException(string message) : base(message)
		{
		}
	}
}
