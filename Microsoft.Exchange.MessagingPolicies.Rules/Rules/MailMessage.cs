using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Filtering;
using Microsoft.Filtering.Results;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class MailMessage
	{
		public MailMessage(MailItem mailItem)
		{
			this.mailItem = mailItem;
			this.headers = new MailMessage.MessageHeaders(mailItem);
			this.attachmentInfos = null;
			this.attachmentStreamIdentities = null;
		}

		public virtual MailMessage.MessageHeaders Headers
		{
			get
			{
				return this.headers;
			}
		}

		public string Subject
		{
			get
			{
				return this.mailItem.Message.Subject;
			}
		}

		public string Sender
		{
			get
			{
				EmailRecipient sender = this.mailItem.Message.Sender;
				if (sender == null)
				{
					return null;
				}
				return sender.SmtpAddress;
			}
		}

		public string MessageId
		{
			get
			{
				return this.GetStringFromFirstHeader(HeaderId.MessageId);
			}
		}

		public string InReplyTo
		{
			get
			{
				return this.GetStringFromFirstHeader(HeaderId.InReplyTo);
			}
		}

		public string References
		{
			get
			{
				return this.GetStringFromFirstHeader(HeaderId.References);
			}
		}

		public string ReturnPath
		{
			get
			{
				return this.GetStringFromFirstHeader(HeaderId.ReturnPath);
			}
		}

		public string Comments
		{
			get
			{
				return this.GetStringFromFirstHeader(HeaderId.Comments);
			}
		}

		public string Keywords
		{
			get
			{
				return this.GetStringFromFirstHeader(HeaderId.Keywords);
			}
		}

		public string ResentMessageId
		{
			get
			{
				return this.GetStringFromFirstHeader(HeaderId.ResentMessageId);
			}
		}

		public string From
		{
			get
			{
				EmailRecipient from = this.mailItem.Message.From;
				if (from == null)
				{
					return null;
				}
				return from.SmtpAddress;
			}
		}

		public List<string> To
		{
			get
			{
				EmailRecipientCollection to = this.mailItem.Message.To;
				List<string> list = new List<string>();
				foreach (EmailRecipient emailRecipient in to)
				{
					list.Add(StringUtil.Unwrap(emailRecipient.SmtpAddress));
				}
				return list;
			}
		}

		public List<string> Cc
		{
			get
			{
				EmailRecipientCollection cc = this.mailItem.Message.Cc;
				List<string> list = new List<string>();
				foreach (EmailRecipient emailRecipient in cc)
				{
					list.Add(StringUtil.Unwrap(emailRecipient.SmtpAddress));
				}
				return list;
			}
		}

		public List<string> ToCc
		{
			get
			{
				EmailRecipientCollection emailRecipientCollection = this.mailItem.Message.To;
				List<string> list = new List<string>();
				foreach (EmailRecipient emailRecipient in emailRecipientCollection)
				{
					list.Add(StringUtil.Unwrap(emailRecipient.SmtpAddress));
				}
				emailRecipientCollection = this.mailItem.Message.Cc;
				foreach (EmailRecipient emailRecipient2 in emailRecipientCollection)
				{
					list.Add(StringUtil.Unwrap(emailRecipient2.SmtpAddress));
				}
				return list;
			}
		}

		public List<string> Bcc
		{
			get
			{
				EmailRecipientCollection bcc = this.mailItem.Message.Bcc;
				List<string> list = new List<string>();
				foreach (EmailRecipient emailRecipient in bcc)
				{
					list.Add(StringUtil.Unwrap(emailRecipient.SmtpAddress));
				}
				return list;
			}
		}

		public List<string> ReplyTo
		{
			get
			{
				return this.GetAddressesFromFirstHeader(HeaderId.ReplyTo);
			}
		}

		public string EnvelopeFrom
		{
			get
			{
				return this.mailItem.FromAddress.ToString();
			}
		}

		public List<string> EnvelopeRecipients
		{
			get
			{
				List<string> list = new List<string>();
				foreach (EnvelopeRecipient envelopeRecipient in this.mailItem.Recipients)
				{
					list.Add(envelopeRecipient.Address.ToString());
				}
				return list;
			}
		}

		public string Auth
		{
			get
			{
				return this.mailItem.OriginalAuthenticator;
			}
		}

		public bool NeedsTracing
		{
			get
			{
				return ((ITransportMailItemWrapperFacade)this.mailItem).TransportMailItem.PipelineTracingEnabled;
			}
		}

		public IDictionary<string, object> ExtendedProperties
		{
			get
			{
				return this.mailItem.Properties;
			}
		}

		public MessageBodies Body
		{
			get
			{
				MessageBodies result;
				if ((result = this.body) == null)
				{
					result = (this.body = new MessageBodies(this.mailItem.Message, 0));
				}
				return result;
			}
		}

		public object SclValue
		{
			get
			{
				Header header = this.mailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-SCL");
				if (header != null)
				{
					try
					{
						int num;
						if (int.TryParse(header.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num) && num <= 9 && num >= -1)
						{
							return num;
						}
					}
					catch (ExchangeDataException)
					{
						return null;
					}
				}
				return null;
			}
		}

		public ulong Size
		{
			get
			{
				return (ulong)this.mailItem.MimeStreamLength;
			}
		}

		public ulong MaxAttachmentSize
		{
			get
			{
				ulong num = 0UL;
				AttachmentCollection attachments = this.mailItem.Message.Attachments;
				foreach (Attachment attachment in attachments)
				{
					Stream stream;
					long num2;
					if (attachment.TryGetContentReadStream(out stream))
					{
						num2 = stream.Length;
					}
					else
					{
						num2 = attachment.MimePart.GetRawContentReadStream().Length;
					}
					if (num2 < 0L)
					{
						num2 = 0L;
					}
					if (num2 > (long)num)
					{
						num = (ulong)num2;
					}
				}
				return num;
			}
		}

		public virtual List<string> AttachmentNames
		{
			get
			{
				return (from attachment in this.GetAttachmentInfos()
				where !TransportUtils.IsAttachmentExemptFromFilenameMatching(attachment)
				select attachment.Name).ToList<string>();
			}
		}

		public virtual List<string> AttachmentExtensions
		{
			get
			{
				return (from attachment in this.GetAttachmentInfos()
				where !TransportUtils.IsAttachmentExemptFromFilenameMatching(attachment)
				select attachment.Extension).ToList<string>();
			}
		}

		public virtual List<IDictionary<string, string>> AttachmentProperties
		{
			get
			{
				List<IDictionary<string, string>> list = new List<IDictionary<string, string>>();
				foreach (StreamIdentity identity in this.GetUnifiedContentResults().Streams.Where(new Func<StreamIdentity, bool>(MailMessage.IsAttachmentPart)))
				{
					Dictionary<string, string> item = new Dictionary<string, string>(RuleAgentResultUtils.GetCustomProperties(identity), StringComparer.InvariantCultureIgnoreCase);
					list.Add(item);
				}
				return list;
			}
		}

		public virtual List<string> AttachmentTypes
		{
			get
			{
				return (from x in this.GetAttachmentInfos()
				select x.FileType).ToList<string>();
			}
		}

		public virtual List<string> ContentCharacterSets
		{
			get
			{
				if (this.contentCharacterSets != null)
				{
					return this.contentCharacterSets;
				}
				if (this.mailItem.MimeDocument != null)
				{
					this.contentCharacterSets = MailMessage.GetCharsets(this.mailItem.MimeDocument.RootPart);
				}
				else
				{
					this.contentCharacterSets = new List<string>();
				}
				return this.contentCharacterSets;
			}
		}

		public bool UnifiedContentIsValid
		{
			get
			{
				return this.filteringResults != null;
			}
		}

		public FilteringResults GetUnifiedContentResults()
		{
			if (this.UnifiedContentIsValid)
			{
				return this.filteringResults;
			}
			this.filteringResults = UnifiedContentServiceInvoker.GetUnifiedContentResults(this.mailItem);
			return this.filteringResults;
		}

		private static string GetCharset(MimePart mimePart)
		{
			ComplexHeader complexHeader = mimePart.Headers.FindFirst(HeaderId.ContentType) as ComplexHeader;
			if (complexHeader == null)
			{
				return null;
			}
			MimeParameter mimeParameter = complexHeader["charset"];
			if (mimeParameter == null)
			{
				return null;
			}
			string text;
			if (!mimeParameter.TryGetValue(out text) || string.IsNullOrEmpty(text))
			{
				return null;
			}
			return text;
		}

		private static bool IsAttachmentPart(StreamIdentity streamIdentity)
		{
			bool flag = -1 == streamIdentity.ParentId;
			bool flag2 = streamIdentity.Properties.ContainsKey("UnifiedContent::PropertyKeys::ExtendedContent");
			bool flag3 = streamIdentity.Properties.ContainsKey("Parsing::ParsingKeys::MessageBody");
			bool flag4 = 0 == streamIdentity.ParentId;
			return !flag && !flag2 && (!flag3 || !flag4);
		}

		public IEnumerable<StreamIdentity> GetAttachmentStreamIdentities()
		{
			if (this.attachmentStreamIdentities != null)
			{
				return (IEnumerable<StreamIdentity>)this.attachmentStreamIdentities;
			}
			IEnumerable<StreamIdentity> result = from streamIdentity in this.GetUnifiedContentResults().Streams
			where MailMessage.IsAttachmentPart(streamIdentity)
			select streamIdentity;
			this.attachmentStreamIdentities = result;
			return result;
		}

		public IEnumerable<StreamIdentity> GetSupportedAttachmentStreamIdentities()
		{
			return from streamIdentity in this.GetAttachmentStreamIdentities()
			where !RuleAgentResultUtils.IsUnsupported(streamIdentity)
			select streamIdentity;
		}

		public void ResetUnifiedContent()
		{
			this.SetUnifiedContent(null);
		}

		internal void SetUnifiedContent(FilteringResults filteringResults)
		{
			this.filteringResults = filteringResults;
		}

		internal static List<string> GetCharsets(MimePart mimePart)
		{
			List<string> list = new List<string>();
			if (mimePart == null)
			{
				return list;
			}
			string charset = MailMessage.GetCharset(mimePart);
			if (!string.IsNullOrEmpty(charset))
			{
				list.Add(charset);
			}
			for (MimePart mimePart2 = mimePart.FirstChild as MimePart; mimePart2 != null; mimePart2 = (mimePart2.NextSibling as MimePart))
			{
				list.AddRange(MailMessage.GetCharsets(mimePart2));
			}
			return list;
		}

		internal static string GetAttachmentName(StreamIdentity attachmentIdentity, bool shouldAppendMsgToMailAttachmentNames)
		{
			if (shouldAppendMsgToMailAttachmentNames && !string.IsNullOrWhiteSpace(attachmentIdentity.Name) && !attachmentIdentity.Name.EndsWith(".msg", StringComparison.OrdinalIgnoreCase))
			{
				if (attachmentIdentity.Types.Any((StreamType binaryType) => binaryType == 1082130439 || binaryType == 8388610))
				{
					return attachmentIdentity.Name + ".msg";
				}
			}
			return attachmentIdentity.Name;
		}

		private List<AttachmentInfo> GetAttachmentInfos()
		{
			if (this.attachmentInfos != null)
			{
				return this.attachmentInfos;
			}
			Dictionary<int, KeyValuePair<int, AttachmentInfo>> dictionary = new Dictionary<int, KeyValuePair<int, AttachmentInfo>>();
			this.attachmentInfos = new List<AttachmentInfo>();
			bool shouldAppendMsgToMailAttachmentNames = TransportUtils.IsMsgAttachmentNameEnabled(this.mailItem);
			try
			{
				foreach (StreamIdentity streamIdentity in this.GetAttachmentStreamIdentities())
				{
					string attachmentName = MailMessage.GetAttachmentName(streamIdentity, shouldAppendMsgToMailAttachmentNames);
					dictionary.Add(streamIdentity.Id, new KeyValuePair<int, AttachmentInfo>(streamIdentity.ParentId, new AttachmentInfo(attachmentName, TransportUtils.GetFileExtension(attachmentName), (from t in streamIdentity.Types
					select t).ToArray<uint>())));
				}
			}
			catch (NotSupportedException ex)
			{
				string message = TransportRulesStrings.AttachmentReadError(string.Format("NotSupportedException encountered while getting attachment content. Most likely reason - FIPS not configured properly. Check if TextExtractionHandler is enabled in FIP-FS\\Data\\configuration.xml. {0}", ex.Message));
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, message);
				throw new TransportRulePermanentException(message);
			}
			foreach (KeyValuePair<int, KeyValuePair<int, AttachmentInfo>> keyValuePair in dictionary)
			{
				AttachmentInfo value = keyValuePair.Value.Value;
				KeyValuePair<int, AttachmentInfo> keyValuePair2;
				if (keyValuePair.Value.Key != 0 && dictionary.TryGetValue(keyValuePair.Value.Key, out keyValuePair2))
				{
					value.Parent = keyValuePair2.Value;
				}
				this.attachmentInfos.Add(value);
			}
			return this.attachmentInfos;
		}

		private string GetStringFromFirstHeader(HeaderId headerId)
		{
			Header header = this.mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(headerId);
			if (header == null)
			{
				return null;
			}
			return header.Value;
		}

		private List<string> GetAddressesFromFirstHeader(HeaderId headerId)
		{
			AddressHeader addressHeader = (AddressHeader)this.mailItem.Message.MimeDocument.RootPart.Headers.FindFirst(headerId);
			if (addressHeader == null)
			{
				return null;
			}
			List<string> result;
			using (MimeNode.Enumerator<AddressItem> enumerator = addressHeader.GetEnumerator())
			{
				enumerator.Reset();
				List<string> list = new List<string>();
				while (enumerator.MoveNext())
				{
					AddressItem addressItem = enumerator.Current;
					MimeRecipient mimeRecipient = addressItem as MimeRecipient;
					if (mimeRecipient != null)
					{
						list.Add(mimeRecipient.Email.ToString());
					}
					else
					{
						MimeGroup mimeGroup = enumerator.Current as MimeGroup;
						if (mimeGroup != null)
						{
							foreach (MimeRecipient mimeRecipient2 in mimeGroup)
							{
								list.Add(mimeRecipient2.Email.ToString());
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		private const string MsgAttachmentExtension = ".msg";

		private MailItem mailItem;

		private MailMessage.MessageHeaders headers;

		private MessageBodies body;

		private List<AttachmentInfo> attachmentInfos;

		private object attachmentStreamIdentities;

		private FilteringResults filteringResults;

		private List<string> contentCharacterSets;

		public class MessageHeaders
		{
			public MessageHeaders(MailItem mailItem)
			{
				if (mailItem != null)
				{
					this.mailItem = mailItem;
				}
			}

			public virtual List<string> this[string index]
			{
				get
				{
					List<string> list = new List<string>();
					if (index == null || this.mailItem == null)
					{
						return list;
					}
					string key;
					switch (key = index.ToLower())
					{
					case "from":
						MailMessage.MessageHeaders.AddToStringList(this.mailItem.Message.From, list);
						return list;
					case "to":
						MailMessage.MessageHeaders.AddToStringList(this.mailItem.Message.To, list);
						return list;
					case "cc":
						MailMessage.MessageHeaders.AddToStringList(this.mailItem.Message.Cc, list);
						return list;
					case "bcc":
						MailMessage.MessageHeaders.AddToStringList(this.mailItem.Message.Bcc, list);
						return list;
					case "reply-to":
						MailMessage.MessageHeaders.AddToStringList(this.mailItem.Message.ReplyTo, list);
						return list;
					case "subject":
						list.Add(this.mailItem.Message.Subject);
						return list;
					case "message-id":
						list.Add(this.mailItem.Message.MessageId);
						return list;
					}
					Header[] array = this.mailItem.Message.MimeDocument.RootPart.Headers.FindAll(index);
					foreach (Header header in array)
					{
						AddressHeader addressHeader = header as AddressHeader;
						if (addressHeader == null)
						{
							if (header.HeaderId == HeaderId.ContentType && "multipart/report".Equals(header.Value, StringComparison.OrdinalIgnoreCase) && header is ComplexHeader)
							{
								string text = Convert.ToString(header.Value);
								MimeParameter mimeParameter = (header as ComplexHeader)["report-type"];
								if (mimeParameter != null && mimeParameter.Value != null)
								{
									text = string.Format("{0};\r\n\t{1}={2}", text, "report-type", mimeParameter.Value);
								}
								list.Add(text);
							}
							else
							{
								list.Add(header.Value);
							}
						}
						else
						{
							using (MimeNode.Enumerator<AddressItem> enumerator = addressHeader.GetEnumerator())
							{
								enumerator.Reset();
								while (enumerator.MoveNext())
								{
									AddressItem addressItem = enumerator.Current;
									MimeRecipient mimeRecipient = addressItem as MimeRecipient;
									if (mimeRecipient != null)
									{
										list.Add(mimeRecipient.DisplayName);
										list.Add(mimeRecipient.Email.ToString());
									}
									else
									{
										MimeGroup mimeGroup = enumerator.Current as MimeGroup;
										if (mimeGroup != null)
										{
											foreach (MimeRecipient mimeRecipient2 in mimeGroup)
											{
												list.Add(mimeRecipient2.DisplayName);
												list.Add(mimeRecipient2.Email.ToString());
											}
										}
									}
								}
							}
						}
					}
					return list;
				}
			}

			private static void AddToStringList(EmailRecipientCollection recipients, List<string> strings)
			{
				foreach (EmailRecipient emailRecipient in recipients)
				{
					strings.Add(emailRecipient.SmtpAddress);
					strings.Add(emailRecipient.DisplayName);
				}
			}

			private static void AddToStringList(EmailRecipient recipient, List<string> strings)
			{
				if (recipient != null)
				{
					strings.Add(recipient.SmtpAddress);
					strings.Add(recipient.DisplayName);
				}
			}

			private MailItem mailItem;
		}
	}
}
