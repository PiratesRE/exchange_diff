using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class AddressBookListView : ListView
	{
		public AddressBookListView(string searchString, UserContext userContext, ColumnId sortedColumn, SortOrder sortOrder, AddressBookBase addressBook, AddressBookBase.RecipientCategory recipientCategory) : base(userContext, sortedColumn, sortOrder, ListView.ViewType.ADContentsListView)
		{
			this.addressBook = addressBook;
			this.searchString = searchString;
			this.recipientCategory = recipientCategory;
			base.FilteredView = !string.IsNullOrEmpty(searchString);
		}

		public override void Initialize(int startRange, int endRange)
		{
			this.InitializeListViewContents();
			this.InitializeDataSource();
			base.DataSource.LoadData(startRange, endRange);
		}

		protected override void InitializeListViewContents()
		{
			base.ViewDescriptor = AddressBookListView.AddressBookViewDescriptor;
			base.Contents = new AddressBookItemList(base.ViewDescriptor, base.SortedColumn, base.SortOrder, base.UserContext);
		}

		protected override void InitializeDataSource()
		{
			base.DataSource = new AddressBookDataSource(base.Contents.Properties, this.searchString, this.addressBook, this.recipientCategory, base.UserContext);
		}

		private static readonly ViewDescriptor AddressBookViewDescriptor = new ViewDescriptor(ColumnId.DisplayNameAD, true, new ColumnId[]
		{
			ColumnId.CheckBoxAD,
			ColumnId.DisplayNameAD,
			ColumnId.AliasAD,
			ColumnId.PhoneAD,
			ColumnId.OfficeAD,
			ColumnId.TitleAD,
			ColumnId.CompanyAD
		});

		private string searchString;

		private AddressBookBase addressBook;

		private AddressBookBase.RecipientCategory recipientCategory;
	}
}
