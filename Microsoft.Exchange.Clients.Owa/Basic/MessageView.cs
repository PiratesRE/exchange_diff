using System;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class MessageView : ListViewPage, IRegistryOnlyForm
	{
		public MessageView() : base(ExTraceGlobals.MailCallTracer, ExTraceGlobals.MailTracer)
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			base.FilteredView = false;
			base.OnLoad(e);
			EditCalendarItemHelper.ClearUserContextData(base.UserContext);
			this.CreateExpiringPasswordNotification();
			this.CreateOutOfOfficeNotification();
			int num = RequestParser.TryGetIntValueFromQueryString(base.Request, "slUsng", -1);
			if (num >= 0 && num <= 2)
			{
				this.selectedUsing = (SecondaryNavigationArea)num;
			}
			if (base.UserContext.IsWebPartRequest)
			{
				base.UserContext.LastClientViewState = new WebPartModuleViewState(base.FolderId, base.Folder.ClassName, base.PageNumber, NavigationModule.Mail, base.SortOrder, base.SortedColumn);
				return;
			}
			if (base.FilteredView)
			{
				base.UserContext.LastClientViewState = new MessageModuleSearchViewState(base.UserContext.LastClientViewState, base.FolderId, base.OwaContext.FormsRegistryContext.Type, this.selectedUsing, base.PageNumber, base.SearchString, base.SearchScope);
				return;
			}
			base.UserContext.LastClientViewState = new MessageModuleViewState(base.FolderId, base.OwaContext.FormsRegistryContext.Type, this.selectedUsing, base.PageNumber);
		}

		internal override StoreObjectId DefaultFolderId
		{
			get
			{
				return base.UserContext.InboxFolderId;
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
				DefaultFolderType defaultFolderType = Utilities.GetDefaultFolderType(base.UserContext.MailboxSession, base.FolderId);
				if (defaultFolderType == DefaultFolderType.SentItems || defaultFolderType == DefaultFolderType.Outbox || defaultFolderType == DefaultFolderType.Drafts)
				{
					return ColumnId.SentTime;
				}
				return ColumnId.DeliveryTime;
			}
		}

		public string UrlEncodedFolderId
		{
			get
			{
				return HttpUtility.UrlEncode(base.Folder.Id.ObjectId.ToBase64String());
			}
		}

		public bool IsDeletedItemsFolder
		{
			get
			{
				return Utilities.IsDefaultFolder(base.Folder, DefaultFolderType.DeletedItems);
			}
		}

		protected bool IsJunkEmailFolder
		{
			get
			{
				return Utilities.IsDefaultFolder(base.Folder, DefaultFolderType.JunkEmail);
			}
		}

		public string EmptyFolderWarning
		{
			get
			{
				if (this.IsDeletedItemsFolder || this.IsJunkEmailFolder)
				{
					return string.Format(LocalizedStrings.GetNonEncoded(1984261115), base.FolderName);
				}
				return LocalizedStrings.GetNonEncoded(1984261112);
			}
		}

		public bool IsDraftsFolder
		{
			get
			{
				return Utilities.IsDefaultFolder(base.Folder, DefaultFolderType.Drafts);
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

		protected override string CheckBoxId
		{
			get
			{
				return "chkmsg";
			}
		}

		protected SecondaryNavigationArea SelectedUsing
		{
			get
			{
				return this.selectedUsing;
			}
		}

		protected bool ShouldShowOofDialog
		{
			get
			{
				return this.shouldShowOofDialog;
			}
		}

		public void RenderMailSecondaryNavigation()
		{
			MailSecondaryNavigation mailSecondaryNavigation = new MailSecondaryNavigation(base.OwaContext, base.Folder.Id.ObjectId, null, null, new SecondaryNavigationArea?(this.selectedUsing));
			mailSecondaryNavigation.Render(base.Response.Output);
		}

		private void UpdateMru(StoreObjectId folderId)
		{
			if (string.CompareOrdinal(Utilities.GetQueryStringParameter(base.Request, "mru", false), "1") == 0)
			{
				string className = base.Folder.ClassName;
				bool flag = true;
				if (string.IsNullOrEmpty(className) || string.CompareOrdinal(className, "IPF.Note") == 0)
				{
					flag = false;
				}
				DefaultFolderType defaultFolderType = Utilities.GetDefaultFolderType(base.Folder);
				if (defaultFolderType == DefaultFolderType.Inbox || defaultFolderType == DefaultFolderType.DeletedItems || defaultFolderType == DefaultFolderType.Drafts || defaultFolderType == DefaultFolderType.JunkEmail || defaultFolderType == DefaultFolderType.SentItems)
				{
					flag = true;
				}
				if (!flag)
				{
					FolderMruCache cacheInstance = FolderMruCache.GetCacheInstance(base.UserContext);
					FolderMruCacheEntry newEntry = new FolderMruCacheEntry(folderId);
					cacheInstance.AddEntry(newEntry);
					cacheInstance.Commit();
				}
			}
		}

		protected override void CreateListView(ColumnId sortColumn, SortOrder sortOrder)
		{
			this.UpdateMru(base.Folder.Id.ObjectId);
			if (!base.FilteredView)
			{
				base.ListView = new MessageListView(base.UserContext, sortColumn, sortOrder, base.Folder);
			}
			else
			{
				base.ListView = new MessageListView(base.UserContext, sortColumn, sortOrder, base.SearchFolder, base.SearchScope);
			}
			base.InitializeListView();
		}

		protected override SanitizedHtmlString BuildConcretSearchInfobarMessage(int resultsCount, SanitizedHtmlString clearSearchLink)
		{
			if (base.SearchScope == SearchScope.AllFoldersAndItems)
			{
				return SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1390621969), new object[]
				{
					resultsCount,
					base.SearchString,
					clearSearchLink
				});
			}
			return SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded((base.SearchScope == SearchScope.SelectedFolder) ? 609609633 : -1674214459), new object[]
			{
				resultsCount,
				base.SearchString,
				base.Folder.DisplayName,
				clearSearchLink
			});
		}

		public void RenderMessageListHeaderToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			if (this.IsJunkEmailFolder)
			{
				if (base.UserContext.IsJunkEmailEnabled)
				{
					toolbar.RenderButton(ToolbarButtons.NotJunk);
					toolbar.RenderDivider();
				}
			}
			else
			{
				toolbar.RenderButton(ToolbarButtons.NewMessage);
				toolbar.RenderDivider();
			}
			toolbar.RenderButton(ToolbarButtons.Move);
			toolbar.RenderSpace();
			toolbar.RenderButton(ToolbarButtons.Delete);
			toolbar.RenderSpace();
			toolbar.RenderDivider();
			if (!this.IsJunkEmailFolder && base.UserContext.IsJunkEmailEnabled)
			{
				toolbar.RenderButton(ToolbarButtons.Junk);
				toolbar.RenderDivider();
			}
			if (this.IsDeletedItemsFolder)
			{
				string toolTip = string.Format(LocalizedStrings.GetHtmlEncoded(462976341), Utilities.HtmlEncode(base.FolderName));
				toolbar.RenderButton(new ToolbarButton("emptyfolder", ToolbarButtonFlags.Image, 491943887, ThemeFileId.BasicDarkDeleted, toolTip));
				toolbar.RenderDivider();
			}
			else if (this.IsJunkEmailFolder)
			{
				string toolTip2 = string.Format(LocalizedStrings.GetHtmlEncoded(462976341), Utilities.HtmlEncode(base.FolderName));
				toolbar.RenderButton(new ToolbarButton("emptyfolder", ToolbarButtonFlags.ImageAndText, 1628292131, ThemeFileId.BasicDarkDeleted, toolTip2));
			}
			if (!this.IsJunkEmailFolder)
			{
				toolbar.RenderButton(ToolbarButtons.MarkAsRead);
				toolbar.RenderSpace();
				toolbar.RenderButton(ToolbarButtons.MarkAsUnread);
				toolbar.RenderDivider();
				toolbar.RenderButton(ToolbarButtons.CheckMessagesImage);
			}
			toolbar.RenderFill();
			base.RenderPaging(false);
			toolbar.RenderEnd();
		}

		public void RenderMessageListFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			if (!base.UserContext.IsWebPartRequest)
			{
				toolbar.RenderButton(ToolbarButtons.Move);
				toolbar.RenderSpace();
			}
			toolbar.RenderButton(ToolbarButtons.Delete);
			toolbar.RenderFill();
			base.RenderPaging(true);
			toolbar.RenderEnd();
		}

		protected void RenderInfobar()
		{
			if (base.Infobar.MessageCount > 0)
			{
				TextWriter output = base.Response.Output;
				output.Write("<tr id=trPwdIB><td class=vwinfbr>");
				base.Infobar.Render(output);
				output.Write("</td></tr>");
			}
		}

		private void CreateExpiringPasswordNotification()
		{
			int num;
			if (!Utilities.ShouldRenderExpiringPasswordInfobar(base.UserContext, out num))
			{
				return;
			}
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<table cellpadding=0 cellspacing=0><tr><td class=tdMvIBSe>");
			if (num == 0)
			{
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(352263686));
			}
			else
			{
				sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(-2025544575), new object[]
				{
					num
				});
			}
			sanitizingStringBuilder.Append("</td>");
			sanitizingStringBuilder.Append("<td class=tdMvIBSe><a href=# onClick=\"return onPwdNtf('yes');\">");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1273337393));
			sanitizingStringBuilder.Append("</a></td>");
			sanitizingStringBuilder.Append("<td class=tdMvIBSe><a href=# onClick=\"return onPwdNtf('no');\">");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1496915101));
			sanitizingStringBuilder.Append("</a></td>");
			sanitizingStringBuilder.Append("</tr></table>");
			base.Infobar.AddMessageHtml(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational);
		}

		protected void CreateOutOfOfficeNotification()
		{
			this.shouldShowOofDialog = ((base.UserContext.MailboxSession.Mailbox.TryGetProperty(MailboxSchema.MailboxOofState) as bool?) ?? false);
			if (!this.shouldShowOofDialog || base.UserContext.IsWebPartRequest)
			{
				return;
			}
			UserOofSettings userOofSettings = null;
			try
			{
				userOofSettings = UserOofSettings.GetUserOofSettings(base.UserContext.MailboxSession);
			}
			catch (QuotaExceededException ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "BasicMessageView.CreateOutOfOfficeNotification: Failed. Exception: {0}", ex.Message);
				return;
			}
			switch (userOofSettings.OofState)
			{
			case OofState.Enabled:
				this.shouldShowOofDialog = base.UserContext.MessageViewFirstRender;
				break;
			case OofState.Scheduled:
			{
				this.shouldShowOofDialog = false;
				if (RenderingFlags.HideOutOfOfficeInfoBar(base.UserContext))
				{
					return;
				}
				DateTime utcNow = DateTime.UtcNow;
				DateTime t = DateTime.MinValue;
				DateTime t2 = DateTime.MinValue;
				if (userOofSettings.Duration != null)
				{
					t = userOofSettings.Duration.StartTime;
					t2 = userOofSettings.Duration.EndTime;
				}
				if (utcNow > t && t2 > utcNow)
				{
					ExDateTime exDateTime = new ExDateTime(base.UserContext.TimeZone, userOofSettings.Duration.EndTime);
					SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
					sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(-1261886615), new object[]
					{
						exDateTime.ToLongDateString() + " " + exDateTime.ToString(base.UserContext.UserOptions.TimeFormat)
					});
					sanitizingStringBuilder.Append(" <a href=# onclick=\"onClkHdOof()\">");
					sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1303059585));
					sanitizingStringBuilder.Append("</a>");
					base.Infobar.AddMessageHtml(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational);
				}
				return;
			}
			default:
				this.shouldShowOofDialog = false;
				return;
			}
		}

		protected void RenderNoScriptInfobar()
		{
			SanitizedHtmlString noScriptHtml = Utilities.GetNoScriptHtml();
			Infobar infobar = new Infobar();
			infobar.AddMessageHtml(noScriptHtml, InfobarMessageType.Error);
			infobar.Render(base.SanitizingResponse);
		}

		protected override void OnUnload(EventArgs e)
		{
			base.OnUnload(e);
			base.UserContext.MessageViewFirstRender = false;
		}

		protected void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, ObjectClass.IsCalendarFolder(base.Folder.ClassName) ? OptionsBar.SearchModule.Calendar : OptionsBar.SearchModule.Mail, OptionsBar.RenderingFlags.ShowSearchContext, OptionsBar.BuildFolderSearchUrlSuffix(base.UserContext, base.FolderId));
			optionsBar.Render(helpFile);
		}

		internal const string SelectedUsingQueryStringParameter = "slUsng";

		private const string MruFlag = "mru";

		private const string MessageCheckBox = "chkmsg";

		private SecondaryNavigationArea selectedUsing;

		private bool shouldShowOofDialog;
	}
}
