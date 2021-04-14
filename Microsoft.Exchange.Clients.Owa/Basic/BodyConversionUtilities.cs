using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal sealed class BodyConversionUtilities
	{
		public static IList<AttachmentLink> GenerateNonEditableMessageBodyAndRenderInfobarMessages(Item item, TextWriter writer, OwaContext owaContext, Infobar infobar, bool allowWebBeacon, bool forceEnableItemLink, string itemType, string action, string state, bool isEmbedded, string attachmentUrl)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			UserContext userContext = owaContext.UserContext;
			WebBeaconFilterLevels filterWebBeaconsAndHtmlForms = userContext.Configuration.FilterWebBeaconsAndHtmlForms;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			JunkEmailUtilities.GetJunkEmailPropertiesForItem(item, isEmbedded, forceEnableItemLink, userContext, out flag, out flag2, out flag3, out flag4);
			OwaSafeHtmlCallbackBase owaSafeHtmlCallbackBase;
			if (!flag4)
			{
				if (filterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.DisableFilter || allowWebBeacon || Utilities.IsWebBeaconsAllowed(item))
				{
					owaSafeHtmlCallbackBase = new OwaSafeHtmlAllowWebBeaconCallbacks(item, userContext.IsPublicLogon, isEmbedded, attachmentUrl, owaContext, false);
				}
				else
				{
					owaSafeHtmlCallbackBase = new OwaSafeHtmlOutboundCallbacks(item, userContext.IsPublicLogon, isEmbedded, attachmentUrl, false, owaContext, false);
				}
			}
			else
			{
				owaSafeHtmlCallbackBase = new OwaSafeHtmlOutboundCallbacks(item, userContext.IsPublicLogon, isEmbedded, attachmentUrl, true, owaContext, false);
			}
			BodyConversionUtilities.RenderReadBody(writer, item, owaSafeHtmlCallbackBase, flag4);
			bool hasBlockedImages = owaSafeHtmlCallbackBase.HasBlockedImages;
			if (flag)
			{
				if (flag2)
				{
					infobar.AddMessageText(LocalizedStrings.GetNonEncoded(1581910613) + " " + LocalizedStrings.GetNonEncoded(614784743), InfobarMessageType.Phishing);
				}
				else if (userContext.IsJunkEmailEnabled)
				{
					infobar.AddMessageText(LocalizedStrings.GetNonEncoded(59853257) + " " + LocalizedStrings.GetNonEncoded(385373859), InfobarMessageType.JunkEmail);
				}
			}
			else if (flag2 && !flag3)
			{
				string s = string.Format(CultureInfo.InvariantCulture, "<a id=\"aIbBlk\" href=\"#\" onclick=\"return onClkBm('{0}', 1, 0)\">{1}</a> {2} ", new object[]
				{
					itemType,
					LocalizedStrings.GetHtmlEncoded(-672110188),
					LocalizedStrings.GetHtmlEncoded(-1020475744)
				});
				string format = "<a href=\"#\" onClick=opnHlp('" + Utilities.JavascriptEncode(Utilities.BuildEhcHref(HelpIdsLight.EmailSafetyLight.ToString())) + "')>{0}</a>";
				string s2 = string.Format(CultureInfo.InvariantCulture, format, new object[]
				{
					LocalizedStrings.GetHtmlEncoded(338562664)
				});
				infobar.AddMessageHtml(SanitizedHtmlString.Format("{0}{1}{2}", new object[]
				{
					LocalizedStrings.GetNonEncoded(1581910613),
					SanitizedHtmlString.GetSanitizedStringWithoutEncoding(s),
					SanitizedHtmlString.GetSanitizedStringWithoutEncoding(s2)
				}), InfobarMessageType.Phishing);
			}
			else if (hasBlockedImages)
			{
				if (filterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.UserFilterChoice)
				{
					string s3 = string.Format(CultureInfo.InvariantCulture, "<a id=\"aIbBlk\" href=\"#\" onclick=\"return onClkBm('{0}', 1, 1);\">{1}</a>", new object[]
					{
						itemType,
						LocalizedStrings.GetHtmlEncoded(469213884)
					});
					infobar.AddMessageHtml(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(2063285740), new object[]
					{
						SanitizedHtmlString.GetSanitizedStringWithoutEncoding(s3)
					}), InfobarMessageType.Informational);
				}
				else if (filterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.ForceFilter)
				{
					infobar.AddMessageLocalized(-1196115124, InfobarMessageType.Informational);
				}
			}
			if (owaSafeHtmlCallbackBase.HasRtfEmbeddedImages)
			{
				infobar.AddMessageLocalized(1338319428, InfobarMessageType.Informational);
			}
			return owaSafeHtmlCallbackBase.AttachmentLinks;
		}

		public static void RenderReadBody(TextWriter output, Item item, OwaSafeHtmlCallbackBase callBack, bool isJunkOrPhishing)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (item == null)
			{
				return;
			}
			ReportMessage reportMessage = item as ReportMessage;
			bool flag = null != reportMessage;
			if (Utilities.IsOpaqueSigned(item))
			{
				MessageItem messageItem = item as MessageItem;
				Item item2 = null;
				if (messageItem != null && ItemConversion.TryOpenSMimeContent(messageItem, OwaContext.Current.UserContext.Configuration.DefaultAcceptedDomain.Name, out item2))
				{
					item = item2;
				}
			}
			if (item.Body == null || (flag && item.Body.Size <= 0L))
			{
				if (flag)
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						Charset charset;
						reportMessage.GenerateReportBody(memoryStream, out charset);
						item.OpenAsReadWrite();
						BodyWriteConfiguration configuration = new BodyWriteConfiguration(BodyFormat.TextHtml, charset.Name);
						using (Stream stream = item.Body.OpenWriteStream(configuration))
						{
							memoryStream.Position = 0L;
							memoryStream.WriteTo(stream);
						}
						goto IL_DC;
					}
				}
				return;
			}
			IL_DC:
			Body body = item.Body;
			if (!OwaContext.Current.UserContext.IsBasicExperience && OwaContext.Current.UserContext.IsIrmEnabled && Utilities.IsIrmRestrictedAndDecrypted(item))
			{
				body = ((RightsManagedMessageItem)item).ProtectedBody;
			}
			if (body.Size > 0L)
			{
				Markup markup = Markup.Html;
				if (isJunkOrPhishing)
				{
					markup = Markup.PlainText;
				}
				BodyConversionUtilities.ConvertAndOutputBody(output, body, markup, callBack, false);
			}
		}

		public static void GenerateEditableMessageBodyAndRenderInfobarMessages(Item item, TextWriter writer, OwaContext owaContext, Infobar infobar)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (infobar == null)
			{
				throw new ArgumentNullException("infobar");
			}
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			UserContext userContext = owaContext.UserContext;
			BodyConversionUtilities.RenderComposeBody(writer, item, userContext);
		}

		public static void ConvertAndOutputBody(TextWriter output, Body body, Markup markup, OwaSafeHtmlCallbackBase callBack, bool isComposeBody)
		{
			string bodyString = string.Empty;
			BodyReadConfiguration bodyReadConfiguration = null;
			try
			{
				switch (markup)
				{
				case Markup.Html:
					bodyReadConfiguration = new BodyReadConfiguration(BodyFormat.TextHtml);
					bodyReadConfiguration.SetHtmlOptions(HtmlStreamingFlags.FilterHtml | HtmlStreamingFlags.Fragment, callBack);
					break;
				case Markup.PlainText:
					bodyReadConfiguration = new BodyReadConfiguration(BodyFormat.TextPlain);
					break;
				}
				if (bodyReadConfiguration != null)
				{
					using (TextReader textReader = body.OpenTextReader(bodyReadConfiguration))
					{
						bodyString = textReader.ReadToEnd();
					}
				}
				BodyConversionUtilities.RenderBodyContent(output, bodyString, markup, isComposeBody);
			}
			catch (InvalidCharsetException innerException)
			{
				throw new OwaBodyConversionFailedException("Body Conversion Failed", innerException);
			}
			catch (ConversionFailedException innerException2)
			{
				throw new OwaBodyConversionFailedException("Body Conversion Failed", innerException2);
			}
		}

		public static string ConvertTextToHtml(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			string @string;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					try
					{
						new TextToHtml
						{
							InputEncoding = Encoding.UTF8,
							OutputEncoding = Encoding.UTF8,
							HtmlTagCallback = new HtmlTagCallback(BodyConversionUtilities.RemoveLinkCallback),
							Header = null,
							Footer = null,
							OutputHtmlFragment = true
						}.Convert(memoryStream, memoryStream2);
					}
					catch (InvalidCharsetException innerException)
					{
						throw new OwaBodyConversionFailedException("Convert to Html Failed", innerException);
					}
					catch (TextConvertersException innerException2)
					{
						throw new OwaBodyConversionFailedException("Convert to Html Failed", innerException2);
					}
					@string = Encoding.UTF8.GetString(memoryStream2.ToArray());
				}
			}
			return @string;
		}

		private static void RenderBodyContent(TextWriter output, string bodyString, Markup markup, bool isComposeBody)
		{
			if (markup == Markup.Html)
			{
				output.Write(bodyString);
				return;
			}
			if (isComposeBody)
			{
				Utilities.HtmlEncode(bodyString, output);
				return;
			}
			output.Write(BodyConversionUtilities.ConvertTextToHtml(bodyString));
		}

		private static void RenderComposeBody(TextWriter output, Item item, UserContext userContext)
		{
			bool flag = true;
			if (item != null && item.Body != null && item.Body.Size > 0L)
			{
				flag = false;
				BodyConversionUtilities.ConvertAndOutputBody(output, item.Body, Markup.PlainText, null, true);
			}
			if (flag && userContext.IsFeatureEnabled(Feature.Signature) && userContext.UserOptions.AutoAddSignature && !(item is CalendarItemBase))
			{
				output.Write("\n\n\n");
				Utilities.HtmlEncode(userContext.UserOptions.SignatureText, output);
			}
		}

		internal static void RemoveLinkCallback(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			if (tagContext.TagId == HtmlTagId.A)
			{
				tagContext.DeleteTag();
				return;
			}
			tagContext.WriteTag();
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				htmlTagContextAttribute.Write();
			}
		}

		private const string OutputDisplayCharset = "utf-8";
	}
}
