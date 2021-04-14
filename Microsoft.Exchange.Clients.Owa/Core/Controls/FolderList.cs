using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal class FolderList : IComparer<object[]>
	{
		public FolderList(UserContext userContext, MailboxSession mailboxSession, NavigationModule navigationModule, int maxFolders, bool isListDeletedFolders, PropertyDefinition sortProperty, PropertyDefinition[] extendedProperties)
		{
			string text;
			switch (navigationModule)
			{
			case NavigationModule.Mail:
				text = "IPF.Note";
				break;
			case NavigationModule.Calendar:
				text = "IPF.Appointment";
				break;
			case NavigationModule.Contacts:
				text = "IPF.Contact";
				break;
			case NavigationModule.Tasks:
				text = "IPF.Task";
				break;
			default:
				throw new ArgumentOutOfRangeException("navigationModule", "Module " + navigationModule);
			}
			this.Initialize(userContext, mailboxSession, userContext.GetRootFolderId(mailboxSession), new string[]
			{
				text
			}, null, maxFolders, isListDeletedFolders, true, false, null, sortProperty, extendedProperties);
		}

		public FolderList(UserContext userContext, MailboxSession mailboxSession, string[] folderTypes, int maxFolders, bool isListDeletedFolders, PropertyDefinition sortProperty, PropertyDefinition[] extendedProperties)
		{
			this.Initialize(userContext, mailboxSession, userContext.GetRootFolderId(mailboxSession), folderTypes, null, maxFolders, isListDeletedFolders, true, false, null, sortProperty, extendedProperties);
		}

		public FolderList(UserContext userContext, MailboxSession mailboxSession, StoreObjectId storeObjectId, QueryFilter queryFilter, int maxFolders, bool isListDeletedFolders, bool isDeepTraversal, bool isListRootFolder, PropertyDefinition sortProperty, PropertyDefinition[] extendedProperties)
		{
			this.Initialize(userContext, mailboxSession, storeObjectId, null, queryFilter, maxFolders, isListDeletedFolders, isDeepTraversal, isListRootFolder, null, sortProperty, extendedProperties);
		}

		public FolderList(UserContext userContext, MailboxSession mailboxSession, StoreObjectId rootFolderId, int maxFolders, bool isListDeletedFolders, bool isDeepTraversal, bool isListRootFolder, FolderList parentFolderList)
		{
			this.Initialize(userContext, mailboxSession, rootFolderId, null, null, maxFolders, isListDeletedFolders, isDeepTraversal, isListRootFolder, parentFolderList, null, null);
		}

		private static void CombinePropertyList(PropertyDefinition[] extendedProperties, out PropertyDefinition[] allProperties, out Dictionary<PropertyDefinition, int> allPropertyIndexes)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>(FolderList.defaultQueryProperties.Length + extendedProperties.Length);
			for (int i = 0; i < FolderList.defaultQueryProperties.Length; i++)
			{
				list.Add(FolderList.defaultQueryProperties[i]);
			}
			foreach (PropertyDefinition item in extendedProperties)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			allProperties = list.ToArray();
			allPropertyIndexes = Utilities.GetPropertyToIndexMap(allProperties);
		}

		private void Initialize(UserContext userContext, MailboxSession session, StoreObjectId rootFolderId, string[] folderTypes, QueryFilter queryFilter, int maxFolders, bool isListDeletedFolders, bool isDeepTraversal, bool isListRootFolder, FolderList parentFolderList, PropertyDefinition sortProperty, PropertyDefinition[] extendedProperties)
		{
			this.userContext = userContext;
			this.maxFolders = maxFolders;
			this.isListDeletedFolders = isListDeletedFolders;
			this.sortProperty = sortProperty;
			this.mailboxSession = session;
			this.rootFolderId = rootFolderId;
			this.isDeepTraversal = isDeepTraversal;
			this.queryFilter = queryFilter;
			this.folderTypes = folderTypes;
			this.isListRootFolder = isListRootFolder;
			if (parentFolderList != null)
			{
				this.queryProperties = parentFolderList.queryProperties;
				this.propertyIndexes = parentFolderList.propertyIndexes;
				this.parentFolderList = parentFolderList;
			}
			else if (extendedProperties != null)
			{
				if (extendedProperties.Equals(FolderList.FolderTreeQueryProperties))
				{
					this.queryProperties = FolderList.FolderTreeQueryProperties;
					this.propertyIndexes = FolderList.FolderTreePropertyIndexes;
				}
				else if (extendedProperties.Equals(FolderList.FolderPropertiesInBasic))
				{
					this.queryProperties = FolderList.FolderPropertiesInBasic;
					this.propertyIndexes = FolderList.FolderPropertyToIndexInBasic;
				}
				else
				{
					FolderList.CombinePropertyList(extendedProperties, out this.queryProperties, out this.propertyIndexes);
				}
			}
			this.Load();
		}

		public int Count
		{
			get
			{
				if (this.folders == null)
				{
					return 0;
				}
				return this.folders.Count;
			}
		}

		public MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		protected UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		internal Dictionary<PropertyDefinition, int> QueryPropertyMap
		{
			get
			{
				return this.propertyIndexes;
			}
		}

		internal PropertyDefinition[] QueryProperties
		{
			get
			{
				return this.queryProperties;
			}
		}

		private Dictionary<StoreObjectId, object[]> FolderMapping
		{
			get
			{
				if (this.folderMapping == null)
				{
					int num = this.propertyIndexes[FolderSchema.Id];
					this.folderMapping = new Dictionary<StoreObjectId, object[]>(this.folders.Count);
					foreach (object[] array in this.folders)
					{
						this.folderMapping[((VersionedId)array[num]).ObjectId] = array;
					}
				}
				return this.folderMapping;
			}
		}

		internal bool ContainsFolder(StoreObjectId folderId)
		{
			return this.FolderMapping.ContainsKey(folderId);
		}

		internal object[] GetFolderProperties(StoreObjectId folderId)
		{
			if (this.FolderMapping.ContainsKey(folderId))
			{
				return this.FolderMapping[folderId];
			}
			return null;
		}

		internal object GetFolderProperty(StoreObjectId folderId, PropertyDefinition propertyDefinition)
		{
			if (!this.propertyIndexes.ContainsKey(propertyDefinition))
			{
				throw new ArgumentException(string.Format("Doesn't contain this property: {0}", propertyDefinition.ToString()));
			}
			object[] folderProperties = this.GetFolderProperties(folderId);
			if (folderProperties == null)
			{
				return null;
			}
			return folderProperties[this.propertyIndexes[propertyDefinition]];
		}

		internal bool TryGetFolderProperty<T>(StoreObjectId folderId, PropertyDefinition propertyDefinition, out T value)
		{
			object folderProperty = this.GetFolderProperty(folderId, propertyDefinition);
			if (folderProperty != null && folderProperty is T)
			{
				value = (T)((object)folderProperty);
				return true;
			}
			value = default(T);
			return false;
		}

		internal StoreObjectId[] GetFolderIds()
		{
			if (this.folderIds == null)
			{
				int num = this.propertyIndexes[FolderSchema.Id];
				this.folderIds = new StoreObjectId[this.folders.Count];
				for (int i = 0; i < this.folders.Count; i++)
				{
					this.folderIds[i] = ((VersionedId)this.folders[i][num]).ObjectId;
				}
			}
			return this.folderIds;
		}

		public object GetPropertyValue(int index, PropertyDefinition propertyDefinition)
		{
			if (this.folders == null)
			{
				throw new OwaInvalidOperationException("The folder list hasn't been loaded!");
			}
			object[] array = this.folders[index];
			return array[this.propertyIndexes[propertyDefinition]];
		}

		public bool IsFolderDeleted(int index)
		{
			if (!this.isListDeletedFolders)
			{
				return false;
			}
			StoreObjectId objectId = ((VersionedId)this.GetPropertyValue(index, FolderSchema.Id)).ObjectId;
			return this.deletedFolderIds.Contains(objectId);
		}

		private void Load()
		{
			this.folders = new List<object[]>(50);
			Folder folder = null;
			object[][] array = null;
			try
			{
				if (this.parentFolderList == null)
				{
					if (this.userContext.IsWebPartRequest && this.userContext.IsExplicitLogon)
					{
						folder = Utilities.SafeFolderBind(this.mailboxSession, this.rootFolderId, this.queryProperties);
						if (folder == null)
						{
							return;
						}
					}
					else
					{
						folder = Folder.Bind(this.mailboxSession, this.rootFolderId, this.queryProperties);
					}
					this.deletedFolderIds = new List<StoreObjectId>(10);
					if (this.isListRootFolder)
					{
						this.folders.Add(folder.GetProperties(this.queryProperties));
					}
					using (QueryResult queryResult = folder.FolderQuery(this.isDeepTraversal ? FolderQueryFlags.DeepTraversal : FolderQueryFlags.None, (this.queryFilter == null) ? FolderList.NonHiddenFilter : new AndFilter(new QueryFilter[]
					{
						FolderList.NonHiddenFilter,
						this.queryFilter
					}), null, this.queryProperties))
					{
						array = Utilities.FetchRowsFromQueryResult(queryResult, 10000);
						goto IL_104;
					}
				}
				array = this.FetchRowsFromParentFolderList();
				IL_104:
				bool flag = false;
				int num = -1;
				int i = 0;
				while (i < array.Length)
				{
					bool flag2 = false;
					StoreObjectId storeObjectId = null;
					object[] array2 = array[i];
					if (!flag)
					{
						storeObjectId = ((VersionedId)array2[this.propertyIndexes[FolderSchema.Id]]).ObjectId;
						if (Utilities.IsDefaultFolderId(this.mailboxSession, storeObjectId, DefaultFolderType.DeletedItems))
						{
							num = (int)array2[this.propertyIndexes[FolderSchema.FolderHierarchyDepth]];
						}
						else if (num != -1)
						{
							int num2 = (int)array2[this.propertyIndexes[FolderSchema.FolderHierarchyDepth]];
							if (num2 == num)
							{
								flag = true;
								flag2 = false;
							}
							else
							{
								flag2 = true;
							}
						}
					}
					if (flag2)
					{
						if (this.isListDeletedFolders)
						{
							goto IL_1E1;
						}
					}
					else
					{
						if (this.folderTypes == null)
						{
							goto IL_1E1;
						}
						object obj = array2[this.propertyIndexes[StoreObjectSchema.ContainerClass]];
						if (!(obj is PropertyError))
						{
							string folderType = (string)obj;
							if (this.IsQueriedType(folderType))
							{
								goto IL_1E1;
							}
						}
					}
					IL_212:
					i++;
					continue;
					IL_1E1:
					if (this.folders.Count >= this.maxFolders)
					{
						break;
					}
					this.folders.Add(array2);
					if (flag2)
					{
						this.deletedFolderIds.Add(storeObjectId);
						goto IL_212;
					}
					goto IL_212;
				}
				if (this.sortProperty != null)
				{
					this.folders.Sort(this);
				}
			}
			finally
			{
				if (folder != null)
				{
					folder.Dispose();
				}
			}
		}

		public int Compare(object[] rowX, object[] rowY)
		{
			if (this.sortProperty == null)
			{
				return 0;
			}
			object obj = rowX[this.propertyIndexes[this.sortProperty]];
			object obj2 = rowY[this.propertyIndexes[this.sortProperty]];
			if (this.sortProperty == StoreObjectSchema.CreationTime)
			{
				ExDateTime dt = (ExDateTime)obj;
				ExDateTime dt2 = (ExDateTime)obj2;
				return ExDateTime.Compare(dt, dt2);
			}
			if (this.sortProperty == FolderSchema.Id || this.sortProperty == FolderSchema.FolderHierarchyDepth)
			{
				int num = (int)obj;
				int num2 = (int)obj2;
				return num - num2;
			}
			string strA = (string)obj;
			string strB = (string)obj2;
			return string.Compare(strA, strB, StringComparison.CurrentCulture);
		}

		private bool IsQueriedType(string folderType)
		{
			if (this.folderTypes == null)
			{
				return true;
			}
			for (int i = 0; i < this.folderTypes.Length; i++)
			{
				if (ObjectClass.IsOfClass(folderType, this.folderTypes[i]))
				{
					return true;
				}
			}
			return false;
		}

		private object[][] FetchRowsFromParentFolderList()
		{
			HashSet<StoreObjectId> hashSet = new HashSet<StoreObjectId>();
			List<object[]> list = new List<object[]>();
			int num = 0;
			bool flag = false;
			hashSet.Add(this.rootFolderId);
			foreach (object[] array in this.parentFolderList.folders)
			{
				StoreObjectId objectId = ((VersionedId)array[this.propertyIndexes[FolderSchema.Id]]).ObjectId;
				StoreObjectId item = (StoreObjectId)array[this.propertyIndexes[StoreObjectSchema.ParentItemId]];
				if (this.rootFolderId.Equals(objectId))
				{
					num = (int)array[this.propertyIndexes[FolderSchema.FolderHierarchyDepth]];
					if (this.isListRootFolder && !flag)
					{
						object[] array2 = new object[this.queryProperties.Length];
						array.CopyTo(array2, 0);
						array2[this.propertyIndexes[FolderSchema.FolderHierarchyDepth]] = 0;
						this.folders.Add(array2);
						flag = true;
					}
				}
				if (hashSet.Contains(item))
				{
					int num2 = (int)array[this.propertyIndexes[FolderSchema.FolderHierarchyDepth]];
					object[] array2 = new object[this.queryProperties.Length];
					array.CopyTo(array2, 0);
					array2[this.propertyIndexes[FolderSchema.FolderHierarchyDepth]] = num2 - num;
					list.Add(array2);
					if (this.isDeepTraversal && !hashSet.Contains(objectId))
					{
						hashSet.Add(objectId);
					}
				}
			}
			if (list != null)
			{
				return list.ToArray();
			}
			return new object[0][];
		}

		private static readonly PropertyDefinition[] defaultQueryProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.DisplayName,
			StoreObjectSchema.ContainerClass,
			StoreObjectSchema.CreationTime,
			FolderSchema.FolderHierarchyDepth
		};

		internal static readonly PropertyDefinition[] FolderPropertiesInBasic = new PropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.DisplayName,
			StoreObjectSchema.ContainerClass,
			FolderSchema.UnreadCount,
			FolderSchema.ItemCount,
			FolderSchema.ExtendedFolderFlags,
			StoreObjectSchema.CreationTime,
			FolderSchema.FolderHierarchyDepth
		};

		internal static readonly PropertyDefinition[] FolderTreeQueryProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			StoreObjectSchema.ParentEntryId,
			StoreObjectSchema.ParentItemId,
			StoreObjectSchema.RecordKey,
			FolderSchema.SearchFolderAllowAgeout,
			FolderSchema.DisplayName,
			StoreObjectSchema.CreationTime,
			StoreObjectSchema.LastModifiedTime,
			FolderSchema.UnreadCount,
			FolderSchema.ItemCount,
			StoreObjectSchema.ContainerClass,
			FolderSchema.FolderHierarchyDepth,
			FolderSchema.HasChildren,
			FolderSchema.ExtendedFolderFlags,
			FolderSchema.IsOutlookSearchFolder,
			FolderSchema.OutlookSearchFolderClsId,
			ViewStateProperties.FilteredViewLabel,
			ViewStateProperties.FilteredViewAccessTime,
			ViewStateProperties.TreeNodeCollapseStatus,
			FolderSchema.AdminFolderFlags,
			FolderSchema.ELCPolicyIds,
			StoreObjectSchema.EffectiveRights
		};

		internal static readonly Dictionary<PropertyDefinition, int> FolderPropertyToIndexInBasic = Utilities.GetPropertyToIndexMap(FolderList.FolderPropertiesInBasic);

		internal static readonly Dictionary<PropertyDefinition, int> FolderTreePropertyIndexes = Utilities.GetPropertyToIndexMap(FolderList.FolderTreeQueryProperties);

		private static readonly Dictionary<PropertyDefinition, int> defaultPropertyIndexes = Utilities.GetPropertyToIndexMap(FolderList.defaultQueryProperties);

		private UserContext userContext;

		private MailboxSession mailboxSession;

		private StoreObjectId rootFolderId;

		private string[] folderTypes;

		private int maxFolders;

		private bool isListDeletedFolders;

		private bool isListRootFolder;

		private bool isDeepTraversal;

		private PropertyDefinition sortProperty;

		private QueryFilter queryFilter;

		private PropertyDefinition[] queryProperties = FolderList.defaultQueryProperties;

		private Dictionary<PropertyDefinition, int> propertyIndexes = FolderList.defaultPropertyIndexes;

		private Dictionary<StoreObjectId, object[]> folderMapping;

		private StoreObjectId[] folderIds;

		private FolderList parentFolderList;

		private List<object[]> folders;

		private List<StoreObjectId> deletedFolderIds;

		protected static readonly QueryFilter NonHiddenFilter = new ComparisonFilter(ComparisonOperator.NotEqual, FolderSchema.IsHidden, true);
	}
}
