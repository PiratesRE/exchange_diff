using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class OfflineGLSStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "OfflineGLS Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "OfflineGLS";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-OfflineGLS_";
			}
		}
	}
}
