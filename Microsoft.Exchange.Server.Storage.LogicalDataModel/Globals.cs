using System;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public static class Globals
	{
		public static void Initialize()
		{
			DatabaseSchema.Initialize();
			LogicalMailbox.Initialize();
			UnifiedMailbox.InitializeUpgraderAction(delegate(Context context)
			{
				FolderTable folderTable = DatabaseSchema.FolderTable(context.Database);
				folderTable.Table.AddColumn(context, folderTable.MailboxNumber);
				MessageTable messageTable = DatabaseSchema.MessageTable(context.Database);
				messageTable.Table.AddColumn(context, messageTable.MailboxNumber);
				AttachmentTable attachmentTable = DatabaseSchema.AttachmentTable(context.Database);
				attachmentTable.Table.AddColumn(context, attachmentTable.MailboxNumber);
				InferenceLogTable inferenceLogTable = DatabaseSchema.InferenceLogTable(context.Database);
				inferenceLogTable.Table.AddColumn(context, inferenceLogTable.MailboxNumber);
			}, delegate(StoreDatabase database)
			{
				FolderTable folderTable = DatabaseSchema.FolderTable(database);
				folderTable.MailboxNumber.MinVersion = UnifiedMailbox.Instance.To.Value;
				MessageTable messageTable = DatabaseSchema.MessageTable(database);
				messageTable.MailboxNumber.MinVersion = UnifiedMailbox.Instance.To.Value;
				AttachmentTable attachmentTable = DatabaseSchema.AttachmentTable(database);
				attachmentTable.MailboxNumber.MinVersion = UnifiedMailbox.Instance.To.Value;
				InferenceLogTable inferenceLogTable = DatabaseSchema.InferenceLogTable(database);
				inferenceLogTable.MailboxNumber.MinVersion = UnifiedMailbox.Instance.To.Value;
			});
			TimedEventDispatcher.RegisterHandler(TimerEventHandler.EventSource, new TimerEventHandler());
			InTransitInfo.Initialize();
			ReceiveFolder.Initialize();
			EventHistory.Initialize();
			DeliveredTo.Initialize();
			ReliableEventNotificationSubscriber.Subscribe(null);
			FolderHierarchy.Initialize();
			Folder.Initialize();
			SearchFolder.Initialize();
			OpenMessageStates.Initialize();
			SubobjectReferenceState.Initialize();
			SubobjectCleanup.Initialize();
			MailboxCleanup.Initialize();
			SpecialFoldersCache.Initialize();
			PerUser.Initialize();
			UserInformation.Initialize();
			Mailbox.TableSizeStatistics[] array = new Mailbox.TableSizeStatistics[5];
			Mailbox.TableSizeStatistics[] array2 = array;
			int num = 0;
			Mailbox.TableSizeStatistics tableSizeStatistics = default(Mailbox.TableSizeStatistics);
			tableSizeStatistics.TableAccessor = ((Context context) => DatabaseSchema.MessageTable(context.Database).Table);
			tableSizeStatistics.TotalPagesProperty = PropTag.Mailbox.MessageTableTotalPages;
			tableSizeStatistics.AvailablePagesProperty = PropTag.Mailbox.MessageTableAvailablePages;
			array2[num] = tableSizeStatistics;
			Mailbox.TableSizeStatistics[] array3 = array;
			int num2 = 1;
			Mailbox.TableSizeStatistics tableSizeStatistics2 = default(Mailbox.TableSizeStatistics);
			tableSizeStatistics2.TableAccessor = ((Context context) => DatabaseSchema.AttachmentTable(context.Database).Table);
			tableSizeStatistics2.TotalPagesProperty = PropTag.Mailbox.AttachmentTableTotalPages;
			tableSizeStatistics2.AvailablePagesProperty = PropTag.Mailbox.AttachmentTableAvailablePages;
			array3[num2] = tableSizeStatistics2;
			Mailbox.TableSizeStatistics[] array4 = array;
			int num3 = 2;
			Mailbox.TableSizeStatistics tableSizeStatistics3 = default(Mailbox.TableSizeStatistics);
			tableSizeStatistics3.TableAccessor = ((Context context) => DatabaseSchema.FolderTable(context.Database).Table);
			tableSizeStatistics3.TotalPagesProperty = PropTag.Mailbox.OtherTablesTotalPages;
			tableSizeStatistics3.AvailablePagesProperty = PropTag.Mailbox.OtherTablesAvailablePages;
			array4[num3] = tableSizeStatistics3;
			Mailbox.TableSizeStatistics[] array5 = array;
			int num4 = 3;
			Mailbox.TableSizeStatistics tableSizeStatistics4 = default(Mailbox.TableSizeStatistics);
			tableSizeStatistics4.TableAccessor = ((Context context) => DatabaseSchema.InferenceLogTable(context.Database).Table);
			tableSizeStatistics4.TotalPagesProperty = PropTag.Mailbox.OtherTablesTotalPages;
			tableSizeStatistics4.AvailablePagesProperty = PropTag.Mailbox.OtherTablesAvailablePages;
			array5[num4] = tableSizeStatistics4;
			Mailbox.TableSizeStatistics[] array6 = array;
			int num5 = 4;
			Mailbox.TableSizeStatistics tableSizeStatistics5 = default(Mailbox.TableSizeStatistics);
			tableSizeStatistics5.TableAccessor = ((Context context) => DatabaseSchema.PerUserTable(context.Database).Table);
			tableSizeStatistics5.TotalPagesProperty = PropTag.Mailbox.OtherTablesTotalPages;
			tableSizeStatistics5.AvailablePagesProperty = PropTag.Mailbox.OtherTablesAvailablePages;
			array6[num5] = tableSizeStatistics5;
			Mailbox.RegisterTableSizeStatistics(array);
		}

		public static void Terminate()
		{
			ReliableEventNotificationSubscriber.Unsubscribe();
			TimedEventDispatcher.UnregisterHandler(TimerEventHandler.EventSource);
		}

		public static void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			DatabaseSchema.Initialize(database);
		}

		public static void DatabaseMounting(Context context, StoreDatabase database, bool readOnly)
		{
			DatabaseSchema.PostMountInitialize(context, database);
			PropertySchemaPopulation.MountEventHandler(database);
			DatabaseSchema.MountEventHandlerForFullTextIndex(database);
			EventHistory.MountEventHandler(context, readOnly);
			PerUser.MountEventHandler(context, database, readOnly);
			SubobjectCleanup.MountEventHandler(context);
		}

		public static void DatabaseMounted(Context context, StoreDatabase database)
		{
			if (!database.IsReadOnly)
			{
				DatabaseSizeCheck.LaunchDatabaseSizeCheckTask(database);
				DeliveredTo.MountedEventHandler(context);
				EventHistory.MountedEventHandler(context);
				MailboxCleanup.MountedEventHandler(context, database);
				SearchFolder.MountedEventHandler(context);
				SearchQueue.DrainSearchQueueTask.Launch(database);
				SubobjectCleanup.MountedEventHandler(context, database);
			}
		}

		public static void DatabaseDismounting(Context context, StoreDatabase database)
		{
			EventHistory.DismountEventHandler(database);
			PerUser.DismountEventHandler(context, database);
			SubobjectCleanup.DismountEventHandler(database);
		}
	}
}
