using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	[Serializable]
	public class HighAvailabilityMAUtilityException : Exception
	{
		public HighAvailabilityMAUtilityException(string message) : base(message)
		{
		}
	}
}
