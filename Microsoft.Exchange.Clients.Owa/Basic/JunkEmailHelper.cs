using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public static class JunkEmailHelper
	{
		internal static InfobarMessage MarkAsJunk(UserContext userContext, params StoreObjectId[] ids)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (ids == null)
			{
				throw new ArgumentNullException("ids");
			}
			InfobarMessage result = null;
			string defaultFolderDisplayName = Utilities.GetDefaultFolderDisplayName(userContext.MailboxSession, DefaultFolderType.JunkEmail);
			if (ids.Length == 1)
			{
				SanitizedHtmlString sanitizedHtmlString = SanitizedHtmlString.Empty;
				string text;
				try
				{
					text = Utilities.GetSenderSmtpAddress(ids[0].ToBase64String(), userContext);
				}
				catch (ObjectNotFoundException)
				{
					text = null;
				}
				if (!string.IsNullOrEmpty(text) && userContext.IsJunkEmailEnabled && !JunkEmailUtilities.IsInternalToOrganization(text, userContext))
				{
					sanitizedHtmlString = JunkEmailHelper.BuildAddToListLinkString(JunkEmailListType.BlockedSenders, text);
				}
				AggregateOperationResult aggregateOperationResult = userContext.MailboxSession.Move(userContext.JunkEmailFolderId, ids);
				if (aggregateOperationResult.OperationResult == OperationResult.Succeeded)
				{
					if (SanitizedStringBase<OwaHtml>.IsNullOrEmpty(sanitizedHtmlString))
					{
						result = InfobarMessage.CreateText(string.Format(LocalizedStrings.GetNonEncoded(-1881687711), defaultFolderDisplayName, sanitizedHtmlString), InfobarMessageType.Informational);
					}
					else
					{
						result = InfobarMessage.CreateHtml(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1881687711), new object[]
						{
							defaultFolderDisplayName,
							sanitizedHtmlString
						}), InfobarMessageType.Informational);
					}
				}
				else if (SanitizedStringBase<OwaHtml>.IsNullOrEmpty(sanitizedHtmlString))
				{
					result = InfobarMessage.CreateText(string.Format(LocalizedStrings.GetNonEncoded(1568239397), sanitizedHtmlString), InfobarMessageType.Informational);
				}
				else
				{
					result = InfobarMessage.CreateHtml(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(1568239397), new object[]
					{
						sanitizedHtmlString
					}), InfobarMessageType.Informational);
				}
			}
			else
			{
				AggregateOperationResult aggregateOperationResult2 = userContext.MailboxSession.Move(userContext.JunkEmailFolderId, ids);
				switch (aggregateOperationResult2.OperationResult)
				{
				case OperationResult.Succeeded:
					result = InfobarMessage.CreateText(string.Format(LocalizedStrings.GetNonEncoded(1632419544), defaultFolderDisplayName), InfobarMessageType.Informational);
					break;
				case OperationResult.Failed:
					result = InfobarMessage.CreateLocalized(-2089288056, InfobarMessageType.Informational);
					break;
				case OperationResult.PartiallySucceeded:
					result = InfobarMessage.CreateLocalized(483208264, InfobarMessageType.Informational);
					break;
				}
			}
			return result;
		}

		internal static InfobarMessage MarkAsNotJunk(UserContext userContext, params StoreObjectId[] ids)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (ids == null)
			{
				throw new ArgumentNullException("ids");
			}
			InfobarMessage result = null;
			string defaultFolderDisplayName = Utilities.GetDefaultFolderDisplayName(userContext.MailboxSession, DefaultFolderType.Inbox);
			if (ids.Length == 1)
			{
				SanitizedHtmlString sanitizedHtmlString = SanitizedHtmlString.Empty;
				string text;
				try
				{
					text = Utilities.GetSenderSmtpAddress(ids[0].ToBase64String(), userContext);
				}
				catch (ObjectNotFoundException)
				{
					text = null;
				}
				if (!string.IsNullOrEmpty(text) && userContext.IsJunkEmailEnabled && !JunkEmailUtilities.IsInternalToOrganization(text, userContext))
				{
					sanitizedHtmlString = JunkEmailHelper.BuildAddToListLinkString(JunkEmailListType.SafeSenders, text);
				}
				AggregateOperationResult aggregateOperationResult = userContext.MailboxSession.Move(userContext.InboxFolderId, ids);
				if (aggregateOperationResult.OperationResult == OperationResult.Succeeded)
				{
					if (SanitizedStringBase<OwaHtml>.IsNullOrEmpty(sanitizedHtmlString))
					{
						result = InfobarMessage.CreateText(string.Format(LocalizedStrings.GetNonEncoded(-1881687711), defaultFolderDisplayName, sanitizedHtmlString), InfobarMessageType.Informational);
					}
					else
					{
						result = InfobarMessage.CreateHtml(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1881687711), new object[]
						{
							defaultFolderDisplayName,
							sanitizedHtmlString
						}), InfobarMessageType.Informational);
					}
				}
				else if (SanitizedStringBase<OwaHtml>.IsNullOrEmpty(sanitizedHtmlString))
				{
					result = InfobarMessage.CreateText(string.Format(LocalizedStrings.GetNonEncoded(300728662), sanitizedHtmlString), InfobarMessageType.Informational);
				}
				else
				{
					result = InfobarMessage.CreateHtml(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(300728662), new object[]
					{
						sanitizedHtmlString
					}), InfobarMessageType.Informational);
				}
			}
			else
			{
				AggregateOperationResult aggregateOperationResult2 = userContext.MailboxSession.Move(userContext.InboxFolderId, ids);
				switch (aggregateOperationResult2.OperationResult)
				{
				case OperationResult.Succeeded:
					result = InfobarMessage.CreateText(string.Format(LocalizedStrings.GetNonEncoded(1632419544), defaultFolderDisplayName), InfobarMessageType.Informational);
					break;
				case OperationResult.Failed:
					result = InfobarMessage.CreateLocalized(1682703853, InfobarMessageType.Informational);
					break;
				case OperationResult.PartiallySucceeded:
					result = InfobarMessage.CreateLocalized(521322677, InfobarMessageType.Informational);
					break;
				}
			}
			return result;
		}

		internal static InfobarMessage AddEmailToSendersList(UserContext userContext, HttpRequest httpRequest)
		{
			string formParameter = Utilities.GetFormParameter(httpRequest, "hidsndrslst");
			string formParameter2 = Utilities.GetFormParameter(httpRequest, "hidsndreml");
			string messageText;
			if (JunkEmailUtilities.Add(formParameter2, JunkEmailHelper.GetListType(formParameter), userContext, false, out messageText))
			{
				return InfobarMessage.CreateText(messageText, InfobarMessageType.Informational);
			}
			return InfobarMessage.CreateText(messageText, InfobarMessageType.Error);
		}

		public static JunkEmailListType GetListType(string listName)
		{
			if (listName != null)
			{
				if (listName == "Ssl")
				{
					return JunkEmailListType.SafeSenders;
				}
				if (listName == "Bsl")
				{
					return JunkEmailListType.BlockedSenders;
				}
				if (listName == "Srl")
				{
					return JunkEmailListType.SafeRecipients;
				}
			}
			throw new OwaInvalidRequestException("Unknown list");
		}

		public static string GetListName(JunkEmailListType listType)
		{
			switch (listType)
			{
			case JunkEmailListType.SafeSenders:
				return "Ssl";
			case JunkEmailListType.BlockedSenders:
				return "Bsl";
			case JunkEmailListType.SafeRecipients:
				return "Srl";
			default:
				throw new ArgumentOutOfRangeException("listType");
			}
		}

		private static SanitizedHtmlString BuildAddToListLinkString(JunkEmailListType junkEmailListType, string email)
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("&nbsp;&nbsp;<a href=\"#\" onclick=\"return onClkAddEml()\">");
			if (junkEmailListType == JunkEmailListType.SafeSenders)
			{
				sanitizingStringBuilder.Append(Strings.AddToSafeSendersList(email));
			}
			else
			{
				sanitizingStringBuilder.Append(Strings.AddToBlockedSendersList(email));
			}
			sanitizingStringBuilder.Append("</a>");
			sanitizingStringBuilder.Append("<input type=\"hidden\" name=\"");
			sanitizingStringBuilder.Append("hidsndrslst");
			sanitizingStringBuilder.Append("\" value=\"");
			if (junkEmailListType == JunkEmailListType.SafeSenders)
			{
				sanitizingStringBuilder.Append("Ssl");
			}
			else
			{
				sanitizingStringBuilder.Append("Bsl");
			}
			sanitizingStringBuilder.Append("\"><input type=\"hidden\" name=\"");
			sanitizingStringBuilder.Append("hidsndreml");
			sanitizingStringBuilder.Append("\" value=\"");
			sanitizingStringBuilder.Append(email);
			sanitizingStringBuilder.Append("\">");
			return sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>();
		}

		public const string SafeSendersListName = "Ssl";

		public const string BlockedSendersListName = "Bsl";

		public const string SafeRecipientsListName = "Srl";

		private const string SendersListNameParameter = "hidsndrslst";

		private const string SenderEmailAddressParameter = "hidsndreml";
	}
}
