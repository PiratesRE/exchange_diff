using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	internal static class ReplyForwardHeader
	{
		public static void CreateForwardReplyHeader(ReplyForwardConfiguration configuration, Item item, ForwardReplyHeaderOptions headerOptions = null)
		{
			if (!(item is MessageItem) && !(item is CalendarItemBase) && !(item is PostItem))
			{
				throw new ArgumentException("HTML reply forward headers can only be created for MessageItem, CalendarItemBase and PostItem");
			}
			if (headerOptions == null)
			{
				headerOptions = new ForwardReplyHeaderOptions();
			}
			CultureInfo cultureInfo = configuration.Culture ?? item.Session.InternalCulture;
			if (item is PostItem)
			{
				ReplyForwardHeader.CreatePostReplyForwardHeader(configuration, item, headerOptions, cultureInfo);
			}
			IList<IRecipientBase> toRecipients = null;
			IList<IRecipientBase> ccRecipients = null;
			string from = string.Empty;
			string sender = null;
			string fromLabel = ClientStrings.FromColon.ToString(cultureInfo);
			string toLabel = ClientStrings.ToColon.ToString(cultureInfo);
			string ccLabel = ClientStrings.CcColon.ToString(cultureInfo);
			bool isMeetingItem = ObjectClass.IsMeetingMessage(item.ClassName);
			if (item is MessageItem)
			{
				MessageItem messageItem = (MessageItem)item;
				if (messageItem.From != null)
				{
					from = messageItem.From.DisplayName;
				}
				if (messageItem.Sender != null)
				{
					sender = messageItem.Sender.DisplayName;
				}
				toRecipients = ReplyForwardHeader.GetMessageRecipientCollection(RecipientItemType.To, messageItem);
				ccRecipients = ReplyForwardHeader.GetMessageRecipientCollection(RecipientItemType.Cc, messageItem);
			}
			else if (item is CalendarItemBase)
			{
				CalendarItemBase calendarItemBase = (CalendarItemBase)item;
				if (calendarItemBase.Organizer != null)
				{
					from = calendarItemBase.Organizer.DisplayName;
				}
				toRecipients = ReplyForwardHeader.GetCalendarItemRecipientCollection(AttendeeType.Required, calendarItemBase);
				ccRecipients = ReplyForwardHeader.GetCalendarItemRecipientCollection(AttendeeType.Optional, calendarItemBase);
			}
			BodyInjectionFormat bodyPrefixFormat;
			string bodyPrefix;
			switch (configuration.TargetFormat)
			{
			case BodyFormat.TextPlain:
				bodyPrefixFormat = BodyInjectionFormat.Text;
				bodyPrefix = ReplyForwardHeader.CreateTextReplyForwardHeader(item, headerOptions, fromLabel, from, sender, toLabel, ccLabel, toRecipients, ccRecipients, isMeetingItem, cultureInfo, configuration.TimeZone);
				break;
			case BodyFormat.TextHtml:
			case BodyFormat.ApplicationRtf:
				bodyPrefixFormat = BodyInjectionFormat.Html;
				bodyPrefix = ReplyForwardHeader.CreateHtmlReplyForwardHeader(item, headerOptions, fromLabel, from, sender, toLabel, ccLabel, toRecipients, ccRecipients, isMeetingItem, cultureInfo, configuration.TimeZone);
				break;
			default:
				throw new ArgumentException("Unsupported body format");
			}
			configuration.AddBodyPrefix(bodyPrefix, bodyPrefixFormat);
		}

		internal static string CreateHtmlReplyForwardHeader(Item item, ForwardReplyHeaderOptions headerOptions, string fromLabel, string from, string sender, string toLabel, string ccLabel, IList<IRecipientBase> toRecipients, IList<IRecipientBase> ccRecipients, bool isMeetingItem, CultureInfo culture, ExTimeZone timeZone)
		{
			StringBuilder stringBuilder = new StringBuilder(150);
			using (StringWriter stringWriter = new StringWriter(stringBuilder, culture))
			{
				ReplyForwardHeader.RenderDefaultUserFontMarkup(headerOptions, stringWriter);
			}
			stringBuilder.Append("<HR style=\"display:inline-block;width:98%\" tabindex=\"-1\">");
			stringBuilder.Append("<FONT FACE=\"Tahoma\" size=\"2\">");
			bool flag = !string.IsNullOrEmpty(from);
			bool flag2 = !string.IsNullOrEmpty(sender);
			if (flag2 || flag)
			{
				stringBuilder.Append("<B>");
				stringBuilder.Append(fromLabel);
				stringBuilder.Append("</B> ");
				if (flag2 && string.Compare(sender, from, StringComparison.Ordinal) != 0)
				{
					ReplyForwardHeader.HtmlEncode(sender, stringBuilder);
					if (flag)
					{
						stringBuilder.Append(string.Format(culture, ClientStrings.OnBehalfOf.ToString(culture), new object[]
						{
							string.Empty,
							string.Empty
						}));
					}
				}
				if (flag)
				{
					ReplyForwardHeader.HtmlEncode(from, stringBuilder);
				}
				stringBuilder.Append("<BR>");
			}
			object obj = item.TryGetProperty(ItemSchema.SentTime);
			if (obj != null && obj is ExDateTime)
			{
				stringBuilder.Append("<B>");
				stringBuilder.Append(ClientStrings.SentColon.ToString(culture));
				stringBuilder.Append("</B> ");
				ExDateTime exDateTime = (ExDateTime)obj;
				if (timeZone != null)
				{
					exDateTime = timeZone.ConvertDateTime(exDateTime);
					stringBuilder.AppendFormat(ClientStrings.SentTime.ToString(culture), exDateTime.ToString("D", culture), exDateTime.ToString("T", culture), timeZone.LocalizableDisplayName.ToString(culture));
				}
				else
				{
					stringBuilder.AppendFormat(ClientStrings.SentTime.ToString(culture), exDateTime.ToString("D", culture), exDateTime.ToString("T", culture), exDateTime.TimeZone.LocalizableDisplayName.ToString(culture));
				}
				stringBuilder.Append("<BR>");
			}
			if (0 < toRecipients.Count)
			{
				stringBuilder.Append("<B>");
				stringBuilder.Append(toLabel);
				stringBuilder.Append("</B> ");
				int num = 0;
				foreach (IRecipientBase recipientBase in toRecipients)
				{
					num++;
					ReplyForwardHeader.HtmlEncode(recipientBase.Participant.DisplayName, stringBuilder);
					if (num < toRecipients.Count)
					{
						stringBuilder.Append("; ");
					}
				}
				stringBuilder.Append("<BR>");
			}
			if (0 < ccRecipients.Count)
			{
				stringBuilder.Append("<B>");
				stringBuilder.Append(ccLabel);
				stringBuilder.Append("</B> ");
				int num2 = 0;
				foreach (IRecipientBase recipientBase2 in ccRecipients)
				{
					num2++;
					ReplyForwardHeader.HtmlEncode(recipientBase2.Participant.DisplayName, stringBuilder);
					if (num2 < ccRecipients.Count)
					{
						stringBuilder.Append("; ");
					}
				}
				stringBuilder.Append("<BR>");
			}
			stringBuilder.Append("<B>");
			stringBuilder.Append(ClientStrings.SubjectColon.ToString(culture));
			stringBuilder.Append("</B> ");
			string text = item.TryGetProperty(ItemSchema.Subject) as string;
			if (text == null)
			{
				text = string.Empty;
			}
			ReplyForwardHeader.HtmlEncode(text, stringBuilder);
			stringBuilder.Append("<BR>");
			if (isMeetingItem)
			{
				stringBuilder.Append("<B>");
				stringBuilder.Append(ClientStrings.WhenPart.ToString(culture));
				stringBuilder.Append("</B> ");
				ReplyForwardHeader.HtmlEncode(ReplyForwardHeader.GenerateWhen(item), stringBuilder);
				stringBuilder.Append("<BR><B>");
				stringBuilder.Append(ClientStrings.WherePart.ToString(culture));
				stringBuilder.Append("</B> ");
				string text2 = item.TryGetProperty(CalendarItemBaseSchema.Location) as string;
				if (!string.IsNullOrEmpty(text2))
				{
					ReplyForwardHeader.HtmlEncode(text2, stringBuilder);
				}
				stringBuilder.Append("<BR>");
			}
			stringBuilder.Append("</FONT><BR></DIV>");
			return stringBuilder.ToString();
		}

		internal static string CreateTextReplyForwardHeader(Item item, ForwardReplyHeaderOptions headerOptions, string fromLabel, string from, string sender, string toLabel, string ccLabel, IList<IRecipientBase> toRecipients, IList<IRecipientBase> ccRecipients, bool isMeetingItem, CultureInfo culture, ExTimeZone timeZone)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("\n\n");
			if (headerOptions.AutoAddSignature)
			{
				stringBuilder.Append("\n\n");
				stringBuilder.Append(headerOptions.SignatureText);
			}
			stringBuilder.Append("\n________________________________________\n");
			bool flag = !string.IsNullOrEmpty(from);
			bool flag2 = !string.IsNullOrEmpty(sender);
			if (flag2 || flag)
			{
				stringBuilder.Append(fromLabel);
				stringBuilder.Append(" ");
				if (flag2 && string.Compare(sender, from, StringComparison.Ordinal) != 0)
				{
					stringBuilder.Append(sender);
					if (flag)
					{
						stringBuilder.Append(string.Format(culture, ClientStrings.OnBehalfOf.ToString(culture), new object[]
						{
							string.Empty,
							string.Empty
						}));
					}
				}
				if (flag)
				{
					stringBuilder.Append(from);
				}
				stringBuilder.Append("\n");
			}
			object obj = item.TryGetProperty(ItemSchema.SentTime);
			if (obj != null && obj is ExDateTime)
			{
				stringBuilder.Append(ClientStrings.SentColon.ToString(culture));
				stringBuilder.Append(" ");
				ExDateTime exDateTime = (ExDateTime)obj;
				if (timeZone != null)
				{
					exDateTime = timeZone.ConvertDateTime(exDateTime);
					stringBuilder.AppendFormat(ClientStrings.SentTime.ToString(culture), exDateTime.ToString("D", culture), exDateTime.ToString("T", culture), timeZone.LocalizableDisplayName.ToString(culture));
				}
				else
				{
					stringBuilder.AppendFormat(ClientStrings.SentTime.ToString(culture), exDateTime.ToString("D", culture), exDateTime.ToString("T", culture), exDateTime.TimeZone.LocalizableDisplayName.ToString(culture));
				}
				stringBuilder.Append("\n");
			}
			if (0 < toRecipients.Count)
			{
				stringBuilder.Append(toLabel);
				stringBuilder.Append(" ");
				int num = 0;
				foreach (IRecipientBase recipientBase in toRecipients)
				{
					num++;
					stringBuilder.Append(recipientBase.Participant.DisplayName);
					if (num < toRecipients.Count)
					{
						stringBuilder.Append("; ");
					}
				}
				stringBuilder.Append("\n");
			}
			if (0 < ccRecipients.Count)
			{
				stringBuilder.Append(ccLabel);
				stringBuilder.Append(" ");
				int num2 = 0;
				foreach (IRecipientBase recipientBase2 in ccRecipients)
				{
					num2++;
					stringBuilder.Append(recipientBase2.Participant.DisplayName);
					if (num2 < ccRecipients.Count)
					{
						stringBuilder.Append("; ");
					}
				}
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(ClientStrings.SubjectColon.ToString(culture));
			stringBuilder.Append(" ");
			string text = item.TryGetProperty(ItemSchema.Subject) as string;
			if (text == null)
			{
				text = string.Empty;
			}
			stringBuilder.Append(text);
			stringBuilder.Append("\n");
			if (isMeetingItem)
			{
				stringBuilder.Append(ClientStrings.WhenPart.ToString(culture));
				stringBuilder.Append(" ");
				ReplyForwardHeader.HtmlEncode(ReplyForwardHeader.GenerateWhen(item), stringBuilder);
				stringBuilder.Append("\n");
				stringBuilder.Append(ClientStrings.WherePart.ToString(culture));
				stringBuilder.Append(" ");
				string text2 = item.TryGetProperty(CalendarItemBaseSchema.Location) as string;
				if (!string.IsNullOrEmpty(text2))
				{
					ReplyForwardHeader.HtmlEncode(text2, stringBuilder);
				}
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		public static void CreatePostReplyForwardHeader(ReplyForwardConfiguration configuration, Item item, ForwardReplyHeaderOptions options, CultureInfo culture)
		{
			if (!(item is PostItem))
			{
				throw new ArgumentException("CreatePostReplyForwardheader is called on a non-PostItem item.");
			}
			PostItem postItem = (PostItem)item;
			StringBuilder stringBuilder = new StringBuilder();
			bool outputHtml = BodyFormat.TextHtml == configuration.TargetFormat;
			if (postItem.Sender != null)
			{
				if (postItem.From != null && string.Compare(postItem.Sender.DisplayName, postItem.From.DisplayName, StringComparison.Ordinal) != 0)
				{
					stringBuilder.Append(string.Format(ClientStrings.OnBehalfOf.ToString(culture), ReplyForwardHeader.GetParticipantDisplayString(postItem.Sender, outputHtml), ReplyForwardHeader.GetParticipantDisplayString(postItem.From, outputHtml)));
				}
				else
				{
					ReplyForwardHeader.AppendParticipantDisplayString(postItem.Sender, stringBuilder, outputHtml);
				}
			}
			string fromLabel = string.Empty;
			BodyInjectionFormat bodyPrefixFormat;
			string bodyPrefix;
			switch (configuration.TargetFormat)
			{
			case BodyFormat.TextPlain:
				fromLabel = ClientStrings.FromColon.ToString(culture);
				bodyPrefixFormat = BodyInjectionFormat.Text;
				bodyPrefix = ReplyForwardHeader.CreatePostTextReplyForwardHeader(postItem, options, fromLabel, stringBuilder.ToString(), culture, configuration.TimeZone);
				break;
			case BodyFormat.TextHtml:
				fromLabel = ClientStrings.FromColon.ToString(culture);
				bodyPrefixFormat = BodyInjectionFormat.Html;
				bodyPrefix = ReplyForwardHeader.CreatePostHtmlReplyForwardHeader(postItem, options, fromLabel, stringBuilder.ToString(), culture, configuration.TimeZone);
				break;
			default:
				throw new ArgumentException("Unsupported body format");
			}
			configuration.AddBodyPrefix(bodyPrefix, bodyPrefixFormat);
		}

		private static string CreatePostHtmlReplyForwardHeader(PostItem item, ForwardReplyHeaderOptions headerOptions, string fromLabel, string fromHtmlMarkup, CultureInfo culture, ExTimeZone timeZone)
		{
			StringBuilder stringBuilder = new StringBuilder(150);
			using (StringWriter stringWriter = new StringWriter(stringBuilder, culture))
			{
				ReplyForwardHeader.RenderDefaultUserFontMarkup(headerOptions, stringWriter);
			}
			stringBuilder.Append("<DIV id=divRplyFwdMsg>");
			stringBuilder.Append("<HR style=\"display:inline-block;width:98%\" tabindex=\"-1\">");
			stringBuilder.Append("<FONT FACE=\"Tahoma\" size=\"2\">");
			if (!string.IsNullOrEmpty(fromHtmlMarkup))
			{
				stringBuilder.Append("<B>");
				stringBuilder.Append(fromLabel);
				stringBuilder.Append("</B> ");
				stringBuilder.Append(fromHtmlMarkup);
				stringBuilder.Append("<BR>");
			}
			ExDateTime postedTime = item.PostedTime;
			stringBuilder.Append("<B>");
			stringBuilder.Append(ClientStrings.PostedOn.ToString(culture));
			stringBuilder.Append("</B> ");
			if (timeZone == null)
			{
				ExDateTime exDateTime = item.PostedTime;
				stringBuilder.AppendFormat(ClientStrings.SentTime.ToString(culture), exDateTime.ToString("D", culture), exDateTime.ToString("T", culture), exDateTime.TimeZone.LocalizableDisplayName.ToString(culture));
			}
			else
			{
				ExDateTime exDateTime = timeZone.ConvertDateTime(item.PostedTime);
				stringBuilder.AppendFormat(ClientStrings.SentTime.ToString(culture), exDateTime.ToString("D", culture), exDateTime.ToString("T", culture), timeZone.LocalizableDisplayName.ToString(culture));
			}
			stringBuilder.Append("<BR>");
			stringBuilder.Append("<B>");
			stringBuilder.Append(ClientStrings.PostedTo.ToString(culture));
			stringBuilder.Append("</B> ");
			string s = null;
			using (Folder folder = Folder.Bind(item.Session, item.ParentId))
			{
				s = folder.DisplayName;
			}
			ReplyForwardHeader.HtmlEncode(s, stringBuilder);
			stringBuilder.Append("<BR>");
			stringBuilder.Append("<B>");
			stringBuilder.Append(ClientStrings.Conversation.ToString(culture));
			stringBuilder.Append("</B> ");
			ReplyForwardHeader.HtmlEncode(item.ConversationTopic, stringBuilder);
			stringBuilder.Append("<BR>");
			stringBuilder.Append("</FONT><BR></DIV>");
			stringBuilder.Append("</DIV>");
			return stringBuilder.ToString();
		}

		private static string CreatePostTextReplyForwardHeader(PostItem item, ForwardReplyHeaderOptions options, string fromLabel, string from, CultureInfo culture, ExTimeZone timeZone)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			if (!string.IsNullOrEmpty(options.SignatureText))
			{
				stringBuilder.Append(options.SignatureText);
			}
			stringBuilder.Append("\n________________________________________\n");
			if (!string.IsNullOrEmpty(from))
			{
				stringBuilder.Append(fromLabel);
				stringBuilder.Append(" ");
				stringBuilder.Append(from);
				stringBuilder.Append("\n");
			}
			ExDateTime postedTime = item.PostedTime;
			stringBuilder.Append(ClientStrings.PostedOn.ToString(culture));
			stringBuilder.Append(" ");
			if (timeZone == null)
			{
				ExDateTime exDateTime = item.PostedTime;
				stringBuilder.AppendFormat(ClientStrings.SentTime.ToString(culture), exDateTime.ToString("D", culture), exDateTime.ToString("T", culture), exDateTime.TimeZone.LocalizableDisplayName.ToString(culture));
			}
			else
			{
				ExDateTime exDateTime = timeZone.ConvertDateTime(item.PostedTime);
				stringBuilder.AppendFormat(ClientStrings.SentTime.ToString(culture), exDateTime.ToString("D", culture), exDateTime.ToString("T", culture), timeZone.LocalizableDisplayName.ToString(culture));
			}
			stringBuilder.Append("\n");
			stringBuilder.Append(ClientStrings.PostedTo.ToString(culture));
			stringBuilder.Append(" ");
			using (Folder folder = Folder.Bind(item.Session, item.ParentId))
			{
				stringBuilder.Append(folder.DisplayName);
			}
			stringBuilder.Append("\n");
			stringBuilder.Append(ClientStrings.Conversation.ToString(culture));
			stringBuilder.Append(" ");
			string value = null;
			if (item.Subject != null)
			{
				value = item.ConversationTopic;
			}
			stringBuilder.Append(value);
			stringBuilder.Append("\n");
			return stringBuilder.ToString();
		}

		private static IList<IRecipientBase> GetMessageRecipientCollection(RecipientItemType type, MessageItem item)
		{
			IList<IRecipientBase> list = new List<IRecipientBase>();
			foreach (Recipient recipient in item.Recipients)
			{
				if (recipient.RecipientItemType == type)
				{
					list.Add(recipient);
				}
			}
			return list;
		}

		private static IList<IRecipientBase> GetCalendarItemRecipientCollection(AttendeeType type, CalendarItemBase item)
		{
			IList<IRecipientBase> list = new List<IRecipientBase>();
			for (int i = 0; i < item.AttendeeCollection.Count; i++)
			{
				Attendee attendee = item.AttendeeCollection[i];
				if (attendee.AttendeeType == type)
				{
					list.Add(attendee);
				}
			}
			return list;
		}

		private static void RenderDefaultUserFontMarkup(ForwardReplyHeaderOptions headerOptions, TextWriter writer)
		{
			if (headerOptions.ComposeFontBold)
			{
				writer.Write("<strong>");
			}
			if (headerOptions.ComposeFontItalics)
			{
				writer.Write("<em>");
			}
			if (headerOptions.ComposeFontUnderline)
			{
				writer.Write("<u>");
			}
			writer.Write("<div><font face=\"");
			writer.Write(headerOptions.ComposeFontName);
			writer.Write("\" color=\"");
			writer.Write(headerOptions.ComposeFontColor);
			writer.Write("\" size=\"");
			writer.Write(headerOptions.ComposeFontSize);
			writer.Write("\">&nbsp;</font></div>");
			if (headerOptions.ComposeFontUnderline)
			{
				writer.Write("</u>");
			}
			if (headerOptions.ComposeFontItalics)
			{
				writer.Write("</em>");
			}
			if (headerOptions.ComposeFontBold)
			{
				writer.Write("</strong>");
			}
		}

		private static string GenerateWhen(Item item)
		{
			if (item is MeetingMessage)
			{
				return (item as MeetingMessage).GenerateWhen();
			}
			if (item is CalendarItemBase)
			{
				return (item as CalendarItemBase).GenerateWhen();
			}
			if (ObjectClass.IsMeetingMessage(item.ClassName))
			{
				return string.Empty;
			}
			throw new ArgumentException("Unsupported type, this is a bug");
		}

		internal static string GetParticipantDisplayString(Participant participant, bool outputHtml)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ReplyForwardHeader.AppendParticipantDisplayString(participant, stringBuilder, outputHtml);
			return stringBuilder.ToString();
		}

		internal static void AppendParticipantDisplayString(Participant participant, StringBuilder stringBuilder, bool outputHtml)
		{
			if (participant.DisplayName != null)
			{
				if (outputHtml)
				{
					ReplyForwardHeader.HtmlEncode(participant.DisplayName, stringBuilder);
				}
				else
				{
					stringBuilder.Append(participant.DisplayName);
				}
			}
			bool flag = false;
			string text = string.Empty;
			if (participant.RoutingType != null && string.CompareOrdinal(participant.RoutingType, "SMTP") == 0 && participant.EmailAddress != null)
			{
				flag = true;
				text = participant.EmailAddress;
			}
			if (flag)
			{
				if (outputHtml)
				{
					stringBuilder.Append(" [");
					ReplyForwardHeader.HtmlEncode(text, stringBuilder);
					stringBuilder.Append("]");
					return;
				}
				stringBuilder.Append(" [").Append(text).Append("]");
			}
		}

		private static void HtmlEncode(string s, StringBuilder stringBuilder)
		{
			stringBuilder.Append(AntiXssEncoder.HtmlEncode(s, false));
		}

		private const string MessageDelimiter = "\n________________________________________\n";

		public const string HRSeperator = "<HR style=\"display:inline-block;width:98%\" tabindex=\"-1\">";
	}
}
