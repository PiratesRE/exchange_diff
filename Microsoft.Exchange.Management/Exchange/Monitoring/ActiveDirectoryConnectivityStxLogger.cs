using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class ActiveDirectoryConnectivityStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "ADConnectivity Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "ADConnectivity";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-ADConnectivity_";
			}
		}
	}
}
