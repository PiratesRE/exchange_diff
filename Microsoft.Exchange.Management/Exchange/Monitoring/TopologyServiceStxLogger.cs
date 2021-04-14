using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class TopologyServiceStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "TopologyService Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "TopologyService";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-TopologyService_";
			}
		}
	}
}
