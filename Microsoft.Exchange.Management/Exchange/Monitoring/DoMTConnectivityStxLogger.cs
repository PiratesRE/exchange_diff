using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class DoMTConnectivityStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "DoMTConnectivity Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "DoMTConnectivity";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "DoMTConnectivity_";
			}
		}
	}
}
