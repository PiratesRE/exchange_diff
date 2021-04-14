using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal static class Utils
	{
		public static OutboundConversionOptions GetOutboundConversionOptions(MailItem mailItem)
		{
			OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomain.DomainName.Domain);
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
			outboundConversionOptions.RecipientCache = (ADRecipientCache<TransportMiniRecipient>)transportMailItemWrapperFacade.TransportMailItem.ADRecipientCacheAsObject;
			if (transportMailItemWrapperFacade.TransportMailItem.TransportSettings != null)
			{
				outboundConversionOptions.ClearCategories = transportMailItemWrapperFacade.TransportMailItem.TransportSettings.ClearCategories;
				outboundConversionOptions.UseRFC2231Encoding = transportMailItemWrapperFacade.TransportMailItem.TransportSettings.Rfc2231EncodingEnabled;
			}
			return outboundConversionOptions;
		}

		public static OutboundConversionOptions GetOutboundConversionOptions(IReadOnlyMailItem mailItem)
		{
			OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomain.DomainName.Domain);
			outboundConversionOptions.RecipientCache = mailItem.ADRecipientCache;
			if (mailItem.TransportSettings != null)
			{
				outboundConversionOptions.ClearCategories = mailItem.TransportSettings.ClearCategories;
				outboundConversionOptions.UseRFC2231Encoding = mailItem.TransportSettings.Rfc2231EncodingEnabled;
			}
			return outboundConversionOptions;
		}

		public static void CopyHeadersDuringDecryption(EmailMessage srcMessage, EmailMessage targetMessage)
		{
			targetMessage.RootPart.Headers.RemoveAll(HeaderId.Subject);
			targetMessage.RootPart.Headers.RemoveAll("X-MS-Has-Attach");
			if (srcMessage.RootPart != null && targetMessage.RootPart != null)
			{
				for (MimeNode mimeNode = srcMessage.RootPart.Headers.LastChild; mimeNode != null; mimeNode = mimeNode.PreviousSibling)
				{
					Header header = mimeNode as Header;
					if (header != null && HeaderId.ContentClass != header.HeaderId && HeaderId.ContentType != header.HeaderId && HeaderId.ContentTransferEncoding != header.HeaderId && !string.Equals(header.Name, "X-MS-TNEF-Correlator", StringComparison.OrdinalIgnoreCase))
					{
						MimeNode newChild = header.Clone();
						targetMessage.RootPart.Headers.InsertAfter(newChild, null);
					}
				}
				if (targetMessage.AttachmentCollection_Count() > 0)
				{
					Utils.StampXHeader(targetMessage, "X-MS-Has-Attach", "yes");
				}
			}
		}

		public static EmailMessage ConvertMessageItemToEmailMessage(MessageItem item, OutboundConversionOptions contentConversionOption, bool useTNEF = true)
		{
			Stream stream = Streams.CreateTemporaryStorageStream();
			if (useTNEF)
			{
				ItemConversion.ConvertItemToSummaryTnef(item, stream, contentConversionOption);
			}
			else
			{
				ItemConversion.ConvertItemToMime(item, stream, contentConversionOption);
			}
			MimeDocument mimeDocument = new MimeDocument();
			mimeDocument.Load(stream, CachingMode.SourceTakeOwnership);
			return EmailMessage.Create(mimeDocument);
		}

		public static OrganizationId OrgIdFromMailItem(MailItem mailItem)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = mailItem as ITransportMailItemWrapperFacade;
			if (transportMailItemWrapperFacade != null && transportMailItemWrapperFacade.TransportMailItem != null)
			{
				return transportMailItemWrapperFacade.TransportMailItem.OrganizationIdAsObject as OrganizationId;
			}
			return null;
		}

		public static bool TryGetCultureInfoAndEncoding(EmailMessage message, out string charsetName, out CultureInfo cultureInfo, out Encoding encoding)
		{
			HeaderList headers = (message.RootPart == null) ? null : message.RootPart.Headers;
			cultureInfo = CultureProcessor.Instance.GetCultureInfo(headers, false);
			Charset charset = Utils.GetCharset(message);
			if (charset != null && charset.TryGetEncoding(out encoding))
			{
				charsetName = charset.Name;
				if (cultureInfo == null)
				{
					cultureInfo = charset.Culture.GetCultureInfo();
					if (string.IsNullOrEmpty(cultureInfo.Name))
					{
						cultureInfo = CultureProcessor.Instance.DefaultCulture;
					}
				}
				return true;
			}
			charsetName = null;
			cultureInfo = null;
			encoding = null;
			return false;
		}

		public static void StampXHeader(EmailMessage message, string xheader, string value)
		{
			Header header = Header.Create(xheader);
			header.Value = value;
			message.MimeDocument.RootPart.Headers.AppendChild(header);
		}

		public static Header GetXHeader(EmailMessage message, string xheader)
		{
			MimePart rootPart = message.RootPart;
			if (rootPart == null)
			{
				return null;
			}
			return rootPart.Headers.FindFirst(xheader);
		}

		public static ReadOnlyCollection<string> GetRecipientEmailAddresses(IReadOnlyMailItem mailItem)
		{
			ReadOnlyCollection<string> result = null;
			if (mailItem.ExtendedProperties.TryGetListValue<string>("Microsoft.Exchange.RMSEncryptionAgent.RecipientListForPL", out result))
			{
				return result;
			}
			List<string> list = new List<string>();
			EmailRecipientCollection emailRecipientCollection = mailItem.Message.To;
			foreach (EmailRecipient emailRecipient in emailRecipientCollection)
			{
				if (!string.IsNullOrEmpty(emailRecipient.SmtpAddress))
				{
					list.Add(emailRecipient.SmtpAddress);
				}
			}
			emailRecipientCollection = mailItem.Message.Cc;
			foreach (EmailRecipient emailRecipient2 in emailRecipientCollection)
			{
				if (!string.IsNullOrEmpty(emailRecipient2.SmtpAddress))
				{
					list.Add(emailRecipient2.SmtpAddress);
				}
			}
			emailRecipientCollection = mailItem.Message.BccFromOrgHeader;
			foreach (EmailRecipient emailRecipient3 in emailRecipientCollection)
			{
				if (!string.IsNullOrEmpty(emailRecipient3.SmtpAddress))
				{
					list.Add(emailRecipient3.SmtpAddress);
				}
			}
			if (list.Count == 0)
			{
				if (mailItem.Recipients == null || mailItem.Recipients.Count == 0)
				{
					return null;
				}
				foreach (MailRecipient mailRecipient in mailItem.Recipients)
				{
					list.Add(mailRecipient.Email.ToString());
				}
			}
			return new ReadOnlyCollection<string>(list);
		}

		public static void SetProtectedContentClass(EmailMessage protectedMessage)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug(0L, "Setting Protected ContentClass - rpmsg.message or the um classes");
			MimePart rootPart = protectedMessage.RootPart;
			HeaderList headers = rootPart.Headers;
			Header header = headers.FindFirst(HeaderId.ContentClass);
			if (header == null)
			{
				header = Header.Create(HeaderId.ContentClass);
				headers.AppendChild(header);
			}
			header.Value = "rpmsg.message";
		}

		public static void SetBodyContent(CultureInfo culture, Body body)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug(0L, "Setting BodyContent for the down level clients");
			string s = string.Format("{0} {1}", SystemMessages.BodyReceiveRMEmail.ToString(culture), SystemMessages.BodyDownload.ToString(culture));
			Encoding encoding = Encoding.GetEncoding(body.CharsetName);
			byte[] bytes = encoding.GetBytes(s);
			using (Stream contentWriteStream = body.GetContentWriteStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(contentWriteStream))
				{
					binaryWriter.Write(bytes);
				}
			}
		}

		public static void CopyHeadersDuringEncryption(EmailMessage srcMessage, EmailMessage targetMessage)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug(0L, "Copying Email Properties to the protected message");
			if (srcMessage.RootPart != null && targetMessage.RootPart != null)
			{
				for (MimeNode mimeNode = srcMessage.RootPart.Headers.LastChild; mimeNode != null; mimeNode = mimeNode.PreviousSibling)
				{
					Header header = mimeNode as Header;
					if (header != null && HeaderId.ContentClass != header.HeaderId && HeaderId.ContentType != header.HeaderId && HeaderId.ContentTransferEncoding != header.HeaderId && !string.Equals("X-MS-Exchange-Organization-RightsProtectMessage", header.Name, StringComparison.OrdinalIgnoreCase) && !string.Equals("X-MS-Has-Attach", header.Name, StringComparison.OrdinalIgnoreCase))
					{
						MimeNode newChild = header.Clone();
						targetMessage.RootPart.Headers.InsertAfter(newChild, null);
					}
				}
				Utils.StampXHeader(targetMessage, "X-MS-Has-Attach", "yes");
			}
		}

		public static bool IsProtectedEmail(EmailMessage message)
		{
			bool result = false;
			if (Utils.IsProtectedContentClass(message) && message.Attachments != null && 1 == message.Attachments.Count && Utils.IsProtectedAttachment(message.Attachments[0]))
			{
				result = true;
			}
			return result;
		}

		private static bool IsProtectedContentClass(EmailMessage message)
		{
			if (message.RootPart == null)
			{
				return false;
			}
			HeaderList headers = message.RootPart.Headers;
			Header header = headers.FindFirst(HeaderId.ContentClass);
			return header != null && (string.Equals(header.Value, "rpmsg.message", StringComparison.OrdinalIgnoreCase) || string.Equals(header.Value, "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA", StringComparison.OrdinalIgnoreCase) || string.Equals(header.Value, "IPM.Note.rpmsg.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase));
		}

		private static bool IsProtectedAttachment(Attachment attachment)
		{
			return string.Equals(attachment.FileName, "message.rpmsg", StringComparison.OrdinalIgnoreCase);
		}

		private static Charset GetCharset(EmailMessage message)
		{
			Charset result;
			if (!EmailMessageHelpers.TryGetTnefBinaryCharset(message, out result))
			{
				string charsetName = message.Body.CharsetName;
				if (string.IsNullOrEmpty(charsetName))
				{
					ExTraceGlobals.RightsManagementTracer.TraceDebug(0L, "Use Default ANSI charset; Body.CharsetName is null.");
					Charset.TryGetCharset(1252, out result);
				}
				else
				{
					ExTraceGlobals.RightsManagementTracer.TraceDebug(0L, "Use Body.CharsetName as charset; PureTnefMessage or PureTnefMessage.BinaryCharset is null.");
					Charset.TryGetCharset(charsetName, out result);
				}
			}
			return result;
		}

		public const int DefaultANSICodePage = 1252;

		public const string RecipientListToGeneratePL = "Microsoft.Exchange.RMSEncryptionAgent.RecipientListForPL";
	}
}
