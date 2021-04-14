using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class DumpsterVirtualListView : VirtualListView2
	{
		internal DumpsterVirtualListView(UserContext userContext, string id, ColumnId sortedColumn, SortOrder sortOrder, Folder folder) : base(userContext, id, true, sortedColumn, sortOrder, false)
		{
			this.folder = folder;
		}

		protected override Folder DataFolder
		{
			get
			{
				return this.folder;
			}
		}

		public override ViewType ViewType
		{
			get
			{
				return ViewType.Dumpster;
			}
		}

		protected override bool IsMultiLine
		{
			get
			{
				return true;
			}
		}

		public override string OehNamespace
		{
			get
			{
				return "DumpsterVLV";
			}
		}

		public SortBy[] GetSortByProperties()
		{
			Column column = ListViewColumns.GetColumn(base.SortedColumn);
			SortBy[] result;
			if (base.SortedColumn == ColumnId.DeletedOnTime)
			{
				result = new SortBy[]
				{
					new SortBy(StoreObjectSchema.LastModifiedTime, base.SortOrder)
				};
			}
			else
			{
				result = new SortBy[]
				{
					new SortBy(column[0], base.SortOrder),
					new SortBy(StoreObjectSchema.LastModifiedTime, SortOrder.Descending)
				};
			}
			return result;
		}

		protected override ListViewContents2 CreateListViewContents()
		{
			return new MessageMultiLineList2(DumpsterVirtualListView.dumpsterViewDescriptor, base.SortedColumn, base.SortOrder, base.UserContext, SearchScope.SelectedFolder);
		}

		protected override IListViewDataSource CreateDataSource(Hashtable properties)
		{
			return new FolderListViewDataSource(base.UserContext, properties, this.folder, this.GetSortByProperties());
		}

		protected override void InternalRenderData(TextWriter writer)
		{
			base.InternalRenderData(writer);
		}

		private static readonly ViewDescriptor dumpsterViewDescriptor = new ViewDescriptor(ColumnId.DeletedOnTime, false, new ColumnId[]
		{
			ColumnId.ObjectIcon,
			ColumnId.Importance,
			ColumnId.HasAttachment,
			ColumnId.ObjectDisplayName,
			ColumnId.DeletedOnTime,
			ColumnId.From
		});

		private Folder folder;
	}
}
