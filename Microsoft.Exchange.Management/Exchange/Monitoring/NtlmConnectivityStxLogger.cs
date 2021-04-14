using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class NtlmConnectivityStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "NtlmConnectivity Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "NtlmConnectivity";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-NtlmConnectivity_";
			}
		}
	}
}
