using System;
using System.Globalization;
using System.Net.Mime;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class CommonConstants
	{
		internal static bool UseDataCenterLogging
		{
			get
			{
				return CommonConstants.useDataCenterLogging;
			}
		}

		internal static bool UseDataCenterCallRouting
		{
			get
			{
				return CommonConstants.useDataCenterRouting;
			}
		}

		internal static bool DataCenterADPresent
		{
			get
			{
				return CommonConstants.dataCenterADPresent;
			}
		}

		internal static int? MaxCallsAllowed
		{
			get
			{
				return CommonConstants.maxCallsAllowed.Value;
			}
		}

		internal static string ApplicationVersion
		{
			get
			{
				return CommonConstants.applicationVersion;
			}
		}

		internal const string UMRegKeyPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole";

		internal const string TestModeRegKeyName = "TestMode";

		internal const string DummyDTMFusedForFindMe = "D";

		internal const string UMServiceShortName = "MSExchangeUM";

		internal const string UMCallRouterShortName = "MSExchangeUMCR";

		internal const string ExchangeDirectoryRegKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";

		internal const string ExchangeDirectoryRegValue = "MSIInstallPath";

		internal const string ADTopologyServiceShortName = "MSExchangeADTopology";

		internal const string CNGKeyIsolationServiceShortName = "KeyIso";

		internal const int CbPasswordSalt = 8;

		internal const int CbPasswordHash = 16;

		internal const string SHA256Name = "SHA256";

		internal const string SHA512Name = "SHA512";

		internal const string SHA256NamePreSP1Beta = "SHA256CryptoServiceProvider";

		internal const string PBKDF1Algorithm = "SHA256";

		internal const int PBKDF1Iterations = 1000;

		internal const string DefaultWorkerProcessName = "UMworkerprocess.exe";

		internal const int WorkerProcessStartupTime = 30;

		internal const string PlusSign = "+";

		internal const string TLStransportHeader = ";transport=TLS";

		internal const int MaxAttemptsToRestartUMServiceEndpoint = 5;

		internal const string TCPtransportHeader = ";transport=TCP";

		internal const string JobObjectName = "Microsoft Exchange UM Job";

		internal const string SmtpAddressPrefix = "SMTP:";

		internal const string SIPAddressPrefix = "SIP:";

		internal const string UserPhoneParam = "user=phone";

		internal const string ReferredByHeader = "Referred-By";

		internal const string SupportedHeader = "Supported";

		internal const string MsFeHeader = "ms-fe";

		internal const int MsOrganizationMaxEntries = 3;

		internal const string MSUMCallOnBehalfOf = "X-MSUM-Call-On-Behalf-Of";

		internal const string MSUMOriginatingSessionCallId = "X-MSUM-Originating-Session-Call-Id";

		internal const string ToHeader = "To";

		internal const string ContactHeader = "Contact";

		internal const string StopSemaphoreName = "Global\\ExchangeUMStopKey-";

		internal const string ResetSemaphoreName = "Global\\ExchangeUMResetKey-";

		internal const string FatalSemaphoreName = "Global\\ExchangeUMFatalKey-";

		internal const string ReadySemaphoreName = "Global\\ExchangeUMReadyKey-";

		internal const string PortArg = "-port:";

		internal const string StopKeyArg = "-stopkey:";

		internal const string ResetKeyArg = "-resetkey:";

		internal const string FatalKeyArg = "-fatalkey:";

		internal const string ReadyKeyArg = "-readykey:";

		internal const string TempDirArg = "-tempdir:";

		internal const string SipPortArg = "-sipport:";

		internal const string PerfArg = "-perfenabled:";

		internal const string StartupModeArg = "-startupMode:";

		internal const string CertThumbprintArg = "-thumbprint:";

		internal const string PassiveArg = "-passive";

		internal const string HeartBeatResponse = "HR";

		internal const int BytesInMB = 1048576;

		internal const double BytesInKB = 1024.0;

		internal const long BytesNeededForUMEnablement = 102400L;

		internal const int DefaultRetireTimeout = 1800;

		internal const int XSOResponseTimeoutMiliseconds = 3000;

		internal const int CAMailboxOperationThresholdMilliseconds = 4000;

		internal const int CAMailboxOperationTimeoutMilliseconds = 6000;

		internal const int DefaultSipTCPPortNumber = 5060;

		internal const int DefaultSipTLSPortNumber = 5061;

		internal const int DefaultDataCenterMailboxServerSipTLSPortNumber = 5063;

		internal const int ConnectionBufferSize = 10;

		internal const int MaxSemaphoreCreationAttempts = 10;

		internal const int StreamBufferSize = 4096;

		internal const string UmrecCfgSchemaFile = "umrecyclerconfig.xsd";

		internal const int PortRangeStart = 16000;

		internal const int PortRangeEnd = 17000;

		internal const string UMComponentGuid = "321b4079-df13-45c3-bbc9-2c610013dff4";

		internal const int ReadChunkSize = 32768;

		internal const int CbPaddedChecksum = 160;

		internal const char SmtpDelimiter = '@';

		internal const char DomainDelimiter = '.';

		internal const string AccessNumberSeparator = ", ";

		internal const int MaxAttempts = 100;

		internal const string DirectoryLookupEnabledAttributeName = "msExchHideFromAddressLists";

		internal const string DialPlanContainerName = "CN=UM DialPlan Container";

		internal const string AutoAttendantContainerName = "CN=UM AutoAttendant Container";

		internal const string UMIPGatewayContainerName = "CN=UM IPGateway Container";

		internal const uint MaxUMMailSize = 2097152U;

		internal const int WmaBitsperSec = 13312;

		internal const int WmaHeaderBytes = 8192;

		internal const int MaxAudioDataMegabytes = 5;

		internal const int PromptFileNameMaximumLength = 128;

		internal const string PromptFileExtension = ".wav";

		internal const string TifFileExtension = ".tif";

		internal const string TmpFileExtension = ".tmp";

		internal const string LogFileExtension = ".log";

		internal const string WmaContentType = "audio/wma";

		internal const string GsmContentType = "audio/gsm";

		internal const string Mp3ContentType = "audio/mp3";

		internal const string WavContentType = "audio/wav";

		internal const string TiffContentType = "image/tiff";

		internal const string XmlContentType = "text/xml";

		internal const bool PrecompileGrammars = true;

		internal const string GrammarFileExtension = ".grxml";

		internal const string CompiledGrammarFileExtension = ".cfg";

		internal const string CommonGrammarFileName = "common.grxml";

		internal const string PeopleSearchGrammarTemplateFileName = "peoplesearchtemplate.grxml";

		internal const string PromptResourceBaseName = "Microsoft.Exchange.UM.Prompts.Prompts.Strings";

		internal const string PromptResourceAssemblyName = "Microsoft.Exchange.UM.Prompts";

		internal const string GrammarResourceBaseName = "Microsoft.Exchange.UM.Grammars.Grammars.Strings";

		internal const string GrammarResourceAssemblyName = "Microsoft.Exchange.UM.Grammars";

		internal const string LocConfigResourceBaseName = "Microsoft.Exchange.UM.UMCommon.LocConfig.Strings";

		internal const string LocConfigResourceAssemblyName = "Microsoft.Exchange.UM.UMCommon";

		internal const string LanguagePromptNameFormat = "Language-{0}";

		internal const int UMMailboxPolicyNameMaxLength = 64;

		internal const string OWAInstanceName = "OWA";

		internal const string OutlookInstanceName = "Outlook";

		internal const string LocalHost = "localhost";

		internal const string PersonalContactEwsType = "Contact";

		internal const string ADUserEwsType = "Mailbox";

		internal const string ActiveWPFileName = "wp.active";

		internal const string DataCenterAutoAttendantPerfCounterName = "DataCenterAutoAttendantPerfCounterName";

		internal const string BadVoiceMailPath = "UnifiedMessaging\\badvoicemail";

		internal const string ExchangeBinFolder = "bin";

		internal const string VoiceMailPath = "UnifiedMessaging\\voicemail";

		internal const string LogFilePath = "UnifiedMessaging\\log";

		internal const string TempPath = "UnifiedMessaging\\temp";

		internal const string UMTempFilePath = "UnifiedMessaging\\temp\\UMTempFiles";

		internal const string ServiceRegistryKey = "MSExchange Unified Messaging";

		internal const string ServiceRegistryKeyPath = "System\\CurrentControlSet\\Services\\MSExchange Unified Messaging";

		internal const string ParameterRegistryKeyPath = "System\\CurrentControlSet\\Services\\MSExchange Unified Messaging\\Parameters";

		internal const string EnableBadVoiceMailFolder = "EnableBadVoiceMailFolder";

		internal const int TestDtmfPort = 7001;

		internal const string MSSPlatformAssembly = "Microsoft.Exchange.UM.MSSPlatform.dll";

		internal const string RTCPlatformAssembly = "Microsoft.Exchange.UM.RTCPlatform.dll";

		internal const string IntelPlatformAssembly = "Microsoft.Exchange.UM.IntelPlatform.dll";

		internal const string UcmaPlatformAssembly = "Microsoft.Exchange.UM.UcmaPlatform.dll";

		internal const string TestPlatformAssembly = "Microsoft.Exchange.UM.TestPlatform.dll";

		internal const string MSSPlatformClassName = "Microsoft.Exchange.UM.MSSPlatform.MSSPlatform";

		internal const string UcmaPlatformClassName = "Microsoft.Exchange.UM.UcmaPlatform.UcmaPlatform";

		internal const float MinimumConfidence = 0.25f;

		public const string FieldSeparator = "​";

		public const char FakeColumnSeparator = '‚';

		internal static readonly System.Net.Mime.ContentType ContentTypeTextPlain = new System.Net.Mime.ContentType("text/plain");

		internal static readonly char[] MsOrganizationDomainSeparators = new char[]
		{
			',',
			';',
			' '
		};

		internal static readonly CultureInfo DefaultCulture = UMLanguage.DefaultLanguage.Culture;

		private static readonly string applicationVersion = typeof(CommonConstants).GetApplicationVersion();

		private static readonly bool useDataCenterLogging = Utils.GetDatacenterLoggingEnabled();

		private static readonly bool useDataCenterRouting = Utils.GetDatacenterRoutingEnabled();

		private static readonly bool dataCenterADPresent = Utils.GetDatacenterADPresent();

		private static readonly Lazy<int?> maxCallsAllowed = new Lazy<int?>(new Func<int?>(Utils.GetMaxCallsAllowed));

		[Flags]
		internal enum UMOutlookUIFlags
		{
			None = 0,
			VoicemailForm = 1,
			VoicemailOptions = 2
		}

		internal enum ApplicationState
		{
			Idle,
			Playing,
			Recording,
			DtmfWait,
			SpeechWait
		}

		internal enum TaskCallType
		{
			Voice,
			Fax,
			OutCall
		}

		internal abstract class Transcription
		{
			internal abstract class Xml
			{
				internal const string Language = "lang";

				internal const string Confidence = "confidence";

				internal const string ConfidenceBand = "confidenceBand";

				internal const string RecognitionResult = "recognitionResult";

				internal const string RecognitionError = "recognitionError";

				internal const string SchemaVersion = "schemaVersion";

				internal const string ProductID = "productID";

				internal const string ProductVersion = "productVersion";

				internal const string Break = "Break";

				internal const string Weight = "wt";

				internal const string High = "high";

				internal const string Medium = "medium";

				internal const string Low = "low";

				internal const string Feature = "Feature";

				internal const string FeatureClass = "class";

				internal const string Reference = "reference";

				internal const string Reference2 = "reference2";

				internal const string NextNodeId = "nx";

				internal const string Id = "id";

				internal const string ConfidenceTag = "c";

				internal const string StartTimeOffset = "ts";

				internal const string EndTimeOffset = "te";

				internal const string BE = "be";

				internal const string EvmNamespace = "http://schemas.microsoft.com/exchange/um/2010/evm";

				internal const string XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

				internal const string XsiNamespaceTag = "xmlns:xsi";

				internal const string AsrTag = "ASR";

				internal const string TextTag = "Text";

				internal const string PhoneNumber = "PhoneNumber";

				internal const string InformationTag = "Information";

				internal const string ErrorInformationTag = "ErrorInformation";

				internal const string LinkText = "linkText";

				internal const string LinkURL = "linkURL";

				internal const string LinkURLValue = "http://go.microsoft.com/fwlink/?LinkId=150048";
			}
		}

		internal abstract class UserConfig
		{
			internal const string Greetings = "Um.CustomGreetings";

			internal const string Password = "Um.Password";

			internal const string General = "Um.General";

			internal const string OWA = "OWA.UserOptions";

			internal const string PersonalAutoAttendant = "UM.E14.PersonalAutoAttendants";

			internal const string Outlook = "UMOLK.UserOptions";

			internal const string External = "External";

			internal const string Oof = "Oof";

			internal const string BlockedNumbers = "BlockedNumbers";

			internal const string Name = "RecordedName";

			internal const string Current = "Password";

			internal const string TimeSet = "PasswordSetTime";

			internal const string PreviousPasswords = "PreviousPasswords";

			internal const string LockoutCount = "LockoutCount";

			internal const string FirstTime = "FirstTimeUser";

			internal const string OofStatus = "OofStatus";

			internal const string PlayOnPhoneDialString = "PlayOnPhoneDialString";

			internal const string TelephoneAccessFolderEmail = "TelephoneAccessFolderEmail";

			internal const string UseAsr = "UseAsr";

			internal const string ReceivedVoiceMailPreviewEnabled = "ReceivedVoiceMailPreviewEnabled";

			internal const string SentVoiceMailPreviewEnabled = "SentVoiceMailPreviewEnabled";

			internal const string ReadUnreadVoicemailInFIFOOrder = "ReadUnreadVoicemailInFIFOOrder";

			internal const string PromptCount = "PromptCount_";

			internal const string TimeZone = "timezone";

			internal const string TimeFormat = "timeformat";

			internal const string OutlookFlags = "outlookFlags";

			internal const int MaxPasswordDays = 36500;

			internal const int MaxPasswordLength = 24;

			internal const int MinPasswordLength = 4;

			internal const string ChecksumAttributeName = "msExchTUIPassword";

			internal const int CbMaxAdGreeting = 32768;

			internal const string VoiceNotificationStatus = "VoiceNotificationStatus";
		}

		internal abstract class SipInfo
		{
			internal const string UserAgent = "Unified Messaging WebService";

			internal const string MessageIdHeaderName = "UMWS-MESSAGE-ID";

			internal const string MessageTypeHeaderName = "UMWS-MESSAGE-TYPE";

			internal const int PingTimeout = 30;

			internal static readonly System.Net.Mime.ContentType XmlContentType = new System.Net.Mime.ContentType("text/xml");
		}

		internal abstract class DiagnosticTool
		{
			internal const string UserAgent = "MSExchangeUM-Diagnostics";

			internal const string UserAgentHeaderName = "user-agent";

			internal const string DiagnosticHeaderName = "msexum-connectivitytest";

			internal const string LocalDiagnosticHeaderValue = "local";

			internal const string RemoteDiagnosticHeaderValue = "remote";

			internal const string DiagnosticInfoMsg = "UM Operation Check";

			internal const string DiagnosticInfoHeader = "UMTUCFirstResponse";

			internal const string DiagnosticDialPlanName = "UMDialPlan";

			internal const string DiagnosticInfoResp = "OK";

			internal const int DiagnosticToolInterDigitSendGapforTUITest = 500;

			internal const int DiagnosticToolInitialResponseGap = 1000;

			internal const int DiagnosticToolInterDigitSendGap = 100;

			internal const int DiagnosticToolToneDuration = 100;

			internal const int DiagnosticToolFaxToneDuration = 4000;

			internal const string DiagnosticSequence = "ABCD*#0123456789";

			internal const int DiagnosticSipPort = 9000;

			internal const int DiagnosticINFOTimeout = 20;

			internal const int DiagnosticMediaEstablishmentTimeout = 60;

			internal const int DiagnosticToolSrvResponseWaitTimeout = 30;

			internal const int DiagnosticINFOStateChangeTimeout = 60;

			internal const int DiagnosticDTMFTimeout = 270;

			internal const int UMPingRetryLimit = 2;

			internal const int OptionsRespCode = 200;

			internal const int OptionsServerUnavailableRespCode = 503;

			internal const int OptionsForbidden = 403;

			internal const string OptionsServerUnavailableRespText = "Service Unavailable";

			internal const int OptionsServerInternalErrorRespCode = 500;

			internal const string OptionsServerInternalErrorRespText = "Server Internal Error";

			internal const string ContentType = "application/sdp";

			internal const string GatewayNotFoundResponseText = "Gateway not found";

			internal const int GatewayNotFoundResponseCode = 404;

			internal const int MovedTemporarilyResponseCode = 302;

			internal const int ProxyRedirectResponseCode = 303;

			internal const int DeclinedResponseCode = 603;

			internal static readonly TimeSpan UMPingResponseTimeout = TimeSpan.FromSeconds(4.0);

			internal static readonly TimeSpan UMPingRetryWaitTime = TimeSpan.FromSeconds(60.0);
		}

		internal abstract class XsoUtil
		{
			internal const string ItemClassSeparator = ".";

			internal const char VoiceMessageAttachmentOrderSeparator = ';';
		}

		internal abstract class MessageContentBuilder
		{
			internal const string UTF8CharSet = "utf-8";
		}

		internal abstract class PromptProvisioning
		{
			internal const string ManifestFileName = "ExchangeUM.xml";

			internal const string PublishingPointShareName = "ExchangeUM";

			internal const string LocalPublishingPointPath = "UnifiedMessaging\\Prompts\\Custom";

			internal const string LocalPickupPath = "UnifiedMessaging\\Prompts\\Pickup";

			internal const string LocalPromptCachePath = "UnifiedMessaging\\Prompts\\Cache";

			internal static readonly TimeSpan PublishingLockRetryInterval = TimeSpan.FromMilliseconds(500.0);

			internal static readonly TimeSpan PublishingLockRetryTimeout = TimeSpan.FromMilliseconds(5000.0);

			internal static readonly TimeSpan PromptCacheLockRetryTimeout = TimeSpan.FromMilliseconds(5000.0);

			internal static readonly TimeSpan KeepOrphanFilesInterval = TimeSpan.FromDays(1.0);

			internal static readonly WaveFormat SupportedInputFormat = WaveFormat.Pcm8WaveFormat;
		}

		internal abstract class SipHeaders
		{
			internal const string DiversionHeader = "Diversion";

			internal const string DiversionHeaderVariant = "CC-Diversion";

			internal const string HistoryInfoHeader = "History-Info";

			internal const string TransportModeTcp = "TCP";

			internal const string TransportModeTls = "TLS";

			internal const string SIPAddressPrefix = "SIP:";

			internal const string TELAddressPrefix = "TEL:";

			internal const string EnclosedSipFormat = "<{0}>";

			internal const string SipUriFormat = "sip:{0}";

			internal const string SipUriFormatWithPort = "sip:{0}:{1}";

			internal const string SipUriFormatWithUserInfo = "sip:{0}@{1}";

			internal const string OpaqueParameter = "opaque";

			internal const string GruuParameter = "gruu";

			internal const string AppExumPrefix = "app:exum:";

			internal const string TCPtransportHeader = ";transport=TCP";

			internal const string TLStransportHeader = ";transport=TLS";

			internal const string UserPhoneParam = "user=phone";

			internal const string ReferredByHeader = "Referred-By";

			internal const string ToHeader = "To";

			internal const string ContactHeader = "Contact";

			internal const string EventHeader = "Event";

			internal const string MSApplicationAorHeader = "ms-application-aor";

			internal const string MsOrganization = "ms-organization";

			internal const string MsOrganizationGuid = "ms-organization-guid";

			internal const string UserDefault = "user-default";

			internal const string MAddrParameter = "maddr";
		}

		internal abstract class UMVersions
		{
			internal static readonly QueryFilter CompatibleServersFilter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, Server.E15MinVersion),
				new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ServerSchema.VersionNumber, Server.E16MinVersion)
			});

			internal static readonly QueryFilter E12LegacyServerFilter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, Server.E2007MinVersion),
				new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, Server.E14MinVersion)
			});

			internal static readonly QueryFilter E14LegacyServerFilter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, Server.E14MinVersion),
				new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, Server.E15MinVersion)
			});
		}

		internal abstract class GalGrammar
		{
			internal const string LocalGrammarCachePath = "UnifiedMessaging\\grammars";

			internal const int MaxEquivalentForms = 1;

			internal const string FileName = "gal";

			internal const string FileExtension = ".grxml";

			internal const string DistributionListGrammarFileName = "distributionList";

			internal const string FileNameComponentSeparator = "_";

			internal const string GeneratedGrammarsRootName = "Names";

			internal const string RuleElementName = "rule";

			internal const string BucketFilePrefix = "\r\n<grammar root=\"Names\" xml:lang=\"{0}\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/06/grammar\" xmlns:sapi=\"http://schemas.microsoft.com/Speech/2002/06/SRGSExtensions\" tag-format=\"semantics-ms/1.0\">\r\n\t<!-- BucketLevel File Grammar -->\r\n";

			internal const string GrammarPrefix = "\r\n<grammar root=\"Names\" xml:lang=\"{0}\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/06/grammar\" xmlns:sapi=\"http://schemas.microsoft.com/Speech/2002/06/SRGSExtensions\" tag-format=\"semantics-ms/1.0\">";

			internal const string PersonalContactsRulePrefix = "\r\n\t<rule id=\"Names\" scope=\"public\">\r\n\t\t<tag>$.RecoEvent={{}};</tag>\r\n\t\t<tag>$.ResultType={{}};</tag>\r\n\t\t<tag>$.UmSubscriberObjectGuid={{}};</tag>\r\n\t\t<tag>$.UmSubscriberObjectGuid._value=\"{0}\";</tag>\r\n\t\t<tag>$.ContactId={{}};</tag>\r\n\t\t<tag>$.ContactName={{}};</tag>\r\n\t\t<tag>$.DisambiguationField={{}};</tag>\r\n";

			internal const string MowaPersonalContactsRulePrefix = "\r\n\t<rule id=\"Names\" scope=\"public\">\r\n\t\t<tag>$.RecoEvent={{}};</tag>\r\n\t\t<tag>$.ResultType={{}};</tag>\r\n\t\t<tag>$.UmSubscriberObjectGuid={{}};</tag>\r\n\t\t<tag>$.UmSubscriberObjectGuid._value=\"{0}\";</tag>\r\n\t\t<tag>$.ContactId={{}};</tag>\r\n\t\t<tag>$.ContactName={{}};</tag>\r\n\t\t<tag>$.DisambiguationField={{}};</tag>\r\n\t\t<tag>$.PersonId={{}};</tag>\r\n\t\t<tag>$.GALLinkID={{}};</tag>\r\n";

			internal const string DirectoryContactRulePrefix = "\r\n\t<rule id=\"Names\" scope=\"public\">\r\n\t\t<tag>$.RecoEvent={};</tag>\r\n\t\t<tag>$.ResultType={};</tag>\r\n\t\t<tag>$.RecoEvent._value=\"recoNameOrDepartment\";</tag>\r\n\t\t<tag>$.ResultType._value=\"DirectoryContact\";</tag>\r\n\t\t<tag>$.ObjectGuid={};</tag>\r\n\t\t<tag>$.SMTP={};</tag>\r\n\t\t<tag>$.ContactName={};</tag>\r\n";

			internal const string OneOfPrefix = "\t\t<one-of>\r\n";

			internal const string OneOfSuffix = "\t\t</one-of>\r\n";

			internal const string ItemNode = "<item>{0}</item>";

			internal const string TopLevelFileOneOfSuffix = "\t\t</one-of>\r\n        <tag>$=$$;</tag>\r\n\t</rule>";

			internal const string RuleRefForBucketFile = "          <item>\r\n\t\t\t\t<ruleref uri=\"{0}#Names\"/>\r\n\t\t\t</item>\r\n";

			internal const string PersonalContactsRuleSuffix = "\r\n\t\t<!-- the following will add an option politeending to the recognition -->\r\n\t\t<item repeat=\"0-1\">\r\n\t\t\t<ruleref uri=\"{0}#politeEndPhrases\"/>\r\n\t\t</item>\r\n\t</rule>";

			internal const string DirectoryContactRuleSuffix = "\r\n\t\t<!-- the following will add an option politeending to the recognition -->\r\n\t\t<item repeat=\"0-1\">\r\n\t\t\t<ruleref uri=\"{0}#politeEndPhrases\"/>\r\n\t\t</item>\r\n\t</rule>";

			internal const string GrammarSuffix = "\r\n</grammar>";

			internal const string EmptyNamesRule = "\r\n\t<rule id=\"Names\" scope=\"public\">\r\n\t\t<ruleref special=\"VOID\" />\r\n\t</rule>";

			internal const string DirectoryContactNode = "\t\t\t<item>{0}\r\n\t\t\t\t<tag>\r\n\t\t\t\t\t$.ObjectGuid._value=\"{2}\";\r\n\t\t\t\t\t$.SMTP._value=\"{1}\";\r\n\t\t\t\t\t$.ContactName._value=\"{0}\";\r\n\t\t\t\t</tag>\r\n\t\t\t</item>\r\n";

			internal const string PersonalContactNode = "\t\t\t<item>{0}\r\n\t\t\t\t<tag>\r\n\t\t\t\t\t$.RecoEvent._value=\"recoNameOrDepartment\";\r\n\t\t\t\t\t$.ResultType._value=\"PersonalContact\";\r\n\t\t\t\t\t$.ContactId._value=\"{1}\";\r\n\t\t\t\t\t$.ContactName._value=\"{0}\";\r\n\t\t\t\t\t$.DisambiguationField._value=\"{2}\";\r\n\t\t\t\t</tag>\r\n\t\t\t</item>\r\n";

			internal const string MowaPersonalContactNode = "\t\t\t<item>{0}\r\n\t\t\t\t<tag>\r\n\t\t\t\t\t$.RecoEvent._value=\"recoNameOrDepartment\";\r\n\t\t\t\t\t$.ResultType._value=\"PersonalContact\";\r\n\t\t\t\t\t$.ContactId._value=\"{1}\";\r\n\t\t\t\t\t$.ContactName._value=\"{5}\";\r\n\t\t\t\t\t$.DisambiguationField._value=\"{2}\";\r\n\t\t\t\t\t$.PersonId._value=\"{3}\";\r\n\t\t\t\t\t$.GALLinkID._value=\"{4}\";\r\n\t\t\t\t</tag>\r\n\t\t\t</item>\r\n";

			internal const string PromptForAliasPrefix = "<grammar xml:lang=\"{0}\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/06/grammar\" tag-format=\"semantics-ms/1.0\">\r\n\r\n\t<rule id=\"Names\" scope=\"public\">\r\n\t\t<tag>$.RecoEvent={{}};</tag>\r\n\t\t<tag>$.ResultType={{}};</tag>\r\n\t\t<tag>$.ObjectGuid={{}};</tag>\r\n\t\t<tag>$.SMTP={{}};</tag>\r\n\t\t<tag>$.ContactName={{}};</tag>\r\n\t\t<one-of>\r\n";

			internal const string PromptForAliasUserNode = "\t\t\t<item>{0}\r\n\t\t\t\t<tag>\r\n\t\t\t\t\t$.RecoEvent._value=\"recoNameOrDepartment\";\r\n\t\t\t\t\t$.ResultType._value=\"DirectoryContact\";\r\n\t\t\t\t\t$.ObjectGuid._value=\"{3}\";\r\n\t\t\t\t\t$.SMTP._value=\"{1}\";\r\n\t\t\t\t\t$.ContactName._value=\"{2}\";\r\n\t\t\t\t</tag>\r\n\t\t\t</item>\r\n";

			internal const string PromptForAliasSuffix = "\r\n\t\t</one-of>\r\n\t\t<!-- the following will add an option politeending to the recognition -->\r\n\t\t<item repeat=\"0-1\">\r\n\t\t\t<ruleref uri=\"{0}#politeEndPhrases\"/>\r\n\t\t</item>\r\n\t</rule>\r\n</grammar>";

			internal const string CustomizedMenuGrammarHeader = "<grammar root=\"{0}\"\txml:lang=\"{2}\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/06/grammar\" tag-format=\"semantics-ms/1.0\">\r\n\t<!-- NoGrammar rule recognizes phrases like 'No Sales', where No is optional -->\r\n\t<rule id=\"No{0}\" scope=\"public\">\r\n\t\t<tag>$.RecoEvent={{}};</tag>\r\n   \t\t <item repeat=\"0-1\">\r\n\t\t\t\t<ruleref uri=\"{1}#noPhrases\"/>\r\n\t\t </item>\r\n\t\t <ruleref uri=\"#{0}\"/>\r\n\t\t<tag>$=$$;</tag>\r\n\t</rule>\r\n\r\n\t<rule id=\"{0}\" scope=\"public\">\r\n\t\t<tag>$.RecoEvent={{}};</tag>\r\n\t\t<tag>$.Extension={{}};</tag>\r\n\t\t<tag>$.ResultType={{}};</tag>\r\n\t\t<tag>$.DepartmentName={{}};</tag>\r\n\t\t<tag>$.MappedKey={{}};</tag>\r\n\t\t<tag>$.CustomMenuTarget={{}};</tag>\r\n\t\t<tag>$.PromptFileName={{}};</tag>\r\n\t\t<one-of>";

			internal const string CustomizedMenuGrammarElementTemplate = "\t\t\t <item>{0}\r\n\t\t\t\t <tag>\r\n\t\t\t\t\t $.RecoEvent._value=\"recoNameOrDepartment\";\r\n\t\t\t\t\t $.Extension._value=\"{1}\";\r\n\t\t\t\t\t $.ResultType._value=\"Department\";\r\n\t\t\t\t\t $.DepartmentName._value=\"{0}\";\r\n\t\t\t\t\t $.MappedKey._value=\"{4}\";\r\n\t\t\t\t\t $.CustomMenuTarget._value = \"{2}\"\r\n\t\t\t\t\t $.PromptFileName._value = \"{3}\"\r\n\t\t\t\t </tag>\r\n\t\t\t </item>";

			internal const string CustomizedMenuGrammarTrailer = "\t\t</one-of>\r\n\t\t<!-- the following will add an option politeending to the recognition -->\r\n\t   \t<item repeat=\"0-1\">\r\n\t\t\t<ruleref uri=\"{0}#politeEndPhrases\"/>\r\n\t\t</item>\r\n\t</rule>\r\n</grammar>";

			internal const string ManifestFileGrammarNode = "<resource src=\"{0}\"/>";

			internal const string ManifestFileGrammarNodeName = "resource";

			internal const string ManifestFileGrammarNodeSrcAttributeName = "src";

			internal const string ManifestFileRegKey = "SOFTWARE\\Microsoft\\Microsoft Speech Server\\2.0\\Applications\\ExUM";

			internal const string ManifestFileRegValue = "PreloadedResourceManifest";

			internal const string ManifestFileName = "manifest.xml";

			internal const string File = "file:///";

			internal const string ManifestFileXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><manifest version=\"2.0\">{0}</manifest>";

			internal const int ManifestFileReloadServiceCommand = 131;

			internal const int SesRecycleServiceCommand = 133;

			internal const int ReloadRegSettingsServiceCommand = 132;

			internal const string AssemblyBasedRegValue = "AssemblyBased";

			internal const string DnisRegValue = "DNIS";

			internal const string EnabledRegValue = "Enabled";

			internal const string PrecedenceRegValue = "Precedence";

			internal const string UriRegValue = "URI";

			internal const string TypeRegValue = "Type";

			internal const string NotifMsgQueueRegValue = "NotificationMessageQueue";

			internal const string UseSRTPRegValue = "UseSecureRTP";

			internal const string ManifestFileContentsRegValue = "PreloadedResourceManifestXml";

			internal const string DefaultUri = "http://localhost";
		}

		internal abstract class ServiceNames
		{
			public const string SearchIndexerServiceShortName = "MSExchangeSearch";
		}

		internal abstract class UMReporting
		{
			public const string AggregatedDataXmlNamespace = "http://schemas.microsoft.com/v1.0/UMReportAggregatedData";

			internal abstract class CallType
			{
				public const string None = "None";

				public const string CAVoiceMessage = "CallAnsweringVoiceMessage";

				public const string CAMissedCall = "CallAnsweringMissedCall";

				public const string VirtualNumberCall = "VirtualNumberCall";

				public const string AutoAttendant = "AutoAttendant";

				public const string SubscriberAccess = "SubscriberAccess";

				public const string Fax = "Fax";

				public const string PlayOnPhone = "PlayOnPhone";

				public const string FindMe = "FindMe";

				public const string UnAuthenticatedPilotNumber = "UnAuthenticatedPilotNumber";

				public const string PromptProvisioning = "PromptProvisioning";

				public const string PlayOnPhonePAAGreeting = "PlayOnPhonePAAGreeting";

				public const string Diagnostics = "Diagnostics";
			}

			internal abstract class ReasonForCall
			{
				public const string None = "None";

				public const string Direct = "Direct";

				public const string DivertNoAnswer = "DivertNoAnswer";

				public const string DivertBusy = "DivertBusy";

				public const string DivertForward = "DivertForward";

				public const string Outbound = "Outbound";
			}

			internal abstract class DropCallReason
			{
				public const string None = "None";

				public const string UserError = "UserError";

				public const string SystemError = "SystemError";

				public const string GracefulHangup = "GracefulHangup";

				public const string OutboundFailedCall = "OutboundFailedCall";
			}

			internal abstract class OfferResult
			{
				public const string None = "None";

				public const string Answer = "Answer";

				public const string Reject = "Reject";

				public const string Redirect = "Redirect";
			}
		}
	}
}
