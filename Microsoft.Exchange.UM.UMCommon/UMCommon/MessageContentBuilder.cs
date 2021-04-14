using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class MessageContentBuilder
	{
		public static LocalizedString GetPhoneLabel(ContactInfo callerInfo)
		{
			LocalizedString result = LocalizedString.Empty;
			switch (callerInfo.FoundBy)
			{
			case FoundByType.BusinessPhone:
				result = Strings.WorkPhoneLabel;
				break;
			case FoundByType.MobilePhone:
				result = Strings.MobilePhoneLabel;
				break;
			case FoundByType.HomePhone:
				result = Strings.HomePhoneLabel;
				break;
			}
			return result;
		}

		public static string FormatTelOrSipEntryAsAnchor(string anchor, string display, UMUriType uriType)
		{
			if (uriType == UMUriType.SipName)
			{
				return string.Concat(new string[]
				{
					"<a style=\"color: #3399ff; \" href=\"sip:",
					anchor,
					"\">",
					display,
					"</a>"
				});
			}
			return string.Concat(new string[]
			{
				"<a style=\"color: #3399ff; \" href=\"tel:",
				anchor,
				"\">",
				display,
				"</a>"
			});
		}

		public static string FormatCallerId(PhoneNumber callerId, CultureInfo culture)
		{
			if (callerId.IsEmpty)
			{
				return Strings.AnonymousCaller.ToString(culture);
			}
			return callerId.ToDisplay;
		}

		public static string FormatCallerIdWithAnchor(PhoneNumber callerId, CultureInfo culture)
		{
			if (callerId.IsEmpty)
			{
				return Strings.AnonymousCaller.ToString(culture);
			}
			return MessageContentBuilder.FormatTelOrSipEntryAsAnchor(callerId.ToDial, callerId.ToDisplay, callerId.UriType);
		}

		protected MessageContentBuilder(CultureInfo culture, UMDialPlan rcptDialPlan)
		{
			this.culture = culture;
			this.sb = new StringBuilder();
			this.RecipientDialPlan = rcptDialPlan;
		}

		protected MessageContentBuilder(CultureInfo culture) : this(culture, null)
		{
		}

		internal virtual string Charset
		{
			get
			{
				return "utf-8";
			}
		}

		protected UMDialPlan RecipientDialPlan { get; set; }

		protected CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
			set
			{
				this.culture = value;
			}
		}

		protected abstract string EmailHeaderLine { get; }

		protected abstract string CalendarHeaderLine { get; }

		public override string ToString()
		{
			return this.sb.ToString();
		}

		internal static MessageContentBuilder Create(CultureInfo culture, UMDialPlan rcptDialPlan)
		{
			return new HtmlContentBuilder(culture, rcptDialPlan);
		}

		internal static MessageContentBuilder Create(CultureInfo culture)
		{
			return new HtmlContentBuilder(culture, null);
		}

		internal virtual void AddEmailHeader(MessageItem original)
		{
			string text = null;
			string headerValue = null;
			string displayName = original.From.DisplayName;
			string headerValue2 = original.SentTime.ToString("f", this.culture);
			string subject = original.Subject;
			XsoUtil.BuildParticipantStrings(original.Recipients, out headerValue, out text);
			string entryName = this.FormatHeaderName(Strings.FromHeader);
			string entryName2 = this.FormatHeaderName(Strings.SentHeader);
			string entryName3 = this.FormatHeaderName(Strings.ToHeader);
			string entryName4 = (text.Length > 0) ? this.FormatHeaderName(Strings.CcHeader) : string.Empty;
			string entryName5 = this.FormatHeaderName(Strings.SubjectHeader);
			this.AppendCultureInvariantText(this.EmailHeaderLine);
			this.Append(Strings.HeaderEntry(entryName, this.FormatHeaderValue(displayName)));
			this.AddNewLine();
			this.Append(Strings.HeaderEntry(entryName2, this.FormatHeaderValue(headerValue2)));
			this.AddNewLine();
			this.Append(Strings.HeaderEntry(entryName3, this.FormatHeaderValue(headerValue)));
			this.AddNewLine();
			if (text.Length > 0)
			{
				this.Append(Strings.HeaderEntry(entryName4, this.FormatHeaderValue(text)));
				this.AddNewLine();
			}
			this.Append(Strings.HeaderEntry(entryName5, this.FormatHeaderValue(subject)));
			this.AddNewLine();
			this.AddNewLine();
		}

		internal virtual void AddCalendarHeader(CalendarItemBase cal, ExTimeZone timezone, bool shouldAddLine)
		{
			string text = null;
			string headerValue = null;
			cal.Load(new PropertyDefinition[]
			{
				ItemSchema.SentTime,
				CalendarItemBaseSchema.OrganizerEmailAddress,
				ItemSchema.SentRepresentingDisplayName,
				ItemSchema.Subject,
				CalendarItemBaseSchema.Location,
				CalendarItemInstanceSchema.StartTime,
				CalendarItemInstanceSchema.EndTime
			});
			object obj = XsoUtil.SafeGetProperty(cal, ItemSchema.SentTime, null);
			string headerValue2 = (obj != null) ? ((ExDateTime)obj).ToString("f", this.culture) : string.Empty;
			string defaultValue = (string)XsoUtil.SafeGetProperty(cal, CalendarItemBaseSchema.OrganizerEmailAddress, string.Empty);
			string headerValue3 = (string)XsoUtil.SafeGetProperty(cal, ItemSchema.SentRepresentingDisplayName, defaultValue);
			string subject = cal.Subject;
			string location = cal.Location;
			string headerValue4 = this.BuildQualifiedMeetingTimeString(cal.StartTime, cal.EndTime, timezone);
			XsoUtil.BuildParticipantStrings(cal.AttendeeCollection, out headerValue, out text);
			string entryName = this.FormatHeaderName(Strings.FromHeader);
			string entryName2 = this.FormatHeaderName(Strings.SentHeader);
			string entryName3 = this.FormatHeaderName(Strings.ToHeader);
			string entryName4 = this.FormatHeaderName(Strings.SubjectHeader);
			string entryName5 = this.FormatHeaderName(Strings.WhenHeader);
			string entryName6 = this.FormatHeaderName(Strings.WhereHeader);
			if (shouldAddLine)
			{
				this.AppendCultureInvariantText(this.CalendarHeaderLine);
			}
			this.Append(Strings.HeaderEntry(entryName, this.FormatHeaderValue(headerValue3)));
			this.AddNewLine();
			this.Append(Strings.HeaderEntry(entryName2, this.FormatHeaderValue(headerValue2)));
			this.AddNewLine();
			this.Append(Strings.HeaderEntry(entryName3, this.FormatHeaderValue(headerValue)));
			this.AddNewLine();
			this.Append(Strings.HeaderEntry(entryName4, this.FormatHeaderValue(subject)));
			this.AddNewLine();
			this.Append(Strings.HeaderEntry(entryName5, this.FormatHeaderValue(headerValue4)));
			this.AddNewLine();
			this.Append(Strings.HeaderEntry(entryName6, this.FormatHeaderValue(location)));
			this.AddNewLine();
			this.AddNewLine();
		}

		internal virtual MemoryStream ToStream()
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(this.sb.ToString()));
		}

		internal abstract void AddVoicemailBody(PhoneNumber callerId, ContactInfo resolvedCallerInfo, string additionalText, ITranscriptionData transcriptionData);

		internal abstract void AddNDRBodyForCADRM(PhoneNumber callerId, ContactInfo resolvedCallerInfo, ExDateTime sentTime);

		internal abstract void AddVoicemailBody(ContactInfo resolvedCallerInfo, string additionalText);

		internal abstract void AddNDRBodyForInterpersonalDRM(UMSubscriber caller, RecipientDetails recipients, ExDateTime sentTime);

		internal abstract void AddFaxBody(PhoneNumber callerId, ContactInfo resolvedCallerInfo, string additionalText);

		internal abstract void AddMissedCallBody(PhoneNumber callerId, ContactInfo resolvedCallerInfo);

		internal abstract void AddTeamPickUpBody(string answeredBy, PhoneNumber callerId, ContactInfo callerInfo);

		internal abstract void AddCallNotForwardedBody(string target, PhoneNumber callerId, ContactInfo callerInfo);

		internal abstract void AddCallForwardedBody(string target, PhoneNumber callerId, ContactInfo callerInfo);

		internal abstract void AddIncomingCallLogBody(PhoneNumber callerId, ContactInfo callerInfo);

		internal abstract void AddOutgoingCallLogBody(PhoneNumber targetPhone, ContactInfo calledPartyInfo);

		internal abstract void AddEnterpriseNotifyMailBody(LocalizedString messageHeader, string[] accessNumbers, string extension, string pin, string additionalText);

		internal abstract void AddConsumerNotifyMailBody(LocalizedString messageHeader, string[] accessNumbers, string extension, string pin, string additionalText);

		internal abstract void AddAudioPreview(ITranscriptionData transcriptionData);

		internal virtual string GetDisplayNameOrCallerId(ContactInfo callerInfo, PhoneNumber callerId)
		{
			return this.GetDisplayName(callerInfo) ?? MessageContentBuilder.FormatCallerId(callerId, this.Culture);
		}

		internal virtual string GetDisplayName(ContactInfo callerInfo)
		{
			return callerInfo.DisplayName ?? callerInfo.EMailAddress;
		}

		internal virtual string GetFaxSubject(ContactInfo callerInfo, PhoneNumber callerId, uint pageCount, bool incomplete)
		{
			this.GetDisplayNameOrCallerId(callerInfo, callerId);
			LocalizedString autoGenSubject;
			if (!incomplete)
			{
				if (pageCount > 1U)
				{
					autoGenSubject = Strings.FaxMailSubjectInPages(pageCount);
				}
				else
				{
					autoGenSubject = Strings.FaxMailSubjectInPage;
				}
			}
			else if (pageCount > 1U)
			{
				autoGenSubject = Strings.IncompleteFaxMailSubjectInPages(pageCount);
			}
			else
			{
				autoGenSubject = Strings.IncompleteFaxMailSubjectInPage;
			}
			return this.GenerateSubject(autoGenSubject, null);
		}

		internal virtual string GetNDRSubjectForInterpersonalDRM()
		{
			return Strings.InterpersonalNDRForDRMSubject.ToString(this.culture);
		}

		internal virtual string GetNDRSubjectForCallAnsweringDRM()
		{
			return Strings.CallAnsweringNDRForDRMSubject.ToString(this.culture);
		}

		internal virtual string GetVoicemailSubject(string durationString, string originalSubject)
		{
			LocalizedString autoGenSubject = Strings.VoicemailSubject(durationString);
			return this.GenerateSubject(autoGenSubject, originalSubject);
		}

		internal virtual string GetMissedCallSubject(string originalSubject)
		{
			LocalizedString missedCallSubject = Strings.MissedCallSubject;
			return this.GenerateSubject(missedCallSubject, originalSubject);
		}

		internal virtual string GetTeamPickUpSubject(ContactInfo callerInfo, PhoneNumber callerId, string answeredBy, string originalSubject)
		{
			LocalizedString phoneLabel = MessageContentBuilder.GetPhoneLabel(callerInfo);
			string displayNameOrCallerId = this.GetDisplayNameOrCallerId(callerInfo, callerId);
			LocalizedString autoGenSubject;
			if (!phoneLabel.IsEmpty)
			{
				autoGenSubject = Strings.TeamPickUpSubjectWithLabel(displayNameOrCallerId, phoneLabel, answeredBy);
			}
			else
			{
				autoGenSubject = Strings.TeamPickUpSubject(displayNameOrCallerId, answeredBy);
			}
			return this.GenerateSubject(autoGenSubject, originalSubject);
		}

		internal virtual string GetCallForwardedSubject(ContactInfo callerInfo, PhoneNumber callerId, string target, string originalSubject)
		{
			LocalizedString phoneLabel = MessageContentBuilder.GetPhoneLabel(callerInfo);
			string displayNameOrCallerId = this.GetDisplayNameOrCallerId(callerInfo, callerId);
			LocalizedString autoGenSubject;
			if (!phoneLabel.IsEmpty)
			{
				autoGenSubject = Strings.CallForwardedSubjectWithLabel(displayNameOrCallerId, phoneLabel, target);
			}
			else
			{
				autoGenSubject = Strings.CallForwardedSubject(displayNameOrCallerId, target);
			}
			return this.GenerateSubject(autoGenSubject, originalSubject);
		}

		internal virtual string GetCallNotForwardedSubject(ContactInfo callerInfo, PhoneNumber callerId, string target, string originalSubject)
		{
			string displayNameOrCallerId = this.GetDisplayNameOrCallerId(callerInfo, callerId);
			LocalizedString phoneLabel = MessageContentBuilder.GetPhoneLabel(callerInfo);
			LocalizedString autoGenSubject;
			if (!phoneLabel.IsEmpty)
			{
				autoGenSubject = Strings.CallNotForwardedSubjectWithLabel(displayNameOrCallerId, phoneLabel, target);
			}
			else
			{
				autoGenSubject = Strings.CallNotForwardedSubject(displayNameOrCallerId, target);
			}
			return this.GenerateSubject(autoGenSubject, originalSubject);
		}

		internal virtual string GetIncomingCallLogSubject(ContactInfo callerInfo, PhoneNumber callerId)
		{
			LocalizedString phoneLabel = MessageContentBuilder.GetPhoneLabel(callerInfo);
			string displayNameOrCallerId = this.GetDisplayNameOrCallerId(callerInfo, callerId);
			LocalizedString autoGenSubject;
			if (!phoneLabel.IsEmpty)
			{
				autoGenSubject = Strings.IncomingCallLogSubjectWithLabel(displayNameOrCallerId, phoneLabel);
			}
			else
			{
				autoGenSubject = Strings.IncomingCallLogSubject(displayNameOrCallerId);
			}
			return this.GenerateSubject(autoGenSubject, null);
		}

		internal virtual string GetOutgoingCallLogSubject(ContactInfo calledPartyInfo, PhoneNumber targetPhone)
		{
			LocalizedString phoneLabel = MessageContentBuilder.GetPhoneLabel(calledPartyInfo);
			string displayNameOrCallerId = this.GetDisplayNameOrCallerId(calledPartyInfo, targetPhone);
			LocalizedString autoGenSubject;
			if (!phoneLabel.IsEmpty)
			{
				autoGenSubject = Strings.OutgoingCallLogSubjectWithLabel(displayNameOrCallerId, phoneLabel);
			}
			else
			{
				autoGenSubject = Strings.OutgoingCallLogSubject(displayNameOrCallerId);
			}
			return this.GenerateSubject(autoGenSubject, null);
		}

		internal abstract void AddLateForMeetingBody(CalendarItemBase cal, ExTimeZone timeZone, LocalizedString lateInfo);

		internal abstract void AddRecordedReplyText(string displayName);

		internal abstract void AddRecordedForwardText(string displayName);

		protected abstract void AddNewLine();

		protected abstract void AddDivider();

		protected abstract void AddDocumentEnd();

		protected abstract void AddStart();

		protected void Append(LocalizedString message)
		{
			this.sb.Append(message.ToString(this.Culture));
		}

		protected void AppendCultureInvariantText(string message)
		{
			this.sb.Append(message);
		}

		protected void AppendFormat(string format, params object[] args)
		{
			if (args != null)
			{
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i] is LocalizedString)
					{
						args[i] = ((LocalizedString)args[i]).ToString(this.Culture);
					}
				}
			}
			this.sb.AppendFormat(CultureInfo.InvariantCulture, format, args);
		}

		protected string BuildQualifiedMeetingTimeString(ExDateTime startTime, ExDateTime endTime, ExTimeZone timezone)
		{
			string format = "f";
			string format2 = "t";
			string start = startTime.ToString(format, this.culture);
			string end = (startTime.Day == endTime.Day) ? endTime.ToString(format2, this.culture) : endTime.ToString(format, this.culture);
			string timezone2 = timezone.LocalizableDisplayName.ToString(this.Culture);
			return Strings.QualifiedMeetingTime(start, end, timezone2).ToString(this.culture);
		}

		protected abstract string FormatHeaderName(LocalizedString headerName);

		protected abstract string FormatHeaderValue(string headerValue);

		private string GenerateSubject(LocalizedString autoGenSubject, string originalSubject)
		{
			if (!string.IsNullOrEmpty(originalSubject))
			{
				return Strings.AutogeneratedPlusOriginalSubject(autoGenSubject, originalSubject).ToString(this.culture);
			}
			return autoGenSubject.ToString(this.culture);
		}

		public const string AnchorStyle = "color: #3399ff; ";

		private CultureInfo culture;

		private StringBuilder sb;
	}
}
