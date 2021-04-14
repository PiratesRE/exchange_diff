using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class RidSetMonitorLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "RidSetMonitor Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "RidSetMonitor";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-RidSetMonitor_";
			}
		}
	}
}
