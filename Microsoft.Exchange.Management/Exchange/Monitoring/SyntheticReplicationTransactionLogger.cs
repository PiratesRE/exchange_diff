using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class SyntheticReplicationTransactionLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "SyntheticReplicationTransaction Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "SyntheticReplicationTransaction";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-SyntheticReplicationTransaction_";
			}
		}
	}
}
