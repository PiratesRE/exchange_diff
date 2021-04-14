using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class GlobalLocatorServiceStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "GlobalLocatorService Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "GlobalLocatorService";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-GlobalLocatorService_";
			}
		}
	}
}
