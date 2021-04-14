using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging.MailboxStatistics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxStatisticsLog : ObjectLog<MailboxStatisticsLogEntry>
	{
		private MailboxStatisticsLog(ObjectLogConfiguration configuration) : base(new MailboxStatisticsLogEntrySchema.MailboxStatisticsLogSchema(), configuration)
		{
		}

		public static ObjectLog<MailboxStatisticsLogEntry> CreateWithConfig(ObjectLogConfiguration config)
		{
			return new MailboxStatisticsLog(config);
		}
	}
}
