using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class ForwardFullSyncStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "ForwardFullSyncProbe Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "ForwardSyncFullProbe";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-ForwardFullSync_";
			}
		}
	}
}
