using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal abstract class DirectoryVirtualListViewEventHandler : VirtualListViewEventHandler2
	{
		protected abstract ViewType ViewType { get; }

		protected override VirtualListViewState ListViewState
		{
			get
			{
				return this.DirectoryVirtualListViewState;
			}
		}

		private DirectoryVirtualListViewState DirectoryVirtualListViewState
		{
			get
			{
				return (DirectoryVirtualListViewState)base.GetParameter("St");
			}
		}

		public static void Register()
		{
			OwaEventRegistry.RegisterStruct(typeof(DirectoryVirtualListViewState));
			OwaEventRegistry.RegisterHandler(typeof(DirectoryBrowserEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(DirectoryPickerEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(RoomBrowserEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(RoomPickerEventHandler));
		}

		[OwaEvent("LoadFresh")]
		[OwaEventParameter("mbl", typeof(string), false, true)]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SR", typeof(int))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(DirectoryVirtualListViewState))]
		[OwaEventParameter("fltr", typeof(string), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		public override void LoadFresh()
		{
			base.InternalLoadFresh();
		}

		[OwaEventParameter("mbl", typeof(string), false, true)]
		[OwaEvent("LoadNext")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("AId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(DirectoryVirtualListViewState))]
		[OwaEventParameter("fltr", typeof(string), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		public override void LoadNext()
		{
			base.InternalLoadNext();
		}

		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEvent("LoadPrevious")]
		[OwaEventParameter("AId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(DirectoryVirtualListViewState))]
		[OwaEventParameter("fltr", typeof(string), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEventParameter("mbl", typeof(string), false, true)]
		public override void LoadPrevious()
		{
			base.InternalLoadPrevious();
		}

		[OwaEvent("SeekNext")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(DirectoryVirtualListViewState))]
		[OwaEventParameter("fltr", typeof(string), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEventParameter("mbl", typeof(string), false, true)]
		[OwaEventParameter("nwSel", typeof(bool), false, true)]
		public override void SeekNext()
		{
			base.InternalSeekNext();
		}

		[OwaEventParameter("RC", typeof(int))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SId", typeof(OwaStoreObjectId))]
		[OwaEvent("SeekPrevious")]
		[OwaEventParameter("St", typeof(DirectoryVirtualListViewState))]
		[OwaEventParameter("fltr", typeof(string), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEventParameter("mbl", typeof(string), false, true)]
		[OwaEventParameter("nwSel", typeof(bool), false, true)]
		public override void SeekPrevious()
		{
			base.InternalSeekPrevious();
		}

		[OwaEventParameter("St", typeof(DirectoryVirtualListViewState))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEvent("Sort")]
		[OwaEventParameter("SId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("fltr", typeof(string), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEventParameter("mbl", typeof(string), false, true)]
		public override void Sort()
		{
			throw new NotImplementedException();
		}

		[OwaEvent("SetML")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SR", typeof(int))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(DirectoryVirtualListViewState))]
		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEventParameter("mbl", typeof(string), false, true)]
		public override void SetMultiLineState()
		{
			base.InternalSetMultiLineState();
		}

		[OwaEventParameter("mbl", typeof(string), false, true)]
		[OwaEventParameter("St", typeof(DirectoryVirtualListViewState))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("td", typeof(string))]
		[OwaEventParameter("fltr", typeof(string), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEvent("TypeDown")]
		public override void TypeDownSearch()
		{
			base.InternalTypeDownSearch();
		}

		protected void BindToAddressBook()
		{
			if (this.addressBookBase == null)
			{
				ADObjectId adobjectId = (ADObjectId)this.ListViewState.SourceContainerId;
				this.addressBookBase = DirectoryAssistance.FindAddressBook(adobjectId.ObjectGuid, base.UserContext);
			}
		}

		protected override VirtualListView2 GetListView()
		{
			this.BindToAddressBook();
			bool isPAA = base.IsParameterSet("paa");
			bool isMobile = base.IsParameterSet("mbl");
			string cookie = this.DirectoryVirtualListViewState.Cookie;
			string preferredDC = this.DirectoryVirtualListViewState.PreferredDC;
			int cookieLcid = this.DirectoryVirtualListViewState.CookieLcid;
			string searchString = base.GetParameter("fltr") as string;
			int cookieIndex = this.DirectoryVirtualListViewState.CookieIndex;
			return new AddressBookVirtualListView(base.UserContext, "divVLV", this.ListViewState.IsMultiLine, this.ViewType, ColumnId.DisplayNameAD, SortOrder.Ascending, isPAA, isMobile, this.addressBookBase, searchString, cookie, cookieIndex, cookieLcid, preferredDC);
		}

		[OwaEventParameter("s", typeof(ReadingPanePosition))]
		[OwaEvent("PersistReadingPane")]
		public void PersistReadingPane()
		{
			if (!base.UserContext.IsWebPartRequest)
			{
				this.PersistReadingPane((ReadingPanePosition)base.GetParameter("s"));
			}
		}

		[OwaEventSegmentation(Feature.AddressLists)]
		[OwaEvent("AllAddressList")]
		public void GetAllAddressList()
		{
			this.RenderAllAddressBooks();
		}

		protected virtual void PersistReadingPane(ReadingPanePosition readingPanePosition)
		{
		}

		private void RenderAllAddressBooks()
		{
			SecondaryNavigationDirectoryList secondaryNavigationDirectoryList = SecondaryNavigationDirectoryList.CreateExtendedDirectoryList(base.UserContext);
			secondaryNavigationDirectoryList.RenderEntries(this.Writer);
		}

		public const string MethodPersistReadingPane = "PersistReadingPane";

		public const string MethodGetAllAddressList = "AllAddressList";

		private AddressBookBase addressBookBase;
	}
}
