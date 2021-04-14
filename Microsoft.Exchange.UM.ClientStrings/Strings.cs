using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCommon.MessageContent
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1163166080U, "VoicemailSettingsInstruction2");
			Strings.stringIDs.Add(1025467514U, "TranscriptionSkippedDueToProtectedVoiceMail");
			Strings.stringIDs.Add(3395113910U, "FaxSearchFolderName");
			Strings.stringIDs.Add(399665603U, "MissedCallSubject");
			Strings.stringIDs.Add(440863302U, "TranscriptionSkippedDueToRejectionDetails");
			Strings.stringIDs.Add(3618250418U, "Font1");
			Strings.stringIDs.Add(2631270417U, "Extension");
			Strings.stringIDs.Add(986397318U, "Recipients");
			Strings.stringIDs.Add(647750600U, "CallAnsweringNDRForDRMSubject");
			Strings.stringIDs.Add(559962466U, "OneMinute");
			Strings.stringIDs.Add(3428332004U, "AnonymousCaller");
			Strings.stringIDs.Add(1801440247U, "SentHeader");
			Strings.stringIDs.Add(1655837788U, "WelcomeMailBodyHeader");
			Strings.stringIDs.Add(1505519378U, "IncompleteFaxMailSubjectInPage");
			Strings.stringIDs.Add(4118032635U, "AccessNumber");
			Strings.stringIDs.Add(1708549140U, "ConsumerGetStartedText");
			Strings.stringIDs.Add(2942039717U, "InterpersonalNDRForDRMFooter");
			Strings.stringIDs.Add(1163166083U, "VoicemailSettingsInstruction1");
			Strings.stringIDs.Add(1712197018U, "FollowUp");
			Strings.stringIDs.Add(4145413099U, "NoTranscription");
			Strings.stringIDs.Add(1111077458U, "Email");
			Strings.stringIDs.Add(3394475845U, "TwelveNoon");
			Strings.stringIDs.Add(1827164709U, "InformationText");
			Strings.stringIDs.Add(2254833676U, "UMBodyDownload");
			Strings.stringIDs.Add(931278067U, "ConsumerUpdatePhoneNoExtension");
			Strings.stringIDs.Add(1158653436U, "MobilePhone");
			Strings.stringIDs.Add(2757445932U, "TranscriptionSkippedDueToSystemErrorDetails");
			Strings.stringIDs.Add(2279047064U, "AccessMailText");
			Strings.stringIDs.Add(1617820192U, "AccessNumberSeparator");
			Strings.stringIDs.Add(4151878805U, "HomePhoneLabel");
			Strings.stringIDs.Add(1455517480U, "WrittenTimeWithZeroMinutesFormat");
			Strings.stringIDs.Add(761385855U, "SubjectHeader");
			Strings.stringIDs.Add(2682518168U, "TranscriptionSkippedDueToLongMessageDetails");
			Strings.stringIDs.Add(1163166081U, "VoicemailSettingsInstruction3");
			Strings.stringIDs.Add(2844710954U, "OneSecond");
			Strings.stringIDs.Add(3899977494U, "OriginalAppointment");
			Strings.stringIDs.Add(642177943U, "Company");
			Strings.stringIDs.Add(2815524084U, "VoiceMailPreviewWithColon");
			Strings.stringIDs.Add(1689870310U, "FaxMailSubjectInPage");
			Strings.stringIDs.Add(3572895339U, "WhenHeader");
			Strings.stringIDs.Add(3435115712U, "IMAddress");
			Strings.stringIDs.Add(1106684832U, "WhereHeader");
			Strings.stringIDs.Add(2138533332U, "TranscriptionSkippedDueToThrottlingDetails");
			Strings.stringIDs.Add(587115635U, "JobTitle");
			Strings.stringIDs.Add(2397318150U, "TranscriptionSkippedDueToTimeoutDetails");
			Strings.stringIDs.Add(2980268693U, "TwelveMidnight");
			Strings.stringIDs.Add(2102852648U, "TranscriptionSkippedDueToPoorAudioQualityDetails");
			Strings.stringIDs.Add(2100892799U, "ConsumerConfigurePhoneGeneric");
			Strings.stringIDs.Add(2065109949U, "UnknownCaller");
			Strings.stringIDs.Add(3334166220U, "ParagraphEndNewLines");
			Strings.stringIDs.Add(3992915955U, "CcHeader");
			Strings.stringIDs.Add(3618250421U, "Font2");
			Strings.stringIDs.Add(1448959650U, "MobilePhoneLabel");
			Strings.stringIDs.Add(1915770880U, "OriginalMessage");
			Strings.stringIDs.Add(1683967347U, "CallAnsweringNDRForDRMFooter");
			Strings.stringIDs.Add(4087711248U, "InterpersonalNDRForDRMSubject");
			Strings.stringIDs.Add(3460905333U, "PasswordResetHeader");
			Strings.stringIDs.Add(2644606382U, "TranscriptionSkippedDueToErrorReadingSettings");
			Strings.stringIDs.Add(605091578U, "ConfigureVoicemailSettings");
			Strings.stringIDs.Add(1133914413U, "LearnMore");
			Strings.stringIDs.Add(2872664749U, "FromHeader");
			Strings.stringIDs.Add(1095233868U, "AccessMailTextConsumer");
			Strings.stringIDs.Add(2366595845U, "ReservedTimeTitle");
			Strings.stringIDs.Add(1736107127U, "WelcomeMailSubject");
			Strings.stringIDs.Add(2199732666U, "ToHeader");
			Strings.stringIDs.Add(3413781026U, "CallNotForwardedText");
			Strings.stringIDs.Add(3756600696U, "NumSpaceBeforeEOS");
			Strings.stringIDs.Add(35799476U, "InterpersonalNDRForDRM");
			Strings.stringIDs.Add(1051645941U, "TranscriptionSkippedDueToBadRequestDetails");
			Strings.stringIDs.Add(2839150813U, "WorkPhone");
			Strings.stringIDs.Add(3423762652U, "Sent");
			Strings.stringIDs.Add(776589994U, "EndOfParagraphMarker");
			Strings.stringIDs.Add(2961085015U, "WorkPhoneLabel");
			Strings.stringIDs.Add(3317306807U, "NumSpaceAfterEOS");
			Strings.stringIDs.Add(2450102343U, "HomePhone");
			Strings.stringIDs.Add(2800104273U, "VoiceMailPreview");
			Strings.stringIDs.Add(711958779U, "Recipient");
			Strings.stringIDs.Add(107281980U, "CallerId");
			Strings.stringIDs.Add(1976913622U, "UMWebServicePage");
			Strings.stringIDs.Add(1801195254U, "PasswordResetSubject");
			Strings.stringIDs.Add(339800695U, "Pin");
			Strings.stringIDs.Add(1655162919U, "TranscriptionSkippedDefaultDetails");
			Strings.stringIDs.Add(3733401898U, "ConsumerConfigurePhone");
			Strings.stringIDs.Add(3439047833U, "EndOfSentenceMarker");
			Strings.stringIDs.Add(143792894U, "OneMinuteOneSecond");
		}

		public static LocalizedString ForwardWithRecording(string displayName)
		{
			return new LocalizedString("ForwardWithRecording", Strings.ResourceManager, new object[]
			{
				displayName
			});
		}

		public static LocalizedString MinutesSeconds(int min, int sec)
		{
			return new LocalizedString("MinutesSeconds", Strings.ResourceManager, new object[]
			{
				min,
				sec
			});
		}

		public static LocalizedString FaxMailSubjectInPages(uint pages)
		{
			return new LocalizedString("FaxMailSubjectInPages", Strings.ResourceManager, new object[]
			{
				pages
			});
		}

		public static LocalizedString MissedCallBodyCallerResolved(string senderName, string senderPhone, string phoneLabel)
		{
			return new LocalizedString("MissedCallBodyCallerResolved", Strings.ResourceManager, new object[]
			{
				senderName,
				senderPhone,
				phoneLabel
			});
		}

		public static LocalizedString OutgoingCallLogBodyTargetMultipleResolved(string multipleResolvedTargets)
		{
			return new LocalizedString("OutgoingCallLogBodyTargetMultipleResolved", Strings.ResourceManager, new object[]
			{
				multipleResolvedTargets
			});
		}

		public static LocalizedString VoicemailSettingsInstruction2
		{
			get
			{
				return new LocalizedString("VoicemailSettingsInstruction2", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TranscriptionSkippedDueToProtectedVoiceMail
		{
			get
			{
				return new LocalizedString("TranscriptionSkippedDueToProtectedVoiceMail", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingCallLogBodyCallerMultipleResolved(string multipleResolvedSenders)
		{
			return new LocalizedString("IncomingCallLogBodyCallerMultipleResolved", Strings.ResourceManager, new object[]
			{
				multipleResolvedSenders
			});
		}

		public static LocalizedString FaxSearchFolderName
		{
			get
			{
				return new LocalizedString("FaxSearchFolderName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailSubject(string duration)
		{
			return new LocalizedString("VoicemailSubject", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString MissedCallSubject
		{
			get
			{
				return new LocalizedString("MissedCallSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TranscriptionSkippedDueToRejectionDetails
		{
			get
			{
				return new LocalizedString("TranscriptionSkippedDueToRejectionDetails", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Font1
		{
			get
			{
				return new LocalizedString("Font1", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissedCallBodyCallerUnresolved(string senderPhone)
		{
			return new LocalizedString("MissedCallBodyCallerUnresolved", Strings.ResourceManager, new object[]
			{
				senderPhone
			});
		}

		public static LocalizedString Extension
		{
			get
			{
				return new LocalizedString("Extension", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Recipients
		{
			get
			{
				return new LocalizedString("Recipients", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CallAnsweringNDRForDRMSubject
		{
			get
			{
				return new LocalizedString("CallAnsweringNDRForDRMSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OneMinute
		{
			get
			{
				return new LocalizedString("OneMinute", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AnonymousCaller
		{
			get
			{
				return new LocalizedString("AnonymousCaller", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingCallLogSubject(string sender)
		{
			return new LocalizedString("IncomingCallLogSubject", Strings.ResourceManager, new object[]
			{
				sender
			});
		}

		public static LocalizedString SentHeader
		{
			get
			{
				return new LocalizedString("SentHeader", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShortTimeMonthDay(string shortTime, string monthDay)
		{
			return new LocalizedString("ShortTimeMonthDay", Strings.ResourceManager, new object[]
			{
				shortTime,
				monthDay
			});
		}

		public static LocalizedString MultipleResolvedContactDisplayWithMoreThanTwoMatches(string number, string firstContact, string secondContact)
		{
			return new LocalizedString("MultipleResolvedContactDisplayWithMoreThanTwoMatches", Strings.ResourceManager, new object[]
			{
				number,
				firstContact,
				secondContact
			});
		}

		public static LocalizedString LateForMeeting_Plural2(int minutesLate)
		{
			return new LocalizedString("LateForMeeting_Plural2", Strings.ResourceManager, new object[]
			{
				minutesLate
			});
		}

		public static LocalizedString OutgoingCallLogBodyTargetUnresolved(string target)
		{
			return new LocalizedString("OutgoingCallLogBodyTargetUnresolved", Strings.ResourceManager, new object[]
			{
				target
			});
		}

		public static LocalizedString WelcomeMailBodyHeader
		{
			get
			{
				return new LocalizedString("WelcomeMailBodyHeader", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncompleteFaxMailSubjectInPage
		{
			get
			{
				return new LocalizedString("IncompleteFaxMailSubjectInPage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FaxBodyCallerResolved(string senderName, string senderPhone, string phoneLabel)
		{
			return new LocalizedString("FaxBodyCallerResolved", Strings.ResourceManager, new object[]
			{
				senderName,
				senderPhone,
				phoneLabel
			});
		}

		public static LocalizedString AccessNumber
		{
			get
			{
				return new LocalizedString("AccessNumber", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CallNotForwardedBody(string sender, string target)
		{
			return new LocalizedString("CallNotForwardedBody", Strings.ResourceManager, new object[]
			{
				sender,
				target
			});
		}

		public static LocalizedString ConsumerGetStartedText
		{
			get
			{
				return new LocalizedString("ConsumerGetStartedText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InterpersonalNDRForDRMFooter
		{
			get
			{
				return new LocalizedString("InterpersonalNDRForDRMFooter", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoicemailSettingsInstruction1
		{
			get
			{
				return new LocalizedString("VoicemailSettingsInstruction1", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FollowUp
		{
			get
			{
				return new LocalizedString("FollowUp", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoTranscription
		{
			get
			{
				return new LocalizedString("NoTranscription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Email
		{
			get
			{
				return new LocalizedString("Email", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TwelveNoon
		{
			get
			{
				return new LocalizedString("TwelveNoon", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InformationText
		{
			get
			{
				return new LocalizedString("InformationText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutgoingCallLogBodyTargetResolved(string target, string phone)
		{
			return new LocalizedString("OutgoingCallLogBodyTargetResolved", Strings.ResourceManager, new object[]
			{
				target,
				phone
			});
		}

		public static LocalizedString FaxBodyCallerMultipleResolved(string multipleResolvedSenders)
		{
			return new LocalizedString("FaxBodyCallerMultipleResolved", Strings.ResourceManager, new object[]
			{
				multipleResolvedSenders
			});
		}

		public static LocalizedString UMBodyDownload
		{
			get
			{
				return new LocalizedString("UMBodyDownload", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamPickUpSubjectWithLabel(string sender, LocalizedString phoneLabel, string answeredBy)
		{
			return new LocalizedString("TeamPickUpSubjectWithLabel", Strings.ResourceManager, new object[]
			{
				sender,
				phoneLabel,
				answeredBy
			});
		}

		public static LocalizedString LateForMeetingRange_Plural2(int minutesLateMin, int minutesLateMax)
		{
			return new LocalizedString("LateForMeetingRange_Plural2", Strings.ResourceManager, new object[]
			{
				minutesLateMin,
				minutesLateMax
			});
		}

		public static LocalizedString ConsumerUpdatePhoneNoExtension
		{
			get
			{
				return new LocalizedString("ConsumerUpdatePhoneNoExtension", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CallForwardedSubject(string sender, string target)
		{
			return new LocalizedString("CallForwardedSubject", Strings.ResourceManager, new object[]
			{
				sender,
				target
			});
		}

		public static LocalizedString VoiceMailBodyCallerResolvedNoCallerId(string senderName)
		{
			return new LocalizedString("VoiceMailBodyCallerResolvedNoCallerId", Strings.ResourceManager, new object[]
			{
				senderName
			});
		}

		public static LocalizedString MissedCallBodyCallerMultipleResolved(string multipleResolvedSenders)
		{
			return new LocalizedString("MissedCallBodyCallerMultipleResolved", Strings.ResourceManager, new object[]
			{
				multipleResolvedSenders
			});
		}

		public static LocalizedString MobilePhone
		{
			get
			{
				return new LocalizedString("MobilePhone", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentNameWithNumber(string callerId, string duration, int dupeNumber)
		{
			return new LocalizedString("AttachmentNameWithNumber", Strings.ResourceManager, new object[]
			{
				callerId,
				duration,
				dupeNumber
			});
		}

		public static LocalizedString TranscriptionSkippedDueToSystemErrorDetails
		{
			get
			{
				return new LocalizedString("TranscriptionSkippedDueToSystemErrorDetails", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessMailText
		{
			get
			{
				return new LocalizedString("AccessMailText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessNumberSeparator
		{
			get
			{
				return new LocalizedString("AccessNumberSeparator", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Seconds(int sec)
		{
			return new LocalizedString("Seconds", Strings.ResourceManager, new object[]
			{
				sec
			});
		}

		public static LocalizedString TeamPickUpBody(string sender, string answeredBy)
		{
			return new LocalizedString("TeamPickUpBody", Strings.ResourceManager, new object[]
			{
				sender,
				answeredBy
			});
		}

		public static LocalizedString OutgoingCallLogSubject(string target)
		{
			return new LocalizedString("OutgoingCallLogSubject", Strings.ResourceManager, new object[]
			{
				target
			});
		}

		public static LocalizedString HomePhoneLabel
		{
			get
			{
				return new LocalizedString("HomePhoneLabel", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LateForMeeting_Singular(int minutesLate)
		{
			return new LocalizedString("LateForMeeting_Singular", Strings.ResourceManager, new object[]
			{
				minutesLate
			});
		}

		public static LocalizedString WrittenTimeWithZeroMinutesFormat
		{
			get
			{
				return new LocalizedString("WrittenTimeWithZeroMinutesFormat", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectHeader
		{
			get
			{
				return new LocalizedString("SubjectHeader", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TranscriptionSkippedDueToLongMessageDetails
		{
			get
			{
				return new LocalizedString("TranscriptionSkippedDueToLongMessageDetails", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FaxBodyCallerResolvedNoPhoneLabel(string senderName, string senderPhone)
		{
			return new LocalizedString("FaxBodyCallerResolvedNoPhoneLabel", Strings.ResourceManager, new object[]
			{
				senderName,
				senderPhone
			});
		}

		public static LocalizedString ConsumerUpdatePhoneWithExtension(string extension)
		{
			return new LocalizedString("ConsumerUpdatePhoneWithExtension", Strings.ResourceManager, new object[]
			{
				extension
			});
		}

		public static LocalizedString LateForMeetingRange_Singular(int minutesLateMin, int minutesLateMax)
		{
			return new LocalizedString("LateForMeetingRange_Singular", Strings.ResourceManager, new object[]
			{
				minutesLateMin,
				minutesLateMax
			});
		}

		public static LocalizedString LateForMeeting_Plural(int minutesLate)
		{
			return new LocalizedString("LateForMeeting_Plural", Strings.ResourceManager, new object[]
			{
				minutesLate
			});
		}

		public static LocalizedString CallNotForwardedSubjectWithLabel(string sender, LocalizedString phoneLabel, string target)
		{
			return new LocalizedString("CallNotForwardedSubjectWithLabel", Strings.ResourceManager, new object[]
			{
				sender,
				phoneLabel,
				target
			});
		}

		public static LocalizedString CallForwardedBody(string sender, string target)
		{
			return new LocalizedString("CallForwardedBody", Strings.ResourceManager, new object[]
			{
				sender,
				target
			});
		}

		public static LocalizedString VoicemailSettingsInstruction3
		{
			get
			{
				return new LocalizedString("VoicemailSettingsInstruction3", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingCallLogSubjectWithLabel(string sender, LocalizedString phoneLabel)
		{
			return new LocalizedString("IncomingCallLogSubjectWithLabel", Strings.ResourceManager, new object[]
			{
				sender,
				phoneLabel
			});
		}

		public static LocalizedString IncomingCallLogBodyCallerResolved(string sender, string phone)
		{
			return new LocalizedString("IncomingCallLogBodyCallerResolved", Strings.ResourceManager, new object[]
			{
				sender,
				phone
			});
		}

		public static LocalizedString QualifiedMeetingTime(string start, string end, string timezone)
		{
			return new LocalizedString("QualifiedMeetingTime", Strings.ResourceManager, new object[]
			{
				start,
				end,
				timezone
			});
		}

		public static LocalizedString OfficeAddress(string off, string addr, string city, string state, string zipcode, string country)
		{
			return new LocalizedString("OfficeAddress", Strings.ResourceManager, new object[]
			{
				off,
				addr,
				city,
				state,
				zipcode,
				country
			});
		}

		public static LocalizedString CallNotForwardedSubject(string sender, string target)
		{
			return new LocalizedString("CallNotForwardedSubject", Strings.ResourceManager, new object[]
			{
				sender,
				target
			});
		}

		public static LocalizedString OneSecond
		{
			get
			{
				return new LocalizedString("OneSecond", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoiceMessageSubject(string duration)
		{
			return new LocalizedString("VoiceMessageSubject", Strings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString DisambiguatedNamePrefix(string userName)
		{
			return new LocalizedString("DisambiguatedNamePrefix", Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString OriginalAppointment
		{
			get
			{
				return new LocalizedString("OriginalAppointment", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Company
		{
			get
			{
				return new LocalizedString("Company", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoiceMailPreviewWithColon
		{
			get
			{
				return new LocalizedString("VoiceMailPreviewWithColon", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FaxMailSubjectInPage
		{
			get
			{
				return new LocalizedString("FaxMailSubjectInPage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutgoingCallLogSubjectWithLabel(string target, LocalizedString phoneLabel)
		{
			return new LocalizedString("OutgoingCallLogSubjectWithLabel", Strings.ResourceManager, new object[]
			{
				target,
				phoneLabel
			});
		}

		public static LocalizedString IncompleteFaxMailSubjectInPages(uint pages)
		{
			return new LocalizedString("IncompleteFaxMailSubjectInPages", Strings.ResourceManager, new object[]
			{
				pages
			});
		}

		public static LocalizedString WhenHeader
		{
			get
			{
				return new LocalizedString("WhenHeader", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DateTime(string timeFormat, string dayFormat)
		{
			return new LocalizedString("DateTime", Strings.ResourceManager, new object[]
			{
				timeFormat,
				dayFormat
			});
		}

		public static LocalizedString IMAddress
		{
			get
			{
				return new LocalizedString("IMAddress", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhereHeader
		{
			get
			{
				return new LocalizedString("WhereHeader", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TranscriptionSkippedDueToThrottlingDetails
		{
			get
			{
				return new LocalizedString("TranscriptionSkippedDueToThrottlingDetails", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CallAnsweringNDRForDRMCallerUnResolved(string senderPhone)
		{
			return new LocalizedString("CallAnsweringNDRForDRMCallerUnResolved", Strings.ResourceManager, new object[]
			{
				senderPhone
			});
		}

		public static LocalizedString VoiceMailBodyCallerResolved(string senderName, string senderPhone, string phoneLabel)
		{
			return new LocalizedString("VoiceMailBodyCallerResolved", Strings.ResourceManager, new object[]
			{
				senderName,
				senderPhone,
				phoneLabel
			});
		}

		public static LocalizedString JobTitle
		{
			get
			{
				return new LocalizedString("JobTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TranscriptionSkippedDueToTimeoutDetails
		{
			get
			{
				return new LocalizedString("TranscriptionSkippedDueToTimeoutDetails", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoEmailAddressDisplayName(string displayName)
		{
			return new LocalizedString("NoEmailAddressDisplayName", Strings.ResourceManager, new object[]
			{
				displayName
			});
		}

		public static LocalizedString AutogeneratedPlusOriginalSubject(LocalizedString autoGenerated, string original)
		{
			return new LocalizedString("AutogeneratedPlusOriginalSubject", Strings.ResourceManager, new object[]
			{
				autoGenerated,
				original
			});
		}

		public static LocalizedString TwelveMidnight
		{
			get
			{
				return new LocalizedString("TwelveMidnight", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FaxAttachmentInPages(string senderNumber, uint numPages, string tifFileExtension)
		{
			return new LocalizedString("FaxAttachmentInPages", Strings.ResourceManager, new object[]
			{
				senderNumber,
				numPages,
				tifFileExtension
			});
		}

		public static LocalizedString TranscriptionSkippedDueToPoorAudioQualityDetails
		{
			get
			{
				return new LocalizedString("TranscriptionSkippedDueToPoorAudioQualityDetails", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoiceMailBodyCallerMultipleResolved(string multipleResolvedSenders)
		{
			return new LocalizedString("VoiceMailBodyCallerMultipleResolved", Strings.ResourceManager, new object[]
			{
				multipleResolvedSenders
			});
		}

		public static LocalizedString ConsumerConfigurePhoneGeneric
		{
			get
			{
				return new LocalizedString("ConsumerConfigurePhoneGeneric", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownCaller
		{
			get
			{
				return new LocalizedString("UnknownCaller", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParagraphEndNewLines
		{
			get
			{
				return new LocalizedString("ParagraphEndNewLines", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CcHeader
		{
			get
			{
				return new LocalizedString("CcHeader", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Font2
		{
			get
			{
				return new LocalizedString("Font2", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Minutes(int min)
		{
			return new LocalizedString("Minutes", Strings.ResourceManager, new object[]
			{
				min
			});
		}

		public static LocalizedString MobilePhoneLabel
		{
			get
			{
				return new LocalizedString("MobilePhoneLabel", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MinutesOneSecond(int min)
		{
			return new LocalizedString("MinutesOneSecond", Strings.ResourceManager, new object[]
			{
				min
			});
		}

		public static LocalizedString OriginalMessage
		{
			get
			{
				return new LocalizedString("OriginalMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LateForMeetingRange_Zero(int minutesLateMin, int minutesLateMax)
		{
			return new LocalizedString("LateForMeetingRange_Zero", Strings.ResourceManager, new object[]
			{
				minutesLateMin,
				minutesLateMax
			});
		}

		public static LocalizedString MissedCallBodyCallerResolvedNoPhoneLabel(string senderName, string senderPhone)
		{
			return new LocalizedString("MissedCallBodyCallerResolvedNoPhoneLabel", Strings.ResourceManager, new object[]
			{
				senderName,
				senderPhone
			});
		}

		public static LocalizedString FaxBodyCallerUnresolved(string senderPhone)
		{
			return new LocalizedString("FaxBodyCallerUnresolved", Strings.ResourceManager, new object[]
			{
				senderPhone
			});
		}

		public static LocalizedString CallAnsweringNDRForDRMFooter
		{
			get
			{
				return new LocalizedString("CallAnsweringNDRForDRMFooter", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CallForwardedSubjectWithLabel(string sender, LocalizedString phoneLabel, string target)
		{
			return new LocalizedString("CallForwardedSubjectWithLabel", Strings.ResourceManager, new object[]
			{
				sender,
				phoneLabel,
				target
			});
		}

		public static LocalizedString InterpersonalNDRForDRMSubject
		{
			get
			{
				return new LocalizedString("InterpersonalNDRForDRMSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PasswordResetHeader
		{
			get
			{
				return new LocalizedString("PasswordResetHeader", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TranscriptionSkippedDueToErrorReadingSettings
		{
			get
			{
				return new LocalizedString("TranscriptionSkippedDueToErrorReadingSettings", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FaxAttachmentInPage(string senderNumber, string tifFileExtension)
		{
			return new LocalizedString("FaxAttachmentInPage", Strings.ResourceManager, new object[]
			{
				senderNumber,
				tifFileExtension
			});
		}

		public static LocalizedString ConsumerAccess(string pin)
		{
			return new LocalizedString("ConsumerAccess", Strings.ResourceManager, new object[]
			{
				pin
			});
		}

		public static LocalizedString ConfigureVoicemailSettings
		{
			get
			{
				return new LocalizedString("ConfigureVoicemailSettings", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LearnMore
		{
			get
			{
				return new LocalizedString("LearnMore", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromHeader
		{
			get
			{
				return new LocalizedString("FromHeader", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessMailTextConsumer
		{
			get
			{
				return new LocalizedString("AccessMailTextConsumer", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReservedTimeTitle
		{
			get
			{
				return new LocalizedString("ReservedTimeTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WelcomeMailSubject
		{
			get
			{
				return new LocalizedString("WelcomeMailSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToHeader
		{
			get
			{
				return new LocalizedString("ToHeader", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InformationTextWithLink(string val1, string val2)
		{
			return new LocalizedString("InformationTextWithLink", Strings.ResourceManager, new object[]
			{
				val1,
				val2
			});
		}

		public static LocalizedString VoiceMailBodyCallerResolvedNoPhoneLabel(string senderName, string senderPhone)
		{
			return new LocalizedString("VoiceMailBodyCallerResolvedNoPhoneLabel", Strings.ResourceManager, new object[]
			{
				senderName,
				senderPhone
			});
		}

		public static LocalizedString CallNotForwardedText
		{
			get
			{
				return new LocalizedString("CallNotForwardedText", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NumSpaceBeforeEOS
		{
			get
			{
				return new LocalizedString("NumSpaceBeforeEOS", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InterpersonalNDRForDRM
		{
			get
			{
				return new LocalizedString("InterpersonalNDRForDRM", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TranscriptionSkippedDueToBadRequestDetails
		{
			get
			{
				return new LocalizedString("TranscriptionSkippedDueToBadRequestDetails", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WorkPhone
		{
			get
			{
				return new LocalizedString("WorkPhone", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Sent
		{
			get
			{
				return new LocalizedString("Sent", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EndOfParagraphMarker
		{
			get
			{
				return new LocalizedString("EndOfParagraphMarker", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SayNumberAndName(int number, string name)
		{
			return new LocalizedString("SayNumberAndName", Strings.ResourceManager, new object[]
			{
				number,
				name
			});
		}

		public static LocalizedString WorkPhoneLabel
		{
			get
			{
				return new LocalizedString("WorkPhoneLabel", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LateForMeetingRange_Plural(int minutesLateMin, int minutesLateMax)
		{
			return new LocalizedString("LateForMeetingRange_Plural", Strings.ResourceManager, new object[]
			{
				minutesLateMin,
				minutesLateMax
			});
		}

		public static LocalizedString ReplyWithRecording(string displayName)
		{
			return new LocalizedString("ReplyWithRecording", Strings.ResourceManager, new object[]
			{
				displayName
			});
		}

		public static LocalizedString DefaultEmailOOFText(string userName)
		{
			return new LocalizedString("DefaultEmailOOFText", Strings.ResourceManager, new object[]
			{
				userName
			});
		}

		public static LocalizedString NumSpaceAfterEOS
		{
			get
			{
				return new LocalizedString("NumSpaceAfterEOS", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncomingCallLogBodyCallerUnresolved(string phone)
		{
			return new LocalizedString("IncomingCallLogBodyCallerUnresolved", Strings.ResourceManager, new object[]
			{
				phone
			});
		}

		public static LocalizedString HomePhone
		{
			get
			{
				return new LocalizedString("HomePhone", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoiceMailPreview
		{
			get
			{
				return new LocalizedString("VoiceMailPreview", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Recipient
		{
			get
			{
				return new LocalizedString("Recipient", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamPickUpSubject(string sender, string answeredBy)
		{
			return new LocalizedString("TeamPickUpSubject", Strings.ResourceManager, new object[]
			{
				sender,
				answeredBy
			});
		}

		public static LocalizedString ReservedTimeBody(string userName, string creationDate)
		{
			return new LocalizedString("ReservedTimeBody", Strings.ResourceManager, new object[]
			{
				userName,
				creationDate
			});
		}

		public static LocalizedString CallerId
		{
			get
			{
				return new LocalizedString("CallerId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UMWebServicePage
		{
			get
			{
				return new LocalizedString("UMWebServicePage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VoiceMailBodyCallerUnresolved(string senderPhone)
		{
			return new LocalizedString("VoiceMailBodyCallerUnresolved", Strings.ResourceManager, new object[]
			{
				senderPhone
			});
		}

		public static LocalizedString PasswordResetSubject
		{
			get
			{
				return new LocalizedString("PasswordResetSubject", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HeaderEntry(string entryName, string entryValue)
		{
			return new LocalizedString("HeaderEntry", Strings.ResourceManager, new object[]
			{
				entryName,
				entryValue
			});
		}

		public static LocalizedString MultipleResolvedContactDisplayWithTwoMatches(string number, string firstContact, string secondContact)
		{
			return new LocalizedString("MultipleResolvedContactDisplayWithTwoMatches", Strings.ResourceManager, new object[]
			{
				number,
				firstContact,
				secondContact
			});
		}

		public static LocalizedString Pin
		{
			get
			{
				return new LocalizedString("Pin", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TranscriptionSkippedDefaultDetails
		{
			get
			{
				return new LocalizedString("TranscriptionSkippedDefaultDetails", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WeekDayShortTime(string weekDay, string shortTime)
		{
			return new LocalizedString("WeekDayShortTime", Strings.ResourceManager, new object[]
			{
				weekDay,
				shortTime
			});
		}

		public static LocalizedString CallAnsweringNDRForDRMCallerResolved(string senderName)
		{
			return new LocalizedString("CallAnsweringNDRForDRMCallerResolved", Strings.ResourceManager, new object[]
			{
				senderName
			});
		}

		public static LocalizedString ConsumerConfigurePhone
		{
			get
			{
				return new LocalizedString("ConsumerConfigurePhone", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttachmentName(string callerId, string duration)
		{
			return new LocalizedString("AttachmentName", Strings.ResourceManager, new object[]
			{
				callerId,
				duration
			});
		}

		public static LocalizedString EndOfSentenceMarker
		{
			get
			{
				return new LocalizedString("EndOfSentenceMarker", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LateForMeeting_Zero(int minutesLate)
		{
			return new LocalizedString("LateForMeeting_Zero", Strings.ResourceManager, new object[]
			{
				minutesLate
			});
		}

		public static LocalizedString OneMinuteSeconds(int sec)
		{
			return new LocalizedString("OneMinuteSeconds", Strings.ResourceManager, new object[]
			{
				sec
			});
		}

		public static LocalizedString CancelledMeetingSubject(string subject)
		{
			return new LocalizedString("CancelledMeetingSubject", Strings.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString OneMinuteOneSecond
		{
			get
			{
				return new LocalizedString("OneMinuteOneSecond", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(85);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.UM.UMCommon.MessageContent.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			VoicemailSettingsInstruction2 = 1163166080U,
			TranscriptionSkippedDueToProtectedVoiceMail = 1025467514U,
			FaxSearchFolderName = 3395113910U,
			MissedCallSubject = 399665603U,
			TranscriptionSkippedDueToRejectionDetails = 440863302U,
			Font1 = 3618250418U,
			Extension = 2631270417U,
			Recipients = 986397318U,
			CallAnsweringNDRForDRMSubject = 647750600U,
			OneMinute = 559962466U,
			AnonymousCaller = 3428332004U,
			SentHeader = 1801440247U,
			WelcomeMailBodyHeader = 1655837788U,
			IncompleteFaxMailSubjectInPage = 1505519378U,
			AccessNumber = 4118032635U,
			ConsumerGetStartedText = 1708549140U,
			InterpersonalNDRForDRMFooter = 2942039717U,
			VoicemailSettingsInstruction1 = 1163166083U,
			FollowUp = 1712197018U,
			NoTranscription = 4145413099U,
			Email = 1111077458U,
			TwelveNoon = 3394475845U,
			InformationText = 1827164709U,
			UMBodyDownload = 2254833676U,
			ConsumerUpdatePhoneNoExtension = 931278067U,
			MobilePhone = 1158653436U,
			TranscriptionSkippedDueToSystemErrorDetails = 2757445932U,
			AccessMailText = 2279047064U,
			AccessNumberSeparator = 1617820192U,
			HomePhoneLabel = 4151878805U,
			WrittenTimeWithZeroMinutesFormat = 1455517480U,
			SubjectHeader = 761385855U,
			TranscriptionSkippedDueToLongMessageDetails = 2682518168U,
			VoicemailSettingsInstruction3 = 1163166081U,
			OneSecond = 2844710954U,
			OriginalAppointment = 3899977494U,
			Company = 642177943U,
			VoiceMailPreviewWithColon = 2815524084U,
			FaxMailSubjectInPage = 1689870310U,
			WhenHeader = 3572895339U,
			IMAddress = 3435115712U,
			WhereHeader = 1106684832U,
			TranscriptionSkippedDueToThrottlingDetails = 2138533332U,
			JobTitle = 587115635U,
			TranscriptionSkippedDueToTimeoutDetails = 2397318150U,
			TwelveMidnight = 2980268693U,
			TranscriptionSkippedDueToPoorAudioQualityDetails = 2102852648U,
			ConsumerConfigurePhoneGeneric = 2100892799U,
			UnknownCaller = 2065109949U,
			ParagraphEndNewLines = 3334166220U,
			CcHeader = 3992915955U,
			Font2 = 3618250421U,
			MobilePhoneLabel = 1448959650U,
			OriginalMessage = 1915770880U,
			CallAnsweringNDRForDRMFooter = 1683967347U,
			InterpersonalNDRForDRMSubject = 4087711248U,
			PasswordResetHeader = 3460905333U,
			TranscriptionSkippedDueToErrorReadingSettings = 2644606382U,
			ConfigureVoicemailSettings = 605091578U,
			LearnMore = 1133914413U,
			FromHeader = 2872664749U,
			AccessMailTextConsumer = 1095233868U,
			ReservedTimeTitle = 2366595845U,
			WelcomeMailSubject = 1736107127U,
			ToHeader = 2199732666U,
			CallNotForwardedText = 3413781026U,
			NumSpaceBeforeEOS = 3756600696U,
			InterpersonalNDRForDRM = 35799476U,
			TranscriptionSkippedDueToBadRequestDetails = 1051645941U,
			WorkPhone = 2839150813U,
			Sent = 3423762652U,
			EndOfParagraphMarker = 776589994U,
			WorkPhoneLabel = 2961085015U,
			NumSpaceAfterEOS = 3317306807U,
			HomePhone = 2450102343U,
			VoiceMailPreview = 2800104273U,
			Recipient = 711958779U,
			CallerId = 107281980U,
			UMWebServicePage = 1976913622U,
			PasswordResetSubject = 1801195254U,
			Pin = 339800695U,
			TranscriptionSkippedDefaultDetails = 1655162919U,
			ConsumerConfigurePhone = 3733401898U,
			EndOfSentenceMarker = 3439047833U,
			OneMinuteOneSecond = 143792894U
		}

		private enum ParamIDs
		{
			ForwardWithRecording,
			MinutesSeconds,
			FaxMailSubjectInPages,
			MissedCallBodyCallerResolved,
			OutgoingCallLogBodyTargetMultipleResolved,
			IncomingCallLogBodyCallerMultipleResolved,
			VoicemailSubject,
			MissedCallBodyCallerUnresolved,
			IncomingCallLogSubject,
			ShortTimeMonthDay,
			MultipleResolvedContactDisplayWithMoreThanTwoMatches,
			LateForMeeting_Plural2,
			OutgoingCallLogBodyTargetUnresolved,
			FaxBodyCallerResolved,
			CallNotForwardedBody,
			OutgoingCallLogBodyTargetResolved,
			FaxBodyCallerMultipleResolved,
			TeamPickUpSubjectWithLabel,
			LateForMeetingRange_Plural2,
			CallForwardedSubject,
			VoiceMailBodyCallerResolvedNoCallerId,
			MissedCallBodyCallerMultipleResolved,
			AttachmentNameWithNumber,
			Seconds,
			TeamPickUpBody,
			OutgoingCallLogSubject,
			LateForMeeting_Singular,
			FaxBodyCallerResolvedNoPhoneLabel,
			ConsumerUpdatePhoneWithExtension,
			LateForMeetingRange_Singular,
			LateForMeeting_Plural,
			CallNotForwardedSubjectWithLabel,
			CallForwardedBody,
			IncomingCallLogSubjectWithLabel,
			IncomingCallLogBodyCallerResolved,
			QualifiedMeetingTime,
			OfficeAddress,
			CallNotForwardedSubject,
			VoiceMessageSubject,
			DisambiguatedNamePrefix,
			OutgoingCallLogSubjectWithLabel,
			IncompleteFaxMailSubjectInPages,
			DateTime,
			CallAnsweringNDRForDRMCallerUnResolved,
			VoiceMailBodyCallerResolved,
			NoEmailAddressDisplayName,
			AutogeneratedPlusOriginalSubject,
			FaxAttachmentInPages,
			VoiceMailBodyCallerMultipleResolved,
			Minutes,
			MinutesOneSecond,
			LateForMeetingRange_Zero,
			MissedCallBodyCallerResolvedNoPhoneLabel,
			FaxBodyCallerUnresolved,
			CallForwardedSubjectWithLabel,
			FaxAttachmentInPage,
			ConsumerAccess,
			InformationTextWithLink,
			VoiceMailBodyCallerResolvedNoPhoneLabel,
			SayNumberAndName,
			LateForMeetingRange_Plural,
			ReplyWithRecording,
			DefaultEmailOOFText,
			IncomingCallLogBodyCallerUnresolved,
			TeamPickUpSubject,
			ReservedTimeBody,
			VoiceMailBodyCallerUnresolved,
			HeaderEntry,
			MultipleResolvedContactDisplayWithTwoMatches,
			WeekDayShortTime,
			CallAnsweringNDRForDRMCallerResolved,
			AttachmentName,
			LateForMeeting_Zero,
			OneMinuteSeconds,
			CancelledMeetingSubject
		}
	}
}
