using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class TrustMonitorProbeLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "TrustMonitorProbeLogger Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "TrustMonitorProbeLogger";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-TrustMonitorProbeLogger_";
			}
		}
	}
}
