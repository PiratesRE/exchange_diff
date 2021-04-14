using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class InfobarMessageBuilder : InfobarMessageBuilderBase
	{
		public static void AddImportance(Infobar infobar, Item item)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			string importance = InfobarMessageBuilderBase.GetImportance(item);
			if (importance != null)
			{
				infobar.AddMessageText(importance, InfobarMessageType.Informational);
			}
		}

		public static void AddSensitivity(Infobar infobar, Item item)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			string sensitivity = InfobarMessageBuilderBase.GetSensitivity(item);
			if (sensitivity != null)
			{
				infobar.AddMessageText(sensitivity, InfobarMessageType.Informational);
			}
		}

		public static void AddFlag(Infobar infobar, Item item, UserContext userContext)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			string flag = InfobarMessageBuilderBase.GetFlag(item, userContext);
			if (flag != null)
			{
				infobar.AddMessageText(flag, InfobarMessageType.Informational);
			}
		}

		public static void AddCompliance(UserContext userContext, Infobar infobar, Item item, bool isSenderMessage)
		{
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			string compliance = InfobarMessageBuilderBase.GetCompliance(userContext, item, isSenderMessage);
			if (compliance != null)
			{
				infobar.AddMessageText(compliance, InfobarMessageType.Informational);
			}
		}

		public static void AddSendReceiptNotice(UserContext userContext, Infobar infobar, MessageItem messageItem)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			if (messageItem == null)
			{
				throw new ArgumentNullException("item");
			}
			if (InfobarMessageBuilderBase.ShouldRenderReadReceiptNoticeInfobar(userContext, messageItem))
			{
				SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(115261126));
				sanitizingStringBuilder.Append(" <a href=\"#\" onclick=\"onClkSndRct()\">");
				sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(1190033799));
				sanitizingStringBuilder.Append("</a>");
				infobar.AddMessageHtml(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>(), InfobarMessageType.Informational);
			}
		}
	}
}
