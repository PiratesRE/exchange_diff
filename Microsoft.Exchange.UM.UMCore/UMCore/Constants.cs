using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class Constants
	{
		private Constants()
		{
		}

		internal const string TestModeCanaryFileName = "UMTEST-C798DBA2-1B87-11DC-9A33-5A6656D89593.bin";

		internal const string Zero = "Zero";

		internal const string Singular = "Singular";

		internal const string Plural = "Plural";

		internal const string Plural2 = "Plural2";

		internal const int ThreadInitTimeout = 30;

		internal const int ContactSearchLimit = 100;

		internal const int HResultMask = 65535;

		internal const byte MaxDtmfSizeUpper = 16;

		internal const byte MaxPromptsPerActivity = 16;

		internal const string VuiPromptPrefix = "vui";

		internal const string UmConfigSchemaFile = "umconfig.xsd";

		internal const string DtmfGrammarFile = "dtmfrules.grxml";

		internal const string LanguageAutoDetectionMinLength = "LanguageAutoDetectionMinLength";

		internal const string LanguageAutoDetectionMaxLength = "LanguageAutoDetectionMaxLength";

		internal const string DtmfGrammarRuleName = "DtmfSequence";

		internal const string DtmfGrammarDigitsNode = "<item repeat=\"{0}-{1}\"><ruleref uri=\"#Digit\" type=\"application/srgs+xml\"/><tag>$._value = $._value + $$._value</tag></item>";

		internal const string DtmfGrammarStopTonesNode = "<one-of>{0}</one-of>";

		internal const string DtmfGrammarStopToneNode = "<item><item>{0}</item><tag>$._value = $._value + \"{0}\"</tag></item>";

		internal const string DtmfGrammarStopPatternNode = "<item><item>{0}</item><tag>$._value = \"{1}\"</tag></item>";

		internal const string AnonymousUser = "Anonymous";

		internal const string AllDtmfDigits = "0123456789#*ABCD";

		internal const int DtmfTimeout = 10;

		internal const int InitialSilenceTimeout = 6;

		internal const int InterDigitTimeout = 1;

		internal const int IncompleteTimeout = 3;

		internal const int NumberIncorrectInputs = 3;

		internal const int MaxRecordingSilence = 3;

		internal const int WaveFileType = 6;

		internal const int MaxSpeechRecognitionAlternatives = 10;

		internal const int MaxRecordingSeconds = 600;

		internal const int MaxErrorsAllowed = 3;

		internal const int TTSPromptsVolume = 70;

		internal const float ProsodyRateIncrement = 0.15f;

		internal const float MaxProsodyRate = 0.6f;

		internal const float MinProsodyRate = -0.6f;

		internal const long MinFreeMegabytesDiskSpaceDatacenter = 50L;

		internal const long MinFreeMegabytesDiskSpaceEnterprise = 500L;

		internal const long MinFreeMegabytesDiskSpaceWarning = 750L;

		internal const char DtmfEscapeCharacter = '#';

		internal const string FaxTone = "faxtone";

		internal const string Diversion = "Diversion";

		internal const string CallId = "CALL-ID";

		internal const string CertSNHeader = "P-Certificate-Subject-Common-Name";

		internal const string CertSANHeader = "P-Certificate-Subject-Alternative-Name";

		internal const string SipDiversion = "sip:";

		internal const string SipUriFormat = "<sip:{0}>";

		internal const string MsDiagnostics = "ms-diagnostics";

		internal const string MsDiagnosticsPublic = "ms-diagnostics-public";

		internal const string UserAgent = "User-Agent";

		internal const int InvalidMailboxClearDigitsTimeout = 1000;

		internal const double RecordBytesPerSecond = 16000.0;

		internal const string TestUserAgentName = "Unified Messaging Test Client";

		internal const string ActiveMonitoringUserAgentName = "ActiveMonitoringClient";

		internal const string MonitoringCertSN = "um.o365.exchangemon.net";

		internal const string MonitoringDomain = "o365.exchangemon.net";

		internal const int ProvisionalResponseCode = 101;

		internal const string ProvisionalResponseName = "Diagnostics";

		internal const string RedirectDiagnosticsFormat = "{0};source=\"{1}\";reason=\"Redirecting to:{2};time={3}\"";

		internal const string CallReceivedDiagnosticsFormat = "{0};source=\"{1}\";reason=\"{2}\";service=\"{3}\";time=\"{4}\"";

		internal const string ServerHealthDiagnosticsFormat = "{0};source=\"{1}\";reason=\"{2}\";service=\"{3}\";health=\"{4}\";time=\"{5}\"";

		internal const string CallTimeoutDiagnosticsFormat = "{0};source=\"{1}\";reason=\"{2}\";service=\"{3}\";time=\"{4}\"";

		internal const string CallState = "Call-State: ";

		internal const string DiversionHeader = "Diversion";

		internal const string DiversionNumber = "number";

		internal const int AudioBufferLength = 4096;

		internal const int AudioNumOfBuffers = 5;

		internal const int DtmfCngTone = 36;

		internal const int MediaInitializeTimeout = 5;

		internal const int SampleRate = 8000;

		internal const int BitsPerSample = 16;

		internal const int NumMediaPoolElements = 128;

		internal const int BitsperSec = 128000;

		internal const int WmaBitsperSec = 13312;

		internal const int WmaHeaderBytes = 8192;

		internal const int WavFileHeaderBytes = 44;

		internal const int VoiceMailSecsLimit = 2;

		internal const int MaxNumOfSearchAttempts = 3;

		internal const int MaxNumOfDiversionLookups = 6;

		internal const string UnicodeCharset = "unicode";

		internal const string VoiceCAContentClass = "Voice-CA";

		internal const string VoiceUCContentClass = "Voice-UC";

		internal const string VoiceContentClass = "Voice";

		internal const string MissedCallContentClass = "MissedCall";

		internal const string FaxCAContentClass = "Fax-CA";

		internal const string ProviderName = "Exchange12";

		internal const string WaveFileExtensionName = "wav";

		internal const int FileLogMaxChars = 128;

		internal const int BadDigitTimeoutMsec = 1000;

		internal const int RelativeMinutesThreshold = 180;

		internal const double MeetingOverThreshold = 0.5;

		internal const string CancelledMeetingClass = "IPM.Schedule.Meeting.Canceled";

		internal const string BeepFileName = "Beep.wav";

		internal const int ChangePasswordChances = 5;

		internal const int MaxExtensionDigits = 15;

		internal const int MaxNameDigits = 75;

		internal const string TrunkStateIdle = "Idle";

		internal const string TrunkStateConnected = "Connected";

		internal const string TrunkStateRemoteDisconnected = "RemoteDisconnected";

		internal const string VoiceMailHeaderFileExtension = ".txt";

		internal const string XSOMessageFileExtension = ".msg";

		internal const int CbConversationHeaderBlock = 22;

		internal const string GrammarLogFileName = "UMSpeechGrammar.log";

		internal const int MrasErrorRetryInterval = 10;

		internal const string EnabledForAsr = "EnabledForAsr";

		internal const string EnabledForTranscription = "EnabledForTranscription";

		internal const string TranscriptionHignConfidence = "TranscriptionHignConfidence";

		internal const string TranscriptionLowConfidence = "TranscriptionLowConfidence";

		internal const string TtsVolume = "TtsVolume";

		internal const string SmartReadingHours = "SmartReadingHours";

		internal const int MaxConcurrentPlayOnPhoneCalls = 2;

		public const string CarriageReturnNewLine = "\r\n";

		public const string PipelineGuidStringForUserWithNoMailbox = "af360a7e-e6d4-494a-ac69-6ae14896d16b";

		public const string PipelineGuidRecipientStringForUserWithNoMailbox = "455e5330-ce1f-48d1-b6b1-2e318d2ff2c4";

		public const string PipelineGuidStringForTransportServers = "70cb6c39-83d9-4123-8013-d95aadda7712";

		internal static readonly byte StarByte = Encoding.ASCII.GetBytes("*")[0];

		internal static readonly byte PoundByte = Encoding.ASCII.GetBytes("#")[0];

		internal static readonly TimeSpan GalGrammarFetchTimeout = TimeSpan.FromSeconds(10.0);

		internal static readonly TimeSpan DLGramamrFetchTimeout = TimeSpan.FromSeconds(5.0);

		internal static readonly TimeSpan RemoveDTMFTime = TimeSpan.FromMilliseconds(300.0);

		internal static readonly TimeSpan TimeZoneErrorSpan = TimeSpan.FromMinutes(7.0);

		internal static readonly char[] CDOCalendarSeparator = "*~*~*~*~*~*~*~*~*~*".ToCharArray();

		internal static readonly TimeSpan DtmfEndSilenceTimeout = TimeSpan.FromMilliseconds(500.0);

		internal static readonly TimeSpan DtmfEndSilenceTimeoutDiagnostic = TimeSpan.FromSeconds(3.0);

		internal static readonly TimeSpan UpdateCurrentCallsTimerInterval = TimeSpan.FromSeconds(1.0);

		internal static readonly TimeSpan SeekTime = TimeSpan.FromSeconds(5.0);

		internal static readonly TimeSpan HeavyBlockingOperationDelay = TimeSpan.FromSeconds(2.0);

		internal static readonly TimeSpan ADNotificationsRetryTime = TimeSpan.FromMinutes(5.0);

		internal static readonly TimeSpan DisableContactResolutionMaxTime = TimeSpan.FromMinutes(2.0);

		internal static readonly ContentType ContentTypeSourceParty = new ContentType("text/source-party");

		internal static readonly Regex DiversionRegex = new Regex("<(tel|sip):(?<number>[^@\\s]+)(@.*)?>(.*;reason=(?<reason>[^;\\s]+))?");

		internal static readonly TimeSpan CallInfoExpirationTime = TimeSpan.FromSeconds(300.0);

		internal abstract class RegularExpressions
		{
			private RegularExpressions()
			{
			}

			private static Regex BuildTextNormalizer()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("(?<basicURL>https?://[^\\s?]+)[^\\s]*");
				stringBuilder.Append("|");
				stringBuilder.Append("(?<nuanceHack>[^\\s\\d\\w\\(\\)\\?\\.\\!\\{\\}\\<\\>]{2,}\\.)");
				return new Regex(stringBuilder.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			}

			private static Regex BuildEmailNormalizer()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("(?<basicURL>https?://[^\\s?]+)[^\\s]*");
				stringBuilder.Append("|");
				stringBuilder.Append("(?<cidURL>\\[cid:[^\\]]{1,256}\\])");
				stringBuilder.Append("|");
				stringBuilder.Append("(?<nuanceHack>[^\\s\\d\\w\\(\\)\\?\\.\\!\\{\\}\\<\\>]{2,}\\.)");
				string format = string.Empty + "(?<fromHeader>[\\r\\n]{{1,2}}\\s*{0}[^\\r\\n]*[\\r\\n]{{1,2}})(?:\\s*{1}[^\\r\\n]*[\\r\\n]{{1,2}})(?:[^\\r\\n:]{{1,50}}:[^\\r\\n]*[\\r\\n]{{1,2}}){{1,8}}[\\r\\n]{{1,2}}";
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				foreach (CultureInfo formatProvider in UmCultures.GetSupportedClientCultures())
				{
					string text = string.Format(CultureInfo.InvariantCulture, format, new object[]
					{
						Strings.FromHeader.ToString(formatProvider),
						Strings.SentHeader.ToString(formatProvider)
					});
					if (!dictionary.ContainsKey(text))
					{
						stringBuilder.Append("|").Append(text);
						dictionary[text] = null;
					}
				}
				return new Regex(stringBuilder.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			}

			internal const string RoomLocationMatch = "(?<building>\\d\\d+)/(?<room>\\d\\d\\d\\d+)";

			internal const string RoomLocationReplacement = "${building} / ${room}";

			internal const string FromHeaderGroupName = "fromHeader";

			internal const string BasicURLGroupName = "basicURL";

			internal const string CidURLGroupName = "cidURL";

			internal const string NuanceHackGroupName = "nuanceHack";

			internal static readonly Regex ValidNumberRegex = new Regex("^\\d+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

			internal static readonly Regex ValidDigitRegex = new Regex("^\\d$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

			internal static readonly Regex TextNormalizer = Constants.RegularExpressions.BuildTextNormalizer();

			internal static readonly Regex EmailNormalizer = Constants.RegularExpressions.BuildEmailNormalizer();
		}

		internal abstract class DirectorySearch
		{
			private DirectorySearch()
			{
			}

			internal static readonly int MaxResultsToPreprocess = 100;

			internal static readonly int MaxResultsToDisplay = 9;

			internal static readonly int MaxPersonalContacts = 5000;
		}

		internal abstract class SpeechMenu
		{
			internal const int DefaultBabbleSeconds = 10;

			internal const int DefaultEndSilenceSeconds = 10;

			internal const float DefaultConfidence = 0.25f;

			internal static readonly byte DtmfFallbackKey = Encoding.ASCII.GetBytes("0")[0];
		}

		internal abstract class HeaderFile
		{
			internal const string CallId = "CallId";

			internal const string CallerId = "CallerId";

			internal const string ContactInfo = "ContactInfo";

			internal const string AttachmentPath = "AttachmentPath";

			internal const string AttachmentName = "AttachmentName";

			internal const string Duration = "Duration";

			internal const string ProcessedCount = "ProcessedCount";

			internal const string MessageID = "MessageID";

			internal const string SentTime = "SentTime";

			internal const string NumberOfPages = "NumberOfPages";

			internal const string SenderAddress = "SenderAddress";

			internal const string RecipientName = "RecipientName";

			internal const string RecipientObjectGuid = "RecipientObjectGuid";

			internal const string SenderObjectGuid = "SenderObjectGuid";

			internal const string MessageFilePath = "MessageFilePath";

			internal const string CultureInfo = "CultureInfo";

			internal const string CallerName = "CallerNAme";

			internal const string CallerIdDisplayName = "CallerIdDisplayName";

			internal const string CallerAddress = "CallerAddress";

			internal const string Important = "Important";

			internal const string Private = "Private";

			internal const string PureInterpersonalMessage = "PureInterpersonalMessage";

			internal const string ProtectedReply = "ProtectedReply";

			internal const string CallAnswering = "CallAnswering";

			internal const string Incomplete = "InComplete";

			internal const string MessageType = "MessageType";

			internal const string SMTPVoiceMailType = "SMTPVoiceMail";

			internal const string XSOVoiceMailType = "XSOVoiceMail";

			internal const string CDRMessageType = "CDR";

			internal const string HealthCheckType = "HealthCheck";

			internal const string CDRData = "CDRData";

			internal const string FaxType = "Fax";

			internal const string Codec = "Codec";

			internal const string MissedCallType = "MissedCall";

			internal const string IncomingCallLogType = "IncomingCallLog";

			internal const string OutgoingCallLogType = "OutgoingCallLog";

			internal const string PartnerTranscriptionRequestType = "PartnerTranscriptionRequest";

			internal const string PartnerTranscriptionContext = "PartnerTranscriptionContext";

			internal const string TranscriptionData = "TranscriptionData";

			internal const string Priority = "Priority";

			internal const string Subject = "Subject";

			internal const string HeaderSeperator = " : ";

			internal const string OCSNotificationType = "OCSNotification";

			internal const string OCSNotificationData = "OCSNotificationData";

			internal const string TenantGuid = "TenantGuid";
		}

		internal abstract class SipUriParameters
		{
			internal const string E12Referrer = "Referrer";

			internal const string Referrer = "referrer";

			internal const string E12Extension = "Extension";

			internal const string Extension = "extension";

			internal const string PhoneContext = "phone-context";

			internal const string Version = "v";

			internal const string Command = "c";

			internal const string FaxRecipient = "msExchUMFaxRecipient";

			internal const string UMContext = "msExchUMContext";
		}

		internal abstract class SmtpHeaders
		{
			internal const string Precedence = "Precedence";

			internal const string MimeVersion = "Mime-Version";
		}

		internal abstract class XHeaders
		{
			internal const string ContentClass = "X-ContentClass";

			internal const string CallingTelephoneNumber = "X-CallingTelephoneNumber";

			internal const string VoiceMessageDuration = "X-VoiceMessageDuration";

			internal const string VoiceMessageSenderName = "X-VoiceMessageSenderName";

			internal const string AttachmentOrder = "X-AttachmentOrder";

			internal const string FaxNumberOfPages = "X-FaxNumberOfPages";

			internal const string CallId = "X-CallID";

			internal const string ExchOrganizationSCL = "X-MS-Exchange-Organization-SCL";
		}

		internal abstract class Transcription
		{
			internal abstract class Grammars
			{
				internal const string LMGrammarSubDirTemplate = "Common Files\\microsoft shared\\Speech\\Tokens\\SR_MS_{0}_TRANS_11.0\\Grammars";

				internal const string LMGrammarName = "TSR-LM.cfp";

				internal const string RootSemanticTagKey = "Fragments";

				internal const string CustomGrammarPrefix = "<?xml version=\"1.0\"?>\r\n<grammar xml:lang=\"{0}\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/06/grammar\" tag-format=\"semantics/1.0\">\r\n<tag>out.customGrammarWords=false;out.topNWords=false;</tag>\r\n    <rule id=\"{1}\" scope=\"public\">\r\n        <one-of>";

				internal const string CustomGrammarSuffix = "      \r\n        </one-of>\r\n    </rule>\r\n</grammar>";

				internal const string SetValueSemanticTagFormat = "out.{0} = \"{1}\";";

				internal const string CustomTopNFileName = "ExtTopN.grxml";

				internal const string CustomCallerInfoFileName = "ExtCallerInfo.grxml";

				internal const string CustomCallerInfoRule = "ExtCallerInfo";

				internal const string CustomPersonNameFileName = "ExtPersonName.grxml";

				internal const string CustomTopNRule = "ExtTopN";

				internal const string CustomPersonNameRule = "ExtPersonName";

				internal const string CustomPhoneNumberFileName = "ExtPhoneNumber.grxml";

				internal const string CustomPhoneNumberRule = "ExtPhoneNumber";

				internal const string CustomGenAppFileName = "ExtGenAppRule.grxml";

				internal const string CustomGenAppRule = "ExtGenAppRule";

				internal const string OuterItemFormat = "\r\n            <item >{0}\r\n            </item>";

				internal const string OuterItemFormatWeight = "\r\n            <item weight='{0}'>{1}\r\n            </item>";

				internal const string SemanticTagItemFormat = "\r\n                <item>{0}</item>\r\n                <tag>{1}</tag>";

				internal const string NoSemanticTagItemFormat = "\r\n                <item>{0}</item>";

				internal const string TagSetValueSemanticTagFormat = "\r\n                <tag>out.{0} = {1};</tag>";

				internal const string RuleRefUri = "\r\n                <ruleref uri=\"{0}#{1}\"/>";

				internal const string RuleRefUriWithHoist = "\r\n                <ruleref uri=\"{0}#{1}\"/>\r\n                <tag>out=rules.latest();</tag>";

				internal const string ScgPersonNameSemanticItemName = "PersonName";

				internal const string ScgPhoneNumberSemanticItemName = "PhoneNumber";

				internal const string ScgDateSemanticItemName = "Date";

				internal const string ScgTimeSemanticItemName = "Time";

				internal const string MailboxSemanticItem = "Mailbox";

				internal const string ContactSemanticItem = "Contact";

				internal const string PhoneNumberSemanticItem = "PhoneNumber";

				internal const string DateSemanticItem = "Date";

				internal const string TimeSemanticItem = "Time";

				internal const string PersonNameSemanticItem = "PersonName";

				internal const string AreaCodeSemanticItem = "AreaCode";

				internal const string LocalNumberSemanticItem = "LocalNumber";

				internal const string ExtensionSemanticItem = "Extension";

				internal const string HourSemanticItem = "Hour";

				internal const string MinuteSemanticItem = "Minute";

				internal const string IsValidDateSemanticItem = "IsValidDate";

				internal const string DaySemanticItem = "Day";

				internal const string MonthSemanticItem = "Month";

				internal const string YearSemanticItem = "Year";

				internal const string AttributesSemanticItem = "_attributes";

				internal const string AttributesNameSemanticItem = "name";

				internal const string AttributesFirstWordIndexSemanticItem = "FirstWordIndex";

				internal const string AttributesCountOfWordsSemanticItem = "CountOfWords";

				internal const string AttributesTextSemanticItem = "text";

				internal const string CustomGrammarWordsSemanticItem = "customGrammarWords";

				internal const string TopNWordsSemanticItem = "topNWords";
			}
		}

		internal abstract class OCS
		{
			internal const string Time = "Time";

			internal const string User = "User";

			internal const string From = "From";

			internal const string Event = "Event";

			internal const string Target = "Target";

			internal const string CallId = "CallId";

			internal const string Subject = "Subject";

			internal const string Priority = "Priority";

			internal const string Template = "Template";

			internal const string ReferredBy = "ReferredBy";

			internal const string TargetClass = "TargetClass";

			internal const string MissedReason = "MissedReason";

			internal const string ConversationId = "ConversationID";

			internal const string EumProxyAddress = "EumProxyAddress";

			internal const string RecipientObjectGuid = "RecipientObjectGuid";

			internal const string TenantGuid = "TenantGuid";

			internal const string UserNotification = "UserNotification";

			internal const string Type = "type";

			internal const string SkipPin = "skip-pin";

			internal const string MsExchangeCommand = "Ms-Exchange-Command";

			internal const string PrivateNoDiversion = "private-no-diversion";

			internal const string MsSensitivityHeader = "Ms-Sensitivity";

			internal const string PAssertedIdentityHeader = "P-Asserted-Identity";

			internal const string MsTargetClass = "Ms-Target-Class";

			internal const string SecondaryTargetClass = "secondary";

			internal const string OpaqueParameter = "opaque";

			internal const string OpaqueAppVoicemailPrefix = "app:voicemail";

			internal const string LocalResourcePathParam = "local-resource-path";

			internal const string ItemIdParamPrefix = "itemId=";

			internal const string Urgent = "urgent";

			internal const string Normal = "normal";

			internal const string Emergency = "emergency";

			internal const string NonUrgent = "non-urgent";

			internal const string FromHeader = "FROM";

			internal const string RTCOpaqueParam = "opaque=app:rtcevent";

			internal const string RTCNotificationSender = "sip:A410AA79-D874-4e56-9B46-709BDD0EB850";

			internal const string SDPMediaDescriptionName = "application";

			internal static readonly ContentType SDPContentType = new ContentType("application/sdp");

			internal static readonly TimeSpan SessionMaxIdleTime = TimeSpan.FromMinutes(12.0);

			internal static readonly TimeSpan SessionCheckTimerInterval = TimeSpan.FromMinutes(1.0);

			internal abstract class SDPAttributes
			{
				internal const string SendOnly = "sendonly";

				internal const string ReceiveOnly = "recvonly";

				internal const string AcceptTypes = "accept-types";

				internal const string AcceptTemplates = "ms-rtc-accept-eventtemplates";

				internal const string UserNotification = "application/ms-rtc-usernotification+xml";

				internal const string RtcDefaultTemplate = "RtcDefault";
			}
		}

		internal abstract class MowaGrammar
		{
			internal const string EmailPeopleKeywordGrammarId = "grEmailPersonByNameMobile";

			internal const string FindPeopleKeywordGrammarId = "grFindPersonByNameMobile";

			internal const string AppointmentCreationKeywordGrammarId = "grCalendarDayNewAppointment";

			internal const string DaySearchKeywordGrammarId = "grCalendarDaySearch";
		}

		internal abstract class Xml
		{
			private Xml()
			{
			}

			internal const string Menu = "Menu";

			internal const string SpeechMenu = "SpeechMenu";

			internal const string PlayBackMenu = "PlayBackMenu";

			internal const string Transition = "Transition";

			internal const string Record = "Record";

			internal const string Prompt = "Prompt";

			internal const string PromptGroup = "PromptGroup";

			internal const string MessageCount = "MessageCount";

			internal const string FaxRequest = "FaxRequest";

			internal const string CallTransfer = "CallTransfer";

			internal const string ConditionNode = "Condition";

			internal const string Grammars = "Grammars";

			internal const string Transitions = "Transitions";

			internal const string Grammar = "Grammar";

			internal const string Main = "Main";

			internal const string Help = "Help";

			internal const string Mumble1 = "Mumble1";

			internal const string Mumble2 = "Mumble2";

			internal const string Silence1 = "Silence1";

			internal const string Silence2 = "Silence2";

			internal const string SpeechError = "SpeechError";

			internal const string Repeat = "Repeat";

			internal const string InvalidCommand = "InvalidCommand";

			internal const string Tag = "Tag";

			internal const string FsmImport = "FsmImport";

			internal const string FsmModule = "FsmModule";

			internal const string FiniteStateMachine = "FiniteStateMachine";

			internal const string GlobalManager = "GlobalActivityManager";

			internal const string SingularPlural = "singularPlural";

			internal const string Event = "event";

			internal const string RefId = "refId";

			internal const string RefInfo = "refInfo";

			internal const string Action = "action";

			internal const string HeavyAction = "heavyaction";

			internal const string PlaybackAction = "playbackAction";

			internal const string Id = "id";

			internal const string MaxDtmfSize = "maxDtmfSize";

			internal const string MinDtmfSize = "minDtmfSize";

			internal const string MaxNumericInput = "maxNumericInput";

			internal const string MinNumericInput = "minNumericInput";

			internal const string DtmfInputValue = "dtmfInputValue";

			internal const string Type = "type";

			internal const string Name = "name";

			internal const string FirstActivityId = "firstActivityId";

			internal const string DtmfStopTones = "dtmfStopTones";

			internal const string Condition = "condition";

			internal const string InterDigitTimeout = "interDigitTimeout";

			internal const string InputTimeout = "inputTimeout";

			internal const string InitialSilenceTimeout = "initialSilenceTimeout";

			internal const string Value = "value";

			internal const string PhoneNumber = "number";

			internal const string PhoneNumberType = "numberType";

			internal const string Message = "message";

			internal const string Greeting = "greeting";

			internal const string SearchPurpose = "searchPurpose";

			internal const string Suffix = "suffix";

			internal const string Culture = "culture";

			internal const string Rule = "rule";

			internal const string BabbleSeconds = "babbleSeconds";

			internal const string EndSilenceSeconds = "endSilenceSeconds";

			internal const string Confidence = "confidence";

			internal const string Scope = "scope";

			internal const string Uninterruptible = "uninterruptible";

			internal const string StopPromptOnBargeIn = "stopPromptOnBargeIn";

			internal const string KeepDtmfOnNoMatch = "keepDtmfOnNoMatch";

			internal const string Href = "href";

			internal const string Module = "module";

			internal const string ProsodyRate = "prosodyRate";

			internal const string Language = "language";

			internal const string LimitKey = "limitKey";

			internal const string Volume = "volume";

			internal const string Public = "public";

			internal const string User = "user";

			internal abstract class GlobCfg
			{
				internal const string SingularPlural = "singularPlural";

				internal const string Singular = "_Singular";

				internal const string Plural = "_Plural";

				internal const string Plural2 = "_Plural2";

				internal const string StartTime = "startTime";

				internal const string EndTime = "endTime";

				internal const string StartDay = "startDay";

				internal const string EndDay = "endDay";

				internal const string StartDayTime = "startDayTime";

				internal const string EndDayTime = "endDayTime";

				internal const string BusinessAddress = "businessAddress";

				internal const string BusinessSchedule = "businessSchedule";

				internal const string UserName = "userName";

				internal const string CustomGreeting = "customGreeting";

				internal const string CustomPrompt = "customPrompt";

				internal const string CustomMenu = "customMenu";

				internal const string DepartmentName = "departmentName";

				internal const string SelectedMenu = "selectedMenu";

				internal const string BusinessName = "businessName";

				internal const string VarTimeZone = "varTimeZone";

				internal const string VarScheduleGroupList = "varScheduleGroupList";

				internal const string VarScheduleIntervalList = "varScheduleIntervalList";

				internal const string AAContext = "AAContext";

				internal const string AALocationContext = "aaLocationContext";
			}
		}

		internal abstract class VariableName
		{
			internal const string UseAsr = "useAsr";

			internal const string CurrentActivity = "currentActivity";

			internal const string LastActivity = "lastActivity";

			internal const string LastRecoEvent = "lastRecoEvent";

			internal const string SavedRecoEvent = "savedRecoEvent";

			internal const string LastInput = "lastInput";

			internal const string Greeting = "greeting";

			internal const string Recording = "recording";

			internal const string UserName = "userName";

			internal const string DefaultLanguage = "defaultLanguage";

			internal const string SelectableLanguages = "selectableLanguages";

			internal const string MessageLanguage = "messageLanguage";

			internal const string LanguageDetected = "languageDetected";

			internal const string InvalidExtension = "invalidExtension";

			internal const string AdminMinPwdLen = "adminMinPwdLen";

			internal const string AdminOldPwdLen = "adminOldPwdLen";

			internal const string PilotNumberWelcomeGreetingFilename = "pilotNumberWelcomeGreetingFilename";

			internal const string PilotNumberWelcomeGreetingEnabled = "pilotNumberWelcomeGreetingEnabled";

			internal const string PilotNumberInfoAnnouncementFilename = "pilotNumberInfoAnnouncementFilename";

			internal const string PilotNumberInfoAnnouncementEnabled = "pilotNumberInfoAnnouncementEnabled";

			internal const string PilotNumberInfoAnnouncementInterruptible = "pilotNumberInfoAnnouncementInterruptible";

			internal const string PilotNumberTransferToOperatorEnabled = "pilotNumberTransferToOperatorEnabled";

			internal const string TUIPromptEditingEnabled = "tuiPromptEditingEnabled";

			internal const string ContactSomeoneEnabled = "contactSomeoneEnabled";

			internal const string Mode = "mode";

			internal const string OCFeature = "ocFeature";

			internal const string SkipPinCheck = "skipPinCheck";

			internal const string DiagnosticDtmfDigits = "diagnosticDtmfDigits";

			internal abstract class Voicemail
			{
				internal const string SenderInfo = "senderInfo";

				internal const string MessageRecievedTime = "messageReceivedTime";

				internal const string CurrentVoicemailMessage = "currentVoicemailMessage";

				internal const string NumberOfMessages = "numberOfMessages";

				internal const string DurationMinutes = "durationMinutes";

				internal const string DurationSeconds = "durationSeconds";

				internal const string OperatorNumber = "operatorNumber";
			}

			internal abstract class Calendar
			{
				internal const string Remaining = "remaining";

				internal const string Time = "time";

				internal const string ConflictTime = "conflictTime";

				internal const string Subject = "subject";

				internal const string Location = "location";

				internal const string NumConflicts = "numConflicts";

				internal const string NumAccepted = "numAccepted";

				internal const string NumDeclined = "numDeclined";

				internal const string NumUndecided = "numUndecided";

				internal const string OwnerName = "ownerName";

				internal const string NumAttendees = "numAttendees";

				internal const string AcceptedList = "acceptedList";

				internal const string DeclinedList = "declinedList";

				internal const string UndecidedList = "undecidedList";

				internal const string AttendeeList = "attendeeList";

				internal const string CalendarDate = "calendarDate";

				internal const string MinutesLateMax = "minutesLateMax";

				internal const string MinutesLateMin = "minutesLateMin";

				internal const string Current = "current";

				internal const string DayOfWeek = "dayOfWeek";

				internal const string DayOffset = "dayOffset";

				internal const string StartTime = "startTime";

				internal const string EndTime = "endTime";

				internal const string ClearTime = "clearTime";

				internal const string ClearDays = "clearDays";

				internal const string MeetingTimeRange = "meetingTimeRange";
			}

			internal abstract class SendMessage
			{
				internal const string NumRecipients = "numRecipients";
			}

			internal abstract class MessageCount
			{
				internal const string HaveSummary = "haveSummary";

				internal const string NumEmail = "numEmail";

				internal const string NumEmailMax = "numEmailMax";

				internal const string NumVoicemail = "numVoicemail";

				internal const string NumMeetings = "numMeetings";

				internal const string Location = "location";

				internal const string StartTime = "startTime";
			}

			internal abstract class DirectorySearch
			{
				internal const string PrimarySearchMode = "primarySearchMode";

				internal const string SecondarySearchMode = "secondarySearchMode";

				internal const string CurrentSearchMode = "currentSearchMode";

				internal const string None = "none";

				internal const string FirstNameLastName = "firstNameLastName";

				internal const string LastNameFirstName = "lastNameFirstName";

				internal const string EmailAlias = "emailAlias";

				internal const string NewSearch = "newSearch";

				internal const string SpokenNameFromAD = "spokenNameFromAD";

				internal const string SpokenNameFromTTS = "spokenNameFromTTS";

				internal const string PromptIndex = "promptindex";

				internal const string ADSpokenName = "adspokenname";

				internal const string InvalidSearchSelection = "invalidSearchSelection";

				internal const string LastDtmfSearchInput = "lastDtmfSearchInput";

				internal const string NumResults = "numResults";

				internal const string SearchInput = "searchInput";

				internal const string Result = "directorySearchResult";

				internal const string AuthenticatedUser = "authenticatedUser";

				internal const string SearchTarget = "searchTarget";

				internal const string InitialSearchTarget = "initialSearchTarget";

				internal const string PersonalContacts = "personalContacts";

				internal const string GlobalAddressList = "globalAddressList";

				internal const string CompanyName = "companyName";

				internal const string HaveDialableMobileNumber = "haveDialableMobileNumber";

				internal const string HaveDialableBusinessNumber = "haveDialableBusinessNumber";

				internal const string HaveDialableHomeNumber = "haveDialableHomeNumber";

				internal const string Email = "email";

				internal const string Email1 = "email1";

				internal const string Email2 = "email2";

				internal const string Email3 = "email3";

				internal const string HaveEmail = "haveEmail";

				internal const string HaveEmail1 = "haveEmail1";

				internal const string HaveEmail2 = "haveEmail2";

				internal const string HaveEmail3 = "haveEmail3";

				internal const string SearchByExtension = "searchByExtension";

				internal const string CurrentMenu = "currentMenu";

				internal const string ExactMatches = "exactMatches";

				internal const string PartialMatches = "partialMatches";

				internal const string HaveMorePartialMatches = "haveMorePartialMatches";

				internal const string ExceedRetryLimit = "exceedRetryLimit";
			}

			internal abstract class AsrSearch
			{
				internal const string SearchResult = "searchResult";

				internal const string SearchContext = "searchContext";

				internal const string NamesOnly = "namesOnly";

				internal const string RecordedNamesOnly = "recordedNamesOnly";

				internal const string ResultList = "resultList";

				internal const string NumUsers = "numUsers";

				internal const string ResultType = "resultType";

				internal const string HaveNameRecording = "haveNameRecording";

				internal const string HaveNameRecording1 = "haveNameRecording1";

				internal const string HaveNameRecording2 = "haveNameRecording2";

				internal const string HaveNameRecording3 = "haveNameRecording3";

				internal const string HaveNameRecording4 = "haveNameRecording4";

				internal const string HaveNameRecording5 = "haveNameRecording5";

				internal const string HaveNameRecording6 = "haveNameRecording6";

				internal const string HaveNameRecording7 = "haveNameRecording7";

				internal const string HaveNameRecording8 = "haveNameRecording8";

				internal const string HaveNameRecording9 = "haveNameRecording9";

				internal const string Mode = "mode";
			}

			internal abstract class AsrContacts
			{
				internal const string NamesGrammar = "namesGrammar";

				internal const string DistributionListGrammar = "distributionListGrammar";

				internal const string EmailAliasGrammar = "emailAliasGrammar";

				internal const string NameLookupEnabled = "contacts_nameLookupEnabled";

				internal const string ResultType = "resultType";

				internal const string ResultTypeString = "resultTypeString";

				internal const string SelectedUser = "selectedUser";

				internal const string SelectedPhoneNumber = "selectedPhoneNumber";

				internal const string EmailAddressSelection = "emailAddressSelection";

				internal const string HasCell = "hasCell";

				internal const string HasHome = "hasHome";

				internal const string HasOffice = "hasOffice";

				internal const string CallingType = "callingType";

				internal const string RetryAsrSearch = "retryAsrSearch";
			}

			internal abstract class AutoAttendant
			{
				internal const string InfoAnnouncementFilename = "infoAnnouncementFilename";

				internal const string InfoAnnouncementEnabled = "infoAnnouncementEnabled";

				internal const string InfoAnnouncementUninterruptible = "infoAnnouncementUninterruptible";

				internal const string MainMenuCustomPromptFilename = "mainMenuCustomPromptFilename";

				internal const string MainMenuCustomPromptEnabled = "mainMenuCustomPromptEnabled";

				internal const string DirectorySearchEnabled = "directorySearchEnabled";

				internal const string TransferToOperatorEnabled = "aa_transferToOperatorEnabled";

				internal const string CallSomeoneEnabled = "aa_callSomeoneEnabled";

				internal const string ContactSomeoneEnabled = "aa_contactSomeoneEnabled";

				internal const string ConnectToExtensionsEnabled = "connectToExtensionsEnabled";

				internal const string CustomizedMenuEnabled = "aa_customizedMenuEnabled";

				internal const string CustomizedMenuOptions = "customizedMenuOptions";

				internal const string HolidayIntroductoryGreetingPrompt = "holidayIntroductoryGreetingPrompt";

				internal const string HolidayHours = "holidayHours";

				internal const string TransferExtensionVariable = "transferExtension";

				internal const string IsBusinessHours = "aa_isBusinessHours";

				internal const string DtmfFallbackEnabled = "aa_dtmfFallbackEnabled";

				internal const string FirstDepartment = "firstDepartment";

				internal const string CustomizedMenuGrammar = "customizedMenuGrammar";

				internal const string DepartmentName = "departmentName";

				internal const string RecordedNamesAndTTS = "recordedNamesAndTTS";

				internal const string User1 = "user1";

				internal const string User2 = "user2";

				internal const string User3 = "user3";

				internal const string User4 = "user4";

				internal const string User5 = "user5";

				internal const string User6 = "user6";

				internal const string User7 = "user7";

				internal const string User8 = "user8";

				internal const string User9 = "user9";

				internal const string DtmfKey1 = "DtmfKey1";

				internal const string DtmfKey2 = "DtmfKey2";

				internal const string DtmfKey3 = "DtmfKey3";

				internal const string DtmfKey4 = "DtmfKey4";

				internal const string DtmfKey5 = "DtmfKey5";

				internal const string DtmfKey6 = "DtmfKey6";

				internal const string DtmfKey7 = "DtmfKey7";

				internal const string DtmfKey8 = "DtmfKey8";

				internal const string DtmfKey9 = "DtmfKey9";

				internal const string TimeoutOption = "TimeoutOption";

				internal const string AllowCall = "allowCall";

				internal const string AllowMessage = "allowMessage";

				internal const string HaveCustomMenuOptionPrompt = "haveCustomMenuOptionPrompt";

				internal const string CustomMenuOption = "customMenuOption";

				internal const string CustomMenuOptionPrompt = "customMenuOptionPrompt";

				internal const string AAMainMenuQA = "AA_MainMenu_QA";

				internal const string AAGotoDtmfAutoAttendant = "aa_goto_dtmf_autoattendant";

				internal const string AAGotoOperator = "aa_goto_operator";

				internal const string NameOfDepartmentFormat = "nameOfDepartment{0}";

				internal const string SelectableDepartments = "selectableDepartments";

				internal const string ForwardCallsToDefaultMailbox = "forwardCallsToDefaultMailbox";
			}

			internal abstract class CallTransfer
			{
				internal const string Variable = "variable";

				internal const string Literal = "literal";

				internal const string TargetContactInfo = "targetContactInfo";

				internal const string ReferredByUri = "ReferredByUri";
			}

			internal abstract class OutDialing
			{
				internal const string DialingAccessDeniedPrompt = "dialingAccessDeniedPrompt";

				internal const string PhoneNumberToDial = "phoneNumberToDial";

				internal const string CanonicalizedNumber = "canonicalizedNumber";
			}

			internal abstract class Email
			{
				internal const string EmailSender = "emailSender";

				internal const string EmailSubject = "emailSubject";

				internal const string NormalizedSubject = "normalizedSubject";

				internal const string EmailReceivedTime = "emailReceivedTime";

				internal const string EmailRequestTime = "emailRequestTime";

				internal const string EmailRequestTimeRange = "emailRequestTimeRange";

				internal const string EmailReplyTime = "emailReplyTime";

				internal const string Location = "location";

				internal const string CalendarStatus = "calendarStatus";

				internal const string EmailToField = "emailToField";

				internal const string EmailCCField = "emailCCField";

				internal const string NumMessagesFromName = "numMessagesFromName";

				internal const string FindByName = "findByName";

				internal const string SenderCallerID = "senderCallerID";
			}

			internal abstract class MessagePlayer
			{
				internal const string MessagePartValue = "messagePart";

				internal const string WaveMessagePartValue = "waveMessagePart";

				internal const string TextMessagePartValue = "textMessagePart";
			}

			internal abstract class PlayOnPhone
			{
				internal const string GreetingType = "greetingType";
			}

			internal abstract class Record
			{
				internal const string Timeout = "recordingTimedOut";

				internal const string FailureCount = "recordingFailureCount";
			}

			internal abstract class PromptProvisioning
			{
				internal const string PromptProvContext = "promptProvContext";

				internal const string SelectedPrompt = "selectedPrompt";

				internal const string SelectedPromptGroup = "selectedPromptGroup";

				internal const string SelectedPromptType = "selectedPromptType";

				internal const string HolidayName = "holidayName";

				internal const string HolidayStartDate = "holidayStartDate";

				internal const string HolidayEndDate = "holidayEndDate";

				internal const string PlaybackIndex = "playbackIndex";

				internal const string HolidayCount = "holidayCount";

				internal const string MoreHolidaysAvailable = "moreHolidaysAvailable";

				internal const string HaveBusinessHoursPrompts = "haveBusinessHoursPrompts";

				internal const string HaveAfterHoursPrompts = "haveAfterHoursPrompts";

				internal const string HaveAutoAttendantPrompts = "haveAutoAttendantPrompts";

				internal const string HaveDialPlanPrompts = "haveDialPlanPrompts";

				internal const string HaveInfoAnnouncement = "haveInfoAnnouncement";

				internal const string HaveHolidayPrompts = "haveHolidayPrompts";

				internal const string HaveWelcomeGreeting = "haveWelcomeGreeting";

				internal const string HaveKeyMapping = "haveKeyMapping";

				internal const string HaveMainMenu = "haveMainMenu";
			}

			internal abstract class PersonalOptions
			{
				internal const string CurrentTimeZone = "currentTimeZone";

				internal const string TimeZoneIndex = "timeZoneIndex";

				internal const string OffsetHours = "offsetHours";

				internal const string OffsetMinutes = "offsetMinutes";
			}
		}

		internal abstract class Action
		{
			private Action()
			{
			}

			internal const string NullAction = "null";

			internal const string GetExtension = "getExtension";

			internal const string DoLogon = "doLogon";

			internal const string ValidateMailbox = "validateMailbox";

			internal const string CreateCallee = "createCallee";

			internal const string ValidateCaller = "validateCaller";

			internal const string ClearCaller = "clearCaller";

			internal const string ResetCallType = "resetCallType";

			internal const string QuickMessage = "quickMessage";

			internal const string OofShortcut = "oofShortcut";

			internal const string HandleCallSomeone = "handleCallSomeone";

			internal const string SetInitialSearchTargetGAL = "setInitialSearchTargetGAL";

			internal const string SetInitialSearchTargetContacts = "setInitialSearchTargetContacts";

			internal const string SetPromptProvContext = "setPromptProvContext";

			internal const string GetExternal = "getExternal";

			internal const string GetInternal = "getInternal";

			internal const string GetOof = "getOof";

			internal const string GetName = "getName";

			internal const string SaveExternal = "saveExternal";

			internal const string SaveInternal = "saveInternal";

			internal const string SaveOof = "saveOof";

			internal const string SaveName = "saveName";

			internal const string DeleteExternal = "deleteExternal";

			internal const string DeleteInternal = "deleteInternal";

			internal const string DeleteOof = "deleteOof";

			internal const string DeleteName = "deleteName";

			internal const string ValidatePassword = "validatePassword";

			internal const string MatchPasswords = "matchPasswords";

			internal const string GetSystemTask = "getSystemTask";

			internal const string GetFirstTimeUserTask = "getFirstTimeUserTask";

			internal const string FirstTimeUserComplete = "firstTimeUserComplete";

			internal const string GetOofStatus = "getOofStatus";

			internal const string SetOofStatus = "setOofStatus";

			internal const string UnsetOofStatus = "unsetOofStatus";

			internal const string GetGreeting = "getGreeting";

			internal const string SubmitVoiceMail = "submitVoiceMail";

			internal const string ClearVoiceMail = "clearVoiceMail";

			internal const string AppendVoiceMail = "appendVoiceMail";

			internal const string RecordPlayTime = "recordPlayTime";

			internal const string FillCallerInfo = "fillCallerInfo";

			internal const string SubmitVoiceMailUrgent = "submitVoiceMailUrgent";

			internal const string GetNewMessages = "getNewMessages";

			internal const string GetPriorityOfMessage = "getPriorityOfMessage";

			internal const string DeleteVoiceMail = "deleteVoiceMail";

			internal const string UndeleteVoiceMail = "undeleteVoiceMail";

			internal const string SaveVoiceMail = "saveVoiceMail";

			internal const string MarkUnreadVoiceMail = "markUnreadVoiceMail";

			internal const string FlagVoiceMail = "flagVoiceMail";

			internal const string GetEnvelopInfo = "getEnvelopInfo";

			internal const string GetNextMessage = "getNextMessage";

			internal const string GetPreviousMessage = "getPreviousMessage";

			internal const string ReplyVoiceMail = "replyVoiceMail";

			internal const string ForwardVoiceMail = "forwardVoiceMail";

			internal const string GetSavedMessages = "getSavedMessages";

			internal const string GetMessageReadProperty = "getMessageReadProperty";

			internal const string GetSenderName = "getSenderName";

			internal const string GetPlayOnPhoneType = "getPlayOnPhoneType";

			internal const string DiagnosticIsLocal = "isLocal";

			internal const string DiagnosticSendDtmf = "sendDtmf";

			internal abstract class Base
			{
				internal const string Disconnect = "disconnect";

				internal const string MoreOptions = "more";

				internal const string ClearRecording = "clearRecording";

				internal const string AppendRecording = "appendRecording";

				internal const string StopASR = "stopASR";

				internal const string SaveRecoEvent = "saveRecoEvent";

				internal const string SetSpeechError = "setSpeechError";
			}

			internal abstract class MessageCount
			{
				internal const string GetSummaryInfo = "getSummaryInfo";
			}

			internal abstract class Calendar
			{
				internal const string GetTodaysMeetings = "getTodaysMeetings";

				internal const string PreviousMeeting = "previousMeeting";

				internal const string NextMeeting = "nextMeeting";

				internal const string NextDay = "nextDay";

				internal const string GetDetails = "getDetails";

				internal const string GetParticipants = "getParticipants";

				internal const string LateForMeeting = "lateForMeeting";

				internal const string CancelOrDecline = "cancelOrDecline";

				internal const string CancelSeveral = "cancelSeveral";

				internal const string ReplyToOrganizer = "replyToOrganizer";

				internal const string Forward = "forward";

				internal const string CallOrganizer = "callOrganizer";

				internal const string ReplyToAll = "replyToAll";

				internal const string GiveShortcutHint = "giveShortcutHint";

				internal const string ParseDateSpeech = "parseDateSpeech";

				internal const string OpenCalendarDate = "openCalendarDate";

				internal const string NextMeetingSameDay = "nextMeetingSameDay";

				internal const string PreviousMeetingSameDay = "previousMeetingSameDay";

				internal const string LastMeetingSameDay = "lastMeetingSameDay";

				internal const string FirstMeetingSameDay = "firstMeetingSameDay";

				internal const string AcceptMeeting = "acceptMeeting";

				internal const string MarkAsTentative = "markAsTentative";

				internal const string SeekValidMeeting = "seekValidMeeting";

				internal const string IsValidMeeting = "isValidMeeting";

				internal const string SkipHeader = "skipHeader";

				internal const string ReadTheHeader = "readTheHeader";

				internal const string ClearLateMinutes = "clearMinutesLate";

				internal const string ParseLateMinutes = "parseLateMinutes";

				internal const string ParseClearTimeDays = "parseClearTimeDays";

				internal const string ParseClearHours = "parseClearHours";

				internal const string GiveLateMinutesHint = "giveLateMinutesHint";

				internal const string SelectLanguage = "selectLanguage";

				internal const string NextLanguage = "nextLanguage";
			}

			internal abstract class PersonalOptions
			{
				internal const string ToggleASR = "toggleASR";

				internal const string ToggleOOF = "toggleOOF";

				internal const string ToggleEmailOOF = "toggleEmailOOF";

				internal const string ToggleTimeFormat = "toggleTimeFormat";

				internal const string SetGreetingsAction = "setGreetingsAction";

				internal const string FindTimeZone = "findTimeZone";

				internal const string NextTimeZone = "nextTimeZone";

				internal const string FirstTimeZone = "firstTimeZone";

				internal const string SelectTimeZone = "selectTimeZone";
			}

			internal abstract class PlayBack
			{
				internal const string Pause = "pause";

				internal const string Rewind = "rewind";

				internal const string FastForward = "fastForward";

				internal const string SlowDown = "slowDown";

				internal const string SpeedUp = "speedUp";

				internal const string Help = "playBackHelp";

				internal const string ResetPlayback = "resetPlayback";
			}

			internal abstract class SendMessage
			{
				internal const string AddRecipientBySearch = "addRecipientBySearch";

				internal const string RemoveRecipient = "removeRecipient";

				internal const string CancelMessage = "cancelMessage";

				internal const string Send = "sendMessage";

				internal const string SendUrgent = "sendMessageUrgent";
			}

			internal abstract class DirectorySearch
			{
				internal const string ChangeSearchMode = "changeSearchMode";

				internal const string SearchDirectory = "searchDirectory";

				internal const string StartNewSearch = "startNewSearch";

				internal const string ContinueSearch = "continueSearch";

				internal const string AnyMoreResultsToPlay = "anyMoreResultsToPlay";

				internal const string ValidateSearchSelection = "validateSearchSelection";

				internal const string ValidateInput = "ValidateInput";

				internal const string ReplayResults = "replayResults";

				internal const string ChangeSearchTarget = "changeSearchTarget";

				internal const string SetSearchTargetToContacts = "setSearchTargetToContacts";

				internal const string SetSearchTargetToGlobalAddressList = "setSearchTargetToGlobalAddressList";

				internal const string SearchDirectoryByExtension = "searchDirectoryByExtension";

				internal const string SetMobileNumber = "setMobileNumber";

				internal const string SetBusinessNumber = "setBusinessNumber";

				internal const string SetHomeNumber = "setHomeNumber";

				internal const string HandleInvalidSearchKey = "handleInvalidSearchKey";

				internal const string PlayContactDetails = "playContactDetails";

				internal const string CheckNonUmExtension = "checkNonUmExtension";
			}

			internal abstract class AsrSearch
			{
				internal const string InitConfirmQA = "initConfirmQA";

				internal const string InitConfirmAgainQA = "initConfirmAgainQA";

				internal const string InitAskAgainQA = "initAskAgainQA";

				internal const string InitNameCollisionQA = "initNameCollisionQA";

				internal const string InitConfirmViaListQA = "initConfirmViaListQA";

				internal const string InitPromptForAliasConfirmQA = "initPromptForAliasConfirmQA";

				internal const string SetExtensionNumber = "setExtensionNumber";

				internal const string HandleRecognition = "handleRecognition";

				internal const string HandleYes = "handleYes";

				internal const string HandleNo = "handleNo";

				internal const string HandleNotListed = "handleNotListed";

				internal const string HandleNotSure = "handleNotSure";

				internal const string HandleMaybe = "handleMaybe";

				internal const string HandlePoliteEnd = "handlePoliteEnd";

				internal const string HandleChoice = "handleChoice";

				internal const string HandleValidChoice = "handleValidChoice";

				internal const string HandleDtmfChoice = "handleDtmfChoice";

				internal const string ResetSearchState = "resetSearchState";
			}

			internal abstract class AsrContacts
			{
				internal const string ProcessResult = "processResult";

				internal const string SetName = "setName";

				internal const string SetContactInfoVariables = "setContactInfoVariables";

				internal const string PrepareForTransferToCell = "prepareForTransferToCell";

				internal const string PrepareForTransferToOffice = "prepareForTransferToOffice";

				internal const string PrepareForTransferToHome = "prepareForTransferToHome";

				internal const string SetEmailAddress = "setEmailAddress";

				internal const string SelectEmailAddress = "selectEmailAddress";

				internal const string SetContactInfo = "setContactInfo";

				internal const string InitializeNamesGrammar = "initializeNamesGrammar";

				internal const string RetryAsrSearch = "retryAsrSearch";

				internal const string HandlePlatformFailure = "handlePlatformFailure";
			}

			internal abstract class AutoAttendant
			{
				internal const string SetExtensionNumber = "setExtensionNumber";

				internal const string InitializeState = "initializeState";

				internal const string TransferToOperator = "transferToOperator";

				internal const string SetOperatorNumber = "setOperatorNumber";

				internal const string SetCustomMenuVoicemailTarget = "setCustomMenuVoicemailTarget";

				internal const string TransferToPAASiteFailed = "transferToPAASiteFailed";

				internal const string SetCustomMenuTargetPAA = "setCustomMenuTargetPAA";

				internal const string SetCustomExtensionNumber = "setCustomExtensionNumber";

				internal const string SetFallbackAutoAttendant = "setFallbackAutoAttendant";

				internal const string SetCustomMenuAutoAttendant = "setCustomMenuAutoAttendant";

				internal const string ProcessCustomMenuSelection = "processCustomMenuSelection";

				internal const string PrepareForTransferToPaa = "prepareForTransferToPaa";

				internal const string ComputeDtmfFallbackAction = "computeDtmfFallbackAction";

				internal const string PrepareForANROperatorTransfer = "prepareForANROperatorTransfer";

				internal const string PrepareForProtectedSubscriberOperatorTransfer = "prepareForProtectedSubscriberOperatorTransfer";

				internal const string PrepareForTransferToDtmfFallbackAutoAttendant = "prepareForTransferToDtmfFallbackAutoAttendant";

				internal const string PrepareForTransferToSendMessage = "prepareForTransferToSendMessage";

				internal const string PrepareForTransferToKeyMappingExtension = "prepareForTransferToKeyMappingExtension";

				internal const string PrepareForTransferToKeyMappingAutoAttendant = "prepareForTransferToKeyMappingAutoAttendant";

				internal const string PrepareForUserInitiatedOperatorTransfer = "prepareForUserInitiatedOperatorTransfer";

				internal const string PrepareForUserInitiatedOperatorTransferFromOpeningMenu = "prepareForUserInitiatedOperatorTransferFromOpeningMenu";

				internal const string HandleMissingGrammarFile = "handleMissingGrammarFile";

				internal const string HandleFaxTone = "handleFaxTone";
			}

			internal abstract class OutDialing
			{
				internal const string CheckRestrictedUser = "checkRestrictedUser";

				internal const string CanonicalizeNumber = "canonicalizeNumber";

				internal const string CheckDialPermissions = "checkDialPermissions";

				internal const string ProcessResult = "processResult";
			}

			internal abstract class Email
			{
				internal const string NextMessage = "nextMessage";

				internal const string NextUnreadMessage = "nextUnreadMessage";

				internal const string PreviousMessage = "previousMessage";

				internal const string AcceptMeeting = "acceptMeeting";

				internal const string AcceptMeetingTentative = "acceptMeetingTentative";

				internal const string DeclineMeeting = "declineMeeting";

				internal const string DeleteMessage = "deleteMessage";

				internal const string DeleteThread = "deleteThread";

				internal const string HideThread = "hideThread";

				internal const string CommitPendingDeletions = "commitPendingDeletions";

				internal const string FindByName = "findByName";

				internal const string UndeleteMessage = "undeleteMessage";

				internal const string Reply = "reply";

				internal const string ReplyAll = "replyAll";

				internal const string Forward = "forward";

				internal const string SaveMessage = "saveMessage";

				internal const string MarkUnread = "markUnread";

				internal const string FlagMessage = "flagMessage";

				internal const string SetMobileNumber = "setMobileNumber";

				internal const string SetBusinessNumber = "setBusinessNumber";

				internal const string SetHomeNumber = "setHomeNumber";

				internal const string SelectLanguage = "selectLanguage";

				internal const string NextLanguage = "nextLanguage";
			}

			internal abstract class MessagePlayer
			{
				internal const string NextMessagePart = "nextMessagePart";

				internal const string FirstMessagePart = "firstMessagePart";

				internal const string NextMessageSection = "nextMessageSection";

				internal const string PreviousMessageSection = "firstMessageSection";

				internal const string SelectLanguagePause = "selectLanguagePause";

				internal const string NextLanguagePause = "nextLanguagePause";
			}

			internal abstract class CAMessageSubmission
			{
				internal const string IsQuotaExceeded = "isQuotaExceeded";

				internal const string IsPipelineHealthy = "isPipelineHealthy";

				internal const string CanAnnonLeaveMessage = "canAnnonLeaveMessage";

				internal const string HandleFailedTransfer = "HandleFailedTransfer";
			}

			internal abstract class PromptProvisioning
			{
				internal const string PublishPrompt = "publishPrompt";

				internal const string CanUpdatePrompts = "canUpdatePrompts";

				internal const string SetDialPlanContext = "setDialPlanContext";

				internal const string SetAutoAttendantContext = "setAutoAttendantContext";

				internal const string PrepareForPlayback = "prepareForPlayback";

				internal const string SelectGroupBusinessHours = "selectBusinessHoursGroup";

				internal const string SelectGroupAfterHours = "selectAfterHoursGroup";

				internal const string SelectKeyMapping = "selectKeyMapping";

				internal const string SelectHolidaySchedule = "selectHolidaySchedule";

				internal const string SelectWelcomeGreeting = "selectWelcomeGreeting";

				internal const string SelectInfoAnnouncement = "selectInfoAnnouncement";

				internal const string SelectMainMenuCustomPrompt = "selectMainMenuCustomPrompt";

				internal const string SelectPromptIndex = "selectPromptIndex";

				internal const string NextPlaybackIndex = "nextPlaybackIndex";

				internal const string ResetPlaybackIndex = "resetPlaybackIndex";

				internal const string SelectNextHolidayPage = "selectNextHolidayPage";

				internal const string ExitPromptProvisioning = "exitPromptProvisioning";
			}
		}

		internal abstract class Condition
		{
			internal const string RepeatMode = "repeat";

			internal const string MoreOptions = "more";

			internal const string ReplyIntro = "replyIntro";

			internal const string ReplyAllIntro = "replyAllIntro";

			internal const string DeclineIntro = "declineIntro";

			internal const string CancelIntro = "cancelIntro";

			internal const string ClearCalendarIntro = "clearCalendarIntro";

			internal const string ForwardIntro = "forwardIntro";

			internal const string CalendarAccessEnabled = "calendarAccessEnabled";

			internal const string EmailAccessEnabled = "emailAccessEnabled";

			internal const string WavePart = "wavePart";

			internal const string TextPart = "textPart";

			internal const string KnowSenderPhoneNumber = "knowSenderPhoneNumber";

			internal const string WaitForSourcePartyInfo = "waitForSourcePartyInfo";

			internal const string DiagnosticTUILogonCheck = "diagnosticTUILogonCheck";

			internal abstract class MessageCount
			{
				internal const string IsInProgress = "isInProgress";

				internal const string IsMaxEmail = "isMaxEmail";
			}

			internal abstract class Calendar
			{
				internal const string Past = "past";

				internal const string Present = "present";

				internal const string Future = "future";

				internal const string Conflict = "conflict";

				internal const string First = "first";

				internal const string FirstConflict = "firstConflict";

				internal const string Middle = "middle";

				internal const string Last = "last";

				internal const string Initial = "initial";

				internal const string Today = "today";

				internal const string Tentative = "tentative";

				internal const string Owner = "owner";

				internal const string LocationPhone = "locationPhone";

				internal const string OrganizerPhone = "organizerPhone";

				internal const string IsMeeting = "isMeeting";

				internal const string IsAllDayEvent = "isAllDayEvent";

				internal const string GiveShortcutHint = "giveShortcutHint";

				internal const string SkipHeader = "skipHeader";

				internal const string GiveMinutesLateHint = "giveMinutesLateHint";

				internal const string ConflictsWithLastHeard = "conflictWithLastHeard";

				internal const string DateChanged = "dateChanged";
			}

			internal abstract class Email
			{
				internal const string MeetingRequest = "meetingRequest";

				internal const string FirstMessage = "firstMessage";

				internal const string LastMessage = "lastMessage";

				internal const string Free = "free";

				internal const string Busy = "busy";

				internal const string Tentative = "tentative";

				internal const string OOF = "oof";

				internal const string MeetingAccepted = "alreadyAccepted";

				internal const string MeetingOver = "meetingOver";

				internal const string OutOfDate = "outOfDate";

				internal const string MeetingCancellation = "meetingCancellation";

				internal const string Urgent = "urgent";

				internal const string Protected = "protected";

				internal const string Attachments = "attachments";

				internal const string Drm = "drm";

				internal const string Read = "read";

				internal const string PlayedUndelete = "playedUndelete";

				internal const string ContactEntry = "ContactEntry";

				internal const string GALEntry = "GALEntry";

				internal const string ValidSenderPhone = "validSenderPhone";

				internal const string IsRecorded = "isRecorded";

				internal const string IsReply = "isReply";

				internal const string IsForward = "isForward";

				internal const string IsMissedCall = "isMissedCall";

				internal const string ReceivedDayOfWeek = "receivedDayOfWeek";

				internal const string ReceivedOffset = "receivedOffset";

				internal const string MeetingDayOfWeek = "meetingDayOfWeek";

				internal const string MeetingOffset = "meetingOffset";

				internal const string CanUndelete = "canUndelete";

				internal const string UndeletedAConversation = "undeletedAConversation";

				internal const string InFindMode = "inFindMode";
			}

			internal abstract class MessagePlayer
			{
				internal const string IsEmptyText = "isEmptyText";

				internal const string IsEmptyWave = "isEmptyWave";

				internal const string PlayMixedContentIntro = "playMixedContentIntro";

				internal const string PlayAudioContentIntro = "playAudioContentIntro";

				internal const string PlayTextContentIntro = "playTextContentIntro";
			}

			internal abstract class Voicemail
			{
				internal const string PlayedUndelete = "playedUndelete";

				internal const string KnowVoicemailSender = "knowVoicemailSender";

				internal const string IsHighPriority = "isHighPriority";

				internal const string IsProtected = "isProtected";
			}

			internal abstract class PersonalOptions
			{
				internal const string Oof = "Oof";

				internal const string EmailOof = "emailOof";

				internal const string TimeFormat24 = "timeFormat24";

				internal const string CanToggleTimeFormat = "canToggleTimeFormat";

				internal const string CanToggleASR = "canToggleASR";

				internal const string LastAction = "lastAction";

				internal const string PlayGMTOffset = "playGMTOffset";

				internal const string PositiveOffset = "positiveOffset";
			}

			internal abstract class QA
			{
				internal const string MainMenuQA = "MainMenuQA";
			}

			internal abstract class PlayOnPhone
			{
				internal const string OofCustom = "OofCustom";

				internal const string NormalCustom = "NormalCustom";
			}
		}

		internal abstract class TransitionEvent
		{
			private TransitionEvent()
			{
			}

			internal static string OutId(uint depth)
			{
				return string.Format(CultureInfo.InvariantCulture, "out-{0}", new object[]
				{
					depth.ToString(CultureInfo.InvariantCulture)
				});
			}

			internal const string NoKey = "noKey";

			internal const string DtmfSent = "dtmfSent";

			internal const string ToolInfoSent = "toolInfoSent";

			internal const string DivertedExtensionNotAllowVoiceMail = "divertedExtensionNotAllowVoiceMail";

			internal const string Timeout = "timeout";

			internal const string Silence = "silence";

			internal const string RecordFailure = "recordFailure";

			internal const string AnyKey = "anyKey";

			internal const string StopEvent = "stopEvent";

			internal const string UserHangup = "userHangup";

			internal const string MaxProsodyRate = "maxProsodyRate";

			internal const string MinProsodyRate = "minProsodyRate";

			internal const string XsoError = "xsoError";

			internal const string QuotaExceeded = "quotaExceeded";

			internal const string UnknownLanguage = "unknownLanguage";

			internal const string ExtensionFound = "extensionFound";

			internal const string VirtualNumberCall = "virtualNumberCall";

			internal const string TroubleshootingToolCall = "troubleshootingToolCall";

			internal const string InvalidExtension = "invalidExtension";

			internal const string MaxInvalidExtensions = "maxInvalidExtensions";

			internal const string ValidUser = "validUser";

			internal const string ValidDtmfAutoAttendant = "validDtmfAutoAttendant";

			internal const string ValidSpeechAutoAttendant = "validSpeechAutoAttendant";

			internal const string RunDefaultAutoAttendant = "runDefaultAutoAttendant";

			internal const string RunCallExtension = "runCallExtension";

			internal const string MailboxFound = "mailboxFound";

			internal const string MailboxNotSupported = "mailboxNotSupported";

			internal const string NoMoreServers = "noMoreServers";

			internal const string MaxInvalidMailbox = "maxInvalidMailbox";

			internal const string LogonAsr = "logonAsr";

			internal const string LogonOk = "logonOk";

			internal const string LogonPP = "logonPP";

			internal const string BadPasswordDisconnect = "badPasswordDisconnect";

			internal const string BadPasswordLockout = "badPasswordLockout";

			internal const string BadPasswordReset = "badPasswordReset";

			internal const string StaleChecksum = "staleChecksum";

			internal const string NoExternal = "noExternal";

			internal const string NoInternal = "noInternal";

			internal const string NoOof = "noOof";

			internal const string NoName = "noName";

			internal const string PasswordValidated = "passwordValidated";

			internal const string PasswordsMatch = "passwordsMatch";

			internal const string ChangePasswordTask = "changePasswordTask";

			internal const string FirstTimeUserTask = "firstTimeUserTask";

			internal const string OofStatusTask = "oofStatusTask";

			internal const string FaxRequestAccepted = "faxRequestAccepted";

			internal const string RecordNameTask = "recordNameTask";

			internal const string RecordInternalTask = "recordInternalTask";

			internal const string RecordExternalTask = "recordExternalTask";

			internal const string NoGreeting = "noGreeting";

			internal const string NoGreetingOof = "noGreetingOof";

			internal const string QuotaNotExceeded = "quotaNotExceeded";

			internal const string PipelineHealthy = "pipelineHealthy";

			internal const string AnnonCanLeaveMessage = "annonCanLeaveMessage";

			internal const string IsHighPriority = "isHighPriority";

			internal const string NoNewMessages = "noNewMessages";

			internal const string NoSavedMessages = "noSavedMessages";

			internal const string NoPreviousNewMessages = "noPreviousNewMessages";

			internal const string NoPreviousSavedMessages = "noPreviousSavedMessages";

			internal const string CurrentNewMessage = "currentNewMessage";

			internal const string CurrentSavedMessage = "currentSavedMessage";

			internal const string InvalidWaveAttachment = "invalidWaveAttachment";

			internal const string NameNotAvailable = "nameNotAvailable";

			internal const string FaxTone = "faxtone";

			internal const string PlayOnPhone = "playOnPhone";

			internal const string FindMeSubscriberCall = "findMeSubscriberCall";

			internal const string PlayOnPhoneVoicemail = "playOnPhoneVoicemail";

			internal const string PlayOnPhoneAAGreeting = "playOnPhoneAAGreeting";

			internal const string PlayOnPhoneGreeting = "playOnPhoneGreeting";

			internal const string PlayOnPhonePAAGreeting = "playOnPhonePAAGreeting";

			internal const string UMDiagnosticCall = "umdiagnosticCall";

			internal const string ForcePinLogin = "forcePinLogin";

			internal const string LocalDiagnostic = "local";

			internal const string PAAFindmeMoreNumbersLeft = "moreFindMeNumbersLeft";

			internal const string AllFindMeNumbersFailedAccessCheck = "dialingRulesCheckFailed";

			internal const string MaxAllowedCallsLimitReached = "maxAllowedCallsLimitReached";

			internal const string BlockedCall = "blockedCall";

			internal const string NoActionLeft = "noActionLeft";

			internal static readonly byte[] AnyKeyBytes = Encoding.ASCII.GetBytes("anyKey");

			internal abstract class Calendar
			{
				internal const string NoMeetings = "noMeetings";

				internal const string EmptyCalendar = "emptyCalendar";

				internal const string InvalidTime = "invalidTime";
			}

			internal abstract class CallTransfer
			{
				internal const string TransferOK = "transferOK";

				internal const string TransferFailed = "transferFailed";
			}

			internal abstract class SendMessage
			{
				internal const string UnknownRecipient = "unknownRecipient";

				internal const string NoRecipients = "noRecipients";
			}

			internal abstract class DirectorySearch
			{
				internal const string ResultsLessThanAllowed = "resultsLessThanAllowed";

				internal const string ResultsMoreThanAllowed = "resultsMoreThanAllowed";

				internal const string MoreResultsToPlay = "moreResultsToPlay";

				internal const string NoMoreResultsToPlay = "noMoreResultsToPlay";

				internal const string ValidSelection = "validSelection";

				internal const string InvalidSelection = "invalidSelection";

				internal const string InvalidInput = "invalidInput";

				internal const string ValidInput = "validInput";

				internal const string InvalidSearchKey = "invalidSearchKey";

				internal const string NoResultsMatched = "noResultsMatched";

				internal const string PhoneNumberNotFound = "phoneNumberNotFound";

				internal const string UserNotEnabledForUm = "userNotEnabledForUm";

				internal const string MaxNumberOfTriesExceeded = "maxNumberOfTriesExceeded";

				internal const string OperatorFallback = "operatorFallback";

				internal const string AmbiguousMatches = "ambiguousMatches";

				internal const string PromptForAlias = "promptForAlias";

				internal const string ADTransientError = "adTransientError";

				internal const string AllowCallNonUmExtension = "allowCallNonUmExtension";

				internal const string DenyCallNonUmExtension = "denyCallNonUmExtension";
			}

			internal abstract class AsrSearch
			{
				internal const string DoFallback = "doFallback";

				internal const string InvalidSearchResult = "invalidSearchResult";

				internal const string Collision = "collision";

				internal const string ValidChoice = "validChoice";

				internal const string ConfirmViaList = "confirmViaList";

				internal const string DoAskAgainQA = "doAskAgainQA";

				internal const string InvalidSelection = "invalidSelection";

				internal const string ResultsMoreThanAllowed = "resultsMoreThanAllowed";

				internal const string PromptForAlias = "promptForAlias";

				internal const string OperatorFallback = "operatorFallback";

				internal const string RetrySearch = "retrySearch";
			}

			internal abstract class AsrContacts
			{
				internal const string InvalidOption = "invalidOption";

				internal const string InvalidResult = "invalidResult";

				internal const string MoreThanOneAddress = "moreThanOneAddress";

				internal const string NoGrammarFile = "noGrammarFile";
			}

			internal abstract class AutoAttendant
			{
				internal const string InvalidOption = "invalidOption";

				internal const string NoPAAFound = "noPAAFound";

				internal const string CannotTransferToCustomExtension = "cannotTransferToCustomExtension";

				internal const string TargetPAAInDifferentSite = "targetPAAInDifferentSite";

				internal const string NoTransferToOperator = "noTransferToOperator";

				internal const string FallbackAutoAttendantFailure = "fallbackAutoAttendantFailure";
			}

			internal abstract class OutDialing
			{
				internal const string DialingPermissionCheckFailed = "dialingPermissionCheckFailed";

				internal const string ValidCanonicalNumber = "validCanonicalNumber";

				internal const string NumberCanonicalizationFailed = "numberCanonicalizationFailed";

				internal const string RestrictedUser = "restrictedUser";

				internal const string UnrestrictedUser = "unrestrictedUser";

				internal const string UnreachableUser = "unreachableUser";

				internal const string AllowSendMessageOnly = "allowSendMessageOnly";

				internal const string AllowCallOnly = "allowCallOnly";
			}

			internal abstract class Email
			{
				internal const string EndOfMessages = "endOfMessages";

				internal const string NoPreviousMessages = "noPreviousMessages";
			}

			internal abstract class MessagePlayer
			{
				internal const string EndOfSection = "endOfSection";
			}

			internal abstract class SpeechMenu
			{
				internal const string Main = "main";

				internal const string Repeat = "repeat";

				internal const string Mumble1 = "mumble1";

				internal const string Mumble2 = "mumble2";

				internal const string Silence1 = "silence1";

				internal const string Silence2 = "silence2";

				internal const string SpeechError = "speechError";

				internal const string Help = "help";

				internal const string InvalidCommand = "invalidCommand";

				internal const string DtmfFallback = "dtmfFallback";
			}

			internal abstract class PlayOnPhoneAA
			{
				internal const string FailedToSaveGreeting = "failedToSaveGreeting";
			}

			internal abstract class PromptProvisioning
			{
				internal const string UpdatePromptsAllowed = "updatePromptsAllowed";

				internal const string PublishingError = "publishingError";

				internal const string InvalidSelectedPrompt = "invalidSelectedPrompt";

				internal const string EndOfHolidayPage = "endOfHolidayPage";
			}

			internal abstract class PersonalOptions
			{
				internal const string InvalidTimeZone = "invalidTimeZone";

				internal const string InvalidTimeFormat = "invalidTimeFormat";

				internal const string EndOfTimeZoneList = "endOfTimeZoneList";
			}

			internal abstract class PersonalAutoAttendant
			{
				internal const string MenuRetriesExceeded = "menuRetriesExceeded";
			}
		}

		internal abstract class DtmfType
		{
			private DtmfType()
			{
			}

			internal const string Option = "option";

			internal const string Extension = "extension";

			internal const string Password = "password";

			internal const string Name = "name";

			internal const string Numeric = "numeric";
		}

		internal abstract class PromptType
		{
			internal const string WaveFile = "wave";

			internal const string VarWaveFile = "varwave";

			internal const string TempWaveFile = "tempwave";

			internal const string TextVariable = "text";

			internal const string DateVariable = "date";

			internal const string DigitVariable = "digit";

			internal const string TimeVariable = "time";

			internal const string CardinalVariable = "cardinal";

			internal const string EmailVariable = "email";

			internal const string Statement = "statement";

			internal const string MultiStatement = "multiStatement";

			internal const string Group = "group";

			internal const string SimpleTime = "simpleTime";

			internal const string TimeRange = "timeRange";

			internal const string TelephoneNumber = "telephone";

			internal const string Name = "name";

			internal const string Address = "address";

			internal const string Silence = "silence";

			internal const string TimeZone = "timeZone";

			internal const string Bookmark = "bookmark";

			internal const string EmailAddress = "emailAddress";

			internal const string Language = "language";

			internal const string LanguageList = "languageList";

			internal const string TextList = "textList";

			internal const string DisambiguatedName = "disambiguatedName";

			internal const string BusinessHours = "businessHours";

			internal const string DayOfWeekTime = "dayOfWeekTime";

			internal const string ScheduleGroup = "scheduleGroup";

			internal const string DayOfWeekList = "dayOfWeekList";

			internal const string ScheduleInterval = "scheduleInterval";

			internal const string AACustomMenu = "aaCustomMenu";

			internal const string AAWelcomeGreeting = "aaWelcomeGreeting";

			internal const string AABusinessLocation = "aaBusinessLocation";

			internal const string MbxVoicemailGreeting = "mbxVoicemailGreeting";

			internal const string MbxAwayGreeting = "mbxAwayGreeting";

			internal const string ScheduleGroupList = "scheduleGroupList";

			internal const string ScheduleIntervalList = "scheduleIntervalList";

			internal const string SearchItemDetail = "searchItemDetail";

			internal const string CallerNameOrNumber = "callerNameOrNumber";
		}

		internal abstract class GrammarType
		{
			internal const string Static = "static";

			internal const string Dynamic = "dynamic";
		}

		internal abstract class RecoEvent
		{
			internal const string Failure = "recoFailure";

			internal const string MainMenu = "recoMainMenu";

			internal const string Repeat = "recoRepeat";

			internal const string Help = "recoHelp";

			internal const string Yes = "recoYes";

			internal const string No = "recoNo";

			internal const string Goodbye = "recoGoodbye";

			internal const string CompleteDate = "recoCompleteDate";

			internal const string PartialDate = "recoPartialDate";

			internal const string CompleteDateWithStartTime = "recoCompleteDateWithStartTime";

			internal const string CompleteDateWithStartTimeAndDuration = "recoCompleteDateWithStartTimeAndDuration";

			internal abstract class MainMenuQA
			{
				internal const string Calendar = "recoCalendar";

				internal const string Contacts = "recoContacts";

				internal const string Directory = "recoDirectory";

				internal const string Email = "recoEmail";

				internal const string VoiceMail = "recoVoiceMail";

				internal const string PersonalOptions = "recoPersonalOptions";

				internal const string SendMessage = "recoSendMessage";

				internal const string Call = "recoCall";
			}

			internal abstract class Record
			{
				internal const string PlayItBack = "recoPlayItBack";

				internal const string Restart = "recoRestart";

				internal const string Send = "recoSend";

				internal const string SendUrgent = "recoSendUrgent";

				internal const string SendPrivate = "recoSendPrivate";

				internal const string SendPrivateAndImportant = "recoSendPrivateAndUrgent";

				internal const string Cancel = "recoCancel";

				internal const string Continue = "recoContinue";
			}

			internal abstract class PlayBack
			{
				internal const string Rewind = "recoRewind";
			}

			internal abstract class Calendar
			{
				internal const string Day = "recoDay";

				internal const string Date = "recoDate";

				internal const string Next = "recoNext";

				internal const string NextUnread = "recoNextUnread";

				internal const string Previous = "recoPrevious";

				internal const string First = "recoFirst";

				internal const string Last = "recoLast";

				internal const string NextDay = "recoNextDay";

				internal const string Accept = "recoAccept";

				internal const string Decline = "recoDecline";

				internal const string Cancel = "recoCancel";

				internal const string CallOrganizer = "recoCallOrganizer";

				internal const string ReplyOrganizer = "recoReplyOrganizer";

				internal const string ReplyAll = "recoReplyAll";

				internal const string CallLocation = "recoCallLocation";

				internal const string SendLateMessage = "recoSendLateMessage";

				internal const string SendLateMessageMinutes = "recoSendLateMessageMinutes";

				internal const string MeetingDetails = "recoMeetingDetails";

				internal const string ParticipantDetails = "recoParticipantDetails";

				internal const string ClearCalendar = "recoClearCalendar";

				internal const string MoreOptions = "recoMoreOptions";

				internal const string ReadTheHeader = "recoReadTheHeader";

				internal const string Minutes = "recoMinutes";

				internal const string MinutesRange = "recoMinutesRange";

				internal const string NotSure = "recoNotSure";

				internal const string TimePhrase = "recoTimePhrase";

				internal const string DayPhrase = "recoDayPhrase";

				internal const string AmbiguousPhrase = "recoAmbiguousPhrase";

				internal const string TimeOfDay = "recoTimeOfDay";

				internal const string NumberOfDays = "recoNumberOfDays";
			}

			internal abstract class AutoAttendant
			{
				internal const string RecoName = "recoName";

				internal const string RecoDepartment = "recoDepartment";

				internal const string RecoNameOrDepartment = "recoNameOrDepartment";

				internal const string RecoReception = "recoReception";

				internal const string RecoNotSure = "recoNotSure";

				internal const string RecoMaybe = "recoMaybe";

				internal const string RecoPoliteEnd = "recoPoliteEnd";

				internal const string RecoChoice = "recoChoice";

				internal const string RecoNotListed = "recoNotListed";

				internal const string RecoSendMessage = "recoSendMessage";
			}

			internal abstract class Email
			{
				internal const string Reply = "recoReply";

				internal const string ReplyAll = "recoReplyAll";

				internal const string CallSender = "recoCallSender";

				internal const string Forward = "recoForward";

				internal const string ForwardWithName = "recoForwardWithName";

				internal const string ForwardToContact = "recoForwardToContact";

				internal const string FindByContact = "recoFindByContact";

				internal const string FindByName = "recoFindByName";

				internal const string FindByNameWithName = "recoFindByNameWithName";

				internal const string Delete = "recoDelete";

				internal const string Undelete = "recoUndelete";

				internal const string MarkAsUnread = "recoMarkAsUnread";

				internal const string Next = "recoNext";

				internal const string Previous = "recoPrevious";

				internal const string HideThread = "recoHideThread";

				internal const string DeleteThread = "recoDeleteThread";

				internal const string FlagForFollowup = "recoFlagForFollowup";

				internal const string Play = "recoPlay";

				internal const string FastForward = "recoFastForward";

				internal const string Rewind = "recoRewind";

				internal const string SpeedUp = "recoSpeedUp";

				internal const string SlowDown = "recoSlowDown";

				internal const string SelectLanguage = "recoSelectLanguage";

				internal const string Accept = "recoAccept";

				internal const string AcceptTentative = "recoAcceptTentative";

				internal const string Decline = "recoDecline";

				internal const string MoreOptions = "recoMoreOptions";

				internal const string MessageDetails = "recoMessageDetails";

				internal const string Pause = "recoPause";

				internal const string ReadTheHeader = "recoReadTheHeader";

				internal const string EndOfMessage = "recoEndOfMessage";

				internal const string Language = "recoLanguage";

				internal const string EnvelopeInfo = "recoEnvelopeInfo";
			}

			internal abstract class Contacts
			{
				internal const string RecoReadDetails = "recoReadDetails";

				internal const string RecoCallCell = "recoCallCell";

				internal const string RecoCallOffice = "recoCallOffice";

				internal const string RecoCallHome = "recoCallHome";

				internal const string RecoSendMessage = "recoSendMessage";

				internal const string RecoFindAnotherContact = "recoFindAnotherContact";
			}
		}

		internal abstract class RpcError
		{
			internal const int EndpointNotRegistered = 1753;

			internal const int RpcServerCallFailedDne = 1727;
		}

		internal abstract class SemanticItems
		{
			internal const string RecoEvent = "RecoEvent";

			internal const string Extension = "Extension";

			internal const string ResultType = "ResultType";

			internal const string DepartmentName = "DepartmentName";

			internal const string DirectoryContact = "DirectoryContact";

			internal const string Department = "Department";

			internal const string CustomMenuTarget = "CustomMenuTarget";

			internal const string MappedKey = "MappedKey";

			internal const string PromptFileName = "PromptFileName";

			internal const string PersonalContact = "PersonalContact";

			internal const string SMTP = "SMTP";

			internal const string PersonId = "PersonId";

			internal const string GalLinkId = "GALLinkID";

			internal const string ObjectGuid = "ObjectGuid";

			internal const string UmSubscriberObjectGuid = "UmSubscriberObjectGuid";

			internal const string ContactId = "ContactId";

			internal const string ContactName = "ContactName";

			internal const string DisambiguationField = "DisambiguationField";

			internal const string Choice = "Choice";

			internal const string Language = "Language";

			internal abstract class Calendar
			{
				internal const string SpokenDay = "SpokenDay";

				internal const string RelativeDayOffset = "RelativeDayOffset";

				internal const string Month = "Month";

				internal const string Day = "Day";

				internal const string Year = "Year";

				internal const string Minutes = "Minutes";

				internal const string RangeStart = "RangeStart";

				internal const string RangeEnd = "RangeEnd";

				internal const string Time = "Time";

				internal const string Days = "Days";

				internal const string Hour = "Hour";

				internal const string AlternateHour = "AlternateHour";

				internal const string Minute = "Minute";

				internal const string DurationInMinutes = "DurationInMinutes";

				internal const string StartHour = "StartHour";

				internal const string StartMinute = "StartMinute";

				internal const string IsStartHourRelative = "IsStartHourRelative";
			}

			internal abstract class Email
			{
			}
		}

		internal abstract class PromptLimits
		{
			internal const int DefaultPromptLimit = 5;

			internal const string CalendarDateHint = "calendarDateHint";

			internal const string MailForwardHint = "mailForwardHint";

			internal const string MailFindHint = "mailFindHint";

			internal const string VuiUndeleteHint = "vuiUndeleteHint";

			internal const string DtmfDirectoryHint = "dtmfDirectoryHint";
		}

		internal abstract class MobileSpeechRecognition
		{
			internal const string MobilePeopleSearchGrammarRuleName = "MobilePeopleSearch";

			internal const string FindPersonByNameMobileGrammarRuleName = "FindPersonByNameMobile";

			internal const string MobileRecoElementName = "MobileReco";

			internal const string AlternateElementName = "Alternate";

			internal const string TextAttributeName = "text";

			internal const string PersonalContactSearchElementName = "PersonalContactSearch";

			internal const string GALSearchElementName = "GALSearch";

			internal const string ResultTypeAttributeName = "ResultType";

			internal const string ConfidenceAttributeName = "confidence";

			internal const string DaySearchBehaviorRuleName = "DaySearch";

			internal const string DateTimeAndDurationRecognitionRuleName = "DateTimeAndDurationRecognition";

			internal const string MowaGrammarName = "Mowascenarios.grxml";

			internal const string PeopleSearchResultsXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><MobileReco ResultType=\"{4}\"><{0}>{1}</{0}><{2}>{3}</{2}></MobileReco>";
		}
	}
}
