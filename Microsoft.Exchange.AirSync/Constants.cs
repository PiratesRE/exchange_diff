using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal static class Constants
	{
		internal static string ProtocolVersionsHeaderValue
		{
			get
			{
				if (GlobalSettings.EnableV160)
				{
					return Constants.ProtocolExperimantalVersionsHeaderValue;
				}
				return "2.0,2.1,2.5,12.0,12.1,14.0,14.1";
			}
		}

		internal static string ProtocolExperimantalVersionsHeaderValue
		{
			get
			{
				return "2.0,2.1,2.5,12.0,12.1,14.0,14.1,16.0";
			}
		}

		internal const string Exchange12Provider = "Exchange12";

		internal const string WbxmlHeader = "application/vnd.ms-sync.wbxml";

		internal const string MultiPartContentType = "application/vnd.ms-sync.multipart";

		internal const string TextHtmlContentType = "text/html";

		internal const string AcceptMultiPartHeader = "MS-ASAcceptMultiPart";

		internal const string ProtocolVersionsHeader = "MS-ASProtocolVersions";

		internal const string ProtocolCommandsHeader = "MS-ASProtocolCommands";

		internal const string OutlookExtensionsHeader = "X-OLK-Extensions";

		internal const string OutlookExtensionHeader = "X-OLK-Extension";

		internal const string OutlookExtensionsHeaderValue = "1=0E47";

		internal const string MinorVersion = "14.1.127";

		internal const string ProtocolOTAUpdateHeader = "X-MS-OTAUpdate";

		internal const string DirectPushOffHeader = "X-MS-NoPush";

		internal const string ProtocolCommandsHeaderValue = "Sync,SendMail,SmartForward,SmartReply,GetAttachment,GetHierarchy,CreateCollection,DeleteCollection,MoveCollection,FolderSync,FolderCreate,FolderDelete,FolderUpdate,MoveItems,GetItemEstimate,MeetingResponse,Search,Settings,Ping,ItemOperations,Provision,ResolveRecipients,ValidateCert";

		internal const string ProtocolCommandsHeaderConsumerVersion25OnlyValue = "Sync,SendMail,SmartForward,SmartReply,GetAttachment,FolderSync,FolderCreate,FolderDelete,FolderUpdate,MoveItems,GetItemEstimate,MeetingResponse,Ping";

		internal const string ProtocolCommandsHeaderConsumerContactsOnlyValue = "Sync,FolderSync,GetItemEstimate,Ping";

		internal const string ProtocolCommandsHeaderConsumerValue = "Sync,SendMail,SmartForward,SmartReply,GetAttachment,FolderSync,FolderCreate,FolderDelete,FolderUpdate,MoveItems,GetItemEstimate,MeetingResponse,Search,Settings,Ping,ItemOperations";

		internal const string DiagnosticDataHeader = "X-MS-Diagnostics";

		internal const string BackOffTimeHeader = "X-MS-BackOffDuration";

		internal const string BackOffReasonHeader = "X-MS-BackOffReason";

		internal const string DeviceTypeParam = "DeviceType";

		internal const string TestActiveSyncConnectivityDeviceType = "TestActiveSyncConnectivity";

		internal const string TestActiveSyncConnectivityUserAgent = "TestActiveSyncConnectivity";

		internal const string DeviceIdParam = "DeviceId";

		internal const string MailboxIdParam = "MailboxId";

		internal const string ContentType = "Content-Type";

		internal const string Host = "Host";

		internal const string ASProtocolVersion = "MS-ASProtocolVersion";

		internal const string ASErrorHeader = "X-MS-ASError";

		internal const string AutoBlockReasonHeader = "X-MS-ASThrottle";

		internal const string ActivityContextDiagnosticsHeader = "X-ActivityContextDiagnostics";

		internal const string ExceptionDiagnosticsHeader = "X-ExceptionDiagnostics";

		internal const string BEServerExceptionHeaderName = "X-BEServerException";

		internal const string BEServerExceptionHeaderValue = "Microsoft.Exchange.Data.Storage.IllegalCrossServerConnectionException";

		internal const string PrimaryMailboxId = "0";

		internal const string ASSeamlessUpgradeVersions = "MS-ASSeamlessUpgradeVersions";

		internal const string AirSync = "AirSync";

		internal const string MOWA = "MOWA";

		internal const string DOWA = "DOWA";

		internal const string LanguageHeader = "Accept-Language";

		internal const string UserAgentHeader = "User-Agent";

		internal const string RetryAfter = "Retry-After";

		internal const string ResetPartnership = "X-MS-RP";

		internal const string MinorVersionHeader = "X-MS-MV";

		internal const string SaveInSentParam = "SaveInSent";

		internal const string ItemIdParam = "ItemId";

		internal const string ParentIdParam = "ParentId";

		internal const string CollectionIdParam = "CollectionId";

		internal const string CollectionNameParam = "CollectionName";

		internal const string LongIdParam = "LongId";

		internal const string MessageRfc822Content = "message/rfc822";

		internal const string AttachmentNameParam = "AttachmentName";

		internal const string CommandParam = "Cmd";

		internal const string UserParam = "User";

		internal const string Occurrence = "Occurrence";

		internal const string DRMLicenseAttachmentId = "DRMLicense";

		internal const string DRMLicenseContentType = "application/x-microsoft-rpmsg-message-license";

		internal const string UTCDateTimeFormat = "yyyy-MM-dd\\THH:mm:ss.fff\\Z";

		internal const string LocalDateTimeFormat = "yyyyMMdd\\THHmmss\\Z";

		internal const string WapProvisionFormat = "MS-WAP-Provisioning-XML";

		internal const string EasProvisionFormat = "MS-EAS-Provisioning-WBXML";

		internal const string BadItemReportClassType = "IPM.Note.Exchange.ActiveSync.Report";

		internal const string DefaultSmimeAttachmentName = "smime.p7m";

		internal const string MultiPartSigned = "multipart/signed";

		internal const string EncryptedSMIMEMessageType = "IPM.Note.SMIME";

		internal const string MailboxLogClassType = "IPM.Note.Exchange.ActiveSync.MailboxLog";

		internal const string RemoteWipeConfirmationMessageType = "IPM.Note.Exchange.ActiveSync.RemoteWipeConfirmation";

		internal const string MeetingRequestClassType = "IPM.Schedule.Meeting.Request";

		internal const string BootstrapMailClassType = "IPM.Note";

		internal const string SmsClassType = "IPM.NOTE.MOBILE.SMS";

		internal const string MmsClassType = "IPM.NOTE.MOBILE.MMS";

		internal const int ConsumerSmsMaxSubjectLength = 78;

		internal const string ProxyHeader = "X-EAS-Proxy";

		internal const string PodProxyHeader = "X-EAS-DC-Proxy";

		internal const string BasicAuthProxyHeader = "X-EAS-BasicAuth-Proxy";

		internal const string ProxyLoginCommand = "ProxyLogin";

		internal const string IsFromCafeHeader = "X-IsFromCafe";

		internal const string ProxyUri = "msExchProxyUri";

		internal const string VDirSettingsHeader = "X-vDirObjectId";

		internal const string XMSWLHeader = "X-MS-WL";

		internal const int HttpStatusNeedIdentity = 441;

		internal const string OwaSettingsRoot = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA";

		internal const string EasSettingsRoot = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ActiveSync";

		internal const string ProtectedVMItemClassPrefix = "IPM.Note.RPMSG.Microsoft.Voicemail";

		internal const int MaxDeviceTypeLength = 32;

		internal const int MaxDeviceIDLength = 32;

		public const string CreateChatsFolder = "CreateChatsFolder";

		internal const string ProtocolVersionsHeaderConsumerVersion25OnlyValue = "2.5";

		internal const string ProtocolVersionsHeaderConsumerVersion140OnlyValue = "14.0";

		internal const string ProtocolVersionsHeaderConsumerValue = "2.5,14.0";

		internal const int ProtocolVersion160 = 160;

		internal const int ProtocolVersion161 = 161;

		internal static readonly Dictionary<OutlookExtension, bool> FeatureAccessMap = new Dictionary<OutlookExtension, bool>(Enum.GetValues(typeof(OutlookExtension)).Length)
		{
			{
				OutlookExtension.FolderTypes,
				true
			},
			{
				OutlookExtension.SystemCategories,
				true
			},
			{
				OutlookExtension.DefaultFromAddress,
				true
			},
			{
				OutlookExtension.Archive,
				false
			},
			{
				OutlookExtension.Unsubscribe,
				false
			},
			{
				OutlookExtension.MessageUpload,
				false
			},
			{
				OutlookExtension.AdvancedSearch,
				true
			},
			{
				OutlookExtension.Safety,
				false
			},
			{
				OutlookExtension.TrueMessageRead,
				false
			},
			{
				OutlookExtension.Rules,
				true
			},
			{
				OutlookExtension.ExtendedDateFilters,
				true
			},
			{
				OutlookExtension.Sms,
				true
			},
			{
				OutlookExtension.ActionableSearch,
				false
			},
			{
				OutlookExtension.FolderPermissions,
				false
			},
			{
				OutlookExtension.FolderExtensionType,
				false
			}
		};

		internal static readonly string[] Version10DeviceUserAgentPrefixes = new string[]
		{
			"Microsoft-AirSync/1.0",
			"Microsoft-PocketPC/4.",
			"Microsoft-SmartPhone/4."
		};

		internal static readonly string[] EvmSupportedItemClassPrefixes = new string[]
		{
			"IPM.Note.Microsoft.Voicemail",
			"IPM.Note.RPMSG.Microsoft.Voicemail",
			"IPM.Note.Microsoft.Missed.Voice"
		};

		internal static readonly Guid PsetidAppointment = new Guid("{00062002-0000-0000-c000-000000000046}");

		internal static readonly Exception FaultInjectionFormatException = new FormatException("Fault injection created exception: 3d911d3b-a4ad-4337-89a8-6e6218a1894e");

		internal static readonly ExPerformanceCounter[] ExceptionPerfCounters = new ExPerformanceCounter[]
		{
			AirSyncCounters.RatePerMinuteOfTransientMailboxConnectionFailures,
			AirSyncCounters.RatePerMinuteOfMailboxOfflineErrors,
			AirSyncCounters.RatePerMinuteOfTransientStorageErrors,
			AirSyncCounters.RatePerMinuteOfPermanentStorageErrors,
			AirSyncCounters.RatePerMinuteOfTransientActiveDirectoryErrors,
			AirSyncCounters.RatePerMinuteOfPermanentActiveDirectoryErrors,
			AirSyncCounters.RatePerMinuteOfTransientErrors
		};

		internal static readonly ExPerformanceCounter[] LatencyPerfCounters = new ExPerformanceCounter[]
		{
			AirSyncCounters.AverageRpcLatency,
			AirSyncCounters.AverageLdapLatency,
			AirSyncCounters.AverageRequestTime,
			AirSyncCounters.AverageHangingTime
		};

		internal static string ServerSideDeletes = "ServerSideDeletes";

		internal static string SyncMms = "SyncMMS";

		internal enum ExceptionPerfCountersType
		{
			ConnectionFailedTransientExceptionRate,
			MailboxOfflineExceptionRate,
			StorageTransientExceptionRate,
			StoragePermanentExceptionRate,
			AdTransientExceptionRate,
			AdPermanentExceptionRate,
			TransientErrorRate,
			MaxExceptionPerfCounters
		}

		internal enum LatencyPerfCountersType
		{
			AverageRpcLatency,
			AverageLdapLatency,
			AverageRequestLatency,
			AverageHangingLatency,
			MaxLatencyPerfCounters
		}
	}
}
