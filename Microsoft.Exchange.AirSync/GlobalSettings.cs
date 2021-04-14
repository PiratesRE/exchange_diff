using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.AirSync
{
	internal class GlobalSettings
	{
		public static GlobalSettings.OnLoadConfigSettingDelegate OnLoadConfigSetting { get; set; }

		static GlobalSettings()
		{
			CertificateValidationManager.RegisterCallback("AirSync", new RemoteCertificateValidationCallback(ProxyHandler.SslCertificateValidationCallback));
		}

		public static void Clear()
		{
			GlobalSettings.settings.Clear();
		}

		public static bool Clear(GlobalSettingsPropertyDefinition propDef)
		{
			object obj;
			return GlobalSettings.settings.TryRemove(propDef, out obj);
		}

		public static SyncLog SyncLog
		{
			get
			{
				if (GlobalSettings.syncLog == null)
				{
					lock (GlobalSettings.syncLogCreationLock)
					{
						if (GlobalSettings.syncLog == null)
						{
							SyncLogConfiguration syncLogConfiguration = new SyncLogConfiguration();
							syncLogConfiguration.Enabled = GlobalSettings.SyncLogEnabled;
							syncLogConfiguration.LogFilePrefix = "AirSync";
							syncLogConfiguration.LogComponent = "AirSync";
							string text = GlobalSettings.SyncLogDirectory;
							if (string.IsNullOrEmpty(text))
							{
								text = Path.Combine(Path.GetTempPath(), "CasSyncLogs");
							}
							syncLogConfiguration.LogFilePath = text;
							syncLogConfiguration.SyncLoggingLevel = SyncLoggingLevel.Information;
							GlobalSettings.syncLog = new SyncLog(syncLogConfiguration);
						}
					}
				}
				return GlobalSettings.syncLog;
			}
		}

		internal static bool UseTestBudget
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.UseTestBudget);
			}
		}

		internal static bool WriteProtocolLogDiagnostics
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.WriteProtocolLogDiagnostics);
			}
		}

		internal static int HangingSyncHintCacheSize
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.HangingSyncHintCacheSize);
			}
		}

		internal static TimeSpan HangingSyncHintCacheTimeout
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.HangingSyncHintCacheTimeout);
			}
		}

		internal static TimeSpan DeviceClassCacheMaxStartDelay
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.DeviceClassCacheMaxStartDelay);
			}
		}

		internal static int DeviceClassCacheMaxADUploadCount
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.DeviceClassCacheMaxADUploadCount);
			}
		}

		internal static int DeviceClassCachePerOrgRefreshInterval
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.DeviceClassCachePerOrgRefreshInterval);
			}
		}

		internal static int RequestCacheMaxCount
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.RequestCacheMaxCount);
			}
		}

		internal static int RequestCacheTimeInterval
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.RequestCacheTimeInterval);
			}
		}

		internal static bool SyncLogEnabled
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.SyncLogEnabled);
			}
		}

		internal static string SyncLogDirectory
		{
			get
			{
				return GlobalSettings.GetSetting<string>(GlobalSettingsSchema.SyncLogDirectory);
			}
		}

		internal static int DeviceClassPerOrgMaxADCount
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.DeviceClassPerOrgMaxADCount);
			}
		}

		internal static int DeviceClassCacheADCleanupInterval
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.DeviceClassCacheADCleanupInterval);
			}
		}

		internal static string[] DeviceTypesSupportingRedirect
		{
			get
			{
				return GlobalSettings.GetSetting<string[]>(GlobalSettingsSchema.DeviceTypesSupportingRedirect);
			}
		}

		internal static string MDMActivationUrl
		{
			get
			{
				return GlobalSettings.GetSetting<string>(GlobalSettingsSchema.MdmActivationUrl);
			}
		}

		internal static Uri MDMComplianceStatusUrl
		{
			get
			{
				return GlobalSettings.GetSetting<Uri>(GlobalSettingsSchema.MdmComplianceStatusUrl);
			}
		}

		internal static Uri MDMEnrollmentUrl
		{
			get
			{
				return GlobalSettings.GetSetting<Uri>(GlobalSettingsSchema.MdmEnrollmentUrl);
			}
		}

		internal static Uri MdmEnrollmentUrlWithBasicSteps
		{
			get
			{
				return GlobalSettings.GetSetting<Uri>(GlobalSettingsSchema.MdmEnrollmentUrlWithBasicSteps);
			}
		}

		internal static string MdmActivationUrlWithBasicSteps
		{
			get
			{
				return GlobalSettings.GetSetting<string>(GlobalSettingsSchema.MdmActivationUrlWithBasicSteps);
			}
		}

		internal static string ADRegistrationServiceUrl
		{
			get
			{
				return GlobalSettings.GetSetting<string>(GlobalSettingsSchema.ADRegistrationServiceUrl);
			}
		}

		internal static List<string> DeviceTypesWithBasicMDMNotification
		{
			get
			{
				return GlobalSettings.GetSetting<List<string>>(GlobalSettingsSchema.DeviceTypesWithBasicMDMNotification);
			}
		}

		internal static List<string> DeviceTypesToParseOSVersion
		{
			get
			{
				return GlobalSettings.GetSetting<List<string>>(GlobalSettingsSchema.DeviceTypesToParseOSVersion);
			}
		}

		internal static TimeSpan NegativeDeviceStatusCacheExpirationInterval
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.NegativeDeviceStatusCacheExpirationInterval);
			}
		}

		internal static TimeSpan DeviceStatusCacheExpirationInterval
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.DeviceStatusCacheExpirationInterval);
			}
		}

		internal static bool DisableDeviceHealthStatusCache
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.DisableAadClientCache);
			}
		}

		internal static bool DisableAadClientCache
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.DisableAadClientCache);
			}
		}

		internal static List<string> ValidSingleNamespaceUrls
		{
			get
			{
				return GlobalSettings.GetSetting<List<string>>(GlobalSettingsSchema.ValidSingleNamespaceUrls);
			}
		}

		internal static int MaxRetrievedItems
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxRetrievedItems);
			}
		}

		internal static int MaxNoOfItemsMove
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxNoOfItemsMove);
			}
		}

		internal static int MaxWindowSize
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxWindowSize);
			}
		}

		internal static TimeSpan BudgetBackOffMinThreshold
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.BudgetBackOffMinThreshold);
			}
		}

		internal static double AutoblockBackOffMediumThreshold
		{
			get
			{
				return GlobalSettings.GetSetting<double>(GlobalSettingsSchema.AutoblockBackOffMediumThreshold);
			}
		}

		internal static double AutoblockBackOffHighThreshold
		{
			get
			{
				return GlobalSettings.GetSetting<double>(GlobalSettingsSchema.AutoblockBackOffHighThreshold);
			}
		}

		internal static int MaxNumberOfClientOperations
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxNumberOfClientOperations);
			}
		}

		internal static int MinRedirectProtocolVersion
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MinRedirectProtocolVersion);
			}
		}

		internal static TimeSpan ADCacheRefreshInterval
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.ADCacheRefreshInterval);
			}
		}

		internal static TimeSpan MaxCleanUpExecutionTime
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.MaxCleanUpExecutionTime);
			}
		}

		internal static TimeSpan VdirCacheTimeoutSeconds
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.VdirCacheTimeoutSeconds);
			}
		}

		internal static TimeSpan EventQueuePollingInterval
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.EventQueuePollingInterval);
			}
		}

		internal static bool AllowInternalUntrustedCerts
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.AllowInternalUntrustedCerts);
			}
		}

		internal static bool AllowProxyingWithoutSsl
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.AllowProxyingWithoutSsl);
			}
		}

		internal static int HDPhotoCacheMaxSize
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.HDPhotoCacheMaxSize);
			}
		}

		internal static TimeSpan MaxRequestExecutionTime
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.MaxRequestExecutionTime);
			}
		}

		internal static TimeSpan HDPhotoCacheExpirationTimeOut
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.HDPhotoCacheExpirationTimeOut);
			}
		}

		internal static UserPhotoResolution HDPhotoDefaultSupportedResolution
		{
			get
			{
				return GlobalSettings.GetSetting<UserPhotoResolution>(GlobalSettingsSchema.HDPhotoDefaultSupportedResolution);
			}
		}

		internal static int HDPhotoMaxNumberOfRequestsToProcess
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.HDPhotoMaxNumberOfRequestsToProcess);
			}
		}

		internal static int BackOffErrorWindow
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.BackOffErrorWindow);
			}
		}

		internal static int BackOffThreshold
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.BackOffThreshold);
			}
		}

		internal static int BackOffTimeOut
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.BackOffTimeOut);
			}
		}

		internal static string BadItemEmailToText
		{
			get
			{
				return GlobalSettings.GetSetting<string>(GlobalSettingsSchema.BadItemEmailToText);
			}
		}

		internal static bool BadItemIncludeEmailToText
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.BadItemIncludeEmailToText);
			}
		}

		internal static bool BadItemIncludeStackTrace
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.BadItemIncludeStackTrace);
			}
		}

		internal static bool BlockLegacyMailboxes
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.BlockLegacyMailboxes);
			}
		}

		internal static bool SkipAzureADCall
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.SkipAzureADCall);
			}
		}

		internal static bool BlockNewMailboxes
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.BlockNewMailboxes);
			}
		}

		internal static string BootstrapCABForWM61HostingURL
		{
			get
			{
				return GlobalSettings.GetSetting<string>(GlobalSettingsSchema.BootstrapCABForWM61HostingURL);
			}
		}

		internal static string MobileUpdateInformationURL
		{
			get
			{
				return GlobalSettings.GetSetting<string>(GlobalSettingsSchema.MobileUpdateInformationURL);
			}
		}

		internal static int BootstrapMailDeliveryDelay
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.BootstrapMailDeliveryDelay);
			}
		}

		internal static bool DisableCaching
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.DisableCaching);
			}
		}

		internal static bool EnableCredentialRequest
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.EnableCredentialRequest);
			}
		}

		internal static bool EnableMailboxLoggingVerboseMode
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.EnableMailboxLoggingVerboseMode);
			}
		}

		internal static int HeartbeatAlertThreshold
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.HeartbeatAlertThreshold);
			}
		}

		internal static int HeartbeatSampleSize
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.HeartbeatSampleSize);
			}
		}

		internal static TimeSpan ADCacheExpirationTimeout
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.ADCacheExpirationTimeout);
			}
		}

		internal static int ADCacheMaxOrgCount
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.ADCacheMaxOrgCount);
			}
		}

		internal static TimeSpan MailboxSearchTimeout
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.MailboxSearchTimeout);
			}
		}

		public static int MailboxSessionCacheInitialSize
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MailboxSessionCacheInitialSize);
			}
		}

		public static int MailboxSessionCacheMaxSize
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MailboxSessionCacheMaxSize);
			}
		}

		public static TimeSpan MailboxSessionCacheTimeout
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.MailboxSessionCacheTimeout);
			}
		}

		internal static TimeSpan MailboxSearchTimeoutNoContentIndexing
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.MailboxSearchTimeoutNoContentIndexing);
			}
		}

		internal static int MaxClientSentBadItems
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxClientSentBadItems);
			}
		}

		internal static int MaxCollectionsToLog
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxCollectionsToLog);
			}
		}

		internal static int MaxDocumentDataSize
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxDocumentDataSize);
			}
		}

		internal static int MaxDocumentLibrarySearchResults
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxDocumentLibrarySearchResults);
			}
		}

		internal static int MaxGALSearchResults
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxGALSearchResults);
			}
		}

		internal static HeartBeatInterval HeartbeatInterval
		{
			get
			{
				return GlobalSettings.GetSetting<HeartBeatInterval>(GlobalSettingsSchema.HeartBeatInterval);
			}
		}

		internal static int MaxMailboxSearchResults
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxMailboxSearchResults);
			}
		}

		internal static int MaxNumOfFolders
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxNumOfFolders);
			}
		}

		internal static int MaxRequestsQueued
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxRequestsQueued);
			}
		}

		internal static int MaxSizeOfMailboxLog
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxSizeOfMailboxLog);
			}
		}

		internal static int MaxNoOfPartnershipToAutoClean
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxNoOfPartnershipToAutoClean);
			}
		}

		internal static int MaxSMimeADDistributionListExpansion
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxSMimeADDistributionListExpansion);
			}
		}

		internal static bool IrmEnabled
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.IrmEnabled);
			}
		}

		internal static int MaxRmsTemplates
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxRmsTemplates);
			}
		}

		internal static TimeSpan NegativeRmsTemplateCacheExpirationInterval
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.NegativeRmsTemplateCacheExpirationInterval);
			}
		}

		internal static int MaxWorkerThreadsPerProc
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxWorkerThreadsPerProc);
			}
		}

		internal static int NumOfQueuedMailboxLogEntries
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.NumOfQueuedMailboxLogEntries);
			}
		}

		internal static int ProxyConnectionPoolConnectionLimit
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.ProxyConnectionPoolConnectionLimit);
			}
		}

		internal static TimeSpan ProxyHandlerLongTimeout
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.ProxyHandlerLongTimeout);
			}
		}

		internal static TimeSpan ProxyHandlerShortTimeout
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.ProxyHandlerShortTimeout);
			}
		}

		internal static int EarlyCompletionTolerance
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.EarlyCompletionTolerance);
			}
		}

		internal static int EarlyWakeupBufferTime
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.EarlyWakeupBufferTime);
			}
		}

		internal static string[] ProxyHeaders
		{
			get
			{
				return GlobalSettings.GetSetting<string[]>(GlobalSettingsSchema.ProxyHeaders);
			}
		}

		internal static string ProxyVirtualDirectory
		{
			get
			{
				return GlobalSettings.GetSetting<string>(GlobalSettingsSchema.ProxyVirtualDirectory);
			}
		}

		internal static string SchemaDirectory
		{
			get
			{
				return GlobalSettings.GetSetting<string>(GlobalSettingsSchema.SchemaDirectory);
			}
		}

		internal static bool SendWatsonReport
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.SendWatsonReport);
			}
		}

		internal static bool FullServerVersion
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.FullServerVersion);
			}
		}

		internal static int ErrorResponseDelay
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.ErrorResponseDelay);
			}
		}

		internal static int MaxThrottlingDelay
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MaxThrottlingDelay);
			}
		}

		internal static List<string> SupportedIPMTypes
		{
			get
			{
				return GlobalSettings.GetSetting<List<string>>(GlobalSettingsSchema.SupportedIPMTypes);
			}
		}

		internal static MultiValuedProperty<string> RemoteDocumentsAllowedServers
		{
			get
			{
				return GlobalSettings.GetSetting<MultiValuedProperty<string>>(GlobalSettingsSchema.RemoteDocumentsAllowedServers);
			}
		}

		internal static RemoteDocumentsActions? RemoteDocumentsActionForUnknownServers
		{
			get
			{
				return GlobalSettings.GetSetting<RemoteDocumentsActions?>(GlobalSettingsSchema.RemoteDocumentsActionForUnknownServers);
			}
		}

		internal static MultiValuedProperty<string> RemoteDocumentsBlockedServers
		{
			get
			{
				return GlobalSettings.GetSetting<MultiValuedProperty<string>>(GlobalSettingsSchema.RemoteDocumentsBlockedServers);
			}
		}

		internal static MultiValuedProperty<string> RemoteDocumentsInternalDomainSuffixList
		{
			get
			{
				return GlobalSettings.GetSetting<MultiValuedProperty<string>>(GlobalSettingsSchema.RemoteDocumentsInternalDomainSuffixList);
			}
		}

		internal static int UpgradeGracePeriod
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.UpgradeGracePeriod);
			}
		}

		internal static int DeviceDiscoveryPeriod
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.DeviceDiscoveryPeriod);
			}
		}

		internal static bool Validate
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.SchemaValidate);
			}
		}

		internal static bool IsMultiTenancyEnabled
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.IsMultiTenancyEnabled);
			}
		}

		internal static bool IsWindowsLiveIDEnabled
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.IsWindowsLiveIDEnabled);
			}
		}

		internal static bool IsGCCEnabled
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.IsGCCEnabled);
			}
		}

		internal static bool AreGccStoredSecretKeysValid
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.AreGccStoredSecretKeysValid);
			}
		}

		internal static bool IsPartnerHostedOnly
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.IsPartnerHostedOnly);
			}
		}

		internal static string ExternalProxy
		{
			get
			{
				return GlobalSettings.GetSetting<string>(GlobalSettingsSchema.ExternalProxy);
			}
		}

		internal static bool ClientAccessRulesLogPeriodicEvent
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.ClientAccessRulesLogPeriodicEvent);
			}
		}

		internal static double ClientAccessRulesLatencyThreshold
		{
			get
			{
				return GlobalSettings.GetSetting<double>(GlobalSettingsSchema.ClientAccessRulesLatencyThreshold);
			}
		}

		public static int DeviceBehaviorCacheInitialSize
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.DeviceBehaviorCacheInitialSize);
			}
		}

		public static int DeviceBehaviorCacheMaxSize
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.DeviceBehaviorCacheMaxSize);
			}
		}

		public static int DeviceBehaviorCacheTimeout
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.DeviceBehaviorCacheTimeout);
			}
		}

		public static bool WriteActivityContextDiagnostics
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.WriteActivityContextDiagnostics);
			}
		}

		public static bool WriteBudgetDiagnostics
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.WriteBudgetDiagnostics);
			}
		}

		public static bool OnlyOrganizersCanSendMeetingChanges
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.OnlyOrganizersCanSendMeetingChanges);
			}
		}

		internal static bool AutoBlockWriteToAd
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.AutoBlockWriteToAd);
			}
		}

		internal static int AutoBlockADWriteDelay
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.AutoBlockADWriteDelay);
			}
		}

		internal static int ADDataSyncInterval
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.ADDataSyncInterval);
			}
		}

		public static bool WriteExceptionDiagnostics
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.WriteExceptionDiagnostics);
			}
		}

		public static bool LogCompressedExceptionDetails
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.LogCompressedExceptionDetails);
			}
		}

		public static bool IncludeRequestInWatson
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.IncludeRequestInWatson);
			}
		}

		public static TimeSpan MeetingOrganizerCleanupTime
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.MeetingOrganizerCleanupTime);
			}
		}

		public static TimeSpan MeetingOrganizerEntryLiveTime
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.MeetingOrganizerEntryLiveTime);
			}
		}

		public static bool TimeTrackingEnabled
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.TimeTrackingEnabled);
			}
		}

		internal static GlobalSettings.DirectPushEnabled AllowDirectPush
		{
			get
			{
				return GlobalSettings.GetSetting<GlobalSettings.DirectPushEnabled>(GlobalSettingsSchema.AllowDirectPush);
			}
		}

		internal static int MinGALSearchLength
		{
			get
			{
				return GlobalSettings.GetSetting<int>(GlobalSettingsSchema.MinGALSearchLength);
			}
		}

		internal static bool EnableV160
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.EnableV160);
			}
		}

		internal static TimeSpan MaxBackOffDuration
		{
			get
			{
				return GlobalSettings.GetSetting<TimeSpan>(GlobalSettingsSchema.MaxBackoffDuration);
			}
		}

		internal static bool AddBackOffReasonHeader
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.AddBackOffReasonHeader);
			}
		}

		internal static bool AllowFlightingOverrides
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.AllowFlightingOverrides);
			}
		}

		internal static bool GetGoidFromCalendarItemForMeetingResponse
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.GetGoidFromCalendarItemForMeetingResponse);
			}
		}

		internal static T GetSetting<T>(GlobalSettingsPropertyDefinition propDef)
		{
			return (T)((object)GlobalSettings.settings.GetOrAdd(propDef, (GlobalSettingsPropertyDefinition propDef2) => GlobalSettings.LoadSetting<T>(propDef2)));
		}

		internal static object GetSetting(GlobalSettingsPropertyDefinition propDef)
		{
			return GlobalSettings.settings.GetOrAdd(propDef, (GlobalSettingsPropertyDefinition propDef2) => GlobalSettings.LoadSetting(propDef2));
		}

		internal static void ForceLoadAllSettings()
		{
			foreach (GlobalSettingsPropertyDefinition propDef in GlobalSettingsSchema.AllProperties)
			{
				GlobalSettings.GetSetting(propDef);
			}
		}

		internal static bool DisableCharsetDetectionInCopyMessageContents
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.DisableCharsetDetectionInCopyMessageContents);
			}
		}

		internal static bool UseOAuthMasterSidForSecurityContext
		{
			get
			{
				return GlobalSettings.GetSetting<bool>(GlobalSettingsSchema.UseOAuthMasterSidForSecurityContext);
			}
		}

		private static object LoadSetting(GlobalSettingsPropertyDefinition propDef)
		{
			object obj = propDef.Getter(propDef);
			PropertyConstraintViolationError propertyConstraintViolationError = propDef.Validate(obj);
			if (propertyConstraintViolationError != null)
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueOutOfRange, new string[]
				{
					propDef.Name,
					propDef.ReadConstraint.ToString(),
					obj.ToString(),
					propDef.DefaultValue.ToString()
				});
				return propDef.DefaultValue;
			}
			return obj;
		}

		private static T LoadSetting<T>(GlobalSettingsPropertyDefinition propDef)
		{
			if (propDef.Type != typeof(T))
			{
				throw new ArgumentException(string.Format("Property {0} is not of the correct type {1}, but is a {2} property", propDef.Name, typeof(T).Name, propDef.Type.Name));
			}
			object obj = propDef.Getter(propDef);
			PropertyConstraintViolationError propertyConstraintViolationError = propDef.Validate(obj);
			if (propertyConstraintViolationError != null)
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueOutOfRange, new string[]
				{
					propDef.Name,
					propDef.ReadConstraint.ToString(),
					obj.ToString(),
					propDef.DefaultValue.ToString()
				});
				return (T)((object)propDef.DefaultValue);
			}
			return (T)((object)obj);
		}

		internal const string CertificateValidationComponentId = "AirSync";

		private const string CasSyncLogsDirectory = "CasSyncLogs";

		private const string SyncLogComponentName = "AirSync";

		private static ConcurrentDictionary<GlobalSettingsPropertyDefinition, object> settings = new ConcurrentDictionary<GlobalSettingsPropertyDefinition, object>();

		private static SyncLog syncLog;

		private static object syncLogCreationLock = new object();

		public enum DirectPushEnabled
		{
			Off,
			On,
			OnWithAddressCheck
		}

		public delegate bool OnLoadConfigSettingDelegate(GlobalSettingsPropertyDefinition propDef, out string value);
	}
}
