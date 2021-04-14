using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Folder : PropertyServerObject
	{
		internal Folder(CoreFolder coreFolder, Logon logon) : this(coreFolder, logon, ClientSideProperties.FolderInstance, PropertyConverter.Folder)
		{
		}

		internal Folder(CoreFolder coreFolder, Logon logon, ClientSideProperties clientSideProperties, PropertyConverter converter) : base(logon, clientSideProperties, converter)
		{
			this.coreFolderReference = new ReferenceCount<CoreFolder>(coreFolder);
			this.propertyDefinitionFactory = new CoreObjectPropertyDefinitionFactory(coreFolder.Session, coreFolder.Session.Mailbox.CoreObject.PropertyBag);
			this.storageObjectProperties = new CoreObjectProperties(coreFolder.PropertyBag);
			if (ExTraceGlobals.FolderTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				StoreId storeId = (coreFolder.Id != null) ? new StoreId(coreFolder.Session.IdConverter.GetFidFromId(coreFolder.Id.ObjectId)) : StoreId.Empty;
				ExTraceGlobals.FolderTracer.TraceInformation(0, Activity.TraceId, "Bind: Fid={0}; ParentFid={1}; ItemCount={2}; UnreadCount={3}; DisplayName=\"{4}\"", new object[]
				{
					storeId,
					new StoreId(coreFolder.PropertyBag.GetValueAsNullable<long>(CoreObjectSchema.ParentFid) ?? 0L),
					coreFolder.PropertyBag.GetValueAsNullable<int>(CoreFolderSchema.ItemCount),
					coreFolder.PropertyBag.GetValueAsNullable<int>(CoreFolderSchema.UnreadCount),
					coreFolder.PropertyBag.GetValueOrDefault<string>(CoreFolderSchema.DisplayName)
				});
			}
		}

		protected override IPropertyDefinitionFactory PropertyDefinitionFactory
		{
			get
			{
				return this.propertyDefinitionFactory;
			}
		}

		protected override IStorageObjectProperties StorageObjectProperties
		{
			get
			{
				return this.storageObjectProperties;
			}
		}

		protected ICorePropertyBag PropertyBag
		{
			get
			{
				return this.coreFolderReference.ReferencedObject.PropertyBag;
			}
		}

		public override StoreSession Session
		{
			get
			{
				return this.coreFolderReference.ReferencedObject.Session;
			}
		}

		public override Schema Schema
		{
			get
			{
				return FolderSchema.Instance;
			}
		}

		internal CoreFolder CoreFolder
		{
			get
			{
				return this.coreFolderReference.ReferencedObject;
			}
		}

		internal ReferenceCount<CoreFolder> CoreFolderReference
		{
			get
			{
				return this.coreFolderReference;
			}
		}

		public Folder CreateFolder(FolderType folderType, CreateFolderFlags flags, string displayName, string folderComment, StoreLongTermId? longTermId, out ReplicaServerInfo? replicaInfo, out bool existed, out bool hasRules, out StoreId storeId)
		{
			Util.ThrowOnNullArgument(displayName, "displayName");
			Util.ThrowOnNullArgument(folderComment, "folderComment");
			if (longTermId != null)
			{
				throw Feature.NotImplemented(59649, "MailboxMove: import folder through CreateFolder");
			}
			if (folderType != FolderType.Normal && folderType != FolderType.Search)
			{
				throw new RopExecutionException(string.Format("FolderType {0} invalid.", folderType), (ErrorCode)2147942487U);
			}
			bool flag = this.Session is PublicFolderSession;
			if (folderType == FolderType.Search && flag)
			{
				throw new RopExecutionException("Search folders cannot be created for public logons.", (ErrorCode)2147746050U);
			}
			RopHandler.CheckEnum<CreateFolderFlags>(flags);
			Folder.CheckDisplayNameValid(displayName, ErrorCode.BadFolderName);
			if ((flags & CreateFolderFlags.CreatePublicFolderDumpster) == CreateFolderFlags.CreatePublicFolderDumpster)
			{
				throw new RopExecutionException("CreatePublicFolderDumpster flag cannot be passed via MOMT.", (ErrorCode)2147942487U);
			}
			if ((flags & CreateFolderFlags.InternalAccess) == CreateFolderFlags.InternalAccess)
			{
				throw new RopExecutionException("InternalAccess flag cannot be passed via MOMT.", (ErrorCode)2147942487U);
			}
			Folder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				PublicLogon publicLogon = base.LogonObject as PublicLogon;
				CoreFolder coreFolder;
				if (publicLogon != null && !publicLogon.IsPrimaryHierarchyLogon)
				{
					StoreId folderId = PublicFolderOperations.CreateFolder(publicLogon, displayName, folderComment, this.CoreFolder.Id, flags);
					coreFolder = CoreFolder.Bind(this.Session, folderId);
					disposeGuard.Add<CoreFolder>(coreFolder);
				}
				else
				{
					CreateMode createMode = ((flags & CreateFolderFlags.OpenIfExists) == CreateFolderFlags.OpenIfExists) ? CreateMode.OpenIfExists : CreateMode.CreateNew;
					if ((flags & CreateFolderFlags.InstantSearch) == CreateFolderFlags.InstantSearch)
					{
						if (folderType != FolderType.Search || createMode == CreateMode.OpenIfExists)
						{
							throw new RopExecutionException("InstantSearch flag can be used only for creating new search folders.", (ErrorCode)2147942487U);
						}
						createMode |= CreateMode.InstantSearch;
					}
					coreFolder = CoreFolder.Create(this.Session, this.CoreFolder.Id.ObjectId, folderType == FolderType.Search, displayName, createMode);
					disposeGuard.Add<CoreFolder>(coreFolder);
					if (!string.IsNullOrEmpty(folderComment))
					{
						coreFolder.PropertyBag[CoreFolderSchema.Description] = folderComment;
					}
					else
					{
						coreFolder.PropertyBag.Delete(CoreFolderSchema.Description);
					}
					bool flag2 = false;
					if (TeamMailboxClientOperations.IsLinkedFolder(this.CoreFolder, false))
					{
						if (!Configuration.ServiceConfiguration.TMPublishEnabled)
						{
							throw new RopExecutionException("Shortcut folder feature is turned off", (ErrorCode)2147746050U);
						}
						TeamMailboxClientOperations teamMailboxClientOperations = TeamMailboxExecutionHelper.GetTeamMailboxClientOperations(base.LogonObject.Connection);
						TeamMailboxExecutionHelper.RunOperationWithExceptionAndExecutionLimitHandler(delegate
						{
							teamMailboxClientOperations.OnCreateFolder(this.CoreFolder, coreFolder, displayName);
						}, "TeamMailboxClientOperations.OnCreateFolder", null);
						flag2 = true;
					}
					coreFolder.Save(SaveMode.NoConflictResolution);
					coreFolder.PropertyBag.Load(null);
					if (flag2)
					{
						((MailboxSession)coreFolder.Session).TryToSyncSiteMailboxNow();
					}
				}
				hasRules = coreFolder.PropertyBag.GetValueOrDefault<bool>(FolderSchema.HasRules);
				if (flag)
				{
					replicaInfo = PublicLogon.GetReplicaServerInfo(coreFolder, true);
				}
				else
				{
					replicaInfo = null;
				}
				existed = (replicaInfo != null && replicaInfo.Value.Replicas.Length != 0);
				storeId = new StoreId(coreFolder.Session.IdConverter.GetFidFromId(((ICoreObject)coreFolder).StoreObjectId));
				Folder folder = new Folder(coreFolder, base.LogonObject);
				disposeGuard.Success();
				result = folder;
			}
			return result;
		}

		public void DeleteFolder(DeleteFolderFlags deleteFolderFlags, StoreId folderId, out bool isPartiallyCompleted)
		{
			if (folderId == StoreId.Empty)
			{
				throw new RopExecutionException(string.Format("FolderId {0} invalid.", folderId), (ErrorCode)2147942487U);
			}
			StoreObjectId folderId2 = this.Session.IdConverter.CreateFolderId(folderId);
			PublicLogon publicLogon = base.LogonObject as PublicLogon;
			if (publicLogon != null && !publicLogon.IsPrimaryHierarchyLogon)
			{
				PublicFolderOperations.DeleteFolder(publicLogon, this.CoreFolder.Id, folderId2, deleteFolderFlags);
				isPartiallyCompleted = false;
				return;
			}
			DeleteFolderFlags deleteFolderFlags2 = DeleteFolderFlags.None;
			if ((byte)(deleteFolderFlags & DeleteFolderFlags.DeleteMessages) != 0)
			{
				deleteFolderFlags2 |= DeleteFolderFlags.DeleteMessages;
			}
			if ((byte)(deleteFolderFlags & DeleteFolderFlags.DeleteFolders) != 0)
			{
				deleteFolderFlags2 |= DeleteFolderFlags.DeleteSubFolders;
			}
			if ((byte)(deleteFolderFlags & DeleteFolderFlags.HardDelete) != 0)
			{
				deleteFolderFlags2 |= DeleteFolderFlags.HardDelete;
			}
			bool flag = false;
			if (Utils.IsTeamMailbox(this.CoreFolder.Session))
			{
				using (Folder sourceFolder = Folder.Bind(this.CoreFolder.Session, folderId2))
				{
					if (TeamMailboxClientOperations.IsLinkedFolder(sourceFolder))
					{
						if (!Configuration.ServiceConfiguration.TMPublishEnabled)
						{
							throw new RopExecutionException("Shortcut folder feature is turned off", (ErrorCode)2147746050U);
						}
						if (sourceFolder.PropertyBag.GetValueOrDefault<bool>(FolderSchema.IsDocumentLibraryFolder, false))
						{
							throw new RopExecutionException("Deleting a root document library folder is not allowed", (ErrorCode)2147746050U);
						}
						TeamMailboxClientOperations teamMailboxClientOperations = TeamMailboxExecutionHelper.GetTeamMailboxClientOperations(base.LogonObject.Connection);
						TeamMailboxExecutionHelper.RunOperationWithExceptionAndExecutionLimitHandler(delegate
						{
							teamMailboxClientOperations.OnDeleteFolder(sourceFolder);
						}, "TeamMailboxClientOperations.OnDeleteFolder", null);
						flag = true;
					}
				}
			}
			GroupOperationResult groupOperationResult = this.CoreFolder.DeleteFolder(deleteFolderFlags2, folderId2);
			if (flag && groupOperationResult.OperationResult == OperationResult.Succeeded)
			{
				((MailboxSession)this.CoreFolder.Session).TryToSyncSiteMailboxNow();
			}
			Folder.ConvertXSOOperationResultToPartialSucceeded(groupOperationResult.OperationResult, groupOperationResult.Exception, out isPartiallyCompleted);
		}

		public void DeleteMessages(StoreId[] messageIds, DeleteItemFlags deleteItemFlags, bool isOkToSendNonReadNotification, bool reportProgress, object progressToken)
		{
			if (!isOkToSendNonReadNotification)
			{
				deleteItemFlags |= DeleteItemFlags.SuppressReadReceipt;
			}
			StoreObjectId[] array = this.ConvertMessageIdsToStoreObjectIds(messageIds);
			IAsyncOperationExecutor asyncOperationExecutor;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				int segmentSize = reportProgress ? SegmentEnumerator.MessageSegmentSize : array.Length;
				TeamMailboxClientOperations teamMailboxClientOperations = null;
				if (TeamMailboxClientOperations.IsLinkedFolder(this.coreFolderReference.ReferencedObject, false))
				{
					if (!Configuration.ServiceConfiguration.TMPublishEnabled)
					{
						throw new RopExecutionException("Shortcut folder feature is turned off", (ErrorCode)2147746050U);
					}
					teamMailboxClientOperations = TeamMailboxExecutionHelper.GetTeamMailboxClientOperations(base.LogonObject.Connection);
				}
				DeleteMessagesSegmentedOperation deleteMessagesSegmentedOperation;
				if ((deleteItemFlags & DeleteItemFlags.HardDelete) == DeleteItemFlags.HardDelete)
				{
					deleteMessagesSegmentedOperation = new HardDeleteMessagesSegmentedOperation(this.coreFolderReference, deleteItemFlags, array, segmentSize, teamMailboxClientOperations);
				}
				else
				{
					deleteMessagesSegmentedOperation = new SoftDeleteMessagesSegmentedOperation(this.coreFolderReference, deleteItemFlags, array, segmentSize, teamMailboxClientOperations);
				}
				disposeGuard.Add<DeleteMessagesSegmentedOperation>(deleteMessagesSegmentedOperation);
				asyncOperationExecutor = base.LogonObject.CreateAsyncOperationExecutor(deleteMessagesSegmentedOperation, progressToken);
				disposeGuard.Success();
			}
			asyncOperationExecutor.BeginOperation(!reportProgress);
		}

		public void EmptyFolder(bool reportProgress, object progressToken, EmptyFolderFlags emptyFolderFlags, bool isHardDelete)
		{
			if (RopHandler.IsSearchFolder(this.CoreFolder.Id))
			{
				throw new RopExecutionException("EmptyFolder on a search folder is not supported.", (ErrorCode)2147746050U);
			}
			if (TeamMailboxClientOperations.IsLinkedFolder(this.CoreFolder, false))
			{
				throw new RopExecutionException("EmptyFolder on a Shorcut folder is not supported.", (ErrorCode)2147746050U);
			}
			PrivateLogon privateLogon = base.LogonObject as PrivateLogon;
			if (privateLogon != null)
			{
				StoreObjectId defaultFolderId = privateLogon.MailboxSession.GetDefaultFolderId(DefaultFolderType.Configuration);
				if (defaultFolderId != null && this.CoreFolder.Id.ObjectId.Equals(defaultFolderId))
				{
					throw new RopExecutionException("EmptyFolder on Mailbox Root is not supported.", (ErrorCode)2147746050U);
				}
			}
			EmptyFolderFlags emptyFolderFlags2 = EmptyFolderFlags.None;
			if (emptyFolderFlags != EmptyFolderFlags.None)
			{
				emptyFolderFlags2 |= EmptyFolderFlags.DeleteAssociatedMessages;
			}
			if (isHardDelete)
			{
				emptyFolderFlags2 |= EmptyFolderFlags.HardDelete;
			}
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				EmptyFolderSegmentedOperation emptyFolderSegmentedOperation;
				if (isHardDelete)
				{
					emptyFolderSegmentedOperation = new HardEmptyFolderSegmentedOperation(this.coreFolderReference, emptyFolderFlags2);
				}
				else
				{
					emptyFolderSegmentedOperation = new SoftEmptyFolderSegmentedOperation(this.coreFolderReference, emptyFolderFlags2);
				}
				disposeGuard.Add<EmptyFolderSegmentedOperation>(emptyFolderSegmentedOperation);
				IAsyncOperationExecutor asyncOperationExecutor = base.LogonObject.CreateAsyncOperationExecutor(emptyFolderSegmentedOperation, progressToken);
				disposeGuard.Success();
				asyncOperationExecutor.BeginOperation(!reportProgress);
			}
		}

		public PermissionsView CreatePermissionsView(Logon logon, TableFlags tableFlags, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle)
		{
			TableFlags tableFlags2 = TableFlags.Associated | TableFlags.NoNotifications | TableFlags.SuppressNotifications;
			RopHandler.CheckEnum<TableFlags>(tableFlags);
			if ((byte)(tableFlags & ~tableFlags2) != 0)
			{
				throw new RopExecutionException(string.Format("tableFlags {0} unsupported", tableFlags & ~tableFlags2), (ErrorCode)2147746050U);
			}
			return new PermissionsView(logon, this.CoreFolderReference, TableFlags.NoNotifications | tableFlags, notificationHandler, returnNotificationHandle);
		}

		public ContentsView CreateContentsView(Logon logon, TableFlags tableFlags, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle, out int rowCount)
		{
			Folder folder = this;
			ContentsView result;
			try
			{
				if (logon.Connection.ClientInformation.Mode != ClientMode.ExchangeServer && (byte)(tableFlags & TableFlags.Depth) == 4)
				{
					throw new RopExecutionException("TableFlags.Depth is invalid", (ErrorCode)2147942487U);
				}
				this.ClearCacheIfNeededForGetProperties();
				this.PropertyBag.Load(null);
				if ((byte)(tableFlags & TableFlags.SoftDeletes) == 32)
				{
					StoreObjectId storeObjectId = null;
					PrivateLogon privateLogon = base.LogonObject as PrivateLogon;
					if (privateLogon != null)
					{
						storeObjectId = privateLogon.MailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDeletions);
					}
					else
					{
						PublicLogon publicLogon = base.LogonObject as PublicLogon;
						if (publicLogon != null)
						{
							storeObjectId = PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId(this.CoreFolder);
						}
					}
					if (storeObjectId != null && !this.CoreFolder.Id.ObjectId.Equals(storeObjectId))
					{
						tableFlags &= ~TableFlags.SoftDeletes;
						folder = new Folder(CoreFolder.Bind(this.Session, storeObjectId), base.LogonObject);
					}
				}
				if ((byte)(tableFlags & TableFlags.SoftDeletes) == 32)
				{
					using (ContentsView contentsView = new ContentsView(logon, this.CoreFolderReference, tableFlags, notificationHandler, returnNotificationHandle))
					{
						contentsView.SetColumns(SetColumnsFlags.None, new PropertyTag[]
						{
							PropertyTag.Mid
						});
						rowCount = contentsView.GetRowCount();
						goto IL_150;
					}
				}
				if ((byte)(tableFlags & TableFlags.Associated) == 2)
				{
					rowCount = (int)folder.PropertyBag[CoreFolderSchema.AssociatedItemCount];
				}
				else
				{
					rowCount = (int)folder.PropertyBag[CoreFolderSchema.ItemCount];
				}
				IL_150:
				result = new ContentsView(logon, folder.coreFolderReference, tableFlags, notificationHandler, returnNotificationHandle);
			}
			finally
			{
				if (folder != this)
				{
					folder.Dispose();
				}
			}
			return result;
		}

		public HierarchyView CreateHierarchyView(Logon logon, TableFlags tableFlags, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle, out int rowCount)
		{
			RopHandler.CheckEnum<TableFlags>(tableFlags);
			Folder folder = this;
			if ((byte)(tableFlags & (TableFlags.RetrieveFromIndex | TableFlags.Associated)) != 0)
			{
				throw new RopExecutionException(string.Format("TableFlags {0} not supported", tableFlags & (TableFlags.RetrieveFromIndex | TableFlags.Associated)), (ErrorCode)2147746050U);
			}
			HierarchyView result;
			try
			{
				this.ClearCacheIfNeededForGetProperties();
				this.PropertyBag.Load(null);
				if ((byte)(tableFlags & TableFlags.SoftDeletes) == 32)
				{
					StoreObjectId storeObjectId = null;
					PrivateLogon privateLogon = base.LogonObject as PrivateLogon;
					if (privateLogon != null)
					{
						storeObjectId = privateLogon.MailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDeletions);
					}
					else
					{
						PublicLogon publicLogon = base.LogonObject as PublicLogon;
						if (publicLogon != null)
						{
							storeObjectId = PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId(this.CoreFolder);
						}
					}
					if (storeObjectId != null && !this.CoreFolder.Id.ObjectId.Equals(storeObjectId))
					{
						tableFlags &= ~TableFlags.SoftDeletes;
						folder = new Folder(CoreFolder.Bind(this.Session, storeObjectId), base.LogonObject);
					}
				}
				if ((byte)(tableFlags & (TableFlags.Depth | TableFlags.DeferredErrors)) == 12)
				{
					rowCount = 0;
				}
				else
				{
					if ((byte)(tableFlags & (TableFlags.Depth | TableFlags.SoftDeletes | TableFlags.SuppressNotifications)) != 0)
					{
						FolderQueryFlags folderQueryFlags = FolderQueryFlags.None;
						if ((byte)(tableFlags & TableFlags.Depth) == 4)
						{
							folderQueryFlags |= FolderQueryFlags.DeepTraversal;
						}
						if ((byte)(tableFlags & TableFlags.SuppressNotifications) == 128)
						{
							folderQueryFlags |= FolderQueryFlags.SuppressNotificationsOnMyActions;
						}
						if ((byte)(tableFlags & TableFlags.SoftDeletes) == 32)
						{
							folderQueryFlags |= FolderQueryFlags.SoftDeleted;
						}
						using (QueryResult queryResult = folder.CoreFolder.QueryExecutor.FolderQuery(folderQueryFlags, null, null, new PropertyDefinition[]
						{
							CoreFolderSchema.ItemCount
						}))
						{
							rowCount = queryResult.EstimatedRowCount;
							goto IL_169;
						}
					}
					rowCount = (int)folder.PropertyBag[CoreFolderSchema.ChildCount];
				}
				IL_169:
				result = new HierarchyView(logon, folder.CoreFolderReference, tableFlags, notificationHandler, returnNotificationHandle);
			}
			finally
			{
				if (folder != this)
				{
					folder.Dispose();
				}
			}
			return result;
		}

		public MessageStatusFlags GetMessageStatus(StoreId messageId)
		{
			StoreId storeId = new StoreId(this.Session.IdConverter.GetFidFromId(((ICoreObject)this.CoreFolder).StoreObjectId));
			object obj;
			using (CoreItem coreItem = CoreItem.Bind(this.Session, this.Session.IdConverter.CreateMessageId(storeId, messageId)))
			{
				obj = coreItem.PropertyBag.TryGetProperty(CoreItemSchema.MessageStatus);
			}
			PropertyError propertyError = obj as PropertyError;
			if (propertyError != null)
			{
				throw new RopExecutionException(string.Format("Unable to read message status property {0}", (ErrorCode)propertyError.PropertyErrorCode), (ErrorCode)propertyError.PropertyErrorCode);
			}
			return (MessageStatusFlags)((int)obj);
		}

		public RulesView CreateRulesView(Logon logon, TableFlags tableFlags, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle)
		{
			TableFlags tableFlags2 = TableFlags.NoNotifications | TableFlags.MapiUnicode | TableFlags.SuppressNotifications;
			RopHandler.CheckEnum<TableFlags>(tableFlags);
			if ((byte)(tableFlags & ~tableFlags2) != 0)
			{
				throw new RopExecutionException(string.Format("TableFlags {0} not supported", tableFlags & ~tableFlags2), (ErrorCode)2147746050U);
			}
			tableFlags &= ~TableFlags.SuppressNotifications;
			tableFlags |= TableFlags.NoNotifications;
			return new RulesView(logon, this.CoreFolderReference, tableFlags, notificationHandler, returnNotificationHandle);
		}

		public void GetSearchCriteria(GetSearchCriteriaFlags flags, out Restriction restriction, out StoreId[] folderIdArray, out SearchState searchState)
		{
			SearchFolderCriteria searchCriteria = this.CoreFolder.GetSearchCriteria(false);
			if ((byte)(flags & GetSearchCriteriaFlags.FolderIds) != 0)
			{
				List<StoreId> list = new List<StoreId>();
				foreach (StoreId id in searchCriteria.FolderScope)
				{
					StoreId item = new StoreId((ulong)this.Session.IdConverter.GetFidFromId(StoreId.GetStoreObjectId(id)));
					list.Add(item);
				}
				folderIdArray = list.ToArray();
			}
			else
			{
				folderIdArray = Array<StoreId>.Empty;
			}
			if ((byte)(flags & GetSearchCriteriaFlags.Restriction) != 0)
			{
				ModernCalendarItemFilteringHelper.RemoveModernCalendarItemRestriction(searchCriteria, base.LogonObject);
				FilterRestrictionTranslator filterRestrictionTranslator = new FilterRestrictionTranslator(this.Session);
				bool useUnicodeType = (byte)(flags & GetSearchCriteriaFlags.Unicode) != 0;
				restriction = filterRestrictionTranslator.Translate(searchCriteria.SearchQuery, useUnicodeType);
				RestrictionHelper.ConvertRestrictionToClient(this.Session, ref restriction, ViewType.MessageView);
			}
			else
			{
				restriction = null;
			}
			searchState = (SearchState)searchCriteria.SearchState;
		}

		public void SetSearchCriteria(Restriction restriction, StoreId[] folderIds, SetSearchCriteriaFlags setSearchCriteriaFlags)
		{
			QueryFilter queryFilter = null;
			StoreId[] array = null;
			RopHandler.CheckEnum<SetSearchCriteriaFlags>(setSearchCriteriaFlags);
			if ((setSearchCriteriaFlags & SetSearchCriteriaFlags.Restart) != SetSearchCriteriaFlags.None && (setSearchCriteriaFlags & SetSearchCriteriaFlags.Stop) != SetSearchCriteriaFlags.None)
			{
				throw new RopExecutionException("Invalid SetSearchCriteriaFlags combination for Stop and Restart bits", (ErrorCode)2147942487U);
			}
			if ((setSearchCriteriaFlags & SetSearchCriteriaFlags.Recursive) != SetSearchCriteriaFlags.None && (setSearchCriteriaFlags & SetSearchCriteriaFlags.Shallow) != SetSearchCriteriaFlags.None)
			{
				throw new RopExecutionException("Invalid SetSearchCriteriaFlags combination for Recursive and Shallow bits", (ErrorCode)2147942487U);
			}
			if ((setSearchCriteriaFlags & SetSearchCriteriaFlags.Foreground) != SetSearchCriteriaFlags.None && (setSearchCriteriaFlags & SetSearchCriteriaFlags.Background) != SetSearchCriteriaFlags.None)
			{
				throw new RopExecutionException("Invalid SetSearchCriteriaFlags combination for Foreground and Background bits", (ErrorCode)2147942487U);
			}
			if ((setSearchCriteriaFlags & SetSearchCriteriaFlags.Static) != SetSearchCriteriaFlags.None && (setSearchCriteriaFlags & SetSearchCriteriaFlags.NonContentIndexed) != SetSearchCriteriaFlags.None)
			{
				throw new RopExecutionException("Static Search with non-ContextIndexed is not supported", (ErrorCode)2147942487U);
			}
			if (folderIds != null && folderIds.Length > 0)
			{
				array = new StoreId[folderIds.Length];
				for (int i = 0; i < folderIds.Length; i++)
				{
					array[i] = this.Session.IdConverter.CreateFolderId(folderIds[i]);
				}
			}
			if (restriction != null)
			{
				RestrictionHelper.ConvertRestrictionFromClient(this.Session, ref restriction, ViewType.MessageView);
				queryFilter = new FilterRestrictionTranslator(this.Session).Translate(restriction);
				queryFilter = ModernCalendarItemFilteringHelper.AddModernCalendarItemsFilterForSearch(queryFilter, base.LogonObject);
			}
			SearchFolderCriteria searchFolderCriteria = new SearchFolderCriteria(queryFilter, array);
			SetSearchCriteriaFlags setSearchCriteriaFlags2 = Folder.SetSearchCriteriaFlagsToXsoSetSearchCriteriaFlags(setSearchCriteriaFlags);
			this.CoreFolder.SetSearchCriteria(searchFolderCriteria, setSearchCriteriaFlags2);
		}

		public IcsDownload CreateIcsDownload(IncrementalConfigOption configOptions, FastTransferSendOption sendOptions, SyncFlag syncFlags, Restriction restriction, SyncExtraFlag extraFlags, PropertyTag[] propertyTags, StoreId[] messageIds)
		{
			RopHandler.CheckEnum<IncrementalConfigOption>(configOptions);
			RopHandler.CheckEnum<FastTransferSendOption>(sendOptions);
			FastTransferSendOption fastTransferSendOption = FastTransferSendOption.SendPropErrors;
			if ((byte)(sendOptions & fastTransferSendOption) != 0)
			{
				throw new RopExecutionException(string.Format("SendOptions {0} not supported", sendOptions & fastTransferSendOption), (ErrorCode)2147746050U);
			}
			IcsHierarchySynchronizer.Options hierarchyDownloaderOptions = IcsHierarchySynchronizer.Options.None;
			IcsContentsSynchronizer.Options contentsDownloaderOptions = IcsContentsSynchronizer.Options.None;
			switch (configOptions)
			{
			case IncrementalConfigOption.Contents:
				if (messageIds != null && messageIds.Length > 0)
				{
					throw Feature.NotImplemented(61913, "selective sync");
				}
				ContentsSynchronizer.CheckFlags(syncFlags, extraFlags);
				if ((extraFlags & SyncExtraFlag.Eid) != SyncExtraFlag.None)
				{
					contentsDownloaderOptions |= IcsContentsSynchronizer.Options.IncludeMid;
				}
				if ((extraFlags & SyncExtraFlag.MessageSize) != SyncExtraFlag.None)
				{
					contentsDownloaderOptions |= IcsContentsSynchronizer.Options.IncludeMessageSize;
				}
				if ((extraFlags & SyncExtraFlag.Cn) != SyncExtraFlag.None)
				{
					contentsDownloaderOptions |= IcsContentsSynchronizer.Options.IncludeChangeNumber;
				}
				if ((extraFlags & SyncExtraFlag.ReadCn) != SyncExtraFlag.None)
				{
					contentsDownloaderOptions |= IcsContentsSynchronizer.Options.IncludeReadChangeNumber;
				}
				if ((ushort)(syncFlags & SyncFlag.ProgressMode) != 0)
				{
					contentsDownloaderOptions |= IcsContentsSynchronizer.Options.ProgressMode;
				}
				break;
			case IncrementalConfigOption.Hierarchy:
				if (restriction != null || (messageIds != null && messageIds.Length > 0))
				{
					throw new RopExecutionException("Hierarchy sync: restriction/selective sync", (ErrorCode)2147746050U);
				}
				HierarchySynchronizer.CheckFlags(syncFlags, extraFlags);
				if ((extraFlags & SyncExtraFlag.Eid) != SyncExtraFlag.None)
				{
					hierarchyDownloaderOptions |= IcsHierarchySynchronizer.Options.IncludeFid;
				}
				break;
			default:
				throw new RopExecutionException(string.Format("ConfigOptions {0} not supported", configOptions.ToString()), (ErrorCode)2147746050U);
			}
			return new IcsDownload(this.CoreFolderReference, configOptions, sendOptions, delegate(IcsDownload newIcsDownload)
			{
				IFastTransferProcessor<FastTransferDownloadContext> result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					IFastTransferProcessor<FastTransferDownloadContext> fastTransferProcessor;
					switch (configOptions)
					{
					case IncrementalConfigOption.Contents:
						fastTransferProcessor = disposeGuard.Add<IcsContentsSynchronizer>(new IcsContentsSynchronizer(disposeGuard.Add<ContentsSynchronizer>(new ContentsSynchronizer(newIcsDownload.FolderReference, syncFlags, restriction, extraFlags, newIcsDownload.IcsState, this.LogonObject.LogonString8Encoding, sendOptions.WantUnicode(), propertyTags)), contentsDownloaderOptions));
						break;
					case IncrementalConfigOption.Hierarchy:
						fastTransferProcessor = disposeGuard.Add<IcsHierarchySynchronizer>(new IcsHierarchySynchronizer(disposeGuard.Add<HierarchySynchronizer>(new HierarchySynchronizer(newIcsDownload.FolderReference, syncFlags, extraFlags, newIcsDownload.IcsState, propertyTags)), hierarchyDownloaderOptions));
						break;
					default:
						throw new NotSupportedException(string.Format("Unsupported configOptions. configOptions = {0}.", configOptions));
					}
					disposeGuard.Success();
					result = fastTransferProcessor;
				}
				return result;
			}, base.LogonObject);
		}

		public IcsDownloadPassThru CreateIcsDownloadPassThru(IncrementalConfigOption configOptions, FastTransferSendOption sendOptions, SyncFlag syncFlags, Restriction restriction, SyncExtraFlag extraFlags, PropertyTag[] propertyTags, StoreId[] messageIds)
		{
			RopHandler.CheckEnum<IncrementalConfigOption>(configOptions);
			RopHandler.CheckEnum<FastTransferSendOption>(sendOptions);
			switch (configOptions)
			{
			case IncrementalConfigOption.Contents:
				if (messageIds != null && messageIds.Length > 0)
				{
					throw Feature.NotImplemented(61913, "selective sync");
				}
				break;
			case IncrementalConfigOption.Hierarchy:
				if (restriction != null || (messageIds != null && messageIds.Length > 0))
				{
					throw new RopExecutionException("Hierarchy sync: restriction/selective sync", (ErrorCode)2147746050U);
				}
				break;
			default:
				throw new RopExecutionException(string.Format("ConfigOptions {0} not supported", configOptions.ToString()), (ErrorCode)2147746050U);
			}
			QueryFilter queryFilter = (restriction != null) ? new FilterRestrictionTranslator(this.CoreFolderReference.ReferencedObject.Session).Translate(restriction) : null;
			if (configOptions == IncrementalConfigOption.Contents)
			{
				queryFilter = ModernCalendarItemFilteringHelper.AddFolderFilterForIcsIfRequired(queryFilter, this.CoreFolder, base.LogonObject);
			}
			SynchronizerConfigFlags xsoConfig = IcsDownloadPassThru.GetSynchronizerConfigFlags(syncFlags, extraFlags, sendOptions);
			propertyTags = base.PropertyConverter.ConvertPropertyTagsFromClient(propertyTags);
			short[] unspecifiedTypeIDs = Folder.SeparateUnspecifiedPropertyTypes(ref propertyTags);
			PropertyDefinition[] array = (PropertyDefinition[])MEDSPropertyTranslator.GetPropertyDefinitionsIgnoreTypeChecking(this.CoreFolderReference.ReferencedObject.Session, this.CoreFolderReference.ReferencedObject.PropertyBag, propertyTags);
			PropertyDefinition[] includeProperties = null;
			PropertyDefinition[] excludeProperties = null;
			List<PropertyDefinition> list = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool isIncludeProperties = (xsoConfig & SynchronizerConfigFlags.OnlySpecifiedProps) == SynchronizerConfigFlags.OnlySpecifiedProps;
			if (isIncludeProperties)
			{
				foreach (PropertyTag a in propertyTags)
				{
					if (a == PropertyTag.EntryId)
					{
						flag = true;
					}
					else if (a == PropertyTag.ChangeNumber)
					{
						flag2 = true;
					}
					else if (a == PropertyTag.ReadChangeNumber)
					{
						flag3 = true;
					}
					else if (a == PropertyTag.MessageSize)
					{
						flag4 = true;
					}
				}
				list = new List<PropertyDefinition>(array);
			}
			else
			{
				excludeProperties = array;
				if (!excludeProperties.Contains(MessageItemSchema.MimeSkeleton))
				{
					excludeProperties = new List<PropertyDefinition>(excludeProperties)
					{
						MessageItemSchema.MimeSkeleton
					}.ToArray();
				}
			}
			if ((extraFlags & (SyncExtraFlag.Eid | SyncExtraFlag.MessageSize | SyncExtraFlag.Cn | SyncExtraFlag.ReadCn)) != SyncExtraFlag.None)
			{
				if (list == null)
				{
					list = new List<PropertyDefinition>();
				}
				if ((extraFlags & SyncExtraFlag.Cn) == SyncExtraFlag.Cn && !flag2)
				{
					list.Add(Folder.ChangeNumberPropertyDefinition);
				}
				if ((extraFlags & SyncExtraFlag.ReadCn) == SyncExtraFlag.ReadCn && !flag3)
				{
					list.Add(Folder.ReadChangeNumberPropertyDefinition);
				}
				if ((extraFlags & SyncExtraFlag.MessageSize) == SyncExtraFlag.MessageSize && !flag4)
				{
					list.Add(CoreItemSchema.Size);
				}
				if ((extraFlags & SyncExtraFlag.Eid) == SyncExtraFlag.Eid && !flag)
				{
					list.Add(CoreObjectSchema.EntryId);
				}
			}
			includeProperties = ((list != null && list.Count > 0) ? list.ToArray() : null);
			return new IcsDownloadPassThru(this.CoreFolderReference, 15840, delegate(IcsDownloadPassThru newIcsDownloadPassThru)
			{
				SessionAdaptor session = new SessionAdaptor(newIcsDownloadPassThru.FolderReference.ReferencedObject.Session);
				IPropertyBag propertyBag = new MemoryPropertyBag(session);
				newIcsDownloadPassThru.IcsState.Checkpoint(propertyBag);
				IcsStateStream icsStateStream = new IcsStateStream(propertyBag);
				SynchronizerProviderBase result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					SynchronizerProviderBase synchronizerProviderBase;
					switch (configOptions)
					{
					case IncrementalConfigOption.Contents:
						synchronizerProviderBase = disposeGuard.Add<ContentsSynchronizerProvider>(new ContentsSynchronizerProvider(newIcsDownloadPassThru.FolderReference.ReferencedObject, xsoConfig, queryFilter, icsStateStream.ToXsoState(), includeProperties, excludeProperties, isIncludeProperties ? unspecifiedTypeIDs : null, isIncludeProperties ? null : unspecifiedTypeIDs, 15840));
						break;
					case IncrementalConfigOption.Hierarchy:
						synchronizerProviderBase = disposeGuard.Add<HierarchySynchronizerProvider>(new HierarchySynchronizerProvider(newIcsDownloadPassThru.FolderReference.ReferencedObject, xsoConfig, queryFilter, icsStateStream.ToXsoState(), includeProperties, excludeProperties, isIncludeProperties ? unspecifiedTypeIDs : null, isIncludeProperties ? null : unspecifiedTypeIDs, 15840));
						break;
					default:
						throw new NotSupportedException(string.Format("Unsupported configOptions. configOptions = {0}.", configOptions));
					}
					disposeGuard.Success();
					result = synchronizerProviderBase;
				}
				return result;
			}, base.LogonObject);
		}

		public void MoveCopyMessages(Folder destinationFolder, StoreId[] messageIds, bool reportProgress, object progressToken, bool isCopy)
		{
			bool flag = false;
			if (!TeamMailboxClientOperations.IsMoveCopyMessageAllowed(this.coreFolderReference.ReferencedObject, destinationFolder.coreFolderReference.ReferencedObject, out flag))
			{
				throw new RopExecutionException("Move or Copy of shadow message is not allowed", (ErrorCode)2147746050U);
			}
			if (flag && !Configuration.ServiceConfiguration.TMPublishEnabled)
			{
				throw new RopExecutionException("Shortcut folder feature is turned off", (ErrorCode)2147746050U);
			}
			StoreObjectId[] array = this.ConvertMessageIdsToStoreObjectIds(messageIds);
			IAsyncOperationExecutor asyncOperationExecutor;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				TeamMailboxClientOperations teamMailboxClientOperations = null;
				if (flag)
				{
					teamMailboxClientOperations = TeamMailboxExecutionHelper.GetTeamMailboxClientOperations(base.LogonObject.Connection);
				}
				MoveCopyMessagesSegmentedOperation moveCopyMessagesSegmentedOperation = new MoveCopyMessagesSegmentedOperation(this.coreFolderReference, destinationFolder.coreFolderReference, isCopy, array, reportProgress ? SegmentEnumerator.MessageSegmentSize : array.Length, teamMailboxClientOperations);
				disposeGuard.Add<MoveCopyMessagesSegmentedOperation>(moveCopyMessagesSegmentedOperation);
				asyncOperationExecutor = base.LogonObject.CreateAsyncOperationExecutor(moveCopyMessagesSegmentedOperation, progressToken);
				disposeGuard.Success();
			}
			asyncOperationExecutor.BeginOperation(!reportProgress);
		}

		public void MoveCopyFolder(Folder destinationFolder, string newFolderName, bool isRecursive, StoreId folderId, bool reportProgress, bool isCopy, object progressToken)
		{
			Util.ThrowOnNullArgument(newFolderName, "newFolderName");
			StoreObjectId storeObjectId = this.Session.IdConverter.CreateFolderId(folderId);
			if (Utils.IsTeamMailbox(this.CoreFolder.Session))
			{
				using (Folder folder = Folder.Bind(this.CoreFolder.Session, storeObjectId))
				{
					if (TeamMailboxClientOperations.IsLinkedFolder(folder))
					{
						throw new RopExecutionException("Move or copy of a Shortcut folder is not allowed", (ErrorCode)2147746050U);
					}
				}
			}
			if (TeamMailboxClientOperations.IsLinkedFolder(destinationFolder.CoreFolder, false))
			{
				throw new RopExecutionException("Move or copy a folder into a Shortcut folder is not allowed", (ErrorCode)2147746050U);
			}
			IAsyncOperationExecutor asyncOperationExecutor;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				SegmentedRopOperation segmentedRopOperation;
				if (isCopy)
				{
					segmentedRopOperation = new CopyFolderSegmentedOperation(this.coreFolderReference, destinationFolder.coreFolderReference, storeObjectId, newFolderName, isRecursive);
				}
				else
				{
					segmentedRopOperation = new MoveFolderSegmentedOperation(this.coreFolderReference, destinationFolder.coreFolderReference, base.LogonObject, storeObjectId, newFolderName);
				}
				disposeGuard.Add<SegmentedRopOperation>(segmentedRopOperation);
				asyncOperationExecutor = base.LogonObject.CreateAsyncOperationExecutor(segmentedRopOperation, progressToken);
				disposeGuard.Success();
			}
			asyncOperationExecutor.BeginOperation(!reportProgress);
		}

		public void SetMessageFlags(StoreId messageId, MessageFlags flags, MessageFlags flagsMask)
		{
			RopHandler.CheckEnum<MessageFlags>(flags);
			RopHandler.CheckEnum<MessageFlags>(flagsMask);
			StoreId storeId = new StoreId(this.Session.IdConverter.GetFidFromId(((ICoreObject)this.CoreFolder).StoreObjectId));
			this.CoreFolder.SetItemFlags(this.Session.IdConverter.CreateMessageId(storeId, messageId), Folder.ConvertMessageFlagsToXsoMessageFlags(flags), Folder.ConvertMessageFlagsToXsoMessageFlags(flagsMask));
		}

		public MessageStatusFlags SetMessageStatus(StoreId messageId, MessageStatusFlags status, MessageStatusFlags statusMask)
		{
			RopHandler.CheckEnum<MessageStatusFlags>(status);
			RopHandler.CheckEnum<MessageStatusFlags>(statusMask);
			StoreId storeId = new StoreId(this.Session.IdConverter.GetFidFromId(((ICoreObject)this.CoreFolder).StoreObjectId));
			MessageStatusFlags result;
			this.CoreFolder.SetItemStatus(this.Session.IdConverter.CreateMessageId(storeId, messageId), (MessageStatusFlags)status, (MessageStatusFlags)statusMask, out result);
			return (MessageStatusFlags)result;
		}

		public void SetReadFlags(bool reportProgress, object progressToken, SetReadFlagFlags flags, StoreId[] messageIds)
		{
			if ((byte)(flags & ~(SetReadFlagFlags.SuppressReceipt | SetReadFlagFlags.FolderMessageDialog | SetReadFlagFlags.ClearReadFlag | SetReadFlagFlags.DeferredErrors | SetReadFlagFlags.GenerateReceiptOnly | SetReadFlagFlags.ClearReadNotificationPending | SetReadFlagFlags.ClearNonReadNotificationPending)) != 0)
			{
				throw new RopExecutionException(string.Format("SetReadFlagFlags {0} not supported", flags & ~(SetReadFlagFlags.SuppressReceipt | SetReadFlagFlags.FolderMessageDialog | SetReadFlagFlags.ClearReadFlag | SetReadFlagFlags.DeferredErrors | SetReadFlagFlags.GenerateReceiptOnly | SetReadFlagFlags.ClearReadNotificationPending | SetReadFlagFlags.ClearNonReadNotificationPending)), (ErrorCode)2147746050U);
			}
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				StoreObjectId[] array = this.ConvertMessageIdsToStoreObjectIds(messageIds);
				SetReadFlagsSegmentedOperation setReadFlagsSegmentedOperation = new SetReadFlagsSegmentedOperation(this.coreFolderReference, flags, (messageIds == null) ? null : array, (reportProgress || messageIds == null) ? SegmentEnumerator.MessageSegmentSize : array.Length);
				disposeGuard.Add<SetReadFlagsSegmentedOperation>(setReadFlagsSegmentedOperation);
				IAsyncOperationExecutor asyncOperationExecutor = base.LogonObject.CreateAsyncOperationExecutor(setReadFlagsSegmentedOperation, progressToken);
				disposeGuard.Success();
				asyncOperationExecutor.BeginOperation(!reportProgress);
			}
		}

		public override PropertyProblem[] SaveAndGetPropertyProblems(NativeStorePropertyDefinition[] propertyDefinitions, PropertyTag[] propertyTags)
		{
			this.CoreFolder.PropertyBag.Load(null);
			FolderSaveResult folderSaveResult = this.CoreFolder.Save(SaveMode.NoConflictResolution);
			return Folder.ConvertFolderSaveResultToProblems(folderSaveResult, propertyDefinitions, propertyTags);
		}

		internal static short[] SeparateUnspecifiedPropertyTypes(ref PropertyTag[] propertyTags)
		{
			List<PropertyTag> list = new List<PropertyTag>();
			List<short> list2 = new List<short>();
			bool flag = false;
			foreach (PropertyTag item in propertyTags)
			{
				if (item.PropertyType != PropertyType.Unspecified)
				{
					list.Add(item);
				}
				else
				{
					list2.Add((short)item.PropertyId);
					flag = true;
				}
			}
			if (flag)
			{
				propertyTags = list.ToArray();
				return list2.ToArray();
			}
			return null;
		}

		public IcsUpload OpenCollector(bool wantMessageCollector)
		{
			ReferenceCount<CoreFolder> referenceCount = ReferenceCount<CoreFolder>.Assign(CoreFolder.Bind(this.Session, this.CoreFolder.Id));
			IcsUpload result;
			try
			{
				if (wantMessageCollector)
				{
					result = new IcsMessageUpload(referenceCount, base.LogonObject);
				}
				else
				{
					result = new IcsFolderUpload(referenceCount, base.LogonObject);
				}
			}
			finally
			{
				referenceCount.Release();
			}
			return result;
		}

		public FastTransferUpload InternalFastTransferDestinationCopyFolder()
		{
			FastTransferUpload result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				this.coreFolderReference.ReferencedObject.PropertyBag.Load(CoreObjectSchema.AllPropertiesOnStore);
				FolderAdaptor folderAdaptor = FolderAdaptor.Create(this.coreFolderReference, base.LogonObject, FastTransferCopyFlag.CopyFolderPerUserData, base.LogonObject.LogonString8Encoding, true, false);
				disposeGuard.Add<FolderAdaptor>(folderAdaptor);
				IFastTransferProcessor<FastTransferUploadContext> fastTransferProcessor = FastTransferCopyFolder.CreateUploadStateMachine(folderAdaptor);
				disposeGuard.Add<IFastTransferProcessor<FastTransferUploadContext>>(fastTransferProcessor);
				FastTransferUpload fastTransferUpload = new FastTransferUpload(fastTransferProcessor, PropertyFilterFactory.IncludeAllFactory, base.LogonObject);
				disposeGuard.Success();
				result = fastTransferUpload;
			}
			return result;
		}

		public FastTransferDownload FastTransferSourceCopyFolder(FastTransferCopyFolderFlag flags, FastTransferSendOption sendOptions)
		{
			base.CheckDisposed();
			RopHandler.CheckEnum<FastTransferCopyFolderFlag>(flags);
			RopHandler.CheckEnum<FastTransferSendOption>(sendOptions);
			FastTransferCopyFlag fastTransferCopyFlag = FastTransferCopyFlag.None;
			if ((byte)(flags & FastTransferCopyFolderFlag.Move) == 1)
			{
				fastTransferCopyFlag |= FastTransferCopyFlag.CopyFolderPerUserData;
				Feature.Stubbed(137790, "We also need to check we have delete rights on the source folder by following Store behavior.");
			}
			else if ((byte)(flags & FastTransferCopyFolderFlag.CopySubFolders) == 16)
			{
				fastTransferCopyFlag |= FastTransferCopyFlag.CopyFolderPerUserData;
			}
			ReferenceCount<CoreFolder> referenceCount = ReferenceCount<CoreFolder>.Assign(CoreFolder.Bind(this.Session, this.CoreFolder.Id, CoreObjectSchema.AllPropertiesOnStore));
			FastTransferDownload result;
			try
			{
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					FolderAdaptor folderAdaptor = FolderAdaptor.Create(referenceCount, base.LogonObject, fastTransferCopyFlag, base.LogonObject.LogonString8Encoding, sendOptions.WantUnicode(), sendOptions.IsUpload());
					disposeGuard.Add<FolderAdaptor>(folderAdaptor);
					FastTransferFolderContentBase.IncludeSubObject includeSubObject = FastTransferFolderContentBase.IncludeSubObject.Messages | FastTransferFolderContentBase.IncludeSubObject.AssociatedMessages;
					bool flag = (fastTransferCopyFlag & FastTransferCopyFlag.CopyFolderPerUserData) != FastTransferCopyFlag.CopyFolderPerUserData;
					if (!flag)
					{
						includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.Subfolders;
					}
					IFastTransferProcessor<FastTransferDownloadContext> fastTransferProcessor = FastTransferCopyFolder.CreateDownloadStateMachine(folderAdaptor, includeSubObject);
					disposeGuard.Add<IFastTransferProcessor<FastTransferDownloadContext>>(fastTransferProcessor);
					uint steps = Folder.CalculateSteps(referenceCount.ReferencedObject, includeSubObject, base.LogonObject is PrivateLogon);
					FastTransferDownload fastTransferDownload = new FastTransferDownload(sendOptions, fastTransferProcessor, steps, new PropertyFilterFactory(flag, false, Array<PropertyTag>.Empty), base.LogonObject);
					disposeGuard.Success();
					result = fastTransferDownload;
				}
			}
			finally
			{
				referenceCount.Release();
			}
			return result;
		}

		public FastTransferDownload FastTransferSourceCopyMessages(StoreId[] messageIds, FastTransferCopyMessagesFlag flags, FastTransferSendOption sendOptions)
		{
			flags &= ~RopHandlerHelper.FastTransferCopyMessagesClientOnlyFlags;
			RopHandler.CheckEnum<FastTransferCopyMessagesFlag>(flags);
			RopHandler.CheckEnum<FastTransferSendOption>(sendOptions);
			Util.ThrowOnNullArgument(messageIds, "messageIds");
			FastTransferDownload result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MessageIterator messageIterator = new MessageIterator(base.LogonObject, new StoreId(base.LogonObject.Session.IdConverter.GetFidFromId(this.CoreFolder.Id.ObjectId)), messageIds, flags, sendOptions);
				disposeGuard.Add<MessageIterator>(messageIterator);
				FastTransferMessageIterator fastTransferMessageIterator = new FastTransferMessageIterator(messageIterator, flags, true);
				disposeGuard.Add<FastTransferMessageIterator>(fastTransferMessageIterator);
				FastTransferDownload fastTransferDownload = new FastTransferDownload(sendOptions, fastTransferMessageIterator, (uint)messageIds.Length, PropertyFilterFactory.IncludeAllFactory, messageIterator.LogonObject);
				disposeGuard.Add<FastTransferDownload>(fastTransferDownload);
				disposeGuard.Success();
				result = fastTransferDownload;
			}
			return result;
		}

		public FastTransferUpload InternalFastTransferDestinationCopyMessages()
		{
			FastTransferUpload result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MessageIteratorClient messageIteratorClient = new MessageIteratorClient(this, base.LogonObject);
				disposeGuard.Add<MessageIteratorClient>(messageIteratorClient);
				FastTransferMessageIterator fastTransferMessageIterator = new FastTransferMessageIterator(messageIteratorClient, true);
				disposeGuard.Add<FastTransferMessageIterator>(fastTransferMessageIterator);
				disposeGuard.Success();
				result = new FastTransferUpload(fastTransferMessageIterator, PropertyFilterFactory.IncludeAllFactory, base.LogonObject);
			}
			return result;
		}

		public void ModifyPermissions(ModifyPermissionsFlags modifyPermissionsFlags, ModifyTableRow[] permissions)
		{
			RopHandler.CheckEnum<ModifyPermissionsFlags>(modifyPermissionsFlags);
			Util.ThrowOnNullArgument(permissions, "permissions");
			if (Utils.IsTeamMailbox(this.CoreFolder.Session))
			{
				throw new RopExecutionException("Cannot modify team mailbox folder permission.", (ErrorCode)2147746050U);
			}
			foreach (ModifyTableRow modifyTableRow in permissions)
			{
				if (modifyTableRow.PropertyValues.Length == 0)
				{
					throw new RopExecutionException("Encountered a ModifyTableRow without property values.", (ErrorCode)2147942487U);
				}
			}
			ModifyTableOptions modifyTableOptions = ModifyTableOptions.None;
			if ((byte)(modifyPermissionsFlags & ModifyPermissionsFlags.IncludeFreeBusy) == 2)
			{
				modifyTableOptions |= ModifyTableOptions.FreeBusyAware;
			}
			bool replaceRows = (byte)(modifyPermissionsFlags & ModifyPermissionsFlags.ReplaceRows) == 1;
			string valueOrDefault = this.CoreFolder.PropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ContainerClass);
			StoreObjectType objectType = ObjectClass.GetObjectType(valueOrDefault);
			bool flag = !PolicyAllowedMemberRights.IsAllowedOnFolder(objectType);
			IEnumerable<ModifyTableRow> modifyTableRows = permissions;
			if (flag)
			{
				modifyTableRows = from row in permissions
				where !Folder.IsExternalUsersRow(row)
				select row;
			}
			using (IModifyTable permissionTable = this.CoreFolder.GetPermissionTable(modifyTableOptions))
			{
				PublicLogon publicLogon = base.LogonObject as PublicLogon;
				if (publicLogon != null && !publicLogon.IsPrimaryHierarchyLogon)
				{
					PublicFolderOperations.ModifyPermissions(publicLogon, this.CoreFolder, permissionTable, modifyTableRows, modifyTableOptions, replaceRows);
				}
				else
				{
					this.ApplyTableChanges(permissionTable, modifyTableRows, replaceRows);
				}
			}
		}

		public void ModifyRules(ModifyRulesFlags modifyRulesFlags, ModifyTableRow[] rulesData)
		{
			RopHandler.CheckEnum<ModifyRulesFlags>(modifyRulesFlags);
			Util.ThrowOnNullArgument(rulesData, "rulesData");
			bool replaceRows = modifyRulesFlags == ModifyRulesFlags.ReplaceRows;
			this.ClearCacheIfNeededForGetProperties();
			using (IModifyTable ruleTable = this.CoreFolder.GetRuleTable())
			{
				this.ApplyTableChanges(ruleTable, rulesData, replaceRows);
			}
		}

		public override PropertyProblem[] SetProperties(PropertyValue[] propertyValues, bool trackChanges)
		{
			PropertyProblem[] result;
			try
			{
				if (!trackChanges)
				{
					this.CoreFolder.SaveFlags |= PropertyBagSaveFlags.NoChangeTracking;
				}
				PublicLogon publicLogon = base.LogonObject as PublicLogon;
				if (publicLogon != null && !publicLogon.IsPrimaryHierarchyLogon)
				{
					result = PublicFolderOperations.SetProperties(publicLogon, this.CoreFolder.Id, propertyValues, trackChanges);
				}
				else
				{
					result = base.InternalSetProperties(propertyValues);
				}
			}
			finally
			{
				this.CoreFolder.SaveFlags &= ~PropertyBagSaveFlags.NoChangeTracking;
			}
			return result;
		}

		public override PropertyProblem[] DeleteProperties(PropertyTag[] propertyTags, bool trackChanges)
		{
			PropertyProblem[] result;
			try
			{
				if (!trackChanges)
				{
					this.CoreFolder.SaveFlags |= PropertyBagSaveFlags.NoChangeTracking;
				}
				PublicLogon publicLogon = base.LogonObject as PublicLogon;
				if (publicLogon != null && !publicLogon.IsPrimaryHierarchyLogon)
				{
					result = PublicFolderOperations.DeleteProperties(publicLogon, this.CoreFolder.Id, propertyTags, trackChanges);
				}
				else
				{
					result = base.InternalDeleteProperties(propertyTags);
				}
			}
			finally
			{
				this.CoreFolder.SaveFlags &= ~PropertyBagSaveFlags.NoChangeTracking;
			}
			return result;
		}

		public override void ClearCacheIfNeededForGetProperties()
		{
			this.PropertyBag.Clear();
		}

		internal static PropertyProblem[] ConvertFolderSaveResultToProblems(FolderSaveResult folderSaveResult, NativeStorePropertyDefinition[] propertyDefinitions, PropertyTag[] propertyTags)
		{
			if (folderSaveResult.PropertyErrors.Length > 0)
			{
				List<PropertyProblem> list = new List<PropertyProblem>(propertyTags.Length);
				foreach (PropertyError propertyError in folderSaveResult.PropertyErrors)
				{
					int num = Array.IndexOf<PropertyDefinition>(propertyDefinitions, propertyError.PropertyDefinition);
					if (num != -1)
					{
						list.Add(new PropertyProblem((ushort)num, propertyTags[num], MEDSPropertyTranslator.PropertyErrorCodeToErrorCode(propertyError.PropertyErrorCode)));
					}
				}
				return list.ToArray();
			}
			return Array<PropertyProblem>.Empty;
		}

		internal static void CheckDisplayNameValid(string displayName, ErrorCode errorCodeIfInvalid)
		{
			if (string.IsNullOrEmpty(displayName))
			{
				throw new RopExecutionException("displayName cannot be Null or Empty.", errorCodeIfInvalid);
			}
		}

		internal static PropValue[] ConvertPropertyValueToXSOPropValue(CoreFolder coreFolder, PropertyValue[] values)
		{
			Util.ThrowOnNullArgument(coreFolder, "coreFolder");
			Util.ThrowOnNullArgument(values, "values");
			PropValue[] array = new PropValue[values.Length];
			int num = 0;
			foreach (PropertyValue propertyValue in values)
			{
				StorePropertyDefinition propDef = MEDSPropertyTranslator.PropertyDefinitionFromPropertyTag(coreFolder, propertyValue.PropertyTag);
				object value = MEDSPropertyTranslator.TranslatePropertyValue(coreFolder.Session, propertyValue);
				array[num++] = new PropValue(propDef, value);
			}
			return array;
		}

		internal void SetLocalReplicaMidsetDeleted(LongTermIdRange[] longTermIdRanges)
		{
			Util.ThrowOnNullArgument(longTermIdRanges, "longTermIdRanges");
			if (longTermIdRanges.Length != 0)
			{
				if (!longTermIdRanges.Any((LongTermIdRange range) => !range.IsValid()))
				{
					try
					{
						byte[] array = new byte[48 * longTermIdRanges.Length + 4];
						using (Writer writer = new BufferWriter(array))
						{
							writer.WriteUInt32((uint)longTermIdRanges.Length);
							foreach (LongTermIdRange longTermIdRange in longTermIdRanges)
							{
								longTermIdRange.Serialize(writer);
							}
							this.PropertyBag[CoreFolderSchema.MergeMidsetDeleted] = array;
							this.CoreFolder.Save(SaveMode.NoConflictResolution);
						}
					}
					catch (ObjectNotFoundException)
					{
					}
					return;
				}
			}
			throw new RopExecutionException(string.Format("longTermIdRanges are invalid: count = {0}, first invalid range = {1}", longTermIdRanges.Length, longTermIdRanges.FirstOrDefault((LongTermIdRange range) => !range.IsValid())), (ErrorCode)2147942487U);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<Folder>(this);
		}

		protected override void InternalDispose()
		{
			this.coreFolderReference.Release();
			base.InternalDispose();
		}

		protected override FastTransferUpload InternalFastTransferDestinationCopyTo()
		{
			this.CoreFolder.PropertyBag.Load(CoreObjectSchema.AllPropertiesOnStore);
			FastTransferUpload result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IFolder folder = FolderAdaptor.Create(this.CoreFolderReference, base.LogonObject, FastTransferCopyFlag.None, base.LogonObject.LogonString8Encoding, true, false);
				disposeGuard.Add<IFolder>(folder);
				IFastTransferProcessor<FastTransferUploadContext> fastTransferProcessor = FastTransferFolderCopyTo.CreateUploadStateMachine(folder);
				disposeGuard.Add<IFastTransferProcessor<FastTransferUploadContext>>(fastTransferProcessor);
				FastTransferUpload fastTransferUpload = new FastTransferUpload(fastTransferProcessor, PropertyFilterFactory.IncludeAllFactory, base.LogonObject);
				disposeGuard.Add<FastTransferUpload>(fastTransferUpload);
				disposeGuard.Success();
				result = fastTransferUpload;
			}
			return result;
		}

		protected override FastTransferUpload InternalFastTransferDestinationCopyProperties()
		{
			return this.InternalFastTransferDestinationCopyTo();
		}

		protected override FastTransferDownload InternalFastTransferSourceCopyProperties(bool isShallowCopy, FastTransferCopyPropertiesFlag flags, FastTransferSendOption sendOptions, PropertyTag[] includedProperties)
		{
			FastTransferFolderContentBase.IncludeSubObject includeSubObject = FastTransferFolderContentBase.IncludeSubObject.None;
			if (!isShallowCopy)
			{
				if (includedProperties.Contains(PropertyTag.ContainerContents))
				{
					includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.Messages;
				}
				if (includedProperties.Contains(PropertyTag.FolderAssociatedContents))
				{
					includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.AssociatedMessages;
				}
				if (includedProperties.Contains(PropertyTag.ContainerHierarchy))
				{
					includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.Subfolders;
				}
			}
			if (flags != FastTransferCopyPropertiesFlag.None)
			{
				Feature.Stubbed(185369, "Support FastTransferCopyPropertiesFlag.Move, which is the only flag in FastTransferCopyPropertiesFlag.");
			}
			return this.InternalFastTransferSourceCopyOperation(isShallowCopy, includeSubObject, FastTransferCopyFlag.None, sendOptions, true, includedProperties);
		}

		protected override FastTransferDownload InternalFastTransferSourceCopyTo(bool isShallowCopy, FastTransferCopyFlag flags, FastTransferSendOption sendOptions, PropertyTag[] excludedPropertyTags)
		{
			FastTransferFolderContentBase.IncludeSubObject includeSubObject = FastTransferFolderContentBase.IncludeSubObject.None;
			if (!isShallowCopy)
			{
				if (!excludedPropertyTags.Contains(PropertyTag.ContainerContents))
				{
					includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.Messages;
				}
				if (!excludedPropertyTags.Contains(PropertyTag.FolderAssociatedContents))
				{
					includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.AssociatedMessages;
				}
				if (!excludedPropertyTags.Contains(PropertyTag.ContainerHierarchy))
				{
					includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.Subfolders;
				}
			}
			return this.InternalFastTransferSourceCopyOperation(isShallowCopy, includeSubObject, flags, sendOptions, false, excludedPropertyTags);
		}

		protected override PropertyError[] InternalCopyTo(PropertyServerObject destinationPropertyServerObject, CopySubObjects copySubObjects, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] excludeProperties)
		{
			Folder folder = RopHandler.Downcast<Folder>(destinationPropertyServerObject);
			if (this.Session is PublicFolderSession || folder.Session is PublicFolderSession)
			{
				throw new RopExecutionException("CopyTo is not supported on public folders.", (ErrorCode)2147746050U);
			}
			return this.CoreFolder.CopyFolder(folder.CoreFolder, copyPropertiesFlags, copySubObjects, excludeProperties);
		}

		protected override PropertyError[] InternalCopyProperties(PropertyServerObject destinationPropertyServerObject, CopyPropertiesFlags copyPropertiesFlags, NativeStorePropertyDefinition[] properties)
		{
			Folder folder = RopHandler.Downcast<Folder>(destinationPropertyServerObject);
			if (this.Session is PublicFolderSession ^ folder.Session is PublicFolderSession)
			{
				throw new RopExecutionException("CopyProperties is not supported between public folders and private folders.", (ErrorCode)2147746050U);
			}
			return this.CoreFolder.CopyProperties(folder.CoreFolder, copyPropertiesFlags, properties);
		}

		protected override bool ShouldSkipPropertyChange(StorePropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == CoreFolderSchema.AclTableAndSecurityDescriptor)
			{
				return true;
			}
			bool isLinked = TeamMailboxClientOperations.IsLinkedFolder(this.CoreFolder, false);
			return FolderPropertyRestriction.Instance.ShouldBlock(propertyDefinition, isLinked);
		}

		protected override void OnBeforeOpenStream(StorePropertyDefinition propertyDefinition, OpenMode openMode)
		{
			if (this.Session is PublicFolderSession && openMode != OpenMode.ReadOnly)
			{
				throw new RopExecutionException("Streamable write operations are not supported on public folders.", (ErrorCode)2147746050U);
			}
			if (propertyDefinition == CoreFolderSchema.AclTableAndSecurityDescriptor && openMode != OpenMode.ReadOnly)
			{
				throw new RopExecutionException("AclTableAndSecurityDescriptor modification is not supported.", (ErrorCode)2147942405U);
			}
		}

		protected override StreamSource GetStreamSource()
		{
			return new StreamSource<CoreFolder>(this.coreFolderReference, (CoreFolder coreFolder) => coreFolder.PropertyBag);
		}

		private static bool IsLocalDirectoryUserEntryId(byte[] entryId)
		{
			if (entryId == null)
			{
				return false;
			}
			if (entryId.Length < 20)
			{
				return false;
			}
			bool result;
			using (BufferReader bufferReader = Reader.CreateBufferReader(entryId))
			{
				bufferReader.ReadUInt32();
				Guid g = bufferReader.ReadGuid();
				result = Folder.LocalDirectoryGuid.Equals(g);
			}
			return result;
		}

		private static bool IsExternalUsersRow(ModifyTableRow row)
		{
			if (row.ModifyTableFlags != ModifyTableFlags.AddRow)
			{
				return false;
			}
			object obj = (from propertyValue in row.PropertyValues
			where propertyValue.PropertyTag == PropertyTag.EntryId
			select propertyValue.Value).FirstOrDefault<object>();
			return Folder.IsLocalDirectoryUserEntryId(obj as byte[]);
		}

		private static void ConvertXSOOperationResultToPartialSucceeded(OperationResult operationResult, Exception exception, out bool isPartiallyCompleted)
		{
			switch (operationResult)
			{
			case OperationResult.Succeeded:
				isPartiallyCompleted = false;
				return;
			case OperationResult.Failed:
				throw new RopExecutionException(string.Format("Failed operation. Result = {0}", operationResult), (exception != null) ? ExceptionTranslator.ErrorFromXsoException(exception) : ((ErrorCode)2147500037U));
			case OperationResult.PartiallySucceeded:
				isPartiallyCompleted = true;
				return;
			default:
			{
				string message = string.Format("The contract between RpcClientAccess and XSO was violated. OperationResult = {0}", operationResult);
				throw new NotSupportedException(message);
			}
			}
		}

		private static uint CalculateSteps(CoreFolder coreFolder, FastTransferFolderContentBase.IncludeSubObject includeSubObject, bool isPrivateLogon)
		{
			uint num = 1U;
			if (isPrivateLogon)
			{
				if ((includeSubObject & FastTransferFolderContentBase.IncludeSubObject.Messages) == FastTransferFolderContentBase.IncludeSubObject.Messages)
				{
					num += (uint)coreFolder.PropertyBag.GetValueOrDefault<int>(CoreFolderSchema.ItemCount);
				}
				if ((includeSubObject & FastTransferFolderContentBase.IncludeSubObject.AssociatedMessages) == FastTransferFolderContentBase.IncludeSubObject.AssociatedMessages)
				{
					num += (uint)coreFolder.PropertyBag.GetValueOrDefault<int>(CoreFolderSchema.AssociatedItemCount);
				}
				if ((includeSubObject & FastTransferFolderContentBase.IncludeSubObject.Subfolders) == FastTransferFolderContentBase.IncludeSubObject.Subfolders)
				{
					Folder.AddStepsOfPrivateSubfolders(coreFolder, ref num);
				}
			}
			else
			{
				Feature.Stubbed(209299, "We do not currently support calculating the total number of steps for progress when copying subfolders under a public folder.");
			}
			return num;
		}

		private static void AddStepsOfPrivateSubfolders(CoreFolder coreFolder, ref uint steps)
		{
			using (QueryResult queryResult = coreFolder.QueryExecutor.FolderQuery(FolderQueryFlags.DeepTraversal | FolderQueryFlags.SuppressNotificationsOnMyActions, null, null, Folder.PropertiesToCalculateStepsOnPrivateSubfolders))
			{
				object[][] rows;
				do
				{
					rows = queryResult.GetRows(int.MaxValue);
					foreach (object[] array2 in rows)
					{
						steps += (uint)(1 + Microsoft.Exchange.Data.Storage.PropertyBag.CheckPropertyValue<int>(CoreFolderSchema.ItemCount, array2[0], 0) + Microsoft.Exchange.Data.Storage.PropertyBag.CheckPropertyValue<int>(CoreFolderSchema.AssociatedItemCount, array2[1], 0));
					}
				}
				while (rows.Length > 0);
			}
		}

		private static SetSearchCriteriaFlags SetSearchCriteriaFlagsToXsoSetSearchCriteriaFlags(SetSearchCriteriaFlags originalSetSearchCriteriaFlags)
		{
			SetSearchCriteriaFlags setSearchCriteriaFlags = originalSetSearchCriteriaFlags;
			SetSearchCriteriaFlags setSearchCriteriaFlags2 = SetSearchCriteriaFlags.None;
			for (int i = 0; i < Folder.SetSearchCriteriaFlagsMap.Length; i++)
			{
				KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags> keyValuePair = Folder.SetSearchCriteriaFlagsMap[i];
				if ((setSearchCriteriaFlags & keyValuePair.Key) == keyValuePair.Key)
				{
					setSearchCriteriaFlags2 |= keyValuePair.Value;
					setSearchCriteriaFlags &= ~keyValuePair.Key;
				}
			}
			if (setSearchCriteriaFlags != SetSearchCriteriaFlags.None)
			{
				throw new RopExecutionException(string.Format("Invalid {0} value: {1}", typeof(SetSearchCriteriaFlags), originalSetSearchCriteriaFlags), (ErrorCode)2147942487U);
			}
			return setSearchCriteriaFlags2;
		}

		private static MessageFlags ConvertMessageFlagsToXsoMessageFlags(MessageFlags messageFlags)
		{
			MessageFlags messageFlags2 = MessageFlags.None;
			if ((messageFlags & ~(MessageFlags.Read | MessageFlags.Unmodified | MessageFlags.Submit | MessageFlags.Unsent | MessageFlags.FromMe | MessageFlags.Associated | MessageFlags.Resend | MessageFlags.RnPending | MessageFlags.NrnPending | MessageFlags.Restricted)) != MessageFlags.None)
			{
				throw new RopExecutionException(string.Format("Unsupported MessageFlags value: {0}", messageFlags & ~(MessageFlags.Read | MessageFlags.Unmodified | MessageFlags.Submit | MessageFlags.Unsent | MessageFlags.FromMe | MessageFlags.Associated | MessageFlags.Resend | MessageFlags.RnPending | MessageFlags.NrnPending | MessageFlags.Restricted)), (ErrorCode)2147942487U);
			}
			if ((messageFlags & MessageFlags.Read) == MessageFlags.Read)
			{
				messageFlags2 |= MessageFlags.IsRead;
			}
			if ((messageFlags & MessageFlags.Unmodified) == MessageFlags.Unmodified)
			{
				messageFlags2 |= MessageFlags.IsUnmodified;
			}
			if ((messageFlags & MessageFlags.Submit) == MessageFlags.Submit)
			{
				messageFlags2 |= MessageFlags.HasBeenSubmitted;
			}
			if ((messageFlags & MessageFlags.Unsent) == MessageFlags.Unsent)
			{
				messageFlags2 |= MessageFlags.IsDraft;
			}
			if ((messageFlags & MessageFlags.FromMe) == MessageFlags.FromMe)
			{
				messageFlags2 |= MessageFlags.IsFromMe;
			}
			if ((messageFlags & MessageFlags.Associated) == MessageFlags.Associated)
			{
				messageFlags2 |= MessageFlags.IsAssociated;
			}
			if ((messageFlags & MessageFlags.Resend) == MessageFlags.Resend)
			{
				messageFlags2 |= MessageFlags.IsResend;
			}
			if ((messageFlags & MessageFlags.RnPending) == MessageFlags.RnPending)
			{
				messageFlags2 |= MessageFlags.IsReadReceiptPending;
			}
			if ((messageFlags & MessageFlags.NrnPending) == MessageFlags.NrnPending)
			{
				messageFlags2 |= MessageFlags.IsNotReadReceiptPending;
			}
			if ((messageFlags & MessageFlags.Restricted) == MessageFlags.Restricted)
			{
				messageFlags2 |= MessageFlags.IsRestricted;
			}
			return messageFlags2;
		}

		private StoreObjectId[] ConvertMessageIdsToStoreObjectIds(StoreId[] messageIds)
		{
			if (messageIds != null && messageIds.Length > 0)
			{
				StoreId folderId = new StoreId(this.Session.IdConverter.GetFidFromId(((ICoreObject)this.CoreFolder).StoreObjectId));
				return (from id in messageIds
				where id != StoreId.Empty
				select StoreId.GetStoreObjectId(this.Session.IdConverter.CreateMessageId(folderId, id))).ToArray<StoreObjectId>();
			}
			return Array<StoreObjectId>.Empty;
		}

		private void ApplyTableChanges(IModifyTable modifyTable, IEnumerable<ModifyTableRow> modifyTableRows, bool replaceRows)
		{
			if (replaceRows)
			{
				modifyTable.Clear();
			}
			foreach (ModifyTableRow modifyTableRow in modifyTableRows)
			{
				PropValue[] propValues = Folder.ConvertPropertyValueToXSOPropValue(this.CoreFolder, modifyTableRow.PropertyValues);
				switch (modifyTableRow.ModifyTableFlags)
				{
				case ModifyTableFlags.AddRow:
					modifyTable.AddRow(propValues);
					continue;
				case ModifyTableFlags.ModifyRow:
					modifyTable.ModifyRow(propValues);
					continue;
				case ModifyTableFlags.RemoveRow:
					modifyTable.RemoveRow(propValues);
					continue;
				}
				throw new RopExecutionException(string.Format("ModifyTableFlags is not valid. ModifyTableFlags = {0}.", modifyTableRow.ModifyTableFlags), (ErrorCode)2147942487U);
			}
			modifyTable.ApplyPendingChanges();
		}

		private FastTransferDownload InternalFastTransferSourceCopyOperation(bool isShallowCopy, FastTransferFolderContentBase.IncludeSubObject includeSubObject, FastTransferCopyFlag fastTransferCopyFlag, FastTransferSendOption sendOptions, bool isInclusion, PropertyTag[] propertyTags)
		{
			FastTransferDownload result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				ReferenceCount<CoreFolder> referenceCount = ReferenceCount<CoreFolder>.Assign(CoreFolder.Bind(this.Session, this.CoreFolder.Id, CoreObjectSchema.AllPropertiesOnStore));
				try
				{
					FolderAdaptor folderAdaptor = FolderAdaptor.Create(referenceCount, base.LogonObject, fastTransferCopyFlag, base.LogonObject.LogonString8Encoding, sendOptions.WantUnicode(), sendOptions.IsUpload());
					disposeGuard.Add<FolderAdaptor>(folderAdaptor);
					IFastTransferProcessor<FastTransferDownloadContext> fastTransferProcessor = FastTransferFolderCopyTo.CreateDownloadStateMachine(folderAdaptor, includeSubObject);
					disposeGuard.Add<IFastTransferProcessor<FastTransferDownloadContext>>(fastTransferProcessor);
					uint steps = Folder.CalculateSteps(referenceCount.ReferencedObject, includeSubObject, base.LogonObject is PrivateLogon);
					FastTransferDownload fastTransferDownload = new FastTransferDownload(sendOptions, fastTransferProcessor, steps, new PropertyFilterFactory(isShallowCopy, isInclusion, propertyTags), base.LogonObject);
					disposeGuard.Add<FastTransferDownload>(fastTransferDownload);
					disposeGuard.Success();
					result = fastTransferDownload;
				}
				finally
				{
					referenceCount.Release();
				}
			}
			return result;
		}

		internal const int MaxFastTransferBlockSize = 15840;

		private static readonly PropertyDefinition ChangeNumberPropertyDefinition = PropertyTagPropertyDefinition.CreateCustom("ChangeNumber", PropertyTag.ChangeNumber);

		private static readonly PropertyDefinition ReadChangeNumberPropertyDefinition = PropertyTagPropertyDefinition.CreateCustom("ReadChangeNumber", PropertyTag.ReadChangeNumber);

		private static readonly PropertyDefinition[] PropertiesToCalculateStepsOnPrivateSubfolders = new PropertyDefinition[]
		{
			CoreFolderSchema.ItemCount,
			CoreFolderSchema.AssociatedItemCount
		};

		private static readonly KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>[] SetSearchCriteriaFlagsMap = new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>[]
		{
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.None, SetSearchCriteriaFlags.None),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.Stop, SetSearchCriteriaFlags.Stop),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.Restart, SetSearchCriteriaFlags.Restart),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.Recursive, SetSearchCriteriaFlags.Recursive),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.Shallow, SetSearchCriteriaFlags.Shallow),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.Foreground, SetSearchCriteriaFlags.Foreground),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.Background, SetSearchCriteriaFlags.Background),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.UseCIForComplexQueries, SetSearchCriteriaFlags.UseCiForComplexQueries),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.ContentIndexed, SetSearchCriteriaFlags.ContentIndexed),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.NonContentIndexed, SetSearchCriteriaFlags.NonContentIndexed),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.Static, SetSearchCriteriaFlags.Static),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.FailOnForeignEID, SetSearchCriteriaFlags.FailOnForeignEID),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.StatisticsOnly, SetSearchCriteriaFlags.StatisticsOnly),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.FailNonContentIndexedSearch, SetSearchCriteriaFlags.FailNonContentIndexedSearch),
			new KeyValuePair<SetSearchCriteriaFlags, SetSearchCriteriaFlags>(SetSearchCriteriaFlags.EstimateCountOnly, SetSearchCriteriaFlags.EstimateCountOnly)
		};

		private static readonly Guid LocalDirectoryGuid = new Guid(new byte[]
		{
			212,
			186,
			25,
			39,
			241,
			181,
			79,
			27,
			184,
			59,
			20,
			115,
			118,
			55,
			226,
			105
		});

		private readonly ReferenceCount<CoreFolder> coreFolderReference;

		private readonly CoreObjectPropertyDefinitionFactory propertyDefinitionFactory;

		private readonly CoreObjectProperties storageObjectProperties;
	}
}
