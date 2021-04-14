using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Encoders;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class EmailGenerationUtilities
	{
		public static string SanitizeSubject(string inputSubject)
		{
			SyncUtilities.ThrowIfArgumentNull("inputSubject", inputSubject);
			string result = inputSubject;
			if (inputSubject.Length > 255)
			{
				result = inputSubject.Substring(0, 255 - "...".Length) + "...";
			}
			return result;
		}

		public static bool TryGetMicrosoftExchangeRecipientSmtpAddress(ADSessionSettings userADSessionSettings, SyncLogSession syncLogSession, out string microsoftExchangeRecipientSmtpAddress)
		{
			microsoftExchangeRecipientSmtpAddress = null;
			MicrosoftExchangeRecipient exchangeRecipient = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.IgnoreInvalid, null, userADSessionSettings, 84, "TryGetMicrosoftExchangeRecipientSmtpAddress", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Common\\EmailGenerationUtilities.cs");
				exchangeRecipient = tenantOrTopologyConfigurationSession.FindMicrosoftExchangeRecipient();
			});
			if (!adoperationResult.Succeeded)
			{
				syncLogSession.LogError((TSLID)176UL, "AD operation failed while trying to find the Microsoft Exchange Recipient; Exception: {0}", new object[]
				{
					adoperationResult.Exception ?? "<null>"
				});
				return false;
			}
			if (exchangeRecipient == null)
			{
				syncLogSession.LogError((TSLID)2UL, "Unable to find a Microsoft Exchange Recipient for user's tenant org.", new object[0]);
				return false;
			}
			microsoftExchangeRecipientSmtpAddress = exchangeRecipient.PrimarySmtpAddress.ToString();
			return true;
		}

		public static MemoryStream GenerateEmailMimeStream(string messageId, string fromDisplayName, string fromSmtpAddress, string toSmtpAddress, string emailSubject, string htmlBodyContent, SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("messageId", messageId);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("fromDisplayName", fromDisplayName);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("fromSmtpAddress", fromSmtpAddress);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("toSmtpAddress", toSmtpAddress);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("emailSubject", emailSubject);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("htmlBodyContent", htmlBodyContent);
			OutboundCodePageDetector outboundCodePageDetector = new OutboundCodePageDetector();
			outboundCodePageDetector.AddText(messageId);
			outboundCodePageDetector.AddText(emailSubject);
			outboundCodePageDetector.AddText(htmlBodyContent);
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Charset charset;
			Encoding encoding;
			EmailGenerationUtilities.DetectCharSetAndEncodingFrom(outboundCodePageDetector, currentCulture, syncLogSession, out charset, out encoding);
			EncodingOptions encodingOptions = new EncodingOptions(charset.Name, currentCulture.Name, EncodingFlags.None);
			MemoryStream memoryStream = new MemoryStream(2000);
			using (MemoryStream memoryStream2 = new MemoryStream(2000))
			{
				using (MimeWriter mimeWriter = new MimeWriter(memoryStream2, true, encodingOptions))
				{
					mimeWriter.StartPart();
					EmailGenerationUtilities.WriteRFC822HeadersTo(mimeWriter, messageId, fromDisplayName, fromSmtpAddress, toSmtpAddress, emailSubject);
					EmailGenerationUtilities.WriteTextBodyPartTo(mimeWriter, htmlBodyContent, charset, encoding);
					EmailGenerationUtilities.WriteHtmlBodyPartTo(mimeWriter, htmlBodyContent, charset, encoding);
					mimeWriter.EndPart();
					mimeWriter.Flush();
					memoryStream2.WriteTo(memoryStream);
				}
			}
			memoryStream.Position = 0L;
			return memoryStream;
		}

		private static void DetectCharSetAndEncodingFrom(OutboundCodePageDetector codePageDetector, CultureInfo cultureInfo, SyncLogSession syncLogSession, out Charset charset, out Encoding encoding)
		{
			Culture culture;
			int codePage;
			if (Culture.TryGetCulture(cultureInfo.LCID, out culture))
			{
				codePage = codePageDetector.GetCodePage(culture, false);
			}
			else
			{
				codePage = codePageDetector.GetCodePage();
			}
			if (!Charset.TryGetCharset(codePage, out charset) || !charset.IsAvailable)
			{
				syncLogSession.LogInformation((TSLID)3UL, "Unable to determine charset for codepage: {0}.  Falling back to the UTF-8 charset.", new object[]
				{
					codePage
				});
				charset = Charset.UTF8;
			}
			if (!charset.TryGetEncoding(out encoding))
			{
				syncLogSession.LogInformation((TSLID)4UL, "Unable to get encoding for charset: {0}.  Falling back to using the UTF-8 charset and encoding.", new object[]
				{
					charset.Name
				});
				charset = Charset.UTF8;
				encoding = Encoding.UTF8;
			}
			syncLogSession.LogInformation((TSLID)5UL, "Detected charset: {0} and encoding: {1}", new object[]
			{
				charset.Name,
				encoding.EncodingName
			});
		}

		private static void WriteRFC822HeadersTo(MimeWriter mimeWriter, string messageId, string fromDisplayName, string fromSmtpAddress, string toSmtpAddress, string emailSubject)
		{
			mimeWriter.WriteHeader(HeaderId.MimeVersion, "1.0");
			mimeWriter.StartHeader(HeaderId.ContentType);
			mimeWriter.WriteHeaderValue("multipart/alternative");
			mimeWriter.WriteParameter("differences", "Content-Type");
			mimeWriter.WriteParameter("boundary", Guid.NewGuid().ToString());
			mimeWriter.WriteHeader(HeaderId.MessageId, messageId);
			mimeWriter.StartHeader(HeaderId.From);
			mimeWriter.WriteRecipient(fromDisplayName, fromSmtpAddress);
			mimeWriter.WriteHeader(HeaderId.To, toSmtpAddress);
			mimeWriter.WriteHeader(HeaderId.ReturnPath, "<>");
			mimeWriter.WriteHeader(HeaderId.Subject, emailSubject);
		}

		private static void WriteTextBodyPartTo(MimeWriter mimeWriter, string htmlContent, Charset charset, Encoding encoding)
		{
			mimeWriter.StartPart();
			mimeWriter.StartHeader(HeaderId.ContentType);
			mimeWriter.WriteHeaderValue("text/plain;");
			mimeWriter.WriteParameter("charset", charset.Name);
			mimeWriter.WriteHeader("Content-Transfer-Encoding", "quoted-printable");
			using (EncoderStream encoderStream = new EncoderStream(mimeWriter.GetRawContentWriteStream(), new QPEncoder(), EncoderStreamAccess.Write))
			{
				new HtmlToText
				{
					InputEncoding = encoding,
					DetectEncodingFromByteOrderMark = false,
					DetectEncodingFromMetaTag = false,
					OutputEncoding = encoding
				}.Convert(new StringReader(htmlContent), encoderStream);
				encoderStream.Flush();
			}
			mimeWriter.EndPart();
		}

		private static void WriteHtmlBodyPartTo(MimeWriter mimeWriter, string htmlContent, Charset charset, Encoding encoding)
		{
			mimeWriter.StartPart();
			mimeWriter.StartHeader(HeaderId.ContentType);
			mimeWriter.WriteHeaderValue("text/html;");
			mimeWriter.WriteParameter("charset", charset.Name);
			mimeWriter.WriteHeader("Content-Transfer-Encoding", "quoted-printable");
			using (EncoderStream encoderStream = new EncoderStream(mimeWriter.GetRawContentWriteStream(), new QPEncoder(), EncoderStreamAccess.Write))
			{
				byte[] bytes = encoding.GetBytes(htmlContent);
				encoderStream.Write(bytes, 0, bytes.Length);
				encoderStream.Flush();
			}
			mimeWriter.EndPart();
		}

		private const int EstimatedMessageDataSize = 2000;
	}
}
