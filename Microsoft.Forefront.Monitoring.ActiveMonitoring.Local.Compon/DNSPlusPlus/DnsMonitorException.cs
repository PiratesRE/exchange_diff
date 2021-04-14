using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	public class DnsMonitorException : Exception
	{
		public DnsMonitorException()
		{
		}

		public DnsMonitorException(Exception innerException) : base(null, innerException)
		{
		}

		public DnsMonitorException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
