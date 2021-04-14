using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class ActiveDirectoryConnectivityConfigDCStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "ADConnectivityConfigDC Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "ADConnectivityConfigDC";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-ADConnectivityConfigDC_";
			}
		}
	}
}
