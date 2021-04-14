using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class RidMonitorLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "RidMonitor Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "RidMonitor";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-RidMonitor_";
			}
		}
	}
}
