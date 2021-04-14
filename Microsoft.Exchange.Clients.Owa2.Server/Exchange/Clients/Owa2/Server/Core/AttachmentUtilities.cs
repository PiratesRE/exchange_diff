using System;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.TextConverters.Internal;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class AttachmentUtilities
	{
		internal static string ToHexString(string fileName, bool chrome)
		{
			StringBuilder stringBuilder = new StringBuilder(fileName.Length);
			byte[] bytes = Encoding.UTF8.GetBytes(fileName);
			for (int i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] >= 0 && bytes[i] <= 127)
				{
					if (AttachmentUtilities.IsMIMEAttributeSpecialChar((char)bytes[i], chrome))
					{
						stringBuilder.AppendFormat("%{0}", Convert.ToString(bytes[i], 16));
					}
					else
					{
						stringBuilder.Append((char)bytes[i]);
					}
				}
				else
				{
					stringBuilder.AppendFormat("%{0}", Convert.ToString(bytes[i], 16));
				}
			}
			return stringBuilder.ToString();
		}

		internal static Stream GetFilteredStream(ConfigurationContextBase configurationContext, Stream inputStream, Charset charset, BlockStatus blockStatus)
		{
			HtmlToHtml htmlToHtml = new HtmlToHtml();
			TextConvertersInternalHelpers.SetPreserveDisplayNoneStyle(htmlToHtml, true);
			WebBeaconFilterLevels filterWebBeaconsAndHtmlForms = configurationContext.FilterWebBeaconsAndHtmlForms;
			bool flag = filterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.DisableFilter || blockStatus == BlockStatus.NoNeverAgain;
			Encoding encoding = null;
			if (charset != null && charset.TryGetEncoding(out encoding))
			{
				htmlToHtml.DetectEncodingFromMetaTag = false;
				htmlToHtml.InputEncoding = encoding;
				htmlToHtml.OutputEncoding = encoding;
			}
			else
			{
				htmlToHtml.DetectEncodingFromMetaTag = true;
				htmlToHtml.InputEncoding = Encoding.ASCII;
				htmlToHtml.OutputEncoding = null;
			}
			htmlToHtml.FilterHtml = true;
			if (!flag)
			{
				htmlToHtml.HtmlTagCallback = new HtmlTagCallback(AttachmentUtilities.webBeaconFilter.ProcessTag);
			}
			return new ConverterStream(inputStream, htmlToHtml, ConverterStreamAccess.Read);
		}

		internal static string GetContentType(Attachment attachment)
		{
			string text = attachment.ContentType;
			if (string.IsNullOrEmpty(text))
			{
				text = attachment.CalculatedContentType;
			}
			if (string.Equals(text, "audio/mp3", StringComparison.OrdinalIgnoreCase))
			{
				text = "audio/mpeg";
			}
			return text;
		}

		internal static bool NeedToFilterHtml(string contentType, string fileExtension, AttachmentPolicyLevel level, ConfigurationContextBase configurationContext)
		{
			bool flag = AttachmentUtilities.IsHtmlAttachment(contentType, fileExtension);
			bool flag2 = AttachmentPolicyLevel.ForceSave == level;
			bool flag3 = configurationContext.IsFeatureEnabled(Feature.ForceSaveAttachmentFiltering);
			return flag && (!flag2 || flag3);
		}

		internal static bool GetDoNotSniff(AttachmentPolicyLevel level, ConfigurationContextBase configurationContext)
		{
			if (configurationContext == null)
			{
				throw new ArgumentNullException("configurationContext");
			}
			return AttachmentPolicyLevel.ForceSave == level && !configurationContext.IsFeatureEnabled(Feature.ForceSaveAttachmentFiltering);
		}

		internal static bool GetIsHtmlOrXml(string contentType, string fileExtension)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			return AttachmentUtilities.IsXmlAttachment(contentType, fileExtension) || AttachmentUtilities.IsHtmlAttachment(contentType, fileExtension);
		}

		internal static string TryGetMailboxIdentityName()
		{
			UserContext userContext = UserContextManager.GetUserContext(HttpContext.Current, CallContext.Current.EffectiveCaller, true);
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			string result = string.Empty;
			if (userContext.MailboxIdentity != null)
			{
				result = userContext.MailboxIdentity.SafeGetRenderableName();
			}
			return result;
		}

		public static bool DeleteAttachment(AttachmentIdType attachmentId)
		{
			bool result = false;
			if (attachmentId != null)
			{
				DeleteAttachmentJsonRequest deleteAttachmentJsonRequest = new DeleteAttachmentJsonRequest();
				DeleteAttachmentRequest deleteAttachmentRequest = new DeleteAttachmentRequest();
				deleteAttachmentRequest.AttachmentIds = new AttachmentIdType[1];
				deleteAttachmentRequest.AttachmentIds[0] = attachmentId;
				deleteAttachmentJsonRequest.Body = deleteAttachmentRequest;
				OWAService owaservice = new OWAService();
				IAsyncResult asyncResult = owaservice.BeginDeleteAttachment(deleteAttachmentJsonRequest, null, null);
				asyncResult.AsyncWaitHandle.WaitOne();
				DeleteAttachmentResponse body = owaservice.EndDeleteAttachment(asyncResult).Body;
				if (body != null && body.ResponseMessages != null && body.ResponseMessages.Items != null && body.ResponseMessages.Items[0] != null)
				{
					result = (body.ResponseMessages.Items[0].ResponseCode == ResponseCodeType.NoError);
				}
			}
			return result;
		}

		private static bool IsHtmlAttachment(string contentType, string fileExtension)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			return contentType.ToLowerInvariant().Contains("text/html") || contentType.ToLowerInvariant().Contains("application/xhtml+xml") || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".htm") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".html") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".xhtml") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".xht") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".shtml") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".shtm") == 0 || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".stm") == 0;
		}

		private static bool IsXmlAttachment(string contentType, string fileExtension)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			return contentType.ToLowerInvariant().Contains("text/xml") || StringComparer.OrdinalIgnoreCase.Compare(fileExtension, ".xml") == 0;
		}

		private static bool IsMIMEAttributeSpecialChar(char c, bool chrome)
		{
			if (chrome && (c == '(' || c == ')'))
			{
				return false;
			}
			if (char.IsControl(c))
			{
				return true;
			}
			switch (c)
			{
			case ' ':
			case '"':
			case '%':
			case '\'':
			case '(':
			case ')':
			case '*':
			case ',':
			case '/':
				break;
			case '!':
			case '#':
			case '$':
			case '&':
			case '+':
			case '-':
			case '.':
				return false;
			default:
				switch (c)
				{
				case ':':
				case ';':
				case '<':
				case '=':
				case '>':
				case '?':
				case '@':
					break;
				default:
					switch (c)
					{
					case '[':
					case '\\':
					case ']':
						break;
					default:
						return false;
					}
					break;
				}
				break;
			}
			return true;
		}

		private static readonly AttachmentWebBeaconFilterCallback webBeaconFilter = new AttachmentWebBeaconFilterCallback();
	}
}
