using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LazyIndexing;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public static class MailboxCleanup
	{
		internal static void Initialize()
		{
			if (MailboxCleanup.markHardDeletedMailboxesForCleanupMaintenance == null)
			{
				MailboxCleanup.markHardDeletedMailboxesForCleanupMaintenance = MaintenanceHandler.RegisterDatabaseMaintenance(MailboxCleanup.MarkHardDeletedMailboxesForCleanupMaintenanceId, RequiredMaintenanceResourceType.Store, new MaintenanceHandler.DatabaseMaintenanceDelegate(MailboxCleanup.MarkHardDeletedMailboxesForCleanupMaintenance), "MailboxCleanup.MarkHardDeletedMailboxesForCleanupMaintenance");
				Mailbox.InitializeHardDeletedMailboxMaintenance(MailboxCleanup.MailboxCleanupMaintenanceId, RequiredMaintenanceResourceType.Store, new MaintenanceHandler.MailboxMaintenanceDelegate(MailboxCleanup.CleanupAndRemoveHardDeletedMailbox), "MailboxCleanup.CleanupAndRemoveHardDeletedMailbox");
				MailboxCleanup.markTombstoneMailboxesForCleanup = MaintenanceHandler.RegisterDatabaseMaintenance(MailboxCleanup.TombstoneMailboxCleanupMaintenanceId, RequiredMaintenanceResourceType.Store, new MaintenanceHandler.DatabaseMaintenanceDelegate(MailboxCleanup.CleanupAndRemoveTombstoneMailboxes), "MailboxCleanup.CleanupAndRemoveTombstoneMailboxes");
			}
		}

		internal static void MountedEventHandler(Context context, StoreDatabase database)
		{
			MailboxCleanup.markHardDeletedMailboxesForCleanupMaintenance.MarkForMaintenance(context);
			MailboxCleanup.markTombstoneMailboxesForCleanup.ScheduleMarkForMaintenance(context, TimeSpan.FromDays(1.0));
		}

		internal static IDisposable SetTombstoneTableCleanupChunkSizeForTest(int chunkSize)
		{
			return MailboxCleanup.tombstoneTableCleanupChunkSize.SetTestHook(chunkSize);
		}

		public static void CleanupAndRemoveTombstoneMailboxes(Context context, DatabaseInfo databaseInfo, out bool completed)
		{
			MailboxTable mailboxTable = Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseSchema.MailboxTable(context.Database);
			IEnumerable<MailboxState> stateListSnapshot = MailboxStateCache.GetStateListSnapshot(context, Factory.CreateSearchCriteriaCompare(mailboxTable.Status, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(6)));
			MaintenanceHandler.ApplyMaintenanceToMailboxes(context, stateListSnapshot, ExecutionDiagnostics.OperationSource.MailboxCleanup, false, new Action<Context, MailboxState>(MailboxCleanup.CleanupAndRemoveTombstoneMailboxesHelper), out completed);
		}

		private static void CleanupAndRemoveTombstoneMailboxesHelper(Context context, MailboxState mailboxState)
		{
			if (!mailboxState.IsTombstone)
			{
				return;
			}
			using (Mailbox mailbox = Mailbox.OpenMailbox(context, mailboxState))
			{
				DateTime utcNow = DateTime.UtcNow;
				if (utcNow - mailbox.GetDeletedOn(context).Value >= ConfigurationSchema.TombstoneMailboxExpirationPeriod.Value)
				{
					try
					{
						mailbox.RemoveMailboxEntriesFromTable(context, Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseSchema.MailboxTable(mailbox.Database).Table);
						mailbox.Save(context);
						MailboxStateCache.DeleteMailboxState(context, mailboxState);
						context.Commit();
					}
					finally
					{
						context.Abort();
					}
				}
			}
		}

		public static void MarkTombstoneMailboxesForCleanupMaintenanceForTest(Context context)
		{
			MailboxCleanup.markTombstoneMailboxesForCleanup.MarkForMaintenance(context);
		}

		public static void CleanupAndRemoveHardDeletedMailbox(Context context, int mailboxNumber)
		{
			bool commit = false;
			try
			{
				context.InitializeMailboxExclusiveOperation(mailboxNumber, ExecutionDiagnostics.OperationSource.MailboxCleanup, MaintenanceHandler.MailboxLockTimeout);
				ErrorCode errorCode = context.StartMailboxOperation();
				if (errorCode != ErrorCode.NoError)
				{
					throw new StoreException((LID)35068U, errorCode);
				}
				bool flag;
				MailboxCleanup.CleanupAndRemoveHardDeletedMailbox(context, context.LockedMailboxState, out flag);
				commit = true;
			}
			finally
			{
				if (context.IsMailboxOperationStarted)
				{
					context.EndMailboxOperation(commit);
				}
			}
		}

		private static void CleanupAndRemoveHardDeletedMailbox(Context context, MailboxState mailboxState, out bool completed)
		{
			completed = false;
			if (!mailboxState.IsHardDeleted)
			{
				DiagnosticContext.TraceDword((LID)45528U, (uint)mailboxState.MailboxNumber);
				throw new StoreException((LID)43640U, ErrorCodeValue.NotSupported, "Cannot clean up non HardDeleted mailbox");
			}
			bool flag;
			if (mailboxState.UnifiedState == null)
			{
				flag = true;
			}
			else
			{
				MailboxTable mailboxTable = Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseSchema.MailboxTable(context.Database);
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					mailboxState.UnifiedState.UnifiedMailboxGuid
				});
				SearchCriteria restriction = Factory.CreateSearchCriteriaCompare(mailboxTable.Status, SearchCriteriaCompare.SearchRelOp.NotEqual, Factory.CreateConstantColumn(6));
				using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, mailboxTable.Table, mailboxTable.UnifiedMailboxGuidIndex, new Column[]
				{
					mailboxTable.MailboxGuid
				}, restriction, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						reader.Read();
						flag = !reader.Read();
					}
				}
			}
			EventHistory eventHistory = EventHistory.GetEventHistory(context.Database);
			uint num;
			eventHistory.DeleteWatermarksForMailbox(context, mailboxState.MailboxNumber, out num);
			try
			{
				TombstoneTable tombstoneTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.TombstoneTable(context.Database);
				StartStopKey startStopKey2 = new StartStopKey(true, new object[]
				{
					mailboxState.MailboxNumber
				});
				using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, tombstoneTable.Table, tombstoneTable.TombstonePK, null, null, null, 0, 0, new KeyRange(startStopKey2, startStopKey2), false, false), false))
				{
					deleteOperator.EnableInterrupts(new WriteChunkingInterruptControl(MailboxCleanup.tombstoneTableCleanupChunkSize.Value, null));
					deleteOperator.ExecuteScalar();
					while (deleteOperator.Interrupted)
					{
						context.Commit();
						if (MaintenanceHandler.ShouldStopMailboxMaintenanceTask(context, mailboxState, MailboxCleanup.MailboxCleanupMaintenanceId))
						{
							return;
						}
						deleteOperator.ExecuteScalar();
					}
					context.Commit();
				}
				using (Mailbox mailbox = Mailbox.OpenMailbox(context, mailboxState))
				{
					if (flag)
					{
						DeliveredTo.RemoveAllEntriesForMailbox(context, mailbox);
						mailbox.RemoveMailboxEntriesFromTable(context, Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.AttachmentTable(mailbox.Database).Table);
						mailbox.RemoveMailboxEntriesFromTable(context, Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(mailbox.Database).Table);
						mailbox.RemoveMailboxEntriesFromTable(context, Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.FolderTable(mailbox.Database).Table);
						mailbox.RemoveMailboxEntriesFromTable(context, Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.PerUserTable(mailbox.Database).Table);
						mailbox.RemoveMailboxEntriesFromTable(context, Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.InferenceLogTable(mailbox.Database).Table);
					}
					ReceiveFolder.RemoveAllReceiveFolders(context, mailbox);
					SearchQueue.RemoveAllEntriesForMailbox(context, mailbox);
					if (flag)
					{
						LogicalIndexCache.DeleteLogicalIndexes(context, mailbox);
						mailbox.RemoveMailboxEntriesFromTable(context, Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseSchema.ExtendedPropertyNameMappingTable(mailbox.Database).Table);
						mailbox.RemoveMailboxEntriesFromTable(context, Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseSchema.ReplidGuidMapTable(mailbox.Database).Table);
						mailbox.RemoveMailboxEntriesFromTable(context, mailbox.MailboxIdentityTable.Table);
					}
					mailbox.MakeTombstone(context);
					context.Commit();
					completed = true;
				}
			}
			finally
			{
				context.Abort();
			}
		}

		private static void MarkHardDeletedMailboxesForCleanupMaintenance(Context context, DatabaseInfo databaseInfo, out bool completed)
		{
			MailboxTable mailboxTable = Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseSchema.MailboxTable(context.Database);
			completed = true;
			using (MailboxList mailboxList = new MailboxList(context, new Column[]
			{
				mailboxTable.MailboxNumber
			}, context.Database, MailboxList.ListType.FinalCleanup))
			{
				using (Reader reader = mailboxList.OpenList())
				{
					while (reader.Read())
					{
						int @int = reader.GetInt32(mailboxTable.MailboxNumber);
						bool flag;
						MaintenanceHandler.ApplyMaintenanceToOneMailbox(context, @int, ExecutionDiagnostics.OperationSource.MailboxCleanup, false, delegate(Context ctx, MailboxState mbxState)
						{
							Mailbox.CleanupHardDeletedMailboxesMaintenance.MarkForMaintenance(ctx, mbxState);
						}, out flag);
						if (!flag)
						{
							completed = false;
						}
					}
				}
			}
		}

		private const int DefaultTombstoneTableCleanupChunkSize = 5000;

		public static readonly Guid MailboxCleanupMaintenanceId = new Guid("{05ad2280-b95c-4e3f-bc1c-baaa8fb97e55}");

		public static readonly Guid TombstoneMailboxCleanupMaintenanceId = new Guid("{81650b69-0c92-488f-9de7-d2e41fca7efa}");

		public static readonly Guid MarkHardDeletedMailboxesForCleanupMaintenanceId = new Guid("{128e9fa8-7013-42d8-a957-9bda9f288649}");

		private static Hookable<int> tombstoneTableCleanupChunkSize = Hookable<int>.Create(true, 5000);

		private static IDatabaseMaintenance markHardDeletedMailboxesForCleanupMaintenance;

		private static IDatabaseMaintenance markTombstoneMailboxesForCleanup;
	}
}
