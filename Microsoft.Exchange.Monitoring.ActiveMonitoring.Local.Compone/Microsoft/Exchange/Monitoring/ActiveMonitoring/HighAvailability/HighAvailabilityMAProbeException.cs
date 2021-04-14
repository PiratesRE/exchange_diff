using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	[Serializable]
	public class HighAvailabilityMAProbeException : Exception
	{
		public HighAvailabilityMAProbeException(string message) : base(message)
		{
		}
	}
}
