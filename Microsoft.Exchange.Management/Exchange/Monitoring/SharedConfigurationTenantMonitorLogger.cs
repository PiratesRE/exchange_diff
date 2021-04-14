using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class SharedConfigurationTenantMonitorLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "SharedConfigurationTenantMonitor Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "SharedConfigurationTenantMonitor";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-SharedConfigurationTenantMonitor_";
			}
		}
	}
}
