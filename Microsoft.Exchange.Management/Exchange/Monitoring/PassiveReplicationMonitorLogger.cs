using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class PassiveReplicationMonitorLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "PassiveReplicationMonitor Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "PassiveReplicationMonitor";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-PassiveReplicationMonitor_";
			}
		}
	}
}
