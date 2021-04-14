using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Pickup;
using Microsoft.Exchange.Transport.RightsManagement;

namespace Microsoft.Exchange.Transport
{
	internal static class ModerationHelper
	{
		internal static EmailMessage EncapsulateOriginalMessage(IReadOnlyMailItem transportMailItem, ICollection<MailRecipient> recipients, string fromAddress, string toAddress, Trace tracer, Action<Exception> onReEncryptionError, out string domain)
		{
			EmailMessage message = transportMailItem.Message;
			HeaderList headers = message.RootPart.Headers;
			EmailMessage emailMessage = EmailMessage.Create(Microsoft.Exchange.Data.Transport.Email.BodyFormat.Html, false, message.Body.CharsetName);
			emailMessage.From = new EmailRecipient(string.Empty, fromAddress);
			emailMessage.To.Add(new EmailRecipient(string.Empty, toAddress));
			emailMessage.Subject = message.Subject;
			emailMessage.Date = message.Date;
			emailMessage.Importance = message.Importance;
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
			if (Components.Configuration.TryGetAcceptedDomainTable(transportMailItem.OrganizationId, out perTenantAcceptedDomainTable) && perTenantAcceptedDomainTable.AcceptedDomainTable.DefaultDomain != null)
			{
				domain = perTenantAcceptedDomainTable.AcceptedDomainTable.DefaultDomainName;
			}
			else
			{
				domain = Components.Configuration.LocalServer.TransportServer.Fqdn;
			}
			emailMessage.MessageId = string.Format("<{0}@{1}>", Guid.NewGuid(), domain);
			Attachment attachment = emailMessage.Attachments.Add("ReplayXHeaders");
			using (Stream contentWriteStream = attachment.GetContentWriteStream())
			{
				if (!ExportStream.TryWriteReplayXHeaders(contentWriteStream, transportMailItem, recipients, false))
				{
					throw new InvalidOperationException("recipient cannot be written");
				}
				contentWriteStream.Flush();
			}
			Attachment attachment2 = emailMessage.Attachments.Add("FireWalledHeaders");
			using (Stream contentWriteStream2 = attachment2.GetContentWriteStream())
			{
				headers.WriteTo(contentWriteStream2, null, new ModerationHelper.FirewallHeaderOnlyFilter(~RestrictedHeaderSet.MTA));
				contentWriteStream2.Flush();
			}
			Attachment attachment3 = emailMessage.Attachments.Add("OriginalMessage");
			bool flag = false;
			EmailMessage emailMessage2 = null;
			try
			{
				if (E4eHelper.IsPipelineDecrypted(transportMailItem))
				{
					emailMessage2 = ModerationHelper.GetOriginalMessageE4e(transportMailItem, out flag, tracer, onReEncryptionError);
				}
				else
				{
					emailMessage2 = ModerationHelper.GetOriginalMessage(transportMailItem, out flag, tracer, onReEncryptionError);
				}
				if (attachment3 != null)
				{
					using (Stream contentWriteStream3 = attachment3.GetContentWriteStream())
					{
						emailMessage2.MimeDocument.RootPart.WriteTo(contentWriteStream3, null, new HeaderFirewall.OutputFilter(~RestrictedHeaderSet.MTA, true));
						contentWriteStream3.Flush();
					}
				}
			}
			finally
			{
				if (flag && emailMessage2 != null)
				{
					emailMessage2.Dispose();
				}
			}
			return emailMessage;
		}

		internal static bool RestoreOriginalMessage(MessageItem messageItem, TransportMailItem transportMailItem, SystemProbeTrace tracer, Guid activityId)
		{
			transportMailItem.Recipients.Clear();
			transportMailItem.ExtendedProperties.Clear();
			ReplayXHeaderProcessor replayXHeaderProcessor = new ReplayXHeaderProcessor(transportMailItem);
			AttachmentCollection attachmentCollection = messageItem.AttachmentCollection;
			Stream stream = null;
			Attachment attachment = null;
			try
			{
				if (!ModerationHelper.TryOpenAttachment(attachmentCollection, "ReplayXHeaders", out attachment, out stream))
				{
					tracer.TraceFail<string>(activityId, 0L, "Cannot get xheaders attachment from {0}", messageItem.InternetMessageId);
					return false;
				}
				using (MimeReader mimeReader = new MimeReader(stream))
				{
					mimeReader.ReadFirstChildPart();
					HeaderList headerList = HeaderList.ReadFrom(mimeReader);
					ReplayFileMailer.XHeaderType xheaderType;
					string text;
					replayXHeaderProcessor.ProcessXHeaders(headerList, out xheaderType, out text, true);
				}
				if (transportMailItem.From == RoutingAddress.Empty)
				{
					string valueOrDefault = messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalRequestor);
					if (!string.IsNullOrEmpty(valueOrDefault))
					{
						transportMailItem.From = new RoutingAddress(valueOrDefault);
					}
				}
				if (string.IsNullOrEmpty(transportMailItem.ReceiveConnectorName))
				{
					transportMailItem.ReceiveConnectorName = "Moderation";
				}
				if (string.IsNullOrEmpty(transportMailItem.HeloDomain))
				{
					transportMailItem.HeloDomain = Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName;
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
					stream = null;
				}
				if (attachment != null)
				{
					attachment.Dispose();
					attachment = null;
				}
			}
			using (Stream stream2 = transportMailItem.OpenMimeWriteStream())
			{
				try
				{
					if (!ModerationHelper.TryOpenAttachment(attachmentCollection, "OriginalMessage", out attachment, out stream))
					{
						tracer.TraceFail<string>(activityId, 0L, "Cannot get message attachment from {0}", messageItem.InternetMessageId);
						return false;
					}
					byte[] buffer = new byte[8192];
					ApprovalProcessor.CopyStream(stream, stream2, buffer);
					stream2.Flush();
				}
				finally
				{
					if (stream != null)
					{
						stream.Dispose();
						stream = null;
					}
					if (attachment != null)
					{
						attachment.Dispose();
						attachment = null;
					}
				}
			}
			try
			{
				if (ModerationHelper.TryOpenAttachment(attachmentCollection, "FireWalledHeaders", out attachment, out stream))
				{
					using (MimeReader mimeReader2 = new MimeReader(stream))
					{
						mimeReader2.ReadFirstChildPart();
						HeaderList headerList2 = HeaderList.ReadFrom(mimeReader2);
						foreach (Header header in headerList2)
						{
							header.RemoveFromParent();
							transportMailItem.RootPart.Headers.AppendChild(header);
						}
						goto IL_22A;
					}
					goto IL_211;
					IL_22A:
					goto IL_243;
				}
				IL_211:
				tracer.TraceFail<string>(activityId, 0L, "Cannot get firewalled header attachment from {0}", messageItem.InternetMessageId);
				return false;
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
					stream = null;
				}
				if (attachment != null)
				{
					attachment.Dispose();
					attachment = null;
				}
			}
			IL_243:
			transportMailItem.PerfCounterAttribution = "MAPISubmit";
			transportMailItem.Directionality = MailDirectionality.Originating;
			MultiTenantTransport.UpdateOrganizationScope(transportMailItem);
			return true;
		}

		private static EmailMessage GetOriginalMessage(IReadOnlyMailItem mailItem, out bool needDispose, Trace tracer, Action<Exception> onReEncryptionError)
		{
			string text = null;
			needDispose = false;
			mailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.RightsManagement.TransportDecryptionPL", out text);
			if (string.IsNullOrEmpty(text))
			{
				return mailItem.Message;
			}
			string text2 = null;
			mailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.RightsManagement.TransportDecryptionUL", out text2);
			if (string.IsNullOrEmpty(text2))
			{
				tracer.TraceError<string>(0L, "UseLicense absent from {0}, NDR the message", mailItem.Message.MessageId);
				onReEncryptionError(null);
			}
			string text3 = null;
			mailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.RightsManagement.TransportDecryptionLicenseUri", out text3);
			if (string.IsNullOrEmpty(text3))
			{
				tracer.TraceError<string>(0L, "LicenseUri absent from {0}, NDR the message", mailItem.Message.MessageId);
				onReEncryptionError(null);
			}
			Uri licenseUri = null;
			if (!Uri.TryCreate(text3, UriKind.Absolute, out licenseUri))
			{
				tracer.TraceError<string>(0L, "LicenseUri is corrupted for {0}, NDR the message", mailItem.Message.MessageId);
				onReEncryptionError(null);
			}
			EmailMessage result2;
			using (RmsEncryptor rmsEncryptor = new RmsEncryptor(mailItem.OrganizationId, mailItem, text, text2, licenseUri, null))
			{
				IAsyncResult result = rmsEncryptor.BeginEncrypt(null, null);
				AsyncOperationResult<EmailMessage> asyncOperationResult = rmsEncryptor.EndEncrypt(result);
				if (!asyncOperationResult.IsSucceeded)
				{
					tracer.TraceError<string, Exception>(0L, "Re-encryption failed for message {0} because of {1}, NDR the message", mailItem.Message.MessageId, asyncOperationResult.Exception);
					onReEncryptionError(asyncOperationResult.Exception);
					result2 = null;
				}
				else
				{
					tracer.TraceDebug<string>(0L, "Successfully Re-encrypted message {0}", mailItem.Message.MessageId);
					needDispose = true;
					result2 = asyncOperationResult.Data;
				}
			}
			return result2;
		}

		private static EmailMessage GetOriginalMessageE4e(IReadOnlyMailItem mailItem, out bool needDispose, Trace tracer, Action<Exception> onReEncryptionError)
		{
			Exception obj;
			EmailMessage originalMessage = E4eEncryptionHelper.GetOriginalMessage(mailItem, tracer, 0L, out needDispose, out obj);
			if (originalMessage == null)
			{
				onReEncryptionError(obj);
			}
			return originalMessage;
		}

		private static bool TryOpenAttachment(AttachmentCollection attachmentCollection, string fileName, out Attachment targetAttachment, out Stream stream)
		{
			targetAttachment = null;
			stream = null;
			foreach (AttachmentHandle handle in attachmentCollection)
			{
				Attachment attachment = null;
				try
				{
					attachment = attachmentCollection.Open(handle);
					if (fileName.Equals(attachment.FileName, StringComparison.OrdinalIgnoreCase))
					{
						StreamAttachment streamAttachment = attachment as StreamAttachment;
						if (streamAttachment != null)
						{
							targetAttachment = attachment;
							attachment = null;
							stream = streamAttachment.GetContentStream(PropertyOpenMode.ReadOnly);
							return true;
						}
					}
				}
				finally
				{
					if (attachment != null)
					{
						attachment.Dispose();
						attachment = null;
					}
				}
			}
			return false;
		}

		private const string ModerationDefaultReceiveConnectorName = "Moderation";

		private class FirewallHeaderOnlyFilter : MimeOutputFilter
		{
			public FirewallHeaderOnlyFilter(RestrictedHeaderSet wanted)
			{
				this.firewalledSet = wanted;
			}

			public override bool FilterPart(MimePart part, Stream stream)
			{
				return false;
			}

			public override bool FilterHeaderList(HeaderList headerList, Stream stream)
			{
				return false;
			}

			public override bool FilterHeader(Header header, Stream stream)
			{
				return !HeaderFirewall.IsHeaderBlocked(header, this.firewalledSet);
			}

			public override bool FilterPartBody(MimePart part, Stream stream)
			{
				return false;
			}

			public override void ClosePart(MimePart part, Stream stream)
			{
			}

			private RestrictedHeaderSet firewalledSet;
		}
	}
}
