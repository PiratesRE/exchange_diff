using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	[Serializable]
	public class HighAvailabilityMAProbeRedResultException : Exception
	{
		public HighAvailabilityMAProbeRedResultException(string message) : base(message)
		{
		}
	}
}
