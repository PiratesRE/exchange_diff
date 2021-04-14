using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class ContactView : ListViewPage, IRegistryOnlyForm
	{
		internal override StoreObjectId DefaultFolderId
		{
			get
			{
				return base.UserContext.ContactsFolderId;
			}
		}

		protected override SortOrder DefaultSortOrder
		{
			get
			{
				return SortOrder.Descending;
			}
		}

		protected override ColumnId DefaultSortedColumn
		{
			get
			{
				return ColumnId.FileAs;
			}
		}

		public string UrlEncodedFolderId
		{
			get
			{
				return HttpUtility.UrlEncode(base.Folder.Id.ObjectId.ToBase64String());
			}
		}

		protected override string CheckBoxId
		{
			get
			{
				return "chkRcpt";
			}
		}

		public string ApplicationElement
		{
			get
			{
				return Convert.ToString(base.OwaContext.FormsRegistryContext.ApplicationElement);
			}
		}

		public string Type
		{
			get
			{
				if (base.OwaContext.FormsRegistryContext.Type != null)
				{
					return base.OwaContext.FormsRegistryContext.Type;
				}
				return string.Empty;
			}
		}

		public ContactView() : base(ExTraceGlobals.ContactsCallTracer, ExTraceGlobals.ContactsTracer)
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (base.UserContext.IsWebPartRequest)
			{
				base.UserContext.LastClientViewState = new WebPartModuleViewState(base.FolderId, base.Folder.ClassName, base.PageNumber, NavigationModule.Contacts, base.SortOrder, base.SortedColumn);
				return;
			}
			if (base.FilteredView)
			{
				base.UserContext.LastClientViewState = new ContactModuleSearchViewState(base.UserContext.LastClientViewState, base.FolderId, base.Folder.ClassName, base.PageNumber, base.SearchString, base.SearchScope);
				return;
			}
			base.UserContext.LastClientViewState = new ContactModuleViewState(base.FolderId, base.Folder.ClassName, base.PageNumber);
		}

		protected override void CreateListView(ColumnId sortColumn, SortOrder sortOrder)
		{
			if (!base.FilteredView)
			{
				base.ListView = new ContactsListView(base.UserContext, sortColumn, sortOrder, base.Folder);
			}
			else
			{
				base.ListView = new ContactsListView(base.UserContext, sortColumn, sortOrder, base.SearchFolder, base.SearchScope);
			}
			base.InitializeListView();
		}

		protected override SanitizedHtmlString BuildConcretSearchInfobarMessage(int resultsCount, SanitizedHtmlString clearSearchLink)
		{
			return SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1403744948), new object[]
			{
				resultsCount,
				base.SearchString,
				clearSearchLink
			});
		}

		public void RenderContactsSecondaryNavigation()
		{
			ContactSecondaryNavigation contactSecondaryNavigation = new ContactSecondaryNavigation(base.OwaContext, base.Folder.Id.ObjectId, null);
			contactSecondaryNavigation.RenderContacts(base.Response.Output);
		}

		public void RenderHeaderToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			toolbar.RenderButton(ToolbarButtons.NewContact);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.Move);
			toolbar.RenderButton(ToolbarButtons.Delete);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.SendEmailToContact);
			if (base.UserContext.IsFeatureEnabled(Feature.Calendar))
			{
				toolbar.RenderSpace();
				toolbar.RenderButton(ToolbarButtons.SendMeetingRequestToContact);
			}
			toolbar.RenderFill();
			base.RenderPaging(false);
			toolbar.RenderEnd();
		}

		public void RenderFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			if (!base.UserContext.IsWebPartRequest)
			{
				toolbar.RenderButton(ToolbarButtons.Move);
			}
			toolbar.RenderButton(ToolbarButtons.Delete);
			toolbar.RenderFill();
			base.RenderPaging(true);
			toolbar.RenderEnd();
		}

		protected void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.Contacts, OptionsBar.RenderingFlags.ShowSearchContext, OptionsBar.BuildFolderSearchUrlSuffix(base.UserContext, base.FolderId));
			optionsBar.Render(helpFile);
		}

		private const string RecipientCheckBox = "chkRcpt";
	}
}
