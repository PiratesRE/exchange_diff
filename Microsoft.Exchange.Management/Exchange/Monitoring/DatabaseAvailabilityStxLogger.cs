using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class DatabaseAvailabilityStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "DatabaseAvailability Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "Store";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "DatabaseAvailability_";
			}
		}
	}
}
