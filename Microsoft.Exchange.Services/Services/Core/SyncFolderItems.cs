using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SyncFolderItems : SingleStepServiceCommand<SyncFolderItemsRequest, SyncFolderItemsChangesType>
	{
		internal override bool SupportsExternalUsers
		{
			get
			{
				return true;
			}
		}

		internal override Offer ExpectedOffer
		{
			get
			{
				return Offer.SharingRead;
			}
		}

		private bool IsClientRequestingTopNChanges
		{
			get
			{
				return base.Request.NumberOfDays > 0 || base.Request.MinimumCount > 0;
			}
		}

		public SyncFolderItems(CallContext callContext, SyncFolderItemsRequest request) : base(callContext, request)
		{
			OwsLogRegistry.Register(base.GetType().Name, typeof(SyncFolderItemsMetadata), new Type[0]);
			this.syncFolderId = base.Request.SyncFolderId.BaseFolderId;
			this.syncStateBase64String = base.Request.SyncState;
			this.responseShape = Global.ResponseShapeResolver.GetResponseShape<ItemResponseShape>(base.Request.ShapeName, base.Request.ItemShape, base.CallContext.FeaturesManager);
			this.ignoreList = ((base.Request.Ignore == null) ? null : base.Request.Ignore.ToList<ItemId>());
			this.maxChangesReturned = base.Request.MaxChangesReturned;
			this.syncScope = base.Request.SyncScope;
			this.optimizeForIdOnly = (this.responseShape.BaseShape == ShapeEnum.IdOnly && (this.responseShape.AdditionalProperties == null || this.responseShape.AdditionalProperties.Length == 0));
			this.optimizeForItemClass = false;
			if (this.responseShape.BaseShape == ShapeEnum.IdOnly && this.responseShape.AdditionalProperties != null && this.responseShape.AdditionalProperties.Length == 1)
			{
				PropertyUri propertyUri = this.responseShape.AdditionalProperties[0] as PropertyUri;
				if (propertyUri != null && propertyUri.Uri == PropertyUriEnum.ItemClass)
				{
					this.optimizeForItemClass = true;
				}
			}
			if (this.responseShape.AdditionalProperties != null && this.responseShape.InferenceEnabled)
			{
				List<PropertyPath> list = new List<PropertyPath>(this.responseShape.AdditionalProperties);
				list.Add(new PropertyUri(PropertyUriEnum.IsClutter));
				this.responseShape.AdditionalProperties = list.ToArray();
			}
			ServiceCommandBase.ThrowIfNull(this.responseShape, "responseShape", "SyncFolderItems");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			BaseInfoResponse baseInfoResponse = new SyncFolderItemsResponse();
			baseInfoResponse.ProcessServiceResult<SyncFolderItemsChangesType>(base.Result);
			return baseInfoResponse;
		}

		private static void CreateItemClassXml(XmlElement classParentXml, string itemClass)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(classParentXml, "ItemClass", ServiceXml.DefaultNamespaceUri);
			xmlElement.InnerText = itemClass;
		}

		internal override ServiceResult<SyncFolderItemsChangesType> Execute()
		{
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			Stopwatch stopwatch3 = new Stopwatch();
			stopwatch.Start();
			base.CallContext.UpdateLastSyncAttemptTime();
			try
			{
				this.syncFolderIdAndSession = base.IdConverter.ConvertTargetFolderIdToIdAndContentSession(this.syncFolderId, false);
				this.session = this.syncFolderIdAndSession.Session;
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new SyncFolderNotFoundException(innerException);
			}
			using (Folder folder = Folder.Bind(this.session, this.syncFolderIdAndSession.Id, null))
			{
				stopwatch2.Start();
				this.DoIcsSync(folder);
				stopwatch2.Stop();
				if (base.Request.DoQuickSync)
				{
					this.changes.IncludesLastItemInRange = false;
				}
				else
				{
					stopwatch3.Start();
					this.DoQuerySync(folder);
					stopwatch3.Stop();
				}
			}
			this.changes.SyncState = this.syncState.SerializeAsBase64String();
			base.CallContext.UpdateLastSyncSuccessTime();
			stopwatch.Stop();
			base.CallContext.ProtocolLog.Set(SyncFolderItemsMetadata.IcsTime, stopwatch2.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncFolderItemsMetadata.QueryTime, stopwatch3.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncFolderItemsMetadata.TotalTime, stopwatch.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncFolderItemsMetadata.SyncStateSize, this.changes.SyncState.Length);
			base.CallContext.ProtocolLog.Set(SyncFolderItemsMetadata.SyncStateHash, this.changes.SyncState.GetHashCode());
			base.CallContext.ProtocolLog.Set(SyncFolderItemsMetadata.ItemCount, this.changes.Count);
			base.CallContext.ProtocolLog.Set(SyncFolderItemsMetadata.IncludesLastItemInRange, this.changes.IncludesLastItemInRange);
			ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug<int, bool>((long)this.GetHashCode(), "SyncFolderItems.Execute: End, {0} items, last = {1}", this.changes.Count, this.changes.IncludesLastItemInRange);
			return new ServiceResult<SyncFolderItemsChangesType>(this.changes);
		}

		private void DoIcsSync(Folder folder)
		{
			ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItems.DoIcsSync: Start");
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			this.processingICSChanges = true;
			if (base.CallContext.IsExternalUser)
			{
				ItemResponseShape itemResponseShape;
				ServiceError serviceError = ExternalUserHandler.CheckAndGetResponseShape(base.GetType(), this.syncFolderIdAndSession as ExternalUserIdAndSession, this.responseShape, out itemResponseShape);
				if (serviceError != null)
				{
					ExTraceGlobals.SyncFolderItemsCallTracer.TraceError<CallContext>((long)this.GetHashCode(), "SyncFolderItems.BuildSyncResponse: Error received when trying to prep the item for external user {0}", base.CallContext);
					throw new ServiceAccessDeniedException();
				}
				this.responseShape = itemResponseShape;
			}
			bool includesLastItemInRange = true;
			this.ignoreItemsDictionary = this.BuildIgnoreItemsDictionary(this.session);
			StoreObjectId asStoreObjectId = this.syncFolderIdAndSession.GetAsStoreObjectId();
			if (this.syncFolderId is DistinguishedFolderId)
			{
				ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(asStoreObjectId, this.syncFolderIdAndSession, null);
				this.syncFolderId = new FolderId(concatenatedId.Id, concatenatedId.ChangeKey);
			}
			MailboxSyncProviderFactory mailboxSyncProviderFactory = new MailboxSyncProviderFactory(this.session, asStoreObjectId);
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010SP2))
			{
				mailboxSyncProviderFactory.ReturnNewestChangesFirst();
			}
			if (asStoreObjectId.ObjectType == StoreObjectType.Folder)
			{
				mailboxSyncProviderFactory.GenerateReadFlagChanges();
			}
			if (this.syncScope == SyncFolderItemsScope.NormalAndAssociatedItems)
			{
				mailboxSyncProviderFactory.GenerateAssociatedMessageChanges();
			}
			try
			{
				using (MailboxSyncProvider mailboxSyncProvider = (MailboxSyncProvider)mailboxSyncProviderFactory.CreateSyncProvider(folder, null))
				{
					this.syncState = new ServicesFolderSyncState(asStoreObjectId, mailboxSyncProvider, this.syncStateBase64String);
					ISyncWatermark syncWatermark = this.syncState.Watermark;
					if (this.IsClientRequestingTopNChanges && string.IsNullOrEmpty(this.syncStateBase64String))
					{
						ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItems.DoIcsSync: Requesting catch-up sync state from ICS");
						stopwatch2.Start();
						syncWatermark = mailboxSyncProvider.GetMaxItemWatermark(syncWatermark);
						stopwatch2.Stop();
					}
					else
					{
						int num = 0;
						bool flag = false;
						do
						{
							Dictionary<ISyncItemId, ServerManifestEntry> dictionary = new Dictionary<ISyncItemId, ServerManifestEntry>();
							stopwatch.Start();
							int numOperations = this.maxChangesReturned - num;
							flag = mailboxSyncProvider.GetNewOperations(syncWatermark, null, true, numOperations, null, dictionary);
							stopwatch.Stop();
							foreach (ServerManifestEntry syncOperation in dictionary.Values)
							{
								SyncFolderItemsChangeTypeBase change;
								if (this.TryBuildFolderItemChangeElement(this.syncFolderIdAndSession, syncOperation, mailboxSyncProvider, out change))
								{
									num++;
									this.changes.AddChange(change);
								}
							}
						}
						while (num < this.maxChangesReturned && flag);
						this.objectsChanged = this.changes.Count;
						includesLastItemInRange = !flag;
						if (flag)
						{
							this.syncState.MoreItemsOnServer = flag;
						}
					}
					this.syncState.Watermark = syncWatermark;
				}
			}
			catch (StoragePermanentException ex)
			{
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
				{
					Exception ex2 = ex.InnerException as MapiExceptionFormatError;
					if (ex2 != null)
					{
						throw new ServiceAccessDeniedException(CoreResources.IDs.MessageInsufficientPermissionsToSync, ex);
					}
				}
				throw;
			}
			this.changes.IncludesLastItemInRange = includesLastItemInRange;
			if (this.IsClientRequestingTopNChanges)
			{
				this.UpdateTopNChangeRelatedFields(folder);
			}
			this.processingICSChanges = false;
			base.CallContext.ProtocolLog.Set(SyncFolderItemsMetadata.ChangesTime, stopwatch.ElapsedMilliseconds);
			base.CallContext.ProtocolLog.Set(SyncFolderItemsMetadata.CatchUpTime, stopwatch2.ElapsedMilliseconds);
			ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItems.DoIcsSync: End");
		}

		private void DoQuerySync(Folder folder)
		{
			ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItems.DoQuerySyncs: Start");
			if (!this.IsClientRequestingTopNChanges)
			{
				ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug<int, int, int>((long)this.GetHashCode(), "SyncFolderItems.DoQuerySyncs: The client is not performing a TopN sync. Most probably the client wants to sync like E14. Not performing any QuerySync. NumberOfDays = {0}, MinimumCount = {1}, MaximumCount = {2}", base.Request.NumberOfDays, base.Request.MinimumCount, base.Request.MaximumCount);
				return;
			}
			ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "SyncFolderItems.DoQuerySyncs: syncFolderId = '{0}'", this.syncFolderId.GetId());
			int num = (base.Request.MinimumCount > 0) ? base.Request.MinimumCount : 0;
			int num2 = (base.Request.MaximumCount > 0) ? base.Request.MaximumCount : int.MaxValue;
			ExDateTime exDateTime = (base.Request.NumberOfDays == 0) ? ExDateTime.MinValue : ExDateTime.UtcNow.AddDays((double)(-(double)base.Request.NumberOfDays));
			SortBy[] sortColumns = new SortBy[]
			{
				new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
			};
			if (this.changes.Count >= base.Request.MaxChangesReturned)
			{
				return;
			}
			PropertyListForViewRowDeterminer determiner = PropertyListForViewRowDeterminer.BuildForItems(this.responseShape, folder);
			using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, sortColumns, this.GetPropertyDefinitionsForDoQuerySync(determiner)))
			{
				int num3 = -1;
				ExDateTime t;
				this.SeekToLastRowAlreadyOnClient(queryResult, out num3, out t);
				if (num3 < num || (num3 < num2 && t > exDateTime))
				{
					this.ReadMoreRows(queryResult, num3, exDateTime, num, num2);
				}
				else if (num3 > num && (num3 > num2 || t < exDateTime))
				{
					this.TrimRowsFromClient(queryResult, exDateTime, num, num2);
				}
				else
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
					this.syncState.MoreItemsOnServer = (propertyBags.Length > 0);
				}
				this.UpdateTopNChangeRelatedFields(folder);
			}
			ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItems.DoQuerySyncs: End");
		}

		private ICollection<PropertyDefinition> GetPropertyDefinitionsForDoQuerySync(PropertyListForViewRowDeterminer determiner)
		{
			if (base.Request.NumberOfDays <= 0)
			{
				return determiner.GetPropertiesToFetch();
			}
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			list.AddRange(determiner.GetPropertiesToFetch());
			PropertyDefinition instanceKey = ItemSchema.InstanceKey;
			PropertyDefinition itemClass = StoreObjectSchema.ItemClass;
			PropertyDefinition receivedTime = ItemSchema.ReceivedTime;
			if (!list.Contains(instanceKey))
			{
				list.Add(instanceKey);
			}
			if (!list.Contains(itemClass))
			{
				list.Add(itemClass);
			}
			if (!list.Contains(receivedTime))
			{
				list.Add(receivedTime);
			}
			return list;
		}

		private void SeekToLastRowAlreadyOnClient(QueryResult queryResult, out int currentIndex, out ExDateTime currentTime)
		{
			currentIndex = -1;
			currentTime = ExDateTime.UtcNow;
			if (this.syncState.LastInstanceKey != null && queryResult.EstimatedRowCount > 0)
			{
				ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItems.SeekToLastRowAlreadyOnClient: Seeking to instance key");
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.InstanceKey, this.syncState.LastInstanceKey)))
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
					if (propertyBags.Length > 0)
					{
						currentTime = ((ExDateTime)propertyBags[0].TryGetProperty(ItemSchema.ReceivedTime)).ToUtc();
						if (ExDateTime.Compare(currentTime, this.syncState.OldestReceivedTime, TimeSpan.FromMilliseconds(1.0)) == 0)
						{
							currentIndex = queryResult.CurrentRow;
						}
						else
						{
							ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItems.SeekToLastRowAlreadyOnClient: The row was moved, we can't trust its location");
							this.syncState.LastInstanceKey = null;
							currentTime = ExDateTime.UtcNow;
						}
					}
				}
				else
				{
					ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItems.SeekToLastRowAlreadyOnClient: Could not find the row");
					this.syncState.LastInstanceKey = null;
				}
			}
			if (currentIndex == -1 && queryResult.EstimatedRowCount > 0)
			{
				ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItems.SeekToLastRowAlreadyOnClient: Seeking to last received time");
				currentTime = this.syncState.OldestReceivedTime.AddMilliseconds(1.0);
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ItemSchema.ReceivedTime, currentTime)))
				{
					currentIndex = queryResult.CurrentRow;
					return;
				}
				ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItems.SeekToLastRowAlreadyOnClient: Reached the bottom of the folder while seeking by ReceivedTime");
				currentIndex = queryResult.EstimatedRowCount;
				currentTime = this.syncState.OldestReceivedTime;
			}
		}

		private void ReadMoreRows(QueryResult queryResult, int currentIndex, ExDateTime windowTime, int minIndex, int maxIndex)
		{
			bool flag = true;
			while (flag && this.changes.Count < base.Request.MaxChangesReturned)
			{
				int num = Math.Min(base.Request.MaxChangesReturned - this.changes.Count + 1, 100);
				ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "SyncFolderItems.ReadMoreRows: Fetching {0} rows", num);
				IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(num);
				if (propertyBags.Length == 0)
				{
					this.syncState.MoreItemsOnServer = false;
					return;
				}
				this.syncState.MoreItemsOnServer = (propertyBags.Length == num);
				int i = 0;
				while (i < propertyBags.Length)
				{
					ExDateTime exDateTime = ((ExDateTime)propertyBags[i].TryGetProperty(ItemSchema.ReceivedTime)).ToUtc();
					if (this.changes.Count >= base.Request.MaxChangesReturned || (currentIndex >= minIndex && (currentIndex >= maxIndex || exDateTime < windowTime)))
					{
						this.syncState.MoreItemsOnServer = true;
						flag = false;
						break;
					}
					this.syncState.LastInstanceKey = (byte[])propertyBags[i].TryGetProperty(ItemSchema.InstanceKey);
					this.syncState.OldestReceivedTime = exDateTime;
					StoreId storeId = (StoreId)propertyBags[i].TryGetProperty(ItemSchema.Id);
					string itemClass = (string)propertyBags[i].TryGetProperty(StoreObjectSchema.ItemClass);
					SyncFolderItemsChangeTypeBase itemChangeFromId = this.GetItemChangeFromId(this.syncFolderIdAndSession, storeId, this.responseShape, itemClass, true, false);
					if (itemChangeFromId != null)
					{
						this.changes.AddChange(itemChangeFromId);
					}
					i++;
					currentIndex++;
				}
			}
		}

		private void TrimRowsFromClient(QueryResult queryResult, ExDateTime windowTime, int minIndex, int maxIndex)
		{
			int num;
			if (queryResult.SeekToCondition(SeekReference.BackwardFromEnd, new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.ReceivedTime, windowTime), SeekToConditionFlags.AllowExtendedSeekReferences))
			{
				num = queryResult.CurrentRow;
			}
			else
			{
				ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug((long)this.GetHashCode(), "SyncFolderItems.TrimRowsFromClient: Unable to seek to window end.");
				num = -1;
			}
			if (num + 1 < minIndex)
			{
				num = queryResult.SeekToOffset(SeekReference.OriginBeginning, minIndex - 1);
			}
			if (num + 1 > maxIndex)
			{
				num = queryResult.SeekToOffset(SeekReference.OriginBeginning, maxIndex - 1);
			}
			if (num == -1)
			{
				this.syncState.LastInstanceKey = null;
				this.syncState.OldestReceivedTime = ExDateTime.UtcNow;
				this.syncState.MoreItemsOnServer = (queryResult.EstimatedRowCount > 0);
				return;
			}
			IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(2);
			if (propertyBags.Length > 0)
			{
				this.syncState.LastInstanceKey = (byte[])propertyBags[0].TryGetProperty(ItemSchema.InstanceKey);
				this.syncState.OldestReceivedTime = ((ExDateTime)propertyBags[0].TryGetProperty(ItemSchema.ReceivedTime)).ToUtc();
				this.syncState.MoreItemsOnServer = (propertyBags.Length > 1);
				return;
			}
			ExTraceGlobals.SyncFolderItemsCallTracer.TraceError((long)this.GetHashCode(), "SyncFolderItems.TrimRowsFromClient: Unable to read the desired row after seeking to it. This should be very rare.");
		}

		private void UpdateTopNChangeRelatedFields(Folder folder)
		{
			this.changes.TotalCount = folder.ItemCount;
			this.changes.IncludesLastItemInRange = (this.changes.Count < base.Request.MaxChangesReturned);
			this.changes.MoreItemsOnServer = this.syncState.MoreItemsOnServer;
			ExDateTime exDateTime = this.syncState.OldestReceivedTime;
			exDateTime = EWSSettings.RequestTimeZone.ConvertDateTime(exDateTime);
			if (EWSSettings.DateTimePrecision == DateTimePrecision.Seconds)
			{
				exDateTime = exDateTime.AddMilliseconds((double)(-(double)exDateTime.Millisecond));
			}
			this.changes.OldestReceivedTime = exDateTime.ToISOString();
		}

		private Dictionary<StoreId, VersionedId> BuildIgnoreItemsDictionary(StoreSession session)
		{
			Dictionary<StoreId, VersionedId> dictionary = new Dictionary<StoreId, VersionedId>();
			if (this.ignoreList != null)
			{
				foreach (ItemId itemId in this.ignoreList)
				{
					IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadWrite(itemId);
					VersionedId versionedId = idAndSession.Id as VersionedId;
					if (versionedId != null)
					{
						StoreId objectId = versionedId.ObjectId;
						if (!dictionary.ContainsKey(objectId))
						{
							dictionary.Add(objectId, versionedId);
						}
					}
				}
			}
			return dictionary;
		}

		private bool TryBuildFolderItemChangeElement(IdAndSession folderIdAndSession, ServerManifestEntry syncOperation, MailboxSyncProvider syncProvider, out SyncFolderItemsChangeTypeBase change)
		{
			change = null;
			switch (syncOperation.ChangeType)
			{
			case ChangeType.Add:
			case ChangeType.Change:
			{
				bool isUpdate = syncOperation.ChangeType == ChangeType.Change || !syncOperation.IsNew;
				change = this.GetItemChange(folderIdAndSession, syncOperation, this.responseShape, true, isUpdate);
				goto IL_93;
			}
			case ChangeType.Delete:
			case ChangeType.SoftDelete:
				change = this.GetItemDeleteChange(folderIdAndSession, syncOperation);
				goto IL_93;
			case ChangeType.ReadFlagChange:
				change = this.GetItemReadFlagChange(folderIdAndSession, syncOperation, syncProvider);
				goto IL_93;
			case ChangeType.OutOfFilter:
				goto IL_93;
			}
			ExTraceGlobals.SyncFolderItemsCallTracer.TraceDebug<ChangeType>((long)this.GetHashCode(), "SyncFolderItems.TryBuildFolderItemChangeElement unknown sync change type {0}", syncOperation.ChangeType);
			IL_93:
			return change != null;
		}

		private SyncFolderItemsChangeTypeBase GetItemDeleteChange(IdAndSession syncFolderIdAndSession, ServerManifestEntry syncOperation)
		{
			StoreObjectId storeId = (StoreObjectId)syncOperation.Id.NativeId;
			IdAndSession idAndSession;
			if (syncFolderIdAndSession.Session is MailboxSession)
			{
				idAndSession = new IdAndSession(storeId, syncFolderIdAndSession.Session);
			}
			else
			{
				idAndSession = new IdAndSession(storeId, syncFolderIdAndSession.ParentFolderId, syncFolderIdAndSession.Session);
			}
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, idAndSession, null);
			ItemId itemId = new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
			return new SyncFolderItemsDeleteType(itemId);
		}

		private SyncFolderItemsChangeTypeBase GetItemReadFlagChange(IdAndSession folderIdAndSession, ServerManifestEntry syncOperation, MailboxSyncProvider syncProvider)
		{
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
			{
				StoreObjectId storeId = (StoreObjectId)syncOperation.Id.NativeId;
				IdAndSession idAndSession = new IdAndSession(storeId, folderIdAndSession.Id, folderIdAndSession.Session);
				ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, idAndSession, null);
				ItemId itemId = new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
				return new SyncFolderItemsReadFlagType(itemId, syncOperation.IsRead);
			}
			return this.GetItemChange(folderIdAndSession, syncOperation, this.responseShape, false, true);
		}

		private SyncFolderItemsChangeTypeBase GetItemChange(IdAndSession folderIdAndSession, ServerManifestEntry syncOperation, ItemResponseShape itemResponseShape, bool checkIgnoreList, bool isUpdate)
		{
			ISyncItemId id = syncOperation.Id;
			StoreObjectId storeObjectId = (StoreObjectId)id.NativeId;
			byte[] changeKey = ((MailboxSyncItemId)id).ChangeKey;
			StoreId storeId = storeObjectId;
			if (changeKey != null)
			{
				storeId = VersionedId.FromStoreObjectId(storeObjectId, changeKey);
			}
			return this.GetItemChangeFromId(folderIdAndSession, storeId, itemResponseShape, syncOperation.MessageClass, checkIgnoreList, isUpdate);
		}

		private SyncFolderItemsChangeTypeBase GetItemChangeFromId(IdAndSession folderIdAndSession, StoreId storeId, ItemResponseShape itemResponseShape, string itemClass, bool checkIgnoreList, bool isUpdate)
		{
			SyncFolderItemsChangeTypeBase result = null;
			try
			{
				ItemType item = this.GetItem(folderIdAndSession, storeId, itemClass, itemResponseShape, checkIgnoreList);
				if (item != null)
				{
					if (this.processingICSChanges && this.IsClientRequestingTopNChanges && item.DateTimeReceived != null && ExDateTime.Compare(ExDateTimeConverter.Parse(item.DateTimeReceived), this.syncState.OldestReceivedTime, TimeSpan.FromSeconds(1.0)) < 0)
					{
						result = null;
					}
					else
					{
						result = new SyncFolderItemsCreateOrUpdateType(item, isUpdate);
					}
				}
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.SyncFolderItemsCallTracer.TraceError((long)this.GetHashCode(), "SyncFolderItems.GetItemChangeXml detected deleted item on Add/Change sync operation");
			}
			return result;
		}

		private ItemType GetItem(IdAndSession folderIdAndSession, StoreId storeId, string itemClass, ItemResponseShape itemResponseShape, bool checkIgnoreList)
		{
			ItemType serviceItem = null;
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					bool flag;
					StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(storeId, out flag);
					ToServiceObjectPropertyList toServiceObjectPropertyListForPropertyBagUsingStoreObject = XsoDataConverter.GetToServiceObjectPropertyListForPropertyBagUsingStoreObject(asStoreObjectId, itemResponseShape, this.ParticipantResolver);
					if (this.optimizeForIdOnly || this.optimizeForItemClass)
					{
						if (!flag || !checkIgnoreList || !this.IsIgnorableItem(storeId))
						{
							IdAndSession idAndSession = new IdAndSession(asStoreObjectId, folderIdAndSession.Id, folderIdAndSession.Session);
							ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, idAndSession, null);
							ItemId itemId = new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
							serviceItem = ItemType.CreateFromStoreObjectType(asStoreObjectId.ObjectType);
							serviceItem.ItemId = itemId;
							if (this.optimizeForItemClass)
							{
								serviceItem.ItemClass = itemClass;
								return;
							}
						}
					}
					else
					{
						Item item = null;
						try
						{
							bool flag2;
							if (flag)
							{
								flag2 = (!checkIgnoreList || !this.IsIgnorableItem(storeId));
							}
							else
							{
								PropertyDefinition[] propertyDefinitions = toServiceObjectPropertyListForPropertyBagUsingStoreObject.GetPropertyDefinitions();
								item = ServiceCommandBase.GetXsoItem(folderIdAndSession.Session, asStoreObjectId, propertyDefinitions);
								flag2 = (!checkIgnoreList || !this.IsIgnorableItem(item.Id));
							}
							if (flag2)
							{
								if (item == null)
								{
									PropertyDefinition[] propertyDefinitions2 = toServiceObjectPropertyListForPropertyBagUsingStoreObject.GetPropertyDefinitions();
									item = ServiceCommandBase.GetXsoItem(folderIdAndSession.Session, asStoreObjectId, propertyDefinitions2);
								}
								IdAndSession idAndSession2 = new IdAndSession(asStoreObjectId, item.ParentId, folderIdAndSession.Session);
								if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012) && item is CalendarItemBase && item.Session is MailboxSession && ((CalendarItemBase)item).Sensitivity != Sensitivity.Normal && ((MailboxSession)item.Session).ShouldFilterPrivateItems)
								{
									toServiceObjectPropertyListForPropertyBagUsingStoreObject.FilterProperties(ExternalUserCalendarResponseShape.CalendarPropertiesPrivateItemWithSubject);
								}
								serviceItem = ItemType.CreateFromStoreObjectType(asStoreObjectId.ObjectType);
								ServiceCommandBase.LoadServiceObject(serviceItem, item, idAndSession2, this.responseShape, toServiceObjectPropertyListForPropertyBagUsingStoreObject);
							}
						}
						finally
						{
							if (item != null)
							{
								item.Dispose();
							}
						}
					}
				}, new GrayException.IsGrayExceptionDelegate(GrayException.IsSystemGrayException));
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.SyncFolderItemsCallTracer.TraceWarning<string, LocalizedException>((long)this.GetHashCode(), "SyncFolderItems.GetItem: Exception thrown while processing item {0}: {1}", storeId.ToBase64String(), ex);
				base.CallContext.ProtocolLog.Set(SyncFolderItemsMetadata.ExceptionItemId, storeId.ToBase64String());
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(base.CallContext.ProtocolLog, ex, "SyncFolderItems_Exception");
				serviceItem = null;
			}
			return serviceItem;
		}

		private bool IsIgnorableItem(StoreId itemId)
		{
			VersionedId versionedId = itemId as VersionedId;
			StoreId storeId = (versionedId != null) ? versionedId.ObjectId : itemId;
			VersionedId versionedId2;
			if (!this.ignoreItemsDictionary.TryGetValue(storeId, out versionedId2))
			{
				return false;
			}
			if (versionedId == null)
			{
				return storeId.Equals(versionedId2.ObjectId);
			}
			return versionedId.Equals(versionedId2);
		}

		private const int MaxChangesAllowed = 512;

		private const int RowBatchSize = 100;

		private ItemResponseShape responseShape;

		private BaseFolderId syncFolderId;

		private List<ItemId> ignoreList;

		private Dictionary<StoreId, VersionedId> ignoreItemsDictionary;

		private string syncStateBase64String;

		private int maxChangesReturned;

		private SyncFolderItemsScope syncScope;

		private bool optimizeForIdOnly;

		private bool optimizeForItemClass;

		private ServicesFolderSyncState syncState;

		private IdAndSession syncFolderIdAndSession;

		private StoreSession session;

		private SyncFolderItemsChangesType changes = new SyncFolderItemsChangesType();

		private bool processingICSChanges;
	}
}
