using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class AirSyncSyncStateTypeFactory
	{
		internal static void EnsureSyncStateTypesRegistered()
		{
			if (!AirSyncSyncStateTypeFactory.initialized)
			{
				lock (AirSyncSyncStateTypeFactory.globalLock)
				{
					if (!AirSyncSyncStateTypeFactory.initialized)
					{
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new FolderIdMapping());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new ItemIdMapping());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new PingCommand.DPFolderInfo());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new GenericDictionaryData<StringData, string, BooleanData, bool>());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, DateTimeCustomSyncFilter.FilterState>());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new GenericDictionaryData<StringData, string, PingCommand.DPFolderInfo>());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new GenericListData<GenericDictionaryData<MailboxLogDataNameData, MailboxLogDataName, StringData, string>, Dictionary<MailboxLogDataName, string>>());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new NullableData<DateTimeData, ExDateTime>());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new ArrayData<StringData, string>());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new GenericListData<StringData, string>());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new RecipientInfoCacheSyncItemId());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new RecipientInfoCacheSyncWatermark());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new FolderTree());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new VirtualFolderItemId());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new GenericListData<Int32Data, int>());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new DeviceBehaviorData());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new GenericListData<DateTimeData, ExDateTime>());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new NullableData<BooleanData, bool>());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new MeetingOrganizerInfoData());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new MeetingOrganizerEntryData());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new FirstTimeSyncWatermark());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new AirSyncCalendarSyncState());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new EntitySyncItemId());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new GenericListData<DerivedData<ISyncItemId>, ISyncItemId>());
						SyncStateTypeFactory.GetInstance().RegisterBuilder(new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, EntityFolderSync.OcurrenceInformation>());
						AirSyncSyncStateTypeFactory.initialized = true;
					}
				}
			}
		}

		private static bool initialized;

		private static object globalLock = new object();
	}
}
