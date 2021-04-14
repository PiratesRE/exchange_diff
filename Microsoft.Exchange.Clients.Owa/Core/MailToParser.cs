using System;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class MailToParser
	{
		private MailToParser()
		{
		}

		public static bool TryParseMailTo(string mailToUrlValue, UserContext userContext, out StoreObjectId mailToItemId)
		{
			if (mailToUrlValue == null)
			{
				throw new ArgumentNullException("mailToUrlValue");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			mailToItemId = null;
			if (null == Utilities.TryParseUri(mailToUrlValue))
			{
				return false;
			}
			using (MessageItem messageItem = MessageItem.Create(userContext.MailboxSession, userContext.DraftsFolderId))
			{
				messageItem[ItemSchema.ConversationIndexTracking] = true;
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.ItemsCreated.Increment();
				}
				if (!MailToParser.TryMailToParse(messageItem, mailToUrlValue))
				{
					return false;
				}
				messageItem.Save(SaveMode.ResolveConflicts);
				messageItem.Load();
				mailToItemId = messageItem.Id.ObjectId;
			}
			return true;
		}

		private static bool TryMailToParse(MessageItem mailToMessage, string mailTo)
		{
			char[] separators = new char[]
			{
				',',
				'?'
			};
			char[] separators2 = new char[]
			{
				',',
				'&'
			};
			int num = 0;
			if (!mailTo.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			num += "mailto:".Length;
			MailToParser.ProcessingFlags processType = MailToParser.ProcessingFlags.ProcessTo;
			num = MailToParser.ProcessTokensFromCommaSeparatedString(mailToMessage, mailTo, processType, num, separators, '?');
			if (num < 0 && num >= mailTo.Length)
			{
				return true;
			}
			while (num >= 0 && num < mailTo.Length)
			{
				processType = MailToParser.ProcessingFlags.None;
				if (object.Equals(mailTo[num], 'b') || object.Equals(mailTo[num], 'B'))
				{
					if (mailTo.Length > num + "bcc=".Length && string.Compare(mailTo, num, "bcc=", 0, "bcc=".Length, StringComparison.OrdinalIgnoreCase) == 0)
					{
						num += "bcc=".Length;
						processType = MailToParser.ProcessingFlags.ProcessBcc;
					}
					else if (mailTo.Length > num + "body=".Length && string.Compare(mailTo, num, "body=", 0, "body=".Length, StringComparison.OrdinalIgnoreCase) == 0)
					{
						num += "body=".Length;
						processType = MailToParser.ProcessingFlags.ProcessBody;
					}
				}
				else if (object.Equals(mailTo[num], 'c') || object.Equals(mailTo[num], 'C'))
				{
					if (mailTo.Length > num + "cc=".Length && string.Compare(mailTo, num, "cc=", 0, "cc=".Length, StringComparison.OrdinalIgnoreCase) == 0)
					{
						num += "cc=".Length;
						processType = MailToParser.ProcessingFlags.ProcessCc;
					}
				}
				else if (object.Equals(mailTo[num], 's') || object.Equals(mailTo[num], 'S'))
				{
					if (mailTo.Length > num + "subject=".Length && string.Compare(mailTo, num, "subject=", 0, "subject=".Length, StringComparison.OrdinalIgnoreCase) == 0)
					{
						num += "subject=".Length;
						processType = MailToParser.ProcessingFlags.ProcessSubject;
					}
				}
				else if ((object.Equals(mailTo[num], 't') || object.Equals(mailTo[num], 'T')) && mailTo.Length > num + "to=".Length && string.Compare(mailTo, num, "to=", 0, "to=".Length, StringComparison.OrdinalIgnoreCase) == 0)
				{
					num += "to=".Length;
					processType = MailToParser.ProcessingFlags.ProcessTo;
				}
				num = MailToParser.ProcessTokensFromCommaSeparatedString(mailToMessage, mailTo, processType, num, separators2, '&');
			}
			return true;
		}

		private static int ProcessTokensFromCommaSeparatedString(MessageItem mailToMessage, string mailTo, MailToParser.ProcessingFlags processType, int currentIndex, char[] separators, char terminator)
		{
			bool flag = false;
			int num;
			while ((num = mailTo.IndexOfAny(separators, currentIndex)) != -1)
			{
				string token = mailTo.Substring(currentIndex, num - currentIndex);
				MailToParser.ProcessToken(mailToMessage, token, processType);
				if (mailTo[num] == terminator)
				{
					currentIndex = num + 1;
					flag = true;
					break;
				}
				currentIndex = num + 1;
			}
			if (!flag)
			{
				MailToParser.ProcessToken(mailToMessage, mailTo.Substring(currentIndex), processType);
			}
			if (num < 0)
			{
				return num;
			}
			return currentIndex;
		}

		private static void ProcessToken(MessageItem mailToMessage, string token, MailToParser.ProcessingFlags processType)
		{
			switch (processType)
			{
			case MailToParser.ProcessingFlags.None:
				break;
			case MailToParser.ProcessingFlags.ProcessBcc:
				MailToParser.ProcessRecipients(mailToMessage, token, RecipientItemType.Bcc);
				return;
			case MailToParser.ProcessingFlags.ProcessCc:
				MailToParser.ProcessRecipients(mailToMessage, token, RecipientItemType.Cc);
				return;
			case MailToParser.ProcessingFlags.ProcessTo:
				MailToParser.ProcessRecipients(mailToMessage, token, RecipientItemType.To);
				return;
			case MailToParser.ProcessingFlags.ProcessSubject:
				if (!string.IsNullOrEmpty(token))
				{
					token = HttpUtility.UrlDecode(token);
					mailToMessage.Subject = token;
					return;
				}
				break;
			case MailToParser.ProcessingFlags.ProcessBody:
				if (!string.IsNullOrEmpty(token))
				{
					token = HttpUtility.UrlDecode(token);
					ItemUtility.SetItemBody(mailToMessage, BodyFormat.TextPlain, token);
				}
				break;
			default:
				return;
			}
		}

		private static void ProcessRecipients(MessageItem mailToMessage, string value, RecipientItemType recipientItemType)
		{
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			value = HttpUtility.UrlDecode(value);
			Participant participant;
			bool flag = Participant.TryParse(value, out participant);
			if (flag)
			{
				ProxyAddress proxyAddress;
				if (ImceaAddress.IsImceaAddress(participant.EmailAddress) && SmtpProxyAddress.TryDeencapsulate(participant.EmailAddress, out proxyAddress))
				{
					participant = new Participant((participant.DisplayName != participant.EmailAddress) ? participant.DisplayName : proxyAddress.AddressString, proxyAddress.AddressString, proxyAddress.PrefixString);
				}
				mailToMessage.Recipients.Add(participant, recipientItemType);
			}
		}

		public const string MailToParameter = "email";

		private enum ProcessingFlags
		{
			None,
			ProcessBcc,
			ProcessCc,
			ProcessTo,
			ProcessSubject,
			ProcessBody
		}
	}
}
