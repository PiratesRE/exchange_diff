using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class LiveIdAuthenticationStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "LiveIdAuthentication Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "LiveIdAuthentication";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-LiveIdAuthentication_";
			}
		}
	}
}
