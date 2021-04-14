using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal static class MessageConverter
	{
		public static Stream GenerateRightsProtectedStream(IReadOnlyMailItem mailItem, Encoding encoding, DisposableTenantLicensePair tenantLicenses, string publishingLicense, string useLicense)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (tenantLicenses == null)
			{
				throw new ArgumentNullException("tenantLicenses");
			}
			if (string.IsNullOrEmpty(publishingLicense))
			{
				throw new ArgumentNullException("publishingLicense");
			}
			if (string.IsNullOrEmpty(useLicense))
			{
				throw new ArgumentNullException("useLicense");
			}
			SafeRightsManagementHandle safeRightsManagementHandle = null;
			SafeRightsManagementHandle safeRightsManagementHandle2 = null;
			Stream result;
			try
			{
				RmsClientManager.BindUseLicenseForEncryption(tenantLicenses.EnablingPrincipalRac, useLicense, publishingLicense, out safeRightsManagementHandle, out safeRightsManagementHandle2);
				result = MessageConverter.InternalGenerateRightsProtectedStream(mailItem, encoding, publishingLicense, safeRightsManagementHandle, safeRightsManagementHandle2);
			}
			finally
			{
				if (safeRightsManagementHandle != null)
				{
					safeRightsManagementHandle.Close();
					safeRightsManagementHandle = null;
				}
				if (safeRightsManagementHandle2 != null)
				{
					safeRightsManagementHandle2.Close();
					safeRightsManagementHandle2 = null;
				}
			}
			return result;
		}

		public static Stream GenerateRightsProtectedStream(IReadOnlyMailItem mailItem, RmsTemplate template, ReadOnlyCollection<string> rcptAddresses, Encoding encoding, DisposableTenantLicensePair tenantLicenses)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (template == null)
			{
				throw new ArgumentNullException("template");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (tenantLicenses == null)
			{
				throw new ArgumentNullException("tenantLicenses");
			}
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string, Guid>(0L, "Trying to create the protected message using template {0} (ID={1})", template.Name, template.Id);
			SafeRightsManagementHandle safeRightsManagementHandle = null;
			SafeRightsManagementHandle safeRightsManagementHandle2 = null;
			Stream result;
			try
			{
				ExTraceGlobals.RightsManagementTracer.TraceDebug<string>(0L, "Creating publish license for message {0}", mailItem.Message.MessageId);
				string useLicense;
				string contentId;
				string contentIdType;
				string issuanceLicense = template.CreatePublishLicense((mailItem.Message.Sender != null) ? mailItem.Message.Sender.SmtpAddress : string.Empty, (mailItem.Message.From != null) ? mailItem.Message.From.SmtpAddress : string.Empty, rcptAddresses, null, tenantLicenses, RmsClientManager.EnvironmentHandle, out useLicense, out contentId, out contentIdType);
				RmsClientManager.BindUseLicenseForEncryption(tenantLicenses.EnablingPrincipalRac, useLicense, contentId, contentIdType, out safeRightsManagementHandle, out safeRightsManagementHandle2);
				result = MessageConverter.InternalGenerateRightsProtectedStream(mailItem, encoding, issuanceLicense, safeRightsManagementHandle, safeRightsManagementHandle2);
			}
			finally
			{
				if (safeRightsManagementHandle != null)
				{
					safeRightsManagementHandle.Close();
					safeRightsManagementHandle = null;
				}
				if (safeRightsManagementHandle2 != null)
				{
					safeRightsManagementHandle2.Close();
					safeRightsManagementHandle2 = null;
				}
			}
			return result;
		}

		private static Stream InternalGenerateRightsProtectedStream(IReadOnlyMailItem mailItem, Encoding encoding, string issuanceLicense, SafeRightsManagementHandle encryptorHandle, SafeRightsManagementHandle decryptorHandle)
		{
			EmailMessage message = mailItem.Message;
			Body body = message.Body;
			Stream stream = null;
			Stream stream2 = null;
			Stream stream3 = null;
			AttachmentCollection attachments = message.Attachments;
			Stream[] array = (attachments.Count == 0) ? null : new Stream[attachments.Count];
			string messageId = message.MessageId;
			MessageItem messageItem = null;
			Stream stream4 = null;
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>(0L, "Get body and attachments stream for message {0}", messageId);
			Stream result;
			try
			{
				Microsoft.Exchange.Data.Transport.Email.BodyFormat bodyFormat = body.BodyFormat;
				if (bodyFormat != Microsoft.Exchange.Data.Transport.Email.BodyFormat.Html && !body.TryGetContentReadStream(out stream3))
				{
					ExTraceGlobals.RightsManagementTracer.TraceError<string>(0L, "TryCreateProtectedMessage failed <== Body.TryGetContentReadStream failed (unsupported encoding) for message {0}", messageId);
					throw new MessageConversionException(Strings.BodyReadFailed, false);
				}
				if (!body.TryGetContentReadStream(out stream2))
				{
					ExTraceGlobals.RightsManagementTracer.TraceError<string>(0L, "TryCreateProtectedMessage failed <== Body.TryGetContentReadStream failed (unsupported encoding) for message {0}", messageId);
					throw new MessageConversionException(Strings.BodyReadFailed, false);
				}
				Microsoft.Exchange.Security.RightsManagement.BodyFormat bodyFormat2;
				switch (bodyFormat)
				{
				case Microsoft.Exchange.Data.Transport.Email.BodyFormat.Text:
				{
					Encoding encoding2 = Encoding.Unicode;
					Charset charset;
					if (!EmailMessageHelpers.TryGetTnefBinaryCharset(message, out charset))
					{
						encoding2 = encoding;
					}
					TextToHtml textToHtml = new TextToHtml();
					textToHtml.InputEncoding = encoding2;
					textToHtml.OutputEncoding = MessageConverter.HTMLEncoding;
					stream = new ConverterStream(stream3, textToHtml, ConverterStreamAccess.Read);
					if (encoding2 != Encoding.Unicode)
					{
						ExTraceGlobals.RightsManagementTracer.TraceDebug<string, int>(0L, "Re-encoding plain text message {0} from {1} to Unicode.", messageId, encoding2.CodePage);
						TextToText textToText = new TextToText();
						textToText.InputEncoding = encoding2;
						textToText.OutputEncoding = Encoding.Unicode;
						stream2 = new ConverterStream(stream2, textToText, ConverterStreamAccess.Read);
					}
					bodyFormat2 = Microsoft.Exchange.Security.RightsManagement.BodyFormat.PlainText;
					break;
				}
				case Microsoft.Exchange.Data.Transport.Email.BodyFormat.Rtf:
				{
					RtfToHtml converter = new RtfToHtml();
					stream = new ConverterStream(stream3, converter, ConverterStreamAccess.Read);
					stream2 = new ConverterStream(stream2, new RtfToRtfCompressed(), ConverterStreamAccess.Read);
					bodyFormat2 = Microsoft.Exchange.Security.RightsManagement.BodyFormat.Rtf;
					break;
				}
				case Microsoft.Exchange.Data.Transport.Email.BodyFormat.Html:
					stream = null;
					bodyFormat2 = Microsoft.Exchange.Security.RightsManagement.BodyFormat.Html;
					break;
				default:
					if (stream2.Length > 0L)
					{
						ExTraceGlobals.RightsManagementTracer.TraceError<Microsoft.Exchange.Data.Transport.Email.BodyFormat, string>(0L, "TryCreateProtectedMessage failed <== UnsupportedBodyType {0} for message {1}", bodyFormat, messageId);
						throw new MessageConversionException(Strings.BodyFormatUnsupported, false);
					}
					bodyFormat2 = Microsoft.Exchange.Security.RightsManagement.BodyFormat.Html;
					stream = null;
					break;
				}
				DrmEmailMessage drmEmailMessage = new DrmEmailMessage(stream2, stream, bodyFormat2, encoding.CodePage);
				if (attachments.Count > 0)
				{
					int num = -1;
					for (int i = 0; i < attachments.Count; i++)
					{
						Attachment attachment = attachments[i];
						if (attachment.EmbeddedMessage != null)
						{
							array[i] = MessageConverter.SaveToMsgStorage(mailItem, ref num, ref messageItem);
						}
						else if (!attachment.TryGetContentReadStream(out array[i]))
						{
							ExTraceGlobals.RightsManagementTracer.TraceError<int, string>(0L, "TryCreateProtectedMessage failed <== Attachment[{0}].TryGetContentReadStream failed (unsupported encoding). Message {1}", i, messageId);
							throw new MessageConversionException(Strings.AttachmentReadFailed, false);
						}
						Microsoft.Exchange.Security.RightsManagement.AttachmentType attachmentType;
						if (attachment.IsOleAttachment)
						{
							attachmentType = Microsoft.Exchange.Security.RightsManagement.AttachmentType.OleObject;
						}
						else
						{
							attachmentType = ((attachment.EmbeddedMessage != null) ? Microsoft.Exchange.Security.RightsManagement.AttachmentType.EmbeddedMessage : Microsoft.Exchange.Security.RightsManagement.AttachmentType.ByValue);
						}
						int renderingPosition = EmailMessageHelpers.GetRenderingPosition(attachment);
						string attachContentID = EmailMessageHelpers.GetAttachContentID(attachment);
						string attachContentLocation = EmailMessageHelpers.GetAttachContentLocation(attachment);
						byte[] attachRendering = EmailMessageHelpers.GetAttachRendering(attachment);
						int attachmentFlags = EmailMessageHelpers.GetAttachmentFlags(attachment);
						bool attachHidden = EmailMessageHelpers.GetAttachHidden(attachment);
						DrmEmailAttachment item = new DrmEmailAttachment(attachmentType, array[i], (uint)renderingPosition, attachContentID ?? string.Empty, attachContentLocation ?? string.Empty, attachRendering, attachment.FileName, attachment.FileName, attachmentFlags, attachHidden);
						drmEmailMessage.Attachments.Add(item);
					}
				}
				stream4 = Streams.CreateTemporaryStorageStream();
				using (DrmEmailMessageContainer drmEmailMessageContainer = new DrmEmailMessageContainer(issuanceLicense, drmEmailMessage))
				{
					DrmEmailMessageBinding messageBinding = new DrmEmailMessageBinding(issuanceLicense, encryptorHandle, decryptorHandle);
					drmEmailMessageContainer.Save(stream4, messageBinding);
				}
				result = stream4;
			}
			finally
			{
				if (messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
				if (stream2 != null)
				{
					stream2.Close();
				}
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j] != null)
						{
							array[j].Close();
						}
					}
				}
				if (stream != null)
				{
					stream.Close();
				}
				if (stream3 != null)
				{
					stream3.Close();
				}
			}
			return result;
		}

		private static Stream SaveToMsgStorage(IReadOnlyMailItem mailItem, ref int lastXsoItemAttachmentIndex, ref MessageItem messageItem)
		{
			bool flag = false;
			bool flag2 = false;
			Stream stream = null;
			try
			{
				if (messageItem == null)
				{
					messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties);
					flag2 = true;
					InboundConversionOptions inboundConversionOptions = new InboundConversionOptions(Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomain.DomainName.Domain);
					inboundConversionOptions.RecipientCache = mailItem.ADRecipientCache;
					ItemConversion.ConvertAnyMimeToItem(messageItem, mailItem.Message, inboundConversionOptions);
					messageItem.Save(SaveMode.NoConflictResolution);
				}
				int num = 0;
				foreach (AttachmentHandle handle in messageItem.AttachmentCollection)
				{
					if (num > lastXsoItemAttachmentIndex)
					{
						using (Attachment attachment = messageItem.AttachmentCollection.Open(handle))
						{
							ItemAttachment itemAttachment = attachment as ItemAttachment;
							if (itemAttachment != null)
							{
								OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomain.DomainName.Domain);
								outboundConversionOptions.RecipientCache = mailItem.ADRecipientCache;
								outboundConversionOptions.ClearCategories = mailItem.TransportSettings.ClearCategories;
								outboundConversionOptions.UseRFC2231Encoding = mailItem.TransportSettings.Rfc2231EncodingEnabled;
								stream = Streams.CreateTemporaryStorageStream();
								using (Item item = itemAttachment.GetItem())
								{
									item.Load(StoreObjectSchema.ContentConversionProperties);
									ItemConversion.ConvertItemToMsgStorage(item, stream, outboundConversionOptions);
								}
								stream.Position = 0L;
								lastXsoItemAttachmentIndex = num;
								flag = true;
								break;
							}
						}
					}
					num++;
				}
				if (!flag)
				{
					throw new InvalidOperationException("Could not find corresponding embedded attachment in XSO message");
				}
			}
			finally
			{
				if (!flag)
				{
					if (flag2 && messageItem != null)
					{
						messageItem.Dispose();
						messageItem = null;
					}
					if (stream != null)
					{
						stream.Dispose();
						stream = null;
					}
				}
			}
			return stream;
		}

		private static readonly Encoding HTMLEncoding = Encoding.GetEncoding("Windows-1252");
	}
}
