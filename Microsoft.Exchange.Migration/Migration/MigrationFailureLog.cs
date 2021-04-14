using System;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	internal static class MigrationFailureLog
	{
		public static void LogFailureEvent(MigrationJob migrationObject, Exception failureException, MigrationFailureFlags failureFlags = MigrationFailureFlags.None, string failureContext = null)
		{
			if (migrationObject != null && failureException != null)
			{
				FailureEvent failureEvent = new FailureEvent(migrationObject.JobId, "MigrationJob", (int)failureFlags, failureContext);
				CommonFailureLog.LogCommonFailureEvent(failureEvent, failureException);
			}
		}

		public static void LogFailureEvent(MigrationJobItem migrationObject, Exception failureException, MigrationFailureFlags failureFlags = MigrationFailureFlags.None, string failureContext = null)
		{
			if (migrationObject != null && failureException != null)
			{
				FailureEvent failureEvent = new FailureEvent(migrationObject.JobItemGuid, "MigrationJobItem", (int)failureFlags, failureContext);
				CommonFailureLog.LogCommonFailureEvent(failureEvent, failureException);
			}
		}
	}
}
