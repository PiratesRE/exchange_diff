using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Folder : StoreObject, IFolder, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal Folder(CoreFolder coreFolder) : base(coreFolder, false)
		{
		}

		protected static bool ExtendedFlagsContains(int? extendedFolderFlags, ExtendedFolderFlags flag)
		{
			return extendedFolderFlags != null && (flag & (ExtendedFolderFlags)extendedFolderFlags.Value) != (ExtendedFolderFlags)0;
		}

		public static Folder Create(StoreSession session, StoreId parentFolderId, StoreObjectType folderType)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			EnumValidator.ThrowIfInvalid<StoreObjectType>(folderType, Folder.FolderTypes);
			ExTraceGlobals.StorageTracer.Information(0L, "Folder::Create.");
			return Folder.InternalCreateFolder(session, parentFolderId, folderType, null, CreateMode.CreateNew, false);
		}

		public static Folder Create(StoreSession session, StoreId parentFolderId, StoreObjectType foldertype, string displayName, CreateMode createMode)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			EnumValidator.ThrowIfInvalid<StoreObjectType>(foldertype, Folder.FolderTypes);
			Util.ThrowOnNullArgument(displayName, "displayName");
			EnumValidator.ThrowIfInvalid<CreateMode>(createMode, "createMode");
			ExTraceGlobals.StorageTracer.Information(0L, "Folder::Create.");
			return Folder.InternalCreateFolder(session, parentFolderId, foldertype, displayName, createMode, false);
		}

		public static Folder CreateSecure(StoreSession session, StoreId parentFolderId, StoreObjectType folderType)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			EnumValidator.ThrowIfInvalid<StoreObjectType>(folderType, Folder.FolderTypes);
			ExTraceGlobals.StorageTracer.Information(0L, "Folder::CreateSecure.");
			return Folder.InternalCreateFolder(session, parentFolderId, folderType, null, CreateMode.CreateNew, true);
		}

		public static Folder CreateSecure(StoreSession session, StoreId parentFolderId, StoreObjectType foldertype, string displayName, CreateMode createMode)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			EnumValidator.ThrowIfInvalid<StoreObjectType>(foldertype, Folder.FolderTypes);
			Util.ThrowOnNullArgument(displayName, "displayName");
			EnumValidator.ThrowIfInvalid<CreateMode>(createMode, "createMode");
			ExTraceGlobals.StorageTracer.Information(0L, "Folder::CreateSecure.");
			return Folder.InternalCreateFolder(session, parentFolderId, foldertype, displayName, createMode, true);
		}

		public static Folder Bind(StoreSession session, StoreId folderId)
		{
			return Folder.Bind(session, folderId, null);
		}

		public static Folder Bind(StoreSession session, StoreId folderId, params PropertyDefinition[] propsToReturn)
		{
			return Folder.Bind(session, folderId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public static Folder Bind(StoreSession session, StoreId folderId, ICollection<PropertyDefinition> propsToReturn)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(folderId, "folderId");
			return Folder.InternalBind<Folder>(session, folderId, propsToReturn);
		}

		public static Folder Bind(StoreSession session, DefaultFolderType defaultFolderType)
		{
			return Folder.Bind(session, defaultFolderType, null);
		}

		public static Folder Bind(StoreSession session, DefaultFolderType defaultFolderType, params PropertyDefinition[] propsToReturn)
		{
			return Folder.Bind(session, defaultFolderType, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public static Folder Bind(StoreSession session, DefaultFolderType defaultFolderType, ICollection<PropertyDefinition> propsToReturn)
		{
			ObjectNotFoundException ex = null;
			StoreObjectId folderId = session.SafeGetDefaultFolderId(defaultFolderType);
			for (int i = 0; i < 2; i++)
			{
				try
				{
					return Folder.Bind(session, folderId, propsToReturn);
				}
				catch (ObjectNotFoundException ex2)
				{
					ex = ex2;
					ExTraceGlobals.StorageTracer.Information<DefaultFolderType>(0L, "Folder::Bind(defaultFolderType): attempting to recreate {0}.", defaultFolderType);
					if (!session.TryFixDefaultFolderId(defaultFolderType, out folderId))
					{
						throw;
					}
				}
			}
			throw ex;
		}

		public static bool IsFolderId(StoreId id)
		{
			if (id == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "StoreSession::IsFolderId. The containerId cannot be null. Argument = {0}.", "id");
				throw new ArgumentNullException(ServerStrings.ExNullParameter("id", 1));
			}
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(id);
			return storeObjectId.ObjectType == StoreObjectType.Folder || storeObjectId.ProviderLevelItemId == null || IdConverter.IsFolderId(storeObjectId);
		}

		public static QueryFilter GetClutterBasedViewFilter(bool isClutter)
		{
			if (!isClutter)
			{
				return Folder.DefaultNoClutterFilter;
			}
			return Folder.DefaultClutterFilter;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<Folder>(this);
		}

		private static bool IsFlagsValid(ExtendedFolderFlags? flags, bool hasPermissions)
		{
			if (hasPermissions)
			{
				if (flags != null && (flags.Value & ExtendedFolderFlags.SharedViaExchange) != (ExtendedFolderFlags)0)
				{
					ExTraceGlobals.StorageTracer.Information(0L, "Folder::IsFlagsValid. ExtendedFolderFlags already contains SharedViaExchange.");
					return true;
				}
			}
			else if (flags == null || (flags.Value & ExtendedFolderFlags.SharedViaExchange) == (ExtendedFolderFlags)0)
			{
				ExTraceGlobals.StorageTracer.Information(0L, "Folder::IsFlagsValid. ExtendedFolderFlags doesn't contain SharedViaExchange.");
				return true;
			}
			return false;
		}

		private static MapiMessage InternalCreateMapiMessage(MapiFolder mapiFolder, CreateMessageType mapiMessageType, StoreSession session)
		{
			EnumValidator.ThrowIfInvalid<CreateMessageType>(mapiMessageType);
			object thisObject = null;
			bool flag = false;
			MapiMessage result;
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
				switch (mapiMessageType)
				{
				case CreateMessageType.Associated:
					result = mapiFolder.CreateAssociatedMessage();
					break;
				case CreateMessageType.Aggregated:
					result = mapiFolder.CreateMessage(CreateMessageFlags.ContentAggregation);
					break;
				default:
					result = mapiFolder.CreateMessage(CreateMessageFlags.DeferredErrors);
					break;
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateMessage, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::InternalCreateMapiMessage failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateMessage, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::InternalCreateMapiMessage failed.", new object[0]),
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
			return result;
		}

		private static MapiMessage InternalCreateMapiMessageForDelivery(MapiFolder mapiFolder, CreateMessageType mapiMessageType, StoreSession session, string internetMessageId, ExDateTime? clientSubmitTime)
		{
			EnumValidator.ThrowIfInvalid<CreateMessageType>(mapiMessageType);
			DateTime? clientSubmitTime2 = null;
			if (clientSubmitTime != null)
			{
				clientSubmitTime2 = new DateTime?((DateTime)clientSubmitTime.Value.ToUtc());
			}
			bool flag = false;
			MapiMessage mapiMessage = null;
			try
			{
				object thisObject = null;
				bool flag2 = false;
				try
				{
					if (session != null)
					{
						session.BeginMapiCall();
						session.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					if (mapiMessageType == CreateMessageType.Aggregated)
					{
						mapiMessage = mapiFolder.CreateMessageForDelivery(CreateMessageFlags.ContentAggregation | CreateMessageFlags.DeferredErrors, internetMessageId, clientSubmitTime2);
					}
					else
					{
						mapiMessage = mapiFolder.CreateMessageForDelivery(CreateMessageFlags.DeferredErrors, internetMessageId, clientSubmitTime2);
					}
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateMessage, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::InternalCreateMapiMessageForDelivery failed.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateMessage, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::InternalCreateMapiMessageForDelivery failed.", new object[0]),
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
							if (flag2)
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
				flag = true;
			}
			finally
			{
				if (!flag && mapiMessage != null)
				{
					mapiMessage.Dispose();
					mapiMessage = null;
				}
			}
			return mapiMessage;
		}

		private static Folder InternalCreateFolder(StoreSession session, StoreId parentId, StoreObjectType itemType, string displayName, CreateMode createMode, bool isSecure)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentId, "parentId");
			EnumValidator.ThrowIfInvalid<StoreObjectType>(itemType, Folder.FolderTypes);
			EnumValidator.ThrowIfInvalid<CreateMode>(createMode, "createMode");
			Folder folder = null;
			CoreFolder coreFolder = null;
			bool flag = false;
			Folder result;
			try
			{
				FolderCreateInfo folderCreateInfo = FolderCreateInfo.GetFolderCreateInfo(itemType);
				coreFolder = CoreFolder.InternalCreate(session, parentId, itemType == StoreObjectType.SearchFolder || itemType == StoreObjectType.OutlookSearchFolder, displayName, createMode, isSecure, folderCreateInfo);
				if (folderCreateInfo.ContainerClass != null)
				{
					coreFolder.PropertyBag[StoreObjectSchema.ContainerClass] = folderCreateInfo.ContainerClass;
				}
				folder = folderCreateInfo.Creator(coreFolder);
				flag = true;
				result = folder;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(folder);
					Util.DisposeIfPresent(coreFolder);
				}
			}
			return result;
		}

		private static T InternalBind<T>(Folder.CoreFolderBindDelegate coreFolderBindDelegate) where T : Folder
		{
			CoreFolder coreFolder = null;
			Folder folder = null;
			T t = default(T);
			bool flag = false;
			T result;
			try
			{
				coreFolder = coreFolderBindDelegate();
				FolderCreateInfo folderCreateInfo = FolderCreateInfo.GetFolderCreateInfo(((ICoreObject)coreFolder).StoreObjectId.ObjectType);
				folder = folderCreateInfo.Creator(coreFolder);
				t = folder.DownCastStoreObject<T>();
				flag = true;
				result = t;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(t);
					Util.DisposeIfPresent(folder);
					Util.DisposeIfPresent(coreFolder);
				}
			}
			return result;
		}

		private static AggregateOperationResult ExecuteOperationOnObjects(Folder.ActOnFolderDelegate actOnFolderDelegate, Folder.ActOnItemsDelegate actOnItemsDelegate, params StoreId[] sourceObjectIds)
		{
			List<StoreObjectId> list = new List<StoreObjectId>(sourceObjectIds.Length);
			List<StoreObjectId> list2 = new List<StoreObjectId>(sourceObjectIds.Length);
			Folder.GroupItemsAndFolders(sourceObjectIds, list, list2);
			List<GroupOperationResult> groupOperationResults = new List<GroupOperationResult>();
			using (List<StoreObjectId>.Enumerator enumerator = list2.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					StoreObjectId sourcefolderId = enumerator.Current;
					Folder.ExecuteGroupOperationAndAggregateResults(groupOperationResults, new StoreObjectId[]
					{
						sourcefolderId
					}, () => actOnFolderDelegate(sourcefolderId));
				}
			}
			if (list.Count > 0)
			{
				StoreObjectId[] sourceItemIdArray = list.ToArray();
				Folder.ExecuteGroupOperationAndAggregateResults(groupOperationResults, sourceItemIdArray, () => actOnItemsDelegate(sourceItemIdArray));
			}
			return Folder.CreateAggregateOperationResult(groupOperationResults);
		}

		private static void GroupItemsAndFolders(StoreId[] sourceObjectIds, List<StoreObjectId> sourceItemIds, List<StoreObjectId> sourceFolderIds)
		{
			foreach (StoreId id in sourceObjectIds)
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(id);
				if (IdConverter.IsFolderId(storeObjectId))
				{
					sourceFolderIds.Add(storeObjectId);
				}
				else
				{
					sourceItemIds.Add(storeObjectId);
				}
			}
		}

		public string DisplayName
		{
			get
			{
				this.CheckDisposed("DisplayName::get");
				return base.GetValueOrDefault<string>(InternalSchema.DisplayName, string.Empty);
			}
			set
			{
				this.CheckDisposed("DisplayName::set");
				this[InternalSchema.DisplayName] = value;
			}
		}

		public string Description
		{
			get
			{
				this.CheckDisposed("Description::get");
				return base.GetValueOrDefault<string>(InternalSchema.Description, string.Empty);
			}
			set
			{
				this.CheckDisposed("Description::set");
				this[InternalSchema.Description] = value;
			}
		}

		public int ItemCount
		{
			get
			{
				this.CheckDisposed("ItemCount::get");
				return base.GetValueOrDefault<int>(InternalSchema.ItemCount);
			}
		}

		public bool HasSubfolders
		{
			get
			{
				this.CheckDisposed("HasSubfolders::get");
				return base.GetValueOrDefault<bool>(InternalSchema.HasChildren);
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return FolderSchema.Instance;
			}
		}

		public int SubfolderCount
		{
			get
			{
				this.CheckDisposed("SubfolderCount::get");
				return base.GetValueOrDefault<int>(InternalSchema.ChildCount);
			}
		}

		public override string ClassName
		{
			get
			{
				this.CheckDisposed("ClassName::get");
				return base.GetValueOrDefault<string>(InternalSchema.ContainerClass, string.Empty);
			}
			set
			{
				this.CheckDisposed("ClassName::set");
				this[InternalSchema.ContainerClass] = value;
			}
		}

		public bool IsExchangeCrossOrgShareFolder
		{
			get
			{
				this.CheckDisposed("IsExchangeCrossOrgShareFolder::get");
				return this.ExtendedFlagsContains(ExtendedFolderFlags.ExchangeCrossOrgShareFolder);
			}
		}

		public bool IsExchangeConsumerShareFolder
		{
			get
			{
				this.CheckDisposed("IsExchangeConsumerShareFolder::get");
				return this.ExtendedFlagsContains(ExtendedFolderFlags.ExchangeConsumerShareFolder);
			}
		}

		public ExDateTime LastAttemptedSyncTime
		{
			get
			{
				this.CheckDisposed("LastAttemptedSyncTime::get");
				return base.GetValueOrDefault<ExDateTime>(FolderSchema.SubscriptionLastAttemptedSyncTime, ExDateTime.MinValue);
			}
			set
			{
				this.CheckDisposed("LastAttemptedSyncTime::set");
				this[FolderSchema.SubscriptionLastAttemptedSyncTime] = value;
			}
		}

		public ExDateTime LastSuccessfulSyncTime
		{
			get
			{
				this.CheckDisposed("LastSuccessfulSyncTime::get");
				return base.GetValueOrDefault<ExDateTime>(FolderSchema.SubscriptionLastSuccessfulSyncTime, ExDateTime.MinValue);
			}
			set
			{
				this.CheckDisposed("LastSuccessfulSyncTime::set");
				this[FolderSchema.SubscriptionLastSuccessfulSyncTime] = value;
			}
		}

		public virtual FolderSaveResult Save()
		{
			return this.Save(SaveMode.NoConflictResolution);
		}

		public FolderSaveResult Save(SaveMode saveMode)
		{
			this.CheckDisposed("Save");
			EnumValidator.ThrowIfInvalid<SaveMode>(saveMode, "saveMode");
			if (saveMode == SaveMode.ResolveConflicts)
			{
				throw new NotSupportedException("Folder does not support resolving conflicts.");
			}
			this.CheckFolderIsShared();
			ExTraceGlobals.StorageTracer.Information<int, SaveMode>((long)this.GetHashCode(), "Folder::Save. HashCode = {0}, saveMode = {1}.", this.GetHashCode(), saveMode);
			FolderSaveResult result = this.CoreFolder.Save(saveMode);
			this.SavePermissions();
			return result;
		}

		private void CheckFolderIsShared()
		{
			if (this.IsPermissionChangePending)
			{
				bool flag = false;
				using (IEnumerator<Permission> enumerator = this.PermissionTable.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						Permission permission = enumerator.Current;
						flag = true;
					}
				}
				ExTraceGlobals.StorageTracer.Information<bool>((long)this.GetHashCode(), "Folder::CheckFolderIsShared. HasPermissions = {0}.", flag);
				ExtendedFolderFlags? valueAsNullable = base.GetValueAsNullable<ExtendedFolderFlags>(FolderSchema.ExtendedFolderFlags);
				Trace storageTracer = ExTraceGlobals.StorageTracer;
				long id = (long)this.GetHashCode();
				string formatString = "Folder::CheckFolderIsShared. Get ExtendedFolderFlags = {0}.";
				object[] array = new object[1];
				object[] array2 = array;
				int num = 0;
				ExtendedFolderFlags? extendedFolderFlags = valueAsNullable;
				array2[num] = ((extendedFolderFlags != null) ? extendedFolderFlags.GetValueOrDefault() : "<null>");
				storageTracer.Information(id, formatString, array);
				if (Folder.IsFlagsValid(valueAsNullable, flag))
				{
					return;
				}
				ExtendedFolderFlags extendedFolderFlags2;
				if (flag)
				{
					extendedFolderFlags2 = ExtendedFolderFlags.SharedViaExchange;
					if (valueAsNullable != null)
					{
						extendedFolderFlags2 |= valueAsNullable.Value;
					}
				}
				else
				{
					extendedFolderFlags2 = (valueAsNullable.Value & ~ExtendedFolderFlags.SharedViaExchange);
				}
				ExTraceGlobals.StorageTracer.Information<ExtendedFolderFlags>((long)this.GetHashCode(), "Folder::CheckFolderIsShared. Set ExtendedFolderFlags = {0}.", extendedFolderFlags2);
				this[FolderSchema.ExtendedFolderFlags] = extendedFolderFlags2;
			}
		}

		private bool IsPermissionChangePending
		{
			get
			{
				return this.PermissionTable != null && this.PermissionTable.IsDirty;
			}
		}

		private void SavePermissions()
		{
			if (this.IsPermissionChangePending)
			{
				this.InternalSavePermissions(this.PermissionTable);
			}
			this.PermissionTable = null;
		}

		protected virtual void InternalSavePermissions(PermissionTable permissionsTable)
		{
			permissionsTable.Save(this.CoreFolder);
		}

		public IQueryResult IFolderQuery(FolderQueryFlags queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			return this.FolderQuery(queryFlags, queryFilter, sortColumns, dataColumns);
		}

		public QueryResult FolderQuery(FolderQueryFlags queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			this.CheckDisposed("FolderQuery");
			return this.CoreFolder.QueryExecutor.FolderQuery(queryFlags, queryFilter, sortColumns, dataColumns);
		}

		public QueryResult ItemQuery(ItemQueryType queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			return this.ItemQuery(queryFlags, queryFilter, sortColumns, (ICollection<PropertyDefinition>)dataColumns);
		}

		public IQueryResult IItemQuery(ItemQueryType queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			return this.ItemQuery(queryFlags, queryFilter, sortColumns, dataColumns);
		}

		public QueryResult ItemQuery(ItemQueryType queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			this.CheckDisposed("ItemQuery");
			return this.CoreFolder.QueryExecutor.ItemQuery(queryFlags, queryFilter, sortColumns, dataColumns);
		}

		public QueryResult ConversationItemQuery(QueryFilter queryFilter, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			return this.ConversationItemQuery(queryFilter, sortColumns, (ICollection<PropertyDefinition>)dataColumns);
		}

		public QueryResult ConversationItemQuery(QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			this.CheckDisposed("ConversationItemQuery");
			if (!(base.Session is MailboxSession))
			{
				throw new InvalidOperationException("ConversationItemQuery can only be performed on folders in MailboxSession");
			}
			if (sortColumns != null)
			{
				foreach (SortBy sortBy in sortColumns)
				{
					if (!ConversationItemSchema.Instance.AllProperties.Contains(sortBy.ColumnDefinition))
					{
						throw new ConversationItemQueryException(new LocalizedString("ConversationItemQuery can only take propertyDefinition from ConversationItemSchema"), sortBy.ColumnDefinition);
					}
				}
			}
			foreach (PropertyDefinition propertyDefinition in dataColumns)
			{
				if (!ConversationItemSchema.Instance.AllProperties.Contains(propertyDefinition) && !ItemSchema.InstanceKey.Equals(propertyDefinition) && !StoreObjectSchema.LastModifiedTime.Equals(propertyDefinition))
				{
					throw new ConversationItemQueryException(new LocalizedString("ConversationItemQuery can only take propertyDefinition from ConversationItemSchema"), propertyDefinition);
				}
			}
			return this.CoreFolder.QueryExecutor.InternalItemQuery(ContentsTableFlags.ShowConversations, queryFilter, sortColumns, QueryExclusionType.Row, dataColumns, null);
		}

		public QueryResult ConversationMembersQuery(QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			this.CheckDisposed("ConversationMembersQuery");
			if (!(base.Session is MailboxSession))
			{
				throw new InvalidOperationException("ConversationMembersQuery can only be performed on folders in MailboxSession");
			}
			return this.CoreFolder.QueryExecutor.InternalItemQuery(ContentsTableFlags.ShowConversationMembers, queryFilter, sortColumns, QueryExclusionType.Row, dataColumns, null);
		}

		public IQueryResult IConversationMembersQuery(QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			return this.ConversationMembersQuery(queryFilter, sortColumns, dataColumns);
		}

		public IQueryResult IConversationItemQuery(QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			return this.ConversationItemQuery(queryFilter, sortColumns, dataColumns);
		}

		public GroupedQueryResult GroupedItemQuery(QueryFilter queryFilter, PropertyDefinition groupBy, GroupSort groupSort, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			this.CheckDisposed("GroupedItemQuery");
			return this.CoreFolder.QueryExecutor.GroupedItemQuery(queryFilter, groupBy, groupSort, sortColumns, dataColumns);
		}

		public QueryResult GroupedItemQuery(QueryFilter queryFilter, ItemQueryType queryFlags, GroupByAndOrder[] groupBy, int expandCount, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			this.CheckDisposed("GroupedItemQuery");
			return this.CoreFolder.QueryExecutor.GroupedItemQuery(queryFilter, queryFlags, groupBy, expandCount, sortColumns, dataColumns);
		}

		public IQueryResult AggregatedItemQuery(QueryFilter queryFilter, QueryFilter aggregationFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns, AggregationExtension aggregationExtension, Schema schema)
		{
			Util.ThrowOnNullArgument(dataColumns, "dataColumns");
			Util.ThrowOnNullArgument(aggregationExtension, "aggregationExtension");
			Util.ThrowOnNullArgument(schema, "schema");
			this.CheckDisposed("AggregatedItemQuery");
			if (!(base.Session is MailboxSession))
			{
				throw new InvalidOperationException("AggregatedItemQuery can only be performed on folders in MailboxSession");
			}
			if (sortColumns != null && sortColumns.Any((SortBy sortBy) => !schema.AllProperties.Contains(sortBy.ColumnDefinition)))
			{
				throw new ArgumentException("AggregatedItemQuery can only take sort columns from given schema", "sortColumns");
			}
			ICollection<PropertyDefinition> collection = null;
			if (aggregationFilter == null)
			{
				collection = dataColumns;
			}
			else
			{
				IEnumerable<PropertyDefinition> enumerable = aggregationFilter.FilterProperties();
				int capacity = dataColumns.Count + enumerable.Count<PropertyDefinition>();
				collection = new List<PropertyDefinition>(capacity);
				foreach (PropertyDefinition item in dataColumns)
				{
					collection.Add(item);
				}
				foreach (PropertyDefinition item2 in enumerable)
				{
					if (!collection.Contains(item2))
					{
						collection.Add(item2);
					}
				}
			}
			if (collection.Any((PropertyDefinition column) => !schema.AllProperties.Contains(column) && column != ItemSchema.InstanceKey))
			{
				throw new ArgumentException("AggregatedItemQuery can only take data columns from given schema", "dataColumns");
			}
			ContentsTableFlags flags = ContentsTableFlags.ShowConversations | ContentsTableFlags.ExpandedConversationView;
			ConversationMembersQueryResult conversationMembersQueryResult = (ConversationMembersQueryResult)this.CoreFolder.QueryExecutor.InternalItemQuery(flags, queryFilter, sortColumns, QueryExclusionType.Row, collection, aggregationExtension);
			if (aggregationFilter != null)
			{
				return AggregationQueryResult.FromQueryResult(conversationMembersQueryResult, aggregationFilter, dataColumns.Count);
			}
			return conversationMembersQueryResult;
		}

		public IQueryResult PersonItemQuery(QueryFilter queryFilter, QueryFilter aggregationFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns, AggregationExtension aggregationExtension)
		{
			ArgumentValidator.ThrowIfNull("aggregationExtension", aggregationExtension);
			return this.AggregatedItemQuery(queryFilter, aggregationFilter, sortColumns, dataColumns, aggregationExtension, PersonSchema.Instance);
		}

		public IQueryResult PersonItemQuery(QueryFilter queryFilter, QueryFilter aggregationFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			return this.PersonItemQuery(queryFilter, aggregationFilter, sortColumns, dataColumns, new PeopleAggregationExtension((MailboxSession)base.Session));
		}

		public IQueryResult PersonItemQuery(SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			return this.PersonItemQuery(null, null, sortColumns, dataColumns);
		}

		public IQueryResult PersonItemQuery(ICollection<PropertyDefinition> dataColumns)
		{
			return this.PersonItemQuery(null, null, null, dataColumns);
		}

		public IQueryResult AggregatedConversationQuery(QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns, AggregationExtension aggregationExtension)
		{
			return this.AggregatedItemQuery(queryFilter, null, sortColumns, dataColumns, aggregationExtension, AggregatedConversationSchema.Instance);
		}

		public GroupOperationResult CopyItems(StoreId destinationFolderId, params StoreId[] sourceItemIds)
		{
			return this.CopyItems(base.Session, destinationFolderId, sourceItemIds);
		}

		public GroupOperationResult CopyItems(StoreSession destinationSession, StoreId destinationFolderId, params StoreId[] sourceItemIds)
		{
			return this.CopyItems(destinationSession, destinationFolderId, true, sourceItemIds);
		}

		public GroupOperationResult CopyItems(StoreSession destinationSession, StoreId destinationFolderId, bool updateSource, params StoreId[] sourceItemIds)
		{
			this.CheckDisposed("CopyItems");
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "Folder::CopyItems. HashCode = {0}.", this.GetHashCode());
			if (sourceItemIds == null)
			{
				throw new ArgumentNullException("sourceItemIds");
			}
			if (destinationFolderId == null)
			{
				throw new ArgumentNullException("destinationFolderId");
			}
			if (destinationSession == null)
			{
				throw new ArgumentNullException("destinationSession");
			}
			GroupOperationResult result;
			using (Folder folder = Folder.Bind(destinationSession, destinationFolderId))
			{
				result = this.CopyItems(folder, Folder.StoreIdsToStoreObjectIds(sourceItemIds), null, null);
			}
			return result;
		}

		public GroupOperationResult MoveItems(StoreId destinationFolderId, params StoreId[] sourceItemIds)
		{
			return this.MoveItems(base.Session, destinationFolderId, sourceItemIds);
		}

		public GroupOperationResult MoveItems(StoreSession destinationSession, StoreId destinationFolderId, params StoreId[] sourceItemIds)
		{
			this.CheckDisposed("MoveItems");
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "Folder::MoveItems. HashCode = {0}.", this.GetHashCode());
			if (sourceItemIds == null)
			{
				throw new ArgumentNullException("sourceItemIds");
			}
			if (destinationFolderId == null)
			{
				throw new ArgumentNullException("destinationFolderId");
			}
			if (destinationSession == null)
			{
				throw new ArgumentNullException("destinationSession");
			}
			GroupOperationResult result;
			using (Folder folder = Folder.Bind(destinationSession, destinationFolderId))
			{
				result = this.MoveItems(folder, Folder.StoreIdsToStoreObjectIds(sourceItemIds), null, null);
			}
			return result;
		}

		public GroupOperationResult CopyFolder(StoreId destinationFolderId, StoreId sourceFolderId)
		{
			return this.CopyFolder(base.Session, destinationFolderId, sourceFolderId);
		}

		public GroupOperationResult CopyFolder(StoreSession destinationSession, StoreId destinationFolderId, StoreId sourceFolderId)
		{
			this.CheckDisposed("CopyFolder");
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "Folder::CopyFolder. HashCode = {0}.", this.GetHashCode());
			if (sourceFolderId == null)
			{
				throw new ArgumentNullException("sourceFolderId");
			}
			if (destinationFolderId == null)
			{
				throw new ArgumentNullException("destinationFolderId");
			}
			if (destinationSession == null)
			{
				throw new ArgumentNullException("destinationSession");
			}
			GroupOperationResult result;
			using (Folder folder = Folder.Bind(destinationSession, destinationFolderId))
			{
				result = this.CoreFolder.CopyFolder(folder.CoreFolder, CopySubObjects.Copy, StoreId.GetStoreObjectId(sourceFolderId));
			}
			return result;
		}

		public GroupOperationResult MoveFolder(StoreId destinationFolderId, StoreId sourceFolderId)
		{
			return this.MoveFolder(base.Session, destinationFolderId, sourceFolderId);
		}

		public GroupOperationResult MoveFolder(StoreSession destinationSession, StoreId destinationFolderId, StoreId sourceFolderId)
		{
			this.CheckDisposed("MoveFolder");
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "Folder::MoveFolder. HashCode = {0}.", this.GetHashCode());
			if (sourceFolderId == null)
			{
				throw new ArgumentNullException("sourceFolderId");
			}
			if (destinationFolderId == null)
			{
				throw new ArgumentNullException("destinationFolderId");
			}
			if (destinationSession == null)
			{
				throw new ArgumentNullException("destinationSession");
			}
			GroupOperationResult result;
			using (Folder folder = Folder.Bind(destinationSession, destinationFolderId))
			{
				result = this.CoreFolder.MoveFolder(folder.CoreFolder, StoreId.GetStoreObjectId(sourceFolderId));
			}
			return result;
		}

		public AggregateOperationResult CopyObjects(StoreId destinationFolderId, params StoreId[] sourceObjectIds)
		{
			return this.CopyObjects(base.Session, destinationFolderId, sourceObjectIds);
		}

		public AggregateOperationResult CopyObjects(StoreSession destinationSession, StoreId destinationFolderId, params StoreId[] sourceObjectIds)
		{
			return this.CopyObjects(destinationSession, destinationFolderId, false, sourceObjectIds);
		}

		public AggregateOperationResult CopyObjects(StoreSession destinationSession, StoreId destinationFolderId, bool returnNewIds, params StoreId[] sourceObjectIds)
		{
			Folder.<>c__DisplayClassf CS$<>8__locals1 = new Folder.<>c__DisplayClassf();
			CS$<>8__locals1.returnNewIds = returnNewIds;
			CS$<>8__locals1.<>4__this = this;
			this.CheckDisposed("CopyObjects");
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "Folder::CopyObjects. HashCode = {0}.", this.GetHashCode());
			if (sourceObjectIds == null)
			{
				throw new ArgumentNullException("sourceObjectIds");
			}
			if (destinationFolderId == null)
			{
				throw new ArgumentNullException("destinationFolderId");
			}
			if (destinationSession == null)
			{
				throw new ArgumentNullException("destinationSession");
			}
			AggregateOperationResult result;
			using (Folder destinationFolder = Folder.Bind(destinationSession, destinationFolderId))
			{
				result = Folder.ExecuteOperationOnObjects((StoreObjectId sourceFolderId) => CS$<>8__locals1.<>4__this.CoreFolder.CopyFolder(destinationFolder.CoreFolder, CopySubObjects.Copy, sourceFolderId), (StoreObjectId[] sourceItemIds) => CS$<>8__locals1.<>4__this.CopyItems(destinationFolder, sourceItemIds, null, null, CS$<>8__locals1.returnNewIds), sourceObjectIds);
			}
			return result;
		}

		public AggregateOperationResult MoveObjects(StoreId destinationFolderId, params StoreId[] sourceObjectIds)
		{
			return this.MoveObjects(destinationFolderId, null, sourceObjectIds);
		}

		public AggregateOperationResult MoveObjects(StoreId destinationFolderId, DeleteItemFlags? deleteFlags, params StoreId[] sourceObjectIds)
		{
			return this.MoveObjects(base.Session, destinationFolderId, deleteFlags, sourceObjectIds);
		}

		public AggregateOperationResult MoveObjects(StoreSession destinationSession, StoreId destinationFolderId, params StoreId[] sourceObjectIds)
		{
			return this.MoveObjects(destinationSession, destinationFolderId, null, sourceObjectIds);
		}

		public AggregateOperationResult MoveObjects(StoreSession destinationSession, StoreId destinationFolderId, DeleteItemFlags? deleteFlags, params StoreId[] sourceObjectIds)
		{
			return this.MoveObjects(destinationSession, destinationFolderId, false, deleteFlags, sourceObjectIds);
		}

		public AggregateOperationResult MoveObjects(StoreSession destinationSession, StoreId destinationFolderId, bool returnNewIds, params StoreId[] sourceObjectIds)
		{
			return this.MoveObjects(destinationSession, destinationFolderId, returnNewIds, null, sourceObjectIds);
		}

		public AggregateOperationResult MoveObjects(StoreSession destinationSession, StoreId destinationFolderId, bool returnNewIds, DeleteItemFlags? deleteFlags, params StoreId[] sourceObjectIds)
		{
			Folder.<>c__DisplayClass17 CS$<>8__locals1 = new Folder.<>c__DisplayClass17();
			CS$<>8__locals1.returnNewIds = returnNewIds;
			CS$<>8__locals1.deleteFlags = deleteFlags;
			CS$<>8__locals1.<>4__this = this;
			this.CheckDisposed("MoveObjects");
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "Folder::MoveObjects. HashCode = {0}.", this.GetHashCode());
			if (sourceObjectIds == null)
			{
				throw new ArgumentNullException("sourceObjectIds");
			}
			if (destinationFolderId == null)
			{
				throw new ArgumentNullException("destinationFolderId");
			}
			if (destinationSession == null)
			{
				throw new ArgumentNullException("destinationSession");
			}
			AggregateOperationResult result;
			using (Folder destinationFolder = Folder.Bind(destinationSession, destinationFolderId))
			{
				result = Folder.ExecuteOperationOnObjects((StoreObjectId sourceFolderId) => CS$<>8__locals1.<>4__this.CoreFolder.MoveFolder(destinationFolder.CoreFolder, sourceFolderId), (StoreObjectId[] sourceItemIds) => CS$<>8__locals1.<>4__this.MoveItems(destinationFolder, sourceItemIds, null, null, CS$<>8__locals1.returnNewIds, CS$<>8__locals1.deleteFlags), sourceObjectIds);
			}
			return result;
		}

		internal static void CheckPairedArray<T>(T[] propertyDefinitions, object[] values, string errorMessage) where T : PropertyDefinition
		{
			if (propertyDefinitions == null || values == null)
			{
				throw new ArgumentException(errorMessage);
			}
			if (propertyDefinitions.Length != values.Length)
			{
				throw new ArgumentException(errorMessage);
			}
		}

		public GroupOperationResult UnsafeMoveItemsAndSetProperties(StoreId destinationFolderId, StoreId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] values)
		{
			this.CheckDisposed("UnsafeMoveItemsAndSetProperties");
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "Folder::UnsafeMoveItemsAndSetProperties. HashCode = {0}.", this.GetHashCode());
			if (sourceItemIds == null)
			{
				throw new ArgumentNullException("sourceItemIds");
			}
			if (destinationFolderId == null)
			{
				throw new ArgumentNullException("destinationFolderId");
			}
			Folder.CheckPairedArray<PropertyDefinition>(propertyDefinitions, values, ServerStrings.PropertyDefinitionsValuesNotMatch);
			GroupOperationResult result;
			using (Folder folder = Folder.Bind(base.Session, destinationFolderId))
			{
				result = this.MoveItems(folder, Folder.StoreIdsToStoreObjectIds(sourceItemIds), propertyDefinitions, values);
			}
			return result;
		}

		public GroupOperationResult UnsafeCopyItemsAndSetProperties(StoreId destinationFolderId, StoreId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] values)
		{
			this.CheckDisposed("UnsafeCopyItemsAndSetProperties");
			ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "Folder::UnsafeCopyItemsAndSetProperties. HashCode = {0}.", this.GetHashCode());
			if (sourceItemIds == null)
			{
				throw new ArgumentNullException("sourceItemIds");
			}
			if (destinationFolderId == null)
			{
				throw new ArgumentNullException("destinationFolderId");
			}
			Folder.CheckPairedArray<PropertyDefinition>(propertyDefinitions, values, ServerStrings.PropertyDefinitionsValuesNotMatch);
			GroupOperationResult result;
			using (Folder folder = Folder.Bind(base.Session, destinationFolderId))
			{
				result = this.CopyItems(folder, Folder.StoreIdsToStoreObjectIds(sourceItemIds), propertyDefinitions, values);
			}
			return result;
		}

		public AggregateOperationResult DeleteObjects(DeleteItemFlags deleteFlags, params StoreId[] ids)
		{
			return this.DeleteObjects(deleteFlags, false, ids);
		}

		public AggregateOperationResult DeleteObjects(DeleteItemFlags deleteFlags, bool returnNewItemIds, params StoreId[] ids)
		{
			this.CheckDisposed("DeleteObjects");
			base.Session.CheckDeleteItemFlags(deleteFlags);
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Folder::DeleteObjects.");
			if ((deleteFlags & DeleteItemFlags.SuppressReadReceipt) != DeleteItemFlags.None && (deleteFlags & DeleteItemFlags.MoveToDeletedItems) == DeleteItemFlags.None)
			{
				this.CoreFolder.ClearNotReadNotificationPending(ids);
			}
			List<OccurrenceStoreObjectId> list = new List<OccurrenceStoreObjectId>();
			List<StoreId> list2 = new List<StoreId>();
			Folder.GroupOccurrencesAndObjectIds(ids, list2, list);
			List<GroupOperationResult> list3 = new List<GroupOperationResult>();
			AggregateOperationResult aggregateOperationResult;
			if ((deleteFlags & DeleteItemFlags.MoveToDeletedItems) == DeleteItemFlags.MoveToDeletedItems)
			{
				aggregateOperationResult = this.MoveObjects(base.Session, ((MailboxSession)base.Session).GetDefaultFolderId(DefaultFolderType.DeletedItems), returnNewItemIds, new DeleteItemFlags?(deleteFlags), list2.ToArray());
			}
			else
			{
				aggregateOperationResult = this.InternalDeleteObjects(deleteFlags, list2.ToArray());
			}
			list3.AddRange(aggregateOperationResult.GroupOperationResults);
			using (List<OccurrenceStoreObjectId>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					OccurrenceStoreObjectId occurrenceId = enumerator.Current;
					Folder.ExecuteGroupOperationAndAggregateResults(list3, new StoreObjectId[]
					{
						occurrenceId
					}, () => this.Session.DeleteCalendarOccurrence(deleteFlags, occurrenceId));
				}
			}
			return Folder.CreateAggregateOperationResult(list3);
		}

		public GroupOperationResult DeleteAllObjects(DeleteItemFlags flags)
		{
			bool deleteAssociated = false;
			return this.DeleteAllObjects(flags, deleteAssociated);
		}

		public GroupOperationResult DeleteAllObjects(DeleteItemFlags flags, bool deleteAssociated)
		{
			this.CheckDisposed("DeleteAllObjects");
			base.Session.CheckDeleteItemFlags(flags);
			ExTraceGlobals.StorageTracer.Information<DeleteItemFlags>((long)this.GetHashCode(), "Folder::DeleteAllObjects. flags = {0}.", flags);
			if ((flags & DeleteItemFlags.SuppressReadReceipt) != DeleteItemFlags.None && (flags & DeleteItemFlags.MoveToDeletedItems) == DeleteItemFlags.None)
			{
				this.CoreFolder.ClearNotReadNotificationPending();
			}
			EmptyFolderFlags emptyFolderFlags;
			switch (CoreFolder.NormalizeDeleteFlags(flags))
			{
			case DeleteItemFlags.SoftDelete:
				emptyFolderFlags = EmptyFolderFlags.None;
				break;
			case DeleteItemFlags.HardDelete:
				emptyFolderFlags = EmptyFolderFlags.HardDelete;
				break;
			default:
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Folder::DeleteAllObjects. DeleteItemFlags is not supported in DeleteAllItems API.");
				throw new NotSupportedException(ServerStrings.Ex12NotSupportedDeleteItemFlags((int)flags));
			}
			if (deleteAssociated)
			{
				emptyFolderFlags |= EmptyFolderFlags.DeleteAssociatedMessages;
			}
			return this.CoreFolder.EmptyFolder(false, emptyFolderFlags);
		}

		public AggregateOperationResult DeleteAllItems(DeleteItemFlags flags)
		{
			return this.DeleteAllItems(flags, null);
		}

		public AggregateOperationResult DeleteAllItems(DeleteItemFlags flags, DateTime? createdBefore)
		{
			this.CheckDisposed("DeleteAllItems");
			base.Session.CheckDeleteItemFlags(flags);
			ExTraceGlobals.StorageTracer.Information<DeleteItemFlags>((long)this.GetHashCode(), "Folder::DeleteAllItems. flags = {0}.", flags);
			List<GroupOperationResult> list = new List<GroupOperationResult>();
			QueryFilter queryFilter = null;
			if (createdBefore != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.ReceivedTime, createdBefore.Value),
					new ExistsFilter(ItemSchema.ReceivedTime)
				});
			}
			using (QueryResult queryResult = this.ItemQuery(ItemQueryType.None, queryFilter, null, new PropertyDefinition[]
			{
				ItemSchema.Id
			}))
			{
				object[][] rows;
				do
				{
					rows = queryResult.GetRows(200);
					if (rows.Length != 0)
					{
						VersionedId[] array = new VersionedId[rows.Length];
						for (int i = 0; i < rows.Length; i++)
						{
							array[i] = Microsoft.Exchange.Data.Storage.PropertyBag.CheckPropertyValue<VersionedId>(ItemSchema.Id, rows[i][0]);
						}
						AggregateOperationResult aggregateOperationResult = this.DeleteObjects(flags, array);
						list.AddRange(aggregateOperationResult.GroupOperationResults);
					}
				}
				while (rows.Length != 0);
			}
			return Folder.CreateAggregateOperationResult(list);
		}

		public void MarkAllAsRead(bool suppressReadReceipts)
		{
			this.CheckDisposed("MarkAllAsRead");
			this.InternalSetReadFlags(suppressReadReceipts, true, new StoreId[0]);
		}

		public void MarkAllAsUnread(bool suppressReadReceipts)
		{
			this.CheckDisposed("MarkAllAsUnRead");
			this.InternalSetReadFlags(suppressReadReceipts, false, new StoreId[0]);
		}

		public void MarkAsRead(bool suppressReadReceipts, params StoreId[] ids)
		{
			this.MarkAsRead(suppressReadReceipts, false, ids);
		}

		public void MarkAsRead(bool suppressReadReceipts, bool suppressNotReadReceipts, params StoreId[] ids)
		{
			this.CheckDisposed("MarkAsRead");
			if (ids == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "Folder::MarkAsRead. The parameter cannot be null. Parameter = {0}.", "itemIds");
				throw new ArgumentNullException(ServerStrings.ExNullParameter("itemIds", 1));
			}
			if (ids.Length != 0)
			{
				this.InternalSetReadFlags(suppressReadReceipts, true, suppressNotReadReceipts, ids);
			}
		}

		public void MarkAsUnread(bool suppressReadReceipts, params StoreId[] ids)
		{
			this.CheckDisposed("MarkAsUnRead");
			if (ids == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "Folder::MarkAsUnRead. The parameter cannot be null. Parameter = {0}.", "itemIds");
				throw new ArgumentNullException(ServerStrings.ExNullParameter("itemIds", 1));
			}
			if (ids.Length != 0)
			{
				this.InternalSetReadFlags(suppressReadReceipts, false, ids);
			}
		}

		public MessageStatusFlags SetItemStatus(StoreObjectId itemId, MessageStatusFlags status, MessageStatusFlags statusMask)
		{
			this.CheckDisposed("SetItemStatus");
			EnumValidator.ThrowIfInvalid<MessageStatusFlags>(status, "status");
			EnumValidator.ThrowIfInvalid<MessageStatusFlags>(statusMask, "statusMask");
			MessageStatusFlags result;
			this.CoreFolder.SetItemStatus(itemId, status, statusMask, out result);
			return result;
		}

		public void SetItemFlags(StoreObjectId itemId, MessageFlags flags, MessageFlags flagsMask)
		{
			this.CheckDisposed("SetItemFlags");
			EnumValidator.ThrowIfInvalid<MessageFlags>(flags, "flags");
			EnumValidator.ThrowIfInvalid<MessageFlags>(flagsMask, "flagsMask");
			this.CoreFolder.SetItemFlags(itemId, flags, flagsMask);
		}

		internal void ClearNotReadNotificationPending(StoreId[] itemIds)
		{
			this.CheckDisposed("ClearNotReadNotificationPending");
			this.CoreFolder.ClearNotReadNotificationPending(itemIds);
		}

		public PermissionSet GetPermissionSet()
		{
			this.CheckDisposed("GetPermissionSet");
			if (this.PermissionTable == null)
			{
				if (base.CoreObject.Origin == Origin.Existing)
				{
					this.PermissionTable = PermissionTable.Load(new Func<PermissionTable, PermissionSet>(this.CreatePermissionSet), this.CoreFolder);
				}
				else
				{
					this.PermissionTable = PermissionTable.Create(new Func<PermissionTable, PermissionSet>(this.CreatePermissionSet));
				}
			}
			return this.PermissionTable.PermissionSet;
		}

		protected CoreFolder CoreFolder
		{
			get
			{
				return (CoreFolder)base.CoreObject;
			}
		}

		internal IModifyTable GetRuleTable()
		{
			return this.CoreFolder.GetRuleTable();
		}

		internal IModifyTable GetRuleTable(IModifyTableRestriction modifyTableRestriction)
		{
			return this.CoreFolder.GetRuleTable(modifyTableRestriction);
		}

		internal static bool IsNotConfigurationFolder(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			VersionedId versionedId = validatablePropertyBag.TryGetProperty(InternalSchema.FolderId) as VersionedId;
			if (versionedId == null)
			{
				return true;
			}
			if (!context.Session.IsPublicFolderSession)
			{
				return !((MailboxSession)context.Session).InternalIsConfigurationFolder(versionedId.ObjectId);
			}
			return !((PublicFolderSession)context.Session).GetPublicFolderRootId().Equals(versionedId.ObjectId);
		}

		internal static bool DoesFolderHaveFixedDisplayName(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			VersionedId versionedId = validatablePropertyBag.TryGetProperty(InternalSchema.FolderId) as VersionedId;
			MailboxSession mailboxSession = context.Session as MailboxSession;
			if (mailboxSession != null && versionedId != null && !mailboxSession.IsMoveUser)
			{
				DefaultFolderType defaultFolderType = mailboxSession.IsDefaultFolderType(versionedId);
				return defaultFolderType != DefaultFolderType.None && defaultFolderType != DefaultFolderType.QuickContacts;
			}
			return false;
		}

		internal QueryResult ItemQueryInternal(ItemQueryType queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, QueryExclusionType queryExclusionType, ICollection<PropertyDefinition> dataColumns)
		{
			return this.CoreFolder.QueryExecutor.InternalItemQuery(QueryExecutor.ItemQueryTypeToContentsTableFlags(queryFlags), queryFilter, sortColumns, queryExclusionType, dataColumns, null);
		}

		internal static MapiFolder DeferBind(StoreSession storeSession, StoreId folderId)
		{
			if (!Folder.IsFolderId(folderId))
			{
				throw new ArgumentException(ServerStrings.InvalidFolderId(folderId.ToBase64String()));
			}
			MapiProp mapiProp = null;
			bool flag = false;
			MapiFolder result;
			try
			{
				object thisObject = null;
				bool flag2 = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					mapiProp = storeSession.GetMapiProp(StoreId.GetStoreObjectId(folderId));
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateMessage, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::DeferBind.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateMessage, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::DeferBind.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag2)
							{
								storeSession.EndServerHealthCall();
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
				MapiFolder mapiFolder = (MapiFolder)mapiProp;
				flag = true;
				result = mapiFolder;
			}
			finally
			{
				if (!flag && mapiProp != null)
				{
					mapiProp.Dispose();
					mapiProp = null;
				}
			}
			return result;
		}

		internal static T InternalBind<T>(StoreSession storeSession, MapiFolder mapiFolder, StoreObjectId folderObjectId, byte[] changeKey, PropertyDefinition[] propsToReturn) where T : Folder
		{
			return Folder.InternalBind<T>(() => CoreFolder.InternalBind(storeSession, mapiFolder, folderObjectId, changeKey, propsToReturn));
		}

		internal static T InternalBind<T>(StoreSession storeSession, StoreId folderId, ICollection<PropertyDefinition> propsToReturn) where T : Folder
		{
			return Folder.InternalBind<T>(() => CoreFolder.InternalBind(storeSession, folderId, false, propsToReturn));
		}

		internal static bool IsFolderType(StoreObjectType type)
		{
			bool flag = type == StoreObjectType.ShortcutFolder;
			return (type != StoreObjectType.Unknown && type <= StoreObjectType.OutlookSearchFolder) || flag;
		}

		internal MessageItem CreateMessageInternal(CreateMessageType type)
		{
			EnumValidator.AssertValid<CreateMessageType>(type);
			ItemCreateInfo messageItemInfo = ItemCreateInfo.MessageItemInfo;
			return ItemBuilder.CreateNewItem<MessageItem>(base.Session, messageItemInfo, () => Folder.InternalCreateMapiMessage(this.MapiFolder, type, this.Session));
		}

		internal static MapiMessage InternalCreateMapiMessage(StoreSession store, StoreId folderId, CreateMessageType itemType)
		{
			PublicFolderSession publicFolderSession = store as PublicFolderSession;
			if (publicFolderSession != null)
			{
				using (CoreFolder coreFolder = CoreFolder.Bind(store, folderId, new PropertyDefinition[]
				{
					CoreFolderSchema.ReplicaList
				}))
				{
					using (MapiFolder mapiFolder = (MapiFolder)Microsoft.Exchange.Data.Storage.CoreObject.GetPersistablePropertyBag(coreFolder).MapiProp)
					{
						return Folder.InternalCreateMapiMessage(mapiFolder, itemType, store);
					}
				}
			}
			MapiMessage result;
			using (MapiFolder mapiFolder2 = Folder.DeferBind(store, folderId))
			{
				result = Folder.InternalCreateMapiMessage(mapiFolder2, itemType, store);
			}
			return result;
		}

		internal MessageItem CreateMessageForDeliveryInternal(CreateMessageType type, string internetMessageId, ExDateTime? clientSubmitTime)
		{
			EnumValidator.AssertValid<CreateMessageType>(type);
			ItemCreateInfo messageItemInfo = ItemCreateInfo.MessageItemInfo;
			return ItemBuilder.CreateNewItem<MessageItem>(base.Session, messageItemInfo, () => Folder.InternalCreateMapiMessageForDelivery(this.MapiFolder, type, this.Session, internetMessageId, clientSubmitTime));
		}

		internal static MapiMessage InternalCreateMapiMessageForDelivery(StoreSession store, StoreId folderId, CreateMessageType itemType, string internetMessageId, ExDateTime? clientSubmitTime)
		{
			bool flag = false;
			MapiMessage mapiMessage = null;
			try
			{
				using (MapiFolder mapiFolder = Folder.DeferBind(store, folderId))
				{
					mapiMessage = Folder.InternalCreateMapiMessageForDelivery(mapiFolder, itemType, store, internetMessageId, clientSubmitTime);
				}
				flag = true;
			}
			finally
			{
				if (!flag && mapiMessage != null)
				{
					mapiMessage.Dispose();
					mapiMessage = null;
				}
			}
			return mapiMessage;
		}

		internal virtual PermissionSet CreatePermissionSet(PermissionTable permissionTable)
		{
			return new PermissionSet(permissionTable);
		}

		internal void InternalSetReadFlags(bool suppressReceipts, bool markAsRead, params StoreId[] itemIds)
		{
			this.InternalSetReadFlags(suppressReceipts, markAsRead, itemIds, false, false);
		}

		internal void InternalSetReadFlags(bool suppressReceipts, bool markAsRead, bool suppressNotReadReceipts, params StoreId[] itemIds)
		{
			this.InternalSetReadFlags(suppressReceipts, markAsRead, itemIds, false, suppressNotReadReceipts);
		}

		internal void InternalSetReadFlags(bool suppressReceipts, bool markAsRead, StoreId[] itemIds, bool throwIfWarning, bool suppressNotReadReceipts = false)
		{
			SetReadFlags setReadFlags = markAsRead ? SetReadFlags.None : SetReadFlags.ClearRead;
			if (suppressReceipts)
			{
				if (markAsRead)
				{
					setReadFlags |= SetReadFlags.SuppressReceipt;
				}
				if (!markAsRead || suppressNotReadReceipts)
				{
					this.CoreFolder.InternalSetReadFlags(SetReadFlags.ClearRnPending | SetReadFlags.CleanNrnPending, itemIds, throwIfWarning);
				}
			}
			this.CoreFolder.InternalSetReadFlags(setReadFlags, itemIds, throwIfWarning);
			ICollection<StoreObjectId> itemIds2 = (from id in itemIds
			select StoreId.GetStoreObjectId(id)).ToArray<StoreObjectId>();
			if (base.Session.ActivitySession != null)
			{
				if (markAsRead)
				{
					base.Session.ActivitySession.CaptureMarkAsRead(itemIds2);
					return;
				}
				base.Session.ActivitySession.CaptureMarkAsUnread(itemIds2);
			}
		}

		internal StoreObjectId FindChildFolderByName(string childFolderName)
		{
			StoreObjectId result;
			using (QueryResult queryResult = this.FolderQuery(FolderQueryFlags.None, null, null, new PropertyDefinition[]
			{
				FolderSchema.Id,
				FolderSchema.DisplayName
			}))
			{
				queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.DisplayName, childFolderName));
				object[][] rows = queryResult.GetRows(1);
				if (rows.Length > 0)
				{
					result = ((VersionedId)rows[0][0]).ObjectId;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private PermissionTable PermissionTable
		{
			get
			{
				return this.permissionTable;
			}
			set
			{
				this.permissionTable = value;
			}
		}

		public MapiFolder MapiFolder
		{
			get
			{
				this.CheckDisposed("MapiFolder::get");
				return (MapiFolder)base.MapiProp;
			}
		}

		private StoreObjectPropertyBag FolderPropertyBag
		{
			get
			{
				return (StoreObjectPropertyBag)PersistablePropertyBag.GetPersistablePropertyBag(this.CoreFolder.PropertyBag);
			}
		}

		private AggregateOperationResult InternalDeleteObjects(DeleteItemFlags flags, StoreId[] sourceObjectIds)
		{
			this.CheckDisposed("InternalDeleteObjects");
			EnumValidator.AssertValid<DeleteItemFlags>(flags);
			if (sourceObjectIds == null)
			{
				throw new ArgumentNullException("sourceObjectIds");
			}
			return Folder.ExecuteOperationOnObjects((StoreObjectId sourceFolderId) => this.InternalDeleteFolder(flags, sourceFolderId), (StoreObjectId[] sourceItemIds) => this.InternalDeleteItems(flags, sourceItemIds), sourceObjectIds);
		}

		internal GroupOperationResult InternalDeleteItems(DeleteItemFlags flags, StoreObjectId[] ids)
		{
			this.CheckDisposed("InternalDeleteItems");
			EnumValidator.AssertValid<DeleteItemFlags>(flags);
			this.CoreFolder.ValidateItemIds(ids, delegate(StoreObjectId storeObjectId)
			{
				this.CheckItemBelongsToThisFolder(storeObjectId);
			});
			return this.CoreFolder.DeleteItems(flags, ids);
		}

		private GroupOperationResult InternalDeleteFolder(DeleteItemFlags flags, StoreObjectId folderId)
		{
			this.CheckDisposed("InternalDeleteFolder");
			EnumValidator.AssertValid<DeleteItemFlags>(flags);
			this.CoreFolder.CheckIsNotDefaultFolder(folderId, Folder.canDeleteDefaultFolders);
			return this.CoreFolder.DeleteFolder(flags, folderId);
		}

		internal static bool CanDeleteFolder(StoreSession session, StoreObjectId folderId)
		{
			return CoreFolder.CheckIsNotDefaultFolder(session, folderId, Folder.canDeleteDefaultFolders);
		}

		internal static void ExecuteGroupOperationAndAggregateResults(List<GroupOperationResult> groupOperationResults, StoreObjectId[] sourceObjectIds, Folder.GroupOperationDelegate groupOperationDelegate)
		{
			GroupOperationResult item;
			try
			{
				item = groupOperationDelegate();
			}
			catch (StoragePermanentException storageException)
			{
				item = new GroupOperationResult(OperationResult.Failed, sourceObjectIds, storageException);
			}
			catch (StorageTransientException storageException2)
			{
				item = new GroupOperationResult(OperationResult.Failed, sourceObjectIds, storageException2);
			}
			groupOperationResults.Add(item);
		}

		internal static AggregateOperationResult CreateAggregateOperationResult(List<GroupOperationResult> groupOperationResults)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			foreach (GroupOperationResult groupOperationResult in groupOperationResults)
			{
				if (groupOperationResult.OperationResult == OperationResult.Succeeded)
				{
					num++;
				}
				else
				{
					if (groupOperationResult.OperationResult != OperationResult.Failed)
					{
						flag = true;
						break;
					}
					num2++;
				}
			}
			OperationResult operationResult;
			if (num == groupOperationResults.Count)
			{
				operationResult = OperationResult.Succeeded;
			}
			else if (flag || num > 0)
			{
				operationResult = OperationResult.PartiallySucceeded;
			}
			else
			{
				operationResult = OperationResult.Failed;
			}
			return new AggregateOperationResult(operationResult, groupOperationResults.ToArray());
		}

		internal static void GroupOccurrencesAndObjectIds(StoreId[] sourceObjectIds, List<StoreId> sourceObjectIdList, List<OccurrenceStoreObjectId> sourceOccurrenceIdList)
		{
			for (int i = 0; i < sourceObjectIds.Length; i++)
			{
				if (sourceObjectIds[i] == null)
				{
					throw new ArgumentException(ServerStrings.ExNullItemIdParameter(0));
				}
				OccurrenceStoreObjectId occurrenceStoreObjectId = StoreId.GetStoreObjectId(sourceObjectIds[i]) as OccurrenceStoreObjectId;
				if (occurrenceStoreObjectId != null)
				{
					sourceOccurrenceIdList.Add(occurrenceStoreObjectId);
				}
				else
				{
					sourceObjectIdList.Add(sourceObjectIds[i]);
				}
			}
		}

		internal static void CoreObjectUpdateRetentionProperties(CoreFolder coreFolder)
		{
			MailboxSession mailboxSession = coreFolder.Session as MailboxSession;
			if (mailboxSession == null || COWSettings.IsMrmAction(mailboxSession))
			{
				return;
			}
			PolicyTagHelper.ApplyPolicyToFolder(mailboxSession, coreFolder);
		}

		internal GroupOperationResult CopyItems(Folder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues)
		{
			return this.CopyItems(destinationFolder, sourceItemIds, propertyDefinitions, propertyValues, false);
		}

		internal GroupOperationResult CopyItems(Folder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues, bool returnNewIds)
		{
			this.CheckDisposed("CopyItems");
			return this.CoreFolder.CopyItems(destinationFolder.CoreFolder, sourceItemIds, propertyDefinitions, propertyValues, delegate(StoreObjectId storeObjectId)
			{
				this.CheckItemBelongsToThisFolder(storeObjectId);
			}, returnNewIds);
		}

		internal GroupOperationResult MoveItems(Folder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues)
		{
			return this.MoveItems(destinationFolder, sourceItemIds, propertyDefinitions, propertyValues, false);
		}

		public GroupOperationResult MoveItems(Folder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues, bool returnNewIds)
		{
			return this.MoveItems(destinationFolder, sourceItemIds, propertyDefinitions, propertyValues, returnNewIds, null);
		}

		public GroupOperationResult MoveItems(Folder destinationFolder, StoreObjectId[] sourceItemIds, PropertyDefinition[] propertyDefinitions, object[] propertyValues, bool returnNewIds, DeleteItemFlags? deleteFlags)
		{
			this.CheckDisposed("MoveItems");
			Util.ThrowOnNullArgument(destinationFolder, "destinationFolder");
			return this.CoreFolder.MoveItems(destinationFolder.CoreFolder, sourceItemIds, propertyDefinitions, propertyValues, delegate(StoreObjectId storeObjectId)
			{
				this.CheckItemBelongsToThisFolder(storeObjectId);
			}, returnNewIds, deleteFlags);
		}

		public bool IsContentAvailable()
		{
			this.CheckDisposed("IsContentAvailable");
			return this.CoreFolder.IsContentAvailable();
		}

		internal static bool CanDeleteDefaultFolder(DefaultFolderType defaultFolderType)
		{
			return ((IList)Folder.canDeleteDefaultFolders).Contains(defaultFolderType);
		}

		internal static byte[] GetFolderProviderLevelItemId(StoreObjectId sourceFolderId)
		{
			if (!IdConverter.IsFolderId(sourceFolderId))
			{
				throw new ArgumentException(ServerStrings.ExInvalidFolderId, "sourceFolderId");
			}
			if (sourceFolderId is OccurrenceStoreObjectId)
			{
				throw new ArgumentException(ServerStrings.ExCannotMoveOrCopyOccurrenceItem(sourceFolderId));
			}
			return sourceFolderId.ProviderLevelItemId;
		}

		internal static StoreObjectId[] StoreIdsToStoreObjectIds(StoreId[] storeIds)
		{
			StoreObjectId[] array = new StoreObjectId[storeIds.Length];
			for (int i = 0; i < storeIds.Length; i++)
			{
				array[i] = StoreId.GetStoreObjectId(storeIds[i]);
			}
			return array;
		}

		internal static DefaultFolderType[] CanDeleteDefaultFolders
		{
			get
			{
				return Folder.canDeleteDefaultFolders;
			}
			set
			{
				Folder.canDeleteDefaultFolders = value;
			}
		}

		protected virtual void CheckItemBelongsToThisFolder(StoreObjectId storeObjectId)
		{
			byte[] providerLevelItemId = base.Session.IdConverter.GetSessionSpecificId(base.StoreObjectId).ProviderLevelItemId;
			byte[] providerLevelItemId2 = base.Session.IdConverter.GetSessionSpecificId(base.Session.GetParentFolderId(storeObjectId)).ProviderLevelItemId;
			if (!ArrayComparer<byte>.Comparer.Equals(providerLevelItemId, providerLevelItemId2))
			{
				throw new ArgumentException(ServerStrings.ExItemDoesNotBelongToCurrentFolder(storeObjectId));
			}
		}

		protected bool ExtendedFlagsContains(ExtendedFolderFlags flag)
		{
			return Folder.ExtendedFlagsContains(base.GetValueAsNullable<int>(FolderSchema.ExtendedFolderFlags), flag);
		}

		public PublicFolderContentMailboxInfo GetContentMailboxInfo()
		{
			return this.CoreFolder.GetContentMailboxInfo();
		}

		private const int MaxDeleteItemCount = 200;

		public const int MaxRows = 10000;

		private static readonly StoreObjectType[] FolderTypes = new StoreObjectType[]
		{
			StoreObjectType.SearchFolder,
			StoreObjectType.OutlookSearchFolder,
			StoreObjectType.CalendarFolder,
			StoreObjectType.ContactsFolder,
			StoreObjectType.TasksFolder,
			StoreObjectType.JournalFolder,
			StoreObjectType.NotesFolder,
			StoreObjectType.ShortcutFolder,
			StoreObjectType.Folder
		};

		private static readonly QueryFilter DefaultClutterFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.IsClutter, true);

		private static readonly QueryFilter DefaultNoClutterFilter = new ComparisonFilter(ComparisonOperator.NotEqual, ItemSchema.IsClutter, true);

		private static DefaultFolderType[] canDeleteDefaultFolders = new DefaultFolderType[]
		{
			DefaultFolderType.ElcRoot,
			DefaultFolderType.UMVoicemail,
			DefaultFolderType.UMFax,
			DefaultFolderType.Drafts,
			DefaultFolderType.JunkEmail,
			DefaultFolderType.Tasks,
			DefaultFolderType.RecipientCache,
			DefaultFolderType.QuickContacts,
			DefaultFolderType.ImContactList,
			DefaultFolderType.OrganizationalContacts,
			DefaultFolderType.PushNotificationRoot,
			DefaultFolderType.BirthdayCalendar
		};

		private PermissionTable permissionTable;

		private delegate CoreFolder CoreFolderBindDelegate();

		internal delegate GroupOperationResult GroupOperationDelegate();

		private delegate GroupOperationResult ActOnItemsDelegate(StoreObjectId[] sourceItemIds);

		private delegate GroupOperationResult ActOnFolderDelegate(StoreObjectId sourceFolderId);
	}
}
