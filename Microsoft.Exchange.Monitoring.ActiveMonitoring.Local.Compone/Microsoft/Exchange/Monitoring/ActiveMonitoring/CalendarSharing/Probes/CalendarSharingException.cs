using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.CalendarSharing.Probes
{
	public class CalendarSharingException : Exception
	{
		public CalendarSharingException(string message) : base(message)
		{
		}
	}
}
