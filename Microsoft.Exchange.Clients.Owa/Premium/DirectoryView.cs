using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class DirectoryView : ListViewSubPage, IRegistryOnlyForm
	{
		public DirectoryView() : base(ExTraceGlobals.DirectoryCallTracer, ExTraceGlobals.DirectoryTracer)
		{
		}

		protected string AddressListName
		{
			get
			{
				return this.addressBookBase.DisplayName;
			}
		}

		protected override int ViewWidth
		{
			get
			{
				return Math.Max(365, 325);
			}
		}

		protected override int ViewHeight
		{
			get
			{
				return 250;
			}
		}

		protected override SortOrder DefaultSortOrder
		{
			get
			{
				return SortOrder.Ascending;
			}
		}

		protected override SortOrder SortOrder
		{
			get
			{
				return this.DefaultSortOrder;
			}
		}

		protected override ColumnId DefaultSortedColumn
		{
			get
			{
				return ColumnId.DisplayNameAD;
			}
		}

		protected override ColumnId SortedColumn
		{
			get
			{
				return this.DefaultSortedColumn;
			}
		}

		protected override ReadingPanePosition DefaultReadingPanePosition
		{
			get
			{
				if (this.IsPicker)
				{
					return ReadingPanePosition.Right;
				}
				return AddressBookViewState.DefaultReadingPanePosition;
			}
		}

		protected override ReadingPanePosition ReadingPanePosition
		{
			get
			{
				return this.addressBookViewState.ReadingPanePosition;
			}
		}

		protected override bool DefaultMultiLineSetting
		{
			get
			{
				return this.addressBookViewState.DefaultMultiLineSetting;
			}
		}

		protected override bool IsMultiLine
		{
			get
			{
				return true;
			}
		}

		protected override bool FindBarOn
		{
			get
			{
				return this.addressBookViewState.FindBarOn;
			}
		}

		protected override bool AllowAdvancedSearch
		{
			get
			{
				return false;
			}
		}

		protected override bool RenderSearchDropDown
		{
			get
			{
				return false;
			}
		}

		protected AddressBookContextMenu ContextMenu
		{
			get
			{
				return this.contextMenu;
			}
		}

		private bool IsPicker
		{
			get
			{
				return (this.type & DirectoryView.Type.Picker) != DirectoryView.Type.None;
			}
		}

		private bool IsDirectoryPaaPicker
		{
			get
			{
				return (this.type & DirectoryView.Type.PaaPicker) != DirectoryView.Type.None;
			}
		}

		private bool IsRoomView
		{
			get
			{
				return (this.type & DirectoryView.Type.Rooms) != DirectoryView.Type.None;
			}
		}

		protected override bool ShouldRenderToolbar
		{
			get
			{
				return true;
			}
		}

		protected override void LoadViewState()
		{
			this.addressBookViewState = AddressBookViewState.Load(base.UserContext, this.IsPicker, this.IsRoomView);
		}

		protected override IListView CreateListView(ColumnId sortedColumn, SortOrder sortOrder)
		{
			bool isMobile = Utilities.IsFlagSet((int)this.type, 8);
			AddressBookVirtualListView addressBookVirtualListView = new AddressBookVirtualListView(base.UserContext, "divVLV", this.IsMultiLine, this.viewType, sortedColumn, sortOrder, base.IsPersonalAutoAttendantPicker || this.IsDirectoryPaaPicker, isMobile, this.addressBookBase);
			addressBookVirtualListView.LoadData(0, 50);
			return addressBookVirtualListView;
		}

		protected override IListViewDataSource CreateDataSource(ListView listView)
		{
			throw new NotImplementedException();
		}

		protected override Toolbar CreateListToolbar()
		{
			return new AddressBookViewListToolbar(this.IsMultiLine, this.ReadingPanePosition);
		}

		protected override Toolbar CreateActionToolbar()
		{
			return new AddressBookViewActionToolbar();
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.contextMenu = new AddressBookContextMenu(base.UserContext, true, false);
			string text = base.OwaContext.FormsRegistryContext.Type;
			string action = base.OwaContext.FormsRegistryContext.Action;
			if (text != null)
			{
				object obj = DirectoryView.elementClassParser.Parse(text);
				if (obj != null && (DirectoryView.ElementClass)obj == DirectoryView.ElementClass.Rooms)
				{
					this.type |= DirectoryView.Type.Rooms;
					this.viewType = ViewType.RoomBrowser;
				}
			}
			if (!string.IsNullOrEmpty(action))
			{
				if (string.Equals(action, "Pick", StringComparison.OrdinalIgnoreCase))
				{
					this.viewType = ((DirectoryView.Type.Rooms == this.type) ? ViewType.RoomPicker : ViewType.DirectoryPicker);
					this.type |= DirectoryView.Type.Picker;
				}
				else if (string.Equals(action, "PickPaa", StringComparison.OrdinalIgnoreCase))
				{
					this.type |= (DirectoryView.Type.Picker | DirectoryView.Type.PaaPicker);
					this.viewType = ViewType.DirectoryPicker;
				}
				else if (string.Equals(action, "PickMobile", StringComparison.OrdinalIgnoreCase))
				{
					this.type |= DirectoryView.Type.Mobile;
					this.viewType = ViewType.DirectoryPicker;
				}
			}
			if (string.IsNullOrEmpty(base.SerializedContainerId))
			{
				if (this.IsRoomView && this.IsPicker && DirectoryAssistance.IsRoomsAddressListAvailable(base.UserContext))
				{
					this.addressBookBase = base.UserContext.AllRoomsAddressBookInfo.ToAddressBookBase();
					return;
				}
				this.addressBookBase = base.UserContext.GlobalAddressListInfo.ToAddressBookBase();
				return;
			}
			else
			{
				if (base.UserContext.GlobalAddressListInfo.Origin == GlobalAddressListInfo.GalOrigin.DefaultGlobalAddressList)
				{
					this.addressBookBase = DirectoryAssistance.FindAddressBook(base.SerializedContainerId, base.UserContext);
					return;
				}
				if (base.UserContext.GlobalAddressListInfo.Id.Equals(DirectoryAssistance.ParseADObjectId(base.SerializedContainerId)))
				{
					this.addressBookBase = base.UserContext.GlobalAddressListInfo.ToAddressBookBase();
					return;
				}
				throw new OwaInvalidRequestException("Address Book Serialized Id is unsupported " + base.SerializedContainerId);
			}
		}

		internal static void RenderSecondaryNavigation(TextWriter output, UserContext userContext)
		{
			DirectoryView.RenderSecondaryNavigation(output, userContext, false);
		}

		internal static void RenderSecondaryNavigation(TextWriter output, UserContext userContext, bool isRoomPicker)
		{
			SecondaryNavigationDirectoryList secondaryNavigationDirectoryList = SecondaryNavigationDirectoryList.CreateCondensedDirectoryList(userContext, isRoomPicker);
			secondaryNavigationDirectoryList.Render(output);
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
				return new SanitizedHtmlString(this.AddressListName);
			}
		}

		public override string PageType
		{
			get
			{
				return "DirectoryView";
			}
		}

		private static FastEnumParser elementClassParser = new FastEnumParser(typeof(DirectoryView.ElementClass), true);

		private DirectoryView.Type type;

		private ViewType viewType = ViewType.DirectoryBrowser;

		private AddressBookViewState addressBookViewState;

		private AddressBookBase addressBookBase;

		private AddressBookContextMenu contextMenu;

		private string[] externalScriptFiles = new string[]
		{
			"uview.js",
			"vlv.js"
		};

		private enum ElementClass
		{
			Recipients,
			Rooms
		}

		[Flags]
		private enum Type
		{
			None = 0,
			Picker = 1,
			Rooms = 2,
			PaaPicker = 4,
			Mobile = 8
		}
	}
}
