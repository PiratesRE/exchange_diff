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
	public abstract class IntegrityCheckTaskBase : IIntegrityCheckTask
	{
		public IntegrityCheckTaskBase(IJobExecutionTracker tracker)
		{
			this.jobExecutionTracker = tracker;
		}

		public abstract string TaskName { get; }

		public IJobExecutionTracker JobExecutionTracker
		{
			get
			{
				return this.jobExecutionTracker;
			}
		}

		public bool IsScheduled
		{
			get
			{
				return this.isScheduled;
			}
		}

		public ErrorCode Execute(Context context, Guid mailboxGuid, bool detectOnly, bool isScheduled, Func<bool> shouldContinue)
		{
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "Integrity check task \"{0}\" invoked on mailbox {1}, detect only={2}, scheduled={3}", new object[]
				{
					this.TaskName,
					mailboxGuid,
					detectOnly,
					isScheduled
				});
			}
			ErrorCode errorCode = ErrorCode.NoError;
			List<MailboxEntry> list = null;
			this.isScheduled = isScheduled;
			this.perfCounters = PerformanceCounterFactory.GetDatabaseInstance(context.Database);
			this.database = context.Database;
			errorCode = this.GetMailboxEntries(context, mailboxGuid, out list, shouldContinue);
			if (errorCode != ErrorCode.NoError)
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError<ErrorCode>(0L, "GetMailboxEntries returned error {0}", errorCode);
				}
				return errorCode.Propagate((LID)65000U);
			}
			if (list.Count == 0)
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceDebug<Guid>(0L, "No matched mailbox found with mailboxGuid {0}", mailboxGuid);
				}
				return ErrorCode.NoError;
			}
			MailboxEntry mailboxEntry = list[0];
			if (!shouldContinue())
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "Task aborted");
				}
				return ErrorCode.CreateExiting((LID)40424U);
			}
			if (!this.IsScheduled)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_IntegrityCheckMailboxStart, new object[]
				{
					mailboxEntry.MailboxGuid,
					this.database.MdbGuid
				});
			}
			bool flag = false;
			try
			{
				errorCode = this.Execute(context, mailboxEntry, detectOnly, shouldContinue).Propagate((LID)34908U);
				flag = true;
			}
			finally
			{
				context.Diagnostics.ClearExceptionHistory();
				if (!flag)
				{
					errorCode = ErrorCode.CreateCallFailed((LID)45032U);
				}
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Integrity check encountered error, mailbox={0}, task={1}, detect only={2}, scheduled={3}, error={4}", new object[]
						{
							mailboxEntry.MailboxGuid,
							this.TaskName,
							detectOnly,
							isScheduled,
							errorCode
						});
					}
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_IntegrityCheckMailboxFailed, new object[]
					{
						mailboxEntry.MailboxGuid,
						errorCode.ToString(),
						this.database.MdbGuid
					});
				}
				else
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "Integrity check task finished successfully");
					}
					if (!this.IsScheduled)
					{
						Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_IntegrityCheckMailboxEnd, new object[]
						{
							mailboxEntry.MailboxGuid,
							this.database.MdbGuid
						});
					}
				}
			}
			return errorCode;
		}

		public virtual ErrorCode Execute(Context context, MailboxEntry mailboxEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<string, Guid, bool>(0L, "Integrity check task \"{0}\" invoked on mailbox {1}, detect only={2}", this.TaskName, mailboxEntry.MailboxGuid, detectOnly);
			}
			ErrorCode errorCode = ErrorCode.NoError;
			List<FolderEntry> folderEntries = null;
			errorCode = IntegrityCheckTaskBase.LockMailboxForOperation(context, mailboxEntry.MailboxNumber, (MailboxState mailboxState) => this.GetFolderEntriesForMailbox(context, mailboxState, out folderEntries, shouldContinue));
			if (errorCode != ErrorCode.NoError)
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Integrity check encountered error, mailbox={0}, task={1}, detect only={2}, error={3}", new object[]
					{
						mailboxEntry.MailboxGuid,
						this.TaskName,
						detectOnly,
						errorCode
					});
				}
				return errorCode.Propagate((LID)56808U);
			}
			if (folderEntries.Count == 0)
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "No matched folder found with mailboxGuid");
				}
				if (this.jobExecutionTracker != null)
				{
					this.jobExecutionTracker.Report(100);
				}
				return ErrorCode.NoError;
			}
			if (!shouldContinue())
			{
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
				}
				return ErrorCode.CreateExiting((LID)44520U);
			}
			int count = folderEntries.Count;
			int num = 0;
			foreach (FolderEntry folderEntry in folderEntries)
			{
				if (!shouldContinue())
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceError(0L, "Task aborted");
					}
					return ErrorCode.CreateExiting((LID)45552U);
				}
				errorCode = IntegrityCheckTaskBase.LockMailboxAndProcessFolder(context, this, mailboxEntry, folderEntry, detectOnly, shouldContinue);
				if (this.jobExecutionTracker != null)
				{
					num++;
					this.jobExecutionTracker.Report((short)(num * 100 / count));
				}
				if (errorCode != ErrorCode.NoError)
				{
					if (errorCode != ErrorCodeValue.NotFound && errorCode != ErrorCodeValue.NoAccess)
					{
						if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
						{
							ExTraceGlobals.OnlineIsintegTracer.TraceError<string, ErrorCode>(0L, "Unexpected error when executing on folder {0}, error code {1}", folderEntry.ToString(), errorCode);
						}
						return errorCode.Propagate((LID)60904U);
					}
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "Folder not found, continue with other folders");
					}
					errorCode = ErrorCode.NoError;
				}
			}
			return ErrorCode.NoError;
		}

		public virtual ErrorCode ExecuteOneFolder(Context context, MailboxState mailboxState, MailboxEntry mailboxEntry, FolderEntry folderEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<string, string>(0L, "Execute task {0} on folder {1}", this.TaskName, folderEntry.ToString());
			}
			using (Mailbox mailbox = Mailbox.OpenMailbox(context, mailboxState))
			{
				if (mailbox == null)
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, "The mailbox has been removed");
					}
					return ErrorCode.NoError;
				}
				ErrorCode errorCode = this.ExecuteOneFolder(mailbox, mailboxEntry, folderEntry, detectOnly, shouldContinue);
				if (errorCode != ErrorCode.NoError)
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceError<string, ErrorCode>(0L, "Unexpected failure on folder {0}, error code {1}", folderEntry.ToString(), errorCode);
					}
					return errorCode.Propagate((LID)65512U);
				}
				mailbox.Save(context);
				context.Commit();
			}
			return ErrorCode.NoError;
		}

		public virtual ErrorCode ExecuteOneFolder(Mailbox mailbox, MailboxEntry mailboxEntry, FolderEntry folderEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			return ErrorCode.NoError;
		}

		internal static IDisposable SetCorruptionDetectedTestHook(Action action)
		{
			return IntegrityCheckTaskBase.corruptionDetectedTestHook.SetTestHook(action);
		}

		internal static ErrorCode LockMailboxForOperation(Context context, int mailboxNumber, Func<MailboxState, ErrorCode> accessor)
		{
			context.InitializeMailboxExclusiveOperation(mailboxNumber, ExecutionDiagnostics.OperationSource.OnlineIntegrityCheck, MaintenanceHandler.MailboxLockTimeout);
			bool commit = false;
			try
			{
				ErrorCode first = context.StartMailboxOperation();
				if (first != ErrorCode.NoError)
				{
					return first.Propagate((LID)57084U);
				}
				context.LockedMailboxState.AddReference();
				try
				{
					if (!context.LockedMailboxState.IsValid || !context.LockedMailboxState.IsAccessible)
					{
						return ErrorCode.CreateNoAccess((LID)34396U);
					}
					first = accessor(context.LockedMailboxState);
					if (first != ErrorCode.NoError)
					{
						return first.Propagate((LID)55036U);
					}
					commit = true;
				}
				finally
				{
					context.LockedMailboxState.ReleaseReference();
				}
			}
			finally
			{
				if (context.IsMailboxOperationStarted)
				{
					context.EndMailboxOperation(commit);
				}
			}
			return ErrorCode.NoError;
		}

		internal static ErrorCode LockMailboxAndProcessFolder(Context context, IntegrityCheckTaskBase task, MailboxEntry mailboxEntry, FolderEntry folderEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			context.InitializeMailboxExclusiveOperation(mailboxEntry.MailboxNumber, ExecutionDiagnostics.OperationSource.OnlineIntegrityCheck, MaintenanceHandler.MailboxLockTimeout);
			bool commit = false;
			try
			{
				IIntegrityCheckTaskWithContinuation integrityCheckTaskWithContinuation = task as IIntegrityCheckTaskWithContinuation;
				ErrorCode errorCode = context.StartMailboxOperation();
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)40124U);
				}
				do
				{
					context.LockedMailboxState.AddReference();
					try
					{
						if (!context.LockedMailboxState.IsValid || !context.LockedMailboxState.IsAccessible)
						{
							return ErrorCode.CreateNoAccess((LID)56508U);
						}
						errorCode = task.ExecuteOneFolder(context, context.LockedMailboxState, mailboxEntry, folderEntry, detectOnly, shouldContinue);
						if (errorCode != ErrorCode.NoError)
						{
							return errorCode.Propagate((LID)44220U);
						}
						if (integrityCheckTaskWithContinuation == null || !integrityCheckTaskWithContinuation.ContinueExecuteOnFolder(context, mailboxEntry, folderEntry))
						{
							commit = true;
							goto IL_12E;
						}
					}
					finally
					{
						context.LockedMailboxState.ReleaseReference();
					}
					errorCode = context.PulseMailboxOperation();
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceDebug<string, Guid, ErrorCode>(0L, "Integrity check task \"{0}\" pulsed the lock on mailbox {1}, result {2}", task.TaskName, mailboxEntry.MailboxGuid, errorCode);
					}
				}
				while (!(errorCode != ErrorCode.NoError));
				return errorCode.Propagate((LID)60604U);
				IL_12E:;
			}
			finally
			{
				if (context.IsMailboxOperationStarted)
				{
					context.EndMailboxOperation(commit);
				}
			}
			return ErrorCode.NoError;
		}

		protected internal virtual bool IgnoreFolder(Context context, Mailbox mailbox, ExchangeId folderId, ExchangeId parentFolderId, bool isSearchFolder, short specialFolderNumber)
		{
			return isSearchFolder || specialFolderNumber == 4 || specialFolderNumber == 20 || specialFolderNumber == 7 || specialFolderNumber == 2 || specialFolderNumber == 1 || specialFolderNumber == 6 || specialFolderNumber == 8 || specialFolderNumber == 3;
		}

		protected ErrorCode GetMailboxEntries(Context context, Guid mailboxGuid, out List<MailboxEntry> mailboxEntries, Func<bool> shouldContinue)
		{
			mailboxEntries = new List<MailboxEntry>(0);
			List<MailboxEntry> localmailboxEntries = null;
			MailboxTable mailboxTable = Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseSchema.MailboxTable(context.Database);
			MailboxPropValueGetter mailboxPropValueGetter = new MailboxPropValueGetter(context);
			mailboxPropValueGetter.Execute(mailboxGuid, new Column[]
			{
				mailboxTable.MailboxNumber,
				mailboxTable.MailboxGuid,
				mailboxTable.MailboxOwnerDisplayName
			}, delegate(Reader reader)
			{
				int @int = reader.GetInt32(mailboxTable.MailboxNumber);
				Guid? nullableGuid = reader.GetNullableGuid(mailboxTable.MailboxGuid);
				string @string = reader.GetString(mailboxTable.MailboxOwnerDisplayName);
				if (localmailboxEntries == null)
				{
					localmailboxEntries = new List<MailboxEntry>();
				}
				localmailboxEntries.Add(new MailboxEntry(@int, @int, nullableGuid.Value, @string));
				return ErrorCode.NoError;
			}, shouldContinue).Propagate((LID)61532U);
			if (localmailboxEntries != null)
			{
				mailboxEntries = localmailboxEntries;
			}
			return ErrorCode.NoError;
		}

		protected ErrorCode GetFolderEntriesForMailbox(Context context, MailboxState mailboxState, out List<FolderEntry> folderEntries, Func<bool> shouldContinue)
		{
			IntegrityCheckTaskBase.<>c__DisplayClass7 CS$<>8__locals1 = new IntegrityCheckTaskBase.<>c__DisplayClass7();
			CS$<>8__locals1.context = context;
			CS$<>8__locals1.<>4__this = this;
			folderEntries = new List<FolderEntry>(0);
			ErrorCode noError = ErrorCode.NoError;
			CS$<>8__locals1.localFolderEntries = null;
			CS$<>8__locals1.folderTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.FolderTable(CS$<>8__locals1.context.Database);
			FolderPropValueGetter folderPropValueGetter = new FolderPropValueGetter(CS$<>8__locals1.context, mailboxState.MailboxPartitionNumber);
			using (Mailbox mailbox = Mailbox.OpenMailbox(CS$<>8__locals1.context, mailboxState))
			{
				if (mailbox == null)
				{
					if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.OnlineIsintegTracer.TraceDebug<Guid>(0L, "The mailbox with MailboxGuid {0} has been removed", mailboxState.MailboxGuid);
					}
					return ErrorCode.NoError;
				}
				folderPropValueGetter.Execute(new Column[]
				{
					CS$<>8__locals1.folderTable.FolderId,
					CS$<>8__locals1.folderTable.ParentFolderId,
					CS$<>8__locals1.folderTable.SpecialFolderNumber,
					CS$<>8__locals1.folderTable.QueryCriteria,
					CS$<>8__locals1.folderTable.DisplayName
				}, delegate(Reader reader)
				{
					byte[] binary = reader.GetBinary(CS$<>8__locals1.folderTable.FolderId);
					ExchangeId folderId = ExchangeId.CreateFrom26ByteArray(CS$<>8__locals1.context, mailbox.ReplidGuidMap, binary);
					byte[] binary2 = reader.GetBinary(CS$<>8__locals1.folderTable.ParentFolderId);
					ExchangeId parentFolderId = ExchangeId.CreateFrom26ByteArray(CS$<>8__locals1.context, mailbox.ReplidGuidMap, binary2);
					short @int = reader.GetInt16(CS$<>8__locals1.folderTable.SpecialFolderNumber);
					string @string = reader.GetString(CS$<>8__locals1.folderTable.DisplayName);
					bool isSearchFolder = reader.GetBinary(CS$<>8__locals1.folderTable.QueryCriteria) != null;
					if (CS$<>8__locals1.<>4__this.IgnoreFolder(CS$<>8__locals1.context, mailbox, folderId, parentFolderId, isSearchFolder, @int))
					{
						return ErrorCode.NoError;
					}
					if (CS$<>8__locals1.localFolderEntries == null)
					{
						CS$<>8__locals1.localFolderEntries = new List<FolderEntry>();
					}
					CS$<>8__locals1.localFolderEntries.Add(new FolderEntry(folderId, @int, @string));
					return ErrorCode.NoError;
				}, shouldContinue).Propagate((LID)45148U);
			}
			if (CS$<>8__locals1.localFolderEntries != null)
			{
				folderEntries = CS$<>8__locals1.localFolderEntries;
			}
			return ErrorCode.NoError;
		}

		protected void ReportCorruption(string description, MailboxEntry mailboxEntry, FolderEntry folderEntry, MessageEntry messageEntry, CorruptionType corruptionType, bool problemFixed)
		{
			if (this.jobExecutionTracker != null)
			{
				Corruption corruption = new Corruption(corruptionType, new ExchangeId?((folderEntry != null) ? folderEntry.FolderId : ExchangeId.Zero), new ExchangeId?((messageEntry != null) ? messageEntry.MessageId : ExchangeId.Zero), problemFixed);
				this.jobExecutionTracker.OnCorruptionDetected(corruption);
			}
			string text;
			if (folderEntry == null)
			{
				text = string.Format("Corruption: {0}\r\n\r\nFixed:{1}", description, problemFixed);
			}
			else if (messageEntry == null)
			{
				text = string.Format("Corruption: {0}\r\nFolder:{1}\r\nFixed:{2}", description, folderEntry.ToString(), problemFixed);
			}
			else
			{
				text = string.Format("Corruption: {0}\r\nFolder:{1}\r\nMessage:{2}\r\nFixed:{3}", new object[]
				{
					description,
					folderEntry.ToString(),
					messageEntry.MessageId.ToString(),
					problemFixed
				});
			}
			Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_IntegrityCheckCorruptionDetected, new object[]
			{
				mailboxEntry.MailboxGuid.ToString(),
				text,
				corruptionType.ToString(),
				this.IsScheduled,
				(this.database != null) ? this.database.MdbGuid : Guid.Empty
			});
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug(0L, text);
			}
			if (this.IsScheduled && this.perfCounters != null)
			{
				if (problemFixed)
				{
					this.perfCounters.ScheduledISIntegCorruptionFixedCount.Increment();
				}
				else
				{
					this.perfCounters.ScheduledISIntegCorruptionDetectedCount.Increment();
				}
			}
			FaultInjection.InjectFault(IntegrityCheckTaskBase.corruptionDetectedTestHook);
		}

		private static Hookable<Action> corruptionDetectedTestHook = Hookable<Action>.Create(true, null);

		private IJobExecutionTracker jobExecutionTracker;

		private bool isScheduled;

		private StoreDatabase database;

		private StorePerDatabasePerformanceCountersInstance perfCounters;
	}
}
