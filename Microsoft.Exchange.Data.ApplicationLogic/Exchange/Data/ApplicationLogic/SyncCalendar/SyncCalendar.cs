using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.ApplicationLogic.SyncCalendar
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncCalendar
	{
		public SyncCalendar(CalendarSyncState syncState, StoreSession session, StoreObjectId folderId, SyncCalendar.GetPropertiesToFetchDelegate getPropertiesToFetchDelegate, ExDateTime windowStart, ExDateTime windowEnd, bool includeAdditionalDataInResponse, int maxChangesReturned)
		{
			if (syncState == null)
			{
				throw new ArgumentNullException("syncState");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (getPropertiesToFetchDelegate == null)
			{
				throw new ArgumentNullException("getPropertiesToFetchDelegate");
			}
			if (windowEnd < windowStart)
			{
				throw new ArgumentException("Window end time must be greater than start time");
			}
			if (windowStart.AddYears(100) < windowEnd)
			{
				throw new ArgumentException("Window time range is too large: " + (windowEnd - windowStart));
			}
			if (maxChangesReturned < 0)
			{
				throw new ArgumentException("maxChangesReturned < 0: " + maxChangesReturned);
			}
			this.syncState = syncState;
			this.session = session;
			this.folderId = folderId;
			this.getPropertiesToFetchDelegate = getPropertiesToFetchDelegate;
			this.windowStart = windowStart;
			this.windowEnd = windowEnd;
			this.includeAdditionalDataInResponse = includeAdditionalDataInResponse;
			if (maxChangesReturned == 0 || maxChangesReturned > 200)
			{
				this.maxChangesReturned = 200;
				return;
			}
			this.maxChangesReturned = maxChangesReturned;
		}

		public SyncCalendarResponse Execute(out IFolderSyncState newSyncState, out IList<KeyValuePair<StoreId, LocalizedException>> caughtExceptions)
		{
			ExTraceGlobals.SyncCalendarTracer.TraceDebug((long)this.GetHashCode(), "XsoSyncCalendar.Execute: Start");
			Stopwatch stopwatch = Stopwatch.StartNew();
			caughtExceptions = new List<KeyValuePair<StoreId, LocalizedException>>();
			MailboxSyncProviderFactory mailboxSyncProviderFactory = new MailboxSyncProviderFactory(this.session, this.folderId);
			HashSet<StoreId> syncItemsHashSet = new HashSet<StoreId>();
			List<SyncCalendarItemType> updatedItemsList = new List<SyncCalendarItemType>();
			List<SyncCalendarItemType> recurrenceMastersWithInstances = new List<SyncCalendarItemType>();
			List<SyncCalendarItemType> recurrenceMastersWithoutInstances = new List<SyncCalendarItemType>();
			Dictionary<StoreId, SyncCalendarItemType> unchangedRecurrenceMastersWithInstances = new Dictionary<StoreId, SyncCalendarItemType>();
			List<StoreId> deletedItemsList = new List<StoreId>();
			bool flag = true;
			CalendarViewQueryResumptionPoint calendarViewQueryResumptionPoint = null;
			using (ISyncProvider syncProvider = mailboxSyncProviderFactory.CreateSyncProvider(null))
			{
				newSyncState = this.syncState.CreateFolderSyncState(this.folderId, syncProvider);
				ExDateTime value;
				if (CalendarSyncState.IsEmpty(this.syncState) || this.syncState.OldWindowEnd == null || this.windowStart >= this.syncState.OldWindowEnd.Value)
				{
					ExTraceGlobals.SyncCalendarTracer.TraceDebug((long)this.GetHashCode(), "XsoSyncCalendar.InternalExecute: Requesting catch-up sync state from ICS");
					newSyncState.Watermark = syncProvider.GetMaxItemWatermark(newSyncState.Watermark);
					value = this.windowStart;
				}
				else
				{
					value = this.syncState.OldWindowEnd.Value;
				}
				if (newSyncState.Watermark != null)
				{
					int num = this.maxChangesReturned;
					int num2 = 0;
					int num3 = 0;
					if (this.windowEnd > value)
					{
						calendarViewQueryResumptionPoint = this.DoQuerySync(syncItemsHashSet, updatedItemsList, recurrenceMastersWithInstances, unchangedRecurrenceMastersWithInstances, value, this.windowEnd, num, caughtExceptions, out num2);
						flag = !calendarViewQueryResumptionPoint.IsEmpty;
						num = this.CalculateRemainingItemsCount(num, num2);
						ExTraceGlobals.SyncCalendarTracer.TraceDebug<int, int, bool>((long)this.GetHashCode(), "XsoSyncCalendar.DoQuerySync added {0} items to the sync response. Remaining Items: {1}; More Available: {2}", num2, num, flag);
					}
					if (num != 0)
					{
						flag = this.DoIcsSync(syncItemsHashSet, updatedItemsList, recurrenceMastersWithInstances, recurrenceMastersWithoutInstances, unchangedRecurrenceMastersWithInstances, deletedItemsList, newSyncState.Watermark, syncProvider, num, caughtExceptions, out num3);
						ExTraceGlobals.SyncCalendarTracer.TraceDebug<int, bool>((long)this.GetHashCode(), "XsoSyncCalendar.DoIcsSync added {0} items to the sync response. More Available: {1}", num3, flag);
					}
					else
					{
						ExTraceGlobals.SyncCalendarTracer.TraceDebug<int, int>((long)this.GetHashCode(), "XsoSyncCalendar; Skipping ICS sync, since we've reached the max items requested (Requested: {0}; Actual: {1}).", this.maxChangesReturned, num2);
					}
					ExTraceGlobals.SyncCalendarTracer.TraceDebug<int, int>((long)this.GetHashCode(), "XsoSyncCalendar; Finished fetching items. Total items synced: {0}; Max requested: {1}", num2 + num3, this.maxChangesReturned);
				}
				else
				{
					flag = false;
					ExTraceGlobals.SyncCalendarTracer.TraceDebug((long)this.GetHashCode(), "XsoSyncCalendar; Nothing to sync. The specified folder is empty.");
				}
			}
			SyncCalendarResponse result = this.AssembleResponse(flag ? calendarViewQueryResumptionPoint : null, flag ? this.syncState.OldWindowEnd : new ExDateTime?(this.windowEnd), updatedItemsList, recurrenceMastersWithInstances, recurrenceMastersWithoutInstances, unchangedRecurrenceMastersWithInstances, deletedItemsList, !flag);
			stopwatch.Stop();
			ExTraceGlobals.SyncCalendarTracer.TraceDebug((long)this.GetHashCode(), "XsoSyncCalendar.InternalExecute: End " + stopwatch.ElapsedMilliseconds);
			return result;
		}

		private int CalculateRemainingItemsCount(int previousRemainingItemsCount, int recentlyAddedItemsCount)
		{
			if (recentlyAddedItemsCount <= previousRemainingItemsCount)
			{
				return previousRemainingItemsCount - recentlyAddedItemsCount;
			}
			return 0;
		}

		private CalendarViewQueryResumptionPoint DoQuerySync(HashSet<StoreId> syncItemsHashSet, List<SyncCalendarItemType> updatedItemsList, List<SyncCalendarItemType> recurrenceMastersWithInstances, Dictionary<StoreId, SyncCalendarItemType> unchangedRecurrenceMastersWithInstances, ExDateTime queryWindowStart, ExDateTime queryWindowEnd, int maxCount, IList<KeyValuePair<StoreId, LocalizedException>> caughtExceptions, out int addedItems)
		{
			ExTraceGlobals.SyncCalendarTracer.TraceDebug((long)this.GetHashCode(), "XsoSyncCalendar.DoQuerySync: Start");
			int localAddedItems = 0;
			CalendarViewBatchingStrategy calendarViewBatchingStrategy = (this.syncState.QueryResumptionPoint == null) ? CalendarViewBatchingStrategy.CreateNewBatchingInstance(maxCount) : CalendarViewBatchingStrategy.CreateResumingInstance(maxCount, this.syncState.QueryResumptionPoint);
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(this.session, this.folderId))
			{
				PropertyDefinition[] xsoRequiredProperties = this.getPropertiesToFetchDelegate(calendarFolder);
				object[][] syncView = calendarFolder.GetSyncView(queryWindowStart, queryWindowEnd, calendarViewBatchingStrategy, xsoRequiredProperties, false);
				object[][] array = syncView;
				for (int i = 0; i < array.Length; i++)
				{
					object[] itemRow = array[i];
					try
					{
						GrayException.MapAndReportGrayExceptions(delegate()
						{
							ExDateTime t;
							SyncCalendarItemType typedItem = this.GetTypedItem(xsoRequiredProperties, itemRow, out t);
							if (typedItem.CalendarItemType != CalendarItemType.RecurringMaster)
							{
								if (this.windowStart == queryWindowStart || t >= queryWindowStart)
								{
									localAddedItems += this.AddSyncItem(typedItem, updatedItemsList, syncItemsHashSet, unchangedRecurrenceMastersWithInstances, false);
								}
								return;
							}
							bool flag = false;
							if (this.windowStart == queryWindowStart)
							{
								flag = true;
							}
							else
							{
								using (CalendarItem calendarItem = CalendarItem.Bind(this.session, typedItem.ItemId))
								{
									IList<OccurrenceInfo> occurrenceInfoList = calendarItem.Recurrence.GetOccurrenceInfoList(this.windowStart, queryWindowStart);
									flag = (occurrenceInfoList.Count == 0);
								}
							}
							if (flag)
							{
								localAddedItems += this.AddSyncItem(typedItem, recurrenceMastersWithInstances, syncItemsHashSet, unchangedRecurrenceMastersWithInstances, false);
								return;
							}
							localAddedItems += this.AddSyncItem(typedItem, null, syncItemsHashSet, unchangedRecurrenceMastersWithInstances, true);
						}, new GrayException.IsGrayExceptionDelegate(GrayException.IsSystemGrayException));
					}
					catch (LocalizedException ex)
					{
						StoreId storeId = (StoreId)itemRow[0];
						ExTraceGlobals.SyncCalendarTracer.TraceWarning<string, LocalizedException>((long)this.GetHashCode(), "XsoSyncCalendar.DoQuerySync: Exception thrown while processing item {0}: {1}", storeId.ToBase64String(), ex);
						caughtExceptions.Add(new KeyValuePair<StoreId, LocalizedException>(storeId, ex));
					}
				}
			}
			ExTraceGlobals.SyncCalendarTracer.TraceDebug((long)this.GetHashCode(), "XsoSyncCalendar.DoQuerySync: End");
			addedItems = localAddedItems;
			return calendarViewBatchingStrategy.ResumptionPoint;
		}

		private bool DoIcsSync(HashSet<StoreId> syncItemsHashSet, IList<SyncCalendarItemType> updatedItemsList, IList<SyncCalendarItemType> recurrenceMastersWithInstances, IList<SyncCalendarItemType> recurrenceMastersWithoutInstances, IDictionary<StoreId, SyncCalendarItemType> unchangedRecurrenceMastersWithInstances, IList<StoreId> deletedItemsList, ISyncWatermark watermark, ISyncProvider syncProvider, int maxCount, IList<KeyValuePair<StoreId, LocalizedException>> caughtExceptions, out int addedItems)
		{
			ExTraceGlobals.SyncCalendarTracer.TraceDebug((long)this.GetHashCode(), "XsoSyncCalendar.DoIcsSync: Start");
			addedItems = 0;
			int num = (maxCount == 1) ? 1 : ((int)(0.8 * (double)maxCount));
			Dictionary<ISyncItemId, ServerManifestEntry> dictionary = new Dictionary<ISyncItemId, ServerManifestEntry>();
			bool flag = false;
			do
			{
				ExTraceGlobals.SyncCalendarTracer.TraceDebug<int>((long)this.GetHashCode(), "XsoSyncCalendar.DoIcsSync: Requesting {0} changes from ICS", 100);
				int numOperations = Math.Min(num - addedItems, 100);
				flag = syncProvider.GetNewOperations(watermark, null, true, numOperations, null, dictionary);
				ExTraceGlobals.SyncCalendarTracer.TraceDebug<int>((long)this.GetHashCode(), "XsoSyncCalendar.DoIcsSync: Received {0} entries from ICS", dictionary.Count);
				foreach (ServerManifestEntry serverManifestEntry in dictionary.Values)
				{
					StoreObjectId id = (StoreObjectId)serverManifestEntry.Id.NativeId;
					switch (serverManifestEntry.ChangeType)
					{
					case ChangeType.Add:
					case ChangeType.Change:
						addedItems += this.AddNewOrChangedItems(id, updatedItemsList, recurrenceMastersWithInstances, recurrenceMastersWithoutInstances, unchangedRecurrenceMastersWithInstances, deletedItemsList, syncItemsHashSet, caughtExceptions);
						continue;
					case ChangeType.Delete:
						addedItems += this.AddDeletedItem(id, syncItemsHashSet, deletedItemsList);
						continue;
					}
					ExTraceGlobals.SyncCalendarTracer.TraceWarning<ChangeType>((long)this.GetHashCode(), "XsoSyncCalendar.DoIcsSyncs unknown/unexpected sync change type {0}", serverManifestEntry.ChangeType);
				}
			}
			while (flag && num > addedItems);
			ExTraceGlobals.SyncCalendarTracer.TraceDebug((long)this.GetHashCode(), "XsoSyncCalendar.DoIcsSync: End");
			return flag;
		}

		private int AddNewOrChangedItems(StoreId id, IList<SyncCalendarItemType> updatedItemsList, IList<SyncCalendarItemType> recurrenceMastersWithInstances, IList<SyncCalendarItemType> recurrenceMastersWithoutInstances, IDictionary<StoreId, SyncCalendarItemType> unchangedRecurrenceMastersWithInstances, IList<StoreId> deletedItemsList, HashSet<StoreId> syncItemsHashSet, IList<KeyValuePair<StoreId, LocalizedException>> caughtExceptions)
		{
			int addedItems = 0;
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					using (CalendarItem calendarItem = CalendarItem.Bind(this.session, id, new PropertyDefinition[]
					{
						CalendarItemInstanceSchema.StartWallClock,
						CalendarItemInstanceSchema.EndWallClock
					}))
					{
						string uid;
						try
						{
							uid = new GlobalObjectId(calendarItem).Uid;
						}
						catch (Exception arg)
						{
							ExTraceGlobals.SyncCalendarTracer.TraceWarning<StoreId, Exception>((long)this.GetHashCode(), "Skipping the corrupt recurring calendar GlobalObjectId (Id: '{0}'). {1}", id, arg);
							uid = null;
						}
						SyncCalendarItemType typedItem = this.GetTypedItem(calendarItem.Id, uid, calendarItem.CalendarItemType, calendarItem.StartTime, calendarItem.StartWallClock, calendarItem.EndTime, calendarItem.EndWallClock);
						if (calendarItem.CalendarItemType == CalendarItemType.RecurringMaster)
						{
							if (calendarItem.Recurrence == null)
							{
								ExTraceGlobals.SyncCalendarTracer.TraceWarning<StoreId>((long)this.GetHashCode(), "Skipping the corrupt recurring calendar item with no recurrence (Id: '{0}').", id);
							}
							else
							{
								IList<OccurrenceInfo> occurrenceInfoList = calendarItem.Recurrence.GetOccurrenceInfoList(this.windowStart, this.windowEnd);
								if (occurrenceInfoList.Count != 0)
								{
									addedItems += this.AddSyncItem(typedItem, recurrenceMastersWithInstances, syncItemsHashSet, unchangedRecurrenceMastersWithInstances, false);
									using (IEnumerator<OccurrenceInfo> enumerator = occurrenceInfoList.GetEnumerator())
									{
										while (enumerator.MoveNext())
										{
											OccurrenceInfo occurrenceInfo = enumerator.Current;
											CalendarItemType type;
											ExDateTime startWallClock;
											ExDateTime endWallClock;
											if (occurrenceInfo is ExceptionInfo)
											{
												type = CalendarItemType.Exception;
												startWallClock = ExDateTime.MinValue;
												endWallClock = ExDateTime.MinValue;
											}
											else
											{
												type = CalendarItemType.Occurrence;
												startWallClock = calendarItem.StartWallClock.TimeZone.ConvertDateTime(occurrenceInfo.StartTime);
												endWallClock = calendarItem.EndWallClock.TimeZone.ConvertDateTime(occurrenceInfo.EndTime);
											}
											SyncCalendarItemType typedItem2 = this.GetTypedItem(occurrenceInfo.VersionedId, uid, type, occurrenceInfo.StartTime, startWallClock, occurrenceInfo.EndTime, endWallClock);
											addedItems += this.AddSyncItem(typedItem2, updatedItemsList, syncItemsHashSet, unchangedRecurrenceMastersWithInstances, false);
										}
										goto IL_2D1;
									}
								}
								addedItems += this.AddSyncItem(typedItem, recurrenceMastersWithoutInstances, syncItemsHashSet, unchangedRecurrenceMastersWithInstances, false);
							}
						}
						else
						{
							bool flag = this.windowStart < calendarItem.EndTime && this.windowEnd > calendarItem.StartTime;
							if (flag)
							{
								addedItems += this.AddSyncItem(typedItem, updatedItemsList, syncItemsHashSet, unchangedRecurrenceMastersWithInstances, false);
							}
							else
							{
								addedItems += this.AddDeletedItem(id, syncItemsHashSet, deletedItemsList);
							}
						}
						IL_2D1:;
					}
				}, new GrayException.IsGrayExceptionDelegate(GrayException.IsSystemGrayException));
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.SyncCalendarTracer.TraceWarning<string, LocalizedException>((long)this.GetHashCode(), "XsoSyncCalendar.AddNewOrChangedItems: Exception thrown while processing item {0}: {1}", id.ToBase64String(), ex);
				caughtExceptions.Add(new KeyValuePair<StoreId, LocalizedException>(id, ex));
			}
			return addedItems;
		}

		private int AddDeletedItem(StoreId id, HashSet<StoreId> syncItemsHashSet, IList<StoreId> deletedItemsList)
		{
			int num = 0;
			if (!syncItemsHashSet.Contains(id))
			{
				syncItemsHashSet.Add(id);
				deletedItemsList.Add(id);
				num++;
			}
			return num;
		}

		private SyncCalendarResponse AssembleResponse(CalendarViewQueryResumptionPoint queryResumptionPoint, ExDateTime? oldWindowEnd, List<SyncCalendarItemType> updatedItemsList, List<SyncCalendarItemType> recurrenceMastersWithInstances, List<SyncCalendarItemType> recurrenceMastersWithoutInstances, IDictionary<StoreId, SyncCalendarItemType> unchangedRecurrenceMastersWithInstances, List<StoreId> deletedItemsList, bool includesLastItemInRange)
		{
			return new SyncCalendarResponse
			{
				QueryResumptionPoint = queryResumptionPoint,
				OldWindowEnd = oldWindowEnd,
				DeletedItems = deletedItemsList,
				UpdatedItems = updatedItemsList,
				RecurrenceMastersWithInstances = recurrenceMastersWithInstances,
				RecurrenceMastersWithoutInstances = recurrenceMastersWithoutInstances,
				UnchangedRecurrenceMastersWithInstances = new List<SyncCalendarItemType>(unchangedRecurrenceMastersWithInstances.Values),
				IncludesLastItemInRange = includesLastItemInRange
			};
		}

		private int AddSyncItem(SyncCalendarItemType calendarItem, IList<SyncCalendarItemType> syncItemsList, HashSet<StoreId> syncItemsHashSet, IDictionary<StoreId, SyncCalendarItemType> unchangedMasters, bool addToUnchangedMasters)
		{
			int num = 0;
			if (!syncItemsHashSet.Contains(calendarItem.ItemId))
			{
				if (calendarItem.CalendarItemType != CalendarItemType.Occurrence)
				{
					calendarItem.StartWallClock = null;
					calendarItem.EndWallClock = null;
					if (!this.includeAdditionalDataInResponse)
					{
						if (calendarItem.CalendarItemType != CalendarItemType.RecurringMaster)
						{
							calendarItem.UID = null;
						}
						calendarItem.Start = null;
						calendarItem.End = null;
					}
				}
				if (addToUnchangedMasters)
				{
					unchangedMasters.Add(calendarItem.ItemId, calendarItem);
					num++;
				}
				else
				{
					syncItemsList.Add(calendarItem);
					syncItemsHashSet.Add(calendarItem.ItemId);
					if (!unchangedMasters.Remove(calendarItem.ItemId))
					{
						num++;
					}
				}
			}
			return num;
		}

		private SyncCalendarItemType GetTypedItem(StoreId manifestNativeId, string uid, CalendarItemType type, ExDateTime start, ExDateTime startWallClock, ExDateTime end, ExDateTime endWallClock)
		{
			return new SyncCalendarItemType
			{
				ItemId = manifestNativeId,
				UID = uid,
				CalendarItemType = type,
				Start = new ExDateTime?(start),
				StartWallClock = new ExDateTime?(startWallClock),
				End = new ExDateTime?(end),
				EndWallClock = new ExDateTime?(endWallClock)
			};
		}

		private SyncCalendarItemType GetTypedItem(PropertyDefinition[] xsoRequiredProperties, object[] itemRow, out ExDateTime itemStart)
		{
			itemStart = ExDateTime.MinValue;
			StoreId itemId = null;
			string uid = null;
			CalendarItemType calendarItemType = CalendarItemType.Single;
			ExDateTime? startWallClock = null;
			ExDateTime? end = null;
			ExDateTime? endWallClock = null;
			Dictionary<PropertyDefinition, object> dictionary = new Dictionary<PropertyDefinition, object>(itemRow.Length);
			for (int i = 0; i < xsoRequiredProperties.Length; i++)
			{
				if (!(itemRow[i] is PropertyError) && !dictionary.ContainsKey(xsoRequiredProperties[i]))
				{
					if (xsoRequiredProperties[i] == CalendarItemInstanceSchema.StartTime)
					{
						itemStart = (ExDateTime)itemRow[i];
					}
					else if (xsoRequiredProperties[i] == CalendarItemInstanceSchema.StartWallClock)
					{
						startWallClock = new ExDateTime?((ExDateTime)itemRow[i]);
					}
					else if (xsoRequiredProperties[i] == CalendarItemInstanceSchema.EndTime)
					{
						end = new ExDateTime?((ExDateTime)itemRow[i]);
					}
					else if (xsoRequiredProperties[i] == CalendarItemInstanceSchema.EndWallClock)
					{
						endWallClock = new ExDateTime?((ExDateTime)itemRow[i]);
					}
					else if (xsoRequiredProperties[i] == ItemSchema.Id)
					{
						itemId = (VersionedId)itemRow[i];
					}
					else if (xsoRequiredProperties[i] == CalendarItemBaseSchema.CalendarItemType)
					{
						calendarItemType = (CalendarItemType)itemRow[i];
					}
					else if (xsoRequiredProperties[i] == CalendarItemBaseSchema.GlobalObjectId)
					{
						uid = new GlobalObjectId((byte[])itemRow[i]).Uid;
					}
					dictionary.Add(xsoRequiredProperties[i], itemRow[i]);
				}
			}
			return new SyncCalendarItemType
			{
				ItemId = itemId,
				UID = uid,
				CalendarItemType = calendarItemType,
				Start = new ExDateTime?(itemStart),
				StartWallClock = startWallClock,
				End = end,
				EndWallClock = endWallClock,
				RowData = dictionary
			};
		}

		private const int MaxChangesReturnedCap = 200;

		private const int MaximumAllowedIcsBatchSize = 100;

		private const double IcsCutOffPercentage = 0.8;

		private readonly int maxChangesReturned;

		private readonly bool includeAdditionalDataInResponse;

		private CalendarSyncState syncState;

		private StoreSession session;

		private StoreObjectId folderId;

		private SyncCalendar.GetPropertiesToFetchDelegate getPropertiesToFetchDelegate;

		private ExDateTime windowStart;

		private ExDateTime windowEnd;

		public delegate PropertyDefinition[] GetPropertiesToFetchDelegate(CalendarFolder folder);
	}
}
