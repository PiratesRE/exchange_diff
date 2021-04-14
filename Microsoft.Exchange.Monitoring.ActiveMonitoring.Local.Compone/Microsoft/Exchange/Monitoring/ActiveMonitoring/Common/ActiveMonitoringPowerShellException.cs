using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class ActiveMonitoringPowerShellException : Exception
	{
		public ActiveMonitoringPowerShellException(string message) : base(message)
		{
		}
	}
}
