using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventSegmentation(Feature.Contacts)]
	internal abstract class ContactVirtualListViewEventHandler : FolderVirtualListViewEventHandler2
	{
		protected abstract ViewType ViewType { get; }

		public new static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(ContactModuleEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(ContactBrowserEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(ContactPickerEventHandler));
		}

		[OwaEventParameter("mbl", typeof(string), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEvent("LoadFresh")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SR", typeof(int))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		public override void LoadFresh()
		{
			base.InternalLoadFresh();
		}

		[OwaEventParameter("mbl", typeof(string), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("AId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEvent("LoadNext")]
		public override void LoadNext()
		{
			base.InternalLoadNext();
		}

		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEventParameter("mbl", typeof(string), false, true)]
		[OwaEvent("LoadPrevious")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("AId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		public override void LoadPrevious()
		{
			base.InternalLoadPrevious();
		}

		[OwaEventParameter("nwSel", typeof(bool), false, true)]
		[OwaEvent("SeekNext")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEventParameter("mbl", typeof(string), false, true)]
		public override void SeekNext()
		{
			base.InternalSeekNext();
		}

		[OwaEventParameter("RC", typeof(int))]
		[OwaEvent("SeekPrevious")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("nwSel", typeof(bool), false, true)]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEventParameter("mbl", typeof(string), false, true)]
		public override void SeekPrevious()
		{
			base.InternalSeekPrevious();
		}

		[OwaEventParameter("mbl", typeof(string), false, true)]
		[OwaEvent("Sort")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("SId", typeof(OwaStoreObjectId), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		public override void Sort()
		{
			base.InternalSort();
		}

		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEvent("SetML")]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("SR", typeof(int))]
		[OwaEventParameter("RC", typeof(int))]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("mbl", typeof(string), false, true)]
		public override void SetMultiLineState()
		{
			base.InternalSetMultiLineState();
		}

		[OwaEventParameter("RC", typeof(int))]
		[OwaEventVerb(OwaEventVerb.Post)]
		[OwaEventParameter("td", typeof(string))]
		[OwaEvent("TypeDown")]
		[OwaEventParameter("St", typeof(FolderVirtualListViewState))]
		[OwaEventParameter("srchf", typeof(FolderVirtualListViewSearchFilter), false, true)]
		[OwaEventParameter("fltr", typeof(int), false, true)]
		[OwaEventParameter("paa", typeof(int), false, true)]
		[OwaEventParameter("mbl", typeof(string), false, true)]
		public override void TypeDownSearch()
		{
			base.InternalTypeDownSearch();
		}

		protected override VirtualListView2 GetListView()
		{
			base.BindToFolder();
			bool isPAA = false;
			bool isMobile = false;
			if (base.IsParameterSet("paa"))
			{
				isPAA = true;
			}
			if (base.IsParameterSet("mbl"))
			{
				isMobile = true;
			}
			return new AddressBookVirtualListView(base.UserContext, "divVLV", this.ListViewState.IsMultiLine, this.ViewType, this.ListViewState.SortedColumn, this.ListViewState.SortOrder, base.IsFiltered, base.SearchScope, isPAA, isMobile, base.DataFolder, this.GetViewFilter());
		}

		protected override QueryFilter GetViewFilter()
		{
			ContactNavigationType filter = ContactNavigationType.All;
			if (base.IsParameterSet("fltr"))
			{
				filter = (ContactNavigationType)base.GetParameter("fltr");
			}
			base.FolderQueryFilter = ContactView.GetFilter(filter);
			return base.GetViewFilter();
		}

		protected override void RenderSearchInPublicFolder(TextWriter writer)
		{
			AdvancedFindComponents advancedFindComponents = AdvancedFindComponents.Categories | AdvancedFindComponents.SearchButton | AdvancedFindComponents.SearchTextInName;
			base.RenderAdvancedFind(this.Writer, advancedFindComponents, null);
		}

		protected override void RenderAdvancedFind(TextWriter writer, OwaStoreObjectId folderId)
		{
			base.RenderAdvancedFind(writer, AdvancedFindComponents.Categories, folderId);
		}
	}
}
