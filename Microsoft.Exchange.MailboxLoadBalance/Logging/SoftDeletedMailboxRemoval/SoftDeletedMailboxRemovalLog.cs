using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging.SoftDeletedMailboxRemoval
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoftDeletedMailboxRemovalLog : ObjectLog<SoftDeletedMailboxRemovalLogEntry>
	{
		private SoftDeletedMailboxRemovalLog(ObjectLogConfiguration configuration) : base(new SoftDeletedMailboxRemovalLogEntrySchema.SoftDeletedMailboxRemovalLogEntryLogSchema(), configuration)
		{
		}

		public static object CreateWithConfig(ObjectLogConfiguration getLogConfig)
		{
			return new SoftDeletedMailboxRemovalLog(getLogConfig);
		}
	}
}
