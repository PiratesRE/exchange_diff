using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class MoveItem : OwaPage, IRegistryOnlyForm
	{
		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		public NavigationModule Module
		{
			get
			{
				return this.module;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		protected string SelectedFolderIdString
		{
			get
			{
				return this.selectedFolderId.ToBase64String();
			}
		}

		protected bool ContainMruRadios
		{
			get
			{
				return this.mruFolderList != null && this.mruFolderList.Count != 0;
			}
		}

		protected Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		protected string HeaderLabelForMove
		{
			get
			{
				return this.headerLabelForMove;
			}
		}

		private static void RenderIconForItem(Item item, TextWriter writer, UserContext userContext)
		{
			int iconFlag = -1;
			bool isInConflict = false;
			bool isRead = false;
			if (item is MessageItem)
			{
				iconFlag = ItemUtility.GetProperty<int>(item, ItemSchema.IconIndex, -1);
				isInConflict = ItemUtility.GetProperty<bool>(item, MessageItemSchema.MessageInConflict, false);
				isRead = ItemUtility.GetProperty<bool>(item, MessageItemSchema.IsRead, false);
			}
			writer.Write("<img class=\"sI\" alt=\"\" src=\"");
			SmallIconManager.RenderItemIconUrl(writer, userContext, item.ClassName, null, isInConflict, isRead, iconFlag);
			writer.Write("\">");
		}

		protected string GetBackURL()
		{
			if (this.previousFormApplicationElement == ApplicationElement.Item)
			{
				StringBuilder stringBuilder = new StringBuilder(200);
				stringBuilder.Append("?ae=");
				stringBuilder.Append(this.previousFormApplicationElement);
				stringBuilder.Append("&t=");
				stringBuilder.Append(Utilities.UrlEncode(this.type));
				stringBuilder.Append("&id=");
				stringBuilder.Append(Utilities.UrlEncode(this.items[0].Id.ObjectId.ToBase64String()));
				return stringBuilder.ToString();
			}
			return base.UserContext.LastClientViewState.ToQueryString();
		}

		protected string GetMoveActionURL()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.Append("?ae=PreFormAction&a=Move&t=");
			stringBuilder.Append(Utilities.UrlEncode(this.type));
			stringBuilder.Append("&");
			stringBuilder.Append("fid");
			stringBuilder.Append("=");
			stringBuilder.Append(Utilities.UrlEncode(this.SelectedFolderIdString));
			return stringBuilder.ToString();
		}

		protected string GetFolderManagementURL()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.Append("?ae=Dialog&t=FolderManagement&m=");
			stringBuilder.Append((int)this.module);
			return stringBuilder.ToString();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.targetFolderId = RequestParser.GetTargetFolderIdFromQueryString(base.Request, false);
			this.selectedFolderId = RequestParser.GetFolderIdFromQueryString(base.Request, false);
			this.FetchModule();
			this.FetchSelectedItems();
			this.InitializeFolderList();
			this.folderDropDown = new FolderDropdown(base.UserContext);
		}

		protected void RenderNavigation()
		{
			Navigation navigation = new Navigation(this.module, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		protected void RenderSecondaryNavigation()
		{
			switch (this.module)
			{
			case NavigationModule.Mail:
			{
				MailSecondaryNavigation mailSecondaryNavigation = new MailSecondaryNavigation(base.OwaContext, this.selectedFolderId, this.allFolderList, this.mruFolderList, null);
				mailSecondaryNavigation.Render(base.Response.Output);
				return;
			}
			case NavigationModule.Calendar:
				break;
			case NavigationModule.Contacts:
			{
				ContactSecondaryNavigation contactSecondaryNavigation = new ContactSecondaryNavigation(base.OwaContext, this.selectedFolderId, this.contactFolderList);
				contactSecondaryNavigation.RenderContacts(base.Response.Output);
				break;
			}
			default:
				return;
			}
		}

		protected void RenderHeaderToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			toolbar.RenderButton(ToolbarButtons.CloseText);
			toolbar.RenderFill();
			toolbar.RenderButton(ToolbarButtons.CloseImage);
			toolbar.RenderEnd();
		}

		protected void RenderFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			toolbar.RenderEnd();
		}

		protected void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.None);
			optionsBar.Render(helpFile);
		}

		private void FetchModule()
		{
			this.type = base.OwaContext.FormsRegistryContext.Type;
			this.module = Navigation.GetNavigationModuleFromFolder(base.UserContext, this.selectedFolderId);
			switch (this.module)
			{
			case NavigationModule.Mail:
				this.headerLabelForMove = LocalizedStrings.GetHtmlEncoded(1182470434);
				return;
			case NavigationModule.Contacts:
				if (!base.UserContext.IsFeatureEnabled(Feature.Contacts))
				{
					throw new OwaSegmentationException("Contacts is not enabled");
				}
				this.headerLabelForMove = LocalizedStrings.GetHtmlEncoded(-1217485730);
				return;
			}
			throw new OwaInvalidRequestException("The " + this.module + " module is not supported to move items in Owa Basic");
		}

		private void InitializeFolderList()
		{
			if (this.Module == NavigationModule.Mail)
			{
				this.mruFolderList = new MruFolderList(base.UserContext);
				this.allFolderList = new FolderList(base.UserContext, base.UserContext.MailboxSession, null, 1024, true, null, FolderList.FolderPropertiesInBasic);
				return;
			}
			if (this.Module == NavigationModule.Contacts)
			{
				this.contactFolderList = new ContactFolderList(base.UserContext, this.selectedFolderId);
			}
		}

		private void FetchSelectedItems()
		{
			this.previousFormApplicationElement = MoveItemHelper.GetApplicationElementFromStoreType(this.type);
			if (!Utilities.IsPostRequest(base.Request))
			{
				if (this.previousFormApplicationElement != ApplicationElement.Item)
				{
					throw new OwaInvalidRequestException("GET request for move item page can only triggered from reading Item Page");
				}
				this.selectedItemsParameterIn = ParameterIn.QueryString;
				this.selectedItemsParameterName = "id";
			}
			else
			{
				this.selectedItemsParameterIn = ParameterIn.Form;
				if (base.IsPostFromMyself())
				{
					this.selectedItemsParameterName = "hidid";
				}
				else if (this.module == NavigationModule.Mail)
				{
					this.selectedItemsParameterName = "chkmsg";
				}
				else if (this.module == NavigationModule.Contacts)
				{
					this.selectedItemsParameterName = "chkRcpt";
				}
			}
			string text;
			if (this.selectedItemsParameterIn == ParameterIn.QueryString)
			{
				text = Utilities.GetQueryStringParameter(base.Request, this.selectedItemsParameterName, true);
			}
			else
			{
				text = Utilities.GetFormParameter(base.Request, this.selectedItemsParameterName, true);
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new OwaInvalidRequestException("No item is selected to be moved");
			}
			string[] array = text.Split(new char[]
			{
				','
			});
			if (base.UserContext.UserOptions.BasicViewRowCount < array.Length)
			{
				throw new OwaInvalidRequestException("According to the user's option, at most " + base.UserContext.UserOptions.BasicViewRowCount + " items are allow to move at a time");
			}
			this.items = new Item[array.Length];
			PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
			{
				ItemSchema.Id,
				ItemSchema.Subject,
				StoreObjectSchema.ItemClass,
				MessageItemSchema.IsRead,
				ItemSchema.IconIndex,
				MessageItemSchema.MessageInConflict,
				ContactBaseSchema.FileAs
			};
			for (int i = 0; i < array.Length; i++)
			{
				StoreObjectId storeId = Utilities.CreateStoreObjectId(base.UserContext.MailboxSession, array[i]);
				this.items[i] = Utilities.GetItem<Item>(base.UserContext, storeId, prefetchProperties);
			}
		}

		protected void RenderTargetFolderList(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<td class=\"ddt\" nowrap>");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-1166023766));
			writer.Write("</td>");
			writer.Write("<td class=\"dd\" nowrap>");
			writer.Write("<table cellspacing=0 cellpadding=0>");
			bool flag = false;
			if (this.Module == NavigationModule.Mail && this.mruFolderList != null)
			{
				for (int i = 0; i < this.mruFolderList.Count; i++)
				{
					writer.Write("<tr><td class=\"chkb\">");
					writer.Write("<input type=radio name=\"tfId\" onClick=\"return onClkRdo()\" value=\"");
					Utilities.HtmlEncode(this.mruFolderList[i].Id.ToBase64String(), writer);
					writer.Write("\"");
					if (this.targetFolderId != null && this.targetFolderId.Equals(this.mruFolderList[i].Id))
					{
						writer.Write(" checked=true");
						flag = true;
					}
					writer.Write(" id=\"rdomru");
					writer.Write(i + 1);
					writer.Write("\">");
					writer.Write("</td><td class=\"mru\" nowrap><label for=\"rdomru");
					writer.Write(i + 1);
					writer.Write("\"><a href=\"#\" onClick=\"return onClkMru(");
					writer.Write(i + 1);
					writer.Write(")\"><img src=\"");
					SmallIconManager.RenderFolderIconUrl(writer, base.OwaContext.UserContext, null);
					writer.Write("\" alt=\"\">");
					Utilities.CropAndRenderText(writer, this.mruFolderList[i].DisplayName, 24);
					writer.Write("</a></label></td><td></td></tr>");
				}
			}
			writer.Write("<tr><td>");
			string dropdownName = "tfId";
			StoreObjectId storeObjectId = flag ? null : this.targetFolderId;
			switch (this.Module)
			{
			case NavigationModule.Mail:
				if (this.mruFolderList != null && this.mruFolderList.Count != 0)
				{
					writer.Write("<input type=radio name=\"tfId\" id=\"rdofldlst\" onClick=\"return onClkRdo()\" ");
					if (this.targetFolderId != null && !flag)
					{
						writer.Write(" checked");
					}
					writer.Write("></td><td>");
					dropdownName = null;
				}
				if (storeObjectId == null)
				{
					storeObjectId = base.UserContext.InboxFolderId;
				}
				this.folderDropDown.RenderMailMove(this.allFolderList, storeObjectId, dropdownName, writer);
				break;
			case NavigationModule.Calendar:
				if (storeObjectId == null)
				{
					storeObjectId = base.UserContext.CalendarFolderId;
				}
				break;
			case NavigationModule.Contacts:
				if (storeObjectId == null)
				{
					storeObjectId = base.UserContext.ContactsFolderId;
				}
				this.folderDropDown.RenderContactMove(this.contactFolderList, storeObjectId, dropdownName, writer);
				break;
			}
			writer.Write("</td><td class=\"btn\" align=\"left\">");
			bool flag2 = this.mruFolderList == null || this.mruFolderList.Count == 0 || this.targetFolderId != null;
			writer.Write("<a href=\"#\" onClick=\"return onClkMv();\" onKeyPress=\"return onKPMv(event);\" id=\"btnmv\"");
			if (!flag2)
			{
				writer.Write(" class=\"fmbtn fmbtnDis\"");
			}
			else
			{
				writer.Write(" class=\"fmbtn fmbtnEnb\"");
			}
			writer.Write(">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(1414245993));
			writer.Write("</a></td></tr></table></td>");
		}

		protected void RenderSelectedItems(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			for (int i = 0; i < this.items.Length; i++)
			{
				if (i != 0)
				{
					writer.Write("; ");
				}
				Item item = this.items[i];
				MoveItem.RenderIconForItem(item, writer, base.UserContext);
				string text;
				if (item is Contact)
				{
					text = ItemUtility.GetProperty<string>(item, ContactBaseSchema.FileAs, string.Empty);
					if (string.IsNullOrEmpty(text))
					{
						text = LocalizedStrings.GetNonEncoded(-808148510);
					}
				}
				else
				{
					text = ItemUtility.GetProperty<string>(item, ItemSchema.Subject, string.Empty);
					if (string.IsNullOrEmpty(text))
					{
						text = LocalizedStrings.GetNonEncoded(730745110);
					}
				}
				writer.Write("&nbsp;");
				Utilities.CropAndRenderText(writer, text, 32);
				writer.Write("<input type=hidden name=\"");
				writer.Write("hidid");
				writer.Write("\" value=\"");
				Utilities.HtmlEncode(item.Id.ObjectId.ToBase64String(), writer);
				writer.Write("\">");
				writer.Write("<input type=hidden name=\"");
				writer.Write("hidt");
				writer.Write("\" value=\"");
				Utilities.HtmlEncode(item.ClassName, writer);
				writer.Write("\">");
			}
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.items == null)
			{
				return;
			}
			for (int i = 0; i < this.items.Length; i++)
			{
				if (this.items[i] != null)
				{
					this.items[i].Dispose();
				}
			}
			base.OnUnload(e);
		}

		private const string CheckedMessageParameterName = "chkmsg";

		private const string CheckedContactParameterName = "chkRcpt";

		private const string ItemIdQueryStringParameterName = "id";

		private string selectedItemsParameterName;

		private ParameterIn selectedItemsParameterIn;

		private NavigationModule module;

		private ApplicationElement previousFormApplicationElement;

		private string type;

		private Item[] items;

		private StoreObjectId selectedFolderId;

		private StoreObjectId targetFolderId;

		private Infobar infobar = new Infobar();

		private string headerLabelForMove;

		private MruFolderList mruFolderList;

		private FolderList allFolderList;

		private ContactFolderList contactFolderList;

		private FolderDropdown folderDropDown;
	}
}
