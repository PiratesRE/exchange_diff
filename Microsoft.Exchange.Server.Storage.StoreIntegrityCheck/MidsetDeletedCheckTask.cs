using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class MidsetDeletedCheckTask : IntegrityCheckTaskBase
	{
		public MidsetDeletedCheckTask(IJobExecutionTracker tracker) : base(tracker)
		{
		}

		public override string TaskName
		{
			get
			{
				return "MidsetDeleted";
			}
		}

		public override ErrorCode ExecuteOneFolder(Mailbox mailbox, MailboxEntry mailboxEntry, FolderEntry folderEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<string, string>(0L, "Execute task {0} on folder {1}", this.TaskName, folderEntry.ToString());
			}
			this.currentMailbox = mailboxEntry;
			this.currentFolder = folderEntry;
			ErrorCode errorCode = ErrorCode.NoError;
			Context currentOperationContext = mailbox.CurrentOperationContext;
			Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(currentOperationContext.Database);
			new List<int>();
			using (Folder folder = Folder.OpenFolder(currentOperationContext, mailbox, folderEntry.FolderId))
			{
				if (folder == null)
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceDebug<string>(0L, "Folder {0} has been removed, continue with other folders", folderEntry.ToString());
					}
					return ErrorCode.NoError;
				}
				IdSet midsetDeleted = folder.GetMidsetDeleted(currentOperationContext);
				if (midsetDeleted.IsEmpty)
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceDebug<string>(0L, "Midset deleted is empty on Folder {0}", folderEntry.ToString());
					}
					return ErrorCode.NoError;
				}
				List<MessageEntry> list;
				errorCode = this.GetCorruptedMessages(mailbox, folderEntry.FolderId, midsetDeleted, out list, shouldContinue);
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceError<string, ErrorCode>(0L, "Unexpected error when retrieving corrupted messages in folder {0}, error code {1}", folderEntry.ToString(), errorCode);
					}
					return errorCode.Propagate((LID)36328U);
				}
				if (list.Count == 0)
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceDebug<string>(0L, "No corrupted message found in folder {0}", folderEntry.ToString());
					}
					return ErrorCode.NoError;
				}
				if (!shouldContinue())
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
					}
					return ErrorCode.CreateExiting((LID)52712U);
				}
				errorCode = this.ReportAndFixCorruption(mailbox, folder, midsetDeleted, list, detectOnly, shouldContinue);
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceError<string, ErrorCode>(0L, "Unexpected error when fixing corruption in folder {0}, error code {1}", folderEntry.ToString(), errorCode);
					}
					return errorCode.Propagate((LID)46568U);
				}
			}
			return ErrorCode.NoError;
		}

		private ErrorCode GetCorruptedMessages(Mailbox mailbox, ExchangeId folderId, IdSet midsetDeleted, out List<MessageEntry> corruptedMessages, Func<bool> shouldContinue)
		{
			corruptedMessages = null;
			ErrorCode errorCode = ErrorCode.NoError;
			Context context = mailbox.CurrentOperationContext;
			List<MessageEntry> localCorruptedMessages = null;
			MessagePropValueGetter messagePropValueGetter = new MessagePropValueGetter(context, mailbox.MailboxPartitionNumber, folderId);
			if (!shouldContinue())
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
				}
				return ErrorCode.CreateExiting((LID)62952U);
			}
			MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(context.Database);
			errorCode = messagePropValueGetter.Execute(false, new Column[]
			{
				messageTable.MessageDocumentId,
				messageTable.MessageId
			}, delegate(Reader reader)
			{
				int @int = reader.GetInt32(messageTable.MessageDocumentId);
				byte[] binary = reader.GetBinary(messageTable.MessageId);
				ExchangeId exchangeId = ExchangeId.CreateFrom26ByteArray(context, mailbox.ReplidGuidMap, binary);
				if (midsetDeleted.Contains(exchangeId))
				{
					if (localCorruptedMessages == null)
					{
						localCorruptedMessages = new List<MessageEntry>();
					}
					localCorruptedMessages.Add(new MessageEntry(@int, exchangeId));
				}
				return ErrorCode.NoError;
			}, shouldContinue);
			if (errorCode != ErrorCode.NoError)
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError<ErrorCode>(0L, "Unexpected error when check messages for corruption, error code {0}", errorCode);
				}
				return errorCode.Propagate((LID)38376U);
			}
			corruptedMessages = (localCorruptedMessages ?? new List<MessageEntry>(0));
			return ErrorCode.NoError;
		}

		private ErrorCode ReportAndFixCorruption(Mailbox mailbox, Folder folder, IdSet midsetDeleted, List<MessageEntry> corruptedMessages, bool detectOnly, Func<bool> shouldContinue)
		{
			Context currentOperationContext = mailbox.CurrentOperationContext;
			bool problemFixed = false;
			if (!detectOnly)
			{
				foreach (MessageEntry messageEntry in corruptedMessages)
				{
					if (!shouldContinue())
					{
						if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
						{
							ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
						}
						return ErrorCode.CreateExiting((LID)54760U);
					}
					midsetDeleted.Remove(messageEntry.MessageId);
				}
				folder.SetMidsetDeleted(currentOperationContext, midsetDeleted, true);
				folder.Save(currentOperationContext);
				problemFixed = true;
			}
			foreach (MessageEntry messageEntry2 in corruptedMessages)
			{
				base.ReportCorruption("Mid put into folder's midsetDeleted", this.currentMailbox, this.currentFolder, messageEntry2, CorruptionType.UndeletedMessageInMidsetDeleted, problemFixed);
			}
			return ErrorCode.NoError;
		}

		private MailboxEntry currentMailbox;

		private FolderEntry currentFolder;
	}
}
