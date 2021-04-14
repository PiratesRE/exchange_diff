using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Basic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal static class BodyConversionUtilities
	{
		public static Markup GetBodyFormatOfEditItem(Item item, NewItemType newItemType, UserOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			if (newItemType == NewItemType.New || newItemType == NewItemType.ImplicitDraft)
			{
				return options.ComposeMarkup;
			}
			Body body = item.Body;
			if (OwaContext.Current.UserContext.IsIrmEnabled && Utilities.IsIrmRestrictedAndDecrypted(item))
			{
				return Markup.Html;
			}
			if (body.Format == BodyFormat.TextPlain)
			{
				return Markup.PlainText;
			}
			return Markup.Html;
		}

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
				throw new ArgumentNullException("context");
			}
			UserContext userContext = owaContext.UserContext;
			WebBeaconFilterLevels filterWebBeaconsAndHtmlForms = userContext.Configuration.FilterWebBeaconsAndHtmlForms;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			JunkEmailUtilities.GetJunkEmailPropertiesForItem(item, isEmbedded, forceEnableItemLink, userContext, out flag, out flag2, out flag3, out flag4);
			OwaSafeHtmlOutboundCallbacks owaSafeHtmlOutboundCallbacks;
			if (!flag4)
			{
				if (filterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.DisableFilter || allowWebBeacon || (!Utilities.IsPublic(item) && Utilities.IsWebBeaconsAllowed(item)))
				{
					owaSafeHtmlOutboundCallbacks = new OwaSafeHtmlAllowWebBeaconCallbacks(item, userContext.IsPublicLogon, isEmbedded, attachmentUrl, owaContext, false);
				}
				else
				{
					owaSafeHtmlOutboundCallbacks = new OwaSafeHtmlOutboundCallbacks(item, userContext.IsPublicLogon, isEmbedded, attachmentUrl, false, owaContext, false);
				}
			}
			else
			{
				owaSafeHtmlOutboundCallbacks = new OwaSafeHtmlOutboundCallbacks(item, userContext.IsPublicLogon, isEmbedded, attachmentUrl, true, owaContext, false);
			}
			BodyConversionUtilities.RenderReadBody(writer, item, owaSafeHtmlOutboundCallbacks, flag4, userContext.IsIrmEnabled);
			bool flag5 = owaSafeHtmlOutboundCallbacks.HasBlockedImages || owaSafeHtmlOutboundCallbacks.HasBlockedForms;
			if (flag)
			{
				if (flag2)
				{
					infobar.AddMessage(SanitizedHtmlString.Format("{0} {1}", new object[]
					{
						LocalizedStrings.GetNonEncoded(1581910613),
						LocalizedStrings.GetNonEncoded(-2026026928)
					}), InfobarMessageType.Phishing);
				}
				else if (userContext.IsJunkEmailEnabled)
				{
					infobar.AddMessage(SanitizedHtmlString.Format("{0} {1}", new object[]
					{
						LocalizedStrings.GetNonEncoded(59853257),
						LocalizedStrings.GetNonEncoded(-2129859766)
					}), InfobarMessageType.JunkEmail);
				}
			}
			else if (flag2 && !flag3)
			{
				string s = "<a id=\"aIbBlk\" href=\"#\"" + SanitizedHtmlString.Format("_sIT=\"{0}\" _sAct=\"{1}\" _iSt=\"{2}\" _fPhsh=1 _fAWB={3}", new object[]
				{
					itemType,
					action,
					state,
					allowWebBeacon ? 1 : 0
				}) + string.Format(CultureInfo.InvariantCulture, ">{0}</a> {1} ", new object[]
				{
					LocalizedStrings.GetHtmlEncoded(-672110188),
					LocalizedStrings.GetHtmlEncoded(-1020475744)
				});
				string s2 = string.Format(CultureInfo.InvariantCulture, "<a href=\"#\" " + Utilities.GetScriptHandler("onclick", "opnHlp(\"" + Utilities.JavascriptEncode(Utilities.BuildEhcHref(HelpIdsLight.EmailSafetyLight.ToString())) + "\");") + ">{0}</a>", new object[]
				{
					LocalizedStrings.GetHtmlEncoded(338562664)
				});
				infobar.AddMessage(SanitizedHtmlString.Format("{0}{1}{2}", new object[]
				{
					SanitizedHtmlString.FromStringId(1581910613),
					SanitizedHtmlString.GetSanitizedStringWithoutEncoding(s),
					SanitizedHtmlString.GetSanitizedStringWithoutEncoding(s2)
				}), InfobarMessageType.Phishing);
			}
			else if (flag5)
			{
				infobar.AddMessage(BodyConversionUtilities.GetWebBeaconBlockedInfobarMessage(filterWebBeaconsAndHtmlForms, SanitizedHtmlString.Format("_sIT=\"{0}\" _sAct=\"{1}\" _iSt=\"{2}\" _fPhsh={3} _fAWB=1", new object[]
				{
					itemType,
					action,
					state,
					forceEnableItemLink ? 1 : 0
				}).ToString()), InfobarMessageType.Informational);
			}
			if (owaSafeHtmlOutboundCallbacks.HasRtfEmbeddedImages)
			{
				infobar.AddMessage(1338319428, InfobarMessageType.Informational);
			}
			return owaSafeHtmlOutboundCallbacks.AttachmentLinks;
		}

		public static SanitizedHtmlString GetWebBeaconBlockedInfobarMessage(WebBeaconFilterLevels level)
		{
			return BodyConversionUtilities.GetWebBeaconBlockedInfobarMessage(level, string.Empty);
		}

		private static SanitizedHtmlString GetWebBeaconBlockedInfobarMessage(WebBeaconFilterLevels level, string additionalAttribute)
		{
			if (additionalAttribute == null)
			{
				throw new ArgumentNullException("item");
			}
			if (level == WebBeaconFilterLevels.UserFilterChoice)
			{
				string s = string.Format("<a id=\"aIbBlk\" href=\"#\" {0}", additionalAttribute) + string.Format(CultureInfo.InvariantCulture, ">{0}</a>", new object[]
				{
					LocalizedStrings.GetHtmlEncoded(469213884)
				});
				return SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(2063285740), new object[]
				{
					SanitizedHtmlString.GetSanitizedStringWithoutEncoding(s)
				});
			}
			if (level == WebBeaconFilterLevels.ForceFilter)
			{
				return SanitizedHtmlString.FromStringId(-1196115124);
			}
			return SanitizedHtmlString.Empty;
		}

		public static bool GenerateEditableMessageBodyAndRenderInfobarMessages(Item item, TextWriter writer, NewItemType newItemType, OwaContext owaContext, ref bool shouldPromptUserOnFormLoad, ref bool hasInlineImages, Infobar infobar, bool allowWebBeacon, Markup markup)
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
				throw new ArgumentNullException("context");
			}
			UserContext userContext = owaContext.UserContext;
			OwaSafeHtmlOutboundCallbacks owaSafeHtmlOutboundCallbacks = null;
			WebBeaconFilterLevels filterWebBeaconsAndHtmlForms = userContext.Configuration.FilterWebBeaconsAndHtmlForms;
			bool flag = false;
			if (item != null)
			{
				flag = JunkEmailUtilities.IsJunkOrPhishing(item, false, userContext);
				if (!flag)
				{
					if (filterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.DisableFilter || allowWebBeacon || (!Utilities.IsPublic(item) && Utilities.IsWebBeaconsAllowed(item)))
					{
						owaSafeHtmlOutboundCallbacks = new OwaSafeHtmlAllowWebBeaconCallbacks(item, userContext.IsPublicLogon, owaContext, true);
					}
					else if (filterWebBeaconsAndHtmlForms != WebBeaconFilterLevels.DisableFilter && (newItemType == NewItemType.Reply || newItemType == NewItemType.Forward || newItemType == NewItemType.ExplicitDraft || newItemType == NewItemType.PostReply) && !allowWebBeacon)
					{
						owaSafeHtmlOutboundCallbacks = new OwaSafeHtmlRemoveWebBeaconCallbacks(item, userContext.IsPublicLogon, owaContext, true);
					}
					else
					{
						owaSafeHtmlOutboundCallbacks = new OwaSafeHtmlOutboundCallbacks(item, userContext.IsPublicLogon, false, owaContext, true);
					}
				}
				else
				{
					owaSafeHtmlOutboundCallbacks = new OwaSafeHtmlOutboundCallbacks(item, userContext.IsPublicLogon, true, owaContext, true);
				}
			}
			if (flag)
			{
				markup = Markup.PlainText;
			}
			BodyConversionUtilities.RenderComposeBody(writer, item, owaSafeHtmlOutboundCallbacks, userContext, markup);
			if (owaSafeHtmlOutboundCallbacks != null)
			{
				hasInlineImages = owaSafeHtmlOutboundCallbacks.HasInlineImages;
			}
			if (item != null)
			{
				if (!flag && (owaSafeHtmlOutboundCallbacks.HasBlockedImages || owaSafeHtmlOutboundCallbacks.HasBlockedForms))
				{
					if (filterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.UserFilterChoice)
					{
						shouldPromptUserOnFormLoad = true;
					}
					else if (filterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.ForceFilter)
					{
						infobar.AddMessage(SanitizedHtmlString.FromStringId(-1196115124), InfobarMessageType.Informational, "divIbImg");
					}
				}
				if (owaSafeHtmlOutboundCallbacks.HasRtfEmbeddedImages)
				{
					infobar.AddMessage(1338319428, InfobarMessageType.Informational);
				}
				if (!ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(item.ClassName))
				{
					return owaSafeHtmlOutboundCallbacks.ApplyAttachmentsUpdates(item);
				}
			}
			return false;
		}

		public static void GeneratePrintMessageBody(Item item, TextWriter writer, OwaContext owaContext, bool isEmbeddedItem, string embeddedItemUrl, bool forceAllowWebBeacon, bool forceEnableItemLink)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("context");
			}
			UserContext userContext = owaContext.UserContext;
			WebBeaconFilterLevels filterWebBeaconsAndHtmlForms = userContext.Configuration.FilterWebBeaconsAndHtmlForms;
			bool flag = JunkEmailUtilities.IsJunkOrPhishing(item, isEmbeddedItem, forceEnableItemLink, userContext);
			OwaSafeHtmlCallbackBase callBack;
			if (!flag)
			{
				if (filterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.DisableFilter || forceAllowWebBeacon || (!Utilities.IsPublic(item) && Utilities.IsWebBeaconsAllowed(item)))
				{
					callBack = new OwaSafeHtmlAllowWebBeaconCallbacks(item, userContext.IsPublicLogon, isEmbeddedItem, embeddedItemUrl, owaContext, false);
				}
				else
				{
					callBack = new OwaSafeHtmlOutboundCallbacks(item, userContext.IsPublicLogon, isEmbeddedItem, embeddedItemUrl, false, owaContext, false);
				}
			}
			else
			{
				callBack = new OwaSafeHtmlOutboundCallbacks(item, userContext.IsPublicLogon, isEmbeddedItem, embeddedItemUrl, true, owaContext, false);
			}
			BodyConversionUtilities.RenderReadBody(writer, item, callBack, flag);
		}

		public static BodyCharsetFlags GetBodyCharsetOptions(UserContext userContext, out string charsetName)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			BodyCharsetFlags bodyCharsetFlags = BodyCharsetFlags.None;
			if (userContext.UseGB18030)
			{
				bodyCharsetFlags |= BodyCharsetFlags.PreferGB18030;
			}
			if (userContext.UseISO885915)
			{
				bodyCharsetFlags |= BodyCharsetFlags.PreferIso885915;
			}
			if (userContext.OutboundCharset == OutboundCharsetOptions.AlwaysUTF8)
			{
				bodyCharsetFlags |= BodyCharsetFlags.DisableCharsetDetection;
				charsetName = "utf-8";
			}
			else
			{
				if (userContext.OutboundCharset == OutboundCharsetOptions.UserLanguageChoice)
				{
					bodyCharsetFlags |= BodyCharsetFlags.DisableCharsetDetection;
				}
				else
				{
					bodyCharsetFlags = bodyCharsetFlags;
				}
				CultureInfo userCulture = Microsoft.Exchange.Clients.Owa.Core.Culture.GetUserCulture();
				Microsoft.Exchange.Data.Globalization.Culture culture = null;
				if (Microsoft.Exchange.Data.Globalization.Culture.TryGetCulture(userCulture.Name, out culture))
				{
					Charset mimeCharset = culture.MimeCharset;
					if (mimeCharset.IsAvailable)
					{
						charsetName = mimeCharset.Name;
						return bodyCharsetFlags;
					}
				}
				charsetName = Microsoft.Exchange.Data.Globalization.Culture.Default.MimeCharset.Name;
			}
			return bodyCharsetFlags;
		}

		internal static void ConvertAndOutputBody(TextWriter output, Body body, Markup markup, OwaSafeHtmlCallbackBase callBack)
		{
			try
			{
				BodyReadConfiguration bodyReadConfiguration = null;
				switch (markup)
				{
				case Markup.Html:
					bodyReadConfiguration = new BodyReadConfiguration(BodyFormat.TextHtml, "utf-8");
					bodyReadConfiguration.SetHtmlOptions(HtmlStreamingFlags.FilterHtml, callBack);
					break;
				case Markup.PlainText:
					bodyReadConfiguration = new BodyReadConfiguration(BodyFormat.TextPlain, "utf-8");
					break;
				}
				if (bodyReadConfiguration != null)
				{
					using (TextReader textReader = body.OpenTextReader(bodyReadConfiguration))
					{
						BodyConversionUtilities.HtmlEncodeAndOutputBody(output, textReader);
					}
				}
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

		public static void RenderReadBody(TextWriter output, Item item, OwaSafeHtmlCallbackBase callBack, bool isJunkOrPhishing, bool isIrmEnabled)
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
			Item item2 = null;
			try
			{
				if (Utilities.IsOpaqueSigned(item))
				{
					MessageItem messageItem = item as MessageItem;
					if (messageItem != null && ItemConversion.TryOpenSMimeContent(messageItem, OwaContext.Current.UserContext.Configuration.DefaultAcceptedDomain.DomainName.ToString(), out item2))
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
							goto IL_E2;
						}
					}
					return;
				}
				IL_E2:
				Body body = item.Body;
				if (isIrmEnabled && Utilities.IsIrmRestrictedAndDecrypted(item))
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
					BodyConversionUtilities.ConvertAndOutputBody(output, body, markup, callBack);
				}
			}
			finally
			{
				if (item2 != null)
				{
					item2.Dispose();
					item2 = null;
				}
			}
		}

		public static bool RenderMeetingPlainTextBody(TextWriter output, Item item, UserContext userContext, bool doSignature)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (item == null)
			{
				return false;
			}
			if (item.Body == null)
			{
				return false;
			}
			if (item.Body.Size > 0L)
			{
				BodyConversionUtilities.ConvertAndOutputBody(output, item.Body, Markup.PlainText, null);
				return true;
			}
			if (doSignature && userContext.UserOptions.AutoAddSignature && userContext.IsFeatureEnabled(Feature.Signature))
			{
				output.Write("\n\n\n");
				Utilities.HtmlEncode(userContext.UserOptions.SignatureText, output);
			}
			return false;
		}

		private static void RenderComposeBody(TextWriter output, Item item, OwaSafeHtmlCallbackBase callBack, UserContext userContext, Markup markup)
		{
			if (item == null)
			{
				return;
			}
			Body body = item.Body;
			if (OwaContext.Current.UserContext.IsIrmEnabled && Utilities.IsIrmRestrictedAndDecrypted(item))
			{
				body = ((RightsManagedMessageItem)item).ProtectedBody;
			}
			if (body != null && body.Size > 0L)
			{
				BodyConversionUtilities.ConvertAndOutputBody(output, body, markup, callBack);
			}
		}

		private static void HtmlEncodeAndOutputBody(TextWriter output, TextReader bodyReader)
		{
			Utilities.SanitizeHtmlEncode(bodyReader.ReadToEnd(), output);
		}

		private static void HtmlEncodeAndOutputBody(TextWriter output, Stream bodyStream)
		{
			HttpWriter httpWriter = output as HttpWriter;
			Stream outputStream = httpWriter.OutputStream;
			byte[] array = new byte[8192];
			int num;
			do
			{
				num = bodyStream.Read(array, 0, array.Length);
				int num2 = 0;
				int i;
				for (i = 0; i < num; i++)
				{
					byte[] array2 = null;
					byte b = array[i];
					if (b != 34)
					{
						if (b != 38)
						{
							switch (b)
							{
							case 60:
								array2 = BodyConversionUtilities.lessThanHtmlEncodedReplacement;
								break;
							case 62:
								array2 = BodyConversionUtilities.greaterThanHtmlEncodedReplacement;
								break;
							}
						}
						else
						{
							array2 = BodyConversionUtilities.ampersandHtmlEncodedReplacement;
						}
					}
					else
					{
						array2 = BodyConversionUtilities.quoteHtmlEncodedReplacement;
					}
					if (array2 != null)
					{
						outputStream.Write(array, num2, i - num2);
						num2 = i + 1;
						outputStream.Write(array2, 0, array2.Length);
					}
				}
				if (num > 0 && num2 < i)
				{
					outputStream.Write(array, num2, i - num2);
				}
				outputStream.Flush();
			}
			while (num > 0);
		}

		public static string GetEmptyBodyHtml(int userFontSize, string userFontName)
		{
			return string.Format("<div><font face=\"{0}\" size=\"{1}\">&nbsp;</font></div>", Utilities.HtmlEncode(userFontName), userFontSize);
		}

		internal static void SetBody(Item item, string body, Markup markup, UserContext userContext)
		{
			BodyConversionUtilities.SetBody(item, body, markup, StoreObjectType.Unknown, userContext);
		}

		internal static bool SetBody(Item item, string body, Markup markup, StoreObjectType storeObjectType, UserContext userContext)
		{
			bool flag = false;
			switch (markup)
			{
			case Markup.Html:
			{
				string targetCharsetName;
				BodyCharsetFlags bodyCharsetOptions = BodyConversionUtilities.GetBodyCharsetOptions(userContext, out targetCharsetName);
				BodyFormat targetFormat = (storeObjectType != StoreObjectType.CalendarItem && storeObjectType != StoreObjectType.MeetingMessage) ? BodyFormat.TextHtml : BodyFormat.ApplicationRtf;
				BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(BodyFormat.TextHtml);
				bodyWriteConfiguration.SetTargetFormat(targetFormat, targetCharsetName, bodyCharsetOptions);
				OwaSafeHtmlInboundCallbacks owaSafeHtmlInboundCallbacks = null;
				owaSafeHtmlInboundCallbacks = new OwaSafeHtmlInboundCallbacks(item, userContext);
				bodyWriteConfiguration.SetHtmlOptions(HtmlStreamingFlags.None, owaSafeHtmlInboundCallbacks);
				try
				{
					Body body2 = item.Body;
					if (userContext.IsIrmEnabled && Utilities.IsIrmDecrypted(item))
					{
						body2 = ((RightsManagedMessageItem)item).ProtectedBody;
					}
					using (TextWriter textWriter = body2.OpenTextWriter(bodyWriteConfiguration))
					{
						textWriter.Write(body);
					}
				}
				catch (InvalidCharsetException innerException)
				{
					throw new OwaEventHandlerException(LocalizedStrings.GetNonEncoded(1825027020), LocalizedStrings.GetNonEncoded(1825027020), innerException);
				}
				catch (TextConvertersException innerException2)
				{
					throw new OwaEventHandlerException(LocalizedStrings.GetNonEncoded(1825027020), LocalizedStrings.GetNonEncoded(1825027020), innerException2);
				}
				Utilities.MakeModifiedCalendarItemOccurrence(item);
				bool flag2 = false;
				if (owaSafeHtmlInboundCallbacks != null)
				{
					flag2 = owaSafeHtmlInboundCallbacks.AttachmentNeedsSave();
				}
				if (flag2 && !Utilities.IsClearSigned(item))
				{
					flag = AttachmentUtility.ApplyAttachmentsUpdates(item, owaSafeHtmlInboundCallbacks);
					item.Load();
				}
				break;
			}
			case Markup.PlainText:
				ItemUtility.SetItemBody(item, BodyFormat.TextPlain, body);
				flag = AttachmentUtility.PromoteInlineAttachments(item);
				if (flag)
				{
					item.Load();
				}
				break;
			}
			return flag;
		}

		private const string OutputDisplayCharset = "utf-8";

		private static readonly byte[] quoteHtmlEncodedReplacement = new byte[]
		{
			38,
			113,
			117,
			111,
			116,
			59
		};

		private static readonly byte[] ampersandHtmlEncodedReplacement = new byte[]
		{
			38,
			97,
			109,
			112,
			59
		};

		private static readonly byte[] greaterThanHtmlEncodedReplacement = new byte[]
		{
			38,
			103,
			116,
			59
		};

		private static readonly byte[] lessThanHtmlEncodedReplacement = new byte[]
		{
			38,
			108,
			116,
			59
		};
	}
}
