using System;
using System.Collections;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class AddressBookVirtualListView : VirtualListView2
	{
		internal AddressBookVirtualListView(UserContext userContext, string id, bool isMultiLine, ViewType viewType, ColumnId sortedColumn, SortOrder sortOrder, bool isPAA, bool isMobile, AddressBookBase addressBookBase) : this(userContext, id, isMultiLine, viewType, sortedColumn, sortOrder, isPAA, isMobile, addressBookBase, null, null, 0, Culture.GetUserCulture().LCID, null)
		{
		}

		internal AddressBookVirtualListView(UserContext userContext, string id, bool isMultiLine, ViewType viewType, ColumnId sortedColumn, SortOrder sortOrder, bool isPAA, bool isMobile, AddressBookBase addressBookBase, string searchString, string cookie, int cookieIndex, int cookieLcid, string preferredDC) : this(userContext, id, isMultiLine, viewType, sortedColumn, sortOrder, !string.IsNullOrEmpty(searchString), isPAA, isMobile)
		{
			this.addressBookBase = addressBookBase;
			this.searchString = searchString;
			this.cookie = cookie;
			this.cookieIndex = cookieIndex;
			this.cookieLcid = cookieLcid;
			this.preferredDC = preferredDC;
			if (string.IsNullOrEmpty(this.cookie))
			{
				this.cookie = string.Empty;
				this.preferredDC = string.Empty;
				this.cookieLcid = Culture.GetUserCulture().LCID;
			}
			if (this.cookieLcid < 0)
			{
				this.cookieLcid = Culture.GetUserCulture().LCID;
			}
			if (!string.IsNullOrEmpty(this.searchString))
			{
				if (256 < this.searchString.Length)
				{
					throw new OwaInvalidInputException("Search string is longer than maximum length of " + 256);
				}
				if (this.cookieIndex < 0)
				{
					this.cookie = string.Empty;
					this.cookieIndex = 0;
					this.preferredDC = string.Empty;
					this.cookieLcid = Culture.GetUserCulture().LCID;
				}
			}
		}

		internal AddressBookVirtualListView(UserContext userContext, string id, bool isMultiLine, ViewType viewType, ColumnId sortedColumn, SortOrder sortOrder, bool isPAA, bool isMobile, Folder folder, QueryFilter filter) : this(userContext, id, isMultiLine, viewType, sortedColumn, sortOrder, false, SearchScope.SelectedFolder, isPAA, isMobile, folder, filter)
		{
		}

		internal AddressBookVirtualListView(UserContext userContext, string id, bool isMultiLine, ViewType viewType, ColumnId sortedColumn, SortOrder sortOrder, bool isFiltered, SearchScope folderScope, bool isPAA, bool isMobile, Folder folder, QueryFilter filter) : this(userContext, id, isMultiLine, viewType, sortedColumn, sortOrder, isFiltered, isPAA, isMobile)
		{
			this.folderScope = folderScope;
			this.folder = folder;
			this.filter = filter;
		}

		internal AddressBookVirtualListView(UserContext userContext, string id, bool isMultiLine, ViewType viewType, ColumnId sortedColumn, SortOrder sortOrder, bool isFiltered, bool isPAA, bool isMobile) : base(userContext, id, isMultiLine, sortedColumn, sortOrder, isFiltered)
		{
			this.isPAA = isPAA;
			this.isMobile = isMobile;
			AddressBookVirtualListView.ValidateViewType(viewType);
			this.viewType = viewType;
			if (isPAA)
			{
				base.AddAttribute("fPaa", "1");
			}
			if (isMobile)
			{
				base.AddAttribute("fMbl", "1");
			}
		}

		private static void ValidateViewType(ViewType viewType)
		{
			switch (viewType)
			{
			case ViewType.ContactModule:
			case ViewType.ContactBrowser:
			case ViewType.ContactPicker:
			case ViewType.DirectoryBrowser:
			case ViewType.DirectoryPicker:
			case ViewType.RoomBrowser:
			case ViewType.RoomPicker:
				return;
			}
			throw new ArgumentException(string.Format("Invalid ViewType for AddressBookVirtualListView: {0}.Only ContactModule, ContactLookup, ContactPicker, DirectoryLookup, DirectoryPicker,RoomLookup, and RoomPicker are valid", (int)viewType));
		}

		private bool IsPicker
		{
			get
			{
				return this.viewType == ViewType.ContactPicker || this.viewType == ViewType.DirectoryPicker || this.viewType == ViewType.RoomPicker;
			}
		}

		private bool IsContactView
		{
			get
			{
				return this.viewType == ViewType.ContactModule || this.viewType == ViewType.ContactBrowser || this.viewType == ViewType.ContactPicker;
			}
		}

		protected override Folder DataFolder
		{
			get
			{
				if (this.IsContactView)
				{
					return this.folder;
				}
				throw new NotImplementedException("DataFolder is not valid only for contact view");
			}
		}

		public override ViewType ViewType
		{
			get
			{
				return this.viewType;
			}
		}

		public string Cookie
		{
			get
			{
				return this.cookie;
			}
		}

		public int CookieLcid
		{
			get
			{
				return this.cookieLcid;
			}
		}

		public string PreferredDC
		{
			get
			{
				return this.preferredDC;
			}
		}

		public override string OehNamespace
		{
			get
			{
				switch (this.viewType)
				{
				case ViewType.ContactModule:
					return "CM";
				case ViewType.ContactBrowser:
					return "CB";
				case ViewType.ContactPicker:
					return "CP";
				case ViewType.DirectoryBrowser:
					return "DB";
				case ViewType.DirectoryPicker:
					return "DP";
				case ViewType.RoomBrowser:
					return "RB";
				case ViewType.RoomPicker:
					return "RP";
				}
				throw new ArgumentException(string.Format("Invalid ViewType for AddressBookVirtualListView: {0}.Only ContactModule, ContactLookup, ContactPicker, DirectoryLookup, DirectoryPicker,RoomLookup, and RoomPicker are valid", (int)this.viewType));
			}
		}

		public override bool GetDidLastSearchFail()
		{
			return this.IsContactView && base.GetDidLastSearchFail();
		}

		public override void LoadData(int startRange, int rowCount)
		{
			base.LoadData(startRange, rowCount);
			this.UpdateCookieInformation();
		}

		public override void LoadData(ObjectId seekRowId, SeekDirection seekDirection, int rowCount)
		{
			if (!this.IsContactView)
			{
				throw new NotImplementedException();
			}
			base.LoadData(seekRowId, seekDirection, rowCount);
		}

		public override void LoadData(string seekValue, int rowCount)
		{
			base.LoadData(seekValue, rowCount);
			this.UpdateCookieInformation();
		}

		public override void LoadAdjacent(ObjectId adjacentId, SeekDirection seekDirection, int rowCount)
		{
			if (!this.IsContactView)
			{
				throw new NotImplementedException();
			}
			base.LoadAdjacent(adjacentId, seekDirection, rowCount);
		}

		protected override ListViewContents2 CreateListViewContents()
		{
			ViewDescriptor viewDescriptor = AddressBookViewDescriptors.GetViewDescriptor(this.IsMultiLine, this.IsContactView ? Utilities.IsJapanese : base.UserContext.IsPhoneticNamesEnabled, this.isMobile, this.ViewType);
			return new AddressBookMultiLineList2(viewDescriptor, this.IsContactView, this.IsPicker, base.SortedColumn, base.SortOrder, base.UserContext, this.folderScope, this.isPAA, this.isMobile);
		}

		protected override IListViewDataSource CreateDataSource(Hashtable properties)
		{
			if (this.IsContactView)
			{
				return new FolderListViewDataSource(base.UserContext, properties, this.folder, this.GetSortByProperties(), this.filter);
			}
			if (string.IsNullOrEmpty(this.searchString))
			{
				if (string.IsNullOrEmpty(this.cookie))
				{
					return ADListViewDataSource.CreateForBrowse(properties, this.addressBookBase, base.UserContext);
				}
				return ADListViewDataSource.CreateForBrowse(properties, this.addressBookBase, this.cookie, this.cookieLcid, this.preferredDC, base.UserContext);
			}
			else
			{
				if (string.IsNullOrEmpty(this.cookie))
				{
					return ADListViewDataSource.CreateForSearch(properties, this.addressBookBase, this.searchString, base.UserContext);
				}
				return ADListViewDataSource.CreateForSearch(properties, this.addressBookBase, this.searchString, this.cookie, this.cookieIndex, this.cookieLcid, this.preferredDC, base.UserContext);
			}
		}

		protected override void OnBeforeRender()
		{
			if (!this.IsContactView)
			{
				base.AddAttribute("sCki", this.Cookie);
				base.AddAttribute("iLcid", this.CookieLcid.ToString(CultureInfo.InvariantCulture));
				base.AddAttribute("sPfdDC", this.PreferredDC);
			}
			base.MakePropertyPublic("ea");
		}

		protected override void InternalRenderData(TextWriter writer)
		{
			base.InternalRenderData(writer);
			if (!this.IsContactView)
			{
				VirtualListView2.RenderAttribute(writer, "sCki", this.Cookie);
				VirtualListView2.RenderAttribute(writer, "iLcid", this.CookieLcid);
				VirtualListView2.RenderAttribute(writer, "sPfdDC", this.PreferredDC);
			}
		}

		private SortBy[] GetSortByProperties()
		{
			SortBy[] array;
			if (base.SortedColumn == ColumnId.FileAs)
			{
				array = new SortBy[]
				{
					new SortBy(ContactBaseSchema.FileAs, base.SortOrder)
				};
			}
			else if (base.SortedColumn == ColumnId.ContactFlagDueDate)
			{
				array = new SortBy[]
				{
					new SortBy(ItemSchema.FlagStatus, base.SortOrder),
					new SortBy(ItemSchema.UtcDueDate, (base.SortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending),
					new SortBy(ItemSchema.ItemColor, base.SortOrder),
					new SortBy(ContactBaseSchema.FileAs, base.SortOrder)
				};
			}
			else if (base.SortedColumn == ColumnId.ContactFlagStartDate)
			{
				array = new SortBy[]
				{
					new SortBy(ItemSchema.FlagStatus, base.SortOrder),
					new SortBy(ItemSchema.UtcStartDate, (base.SortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending),
					new SortBy(ItemSchema.ItemColor, base.SortOrder),
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
			return array;
		}

		private void UpdateCookieInformation()
		{
			if (!this.IsContactView)
			{
				ADListViewDataSource adlistViewDataSource = (ADListViewDataSource)base.Contents.DataSource;
				this.cookie = adlistViewDataSource.Cookie;
				this.cookieLcid = adlistViewDataSource.Lcid;
				this.preferredDC = adlistViewDataSource.PreferredDC;
			}
		}

		private const int MaxSearchStringLength = 256;

		private const string InvalidViewTypeMessage = "Invalid ViewType for AddressBookVirtualListView: {0}.Only ContactModule, ContactLookup, ContactPicker, DirectoryLookup, DirectoryPicker,RoomLookup, and RoomPicker are valid";

		private ViewType viewType;

		private SearchScope folderScope;

		private bool isPAA;

		private bool isMobile;

		private AddressBookBase addressBookBase;

		private string searchString;

		private string cookie = string.Empty;

		private int cookieIndex;

		private int cookieLcid;

		private string preferredDC = string.Empty;

		private Folder folder;

		private QueryFilter filter;
	}
}
