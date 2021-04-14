using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.SyncCalendar;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EntitySyncProvider : MailboxSyncProvider
	{
		public EntitySyncProvider(Folder folder, bool trackReadFlagChanges, bool trackAssociatedMessageChanges, bool returnNewestFirst, bool trackConversations, bool allowTableRestrict) : this(folder, trackReadFlagChanges, trackAssociatedMessageChanges, returnNewestFirst, trackConversations, allowTableRestrict, true)
		{
		}

		public EntitySyncProvider(Folder folder, bool trackReadFlagChanges, bool trackAssociatedMessageChanges, bool returnNewestFirst, bool trackConversations, bool allowTableRestrict, bool disposeFolder) : base(folder, trackReadFlagChanges, trackAssociatedMessageChanges, returnNewestFirst, trackConversations, allowTableRestrict, disposeFolder, AirSyncDiagnostics.GetSyncLogger())
		{
		}

		protected EntitySyncProvider()
		{
		}

		public ExDateTime WindowStart { get; set; }

		public ExDateTime WindowEnd { get; set; }

		public AirSyncCalendarSyncState CalendarSyncState { get; set; }

		public override ISyncWatermark CreateNewWatermark()
		{
			base.CheckDisposed("CreateNewWatermark");
			return EntitySyncWatermark.Create();
		}

		public override bool GetNewOperations(ISyncWatermark minSyncWatermark, ISyncWatermark maxSyncWatermark, bool enumerateDeletes, int numOperations, QueryFilter filter, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			base.CheckDisposed("GetNewOperations");
			AirSyncDiagnostics.TraceInfo<int>(ExTraceGlobals.RequestsTracer, this, "EntitySyncProvider.GetNewOperations. numOperations = {0}", numOperations);
			if (newServerManifest == null)
			{
				throw new ArgumentNullException("newServerManifest");
			}
			if (!enumerateDeletes)
			{
				throw new NotImplementedException("enumerateDeletes is false!");
			}
			if (filter != null)
			{
				throw new NotImplementedException("filter is non-null! Filters are not supported on EntitySyncProvider");
			}
			SyncCalendar syncCalendar = new SyncCalendar(this.CalendarSyncState, base.Folder.Session, base.Folder.Id.ObjectId, (CalendarFolder folder) => EntitySyncProvider.PropertiesToSync, this.WindowStart, this.WindowEnd, false, numOperations);
			IFolderSyncState folderSyncState;
			IList<KeyValuePair<StoreId, LocalizedException>> list;
			SyncCalendarResponse syncCalendarResponse = syncCalendar.Execute(out folderSyncState, out list);
			AirSyncDiagnostics.TraceInfo<IFolderSyncState>(ExTraceGlobals.RequestsTracer, this, "newSyncState:{0}", folderSyncState);
			SyncCalendarFolderSyncState syncCalendarFolderSyncState = (SyncCalendarFolderSyncState)folderSyncState;
			this.CalendarSyncState = new AirSyncCalendarSyncState(syncCalendarFolderSyncState.SerializeAsBase64String(), syncCalendarResponse.QueryResumptionPoint, syncCalendarResponse.OldWindowEnd);
			if (list.Count > 0 && Command.CurrentCommand.MailboxLogger != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<StoreId, LocalizedException> keyValuePair in list)
				{
					stringBuilder.AppendFormat("Exception caught for item {0}\r\n{1}\r\n\r\n", keyValuePair.Key, keyValuePair.Value);
				}
				Command.CurrentCommand.MailboxLogger.SetData(MailboxLogDataName.CalendarSync_Exception, stringBuilder.ToString());
			}
			AirSyncDiagnostics.TraceInfo<int>(ExTraceGlobals.RequestsTracer, this, "DeletedItems:{0}", syncCalendarResponse.DeletedItems.Count);
			foreach (StoreId storeId in syncCalendarResponse.DeletedItems)
			{
				ISyncItemId syncItemId = EntitySyncItemId.CreateFromId(storeId);
				newServerManifest.Add(syncItemId, new ServerManifestEntry(ChangeType.Delete, syncItemId, null));
			}
			this.CopyListToDictionary(syncCalendarResponse.UpdatedItems, "UpdatedItems", newServerManifest);
			this.CopyListToDictionary(syncCalendarResponse.RecurrenceMastersWithInstances, "RecurrenceMastersWithInstances", newServerManifest);
			this.CopyListToDictionary(syncCalendarResponse.RecurrenceMastersWithoutInstances, "RecurrenceMastersWithoutInstances", newServerManifest);
			this.CopyListToDictionary(syncCalendarResponse.UnchangedRecurrenceMastersWithInstances, "UnchangedRecurrenceMastersWithInstances", newServerManifest);
			AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, this, "MoreAvailable:{0}", !syncCalendarResponse.IncludesLastItemInRange);
			return !syncCalendarResponse.IncludesLastItemInRange;
		}

		public override OperationResult DeleteItems(params ISyncItemId[] syncIds)
		{
			base.CheckDisposed("DeleteItems");
			throw new NotImplementedException("EntitySyncProvider.DeleteItems");
		}

		public override ISyncItemId CreateISyncItemIdForNewItem(StoreObjectId itemId)
		{
			base.CheckDisposed("CreateISyncItemIdForNewItem");
			if (itemId == null)
			{
				throw new ArgumentNullException("itemId");
			}
			return MailboxSyncItemId.CreateForNewItem(itemId);
		}

		protected override ISyncItem GetItem(Item item)
		{
			return EntitySyncItem.Bind(item);
		}

		private void CopyListToDictionary(IList<SyncCalendarItemType> items, string listName, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			AirSyncDiagnostics.TraceInfo<string, int>(ExTraceGlobals.RequestsTracer, this, "{0}:{1}", listName, items.Count);
			foreach (SyncCalendarItemType syncCalendarItemType in items)
			{
				EntitySyncWatermark watermark = null;
				object obj;
				if (syncCalendarItemType.RowData != null && syncCalendarItemType.RowData.TryGetValue(ItemSchema.ArticleId, out obj) && !(obj is PropertyError))
				{
					watermark = EntitySyncWatermark.CreateWithChangeNumber((int)obj);
				}
				ISyncItemId syncItemId = EntitySyncItemId.CreateFromId(syncCalendarItemType.ItemId);
				ServerManifestEntry serverManifestEntry = new ServerManifestEntry(ChangeType.Add, syncItemId, watermark);
				serverManifestEntry.MessageClass = "IPM.APPOINTMENT";
				serverManifestEntry.CalendarItemType = syncCalendarItemType.CalendarItemType;
				OccurrenceStoreObjectId occurrenceStoreObjectId = StoreId.GetStoreObjectId(syncCalendarItemType.ItemId) as OccurrenceStoreObjectId;
				if (occurrenceStoreObjectId != null)
				{
					serverManifestEntry.SeriesMasterId = occurrenceStoreObjectId.GetMasterStoreObjectId();
				}
				newServerManifest.Add(syncItemId, serverManifestEntry);
			}
		}

		public static readonly PropertyDefinition[] PropertiesToSync = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.ArticleId,
			CalendarItemBaseSchema.CalendarItemType
		};
	}
}
