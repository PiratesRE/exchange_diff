using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.RightsManagement.Protectors;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public static class RenderingUtilities
	{
		public static void RenderButton(TextWriter output, string buttonId, string attributes, string onClick, string innerHtml)
		{
			RenderingUtilities.RenderButton(output, buttonId, attributes, onClick, innerHtml, false);
		}

		public static void RenderButton(TextWriter output, string buttonId, string attributes, string onClick, string innerHtml, bool isDisabled)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (buttonId == null)
			{
				throw new ArgumentNullException("buttonId");
			}
			if (attributes == null)
			{
				throw new ArgumentNullException("attributes");
			}
			if (onClick == null)
			{
				throw new ArgumentNullException("onClick");
			}
			if (innerHtml == null)
			{
				throw new ArgumentNullException("innerHtml");
			}
			output.Write("<button");
			if (isDisabled)
			{
				output.Write(" disabled");
			}
			output.Write(" id=\"");
			output.Write(buttonId);
			output.Write("\" hidefocus=true ");
			Utilities.RenderScriptHandler(output, "onmouseover", "btnOnMsOvrBtn(_this);");
			output.Write(" ");
			Utilities.RenderScriptHandler(output, "onmouseout", "btnOnMsOutBtn(_this);");
			if (!string.IsNullOrEmpty(onClick))
			{
				output.Write(" ");
				Utilities.RenderScriptHandler(output, "onclick", onClick);
			}
			output.Write(" ");
			output.Write(attributes);
			output.Write(">");
			output.Write(innerHtml);
			output.Write("</button>");
		}

		internal static void RenderSender(UserContext userContext, TextWriter output, CalendarItemBase calendarItemBase)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (calendarItemBase == null)
			{
				throw new ArgumentNullException("calendarItemBase");
			}
			RenderingUtilities.RenderSender(userContext, output, calendarItemBase.Organizer, SenderDisplayMode.DefaultDisplay, null);
		}

		internal static void RenderSender(UserContext userContext, TextWriter output, MessageItem item, RenderSubHeaderDelegate renderSubHeader)
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
				throw new ArgumentNullException("item");
			}
			Participant sender = item.Sender;
			Participant from = item.From;
			if (Utilities.IsOnBehalfOf(sender, from))
			{
				SanitizedHtmlString sender2 = RenderingUtilities.GetSender(userContext, item, "spnSender", from, renderSubHeader);
				SanitizedHtmlString sender3 = RenderingUtilities.GetSender(userContext, from, "spnFrom", SenderDisplayMode.NoPhoto, null);
				output.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-165544498), new object[]
				{
					sender2,
					sender3
				}));
				return;
			}
			SanitizedHtmlString sender4 = RenderingUtilities.GetSender(userContext, item, "spnFrom", null, renderSubHeader);
			output.Write(sender4);
		}

		internal static void RenderSender(UserContext userContext, TextWriter output, PostItem item)
		{
			RenderingUtilities.RenderSender(userContext, output, item.Sender, item.From, null);
		}

		internal static void RenderSender(UserContext userContext, TextWriter output, Participant sender, RenderSubHeaderDelegate renderSubHeader)
		{
			RenderingUtilities.RenderSender(userContext, output, sender, SenderDisplayMode.DefaultDisplay, renderSubHeader);
		}

		internal static void RenderSender(UserContext userContext, TextWriter output, Participant sender, SenderDisplayMode senderDisplayMode, RenderSubHeaderDelegate renderSubHeader)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write(RenderingUtilities.GetSender(userContext, sender, "spnFrom", senderDisplayMode, renderSubHeader));
		}

		internal static void RenderSender(UserContext userContext, TextWriter output, Participant sender, Participant from, RenderSubHeaderDelegate renderSubHeader)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (sender == null)
			{
				throw new ArgumentNullException("sender");
			}
			if (from == null)
			{
				throw new ArgumentNullException("from");
			}
			if (Utilities.IsOnBehalfOf(sender, from))
			{
				output.Write(LocalizedStrings.GetHtmlEncoded(-165544498), RenderingUtilities.GetSender(userContext, sender, "spnSender", from, renderSubHeader), RenderingUtilities.GetSender(userContext, from, "spnFrom", SenderDisplayMode.NoPhoto, null));
				return;
			}
			output.Write(RenderingUtilities.GetSender(userContext, sender, "spnFrom", from, renderSubHeader));
		}

		public static void RenderSentTime(TextWriter output, ExDateTime sentTime, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			RenderingUtilities.RenderSentTime(sanitizingStringBuilder, sentTime, userContext);
			output.Write(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>());
		}

		public static void RenderSentTime(SanitizingStringBuilder<OwaHtml> stringBuilder, ExDateTime sentTime, UserContext userContext)
		{
			if (stringBuilder == null)
			{
				throw new ArgumentNullException("stringBuilder");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			stringBuilder.Append(string.Format(LocalizedStrings.GetHtmlEncoded(-1617047463), sentTime.ToString(DateTimeFormatInfo.CurrentInfo.LongDatePattern), sentTime.ToString(userContext.UserOptions.TimeFormat)));
		}

		internal static void RenderSubject(TextWriter output, Item item)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			RenderingUtilities.RenderSubject(output, item, string.Empty);
		}

		internal static void RenderSubject(TextWriter output, Item item, string untitled)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			string subject = (item == null) ? string.Empty : (item.TryGetProperty(ItemSchema.Subject) as string);
			RenderingUtilities.RenderSubject(output, subject, untitled);
		}

		internal static void RenderSubject(TextWriter output, string subject, string untitled)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write(RenderingUtilities.GetSubject(subject, untitled));
		}

		internal static SanitizedHtmlString GetSubject(Item item, string untitled)
		{
			string subject = (item == null) ? string.Empty : (item.TryGetProperty(ItemSchema.Subject) as string);
			return RenderingUtilities.GetSubject(subject, untitled);
		}

		internal static SanitizedHtmlString GetSubject(string subject, string untitled)
		{
			if (!string.IsNullOrEmpty(subject))
			{
				if (subject.Length <= 255)
				{
					return new SanitizedHtmlString(subject);
				}
				return new SanitizedHtmlString(subject.Substring(0, 255));
			}
			else
			{
				if (!string.IsNullOrEmpty(untitled))
				{
					return new SanitizedHtmlString(untitled);
				}
				return SanitizedHtmlString.Empty;
			}
		}

		internal static SanitizedHtmlString GetSender(UserContext userContext, MessageItem item, string id, Participant onBehalfOfSender, RenderSubHeaderDelegate renderSubHeader)
		{
			string senderSmtpAddress = null;
			string senderSipUri = null;
			bool? senderIsDl = null;
			string senderADObjectId = null;
			Participant sender = item.Sender;
			RenderingUtilities.GetSipUriFromMessageItem(item, sender, ref senderSipUri);
			return RenderingUtilities.GetSender(userContext, sender, id, null, true, onBehalfOfSender, true, senderADObjectId, senderSmtpAddress, senderSipUri, null, senderIsDl, SenderDisplayMode.DefaultDisplay, renderSubHeader);
		}

		internal static SanitizedHtmlString GetSender(UserContext userContext, Participant sender, string id, SenderDisplayMode senderDisplayMode, RenderSubHeaderDelegate renderSubHeader)
		{
			return RenderingUtilities.GetSender(userContext, sender, id, null, true, null, true, null, null, null, null, null, senderDisplayMode, renderSubHeader);
		}

		internal static SanitizedHtmlString GetSender(UserContext userContext, Participant sender, string id, RenderSubHeaderDelegate renderSubHeader)
		{
			return RenderingUtilities.GetSender(userContext, sender, id, null, true, null, true, null, null, null, null, null, SenderDisplayMode.DefaultDisplay, renderSubHeader);
		}

		internal static SanitizedHtmlString GetSender(UserContext userContext, Participant sender, string id, bool hasJunkEmailContextMenu, SenderDisplayMode senderDisplayMode)
		{
			return RenderingUtilities.GetSender(userContext, sender, id, null, true, null, hasJunkEmailContextMenu, null, null, null, null, null, senderDisplayMode, null);
		}

		internal static SanitizedHtmlString GetSender(UserContext userContext, Participant sender, string id, Participant onBehalfOfSender, RenderSubHeaderDelegate renderSubHeader)
		{
			return RenderingUtilities.GetSender(userContext, sender, id, null, true, onBehalfOfSender, true, null, null, null, null, null, SenderDisplayMode.DefaultDisplay, renderSubHeader);
		}

		internal static SanitizedHtmlString GetSender(UserContext userContext, Participant sender, string id, string displayName, bool hasContextMenu, SenderDisplayMode senderDisplayMode)
		{
			return RenderingUtilities.GetSender(userContext, sender, id, displayName, hasContextMenu, null, true, null, null, null, null, null, senderDisplayMode, null);
		}

		internal static SanitizedHtmlString GetSender(UserContext userContext, Participant sender, string id, string displayName, string mobilePhoneNumber, bool hasContextMenu, SenderDisplayMode senderDisplayMode)
		{
			return RenderingUtilities.GetSender(userContext, sender, id, displayName, hasContextMenu, null, true, null, null, null, mobilePhoneNumber, null, senderDisplayMode, null);
		}

		internal static SanitizedHtmlString GetSender(UserContext userContext, Participant sender, string id, string displayName, bool hasContextMenu, Participant from, bool hasJunkEmailContextMenu, string senderADObjectId, string senderSmtpAddress, string senderSipUri, string senderMobilePhoneNumber, bool? senderIsDl, SenderDisplayMode senderDisplayMode, RenderSubHeaderDelegate renderSubHeader)
		{
			if (sender == null)
			{
				return SanitizedHtmlString.Empty;
			}
			string text = null;
			string text2 = null;
			string text3 = null;
			bool? flag = null;
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append("<span id=\"");
			if (!string.IsNullOrEmpty(id))
			{
				sanitizingStringBuilder.Append(id);
			}
			if (senderSipUri == null && from != null && !string.IsNullOrEmpty(from.DisplayName) && !Utilities.IsOnBehalfOf(sender, from))
			{
				RenderingUtilities.GetSipUriFromParticipant(from, ref senderSipUri);
			}
			string routingType = null;
			string text4 = null;
			if (string.CompareOrdinal(sender.RoutingType, "SMTP") == 0 && ImceaAddress.IsImceaAddress(sender.EmailAddress) && Utilities.TryDecodeImceaAddress(sender.EmailAddress, ref routingType, ref text4))
			{
				sender = new Participant(sender.DisplayName, text4, routingType);
				string emailAddress;
				if (Utilities.IsMobileRoutingType(routingType) && !string.IsNullOrEmpty(emailAddress = Utilities.NormalizePhoneNumber(text4)))
				{
					sender = new Participant(sender.DisplayName, emailAddress, "MOBILE");
					AnrManager.Options options = new AnrManager.Options();
					options.DefaultRoutingType = "MOBILE";
					RecipientAddress recipientAddress = AnrManager.ResolveAnrString(text4, options, userContext);
					if (recipientAddress != null && recipientAddress.AddressOrigin != AddressOrigin.OneOff && !Utilities.CreateExchangeParticipant(out sender, recipientAddress))
					{
						if (recipientAddress.ADObjectId != null)
						{
							senderADObjectId = Utilities.GetBase64StringFromADObjectId(recipientAddress.ADObjectId);
						}
						else if (recipientAddress.StoreObjectId != null)
						{
							senderADObjectId = recipientAddress.StoreObjectId.ToBase64String();
						}
						senderSipUri = recipientAddress.SipUri;
						senderIsDl = new bool?(recipientAddress.IsDistributionList);
						senderMobilePhoneNumber = recipientAddress.MobilePhoneNumber;
					}
				}
			}
			else if (senderSmtpAddress == null || senderADObjectId == null || senderSipUri == null || senderIsDl == null || senderMobilePhoneNumber == null)
			{
				RenderingUtilities.GetSenderSmtpAddress(userContext, sender, ref senderSmtpAddress, ref senderADObjectId, ref senderSipUri, ref senderMobilePhoneNumber, ref senderIsDl, false);
			}
			AddressOrigin addressOrigin = RecipientAddress.ToAddressOrigin(sender);
			if (hasContextMenu)
			{
				sanitizingStringBuilder.Append("\" class=\"rwRRO\" ");
				sanitizingStringBuilder.Append<SanitizedEventHandlerString>(new SanitizedEventHandlerString("ondblclick", "onDblClkReadRcp(event);"));
				sanitizingStringBuilder.Append(" ");
				sanitizingStringBuilder.Append<SanitizedEventHandlerString>(new SanitizedEventHandlerString("onclick", "onClkRcp(event);"));
				sanitizingStringBuilder.Append(" ");
				sanitizingStringBuilder.Append<SanitizedEventHandlerString>(new SanitizedEventHandlerString("oncontextmenu", "onRwCm(event);"));
				sanitizingStringBuilder.Append(" _fJnk=");
				if (hasJunkEmailContextMenu)
				{
					sanitizingStringBuilder.Append("1");
				}
				else
				{
					sanitizingStringBuilder.Append("0");
				}
				if (!userContext.IsFeatureEnabled(Feature.GlobalAddressList) && !string.IsNullOrEmpty(senderSmtpAddress))
				{
					sanitizingStringBuilder.Append(" _sa=\"");
					sanitizingStringBuilder.Append(senderSmtpAddress);
					sanitizingStringBuilder.Append("\"");
				}
				if (!string.IsNullOrEmpty(sender.DisplayName))
				{
					sanitizingStringBuilder.Append(" _dn=\"");
					sanitizingStringBuilder.Append(sender.DisplayName);
					sanitizingStringBuilder.Append("\"");
				}
				if (!string.IsNullOrEmpty(sender.RoutingType))
				{
					sanitizingStringBuilder.Append(" _rt=\"");
					sanitizingStringBuilder.Append(sender.RoutingType);
					sanitizingStringBuilder.Append("\"");
				}
				if (!string.IsNullOrEmpty(sender.EmailAddress))
				{
					sanitizingStringBuilder.Append(" _em=\"");
					sanitizingStringBuilder.Append(sender.EmailAddress);
					sanitizingStringBuilder.Append("\"");
				}
				sanitizingStringBuilder.Append(" _ao=\"");
				sanitizingStringBuilder.Append<int>((int)addressOrigin);
				sanitizingStringBuilder.Append("\"");
				if (senderIsDl.GetValueOrDefault())
				{
					sanitizingStringBuilder.Append(" _rf=\"");
					sanitizingStringBuilder.Append<int>(1);
					sanitizingStringBuilder.Append("\"");
				}
				if (senderADObjectId != null)
				{
					sanitizingStringBuilder.Append(" _id=\"");
					sanitizingStringBuilder.Append(senderADObjectId);
					sanitizingStringBuilder.Append("\"");
				}
				if (Utilities.IsOnBehalfOf(sender, from))
				{
					RenderingUtilities.GetSenderSmtpAddress(userContext, from, ref text, ref text3, ref text2, ref senderMobilePhoneNumber, ref flag, false);
				}
				if (userContext.IsInstantMessageEnabled() && userContext.InstantMessagingType == InstantMessagingTypeOptions.Ocs && !string.IsNullOrEmpty(senderSipUri))
				{
					if (senderADObjectId == null)
					{
						sanitizingStringBuilder.Append(" _sipTrsp=1 ");
					}
					sanitizingStringBuilder.Append(" _uri=\"");
					sanitizingStringBuilder.Append(senderSipUri);
					sanitizingStringBuilder.Append("\"");
				}
				if (userContext.IsSmsEnabled && !string.IsNullOrEmpty(senderMobilePhoneNumber))
				{
					sanitizingStringBuilder.Append(" _mo=\"");
					sanitizingStringBuilder.Append(senderMobilePhoneNumber);
					sanitizingStringBuilder.Append("\"");
				}
			}
			else
			{
				sanitizingStringBuilder.Append(" class=\"rwRRO\"");
			}
			if (senderSmtpAddress != null)
			{
				sanitizingStringBuilder.Append(" title=\"");
				string str;
				if (text == null)
				{
					str = senderSmtpAddress;
				}
				else
				{
					str = string.Format(userContext.UserCulture, LocalizedStrings.GetNonEncoded(-165544498), new object[]
					{
						senderSmtpAddress,
						text
					});
				}
				sanitizingStringBuilder.Append(str);
				sanitizingStringBuilder.Append("\"");
			}
			sanitizingStringBuilder.Append(">");
			bool flag2 = senderDisplayMode == SenderDisplayMode.DefaultDisplay && userContext.IsSenderPhotosFeatureEnabled(Feature.DisplayPhotos);
			if (senderDisplayMode != SenderDisplayMode.NameOnly && userContext.IsInstantMessageEnabled() && !string.IsNullOrEmpty(sender.RoutingType) && ((string.CompareOrdinal(sender.RoutingType, "EX") == 0 && !senderIsDl.GetValueOrDefault()) || string.CompareOrdinal(sender.RoutingType, "SMTP") == 0))
			{
				RenderingUtilities.RenderPresenceJellyBean(sanitizingStringBuilder, userContext, true, "onRwCmJb(event);", flag2, "sndrJb");
			}
			if (flag2)
			{
				RenderingUtilities.RenderSenderPhoto(sanitizingStringBuilder, userContext, addressOrigin, sender.EmailAddress, sender.RoutingType, senderIsDl.GetValueOrDefault());
				if (renderSubHeader != null)
				{
					renderSubHeader(sanitizingStringBuilder);
				}
			}
			if (!string.IsNullOrEmpty(displayName))
			{
				sanitizingStringBuilder.Append(displayName);
			}
			else
			{
				sanitizingStringBuilder.Append<SanitizedHtmlString>(RenderingUtilities.GetDisplaySenderName(sender));
			}
			sanitizingStringBuilder.Append("</span>");
			return sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>();
		}

		internal static void RenderSenderPhoto(SanitizingStringBuilder<OwaHtml> stringBuilder, UserContext userContext, AddressOrigin addressOrigin, string senderEmail, string routingType, bool isDl)
		{
			bool flag = false;
			string srcUrl = string.Empty;
			if (addressOrigin == AddressOrigin.Directory)
			{
				flag = (!string.IsNullOrEmpty(senderEmail) && string.Equals(senderEmail, userContext.ExchangePrincipal.LegacyDn, StringComparison.OrdinalIgnoreCase));
				srcUrl = (isDl ? string.Empty : RenderingUtilities.GetADPictureUrl(senderEmail, routingType, userContext, flag));
			}
			else
			{
				srcUrl = (isDl ? string.Empty : RenderingUtilities.GetContactPictureUrl(string.Empty, string.Empty, senderEmail));
			}
			using (StringWriter stringWriter = new StringWriter(stringBuilder.UnsafeInnerStringBuilder))
			{
				RenderingUtilities.RenderDisplayPicture(stringWriter, userContext, srcUrl, 64, flag, !flag, isDl ? ThemeFileId.DoughboyDL : ThemeFileId.DoughboyPerson);
			}
		}

		internal static void RenderDisplayPicture(TextWriter writer, UserContext userContext, string srcUrl, int maxSize, bool isSelfADPicture, ThemeFileId doughboyThemeId)
		{
			RenderingUtilities.RenderDisplayPicture(writer, userContext, srcUrl, maxSize, isSelfADPicture, false, doughboyThemeId);
		}

		internal static void RenderDisplayPicture(TextWriter writer, UserContext userContext, string srcUrl, int maxSize, bool isSelfADPicture, bool isDelayedLoad, ThemeFileId doughboyThemeId)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			string arg = string.Format(" class=\"csimg {0}\"", ThemeManager.BaseTheme.GetThemeFileClass(doughboyThemeId));
			string arg2 = (isDelayedLoad && !string.IsNullOrEmpty(srcUrl)) ? string.Format(" img-src=\"{0}\"", srcUrl) : string.Empty;
			writer.Write(string.Format("<span id=\"dpFrame\"{0}><div id=\"dpDiv\"{1}{2}>", userContext.IsInstantMessageEnabled() ? string.Empty : " class=\"noImDispPic\"", arg, arg2));
			RenderingUtilities.RenderDisplayPictureImage(writer, userContext, isDelayedLoad ? userContext.GetThemeFileUrl(ThemeFileId.Clear) : srcUrl, maxSize, isSelfADPicture, doughboyThemeId);
			writer.Write("</div></span>");
		}

		internal static void RenderDisplayPictureImage(TextWriter writer, UserContext userContext, string srcUrl, int maxSize, bool isSelfADPicture, ThemeFileId doughboyThemeId)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			string arg = isSelfADPicture ? " name=\"selfPic\"" : string.Empty;
			string text = string.Format("id=\"dpImg\"{0}", arg);
			if (string.IsNullOrEmpty(srcUrl))
			{
				object[] extraAttributes = new object[]
				{
					text
				};
				userContext.RenderThemeImage(writer, doughboyThemeId, string.Empty, extraAttributes);
				return;
			}
			string str = string.Format(" onload=\"clipDispPic(this, {0});\"", maxSize.ToString());
			writer.Write("<IMG style=\"opacity:0;filter:alpha(opacity=0)\"" + text + str + " src=\"");
			writer.Write(Utilities.HtmlEncode(srcUrl));
			writer.Write("\"/>");
		}

		internal static string GetADPictureUrl(string email, string routingType, UserContext userContext, bool isSelf)
		{
			return RenderingUtilities.GetADPictureUrl(email, routingType, userContext, isSelf, false);
		}

		internal static string GetADPictureUrl(string email, string routingType, UserContext userContext, bool isSelf, bool isFromCache)
		{
			if (isSelf && !isFromCache && userContext.HasPicture != null && !userContext.HasPicture.Value)
			{
				return string.Empty;
			}
			string arg = string.Format("em={0}&rt={1}", string.IsNullOrEmpty(email) ? string.Empty : Utilities.UrlEncode(email.ToLower()), string.IsNullOrEmpty(routingType) ? string.Empty : Utilities.UrlEncode(routingType));
			string arg2 = isFromCache ? "1" : string.Empty;
			string format = OwaUrl.Oeh.GetExplicitUrl(OwaContext.Current) + "?oeh=1&ns=Attachments&ev=RenderADPhoto&{0}&FC={1}&Dpc={2}";
			return string.Format(format, arg, arg2, isSelf ? Utilities.UrlEncode(userContext.DisplayPictureCanary) : string.Empty);
		}

		internal static string GetContactPictureAttachmentId(Item item)
		{
			string result = string.Empty;
			if (item == null || item.AttachmentCollection == null)
			{
				return result;
			}
			foreach (AttachmentHandle handle in item.AttachmentCollection)
			{
				using (Attachment attachment = item.AttachmentCollection.Open(handle))
				{
					if (attachment.IsContactPhoto && attachment.Size > 0L)
					{
						result = attachment.Id.ToBase64String();
						break;
					}
				}
			}
			return result;
		}

		internal static string GetContactPictureUrl(string id, string attId, string email)
		{
			string arg = string.IsNullOrEmpty(id) ? string.Empty : ("&Id=" + Utilities.UrlEncode(id));
			string arg2 = string.IsNullOrEmpty(attId) ? string.Empty : ("&AttId=" + Utilities.UrlEncode(attId));
			string arg3 = string.IsNullOrEmpty(email) ? string.Empty : ("&em=" + Utilities.UrlEncode(email));
			string format = OwaUrl.Oeh.GetExplicitUrl(OwaContext.Current) + "?oeh=1&ns=Attachments&ev=RenderImage{0}{1}{2}";
			return string.Format(format, arg3, arg, arg2);
		}

		public static void RenderPresenceJellyBean(TextWriter writer, UserContext userContext, bool renderOnClickHandler, string onClickHandler, bool isVbar, string id)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			RenderingUtilities.RenderPresenceJellyBean(sanitizingStringBuilder, userContext, renderOnClickHandler, onClickHandler, isVbar, id);
			writer.Write(sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>());
		}

		private static void RenderPresenceJellyBean(SanitizingStringBuilder<OwaHtml> stringBuilder, UserContext userContext, bool renderOnClickHandler, string onClickHandler, bool isVbar, string id)
		{
			if (stringBuilder == null)
			{
				throw new ArgumentNullException("stringBuilder");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (renderOnClickHandler && string.IsNullOrEmpty(onClickHandler))
			{
				throw new ArgumentNullException("onClickHandler");
			}
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			sanitizingStringBuilder.Append(string.IsNullOrEmpty(id) ? "id=\"imgjb\" " : ("id=\"" + id + "\" "));
			sanitizingStringBuilder.Append("unselectable=\"on\" ");
			if (renderOnClickHandler)
			{
				Utilities.RenderScriptHandler(sanitizingStringBuilder, "onclick", onClickHandler);
			}
			Utilities.RenderScriptHandler(sanitizingStringBuilder, "ondblclick", "onDblClkJb(event);");
			string text = "jb ";
			ThemeFileId themeFileId = ThemeFileId.PresenceUnknown;
			if (isVbar)
			{
				text += "Vbar";
				themeFileId = ThemeFileId.PresenceUnknownVbar;
			}
			else
			{
				text += "jbs";
			}
			userContext.RenderThemeImage(stringBuilder.UnsafeInnerStringBuilder, themeFileId, text, new string[]
			{
				sanitizingStringBuilder.ToString()
			});
		}

		private static void RenderDropShadows(TextWriter output, ISessionContext sessionContext, bool includeUpperImages, bool includeLowerImages)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (sessionContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (includeLowerImages)
			{
				output.Write("<div class=\"dropshadow dropshadow-bottom\"></div>");
			}
			if (!sessionContext.IsRtl)
			{
				if (includeUpperImages)
				{
					sessionContext.RenderThemeImage(output, ThemeFileId.DropShadowTopRight, "dropshadow dropshadow-top-right", new object[0]);
					output.Write("<div class=\"dropshadow dropshadow-right\"></div>");
				}
				else
				{
					output.Write("<div class=\"dropshadow dropshadow-right-forDialogWithButton\"></div>");
				}
				if (includeLowerImages)
				{
					sessionContext.RenderThemeImage(output, ThemeFileId.DropShadowCornerBottomRight, "dropshadow dropshadow-corner-bottom-right", new object[0]);
					sessionContext.RenderThemeImage(output, ThemeFileId.DropShadowBottomLeft, "dropshadow dropshadow-bottom-left", new object[0]);
					return;
				}
			}
			else
			{
				if (includeUpperImages)
				{
					sessionContext.RenderThemeImage(output, ThemeFileId.DropShadowTopLeft, "dropshadow dropshadow-top-left", new object[0]);
					output.Write("<div class=\"dropshadow dropshadow-left\"></div>");
				}
				else
				{
					output.Write("<div class=\"dropshadow dropshadow-left-forDialogWithButton\"></div>");
				}
				if (includeLowerImages)
				{
					sessionContext.RenderThemeImage(output, ThemeFileId.DropShadowCornerBottomLeft, "dropshadow dropshadow-corner-bottom-left", new object[0]);
					sessionContext.RenderThemeImage(output, ThemeFileId.DropShadowBottomRight, "dropshadow dropshadow-bottom-right", new object[0]);
				}
			}
		}

		private static void GetSipUriFromMessageItem(Item message, Participant sender, ref string sipUri)
		{
			if (string.CompareOrdinal(sender.RoutingType, "EX") == 0)
			{
				object obj = message.TryGetProperty(ParticipantSchema.SipUri);
				if (obj == null || obj is PropertyError)
				{
					sipUri = null;
					return;
				}
				sipUri = (string)obj;
				if (string.IsNullOrEmpty(sipUri.Trim()))
				{
					sipUri = string.Empty;
				}
			}
		}

		private static void GetSipUriFromParticipant(Participant from, ref string sipUri)
		{
			if (string.CompareOrdinal(from.RoutingType, "EX") == 0)
			{
				sipUri = Utilities.GetParticipantProperty<string>(from, ParticipantSchema.SipUri, null);
			}
		}

		private static void GetSenderSmtpAddress(UserContext userContext, Participant sender, ref string smtpAddress, ref string id, ref string sipUri, ref string mobilePhoneNumber, ref bool? isDl, bool getDataFromAD)
		{
			if (string.CompareOrdinal(sender.RoutingType, "EX") == 0)
			{
				if (sipUri == null && !getDataFromAD)
				{
					sipUri = Utilities.GetParticipantProperty<string>(sender, ParticipantSchema.SipUri, null);
				}
				if (sipUri == null || (userContext.IsSmsEnabled && mobilePhoneNumber == null) || getDataFromAD)
				{
					IRecipientSession session = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, userContext);
					ADRawEntry adRecipientByLegacyExchangeDN = Utilities.GetAdRecipientByLegacyExchangeDN(session, sender.EmailAddress);
					if (adRecipientByLegacyExchangeDN != null)
					{
						smtpAddress = ((SmtpAddress)adRecipientByLegacyExchangeDN[ADRecipientSchema.PrimarySmtpAddress]).ToString();
						id = Convert.ToBase64String(((ADObjectId)adRecipientByLegacyExchangeDN[ADObjectSchema.Id]).ObjectGuid.ToByteArray());
						sipUri = InstantMessageUtilities.GetSipUri((ProxyAddressCollection)adRecipientByLegacyExchangeDN[ADRecipientSchema.EmailAddresses]);
						mobilePhoneNumber = Utilities.NormalizePhoneNumber(adRecipientByLegacyExchangeDN[ADOrgPersonSchema.MobilePhone] as string);
						isDl = new bool?(DirectoryAssistance.IsADRecipientDL((RecipientDisplayType?)adRecipientByLegacyExchangeDN[ADRecipientSchema.RecipientDisplayType]));
						return;
					}
				}
				else
				{
					if (isDl == null)
					{
						isDl = new bool?(Utilities.GetParticipantProperty<bool>(sender, ParticipantSchema.IsRoom, false));
					}
					if (smtpAddress == null)
					{
						smtpAddress = Utilities.GetParticipantProperty<string>(sender, ParticipantSchema.SmtpAddress, null);
						return;
					}
				}
			}
			else if (string.CompareOrdinal(sender.RoutingType, "SMTP") == 0)
			{
				smtpAddress = sender.EmailAddress;
				sipUri = sender.EmailAddress;
			}
		}

		internal static SanitizedHtmlString GetDisplaySenderName(Participant sender)
		{
			if (sender == null)
			{
				return SanitizedHtmlString.Empty;
			}
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>();
			if (!string.IsNullOrEmpty(sender.DisplayName))
			{
				if (!string.IsNullOrEmpty(sender.EmailAddress) && (string.CompareOrdinal(sender.RoutingType, "SMTP") == 0 || string.CompareOrdinal(sender.RoutingType, "MOBILE") == 0) && !sender.DisplayName.Contains(sender.EmailAddress))
				{
					sanitizingStringBuilder.AppendFormat("{0} [{1}]", new object[]
					{
						sender.DisplayName,
						sender.EmailAddress
					});
				}
				else
				{
					sanitizingStringBuilder.Append(sender.DisplayName);
				}
			}
			else if (!string.IsNullOrEmpty(sender.EmailAddress))
			{
				sanitizingStringBuilder.Append(sender.EmailAddress);
			}
			return sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>();
		}

		public static void RenderThemeUrlVariable(TextWriter output, ISessionContext sessionContext)
		{
			output.Write("a_sThmUrl=\"");
			Utilities.JavascriptEncode(sessionContext.Theme.Url, output);
			output.Write("\";");
		}

		public static void RenderBaseThemeUrlVariable(TextWriter output)
		{
			output.Write("a_sThmUrl=\"");
			Utilities.JavascriptEncode(ThemeManager.BaseTheme.Url, output);
			output.Write("\";");
		}

		public static void RenderStringVariable(TextWriter output, string variableName, Strings.IDs stringID)
		{
			RenderingUtilities.RenderStringVariable(output, variableName, RenderingUtilities.GetSanitizedJavascriptString(stringID));
		}

		public static void RenderStringVariable(TextWriter output, string variableName, string input)
		{
			RenderingUtilities.RenderStringVariable(output, variableName, SanitizedHtmlString.GetSanitizedStringWithoutEncoding(Utilities.JavascriptEncode(input)));
		}

		public static void RenderStringVariable(TextWriter output, string variableName, SanitizedHtmlString input)
		{
			RenderingUtilities.RenderStringVariable(output, variableName, Utilities.JavascriptEncode(input));
		}

		private static void RenderStringVariable(TextWriter output, string variableName, object input)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (variableName == null)
			{
				throw new ArgumentNullException("variableName");
			}
			if (variableName.Length == 0)
			{
				throw new ArgumentException("variableName is empty.");
			}
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			output.Write("var ");
			output.Write(SanitizedHtmlString.GetSanitizedStringWithoutEncoding(variableName));
			output.Write(" = \"");
			output.Write(input);
			output.Write("\";\n");
		}

		public static void RenderOptionalStringVariable(TextWriter output, string variableName, string input)
		{
			RenderingUtilities.RenderStringVariable(output, variableName, string.IsNullOrEmpty(input) ? string.Empty : input);
		}

		internal static void RenderSpecialFolderIconForSharedFolders(TextWriter output, UserContext userContext, bool isRoot, bool isInbox)
		{
			ThemeFileId themeFileId = ThemeFileId.Folder;
			if (isRoot)
			{
				themeFileId = ThemeFileId.Root;
			}
			else if (isInbox)
			{
				themeFileId = ThemeFileId.Inbox;
			}
			userContext.RenderThemeImage(output, themeFileId);
		}

		internal static void RenderSpecialFolderIcon(TextWriter output, UserContext userContext, DefaultFolderType folderType, string folderClass, bool isSearchFolder, bool isELCFolder, FolderSharingFlag sharingFlag, params string[] extraAttributes)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			ThemeFileId themeFileId = ThemeFileId.None;
			switch (folderType)
			{
			case DefaultFolderType.DeletedItems:
				themeFileId = ThemeFileId.Deleted;
				goto IL_EB;
			case DefaultFolderType.Drafts:
				themeFileId = ThemeFileId.Drafts;
				goto IL_EB;
			case DefaultFolderType.Inbox:
				themeFileId = ThemeFileId.Inbox;
				goto IL_EB;
			case DefaultFolderType.JunkEmail:
				themeFileId = ThemeFileId.JunkEMail;
				goto IL_EB;
			case DefaultFolderType.Journal:
				themeFileId = ThemeFileId.Journal;
				goto IL_EB;
			case DefaultFolderType.Notes:
				themeFileId = ThemeFileId.Notes;
				goto IL_EB;
			case DefaultFolderType.Outbox:
				themeFileId = ThemeFileId.Outbox;
				goto IL_EB;
			case DefaultFolderType.SentItems:
				themeFileId = ThemeFileId.SentItems;
				goto IL_EB;
			case DefaultFolderType.Tasks:
			case DefaultFolderType.Reminders:
			case DefaultFolderType.Conflicts:
			case DefaultFolderType.SyncIssues:
			case DefaultFolderType.LocalFailures:
			case DefaultFolderType.ServerFailures:
			case DefaultFolderType.CommonViews:
			case DefaultFolderType.ElcRoot:
				break;
			case DefaultFolderType.RssSubscription:
				themeFileId = ThemeFileId.RssSubscription;
				goto IL_EB;
			case DefaultFolderType.ToDoSearch:
				themeFileId = ThemeFileId.Flag;
				goto IL_EB;
			case DefaultFolderType.CommunicatorHistory:
				themeFileId = ThemeFileId.ConversationHistory;
				goto IL_EB;
			default:
				if (folderType == DefaultFolderType.Root)
				{
					themeFileId = ThemeFileId.Root;
					goto IL_EB;
				}
				break;
			}
			if (isSearchFolder)
			{
				themeFileId = ThemeFileId.SearchFolderIcon;
			}
			else if (isELCFolder)
			{
				themeFileId = ThemeFileId.ELCFolderIcon;
			}
			IL_EB:
			if (themeFileId == ThemeFileId.None)
			{
				SmallIconManager.RenderFolderIcon(output, userContext, folderClass, sharingFlag, false, extraAttributes);
				return;
			}
			userContext.RenderThemeImage(output, themeFileId, null, extraAttributes);
		}

		public static void RenderDateTimeScriptObject(TextWriter output, ExDateTime dateTime)
		{
			RenderingUtilities.RenderDateTimeScriptObject(output, dateTime, false);
		}

		public static void RenderDateTimeScriptObject(TextWriter output, ExDateTime dateTime, bool seconds)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("new Date(Date.UTC(");
			output.Write(dateTime.Year);
			output.Write(",");
			output.Write(dateTime.Month - 1);
			output.Write(",");
			output.Write(dateTime.Day);
			output.Write(",");
			output.Write(dateTime.Hour);
			output.Write(",");
			output.Write(dateTime.Minute);
			if (seconds)
			{
				output.Write(",");
				output.Write(dateTime.Second);
			}
			output.Write("))");
		}

		public static void RenderFormatBarState(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			output.WriteLine("var a_iFmtBrSt = {0};", (int)userContext.UserOptions.FormatBarState);
			RenderingUtilities.RenderOptionalStringVariable(output, "a_sMruFnts", userContext.UserOptions.MruFonts);
		}

		public static void RenderDefaultFontStyle(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			output.Write(ReplyForwardUtilities.GetDefaultUserFontStyle(userContext));
		}

		public static void RenderJavascriptStringArray(TextWriter output, IEnumerable<string> strings)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (strings == null)
			{
				output.Write("null");
				return;
			}
			string[] array = strings as string[];
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (0 < i)
					{
						output.Write(",");
					}
					output.Write("\"");
					output.Write(SanitizedHtmlString.GetSanitizedStringWithoutEncoding(Utilities.JavascriptEncode(array[i])));
					output.Write("\"");
				}
				return;
			}
			bool flag = false;
			foreach (string s in strings)
			{
				if (flag)
				{
					output.Write(",");
				}
				else
				{
					flag = true;
				}
				output.Write("\"");
				output.Write(SanitizedHtmlString.GetSanitizedStringWithoutEncoding(Utilities.JavascriptEncode(s)));
				output.Write("\"");
			}
		}

		public static void RenderJavascriptStringArray(TextWriter writer, string variableName, IEnumerable<string> strings)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (variableName == null)
			{
				throw new ArgumentNullException("variableName");
			}
			writer.Write("var ");
			writer.Write(SanitizedHtmlString.GetSanitizedStringWithoutEncoding(variableName));
			writer.Write(" = new Array(");
			RenderingUtilities.RenderJavascriptStringArray(writer, strings);
			writer.Write(");\n");
		}

		public static void RenderAbbreviatedDayNames(TextWriter output)
		{
			RenderingUtilities.RenderAbbreviatedDayNames(output, false);
		}

		public static void RenderFullDayNames(TextWriter output)
		{
			RenderingUtilities.RenderFullDayNames(output, false);
		}

		public static void RenderAbbreviatedDayNames(TextWriter output, bool convertToLowerCase)
		{
			string[] abbreviatedDayNames = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
			IEnumerable<string> strings;
			if (!convertToLowerCase)
			{
				strings = abbreviatedDayNames;
			}
			else
			{
				strings = from s in abbreviatedDayNames
				select s.ToLower();
			}
			RenderingUtilities.RenderJavascriptStringArray(output, strings);
		}

		public static void RenderFullDayNames(TextWriter output, bool convertToLowerCase)
		{
			string[] dayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
			IEnumerable<string> strings;
			if (!convertToLowerCase)
			{
				strings = dayNames;
			}
			else
			{
				strings = from s in dayNames
				select s.ToLower();
			}
			RenderingUtilities.RenderJavascriptStringArray(output, strings);
		}

		public static void RenderFullMonthNames(TextWriter output)
		{
			RenderingUtilities.RenderJavascriptStringArray(output, CultureInfo.CurrentCulture.DateTimeFormat.MonthNames);
		}

		public static void RenderAbbreviatedMonthNames(TextWriter output)
		{
			RenderingUtilities.RenderJavascriptStringArray(output, CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames);
		}

		public static void RenderSignature(TextWriter output, UserContext userContext, bool renderAsHtml)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (userContext.IsFeatureEnabled(Feature.Signature))
			{
				if (renderAsHtml)
				{
					if (!string.IsNullOrEmpty(userContext.UserOptions.SignatureHtml))
					{
						Utilities.JavascriptEncode(userContext.UserOptions.SignatureHtml, output);
						return;
					}
				}
				else if (!string.IsNullOrEmpty(userContext.UserOptions.SignatureText))
				{
					Utilities.JavascriptEncode("\n", output);
					Utilities.JavascriptEncode(userContext.UserOptions.SignatureText, output);
				}
			}
		}

		public static void RenderHelpButton(TextWriter output, UserContext userContext, string helpFile, string helpAnchor)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (string.IsNullOrEmpty(helpFile))
			{
				throw new ArgumentException("helpFile may not be null or empty string");
			}
			if (helpAnchor == null)
			{
				throw new ArgumentNullException("helpAnchor");
			}
			output.Write("<a id=help class=btnDf name=lnkB href=\"");
			output.Write(Utilities.HtmlEncode(Utilities.BuildEhcHref(helpFile)));
			output.Write("\">");
			userContext.RenderThemeImage(output, ThemeFileId.Help);
			output.Write("<span id=spnHlpTxt>");
			output.Write(LocalizedStrings.GetHtmlEncoded(1454393937));
			output.Write("</span></a>");
		}

		public static void RenderErrorInfobar(ISessionContext sessionContext, TextWriter output, string id)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (sessionContext == null)
			{
				throw new ArgumentNullException("sessionContext");
			}
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id cannot be null or empty.");
			}
			output.Write("<div id=");
			output.Write(id);
			output.Write(" style=\"display:none\"");
			output.Write(" iType=");
			output.Write(10);
			output.Write(">");
			output.Write("<div id=divEIB><table cellpadding=0 cellspacing=0><tr><td id=tdIcon valign=top>");
			sessionContext.RenderThemeImage(output, ThemeFileId.Error);
			output.Write("</td><td id=tdMsg style=\"width:100%;\"></td>");
			if (Globals.ShowDebugInformation)
			{
				output.Write("<td id=tdEISnd style=\"display:none;vertical-align:top;\">");
				sessionContext.RenderThemeImageStart(output, ThemeFileId.Send, null);
				output.Write("id=imgEISnd style=cursor:hand ");
				Utilities.RenderScriptHandler(output, "onclick", "onClkEISnd();");
				output.Write(" _msgSent=\"");
				output.Write(SanitizedHtmlString.FromStringId(1514109018));
				output.Write("\" title=\"");
				output.Write(SanitizedHtmlString.FromStringId(796120554));
				output.Write("\"");
				sessionContext.RenderThemeImageEnd(output, ThemeFileId.Send);
				output.Write("</td><td id=tdEICpy style=\"display:none;vertical-align:top;\">");
				sessionContext.RenderThemeImageStart(output, ThemeFileId.Copy, null);
				output.Write("id=imgEICpy style=\"cursor:hand\" ");
				Utilities.RenderScriptHandler(output, "onclick", "onClkEICpy();");
				output.Write(" title=\"");
				output.Write(SanitizedHtmlString.FromStringId(1353584623));
				output.Write("\"");
				sessionContext.RenderThemeImageEnd(output, ThemeFileId.Copy);
				output.Write("</td>");
			}
			output.Write("</tr></table></div></div>");
		}

		public static void RenderError(UserContext userContext, TextWriter writer, Strings.IDs errorString)
		{
			RenderingUtilities.RenderError(userContext, writer, SanitizedHtmlString.FromStringId(errorString));
		}

		public static void RenderError(UserContext userContext, TextWriter writer, SanitizedHtmlString messageHtml)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			writer.Write("<div id=divEIB><table cellpadding=0 cellspacing=0><tr><td valign=top>");
			userContext.RenderThemeImage(writer, ThemeFileId.Error);
			writer.Write("</td><td id=tdMsg>");
			writer.Write(messageHtml);
			writer.Write("</td></tr></table></div>");
		}

		public static void RenderDurationFormatString(TextWriter output, ISessionContext sessionContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (sessionContext == null)
			{
				throw new ArgumentNullException("sessionContext");
			}
			output.Write(sessionContext.IsRtl ? "&nbsp;" : " ");
			output.Write(sessionContext.GetDirectionMark());
			output.Write("({0} ");
			output.Write(SanitizedHtmlString.FromStringId(-1229628825));
			output.Write(")");
			output.Write(sessionContext.GetDirectionMark());
		}

		public static void RenderFullWeekdayDateFormat(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			output.Write(userContext.UserOptions.GetWeekdayDateFormat(true));
		}

		public static void RenderOptionsInfobars(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			Infobar infobar = new Infobar();
			infobar.Render(output);
		}

		internal static void RenderReplyForwardMessageStatus(IStorePropertyBag storePropertyBag, Infobar infobar, UserContext userContext)
		{
			object obj = storePropertyBag.TryGetProperty(MessageItemSchema.LastVerbExecuted);
			object obj2 = storePropertyBag.TryGetProperty(MessageItemSchema.LastVerbExecutionTime);
			if (obj2 is ExDateTime)
			{
				ExDateTime exDateTime = (ExDateTime)obj2;
				if (obj is int && ((int)obj == 102 || (int)obj == 103))
				{
					SanitizedHtmlString messageHtml = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1084747171), new object[]
					{
						exDateTime.ToString(userContext.UserOptions.DateFormat),
						exDateTime.ToString(userContext.UserOptions.TimeFormat)
					});
					infobar.AddMessage(messageHtml, InfobarMessageType.Informational);
					return;
				}
				if (obj is int && (int)obj == 104)
				{
					SanitizedHtmlString messageHtml = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(1995820000), new object[]
					{
						exDateTime.ToString(userContext.UserOptions.DateFormat),
						exDateTime.ToString(userContext.UserOptions.TimeFormat)
					});
					infobar.AddMessage(messageHtml, InfobarMessageType.Informational);
				}
			}
		}

		internal static void RenderVotingInfobarMessages(MessageItem message, Infobar infobar, UserContext userContext)
		{
			if (message == null || infobar == null || userContext == null)
			{
				return;
			}
			VotingInfo votingInfo = message.VotingInfo;
			string[] array = (string[])votingInfo.GetOptionsList();
			int num = array.Length;
			if (num > 0)
			{
				bool decisionUpdateFromTransport = false;
				int num2 = 0;
				bool flag = Utilities.IsValidApprovalRequest(message);
				int? valueAsNullable = message.GetValueAsNullable<int>(MessageItemSchema.LastVerbExecuted);
				if (valueAsNullable != null)
				{
					num2 = valueAsNullable.Value;
				}
				string response = null;
				if (flag)
				{
					int? valueAsNullable2 = message.GetValueAsNullable<int>(MessageItemSchema.ApprovalDecision);
					if (valueAsNullable2 != null)
					{
						num2 = valueAsNullable2.Value;
						decisionUpdateFromTransport = true;
					}
				}
				if (num2 <= num && num2 >= 1 && num2 < 100)
				{
					if (flag && num2 <= 2)
					{
						if (num2 == 1)
						{
							response = LocalizedStrings.GetNonEncoded(-236685197);
						}
						else
						{
							response = LocalizedStrings.GetNonEncoded(-2059328365);
						}
					}
					else
					{
						response = array[num2 - 1];
					}
				}
				SanitizedHtmlString votingRequestInfobarMessage = RenderingUtilities.GetVotingRequestInfobarMessage(response, message, decisionUpdateFromTransport, userContext);
				if (!SanitizedStringBase<OwaHtml>.IsNullOrEmpty(votingRequestInfobarMessage))
				{
					infobar.AddMessage(votingRequestInfobarMessage, InfobarMessageType.Informational);
					return;
				}
			}
			else if (num == 0 && !string.IsNullOrEmpty(votingInfo.Response))
			{
				SanitizedHtmlString messageHtml = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(190230518), new object[]
				{
					votingInfo.Response
				});
				infobar.AddMessage(messageHtml, InfobarMessageType.Informational);
			}
		}

		private static SanitizedHtmlString GetVotingRequestInfobarMessage(string response, MessageItem message, bool decisionUpdateFromTransport, UserContext userContext)
		{
			if (message == null || userContext == null)
			{
				return null;
			}
			bool flag = !string.IsNullOrEmpty(response);
			if (flag && !decisionUpdateFromTransport)
			{
				ExDateTime? valueAsNullable = message.GetValueAsNullable<ExDateTime>(MessageItemSchema.LastVerbExecutionTime);
				if (valueAsNullable != null)
				{
					return SanitizedHtmlString.Format(LocalizedStrings.GetNonEncoded(1367530482), new object[]
					{
						response,
						valueAsNullable.Value.ToString(userContext.UserOptions.DateFormat),
						valueAsNullable.Value.ToString(userContext.UserOptions.TimeFormat)
					});
				}
			}
			else if (flag && decisionUpdateFromTransport)
			{
				string valueOrDefault = message.GetValueOrDefault<string>(MessageItemSchema.ApprovalDecisionMaker);
				ExDateTime? valueAsNullable2 = message.GetValueAsNullable<ExDateTime>(MessageItemSchema.ApprovalDecisionTime);
				if (!string.IsNullOrEmpty(valueOrDefault) && valueAsNullable2 != null)
				{
					return SanitizedHtmlString.Format(LocalizedStrings.GetNonEncoded(-145538990), new object[]
					{
						valueOrDefault,
						response,
						valueAsNullable2.Value.ToString(userContext.UserOptions.DateFormat),
						valueAsNullable2.Value.ToString(userContext.UserOptions.TimeFormat)
					});
				}
			}
			else if (ObjectClass.IsOfClass(message.ClassName, "IPM.Note.Microsoft.Approval.Request"))
			{
				return SanitizedHtmlString.FromStringId(352974238);
			}
			return null;
		}

		internal static void RenderAttribute(TextWriter output, string name, string value)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			output.Write(" ");
			output.Write(name);
			output.Write("=\"");
			Utilities.HtmlEncode(value, output);
			output.Write("\"");
		}

		internal static bool AddAttachmentInfobarMessages(IStorePropertyBag storePropertyBag, bool isEmbeddedItem, bool forceEnableItemLink, Infobar infobar, ArrayList attachmentList)
		{
			if (storePropertyBag == null)
			{
				throw new ArgumentNullException("storePropertyBag");
			}
			UserContext userContext = UserContextManager.GetUserContext();
			if (JunkEmailUtilities.IsJunkOrPhishing(storePropertyBag, isEmbeddedItem, forceEnableItemLink, userContext))
			{
				return false;
			}
			InfobarRenderingHelper infobarRenderingHelper = new InfobarRenderingHelper(attachmentList);
			if (infobar != null && infobarRenderingHelper.HasLevelOneAndBlock)
			{
				infobar.AddMessage(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-824680214), new object[]
				{
					infobarRenderingHelper.FileNameStringForLevelOneAndBlock
				}), InfobarMessageType.Informational);
			}
			return infobarRenderingHelper.HasLevelTwo || infobarRenderingHelper.HasLevelThree || infobarRenderingHelper.HasWebReadyFirst;
		}

		public static void RenderQuotaBar(TextWriter writer, UserContext userContext, int percentageUsed, QuotaLevel quotaLevel)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (percentageUsed < 0)
			{
				throw new ArgumentOutOfRangeException("percentageUsed", "percentageUsed cannot be less than " + 0);
			}
			string value = "elcQtNorm";
			switch (quotaLevel)
			{
			case QuotaLevel.AboveWarning:
				value = "elcQtWarn";
				break;
			case QuotaLevel.Exceeded:
				value = "elcQtExc";
				break;
			}
			writer.Write("<div id=\"elcQuotaBar\">");
			writer.Write("<div id=\"elcQuotaFree\">");
			writer.Write("</div>");
			writer.Write("<div id=\"elcQuotaUsed\" class=\"");
			writer.Write(value);
			writer.Write("\" style=\"width:");
			writer.Write((percentageUsed >= 100) ? 100 : percentageUsed);
			writer.Write("%");
			writer.Write((percentageUsed <= 0) ? ";display:none;" : string.Empty);
			writer.Write("\">");
			writer.Write("</div>");
			writer.Write("</div>");
		}

		public static void RenderMailboxQuota(TextWriter writer, UserContext userContext)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			long num = userContext.UpdateUsedQuota();
			QuotaLevel quotaLevel = QuotaLevel.Normal;
			double num2 = 0.0;
			string text = string.Empty;
			string text2 = string.Empty;
			if (userContext.QuotaSend > 0L)
			{
				if (num > userContext.QuotaSend)
				{
					quotaLevel = QuotaLevel.Exceeded;
				}
				else if (userContext.QuotaWarning > 0L && num > userContext.QuotaWarning)
				{
					quotaLevel = QuotaLevel.AboveWarning;
				}
				num2 = (double)num / (double)userContext.QuotaSend;
				StringBuilder stringBuilder = new StringBuilder();
				using (StringWriter stringWriter = new StringWriter(stringBuilder))
				{
					Utilities.RenderSizeWithUnits(stringWriter, userContext.QuotaSend, false, false);
				}
				text = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(406437542), new object[]
				{
					stringBuilder.ToString()
				});
				text2 = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(611216529), new object[]
				{
					stringBuilder.ToString()
				});
			}
			string value = "mbqtNrm";
			string value2 = "mbqtTxtBlk";
			string value3 = "mbqtTxtBlk";
			string value4 = "mbqtUsdNrm";
			int num3 = (int)(num2 * 128.0 + 0.5) + 2;
			if (num3 <= 2)
			{
				num3 = 0;
			}
			int num4 = 132 - num3;
			if (num4 <= 2)
			{
				num4 = 0;
				num3 = 132;
			}
			switch (quotaLevel)
			{
			case QuotaLevel.AboveWarning:
				value = "mbqtAbvWrn";
				value2 = "mbqtTxtRd";
				value4 = "mbqtUsdAbvWrn";
				break;
			case QuotaLevel.Exceeded:
				value = "mbqtAbvWrn";
				value2 = "mbqtTxtRd";
				value3 = "mbqtTxtRd";
				value4 = "mbqtUsdExcQt";
				break;
			}
			writer.Write("<div id=\"divMbxQt\" class=\"");
			writer.Write(value);
			writer.Write("\" _fDsp=\"");
			if (quotaLevel == QuotaLevel.Normal)
			{
				writer.Write("0");
			}
			else
			{
				writer.Write("1");
			}
			writer.Write("\"><div class=\"mbqtSzInd\">");
			if (num3 != 0)
			{
				writer.Write("<div class=\"mbqtSz ");
				writer.Write(value4);
				writer.Write("\" style=\"width:");
				writer.Write(num3 - 2);
				writer.Write("px\"></div>");
			}
			if (num4 != 0)
			{
				writer.Write("<div class=\"mbqtFrSz mbqtSz\" style=\"width:");
				writer.Write(num4 - 2);
				writer.Write("px\"></div>");
			}
			writer.Write("</div><div class=\"mbqtTxt\"><span id=\"spnMbQtUsg\" class=\"");
			writer.Write(value2);
			writer.Write("\">");
			StringBuilder stringBuilder2 = new StringBuilder();
			using (StringWriter stringWriter2 = new StringWriter(stringBuilder2))
			{
				Utilities.RenderSizeWithUnits(stringWriter2, num, false, false);
			}
			Utilities.HtmlEncode(string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(-990588681), new object[]
			{
				stringBuilder2.ToString()
			}), writer);
			writer.Write("</span>&nbsp;<span id=\"spnMbQtWrn\" class=\"");
			writer.Write(value3);
			writer.Write("\">");
			Utilities.HtmlEncode((quotaLevel == QuotaLevel.Exceeded) ? text2 : text, writer);
			writer.Write("</span></div></div>");
		}

		public static void RenderSMimeControl(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<object id=\"MimeNS\" classid=\"CLSID:4F40839A-C1E5-47E3-804D-A2A17F42DA21\">");
			writer.Write("<param name=\"\" value=\"\">");
			writer.Write("</object>");
			writer.Write("<?import namespace=\"MIME\" implementation=#MimeNS>");
		}

		public static void RenderSMimeEdit(UserContext userContext, TextWriter writer, bool isComposeForm, bool allowWebBeacons)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer cannot be null");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext cannot be null");
			}
			SanitizedHtmlString arg = SanitizedHtmlString.Format("{0}{1}{2}", new object[]
			{
				OwaContext.Current.UrlToHost,
				OwaUrl.ApplicationRoot.GetExplicitUrl(OwaContext.Current),
				Redir.BuildRedirUrl(userContext, string.Empty)
			});
			SanitizedHtmlString arg2 = SanitizedHtmlString.Format("{0}{1}{2}", new object[]
			{
				OwaContext.Current.UrlToHost,
				OwaUrl.ApplicationRoot.GetExplicitUrl(OwaContext.Current),
				Redir.BuildRedirUrlForSMime(userContext, string.Empty)
			});
			writer.Write("<mime:mimeedit id=\"mimeEdit\" class=\"wh100");
			if (!isComposeForm)
			{
				writer.Write(" mrg");
			}
			writer.Write("\" dir=\"");
			writer.Write(userContext.IsRtl ? "rtl" : "ltr");
			writer.Write("\" ");
			writer.Write(RenderingUtilities.mimeCtrlAttribs, arg, arg2);
			if (allowWebBeacons || userContext.Configuration.FilterWebBeaconsAndHtmlForms == WebBeaconFilterLevels.DisableFilter)
			{
				writer.Write("filterWebBeaconsAndHtmlForms=\"false\" ");
			}
			else
			{
				writer.Write("filterWebBeaconsAndHtmlForms=\"true\" OnExternalContentBlocked=\"onMmExCtBlk()\" ");
			}
			if (isComposeForm)
			{
				writer.Write("squiggleUrl=\"");
				if (OwaContext.Current.IsProxyRequest)
				{
					Utilities.SanitizeHtmlEncode(OwaContext.Current.ProxyCasUri.ToString(), writer);
				}
				else
				{
					Utilities.SanitizeHtmlEncode(OwaContext.Current.LocalHostName, writer);
				}
				writer.Write('/');
				userContext.RenderThemeFileUrl(writer, ThemeFileId.SpellCheckUnderline);
				writer.Write("\" ");
				writer.Write("mimeRequestUrlBase=\"");
				if (OwaContext.Current.IsProxyRequest)
				{
					Utilities.SanitizeHtmlEncode(OwaContext.Current.ProxyCasUri.ToString(), writer);
				}
				else
				{
					Utilities.SanitizeHtmlEncode(OwaContext.Current.LocalHostName, writer);
				}
				writer.Write('/');
				writer.Write("\" ");
				if (userContext.UserOptions.UseManuallyPickedSigningCertificate)
				{
					writer.Write("signingCertID=\"");
					Utilities.SanitizeHtmlEncode(userContext.UserOptions.SigningCertificateId, writer);
					writer.Write("\" ");
				}
			}
			string displayName = userContext.MailboxIdentity.GetOWAMiniRecipient().DisplayName;
			string text = userContext.MailboxIdentity.GetOWAMiniRecipient().PrimarySmtpAddress.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			ProxyAddressCollection emailAddresses = userContext.MailboxIdentity.GetOWAMiniRecipient().EmailAddresses;
			if (emailAddresses != null && emailAddresses.Count > 0)
			{
				for (int i = 0; i < emailAddresses.Count; i++)
				{
					if (emailAddresses[i].Prefix == ProxyAddressPrefix.Smtp)
					{
						stringBuilder = stringBuilder.AppendFormat("{0}; ", emailAddresses[i].AddressString);
					}
				}
			}
			writer.Write("fromAddress=\"");
			Utilities.SanitizeHtmlEncode(string.Concat(new string[]
			{
				"\"",
				displayName,
				"\"<",
				text,
				">"
			}), writer);
			writer.Write("\" routingAddresses=\"");
			Utilities.SanitizeHtmlEncode(stringBuilder.ToString(), writer);
			writer.Write("\"");
			RenderingUtilities.SMimeSecurityFlags smimeSecurityFlags = RenderingUtilities.SMimeSecurityFlags.None;
			if (isComposeForm)
			{
				writer.Write(" tripleWrap=\"");
				writer.Write(OwaRegistryKeys.TripleWrapSignedEncryptedMail ? "true" : "false");
				writer.Write("\"");
				writer.Write(" clearSign=\"");
				writer.Write(OwaRegistryKeys.ClearSign ? "true" : "false");
				writer.Write("\"");
				switch (OwaRegistryKeys.BccEncryptedEmailForking)
				{
				case 1:
					smimeSecurityFlags |= RenderingUtilities.SMimeSecurityFlags.RecipOneForAllBcc;
					break;
				case 2:
					smimeSecurityFlags |= RenderingUtilities.SMimeSecurityFlags.RecipAll;
					break;
				}
				if (OwaRegistryKeys.IncludeSMIMECapabilitiesInMessage)
				{
					smimeSecurityFlags |= RenderingUtilities.SMimeSecurityFlags.SendSmimeCapabilities;
				}
				if (OwaRegistryKeys.CopyRecipientHeaders)
				{
					smimeSecurityFlags |= RenderingUtilities.SMimeSecurityFlags.CopyRecipHeaders;
				}
				if (OwaRegistryKeys.IncludeCertificateChainWithoutRootCertificate)
				{
					smimeSecurityFlags |= RenderingUtilities.SMimeSecurityFlags.SendChainNoRoot;
				}
				if (OwaRegistryKeys.IncludeCertificateChainAndRootCertificate)
				{
					smimeSecurityFlags |= RenderingUtilities.SMimeSecurityFlags.SendChain;
				}
				if (!OwaRegistryKeys.SignedEmailCertificateInclusion)
				{
					smimeSecurityFlags |= RenderingUtilities.SMimeSecurityFlags.SigningCertOnly;
				}
			}
			writer.Write(" smartcardOnly=\"");
			writer.Write(OwaRegistryKeys.OnlyUseSmartCard ? "true" : "false");
			writer.Write("\"");
			if (!OwaRegistryKeys.EncryptTemporaryBuffers)
			{
				smimeSecurityFlags |= RenderingUtilities.SMimeSecurityFlags.TempBufferNoEncrypt;
			}
			if (OwaRegistryKeys.UseEmbeddedMessageFileNameAsAttachmentName)
			{
				smimeSecurityFlags |= RenderingUtilities.SMimeSecurityFlags.UseEmbeddedMessageFileNameAsAttachmentName;
			}
			writer.Write(" securityFlags=\"");
			writer.Write((uint)smimeSecurityFlags);
			writer.Write("\"");
			if (!string.IsNullOrEmpty(OwaRegistryKeys.EncryptionAlgorithms))
			{
				writer.Write(" encryptionAlgorithms=\"");
				writer.Write(Utilities.SanitizeHtmlEncode(OwaRegistryKeys.EncryptionAlgorithms));
				writer.Write("\" ");
			}
			if (!string.IsNullOrEmpty(OwaRegistryKeys.SigningAlgorithms))
			{
				writer.Write(" signingAlgorithms=\"");
				writer.Write(Utilities.SanitizeHtmlEncode(OwaRegistryKeys.SigningAlgorithms));
				writer.Write("\" ");
			}
			writer.Write("/>");
		}

		internal static void RenderSecondaryNavigationDatePicker(CalendarFolder folder, TextWriter output, string errorDivId, string datePickerId, UserContext userContext)
		{
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (string.IsNullOrEmpty(datePickerId))
			{
				throw new ArgumentException("The date picker Id cannot be " + ((datePickerId == null) ? "null" : "empty"), "datePickerId");
			}
			if (string.IsNullOrEmpty(errorDivId))
			{
				throw new ArgumentException("The errorDivId cannot be " + ((errorDivId == null) ? "null" : "empty"), "errorDivId");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			FolderViewStates folderViewStates = userContext.GetFolderViewStates(folder);
			CalendarViewType calendarViewType = folderViewStates.CalendarViewType;
			ExDateTime[] viewDays = CalendarUtilities.GetViewDays(userContext, null, calendarViewType, OwaStoreObjectId.CreateFromStoreObject(folder), folderViewStates);
			Infobar infobar = new Infobar(errorDivId, "infobar");
			infobar.Render(output);
			DatePicker.Features features = DatePicker.Features.MultiDaySelection | DatePicker.Features.WeekSelector;
			if (userContext.UserOptions.ShowWeekNumbers)
			{
				features |= DatePicker.Features.WeekNumbers;
			}
			DatePicker datePicker = new DatePicker(datePickerId, viewDays, (int)features);
			datePicker.Render(output);
		}

		public static void RenderInteger(TextWriter writer, string variableName, int input)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (variableName == null)
			{
				throw new ArgumentNullException("variableName");
			}
			writer.Write("var ");
			writer.Write(SanitizedHtmlString.GetSanitizedStringWithoutEncoding(variableName));
			writer.Write(" = ");
			writer.Write(input);
			writer.Write(";\n");
		}

		public static void RenderWebReadyPolicy(TextWriter writer, UserContext userContext)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			AttachmentPolicy attachmentPolicy = userContext.AttachmentPolicy;
			if (attachmentPolicy == null)
			{
				throw new ArgumentException("userContext.AttachmentPolicy is null", "userContext");
			}
			RenderingUtilities.RenderInteger(writer, "a_iWRDVE", attachmentPolicy.WebReadyDocumentViewingEnable ? 1 : 0);
			if (attachmentPolicy.WebReadyDocumentViewingEnable)
			{
				RenderingUtilities.RenderInteger(writer, "a_iWRDVAST", attachmentPolicy.WebReadyDocumentViewingForAllSupportedTypes ? 1 : 0);
				RenderingUtilities.RenderJavascriptStringArray(writer, "a_rgWRFT", attachmentPolicy.WebReadyFileTypes);
				RenderingUtilities.RenderJavascriptStringArray(writer, "a_rgWRDVSFT", attachmentPolicy.WebReadyDocumentViewingSupportedFileTypes);
			}
		}

		public static void RenderSmallIconTable(TextWriter writer, bool renderDefaultTable)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			using (Dictionary<int, Dictionary<string, SmallIconManager.SmallIcon>>.Enumerator smallIconTable = SmallIconManager.SmallIconTable)
			{
				while (smallIconTable.MoveNext())
				{
					KeyValuePair<int, Dictionary<string, SmallIconManager.SmallIcon>> keyValuePair = smallIconTable.Current;
					foreach (KeyValuePair<string, SmallIconManager.SmallIcon> keyValuePair2 in keyValuePair.Value)
					{
						if (keyValuePair2.Key.StartsWith(".") && !keyValuePair2.Value.IsCustom && renderDefaultTable)
						{
							list.Add(keyValuePair2.Key);
							list2.Add(ThemeFileList.GetNameFromId(keyValuePair2.Value.ThemeId));
						}
						if (keyValuePair2.Key.StartsWith(".") && keyValuePair2.Value.IsCustom && !renderDefaultTable)
						{
							list.Add(keyValuePair2.Key);
							list2.Add(keyValuePair2.Value.CustomUrl);
						}
					}
				}
			}
			string variableName;
			string variableName2;
			if (renderDefaultTable)
			{
				variableName = "a_rgDE";
				variableName2 = "a_rgDI";
			}
			else
			{
				variableName = "a_rgCE";
				variableName2 = "a_rgCI";
			}
			RenderingUtilities.RenderJavascriptStringArray(writer, variableName, list);
			RenderingUtilities.RenderJavascriptStringArray(writer, variableName2, list2);
		}

		public static void RenderAttachmentBlockingPolicy(TextWriter writer, UserContext userContext)
		{
			RenderingUtilities.RenderAttachmentBlockingPolicy(writer, userContext, true);
		}

		internal static void RenderAttachmentBlockingPolicy(TextWriter writer, UserContext userContext, bool renderScriptTag)
		{
			AttachmentPolicy attachmentPolicy;
			if (userContext != null)
			{
				attachmentPolicy = userContext.AttachmentPolicy;
			}
			else
			{
				attachmentPolicy = OwaConfigurationManager.Configuration.AttachmentPolicy;
			}
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			List<string> list4 = new List<string>();
			List<string> list5 = new List<string>();
			List<string> list6 = new List<string>();
			List<string> list7 = new List<string>();
			List<string> list8 = new List<string>();
			using (SortedDictionary<string, AttachmentPolicy.Level>.Enumerator fileTypeLevels = attachmentPolicy.FileTypeLevels)
			{
				using (SortedDictionary<string, AttachmentPolicy.Level>.Enumerator mimeTypeLevels = attachmentPolicy.MimeTypeLevels)
				{
					while (fileTypeLevels.MoveNext())
					{
						KeyValuePair<string, AttachmentPolicy.Level> keyValuePair = fileTypeLevels.Current;
						switch (keyValuePair.Value)
						{
						case AttachmentPolicy.Level.Block:
							list.Add(keyValuePair.Key.ToLowerInvariant());
							break;
						case AttachmentPolicy.Level.ForceSave:
							list2.Add(keyValuePair.Key.ToLowerInvariant());
							break;
						case AttachmentPolicy.Level.Allow:
							list3.Add(keyValuePair.Key.ToLowerInvariant());
							break;
						case AttachmentPolicy.Level.Unknown:
							list4.Add(keyValuePair.Key.ToLowerInvariant());
							break;
						}
					}
					while (mimeTypeLevels.MoveNext())
					{
						KeyValuePair<string, AttachmentPolicy.Level> keyValuePair2 = mimeTypeLevels.Current;
						switch (keyValuePair2.Value)
						{
						case AttachmentPolicy.Level.Block:
							list5.Add(keyValuePair2.Key.ToLowerInvariant());
							break;
						case AttachmentPolicy.Level.ForceSave:
							list6.Add(keyValuePair2.Key.ToLowerInvariant());
							break;
						case AttachmentPolicy.Level.Allow:
							list7.Add(keyValuePair2.Key.ToLowerInvariant());
							break;
						case AttachmentPolicy.Level.Unknown:
							list8.Add(keyValuePair2.Key.ToLowerInvariant());
							break;
						}
					}
				}
			}
			if (renderScriptTag)
			{
				Utilities.RenderScriptTagStart(writer);
			}
			RenderingUtilities.RenderJavascriptStringArray(writer, "a_rgBkFT", list);
			RenderingUtilities.RenderJavascriptStringArray(writer, "a_rgFsFT", list2);
			RenderingUtilities.RenderJavascriptStringArray(writer, "a_rgAwFT", list3);
			RenderingUtilities.RenderJavascriptStringArray(writer, "a_rgBkMT", list5);
			RenderingUtilities.RenderJavascriptStringArray(writer, "a_rgFsMT", list6);
			RenderingUtilities.RenderJavascriptStringArray(writer, "a_rgAwMT", list7);
			IEnumerable<string> strings = null;
			try
			{
				strings = ProtectorsManager.Instance.ProtectableFileExtensions;
			}
			catch (AttachmentProtectionException)
			{
			}
			RenderingUtilities.RenderJavascriptStringArray(writer, "a_rgProtectableFileType", strings);
			RenderingUtilities.RenderInteger(writer, "a_iMED", AttachmentPolicy.MaxEmbeddedDepth);
			RenderingUtilities.RenderInteger(writer, "a_iAPTUTA", (int)attachmentPolicy.TreatUnknownTypeAs);
			RenderingUtilities.RenderInteger(writer, "a_iAPDFA", attachmentPolicy.DirectFileAccessEnabled ? 1 : 0);
			if (renderScriptTag)
			{
				Utilities.RenderScriptTagEnd(writer);
			}
		}

		internal static void RenderSendOnBehalf(TextWriter writer, UserContext userContext, Participant from)
		{
			Participant sender = new Participant(userContext.ExchangePrincipal.MailboxInfo.DisplayName, userContext.ExchangePrincipal.LegacyDn, "EX");
			SanitizedHtmlString value = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-165544498), new object[]
			{
				RenderingUtilities.GetSender(userContext, sender, "spnDelgt", null),
				RenderingUtilities.GetSender(userContext, from, "spnPrnpl", null)
			});
			writer.Write(value);
		}

		public static void RenderDefaultFolderTypes(TextWriter writer)
		{
			RenderingUtilities.RenderInteger(writer, "DFT_ROT", 34);
			RenderingUtilities.RenderInteger(writer, "DFT_IBX", 5);
			RenderingUtilities.RenderInteger(writer, "DFT_CAL", 1);
			RenderingUtilities.RenderInteger(writer, "DFT_CNT", 2);
			RenderingUtilities.RenderInteger(writer, "DFT_TSK", 11);
			RenderingUtilities.RenderInteger(writer, "DFT_TDS", 19);
			RenderingUtilities.RenderInteger(writer, "DFT_NTS", 8);
			RenderingUtilities.RenderInteger(writer, "DFT_OBX", 9);
			RenderingUtilities.RenderInteger(writer, "DFT_JNK", 6);
			RenderingUtilities.RenderInteger(writer, "DFT_DFT", 4);
			RenderingUtilities.RenderInteger(writer, "DFT_DEL", 3);
			RenderingUtilities.RenderInteger(writer, "DFT_JNL", 7);
			RenderingUtilities.RenderInteger(writer, "DFT_SNT", 10);
			RenderingUtilities.RenderInteger(writer, "DFT_RSS", 17);
			RenderingUtilities.RenderInteger(writer, "DFT_ERT", 20);
			RenderingUtilities.RenderInteger(writer, "DFT_SFR", 36);
		}

		internal static void RenderNavigationTreeDirtyFlag(TextWriter writer, UserContext userContext, NavigationTreeDirtyFlag flag, params NavigationModule[] clientNavigationModules)
		{
			if (!userContext.CanActAsOwner)
			{
				return;
			}
			List<NavigationNodeGroupSection> list = new List<NavigationNodeGroupSection>();
			HashSet<NavigationModule> hashSet = new HashSet<NavigationModule>(clientNavigationModules);
			if (Utilities.IsFlagSet((int)flag, 1) && hashSet.Contains(NavigationModule.Mail))
			{
				list.Add(NavigationNodeGroupSection.First);
			}
			if (Utilities.IsFlagSet((int)flag, 2) && hashSet.Contains(NavigationModule.Calendar))
			{
				list.Add(NavigationNodeGroupSection.Calendar);
			}
			if (Utilities.IsFlagSet((int)flag, 4) && hashSet.Contains(NavigationModule.Contacts))
			{
				list.Add(NavigationNodeGroupSection.Contacts);
			}
			if (Utilities.IsFlagSet((int)flag, 8) && hashSet.Contains(NavigationModule.Tasks))
			{
				list.Add(NavigationNodeGroupSection.Tasks);
			}
			if (list.Count > 0)
			{
				NavigationHost.RenderFavoritesAndNavigationTrees(writer, userContext, null, list.ToArray());
			}
		}

		internal static void RenderExpando(TextWriter writer, string name, string value)
		{
			RenderingUtilities.RenderExpando(writer, name, Utilities.SanitizeHtmlEncode(value));
		}

		internal static void RenderExpando(TextWriter writer, string name, SanitizedHtmlString value)
		{
			writer.Write(" ");
			writer.Write(name);
			writer.Write("=\"");
			writer.Write(value);
			writer.Write("\"");
		}

		internal static void RenderInlineSpacer(TextWriter writer, UserContext userContext, ushort width)
		{
			RenderingUtilities.RenderInlineSpacer(writer, userContext, width, null);
		}

		internal static void RenderInlineSpacer(TextWriter writer, UserContext userContext, ushort width, string id)
		{
			writer.Write("<img");
			if (!string.IsNullOrEmpty(id))
			{
				writer.Write(" id=\"");
				writer.Write(id);
				writer.Write("\"");
			}
			writer.Write(" style=\"height:1px;width:");
			writer.Write((int)width);
			writer.Write("px\" src=\"");
			userContext.RenderThemeFileUrl(writer, ThemeFileId.Clear1x1);
			writer.Write("\">");
		}

		internal static void RenderExpando(TextWriter writer, string name, int value)
		{
			writer.Write(" ");
			writer.Write(name);
			writer.Write("=\"");
			writer.Write(value);
			writer.Write("\"");
		}

		internal static void RenderDropShadowsForButtonWithDialog(TextWriter output, ISessionContext sessionContext)
		{
			RenderingUtilities.RenderDropShadows(output, sessionContext, true, false);
		}

		internal static void RenderDropShadowsForDialogWithButton(TextWriter output, ISessionContext sessionContext)
		{
			RenderingUtilities.RenderDropShadows(output, sessionContext, false, true);
		}

		internal static void RenderDropShadows(TextWriter output, ISessionContext sessionContext)
		{
			RenderingUtilities.RenderDropShadows(output, sessionContext, true, true);
		}

		public static void RenderCanaryParameters(TextWriter writer)
		{
			if (Globals.CanaryProtectionRequired)
			{
				RenderingUtilities.RenderStringVariable(writer, "a_sUCCkStr", "UserContext");
				RenderingUtilities.RenderStringVariable(writer, "a_sCnNm", "canary");
			}
		}

		public static void RenderAlertBarButtons(TextWriter output, OwaContext owaContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			UserContext userContext = owaContext.UserContext;
			output.Write("<div id=\"divAlertBar\"");
			if (userContext.BrowserPlatform == BrowserPlatform.Macintosh)
			{
				if (userContext.BrowserType == BrowserType.Firefox)
				{
					output.Write(" class=\"macBrowserFF\"");
				}
				else
				{
					output.Write(" class=\"macBrowser\"");
				}
			}
			output.Write(">");
			output.Write("<div id=\"divAlertSpace\" class=\"fltAfter\"></div>");
			output.Write("<a id=\"lnkHelp\" class=\"alertBtn alertBtnHelp alertBtnDf\" href=\"#\" name=\"lnkNB\" _ntf=\"1\">");
			userContext.RenderThemeImage(output, ThemeFileId.Help, "alertBtnImg", new object[0]);
			userContext.RenderThemeImage(output, ThemeFileId.DownButton3, "alertBtnHelpImgDD", new object[0]);
			output.Write("<SPAN class=\"spnAlertHelpTxt\">&nbsp;</span>");
			output.Write("<DIV id=\"lnkHelpBorder\" class=\"alertBtnBorder\">");
			RenderingUtilities.RenderDropShadowsForButtonWithDialog(output, owaContext.SessionContext);
			output.Write("</DIV>");
			output.Write("</a>");
			output.Write("<a id=\"lnkOptions\" class=\"alertBtn alertBtnTxtOnly alertBtnDf\" href=\"#\" name=\"lnkNB\" _ntf=\"1\">");
			output.Write("<span id=\"spnOptions\" class=\"alertBtnTxt\">");
			output.Write(LocalizedStrings.GetHtmlEncoded(1511584348));
			output.Write("</span>");
			userContext.RenderThemeImage(output, ThemeFileId.DownButton3, "alertBtnOptImgDD", new object[0]);
			output.Write("<DIV id=\"lnkOptionsBorder\" class=\"alertBtnBorder\">");
			RenderingUtilities.RenderDropShadowsForButtonWithDialog(output, owaContext.SessionContext);
			output.Write("</DIV>");
			output.Write("</a>");
			string homePageForMailboxUser = Utilities.GetHomePageForMailboxUser(owaContext);
			if (!string.IsNullOrEmpty(homePageForMailboxUser))
			{
				output.Write("<a id=\"lnkMySite\" class=\"alertBtn alertBtnTxtOnly alertBtnDf\" href=\"#\" name=\"lnkNB\" _ntf=\"1\" sMySiteUrl=\"");
				Utilities.HtmlEncode(homePageForMailboxUser, output);
				output.Write("\"><span id=\"spnMySite\" class=\"alertBtnTxt\">");
				output.Write(LocalizedStrings.GetHtmlEncoded(1273180895));
				output.Write("</span><DIV id=\"lnkMySiteBorder\" class=\"alertBtnBorder\"></DIV></a>");
			}
			output.Write("<div id=\"divFndSmnRw\" class=\"fltAfter\">");
			output.Write("<a id=\"lnkAddrBk\" class=\"alertBtn alertAddrBkBtn alertBtnDf\" href=\"#\" name=\"lnkNB\" _ntf=\"1\">");
			output.Write("<SPAN class=\"spnAlertABTxt\">&nbsp;</span>");
			userContext.RenderThemeImage(output, ThemeFileId.AddressBook2, "alertBtnImg", new object[0]);
			output.Write("<DIV id=\"lnkAddrBkBorder\" class=\"alertBtnBorder\"></DIV>");
			output.Write("</a>");
			new MessageRecipientWell().Render(output, userContext, RecipientWellType.To, RecipientWell.RenderFlags.None, string.Empty, LocalizedStrings.GetNonEncoded(-903656651), "addBdyTxtFaded");
			output.Write("</div>");
			output.Write("<a id=\"lnkRm\" class=\"alertBtn alertBtnMixed alertBtnDf\" href=\"#\" style=\"display:none\" name=\"lnkNB\" _stky=\"1\" _ntf=\"1\">");
			userContext.RenderThemeImage(output, ThemeFileId.ReminderSmall, "alertBtnImg", new object[0]);
			output.Write("<span id=\"spnRmT\" class=\"alertBtnTxt\">0</span>");
			userContext.RenderThemeImage(output, ThemeFileId.DownButton3, "alertBtnImgDD", new object[]
			{
				"id=\"imgRm\""
			});
			output.Write("<DIV id=\"lnkRmBorder\" class=\"alertBtnBorder\">");
			RenderingUtilities.RenderDropShadowsForButtonWithDialog(output, owaContext.SessionContext);
			output.Write("</DIV>");
			output.Write("</a>");
			output.Write("<a id=\"lnkNwVMl\" class=\"alertBtn alertBtnDf alertBtnIconOnly\" href=\"#\" style=\"display:none\" name=\"lnkNB\" _ntf=\"1\">");
			userContext.RenderThemeImage(output, ThemeFileId.VoiceMessage, "alertBtnImg", new object[0]);
			output.Write("<DIV id=\"lnkNwVMlBorder\" class=\"alertBtnBorder\">");
			RenderingUtilities.RenderDropShadowsForButtonWithDialog(output, owaContext.SessionContext);
			output.Write("</DIV>");
			output.Write("</a>");
			output.Write("<a id=\"lnkNwFx\" class=\"alertBtn alertBtnDf alertBtnIconOnly\" href=\"#\" style=\"display:none\" name=\"lnkNB\" _ntf=\"1\">");
			userContext.RenderThemeImage(output, ThemeFileId.FaxMessage, "alertBtnImg", new object[0]);
			output.Write("<DIV id=\"lnkNwFxBorder\" class=\"alertBtnBorder\">");
			RenderingUtilities.RenderDropShadowsForButtonWithDialog(output, owaContext.SessionContext);
			output.Write("</DIV>");
			output.Write("</a>");
			output.Write("<a id=\"lnkBr\" class=\"alertBtn alertBtnMixed alertBtnDf\" href=\"#\" style=\"display:none\" name=\"lnkNB\" _stky=\"1\" _ntf=\"1\">");
			userContext.RenderThemeImage(output, ThemeFileId.AddBuddy, "alertBtnImg", new object[0]);
			output.Write("<span id=\"spnRmT\" class=\"alertBtnTxt\">0</span>");
			userContext.RenderThemeImage(output, ThemeFileId.DownButton3, "alertBtnImgDD", new object[]
			{
				"id=\"imgBr\""
			});
			output.Write("<DIV id=\"lnkBrBorder\" class=\"alertBtnBorder\">");
			RenderingUtilities.RenderDropShadowsForButtonWithDialog(output, owaContext.SessionContext);
			output.Write("</DIV>");
			output.Write("</a>");
			output.Write("<a id=\"lnkAc\" class=\"alertBtn alertBtnMixed alertBtnDf\" href=\"#\" style=\"display:none\" name=\"lnkNB\" _stky=\"1\" _ntf=\"1\">");
			userContext.RenderThemeImage(output, ThemeFileId.Chat, "alertBtnImg", new object[0]);
			output.Write("<span id=\"spnRmT\" class=\"alertBtnTxt\">0</span>");
			userContext.RenderThemeImage(output, ThemeFileId.DownButton3, "alertBtnImgDD", new object[]
			{
				"id=\"imgBr\""
			});
			output.Write("<DIV id=\"lnkAcBorder\" class=\"alertBtnBorder\">");
			RenderingUtilities.RenderDropShadowsForButtonWithDialog(output, owaContext.SessionContext);
			output.Write("</DIV>");
			output.Write("</a>");
			output.Write("<a id=\"lnkNwCht\" class=\"alertBtn alertBtnDf alertBtnIconOnly\" href=\"#\" style=\"display:none\" name=\"lnkNB\" _ntf=\"1\">");
			userContext.RenderThemeImage(output, ThemeFileId.NewChat, "alertBtnImg", new object[0]);
			output.Write("<DIV id=\"lnkNwChtBorder\" class=\"alertBtnBorder\">");
			RenderingUtilities.RenderDropShadowsForButtonWithDialog(output, owaContext.SessionContext);
			output.Write("</DIV>");
			output.Write("</a>");
			output.Write("<a id=\"lnkNwMl\" class=\"alertBtn alertBtnDf alertBtnIconOnly\" style=\"display:none\" href=\"#\" name=\"lnkNB\" _ntf=\"1\">");
			userContext.RenderThemeImage(output, ThemeFileId.EMail, "alertBtnImg", new object[0]);
			output.Write("<DIV id=\"lnkNwMlBorder\" class=\"alertBtnBorder\">");
			RenderingUtilities.RenderDropShadowsForButtonWithDialog(output, owaContext.SessionContext);
			output.Write("</DIV>");
			output.Write("</a>");
			output.Write("<a id=\"lnkNE\" class=\"alertBtn alertBtnDf alertBtnIconOnly\" style=\"display:none\" href=\"#\" name=\"lnkNB\" _ntf=\"1\">");
			userContext.RenderThemeImageWithToolTip(output, ThemeFileId.NotificationError, "alertBtnImg", 1879732715, new string[0]);
			output.Write("<DIV id=\"lnkNEBorder\" class=\"alertBtnBorder\"></DIV>");
			output.Write("</a>");
			output.Write("<div id=\"divAlertMeasure\" class=\"fltAfter\"></div></div>");
		}

		public static void RenderCopyToClipboardLink(TextWriter output, UserContext userContext, string controlId)
		{
			if (userContext.BrowserType == BrowserType.IE)
			{
				if (string.IsNullOrEmpty(controlId))
				{
					throw new ArgumentNullException("controlId");
				}
				output.Write("<div class=\"act\" ");
				Utilities.RenderScriptHandler(output, "onclick", "Owa.Components.Storage.Clipboard.saveContolContentsToClipboard(\"" + controlId + "\");", false);
				output.Write(">");
				userContext.RenderThemeImage(output, ThemeFileId.Copy);
				output.Write(SanitizedHtmlString.FromStringId(-162016381));
				output.Write("</div>");
			}
		}

		public static void RenderFreeBusyQueryErrorStringResource(TextWriter output)
		{
			output.Write("var a_rgASErrs = {");
			output.Write("L_ASErrorTooManyRecipients : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(-419886643));
			output.Write("\",");
			output.Write("L_ASErrorRequestProcessFailure : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(-1117318808));
			output.Write("\",");
			output.Write("L_ASErrorRequestSubmitFailure : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(-1996936457));
			output.Write("\",");
			output.Write("L_ASErrorInvalidSMTPAddress : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(457947879));
			output.Write("\",");
			output.Write("L_ASErrorDirectoryServerBusy : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(-431028489));
			output.Write("\",");
			output.Write("L_ASErrorLocalRecipientServerNotFound : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(990294866));
			output.Write("\",");
			output.Write("L_ASErrorRecipientNotFound : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(646942272));
			output.Write("\",");
			output.Write("L_ASErrorRemoteServerNotFound : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(-1496139982));
			output.Write("\",");
			output.Write("L_ASErrorProxyRequestFailure : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(-1406360037));
			output.Write("\",");
			output.Write("L_ASErrorMailboxServerBusy : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(-742223442));
			output.Write("\",");
			output.Write("L_ASErrorMailboxServerConnectionFailure : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(447019921));
			output.Write("\",");
			output.Write("L_ASErrorResultTooBig : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(1686015507));
			output.Write("\",");
			output.Write("L_ASErrorAccessDeniedByRecipient : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(2091753233));
			output.Write("\",");
			output.Write("L_ASErrorRequestTimeout : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(1872431360));
			output.Write("\",");
			output.Write("L_ASErrorServerError : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(-1505146289));
			output.Write("\",");
			output.Write("L_ASErrorDLTooBig : \"");
			output.Write(RenderingUtilities.GetSanitizedJavascriptString(260633368));
			output.Write("\"};");
		}

		private static SanitizedHtmlString GetSanitizedJavascriptString(Strings.IDs localizedId)
		{
			return SanitizedHtmlString.GetSanitizedStringWithoutEncoding(LocalizedStrings.GetJavascriptEncoded(localizedId));
		}

		public static void RenderDeletedItemsFolderIds(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			List<string> deletedItemFolderIds = new List<string>();
			OwaStoreObjectId deletedItemsFolderId = userContext.GetDeletedItemsFolderId(userContext.MailboxSession);
			deletedItemFolderIds.Add(deletedItemsFolderId.ToBase64String());
			if (userContext.ArchiveAccessed)
			{
				userContext.TryLoopArchiveMailboxes(delegate(MailboxSession archiveSession)
				{
					deletedItemsFolderId = userContext.GetDeletedItemsFolderId(archiveSession);
					deletedItemFolderIds.Add(deletedItemsFolderId.ToBase64String());
				});
			}
			RenderingUtilities.RenderJavascriptStringArray(output, deletedItemFolderIds);
		}

		public static void RenderAttachmentItems(TextWriter writer, ArrayList attachmentWellInfoObjects, UserContext userContext)
		{
			writer.Write("<div id=attItms>");
			AttachmentWell.RenderAttachments(writer, AttachmentWellType.ReadWrite, attachmentWellInfoObjects, userContext);
			writer.Write("</div>");
		}

		public static void RenderGradientDivider(TextWriter writer, UserContext userContext)
		{
			RenderingUtilities.RenderGradientDivider(writer, userContext, ThemeFileId.HorizontalDividerImageLeft, ThemeFileId.HorizontalDividerImageRight);
		}

		public static void RenderGradientDivider(TextWriter writer, UserContext userContext, ThemeFileId leftImageId, ThemeFileId rightImageId)
		{
			userContext.RenderThemeImage(writer, leftImageId, null, new object[]
			{
				"id=\"divDividerLeft\""
			});
			userContext.RenderThemeImage(writer, rightImageId, null, new object[]
			{
				"id=\"divDividerRight\""
			});
			writer.Write("<div id=\"divDividerTile\"></div>");
		}

		public static void RenderJunkMailActionIcons(TextWriter writer, UserContext userContext)
		{
			writer.Write("<div id=\"divActionIcons\"><span id=\"spnNotJunk\">");
			writer.Write(SanitizedHtmlString.FromStringId(856598503));
			writer.Write("</span></div>");
		}

		public static void RenderActiveActionIcons(TextWriter writer, UserContext userContext)
		{
			RenderingUtilities.RenderActiveActionIcons(writer, userContext, false);
		}

		public static void RenderActiveActionIcons(TextWriter writer, UserContext userContext, bool isPost)
		{
			writer.Write("<div id=\"divActionIcons\"");
			if (isPost)
			{
				writer.Write(" class=\"postActions\"");
			}
			writer.Write(">");
			userContext.RenderThemeImage(writer, ThemeFileId.ReplyActiveIcon, null, new object[]
			{
				SanitizedHtmlString.Format("id=\"imgReplyIcon\" title=\"{0}\"", new object[]
				{
					SanitizedHtmlString.FromStringId(-327372070)
				})
			});
			if (isPost)
			{
				userContext.RenderThemeImage(writer, ThemeFileId.PostReplyActive, null, new object[]
				{
					SanitizedHtmlString.Format("id=\"imgPostReplyIcon\" title=\"{0}\"", new object[]
					{
						SanitizedHtmlString.FromStringId(-1780771632)
					})
				});
			}
			else
			{
				userContext.RenderThemeImage(writer, ThemeFileId.ReplyAllActiveIcon, null, new object[]
				{
					SanitizedHtmlString.Format("id=\"imgReplyAllIcon\" title=\"{0}\"", new object[]
					{
						SanitizedHtmlString.FromStringId(826363927)
					})
				});
			}
			userContext.RenderThemeImage(writer, ThemeFileId.ForwardActiveIcon, null, new object[]
			{
				SanitizedHtmlString.Format("id=\"imgForwardIcon\" title=\"{0}\"", new object[]
				{
					SanitizedHtmlString.FromStringId(-1428116961)
				})
			});
			writer.Write("</div>");
		}

		public static void RenderGhostActionIcons(TextWriter writer, UserContext userContext)
		{
			RenderingUtilities.RenderGhostActionIcons(writer, userContext, false);
		}

		public static void RenderGhostActionIcons(TextWriter writer, UserContext userContext, bool isPost)
		{
			writer.Write("<div id=\"divActionIcons\" ");
			if (isPost)
			{
				writer.Write("class = \"postActions\"");
			}
			writer.Write(">");
			userContext.RenderThemeImage(writer, ThemeFileId.ReplyGhostIcon, null, new object[]
			{
				SanitizedHtmlString.Format("id=\"imgReplyIcon\" style=\"display:none\" title=\"{0}\"", new object[]
				{
					SanitizedHtmlString.FromStringId(-327372070)
				})
			});
			if (isPost)
			{
				userContext.RenderThemeImage(writer, ThemeFileId.PostReplyGhost, null, new object[]
				{
					SanitizedHtmlString.Format("id=\"imgPostReplyIcon\" title=\"{0}\"", new object[]
					{
						SanitizedHtmlString.FromStringId(-1780771632)
					})
				});
			}
			else
			{
				userContext.RenderThemeImage(writer, ThemeFileId.ReplyAllGhostIcon, null, new object[]
				{
					SanitizedHtmlString.Format("id=\"imgReplyAllIcon\" title=\"{0}\"", new object[]
					{
						SanitizedHtmlString.FromStringId(826363927)
					})
				});
			}
			userContext.RenderThemeImage(writer, ThemeFileId.ForwardGhostIcon, null, new object[]
			{
				SanitizedHtmlString.Format("id=\"imgForwardIcon\" style=\"display:none\" title=\"{0}\"", new object[]
				{
					SanitizedHtmlString.FromStringId(-1428116961)
				})
			});
			writer.Write("</div>");
		}

		public static void RenderPlayOnPhoneVariables(TextWriter writer)
		{
			RenderingUtilities.RenderInteger(writer, "a_iClStsIdl", 0);
			RenderingUtilities.RenderInteger(writer, "a_iClStsCnctn", 1);
			RenderingUtilities.RenderInteger(writer, "a_iClStsCnctd", 3);
			RenderingUtilities.RenderInteger(writer, "a_iClStsDsCnctd", 4);
			RenderingUtilities.RenderStringVariable(writer, "a_sDialMsg", -2047301120);
			RenderingUtilities.RenderStringVariable(writer, "a_sHngupMsg", -888079595);
			RenderingUtilities.RenderStringVariable(writer, "a_sDialngMsg", 2079242004);
			RenderingUtilities.RenderStringVariable(writer, "a_sPlyngMsg", -1813432087);
			RenderingUtilities.RenderStringVariable(writer, "L_PlyTtl", 1086057849);
			RenderingUtilities.RenderStringVariable(writer, "L_DialNumber", -399835767);
		}

		public static void RenderPlayOnPhoneDialog(TextWriter writer, UserContext userContext, string phoneNumber)
		{
			writer.Write("<div id=\"divDlgMarkup\" style=\"display:none;\">");
			writer.Write("  <div id=divmainbox>");
			writer.Write("        <div id=divgraybox>");
			writer.Write("            <div>");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-399835767));
			writer.Write("           </div>");
			writer.Write("            <input type=text id=txtPhNum value=\"");
			writer.Write(Utilities.HtmlEncode(phoneNumber));
			writer.Write("\" maxlength=\"454\">&nbsp;&nbsp;");
			writer.Write("            <button id=btnDial ");
			Utilities.RenderScriptHandler(writer, "onmouseover", "btnOnMsOvrBtn(_this);");
			writer.Write(" ");
			Utilities.RenderScriptHandler(writer, "onmouseout", "btnOnMsOutBtn(_this);");
			writer.Write(">");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-2047301120));
			writer.Write("            </button>");
			writer.Write("        </div>");
			writer.Write("        <div id=divwhitebox>");
			writer.Write("            <div id=divPrg>");
			writer.Write("            <img align=absmiddle src=\"");
			userContext.RenderThemeFileUrl(writer, ThemeFileId.ProgressSmall);
			writer.Write("\">");
			writer.Write("            <div id=divPTxt></div>");
			writer.Write("            </div>");
			writer.Write("        </div>");
			writer.Write("    </div>");
			writer.Write("</div>");
		}

		public static void RenderAutoSaveVariables(TextWriter writer, UserContext userContext)
		{
			int autoSaveInterval = Globals.AutoSaveInterval;
			int timeZoneOffset = Utilities.GetTimeZoneOffset(userContext);
			RenderingUtilities.RenderInteger(writer, "a_iASI", autoSaveInterval);
			RenderingUtilities.RenderStringVariable(writer, "L_DrftAutoSavedAt", 1992418784);
			RenderingUtilities.RenderStringVariable(writer, "L_DrftSavedAt", -976959931);
			RenderingUtilities.RenderStringVariable(writer, "L_ErrTimeoutAutosave", 73931748);
			RenderingUtilities.RenderOptionalStringVariable(writer, "L_AM", Culture.AMDesignator);
			RenderingUtilities.RenderOptionalStringVariable(writer, "L_PM", Culture.PMDesignator);
			RenderingUtilities.RenderStringVariable(writer, "a_sShtTmFmt", Utilities.JavascriptEncode(userContext.UserOptions.TimeFormat));
			RenderingUtilities.RenderInteger(writer, "a_iTzOffset", timeZoneOffset);
		}

		public const int LARGE_DISPLAY_PICTURE_SIZE = 64;

		public const int SMALL_DISPLAY_PICTURE_SIZE = 32;

		private static string mimeCtrlAttribs = "tzi=\"-480;0,0,0,0,0,0,0;0;0,0,0,0,0,0,0;-60\" onMimeActionEnd=\"return onMmActEnd()\" acceptLanguage=\"en-us\" preferredCharset=\"iso-8859-1\" style=\"display:none;\" beaconSubstitute=\"transparentGif\" redirPrefix=\"{0}\"smimeRedirPrefix=\"{1}\"disableAddAttachment=\"false\" ";

		[Flags]
		internal enum SMimeSecurityFlags : uint
		{
			None = 0U,
			SendChainNoRoot = 1U,
			SendChain = 2U,
			TempBufferNoEncrypt = 4U,
			SigningCertOnly = 8U,
			EncryptSecureReceipts = 32U,
			RecipOneForAllBcc = 64U,
			RecipAll = 128U,
			SendSmimeCapabilities = 256U,
			CopyRecipHeaders = 512U,
			UserReceivedCerts = 1024U,
			UseEmbeddedMessageFileNameAsAttachmentName = 2048U,
			GenerateMIdFromString = 4096U
		}
	}
}
