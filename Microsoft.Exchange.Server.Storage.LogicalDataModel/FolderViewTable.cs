using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class FolderViewTable : ViewTable
	{
		public FolderViewTable(Context context, Mailbox mailbox, ExchangeId parentFolderId, FolderViewTable.ConfigureFlags configureFlags) : this(context, mailbox, parentFolderId, configureFlags, FolderInformationType.Basic, null)
		{
		}

		public FolderViewTable(Context context, Mailbox mailbox, ExchangeId parentFolderId, FolderViewTable.ConfigureFlags configureFlags, FolderInformationType folderInformationType, Func<Context, IFolderInformation, bool> isVisiblePredicate) : base(mailbox, DatabaseSchema.FolderTable(mailbox.Database).Table)
		{
			this.parentFolderId = parentFolderId;
			this.configureFlags = configureFlags;
			this.isVisiblePredicate = isVisiblePredicate;
			this.folderInformationType = folderInformationType;
			this.folderTable = DatabaseSchema.FolderTable(mailbox.Database);
			base.SetImplicitCriteria(Factory.CreateSearchCriteriaTrue());
			this.folderHierarchyBlobTableFunction = DatabaseSchema.FolderHierarchyBlobTableFunction(base.Mailbox.Database);
			this.renameDictionary = new Dictionary<Column, Column>(5);
			this.renameDictionary.Add(this.folderTable.MailboxPartitionNumber, this.folderHierarchyBlobTableFunction.MailboxPartitionNumber);
			this.renameDictionary.Add(this.folderTable.FolderId, this.folderHierarchyBlobTableFunction.FolderId);
			this.renameDictionary.Add(this.folderTable.ParentFolderId, this.folderHierarchyBlobTableFunction.ParentFolderId);
			this.renameDictionary.Add(this.folderTable.DisplayName, this.folderHierarchyBlobTableFunction.DisplayName);
			if (UnifiedMailbox.IsReady(context, context.Database))
			{
				this.renameDictionary.Add(this.folderTable.MailboxNumber, this.folderHierarchyBlobTableFunction.MailboxNumber);
			}
			base.SortTable(this.folderHierarchyBlobTableFunction.TableFunction.PrimaryKeyIndex.SortOrder);
		}

		public FolderViewTable(Context context, Mailbox mailbox, ExchangeId parentFolderId, FolderViewTable.ConfigureFlags configureFlags, IList<Column> columns, SortOrder sortOrder, SearchCriteria criteria) : this(context, mailbox, parentFolderId, configureFlags)
		{
			base.SetColumns(context, columns);
			this.SortTable(sortOrder);
			this.Restrict(context, criteria);
		}

		public bool Recursive
		{
			get
			{
				return (this.configureFlags & FolderViewTable.ConfigureFlags.Recursive) == FolderViewTable.ConfigureFlags.Recursive;
			}
		}

		protected override Index LogicalKeyIndex
		{
			get
			{
				return this.folderHierarchyBlobTableFunction.TableFunction.PrimaryKeyIndex;
			}
		}

		protected override bool MustUseLazyIndex
		{
			get
			{
				return true;
			}
		}

		public override void SortTable(SortOrder sortOrder)
		{
			if (!this.Recursive)
			{
				bool flag = sortOrder.Count == 0;
				bool flag2 = false;
				if (sortOrder.Count == 1 && sortOrder.Columns[0] == this.folderTable.DisplayName)
				{
					flag = true;
					flag2 = !sortOrder.Ascending[0];
				}
				if (flag)
				{
					sortOrder = this.folderHierarchyBlobTableFunction.TableFunction.PrimaryKeyIndex.SortOrder;
					if (flag2)
					{
						sortOrder = sortOrder.Reverse();
					}
				}
				base.SortTable(sortOrder);
			}
		}

		internal byte[] GetRecursiveQueryBlobForTest(Context context)
		{
			IList<IIndex> list;
			object obj = ((SimplePseudoIndex)this.GetInScopePseudoIndexes(context, null, out list)[0]).IndexTableFunctionParameters[0];
			if (context.Database.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
			{
				IEnumerable<FolderHierarchyBlob> source = (IEnumerable<FolderHierarchyBlob>)obj;
				return FolderHierarchyBlob.Serialize(source.ToArray<FolderHierarchyBlob>());
			}
			return (byte[])obj;
		}

		protected internal override IList<IIndex> GetInScopePseudoIndexes(Context context, SearchCriteria findRowCriteria, out IList<IIndex> masterIndexes)
		{
			masterIndexes = null;
			object[] array = new object[1];
			FolderViewTable.HierarchyTableContentsGenerator hierarchyTableContentsGenerator = new FolderViewTable.HierarchyTableContentsGenerator(context, this);
			if (context.Database.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
			{
				array[0] = hierarchyTableContentsGenerator;
			}
			else
			{
				array[0] = FolderHierarchyBlob.Serialize(hierarchyTableContentsGenerator.ToArray<FolderHierarchyBlob>());
			}
			SimplePseudoIndex simplePseudoIndex = new SimplePseudoIndex(this.folderTable.Table, this.folderHierarchyBlobTableFunction.TableFunction, array, this.folderHierarchyBlobTableFunction.TableFunction.PrimaryKeyIndex.SortOrder, this.renameDictionary, null, true);
			return new IIndex[]
			{
				simplePseudoIndex
			};
		}

		protected override void BringIndexesToCurrent(Context context, IList<IIndex> indexList, DataAccessOperator queryPlan)
		{
		}

		protected override IReadOnlyDictionary<Column, Column> GetColumnRenames(Context context)
		{
			Dictionary<Column, Column> dictionary = new Dictionary<Column, Column>(1);
			dictionary[this.folderTable.VirtualUnreadMessageCount] = Factory.CreateFunctionColumn("PerUserUnreadMessageCount", typeof(long), PropertyTypeHelper.SizeFromPropType(PropertyType.Int64), PropertyTypeHelper.MaxLengthFromPropType(PropertyType.Int64), base.Table, new Func<object[], object>(this.GetFolderUnreadCountColumnFunction), "ComputeGetUnreadMessageCount", new Column[]
			{
				this.folderTable.FolderId,
				this.folderTable.UnreadMessageCount
			});
			return dictionary;
		}

		private object GetFolderUnreadCountColumnFunction(object[] columnValues)
		{
			Context currentOperationContext = base.Mailbox.CurrentOperationContext;
			ExchangeId id = ExchangeId.CreateFrom26ByteArray(currentOperationContext, base.Mailbox.ReplidGuidMap, (byte[])columnValues[0]);
			Folder folder = Folder.OpenFolder(currentOperationContext, base.Mailbox, id);
			long num = (long)columnValues[1];
			if (folder == null)
			{
				return num;
			}
			return folder.GetUnreadMessageCount(currentOperationContext, num);
		}

		private IEnumerable<FolderHierarchyBlob> GetTableContents(Context context, bool backwards, StartStopKey startKey)
		{
			FolderHierarchy folderHierarchy = FolderHierarchy.GetFolderHierarchy(context, base.Mailbox, this.parentFolderId.ToExchangeShortId(), this.folderInformationType);
			IFolderInformation root = this.parentFolderId.IsNullOrZero ? null : folderHierarchy.Find(context, this.parentFolderId.ToExchangeShortId());
			ExchangeId exchangeId = ExchangeId.Null;
			int startFolderSortPosition = 0;
			bool startFolderInclusive = true;
			if (!backwards && base.SortOrder.Count == 2 && base.SortOrder.Columns[0] == this.folderHierarchyBlobTableFunction.TableFunction.PrimaryKeyIndex.SortOrder.Columns[0] && base.SortOrder.Columns[1] == this.folderHierarchyBlobTableFunction.TableFunction.PrimaryKeyIndex.SortOrder.Columns[1] && !startKey.IsEmpty)
			{
				startFolderSortPosition = (int)startKey.Values[0];
				exchangeId = ExchangeId.CreateFrom26ByteArray(context, base.Mailbox.ReplidGuidMap, (byte[])startKey.Values[1]);
				startFolderInclusive = startKey.Inclusive;
			}
			IEnumerable<FolderHierarchyBlob> enumerable = folderHierarchy.SerializeRecursiveHierarchyBlob(context, base.Mailbox, root, this.Recursive, this.isVisiblePredicate, exchangeId.ToExchangeShortId(), startFolderSortPosition, startFolderInclusive);
			if (backwards)
			{
				IEnumerator<FolderHierarchyBlob> enumerator = enumerable.GetEnumerator();
				Stack<FolderHierarchyBlob> stack = new Stack<FolderHierarchyBlob>();
				while (enumerator.MoveNext())
				{
					FolderHierarchyBlob item = enumerator.Current;
					stack.Push(item);
				}
				enumerable = stack;
			}
			return enumerable;
		}

		private readonly FolderViewTable.ConfigureFlags configureFlags;

		private readonly FolderHierarchyBlobTableFunction folderHierarchyBlobTableFunction;

		private readonly Dictionary<Column, Column> renameDictionary;

		private readonly FolderTable folderTable;

		private readonly ExchangeId parentFolderId;

		private readonly Func<Context, IFolderInformation, bool> isVisiblePredicate;

		private readonly FolderInformationType folderInformationType;

		protected SearchCriteria viewCriteria;

		[Flags]
		public enum ConfigureFlags
		{
			None = 0,
			NeedOnlyFolderList = 2,
			Recursive = 4,
			NoNotifications = 16,
			EmptyTable = 32,
			SuppressNotifications = 128,
			UseChangeNumberIndex = 256
		}

		private class HierarchyTableContentsGenerator : IEnumerable<FolderHierarchyBlob>, IEnumerable, IConfigurableTableContents
		{
			public HierarchyTableContentsGenerator(Context context, FolderViewTable view)
			{
				this.context = context;
				this.view = view;
			}

			public void Configure(bool backwards, StartStopKey startKey)
			{
				this.backwards = backwards;
				this.startKey = startKey;
			}

			public IEnumerator<FolderHierarchyBlob> GetEnumerator()
			{
				IEnumerable<FolderHierarchyBlob> tableContents = this.view.GetTableContents(this.context, this.backwards, this.startKey);
				return tableContents.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private Context context;

			private FolderViewTable view;

			private bool backwards;

			private StartStopKey startKey;
		}
	}
}
