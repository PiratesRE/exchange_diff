using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class ForwardSyncCookieResponderStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "ForwardSyncCookieResponderStxLogger Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "ForwardSyncCookieResponder";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-ForwardSyncCookieResponder_";
			}
		}
	}
}
