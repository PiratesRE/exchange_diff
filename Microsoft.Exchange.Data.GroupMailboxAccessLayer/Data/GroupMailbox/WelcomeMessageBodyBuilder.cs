using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class WelcomeMessageBodyBuilder
	{
		public static void WriteWarmingMessageBody(StreamWriter streamWriter, string encodedGroupDisplayName, CultureInfo cultureInfo)
		{
			ArgumentValidator.ThrowIfNull("streamWriter", streamWriter);
			streamWriter.Write(string.Format("<html><body><font face={0}><table cellspacing=\"0\"cellpadding=\"0\"border=\"0\"><tbody><tr><td style=\"height:64px;\"colspan=\"2\">{1}</td></tr><tr><td colspan=\"2\"><div style=\"margin-top:0px;height:20px;\"/></td></tr></tbody></table><table style=\"font-size:8pt;margin-top:0px;border-color:#EAEAEA\"align=\"left\"cellspacing=\"0\"cellpadding=\"0\"width=\"639\"border=\"0\"><tbody><tr>{2}</tr></tbody></table></font></body></html>", ClientStrings.GroupMailboxAddedMemberMessageFont.ToString(cultureInfo), string.Format("<div style=\"margin-left:20px;font-size:18pt;color:#0072C6\">{0}</div><div style=\"margin-left:20px;margin-top:5px;font-size:10pt;color:#000000\">{1}</div>", ClientStrings.GroupMailboxWelcomeMessageHeader1(encodedGroupDisplayName).ToString(cultureInfo), ClientStrings.GroupMailboxWelcomeMessageHeader2.ToString(cultureInfo)), WelcomeMessageBodyBuilder.GenerateGroupContent(cultureInfo, ModernGroupObjectType.None, null, null, null)));
		}

		private static string GenerateGroupContent(CultureInfo cultureInfo, ModernGroupObjectType groupType = ModernGroupObjectType.None, string inboxUrl = null, string calendarUrl = null, string sharePointUrl = null)
		{
			bool flag = string.IsNullOrEmpty(inboxUrl);
			string arg = string.Format("<div style=\"margin:0 20px 5px 20px\">{0}<img onerror='this.style.display=\"none\"' border=\"0\"src=\"{1}\"width=\"64\"height=\"64\">{2}</div><div style=\"margin:5px 20px;color:#0072C6;font-size:14pt\">{3}</div><div style=\"margin:5px 20px;color:#666666;font-size:10pt\">{4}</div><div style=\"margin:5px 20px;color:#0072C6;font-size:10pt\">{5}</div>", new object[]
			{
				flag ? string.Empty : string.Format("<a href=\"{0}\">", inboxUrl),
				"cid:" + WelcomeMessageBodyBuilder.ConversationIcon.ImageId,
				flag ? string.Empty : "</a>",
				ClientStrings.GroupMailboxAddedMemberMessageConversation1.ToString(cultureInfo),
				ClientStrings.GroupMailboxAddedMemberMessageConversation2.ToString(cultureInfo),
				flag ? string.Empty : string.Format("<a href=\"{0}\">{1}</a>", inboxUrl, ClientStrings.GroupMailboxAddedMemberMessageConversation3.ToString(cultureInfo))
			});
			bool flag2 = string.IsNullOrEmpty(sharePointUrl);
			string arg2 = string.Format("<div style=\"margin:0 20px 5px 20px\">{0}<img onerror='this.style.display=\"none\"' border=\"0\"src=\"{1}\"width=\"64\"height=\"64\">{2}</div><div style=\"margin:5px 20px;color:#0072C6;font-size:14pt\">{3}</div><div style=\"margin:5px 20px;color:#666666;font-size:10pt\">{4}</div><div style=\"margin:5px 20px;color:#0072C6;font-size:10pt\">{5}</div>", new object[]
			{
				flag2 ? string.Empty : string.Format("<a href=\"{0}\">", sharePointUrl),
				"cid:" + WelcomeMessageBodyBuilder.DocumentIcon.ImageId,
				flag2 ? string.Empty : "</a>",
				ClientStrings.GroupMailboxAddedMemberMessageDocument1.ToString(cultureInfo),
				ClientStrings.GroupMailboxAddedMemberMessageDocument2.ToString(cultureInfo),
				flag2 ? string.Empty : string.Format("<a href=\"{0}\">{1}</a>", sharePointUrl, ClientStrings.GroupMailboxAddedMemberMessageDocument3.ToString(cultureInfo))
			});
			bool flag3 = string.IsNullOrEmpty(calendarUrl);
			string arg3 = string.Format("<div style=\"margin:0 20px 5px 20px\">{0}<img onerror='this.style.display=\"none\"' border=\"0\"src=\"{1}\"width=\"64\"height=\"64\">{2}</div><div style=\"margin:5px 20px;color:#0072C6;font-size:14pt\">{3}</div><div style=\"margin:5px 20px;color:#666666;font-size:10pt\">{4}</div><div style=\"margin:5px 20px;color:#0072C6;font-size:10pt\">{5}</div>", new object[]
			{
				flag3 ? string.Empty : string.Format("<a href=\"{0}\">", calendarUrl),
				"cid:" + WelcomeMessageBodyBuilder.CalendarIcon.ImageId,
				flag3 ? string.Empty : "</a>",
				ClientStrings.GroupMailboxAddedMemberMessageCalendar1.ToString(cultureInfo),
				ClientStrings.GroupMailboxAddedMemberMessageCalendar2.ToString(cultureInfo),
				flag3 ? string.Empty : string.Format("<a href=\"{0}\">{1}</a>", calendarUrl, ClientStrings.GroupMailboxAddedMemberMessageCalendar3.ToString(cultureInfo))
			});
			return string.Format("<td valign=\"top\"width=\"213\">{0}</td><td valign=\"top\"width=\"213\">{1}</td><td valign=\"top\"width=\"213\">{2}</td>", arg, arg2, arg3);
		}

		private static string GetGroupTypeMessage(ModernGroupObjectType groupType, CultureInfo cultureInfo)
		{
			switch (groupType)
			{
			case ModernGroupObjectType.Private:
				return ClientStrings.GroupMailboxWelcomeEmailPrivateTypeText.ToString(cultureInfo);
			case ModernGroupObjectType.Public:
				return ClientStrings.GroupMailboxWelcomeEmailPublicTypeText.ToString(cultureInfo);
			}
			return string.Empty;
		}

		private const string WarmingMessageBodyFormat = "<html><body><font face={0}><table cellspacing=\"0\"cellpadding=\"0\"border=\"0\"><tbody><tr><td style=\"height:64px;\"colspan=\"2\">{1}</td></tr><tr><td colspan=\"2\"><div style=\"margin-top:0px;height:20px;\"/></td></tr></tbody></table><table style=\"font-size:8pt;margin-top:0px;border-color:#EAEAEA\"align=\"left\"cellspacing=\"0\"cellpadding=\"0\"width=\"639\"border=\"0\"><tbody><tr>{2}</tr></tbody></table></font></body></html>";

		private const string WelcomeMessageHeaderFormat = "<div style=\"margin-left:20px;margin-top:-5px;font-size:20pt;color:#FFFFFF\">{0}</div><div style=\"margin-left:20px;margin-top:-5px;font-size:10pt;color:#CDE6F7\">{1}</div>";

		private const string WarmingMessageHeaderFormat = "<div style=\"margin-left:20px;font-size:18pt;color:#0072C6\">{0}</div><div style=\"margin-left:20px;margin-top:5px;font-size:10pt;color:#000000\">{1}</div>";

		public static readonly ImageAttachment ConversationIcon = new ImageAttachment("conversation_icon.png", "conversationicon", "image/png", null);

		public static readonly ImageAttachment DocumentIcon = new ImageAttachment("document_icon.png", "documenticon", "image/png", null);

		public static readonly ImageAttachment CalendarIcon = new ImageAttachment("calendar_icon.png", "calendaricon", "image/png", null);
	}
}
