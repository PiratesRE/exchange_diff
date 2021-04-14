using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class MessageView2 : FolderListViewSubPage, IRegistryOnlyForm
	{
		public MessageView2() : base(ExTraceGlobals.MailCallTracer, ExTraceGlobals.MailTracer)
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.arrangeByMenu = new MessageViewArrangeByMenu(base.Folder, base.UserContext, this.SortedColumn);
		}

		protected override void OnUnload(EventArgs e)
		{
			base.OnUnload(e);
			base.UserContext.MessageViewFirstRender = false;
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

		protected override bool FindBarOn
		{
			get
			{
				return !base.IsPublicFolder && base.UserContext.UserOptions.MailFindBarOn;
			}
		}

		protected override int LVRPContainerTop
		{
			get
			{
				return this.lvRPContainerTop;
			}
		}

		protected override ColumnId DefaultSortedColumn
		{
			get
			{
				if (base.FolderType == DefaultFolderType.DeletedItems || base.FolderType == DefaultFolderType.JunkEmail)
				{
					return ColumnId.DeliveryTime;
				}
				if (base.FolderType == DefaultFolderType.Outbox)
				{
					return ColumnId.SentTime;
				}
				return ColumnId.ConversationLastDeliveryTime;
			}
		}

		protected bool ShowToInFilter
		{
			get
			{
				return base.FolderType == DefaultFolderType.SentItems || base.FolderType == DefaultFolderType.Drafts || base.FolderType == DefaultFolderType.Outbox;
			}
		}

		protected override ReadingPanePosition DefaultReadingPanePosition
		{
			get
			{
				if (base.FolderType == DefaultFolderType.JunkEmail || base.FolderType == DefaultFolderType.SentItems || base.FolderType == DefaultFolderType.Outbox || base.FolderType == DefaultFolderType.DeletedItems)
				{
					return ReadingPanePosition.Off;
				}
				return ReadingPanePosition.Right;
			}
		}

		protected override bool DefaultMultiLineSetting
		{
			get
			{
				return true;
			}
		}

		protected MessageViewContextMenu ContextMenu
		{
			get
			{
				if (this.contextMenu == null)
				{
					this.contextMenu = new MessageViewContextMenu(base.UserContext, "divVwm", base.IsPublicFolder, ConversationUtilities.ShouldAllowConversationView(base.UserContext, base.Folder));
				}
				return this.contextMenu;
			}
		}

		protected MessageViewArrangeByMenu ArrangeByMenu
		{
			get
			{
				return this.arrangeByMenu;
			}
		}

		protected bool IsConversationView
		{
			get
			{
				return ConversationUtilities.IsConversationSortColumn(this.SortedColumn);
			}
		}

		protected bool IsJunkMailFolder
		{
			get
			{
				return Utilities.IsDefaultFolder(base.Folder, DefaultFolderType.JunkEmail);
			}
		}

		protected bool IsSearchFolder
		{
			get
			{
				return base.Folder is SearchFolder;
			}
		}

		protected bool DontAllowAddFilterToFavorites
		{
			get
			{
				return base.IsFilteredViewInFavorites || base.IsInDeleteItems || base.IsDeletedItemsSubFolder;
			}
		}

		protected static int StoreObjectTypeFolder
		{
			get
			{
				return 1;
			}
		}

		protected override IListView CreateListView(ColumnId sortedColumn, SortOrder sortOrder)
		{
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.MailViewsLoaded.Increment();
			}
			MessageVirtualListView2 messageVirtualListView = new MessageVirtualListView2(base.UserContext, "divVLV", this.IsMultiLine, sortedColumn, sortOrder, base.Folder, base.Folder, null, (base.Folder is SearchFolder) ? SearchScope.AllFoldersAndItems : SearchScope.SelectedFolder);
			if (Utilities.IsDefaultFolder(base.Folder, DefaultFolderType.JunkEmail))
			{
				messageVirtualListView.AddAttribute("fJnk", "1");
			}
			messageVirtualListView.LoadData(0, 50);
			return messageVirtualListView;
		}

		protected override Toolbar CreateListToolbar()
		{
			MessageItemManageToolbar toolbar = new MessageItemManageToolbar(base.IsPublicFolder, ConversationUtilities.ShouldAllowConversationView(base.UserContext, base.Folder), base.IsInDeleteItems || this.IsJunkMailFolder, this.IsMultiLine, base.IsOtherMailboxFolder, base.UserContext.IsWebPartRequest, base.Folder.ClassName, this.ReadingPanePosition, this.IsConversationView, base.UserContext.UserOptions.ConversationSortOrder == ConversationSortOrder.ChronologicalNewestOnTop, base.UserContext.UserOptions.ShowTreeInListView, base.IsInDeleteItems || base.IsDeletedItemsSubFolder, this.IsJunkMailFolder);
			MessageViewManageToolbar toolbar2 = new MessageViewManageToolbar();
			MessageViewActionToolbar toolbar3 = new MessageViewActionToolbar(this.IsJunkMailFolder);
			MessageViewActionsButtonToolbar toolbar4 = new MessageViewActionsButtonToolbar();
			return new MultipartToolbar(new MultipartToolbar.ToolbarInfo[]
			{
				new MultipartToolbar.ToolbarInfo(toolbar, "divItemToolbar"),
				new MultipartToolbar.ToolbarInfo(toolbar2, "divViewToolbar"),
				new MultipartToolbar.ToolbarInfo(toolbar3, "divReplyForwardToolbar"),
				new MultipartToolbar.ToolbarInfo(toolbar4, "divActionsButtonToolbar")
			});
		}

		protected override Toolbar CreateActionToolbar()
		{
			return null;
		}

		private bool IsUserOofEnabled
		{
			get
			{
				return (base.UserContext.MailboxSession.Mailbox.TryGetProperty(MailboxSchema.MailboxOofState) as bool?) ?? false;
			}
		}

		private UserOofSettings UserOofSettings
		{
			get
			{
				if (!base.UserContext.IsWebPartRequest && this.userOofSettings == null)
				{
					try
					{
						this.userOofSettings = UserOofSettings.GetUserOofSettings(base.UserContext.MailboxSession);
					}
					catch (QuotaExceededException ex)
					{
						ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "MessageView.UserOofSettings: Failed. Exception: {0}", ex.Message);
					}
				}
				return this.userOofSettings;
			}
		}

		protected bool ShouldShowScheduledOofInfobar()
		{
			if (!this.IsUserOofEnabled || base.UserContext.IsWebPartRequest || this.UserOofSettings == null)
			{
				return false;
			}
			if (!base.UserContext.MessageViewFirstRender || this.UserOofSettings.OofState != OofState.Scheduled)
			{
				return false;
			}
			DateTime utcNow = DateTime.UtcNow;
			DateTime t = DateTime.MinValue;
			DateTime t2 = DateTime.MinValue;
			if (this.UserOofSettings.Duration != null)
			{
				t = this.UserOofSettings.Duration.StartTime;
				t2 = this.UserOofSettings.Duration.EndTime;
			}
			return utcNow > t && t2 > utcNow;
		}

		protected bool ShouldShowOofDialog()
		{
			return !base.UserContext.IsWebPartRequest && base.UserContext.MessageViewFirstRender && this.IsUserOofEnabled && this.UserOofSettings != null && this.UserOofSettings.OofState == OofState.Enabled;
		}

		protected void RenderOofNotificationInfobar(Infobar infobar)
		{
			ExDateTime exDateTime = new ExDateTime(base.UserContext.TimeZone, this.UserOofSettings.Duration.EndTime);
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<div class=\"divIBTxt\">");
			sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(-1261886615), new object[]
			{
				exDateTime.ToLongDateString() + " " + exDateTime.ToString(base.UserContext.UserOptions.TimeFormat)
			});
			sanitizingStringBuilder.Append("</div>");
			sanitizingStringBuilder.Append("<div class=\"divIBTxt\"><a href=# id=\"lnkRmvOofIB\" _sRmvId=\"divOofIB\">");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1303059585));
			sanitizingStringBuilder.Append("</a></div>");
			infobar.AddMessage(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational, "divOofIB");
		}

		protected void RenderExpiringPasswordNotificationInfobar(Infobar infobar, int daysUntilExpiration)
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<div class=\"divIBTxt\">");
			if (daysUntilExpiration == 0)
			{
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(352263686));
			}
			else
			{
				sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(-2025544575), new object[]
				{
					daysUntilExpiration
				});
			}
			sanitizingStringBuilder.Append("</div>");
			sanitizingStringBuilder.Append("<div class=\"divIBTxt\"><a href=# id=\"lnkChgPwd\">");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1273337393));
			sanitizingStringBuilder.Append("</a></div>");
			sanitizingStringBuilder.Append("<div class=\"divIBTxt\"><a href=# id=\"lnkRmvPwdIB\" _sRmvId=\"divPwdIB\">");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1496915101));
			sanitizingStringBuilder.Append("</a></div>");
			infobar.AddMessage(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational, "divPwdIB");
		}

		protected void RenderHtmlEncodedFolderName()
		{
			Utilities.HtmlEncode(this.ContainerName, base.Response.Output);
		}

		protected void RenderJavascriptEncodedInboxFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.InboxFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedJunkEmailFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.JunkEmailFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedSentItemsFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.SentItemsFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderSMimeControlUpdateInfobar(Infobar infobar)
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<div class=\"divIBTxt\">");
			sanitizingStringBuilder.AppendFormat(LocalizedStrings.GetHtmlEncoded(-172046453), new object[]
			{
				"<a href=\"/ecp/?p=Security/SMIME.aspx\" target=\"_parent\" class=\"lnk\">",
				"</a>"
			});
			sanitizingStringBuilder.Append("</div>");
			infobar.AddMessage(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational, "divSMimeIB", true);
		}

		protected void RenderELCCommentAndQuota(Infobar infobar)
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			SanitizedHtmlString sanitizedHtmlString = new SanitizedHtmlString(base.RenderELCComment());
			sanitizedHtmlString.DecreeToBeTrusted();
			SanitizedHtmlString sanitizedHtmlString2 = new SanitizedHtmlString(base.RenderELCQuota());
			sanitizedHtmlString2.DecreeToBeTrusted();
			sanitizingStringBuilder.Append<SanitizedHtmlString>(sanitizedHtmlString);
			sanitizingStringBuilder.Append<SanitizedHtmlString>(sanitizedHtmlString2);
			infobar.AddMessage(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational, "divElcIB", !base.IsELCInfobarVisible);
		}

		protected override void RenderViewInfobars()
		{
			Infobar infobar = new Infobar("divErr", "infobarMessageView");
			int num = 0;
			if (base.ShouldRenderELCInfobar)
			{
				this.RenderELCCommentAndQuota(infobar);
				if (base.IsELCInfobarVisible)
				{
					num += 34;
				}
			}
			if (this.ShouldShowScheduledOofInfobar())
			{
				this.RenderOofNotificationInfobar(infobar);
				num += 20;
			}
			int daysUntilExpiration;
			if (Utilities.ShouldRenderExpiringPasswordInfobar(base.UserContext, out daysUntilExpiration))
			{
				this.RenderExpiringPasswordNotificationInfobar(infobar, daysUntilExpiration);
				num += 20;
			}
			if (Utilities.IsSMimeFeatureUsable(base.OwaContext) && !base.IsPublicFolder)
			{
				this.RenderSMimeControlUpdateInfobar(infobar);
			}
			if (0 < num)
			{
				int num2 = 60;
				if (num2 < num)
				{
					num = num2;
				}
				this.lvRPContainerTop = num + 3 + 1;
			}
			infobar.Render(base.SanitizingResponse);
		}

		protected bool IsMessagePrefetchEnabled()
		{
			return MessagePrefetchConfiguration.IsMessagePrefetchEnabledForSession(base.UserContext, base.Folder.Session);
		}

		public override IEnumerable<string> ExternalScriptFiles
		{
			get
			{
				foreach (string script in this.externalScriptFiles)
				{
					yield return script;
				}
				if (base.UserContext.ExchangePrincipal.RecipientTypeDetails == RecipientTypeDetails.DiscoveryMailbox)
				{
					foreach (string script2 in this.externalScriptFilesForAnnotation)
					{
						yield return script2;
					}
				}
				yield break;
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
				return "MessageViewSubPage";
			}
		}

		private const string OofEndTime = "oof";

		private const int InfobarColorbarHeight = 3;

		private const int InfobarMessageHeight = 20;

		private const int InfobarElcMessageHeight = 34;

		private const int InfobarBottomMargin = 1;

		private MessageViewContextMenu contextMenu;

		private MessageViewArrangeByMenu arrangeByMenu;

		private UserOofSettings userOofSettings;

		private int lvRPContainerTop;

		private string[] externalScriptFiles = new string[]
		{
			"uview.js",
			"vlv.js"
		};

		private string[] externalScriptFilesForAnnotation = new string[]
		{
			"MessageAnnotationDialog.js"
		};
	}
}
