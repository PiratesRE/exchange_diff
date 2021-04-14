using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class FolderHierarchy : IStateObject
	{
		private FolderHierarchy(FolderInformationComparer folderInformationComparer, IDictionary<ExchangeShortId, FolderHierarchy.FolderInformationImpl> hierarchyFolders, List<IFolderInformation> hierarchyRoots, FolderInformationType informationType)
		{
			this.folderInformationComparer = folderInformationComparer;
			this.hierarchyFolders = hierarchyFolders;
			this.hierarchyRoots = hierarchyRoots;
			this.informationType = informationType;
			this.isValid = true;
		}

		public bool IsEmpty
		{
			get
			{
				return this.hierarchyRoots.Count == 0;
			}
		}

		public IList<IFolderInformation> HierarchyRoots
		{
			get
			{
				return this.hierarchyRoots;
			}
		}

		public int TotalFolderCount
		{
			get
			{
				return this.hierarchyFolders.Count;
			}
		}

		public static void Initialize()
		{
			if (FolderHierarchy.folderHierarchyDataSlot == -1)
			{
				FolderHierarchy.folderHierarchyDataSlot = MailboxState.AllocateComponentDataSlot(false);
			}
		}

		public static FolderHierarchy GetFolderHierarchy(Context context, Mailbox mailbox, ExchangeShortId rootFolderId, FolderInformationType requestedInformationType)
		{
			FolderHierarchy folderHierarchy = FolderHierarchy.GetFolderHierarchyNoCreate(context, mailbox);
			if (folderHierarchy == null || (folderHierarchy.informationType != requestedInformationType && requestedInformationType == FolderInformationType.Extended))
			{
				folderHierarchy = FolderHierarchy.CreateFolderHierarchy(context, mailbox, requestedInformationType);
				if (context.TransactionStarted)
				{
					context.RegisterStateObject(folderHierarchy);
				}
				mailbox.SharedState.SetComponentData(FolderHierarchy.folderHierarchyDataSlot, folderHierarchy);
			}
			if (rootFolderId.IsZero)
			{
				return folderHierarchy;
			}
			FolderHierarchy.FolderInformationImpl folderInformationImpl;
			if (!folderHierarchy.hierarchyFolders.TryGetValue(rootFolderId, out folderInformationImpl))
			{
				return FolderHierarchy.emptyHierarchy;
			}
			if (folderInformationImpl.Parent != null)
			{
				folderHierarchy.PopulateChildren(context, folderInformationImpl.Parent);
			}
			List<IFolderInformation> list = new List<IFolderInformation>(1);
			list.Add(folderInformationImpl);
			return new FolderHierarchy(folderHierarchy.folderInformationComparer, folderHierarchy.hierarchyFolders, list, folderHierarchy.informationType);
		}

		public static FolderHierarchy GetFolderHierarchyNoCreate(Context context, Mailbox mailbox)
		{
			FolderHierarchy folderHierarchy = (FolderHierarchy)mailbox.SharedState.GetComponentData(FolderHierarchy.folderHierarchyDataSlot);
			if (folderHierarchy != null && !folderHierarchy.isValid)
			{
				folderHierarchy = null;
			}
			return folderHierarchy;
		}

		public static FolderHierarchy CreateFolderHierarchyForTest(Tuple<ExchangeShortId, ExchangeShortId, string>[] folders, CultureInfo culture, bool sortChildren)
		{
			FolderInformationComparer comparer = new FolderInformationComparer(culture.CompareInfo);
			List<IFolderInformation> list = new List<IFolderInformation>();
			Dictionary<ExchangeShortId, FolderHierarchy.FolderInformationImpl> dictionary = new Dictionary<ExchangeShortId, FolderHierarchy.FolderInformationImpl>();
			foreach (Tuple<ExchangeShortId, ExchangeShortId, string> tuple in folders)
			{
				ExchangeShortId item = tuple.Item1;
				ExchangeShortId item2 = tuple.Item2;
				string item3 = tuple.Item3;
				FolderHierarchy.FolderInformationImpl folderInformationImpl = null;
				if (!item2.IsZero)
				{
					folderInformationImpl = dictionary[item2];
				}
				FolderHierarchy.FolderInformationImpl folderInformationImpl2 = new FolderHierarchy.FolderInformationImpl(item, folderInformationImpl, FolderHierarchy.FolderInformationImpl.FolderInformationFlags.None, 0, item3, 0L, null);
				folderInformationImpl2.ChildrenArePopulated = true;
				dictionary.Add(item, folderInformationImpl2);
				if (folderInformationImpl != null)
				{
					folderInformationImpl.LinkChild(folderInformationImpl2, comparer);
				}
			}
			foreach (FolderHierarchy.FolderInformationImpl folderInformationImpl3 in dictionary.Values)
			{
				if (sortChildren)
				{
					folderInformationImpl3.SortChildrenAndCompact(comparer);
				}
				if (folderInformationImpl3.Parent == null)
				{
					list.Add(folderInformationImpl3);
				}
			}
			list.Sort(comparer);
			return new FolderHierarchy(comparer, dictionary, list, FolderInformationType.Basic);
		}

		public static void SortFolderInformationChildrenAndCompactForTest(Context context, FolderHierarchy folderHierarchy, IFolderInformation folderInformation, FolderInformationComparer folderInformationComparer)
		{
			folderHierarchy.PopulateChildren(context, (FolderHierarchy.FolderInformationImpl)folderInformation);
			((FolderHierarchy.FolderInformationImpl)folderInformation).SortChildrenAndCompact(folderInformationComparer);
		}

		public static IFolderInformation CreateFolderInformationForTest(ExchangeShortId fid, IFolderInformation parentInformation, int mailboxNumber, string displayName, long messageCount, SecurityDescriptor securityDescriptor)
		{
			return new FolderHierarchy.FolderInformationImpl(fid, (FolderHierarchy.FolderInformationImpl)parentInformation, FolderHierarchy.FolderInformationImpl.FolderInformationFlags.None, mailboxNumber, displayName, messageCount, securityDescriptor);
		}

		public static void LinkChildForTest(IFolderInformation parentInformation, IFolderInformation folderInformationToLink, FolderInformationComparer folderInformationComparer)
		{
			((FolderHierarchy.FolderInformationImpl)parentInformation).LinkChild((FolderHierarchy.FolderInformationImpl)folderInformationToLink, folderInformationComparer);
		}

		public static void UnlinkChildForTest(IFolderInformation parentInformation, IFolderInformation folderInformationToLink, FolderInformationComparer folderInformationComparer)
		{
			((FolderHierarchy.FolderInformationImpl)parentInformation).UnlinkChild((FolderHierarchy.FolderInformationImpl)folderInformationToLink, folderInformationComparer, false);
		}

		public static void ChangeFolderInformationPropertiesForTest(IFolderInformation folderInformation, string newDisplayName, long newMessageCount, FolderInformationComparer folderInformationComparer)
		{
			((FolderHierarchy.FolderInformationImpl)folderInformation).ChangeDisplayName(newDisplayName, folderInformationComparer);
			((FolderHierarchy.FolderInformationImpl)folderInformation).MessageCount = newMessageCount;
		}

		public static IDisposable SetMaxChildrenForCompactionForTest(int maxChildrenForCompaction)
		{
			return FolderHierarchy.maxChildrenForCompaction.SetTestHook(maxChildrenForCompaction);
		}

		public static void OnFolderCreated(Context context, Folder folder)
		{
			FolderHierarchy folderHierarchyNoCreate = FolderHierarchy.GetFolderHierarchyNoCreate(context, folder.Mailbox);
			if (folderHierarchyNoCreate == null)
			{
				return;
			}
			if (!context.IsStateObjectRegistered(folderHierarchyNoCreate))
			{
				context.RegisterStateObject(folderHierarchyNoCreate);
			}
			FolderTable folderTable = DatabaseSchema.FolderTable(context.Database);
			ExchangeShortId exchangeShortId = folder.GetId(context).ToExchangeShortId();
			ExchangeShortId key = folder.GetParentFolderId(context).ToExchangeShortId();
			FolderHierarchy.FolderInformationImpl folderInformationImpl = null;
			if (!key.IsZero)
			{
				folderInformationImpl = folderHierarchyNoCreate.hierarchyFolders[key];
			}
			FolderHierarchy.FolderInformationImpl.FolderInformationFlags folderInformationFlags = FolderHierarchy.FolderInformationImpl.FolderInformationFlags.None;
			int mailboxNumber = 0;
			string displayName = null;
			long messageCount = 0L;
			SecurityDescriptor securityDescriptor = null;
			if (folderInformationImpl == null || folderInformationImpl.ChildrenArePopulated)
			{
				if (folder.IsSearchFolder(context))
				{
					folderInformationFlags |= FolderHierarchy.FolderInformationImpl.FolderInformationFlags.IsSearchFolder;
				}
				if ((bool)folder.GetPropertyValue(context, PropTag.Folder.PartOfContentIndexing))
				{
					folderInformationFlags |= FolderHierarchy.FolderInformationImpl.FolderInformationFlags.IsPartOfContentIndexing;
				}
				if (folder.IsInternalAccess(context))
				{
					folderInformationFlags |= FolderHierarchy.FolderInformationImpl.FolderInformationFlags.InternalAccess;
				}
				mailboxNumber = (int)folder.GetPropertyValue(context, PropTag.Folder.MailboxNum);
				displayName = (((string)folder.GetColumnValue(context, folderTable.DisplayName)) ?? string.Empty);
				messageCount = folder.GetMessageCount(context);
				if (folderHierarchyNoCreate.informationType == FolderInformationType.Extended)
				{
					byte[] buffer = (byte[])folder.GetColumnValue(context, folderTable.AclTableAndSecurityDescriptor);
					FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = AclTableHelper.Parse(context, buffer);
					securityDescriptor = aclTableAndSecurityDescriptorProperty.SecurityDescriptor;
				}
			}
			FolderHierarchy.FolderInformationImpl folderInformationImpl2 = new FolderHierarchy.FolderInformationImpl(exchangeShortId, folderInformationImpl, folderInformationFlags, mailboxNumber, displayName, messageCount, securityDescriptor);
			if (folderInformationImpl != null)
			{
				folderInformationImpl.LinkChild(folderInformationImpl2, folderHierarchyNoCreate.folderInformationComparer);
			}
			folderHierarchyNoCreate.hierarchyFolders.Add(exchangeShortId, folderInformationImpl2);
			if (key.IsZero)
			{
				folderHierarchyNoCreate.hierarchyRoots.Add(folderInformationImpl2);
				folderHierarchyNoCreate.hierarchyRoots.Sort(folderHierarchyNoCreate.folderInformationComparer);
			}
		}

		public static void OnFolderDeleted(Context context, Folder folder, ExchangeShortId parentFid)
		{
			FolderHierarchy folderHierarchyNoCreate = FolderHierarchy.GetFolderHierarchyNoCreate(context, folder.Mailbox);
			if (folderHierarchyNoCreate == null)
			{
				return;
			}
			if (!context.IsStateObjectRegistered(folderHierarchyNoCreate))
			{
				context.RegisterStateObject(folderHierarchyNoCreate);
			}
			ExchangeShortId fid = folder.GetId(context).ToExchangeShortId();
			if (!parentFid.IsZero)
			{
				FolderHierarchy.FolderInformationImpl folderInformationImpl = folderHierarchyNoCreate.hierarchyFolders[parentFid];
				FolderHierarchy.FolderInformationImpl child = folderHierarchyNoCreate.hierarchyFolders[fid];
				folderInformationImpl.UnlinkChild(child, folderHierarchyNoCreate.folderInformationComparer, false);
			}
			else
			{
				int index = folderHierarchyNoCreate.hierarchyRoots.FindIndex((IFolderInformation fi) => fi.Fid == fid);
				folderHierarchyNoCreate.hierarchyRoots.RemoveAt(index);
			}
			folderHierarchyNoCreate.hierarchyFolders.Remove(fid);
		}

		public static void OnFolderMoved(Context context, Folder folder, ExchangeShortId originalParentFid)
		{
			FolderHierarchy folderHierarchyNoCreate = FolderHierarchy.GetFolderHierarchyNoCreate(context, folder.Mailbox);
			if (folderHierarchyNoCreate == null)
			{
				return;
			}
			if (!context.IsStateObjectRegistered(folderHierarchyNoCreate))
			{
				context.RegisterStateObject(folderHierarchyNoCreate);
			}
			ExchangeShortId key = folder.GetId(context).ToExchangeShortId();
			FolderHierarchy.FolderInformationImpl folderInformationImpl = folderHierarchyNoCreate.hierarchyFolders[key];
			IList<IFolderInformation> children = folderInformationImpl.Children;
			bool childrenArePopulated = folderInformationImpl.ChildrenArePopulated;
			FolderHierarchy.OnFolderDeleted(context, folder, originalParentFid);
			FolderHierarchy.OnFolderCreated(context, folder);
			FolderHierarchy.FolderInformationImpl folderInformationImpl2 = folderHierarchyNoCreate.hierarchyFolders[key];
			folderInformationImpl2.SetChildren(children);
			folderInformationImpl2.ChildrenArePopulated = childrenArePopulated;
		}

		public static void OnFolderChanged(Context context, Folder folder)
		{
			FolderHierarchy folderHierarchyNoCreate = FolderHierarchy.GetFolderHierarchyNoCreate(context, folder.Mailbox);
			if (folderHierarchyNoCreate == null)
			{
				return;
			}
			FolderHierarchy.FolderInformationImpl folderInformationImpl = folderHierarchyNoCreate.hierarchyFolders[folder.GetId(context).ToExchangeShortId()];
			if (folderInformationImpl.Parent == null || folderInformationImpl.Parent.ChildrenArePopulated)
			{
				if (!context.IsStateObjectRegistered(folderHierarchyNoCreate))
				{
					context.RegisterStateObject(folderHierarchyNoCreate);
				}
				FolderTable folderTable = DatabaseSchema.FolderTable(context.Database);
				if (folder.DataRow.ColumnDirty(folderTable.MessageCount))
				{
					folderInformationImpl.MessageCount = (long)folder.GetColumnValue(context, folderTable.MessageCount);
				}
				if (folder.DataRow.ColumnDirty(folderTable.DisplayName))
				{
					folderInformationImpl.ChangeDisplayName(((string)folder.GetColumnValue(context, folderTable.DisplayName)) ?? string.Empty, folderHierarchyNoCreate.folderInformationComparer);
					if (folderInformationImpl.Parent == null)
					{
						folderHierarchyNoCreate.hierarchyRoots.Sort(folderHierarchyNoCreate.folderInformationComparer);
					}
				}
				if (folder.Mailbox.SharedState.UnifiedState != null && folder.DataRow.ColumnDirty(folderTable.MailboxNumber))
				{
					int mailboxNumber = (int)folder.GetColumnValue(context, folderTable.MailboxNumber);
					folderInformationImpl.MailboxNumber = mailboxNumber;
				}
				if (folderHierarchyNoCreate.informationType == FolderInformationType.Extended && folder.DataRow.ColumnDirty(folderTable.AclTableAndSecurityDescriptor))
				{
					byte[] buffer = (byte[])folder.GetColumnValue(context, folderTable.AclTableAndSecurityDescriptor);
					FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = AclTableHelper.Parse(context, buffer);
					folderInformationImpl.SecurityDescriptor = aclTableAndSecurityDescriptorProperty.SecurityDescriptor;
				}
				folderInformationImpl.IsPartOfContentIndexing = folder.IsPartOfContentIndexing(context);
			}
		}

		public static void DiscardFolderHierarchyCache(Context context, MailboxState mailboxState)
		{
			mailboxState.SetComponentData(FolderHierarchy.folderHierarchyDataSlot, null);
		}

		public static IFolderInformation FolderInformationFromFolderId(ExchangeShortId folderId)
		{
			return new FolderHierarchy.FolderInformationImpl(folderId);
		}

		public void OnBeforeCommit(Context context)
		{
		}

		public void OnCommit(Context context)
		{
		}

		public void OnAbort(Context context)
		{
			this.isValid = false;
		}

		public IFolderInformation Find(Context context, ExchangeShortId fid)
		{
			int num;
			return this.Find(context, fid, out num);
		}

		public IFolderInformation GetParent(Context context, IFolderInformation child)
		{
			FolderHierarchy.FolderInformationImpl folderInformationImpl = (FolderHierarchy.FolderInformationImpl)child;
			if (folderInformationImpl.Parent != null && folderInformationImpl.Parent.Parent != null)
			{
				this.PopulateChildren(context, folderInformationImpl.Parent.Parent);
			}
			return folderInformationImpl.Parent;
		}

		public IList<IFolderInformation> GetChildren(Context context, IFolderInformation parent)
		{
			return this.GetSortedChildrenList(context, (FolderHierarchy.FolderInformationImpl)parent, null);
		}

		public IFolderInformation Find(Context context, ExchangeShortId fid, out int depth)
		{
			depth = 0;
			FolderHierarchy.FolderInformationImpl folderInformationImpl;
			if (!this.hierarchyFolders.TryGetValue(fid, out folderInformationImpl))
			{
				return null;
			}
			if (folderInformationImpl.Parent != null)
			{
				this.PopulateChildren(context, folderInformationImpl.Parent);
			}
			for (FolderHierarchy.FolderInformationImpl folderInformationImpl2 = folderInformationImpl; folderInformationImpl2 != null; folderInformationImpl2 = folderInformationImpl2.Parent)
			{
				if (this.hierarchyRoots.Contains(folderInformationImpl2))
				{
					return folderInformationImpl;
				}
				depth++;
			}
			return null;
		}

		public IFolderInformation FindByName(Context context, ExchangeShortId parentFid, string displayName, CompareInfo compareInfo)
		{
			int num;
			return this.FindByName(context, parentFid, displayName, compareInfo, false, out num);
		}

		public IFolderInformation FindByName(Context context, ExchangeShortId parentFid, string displayName, CompareInfo compareInfo, bool alwaysBSearch, out int childIndex)
		{
			bool flag = this.folderInformationComparer.CompareInfo == compareInfo;
			IList<IFolderInformation> sortedChildrenList;
			bool flag2;
			if (parentFid.IsZero)
			{
				sortedChildrenList = this.hierarchyRoots;
				flag2 = flag;
			}
			else
			{
				FolderHierarchy.FolderInformationImpl folderInformationImpl;
				if (!this.hierarchyFolders.TryGetValue(parentFid, out folderInformationImpl))
				{
					childIndex = -1;
					return null;
				}
				this.PopulateChildren(context, folderInformationImpl);
				sortedChildrenList = this.GetSortedChildrenList(context, folderInformationImpl, null);
				flag2 = (flag && folderInformationImpl.IsSorted);
			}
			if (sortedChildrenList == null || sortedChildrenList.Count == 0)
			{
				childIndex = -1;
				return null;
			}
			if (flag2 && (alwaysBSearch || sortedChildrenList.Count > 4))
			{
				FolderHierarchy.FolderInformationImpl folderInformationImpl2 = new FolderHierarchy.FolderInformationImpl(ExchangeShortId.Zero, null, FolderHierarchy.FolderInformationImpl.FolderInformationFlags.None, 0, displayName, 0L, null);
				if (sortedChildrenList is List<IFolderInformation>)
				{
					List<IFolderInformation> list = (List<IFolderInformation>)sortedChildrenList;
					childIndex = list.BinarySearch(folderInformationImpl2, this.folderInformationComparer);
				}
				else
				{
					IFolderInformation[] array = (IFolderInformation[])sortedChildrenList;
					childIndex = Array.BinarySearch<IFolderInformation>(array, folderInformationImpl2, this.folderInformationComparer);
				}
				if (childIndex < 0)
				{
					childIndex = ~childIndex;
				}
				if (childIndex < sortedChildrenList.Count)
				{
					IFolderInformation folderInformation = sortedChildrenList[childIndex];
					if (compareInfo.Compare(folderInformation.DisplayName, displayName, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
					{
						return folderInformation;
					}
				}
			}
			else
			{
				for (int i = 0; i < sortedChildrenList.Count; i++)
				{
					IFolderInformation folderInformation2 = sortedChildrenList[i];
					if (compareInfo.Compare(folderInformation2.DisplayName, displayName, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
					{
						childIndex = i;
						return folderInformation2;
					}
				}
				childIndex = -1;
			}
			return null;
		}

		public long GetFolderHierarchyDepth(ExchangeShortId folderId)
		{
			long num = 0L;
			FolderHierarchy.FolderInformationImpl parent;
			if (this.hierarchyFolders.TryGetValue(folderId, out parent))
			{
				num = 1L;
				while (parent.Parent != null && !parent.Parent.Fid.IsZero)
				{
					parent = parent.Parent;
					num += 1L;
				}
			}
			return num;
		}

		public void ForEachFolderInformation(Context context, Action<IFolderInformation> action)
		{
			foreach (KeyValuePair<ExchangeShortId, FolderHierarchy.FolderInformationImpl> keyValuePair in this.hierarchyFolders)
			{
				if (keyValuePair.Value.Parent != null)
				{
					this.PopulateChildren(context, keyValuePair.Value.Parent);
				}
				action(keyValuePair.Value);
			}
		}

		public IEnumerable<FolderHierarchyBlob> SerializeRecursiveHierarchyBlob(Context context, Mailbox mailbox, IFolderInformation root, bool recursive, Func<Context, IFolderInformation, bool> isVisiblePredicate, ExchangeShortId startFolderId, int startFolderSortPosition, bool startFolderInclusive)
		{
			IReplidGuidMap replidGuidMap = mailbox.ReplidGuidMap;
			FolderInformationComparer alternateComparer = null;
			if (this.folderInformationComparer.CompareInfo != context.Culture.CompareInfo)
			{
				alternateComparer = new FolderInformationComparer(context.Culture.CompareInfo);
			}
			int sortPosition;
			Stack<FolderHierarchy.FolderHierarchyEnumeratorStackEntry> enumeratorStack = this.InitEnumeratorStack(context, (FolderHierarchy.FolderInformationImpl)root, recursive, isVisiblePredicate, startFolderId, startFolderSortPosition, startFolderInclusive, alternateComparer, out sortPosition);
			ExchangeId lastParentId = ExchangeId.Zero;
			ExchangeShortId lastParentShortId = ExchangeShortId.Zero;
			while (enumeratorStack.Count != 0)
			{
				FolderHierarchy.FolderHierarchyEnumeratorStackEntry entry = this.PopFromEnumeratorStack(context, enumeratorStack, recursive, isVisiblePredicate, alternateComparer);
				FolderHierarchy.FolderInformationImpl folderInfo = (FolderHierarchy.FolderInformationImpl)entry.List[entry.Position];
				ExchangeShortId parentFolderShortId = (folderInfo.Parent == null) ? ExchangeShortId.Zero : folderInfo.Parent.Fid;
				if (parentFolderShortId != lastParentShortId)
				{
					lastParentShortId = parentFolderShortId;
					lastParentId = ExchangeId.CreateFromInternalShortId(context, replidGuidMap, parentFolderShortId);
				}
				int mailboxPartitionNumber = mailbox.MailboxPartitionNumber;
				int mailboxNumber = mailbox.MailboxNumber;
				byte[] parentFolderId = lastParentId.To26ByteArray();
				byte[] folderId = ExchangeId.CreateFromInternalShortId(context, replidGuidMap, folderInfo.Fid).To26ByteArray();
				string displayName = folderInfo.DisplayName;
				int depth = entry.Depth;
				int sortPosition2;
				sortPosition = (sortPosition2 = sortPosition) + 1;
				yield return new FolderHierarchyBlob(mailboxPartitionNumber, mailboxNumber, parentFolderId, folderId, displayName, depth, sortPosition2);
			}
			yield break;
		}

		private static FolderHierarchy CreateFolderHierarchy(Context context, Mailbox mailbox, FolderInformationType informationType)
		{
			if (context.PerfInstance != null)
			{
				context.PerfInstance.FolderHierarchyLoadRecursiveRate.Increment();
			}
			bool flag = false;
			FolderHierarchy result;
			try
			{
				if (!context.TransactionStarted)
				{
					context.BeginTransactionIfNeeded();
					flag = true;
				}
				ReplidGuidMap replidGuidMap = mailbox.ReplidGuidMap;
				FolderTable folderTable = DatabaseSchema.FolderTable(mailbox.Database);
				List<IFolderInformation> list = new List<IFolderInformation>(5);
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					mailbox.MailboxPartitionNumber
				});
				Column[] columnsToFetch = new Column[]
				{
					folderTable.FolderId,
					folderTable.ParentFolderId
				};
				Dictionary<ExchangeShortId, FolderHierarchy.FolderInformationImpl> dictionary;
				using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, folderTable.Table, folderTable.FolderByParentIndex, columnsToFetch, null, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, false, true))
				{
					using (CountOperator countOperator = Factory.CreateCountOperator(context.Culture, context, tableOperator, false))
					{
						dictionary = new Dictionary<ExchangeShortId, FolderHierarchy.FolderInformationImpl>((int)countOperator.ExecuteScalar());
					}
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						while (reader.Read())
						{
							ExchangeId exchangeId = ExchangeId.CreateFrom26ByteArray(context, replidGuidMap, reader.GetBinary(folderTable.FolderId));
							FolderHierarchy.FolderInformationImpl parentInformation = null;
							ExchangeId exchangeId2 = ExchangeId.CreateFrom26ByteArray(context, replidGuidMap, reader.GetBinary(folderTable.ParentFolderId));
							if (exchangeId2.IsValid)
							{
								parentInformation = new FolderHierarchy.FolderInformationImpl(exchangeId2.ToExchangeShortId());
							}
							FolderHierarchy.FolderInformationImpl folderInformationImpl = new FolderHierarchy.FolderInformationImpl(exchangeId.ToExchangeShortId(), parentInformation, FolderHierarchy.FolderInformationImpl.FolderInformationFlags.None, 0, null, 0L, null);
							dictionary.Add(folderInformationImpl.Fid, folderInformationImpl);
						}
					}
				}
				FolderInformationComparer comparer = new FolderInformationComparer(context.Culture.CompareInfo);
				foreach (KeyValuePair<ExchangeShortId, FolderHierarchy.FolderInformationImpl> keyValuePair in dictionary)
				{
					FolderHierarchy.FolderInformationImpl folderInformationImpl2 = null;
					if (keyValuePair.Value.Parent != null)
					{
						dictionary.TryGetValue(keyValuePair.Value.Parent.Fid, out folderInformationImpl2);
					}
					if (folderInformationImpl2 != null)
					{
						folderInformationImpl2.LinkChild(keyValuePair.Value, comparer);
					}
					else
					{
						list.Add(keyValuePair.Value);
					}
				}
				foreach (KeyValuePair<ExchangeShortId, FolderHierarchy.FolderInformationImpl> keyValuePair2 in dictionary)
				{
					FolderHierarchy.FolderInformationImpl value = keyValuePair2.Value;
					if (value.Children.Count <= FolderHierarchy.maxChildrenForCompaction.Value && value.Children is List<IFolderInformation>)
					{
						value.Children = value.Children.ToArray<IFolderInformation>();
					}
				}
				FolderHierarchy folderHierarchy = new FolderHierarchy(comparer, dictionary, list, informationType);
				folderHierarchy.PopulateChildren(context, null);
				if (list.Count > 1)
				{
					list.Sort(comparer);
				}
				result = folderHierarchy;
			}
			finally
			{
				if (flag)
				{
					context.Commit();
				}
			}
			return result;
		}

		private void PopulateChildren(Context context, FolderHierarchy.FolderInformationImpl parent)
		{
			if (parent != null && parent.ChildrenArePopulated)
			{
				return;
			}
			IReplidGuidMap cacheForMailbox = ReplidGuidMap.GetCacheForMailbox(context, context.LockedMailboxState);
			object lockObject = (parent != null) ? parent : this.hierarchyRoots;
			using (LockManager.Lock(lockObject, LockManager.LockType.LeafMonitorLock))
			{
				if (parent == null || !parent.ChildrenArePopulated)
				{
					IList<IFolderInformation> list = (parent != null) ? parent.Children : this.hierarchyRoots;
					if (list != null && list.Count != 0)
					{
						bool flag = false;
						try
						{
							if (!context.TransactionStarted)
							{
								context.BeginTransactionIfNeeded();
								flag = true;
							}
							FolderTable folderTable = DatabaseSchema.FolderTable(context.Database);
							FolderHierarchy.FolderInformationImpl.FolderColumnsInformation folderColumnsInformation = new FolderHierarchy.FolderInformationImpl.FolderColumnsInformation
							{
								Table = folderTable,
								PartOfContentIndexingColumn = PropertySchema.MapToColumn(context.Database, ObjectType.Folder, PropTag.Folder.PartOfContentIndexing),
								MailboxNumberColumn = PropertySchema.MapToColumn(context.Database, ObjectType.Folder, PropTag.Folder.MailboxNum),
								InternalAccessColumn = PropertySchema.MapToColumn(context.Database, ObjectType.Folder, PropTag.Folder.InternalAccess)
							};
							IList<Column> columnsToFetch = FolderHierarchy.FolderInformationImpl.GetColumnsToFetch(folderColumnsInformation, this.informationType);
							KeyRange[] array = new KeyRange[list.Count];
							for (int i = 0; i < list.Count; i++)
							{
								StartStopKey startStopKey = new StartStopKey(true, new object[]
								{
									context.LockedMailboxState.MailboxPartitionNumber,
									ExchangeId.CreateFromInternalShortId(context, cacheForMailbox, list[i].Fid).To26ByteArray()
								});
								array[i] = new KeyRange(startStopKey, startStopKey);
							}
							using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, folderTable.Table, folderTable.Table.PrimaryKeyIndex, columnsToFetch, null, null, 0, 0, array, false, true))
							{
								using (Reader reader = tableOperator.ExecuteReader(false))
								{
									while (reader.Read())
									{
										ExchangeId exchangeId = ExchangeId.CreateFrom26ByteArray(context, null, reader.GetBinary(folderColumnsInformation.Table.FolderId));
										this.hierarchyFolders[exchangeId.ToExchangeShortId()].Populate(context, folderColumnsInformation, this.informationType, reader);
									}
								}
							}
						}
						finally
						{
							if (flag)
							{
								context.Commit();
							}
						}
					}
					if (parent != null)
					{
						parent.ChildrenArePopulated = true;
					}
				}
			}
		}

		internal static FolderHierarchy FolderHierarchySnapshotFromDisk(Context context, Mailbox mailbox, FolderInformationType informationType)
		{
			return FolderHierarchy.CreateFolderHierarchy(context, mailbox, informationType);
		}

		private Stack<FolderHierarchy.FolderHierarchyEnumeratorStackEntry> InitEnumeratorStack(Context context, FolderHierarchy.FolderInformationImpl root, bool recursive, Func<Context, IFolderInformation, bool> isVisiblePredicate, ExchangeShortId startFolderId, int startFolderSortPosition, bool startFolderInclusive, FolderInformationComparer alternateComparer, out int sortPosition)
		{
			Stack<FolderHierarchy.FolderHierarchyEnumeratorStackEntry> result;
			if (startFolderId.IsZero)
			{
				result = this.InitEnumeratorStackForSortPosition(context, root, recursive, isVisiblePredicate, 1, alternateComparer);
				sortPosition = 1;
			}
			else
			{
				result = this.InitEnumeratorStackForFolderId(context, root, recursive, isVisiblePredicate, startFolderId, startFolderSortPosition, startFolderInclusive, alternateComparer, out sortPosition);
			}
			return result;
		}

		private Stack<FolderHierarchy.FolderHierarchyEnumeratorStackEntry> InitEnumeratorStackForSortPosition(Context context, FolderHierarchy.FolderInformationImpl root, bool recursive, Func<Context, IFolderInformation, bool> isVisiblePredicate, int startFolderSortPosition, FolderInformationComparer alternateComparer)
		{
			Stack<FolderHierarchy.FolderHierarchyEnumeratorStackEntry> stack = new Stack<FolderHierarchy.FolderHierarchyEnumeratorStackEntry>(recursive ? 4 : 1);
			IList<IFolderInformation> sortedChildrenList = this.GetSortedChildrenList(context, root, alternateComparer);
			if (sortedChildrenList.Count != 0)
			{
				int num = 0;
				if (isVisiblePredicate != null)
				{
					while (!isVisiblePredicate(context, sortedChildrenList[num]))
					{
						num++;
						if (num >= sortedChildrenList.Count)
						{
							break;
						}
					}
				}
				if (num < sortedChildrenList.Count)
				{
					stack.Push(new FolderHierarchy.FolderHierarchyEnumeratorStackEntry(sortedChildrenList, num, 1));
					if (startFolderSortPosition > 1)
					{
						int num2 = 1;
						while (stack.Count != 0 && num2 != startFolderSortPosition)
						{
							this.PopFromEnumeratorStack(context, stack, recursive, isVisiblePredicate, alternateComparer);
							num2++;
						}
					}
				}
			}
			return stack;
		}

		private Stack<FolderHierarchy.FolderHierarchyEnumeratorStackEntry> InitEnumeratorStackForFolderId(Context context, FolderHierarchy.FolderInformationImpl root, bool recursive, Func<Context, IFolderInformation, bool> isVisiblePredicate, ExchangeShortId startFolderId, int startFolderSortPosition, bool startFolderInclusive, FolderInformationComparer alternateComparer, out int sortPosition)
		{
			sortPosition = startFolderSortPosition;
			Stack<FolderHierarchy.FolderInformationImpl> stack = null;
			FolderHierarchy.FolderInformationImpl folderInformationImpl;
			if (this.hierarchyFolders.TryGetValue(startFolderId, out folderInformationImpl))
			{
				stack = new Stack<FolderHierarchy.FolderInformationImpl>();
				if (!recursive)
				{
					if (folderInformationImpl.Parent == root)
					{
						stack.Push(folderInformationImpl);
						stack.Push(root);
					}
					else
					{
						stack = null;
					}
				}
				else
				{
					FolderHierarchy.FolderInformationImpl folderInformationImpl2 = folderInformationImpl;
					do
					{
						stack.Push(folderInformationImpl2);
						folderInformationImpl2 = folderInformationImpl2.Parent;
					}
					while (folderInformationImpl2 != null && folderInformationImpl2 != root);
					stack.Push(folderInformationImpl2);
					if (folderInformationImpl2 != root)
					{
						stack = null;
					}
				}
			}
			Stack<FolderHierarchy.FolderHierarchyEnumeratorStackEntry> stack2;
			if (stack == null)
			{
				stack2 = this.InitEnumeratorStackForSortPosition(context, root, recursive, isVisiblePredicate, startFolderSortPosition, alternateComparer);
				if (!startFolderInclusive)
				{
					sortPosition++;
				}
			}
			else
			{
				FolderInformationComparer comparer = alternateComparer ?? this.folderInformationComparer;
				stack2 = new Stack<FolderHierarchy.FolderHierarchyEnumeratorStackEntry>(recursive ? 4 : 1);
				int num = 1;
				bool flag = false;
				while (stack.Count > 1)
				{
					FolderHierarchy.FolderInformationImpl parent = stack.Pop();
					IList<IFolderInformation> sortedChildrenList = this.GetSortedChildrenList(context, parent, alternateComparer);
					FolderHierarchy.FolderInformationImpl folderInformationImpl3 = stack.Peek();
					int num2;
					if (sortedChildrenList is List<IFolderInformation>)
					{
						List<IFolderInformation> list = (List<IFolderInformation>)sortedChildrenList;
						num2 = list.BinarySearch(folderInformationImpl3, comparer);
					}
					else
					{
						IFolderInformation[] array = (IFolderInformation[])sortedChildrenList;
						num2 = Array.BinarySearch<IFolderInformation>(array, folderInformationImpl3, comparer);
					}
					if (num2 < 0)
					{
						for (int i = 0; i < sortedChildrenList.Count; i++)
						{
							if (object.ReferenceEquals(sortedChildrenList[i], folderInformationImpl3))
							{
								num2 = i;
								break;
							}
						}
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(num2 >= 0, "the child was not found in a parent's children list? #2");
					}
					if (stack.Count > 1)
					{
						num2++;
					}
					if (isVisiblePredicate != null)
					{
						flag = false;
						while (num2 < sortedChildrenList.Count && !isVisiblePredicate(context, sortedChildrenList[num2]))
						{
							flag = true;
							num2++;
						}
					}
					if (num2 < sortedChildrenList.Count)
					{
						stack2.Push(new FolderHierarchy.FolderHierarchyEnumeratorStackEntry(sortedChildrenList, num2, num));
					}
					num++;
				}
				if (!startFolderInclusive)
				{
					if (!flag && stack2.Count != 0)
					{
						this.PopFromEnumeratorStack(context, stack2, recursive, isVisiblePredicate, alternateComparer);
					}
					sortPosition++;
				}
			}
			return stack2;
		}

		private FolderHierarchy.FolderHierarchyEnumeratorStackEntry PopFromEnumeratorStack(Context context, Stack<FolderHierarchy.FolderHierarchyEnumeratorStackEntry> enumeratorStack, bool recursive, Func<Context, IFolderInformation, bool> isVisiblePredicate, FolderInformationComparer alternateComparer)
		{
			FolderHierarchy.FolderHierarchyEnumeratorStackEntry result = enumeratorStack.Pop();
			FolderHierarchy.FolderInformationImpl folderInformationImpl = (FolderHierarchy.FolderInformationImpl)result.List[result.Position];
			int num = result.Position + 1;
			if (num < result.List.Count)
			{
				if (isVisiblePredicate != null)
				{
					while (!isVisiblePredicate(context, result.List[num]))
					{
						num++;
						if (num >= result.List.Count)
						{
							break;
						}
					}
				}
				if (num < result.List.Count)
				{
					enumeratorStack.Push(new FolderHierarchy.FolderHierarchyEnumeratorStackEntry(result.List, num, result.Depth));
				}
			}
			if (recursive && folderInformationImpl.Children.Count != 0)
			{
				IList<IFolderInformation> sortedChildrenList = this.GetSortedChildrenList(context, folderInformationImpl, alternateComparer);
				num = 0;
				if (isVisiblePredicate != null)
				{
					while (!isVisiblePredicate(context, sortedChildrenList[num]))
					{
						num++;
						if (num >= sortedChildrenList.Count)
						{
							break;
						}
					}
				}
				if (num < sortedChildrenList.Count)
				{
					enumeratorStack.Push(new FolderHierarchy.FolderHierarchyEnumeratorStackEntry(sortedChildrenList, num, result.Depth + 1));
				}
			}
			return result;
		}

		private IList<IFolderInformation> GetSortedChildrenList(Context context, FolderHierarchy.FolderInformationImpl parent, FolderInformationComparer alternateComparer)
		{
			if (parent != null)
			{
				this.PopulateChildren(context, parent);
			}
			IList<IFolderInformation> list = (parent == null) ? this.hierarchyRoots : parent.Children;
			if (list.Count > 1)
			{
				if (alternateComparer != null)
				{
					IFolderInformation[] array = list.ToArray<IFolderInformation>();
					Array.Sort<IFolderInformation>(array, alternateComparer);
					list = array;
				}
				else if (parent != null)
				{
					parent.SortChildrenAndCompact(this.folderInformationComparer);
					list = parent.Children;
				}
			}
			else if (parent != null)
			{
				parent.IsSorted = true;
			}
			return list;
		}

		private const int InitialSizeOfRootsList = 5;

		private const int InitialSizeOfDescendantStack = 5;

		private static Hookable<int> maxChildrenForCompaction = Hookable<int>.Create(true, 1000);

		private static readonly FolderHierarchy emptyHierarchy = new FolderHierarchy(new FolderInformationComparer(null), new Dictionary<ExchangeShortId, FolderHierarchy.FolderInformationImpl>(0), new List<IFolderInformation>(), FolderInformationType.Basic);

		private static int folderHierarchyDataSlot = -1;

		private readonly IDictionary<ExchangeShortId, FolderHierarchy.FolderInformationImpl> hierarchyFolders;

		private readonly List<IFolderInformation> hierarchyRoots;

		private readonly FolderInformationType informationType;

		private readonly FolderInformationComparer folderInformationComparer;

		private bool isValid;

		private class FolderInformationImpl : IFolderInformation
		{
			internal FolderInformationImpl(ExchangeShortId fid, FolderHierarchy.FolderInformationImpl parentInformation, FolderHierarchy.FolderInformationImpl.FolderInformationFlags folderFlags, int mailboxNumber, string displayName, long messageCount, SecurityDescriptor securityDescriptor)
			{
				this.parent = parentInformation;
				this.fid = fid;
				this.folderFlags = (int)folderFlags;
				this.mailboxNumber = mailboxNumber;
				this.displayName = displayName;
				this.messageCount = messageCount;
				this.securityDescriptor = securityDescriptor;
			}

			internal FolderInformationImpl(ExchangeShortId parentFid)
			{
				this.fid = parentFid;
			}

			public FolderHierarchy.FolderInformationImpl Parent
			{
				get
				{
					return this.parent;
				}
			}

			public ExchangeShortId Fid
			{
				get
				{
					return this.fid;
				}
			}

			public ExchangeShortId ParentFid
			{
				get
				{
					if (this.parent == null)
					{
						return ExchangeShortId.Zero;
					}
					return this.parent.Fid;
				}
			}

			public int MailboxNumber
			{
				get
				{
					return this.mailboxNumber;
				}
				set
				{
					this.mailboxNumber = value;
				}
			}

			public bool IsSearchFolder
			{
				get
				{
					return this.GetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.IsSearchFolder);
				}
			}

			public string DisplayName
			{
				get
				{
					return this.displayName;
				}
			}

			public IList<IFolderInformation> Children
			{
				get
				{
					return this.children;
				}
				set
				{
					this.children = value;
				}
			}

			public SecurityDescriptor SecurityDescriptor
			{
				get
				{
					return this.securityDescriptor;
				}
				set
				{
					this.securityDescriptor = value;
				}
			}

			public bool ChildrenArePopulated
			{
				get
				{
					return this.GetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.ChildrenArePopulated);
				}
				set
				{
					if (value)
					{
						this.SetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.ChildrenArePopulated);
						return;
					}
					this.RemoveFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.ChildrenArePopulated);
				}
			}

			public bool IsSorted
			{
				get
				{
					return this.GetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.IsSorted);
				}
				set
				{
					if (value)
					{
						this.SetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.IsSorted);
						return;
					}
					this.RemoveFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.IsSorted);
				}
			}

			public bool IsPartOfContentIndexing
			{
				get
				{
					return this.GetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.IsPartOfContentIndexing);
				}
				set
				{
					if (value)
					{
						this.SetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.IsPartOfContentIndexing);
						return;
					}
					this.RemoveFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.IsPartOfContentIndexing);
				}
			}

			public bool IsInternalAccess
			{
				get
				{
					return this.GetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.InternalAccess);
				}
				set
				{
					if (value)
					{
						this.SetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.InternalAccess);
						return;
					}
					this.RemoveFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.InternalAccess);
				}
			}

			public long MessageCount
			{
				get
				{
					return this.messageCount;
				}
				set
				{
					this.messageCount = value;
				}
			}

			public static FolderHierarchy.FolderInformationImpl FromFolderId(ExchangeShortId folderId)
			{
				return new FolderHierarchy.FolderInformationImpl(folderId);
			}

			public static IList<Column> GetColumnsToFetch(FolderHierarchy.FolderInformationImpl.FolderColumnsInformation folderColumnsInformation, FolderInformationType informationType)
			{
				List<Column> list = new List<Column>(8);
				list.Add(folderColumnsInformation.Table.ParentFolderId);
				list.Add(folderColumnsInformation.Table.FolderId);
				list.Add(folderColumnsInformation.Table.DisplayName);
				list.Add(folderColumnsInformation.Table.QueryCriteria);
				list.Add(folderColumnsInformation.PartOfContentIndexingColumn);
				list.Add(folderColumnsInformation.Table.MessageCount);
				list.Add(folderColumnsInformation.MailboxNumberColumn);
				list.Add(folderColumnsInformation.InternalAccessColumn);
				if (informationType == FolderInformationType.Extended)
				{
					list.Add(folderColumnsInformation.Table.AclTableAndSecurityDescriptor);
				}
				return list;
			}

			public IEnumerable<ExchangeShortId> AllDescendantFolderIds()
			{
				Queue<FolderHierarchy.FolderInformationImpl> queue = new Queue<FolderHierarchy.FolderInformationImpl>(100);
				queue.Enqueue(this);
				while (queue.Count != 0)
				{
					FolderHierarchy.FolderInformationImpl fi = queue.Dequeue();
					if (fi.Children != null && fi.Children.Count != 0)
					{
						using (LockManager.Lock(fi, LockManager.LockType.LeafMonitorLock))
						{
							for (int i = 0; i < fi.Children.Count; i++)
							{
								queue.Enqueue((FolderHierarchy.FolderInformationImpl)fi.Children[i]);
							}
						}
					}
					yield return fi.Fid;
				}
				yield break;
			}

			public void LinkChild(FolderHierarchy.FolderInformationImpl child, FolderInformationComparer folderInformationComparer)
			{
				if (!(this.children is List<IFolderInformation>))
				{
					List<IFolderInformation> list = new List<IFolderInformation>(this.children.Count + 1);
					list.AddRange(this.children);
					this.children = list;
				}
				if (!this.IsSorted)
				{
					this.children.Add(child);
				}
				else
				{
					int num = ((List<IFolderInformation>)this.children).BinarySearch(child, folderInformationComparer);
					this.children.Insert(~num, child);
				}
				child.parent = this;
			}

			public void Populate(Context context, FolderHierarchy.FolderInformationImpl.FolderColumnsInformation folderColumnsInformation, FolderInformationType informationType, Reader reader)
			{
				this.displayName = (reader.GetString(folderColumnsInformation.Table.DisplayName) ?? string.Empty);
				this.messageCount = reader.GetInt64(folderColumnsInformation.Table.MessageCount);
				this.mailboxNumber = reader.GetInt32(folderColumnsInformation.MailboxNumberColumn);
				bool flag = reader.GetBinary(folderColumnsInformation.Table.QueryCriteria) != null;
				if (flag)
				{
					this.SetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.IsSearchFolder);
				}
				bool boolean = reader.GetBoolean(folderColumnsInformation.PartOfContentIndexingColumn);
				if (boolean)
				{
					this.SetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.IsPartOfContentIndexing);
				}
				bool? nullableBoolean = reader.GetNullableBoolean(folderColumnsInformation.InternalAccessColumn);
				if (nullableBoolean != null && nullableBoolean.Value)
				{
					this.SetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags.InternalAccess);
				}
				if (informationType == FolderInformationType.Extended)
				{
					byte[] binary = reader.GetBinary(folderColumnsInformation.Table.AclTableAndSecurityDescriptor);
					FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = AclTableHelper.Parse(context, binary);
					this.securityDescriptor = aclTableAndSecurityDescriptorProperty.SecurityDescriptor;
				}
			}

			public void UnlinkChild(FolderHierarchy.FolderInformationImpl child, FolderInformationComparer folderInformationComparer, bool skipCompaction)
			{
				if (!(this.children is List<IFolderInformation>))
				{
					this.children = new List<IFolderInformation>(this.children);
				}
				List<IFolderInformation> list = (List<IFolderInformation>)this.children;
				int num;
				if (this.ChildrenArePopulated)
				{
					if (!this.IsSorted)
					{
						list.Sort(folderInformationComparer);
						this.IsSorted = true;
					}
					num = list.BinarySearch(child, folderInformationComparer);
					if (num < 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (object.ReferenceEquals(list[i], child))
							{
								num = i;
								break;
							}
						}
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(num >= 0, "the child was not found in a parent's children list? #1");
					}
				}
				else
				{
					num = list.IndexOf(child);
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(num >= 0, "the child was not found in a parent's children list? #2");
				}
				list.RemoveAt(num);
				if (!skipCompaction)
				{
					if (list.Count == 0)
					{
						this.children = Array<IFolderInformation>.Empty;
						return;
					}
					if (list.Count <= FolderHierarchy.maxChildrenForCompaction.Value)
					{
						this.children = list.ToArray();
					}
				}
			}

			public void ChangeDisplayName(string newDisplayName, FolderInformationComparer folderInformationComparer)
			{
				if (this.parent == null || !this.parent.IsSorted)
				{
					this.displayName = newDisplayName;
					return;
				}
				FolderHierarchy.FolderInformationImpl folderInformationImpl = this.parent;
				folderInformationImpl.UnlinkChild(this, folderInformationComparer, true);
				this.displayName = newDisplayName;
				folderInformationImpl.LinkChild(this, folderInformationComparer);
			}

			public void SortChildrenAndCompact(FolderInformationComparer folderInformationComparer)
			{
				if (this.children.Count <= FolderHierarchy.maxChildrenForCompaction.Value)
				{
					if (this.IsSorted && !(this.children is List<IFolderInformation>))
					{
						return;
					}
				}
				else if (this.IsSorted)
				{
					return;
				}
				using (LockManager.Lock(this, LockManager.LockType.LeafMonitorLock))
				{
					if (this.children.Count <= FolderHierarchy.maxChildrenForCompaction.Value)
					{
						if (!this.IsSorted)
						{
							if (!(this.children is List<IFolderInformation>))
							{
								this.children = new List<IFolderInformation>(this.children);
							}
							((List<IFolderInformation>)this.children).Sort(folderInformationComparer);
							this.IsSorted = true;
						}
						if (this.children is List<IFolderInformation>)
						{
							this.children = this.children.ToArray<IFolderInformation>();
						}
					}
					else if (!this.IsSorted)
					{
						((List<IFolderInformation>)this.children).Sort(folderInformationComparer);
						this.IsSorted = true;
					}
				}
			}

			public void SetChildren(IList<IFolderInformation> children)
			{
				this.children = children;
				for (int i = 0; i < children.Count; i++)
				{
					((FolderHierarchy.FolderInformationImpl)children[i]).parent = this;
				}
			}

			private bool GetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags flag)
			{
				return (this.folderFlags & (int)flag) != 0;
			}

			private void SetFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags flag)
			{
				int num;
				do
				{
					num = this.folderFlags;
				}
				while (num != Interlocked.CompareExchange(ref this.folderFlags, num | (int)flag, num));
			}

			private void RemoveFolderInformationFlag(FolderHierarchy.FolderInformationImpl.FolderInformationFlags flag)
			{
				int num;
				do
				{
					num = this.folderFlags;
				}
				while (num != Interlocked.CompareExchange(ref this.folderFlags, num & (int)(~(int)flag), num));
			}

			private readonly ExchangeShortId fid;

			private int folderFlags;

			private int mailboxNumber;

			private long messageCount;

			private string displayName;

			private FolderHierarchy.FolderInformationImpl parent;

			private IList<IFolderInformation> children = Array<IFolderInformation>.Empty;

			private SecurityDescriptor securityDescriptor;

			[Flags]
			internal enum FolderInformationFlags
			{
				None = 0,
				IsSearchFolder = 1,
				IsPartOfContentIndexing = 2,
				IsSorted = 4,
				InternalAccess = 8,
				ChildrenArePopulated = 16
			}

			public struct FolderColumnsInformation
			{
				internal FolderTable Table { get; set; }

				internal Column PartOfContentIndexingColumn { get; set; }

				internal Column MailboxNumberColumn { get; set; }

				internal Column InternalAccessColumn { get; set; }
			}
		}

		private struct FolderHierarchyEnumeratorStackEntry
		{
			internal FolderHierarchyEnumeratorStackEntry(IList<IFolderInformation> list, int position, int depth)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(position >= 0 && position < list.Count, "Entry position is out of range");
				this.list = list;
				this.position = position;
				this.depth = depth;
			}

			public IList<IFolderInformation> List
			{
				get
				{
					return this.list;
				}
			}

			public int Position
			{
				get
				{
					return this.position;
				}
			}

			public int Depth
			{
				get
				{
					return this.depth;
				}
			}

			private IList<IFolderInformation> list;

			private int position;

			private int depth;
		}
	}
}
