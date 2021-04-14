using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.EventLogs
{
	public static class ClientsEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsFolderNotFound = new ExEventLog.EventTuple(3221225473U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryNotFound = new ExEventLog.EventTuple(3221225474U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryMissingBaseExperience = new ExEventLog.EventTuple(3221225475U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryReDefinition = new ExEventLog.EventTuple(3221225476U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryExpectedElement = new ExEventLog.EventTuple(3221225477U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryExpectedAttribute = new ExEventLog.EventTuple(3221225478U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryInvalidUserOfBaseExperience = new ExEventLog.EventTuple(3221225479U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryExpectedBaseExperienceOrInheritsFrom = new ExEventLog.EventTuple(3221225480U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryParseError = new ExEventLog.EventTuple(3221225481U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryInvalidMinimumVersion = new ExEventLog.EventTuple(3221225482U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryInvalidApplicationElement = new ExEventLog.EventTuple(3221225483U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryInvalidClientControl = new ExEventLog.EventTuple(3221225484U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryExpectedClientOrApplicationElement = new ExEventLog.EventTuple(3221225485U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaStartedSuccessfully = new ExEventLog.EventTuple(14U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaAttachmentFileTypeInvalidCharacter = new ExEventLog.EventTuple(3221225487U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaConfigurationNotFound = new ExEventLog.EventTuple(3221225488U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoThemesFolder = new ExEventLog.EventTuple(3221225490U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ThemeInfoExpectedElement = new ExEventLog.EventTuple(3221225491U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ThemeInfoDuplicatedAttribute = new ExEventLog.EventTuple(3221225492U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ThemeInfoAttributeExceededMaximumLength = new ExEventLog.EventTuple(3221225494U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ThemeInfoEmptyAttribute = new ExEventLog.EventTuple(3221225493U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ThemeInfoMissingAttribute = new ExEventLog.EventTuple(3221225496U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ThemeInfoErrorParsingXml = new ExEventLog.EventTuple(3221225497U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoBaseTheme = new ExEventLog.EventTuple(3221225498U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryInvalidRequiredFeatures = new ExEventLog.EventTuple(3221225499U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmallIconsFileNotFound = new ExEventLog.EventTuple(3221225500U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WebConfigAuthenticationIncorrect = new ExEventLog.EventTuple(3221225501U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_VDirAnonymous = new ExEventLog.EventTuple(3221225502U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CustomPropertiesRootElementNotFound = new ExEventLog.EventTuple(3221225503U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidCustomPropertiesAttributeCount = new ExEventLog.EventTuple(3221225504U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CustomPropertiesAttributeNotFound = new ExEventLog.EventTuple(3221225505U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidElementInCustomPropertiesFile = new ExEventLog.EventTuple(3221225506U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CustomPropertiesParseError = new ExEventLog.EventTuple(3221225507U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CustomPropertiesInvalidAttibuteValue = new ExEventLog.EventTuple(3221225508U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PrimarySmtpAddressUnavailable = new ExEventLog.EventTuple(3221225509U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorWrongUriFormat = new ExEventLog.EventTuple(3221225510U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorCASFailoverTryNextOne = new ExEventLog.EventTuple(3221225511U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorCASFailoverAllAttemptsFailed = new ExEventLog.EventTuple(3221225512U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorCouldNotFindCAS = new ExEventLog.EventTuple(3221225513U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorSslConnection = new ExEventLog.EventTuple(3221225514U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorSslTrustFailure = new ExEventLog.EventTuple(3221225515U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorTooManySidsInContext = new ExEventLog.EventTuple(3221225516U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorAccessCheck = new ExEventLog.EventTuple(3221225517U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorCASCompatibility = new ExEventLog.EventTuple(3221225518U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscodingCacheFolderCreationFailed = new ExEventLog.EventTuple(3221225519U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscodingWorkerApplicationNotRegistered = new ExEventLog.EventTuple(3221225520U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscodingWorkerApplicationNotFound = new ExEventLog.EventTuple(3221225521U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADNotificationsRegistration = new ExEventLog.EventTuple(3221225522U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADMessageClassificationRegistration = new ExEventLog.EventTuple(3221225523U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReadSettingsFromAD = new ExEventLog.EventTuple(3221225524U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReadMessageClassificationFromAD = new ExEventLog.EventTuple(3221225525U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADSystemConfigurationSession = new ExEventLog.EventTuple(3221225526U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FormsRegistryInvalidUserOfIsRichClient = new ExEventLog.EventTuple(3221225527U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscodingManagerInitializationFailed = new ExEventLog.EventTuple(1073741880U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorServiceDiscovery = new ExEventLog.EventTuple(3221225529U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaRestartingAfterFailedLoad = new ExEventLog.EventTuple(58U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CorruptCalendarConfiguration = new ExEventLog.EventTuple(3221225531U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmallIconsAltReferenceInvalid = new ExEventLog.EventTuple(3221225532U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxServerVersionConfiguration = new ExEventLog.EventTuple(3221225533U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscodingWorkerProcessFails = new ExEventLog.EventTuple(1073741886U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationSettingsUpdated = new ExEventLog.EventTuple(63U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToUpdateConfigurationSettings = new ExEventLog.EventTuple(3221225536U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GenericConfigurationUpdateError = new ExEventLog.EventTuple(3221225537U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnregisterADNotifications = new ExEventLog.EventTuple(1073741890U, 8, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UnregisterADMessageClassification = new ExEventLog.EventTuple(1073741891U, 8, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscodingWorkerInitializationFailed = new ExEventLog.EventTuple(1073741892U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscodingCacheReachedQuota = new ExEventLog.EventTuple(1073741893U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscodingCacheFolderACLSettingAccessDenied = new ExEventLog.EventTuple(3221225542U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorAuthenticationToCas2Failure = new ExEventLog.EventTuple(3221225543U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CustomizationUIExtensionParseError = new ExEventLog.EventTuple(3221225544U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CustomizationFormsRegistryLoadSuccessfully = new ExEventLog.EventTuple(73U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PayloadNotBeingPickedup = new ExEventLog.EventTuple(3221225546U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToEstablishMTLSConnection = new ExEventLog.EventTuple(3221225547U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorSipEndpointTerminate = new ExEventLog.EventTuple(3221225548U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorCollaborationManagerTerminate = new ExEventLog.EventTuple(3221225549U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToEstablishIMConnection = new ExEventLog.EventTuple(3221225550U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IncompatibleTimeoutSetting = new ExEventLog.EventTuple(3221225551U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorNoCASFoundForInSiteMailbox = new ExEventLog.EventTuple(3221225552U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorNoCASFoundForCrossSiteMailboxToRedirect = new ExEventLog.EventTuple(3221225553U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorMSNSignOut = new ExEventLog.EventTuple(3221225554U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMProviderNoRegistrySetting = new ExEventLog.EventTuple(3221225555U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMProviderFileDoesNotExist = new ExEventLog.EventTuple(3221225556U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMProviderMultipleClasses = new ExEventLog.EventTuple(3221225557U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMProviderNoValidConstructor = new ExEventLog.EventTuple(3221225558U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMProviderExceptionDuringLoad = new ExEventLog.EventTuple(3221225559U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMCreateEndpointFailure = new ExEventLog.EventTuple(3221225560U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorNoLegacyCasToRedirect = new ExEventLog.EventTuple(3221225562U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorNoSslE2003CasToRedirect = new ExEventLog.EventTuple(3221225563U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorNoSslE2007CasToRedirect = new ExEventLog.EventTuple(3221225564U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LiveHeaderConfigurationError = new ExEventLog.EventTuple(3221225565U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorMailboxServerTooBusy = new ExEventLog.EventTuple(3221225566U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscodingCacheFolderDeletingAccessDenied = new ExEventLog.EventTuple(1073741919U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TranscodingStartSuccessfully = new ExEventLog.EventTuple(96U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorCreatingClientContext = new ExEventLog.EventTuple(3221225569U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TooManyOWAReInitializationRequests = new ExEventLog.EventTuple(1073741922U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MessagingPayloadNotBeingPickedup = new ExEventLog.EventTuple(1073741923U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMServerNameInvalid = new ExEventLog.EventTuple(3221225572U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMCertificateThumbprintInvalid = new ExEventLog.EventTuple(3221225573U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMCertificateExpired = new ExEventLog.EventTuple(3221225574U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMCertificateNotFound = new ExEventLog.EventTuple(3221225575U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMCertificateInvalidDate = new ExEventLog.EventTuple(3221225576U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMCertificateNoPrivateKey = new ExEventLog.EventTuple(3221225577U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorIMCertificateWillExpireSoon = new ExEventLog.EventTuple(1073741930U, 9, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoThemeResources = new ExEventLog.EventTuple(3221225579U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaEwsConnectionError = new ExEventLog.EventTuple(3221225580U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_BposHeaderConfigurationError = new ExEventLog.EventTuple(3221225581U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MdbConcurrencySettingsInvalid = new ExEventLog.EventTuple(3221225582U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorResourceUnhealthy = new ExEventLog.EventTuple(3221225583U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_IMEndpointManagerInitializedSuccessfully = new ExEventLog.EventTuple(112U, 9, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StorageTransientExceptionWarning = new ExEventLog.EventTuple(2147483761U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ArchiveMailboxAccessFailedWarning = new ExEventLog.EventTuple(1073741939U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidThemeFolder = new ExEventLog.EventTuple(2147483764U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MSNProviderInitializationError = new ExEventLog.EventTuple(3221225589U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MSNProviderInitializationSucceeded = new ExEventLog.EventTuple(118U, 9, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaStartPageInitializationError = new ExEventLog.EventTuple(3221225591U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaStartPageInitializationWarning = new ExEventLog.EventTuple(1073741944U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_O365SetUserThemeError = new ExEventLog.EventTuple(3221225593U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LiveAssetReaderInitResourceConsumerStarted = new ExEventLog.EventTuple(1073741955U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LiveAssetReaderInitResourceConsumerSucceeded = new ExEventLog.EventTuple(1073741956U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LiveAssetReaderInitResourceConsumerError = new ExEventLog.EventTuple(1073741957U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaConfigurationWebSiteUnavailable = new ExEventLog.EventTuple(3221225606U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorNoSslE2010CasToRedirect = new ExEventLog.EventTuple(3221225607U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorNo2007OrAboveCasToRedirect = new ExEventLog.EventTuple(3221225608U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UserContextTerminationError = new ExEventLog.EventTuple(3221225609U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WacConfigurationSetupError = new ExEventLog.EventTuple(3221225611U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WacConfigurationSetupSuccessful = new ExEventLog.EventTuple(140U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WacDiscoveryDataRetrievalFailure = new ExEventLog.EventTuple(3221225613U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WacDiscoveryDataRetrievedSuccessfully = new ExEventLog.EventTuple(142U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RmsTemplateLoadFailure = new ExEventLog.EventTuple(3221225615U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaManifestInvalid = new ExEventLog.EventTuple(3221225616U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DfpOwaStartedSuccessfully = new ExEventLog.EventTuple(145U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OwaFailedToCreateExchangePrincipal = new ExEventLog.EventTuple(3221225618U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationCachePerfCountersLoadFailure = new ExEventLog.EventTuple(3221225619U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FblFailedToConnectToSmtpServer = new ExEventLog.EventTuple(3221225620U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FblSmtpServerResponse = new ExEventLog.EventTuple(3221225621U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FblErrorSendingMessage = new ExEventLog.EventTuple(3221225622U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransientFblErrorUpdatingMServ = new ExEventLog.EventTuple(3221225623U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PermanentFblErrorUpdatingMServ = new ExEventLog.EventTuple(3221225624U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransientFblErrorReadingMServ = new ExEventLog.EventTuple(3221225625U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PermanentFblErrorReadingMServ = new ExEventLog.EventTuple(3221225626U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FblUnableToProcessRequest = new ExEventLog.EventTuple(3221225627U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			FormsRegistry = 1,
			Core,
			Configuration,
			Themes,
			SmallIcons,
			Proxy,
			Transcoding,
			ADNotifications,
			InstantMessage,
			Wac,
			EOP
		}

		internal enum Message : uint
		{
			FormsFolderNotFound = 3221225473U,
			FormsRegistryNotFound,
			FormsRegistryMissingBaseExperience,
			FormsRegistryReDefinition,
			FormsRegistryExpectedElement,
			FormsRegistryExpectedAttribute,
			FormsRegistryInvalidUserOfBaseExperience,
			FormsRegistryExpectedBaseExperienceOrInheritsFrom,
			FormsRegistryParseError,
			FormsRegistryInvalidMinimumVersion,
			FormsRegistryInvalidApplicationElement,
			FormsRegistryInvalidClientControl,
			FormsRegistryExpectedClientOrApplicationElement,
			OwaStartedSuccessfully = 14U,
			OwaAttachmentFileTypeInvalidCharacter = 3221225487U,
			OwaConfigurationNotFound,
			NoThemesFolder = 3221225490U,
			ThemeInfoExpectedElement,
			ThemeInfoDuplicatedAttribute,
			ThemeInfoAttributeExceededMaximumLength = 3221225494U,
			ThemeInfoEmptyAttribute = 3221225493U,
			ThemeInfoMissingAttribute = 3221225496U,
			ThemeInfoErrorParsingXml,
			NoBaseTheme,
			FormsRegistryInvalidRequiredFeatures,
			SmallIconsFileNotFound,
			WebConfigAuthenticationIncorrect,
			VDirAnonymous,
			CustomPropertiesRootElementNotFound,
			InvalidCustomPropertiesAttributeCount,
			CustomPropertiesAttributeNotFound,
			InvalidElementInCustomPropertiesFile,
			CustomPropertiesParseError,
			CustomPropertiesInvalidAttibuteValue,
			PrimarySmtpAddressUnavailable,
			ProxyErrorWrongUriFormat,
			ProxyErrorCASFailoverTryNextOne,
			ProxyErrorCASFailoverAllAttemptsFailed,
			ProxyErrorCouldNotFindCAS,
			ProxyErrorSslConnection,
			ProxyErrorSslTrustFailure,
			ProxyErrorTooManySidsInContext,
			ProxyErrorAccessCheck,
			ProxyErrorCASCompatibility,
			TranscodingCacheFolderCreationFailed,
			TranscodingWorkerApplicationNotRegistered,
			TranscodingWorkerApplicationNotFound,
			ADNotificationsRegistration,
			ADMessageClassificationRegistration,
			ReadSettingsFromAD,
			ReadMessageClassificationFromAD,
			ADSystemConfigurationSession,
			FormsRegistryInvalidUserOfIsRichClient,
			TranscodingManagerInitializationFailed = 1073741880U,
			ProxyErrorServiceDiscovery = 3221225529U,
			OwaRestartingAfterFailedLoad = 58U,
			CorruptCalendarConfiguration = 3221225531U,
			SmallIconsAltReferenceInvalid,
			MailboxServerVersionConfiguration,
			TranscodingWorkerProcessFails = 1073741886U,
			ConfigurationSettingsUpdated = 63U,
			FailedToUpdateConfigurationSettings = 3221225536U,
			GenericConfigurationUpdateError,
			UnregisterADNotifications = 1073741890U,
			UnregisterADMessageClassification,
			TranscodingWorkerInitializationFailed,
			TranscodingCacheReachedQuota,
			TranscodingCacheFolderACLSettingAccessDenied = 3221225542U,
			ProxyErrorAuthenticationToCas2Failure,
			CustomizationUIExtensionParseError,
			CustomizationFormsRegistryLoadSuccessfully = 73U,
			PayloadNotBeingPickedup = 3221225546U,
			FailedToEstablishMTLSConnection,
			ErrorSipEndpointTerminate,
			ErrorCollaborationManagerTerminate,
			FailedToEstablishIMConnection,
			IncompatibleTimeoutSetting,
			ProxyErrorNoCASFoundForInSiteMailbox,
			ProxyErrorNoCASFoundForCrossSiteMailboxToRedirect,
			ErrorMSNSignOut,
			ErrorIMProviderNoRegistrySetting,
			ErrorIMProviderFileDoesNotExist,
			ErrorIMProviderMultipleClasses,
			ErrorIMProviderNoValidConstructor,
			ErrorIMProviderExceptionDuringLoad,
			ErrorIMCreateEndpointFailure,
			ProxyErrorNoLegacyCasToRedirect = 3221225562U,
			ProxyErrorNoSslE2003CasToRedirect,
			ProxyErrorNoSslE2007CasToRedirect,
			LiveHeaderConfigurationError,
			ErrorMailboxServerTooBusy,
			TranscodingCacheFolderDeletingAccessDenied = 1073741919U,
			TranscodingStartSuccessfully = 96U,
			ErrorCreatingClientContext = 3221225569U,
			TooManyOWAReInitializationRequests = 1073741922U,
			MessagingPayloadNotBeingPickedup,
			ErrorIMServerNameInvalid = 3221225572U,
			ErrorIMCertificateThumbprintInvalid,
			ErrorIMCertificateExpired,
			ErrorIMCertificateNotFound,
			ErrorIMCertificateInvalidDate,
			ErrorIMCertificateNoPrivateKey,
			ErrorIMCertificateWillExpireSoon = 1073741930U,
			NoThemeResources = 3221225579U,
			OwaEwsConnectionError,
			BposHeaderConfigurationError,
			MdbConcurrencySettingsInvalid,
			ErrorResourceUnhealthy,
			IMEndpointManagerInitializedSuccessfully = 112U,
			StorageTransientExceptionWarning = 2147483761U,
			ArchiveMailboxAccessFailedWarning = 1073741939U,
			InvalidThemeFolder = 2147483764U,
			MSNProviderInitializationError = 3221225589U,
			MSNProviderInitializationSucceeded = 118U,
			OwaStartPageInitializationError = 3221225591U,
			OwaStartPageInitializationWarning = 1073741944U,
			O365SetUserThemeError = 3221225593U,
			LiveAssetReaderInitResourceConsumerStarted = 1073741955U,
			LiveAssetReaderInitResourceConsumerSucceeded,
			LiveAssetReaderInitResourceConsumerError,
			OwaConfigurationWebSiteUnavailable = 3221225606U,
			ProxyErrorNoSslE2010CasToRedirect,
			ProxyErrorNo2007OrAboveCasToRedirect,
			UserContextTerminationError,
			WacConfigurationSetupError = 3221225611U,
			WacConfigurationSetupSuccessful = 140U,
			WacDiscoveryDataRetrievalFailure = 3221225613U,
			WacDiscoveryDataRetrievedSuccessfully = 142U,
			RmsTemplateLoadFailure = 3221225615U,
			OwaManifestInvalid,
			DfpOwaStartedSuccessfully = 145U,
			OwaFailedToCreateExchangePrincipal = 3221225618U,
			ConfigurationCachePerfCountersLoadFailure,
			FblFailedToConnectToSmtpServer,
			FblSmtpServerResponse,
			FblErrorSendingMessage,
			TransientFblErrorUpdatingMServ,
			PermanentFblErrorUpdatingMServ,
			TransientFblErrorReadingMServ,
			PermanentFblErrorReadingMServ,
			FblUnableToProcessRequest
		}
	}
}
