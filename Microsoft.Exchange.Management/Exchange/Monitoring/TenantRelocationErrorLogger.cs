using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class TenantRelocationErrorLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "TenantRelocationErrorMonitor Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "TenantRelocationErrorMonitor";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-TenantRelocationErrorMonitor_";
			}
		}
	}
}
