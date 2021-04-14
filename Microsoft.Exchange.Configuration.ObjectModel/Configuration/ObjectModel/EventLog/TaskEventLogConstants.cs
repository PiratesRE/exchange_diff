using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.ObjectModel.EventLog
{
	public static class TaskEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogCmdletSuccess = new ExEventLog.EventTuple(1073741825U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogLowLevelCmdletSuccess = new ExEventLog.EventTuple(1073741826U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogMediumLevelCmdletSuccess = new ExEventLog.EventTuple(1073741827U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogCmdletStopped = new ExEventLog.EventTuple(2147483652U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogCmdletCancelled = new ExEventLog.EventTuple(2147483653U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogCmdletError = new ExEventLog.EventTuple(3221225478U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogLowLevelCmdletError = new ExEventLog.EventTuple(3221225479U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TaskThrowingUnhandledException = new ExEventLog.EventTuple(3221225480U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TaskThrottled = new ExEventLog.EventTuple(1073741874U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SlimTenantTaskThrottled = new ExEventLog.EventTuple(1073741882U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HydrationTaskFailed = new ExEventLog.EventTuple(3221229476U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DestructiveTaskThrottledForFirstOrg = new ExEventLog.EventTuple(1073741880U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DestructiveTaskThrottledForTenant = new ExEventLog.EventTuple(1073741881U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResourceHealthCutOff = new ExEventLog.EventTuple(1073741883U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_NativeCallFailed = new ExEventLog.EventTuple(3221225487U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_UserNotFoundBySid = new ExEventLog.EventTuple(3221225488U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_NoRoleAssignments = new ExEventLog.EventTuple(3221225489U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_NoValidEnabledRoleAssignments = new ExEventLog.EventTuple(3221225490U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_UserNotEnabledForRemotePS = new ExEventLog.EventTuple(3221225491U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CmdletAccessDenied_InvalidCmdlet = new ExEventLog.EventTuple(3221225492U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CmdletAccessDenied_InvalidParameter = new ExEventLog.EventTuple(3221225493U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RBACUnavailable_TransientError = new ExEventLog.EventTuple(3221225494U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RBACUnavailable_UnknownError = new ExEventLog.EventTuple(3221225495U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_DelegatedUser = new ExEventLog.EventTuple(3221225501U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_NoPartnerScopes = new ExEventLog.EventTuple(3221225496U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_OrgNotFound = new ExEventLog.EventTuple(3221225497U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_OrgOutOfPartnerScope = new ExEventLog.EventTuple(3221225498U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TaskMediumDetailWritingErrorInProcessing = new ExEventLog.EventTuple(3221225499U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TaskMediumDetailWritingErrorNotProcessing = new ExEventLog.EventTuple(3221225500U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_MultipleUsersFoundByCertificate = new ExEventLog.EventTuple(3221225502U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_CannotCreateSecurityIdentifier = new ExEventLog.EventTuple(3221225523U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_NoGroupsResolvedForDelegatedAdmin = new ExEventLog.EventTuple(3221225524U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SnapinLoadFailed = new ExEventLog.EventTuple(3221225503U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ScriptNotFound = new ExEventLog.EventTuple(3221225504U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ScriptCorrupted = new ExEventLog.EventTuple(3221225505U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_UserPrincipalNameNotSet = new ExEventLog.EventTuple(3221225506U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToLoadValidationRules = new ExEventLog.EventTuple(3221225511U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RoleBasedStringMappingFailure = new ExEventLog.EventTuple(2147483688U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_FailedToMapPUIDToADAccount = new ExEventLog.EventTuple(3221225513U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProvisioningCacheActivating = new ExEventLog.EventTuple(1073741866U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProvisioningCacheDeactivating = new ExEventLog.EventTuple(1073741867U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RBACUnavailable_InitialSessionStateIsNull = new ExEventLog.EventTuple(3221225516U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RuntimeException = new ExEventLog.EventTuple(3221225517U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RBAC_TenantRedirectionFailed = new ExEventLog.EventTuple(3221225518U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebConfigCorrupted = new ExEventLog.EventTuple(3221225519U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SnapinTypeLoadFailed = new ExEventLog.EventTuple(3221225520U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RBACUnavailable_CannotOpenWebConfig = new ExEventLog.EventTuple(3221225521U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReachedMaxPSConnectionLimit = new ExEventLog.EventTuple(3221225527U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReachedMaxTenantPSConnectionLimit = new ExEventLog.EventTuple(3221225525U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToResolveOrganizationIdForDelegatedPrincipal = new ExEventLog.EventTuple(3221225526U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AccessDenied_CertificateAuthenticationNotAllowed = new ExEventLog.EventTuple(3221225532U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProvisioningCacheError = new ExEventLog.EventTuple(3221225533U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowershellSessionExpired = new ExEventLog.EventTuple(2147483710U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReachedMaxUserPSConnectionLimit = new ExEventLog.EventTuple(3221225542U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReachedMaxPSRunspaceInTimePeriodLimit = new ExEventLog.EventTuple(3221225543U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReachedMaxPowershellCmdletLimit = new ExEventLog.EventTuple(3221225544U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReachedMaxTenantPSRunspaceInTimePeriodLimit = new ExEventLog.EventTuple(3221225545U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PSConnectionLeakDetected = new ExEventLog.EventTuple(3221225546U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PSConnectionLeakPassivelyCorrected = new ExEventLog.EventTuple(3221225547U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RBACUnavailable_FatalError = new ExEventLog.EventTuple(3221225552U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NullItemAddedIntoInitialSessionStateEntryCollection = new ExEventLog.EventTuple(3221225553U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExecuteTaskScriptLatency = new ExEventLog.EventTuple(252U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HandleADDriverTimeoutInPowershellStarted = new ExEventLog.EventTuple(253U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HandleADDriverTimeoutInPowershellFinishedSuccessfully = new ExEventLog.EventTuple(254U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HandleADDriverTimeoutInPowershellFinishedWithError = new ExEventLog.EventTuple(3221225727U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TestRpsConnectivityRecoveryWorkflowRecyclePoolSuccessfully = new ExEventLog.EventTuple(256U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToInitailizeCmdletDataRedactionConfiguration = new ExEventLog.EventTuple(3221225729U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RemotePSPublicAPIFailed = new ExEventLog.EventTuple(3221225730U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PswsPublicAPIFailed = new ExEventLog.EventTuple(3221225733U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidCultureInfo = new ExEventLog.EventTuple(3221225734U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PswsOverBudgetException = new ExEventLog.EventTuple(3221225735U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			General = 1,
			RBAC,
			StringInterfacePacks
		}

		internal enum Message : uint
		{
			LogCmdletSuccess = 1073741825U,
			LogLowLevelCmdletSuccess,
			LogMediumLevelCmdletSuccess,
			LogCmdletStopped = 2147483652U,
			LogCmdletCancelled,
			LogCmdletError = 3221225478U,
			LogLowLevelCmdletError,
			TaskThrowingUnhandledException,
			TaskThrottled = 1073741874U,
			SlimTenantTaskThrottled = 1073741882U,
			HydrationTaskFailed = 3221229476U,
			DestructiveTaskThrottledForFirstOrg = 1073741880U,
			DestructiveTaskThrottledForTenant,
			ResourceHealthCutOff = 1073741883U,
			AccessDenied_NativeCallFailed = 3221225487U,
			AccessDenied_UserNotFoundBySid,
			AccessDenied_NoRoleAssignments,
			AccessDenied_NoValidEnabledRoleAssignments,
			AccessDenied_UserNotEnabledForRemotePS,
			CmdletAccessDenied_InvalidCmdlet,
			CmdletAccessDenied_InvalidParameter,
			RBACUnavailable_TransientError,
			RBACUnavailable_UnknownError,
			AccessDenied_DelegatedUser = 3221225501U,
			AccessDenied_NoPartnerScopes = 3221225496U,
			AccessDenied_OrgNotFound,
			AccessDenied_OrgOutOfPartnerScope,
			TaskMediumDetailWritingErrorInProcessing,
			TaskMediumDetailWritingErrorNotProcessing,
			AccessDenied_MultipleUsersFoundByCertificate = 3221225502U,
			AccessDenied_CannotCreateSecurityIdentifier = 3221225523U,
			AccessDenied_NoGroupsResolvedForDelegatedAdmin,
			SnapinLoadFailed = 3221225503U,
			ScriptNotFound,
			ScriptCorrupted,
			AccessDenied_UserPrincipalNameNotSet,
			FailedToLoadValidationRules = 3221225511U,
			RoleBasedStringMappingFailure = 2147483688U,
			AccessDenied_FailedToMapPUIDToADAccount = 3221225513U,
			ProvisioningCacheActivating = 1073741866U,
			ProvisioningCacheDeactivating,
			RBACUnavailable_InitialSessionStateIsNull = 3221225516U,
			RuntimeException,
			RBAC_TenantRedirectionFailed,
			WebConfigCorrupted,
			SnapinTypeLoadFailed,
			RBACUnavailable_CannotOpenWebConfig,
			ReachedMaxPSConnectionLimit = 3221225527U,
			ReachedMaxTenantPSConnectionLimit = 3221225525U,
			FailedToResolveOrganizationIdForDelegatedPrincipal,
			AccessDenied_CertificateAuthenticationNotAllowed = 3221225532U,
			ProvisioningCacheError,
			PowershellSessionExpired = 2147483710U,
			ReachedMaxUserPSConnectionLimit = 3221225542U,
			ReachedMaxPSRunspaceInTimePeriodLimit,
			ReachedMaxPowershellCmdletLimit,
			ReachedMaxTenantPSRunspaceInTimePeriodLimit,
			PSConnectionLeakDetected,
			PSConnectionLeakPassivelyCorrected,
			RBACUnavailable_FatalError = 3221225552U,
			NullItemAddedIntoInitialSessionStateEntryCollection,
			ExecuteTaskScriptLatency = 252U,
			HandleADDriverTimeoutInPowershellStarted,
			HandleADDriverTimeoutInPowershellFinishedSuccessfully,
			HandleADDriverTimeoutInPowershellFinishedWithError = 3221225727U,
			TestRpsConnectivityRecoveryWorkflowRecyclePoolSuccessfully = 256U,
			FailedToInitailizeCmdletDataRedactionConfiguration = 3221225729U,
			RemotePSPublicAPIFailed,
			PswsPublicAPIFailed = 3221225733U,
			InvalidCultureInfo,
			PswsOverBudgetException
		}
	}
}
