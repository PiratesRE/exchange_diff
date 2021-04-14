using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public static class NotificationRenderingUtilities
	{
		public static void RenderNotificationMenu(UserContext userContext, TextWriter output, string divPrefix, string divAttributes, Strings.IDs title, Strings.IDs description, string buttonAllId, Strings.IDs buttonAllText, string button1Id, Strings.IDs button1Text, string button2Id, Strings.IDs button2Text, string additionalHtml)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			output.Write("<div id=\"");
			output.Write(divPrefix);
			output.Write("Dlg\"");
			output.Write(divAttributes);
			output.Write(" class=\"notificationsDialog\" style=\"display:none;\"><div class=\"alertPopupShading\"></div><div id=\"");
			output.Write(divPrefix);
			output.Write("DlgTopBorderLeft\" class=\"alertDialogTopBorder\"></div><div id=\"");
			output.Write(divPrefix);
			output.Write("DlgTopBorderRight\" class=\"alertDialogTopBorder\"></div><div id=\"");
			output.Write(divPrefix);
			output.Write("DlgBdy\" class=\"notificationsDialogBody\"><div class=\"notificationsDialogTitle\">");
			output.Write(LocalizedStrings.GetHtmlEncoded(title));
			output.Write("</div>");
			if (description != -1018465893)
			{
				output.Write("<div class=\"notificationsDialogDescription\">");
				output.Write(LocalizedStrings.GetHtmlEncoded(description));
				output.Write("</div>");
			}
			output.Write("<div id=\"");
			output.Write(divPrefix);
			output.Write("Err\" style=\"display:none;\">");
			userContext.RenderThemeImage(output, ThemeFileId.Error, "rmdErrImg", new object[0]);
			output.Write("<div id=\"");
			output.Write(divPrefix);
			output.Write("ErrTxt\" unselectable=on></div></div>");
			output.Write("<div id=\"");
			output.Write(divPrefix);
			output.Write("Data\" class=\"notificationsDialogData\"></div><div class=\"notificationsButtons\">");
			if (!string.IsNullOrEmpty(buttonAllId))
			{
				output.Write("<a id=\"");
				output.Write(buttonAllId);
				output.Write("\" preventDisable=\"1\" class=\"aNotificationsButtonAll\">");
				output.Write(LocalizedStrings.GetHtmlEncoded(buttonAllText));
				output.Write("</a>");
			}
			if (!string.IsNullOrEmpty(button1Id))
			{
				output.Write("<a id=\"");
				output.Write(button1Id);
				output.Write("\" class=\"aNotificationsButton1\">");
				output.Write(LocalizedStrings.GetHtmlEncoded(button1Text));
				output.Write("</a>");
				output.Write("<div class=\"divNotificationsButtonSep\"></div>");
			}
			if (!string.IsNullOrEmpty(button2Id))
			{
				output.Write("<a id=\"");
				output.Write(button2Id);
				output.Write("\" class=\"aNotificationsButton2\">");
				output.Write(LocalizedStrings.GetHtmlEncoded(button2Text));
				output.Write("</a>");
			}
			output.Write("</div>");
			output.Write(additionalHtml);
			output.Write("</div>");
			RenderingUtilities.RenderDropShadowsForDialogWithButton(output, userContext);
			output.Write("</div>");
		}
	}
}
