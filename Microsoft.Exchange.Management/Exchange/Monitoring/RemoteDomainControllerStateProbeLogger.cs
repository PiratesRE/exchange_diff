using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class RemoteDomainControllerStateProbeLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "RemoteDomainControllerStateProbeLogger Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "RemoteDomainControllerStateProbeLogger";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-RemoteDomainControllerStateProbeLogger_";
			}
		}
	}
}
