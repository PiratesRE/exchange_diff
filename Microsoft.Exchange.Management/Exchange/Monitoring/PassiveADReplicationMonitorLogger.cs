using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class PassiveADReplicationMonitorLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "PassiveADReplicationMonitor Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "PassiveADReplicationMonitor";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "PassiveADReplicationMonitor_";
			}
		}
	}
}
