using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LazyIndexing;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class DropAllLazyIndexesTask : IntegrityCheckTaskBase
	{
		public DropAllLazyIndexesTask(IJobExecutionTracker tracker) : base(tracker)
		{
		}

		public override string TaskName
		{
			get
			{
				return "DropAllLazyIndexes";
			}
		}

		public override ErrorCode Execute(Context context, MailboxEntry mailboxEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<Guid, bool>(0L, "DropAllLazyIndexesTask.Execute invoked on mailbox {0}, detect only = {1}", mailboxEntry.MailboxGuid, detectOnly);
			}
			if (!shouldContinue())
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
				}
				return ErrorCode.CreateExiting((LID)64956U);
			}
			ErrorCode errorCode = IntegrityCheckTaskBase.LockMailboxForOperation(context, mailboxEntry.MailboxNumber, delegate(MailboxState mailboxState)
			{
				ErrorCode noError;
				try
				{
					if (!detectOnly)
					{
						bool flag;
						LogicalIndexCache.CleanupLogicalIndexes(context, mailboxState, TimeSpan.Zero, out flag);
						if (!flag)
						{
							if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
							{
								ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Cleanup not completed");
							}
							return ErrorCode.CreateExiting((LID)45852U);
						}
					}
					noError = ErrorCode.NoError;
				}
				finally
				{
					context.Abort();
				}
				return noError;
			});
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<Guid, ErrorCode>(0L, "DropAllLazyIndexesTask.Execute finished on mailbox {0} with error code {1}", mailboxEntry.MailboxGuid, errorCode);
			}
			return errorCode;
		}
	}
}
