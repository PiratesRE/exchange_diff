using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class ForwardSyncCompanyResponderStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "ForwardSyncCompanyResponderStxLogger Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "ForwardSyncCompanyResponder";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-ForwardSyncCompanyResponder_";
			}
		}
	}
}
