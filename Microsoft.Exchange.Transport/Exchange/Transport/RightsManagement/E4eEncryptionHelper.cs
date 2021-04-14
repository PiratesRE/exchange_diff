using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.E4E;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal class E4eEncryptionHelper
	{
		internal static E4eEncryptionHelper Instance
		{
			get
			{
				if (E4eEncryptionHelper.instance == null)
				{
					E4eEncryptionHelper.instance = new E4eEncryptionHelper();
				}
				return E4eEncryptionHelper.instance;
			}
		}

		internal static bool ShouldReEncryptMessageOrGenerateHtml(EmailMessage emailMessage)
		{
			return E4eHelper.IsHeaderSetToTrue(emailMessage, "X-MS-Exchange-Organization-E4eReEncryptMessage") && E4eHelper.IsHeaderSetToTrue(emailMessage, "X-MS-Exchange-Organization-E4eMessageDecrypted") && !E4eHelper.IsHeaderSetToTrue(emailMessage, "X-MS-Exchange-Organization-E4eHtmlFileGenerated");
		}

		internal static bool ShouldEncryptMessageOrGenrateHtml(EmailMessage emailMessage, bool isOriginatingMail, bool isSupportedMapiMessageClass, bool messageEncrypted)
		{
			if (!isOriginatingMail)
			{
				return false;
			}
			if (!E4eHelper.IsHeaderSetToTrue(emailMessage, "X-MS-Exchange-Organization-E4eEncryptMessage"))
			{
				return false;
			}
			if (E4eHelper.IsHeaderSetToTrue(emailMessage, "X-MS-Exchange-Organization-E4eHtmlFileGenerated"))
			{
				return false;
			}
			if (!messageEncrypted)
			{
				if (emailMessage.IsSystemMessage || emailMessage.IsOpaqueMessage)
				{
					string text = emailMessage.IsSystemMessage ? "SystemMessage" : "OpaqueMessage";
					E4eLog.Instance.LogInfo(emailMessage.MessageId, "Not encrypting b/c the message is a {0}", new object[]
					{
						text
					});
					return false;
				}
				if (!isSupportedMapiMessageClass)
				{
					E4eLog.Instance.LogInfo(emailMessage.MessageId, "Not encrypting b/c isSupportedMapiMessageClass = false", new object[0]);
					return false;
				}
			}
			return true;
		}

		internal static void ValidateOriginalSender(string safeSenderName, string messageId, OrganizationId originalSenderOrgId, ref string originalSender)
		{
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
			if (Components.Configuration.TryGetAcceptedDomainTable(originalSenderOrgId, out perTenantAcceptedDomainTable) && perTenantAcceptedDomainTable.AcceptedDomainTable != null)
			{
				bool flag = false;
				if (!string.IsNullOrWhiteSpace(originalSender) && RoutingAddress.IsValidAddress(originalSender))
				{
					RoutingAddress value = RoutingAddress.Parse(originalSender);
					if (value != RoutingAddress.NullReversePath)
					{
						flag = (perTenantAcceptedDomainTable.AcceptedDomainTable.Find(value.DomainPart) != null);
						E4eLog.Instance.LogInfo(messageId, "senderDomainInOrg: {0}", new object[]
						{
							flag
						});
					}
				}
				else
				{
					E4eLog.Instance.LogError(messageId, "originalSender is not valid: {0}", new object[]
					{
						string.IsNullOrWhiteSpace(originalSender) ? "<blank>" : originalSender
					});
				}
				if (!flag)
				{
					string defaultDomainName = perTenantAcceptedDomainTable.AcceptedDomainTable.DefaultDomainName;
					if (!string.IsNullOrWhiteSpace(defaultDomainName) && SmtpAddress.IsValidDomain(defaultDomainName))
					{
						originalSender = string.Format("{0}@{1}", safeSenderName, defaultDomainName);
						return;
					}
					E4eLog.Instance.LogError(messageId, "defaultDomain is not valid: {0}", new object[]
					{
						string.IsNullOrWhiteSpace(defaultDomainName) ? "<blank>" : defaultDomainName
					});
					return;
				}
			}
			else
			{
				E4eLog.Instance.LogError(messageId, "Unable to get accepted domain table for org: {0}.", new object[]
				{
					originalSenderOrgId.ToString()
				});
			}
		}

		internal static EmailMessage GetOriginalMessage(IReadOnlyMailItem mailItem, Trace tracer, long tracerId, out bool needDispose, out Exception exception)
		{
			exception = null;
			OrganizationId originalSenderOrgId = null;
			string text;
			string text2;
			Uri uri;
			E4eHelper.GetTransportPLAndULAndLicenseUri(mailItem, out text, out text2, out uri);
			needDispose = false;
			if (string.IsNullOrWhiteSpace(text))
			{
				return mailItem.Message;
			}
			if (string.IsNullOrEmpty(text2))
			{
				tracer.TraceError<string>(tracerId, "UseLicense absent from message {0}", mailItem.Message.MessageId);
				return null;
			}
			if (uri == null)
			{
				tracer.TraceError<string>(tracerId, "LicenseUri is absent or corrupted for message {0}", mailItem.Message.MessageId);
				return null;
			}
			bool flag;
			E4eHelper.RunUnderExceptionHandler(mailItem.Message.MessageId, delegate
			{
				originalSenderOrgId = E4eHelper.GetOriginalSenderOrgId(mailItem);
			}, null, out exception, out flag);
			if (originalSenderOrgId == null)
			{
				tracer.TraceError<string, string>(tracerId, "Unable to get OriginalSenderOrgId for message {0} because of exception {1}", mailItem.Message.MessageId, (exception == null) ? "[No exception occurred]" : exception.ToString());
				return null;
			}
			EmailMessage result2;
			using (RmsEncryptor rmsEncryptor = new RmsEncryptor(originalSenderOrgId, mailItem, text, text2, uri, null))
			{
				IAsyncResult result = rmsEncryptor.BeginEncrypt(null, null);
				AsyncOperationResult<EmailMessage> encryptionResult = rmsEncryptor.EndEncrypt(result);
				if (!encryptionResult.IsSucceeded)
				{
					tracer.TraceError<string, string>(tracerId, "IRM re-encryption failed for message {0} because of {1}", mailItem.Message.MessageId, encryptionResult.Exception.ToString());
					exception = encryptionResult.Exception;
					result2 = null;
				}
				else
				{
					tracer.TraceDebug<string>(tracerId, "Successfully IRM re-encrypted message {0}", mailItem.Message.MessageId);
					EmailMessage modifiedMessage = null;
					E4eHelper.RunUnderExceptionHandler(mailItem.Message.MessageId, delegate
					{
						string p2From = E4eHelper.GetP2From(mailItem.Message);
						string originalSender = E4eHelper.GetOriginalSender(mailItem);
						MiniRecipient miniRecipient = E4eHelper.CreateMiniRecipient(originalSender, originalSenderOrgId);
						E4eEncryptionHelper e4eEncryptionHelper = E4eHelper.GetE4eEncryptionHelper(miniRecipient);
						string text3;
						CultureInfo cultureInfo;
						Encoding encoding;
						E4eHelper.GetCultureInfo(mailItem.Message, out text3, out cultureInfo, out encoding);
						string emailMessage = e4eEncryptionHelper.EmailMessageToString(encryptionResult.Data, Utils.GetOutboundConversionOptions(mailItem));
						modifiedMessage = e4eEncryptionHelper.CreateE4eMessage(encryptionResult.Data, emailMessage, originalSenderOrgId, originalSender, p2From, mailItem.Recipients[0].Email.ToString(), mailItem.DateReceived, cultureInfo, mailItem.IsProbe);
					}, null, out exception, out flag);
					encryptionResult.Data.Dispose();
					if (exception == null && modifiedMessage != null)
					{
						tracer.TraceDebug<string>(tracerId, "Successfully E4E message created for message {0}", mailItem.Message.MessageId);
						needDispose = true;
						result2 = modifiedMessage;
					}
					else
					{
						tracer.TraceError<string, string>(tracerId, "E4E message creation failed for message {0} because of exception {1}", mailItem.Message.MessageId, (exception == null) ? "[No exception occurred]" : exception.ToString());
						result2 = null;
					}
				}
			}
			return result2;
		}

		internal List<string> GetP1Recipients(MailItem mailItem)
		{
			List<string> list = new List<string>();
			foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
			{
				list.Add(envelopeRecipient.Address.ToString());
			}
			return list;
		}

		internal List<string> GetP2Recipients(MailItem mailItem)
		{
			List<string> list = new List<string>();
			foreach (EmailRecipient emailRecipient in mailItem.Message.To)
			{
				if (!string.IsNullOrWhiteSpace(emailRecipient.SmtpAddress))
				{
					list.Add(emailRecipient.SmtpAddress);
				}
			}
			foreach (EmailRecipient emailRecipient2 in mailItem.Message.Cc)
			{
				if (!string.IsNullOrWhiteSpace(emailRecipient2.SmtpAddress))
				{
					list.Add(emailRecipient2.SmtpAddress);
				}
			}
			foreach (EmailRecipient emailRecipient3 in mailItem.Message.BccFromOrgHeader)
			{
				if (!string.IsNullOrWhiteSpace(emailRecipient3.SmtpAddress))
				{
					list.Add(emailRecipient3.SmtpAddress);
				}
			}
			return list;
		}

		internal RmsEncryptor CreateRmsEncryptor(MailItem mailItem, OrganizationId originalSenderOrgId, Breadcrumbs<string> breadcrumbs)
		{
			IReadOnlyMailItem mailItem2 = (IReadOnlyMailItem)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			return new RmsEncryptor(originalSenderOrgId, mailItem2, new Guid("81E24817-F117-4943-8959-60E1477E67B6"), breadcrumbs, null);
		}

		internal RmsEncryptor CreateRmsEncryptor(MailItem mailItem, OrganizationId originalSenderOrgId, string publishingLicense, string useLicense, Uri licenseUri, Breadcrumbs<string> breadcrumbs)
		{
			IReadOnlyMailItem mailItem2 = (IReadOnlyMailItem)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem;
			return new RmsEncryptor(originalSenderOrgId, mailItem2, publishingLicense, useLicense, licenseUri, breadcrumbs);
		}

		internal void EncryptMessage(RmsEncryptor rmsEncryptor, AsyncCallback asyncCallback, object state)
		{
			rmsEncryptor.BeginEncrypt(asyncCallback, state);
		}

		internal EmailMessage CreateE4eMessage(EmailMessage message, string emailMessage, OrganizationId originalSenderOrgId, string originalSender, string currentP2Sender, string recipient, DateTime receivedTime, CultureInfo cultureInfo, bool isProbe)
		{
			EncryptionConfigurationData encryptionConfigurationData = null;
			try
			{
				SmtpAddress smtpAddress = new SmtpAddress(originalSender);
				EncryptionConfigurationTable.RequestData requestData;
				encryptionConfigurationData = EncryptionConfigurationTable.GetEncryptionConfiguration(smtpAddress.Domain, true, out requestData);
			}
			catch (Exception ex)
			{
				if (!isProbe)
				{
					E4eLog.Instance.LogError(message.MessageId, "Encountered an exception while getting encryption configuration. Using default values. Exception: {0}", new object[]
					{
						ex.ToString()
					});
				}
				encryptionConfigurationData = new EncryptionConfigurationData();
			}
			if (encryptionConfigurationData == null)
			{
				if (!isProbe)
				{
					E4eLog.Instance.LogError(message.MessageId, "Null returned while getting encryption configuration. Using default values.", new object[0]);
				}
				encryptionConfigurationData = new EncryptionConfigurationData();
			}
			bool otpEnabled = E4eHelper.IsOTPEnabledForSender(originalSender, originalSenderOrgId) && encryptionConfigurationData.OTPEnabled;
			E4eCssStyle cssStyle = E4eEncryptionHelper.CreateCssStyle(originalSender, originalSenderOrgId);
			string htmlContent = this.GenerateHtml(emailMessage, originalSender, currentP2Sender, recipient, E4eHelper.ToBase64String(originalSenderOrgId), receivedTime.ToString(), message.MessageId, encryptionConfigurationData, cultureInfo, otpEnabled, cssStyle);
			EmailMessage emailMessage2 = EmailMessage.Create(Microsoft.Exchange.Data.Transport.Email.BodyFormat.Html, false, Charset.UTF8.Name);
			this.WriteBody(emailMessage2, currentP2Sender, recipient, encryptionConfigurationData, cultureInfo, cssStyle);
			this.AddAttachment(emailMessage2, htmlContent);
			Utils.CopyHeadersDuringEncryption(message, emailMessage2);
			return emailMessage2;
		}

		internal static E4eCssStyle CreateCssStyle(string originalSenderEmailAddress, OrganizationId originalSenderOrgId)
		{
			MiniRecipient recipient = E4eHelper.CreateMiniRecipient(originalSenderEmailAddress, originalSenderOrgId);
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(recipient.GetContext(null), null, null);
			IFeature touchLayout = snapshot.E4E.TouchLayout;
			if (touchLayout != null && touchLayout.Enabled)
			{
				return new E4eCssStyleV2();
			}
			return new E4eCssStyleV1();
		}

		internal void ReplaceRpmsgWithE4eMsg(MailItem mailItem, OrganizationId originalSenderOrgId, string originalSender, string currentP2Sender, CultureInfo cultureInfo)
		{
			if (mailItem.Message.Attachments == null || mailItem.Message.Attachments.Count != 1 || !mailItem.Message.Attachments[0].FileName.Equals("message.rpmsg", StringComparison.OrdinalIgnoreCase))
			{
				throw new E4eException(string.Format("Attachments is null or AttachmentCount not equal 1 or Attachment name not equal: {0}", "message.rpmsg"));
			}
			if (mailItem.Recipients.Count != 1)
			{
				throw new E4eException(string.Format("Message recipient count is not equal to 1. Message recipient count: ", mailItem.Recipients.Count));
			}
			object obj;
			mailItem.Properties.TryGetValue("Microsoft.Exchange.Encryption.TransportRpmsg", out obj);
			string text = (string)obj;
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new E4eException("Rpmsg not found in mailItem properties.");
			}
			mailItem.Properties["Microsoft.Exchange.Encryption.TransportRpmsg"] = string.Empty;
			EmailMessage emailMessage = this.CreateE4eMessage(mailItem.Message, text, originalSenderOrgId, originalSender, currentP2Sender, mailItem.Recipients[0].Address.ToString(), mailItem.DateTimeReceived, cultureInfo, mailItem.IsProbeMessage);
			E4eHelper.OverrideMime(mailItem, emailMessage);
		}

		internal void CreateMetadata(string originalSender, string recipient, string originalSenderOrgId, string sentTime, string messageId, out string metadata, out string signature, out string exportedCertificate)
		{
			Encoding utf = Encoding.UTF8;
			originalSender = Convert.ToBase64String(utf.GetBytes(originalSender));
			recipient = Convert.ToBase64String(utf.GetBytes(recipient));
			sentTime = Convert.ToBase64String(utf.GetBytes(sentTime));
			messageId = Convert.ToBase64String(utf.GetBytes(messageId));
			metadata = string.Join('|'.ToString(), new string[]
			{
				originalSender,
				recipient,
				originalSenderOrgId,
				sentTime,
				messageId
			});
			X509Certificate2 x509Certificate;
			signature = Convert.ToBase64String(this.SignText(metadata, out x509Certificate));
			exportedCertificate = Convert.ToBase64String(x509Certificate.Export(X509ContentType.Cert));
		}

		internal string EmailMessageToString(EmailMessage message, OutboundConversionOptions outboundOptions)
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ItemConversion.ConvertAnyMimeToMime(message, memoryStream, outboundOptions);
				byte[] bytes = memoryStream.ToArray();
				result = AntiXssEncoder.HtmlEncode(Encoding.UTF8.GetString(bytes), false);
			}
			return result;
		}

		private void AddAttachment(EmailMessage message, string htmlContent)
		{
			Attachment attachment = message.Attachments.Add("message.html", "text/html");
			byte[] bytes = Encoding.UTF8.GetBytes(htmlContent);
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				MimePart mimePart = attachment.MimePart;
				mimePart.SetContentStream(ContentTransferEncoding.SevenBit, memoryStream, CachingMode.Copy);
			}
		}

		private string AddInlineImage(EmailMessage message, string imageName, string base64Image)
		{
			Attachment attachment = message.Attachments.Add(imageName, "image/png");
			attachment.AttachmentType = AttachmentType.Inline;
			byte[] buffer = Convert.FromBase64String(base64Image);
			using (Stream stream = new MemoryStream(buffer))
			{
				MimePart mimePart = attachment.MimePart;
				mimePart.SetContentStream(ContentTransferEncoding.Base64, stream, CachingMode.Copy);
			}
			Header header = Header.Create(HeaderId.ContentId);
			string text = Guid.NewGuid().ToString();
			header.Value = string.Format("<{0}>", text);
			attachment.MimePart.Headers.AppendChild(header);
			return text;
		}

		private byte[] SignText(string text, out X509Certificate2 certificate)
		{
			string certificateName = E4eHelper.GetCertificateName();
			certificate = null;
			X509Store x509Store = null;
			byte[] result;
			try
			{
				x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.ReadOnly);
				RSACryptoServiceProvider rsacryptoServiceProvider = null;
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, certificateName, true);
				foreach (X509Certificate2 x509Certificate in x509Certificate2Collection)
				{
					if (x509Certificate.HasPrivateKey)
					{
						try
						{
							certificate = x509Certificate;
							rsacryptoServiceProvider = (RSACryptoServiceProvider)x509Certificate.PrivateKey;
							break;
						}
						catch (CryptographicException)
						{
						}
					}
				}
				if (rsacryptoServiceProvider == null)
				{
					throw new E4eException(string.Format("Certificate not found. CertificateName: {0}", certificateName));
				}
				UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
				byte[] bytes = unicodeEncoding.GetBytes(text);
				result = this.SignText(rsacryptoServiceProvider, bytes);
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
			return result;
		}

		private void WriteBody(EmailMessage message, string currentP2Sender, string recipientAddress, EncryptionConfigurationData encryptionConfigurationData, CultureInfo cultureInfo, E4eCssStyle cssStyle)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = SystemMessages.E4EReceivedMessage.ToString(cultureInfo);
			string arg = SystemMessages.E4EViewMessage.ToString(cultureInfo);
			string arg2 = SystemMessages.E4EOpenAttachment("message.html").ToString(cultureInfo);
			string text2 = SystemMessages.E4ESignIn.ToString(cultureInfo);
			string imageName = "lock.png";
			string arg3 = this.AddInlineImage(message, imageName, cssStyle.LockImgBase64);
			string arg4 = string.IsNullOrEmpty(encryptionConfigurationData.DisclaimerText) ? SystemMessages.E4EDisclaimer.ToString(cultureInfo) : encryptionConfigurationData.DisclaimerText;
			string arg5 = cultureInfo.TextInfo.IsRightToLeft ? " dir='rtl'" : string.Empty;
			stringBuilder.Append(string.Format("<div style='width:600px; border-top: 1px solid #EAECEE;'{0}><br/>", arg5));
			if (string.IsNullOrEmpty(encryptionConfigurationData.EmailText))
			{
				stringBuilder.Append(string.Format("<span id='ReceivedMessage' style='{0}'>{1} </span> <span><a id='SenderAddress' style='{2}; {3}' href=''>{4}</a></span>", new object[]
				{
					cssStyle.RegularTextStyle,
					text,
					cssStyle.BoldTextStyle,
					cssStyle.EmailTextAnchorStyle,
					currentP2Sender
				}));
			}
			else
			{
				stringBuilder.Append(string.Format("<span id='ReceivedMessage' style='{0}'>{1} </span>", cssStyle.RegularTextStyle, encryptionConfigurationData.EmailText));
			}
			stringBuilder.Append(string.Format("<br/><br/><div id='ViewMessage' style='{0}'>{1}</div>", cssStyle.BoldTextStyle, arg));
			stringBuilder.Append(string.Format("<div id='OpenAttachment' style='{0}'>{1}</div>", cssStyle.RegularTextStyle, arg2));
			stringBuilder.Append("<div>");
			stringBuilder.Append(string.Format("<span id='SignIn' style='{0}'>{1} </span> <span><a id='RecipientAddress' style='{2}; {3}' href=''>{4}</a></span>", new object[]
			{
				cssStyle.RegularTextStyle,
				text2,
				cssStyle.BoldTextStyle,
				cssStyle.EmailTextAnchorStyle,
				recipientAddress
			}));
			stringBuilder.Append("</div><br/><br/><br/>");
			stringBuilder.Append(string.Format("<div id='Disclaimer' style='{0}'>{1}</div><br/><br/><br/>", cssStyle.DisclaimerTextStyle, arg4));
			stringBuilder.Append("<table style='padding-top:8px; border-top: 1px solid #EAECEE;'><tr>");
			stringBuilder.Append(string.Format("<td><img id='LockImage' style='{0}' src='cid:{1}'/></td>", cssStyle.LockSizeStyle, arg3));
			this.AppendHostedMessage(stringBuilder, cultureInfo, cssStyle);
			stringBuilder.Append("</tr></table><br/>");
			if (!string.IsNullOrEmpty(encryptionConfigurationData.ImageBase64))
			{
				string imageName2 = "logo.png";
				string arg6 = this.AddInlineImage(message, imageName2, encryptionConfigurationData.ImageBase64);
				stringBuilder.Append("<table><tr>");
				stringBuilder.Append(string.Format("<td><img id='LogoImage' src='cid:{0}'/></td>", arg6));
				stringBuilder.Append("</tr></table>");
			}
			stringBuilder.Append("</div>");
			using (Stream contentWriteStream = message.Body.GetContentWriteStream())
			{
				byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
				using (BinaryWriter binaryWriter = new BinaryWriter(contentWriteStream))
				{
					binaryWriter.Write(bytes);
				}
			}
		}

		private string GenerateHtml(string emailMessage, string originalSender, string currentP2Sender, string recipient, string originalSenderOrgId, string sentTime, string messageId, EncryptionConfigurationData encryptionConfigurationData, CultureInfo cultureInfo, bool otpEnabled, E4eCssStyle cssStyle)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = SystemMessages.E4EHeaderCustom.ToString(cultureInfo);
			string text2 = SystemMessages.E4EEncryptedMessage.ToString(cultureInfo);
			string text3 = SystemMessages.E4EViewMessageInfo.ToString(cultureInfo);
			string text4 = SystemMessages.E4EViewMessageButton.ToString(cultureInfo);
			string arg = SystemMessages.E4EViewMessageOTPButton.ToString(cultureInfo);
			string text5 = SystemMessages.E4EWaitMessage.ToString(cultureInfo);
			if (!string.IsNullOrEmpty(encryptionConfigurationData.PortalText))
			{
				text = encryptionConfigurationData.PortalText;
			}
			string arg2 = cultureInfo.TextInfo.IsRightToLeft ? " dir='rtl'" : string.Empty;
			stringBuilder.AppendLine("<!DOCTYPE HTML>");
			stringBuilder.AppendLine("<!-- saved from url=(0014)about:internet -->");
			stringBuilder.AppendLine(string.Format("<html xmlns='http://www.w3.org/1999/xhtml'{0}>", arg2));
			stringBuilder.AppendLine(string.Format("<head><meta http-equiv='Content-type' content='text/html;charset=UTF-8'/><title>{0}</title>", text));
			stringBuilder.AppendLine(cssStyle.ViewportMetaTag);
			stringBuilder.AppendLine("</head>");
			stringBuilder.AppendLine("<body onload=\"if (document.getElementById('submit1').disabled || document.getElementById('submit2').disabled) document.getElementById('submit1').form.submit();\">");
			stringBuilder.AppendLine(string.Format("<form name='form1' id='form1' method='post' enctype='multipart/form-data' onsubmit='if (document.readyState !== \"complete\") return false;' action='{0}?recipientemailaddress={1}&senderemailaddress={2}&senderorganization={3}'>", new object[]
			{
				this.GetE4eServiceUrl(),
				HttpUtility.UrlEncode(recipient),
				HttpUtility.UrlEncode(originalSender),
				HttpUtility.UrlEncode(originalSenderOrgId)
			}));
			this.AppendVersionInfo(stringBuilder, messageId);
			this.AppendMetadata(stringBuilder, originalSender, recipient, originalSenderOrgId, sentTime, messageId);
			E4eLog.Instance.LogInfo(messageId, "[E][form].rpmsgString.Length: {0}", new object[]
			{
				emailMessage.Length
			});
			stringBuilder.AppendLine(string.Format("<div id='Header' style='{0}; {1}'>{2}</div>", cssStyle.HeaderDivStyle, cssStyle.HeaderTextStyle, text));
			stringBuilder.AppendLine(string.Format("<div style='{0}'>", cssStyle.MainContentDivStyle));
			stringBuilder.AppendLine(string.Format("<div id='EncryptedMessage' style='{0}'>{1}<br><a id='SenderAddress' style='{2} color:#666666;'><span style='{3}'>{4}</span></a></div>", new object[]
			{
				cssStyle.EncryptedMessageDivStyle,
				text2,
				cssStyle.EmailTextAnchorStyle,
				cssStyle.EmailAddressSpanStyle,
				currentP2Sender
			}));
			stringBuilder.AppendLine(string.Format("<div id='ViewMessageText' style='padding-bottom:25px; font: 15px Segoe UI Semilight; color:#333333; font-family: Segoe UI Semilight, Segoe UI, Segoe, Helvetica, Tahoma, Arial, sans-serif;'>{0}<br><a id='RecipientAddress' style='{1} color:#333333;'><span style='{2}'>{3}</span></a></div>", new object[]
			{
				text3,
				cssStyle.EmailTextAnchorStyle,
				cssStyle.EmailAddressSpanStyle,
				recipient
			}));
			stringBuilder.AppendLine(string.Format("<div id='ViewMessageButton' style='{0}'><input style='{1}' id='submit1' type='submit' value='{2}' onclick='this.disabled=true;this.value=\"{3}\"; if (document.readyState === \"complete\") this.form.submit();'/></div>", new object[]
			{
				cssStyle.ViewMessageButtonDivStyle,
				cssStyle.ButtonStyle(cssStyle.ArrowImgBase64),
				text4,
				text5
			}));
			if (otpEnabled)
			{
				stringBuilder.AppendLine(string.Format("<div id='ViewMessageOTPButton' style='padding-bottom:8px;'><input name='OTPButton' id='submit2' style='{0}' type='submit' value=\"{1}\" onclick='this.disabled=true;this.form.action+=\"{2}\"; if (document.readyState === \"complete\") this.form.submit();'/></div>", cssStyle.ViewMessageOTPButtonStyle, arg, "&otp=true"));
			}
			stringBuilder.AppendLine(string.Format("<table style='{0}'><tr>", cssStyle.HostedMessageTableStyle));
			stringBuilder.AppendLine(string.Format("<td id='LockImage' style='{0} background:#ffffff url(data:{1};base64,{2}) no-repeat center center;'></td>", cssStyle.LockSizeStyle, "image/png", cssStyle.LockImgBase64));
			this.AppendHostedMessage(stringBuilder, cultureInfo, cssStyle);
			stringBuilder.AppendLine("</tr></table>");
			if (!string.IsNullOrEmpty(encryptionConfigurationData.ImageBase64))
			{
				stringBuilder.AppendLine("<table><tr>");
				stringBuilder.AppendLine(string.Format("<td id='LogoImage' style='{0} background:#ffffff url(data:{1};base64,{2}) no-repeat;'></td>", cssStyle.LogoSizeStyle, "image/png", encryptionConfigurationData.ImageBase64));
				stringBuilder.AppendLine("</tr></table>");
			}
			stringBuilder.AppendLine("</div>");
			stringBuilder.AppendLine(string.Format("<input type='hidden' name='{0}' value='{1}' />", "rpmsg", emailMessage));
			stringBuilder.AppendLine("</form>");
			stringBuilder.AppendLine("</body>");
			stringBuilder.AppendLine("</html>");
			return stringBuilder.ToString();
		}

		private void AppendMetadata(StringBuilder html, string originalSender, string recipient, string originalSenderOrgId, string sentTime, string messageId)
		{
			string text;
			string text2;
			string text3;
			this.CreateMetadata(originalSender, recipient, originalSenderOrgId, sentTime, messageId, out text, out text2, out text3);
			html.AppendLine(string.Format("<input type='hidden' name='{0}' value='{1}' />", "metadata", text));
			html.AppendLine(string.Format("<input type='hidden' name='{0}' value='{1}' />", "signature", text2));
			html.AppendLine(string.Format("<input type='hidden' name='{0}' value='{1}' />", "certificate", text3));
			E4eLog.Instance.LogInfo(messageId, "[E][form].metaData.Length: {0}", new object[]
			{
				text.Length
			});
			E4eLog.Instance.LogInfo(messageId, "[E][form].signature.Length: {0}", new object[]
			{
				text2.Length
			});
			E4eLog.Instance.LogInfo(messageId, "[E][form].exportedCertificate.Length: {0}", new object[]
			{
				text3.Length
			});
			E4eLog.Instance.LogInfo(messageId, "[E][form].metaData[OriginalSender]: {0}", new object[]
			{
				originalSender
			});
			E4eLog.Instance.LogInfo(messageId, "[E][form].metaData[Recipient]: {0}", new object[]
			{
				recipient
			});
			E4eLog.Instance.LogInfo(messageId, "[E][form].metaData[OriginalSenderOrgId]: {0}", new object[]
			{
				originalSenderOrgId
			});
			E4eLog.Instance.LogInfo(messageId, "[E][form].metaData[SentTime]: {0}", new object[]
			{
				sentTime
			});
			E4eLog.Instance.LogInfo(messageId, "[E][form].metaData[MessageId]: {0}", new object[]
			{
				messageId
			});
		}

		private void AppendHostedMessage(StringBuilder html, CultureInfo cultureInfo, E4eCssStyle cssStyle)
		{
			string arg = SystemMessages.E4EHosted.ToString(cultureInfo);
			html.AppendLine(string.Format("<td id='Hosted' style='{0}'>{1}</td>", cssStyle.HostedTextStyle, arg));
		}

		private string GetE4eServiceUrl()
		{
			if (!string.IsNullOrEmpty(E4eEncryptionHelper.cachedE4eServiceUrl))
			{
				return E4eEncryptionHelper.cachedE4eServiceUrl;
			}
			string result;
			lock (E4eEncryptionHelper.cacheLock)
			{
				if (!string.IsNullOrEmpty(E4eEncryptionHelper.cachedE4eServiceUrl))
				{
					result = E4eEncryptionHelper.cachedE4eServiceUrl;
				}
				else
				{
					object obj2 = E4eHelper.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "E4eServiceUrl");
					if (obj2 == null)
					{
						result = "https://outlook.com/Encryption/store.ashx";
					}
					else
					{
						string value = obj2.ToString();
						if (string.IsNullOrEmpty(value))
						{
							result = "https://outlook.com/Encryption/store.ashx";
						}
						else
						{
							E4eEncryptionHelper.cachedE4eServiceUrl = value;
							result = E4eEncryptionHelper.cachedE4eServiceUrl;
						}
					}
				}
			}
			return result;
		}

		internal virtual void AppendVersionInfo(StringBuilder html, string messageId)
		{
			E4eLog.Instance.LogInfo(messageId, "[E][form].Version: <blank>", new object[0]);
		}

		internal virtual byte[] SignText(RSACryptoServiceProvider csp, byte[] data)
		{
			byte[] result;
			using (SHA1Cng sha1Cng = new SHA1Cng())
			{
				byte[] rgbHash = sha1Cng.ComputeHash(data);
				result = csp.SignHash(rgbHash, CryptoConfig.MapNameToOID("SHA1"));
			}
			return result;
		}

		internal const string DeferralCountPropertyName = "Microsoft.Exchange.E4eEncryptionAgent.DeferralCount";

		internal const string ImageContentType = "image/png";

		internal const string NoExceptionOccurred = "[No exception occurred]";

		private const string DefautlE4eServiceUrl = "https://outlook.com/Encryption/store.ashx";

		private const string LegalLink = "http://go.microsoft.com/fwlink/?LinkID=287023&clcid=0x409";

		private const string PrivacyLink = "http://go.microsoft.com/fwlink/?LinkID=287024&clcid=0x409";

		private static string cachedE4eServiceUrl = string.Empty;

		private static object cacheLock = new object();

		private static E4eEncryptionHelper instance;
	}
}
