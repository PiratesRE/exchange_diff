using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CoreFolder : CoreObject
	{
		internal CoreFolder(StoreSession session, FolderSchema schema, PersistablePropertyBag propertyBag, StoreObjectId storeObjectId, byte[] changeKey, Origin origin, ICollection<PropertyDefinition> prefetchProperties) : base(session, propertyBag, storeObjectId, changeKey, origin, ItemLevel.TopLevel, prefetchProperties)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.schema = schema;
				this.queryExecutor = new QueryExecutor(base.Session, new QueryExecutor.GetMapiFolderDelegate(this.GetMapiFolder));
				disposeGuard.Success();
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CoreFolder>(this);
		}

		public static CoreFolder Bind(StoreSession session, StoreId folderId)
		{
			return CoreFolder.Bind(session, folderId, null);
		}

		public static CoreFolder Bind(StoreSession session, StoreId folderId, params PropertyDefinition[] propsToReturn)
		{
			return CoreFolder.Bind(session, folderId, false, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public static CoreFolder Bind(StoreSession session, StoreId folderId, bool allowSoftDeleted, params PropertyDefinition[] propsToReturn)
		{
			return CoreFolder.Bind(session, folderId, allowSoftDeleted, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public static CoreFolder Bind(StoreSession session, StoreId folderId, ICollection<PropertyDefinition> propsToReturn)
		{
			return CoreFolder.Bind(session, folderId, false, propsToReturn);
		}

		public static CoreFolder Bind(StoreSession session, StoreId folderId, bool allowSoftDeleted, ICollection<PropertyDefinition> propsToReturn)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(folderId, "folderId");
			return CoreFolder.InternalBind(session, folderId, allowSoftDeleted, propsToReturn);
		}

		public static CoreFolder Create(StoreSession session, StoreId parentId, bool isSearchFolder, string displayName, CreateMode createMode)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentId, "parentId");
			Util.ThrowOnNullOrEmptyArgument(displayName, "displayName");
			EnumValidator.ThrowIfInvalid<CreateMode>(createMode, "createMode");
			return CoreFolder.InternalCreate(session, parentId, isSearchFolder, displayName, createMode, false, FolderCreateInfo.GenericFolderInfo);
		}

		public static CoreFolder CreateSecure(StoreSession session, StoreId parentId, bool isSearchFolder, string displayName, CreateMode createMode)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentId, "parentId");
			Util.ThrowOnNullOrEmptyArgument(displayName, "displayName");
			EnumValidator.ThrowIfInvalid<CreateMode>(createMode, "createMode");
			return CoreFolder.InternalCreate(session, parentId, isSearchFolder, displayName, createMode, true, FolderCreateInfo.GenericFolderInfo);
		}

		public static CoreFolder Import(HierarchySynchronizationUploadContext context, StoreObjectId parentFolderId, VersionedId folderId, ExDateTime lastModificationTime, byte[] predecessorChangeList, string displayName)
		{
			Util.ThrowOnNullArgument(context, "context");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(folderId, "folderId");
			Util.ThrowOnNullArgument(predecessorChangeList, "predecessorChangeList");
			Util.ThrowOnNullOrEmptyArgument(displayName, "displayName");
			context.Session.ValidateOperation(FolderChangeOperation.Create, parentFolderId);
			CoreFolder coreFolder = null;
			FolderPropertyBag folderPropertyBag = null;
			FolderCreateInfo folderCreateInfo = (folderId.ObjectId.ObjectType == StoreObjectType.SearchFolder) ? FolderCreateInfo.SearchFolderInfo : FolderCreateInfo.GenericFolderInfo;
			bool flag = false;
			CoreFolder result;
			try
			{
				folderPropertyBag = new FolderImportPropertyBag(context, parentFolderId, StoreId.GetStoreObjectId(folderId), folderCreateInfo.Schema.AutoloadProperties);
				coreFolder = new CoreFolder(context.Session, folderCreateInfo.Schema, folderPropertyBag, null, null, Origin.New, folderCreateInfo.Schema.AutoloadProperties);
				coreFolder.PropertyBag[CoreObjectSchema.ParentSourceKey] = context.Session.IdConverter.GetLongTermIdFromId(parentFolderId);
				coreFolder.PropertyBag[CoreObjectSchema.SourceKey] = context.Session.IdConverter.GetLongTermIdFromId(folderId.ObjectId);
				coreFolder.PropertyBag[CoreObjectSchema.LastModifiedTime] = lastModificationTime;
				coreFolder.PropertyBag[CoreObjectSchema.ChangeKey] = folderId.ChangeKeyAsByteArray();
				coreFolder.PropertyBag[CoreObjectSchema.PredecessorChangeList] = predecessorChangeList;
				coreFolder.PropertyBag[CoreFolderSchema.DisplayName] = displayName;
				flag = true;
				result = coreFolder;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(coreFolder);
					Util.DisposeIfPresent(folderPropertyBag);
				}
			}
			return result;
		}

		public static OperationResult Delete(HierarchySynchronizationUploadContext context, DeleteItemFlags deleteItemFlags, IList<StoreObjectId> folderIds)
		{
			Util.ThrowOnNullArgument(context, "context");
			Util.ThrowOnNullArgument(folderIds, "folderIds");
			EnumValidator.ThrowIfInvalid<DeleteItemFlags>(deleteItemFlags, "deleteItemFlags");
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(context.Session, folderIds);
			FolderChangeOperation folderChangeOperation = ((deleteItemFlags & DeleteItemFlags.HardDelete) == DeleteItemFlags.HardDelete) ? FolderChangeOperation.HardDelete : FolderChangeOperation.SoftDelete;
			foreach (StoreObjectId folderId in folderIds)
			{
				context.Session.ValidateOperation(folderChangeOperation, folderId);
			}
			FolderChangeOperationFlags folderChangeOperationFlags = CoreFolder.GetFolderChangeOperationFlags(FolderChangeOperationFlags.IncludeAll, deleteItemFlags);
			GroupOperationResult groupOperationResult = null;
			using (CallbackContext callbackContext = new CallbackContext(context.Session))
			{
				try
				{
					bool flag = context.Session.OnBeforeFolderChange(folderChangeOperation, folderChangeOperationFlags, context.Session, null, null, null, folderIds, callbackContext);
					if (flag)
					{
						groupOperationResult = context.Session.GetCallbackResults();
					}
					else
					{
						context.ImportDeletes(deleteItemFlags, context.Session.IdConverter.GetSourceKeys(folderIds, new Predicate<StoreObjectId>(IdConverter.IsFolderId)));
						groupOperationResult = new GroupOperationResult(OperationResult.Succeeded, folderIds, null);
					}
				}
				catch (StoragePermanentException storageException)
				{
					groupOperationResult = new GroupOperationResult(OperationResult.Failed, folderIds, storageException);
					throw;
				}
				catch (StorageTransientException storageException2)
				{
					groupOperationResult = new GroupOperationResult(OperationResult.Failed, folderIds, storageException2);
					throw;
				}
				finally
				{
					context.Session.OnAfterFolderChange(folderChangeOperation, folderChangeOperationFlags, context.Session, null, null, null, folderIds, groupOperationResult, callbackContext);
				}
			}
			return groupOperationResult.OperationResult;
		}

		public QueryExecutor QueryExecutor
		{
			get
			{
				this.CheckDisposed(null);
				return this.queryExecutor;
			}
		}

		public PropertyBagSaveFlags SaveFlags
		{
			get
			{
				this.CheckDisposed(null);
				return this.FolderPropertyBag.SaveFlags;
			}
			set
			{
				this.CheckDisposed(null);
				EnumValidator.ThrowIfInvalid<PropertyBagSaveFlags>(value, "value");
				this.FolderPropertyBag.SaveFlags = value;
			}
		}

		public IModifyTable GetRuleTable()
		{
			return this.GetRuleTable(new RuleTableRestriction(this));
		}

		public IModifyTable GetRuleTable(IModifyTableRestriction modifyTableRestriction)
		{
			return new PropertyTable(this, CoreFolderSchema.MapiRulesTable, ModifyTableOptions.None, modifyTableRestriction);
		}

		public SearchFolderCriteria GetSearchCriteria(bool needsConvertToSmartFilter)
		{
			this.CheckDisposed(null);
			Restriction restriction = null;
			byte[][] array = null;
			SearchState searchState = SearchState.None;
			StoreSession session = base.Session;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.GetMapiFolder().GetSearchCriteria(out restriction, out array, out searchState);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotGetSearchCriteria, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreFolder::GetSearchCriteria.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotGetSearchCriteria, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreFolder::GetSearchCriteria.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			Restriction.CountRestriction countRestriction = null;
			if (restriction is Restriction.CountRestriction)
			{
				countRestriction = (Restriction.CountRestriction)restriction;
				restriction = countRestriction.Restriction;
			}
			QueryFilter searchQuery = FilterRestrictionConverter.CreateFilter(base.Session, this.GetMapiFolder(), restriction, needsConvertToSmartFilter);
			StoreId[] array2 = new StoreObjectId[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = StoreObjectId.FromProviderSpecificId(array[i]);
			}
			SearchFolderCriteria searchFolderCriteria = new SearchFolderCriteria(searchQuery, array2, searchState);
			if (countRestriction != null)
			{
				searchFolderCriteria.MaximumResultsCount = new int?(countRestriction.Count);
			}
			return searchFolderCriteria;
		}

		public void SetSearchCriteria(SearchFolderCriteria searchFolderCriteria, SetSearchCriteriaFlags setSearchCriteriaFlags)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<SetSearchCriteriaFlags>(setSearchCriteriaFlags);
			Util.ThrowOnNullArgument(searchFolderCriteria, "searchFolderCriteria");
			SearchCriteriaFlags flags = MapiUtil.SetSearchCriteriaFlagsToMapiSearchCriteriaFlags(setSearchCriteriaFlags);
			Restriction restriction = null;
			if (searchFolderCriteria.SearchQuery != null)
			{
				restriction = FilterRestrictionConverter.CreateRestriction(base.Session, base.Session.ExTimeZone, this.GetMapiFolder(), searchFolderCriteria.SearchQuery);
				if (searchFolderCriteria.MaximumResultsCount != null)
				{
					restriction = new Restriction.CountRestriction(searchFolderCriteria.MaximumResultsCount.Value, restriction);
				}
			}
			byte[][] entryIds = null;
			if (searchFolderCriteria.FolderScope != null && searchFolderCriteria.FolderScope.Length > 0)
			{
				entryIds = StoreId.StoreIdsToEntryIds(searchFolderCriteria.FolderScope);
			}
			StoreSession session = base.Session;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.GetMapiFolder().SetSearchCriteria(restriction, entryIds, flags);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotSetSearchCriteria, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreFolder::SetSearchCriteria.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ExCannotSetSearchCriteria, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreFolder::SetSearchCriteria.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		public FolderSaveResult Save(SaveMode saveMode)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<SaveMode>(saveMode, "saveMode");
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(base.Session, this);
			if (saveMode == SaveMode.ResolveConflicts)
			{
				throw new NotSupportedException("Folder does not support resolving conflicts.");
			}
			this.CoreObjectUpdate();
			base.ValidateCoreObject();
			this.OnBeforeFolderSave();
			bool needVersionCheck = saveMode == SaveMode.FailOnAnyConflict;
			FolderSaveResult result = this.FolderPropertyBag.SaveFolderPropertyBag(needVersionCheck);
			this.OnAfterFolderSave();
			base.Origin = Origin.Existing;
			((ICoreObject)this).ResetId();
			return result;
		}

		public void SetItemStatus(StoreObjectId itemId, MessageStatusFlags status, MessageStatusFlags statusMask, out MessageStatusFlags oldStatus)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(itemId, "itemId");
			EnumValidator.ThrowIfInvalid<MessageStatusFlags>(status, "status");
			EnumValidator.ThrowIfInvalid<MessageStatusFlags>(statusMask, "statusMask");
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(base.Session, this);
			MessageStatusFlags bitsSet = statusMask & status;
			MessageStatusFlags bitsClear = statusMask & ~status;
			MapiFolder mapiFolder = this.GetMapiFolder();
			MessageStatus messageStatus;
			CoreFolder.InternalSetItemStatus(mapiFolder, base.Session, this, itemId, (MessageStatus)bitsSet, (MessageStatus)bitsClear, out messageStatus);
			oldStatus = (MessageStatusFlags)messageStatus;
		}

		public void SetItemFlags(StoreObjectId itemId, MessageFlags flags, MessageFlags flagsMask)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(itemId, "itemId");
			EnumValidator.ThrowIfInvalid<MessageFlags>(flags, "flags");
			EnumValidator.ThrowIfInvalid<MessageFlags>(flagsMask, "flagsMask");
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(base.Session, this);
			MessageFlags bitsSet = flagsMask & flags;
			MessageFlags bitsClear = flagsMask & ~flags;
			MapiFolder mapiFolder = this.GetMapiFolder();
			CoreFolder.InternalSetItemFlags(mapiFolder, base.Session, this, itemId, (MessageFlags)bitsSet, (MessageFlags)bitsClear);
		}

		public void SetReadFlags(int flags, StoreId[] itemIds, out bool partialCompletion)
		{
			this.CheckDisposed(null);
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(base.Session, this);
			try
			{
				this.InternalSetReadFlags((SetReadFlags)flags, itemIds);
				partialCompletion = false;
			}
			catch (PartialCompletionException)
			{
				partialCompletion = true;
			}
		}

		public IMapiFxProxy GetFxProxyCollector()
		{
			return this.GetMapiFolder().GetFxProxyCollector();
		}

		public HierarchyManifestProvider GetHierarchyManifest(ManifestConfigFlags flags, StorageIcsState state, PropertyDefinition[] includePropertyDefinitions, PropertyDefinition[] excludePropertyDefinitions)
		{
			this.CheckDisposed(null);
			return new HierarchyManifestProvider(this, flags, state, includePropertyDefinitions, excludePropertyDefinitions);
		}

		public HierarchySynchronizationUploadContext GetHierarchySynchronizationUploadContext(StorageIcsState state)
		{
			this.CheckDisposed(null);
			return new HierarchySynchronizationUploadContext(this, state);
		}

		public ContentManifestProvider GetContentManifest(ManifestConfigFlags flags, QueryFilter filter, StorageIcsState state, PropertyDefinition[] includePropertyDefinitions)
		{
			this.CheckDisposed(null);
			return new ContentManifestProvider(this, flags, filter, state, includePropertyDefinitions);
		}

		public ContentsSynchronizerProvider GetContentsSynchronizer(SynchronizerConfigFlags flags, QueryFilter filter, StorageIcsState state, PropertyDefinition[] includePropertyDefinitions, PropertyDefinition[] excludePropertyDefinitions, int bufferSize)
		{
			this.CheckDisposed(null);
			return new ContentsSynchronizerProvider(this, flags, filter, state, includePropertyDefinitions, excludePropertyDefinitions, bufferSize);
		}

		public HierarchySynchronizerProvider GetHierarchySynchronizer(SynchronizerConfigFlags flags, QueryFilter filter, StorageIcsState state, PropertyDefinition[] includePropertyDefinitions, PropertyDefinition[] excludePropertyDefinitions, int bufferSize)
		{
			this.CheckDisposed(null);
			return new HierarchySynchronizerProvider(this, flags, filter, state, includePropertyDefinitions, excludePropertyDefinitions, bufferSize);
		}

		public ContentsSynchronizationUploadContext GetContentsSynchronizationUploadContext(StorageIcsState state)
		{
			this.CheckDisposed(null);
			return new ContentsSynchronizationUploadContext(this, state);
		}

		public GroupOperationResult CopyItems(CoreFolder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues, CoreFolder.ItemIdValidatorDelegate itemIdValidator)
		{
			return this.CopyItems(destinationFolder, sourceItemIds, propertyDefinitions, propertyValues, itemIdValidator, false);
		}

		public GroupOperationResult CopyItems(CoreFolder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues, CoreFolder.ItemIdValidatorDelegate itemIdValidator, bool returnNewIds)
		{
			return this.CopyItems(destinationFolder, sourceItemIds, propertyDefinitions, propertyValues, itemIdValidator, returnNewIds, true);
		}

		public GroupOperationResult CopyItems(CoreFolder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues, CoreFolder.ItemIdValidatorDelegate itemIdValidator, bool returnNewIds, bool updateSource)
		{
			this.CheckDisposed(null);
			this.ValidateItemIds(sourceItemIds, itemIdValidator);
			return this.InternalMoveOrCopyItems(destinationFolder, sourceItemIds, propertyDefinitions, propertyValues, false, returnNewIds, null, updateSource);
		}

		public GroupOperationResult MoveItems(CoreFolder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues, CoreFolder.ItemIdValidatorDelegate itemIdValidator)
		{
			return this.MoveItems(destinationFolder, sourceItemIds, propertyDefinitions, propertyValues, itemIdValidator, false);
		}

		public GroupOperationResult MoveItems(CoreFolder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues, CoreFolder.ItemIdValidatorDelegate itemIdValidator, bool returnNewIds)
		{
			return this.MoveItems(destinationFolder, sourceItemIds, propertyDefinitions, propertyValues, itemIdValidator, returnNewIds, null);
		}

		public GroupOperationResult MoveItems(CoreFolder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues, CoreFolder.ItemIdValidatorDelegate itemIdValidator, bool returnNewIds, DeleteItemFlags? deleteFlags)
		{
			this.CheckDisposed(null);
			this.ValidateItemIds(sourceItemIds, itemIdValidator);
			if (destinationFolder.StoreObjectId.Equals(this.StoreObjectId) && propertyValues == null)
			{
				return new GroupOperationResult(OperationResult.Succeeded, sourceItemIds, null);
			}
			return this.InternalMoveOrCopyItems(destinationFolder, sourceItemIds, propertyDefinitions, propertyValues, true, returnNewIds, deleteFlags, true);
		}

		public GroupOperationResult DeleteItems(DeleteItemFlags deleteItemFlags, StoreObjectId[] ids)
		{
			this.CheckDisposed(null);
			this.ResolveStoreObjectType(ids);
			EnumValidator.ThrowIfInvalid<DeleteItemFlags>(deleteItemFlags, "deleteItemFlags");
			Util.ThrowOnNullArgument(ids, "ids");
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(base.Session, this);
			byte[][] entryIDs = CoreFolder.StoreObjectIdsToEntryIds(base.Session, ids);
			DeleteMessagesFlags mapiDeleteFlags = CoreFolder.MapiDeleteFlagsFromXsoDeleteFlags(deleteItemFlags);
			FolderChangeOperation operation = ((deleteItemFlags & DeleteItemFlags.HardDelete) == DeleteItemFlags.HardDelete) ? FolderChangeOperation.HardDelete : FolderChangeOperation.SoftDelete;
			FolderChangeOperationFlags folderChangeOperationFlags = CoreFolder.GetFolderChangeOperationFlags(FolderChangeOperationFlags.IncludeAll, deleteItemFlags);
			GroupOperationResult result = null;
			using (CallbackContext callbackContext = new CallbackContext(base.Session))
			{
				try
				{
					bool flag = base.Session.OnBeforeFolderChange(operation, folderChangeOperationFlags, base.Session, null, this.StoreObjectId, null, ids, callbackContext);
					if (flag)
					{
						result = base.Session.GetCallbackResults();
					}
					else
					{
						result = this.ExecuteMapiGroupOperation("DeleteItems", ids, delegate()
						{
							this.GetMapiFolder().DeleteMessages(mapiDeleteFlags, entryIDs);
						});
					}
				}
				finally
				{
					base.Session.OnAfterFolderChange(operation, folderChangeOperationFlags, base.Session, null, this.StoreObjectId, null, ids, result, callbackContext);
				}
			}
			return result;
		}

		public GroupOperationResult DeleteFolder(DeleteFolderFlags storageDeleteFlags, StoreObjectId folderId)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<DeleteFolderFlags>(storageDeleteFlags, "storageDeleteFlags");
			Util.ThrowOnNullArgument(folderId, "folderId");
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(base.Session, new StoreObjectId[]
			{
				folderId
			});
			DeleteFolderFlags mapiDeleteFlags = DeleteFolderFlags.None;
			FolderChangeOperationFlags folderChangeOperationFlags = FolderChangeOperationFlags.None;
			if ((storageDeleteFlags & DeleteFolderFlags.DeleteMessages) != DeleteFolderFlags.None)
			{
				mapiDeleteFlags |= DeleteFolderFlags.DeleteMessages;
				folderChangeOperationFlags |= (FolderChangeOperationFlags.IncludeAssociated | FolderChangeOperationFlags.IncludeItems);
			}
			if ((storageDeleteFlags & DeleteFolderFlags.DeleteSubFolders) != DeleteFolderFlags.None)
			{
				mapiDeleteFlags |= DeleteFolderFlags.DelSubFolders;
				folderChangeOperationFlags |= FolderChangeOperationFlags.IncludeSubFolders;
			}
			if ((storageDeleteFlags & DeleteFolderFlags.HardDelete) != DeleteFolderFlags.None)
			{
				mapiDeleteFlags |= DeleteFolderFlags.ForceHardDelete;
			}
			FolderChangeOperation folderChangeOperation = ((storageDeleteFlags & DeleteFolderFlags.HardDelete) != DeleteFolderFlags.None) ? FolderChangeOperation.HardDelete : FolderChangeOperation.SoftDelete;
			base.Session.ValidateOperation(folderChangeOperation, folderId);
			GroupOperationResult result = null;
			using (CallbackContext callbackContext = new CallbackContext(base.Session))
			{
				try
				{
					bool flag = base.Session.OnBeforeFolderChange(folderChangeOperation, folderChangeOperationFlags, base.Session, null, this.StoreObjectId, null, new StoreObjectId[]
					{
						folderId
					}, callbackContext);
					if (flag)
					{
						result = base.Session.GetCallbackResults();
					}
					else
					{
						result = this.ExecuteMapiGroupOperation("DeleteFolder", new StoreObjectId[]
						{
							folderId
						}, delegate()
						{
							this.GetMapiFolder().DeleteFolder(Folder.GetFolderProviderLevelItemId(folderId), mapiDeleteFlags);
						});
					}
				}
				finally
				{
					base.Session.OnAfterFolderChange(folderChangeOperation, folderChangeOperationFlags, base.Session, null, this.StoreObjectId, null, new StoreObjectId[]
					{
						folderId
					}, result, callbackContext);
				}
			}
			return result;
		}

		public GroupOperationResult DeleteFolder(DeleteItemFlags deleteItemFlags, StoreObjectId folderId)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<DeleteItemFlags>(deleteItemFlags, "deleteItemFlags");
			Util.ThrowOnNullArgument(folderId, "folderId");
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(base.Session, new StoreObjectId[]
			{
				folderId
			});
			DeleteFolderFlags deleteFolderFlags = DeleteFolderFlags.DeleteMessages | DeleteFolderFlags.DeleteSubFolders;
			if ((deleteItemFlags & DeleteItemFlags.HardDelete) == DeleteItemFlags.HardDelete)
			{
				deleteFolderFlags |= DeleteFolderFlags.HardDelete;
			}
			return this.DeleteFolder(deleteFolderFlags, folderId);
		}

		public GroupOperationResult EmptyFolder(bool reportProgress, EmptyFolderFlags storageEmptyFlags)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<EmptyFolderFlags>(storageEmptyFlags, "storageEmptyFlags");
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(base.Session, this);
			base.Session.ValidateOperation(FolderChangeOperation.Empty, this.StoreObjectId);
			bool flag = (storageEmptyFlags & EmptyFolderFlags.DeleteAssociatedMessages) != EmptyFolderFlags.None;
			bool flag2 = (storageEmptyFlags & EmptyFolderFlags.HardDelete) != EmptyFolderFlags.None;
			FolderChangeOperation operation = flag2 ? FolderChangeOperation.HardDelete : FolderChangeOperation.SoftDelete;
			FolderChangeOperationFlags folderChangeOperationFlags = FolderChangeOperationFlags.IncludeItems | FolderChangeOperationFlags.IncludeSubFolders | FolderChangeOperationFlags.EmptyFolder;
			EmptyFolderFlags mapiEmptyFolderFlags = EmptyFolderFlags.None;
			if (flag)
			{
				mapiEmptyFolderFlags |= EmptyFolderFlags.DeleteAssociatedMessages;
				folderChangeOperationFlags |= FolderChangeOperationFlags.IncludeAssociated;
			}
			if ((storageEmptyFlags & EmptyFolderFlags.Force) != EmptyFolderFlags.None)
			{
				mapiEmptyFolderFlags |= EmptyFolderFlags.Force;
			}
			if (flag2)
			{
				mapiEmptyFolderFlags |= EmptyFolderFlags.ForceHardDelete;
			}
			GroupOperationResult result = null;
			using (CallbackContext callbackContext = new CallbackContext(base.Session))
			{
				try
				{
					bool flag3 = base.Session.OnBeforeFolderChange(operation, folderChangeOperationFlags, base.Session, null, this.StoreObjectId, null, new StoreObjectId[]
					{
						this.StoreObjectId
					}, callbackContext);
					if (flag3)
					{
						result = base.Session.GetCallbackResults();
					}
					else
					{
						result = this.ExecuteMapiGroupOperation("EmptyFolder", new StoreObjectId[]
						{
							this.StoreObjectId
						}, delegate()
						{
							this.GetMapiFolder().EmptyFolder(mapiEmptyFolderFlags);
						});
					}
				}
				finally
				{
					base.Session.OnAfterFolderChange(operation, folderChangeOperationFlags, base.Session, null, this.StoreObjectId, null, new StoreObjectId[]
					{
						this.StoreObjectId
					}, result, callbackContext);
				}
			}
			return result;
		}

		public PropertyError[] CopyFolder(CoreFolder destinationFolder, CopyPropertiesFlags copyPropertiesFlags, CopySubObjects copySubObjects, NativeStorePropertyDefinition[] excludeProperties)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(destinationFolder, "destinationFolder");
			Util.ThrowOnNullArgument(excludeProperties, "excludeProperties");
			EnumValidator.ThrowIfInvalid<CopyPropertiesFlags>(copyPropertiesFlags, "copyPropertiesFlags");
			EnumValidator.ThrowIfInvalid<CopySubObjects>(copySubObjects, "copySubObjects");
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(destinationFolder.Session, destinationFolder);
			base.Session.ValidateOperation(FolderChangeOperation.Copy, this.StoreObjectId);
			PropertyError[] array = null;
			LocalizedException ex = null;
			StoreObjectId[] array2 = new StoreObjectId[]
			{
				this.StoreObjectId
			};
			byte[] entryId = base.PropertyBag[CoreObjectSchema.ParentEntryId] as byte[];
			StoreObjectId sourceFolderId = StoreObjectId.FromProviderSpecificId(entryId, StoreObjectType.Folder);
			FolderChangeOperationFlags flags = FolderChangeOperationFlags.IncludeAll;
			if (destinationFolder.IsPermissionChangeBlocked())
			{
				int num = excludeProperties.Length;
				int num2 = num + CoreFolder.permissionChangeRelatedProperties.Length;
				Array.Resize<NativeStorePropertyDefinition>(ref excludeProperties, num2);
				for (int i = num; i < num2; i++)
				{
					excludeProperties[i] = CoreFolder.permissionChangeRelatedProperties[i - num];
				}
			}
			PropertyError[] result;
			using (CallbackContext callbackContext = new CallbackContext(base.Session))
			{
				try
				{
					base.Session.OnBeforeFolderChange(FolderChangeOperation.Copy, flags, base.Session, base.Session, sourceFolderId, destinationFolder.StoreObjectId, array2, callbackContext);
					array = CoreObject.MapiCopyTo(this.FolderPropertyBag.MapiProp, destinationFolder.FolderPropertyBag.MapiProp, base.Session, destinationFolder.Session, copyPropertiesFlags, copySubObjects, excludeProperties);
					result = array;
				}
				catch (StoragePermanentException ex2)
				{
					ex = ex2;
					throw;
				}
				catch (StorageTransientException ex3)
				{
					ex = ex3;
					throw;
				}
				finally
				{
					OperationResult operationResult = OperationResult.Succeeded;
					if (ex != null)
					{
						operationResult = OperationResult.Failed;
					}
					else if (array != null && array.Length != 0)
					{
						operationResult = OperationResult.PartiallySucceeded;
					}
					GroupOperationResult result2 = new GroupOperationResult(operationResult, array2, ex);
					base.Session.OnAfterFolderChange(FolderChangeOperation.Copy, flags, base.Session, base.Session, sourceFolderId, destinationFolder.StoreObjectId, array2, result2, callbackContext);
				}
			}
			return result;
		}

		public GroupOperationResult MoveFolder(CoreFolder destinationFolder, StoreObjectId sourceFolderId)
		{
			return this.MoveFolder(destinationFolder, sourceFolderId, null);
		}

		public GroupOperationResult MoveFolder(CoreFolder destinationFolder, StoreObjectId sourceFolderId, string newFolderName)
		{
			this.CheckDisposed(null);
			if (!base.Session.IsMoveUser)
			{
				this.CheckIsNotDefaultFolder(sourceFolderId, new DefaultFolderType[0]);
			}
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(destinationFolder.Session, destinationFolder);
			base.Session.ValidateOperation(FolderChangeOperation.Move, sourceFolderId);
			FolderChangeOperation operation = FolderChangeOperation.Move;
			StoreObjectId storeObjectId = destinationFolder.StoreObjectId;
			StoreObjectId[] itemIds = new StoreObjectId[]
			{
				sourceFolderId
			};
			GroupOperationResult result = null;
			FolderChangeOperationFlags flags = FolderChangeOperationFlags.IncludeAll;
			using (CallbackContext callbackContext = new CallbackContext(base.Session))
			{
				try
				{
					if (base.Session != null)
					{
						MailboxSession mailboxSession = base.Session as MailboxSession;
						if (mailboxSession != null && mailboxSession.IsDefaultFolderType(storeObjectId) == DefaultFolderType.DeletedItems)
						{
							operation = FolderChangeOperation.MoveToDeletedItems;
						}
						base.Session.OnBeforeFolderChange(operation, flags, base.Session, destinationFolder.Session, this.StoreObjectId, storeObjectId, itemIds, callbackContext);
					}
					result = this.ExecuteMapiGroupOperation("MoveFolder", new StoreObjectId[]
					{
						sourceFolderId
					}, delegate()
					{
						this.GetMapiFolder().CopyFolder(CopyFolderFlags.FolderMove, destinationFolder.GetMapiFolder(), Folder.GetFolderProviderLevelItemId(sourceFolderId), newFolderName);
					});
				}
				finally
				{
					if (base.Session != null)
					{
						base.Session.OnAfterFolderChange(operation, flags, base.Session, destinationFolder.Session, this.StoreObjectId, destinationFolder.Id.ObjectId, itemIds, result, callbackContext);
					}
				}
			}
			return result;
		}

		public GroupOperationResult CopyFolder(CoreFolder destinationFolder, CopySubObjects copySubObjects, StoreObjectId sourceFolderId)
		{
			return this.CopyFolder(destinationFolder, copySubObjects, sourceFolderId, null);
		}

		public GroupOperationResult CopyFolder(CoreFolder destinationFolder, CopySubObjects copySubObjects, StoreObjectId sourceFolderId, string newFolderName)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<CopySubObjects>(copySubObjects);
			base.Session.ValidateOperation(FolderChangeOperation.Copy, sourceFolderId);
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(destinationFolder.Session, destinationFolder);
			GroupOperationResult result = null;
			CopyFolderFlags mapiCopyFolderFlags;
			switch (copySubObjects)
			{
			case CopySubObjects.Copy:
				mapiCopyFolderFlags = CopyFolderFlags.CopySubfolders;
				break;
			case CopySubObjects.DoNotCopy:
				mapiCopyFolderFlags = CopyFolderFlags.None;
				break;
			default:
				throw new ArgumentOutOfRangeException("copySubObjects");
			}
			FolderChangeOperationFlags flags = FolderChangeOperationFlags.IncludeAll;
			using (CallbackContext callbackContext = new CallbackContext(base.Session))
			{
				try
				{
					if (base.Session != null)
					{
						base.Session.OnBeforeFolderChange(FolderChangeOperation.Copy, flags, base.Session, base.Session, this.StoreObjectId, destinationFolder.StoreObjectId, new StoreObjectId[]
						{
							sourceFolderId
						}, callbackContext);
					}
					result = this.ExecuteMapiGroupOperation("CopyFolder", new StoreObjectId[]
					{
						sourceFolderId
					}, delegate()
					{
						this.GetMapiFolder().CopyFolder(mapiCopyFolderFlags, destinationFolder.GetMapiFolder(), Folder.GetFolderProviderLevelItemId(sourceFolderId), newFolderName);
					});
				}
				finally
				{
					if (base.Session != null)
					{
						base.Session.OnAfterFolderChange(FolderChangeOperation.Copy, flags, base.Session, base.Session, this.StoreObjectId, destinationFolder.StoreObjectId, new StoreObjectId[]
						{
							sourceFolderId
						}, result, callbackContext);
					}
				}
			}
			return result;
		}

		internal void CheckIsNotDefaultFolder(StoreObjectId sourceFolderId, params DefaultFolderType[] allowedDefaultFolders)
		{
			if (!CoreFolder.CheckIsNotDefaultFolder(base.Session, sourceFolderId, allowedDefaultFolders))
			{
				ExTraceGlobals.StorageTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "Folder::CheckIsNotDefaultFolder. Failed to move/delete a folder. It was a default folder. FolderId = {0}", sourceFolderId);
				throw new CannotMoveDefaultFolderException(ServerStrings.ExCannotMoveOrDeleteDefaultFolders);
			}
		}

		internal static bool CheckIsNotDefaultFolder(StoreSession session, StoreObjectId sourceFolderId, params DefaultFolderType[] allowedDefaultFolders)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession != null)
			{
				DefaultFolderType defaultFolderType = mailboxSession.IsDefaultFolderType(sourceFolderId);
				if (defaultFolderType != DefaultFolderType.None && Array.IndexOf<DefaultFolderType>(allowedDefaultFolders, defaultFolderType) == -1)
				{
					return false;
				}
			}
			return true;
		}

		public PropertyError[] CopyProperties(CoreFolder destinationFolder, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] includeProperties)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(destinationFolder, "destinationFolder");
			Util.ThrowOnNullArgument(includeProperties, "includeProperties");
			EnumValidator.ThrowIfInvalid<CopyPropertiesFlags>(copyPropertiesFlags, "copyPropertiesFlags");
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(destinationFolder.Session, destinationFolder);
			destinationFolder.Session.ValidateOperation(FolderChangeOperation.Update, destinationFolder.StoreObjectId);
			if (Array.Exists<NativeStorePropertyDefinition>(includeProperties, (NativeStorePropertyDefinition x) => Array.IndexOf<NativeStorePropertyDefinition>(CoreFolder.permissionChangeRelatedProperties, x) != -1) && destinationFolder.IsPermissionChangeBlocked())
			{
				HashSet<NativeStorePropertyDefinition> hashSet = new HashSet<NativeStorePropertyDefinition>(includeProperties);
				hashSet.ExceptWith(CoreFolder.permissionChangeRelatedProperties);
				includeProperties = new NativeStorePropertyDefinition[hashSet.Count];
				hashSet.CopyTo(includeProperties);
			}
			return CoreObject.MapiCopyProps(this.FolderPropertyBag.MapiProp, destinationFolder.FolderPropertyBag.MapiProp, base.Session, destinationFolder.Session, copyPropertiesFlags, includeProperties);
		}

		public bool IsContentAvailable()
		{
			this.CheckDisposed(null);
			return CoreFolder.IsContentAvailable(base.Session, this.GetContentMailboxInfo());
		}

		public PublicFolderContentMailboxInfo GetContentMailboxInfo()
		{
			this.CheckDisposed(null);
			return CoreFolder.GetContentMailboxInfo(base.PropertyBag.GetValueOrDefault<string[]>(CoreFolderSchema.ReplicaList, Array<string>.Empty));
		}

		public static bool IsContentAvailable(StoreSession session, PublicFolderContentMailboxInfo contentMailboxInfo)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(contentMailboxInfo, "contentMailboxInfo");
			return session is MailboxSession || !contentMailboxInfo.IsValid || contentMailboxInfo.MailboxGuid == session.MailboxGuid;
		}

		public static PublicFolderContentMailboxInfo GetContentMailboxInfo(string[] replicaList)
		{
			Util.ThrowOnNullArgument(replicaList, "replicaList");
			return new PublicFolderContentMailboxInfo((replicaList.Length > 0) ? replicaList[0] : null);
		}

		internal void ClearNotReadNotificationPending()
		{
			this.ClearNotReadNotificationPending(null);
		}

		internal void ClearNotReadNotificationPending(StoreId[] itemIds)
		{
			this.InternalSetReadFlags(Microsoft.Mapi.SetReadFlags.DeferredErrors | Microsoft.Mapi.SetReadFlags.CleanNrnPending, itemIds);
		}

		internal void OnBeforeFolderSave()
		{
			if (!base.Session.IsMoveUser && base.Origin == Origin.Existing)
			{
				base.Session.ValidateOperation(FolderChangeOperation.Update, this.StoreObjectId);
			}
		}

		internal void OnAfterFolderSave()
		{
			PublicFolderSession publicFolderSession = base.Session as PublicFolderSession;
			if (publicFolderSession == null || !publicFolderSession.IsPrimaryHierarchySession || publicFolderSession.IsMoveUser)
			{
				return;
			}
			StoreObjectId storeObjectId = null;
			try
			{
				base.PropertyBag.Load(null);
				storeObjectId = this.StoreObjectId;
				if (storeObjectId.ObjectType != StoreObjectType.SearchFolder)
				{
					this.InternalSyncHierarchyToContentMailbox(publicFolderSession, storeObjectId);
				}
			}
			catch (StorageTransientException arg)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string, StorageTransientException>((long)this.GetHashCode(), "CoreFolder::OnAfterChangeForPublicFolder. Ignoring Error. Failed to sync properties on content folder. FolderId = {0}. Exception = {1}", (storeObjectId == null) ? "Unknown" : storeObjectId.ToHexEntryId(), arg);
			}
			catch (StoragePermanentException arg2)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string, StoragePermanentException>((long)this.GetHashCode(), "CoreFolder::OnAfterChangeForPublicFolder. Ignoring Error. Failed to sync properties on content folder. FolderId = {0}. Exception = {1}", (storeObjectId == null) ? "Unknown" : storeObjectId.ToHexEntryId(), arg2);
			}
		}

		internal static void InternalSetItemStatus(MapiFolder mapiFolder, StoreSession session, object currentObject, StoreObjectId itemId, MessageStatus bitsSet, MessageStatus bitsClear)
		{
			MessageStatus messageStatus;
			CoreFolder.InternalSetItemStatus(mapiFolder, session, currentObject, itemId, bitsSet, bitsClear, out messageStatus);
		}

		internal static void InternalSetItemStatus(MapiFolder mapiFolder, StoreSession session, object currentObject, StoreObjectId itemId, MessageStatus bitsSet, MessageStatus bitsClear, out MessageStatus oldStatus)
		{
			MessageStatus messageStatus = MessageStatus.None;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				mapiFolder.SetMessageStatus(itemId.ProviderLevelItemId, bitsSet, bitsClear, out messageStatus);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotSetMessageFlagStatus, ex, session, currentObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreFolder::InternalSetItemStatus.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotSetMessageFlagStatus, ex2, session, currentObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreFolder::InternalSetItemStatus.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			oldStatus = messageStatus;
		}

		internal static void InternalSetItemFlags(MapiFolder mapiFolder, StoreSession session, object currentObject, StoreObjectId itemId, MessageFlags bitsSet, MessageFlags bitsClear)
		{
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				mapiFolder.SetMessageFlags(itemId.ProviderLevelItemId, bitsSet, bitsClear);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotSetMessageFlags, ex, session, currentObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreFolder::InternalSetItemFlags.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotSetMessageFlags, ex2, session, currentObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("CoreFolder::InternalSetItemFlags.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		internal void InternalSetReadFlags(SetReadFlags flags, params StoreId[] itemIds)
		{
			this.InternalSetReadFlags(flags, itemIds, false);
		}

		internal void InternalSetReadFlags(SetReadFlags flags, StoreId[] itemIds, bool throwIfWarning)
		{
			MapiFolder mapiFolder = this.GetMapiFolder();
			StoreSession session = base.Session;
			object thisObject = null;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				bool allowWarnings = mapiFolder.AllowWarnings;
				try
				{
					mapiFolder.AllowWarnings = true;
					if (itemIds != null && itemIds.Length > 0)
					{
						mapiFolder.SetReadFlags(flags, StoreId.StoreIdsToEntryIds(itemIds), throwIfWarning);
					}
					else
					{
						mapiFolder.SetReadFlagsOnAllMessages(flags, throwIfWarning);
					}
				}
				finally
				{
					mapiFolder.AllowWarnings = allowWarnings;
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Failed to set read flags on messages in a folder", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetReadFlags, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Failed to set read flags on messages in a folder", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		protected override Schema GetSchema()
		{
			return this.schema;
		}

		protected override StorePropertyDefinition IdProperty
		{
			get
			{
				return CoreFolderSchema.Id;
			}
		}

		internal bool IsMailEnabled()
		{
			this.CheckDisposed(null);
			base.PropertyBag.Load(CoreFolder.MailEnabledFolderProperty);
			return base.PropertyBag.GetValueOrDefault<bool>(InternalSchema.MailEnabled);
		}

		internal bool IsPermissionChangeBlocked()
		{
			base.PropertyBag.Load(new NativeStorePropertyDefinition[]
			{
				CoreFolderSchema.PermissionChangeBlocked
			});
			return base.PropertyBag.GetValueAsNullable<bool>(CoreFolderSchema.PermissionChangeBlocked) != null;
		}

		internal static CoreFolder InternalBind(StoreSession storeSession, MapiFolder mapiFolder, StoreObjectId folderObjectId, byte[] changeKey, ICollection<PropertyDefinition> propsToReturn)
		{
			Util.ThrowOnNullArgument(storeSession, "storeSession");
			Util.ThrowOnNullArgument(mapiFolder, "mapiFolder");
			Util.ThrowOnNullArgument(folderObjectId, "folderObjectId");
			storeSession.CheckSystemFolderAccess(folderObjectId);
			propsToReturn = InternalSchema.Combine<PropertyDefinition>(FolderSchema.Instance.AutoloadProperties, propsToReturn);
			CoreFolder coreFolder = null;
			bool success = false;
			using (CallbackContext callbackContext = new CallbackContext(storeSession))
			{
				storeSession.OnBeforeFolderBind(folderObjectId, callbackContext);
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					try
					{
						FolderPropertyBag folderPropertyBag = new FolderPropertyBag(storeSession, mapiFolder, propsToReturn);
						disposeGuard.Add<FolderPropertyBag>(folderPropertyBag);
						StoreObjectType storeObjectType = InternalSchema.FolderId.GetStoreObjectType(folderPropertyBag);
						if (folderObjectId.ObjectType != storeObjectType)
						{
							folderObjectId = StoreObjectId.FromProviderSpecificId(folderObjectId.ProviderLevelItemId, storeObjectType);
						}
						FolderCreateInfo folderCreateInfo = FolderCreateInfo.GetFolderCreateInfo(storeObjectType);
						coreFolder = new CoreFolder(storeSession, folderCreateInfo.Schema, folderPropertyBag, folderObjectId, changeKey, Origin.Existing, propsToReturn);
						disposeGuard.Add<CoreFolder>(coreFolder);
						success = true;
					}
					finally
					{
						storeSession.OnAfterFolderBind(folderObjectId, coreFolder, success, callbackContext);
					}
					disposeGuard.Success();
				}
			}
			return coreFolder;
		}

		internal static CoreFolder InternalBind(StoreSession session, StoreId folderId, bool allowSoftDeleted, ICollection<PropertyDefinition> propsToReturn)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(folderId, "folderId");
			if (!Folder.IsFolderId(folderId))
			{
				throw new ArgumentException(ServerStrings.InvalidFolderId(folderId.ToBase64String()));
			}
			StoreObjectId storeObjectId;
			byte[] changeKey;
			StoreId.SplitStoreObjectIdAndChangeKey(folderId, out storeObjectId, out changeKey);
			CoreFolder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				OpenEntryFlags openEntryFlags = OpenEntryFlags.BestAccess | OpenEntryFlags.DeferredErrors;
				if (allowSoftDeleted)
				{
					openEntryFlags |= OpenEntryFlags.ShowSoftDeletes;
				}
				MapiProp mapiProp = session.GetMapiProp(storeObjectId, openEntryFlags);
				disposeGuard.Add<MapiProp>(mapiProp);
				MapiFolder mapiFolder = (MapiFolder)mapiProp;
				disposeGuard.Add<MapiFolder>(mapiFolder);
				CoreFolder coreFolder = CoreFolder.InternalBind(session, mapiFolder, storeObjectId, changeKey, propsToReturn);
				disposeGuard.Add<CoreFolder>(coreFolder);
				disposeGuard.Success();
				result = coreFolder;
			}
			return result;
		}

		internal static CoreFolder InternalCreate(StoreSession session, StoreId parentId, bool isSearchFolder, string displayName, CreateMode createMode, bool isSecure, FolderCreateInfo folderCreateInfo)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentId, "parentId");
			EnumValidator.ThrowIfInvalid<CreateMode>(createMode, "createMode");
			Util.ThrowOnNullArgument(folderCreateInfo, "folderCreateInfo");
			session.ValidateOperation(FolderChangeOperation.Create, StoreId.GetStoreObjectId(parentId));
			PublicFolderSession publicFolderSession = session as PublicFolderSession;
			if (isSearchFolder && publicFolderSession != null && !publicFolderSession.IsSystemOperation())
			{
				throw new NoSupportException(ServerStrings.CannotCreateSearchFoldersInPublicStore);
			}
			if (session.BlockFolderCreation && !createMode.HasFlag(CreateMode.OverrideFolderCreationBlock))
			{
				throw new FolderCreationBlockedException();
			}
			CoreFolder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				FolderPropertyBag folderPropertyBag = new FolderCreatePropertyBag(session, StoreId.GetStoreObjectId(parentId), isSearchFolder, createMode, isSecure, folderCreateInfo.Schema.AutoloadProperties);
				disposeGuard.Add<FolderPropertyBag>(folderPropertyBag);
				if (!string.IsNullOrEmpty(displayName))
				{
					folderPropertyBag[CoreFolderSchema.DisplayName] = displayName;
				}
				CoreFolder coreFolder = new CoreFolder(session, folderCreateInfo.Schema, folderPropertyBag, null, null, Origin.New, folderCreateInfo.Schema.AutoloadProperties);
				disposeGuard.Add<CoreFolder>(coreFolder);
				disposeGuard.Success();
				result = coreFolder;
			}
			return result;
		}

		internal static DeleteItemFlags NormalizeDeleteFlags(DeleteItemFlags flags)
		{
			return flags & DeleteItemFlags.NormalizedDeleteFlags;
		}

		private static FolderChangeOperationFlags GetFolderChangeOperationFlags(FolderChangeOperationFlags baseFlags, DeleteItemFlags deleteFlags)
		{
			if ((deleteFlags & DeleteItemFlags.DeclineCalendarItemWithoutResponse) == DeleteItemFlags.DeclineCalendarItemWithoutResponse)
			{
				return baseFlags | FolderChangeOperationFlags.DeclineCalendarItemWithoutResponse;
			}
			if ((deleteFlags & DeleteItemFlags.DeclineCalendarItemWithResponse) == DeleteItemFlags.DeclineCalendarItemWithResponse)
			{
				return baseFlags | FolderChangeOperationFlags.DeclineCalendarItemWithResponse;
			}
			if ((deleteFlags & DeleteItemFlags.CancelCalendarItem) == DeleteItemFlags.CancelCalendarItem)
			{
				return baseFlags | FolderChangeOperationFlags.CancelCalendarItem;
			}
			if ((deleteFlags & DeleteItemFlags.DeleteAllClutter) == DeleteItemFlags.DeleteAllClutter)
			{
				return baseFlags | FolderChangeOperationFlags.DeleteAllClutter | FolderChangeOperationFlags.EmptyFolder;
			}
			if ((deleteFlags & DeleteItemFlags.EmptyFolder) == DeleteItemFlags.EmptyFolder)
			{
				return baseFlags | FolderChangeOperationFlags.EmptyFolder;
			}
			return baseFlags;
		}

		private static DeleteMessagesFlags MapiDeleteFlagsFromXsoDeleteFlags(DeleteItemFlags flags)
		{
			DeleteItemFlags deleteItemFlags = CoreFolder.NormalizeDeleteFlags(flags);
			DeleteMessagesFlags result = DeleteMessagesFlags.None;
			if (deleteItemFlags == DeleteItemFlags.HardDelete)
			{
				result = DeleteMessagesFlags.ForceHardDelete;
			}
			return result;
		}

		private void ResolveStoreObjectType(ICollection<StoreObjectId> sourceIds)
		{
			foreach (StoreObjectId storeObjectId in sourceIds)
			{
				if (storeObjectId.ObjectType == StoreObjectType.Unknown)
				{
					try
					{
						StoreObjectId parentIdFromMessageId = IdConverter.GetParentIdFromMessageId(storeObjectId);
						bool flag;
						if (!base.Session.IsContactFolder.TryGetValue(parentIdFromMessageId, out flag))
						{
							using (CoreFolder coreFolder = CoreFolder.Bind(base.Session, parentIdFromMessageId))
							{
								flag = (coreFolder.StoreObjectId.ObjectType == StoreObjectType.ContactsFolder);
							}
							base.Session.IsContactFolder.Add(parentIdFromMessageId, flag);
						}
						if (flag)
						{
							using (ICoreItem coreItem = CoreItem.Bind(base.Session, storeObjectId))
							{
								storeObjectId.UpdateItemType(coreItem.StoreObjectId.ObjectType);
							}
						}
					}
					catch (StoragePermanentException)
					{
					}
					catch (StorageTransientException)
					{
					}
				}
			}
		}

		private bool SourceIdsContainContactsOrDistributionsLists(ICollection<StoreObjectId> sourceIds)
		{
			foreach (StoreObjectId storeObjectId in sourceIds)
			{
				if (storeObjectId.ObjectType == StoreObjectType.DistributionList || storeObjectId.ObjectType == StoreObjectType.Contact)
				{
					return true;
				}
			}
			return false;
		}

		private StoreObjectId StoreObjectId
		{
			get
			{
				return ((ICoreObject)this).StoreObjectId;
			}
		}

		private FolderPropertyBag FolderPropertyBag
		{
			get
			{
				this.CheckDisposed(null);
				return (FolderPropertyBag)base.PropertyBag;
			}
		}

		private MapiFolder GetMapiFolder()
		{
			this.CheckDisposed(null);
			return (MapiFolder)CoreObject.GetPersistablePropertyBag(this).MapiProp;
		}

		private GroupOperationResult ExecuteMapiGroupOperationMethod(string operationAttempted, StoreObjectId[] sourceObjectIds, CoreFolder.MapiGroupOperationWithResult mapiGroupOperationCall)
		{
			byte[][] array = null;
			byte[][] array2 = null;
			StoreSession session = base.Session;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				mapiGroupOperationCall(out array, out array2);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCopyMessagesFailed, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Failed to {0}", operationAttempted),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCopyMessagesFailed, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Failed to {0}", operationAttempted),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			if ((array != null || array2 != null) && array.Length != array2.Length)
			{
				throw new InvalidOperationException("Unexpected result returned from MAPI. Different number of new entryids and change numbers");
			}
			if (array != null && array2 != null && array.Length == sourceObjectIds.Length)
			{
				StoreObjectId[] array3 = new StoreObjectId[sourceObjectIds.Length];
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i] = StoreObjectId.FromProviderSpecificId(array[i], sourceObjectIds[i].ObjectType);
				}
				return new GroupOperationResult(OperationResult.Succeeded, sourceObjectIds, null, array3, array2);
			}
			return new GroupOperationResult(OperationResult.Succeeded, sourceObjectIds, null);
		}

		private GroupOperationResult ExecuteMapiGroupOperationMethod(string operationAttempted, StoreObjectId[] sourceObjectIds, CoreFolder.MapiGroupOperation mapiGroupOperationCall)
		{
			StoreSession session = base.Session;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				mapiGroupOperationCall();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCopyMessagesFailed, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Failed to {0}", operationAttempted),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCopyMessagesFailed, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Failed to {0}", operationAttempted),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return new GroupOperationResult(OperationResult.Succeeded, sourceObjectIds, null);
		}

		private GroupOperationResult ExecuteMapiGroupOperationInternal(StoreObjectId[] sourceObjectIds, CoreFolder.MapiGroupOperationInternalMethod method)
		{
			foreach (StoreObjectId id in sourceObjectIds)
			{
				base.Session.CheckSystemFolderAccess(id);
			}
			MapiFolder mapiFolder = this.GetMapiFolder();
			bool allowWarnings = mapiFolder.AllowWarnings;
			GroupOperationResult result;
			try
			{
				mapiFolder.AllowWarnings = true;
				result = method();
			}
			catch (PartialCompletionException storageException)
			{
				result = new GroupOperationResult(OperationResult.PartiallySucceeded, sourceObjectIds, storageException);
			}
			catch (ObjectExistedException storageException2)
			{
				result = new GroupOperationResult(OperationResult.Failed, sourceObjectIds, storageException2);
			}
			catch (FolderCycleException storageException3)
			{
				result = new GroupOperationResult(OperationResult.Failed, sourceObjectIds, storageException3);
			}
			finally
			{
				mapiFolder.AllowWarnings = allowWarnings;
			}
			return result;
		}

		private GroupOperationResult ExecuteMapiGroupOperation(string operationAttempted, StoreObjectId[] sourceObjectIds, CoreFolder.MapiGroupOperationWithResult mapiGroupOperationCall)
		{
			return this.ExecuteMapiGroupOperationInternal(sourceObjectIds, () => this.ExecuteMapiGroupOperationMethod(operationAttempted, sourceObjectIds, mapiGroupOperationCall));
		}

		private GroupOperationResult ExecuteMapiGroupOperation(string operationAttempted, StoreObjectId[] sourceObjectIds, CoreFolder.MapiGroupOperation mapiGroupOperationCall)
		{
			return this.ExecuteMapiGroupOperationInternal(sourceObjectIds, () => this.ExecuteMapiGroupOperationMethod(operationAttempted, sourceObjectIds, mapiGroupOperationCall));
		}

		private PropValue[] OptionalXsoPropertiesToPropValues(PropertyDefinition[] propertyDefinitions, object[] propertyValues)
		{
			if (propertyDefinitions == null)
			{
				return Array<PropValue>.Empty;
			}
			return MapiPropertyBag.MapiPropValuesFromXsoProperties(base.Session, this.GetMapiFolder(), propertyDefinitions, propertyValues);
		}

		internal static byte[][] StoreObjectIdsToEntryIds(StoreSession storeSession, StoreObjectId[] storeObjectIds)
		{
			byte[][] array = new byte[storeObjectIds.Length][];
			for (int i = 0; i < storeObjectIds.Length; i++)
			{
				array[i] = storeSession.IdConverter.GetSessionSpecificId(storeObjectIds[i]).ProviderLevelItemId;
			}
			return array;
		}

		internal void ValidateItemIds(StoreObjectId[] storeObjectIds, CoreFolder.ItemIdValidatorDelegate itemIdValidator)
		{
			foreach (StoreObjectId storeObjectId in storeObjectIds)
			{
				if (IdConverter.IsFolderId(storeObjectId))
				{
					throw new ArgumentException(ServerStrings.ExInvalidItemId);
				}
				if (storeObjectId is OccurrenceStoreObjectId)
				{
					throw new ArgumentException(ServerStrings.ExCannotMoveOrCopyOccurrenceItem(storeObjectId));
				}
				if (itemIdValidator != null)
				{
					itemIdValidator(storeObjectId);
				}
			}
		}

		internal static bool IsMovingToDeletedItems(StoreSession session, StoreObjectId destinationFolderId)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			return mailboxSession != null && mailboxSession.IsDefaultFolderType(destinationFolderId) == DefaultFolderType.DeletedItems;
		}

		internal GroupOperationResult InternalMoveOrCopyItems(CoreFolder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues, bool move, bool returnNewIds, DeleteItemFlags? deleteFlags, bool updateSource)
		{
			this.CheckDisposed(null);
			bool flag = returnNewIds;
			this.ResolveStoreObjectType(sourceItemIds);
			TeamMailboxClientOperations.ThrowIfInvalidFolderOperation(destinationFolder.Session, destinationFolder);
			if (base.Session == destinationFolder.Session && (this.SourceIdsContainContactsOrDistributionsLists(sourceItemIds) || (base.Session.ActivitySession != null && move)))
			{
				returnNewIds = true;
			}
			PropValue[] propValues = this.OptionalXsoPropertiesToPropValues(propertyDefinitions, propertyValues);
			StoreObjectId[] array = sourceItemIds;
			Set<StoreObjectId> set = null;
			GroupOperationResult groupOperationResult = null;
			if (move && sourceItemIds.Length > 1)
			{
				set = new Set<StoreObjectId>(sourceItemIds.Length);
				for (int i = 0; i < sourceItemIds.Length; i++)
				{
					set.SafeAdd(sourceItemIds[i]);
				}
				array = set.ToArray();
			}
			FolderChangeOperationFlags folderChangeOperationFlags = FolderChangeOperationFlags.IncludeAll;
			FolderChangeOperation operation;
			if (!move)
			{
				operation = FolderChangeOperation.Copy;
			}
			else if (CoreFolder.IsMovingToDeletedItems(base.Session, destinationFolder.Id.ObjectId))
			{
				operation = FolderChangeOperation.MoveToDeletedItems;
				if (deleteFlags != null)
				{
					folderChangeOperationFlags = CoreFolder.GetFolderChangeOperationFlags(folderChangeOperationFlags, deleteFlags.Value);
				}
			}
			else
			{
				operation = FolderChangeOperation.Move;
				if (deleteFlags != null && (deleteFlags.Value & DeleteItemFlags.ClutterActionByUserOverride) != DeleteItemFlags.None)
				{
					folderChangeOperationFlags |= FolderChangeOperationFlags.ClutterActionByUserOverride;
				}
			}
			ICollection<StoreObjectId> itemIds = null;
			ICollection<StoreObjectId> collection2;
			if (set != null)
			{
				ICollection<StoreObjectId> collection = set;
				collection2 = collection;
			}
			else
			{
				collection2 = sourceItemIds;
			}
			itemIds = collection2;
			using (CallbackContext callbackContext = new CallbackContext(base.Session))
			{
				try
				{
					base.Session.OnBeforeFolderChange(operation, folderChangeOperationFlags, base.Session, destinationFolder.Session, this.StoreObjectId, destinationFolder.StoreObjectId, itemIds, callbackContext);
					byte[][] entryIds = CoreFolder.StoreObjectIdsToEntryIds(base.Session, array);
					CopyMessagesFlags flags = move ? CopyMessagesFlags.Move : CopyMessagesFlags.None;
					if (!move && !updateSource)
					{
						flags |= CopyMessagesFlags.DontUpdateSource;
					}
					if (returnNewIds)
					{
						groupOperationResult = this.ExecuteMapiGroupOperation("InternalMoveOrCopyItems", array, delegate(out byte[][] newEntryIds, out byte[][] newChangeNumbers)
						{
							this.GetMapiFolder().CopyMessages(flags, destinationFolder.GetMapiFolder(), propValues, entryIds, out newEntryIds, out newChangeNumbers);
						});
					}
					else if (propValues.Length > 0)
					{
						groupOperationResult = this.ExecuteMapiGroupOperation("InternalMoveOrCopyItems", array, delegate()
						{
							this.GetMapiFolder().CopyMessages(flags, destinationFolder.GetMapiFolder(), propValues, entryIds);
						});
					}
					else
					{
						groupOperationResult = this.ExecuteMapiGroupOperation("InternalMoveOrCopyItems", array, delegate()
						{
							this.GetMapiFolder().CopyMessages(flags, destinationFolder.GetMapiFolder(), entryIds);
						});
					}
				}
				finally
				{
					base.Session.OnAfterFolderChange(operation, folderChangeOperationFlags, base.Session, destinationFolder.Session, this.StoreObjectId, destinationFolder.StoreObjectId, itemIds, groupOperationResult, callbackContext);
				}
			}
			if (flag != returnNewIds)
			{
				groupOperationResult = new GroupOperationResult(groupOperationResult.OperationResult, groupOperationResult.ObjectIds, groupOperationResult.Exception);
			}
			return groupOperationResult;
		}

		private void CoreObjectUpdate()
		{
			((FolderSchema)this.GetSchema()).CoreObjectUpdate(this);
		}

		public IModifyTable GetPermissionTableDoNotLoadEntries(ModifyTableOptions options)
		{
			return this.GetPermissionTable(options, false, false);
		}

		public IModifyTable GetPermissionTable(ModifyTableOptions options)
		{
			return this.GetPermissionTable(options, false);
		}

		internal IModifyTable GetPermissionTable(ModifyTableOptions options, bool useSecurityDescriptorOnly)
		{
			return this.GetPermissionTable(options, false, true);
		}

		internal AclTableIdMap AclTableIdMap
		{
			get
			{
				if (this.aclTableIdMap == null)
				{
					this.aclTableIdMap = new AclTableIdMap();
				}
				return this.aclTableIdMap;
			}
		}

		private IModifyTable GetPermissionTable(ModifyTableOptions options, bool useSecurityDescriptorOnly, bool loadTableEntries)
		{
			EnumValidator.ThrowIfInvalid<ModifyTableOptions>(options, "options");
			if (base.Session.IsE15Session)
			{
				return new AclModifyTable(this, options, new MapiAclTableRestriction(this), useSecurityDescriptorOnly, loadTableEntries);
			}
			return new PropertyTable(this, CoreFolderSchema.MapiAclTable, options, new MapiAclTableRestriction(this));
		}

		private void InternalSyncHierarchyToContentMailbox(PublicFolderSession publicFolderSession, StoreObjectId folderId)
		{
			PublicFolderContentMailboxInfo contentMailboxInfo = this.GetContentMailboxInfo();
			Guid guid = contentMailboxInfo.IsValid ? contentMailboxInfo.MailboxGuid : Guid.Empty;
			ExchangePrincipal contentMailboxPrincipal;
			if (guid != Guid.Empty && guid != publicFolderSession.MailboxGuid && PublicFolderSession.TryGetPublicFolderMailboxPrincipal(publicFolderSession.OrganizationId, guid, false, out contentMailboxPrincipal))
			{
				PublicFolderSyncJobRpc.SyncFolder(contentMailboxPrincipal, folderId.ProviderLevelItemId);
			}
		}

		private readonly FolderSchema schema;

		private readonly QueryExecutor queryExecutor;

		private static readonly PropertyDefinition[] MailEnabledFolderProperty = new PropertyDefinition[]
		{
			InternalSchema.MailEnabled
		};

		private static NativeStorePropertyDefinition[] permissionChangeRelatedProperties = new NativeStorePropertyDefinition[]
		{
			CoreFolderSchema.PermissionChangeBlocked,
			CoreFolderSchema.RawSecurityDescriptor,
			CoreFolderSchema.RawFreeBusySecurityDescriptor
		};

		private static StorePropertyDefinition[] propertiesToSyncForPublicFolders = new StorePropertyDefinition[]
		{
			InternalSchema.OverallAgeLimit,
			InternalSchema.RetentionAgeLimit,
			InternalSchema.PfOverHardQuotaLimit,
			InternalSchema.PfStorageQuota,
			InternalSchema.PfMsgSizeLimit,
			InternalSchema.DisablePerUserRead
		};

		private AclTableIdMap aclTableIdMap;

		internal delegate void MapiGroupOperation();

		internal delegate void MapiGroupOperationWithResult(out byte[][] newEntryIds, out byte[][] newChangeNumbers);

		private delegate GroupOperationResult MapiGroupOperationInternalMethod();

		internal delegate void ItemIdValidatorDelegate(StoreObjectId storeObjectIds);
	}
}
