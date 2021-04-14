using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class FirstTimeSyncProvider : MailboxSyncProvider
	{
		public FirstTimeSyncProvider(Folder folder, bool trackReadFlagChanges, bool trackAssociatedMessageChanges, bool trackConversations, bool allowTableRestrict) : base(folder, trackReadFlagChanges, trackAssociatedMessageChanges, true, trackConversations, allowTableRestrict, AirSyncDiagnostics.GetSyncLogger())
		{
		}

		public FirstTimeSyncProvider(Folder folder, bool trackReadFlagChanges, bool trackAssociatedMessageChanges, bool trackConversations, bool allowTableRestrict, bool shouldDisposeFolder) : base(folder, trackReadFlagChanges, trackAssociatedMessageChanges, true, trackConversations, allowTableRestrict, shouldDisposeFolder, AirSyncDiagnostics.GetSyncLogger())
		{
		}

		private ExDateTime? GetFilterTypeBoundary(QueryFilter filter)
		{
			CompositeFilter compositeFilter = filter as CompositeFilter;
			if (compositeFilter == null)
			{
				return this.GetFilterTypeBoundary(filter as ComparisonFilter);
			}
			ExDateTime? result = null;
			foreach (QueryFilter queryFilter in compositeFilter.Filters)
			{
				result = this.GetFilterTypeBoundary(queryFilter as ComparisonFilter);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		private ExDateTime? GetFilterTypeBoundary(ComparisonFilter comparisonFilter)
		{
			if (comparisonFilter != null && comparisonFilter.Property == ItemSchema.ReceivedTime)
			{
				AirSyncDiagnostics.TraceInfo<ExDateTime>(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.GetFilterTypeBoundary] ReceivedTime boundary: {0}", (ExDateTime)comparisonFilter.PropertyValue);
				return new ExDateTime?((ExDateTime)comparisonFilter.PropertyValue);
			}
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.GetFilterTypeBoundary] Filter was not on received time, but on: {0}", (comparisonFilter == null) ? "<NULL>" : comparisonFilter.Property.Name);
			return null;
		}

		public List<SyncCommand.BadItem> BadItems { get; private set; }

		public StoreObjectId ABQMailId { get; set; }

		public override bool GetNewOperations(ISyncWatermark minSyncWatermark, ISyncWatermark maxSyncWatermark, bool enumerateDeletes, int numOperations, QueryFilter filter, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			if (maxSyncWatermark != null)
			{
				throw new NotSupportedException("First Time Sync behavior needs to be accessed via the FirstTimeSync method.  Only call GetNewOperations for ICS changes.");
			}
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.GetNewOperations] Getting new ICS operations");
			return base.GetNewOperations(minSyncWatermark, maxSyncWatermark, enumerateDeletes, numOperations, filter, newServerManifest);
		}

		private QueryResult GetPositionedQueryResult(FirstTimeSyncWatermark minWatermark, ExDateTime? filterCutoffDate, ICollection<PropertyDefinition> propDefs, out int rowsToFetch)
		{
			return FirstTimeSyncProvider.GetPositionedQueryResult(this.folder, this.ABQMailId, minWatermark, filterCutoffDate, propDefs, this.trackAssociatedMessageChanges, out rowsToFetch);
		}

		internal static QueryResult GetPositionedQueryResult(Folder folderToQuery, StoreObjectId abqMailId, FirstTimeSyncWatermark minWatermark, ExDateTime? filterCutoffDate, ICollection<PropertyDefinition> propDefs, bool trackAssociatedMessageChanges, out int rowsToFetch)
		{
			AirSyncDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.SyncProcessTracer, null, "[FirstTimeSyncProvider.GetPositionedQueryResult] Folder: {0}, filterCutoffDate: {1}, minWatermark: {2}", folderToQuery.DisplayName, (filterCutoffDate != null) ? filterCutoffDate.Value.ToString() : "<NULL>", minWatermark.IsNew ? "<NEW>" : minWatermark.ReceivedDateUtc.ToString());
			ComparisonFilter comparisonFilter = null;
			rowsToFetch = -1;
			if (abqMailId != null)
			{
				AirSyncDiagnostics.TraceDebug<StoreObjectId>(ExTraceGlobals.SyncProcessTracer, null, "[FirstTimeSyncProvider.GetPositionedQueryResult] ABQMail Id is not null.  Limiting result set to item: {0}", abqMailId);
				comparisonFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Id, abqMailId);
				rowsToFetch = 1;
			}
			else if (!minWatermark.IsNew)
			{
				AirSyncDiagnostics.TraceDebug<ExDateTime>(ExTraceGlobals.SyncProcessTracer, null, "[FirstTimeSyncProvider.GetPositionedQueryResult] Watermark is not new. Starting watermark received date: {0}", minWatermark.ReceivedDateUtc);
				comparisonFilter = new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ItemSchema.ReceivedTime, minWatermark.ReceivedDateUtc);
			}
			else
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.SyncProcessTracer, null, "[FirstTimeSyncProvider.GetPositionedQueryResult] Watermark is NEW.");
			}
			SortBy[] sortColumns = FirstTimeSyncProvider.newestFirstSortOrder;
			QueryResult queryResult = null;
			bool flag = false;
			try
			{
				ItemQueryType itemQueryType = ItemQueryType.None;
				if (trackAssociatedMessageChanges)
				{
					itemQueryType |= ItemQueryType.Associated;
				}
				queryResult = folderToQuery.ItemQuery(itemQueryType, null, sortColumns, propDefs);
				if (comparisonFilter != null)
				{
					if (!queryResult.SeekToCondition(SeekReference.OriginBeginning, comparisonFilter))
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.SyncProcessTracer, null, "[FirstTimeSyncProvider.GetPositionedQueryResult] Did not find any matching items for our initial seekToFilter.");
						rowsToFetch = 0;
						flag = true;
						return queryResult;
					}
					AirSyncDiagnostics.TraceDebug<int, int>(ExTraceGlobals.SyncProcessTracer, null, "[FirstTimeSyncProvider.GetPositionedQueryResult] Done seeking to condition.  Current Row: {0}, Estimated Row Count: {1}", queryResult.CurrentRow, queryResult.EstimatedRowCount);
				}
				else
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.SyncProcessTracer, null, "[FirstTimeSyncProvider.GetPositionedQueryResult] No seekToFilter, positioning at beginning.");
					queryResult.SeekToOffset(SeekReference.OriginBeginning, 0);
				}
				if (abqMailId == null)
				{
					int currentRow = queryResult.CurrentRow;
					if (filterCutoffDate != null && currentRow >= 0)
					{
						ComparisonFilter seekFilter = new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.ReceivedTime, filterCutoffDate.Value);
						bool flag2 = queryResult.SeekToCondition(SeekReference.OriginCurrent, seekFilter);
						int num = queryResult.CurrentRow - (flag2 ? 1 : 0);
						rowsToFetch = num - currentRow + 1;
						if (rowsToFetch < 0)
						{
							rowsToFetch = 0;
						}
						AirSyncDiagnostics.TraceDebug<bool, int>(ExTraceGlobals.SyncProcessTracer, null, "[FirstTimeSyncProvider.GetPositionedQueryResult] Finding ending condition. Found? {0}.  Rows to fetch: {1}", flag2, rowsToFetch);
						queryResult.SeekToOffset(SeekReference.OriginBeginning, currentRow);
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag && queryResult != null)
				{
					queryResult.Dispose();
				}
			}
			return queryResult;
		}

		public bool FirstTimeSync(Dictionary<ISyncItemId, FolderSync.ClientStateInformation> clientManifest, FirstTimeSyncWatermark minWatermark, QueryFilter filter, int numOperations, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			ExDateTime receivedDateUtc = minWatermark.ReceivedDateUtc;
			if (numOperations < 0 && numOperations != -1)
			{
				throw new ArgumentException("numOperations is not valid, value = " + numOperations);
			}
			AirSyncDiagnostics.TraceInfo<int, QueryFilter>(ExTraceGlobals.SyncProcessTracer, this, "FirstTimeSyncProvider.FirstTimeSync. numOperations = {0} filter = {1}", numOperations, filter);
			ExDateTime? filterTypeBoundary = this.GetFilterTypeBoundary(filter);
			MailboxSyncPropertyBag mailboxSyncPropertyBag = new MailboxSyncPropertyBag(FirstTimeSyncProvider.newQueryProps);
			if (filter != null)
			{
				mailboxSyncPropertyBag.AddColumnsFromFilter(filter);
			}
			int num;
			using (QueryResult positionedQueryResult = this.GetPositionedQueryResult(minWatermark, filterTypeBoundary, mailboxSyncPropertyBag.Columns, out num))
			{
				bool flag = false;
				while (!flag)
				{
					if (newServerManifest.Count >= numOperations && numOperations > 0)
					{
						return !flag;
					}
					int num2;
					if (numOperations == -1)
					{
						num2 = 10000;
					}
					else
					{
						int num3 = numOperations - newServerManifest.Count;
						num2 = num3 + 1;
					}
					if (num2 < 0)
					{
						throw new InvalidOperationException(ServerStrings.ExNumberOfRowsToFetchInvalid(num2.ToString()));
					}
					bool flag2 = false;
					if (num >= 0 && num < num2)
					{
						AirSyncDiagnostics.TraceDebug<int, int>(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvier.FirstTimeSync] Limiting rows to fetch based on end filter. Original value: {0}, Filter Based Value: {1}", num2, num + 1);
						num2 = num;
						flag2 = true;
					}
					if (num2 <= 0)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.FirstTimeSync] No more rows to fetch. Bailing out.");
						flag = true;
						return false;
					}
					AirSyncDiagnostics.TraceInfo<int>(ExTraceGlobals.SyncProcessTracer, this, "FirstTimeSyncProvider.FirstTimeSync. Asking queryResult for {0} rows", num2);
					IStorePropertyBag[] propertyBags = positionedQueryResult.GetPropertyBags(num2);
					AirSyncDiagnostics.TraceInfo<int>(ExTraceGlobals.SyncProcessTracer, this, "FirstTimeSyncProvider.FirstTimeSync. Retrieved {0} rows", propertyBags.Length);
					if (num > -1)
					{
						num -= propertyBags.Length;
					}
					flag = (propertyBags.Length == 0 || (flag2 && num == 0));
					if (propertyBags.Length < num2)
					{
						flag |= (positionedQueryResult.CurrentRow == positionedQueryResult.EstimatedRowCount);
					}
					for (int i = 0; i < propertyBags.Length; i++)
					{
						VersionedId versionedId = null;
						IStorePropertyBag storePropertyBag = propertyBags[i];
						try
						{
							if (numOperations != -1 && newServerManifest.Count >= numOperations)
							{
								return true;
							}
							if (!this.TryGetPropertyFromBag<VersionedId>(storePropertyBag, ItemSchema.Id, out versionedId))
							{
								AirSyncDiagnostics.TraceError(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.FirstTimeSync] Could not get versioned Id from property bag.  Skipping item.");
							}
							else
							{
								ExDateTime exDateTime;
								bool flag3 = this.TryGetPropertyFromBag<ExDateTime>(storePropertyBag, ItemSchema.ReceivedTime, out exDateTime);
								if (!flag3 && filterTypeBoundary != null)
								{
									AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.SyncProcessTracer, this, "FirstTimeSyncProvider.FirstTimeSync. Found item with missing/corrupt DateTimeReceived. Bailing out of FirstTimeSync mode. Mailbox: {0} ItemId: {1}", this.folder.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), versionedId.ObjectId.ToBase64String());
									flag = true;
									return false;
								}
								ExDateTime value = exDateTime;
								if (!flag3 && !this.TryGetPropertyFromBag<ExDateTime>(storePropertyBag, StoreObjectSchema.CreationTime, out value))
								{
									AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.SyncProcessTracer, this, "FirstTimeSyncProvider.FirstTimeSync. Found item with missing/corrupt CreationDate. Will use DateTime.MinValue instead.  Mailbox: {0}, ItemId: {1}", this.folder.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), versionedId.ObjectId.ToBase64String());
								}
								if (filterTypeBoundary != null && exDateTime < filterTypeBoundary)
								{
									AirSyncDiagnostics.TraceDebug<ExDateTime>(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.FirstTimeSync]  Passed our sync cutoff date of {0}. All rows fetched.", filterTypeBoundary.Value);
									flag = true;
									return false;
								}
								int num4;
								if (!this.TryGetPropertyFromBag<int>(storePropertyBag, ItemSchema.ArticleId, out num4))
								{
									AirSyncDiagnostics.TraceDebug<VersionedId>(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.FirstTimeSync] Unable to get articleId from property bag item. Id: {0}", versionedId);
								}
								else
								{
									ISyncItemId syncItemId = MailboxSyncItemId.CreateForNewItem(versionedId.ObjectId);
									FolderSync.ClientStateInformation clientStateInformation;
									if (clientManifest != null && clientManifest.TryGetValue(syncItemId, out clientStateInformation) && clientStateInformation.ClientHasItem)
									{
										AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.FirstTimeSync] Skipping item {0} because it is already in client manifest.", syncItemId.NativeId.ToString());
										minWatermark.ChangeNumber = num4;
										minWatermark.ReceivedDateUtc = exDateTime;
									}
									else if (filter != null && !EvaluatableFilter.Evaluate(filter, storePropertyBag))
									{
										AirSyncDiagnostics.TraceInfo<StoreObjectId, ExDateTime, int>(ExTraceGlobals.SyncTracer, this, "[FirstTimeSyncProvider.FirstTimeSync] Dropping item '{0}' because it failed to match our filter. ReceivedDateUtc: {1}, ChangeNumber: {2}", versionedId.ObjectId, exDateTime, num4);
										minWatermark.ChangeNumber = num4;
										minWatermark.ReceivedDateUtc = exDateTime;
									}
									else
									{
										bool read;
										if (!this.TryGetPropertyFromBag<bool>(storePropertyBag, MessageItemSchema.IsRead, out read))
										{
											AirSyncDiagnostics.TraceDebug(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.FirstTimeSync] Unabled to get read flag from property bag. Default to false.");
											read = false;
										}
										MailboxSyncWatermark mailboxSyncWatermark = MailboxSyncWatermark.Create();
										mailboxSyncWatermark.UpdateWithChangeNumber(num4, read);
										AirSyncDiagnostics.TraceInfo(ExTraceGlobals.SyncTracer, this, "FirstTimeSyncProvider.FirstTimeSync. Adding Sync'd item {0} to manifest", new object[]
										{
											syncItemId.NativeId
										});
										ServerManifestEntry serverManifestEntry = base.CreateItemChangeManifestEntry(syncItemId, mailboxSyncWatermark);
										ConversationId conversationId = null;
										if (this.TryGetPropertyFromBag<ConversationId>(storePropertyBag, ItemSchema.ConversationId, out conversationId))
										{
											serverManifestEntry.ConversationId = conversationId;
										}
										byte[] bytes;
										ConversationIndex index;
										if (this.TryGetPropertyFromBag<byte[]>(storePropertyBag, ItemSchema.ConversationIndex, out bytes) && ConversationIndex.TryCreate(bytes, out index) && index != ConversationIndex.Empty && index.Components != null && index.Components.Count == 1)
										{
											AirSyncDiagnostics.TraceInfo(ExTraceGlobals.SyncTracer, this, "FirstTimeSyncProvider.FirstTimeSync. First message in conversation.", new object[]
											{
												syncItemId.NativeId
											});
											serverManifestEntry.FirstMessageInConversation = true;
										}
										serverManifestEntry.FilterDate = new ExDateTime?(value);
										string text;
										if (this.TryGetPropertyFromBag<string>(storePropertyBag, StoreObjectSchema.ItemClass, out text))
										{
											serverManifestEntry.MessageClass = (storePropertyBag[StoreObjectSchema.ItemClass] as string);
										}
										newServerManifest[serverManifestEntry.Id] = serverManifestEntry;
										minWatermark.ChangeNumber = num4;
										minWatermark.ReceivedDateUtc = exDateTime;
									}
								}
							}
						}
						catch (Exception arg)
						{
							AirSyncDiagnostics.TraceError<string, Exception>(ExTraceGlobals.SyncTracer, this, "FirstTimeSyncProvider.FirstTimeSync. Caught exception processing item: {0}. Exception: {1}", (versionedId == null) ? "<NULL>" : versionedId.ToBase64String(), arg);
							throw;
						}
					}
				}
			}
			return false;
		}

		private bool TryGetPropertyFromBag<T>(IStorePropertyBag propertyBag, PropertyDefinition propDef, out T value)
		{
			object obj = null;
			try
			{
				obj = propertyBag.TryGetProperty(propDef);
			}
			catch (NotInBagPropertyErrorException)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.TryGetPropertyFromBag] Caught NotInBag exception for property {0}.  Returning default for type.", propDef.Name);
				value = default(T);
				return false;
			}
			if (obj is T)
			{
				value = (T)((object)obj);
				return true;
			}
			PropertyError propertyError = obj as PropertyError;
			if (propertyError != null)
			{
				AirSyncDiagnostics.TraceDebug<Type, string, PropertyErrorCode>(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.TryGetPropertyFromBag] Expected property of type {0} in bag for propDef {1}, but encountered error {2}.", typeof(T), propDef.Name, propertyError.PropertyErrorCode);
			}
			else
			{
				try
				{
					value = (T)((object)obj);
					return true;
				}
				catch (InvalidCastException ex)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.SyncProcessTracer, this, "[FirstTimeSyncProvider.TryGetPropertyFromBag] Tried to cast property '{0}' with value '{1}' to type '{2}', but the cast failed with error '{3}'.", new object[]
					{
						propDef.Name,
						(obj == null) ? "<NULL>" : obj,
						typeof(T).FullName,
						ex
					});
				}
			}
			value = default(T);
			return false;
		}

		public ISyncWatermark GetNewFirstTimeSyncWatermark()
		{
			return FirstTimeSyncWatermark.CreateNew();
		}

		private static readonly SortBy[] newestFirstSortOrder = new SortBy[]
		{
			new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
		};

		private static readonly PropertyDefinition[] badItemProps = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			StoreObjectSchema.CreationTime,
			StoreObjectSchema.ParentItemId,
			ItemSchema.Subject,
			StoreObjectSchema.DisplayName
		};

		private static readonly PropertyDefinition[] newQueryProps = new PropertyDefinition[]
		{
			ItemSchema.ArticleId,
			ItemSchema.Id,
			MessageItemSchema.IsRead,
			ItemSchema.ConversationId,
			ItemSchema.ConversationIndex,
			ItemSchema.ReceivedTime,
			StoreObjectSchema.ItemClass,
			StoreObjectSchema.CreationTime
		};
	}
}
