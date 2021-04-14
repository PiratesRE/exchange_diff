using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class PassiveReplicationPerfCounterProbeLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "PassiveReplicationPerfCounterProbeLogger Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "PassiveReplicationPerfCounterProbeLogger";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-PassiveReplicationPerfCounterProbe_";
			}
		}
	}
}
