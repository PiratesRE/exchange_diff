using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class StartPage : NavigationHost, IRegistryOnlyForm
	{
		protected string QuotaWarningMessage
		{
			get
			{
				return this.quotaWarningMessage;
			}
		}

		protected string QuotaExceededMessage
		{
			get
			{
				return this.quotaExceededMessage;
			}
		}

		protected string MySiteUrl
		{
			get
			{
				return Utilities.GetHomePageForMailboxUser(base.OwaContext);
			}
		}

		protected bool IsReInitializationRequest
		{
			get
			{
				if (this.isReInitializationRequest == null)
				{
					this.isReInitializationRequest = new bool?(!string.IsNullOrEmpty(base.OwaContext.HttpContext.Request.QueryString["reInit"]));
				}
				return this.isReInitializationRequest == true;
			}
		}

		protected override NavigationModule SelectNavagationModule()
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "rru", false);
			if (string.Equals(queryStringParameter, "contacts", StringComparison.Ordinal))
			{
				return NavigationModule.Contacts;
			}
			string queryStringParameter2 = Utilities.GetQueryStringParameter(base.Request, "modurl", false);
			if (queryStringParameter2 != null)
			{
				int num;
				if (!int.TryParse(queryStringParameter2, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
				{
					throw new OwaInvalidRequestException("Invalid modurl querystring parameter");
				}
				if (num >= 0 && num <= 7)
				{
					return (NavigationModule)num;
				}
			}
			return NavigationModule.Mail;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.navigationBar = new NavigationBar(base.UserContext);
			InstantMessageManager instantMessageManager = base.UserContext.InstantMessageManager;
			if (!this.IsReInitializationRequest)
			{
				base.UserContext.PendingRequestManager.HandleFinishRequestFromClient(true);
			}
			if (instantMessageManager != null && !this.IsReInitializationRequest)
			{
				instantMessageManager.StartProvider();
			}
			if (base.UserContext.IsFeatureEnabled(Feature.Notifications))
			{
				if (base.UserContext.IsPushNotificationsEnabled)
				{
					base.UserContext.MapiNotificationManager.SubscribeForFolderCounts(null, base.UserContext.MailboxSession);
					if (base.UserContext.UserOptions.EnableReminders)
					{
						base.UserContext.MapiNotificationManager.SubscribeForReminders();
					}
					if (base.UserContext.UserOptions.NewItemNotify != NewNotification.None)
					{
						base.UserContext.MapiNotificationManager.SubscribeForNewMail();
					}
					base.UserContext.MapiNotificationManager.SubscribeForSubscriptionChanges();
				}
				if (base.UserContext.IsPullNotificationsEnabled)
				{
					if (base.UserContext.NotificationManager.FolderCountAdvisor == null)
					{
						base.UserContext.NotificationManager.CreateOwaFolderCountAdvisor(base.UserContext.MailboxSession);
					}
					Dictionary<OwaStoreObjectId, OwaConditionAdvisor> conditionAdvisorTable = base.UserContext.NotificationManager.ConditionAdvisorTable;
					if (base.UserContext.UserOptions.EnableReminders && (conditionAdvisorTable == null || !conditionAdvisorTable.ContainsKey(base.UserContext.RemindersSearchFolderOwaId)))
					{
						base.UserContext.NotificationManager.CreateOwaConditionAdvisor(base.UserContext, base.UserContext.MailboxSession, base.UserContext.RemindersSearchFolderOwaId, EventObjectType.None, EventType.None);
					}
					if (base.UserContext.UserOptions.NewItemNotify != NewNotification.None && base.UserContext.NotificationManager.LastEventAdvisor == null)
					{
						base.UserContext.NotificationManager.CreateOwaLastEventAdvisor(base.UserContext, base.UserContext.InboxFolderId, EventObjectType.Item, EventType.NewMail);
					}
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				Utilities.RenderSizeWithUnits(stringWriter, base.UserContext.QuotaSend, false);
			}
			this.quotaWarningMessage = string.Format(LocalizedStrings.GetHtmlEncoded(406437542), stringBuilder.ToString());
			this.quotaExceededMessage = string.Format(LocalizedStrings.GetHtmlEncoded(611216529), stringBuilder.ToString());
			if (this.IsReInitializationRequest)
			{
				PerformanceCounterManager.ProcessUserContextReInitializationRequest();
			}
			if (base.UserContext.IsClientSideDataCollectingEnabled)
			{
				this.UpdateClientCookie("owacsdc", "1");
				return;
			}
			Utilities.DeleteCookie(base.OwaContext.HttpContext.Response, "owacsdc");
		}

		protected void UpdateClientCookie(string cookieName, string cookieValue)
		{
			HttpCookie httpCookie = new HttpCookie(cookieName);
			httpCookie.Value = cookieValue;
			base.OwaContext.HttpContext.Response.Cookies.Add(httpCookie);
		}

		protected RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		protected int InstantMessagingType
		{
			get
			{
				return (int)base.UserContext.InstantMessagingType;
			}
		}

		protected void RenderNoScriptInfobar()
		{
			SanitizedHtmlString noScriptHtml = Utilities.GetNoScriptHtml();
			RenderingUtilities.RenderError(base.UserContext, base.Response.Output, noScriptHtml);
		}

		protected void RenderJavascriptEncodedInboxFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.InboxFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedCalendarFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.CalendarFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedContactsFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.ContactsFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedFlaggedItemsAndTasksFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.FlaggedItemsAndTasksFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedJunkEmailFolderId()
		{
			Utilities.JavascriptEncode(base.UserContext.JunkEmailFolderId.ToBase64String(), base.Response.Output);
		}

		protected void RenderJavascriptEncodedPublicFolderRootId()
		{
			string text = null;
			if (base.NavigationModule == NavigationModule.PublicFolders)
			{
				text = base.UserContext.TryGetPublicFolderRootIdString();
			}
			if (text == null)
			{
				text = string.Empty;
			}
			Utilities.JavascriptEncode(text, base.Response.Output);
		}

		protected void RenderUserTile()
		{
			base.Response.Write("<div id=\"divUserTile\">");
			bool flag = base.UserContext.IsSenderPhotosFeatureEnabled(Feature.SetPhoto);
			bool flag2 = base.UserContext.IsSenderPhotosFeatureEnabled(Feature.DisplayPhotos) || flag;
			bool flag3 = base.UserContext.IsFeatureEnabled(Feature.ExplicitLogon);
			bool flag4 = base.UserContext.IsInstantMessageEnabled();
			if (flag4 || flag3 || flag2)
			{
				base.Response.Write("<a id=\"aUserTile\"  hidefocus href=\"#\" ");
				if (flag4 || flag)
				{
					base.RenderOnClick("opnMeCardMenu();");
					base.RenderScriptHandler("oncontextmenu", "opnMeCardMenu();");
				}
				else if (flag3)
				{
					base.RenderOnClick("showOpenMailboxDialog();");
				}
				base.Response.Write("><span id=\"spnUserTileTxt\" class=\"userTileTxt\">");
				Utilities.RenderDirectionEnhancedValue(base.Response.Output, Utilities.HtmlEncode(base.ExchangePrincipalDisplayName), base.UserContext.IsRtl);
				base.Response.Write("</span>");
				if (flag4)
				{
					ThemeFileId themeFileId;
					if (flag2)
					{
						themeFileId = (this.IsInstantMessagingTypeOcs(base.UserContext) ? ThemeFileId.PresenceUnknownVbarSmall : ThemeFileId.PresenceOfflineVbarSmall);
					}
					else
					{
						themeFileId = (this.IsInstantMessagingTypeOcs(base.UserContext) ? ThemeFileId.PresenceUnknown : ThemeFileId.PresenceOffline);
					}
					base.UserContext.RenderThemeImage(base.Response.Output, themeFileId, flag2 ? "VbarSm" : "noPic", new object[]
					{
						"id=\"imgUserTileJB\" unselectable=\"on\""
					});
				}
				if (flag2)
				{
					string adpictureUrl = RenderingUtilities.GetADPictureUrl(base.UserContext.ExchangePrincipal.LegacyDn, "EX", base.UserContext, true);
					RenderingUtilities.RenderDisplayPicture(base.Response.Output, base.UserContext, adpictureUrl, 32, true, ThemeFileId.DoughboyPersonSmall);
				}
				if (flag4 || flag || flag3)
				{
					base.UserContext.RenderThemeImage(base.Response.Output, ThemeFileId.UserTileDropDownArrow, null, new object[]
					{
						"id=\"imgUserTileDD\""
					});
				}
				base.Response.Write("</a>");
			}
			else
			{
				base.Response.Write("<span id=\"spnUserTileTxt\" class=\"userTileTxt\">");
				Utilities.RenderDirectionEnhancedValue(base.Response.Output, Utilities.HtmlEncode(base.ExchangePrincipalDisplayName), base.UserContext.IsRtl);
				base.Response.Write("</span>");
			}
			base.Response.Write("</div>");
		}

		protected void RenderHelpContextMenu()
		{
			new HelpContextMenu(base.UserContext).Render(base.Response.Output);
		}

		protected void RenderNonMailModuleContextMenu()
		{
			NonMailModuleContextMenu nonMailModuleContextMenu = new NonMailModuleContextMenu(base.UserContext);
			nonMailModuleContextMenu.Render(base.Response.Output);
		}

		protected void RenderNavigationBar(NavigationBarType type)
		{
			this.navigationBar.Render(base.Response.Output, base.NavigationModule, type);
		}

		protected int GetNavigationBarHeight(NavigationBarType type)
		{
			return this.navigationBar.GetNavigationBarHeight(type);
		}

		protected void RenderMeCardContextMenu()
		{
			InstantMessagePresenceContextMenu instantMessagePresenceContextMenu = new InstantMessagePresenceContextMenu(base.UserContext);
			instantMessagePresenceContextMenu.Render(base.Response.Output);
		}

		protected void RenderJavascriptEncodedDisplayName()
		{
			string mailboxOwnerDisplayName = Utilities.GetMailboxOwnerDisplayName(base.UserContext.MailboxSession);
			Utilities.JavascriptEncode(mailboxOwnerDisplayName ?? string.Empty, base.Response.Output);
		}

		protected void RenderJavascriptEncodedLegacyDN()
		{
			Utilities.JavascriptEncode(base.UserContext.ExchangePrincipal.LegacyDn, base.Response.Output);
		}

		protected void RenderJavascriptEncodedSipUri()
		{
			if (this.IsInstantMessagingTypeOcs(base.UserContext))
			{
				Utilities.JavascriptEncode(base.UserContext.SipUri ?? string.Empty, base.Response.Output);
			}
		}

		protected void RenderJavascriptEncodedSetPhotoUrl()
		{
			if (base.UserContext.IsSenderPhotosFeatureEnabled(Feature.SetPhoto) && !string.IsNullOrEmpty(base.UserContext.SetPhotoURL))
			{
				Utilities.JavascriptEncode(base.UserContext.SetPhotoURL, base.Response.Output);
			}
		}

		protected bool IsInstantMessagingTypeOcs(UserContext userContext)
		{
			return base.UserContext.InstantMessagingType == InstantMessagingTypeOptions.Ocs;
		}

		protected void RenderBrandBarLogo()
		{
		}

		protected void RenderBrandBarHeaderLinks()
		{
		}

		protected void RenderBrandThemeMask(TextWriter output, UserContext userContext)
		{
		}

		protected void RenderBrandHeaderMask(TextWriter output, UserContext userContext)
		{
		}

		protected void RenderLiveHeaderContextMenus()
		{
		}

		protected void RenderLiveHeaderMenuLinkIds(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
		}

		protected void RenderBreadcrumbs()
		{
			Breadcrumbs breadcrumbs = new Breadcrumbs(base.UserContext, base.NavigationModule);
			breadcrumbs.Render(base.Response.Output);
		}

		protected void EndOfStartpage()
		{
			base.UserContext.SetFullyInitialized();
		}

		public const string ClientSideDataCollectionCookieName = "owacsdc";

		private MessageRecipientWell recipientWell = new MessageRecipientWell();

		private string quotaWarningMessage;

		private string quotaExceededMessage;

		private NavigationBar navigationBar;

		private bool? isReInitializationRequest = null;
	}
}
