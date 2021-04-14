using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class UniqueMidIndexCheckTask : IntegrityCheckTaskBase
	{
		public UniqueMidIndexCheckTask(IJobExecutionTracker tracker) : base(tracker)
		{
		}

		public override string TaskName
		{
			get
			{
				return "UniqueMidIndex";
			}
		}

		public override ErrorCode Execute(Context context, MailboxEntry mailboxEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<Guid, bool>(0L, "UniqueMidIndexCheckTask.Execute invoked on mailbox {0}, detect only = {1}", mailboxEntry.MailboxGuid, detectOnly);
			}
			if (!shouldContinue())
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
				}
				return ErrorCode.CreateExiting((LID)35808U);
			}
			ErrorCode errorCode = IntegrityCheckTaskBase.LockMailboxForOperation(context, mailboxEntry.MailboxNumber, delegate(MailboxState mailboxState)
			{
				ErrorCode noError;
				try
				{
					using (Mailbox mailbox = Mailbox.OpenMailbox(context, mailboxState))
					{
						if (mailbox == null)
						{
							if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "The mailbox has been removed");
							}
							noError = ErrorCode.NoError;
						}
						else
						{
							MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(mailbox.Database);
							if (messageTable.Table.IsIndexCreated(context, messageTable.MessageIdUnique, new List<object>
							{
								mailbox.MailboxPartitionNumber
							}))
							{
								if (!detectOnly)
								{
									messageTable.Table.DeleteIndex(context, messageTable.MessageIdUnique, new List<object>
									{
										mailbox.MailboxPartitionNumber
									});
									context.Commit();
								}
								this.ReportCorruption("MessageIdUnique created on message table", mailboxEntry, null, null, CorruptionType.MessageIdUniqueIndexExists, !detectOnly);
							}
							noError = ErrorCode.NoError;
						}
					}
				}
				finally
				{
					context.Abort();
				}
				return noError;
			});
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<Guid, ErrorCode>(0L, "UniqueMidIndexCheckTask.Execute finished on mailbox {0} with error code {1}", mailboxEntry.MailboxGuid, errorCode);
			}
			return errorCode;
		}
	}
}
