using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class SyntheticReplicationMonitorLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "SyntheticReplicationMonitor Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "SyntheticReplicationMonitor";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-SyntheticReplicationMonitor_";
			}
		}
	}
}
