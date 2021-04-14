using System;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal class Navigation
	{
		private HttpRequest Request
		{
			get
			{
				return this.request;
			}
		}

		private UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		public static NavigationModule GetNavigationModuleFromFolder(UserContext userContext, StoreObjectId folderId)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			ModuleViewState moduleViewState = userContext.LastClientViewState as ModuleViewState;
			if (moduleViewState != null && moduleViewState.FolderId == folderId)
			{
				return moduleViewState.NavigationModule;
			}
			string className;
			using (Folder folder = Folder.Bind(userContext.MailboxSession, folderId))
			{
				className = folder.ClassName;
			}
			if (className == null)
			{
				return NavigationModule.Mail;
			}
			if (ObjectClass.IsCalendarFolder(className))
			{
				return NavigationModule.Calendar;
			}
			if (ObjectClass.IsContactsFolder(className))
			{
				return NavigationModule.Contacts;
			}
			return NavigationModule.Mail;
		}

		public Navigation(NavigationModule navigationModule, OwaContext owaContext, TextWriter writer)
		{
			this.navigationModule = navigationModule;
			this.request = owaContext.HttpContext.Request;
			this.userContext = owaContext.UserContext;
			this.writer = writer;
		}

		public void Render()
		{
			this.SetParameters();
			if (this.UserContext.UserOptions.PrimaryNavigationCollapsed)
			{
				this.writer.Write("<table cellspacing=0 cellpadding=0 class=\"pntc\"><caption>");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(-730863418));
				this.writer.Write("</caption><tr><td class=\"tp\"><img class=\"crv\" src=\"");
				this.UserContext.RenderThemeFileUrl(this.writer, ThemeFileId.CornerTopLeft);
				this.writer.Write("\" alt=\"\"></td><td class=\"txt\" nowrap>");
				if (this.navigationModule == NavigationModule.Mail)
				{
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(405905481));
				}
				else if (this.navigationModule == NavigationModule.Calendar)
				{
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(1292798904));
				}
				else if (this.navigationModule == NavigationModule.Contacts)
				{
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(1716044995));
				}
				else if (this.navigationModule == NavigationModule.Options)
				{
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(1511584348));
				}
				else if (this.navigationModule == NavigationModule.AddressBook)
				{
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(1139489555));
				}
				this.writer.Write("</td><td class=\"btn\"><a href=\"#\" id=\"lnkNavMail\" onclick=\"return onClkPN(");
				this.writer.Write(0);
				this.writer.Write(");\" title=\"");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(405905481));
				this.writer.Write("\"");
				if (this.navigationModule == NavigationModule.Mail)
				{
					this.writer.Write(" class=\"s\"");
				}
				this.writer.Write("><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.EMail2Small);
				this.writer.Write("\" alt=\"");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(405905481));
				this.writer.Write("\"></a></td>");
				if (this.UserContext.IsFeatureEnabled(Feature.Calendar))
				{
					this.writer.Write("<td class=\"btn\"><a href=\"#\" id=\"lnkNavCal\" onClick=\"return onClkPN(");
					this.writer.Write(1);
					this.writer.Write(");\" title=\"");
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(1292798904));
					this.writer.Write("\"");
					if (this.navigationModule == NavigationModule.Calendar)
					{
						this.writer.Write(" class=\"s\"");
					}
					this.writer.Write("><img src=\"");
					this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Calendar2Small);
					this.writer.Write("\" alt=\"");
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(1292798904));
					this.writer.Write("\"></a></td>");
				}
				if (this.UserContext.IsFeatureEnabled(Feature.Contacts))
				{
					this.writer.Write("<td class=\"btn\"><a href=\"#\" id=\"lnkNavContact\" onClick=\"return onClkPN(");
					this.writer.Write(2);
					this.writer.Write(");\" title=\"");
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(1716044995));
					this.writer.Write("\"");
					if (this.navigationModule == NavigationModule.Contacts)
					{
						this.writer.Write(" class=\"s\"");
					}
					this.writer.Write("><img src=\"");
					this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Contact2Small);
					this.writer.Write("\" alt=\"");
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(447307630));
					this.writer.Write("\"></a></td>");
				}
				this.writer.Write("</tr></table>");
				this.writer.Write("<table cellspacing=0 cellpadding=0 class=\"pntc\"><tr><td class=\"rsz\" align=\"center\"><img class=\"nvImg\" src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
				this.writer.Write("\" alt=\"\">");
				this.writer.Write("<a href=\"#\" onClick=\"return onClkPNTgl(1);\" title=\"\"><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.NavigationResize);
				this.writer.Write("\" alt=\"");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(-552806466));
				this.writer.Write("\"></a></td></tr></table>");
			}
			else
			{
				this.writer.Write("<table cellspacing=0 cellpadding=0 class=\"pnt\"><caption>");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(-730863418));
				this.writer.Write("</caption><tr><td class=\"ml\" nowrap><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.CornerTopLeft);
				this.writer.Write("\" class=\"crv\" alt=\"\"><a href=\"#\" id=\"lnkNavMail\" onclick=\"return onClkPN(");
				this.writer.Write(0);
				this.writer.Write(");\" title=\"");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(405905481));
				this.writer.Write("\" class=\"pn tp ");
				switch (this.navigationModule)
				{
				case NavigationModule.Mail:
					this.writer.Write("s db");
					break;
				case NavigationModule.Calendar:
					this.writer.Write("db");
					break;
				default:
					this.writer.Write("lb");
					break;
				}
				this.writer.Write("\"><img class=\"tpNoSrc\" src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
				this.writer.Write("\" alt=\"\"><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.EMail2);
				this.writer.Write("\" alt=\"\">&nbsp;");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(405905481));
				this.writer.Write("</a></td></tr>");
				if (this.UserContext.IsFeatureEnabled(Feature.Calendar))
				{
					this.writer.Write("<tr><td nowrap><a href=\"#\" id=\"lnkNavCal\" onclick=\"return onClkPN(");
					this.writer.Write(1);
					this.writer.Write(");\" title=\"");
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(1292798904));
					this.writer.Write("\" class=\"pn ");
					switch (this.navigationModule)
					{
					case NavigationModule.Calendar:
						this.writer.Write("s db");
						break;
					case NavigationModule.Contacts:
						this.writer.Write("db");
						break;
					default:
						this.writer.Write("lb");
						break;
					}
					this.writer.Write("\"><img class=\"crvNoSrc\" src=\"");
					this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
					this.writer.Write("\" alt=\"\"><img src=\"");
					this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Calendar2);
					this.writer.Write("\" alt=\"\">&nbsp;");
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(1292798904));
					this.writer.Write("</a></td></tr>");
				}
				if (this.UserContext.IsFeatureEnabled(Feature.Contacts))
				{
					this.writer.Write("<tr><td nowrap><a href=\"#\"  id=\"lnkNavContact\" onclick=\"return onClkPN(");
					this.writer.Write(2);
					this.writer.Write(");\" title=\"");
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(1716044995));
					this.writer.Write("\" class=\"pn ");
					if (this.navigationModule == NavigationModule.Contacts)
					{
						this.writer.Write("s");
					}
					this.writer.Write("\"><img class=\"crvNoSrc\" src=\"");
					this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
					this.writer.Write("\" alt=\"\"><img src=\"");
					this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Contact2);
					this.writer.Write("\" alt=\"\">&nbsp;");
					this.writer.Write(LocalizedStrings.GetHtmlEncoded(1716044995));
					this.writer.Write("</a></td></tr>");
				}
				this.writer.Write("<tr><td class=\"rsz\" align=\"center\"><img class=\"nvImg\" src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.Clear);
				this.writer.Write("\" alt=\"\">");
				this.writer.Write("<a href=\"#\" id=\"lnkNavResz\" onClick=\"return onClkPNTgl(0);\" title=\"\"><img src=\"");
				this.userContext.RenderThemeFileUrl(this.writer, ThemeFileId.NavigationResize);
				this.writer.Write("\"  alt=\"");
				this.writer.Write(LocalizedStrings.GetHtmlEncoded(-693980577));
				this.writer.Write("\"></a></td></tr></table>");
			}
			this.writer.Write("<input type=\"hidden\" name=\"hidpnst\" value=\"\">");
		}

		private void SetParameters()
		{
			string formParameter = Utilities.GetFormParameter(this.Request, "hidpnst", false);
			if (!this.UserContext.IsWebPartRequest && string.Equals(this.Request.HttpMethod, "post", StringComparison.OrdinalIgnoreCase))
			{
				if (string.Equals(formParameter, "0", StringComparison.Ordinal))
				{
					this.UserContext.UserOptions.PrimaryNavigationCollapsed = true;
					this.UserContext.UserOptions.CommitChanges();
					return;
				}
				if (string.Equals(formParameter, "1", StringComparison.Ordinal))
				{
					this.UserContext.UserOptions.PrimaryNavigationCollapsed = false;
					this.UserContext.UserOptions.CommitChanges();
					return;
				}
				if (!string.IsNullOrEmpty(formParameter))
				{
					throw new OwaInvalidRequestException("primaryNavigationStateFormParameterValue can only be 0 or 1");
				}
			}
		}

		private const string PrimaryNavigationStateFormParameter = "hidpnst";

		private NavigationModule navigationModule;

		private HttpRequest request;

		private UserContext userContext;

		private TextWriter writer;
	}
}
