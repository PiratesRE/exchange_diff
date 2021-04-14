using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal static class ApplicationLogicEventLogConstants
	{
		public const string EventSource = "MSExchangeApplicationLogic";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MonitorHostingDataFileFailure = new ExEventLog.EventTuple(3221488617U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LoadHostingDataFileFailure = new ExEventLog.EventTuple(3221488618U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LoadHostingDataFilesSuccess = new ExEventLog.EventTuple(1074004971U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LocalServerNotInSite = new ExEventLog.EventTuple(3221496716U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LocalServerNotInSiteWarning = new ExEventLog.EventTuple(2147754893U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CantGetLocalIP = new ExEventLog.EventTuple(3221496718U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CantGetLocalIPWarning = new ExEventLog.EventTuple(2147754895U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoLocalServer = new ExEventLog.EventTuple(3221496720U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoLocalServerWarning = new ExEventLog.EventTuple(2147754897U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TopologyException = new ExEventLog.EventTuple(3221496722U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoServerInSite = new ExEventLog.EventTuple(3221496723U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MisconfiguredServer = new ExEventLog.EventTuple(3221496724U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidExtensionRemoved = new ExEventLog.EventTuple(3221490617U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ExtensionsCacheReachedMaxSize = new ExEventLog.EventTuple(1074006970U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ExtensionUpdateQueryMaxExceeded = new ExEventLog.EventTuple(3221490619U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MismatchedCacheMailboxExtensionId = new ExEventLog.EventTuple(3221490620U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ExtensionUpdateFailed = new ExEventLog.EventTuple(3221490621U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CachedManifestParseFailed = new ExEventLog.EventTuple(3221490622U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MismatchedExtensionIdUpdateFailed = new ExEventLog.EventTuple(3221490623U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MoreCapabilitiesSkipUpdate = new ExEventLog.EventTuple(1074006976U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidStateSkipUpdate = new ExEventLog.EventTuple(1074006977U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidAssetIDReturnedInDownload = new ExEventLog.EventTuple(3221490626U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MismatchedExtensionIDReturnedInDownload = new ExEventLog.EventTuple(3221490627U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OldVersionReturnedInDownload = new ExEventLog.EventTuple(3221490628U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MoreCapabilitiesReturnedInDownload = new ExEventLog.EventTuple(3221490629U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidAssetIDReturnedByAppState = new ExEventLog.EventTuple(3221490630U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MismatchedExtensionIDReturnedByAppState = new ExEventLog.EventTuple(3221490631U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TooManyExtensionsForAutomaticUpdate = new ExEventLog.EventTuple(3221490632U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ResponseExceedsBufferSize = new ExEventLog.EventTuple(3221490633U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RequestFailed = new ExEventLog.EventTuple(3221490634U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EmptyAppStateResponse = new ExEventLog.EventTuple(3221490635U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OmexWebServiceResponseParsed = new ExEventLog.EventTuple(1074006989U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidAssetSignatureReturnedInDownload = new ExEventLog.EventTuple(3221490638U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ManifestExceedsAllowedSize = new ExEventLog.EventTuple(3221490639U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidVersionSubmitUpdateQuery = new ExEventLog.EventTuple(3221490640U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DownloadKillbitListFailed = new ExEventLog.EventTuple(3221490641U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DownloadKillbitListSuccessed = new ExEventLog.EventTuple(1074006994U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EntryAddedToKillbitList = new ExEventLog.EventTuple(1074006995U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToReadKillbitList = new ExEventLog.EventTuple(2147748820U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_KillbitAssetTagRefreshRateNotFound = new ExEventLog.EventTuple(3221490645U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AppInKillbitListRemoved = new ExEventLog.EventTuple(1074006998U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AssetIdMissingInKillbitEntry = new ExEventLog.EventTuple(3221490647U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AppIdMissingInKillbitEntry = new ExEventLog.EventTuple(3221490648U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AppStateResponseInvalidVersion = new ExEventLog.EventTuple(3221490649U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AppStateResponseInvalidMarketplaceAssetID = new ExEventLog.EventTuple(3221490650U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AppStateResponseInvalidExtensionID = new ExEventLog.EventTuple(3221490651U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AppStateResponseInvalidState = new ExEventLog.EventTuple(3221490652U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigResponseTokenNamesMissing = new ExEventLog.EventTuple(3221490653U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigResponseServiceNameMissing = new ExEventLog.EventTuple(3221490654U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigResponseUrlsMissing = new ExEventLog.EventTuple(3221490655U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigResponseServiceNameParseFailed = new ExEventLog.EventTuple(3221490656U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigResponseUrlParseFailed = new ExEventLog.EventTuple(3221490657U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigResponseUrlNoTokens = new ExEventLog.EventTuple(2147748834U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigResponseUrlTooManyTokens = new ExEventLog.EventTuple(3221490659U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigResponseUrlTokenNotFound = new ExEventLog.EventTuple(3221490660U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigResponseUrlNotWellFormed = new ExEventLog.EventTuple(3221490661U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EmptyKillbitListLocalFile = new ExEventLog.EventTuple(3221490662U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_KillbitFolderNotExist = new ExEventLog.EventTuple(2147748839U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CanNotCreateKillbitFolder = new ExEventLog.EventTuple(3221490664U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_KillbitFileWatcherFailed = new ExEventLog.EventTuple(3221490665U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OwaVersionRetrievalFailed = new ExEventLog.EventTuple(3221490666U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DefaultExtensionPathNotExist = new ExEventLog.EventTuple(3221490667U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DefaultExtensionFolderAccessFailed = new ExEventLog.EventTuple(3221490668U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DefaultExtensionRetrievalFailed = new ExEventLog.EventTuple(3221490669U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OrganizationMailboxRetrievalFailed = new ExEventLog.EventTuple(3221490670U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OrganizationMailboxWebServiceUrlRetrievalFailed = new ExEventLog.EventTuple(3221490671U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MastertableSaveFailedSaveConflict = new ExEventLog.EventTuple(3221490672U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetOrgExtensionsTimedOut = new ExEventLog.EventTuple(3221490673U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetContentDeliveryNetworkEndpointFailed = new ExEventLog.EventTuple(3221490674U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EcpUriRetrievalFailed = new ExEventLog.EventTuple(3221490675U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FindFronEndOwaServiceFailed = new ExEventLog.EventTuple(3221490676U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_OrgExtensionParsingFailed = new ExEventLog.EventTuple(1074007029U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DownloadDataFromOfficeMarketPlaceSucceeded = new ExEventLog.EventTuple(1074007030U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DownloadDataFromOfficeMarketPlaceFailed = new ExEventLog.EventTuple(3221490679U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_StoredEtokenCorrupted = new ExEventLog.EventTuple(3221490680U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MissingNodeInEtoken = new ExEventLog.EventTuple(3221490681U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidDeploymentIdInEtoken = new ExEventLog.EventTuple(3221490682U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AssetIdNotMatchInEtoken = new ExEventLog.EventTuple(3221490683U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToGetDeploymentId = new ExEventLog.EventTuple(1074007036U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToConfigAppStatus = new ExEventLog.EventTuple(3221490685U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ParseEtokenSuccess = new ExEventLog.EventTuple(1074007038U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetExtensionsSuccess = new ExEventLog.EventTuple(1074007039U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExtensionUpdateSuccess = new ExEventLog.EventTuple(1074007040U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GetExtensionsFailed = new ExEventLog.EventTuple(3221490689U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToWritebackRenewedTokens = new ExEventLog.EventTuple(3221490691U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExtensionTokenQueryMaxExceeded = new ExEventLog.EventTuple(3221490692U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MismatchedReturnedToken = new ExEventLog.EventTuple(3221490693U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProcessTokenRenewCompleted = new ExEventLog.EventTuple(1074007046U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OrgLevelEtokenMustBeSiteLicense = new ExEventLog.EventTuple(3221490695U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_E4EOrganizationMailboxWebServiceUrlRetrievalFailed = new ExEventLog.EventTuple(3221491617U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_E4EOrganizationMailboxRetrievalFailed = new ExEventLog.EventTuple(3221491618U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PersistentHandlerRegistrationFailed = new ExEventLog.EventTuple(3221492617U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			TextMessaging = 1,
			ServerPicker,
			Extension,
			E4E,
			DiagnosticHandlers
		}

		internal enum Message : uint
		{
			MonitorHostingDataFileFailure = 3221488617U,
			LoadHostingDataFileFailure,
			LoadHostingDataFilesSuccess = 1074004971U,
			LocalServerNotInSite = 3221496716U,
			LocalServerNotInSiteWarning = 2147754893U,
			CantGetLocalIP = 3221496718U,
			CantGetLocalIPWarning = 2147754895U,
			NoLocalServer = 3221496720U,
			NoLocalServerWarning = 2147754897U,
			TopologyException = 3221496722U,
			NoServerInSite,
			MisconfiguredServer,
			InvalidExtensionRemoved = 3221490617U,
			ExtensionsCacheReachedMaxSize = 1074006970U,
			ExtensionUpdateQueryMaxExceeded = 3221490619U,
			MismatchedCacheMailboxExtensionId,
			ExtensionUpdateFailed,
			CachedManifestParseFailed,
			MismatchedExtensionIdUpdateFailed,
			MoreCapabilitiesSkipUpdate = 1074006976U,
			InvalidStateSkipUpdate,
			InvalidAssetIDReturnedInDownload = 3221490626U,
			MismatchedExtensionIDReturnedInDownload,
			OldVersionReturnedInDownload,
			MoreCapabilitiesReturnedInDownload,
			InvalidAssetIDReturnedByAppState,
			MismatchedExtensionIDReturnedByAppState,
			TooManyExtensionsForAutomaticUpdate,
			ResponseExceedsBufferSize,
			RequestFailed,
			EmptyAppStateResponse,
			OmexWebServiceResponseParsed = 1074006989U,
			InvalidAssetSignatureReturnedInDownload = 3221490638U,
			ManifestExceedsAllowedSize,
			InvalidVersionSubmitUpdateQuery,
			DownloadKillbitListFailed,
			DownloadKillbitListSuccessed = 1074006994U,
			EntryAddedToKillbitList,
			FailedToReadKillbitList = 2147748820U,
			KillbitAssetTagRefreshRateNotFound = 3221490645U,
			AppInKillbitListRemoved = 1074006998U,
			AssetIdMissingInKillbitEntry = 3221490647U,
			AppIdMissingInKillbitEntry,
			AppStateResponseInvalidVersion,
			AppStateResponseInvalidMarketplaceAssetID,
			AppStateResponseInvalidExtensionID,
			AppStateResponseInvalidState,
			ConfigResponseTokenNamesMissing,
			ConfigResponseServiceNameMissing,
			ConfigResponseUrlsMissing,
			ConfigResponseServiceNameParseFailed,
			ConfigResponseUrlParseFailed,
			ConfigResponseUrlNoTokens = 2147748834U,
			ConfigResponseUrlTooManyTokens = 3221490659U,
			ConfigResponseUrlTokenNotFound,
			ConfigResponseUrlNotWellFormed,
			EmptyKillbitListLocalFile,
			KillbitFolderNotExist = 2147748839U,
			CanNotCreateKillbitFolder = 3221490664U,
			KillbitFileWatcherFailed,
			OwaVersionRetrievalFailed,
			DefaultExtensionPathNotExist,
			DefaultExtensionFolderAccessFailed,
			DefaultExtensionRetrievalFailed,
			OrganizationMailboxRetrievalFailed,
			OrganizationMailboxWebServiceUrlRetrievalFailed,
			MastertableSaveFailedSaveConflict,
			GetOrgExtensionsTimedOut,
			GetContentDeliveryNetworkEndpointFailed,
			EcpUriRetrievalFailed,
			FindFronEndOwaServiceFailed,
			OrgExtensionParsingFailed = 1074007029U,
			DownloadDataFromOfficeMarketPlaceSucceeded,
			DownloadDataFromOfficeMarketPlaceFailed = 3221490679U,
			StoredEtokenCorrupted,
			MissingNodeInEtoken,
			InvalidDeploymentIdInEtoken,
			AssetIdNotMatchInEtoken,
			FailedToGetDeploymentId = 1074007036U,
			FailedToConfigAppStatus = 3221490685U,
			ParseEtokenSuccess = 1074007038U,
			GetExtensionsSuccess,
			ExtensionUpdateSuccess,
			GetExtensionsFailed = 3221490689U,
			FailedToWritebackRenewedTokens = 3221490691U,
			ExtensionTokenQueryMaxExceeded,
			MismatchedReturnedToken,
			ProcessTokenRenewCompleted = 1074007046U,
			OrgLevelEtokenMustBeSiteLicense = 3221490695U,
			E4EOrganizationMailboxWebServiceUrlRetrievalFailed = 3221491617U,
			E4EOrganizationMailboxRetrievalFailed,
			PersistentHandlerRegistrationFailed = 3221492617U
		}
	}
}
