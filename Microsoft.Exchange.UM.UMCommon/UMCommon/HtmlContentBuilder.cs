using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class HtmlContentBuilder : MessageContentBuilder
	{
		private string TdStyle
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "font-family: {0}; color: #000000; border-width: 0in; font-size:10pt; vertical-align: top; padding-left: 10px; padding-right: 10px;", new object[]
				{
					Strings.Font1.ToString(base.Culture)
				});
			}
		}

		private string IndentedStyle
		{
			get
			{
				return "margin-left: 12px;";
			}
		}

		private string NoTranscriptionStyle
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "font-family: {0}; font-size: 10pt; color: #000066; font-weight: bold;", new object[]
				{
					Strings.Font2.ToString(base.Culture)
				});
			}
		}

		private string NoTranscriptionDetailsStyle
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "font-family: {0}; font-size: 10pt; color: #3b3b3b;", new object[]
				{
					Strings.Font2.ToString(base.Culture)
				});
			}
		}

		private string DefaultStyle
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "font-family: {0}; background-color: #ffffff; color: #000000; font-size:10pt;", new object[]
				{
					Strings.Font1.ToString(base.Culture)
				});
			}
		}

		private string CallInfoHeaderStyle
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "font-family: {0}; color: #686a6b; font-size:10pt;border-width: 0in;", new object[]
				{
					Strings.Font1.ToString(base.Culture)
				});
			}
		}

		private string TitleStyle
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "font-family: {0}; color: #000066; margin: 0in; font-size: 10pt; font-weight:bold; ", new object[]
				{
					Strings.Font2.ToString(base.Culture)
				});
			}
		}

		internal string MessageHeaderStyle
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "font-family: {0}; font-size: 10pt; color:#000066; font-weight: bold;", new object[]
				{
					Strings.Font2.ToString(base.Culture)
				});
			}
		}

		internal string DividerMarginStyle
		{
			get
			{
				return "margin-bottom:10px";
			}
		}

		internal HtmlContentBuilder(CultureInfo c, UMDialPlan rcptDialPlan) : base(c, rcptDialPlan)
		{
		}

		internal HtmlContentBuilder(CultureInfo c) : this(c, null)
		{
		}

		protected override string EmailHeaderLine
		{
			get
			{
				return "<hr/>";
			}
		}

		protected override string CalendarHeaderLine
		{
			get
			{
				return "<hr/>";
			}
		}

		internal override void AddVoicemailBody(PhoneNumber callerId, ContactInfo resolvedCallerInfo, string additionalText, ITranscriptionData transcriptionData)
		{
			this.AddVoicemailBody(callerId, true, resolvedCallerInfo, additionalText, transcriptionData);
		}

		internal override void AddVoicemailBody(ContactInfo resolvedCallerInfo, string additionalText)
		{
			this.AddVoicemailBody(PhoneNumber.Empty, false, resolvedCallerInfo, additionalText, null);
		}

		internal override void AddNDRBodyForCADRM(PhoneNumber callerId, ContactInfo resolvedCallerInfo, ExDateTime sentTime)
		{
			LocalizedString message;
			if (!string.IsNullOrEmpty(resolvedCallerInfo.DisplayName))
			{
				message = Strings.CallAnsweringNDRForDRMCallerResolved(resolvedCallerInfo.DisplayName);
			}
			else
			{
				message = Strings.CallAnsweringNDRForDRMCallerUnResolved(MessageContentBuilder.FormatCallerId(callerId, base.Culture));
			}
			this.AddUMMessageBody(callerId, true, resolvedCallerInfo, message, Strings.CallAnsweringNDRForDRMFooter.ToString(base.Culture), null, HtmlContentBuilder.TypeOfUMMessage.CallAnsweringNDR, sentTime.ToString("f", base.Culture), null);
		}

		internal override void AddNDRBodyForInterpersonalDRM(UMSubscriber caller, RecipientDetails recipients, ExDateTime sentTime)
		{
			List<string> list = new List<string>();
			if (recipients.Count > 1)
			{
				foreach (Participant participant in recipients.Participants)
				{
					list.Add(string.Concat(new string[]
					{
						participant.DisplayName,
						"(<a style=\"color: #3399ff; \" href=\"mailto:",
						participant.EmailAddress,
						"\">",
						participant.EmailAddress,
						"</a>)"
					}));
				}
				this.AddNDRBodyForInterpersonalDRM(list, sentTime);
				return;
			}
			if (recipients.IsDistributionList)
			{
				list.Add(string.Concat(new string[]
				{
					recipients.Participants[0].DisplayName,
					"(<a style=\"color: #3399ff; \" href=\"mailto:",
					recipients.Participants[0].EmailAddress,
					"\">",
					recipients.Participants[0].EmailAddress,
					"</a>)"
				}));
				this.AddNDRBodyForInterpersonalDRM(list, sentTime);
				return;
			}
			if (recipients.IsPersonalDistributionList)
			{
				list.Add(recipients.Participants[0].DisplayName);
				this.AddNDRBodyForInterpersonalDRM(list, sentTime);
				return;
			}
			ContactInfo resolvedRecipientInfo = ContactInfo.FindByParticipant(caller, recipients.Participants[0]);
			this.AddNDRBodyForInterpersonalDRM(resolvedRecipientInfo, sentTime);
		}

		internal override void AddFaxBody(PhoneNumber callerId, ContactInfo resolvedCallerInfo, string additionalText)
		{
			LocalizedString faxBodyDisplayLabel = resolvedCallerInfo.GetFaxBodyDisplayLabel(callerId, base.Culture);
			this.AddUMMessageBody(callerId, resolvedCallerInfo, faxBodyDisplayLabel, additionalText);
		}

		internal override void AddMissedCallBody(PhoneNumber callerId, ContactInfo resolvedCallerInfo)
		{
			LocalizedString missedCallBodyDisplayLabel = resolvedCallerInfo.GetMissedCallBodyDisplayLabel(callerId, base.Culture);
			this.AddUMMessageBody(callerId, resolvedCallerInfo, missedCallBodyDisplayLabel, null);
		}

		internal override void AddTeamPickUpBody(string answeredBy, PhoneNumber callerId, ContactInfo callerInfo)
		{
			LocalizedString message = Strings.TeamPickUpBody(this.GetDisplayNameOrCallerId(callerInfo, callerId), answeredBy);
			this.AddUMMessageBody(callerId, callerInfo, message, null);
		}

		internal override void AddCallNotForwardedBody(string target, PhoneNumber callerId, ContactInfo callerInfo)
		{
			LocalizedString message = Strings.CallNotForwardedBody(this.GetDisplayNameOrCallerId(callerInfo, callerId), target);
			this.AddUMMessageBody(callerId, callerInfo, message, Strings.CallNotForwardedText);
		}

		internal override void AddCallForwardedBody(string target, PhoneNumber callerId, ContactInfo callerInfo)
		{
			LocalizedString message = Strings.CallForwardedBody(this.GetDisplayNameOrCallerId(callerInfo, callerId), target);
			this.AddUMMessageBody(callerId, callerInfo, message, null);
		}

		internal override void AddIncomingCallLogBody(PhoneNumber callerId, ContactInfo callerInfo)
		{
			string displayName = this.GetDisplayName(callerInfo);
			LocalizedString message;
			if (!string.IsNullOrEmpty(displayName))
			{
				message = (callerInfo.ResolvesToMultipleContacts ? Strings.IncomingCallLogBodyCallerMultipleResolved(displayName) : Strings.IncomingCallLogBodyCallerResolved(displayName, MessageContentBuilder.FormatCallerId(callerId, base.Culture)));
			}
			else
			{
				message = Strings.IncomingCallLogBodyCallerUnresolved(MessageContentBuilder.FormatCallerId(callerId, base.Culture));
			}
			this.AddUMMessageBody(callerId, callerInfo, message, null);
		}

		internal override void AddOutgoingCallLogBody(PhoneNumber targetPhone, ContactInfo calledPartyInfo)
		{
			string displayName = this.GetDisplayName(calledPartyInfo);
			LocalizedString message;
			if (!string.IsNullOrEmpty(displayName))
			{
				message = (calledPartyInfo.ResolvesToMultipleContacts ? Strings.OutgoingCallLogBodyTargetMultipleResolved(displayName) : Strings.OutgoingCallLogBodyTargetResolved(displayName, MessageContentBuilder.FormatCallerId(targetPhone, base.Culture)));
			}
			else
			{
				message = Strings.OutgoingCallLogBodyTargetUnresolved(MessageContentBuilder.FormatCallerId(targetPhone, base.Culture));
			}
			this.AddUMMessageBody(targetPhone, calledPartyInfo, message, null);
		}

		internal override void AddEnterpriseNotifyMailBody(LocalizedString messageHeader, string[] accessNumbers, string extension, string pin, string additionalText)
		{
			this.AddNotifyMailBody(messageHeader, Strings.AccessMailText, accessNumbers, extension, pin, additionalText);
		}

		internal override void AddConsumerNotifyMailBody(LocalizedString messageHeader, string[] accessNumbers, string extension, string pin, string additionalText)
		{
			this.AddNotifyMailBody(messageHeader, Strings.AccessMailTextConsumer, accessNumbers, extension, pin, additionalText);
		}

		internal override void AddLateForMeetingBody(CalendarItemBase cal, ExTimeZone timeZone, LocalizedString lateInfo)
		{
			this.AddUMMessageBodyPrefix(lateInfo);
			this.AddNewLine();
			this.AddCalendarHeader(cal, timeZone, false);
			this.AddDocumentEnd();
		}

		internal override void AddRecordedReplyText(string displayName)
		{
			this.AddRecordedMessageText(Strings.ReplyWithRecording(displayName));
		}

		internal override void AddRecordedForwardText(string displayName)
		{
			this.AddRecordedMessageText(Strings.ForwardWithRecording(displayName));
		}

		internal override void AddAudioPreview(ITranscriptionData transcriptionData)
		{
			this.AddAudioPreviewHelper(transcriptionData, delegate
			{
				this.AddNewLine();
			});
		}

		protected override string FormatHeaderName(LocalizedString headerName)
		{
			return string.Format(CultureInfo.InvariantCulture, "<b><font size=2 face={0}><span style='font-size:10.0pt;font-family:{0};font-weight:bold'>{1}</span></font></b>", new object[]
			{
				Strings.Font1.ToString(base.Culture),
				AntiXssEncoder.HtmlEncode(headerName.ToString(base.Culture), false)
			});
		}

		protected override string FormatHeaderValue(string headerValue)
		{
			return string.Format(CultureInfo.InvariantCulture, "<font size=2 face={0}><span style='font-size:10.0pt;font-family:{0}'>{1}</span></font>", new object[]
			{
				Strings.Font1.ToString(base.Culture),
				AntiXssEncoder.HtmlEncode(headerValue, false)
			});
		}

		protected override void AddNewLine()
		{
			base.AppendCultureInvariantText("<br/>");
		}

		protected override void AddDivider()
		{
			base.AppendCultureInvariantText("<hr/>");
		}

		protected override void AddDocumentEnd()
		{
			base.AppendCultureInvariantText("</div></BODY></HTML>");
		}

		protected override void AddStart()
		{
			if (base.Culture.TextInfo.IsRightToLeft)
			{
				base.AppendCultureInvariantText("<HTML dir=\"rtl\">");
				return;
			}
			base.AppendCultureInvariantText("<HTML>");
		}

		private void AddNotifyMailBody(LocalizedString messageHeader, LocalizedString accessMailText, string[] accessNumbers, string extension, string pin, string additionalText)
		{
			this.AddUMMessageBodyPrefix(messageHeader);
			this.AddText(accessMailText);
			this.AddNewLine();
			this.AddNewLine();
			this.AddAccessInformation(accessNumbers, extension, pin);
			this.AddVoicemailSettingsDiscoverability();
			if (additionalText.Length > 0)
			{
				this.AddNewLine();
				base.AppendCultureInvariantText(additionalText);
				this.AddNewLine();
			}
			this.AddUMMessageBodySuffix();
		}

		private void AddVoicemailSettingsDiscoverability()
		{
			this.AddNewLine();
			this.AddNewLine();
			this.AddText(Strings.ConfigureVoicemailSettings);
			base.AppendFormat("<ul dir=\"{0}\">", new object[]
			{
				base.Culture.TextInfo.IsRightToLeft ? "rtl" : "ltr"
			});
			this.AddListItem(Strings.VoicemailSettingsInstruction1);
			this.AddListItem(Strings.VoicemailSettingsInstruction2);
			this.AddListItem(Strings.VoicemailSettingsInstruction3);
			base.AppendCultureInvariantText("</ul>");
		}

		private void AddAudioPreviewHelper(ITranscriptionData transcriptionData, Action callerInfoDelegate)
		{
			if (transcriptionData == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "transcriptionData == null. Likely means that User/policy/dialplan must have not been enabled for transcription.", new object[0]);
				if (callerInfoDelegate != null)
				{
					callerInfoDelegate();
				}
				return;
			}
			if (transcriptionData.RecognitionError == RecoErrorType.LanguageNotSupported)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Language is not supported for transcription", new object[0]);
				if (callerInfoDelegate != null)
				{
					callerInfoDelegate();
				}
				return;
			}
			if (transcriptionData.RecognitionResult != RecoResultType.Skipped)
			{
				this.AddTranscriptionDetailMessage(transcriptionData);
				if (callerInfoDelegate != null)
				{
					this.AddDividerMargin();
					callerInfoDelegate();
				}
				return;
			}
			if (callerInfoDelegate != null)
			{
				callerInfoDelegate();
				this.AddDividerMargin();
				this.AddTranscriptionSkippedMessage(transcriptionData);
				return;
			}
			this.AddTranscriptionSkippedMessage(transcriptionData);
		}

		private void AddTranscriptionDetailMessage(ITranscriptionData transcriptionData)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Transcription was successful. Adding audio preview to voicemail message body.", new object[0]);
			base.AppendFormat("<span id=\"UM-ASR-text\" lang=\"{0}\" style=\"{1}\">", new object[]
			{
				base.Culture.TwoLetterISOLanguageName,
				this.DefaultStyle
			});
			base.AppendFormat("<span lang=\"{0}\" dir=\"{1}\">", new object[]
			{
				transcriptionData.Language.TwoLetterISOLanguageName,
				transcriptionData.Language.TextInfo.IsRightToLeft ? "rtl" : "ltr"
			});
			EvmTranscriptWriter evmTranscriptWriter = HtmlContentBuilder.HtmlEvmTranscriptWriter.Create(this, transcriptionData.Language);
			XmlElement documentElement = transcriptionData.TranscriptionXml.DocumentElement;
			evmTranscriptWriter.WriteTranscript(documentElement);
			base.AppendCultureInvariantText("</span>");
			base.AppendCultureInvariantText("</span>");
		}

		private void AddTranscriptionSkippedMessage(ITranscriptionData transcriptionData)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Skipped is set on the ASR data. RecognitionError:{0} ErrorInfo:{1}.", new object[]
			{
				transcriptionData.RecognitionError,
				transcriptionData.ErrorInformation
			});
			this.AddMessageHeader(Strings.NoTranscription);
			LocalizedString localizedText;
			switch (transcriptionData.RecognitionError)
			{
			case RecoErrorType.AudioQualityPoor:
				localizedText = Strings.TranscriptionSkippedDueToPoorAudioQualityDetails;
				goto IL_C7;
			case RecoErrorType.Rejected:
				localizedText = Strings.TranscriptionSkippedDueToRejectionDetails;
				goto IL_C7;
			case RecoErrorType.BadRequest:
				localizedText = Strings.TranscriptionSkippedDueToBadRequestDetails;
				goto IL_C7;
			case RecoErrorType.SystemError:
				localizedText = Strings.TranscriptionSkippedDueToSystemErrorDetails;
				goto IL_C7;
			case RecoErrorType.Timeout:
				localizedText = Strings.TranscriptionSkippedDueToTimeoutDetails;
				goto IL_C7;
			case RecoErrorType.MessageTooLong:
				localizedText = Strings.TranscriptionSkippedDueToLongMessageDetails;
				goto IL_C7;
			case RecoErrorType.ProtectedVoiceMail:
				localizedText = Strings.TranscriptionSkippedDueToProtectedVoiceMail;
				goto IL_C7;
			case RecoErrorType.Throttled:
				localizedText = Strings.TranscriptionSkippedDueToThrottlingDetails;
				goto IL_C7;
			case RecoErrorType.ErrorReadingSettings:
				localizedText = Strings.TranscriptionSkippedDueToErrorReadingSettings;
				goto IL_C7;
			}
			localizedText = Strings.TranscriptionSkippedDefaultDetails;
			IL_C7:
			base.AppendFormat("<div style=\"{0}\">", new object[]
			{
				this.IndentedStyle + this.DefaultStyle
			});
			this.AddTextSpan(localizedText, this.NoTranscriptionDetailsStyle);
			EvmTranscriptWriter evmTranscriptWriter = HtmlContentBuilder.HtmlEvmTranscriptWriter.Create(this, transcriptionData.Language);
			XmlElement documentElement = transcriptionData.TranscriptionXml.DocumentElement;
			evmTranscriptWriter.WriteErrorInformationOnly(documentElement);
			base.AppendCultureInvariantText("</div>");
			this.AddNewLine();
		}

		private void AddDividerMargin()
		{
			base.AppendFormat("<div style=\"{0}\">", new object[]
			{
				this.DividerMarginStyle
			});
			this.AddDivider();
			base.AppendCultureInvariantText("</div>");
		}

		private void AddMessageHeader(LocalizedString messageHeader)
		{
			this.AddMessageHeader(messageHeader, true);
		}

		private void AddMessageHeader(LocalizedString messageHeader, bool escape)
		{
			base.AppendFormat("<div style=\"{0}\">", new object[]
			{
				this.MessageHeaderStyle
			});
			string message;
			if (escape)
			{
				message = AntiXssEncoder.HtmlEncode(messageHeader.ToString(base.Culture), false);
			}
			else
			{
				message = messageHeader.ToString(base.Culture);
			}
			base.AppendCultureInvariantText(message);
			base.AppendFormat("</div><br/>", new object[0]);
		}

		private string RenderStyle(string styleDefinition, LocalizedString fontName)
		{
			return string.Format(CultureInfo.InvariantCulture, styleDefinition, new object[]
			{
				fontName.ToString(base.Culture)
			});
		}

		private void AddRecordedMessageText(LocalizedString text)
		{
			base.AppendFormat("<div style=\"{0}\">", new object[]
			{
				this.TitleStyle
			});
			this.AddText(text);
			base.AppendCultureInvariantText("</div><br/>");
		}

		private void AddVoicemailBody(PhoneNumber callerId, bool addCallerId, ContactInfo resolvedCallerInfo, string additionalText, ITranscriptionData transcriptionData)
		{
			LocalizedString voicemailBodyDisplayLabel = resolvedCallerInfo.GetVoicemailBodyDisplayLabel(callerId, base.Culture);
			this.AddUMMessageBody(callerId, addCallerId, resolvedCallerInfo, voicemailBodyDisplayLabel, additionalText, transcriptionData, HtmlContentBuilder.TypeOfUMMessage.NormalMessage, null, null);
		}

		private void AddNDRBodyForInterpersonalDRM(ContactInfo resolvedRecipientInfo, ExDateTime sentTime)
		{
			this.AddUMMessageBody(PhoneNumber.Empty, false, resolvedRecipientInfo, Strings.InterpersonalNDRForDRM, Strings.InterpersonalNDRForDRMFooter.ToString(base.Culture), null, HtmlContentBuilder.TypeOfUMMessage.InterpersonalNDR, sentTime.ToString("f", base.Culture), null);
		}

		private void AddNDRBodyForInterpersonalDRM(List<string> originalRecipients, ExDateTime sentTime)
		{
			this.AddUMMessageBody(PhoneNumber.Empty, false, null, Strings.InterpersonalNDRForDRM, Strings.InterpersonalNDRForDRMFooter.ToString(base.Culture), null, HtmlContentBuilder.TypeOfUMMessage.InterpersonalNDR, sentTime.ToString("f", base.Culture), originalRecipients);
		}

		private void AddUMMessageBody(PhoneNumber callerId, ContactInfo resolvedCaller, LocalizedString message, LocalizedString additionalText)
		{
			this.AddUMMessageBody(callerId, true, resolvedCaller, message, additionalText.ToString(base.Culture), null, HtmlContentBuilder.TypeOfUMMessage.NormalMessage, null, null);
		}

		private void AddUMMessageBody(PhoneNumber callerId, ContactInfo resolvedCaller, LocalizedString message, string additionalText)
		{
			this.AddUMMessageBody(callerId, true, resolvedCaller, message, additionalText, null, HtmlContentBuilder.TypeOfUMMessage.NormalMessage, null, null);
		}

		private void AddUMMessageBody(PhoneNumber callerId, bool addCallerId, ContactInfo resolvedContact, LocalizedString message, string additionalText, ITranscriptionData transcriptionData, HtmlContentBuilder.TypeOfUMMessage messageType, string sentTime, List<string> originalListOfRecipients)
		{
			Action callerInfoDelegate;
			if (resolvedContact != null)
			{
				callerInfoDelegate = delegate()
				{
					this.AddCallerHeaderInfo(message);
					this.AddCallerInformation(callerId, addCallerId, resolvedContact, messageType, sentTime);
					this.AddCallerHeaderInfoEnd();
				};
			}
			else
			{
				callerInfoDelegate = delegate()
				{
					this.AddCallerHeaderInfo(message);
					this.AddCallerHeaderInfoEnd();
				};
			}
			this.AddUMMessageBodyPrefix(transcriptionData, callerInfoDelegate);
			if (messageType == HtmlContentBuilder.TypeOfUMMessage.InterpersonalNDR && originalListOfRecipients != null)
			{
				this.AddInterpersonalSpecialNDR(originalListOfRecipients, sentTime);
			}
			if (!string.IsNullOrEmpty(additionalText))
			{
				this.AddNewLine();
				base.AppendCultureInvariantText(additionalText);
				this.AddNewLine();
			}
			this.AddUMMessageBodySuffix();
		}

		private void AddCallerHeaderInfo(LocalizedString message)
		{
			base.AppendFormat("<div id=\"UM-call-info\" lang=\"{0}\">", new object[]
			{
				base.Culture.TwoLetterISOLanguageName
			});
			this.AddMessageHeader(message, false);
		}

		private void AddCallerHeaderInfoEnd()
		{
			base.AppendCultureInvariantText("</div>");
		}

		private void AddInterpersonalSpecialNDR(List<string> originalListOfRecipients, string sentTime)
		{
			base.AppendCultureInvariantText("<table border=\"0\" width=\"100%\">");
			if (originalListOfRecipients.Count == 1)
			{
				this.AddTableEntry(Strings.Recipient, originalListOfRecipients[0]);
			}
			else
			{
				this.AddTableEntry(Strings.Recipients, originalListOfRecipients[0]);
				for (int i = 1; i < originalListOfRecipients.Count; i++)
				{
					this.AddTableEntry(LocalizedString.Empty, originalListOfRecipients[i]);
				}
			}
			this.AddTableEntry(Strings.Sent, sentTime);
			base.AppendCultureInvariantText("</table>");
		}

		private void AddUMMessageBodyPrefix(LocalizedString message)
		{
			this.AddUMMessageBodyPrefix(null, delegate()
			{
				this.AddCallerHeaderInfo(message);
				this.AddCallerHeaderInfoEnd();
			});
		}

		private void AddStylesheet()
		{
			base.AppendCultureInvariantText("<style type=\"text/css\"> a:link { color: #3399ff; } a:visited { color: #3366cc; } a:active { color: #ff9900; } </style>");
		}

		private void AddUMMessageBodyPrefix(ITranscriptionData transcriptionData, Action callerInfoDelegate)
		{
			this.AddStart();
			base.AppendCultureInvariantText("<head>");
			this.AddStylesheet();
			base.AppendCultureInvariantText("</head>");
			base.AppendCultureInvariantText("<body>");
			this.AddStylesheet();
			base.AppendFormat("<div style=\"{0}\">", new object[]
			{
				this.DefaultStyle
			});
			this.AddAudioPreviewHelper(transcriptionData, callerInfoDelegate);
		}

		private void AddUMMessageBodySuffix()
		{
			this.AddDocumentEnd();
		}

		private void AddTelOrSipUriEntry(LocalizedString label, string phoneOrSipUri)
		{
			PhoneNumber phoneNumber;
			if (PhoneNumber.TryParse(base.RecipientDialPlan, phoneOrSipUri, out phoneNumber))
			{
				this.AddTelOrSipUriEntry(label, phoneNumber);
				return;
			}
			this.AddTableEntry(Strings.CallerId, phoneOrSipUri);
		}

		private void AddTelOrSipUriEntry(LocalizedString label, PhoneNumber phoneNumber)
		{
			string anchor = string.Empty;
			string display = string.Empty;
			if (phoneNumber.UriType == UMUriType.SipName)
			{
				string text = Utils.RemoveSIPPrefix(phoneNumber.ToDial);
				anchor = text;
				display = text;
			}
			else
			{
				anchor = AntiXssEncoder.HtmlEncode(phoneNumber.ToDial, false);
				display = AntiXssEncoder.HtmlEncode(phoneNumber.ToDisplay, false);
			}
			this.AddTableEntry(label, MessageContentBuilder.FormatTelOrSipEntryAsAnchor(anchor, display, phoneNumber.UriType));
		}

		private void AddCallerInformation(PhoneNumber callerId, bool addCallerId, ContactInfo contactInfo, HtmlContentBuilder.TypeOfUMMessage messageType, string sentTime)
		{
			base.AppendCultureInvariantText("<table border=\"0\" style=\"width:100%; table-layout:auto;\">");
			if (contactInfo != null)
			{
				if (addCallerId)
				{
					if (!callerId.IsEmpty)
					{
						this.AddTelOrSipUriEntry(Strings.CallerId, callerId);
					}
					else
					{
						this.AddTableEntry(Strings.CallerId, MessageContentBuilder.FormatCallerId(callerId, base.Culture));
					}
				}
				if (messageType == HtmlContentBuilder.TypeOfUMMessage.InterpersonalNDR && !string.IsNullOrEmpty(contactInfo.DisplayName))
				{
					this.AddTableEntry(Strings.Recipient, contactInfo.DisplayName);
				}
				if (!string.IsNullOrEmpty(contactInfo.Title))
				{
					this.AddTableEntry(Strings.JobTitle, contactInfo.Title);
				}
				if (!string.IsNullOrEmpty(contactInfo.Company))
				{
					this.AddTableEntry(Strings.Company, contactInfo.Company);
				}
				if (!string.IsNullOrEmpty(contactInfo.BusinessPhone))
				{
					this.AddTelOrSipUriEntry(Strings.WorkPhone, contactInfo.BusinessPhone);
				}
				if (!string.IsNullOrEmpty(contactInfo.MobilePhone))
				{
					this.AddTelOrSipUriEntry(Strings.MobilePhone, contactInfo.MobilePhone);
				}
				if (!string.IsNullOrEmpty(contactInfo.HomePhone))
				{
					this.AddTelOrSipUriEntry(Strings.HomePhone, contactInfo.HomePhone);
				}
				if (!string.IsNullOrEmpty(contactInfo.EMailAddress))
				{
					this.AddTableEntry(Strings.Email, string.Concat(new string[]
					{
						"<a style=\"color: #3399ff; \" href=\"mailto:",
						contactInfo.EMailAddress,
						"\">",
						contactInfo.EMailAddress,
						"</a>"
					}));
				}
				if (messageType == HtmlContentBuilder.TypeOfUMMessage.CallAnsweringNDR || messageType == HtmlContentBuilder.TypeOfUMMessage.InterpersonalNDR)
				{
					this.AddTableEntry(Strings.Sent, sentTime);
				}
			}
			base.AppendCultureInvariantText("</table>");
		}

		private void AddTableEntry(LocalizedString name, string value)
		{
			base.AppendFormat("<tr><td width=\"15%\" nowrap style=\"{0}\">", new object[]
			{
				this.CallInfoHeaderStyle
			});
			if (!name.IsEmpty)
			{
				base.Append(name);
				base.AppendCultureInvariantText(":</td>");
			}
			else
			{
				base.AppendCultureInvariantText("</td>");
			}
			base.AppendFormat("<td width=\"85%\" style=\"{0}\">", new object[]
			{
				this.TdStyle
			});
			base.AppendCultureInvariantText(value);
			base.AppendCultureInvariantText("</td></tr>");
		}

		private void AddTableEntry(LocalizedString name, string[] values)
		{
			StringBuilder stringBuilder = new StringBuilder(values[0]);
			for (int i = 1; i < values.Length; i++)
			{
				stringBuilder.Append(" " + Strings.AccessNumberSeparator.ToString(base.Culture) + " " + values[i]);
			}
			this.AddTableEntry(name, stringBuilder.ToString());
		}

		private void AddListItem(string value)
		{
			base.AppendCultureInvariantText("<li>");
			base.AppendCultureInvariantText(AntiXssEncoder.HtmlEncode(value, false));
			base.AppendCultureInvariantText("</li>");
		}

		private void AddListItem(LocalizedString localizedValue)
		{
			this.AddListItem(localizedValue.ToString(base.Culture));
		}

		private void AddAccessInformation(string[] accessNumbers, string extension, string pin)
		{
			base.AppendCultureInvariantText("<table border=\"0\" style=\"width:100%; table-layout:auto;\">");
			if (accessNumbers != null && accessNumbers.Length > 0)
			{
				this.AddTableEntry(Strings.AccessNumber, accessNumbers);
			}
			if (extension != null && extension.Length > 0)
			{
				this.AddTableEntry(Strings.Extension, extension);
			}
			if (pin != null && pin.Length > 0)
			{
				this.AddTableEntry(Strings.Pin, pin);
			}
			base.AppendCultureInvariantText("</table>");
		}

		private void AddTextSpan(string text, string spanStyle)
		{
			this.AddTextSpan(text, spanStyle, true);
		}

		private void AddTextSpan(string text, string spanStyle, bool escape)
		{
			if (escape)
			{
				text = AntiXssEncoder.HtmlEncode(text, false);
			}
			text = text.Replace("  ", "&nbsp;&nbsp;");
			base.AppendFormat("<span style=\"{0}\">{1}</span>", new object[]
			{
				spanStyle,
				text
			});
		}

		private void AddTextSpan(LocalizedString localizedText, string spanStyle)
		{
			this.AddTextSpan(localizedText.ToString(base.Culture), spanStyle);
		}

		private void AddText(string text)
		{
			base.AppendCultureInvariantText(AntiXssEncoder.HtmlEncode(text, false));
		}

		private void AddText(LocalizedString localizedText)
		{
			this.AddText(localizedText.ToString(base.Culture));
		}

		public const string UMCallInfoDiv = "<div id=\"UM-call-info\" lang=\"{0}\">";

		public const string UMCallInfoDivEnd = "</div>";

		private const string TranscriptionTextSpan = "<span id=\"UM-ASR-text\" lang=\"{0}\" style=\"{1}\">";

		private const string TranscriptionTextSpanEnd = "</span>";

		private const string UMInfoTable = "<table border=\"0\" style=\"width:100%; table-layout:auto;\">";

		private const string UMInfoTableEnd = "</table>";

		private const string ListInfo = "<ul dir=\"{0}\">";

		private const string ListInfoEnd = "</ul>";

		private const string RightToLeft = "rtl";

		private const string LeftToRight = "ltr";

		private const string AnchorStyleSheet = "<style type=\"text/css\"> a:link { color: #3399ff; } a:visited { color: #3366cc; } a:active { color: #ff9900; } </style>";

		internal enum TypeOfUMMessage
		{
			NormalMessage,
			InterpersonalNDR,
			CallAnsweringNDR
		}

		private class HtmlEvmTranscriptWriter : EvmTranscriptWriter
		{
			private string TranscriptionInformationStyle
			{
				get
				{
					return string.Format(CultureInfo.InvariantCulture, "font-family: {0}; color: #686a6b; font-size:7.5pt; margin-top:12px;", new object[]
					{
						Strings.Font2.ToString(base.Language)
					});
				}
			}

			private HtmlEvmTranscriptWriter(HtmlContentBuilder builder, CultureInfo language) : base(language)
			{
				this.builder = builder;
			}

			public static EvmTranscriptWriter Create(HtmlContentBuilder builder, CultureInfo language)
			{
				return new HtmlContentBuilder.HtmlEvmTranscriptWriter(builder, language);
			}

			private void WriteText(string text)
			{
				string text2 = text.TrimEnd(new char[0]);
				this.lastTextEndsWithPunctuation = (text2.Length > 0 && char.IsPunctuation(text2[text2.Length - 1]));
				this.builder.AddText(text);
			}

			protected override void WriteEndOfParagraph()
			{
				if (!this.lastTextEndsWithPunctuation)
				{
					this.WriteText(Strings.EndOfParagraphMarker.ToString(base.Language));
				}
			}

			protected override void WriteInformation(XmlElement informationElement)
			{
				string text = informationElement.InnerText;
				if (string.IsNullOrEmpty(text))
				{
					return;
				}
				CultureInfo cultureInfo = null;
				try
				{
					cultureInfo = new CultureInfo(informationElement.Attributes["lang"].Value);
				}
				catch (Exception ex)
				{
					if (ex is ArgumentException || ex is ArgumentNullException)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Cant determine the culture of information element. Caught an exception e {0}.", new object[]
						{
							ex.ToString()
						});
						return;
					}
					throw;
				}
				string text2 = (informationElement.Attributes["linkURL"] == null) ? null : informationElement.Attributes["linkURL"].Value;
				string text3 = (informationElement.Attributes["linkText"] == null) ? null : informationElement.Attributes["linkText"].Value;
				if (!string.IsNullOrEmpty(text2))
				{
					string val;
					if (string.IsNullOrEmpty(text3))
					{
						val = text2;
					}
					else
					{
						val = string.Concat(new string[]
						{
							"<a style=\"color: #3399ff; \" href=\"",
							text2,
							"\">",
							AntiXssEncoder.HtmlEncode(text3, false),
							"</a>"
						});
					}
					text = Strings.InformationTextWithLink(text, val).ToString(this.builder.Culture);
				}
				this.builder.AppendFormat("<div lang=\"{0}\" dir=\"{1}\" style=\"{2}\">{3}</div>", new object[]
				{
					cultureInfo.Name,
					cultureInfo.TextInfo.IsRightToLeft ? "rtl" : "ltr",
					this.TranscriptionInformationStyle,
					text
				});
			}

			protected override void WritePhoneNumber(XmlElement element)
			{
				string str = element.Attributes["reference"].Value;
				StringBuilder stringBuilder = new StringBuilder(32);
				foreach (object obj in element.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					stringBuilder.Append(xmlNode.InnerText);
				}
				string text = stringBuilder.ToString();
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				while (num3 < text.Length && EvmTranscriptWriter.IgnoreLeadingOrTrailingCharInTelAnchor(text[num3++]))
				{
					num++;
				}
				num3 = text.Length - 1;
				while (num3 >= num && EvmTranscriptWriter.IgnoreLeadingOrTrailingCharInTelAnchor(text[num3--]))
				{
					num2++;
				}
				int num4 = text.Length - num - num2;
				string text2 = (num4 == text.Length) ? text : text.Substring(num, num4);
				string input = (num == 0) ? string.Empty : text.Substring(0, num);
				string input2 = (num2 == 0) ? string.Empty : text.Substring(text.Length - num2);
				PhoneNumber phoneNumber = null;
				if (PhoneNumber.TryParse(text2, out phoneNumber))
				{
					str = phoneNumber.ToDial;
				}
				string str2 = MessageContentBuilder.FormatTelOrSipEntryAsAnchor(HttpUtility.UrlEncode(str), AntiXssEncoder.HtmlEncode(text2, false), UMUriType.TelExtn);
				string message = AntiXssEncoder.HtmlEncode(input, false) + str2 + AntiXssEncoder.HtmlEncode(input2, false);
				this.builder.AppendCultureInvariantText(message);
			}

			protected override void WriteGenericFeature(XmlElement element)
			{
				foreach (object obj in element.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					this.WriteText(xmlNode.InnerText);
				}
			}

			protected override void WriteGenericTextElement(XmlElement element)
			{
				this.WriteText(element.InnerText);
			}

			protected override void WriteBreakElement(XmlElement element)
			{
				XmlAttribute xmlAttribute = element.Attributes["wt"];
				string a = (xmlAttribute == null) ? "low" : xmlAttribute.Value;
				if (string.Equals(a, "high", StringComparison.OrdinalIgnoreCase))
				{
					this.WriteEndOfParagraph();
					int num = SafeConvert.ToInt32(Strings.ParagraphEndNewLines.ToString(base.Language), 0, 6, 2);
					for (int i = 0; i < num; i++)
					{
						this.builder.AddNewLine();
					}
					return;
				}
				string text = Strings.EndOfSentenceMarker.ToString(base.Language);
				int num2 = SafeConvert.ToInt32(Strings.NumSpaceBeforeEOS.ToString(base.Language), 0, 3, 1);
				int num3 = SafeConvert.ToInt32(Strings.NumSpaceAfterEOS.ToString(base.Language), 0, 3, 1);
				StringBuilder stringBuilder = new StringBuilder(num2 + num3 + text.Length);
				for (int j = 0; j <= num2; j++)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(text);
				for (int k = 0; k <= num3; k++)
				{
					stringBuilder.Append(" ");
				}
				this.WriteText(stringBuilder.ToString());
			}

			private HtmlContentBuilder builder;

			private bool lastTextEndsWithPunctuation;
		}
	}
}
