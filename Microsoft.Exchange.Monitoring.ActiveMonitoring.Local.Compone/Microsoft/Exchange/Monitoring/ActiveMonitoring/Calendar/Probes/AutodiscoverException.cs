using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Calendar.Probes
{
	public class AutodiscoverException : Exception
	{
		public AutodiscoverException(string message) : base(message)
		{
		}
	}
}
