using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class SmtpConnectionProbeException : Exception
	{
		public SmtpConnectionProbeException(string message) : base(message)
		{
		}
	}
}
