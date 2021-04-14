using System;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class MessageItemApprovalRequest : IDisposable
	{
		protected MessageItemApprovalRequest(MessageItem messageItem, OrganizationId organizationId)
		{
			this.messageItem = messageItem;
			this.organizationId = organizationId;
			this.creationTime = DateTime.UtcNow;
		}

		public RoutingAddress ApprovalRequestor
		{
			set
			{
				if (value.IsValid && value != RoutingAddress.NullReversePath)
				{
					this.approvalRequestor = new RoutingAddress?(value);
					this.messageItem[MessageItemSchema.ApprovalRequestor] = (string)value;
				}
			}
		}

		internal MessageItem MessageItem
		{
			get
			{
				return this.messageItem;
			}
		}

		private byte[] Buffer
		{
			get
			{
				if (this.buffer == null)
				{
					this.buffer = new byte[4096];
				}
				return this.buffer;
			}
		}

		public static MessageItemApprovalRequest Create(MbxTransportMailItem mbxTransportMailItem)
		{
			MessageItem messageItem = MessageItem.CreateInMemory(MessageItemApprovalRequest.PrefetchProperties);
			return new MessageItemApprovalRequest(messageItem, mbxTransportMailItem.OrganizationId);
		}

		public void SetSender(RoutingAddress address)
		{
			Participant sender = new Participant(string.Empty, (string)address, "smtp");
			this.messageItem.Sender = sender;
		}

		public void AddRecipient(RoutingAddress address, bool sendToThisRecipient)
		{
			Participant participant = new Participant(string.Empty, (string)address, "smtp");
			Recipient recipient = this.messageItem.Recipients.Add(participant, RecipientItemType.To);
			recipient[ItemSchema.Responsibility] = sendToThisRecipient;
		}

		public void AddVotingOption(string votingOption, bool allowComments)
		{
			VotingInfo.OptionData data = default(VotingInfo.OptionData);
			data.DisplayName = votingOption;
			data.SendPrompt = (allowComments ? VotingInfo.SendPrompt.VotingOption : VotingInfo.SendPrompt.Send);
			this.messageItem.VotingInfo.AddOption(data);
		}

		public void AddAttachment(Attachment attachment, IRecipientSession adRecipientSession)
		{
			bool flag = false;
			MimePart mimePart = attachment.MimePart;
			Header header = null;
			if (mimePart != null)
			{
				header = mimePart.Headers.FindFirst("X-MS-Exchange-Organization-Approval-AttachToApprovalRequest");
			}
			string text;
			if (header != null && header.TryGetValue(out text))
			{
				if (text.Equals("Never"))
				{
					return;
				}
				if (text.Equals("AsMessage"))
				{
					flag = true;
				}
			}
			if (flag)
			{
				using (Stream contentReadStream = attachment.GetContentReadStream())
				{
					using (ItemAttachment itemAttachment = (ItemAttachment)this.messageItem.AttachmentCollection.Create(AttachmentType.EmbeddedMessage))
					{
						using (Item item = itemAttachment.GetItem())
						{
							ItemConversion.ConvertAnyMimeToItem(item, contentReadStream, new InboundConversionOptions(Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName)
							{
								UserADSession = adRecipientSession
							});
							item[MessageItemSchema.Flags] = MessageFlags.None;
							item.Save(SaveMode.NoConflictResolution);
							string valueOrDefault = item.GetValueOrDefault<string>(ItemSchema.Subject);
							if (!string.IsNullOrEmpty(valueOrDefault))
							{
								itemAttachment[AttachmentSchema.DisplayName] = valueOrDefault;
							}
							itemAttachment.Save();
						}
					}
					return;
				}
			}
			using (StreamAttachment streamAttachment = (StreamAttachment)this.messageItem.AttachmentCollection.Create(AttachmentType.Stream))
			{
				streamAttachment.FileName = attachment.FileName;
				using (Stream contentStream = streamAttachment.GetContentStream())
				{
					using (Stream contentReadStream2 = attachment.GetContentReadStream())
					{
						ApprovalProcessor.CopyStream(contentReadStream2, contentStream, this.Buffer);
					}
					contentStream.Flush();
				}
				streamAttachment.Save();
			}
		}

		public void SetBody(Body body)
		{
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(this.GetXsoBodyFormat(body), body.CharsetName);
			Body body2 = this.messageItem.Body;
			using (Stream stream = body2.OpenWriteStream(configuration))
			{
				using (Stream contentReadStream = body.GetContentReadStream())
				{
					ApprovalProcessor.CopyStream(contentReadStream, stream, this.Buffer);
				}
				stream.Flush();
			}
		}

		public void SetBody(string body)
		{
			using (TextWriter textWriter = this.messageItem.Body.OpenTextWriter(Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml))
			{
				textWriter.Write(body);
			}
		}

		public void Send(string messageId, byte[] corelationBlob, MbxTransportMailItem mbxTransportMailItem)
		{
			this.messageItem.VotingInfo.MessageCorrelationBlob = corelationBlob;
			this.messageItem.ClassName = "IPM.Note.Microsoft.Approval.Request";
			this.messageItem.InternetMessageId = messageId;
			this.messageItem.Save(SaveMode.NoConflictResolution);
			using (MemorySubmissionItem memorySubmissionItem = new MemorySubmissionItem(this.messageItem, this.organizationId))
			{
				memorySubmissionItem.Submit(MessageTrackingSource.APPROVAL, new MemorySubmissionItem.OnConvertedToTransportMailItemDelegate(this.TransportMailItemHandler), mbxTransportMailItem);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual bool TransportMailItemHandler(TransportMailItem mailItem, bool isValid)
		{
			if (!isValid)
			{
				return false;
			}
			if (this.approvalRequestor != null)
			{
				mailItem.Message.Sender = new EmailRecipient(null, this.messageItem.Sender.EmailAddress);
				mailItem.Message.From = new EmailRecipient(null, (string)this.approvalRequestor.Value);
				mailItem.Message.ReplyTo.Add(new EmailRecipient(null, this.messageItem.Sender.EmailAddress));
			}
			else
			{
				mailItem.Message.From = new EmailRecipient(null, this.messageItem.Sender.EmailAddress);
				mailItem.Message.ReplyTo.Add(new EmailRecipient(null, this.messageItem.Sender.EmailAddress));
			}
			mailItem.RootPart.Headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-Mapi-Admin-Submission", string.Empty));
			mailItem.RootPart.Headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-OriginalArrivalTime", Util.FormatOrganizationalMessageArrivalTime(this.creationTime)));
			Header newChild = Header.Create("X-MS-Exchange-Forest-RulesExecuted");
			mailItem.RootPart.Headers.AppendChild(newChild);
			return true;
		}

		private void Dispose(bool disposing)
		{
			if (disposing && this.messageItem != null)
			{
				this.messageItem.Dispose();
				this.messageItem = null;
			}
		}

		private Microsoft.Exchange.Data.Storage.BodyFormat GetXsoBodyFormat(Body body)
		{
			switch (body.BodyFormat)
			{
			case Microsoft.Exchange.Data.Transport.Email.BodyFormat.Text:
				return Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain;
			case Microsoft.Exchange.Data.Transport.Email.BodyFormat.Html:
				return Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml;
			}
			return Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain;
		}

		private const string Smtp = "smtp";

		private const int BufferSize = 4096;

		internal static readonly StorePropertyDefinition[] PrefetchProperties = StoreObjectSchema.ContentConversionProperties;

		private readonly DateTime creationTime;

		private MessageItem messageItem;

		private RoutingAddress? approvalRequestor;

		private byte[] buffer;

		private OrganizationId organizationId;
	}
}
