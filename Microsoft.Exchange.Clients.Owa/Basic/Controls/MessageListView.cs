using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class MessageListView : ListView
	{
		internal MessageListView(UserContext userContext, ColumnId sortedColumn, SortOrder sortOrder, Folder folder) : base(userContext, sortedColumn, sortOrder, ListView.ViewType.MessageListView)
		{
			this.folder = folder;
		}

		internal MessageListView(UserContext userContext, ColumnId sortedColumn, SortOrder sortOrder, Folder folder, SearchScope searchScope) : this(userContext, sortedColumn, sortOrder, folder)
		{
			base.FilteredView = true;
			this.searchScope = searchScope;
		}

		protected override void InitializeListViewContents()
		{
			DefaultFolderType defaultFolderType = Utilities.GetDefaultFolderType(this.folder);
			if (defaultFolderType == DefaultFolderType.SentItems || defaultFolderType == DefaultFolderType.Outbox || defaultFolderType == DefaultFolderType.Drafts)
			{
				base.ViewDescriptor = MessageListView.To;
			}
			else
			{
				base.ViewDescriptor = MessageListView.From;
			}
			bool showFolderNameTooltip = base.FilteredView && this.searchScope != SearchScope.SelectedFolder;
			base.Contents = new MessageListViewContents(base.ViewDescriptor, base.SortedColumn, base.SortOrder, showFolderNameTooltip, base.UserContext);
		}

		protected override void InitializeDataSource()
		{
			Column column = ListViewColumns.GetColumn(base.SortedColumn);
			SortBy[] sortBy;
			if (base.SortedColumn == ColumnId.DeliveryTime)
			{
				sortBy = new SortBy[]
				{
					new SortBy(ItemSchema.ReceivedTime, base.SortOrder)
				};
			}
			else
			{
				sortBy = new SortBy[]
				{
					new SortBy(column[0], base.SortOrder),
					new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
				};
			}
			base.DataSource = new MessageListViewDataSource(base.Contents.Properties, this.folder, sortBy);
		}

		private static readonly ViewDescriptor From = new ViewDescriptor(ColumnId.DeliveryTime, false, new ColumnId[]
		{
			ColumnId.Importance,
			ColumnId.MailIcon,
			ColumnId.HasAttachment,
			ColumnId.CheckBox,
			ColumnId.From,
			ColumnId.Subject,
			ColumnId.DeliveryTime,
			ColumnId.Size
		});

		private static readonly ViewDescriptor To = new ViewDescriptor(ColumnId.SentTime, false, new ColumnId[]
		{
			ColumnId.Importance,
			ColumnId.MailIcon,
			ColumnId.HasAttachment,
			ColumnId.CheckBox,
			ColumnId.To,
			ColumnId.Subject,
			ColumnId.SentTime,
			ColumnId.Size
		});

		private Folder folder;

		private SearchScope searchScope;
	}
}
