using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class AggregateCountsCheckTask : IntegrityCheckTaskBase
	{
		public AggregateCountsCheckTask(IJobExecutionTracker tracker) : base(tracker)
		{
		}

		public override string TaskName
		{
			get
			{
				return "AggregateCounts";
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
			MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(currentOperationContext.Database);
			using (Folder folder = Folder.OpenFolder(currentOperationContext, mailbox, folderEntry.FolderId))
			{
				List<AggregateCountsCheckTask.IAggregateFolderCounter> aggregationProperties = this.SetupAggregationProperties(currentOperationContext, folder, messageTable);
				errorCode = this.GetAggregateCounters(mailbox, folderEntry.FolderId, aggregationProperties, shouldContinue);
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceError<string, ErrorCode>(0L, "Unexpected error when reading aggregate counters in folder {0}, error code {1}", folderEntry.ToString(), errorCode);
					}
					return errorCode.Propagate((LID)2391158077U);
				}
				if (!shouldContinue())
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
					}
					return ErrorCode.CreateExiting((LID)3464899901U);
				}
				errorCode = this.ReportAndFixCorruption(mailbox, folder, aggregationProperties, detectOnly, shouldContinue);
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceError<string, ErrorCode>(0L, "Unexpected error when fixing corruption in folder {0}, error code {1}", folderEntry.ToString(), errorCode);
					}
					return errorCode.Propagate((LID)2995137853U);
				}
			}
			return ErrorCode.NoError;
		}

		private List<AggregateCountsCheckTask.IAggregateFolderCounter> SetupAggregationProperties(Context context, Folder folder, MessageTable messageTable)
		{
			List<AggregateCountsCheckTask.IAggregateFolderCounter> list = new List<AggregateCountsCheckTask.IAggregateFolderCounter>
			{
				new AggregateCountsCheckTask.AggregateFolderCounter<long>(folder.FolderTable.MessageCount, new Column[]
				{
					messageTable.IsHidden
				}, (Reader reader, long origValue) => origValue + (reader.GetBoolean(messageTable.IsHidden) ? 0L : 1L)),
				new AggregateCountsCheckTask.AggregateFolderCounter<long>(folder.FolderTable.HiddenItemCount, new Column[]
				{
					messageTable.IsHidden
				}, (Reader reader, long origValue) => origValue + (reader.GetBoolean(messageTable.IsHidden) ? 1L : 0L)),
				new AggregateCountsCheckTask.AggregateFolderCounter<long>(folder.FolderTable.MessageHasAttachCount, new Column[]
				{
					messageTable.IsHidden,
					messageTable.HasAttachments
				}, delegate(Reader reader, long origValue)
				{
					bool boolean = reader.GetBoolean(messageTable.IsHidden);
					bool boolean2 = reader.GetBoolean(messageTable.HasAttachments);
					return origValue + ((boolean2 && !boolean) ? 1L : 0L);
				}),
				new AggregateCountsCheckTask.AggregateFolderCounter<long>(folder.FolderTable.HiddenItemHasAttachCount, new Column[]
				{
					messageTable.IsHidden,
					messageTable.HasAttachments
				}, delegate(Reader reader, long origValue)
				{
					bool boolean = reader.GetBoolean(messageTable.IsHidden);
					bool boolean2 = reader.GetBoolean(messageTable.HasAttachments);
					return origValue + ((boolean2 && boolean) ? 1L : 0L);
				}),
				new AggregateCountsCheckTask.AggregateFolderCounter<long>(folder.FolderTable.MessageSize, new Column[]
				{
					messageTable.IsHidden,
					messageTable.Size
				}, (Reader reader, long origValue) => origValue + (reader.GetBoolean(messageTable.IsHidden) ? 0L : reader.GetInt64(messageTable.Size))),
				new AggregateCountsCheckTask.AggregateFolderCounter<long>(folder.FolderTable.HiddenItemSize, new Column[]
				{
					messageTable.IsHidden,
					messageTable.Size
				}, (Reader reader, long origValue) => origValue + (reader.GetBoolean(messageTable.IsHidden) ? reader.GetInt64(messageTable.Size) : 0L))
			};
			if (!folder.IsPerUserReadUnreadTrackingEnabled)
			{
				List<AggregateCountsCheckTask.IAggregateFolderCounter> list2 = list;
				List<AggregateCountsCheckTask.IAggregateFolderCounter> list3 = new List<AggregateCountsCheckTask.IAggregateFolderCounter>();
				list3.Add(new AggregateCountsCheckTask.AggregateFolderCounter<long>(folder.FolderTable.UnreadMessageCount, new Column[]
				{
					messageTable.IsHidden,
					messageTable.IsRead
				}, delegate(Reader reader, long origValue)
				{
					bool boolean = reader.GetBoolean(messageTable.IsHidden);
					bool flag = !reader.GetBoolean(messageTable.IsRead);
					return origValue + ((flag && !boolean) ? 1L : 0L);
				}));
				list3.Add(new AggregateCountsCheckTask.AggregateFolderCounter<long>(folder.FolderTable.UnreadHiddenItemCount, new Column[]
				{
					messageTable.IsHidden,
					messageTable.IsRead
				}, delegate(Reader reader, long origValue)
				{
					bool boolean = reader.GetBoolean(messageTable.IsHidden);
					bool flag = !reader.GetBoolean(messageTable.IsRead);
					return origValue + ((flag && boolean) ? 1L : 0L);
				}));
				list2.AddRange(list3);
			}
			else
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<string, string>(0L, "Execute task {0} skipping folder {1} with no PerUserReadUnreadTrackingEnabled", this.TaskName, folder.GetName(context));
			}
			return list;
		}

		private ErrorCode GetAggregateCounters(Mailbox mailbox, ExchangeId folderId, List<AggregateCountsCheckTask.IAggregateFolderCounter> aggregationProperties, Func<bool> shouldContinue)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			Context currentOperationContext = mailbox.CurrentOperationContext;
			MessagePropValueGetter messagePropValueGetter = new MessagePropValueGetter(currentOperationContext, mailbox.MailboxNumber, folderId);
			if (!shouldContinue())
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
				}
				return ErrorCode.CreateExiting((LID)4068879677U);
			}
			Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(currentOperationContext.Database);
			HashSet<Column> hashSet = new HashSet<Column>();
			foreach (AggregateCountsCheckTask.IAggregateFolderCounter aggregateFolderCounter in aggregationProperties)
			{
				hashSet.UnionWith(aggregateFolderCounter.ColumnsToFetch());
			}
			errorCode = messagePropValueGetter.Execute(false, hashSet.ToArray<Column>(), delegate(Reader reader)
			{
				foreach (AggregateCountsCheckTask.IAggregateFolderCounter aggregateFolderCounter2 in aggregationProperties)
				{
					aggregateFolderCounter2.UpdateAggregation(reader);
				}
				return ErrorCode.NoError;
			}, shouldContinue);
			if (errorCode != ErrorCode.NoError)
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError<ErrorCode>(0L, "Unexpected error when check messages for aggregation, error code {0}", errorCode);
				}
				return errorCode.Propagate((LID)2458266941U);
			}
			return ErrorCode.NoError;
		}

		private ErrorCode ReportAndFixCorruption(Mailbox mailbox, Folder folder, List<AggregateCountsCheckTask.IAggregateFolderCounter> aggregationProperties, bool detectOnly, Func<bool> shouldContinue)
		{
			Context currentOperationContext = mailbox.CurrentOperationContext;
			bool flag = false;
			foreach (AggregateCountsCheckTask.IAggregateFolderCounter aggregateFolderCounter in aggregationProperties)
			{
				string text;
				flag |= aggregateFolderCounter.ReportAndFixCorruption(currentOperationContext, mailbox, folder, detectOnly, out text);
				if (text != null)
				{
					base.ReportCorruption(text, this.currentMailbox, this.currentFolder, null, CorruptionType.AggregateCountMismatch, flag);
				}
				if (!shouldContinue())
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
					}
					return ErrorCode.CreateExiting((LID)3532008765U);
				}
			}
			if (flag)
			{
				folder.Save(currentOperationContext);
			}
			return ErrorCode.NoError;
		}

		private MailboxEntry currentMailbox;

		private FolderEntry currentFolder;

		public interface IAggregateFolderCounter
		{
			void UpdateAggregation(Reader reader);

			Column[] ColumnsToFetch();

			bool ReportAndFixCorruption(Context context, Mailbox mailbox, Folder folder, bool detectOnly, out string corruptionInfo);
		}

		private class AggregateFolderCounter<ColT> : AggregateCountsCheckTask.IAggregateFolderCounter where ColT : IComparable<ColT>
		{
			public AggregateFolderCounter(PhysicalColumn column, Column[] columnsToFetch, Func<Reader, ColT, ColT> aggregateGetter)
			{
				this.column = column;
				this.columnsToFetch = columnsToFetch;
				this.aggregateGetter = aggregateGetter;
			}

			public virtual void UpdateAggregation(Reader reader)
			{
				this.currentValue = this.aggregateGetter(reader, this.currentValue);
			}

			public virtual Column[] ColumnsToFetch()
			{
				return this.columnsToFetch;
			}

			public virtual bool ReportAndFixCorruption(Context context, Mailbox mailbox, Folder folder, bool detectOnly, out string corruptionInfo)
			{
				bool result = false;
				ColT colT = (ColT)((object)folder.GetColumnValue(context, this.column));
				corruptionInfo = null;
				if (colT.CompareTo(this.currentValue) != 0)
				{
					if (!detectOnly)
					{
						folder.SetColumn(context, this.column, this.currentValue);
						result = true;
					}
					corruptionInfo = string.Format("Column {0}:{1} -> {2}", this.column.Name, colT, this.currentValue);
				}
				return result;
			}

			private ColT currentValue;

			private PhysicalColumn column;

			private Column[] columnsToFetch;

			private Func<Reader, ColT, ColT> aggregateGetter;
		}
	}
}
