using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal static class EnvelopeJournalUtility
	{
		public static EnvelopeJournalVersion CheckEnvelopeJournalVersion(EmailMessage emailMessage)
		{
			if (emailMessage == null || emailMessage.RootPart == null)
			{
				return EnvelopeJournalVersion.None;
			}
			HeaderList headers = emailMessage.RootPart.Headers;
			Header[] array = headers.FindAll("Content-Identifier");
			foreach (Header header in array)
			{
				if (string.Compare(MessageUtility.GetHeaderValue(header), "exjournalreport", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return EnvelopeJournalVersion.Exchange2003;
				}
			}
			array = headers.FindAll("Content-Identifer");
			foreach (Header header2 in array)
			{
				if (string.Compare(MessageUtility.GetHeaderValue(header2), "exjournalreport", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return EnvelopeJournalVersion.Exchange2003;
				}
			}
			array = headers.FindAll("X-MS-Journal-Report");
			if (array.Length > 0)
			{
				return EnvelopeJournalVersion.Exchange2007;
			}
			return EnvelopeJournalVersion.None;
		}

		public static EnvelopeJournalReport ExtractEnvelopeJournalMessage(EmailMessage emailMessage, EnvelopeJournalVersion journalReportVersion)
		{
			return EnvelopeJournalUtility.ExtractEnvelopeJournalMessage(emailMessage, journalReportVersion, true);
		}

		internal static T ReadHeaderValueWithDefault<T>(HeaderList headerList, string headerTag, T defaultValue, EnvelopeJournalUtility.Parser<T> parser)
		{
			T result = defaultValue;
			if (headerList != null)
			{
				Header header = headerList.FindFirst(headerTag);
				string text = string.Empty;
				if (header != null)
				{
					text = MessageUtility.GetHeaderValue(header);
					if (!string.IsNullOrEmpty(text) && !parser(text, out result))
					{
						result = defaultValue;
					}
				}
			}
			return result;
		}

		private static EnvelopeJournalReport ExtractEnvelopeJournalMessage(EmailMessage emailMessage, EnvelopeJournalVersion journalReportVersion, bool retrieveMessage)
		{
			if (emailMessage == null)
			{
				throw new ArgumentNullException("emailMessage");
			}
			if (journalReportVersion == EnvelopeJournalVersion.None)
			{
				throw new ArgumentException("Journal Report verison is invalid");
			}
			EmailMessage emailMessage2 = null;
			Attachment embeddedMessageAttachment = null;
			foreach (Attachment attachment in emailMessage.Attachments)
			{
				if (attachment.EmbeddedMessage != null)
				{
					emailMessage2 = attachment.EmbeddedMessage;
					embeddedMessageAttachment = attachment;
					break;
				}
			}
			if (emailMessage2 == null)
			{
				throw new InvalidEnvelopeJournalMessageException(AgentStrings.InvalidEnvelopeJournalMessageMissingEmbedded(emailMessage.MessageId));
			}
			bool flag = false;
			Header header;
			if (emailMessage2.RootPart != null)
			{
				header = emailMessage2.RootPart.Headers.FindFirst(HeaderId.MessageId);
				if (header == null || string.IsNullOrEmpty(MessageUtility.GetHeaderValue(header)))
				{
					flag = true;
				}
			}
			Exception ex = null;
			EnvelopeJournalReport envelopeJournalReport = null;
			try
			{
				envelopeJournalReport = EnvelopeJournalUtility.ParseJournalReportFromEmailMessage(emailMessage, journalReportVersion, flag);
			}
			catch (ArgumentNullException ex2)
			{
				ex = ex2;
			}
			catch (FormatException ex3)
			{
				ex = ex3;
			}
			catch (Exception ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				throw new InvalidEnvelopeJournalMessageException(AgentStrings.InvalidEnvelopeJournalMessagesInvalidFormat(emailMessage.MessageId, ex.ToString()));
			}
			if (envelopeJournalReport == null)
			{
				throw new InvalidEnvelopeJournalMessageException(AgentStrings.InvalidEnvelopeJournalMessageMissingReport(emailMessage.MessageId));
			}
			AddressInfo sender = envelopeJournalReport.Sender;
			List<AddressInfo> recipients = envelopeJournalReport.Recipients;
			string messageId = envelopeJournalReport.MessageId;
			bool defective = envelopeJournalReport.Defective;
			envelopeJournalReport.EmbeddedMessageAttachment = embeddedMessageAttachment;
			if (sender == null || sender.Address == RoutingAddress.NullReversePath)
			{
				throw new InvalidEnvelopeJournalMessageException(AgentStrings.InvalidEnvelopeJournalMessageMissingSender(emailMessage.MessageId));
			}
			if (flag && messageId == null)
			{
				throw new InvalidEnvelopeJournalMessageException(AgentStrings.InvalidEnvelopeJournalMessageMissingRequiredMessageId(emailMessage.MessageId));
			}
			header = emailMessage2.RootPart.Headers.FindFirst(HeaderId.MessageId);
			if (header == null)
			{
				header = Header.Create(HeaderId.MessageId);
				header.Value = messageId;
				emailMessage2.RootPart.Headers.AppendChild(header);
			}
			else if (string.IsNullOrEmpty(MessageUtility.GetHeaderValue(header)))
			{
				header.Value = messageId;
			}
			EnvelopeJournalUtility.RemoveNullReversePathRecipients(envelopeJournalReport);
			return envelopeJournalReport;
		}

		private static void RemoveNullReversePathRecipients(EnvelopeJournalReport report)
		{
			List<AddressInfo> list = new List<AddressInfo>(report.Recipients);
			foreach (AddressInfo addressInfo in list)
			{
				if (addressInfo.Address == RoutingAddress.NullReversePath)
				{
					report.Recipients.Remove(addressInfo);
				}
			}
		}

		private static EnvelopeJournalReport ParseEnvelopeJournalReport2003(Stream mimeStream, bool plaintext)
		{
			AddressInfo senderAddress = null;
			List<AddressInfo> list = new List<AddressInfo>();
			string messageIdString = string.Empty;
			bool defective = false;
			using (ReportReader reportReader = new ReportReader(mimeStream, plaintext))
			{
				string text = null;
				JournalReportTags journalReportTags = JournalReportTags.None;
				JournalReportTags journalReportTags2 = JournalReportTags.Sender | JournalReportTags.MessageId | JournalReportTags.Recipients;
				JournalReportTags journalReportTags3 = JournalReportTags.None;
				string text2;
				do
				{
					text2 = (text ?? reportReader.ReadLine());
					text = null;
					string friendlyName = string.Empty;
					string text3 = string.Empty;
					string text4 = string.Empty;
					if (text2 != null)
					{
						if (journalReportTags3 == JournalReportTags.None)
						{
							if ((journalReportTags & JournalReportTags.Sender) == JournalReportTags.None && text2.StartsWith("Sender:", StringComparison.OrdinalIgnoreCase))
							{
								journalReportTags3 = JournalReportTags.Sender;
							}
							else if ((journalReportTags & JournalReportTags.MessageId) == JournalReportTags.None && text2.StartsWith("Message-ID:", StringComparison.OrdinalIgnoreCase))
							{
								journalReportTags3 = JournalReportTags.MessageId;
							}
							else if ((journalReportTags & JournalReportTags.Recipients) == JournalReportTags.None && text2.StartsWith("Recipients:", StringComparison.OrdinalIgnoreCase))
							{
								journalReportTags3 = JournalReportTags.Recipients;
								if (Match.Empty != EnvelopeJournalUtility.RegexJournalReportRecipient2003.Match(text2))
								{
									goto IL_4D7;
								}
							}
						}
						switch (journalReportTags3)
						{
						case JournalReportTags.Sender:
						{
							int num = 0;
							Match match;
							while (Match.Empty == (match = EnvelopeJournalUtility.RegexJournalReportSender2003.Match(text2)))
							{
								text = reportReader.ReadLine();
								if (num >= 5 || text == null || MessageUtility.IsBlankLine(text))
								{
									break;
								}
								if (text.StartsWith("Message-ID:", StringComparison.OrdinalIgnoreCase))
								{
									journalReportTags3 = JournalReportTags.MessageId;
									break;
								}
								if (text.StartsWith("Recipients:", StringComparison.OrdinalIgnoreCase))
								{
									journalReportTags3 = JournalReportTags.Recipients;
									if (Match.Empty != EnvelopeJournalUtility.RegexJournalReportRecipient2003.Match(text))
									{
										text = null;
										break;
									}
									break;
								}
								else
								{
									text2 += text;
									num++;
								}
							}
							if (Match.Empty != match)
							{
								text = null;
								string text5 = string.Empty;
								friendlyName = match.Result("${DisplayName}");
								text3 = match.Result("${AddressType}");
								text4 = match.Result("${Address}");
								if (!string.IsNullOrEmpty(text3) && text3.Equals("smtp"))
								{
									text5 = text4;
								}
								else
								{
									string text6 = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
									{
										text3,
										text4
									});
									text5 = text6;
								}
								if (!string.IsNullOrEmpty(text5) && RoutingAddress.IsValidAddress(text5))
								{
									senderAddress = new AddressInfo(friendlyName, new RoutingAddress(text5));
								}
							}
							journalReportTags |= JournalReportTags.Sender;
							if (JournalReportTags.Sender == journalReportTags3)
							{
								journalReportTags3 = JournalReportTags.None;
							}
							break;
						}
						case JournalReportTags.MessageId:
						{
							int num2 = 0;
							for (;;)
							{
								text = reportReader.ReadLine();
								if (num2 >= 14 || text2.Length >= 1020 || text == null)
								{
									goto IL_194;
								}
								if (MessageUtility.IsBlankLine(text))
								{
									goto Block_16;
								}
								if (text.StartsWith("Sender:", StringComparison.OrdinalIgnoreCase))
								{
									goto Block_17;
								}
								if (text.StartsWith("Recipients:", StringComparison.OrdinalIgnoreCase))
								{
									goto Block_18;
								}
								text2 += text;
								num2++;
							}
							IL_1A2:
							Match match;
							if (Match.Empty != match)
							{
								messageIdString = match.Groups[1].Value;
							}
							journalReportTags |= JournalReportTags.MessageId;
							if (JournalReportTags.MessageId == journalReportTags3)
							{
								journalReportTags3 = JournalReportTags.None;
								break;
							}
							break;
							IL_194:
							match = EnvelopeJournalUtility.RegexJournalReportMessageId.Match(text2);
							goto IL_1A2;
							Block_18:
							match = EnvelopeJournalUtility.RegexJournalReportMessageId.Match(text2);
							journalReportTags3 = JournalReportTags.Recipients;
							if (Match.Empty != EnvelopeJournalUtility.RegexJournalReportRecipient2003.Match(text))
							{
								text = null;
								goto IL_1A2;
							}
							goto IL_1A2;
							Block_17:
							match = EnvelopeJournalUtility.RegexJournalReportMessageId.Match(text2);
							journalReportTags3 = JournalReportTags.Sender;
							goto IL_1A2;
							Block_16:
							match = EnvelopeJournalUtility.RegexJournalReportMessageId.Match(text2);
							goto IL_1A2;
						}
						case JournalReportTags.Recipients:
							if (MessageUtility.IsBlankLine(text2))
							{
								if (plaintext)
								{
									journalReportTags |= JournalReportTags.Recipients;
									journalReportTags3 = JournalReportTags.None;
									break;
								}
								text2 = reportReader.ReadLine();
								if (text2 == null || MessageUtility.IsBlankLine(text2))
								{
									journalReportTags |= JournalReportTags.Recipients;
									journalReportTags3 = JournalReportTags.None;
									break;
								}
							}
							if (text2.StartsWith("Sender:", StringComparison.OrdinalIgnoreCase))
							{
								text = text2;
								journalReportTags3 = JournalReportTags.Sender;
							}
							else if (text2.StartsWith("Message-ID:", StringComparison.OrdinalIgnoreCase))
							{
								text = text2;
								journalReportTags3 = JournalReportTags.MessageId;
							}
							else
							{
								int num3 = 0;
								Match match;
								while (Match.Empty == (match = EnvelopeJournalUtility.RegexJournalReportAddress2003.Match(text2)))
								{
									text = reportReader.ReadLine();
									if (num3 >= 5 || text == null)
									{
										journalReportTags |= JournalReportTags.Recipients;
										journalReportTags3 = JournalReportTags.None;
										defective = true;
										break;
									}
									if (MessageUtility.IsBlankLine(text))
									{
										break;
									}
									if (text.StartsWith("Sender:", StringComparison.OrdinalIgnoreCase))
									{
										journalReportTags3 = JournalReportTags.Sender;
										break;
									}
									if (text.StartsWith("Message-ID:", StringComparison.OrdinalIgnoreCase))
									{
										journalReportTags3 = JournalReportTags.MessageId;
										break;
									}
									text2 += text;
									num3++;
								}
								if (Match.Empty != match)
								{
									text = null;
									string text7 = string.Empty;
									friendlyName = match.Result("${DisplayName}");
									text3 = match.Result("${AddressType}");
									text4 = match.Result("${Address}");
									if (!string.IsNullOrEmpty(text3) && text3.Equals("smtp"))
									{
										text7 = text4;
										AddressInfo item = new AddressInfo(friendlyName, new RoutingAddress(text4));
										list.Add(item);
									}
									else
									{
										string text8 = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
										{
											text3,
											text4
										});
										text7 = text8;
									}
									if (!string.IsNullOrEmpty(text7) && RoutingAddress.IsValidAddress(text7))
									{
										AddressInfo item2 = new AddressInfo(friendlyName, new RoutingAddress(text7));
										list.Add(item2);
									}
								}
							}
							break;
						}
					}
					IL_4D7:;
				}
				while (journalReportTags2 != (journalReportTags & journalReportTags2) && text2 != null);
			}
			return new EnvelopeJournalReport(senderAddress, list, messageIdString, defective);
		}

		private static EnvelopeJournalReport ParseEnvelopeJournalReport2007(Stream mimeStream, bool plaintext)
		{
			AddressInfo addressInfo = null;
			List<AddressInfo> list = new List<AddressInfo>();
			string text = string.Empty;
			using (ReportReader reportReader = new ReportReader(mimeStream, plaintext))
			{
				string input;
				while ((input = reportReader.ReadLine()) != null)
				{
					string text2 = string.Empty;
					string text3 = string.Empty;
					string empty = string.Empty;
					string empty2 = string.Empty;
					Match match;
					if (addressInfo == null && Match.Empty != (match = EnvelopeJournalUtility.RegexJournalReportSender2007.Match(input)))
					{
						text3 = match.Result("${SmtpAddress}").ToUpperInvariant();
						if (!string.IsNullOrEmpty(text3) && RoutingAddress.IsValidAddress(text3))
						{
							addressInfo = new AddressInfo(new RoutingAddress(text3));
						}
					}
					else if (Match.Empty != (match = EnvelopeJournalUtility.RegexJournalReportRecipient2007.Match(input)))
					{
						text2 = match.Result("${RecipientType}");
						text3 = match.Result("${SmtpAddress}");
						match.Result("${SubField}");
						match.Result("${SubFieldSmtpAddress}");
						if (!string.IsNullOrEmpty(text3) && RoutingAddress.IsValidAddress(text3))
						{
							AddressInfo addressInfo2 = new AddressInfo(new RoutingAddress(text3));
							string a;
							if ((a = text2) != null)
							{
								if (!(a == "To"))
								{
									if (!(a == "Cc"))
									{
										if (a == "Bcc")
										{
											addressInfo2.IncludedInBcc = true;
										}
									}
									else
									{
										addressInfo2.IncludedInCc = true;
									}
								}
								else
								{
									addressInfo2.IncludedInTo = true;
								}
							}
							list.Add(addressInfo2);
						}
					}
					else if (string.IsNullOrEmpty(text) && Match.Empty != (match = EnvelopeJournalUtility.RegexJournalReportMessageId.Match(input)))
					{
						text = match.Groups[1].Value;
					}
				}
			}
			return new EnvelopeJournalReport(addressInfo, list, text, false);
		}

		private static EnvelopeJournalReport ParseJournalReportFromEmailMessage(EmailMessage emailMessage, EnvelopeJournalVersion journalReportVersion, bool requireMessageId)
		{
			MimePart mimePart = emailMessage.Body.MimePart;
			if (mimePart != null)
			{
				while (mimePart.PreviousSibling != null)
				{
					mimePart = (mimePart.PreviousSibling as MimePart);
				}
				while (mimePart != null)
				{
					EnvelopeJournalReport envelopeJournalReport = EnvelopeJournalUtility.ParseJournalReportFromMimePart(mimePart, journalReportVersion);
					if (envelopeJournalReport.Sender != null && (!requireMessageId || !string.IsNullOrEmpty(envelopeJournalReport.MessageId)))
					{
						return envelopeJournalReport;
					}
					mimePart = (mimePart.NextSibling as MimePart);
				}
			}
			foreach (Attachment attachment in emailMessage.Attachments)
			{
				mimePart = attachment.MimePart;
				if (mimePart != null)
				{
					EnvelopeJournalReport envelopeJournalReport = EnvelopeJournalUtility.ParseJournalReportFromMimePart(mimePart, journalReportVersion);
					if (envelopeJournalReport.Sender != null && (!requireMessageId || !string.IsNullOrEmpty(envelopeJournalReport.MessageId)))
					{
						return envelopeJournalReport;
					}
				}
			}
			return null;
		}

		private static EnvelopeJournalReport ParseJournalReportFromMimePart(MimePart mimePart, EnvelopeJournalVersion journalReportVersion)
		{
			EnvelopeJournalReport result = null;
			bool plaintext;
			if (mimePart.ContentType.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
			{
				plaintext = true;
			}
			else
			{
				if (!mimePart.ContentType.Equals("text/html", StringComparison.OrdinalIgnoreCase))
				{
					return EnvelopeJournalUtility.emptyReport;
				}
				plaintext = false;
			}
			using (Stream mimePartReadStream = MessageUtility.GetMimePartReadStream(mimePart))
			{
				switch (journalReportVersion)
				{
				case EnvelopeJournalVersion.Exchange2003:
					result = EnvelopeJournalUtility.ParseEnvelopeJournalReport2003(mimePartReadStream, plaintext);
					break;
				case EnvelopeJournalVersion.Exchange2007:
					result = EnvelopeJournalUtility.ParseEnvelopeJournalReport2007(mimePartReadStream, plaintext);
					break;
				}
			}
			return result;
		}

		private const string MatchResultDomain = "${Domain}";

		private const string MatchResultDisplayName = "${DisplayName}";

		private const string MatchResultAddressType = "${AddressType}";

		private const string MatchResultAddress = "${Address}";

		private const string MatchResultSmtpAddress = "${SmtpAddress}";

		private const string MatchResultSubField = "${SubField}";

		private const string MatchResultSubFieldSmtpAddress = "${SubFieldSmtpAddress}";

		private const string MatchResultRecipientType = "${RecipientType}";

		private const string MatchResultValue = "${value}";

		private const string MatchResultExt = "${ext}";

		private const string NonSmtpAddressFormat = "{0}:{1}";

		private static readonly Regex RegexJournalReportSender2003 = new Regex("^Sender:\\s*(\"(?<DisplayName>[^\"]*(\\\"[^\"]*)*)\")?\\s*<(?<AddressType>[^:]*):(?<Address>([^>]*|<>))>\\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex RegexJournalReportAddress2003 = new Regex("\\s*(\"(?<DisplayName>[^\"]*(\\\"[^\"]*)*)\")?\\s*<(?<AddressType>[^:]*):(?<Address>[^>]*)>\\s*,?\\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex RegexJournalReportRecipient2003 = new Regex("^Recipients:\\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex RegexJournalReportSender2007 = new Regex("^Sender:\\s*(?<SmtpAddress>.+)\\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex RegexJournalReportRecipient2007 = new Regex("^(?<RecipientType>Recipient|To|Cc|Bcc|On-Behalf-Of):\\s*(?<SmtpAddress>.+?)(,\\s*(?<SubField>Expanded|Forwarded):\\s*(?<SubFieldSmtpAddress>\\S+))?\\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex RegexJournalReportMessageId = new Regex("Message-Id:\\s*(.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static EnvelopeJournalReport emptyReport = new EnvelopeJournalReport(null, null, string.Empty, false);

		internal delegate bool Parser<T>(string toBeParsed, out T result);
	}
}
