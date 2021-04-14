using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Internal;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.RightsManagement;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ContentConverter : IMailBifurcationHelper<RecipientEncoding>
	{
		public ContentConverter(TransportMailItem mailItem)
		{
			this.originalMailItem = mailItem;
			this.LoadMessageInformation();
		}

		public static RemoteDomainEntry GetDomainContentConfig(RoutingAddress smtpAddress, OrganizationId orgId)
		{
			ProxyAddress proxyAddress;
			if (Resolver.TryDeencapsulate(smtpAddress, out proxyAddress))
			{
				return null;
			}
			RemoteDomainEntry domainEntry = Components.Configuration.GetRemoteDomainTable(orgId).RemoteDomainTable.GetDomainEntry(SmtpDomain.GetDomainPart(smtpAddress));
			if (domainEntry != null)
			{
				ExTraceGlobals.ContentConversionTracer.TraceDebug<SmtpDomainWithSubdomains, string>(0L, "Find matching domain {0} for recipient: {1}", domainEntry.DomainName, smtpAddress.ToString());
			}
			return domainEntry;
		}

		public virtual TransportMailItem GenerateNewMailItem(IList<MailRecipient> newMailItemRecipients, RecipientEncoding encoding)
		{
			TransportMailItem transportMailItem = this.originalMailItem.NewCloneWithoutRecipients();
			this.GenerateContentForNewMailItem(this.originalMailItem, transportMailItem, encoding);
			foreach (MailRecipient mailRecipient in newMailItemRecipients)
			{
				mailRecipient.MoveTo(transportMailItem);
			}
			ContentConverter.diag.TraceDebug(0L, "Generate new Mail Item {0} using encoding: TNEF {1} InternetEncoding {2} Charset: {3}", new object[]
			{
				transportMailItem.RecordId,
				Convert.ToInt32(encoding.TNEFEnabled.GetValueOrDefault()),
				encoding.InternetEncoding.GetValueOrDefault(),
				encoding.CharacterSet
			});
			transportMailItem.CommitLazy();
			MessageTrackingLog.TrackTransfer(MessageTrackingSource.ROUTING, transportMailItem, this.originalMailItem.RecordId, "ContentConversion");
			return transportMailItem;
		}

		public bool GetBifurcationInfo(MailRecipient recipient, out RecipientEncoding encoding)
		{
			encoding = default(RecipientEncoding);
			if (!this.NeedsBifurcation(recipient))
			{
				return false;
			}
			if (recipient.NextHop.NextHopType.DeliveryType == DeliveryType.DeliveryAgent)
			{
				ContentConverter.diag.TraceDebug(0L, "No content conversion is performed for recipients going to Delivery Agent Connectors");
				return false;
			}
			RecipientEncoding recipientEncoding = this.GetADAndMapiRecipientEncoding(recipient);
			RemoteDomainEntry domainContentConfig = ContentConverter.GetDomainContentConfig(recipient.Email, this.originalMailItem.OrganizationId);
			MailFlowPartnerInternalMailContentType? partnerContentConfig = null;
			int num;
			if (recipient.ExtendedProperties.TryGetValue<int>("Microsoft.Exchange.Transport.Agent.OpenDomainRouting.MailFlowPartnerSettings.InternalMailContentType", out num))
			{
				MailFlowPartnerInternalMailContentType value = (MailFlowPartnerInternalMailContentType)num;
				if (EnumValidator<MailFlowPartnerInternalMailContentType>.IsValidValue(value))
				{
					partnerContentConfig = new MailFlowPartnerInternalMailContentType?(value);
				}
				else
				{
					ContentConverter.diag.TraceDebug<int>(0L, "Could not retrieve MailFlowPartnerInternalMailContentType from given extended property value : {0}.", num);
				}
			}
			recipientEncoding = ContentConverter.CombineRecipientEncodingWithDomainConfig(recipient, recipientEncoding, domainContentConfig, partnerContentConfig);
			if (recipientEncoding.TNEFEnabled == null)
			{
				recipientEncoding.TNEFEnabled = new bool?(SmtpProxyAddress.IsEncapsulatedAddress((string)recipient.Email));
				ContentConverter.diag.TraceDebug<string, int>(0L, "Recipient {0} TNEFEnabled is set to {1} due to the address type", recipient.Email.ToString(), Convert.ToInt32(recipientEncoding.TNEFEnabled.GetValueOrDefault()));
			}
			if (recipientEncoding.TNEFEnabled.GetValueOrDefault())
			{
				if (this.originalMailItem.Message.TnefPart != this.originalMailItem.Message.RootPart)
				{
					ContentConverter.diag.TraceDebug(0L, "Original mail item is in LTNEF as well. Skip conversion.");
					return false;
				}
				if (recipient.NextHop.NextHopType.DeliveryType == DeliveryType.NonSmtpGatewayDelivery)
				{
					ContentConverter.diag.TraceDebug(0L, "No content conversion to TNEF is performed for recipients going to Foreign Connectors");
					return false;
				}
			}
			encoding = recipientEncoding;
			return true;
		}

		public bool NeedsBifurcation()
		{
			Header header = this.originalMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-ContentConversionOptions");
			if (header != null)
			{
				ContentConverter.diag.TraceDebug(0L, "Found content conversion header on the message. Skip conversion.");
				return false;
			}
			EmailMessage message = this.originalMailItem.Message;
			if (message.TnefPart != null)
			{
				return true;
			}
			bool flag = false;
			TransportConfigContainer transportConfigObject = Configuration.TransportConfigObject;
			if (transportConfigObject.ConvertDisclaimerWrapperToEml)
			{
				Header header2 = this.originalMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Disclaimer-Wrapper");
				if (header2 != null && header2.Value != null && header2.Value.Equals("True"))
				{
					flag = true;
				}
			}
			if ((this.originalMailItem.IsJournalReport() || flag) && message.Attachments.Count == 1 && message.Attachments[0].IsEmbeddedMessage && message.Attachments[0].EmbeddedMessage.TnefPart != null)
			{
				return true;
			}
			if (ContentConverter.IsJournalReportContainingIrmMessage(this.originalMailItem, message))
			{
				return true;
			}
			ContentConverter.diag.TraceDebug(0L, "Message doesn't contain TNEF and is not journal report. Skip Conversion.");
			return false;
		}

		protected virtual void GenerateContentForNewMailItem(TransportMailItem originalMailItem, TransportMailItem newMailItem, RecipientEncoding encoding)
		{
			if (encoding.TNEFEnabled == null)
			{
				throw new InvalidOperationException("TNEF setting shouldn't be empty!");
			}
			OutboundConversionOptions conversionOptionsFromRecipientEncoding = this.GetConversionOptionsFromRecipientEncoding(encoding);
			using (Stream stream = newMailItem.OpenMimeWriteStream())
			{
				if (encoding.TNEFEnabled ?? false)
				{
					ItemConversion.ConvertAnyMimeToLegacyTnef(originalMailItem.Message, stream, conversionOptionsFromRecipientEncoding);
				}
				else
				{
					ItemConversion.ConvertAnyMimeToMime(originalMailItem.Message, stream, conversionOptionsFromRecipientEncoding);
				}
			}
			ContentConverter.AddContentConversionHeader(encoding, newMailItem.RootPart.Headers);
		}

		private static bool IsJournalReportContainingIrmMessage(TransportMailItem mailItem, EmailMessage message)
		{
			return mailItem != null && mailItem.IsJournalReport() && message != null && message.Attachments != null && message.Attachments.Count == 1 && message.Attachments[0].IsEmbeddedMessage && Utils.IsProtectedEmail(message.Attachments[0].EmbeddedMessage);
		}

		private static void AddContentConversionHeader(RecipientEncoding encoding, HeaderList headers)
		{
			headers.RemoveAll("X-MS-Exchange-Organization-ContentConversionOptions");
			Header newChild = new AsciiTextHeader("X-MS-Exchange-Organization-ContentConversionOptions", new BifInfo
			{
				SendTNEF = encoding.TNEFEnabled,
				SendInternetEncoding = ((encoding.InternetEncoding != null) ? new uint?((uint)encoding.InternetEncoding.Value) : null),
				FriendlyNames = new bool?(encoding.DisplaySenderName),
				CharSet = encoding.CharacterSet
			}.GetContentConversionOptionsString());
			headers.AppendChild(newChild);
		}

		private static bool IsOneOffOrContact(MailRecipient recipient)
		{
			RecipientItem recipientItem = RecipientItem.Create(recipient);
			return recipientItem is OneOffItem || recipientItem is ContactItem;
		}

		private static bool IsPreferenceBitSet(int encoding)
		{
			return (encoding & 131072) != 0;
		}

		private static RecipientEncoding CombineRecipientEncodingWithDomainConfig(MailRecipient recipient, RecipientEncoding recipientEncoding, RemoteDomainEntry domainContentConfig, MailFlowPartnerInternalMailContentType? partnerContentConfig)
		{
			if (partnerContentConfig != null)
			{
				ContentConverter.diag.TraceDebug<string>(0L, "Override TNEFEnabled using mailflow partner setting for recipient: {0}", recipient.Email.ToString());
				recipientEncoding.TNEFEnabled = new bool?(partnerContentConfig.Value == MailFlowPartnerInternalMailContentType.TNEF);
			}
			else if (domainContentConfig != null)
			{
				recipientEncoding.DisplaySenderName = domainContentConfig.DisplaySenderName;
				if (domainContentConfig.TNEFEnabled != null)
				{
					ContentConverter.diag.TraceDebug<string>(0L, "Override TNEFEnabled using domain setting for recipient: {0}", recipient.Email.ToString());
					recipientEncoding.TNEFEnabled = new bool?(domainContentConfig.TNEFEnabled.Value);
				}
				recipientEncoding.UseSimpleDisplayName = domainContentConfig.UseSimpleDisplayName;
			}
			if (domainContentConfig != null)
			{
				recipientEncoding.PreferredInternetCodePageForShiftJis = domainContentConfig.PreferredInternetCodePageForShiftJis;
				recipientEncoding.ByteEncoderTypeFor7BitCharsets = domainContentConfig.ByteEncoderTypeFor7BitCharsets;
				if (domainContentConfig.RequiredCharsetCoverage != null)
				{
					recipientEncoding.RequiredCharsetCoverage = new int?(domainContentConfig.RequiredCharsetCoverage.Value);
				}
			}
			if (recipientEncoding.PreferredInternetCodePageForShiftJis.Equals(PreferredInternetCodePageForShiftJisEnum.Undefined))
			{
				recipientEncoding.PreferredInternetCodePageForShiftJis = (PreferredInternetCodePageForShiftJisEnum)Components.TransportAppConfig.ContentConversion.PreferredInternetCodePageForShiftJis;
			}
			if (recipientEncoding.ByteEncoderTypeFor7BitCharsets.Equals(ByteEncoderTypeFor7BitCharsetsEnum.Undefined))
			{
				recipientEncoding.ByteEncoderTypeFor7BitCharsets = (ByteEncoderTypeFor7BitCharsetsEnum)Components.TransportAppConfig.ContentConversion.ByteEncoderTypeFor7BitCharsets;
			}
			if (recipientEncoding.RequiredCharsetCoverage == null)
			{
				recipientEncoding.RequiredCharsetCoverage = new int?(Components.TransportAppConfig.ContentConversion.RequiredCharsetCoverage);
			}
			if (recipientEncoding.InternetEncoding == null)
			{
				if (partnerContentConfig != null)
				{
					ContentConverter.diag.TraceDebug<int, string>(0L, "Use mailflow partner contentType {0} for recipient: {1}", (int)partnerContentConfig.Value, recipient.Email.ToString());
					recipientEncoding.InternetEncoding = new int?(ContentConverter.ConvertPartnerContentTypeToEncoding(partnerContentConfig.Value));
				}
				else if (domainContentConfig != null)
				{
					ContentConverter.diag.TraceDebug<int, string>(0L, "Use domain contentType {0} for recipient: {1}", (int)domainContentConfig.ContentType, recipient.Email.ToString());
					recipientEncoding.InternetEncoding = new int?(ContentConverter.ConvertContentTypeToEncoding(domainContentConfig.ContentType));
				}
			}
			if (string.IsNullOrEmpty(recipientEncoding.CharacterSet) && domainContentConfig != null)
			{
				if (recipientEncoding.IsMimeEncoding)
				{
					ContentConverter.diag.TraceDebug<string, string>(0L, "Use domain CharacterSet {0} for recipient: {1}", domainContentConfig.CharacterSet, recipient.Email.ToString());
					recipientEncoding.CharacterSet = domainContentConfig.CharacterSet;
				}
				else
				{
					ContentConverter.diag.TraceDebug<string, string>(0L, "Use domain NonMimeCharacterSet {0} for recipient: {1}", domainContentConfig.NonMimeCharacterSet, recipient.Email.ToString());
					recipientEncoding.CharacterSet = domainContentConfig.NonMimeCharacterSet;
				}
			}
			return recipientEncoding;
		}

		private static int ConvertContentTypeToEncoding(ContentType contentType)
		{
			switch (contentType)
			{
			case ContentType.MimeHtmlText:
				return 1441792;
			case ContentType.MimeText:
				return 393216;
			case ContentType.MimeHtml:
				return 917504;
			}
			return 0;
		}

		private static int? ConvertOverrideFormatToInternetEncoding(int overrideFormat)
		{
			if ((overrideFormat & 131072) != 0)
			{
				return new int?(overrideFormat);
			}
			switch (overrideFormat)
			{
			case 1:
				return new int?(1441792);
			case 2:
				return new int?(2228224);
			case 3:
				return new int?(131072);
			default:
				return null;
			}
		}

		private static int ConvertPartnerContentTypeToEncoding(MailFlowPartnerInternalMailContentType partnerInternalMailContentType)
		{
			switch (partnerInternalMailContentType)
			{
			case MailFlowPartnerInternalMailContentType.MimeHtmlText:
				return 1441792;
			case MailFlowPartnerInternalMailContentType.MimeText:
				return 393216;
			case MailFlowPartnerInternalMailContentType.MimeHtml:
				return 917504;
			default:
				return 0;
			}
		}

		private void LoadMessageInformation()
		{
			int num;
			if (this.originalMailItem.Message.TryGetMapiProperty<int>(TnefPropertyTag.INetMailOverrideFormat, out num))
			{
				ContentConverter.diag.TraceDebug<int>(0L, "Found Mapi Property INetMailOverrideFormat: {0}", num);
				this.mailOverrideFormatEncoding = ContentConverter.ConvertOverrideFormatToInternetEncoding(num);
			}
			string text;
			this.originalMailItem.Message.TryGetMapiProperty<string>(TnefPropertyTag.INetMailOverrideCharset, out text);
			if (!string.IsNullOrEmpty(text))
			{
				ContentConverter.diag.TraceDebug<string>(0L, "Found Mapi Property INetMailOverrideCharset: {0}", text);
				this.mailOverrideCharset = text;
			}
			Header header = this.originalMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-ContentConvertInternalMessage");
			this.contentConvertInternalMessage = (header != null);
		}

		private bool NeedsBifurcation(MailRecipient recipient)
		{
			return recipient.IsActive && !recipient.IsProcessed && (this.contentConvertInternalMessage || ContentConverter.IsOneOffOrContact(recipient));
		}

		private RecipientEncoding GetADAndMapiRecipientEncoding(MailRecipient recipient)
		{
			int? internetEncoding = null;
			bool? tnefEnabled = null;
			string arg = recipient.Email.ToString();
			int? value = recipient.ExtendedProperties.GetValue<int?>("Microsoft.Exchange.Transport.DirectoryData.InternetEncoding", null);
			if (value != null && ContentConverter.IsPreferenceBitSet(value.Value))
			{
				internetEncoding = value;
				ContentConverter.diag.TraceDebug<int, string>(0L, "Use Contact sendInternetEncoding {0} for {1}", value.Value, arg);
			}
			else if (this.mailOverrideFormatEncoding != null)
			{
				internetEncoding = this.mailOverrideFormatEncoding;
				ContentConverter.diag.TraceDebug<int?, string>(0L, "Use MAPI Message sendInternetEncoding {0} for {1}", this.mailOverrideFormatEncoding, arg);
			}
			else
			{
				int value2 = recipient.ExtendedProperties.GetValue<int>("Microsoft.Exchange.Transport.ClientRequestedInternetEncoding", 0);
				if (ContentConverter.IsPreferenceBitSet(value2))
				{
					internetEncoding = new int?(value2);
					ContentConverter.diag.TraceDebug<int, string>(0L, "Use MAPI Recipient sendInternetEncoding {0} for {1}", value2, arg);
				}
			}
			bool? value3 = recipient.ExtendedProperties.GetValue<bool?>("Microsoft.Exchange.Transport.DirectoryData.UseMapiRichTextFormat", null);
			bool flag;
			if (value3 != null)
			{
				tnefEnabled = value3;
				ContentConverter.diag.TraceDebug<bool, string>(0L, "Use Contact TNEFEnabled {0} for {1}", tnefEnabled.Value, arg);
			}
			else if (recipient.ExtendedProperties.TryGetValue<bool>("Microsoft.Exchange.Transport.ClientRequestedSendRichInfo", out flag))
			{
				ContentConverter.diag.TraceDebug<bool, string>(0L, "Use MAPI TNEFEnabled {0} for {1}", flag, arg);
				tnefEnabled = new bool?(flag);
			}
			return new RecipientEncoding(tnefEnabled, internetEncoding, this.mailOverrideCharset);
		}

		private OutboundConversionOptions GetConversionOptionsFromRecipientEncoding(RecipientEncoding encoding)
		{
			OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName);
			Charset preferredCharset = null;
			if (Charset.TryGetCharset(encoding.CharacterSet, out preferredCharset))
			{
				outboundConversionOptions.DetectionOptions.PreferredCharset = preferredCharset;
			}
			outboundConversionOptions.InternetMessageFormat = encoding.InternetMessageFormat;
			outboundConversionOptions.InternetTextFormat = encoding.InternetTextFormat;
			outboundConversionOptions.SuppressDisplayName = !encoding.DisplaySenderName;
			outboundConversionOptions.RecipientCache = this.originalMailItem.ADRecipientCache;
			outboundConversionOptions.ClearCategories = this.originalMailItem.TransportSettings.ClearCategories;
			outboundConversionOptions.Limits.MimeLimits = MimeLimits.Unlimited;
			outboundConversionOptions.LogDirectoryPath = Components.Configuration.LocalServer.ContentConversionTracingPath;
			outboundConversionOptions.UseRFC2231Encoding = this.originalMailItem.TransportSettings.Rfc2231EncodingEnabled;
			outboundConversionOptions.ByteEncoderTypeFor7BitCharsets = (ByteEncoderTypeFor7BitCharsets)encoding.ByteEncoderTypeFor7BitCharsets;
			outboundConversionOptions.UseSimpleDisplayName = encoding.UseSimpleDisplayName;
			outboundConversionOptions.UserADSession = this.originalMailItem.ADRecipientCache.ADSession;
			outboundConversionOptions.QuoteDisplayNameBeforeRfc2047Encoding = Components.TransportAppConfig.ContentConversion.QuoteDisplayNameBeforeRfc2047Encoding;
			outboundConversionOptions.DetectionOptions.RequiredCoverage = encoding.RequiredCharsetCoverage.Value;
			outboundConversionOptions.DetectionOptions.PreferredInternetCodePageForShiftJis = (int)encoding.PreferredInternetCodePageForShiftJis;
			return outboundConversionOptions;
		}

		public const string ClientRequestedInternetEncoding = "Microsoft.Exchange.Transport.ClientRequestedInternetEncoding";

		public const string ClientRequestedSendRichInfo = "Microsoft.Exchange.Transport.ClientRequestedSendRichInfo";

		private static readonly Trace diag = ExTraceGlobals.ContentConversionTracer;

		private TransportMailItem originalMailItem;

		private int? mailOverrideFormatEncoding;

		private string mailOverrideCharset;

		private bool contentConvertInternalMessage;
	}
}
