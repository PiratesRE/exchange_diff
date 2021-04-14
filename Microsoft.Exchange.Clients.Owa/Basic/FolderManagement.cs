using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class FolderManagement : OwaPage
	{
		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected NavigationModule Module
		{
			get
			{
				return this.module;
			}
		}

		protected string BackUrl
		{
			get
			{
				return base.UserContext.LastClientViewState.ToQueryString();
			}
		}

		public string SelectedFolderId
		{
			get
			{
				return this.selectedFolderId.ToBase64String();
			}
		}

		public Infobar Infobar
		{
			get
			{
				return this.infobar;
			}
		}

		private static void RenderLeadingText(Strings.IDs text, TextWriter writer)
		{
			writer.Write("<td class=\"ddtfm\" nowrap>");
			writer.Write(LocalizedStrings.GetHtmlEncoded(text));
			writer.Write("</td>");
		}

		private static void RenderTextInput(string name, string onKeyUpEvent, string onBlurEvent, string onKeyPressEvent, TextWriter writer)
		{
			writer.Write("<td class=\"dd\"><input maxlength=");
			writer.Write(256);
			writer.Write(" type=\"text\" name=\"");
			writer.Write(name);
			writer.Write("\" id=\"");
			writer.Write(name);
			writer.Write("\" class=\"fldnm\" onkeyup=\"");
			writer.Write(onKeyUpEvent);
			writer.Write("\" onblur=\"");
			writer.Write(onBlurEvent);
			writer.Write("\" onkeypress=\"");
			writer.Write(onKeyPressEvent);
			writer.Write("\"></td>");
		}

		private static void RenderButton(Strings.IDs label, string id, string onClickEvent, TextWriter writer)
		{
			writer.Write("<td class=\"btn\"><a href=\"#\" id=");
			writer.Write(id);
			writer.Write(" class=\"fmbtn fmbtnDis\" onClick=\"");
			writer.Write(onClickEvent);
			writer.Write("\">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(label));
			writer.Write("</a></td>");
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.folderDropdown = new FolderDropdown(base.UserContext);
			this.selectedFolderId = RequestParser.GetFolderIdFromQueryString(base.Request, false);
			if (this.selectedFolderId == null)
			{
				ModuleViewState moduleViewState = base.UserContext.LastClientViewState as ModuleViewState;
				if (moduleViewState != null)
				{
					this.selectedFolderId = moduleViewState.FolderId;
				}
			}
			if (this.selectedFolderId == null)
			{
				switch (this.module)
				{
				case NavigationModule.Mail:
					this.selectedFolderId = base.UserContext.InboxFolderId;
					break;
				case NavigationModule.Calendar:
					this.selectedFolderId = base.UserContext.CalendarFolderId;
					break;
				case NavigationModule.Contacts:
					this.selectedFolderId = base.UserContext.ContactsFolderId;
					break;
				}
			}
			this.module = RequestParser.GetNavigationModuleFromQueryString(base.Request, NavigationModule.Mail, true);
			if ((this.module == NavigationModule.Calendar && !base.UserContext.IsFeatureEnabled(Feature.Calendar)) || (this.module == NavigationModule.Contacts && !base.UserContext.IsFeatureEnabled(Feature.Contacts)))
			{
				throw new OwaSegmentationException("The " + this.module.ToString() + " feature is disabled");
			}
			this.InitializeFolderList();
		}

		private void InitializeFolderList()
		{
			this.allFolderList = new FolderList(base.UserContext, base.UserContext.MailboxSession, null, 1024, true, null, FolderList.FolderPropertiesInBasic);
		}

		private void RenderTitle(Strings.IDs title, TextWriter writer)
		{
			writer.Write("<tr><td class=\"hdl\" colspan=3>");
			writer.Write(LocalizedStrings.GetHtmlEncoded(title));
			writer.Write("</td></tr>");
			writer.Write("<tr><td colspan=3 class=dvdr>");
			RenderingUtilities.RenderHorizontalDividerForFolderManagerForm(base.UserContext, writer);
			writer.Write("</td></tr>");
		}

		private void RenderCreateMailFolder(TextWriter writer)
		{
			this.RenderTitle(-1171996716, writer);
			writer.Write("<tr>");
			FolderManagement.RenderLeadingText(1058761412, writer);
			writer.Write("<td class=\"dd\" nowrap><table class=\"tblIcnSel\" cellspacing=0 cellpadding=0><tr><td><img src=\"");
			base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.EMail2Small);
			writer.Write("\" alt=\"\"></td><td class=\"tdNewFldSel\">");
			this.folderDropdown.RenderMailFolderToCreateIn(this.allFolderList, this.selectedFolderId, writer);
			writer.Write("</td></tr></table></td><td class=\"btn\"></td>");
			writer.Write("</tr><tr>");
			FolderManagement.RenderLeadingText(-868987232, writer);
			FolderManagement.RenderTextInput("nnfc", "onKUFNCr()", "onBlurFNCr()", "onKPFNCr(event)", writer);
			FolderManagement.RenderButton(-119614694, "btnCr", "return onClkCr()", writer);
			writer.Write("</tr>");
		}

		private void RenderRenameMailFolder(TextWriter writer)
		{
			this.RenderTitle(-1362943956, writer);
			writer.Write("<tr>");
			FolderManagement.RenderLeadingText(824414759, writer);
			writer.Write("<td class=\"dd\" nowrap>");
			this.folderDropdown.RenderMailFolderToRename(this.allFolderList, this.selectedFolderId, writer);
			writer.Write("</td><td class=\"btn\"></td>");
			writer.Write("</tr><tr>");
			FolderManagement.RenderLeadingText(437857602, writer);
			FolderManagement.RenderTextInput("nnfr", "onKUFNRn()", "onBlurFNRn()", "return onKPFNRn(event)", writer);
			FolderManagement.RenderButton(461135208, "btnRn", "return onClkRn()", writer);
		}

		private void RenderMoveMailFolder(TextWriter writer)
		{
			this.RenderTitle(1847000671, writer);
			writer.Write("<tr>");
			FolderManagement.RenderLeadingText(-1506697407, writer);
			writer.Write("<td class=\"dd\" nowrap>");
			this.folderDropdown.RenderMailFolderToMove(this.allFolderList, this.selectedFolderId, writer);
			writer.Write("</td><td class=\"btn\"></td>");
			writer.Write("</tr><tr>");
			FolderManagement.RenderLeadingText(1349630, writer);
			writer.Write("<td class=\"dd\">");
			this.folderDropdown.RenderMailNewLocationForMove(this.allFolderList, this.selectedFolderId, writer);
			FolderManagement.RenderButton(1414245993, "btnMv", "return onClkMvFld()", writer);
			writer.Write("</tr>");
		}

		private void RenderDeleteFolder(TextWriter writer)
		{
			this.RenderTitle(677440499, writer);
			writer.Write("<tr><td colspan=3 class=info>");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-404811311));
			writer.Write("</td></tr>");
			writer.Write("<tr>");
			FolderManagement.RenderLeadingText(-868987232, writer);
			writer.Write("<td class=\"dd\">");
			this.folderDropdown.RenderFolderToDelete(this.allFolderList, LocalizedStrings.GetHtmlEncoded(283502113), writer, new FolderDropDownFilterDelegate[]
			{
				new FolderDropDownFilterDelegate(this.ExternalFolderFilter)
			});
			FolderManagement.RenderButton(1381996313, "btnDel", "return onClkDel()", writer);
			writer.Write("</tr>");
		}

		private void RenderCreateCalendarFolder(TextWriter writer)
		{
			this.RenderTitle(-1989251350, writer);
			writer.Write("<tr>");
			FolderManagement.RenderLeadingText(1058761412, writer);
			writer.Write("<td class=\"dfn\" nowrap><img src=\"");
			base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Appointment);
			writer.Write("\" alt=\"\">");
			using (Folder folder = Folder.Bind(base.UserContext.MailboxSession, base.UserContext.CalendarFolderId, new PropertyDefinition[]
			{
				FolderSchema.DisplayName
			}))
			{
				Utilities.HtmlEncode(folder.DisplayName, writer);
			}
			writer.Write("</td><td class=\"btn\"></td>");
			writer.Write("</tr><tr>");
			FolderManagement.RenderLeadingText(2101032677, writer);
			FolderManagement.RenderTextInput("nnfc", "onKUFNCr()", "onBlurFNCr()", "return onKPFNCr(event)", writer);
			FolderManagement.RenderButton(-119614694, "btnCr", "return onClkCr()", writer);
			writer.Write("</tr>");
		}

		private void RenderRenameCalendarFolder(TextWriter writer)
		{
			this.RenderTitle(-2073351774, writer);
			writer.Write("<tr>");
			FolderManagement.RenderLeadingText(824414759, writer);
			writer.Write("<td class=\"dd\">");
			this.folderDropdown.RenderCalendarFolderToRename(this.allFolderList, this.selectedFolderId, writer, new FolderDropDownFilterDelegate[]
			{
				new FolderDropDownFilterDelegate(this.ExternalFolderFilter)
			});
			writer.Write("</td><td class=\"btn\"></td>");
			writer.Write("</tr><tr>");
			FolderManagement.RenderLeadingText(437857602, writer);
			FolderManagement.RenderTextInput("nnfr", "onKUFNRn()", "onBlurFNRn()", "return onKPFNRn(event)", writer);
			FolderManagement.RenderButton(461135208, "btnRn", "return onClkRn()", writer);
			writer.Write("</tr>");
		}

		private void RenderCreateContactFolder(TextWriter writer)
		{
			this.RenderTitle(1463488076, writer);
			writer.Write("<tr>");
			FolderManagement.RenderLeadingText(1058761412, writer);
			writer.Write("<td class=\"dfn\" nowrap><img src=\"");
			base.UserContext.RenderThemeFileUrl(writer, ThemeFileId.Contact2Small);
			writer.Write("\" alt=\"\">");
			using (Folder folder = Folder.Bind(base.UserContext.MailboxSession, base.UserContext.ContactsFolderId, new PropertyDefinition[]
			{
				FolderSchema.DisplayName
			}))
			{
				Utilities.HtmlEncode(folder.DisplayName, writer);
			}
			writer.Write("</td><td class=\"btn\"></td>");
			writer.Write("</tr><tr>");
			FolderManagement.RenderLeadingText(-868987232, writer);
			FolderManagement.RenderTextInput("nnfc", "onKUFNCr()", "onBlurFNCr()", "return onKPFNCr(event)", writer);
			FolderManagement.RenderButton(-119614694, "btnCr", "return onClkCr()", writer);
			writer.Write("</tr>");
		}

		private void RenderRenameContactFolder(TextWriter writer)
		{
			this.RenderTitle(1632531764, writer);
			writer.Write("<tr>");
			FolderManagement.RenderLeadingText(824414759, writer);
			writer.Write("<td class=\"dd\">");
			this.folderDropdown.RenderContactFolderToRename(this.allFolderList, this.selectedFolderId, writer);
			writer.Write("</td><td class=\"btn\"></td>");
			writer.Write("</tr><tr>");
			FolderManagement.RenderLeadingText(437857602, writer);
			FolderManagement.RenderTextInput("nnfr", "onKUFNRn()", "onBlurFNRn()", "return onKPFNRn(event)", writer);
			FolderManagement.RenderButton(461135208, "btnRn", "return onClkRn()", writer);
			writer.Write("</tr>");
		}

		public static void RenderHeaderToolbar(TextWriter writer)
		{
			Toolbar toolbar = new Toolbar(writer, true);
			toolbar.RenderStart();
			toolbar.RenderButton(ToolbarButtons.CloseText);
			toolbar.RenderFill();
			toolbar.RenderButton(ToolbarButtons.CloseImage);
			toolbar.RenderEnd();
		}

		public static void RenderFooterToolbar(TextWriter writer)
		{
			Toolbar toolbar = new Toolbar(writer, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			toolbar.RenderEnd();
		}

		public void RenderFolderManagementForm(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			switch (this.Module)
			{
			case NavigationModule.Mail:
				this.RenderCreateMailFolder(writer);
				this.RenderRenameMailFolder(writer);
				this.RenderMoveMailFolder(writer);
				this.RenderDeleteFolder(writer);
				return;
			case NavigationModule.Calendar:
				this.RenderCreateCalendarFolder(writer);
				this.RenderRenameCalendarFolder(writer);
				this.RenderDeleteFolder(writer);
				return;
			case NavigationModule.Contacts:
				this.RenderCreateContactFolder(writer);
				this.RenderRenameContactFolder(writer);
				this.RenderDeleteFolder(writer);
				return;
			default:
				return;
			}
		}

		public void RenderNavigation(TextWriter writer)
		{
			Navigation navigation = new Navigation(this.Module, base.OwaContext, writer);
			navigation.Render();
		}

		public void RenderSecondaryNavigation(TextWriter writer)
		{
			switch (this.Module)
			{
			case NavigationModule.Mail:
			{
				MailSecondaryNavigation mailSecondaryNavigation = new MailSecondaryNavigation(base.OwaContext, this.selectedFolderId, this.allFolderList, null, null);
				mailSecondaryNavigation.Render(writer);
				return;
			}
			case NavigationModule.Calendar:
			{
				CalendarSecondaryNavigation calendarSecondaryNavigation = new CalendarSecondaryNavigation(base.OwaContext, this.selectedFolderId, null, null);
				calendarSecondaryNavigation.Render(writer);
				return;
			}
			case NavigationModule.Contacts:
			{
				ContactSecondaryNavigation contactSecondaryNavigation = new ContactSecondaryNavigation(base.OwaContext, this.selectedFolderId, null);
				contactSecondaryNavigation.RenderContacts(writer);
				return;
			}
			default:
				throw new ArgumentOutOfRangeException("Module", "The secondary navigation for Module " + this.Module + " is not supported");
			}
		}

		public void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.None);
			optionsBar.Render(helpFile);
		}

		protected void RenderJavascriptEncodedInboxFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.InboxFolderId.ToBase64String(), base.Response.Output);
		}

		private bool ExternalFolderFilter(FolderList folderList, StoreObjectId folderId)
		{
			return !Utilities.IsExternalSharedInFolder(folderList.GetFolderProperty(folderId, FolderSchema.ExtendedFolderFlags));
		}

		internal const string FolderToCreateInFormParameter = "ftci";

		internal const string NewNameForCreateFormParameter = "nnfc";

		internal const string FolderToRenameFormParameter = "ftr";

		internal const string NewNameForRenameFormParameter = "nnfr";

		internal const string FolderToMoveFormParameter = "ftm";

		internal const string NewLocationForMoveFormParameter = "nlfm";

		internal const string FolderToDeleteFormParameter = "ftd";

		internal const string PermanentDeleteFolderQueryStringParameter = "hd";

		private NavigationModule module;

		private StoreObjectId selectedFolderId;

		private FolderList allFolderList;

		private FolderDropdown folderDropdown;

		private Infobar infobar = new Infobar();
	}
}
