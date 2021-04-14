using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class ModerationApprovalRequestWriter : ApprovalRequestWriter
	{
		private ModerationApprovalRequestWriter(OrganizationId organizationId, InitiationMessage initiationMessage)
		{
			this.organizationId = organizationId;
			this.initiationMessage = initiationMessage;
		}

		public override bool SupportMultipleRequestsForDifferentCultures
		{
			get
			{
				return true;
			}
		}

		public override bool WriteSubjectAndBody(MessageItemApprovalRequest approvalRequest, CultureInfo cultureInfo, out CultureInfo cultureInfoWritten)
		{
			cultureInfoWritten = cultureInfo;
			ModerationApprovalRequestWriter.diag.TraceDebug<CultureInfo>((long)this.GetHashCode(), "Generating approval request. cultureInfo={0}", cultureInfo);
			DsnHumanReadableWriter defaultDsnHumanReadableWriter = DsnHumanReadableWriter.DefaultDsnHumanReadableWriter;
			this.CacheDataFromOriginalMessage();
			if (this.originalMessageStream == null)
			{
				ModerationApprovalRequestWriter.diag.TraceError<string>((long)this.GetHashCode(), "No original message to write approval request body.  Approval data: {0}", this.initiationMessage.ApprovalData);
				return false;
			}
			bool flag = !this.hasInlineAttachment && !this.isOpaqueMessage;
			if (this.hasInlineAttachment)
			{
				ModerationApprovalRequestWriter.diag.TraceDebug<string>((long)this.GetHashCode(), "Not writing preview for message with approval data: {0}, because it has inline attachment", this.initiationMessage.ApprovalData);
			}
			ApprovalInformation approvalRequestMessageInformation = defaultDsnHumanReadableWriter.GetApprovalRequestMessageInformation(this.initiationMessage.Subject, this.originalSenderDisplayName, this.originalToFormattedLine, this.originalCcFormattedLine, this.moderatedRecipients, flag, cultureInfo);
			approvalRequest.MessageItem.Subject = approvalRequestMessageInformation.Subject;
			this.WriteApprovalRequestBody(approvalRequest.MessageItem.Body, defaultDsnHumanReadableWriter, approvalRequestMessageInformation, flag);
			return true;
		}

		public override void Dispose()
		{
			if (this.originalEmailMessage != null)
			{
				this.originalEmailMessage.Dispose();
				this.originalEmailMessage = null;
			}
			if (this.originalMimeDom != null)
			{
				this.originalMimeDom.Dispose();
				this.originalMimeDom = null;
			}
			if (this.originalMessageStream != null)
			{
				this.originalMessageStream.Dispose();
				this.originalMessageStream = null;
			}
		}

		internal static ModerationApprovalRequestWriter GetInstance(OrganizationId organizationId, InitiationMessage initiationMessage)
		{
			return new ModerationApprovalRequestWriter(organizationId, initiationMessage);
		}

		private static bool CheckInputOutputCharsetMatch(IEnumerable<int> codepages, Charset inputCharset, out Charset outputCharset)
		{
			outputCharset = null;
			if (inputCharset == null)
			{
				ModerationApprovalRequestWriter.diag.TraceDebug(0L, "original message charset not valid");
				return false;
			}
			foreach (int num in codepages)
			{
				if (inputCharset.CodePage == num)
				{
					outputCharset = inputCharset;
					return true;
				}
			}
			outputCharset = Charset.UTF8;
			return true;
		}

		private static string FormatRecipients(EmailRecipientCollection recipientCollection)
		{
			if (recipientCollection.Count == 0)
			{
				return string.Empty;
			}
			int num = 0;
			foreach (EmailRecipient emailRecipient in recipientCollection)
			{
				if (!string.IsNullOrEmpty(emailRecipient.DisplayName))
				{
					num += emailRecipient.DisplayName.Length;
				}
				else if (!string.IsNullOrEmpty(emailRecipient.NativeAddress))
				{
					num += emailRecipient.NativeAddress.Length;
				}
			}
			num += "; ".Length * recipientCollection.Count;
			StringBuilder stringBuilder = new StringBuilder(num);
			for (int i = 0; i < recipientCollection.Count; i++)
			{
				EmailRecipient emailRecipient2 = recipientCollection[i];
				if (!string.IsNullOrEmpty(emailRecipient2.DisplayName))
				{
					stringBuilder.Append(emailRecipient2.DisplayName);
				}
				else if (!string.IsNullOrEmpty(emailRecipient2.NativeAddress))
				{
					stringBuilder.Append(emailRecipient2.NativeAddress);
				}
				else
				{
					ModerationApprovalRequestWriter.diag.TraceError<EmailRecipient>(0L, "Cannot get suitable value to write recipient address {0}.", emailRecipient2);
				}
				if (i < recipientCollection.Count - 1)
				{
					stringBuilder.Append("; ");
				}
			}
			return stringBuilder.ToString();
		}

		private void CacheDataFromOriginalMessage()
		{
			try
			{
				if (this.isOriginalMessageDataCached)
				{
					ModerationApprovalRequestWriter.diag.TraceDebug((long)this.GetHashCode(), "Original data cached.");
				}
				else
				{
					this.isOriginalMessageDataCached = true;
					ModerationApprovalRequestWriter.diag.TraceDebug((long)this.GetHashCode(), "Getting original message for cache.");
					this.CacheModeratedRecipients();
					this.originalMessageStream = this.GetOriginalMessageReadStream();
					if (this.originalMessageStream == null)
					{
						ModerationApprovalRequestWriter.diag.TraceError(0L, "There is no original message attached in initiation message.");
					}
					else
					{
						this.originalMimeDom = new MimeDocument();
						this.originalMimeDom.Load(this.originalMessageStream, CachingMode.Source);
						this.originalEmailMessage = EmailMessage.Create(this.originalMimeDom);
						this.originalToFormattedLine = ModerationApprovalRequestWriter.FormatRecipients(this.originalEmailMessage.To);
						this.originalCcFormattedLine = ModerationApprovalRequestWriter.FormatRecipients(this.originalEmailMessage.Cc);
						EmailRecipient sender = this.originalEmailMessage.Sender;
						if (sender != null && !string.IsNullOrEmpty(sender.DisplayName))
						{
							if (!string.IsNullOrEmpty(sender.SmtpAddress) && this.organizationId != null && !IsInternalResolver.IsInternal(new RoutingAddress(sender.SmtpAddress), this.organizationId))
							{
								this.originalSenderDisplayName = sender.DisplayName + "(" + sender.SmtpAddress + ")";
							}
							else
							{
								this.originalSenderDisplayName = sender.DisplayName;
							}
						}
						else
						{
							this.originalSenderDisplayName = (string)this.initiationMessage.Requestor;
						}
						this.originalMesssageBodyFormat = this.originalEmailMessage.Body.BodyFormat;
						string charsetName = this.originalEmailMessage.Body.CharsetName;
						Charset charset;
						if (!string.IsNullOrEmpty(charsetName) && Charset.TryGetCharset(charsetName, out charset) && charset.IsAvailable)
						{
							ModerationApprovalRequestWriter.diag.TraceDebug<string>((long)this.GetHashCode(), "original message body has charset {0}", charsetName);
							this.originalMessageBodyCharset = charset;
						}
						else
						{
							ModerationApprovalRequestWriter.diag.TraceDebug<string>((long)this.GetHashCode(), "original message charset not valid {0}", charsetName);
						}
						this.hasInlineAttachment = false;
						if (this.originalMesssageBodyFormat == Microsoft.Exchange.Data.Transport.Email.BodyFormat.Rtf)
						{
							if (this.originalEmailMessage.Attachments != null && this.originalEmailMessage.Attachments.Count > 0)
							{
								this.hasInlineAttachment = true;
							}
						}
						else if (this.originalMesssageBodyFormat != Microsoft.Exchange.Data.Transport.Email.BodyFormat.Text && this.originalMesssageBodyFormat == Microsoft.Exchange.Data.Transport.Email.BodyFormat.Html && this.originalEmailMessage.Attachments != null && this.originalEmailMessage.Attachments.Count > 0)
						{
							foreach (Attachment attachment in this.originalEmailMessage.Attachments)
							{
								if (attachment.AttachHidden || attachment.AttachmentType == AttachmentType.Inline)
								{
									this.hasInlineAttachment = true;
									break;
								}
							}
						}
						this.isOpaqueMessage = this.originalEmailMessage.IsOpaqueMessage;
					}
				}
			}
			catch (ExchangeDataException arg)
			{
				ModerationApprovalRequestWriter.diag.TraceError<ExchangeDataException>((long)this.GetHashCode(), "Cannot cache all data required for moderation approval request {0}", arg);
			}
		}

		private Stream GetOriginalMessageReadStream()
		{
			foreach (Attachment attachment in this.initiationMessage.EmailMessage.Attachments)
			{
				if ("OriginalMessage".Equals(attachment.FileName, StringComparison.OrdinalIgnoreCase))
				{
					return attachment.GetContentReadStream();
				}
			}
			return null;
		}

		private void CacheModeratedRecipients()
		{
			if (this.moderatedRecipients != null)
			{
				throw new InvalidOperationException("Moderated recipients cached more than once");
			}
			EmailRecipientCollection cc = this.initiationMessage.EmailMessage.Cc;
			List<string> list = new List<string>(cc.Count);
			foreach (EmailRecipient emailRecipient in cc)
			{
				string text = emailRecipient.DisplayName;
				if (string.IsNullOrEmpty(text))
				{
					text = emailRecipient.SmtpAddress;
					if (string.IsNullOrEmpty(text))
					{
						ModerationApprovalRequestWriter.diag.TraceDebug(0L, "skipping moderated recipient without address or display name");
						continue;
					}
				}
				list.Add(text);
			}
			this.moderatedRecipients = list.AsReadOnly();
		}

		private void WriteApprovalRequestBody(Body destiniationBody, DsnHumanReadableWriter writer, ApprovalInformation info, bool writePreview)
		{
			if (writePreview)
			{
				Charset outputCharset;
				bool flag = !ModerationApprovalRequestWriter.CheckInputOutputCharsetMatch(info.Codepages, this.originalMessageBodyCharset, out outputCharset);
				if (!flag && this.TryWriteApprovalRequestBodyWithPreview(destiniationBody, writer, info, outputCharset))
				{
					return;
				}
				ModerationApprovalRequestWriter.diag.TraceError<bool, string>((long)this.GetHashCode(), "Failed to write preview for message.  Falling back to no preview. Charset mismatch:{0}. Approval data:{1}.", flag, this.initiationMessage.ApprovalData);
				info = writer.GetApprovalRequestMessageInformation(this.initiationMessage.Subject, this.originalSenderDisplayName, this.originalToFormattedLine, this.originalCcFormattedLine, this.moderatedRecipients, false, info.Culture);
			}
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml, info.MessageCharset.Name);
			using (Stream stream = destiniationBody.OpenWriteStream(configuration))
			{
				writer.WriteHtmlModerationBody(stream, info);
			}
		}

		private bool TryWriteApprovalRequestBodyWithPreview(Body destiniationBody, DsnHumanReadableWriter approvalRequestWriter, ApprovalInformation info, Charset outputCharset)
		{
			if (this.originalEmailMessage == null)
			{
				return false;
			}
			TextConverter textConverter;
			if (this.originalMesssageBodyFormat == Microsoft.Exchange.Data.Transport.Email.BodyFormat.Rtf)
			{
				textConverter = new RtfToHtml
				{
					Header = approvalRequestWriter.GetHtmlModerationBody(info),
					HeaderFooterFormat = HeaderFooterFormat.Html,
					OutputEncoding = outputCharset.GetEncoding()
				};
			}
			else if (this.originalMesssageBodyFormat == Microsoft.Exchange.Data.Transport.Email.BodyFormat.Text)
			{
				textConverter = new TextToHtml
				{
					Header = approvalRequestWriter.GetHtmlModerationBody(info),
					HeaderFooterFormat = HeaderFooterFormat.Html,
					InputEncoding = this.originalMessageBodyCharset.GetEncoding(),
					OutputEncoding = outputCharset.GetEncoding()
				};
			}
			else
			{
				if (this.originalMesssageBodyFormat != Microsoft.Exchange.Data.Transport.Email.BodyFormat.Html)
				{
					return false;
				}
				textConverter = new HtmlToHtml
				{
					Header = approvalRequestWriter.GetHtmlModerationBody(info),
					HeaderFooterFormat = HeaderFooterFormat.Html,
					InputEncoding = this.originalMessageBodyCharset.GetEncoding(),
					OutputEncoding = outputCharset.GetEncoding()
				};
			}
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml, outputCharset);
			using (Stream stream = destiniationBody.OpenWriteStream(configuration))
			{
				using (Stream contentReadStream = this.originalEmailMessage.Body.GetContentReadStream())
				{
					try
					{
						textConverter.Convert(contentReadStream, stream);
					}
					catch (ExchangeDataException arg)
					{
						ModerationApprovalRequestWriter.diag.TraceDebug<ExchangeDataException>(0L, "Approval request with inline preview failed {0}", arg);
						return false;
					}
				}
			}
			return true;
		}

		private const string RecipientSeparator = "; ";

		private static readonly Trace diag = ExTraceGlobals.ModeratedTransportTracer;

		private InitiationMessage initiationMessage;

		private IList<string> moderatedRecipients;

		private string originalSenderDisplayName;

		private string originalToFormattedLine;

		private string originalCcFormattedLine;

		private Stream originalMessageStream;

		private MimeDocument originalMimeDom;

		private EmailMessage originalEmailMessage;

		private Charset originalMessageBodyCharset;

		private Microsoft.Exchange.Data.Transport.Email.BodyFormat originalMesssageBodyFormat;

		private bool isOriginalMessageDataCached;

		private bool hasInlineAttachment;

		private bool isOpaqueMessage;

		private OrganizationId organizationId;
	}
}
