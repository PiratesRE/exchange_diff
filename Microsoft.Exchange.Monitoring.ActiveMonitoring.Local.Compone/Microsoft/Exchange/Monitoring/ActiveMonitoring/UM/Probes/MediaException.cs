using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM.Probes
{
	public class MediaException : Exception
	{
		public MediaException(string message) : base(message)
		{
		}
	}
}
