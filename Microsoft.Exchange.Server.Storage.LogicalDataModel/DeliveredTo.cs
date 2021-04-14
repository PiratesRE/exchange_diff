using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public static class DeliveredTo
	{
		public static void Initialize()
		{
			if (DeliveredTo.deliveredToCleanupMaintenance == null)
			{
				DeliveredTo.deliveredToCleanupMaintenance = MaintenanceHandler.RegisterDatabaseMaintenance(DeliveredTo.DeliveredToCleanupMaintenanceId, RequiredMaintenanceResourceType.Store, new MaintenanceHandler.DatabaseMaintenanceDelegate(DeliveredTo.DeliveredToCleanupMaintenance), "DeliveredTo.DeliveredToCleanupMaintenance");
			}
		}

		internal static void MountedEventHandler(Context context)
		{
			DeliveredTo.deliveredToCleanupMaintenance.ScheduleMarkForMaintenance(context, TimeSpan.FromDays(1.0));
		}

		public static void DeliveredToCleanupMaintenance(Context context, DatabaseInfo databaseInfo, out bool completed)
		{
			DeliveredToTable deliveredToTable = DatabaseSchema.DeliveredToTable(context.Database);
			StartStopKey startKey = new StartStopKey(true, new object[]
			{
				DateTime.MinValue
			});
			StartStopKey stopKey = new StartStopKey(true, new object[]
			{
				DateTime.UtcNow - DeliveredTo.CleanupRange
			});
			using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, deliveredToTable.Table, deliveredToTable.Table.PrimaryKeyIndex, null, null, null, 0, 0, new KeyRange(startKey, stopKey), false, false), false))
			{
				int num = (int)deleteOperator.ExecuteScalar();
			}
			completed = true;
		}

		internal static void RemoveAllEntriesForMailbox(Context context, Mailbox mailbox)
		{
			DeliveredToTable deliveredToTable = DatabaseSchema.DeliveredToTable(mailbox.Database);
			SearchCriteriaCompare restriction = Factory.CreateSearchCriteriaCompare(deliveredToTable.MailboxNumber, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(mailbox.MailboxNumber));
			using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, deliveredToTable.Table, deliveredToTable.Table.PrimaryKeyIndex, null, restriction, null, 0, 0, KeyRange.AllRows, false, false), false))
			{
				int num = (int)deleteOperator.ExecuteScalar();
			}
		}

		public static bool AlreadyDelivered(Context context, Mailbox mailbox, DateTime submitTime, ExchangeId folderId, string messageId)
		{
			bool result = false;
			DeliveredToTable deliveredToTable = DatabaseSchema.DeliveredToTable(mailbox.Database);
			long deliveredToHash = DeliveredTo.GetDeliveredToHash(messageId, folderId);
			DeliveredTo.SanitizeSubmitTime(messageId, ref submitTime);
			using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, deliveredToTable.Table, true, new ColumnValue[]
			{
				new ColumnValue(deliveredToTable.MailboxNumber, mailbox.MailboxNumber),
				new ColumnValue(deliveredToTable.SubmitTime, submitTime),
				new ColumnValue(deliveredToTable.MessageIdHash, deliveredToHash)
			}))
			{
				if (dataRow != null)
				{
					result = true;
				}
			}
			return result;
		}

		public static void AddToDeliveredToTable(Context context, Mailbox mailbox, DateTime submitTime, ExchangeId folderId, string messageId)
		{
			DeliveredToTable deliveredToTable = DatabaseSchema.DeliveredToTable(mailbox.Database);
			long deliveredToHash = DeliveredTo.GetDeliveredToHash(messageId, folderId);
			DeliveredTo.SanitizeSubmitTime(messageId, ref submitTime);
			if (submitTime > DateTime.UtcNow.Add(DeliveredTo.driftThreshold))
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_InvalidSubmitTime, new object[]
				{
					messageId,
					submitTime
				});
				return;
			}
			using (DataRow dataRow = Factory.CreateDataRow(context.Culture, context, deliveredToTable.Table, true, new ColumnValue[]
			{
				new ColumnValue(deliveredToTable.MailboxNumber, mailbox.MailboxNumber),
				new ColumnValue(deliveredToTable.SubmitTime, submitTime),
				new ColumnValue(deliveredToTable.MessageIdHash, deliveredToHash)
			}))
			{
				dataRow.Flush(context);
			}
		}

		public static long GetDeliveredToHash(string messageId, ExchangeId folderId)
		{
			if (string.IsNullOrEmpty(messageId))
			{
				return 0L;
			}
			long num = (folderId != ExchangeId.Null) ? folderId.ToLong() : 0L;
			for (int i = 0; i < messageId.Length; i++)
			{
				long num2 = (long)((ulong)messageId[i]);
				num ^= num2;
				for (int j = 0; j < 8; j++)
				{
					long num3 = num & 1L;
					num >>= 1;
					if (0L != num3)
					{
						num ^= -3932672073523589310L;
					}
				}
			}
			return num;
		}

		private static void SanitizeSubmitTime(string messageId, ref DateTime submitTime)
		{
			if (messageId.StartsWith("ed590c4ca1674effa0067475ab2b93b2_", StringComparison.OrdinalIgnoreCase))
			{
				submitTime = DateTime.MinValue;
				return;
			}
			submitTime.AddMilliseconds((double)(-(double)submitTime.Millisecond));
		}

		public static readonly Guid DeliveredToCleanupMaintenanceId = new Guid("{f6f50b68-76c8-4b41-865f-e984022602ac}");

		internal static TimeSpan CleanupRange = TimeSpan.FromDays(7.0);

		private static IDatabaseMaintenance deliveredToCleanupMaintenance;

		private static TimeSpan driftThreshold = TimeSpan.FromDays(7.0);
	}
}
