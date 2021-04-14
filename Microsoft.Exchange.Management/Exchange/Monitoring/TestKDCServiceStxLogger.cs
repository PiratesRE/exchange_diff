using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class TestKDCServiceStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "TestKDCService Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "TestKDCService";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "TestKDCService_";
			}
		}
	}
}
