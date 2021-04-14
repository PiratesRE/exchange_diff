using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ContactView : FolderListViewSubPage, IRegistryOnlyForm
	{
		public ContactView() : base(ExTraceGlobals.ContactsCallTracer, ExTraceGlobals.ContactsTracer)
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.arrangeByMenu = new PersonViewArrangeByMenu();
		}

		protected PersonViewArrangeByMenu ArrangeByMenu
		{
			get
			{
				return this.arrangeByMenu;
			}
		}

		protected override string ContainerName
		{
			get
			{
				if (base.IsArchiveMailboxFolder)
				{
					return string.Format(LocalizedStrings.GetNonEncoded(-83764036), base.Folder.DisplayName, Utilities.GetMailboxOwnerDisplayName((MailboxSession)base.Folder.Session));
				}
				return base.ContainerName;
			}
		}

		internal override StoreObjectId DefaultFolderId
		{
			get
			{
				return base.UserContext.ContactsFolderId;
			}
		}

		protected override bool ShouldRenderELCCommentAndQuotaLink
		{
			get
			{
				return false;
			}
		}

		protected override int ViewWidth
		{
			get
			{
				if (this.addressBookViewState == null)
				{
					return base.ViewWidth;
				}
				return Math.Max(365, 325);
			}
		}

		protected override int ViewHeight
		{
			get
			{
				if (this.addressBookViewState != null)
				{
					return 250;
				}
				return base.ViewHeight;
			}
		}

		protected AddressBookContextMenu ContextMenu
		{
			get
			{
				if (this.contextMenu == null)
				{
					this.contextMenu = new AddressBookContextMenu(base.UserContext, this.InAddressBook, true);
				}
				return this.contextMenu;
			}
		}

		protected override SortOrder DefaultSortOrder
		{
			get
			{
				return SortOrder.Ascending;
			}
		}

		protected override ColumnId DefaultSortedColumn
		{
			get
			{
				return ColumnId.FileAs;
			}
		}

		protected override ReadingPanePosition DefaultReadingPanePosition
		{
			get
			{
				return AddressBookViewState.DefaultReadingPanePosition;
			}
		}

		protected override ReadingPanePosition ReadingPanePosition
		{
			get
			{
				if (this.addressBookViewState != null)
				{
					return this.addressBookViewState.ReadingPanePosition;
				}
				return base.ReadingPanePosition;
			}
		}

		protected override bool DefaultMultiLineSetting
		{
			get
			{
				return this.addressBookViewState == null || this.addressBookViewState.DefaultMultiLineSetting;
			}
		}

		protected override bool IsMultiLine
		{
			get
			{
				if (this.addressBookViewState != null)
				{
					return this.addressBookViewState.IsMultiLine;
				}
				return base.IsMultiLine;
			}
		}

		protected override bool FindBarOn
		{
			get
			{
				if (base.IsPublicFolder)
				{
					return false;
				}
				if (this.addressBookViewState != null)
				{
					return this.addressBookViewState.FindBarOn;
				}
				return base.UserContext.UserOptions.ContactsFindBarOn;
			}
		}

		protected bool InAddressBook
		{
			get
			{
				return this.location != ContactView.Location.ContactModule;
			}
		}

		protected static int StoreObjectTypeContactsFolder
		{
			get
			{
				return 3;
			}
		}

		protected override void LoadViewState()
		{
			base.LoadViewState();
			if (this.InAddressBook)
			{
				this.addressBookViewState = AddressBookViewState.Load(base.UserContext, this.location == ContactView.Location.AddressBookPicker, false);
			}
		}

		protected override IListView CreateListView(ColumnId sortedColumn, SortOrder sortOrder)
		{
			AddressBookVirtualListView addressBookVirtualListView = new AddressBookVirtualListView(base.UserContext, "divVLV", this.IsMultiLine, ContactView.viewType[(int)this.location], sortedColumn, sortOrder, this.isPaaPkr, this.isMobilePicker, base.Folder, ContactView.GetFilter(this.filter));
			VirtualListView2 virtualListView = addressBookVirtualListView;
			string name = "iFltr";
			int num = (int)this.filter;
			virtualListView.AddAttribute(name, num.ToString(CultureInfo.InvariantCulture));
			addressBookVirtualListView.LoadData(0, 50);
			return addressBookVirtualListView;
		}

		protected override IListViewDataSource CreateDataSource(ListView listView)
		{
			return new FolderListViewDataSource(base.UserContext, listView.Properties, base.Folder, listView.GetSortByProperties(), ContactView.GetFilter(this.filter));
		}

		protected override Toolbar CreateListToolbar()
		{
			Toolbar result;
			if (this.addressBookViewState != null)
			{
				result = new AddressBookViewListToolbar(this.IsMultiLine, this.ReadingPanePosition);
			}
			else
			{
				result = new PersonViewListToolbar(this.IsMultiLine, base.IsPublicFolder, base.UserContext.IsWebPartRequest, this.ReadingPanePosition);
			}
			return result;
		}

		protected override Toolbar CreateActionToolbar()
		{
			if (this.addressBookViewState != null)
			{
				return new AddressBookViewActionToolbar();
			}
			return new PersonViewActionToolbar();
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.DetermineContext();
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "fltr", false);
			int num;
			if (string.IsNullOrEmpty(queryStringParameter) || !int.TryParse(queryStringParameter, NumberStyles.Integer, CultureInfo.InvariantCulture, out num) || num < 1 || num > 3)
			{
				this.filter = ContactNavigationType.All;
				return;
			}
			this.filter = (ContactNavigationType)num;
		}

		internal static QueryFilter GetFilter(ContactNavigationType filter)
		{
			QueryFilter result = null;
			if (ContactNavigationType.People == filter)
			{
				result = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Contact");
			}
			else if (ContactNavigationType.DistributionList == filter)
			{
				result = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.DistList");
			}
			return result;
		}

		internal static void RenderSecondaryNavigation(TextWriter output, UserContext userContext, bool isPicker)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (!isPicker)
			{
				ContactView.RenderSecondaryNavigationFilter(output, "divCntFlt");
			}
			NavigationHost.RenderNavigationTreeControl(output, userContext, NavigationModule.Contacts);
		}

		internal static void RenderSecondaryNavigationFilter(TextWriter output, string filterId)
		{
			SecondaryNavigationFilter secondaryNavigationFilter = new SecondaryNavigationFilter(filterId, LocalizedStrings.GetNonEncoded(-428271462), "onClkCntF(\"" + Utilities.JavascriptEncode(filterId) + "\")");
			secondaryNavigationFilter.AddFilter(LocalizedStrings.GetNonEncoded(-1069600488), 1, true);
			secondaryNavigationFilter.AddFilter(LocalizedStrings.GetNonEncoded(-1434067361), 2, false);
			secondaryNavigationFilter.AddFilter(LocalizedStrings.GetNonEncoded(171820412), 3, false);
			secondaryNavigationFilter.Render(output);
		}

		private void DetermineContext()
		{
			this.location = ContactView.Location.ContactModule;
			string state = base.OwaContext.FormsRegistryContext.State;
			this.action = base.OwaContext.FormsRegistryContext.Action;
			if (string.CompareOrdinal(this.action, "PAA") == 0)
			{
				this.isPaaPkr = true;
			}
			else if (string.CompareOrdinal(this.action, "Mobile") == 0)
			{
				this.isMobilePicker = true;
			}
			if (state != null)
			{
				object obj = ContactView.locationParser.Parse(state);
				this.location = ((obj != null) ? ((ContactView.Location)obj) : this.location);
			}
		}

		protected override bool ShouldRenderToolbar
		{
			get
			{
				return true;
			}
		}

		public override IEnumerable<string> ExternalScriptFiles
		{
			get
			{
				return this.externalScriptFiles;
			}
		}

		public override SanitizedHtmlString Title
		{
			get
			{
				return new SanitizedHtmlString(this.ContainerName);
			}
		}

		public override string PageType
		{
			get
			{
				return "ContactViewPage";
			}
		}

		private const string FilterString = "fltr";

		private static readonly ViewType[] viewType = new ViewType[]
		{
			ViewType.ContactModule,
			ViewType.ContactBrowser,
			ViewType.ContactPicker
		};

		private static readonly FastEnumParser locationParser = new FastEnumParser(typeof(ContactView.Location), true);

		private static readonly PropertyDefinition[] requiredProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.DisplayName,
			FolderSchema.ItemCount
		};

		private PersonViewArrangeByMenu arrangeByMenu;

		private AddressBookViewState addressBookViewState;

		private ContactView.Location location;

		private ContactNavigationType filter = ContactNavigationType.All;

		private string action;

		private bool isPaaPkr;

		private bool isMobilePicker;

		private AddressBookContextMenu contextMenu;

		private string[] externalScriptFiles = new string[]
		{
			"uview.js",
			"contactvw.js",
			"vlv.js"
		};

		public enum Location
		{
			ContactModule,
			AddressBookBrowse,
			AddressBookPicker
		}
	}
}
