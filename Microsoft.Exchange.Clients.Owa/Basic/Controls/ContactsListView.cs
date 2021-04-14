using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class ContactsListView : ListView
	{
		internal ContactsListView(UserContext userContext, ColumnId sortedColumn, SortOrder sortOrder, Folder folder) : base(userContext, sortedColumn, sortOrder, ListView.ViewType.ContactsListView)
		{
			this.folder = folder;
		}

		internal ContactsListView(UserContext userContext, ColumnId sortedColumn, SortOrder sortOrder, Folder folder, SearchScope searchScope) : this(userContext, sortedColumn, sortOrder, folder)
		{
			base.FilteredView = true;
			this.searchScope = searchScope;
		}

		protected override void InitializeListViewContents()
		{
			base.ViewDescriptor = ContactsListView.Contacts;
			bool showFolderNameTooltip = base.FilteredView && this.searchScope != SearchScope.SelectedFolder;
			base.Contents = new ContactsListViewContents(base.ViewDescriptor, base.SortedColumn, base.SortOrder, showFolderNameTooltip, base.UserContext);
		}

		protected override void InitializeDataSource()
		{
			SortBy[] array;
			if (base.SortedColumn == ColumnId.FileAs)
			{
				array = new SortBy[]
				{
					new SortBy(ContactBaseSchema.FileAs, base.SortOrder)
				};
			}
			else
			{
				array = new SortBy[2];
				Column column = ListViewColumns.GetColumn(base.SortedColumn);
				array[0] = new SortBy(column[0], base.SortOrder);
				array[1] = new SortBy(ContactBaseSchema.FileAs, base.SortOrder);
			}
			base.DataSource = new MessageListViewDataSource(base.Contents.Properties, this.folder, array);
		}

		private static readonly ViewDescriptor Contacts = new ViewDescriptor(ColumnId.FileAs, true, new ColumnId[]
		{
			ColumnId.CheckBoxContact,
			ColumnId.FileAs,
			ColumnId.EmailAddresses,
			ColumnId.PhoneNumbers,
			ColumnId.Title,
			ColumnId.CompanyName
		});

		private Folder folder;

		private SearchScope searchScope;
	}
}
