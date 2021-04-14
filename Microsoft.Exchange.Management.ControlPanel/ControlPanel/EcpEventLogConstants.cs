using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class EcpEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpStarted = new ExEventLog.EventTuple(1073741825U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpDisposed = new ExEventLog.EventTuple(1073741826U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpWebServiceStarted = new ExEventLog.EventTuple(1073741827U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RequestFailed = new ExEventLog.EventTuple(3221225476U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceFailed = new ExEventLog.EventTuple(3221225477U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ADTransientFailure = new ExEventLog.EventTuple(3221225478U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpWebServiceRequestStarted = new ExEventLog.EventTuple(1073741831U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpWebServiceRequestCompleted = new ExEventLog.EventTuple(1073741832U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpPowerShellInvoked = new ExEventLog.EventTuple(1073741833U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpPowerShellCompleted = new ExEventLog.EventTuple(1073741834U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpApplicationRequestStarted = new ExEventLog.EventTuple(1073741835U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpApplicationRequestEnded = new ExEventLog.EventTuple(1073741836U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_EcpPerformanceConsoleEnabled = new ExEventLog.EventTuple(2147483661U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_EcpPerformanceIisLogEnabled = new ExEventLog.EventTuple(2147483662U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpPerformanceRecord = new ExEventLog.EventTuple(1073741839U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_EcpPerformanceEventLogMediumEnabled = new ExEventLog.EventTuple(2147483664U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_EcpPerformanceEventLogHighEnabled = new ExEventLog.EventTuple(2147483665U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EcpRedirectCasServer = new ExEventLog.EventTuple(1073741842U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpRedirectCantFindCasServer = new ExEventLog.EventTuple(2147483667U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EcpRedirectCantFindUserMailbox = new ExEventLog.EventTuple(1073741844U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ScriptRequestFailed = new ExEventLog.EventTuple(3221225493U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EcpProxyCantFindCasServer = new ExEventLog.EventTuple(2147483670U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OutboundProxySessionInitialize = new ExEventLog.EventTuple(1073741847U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InboundProxySessionInitialize = new ExEventLog.EventTuple(1073741848U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StandardSessionInitialize = new ExEventLog.EventTuple(1073741849U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorSslTrustFailure = new ExEventLog.EventTuple(3221225498U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorUnauthorized = new ExEventLog.EventTuple(3221225499U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorForbidden = new ExEventLog.EventTuple(3221225500U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorServiceUnavailable = new ExEventLog.EventTuple(3221225501U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyRequestFailed = new ExEventLog.EventTuple(3221225502U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToDetectRbacRoleViaCmdlet = new ExEventLog.EventTuple(3221225503U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EsoStandardSessionInitialize = new ExEventLog.EventTuple(1073741856U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PowershellExceptionTranslated = new ExEventLog.EventTuple(1073741857U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyErrorCASCompatibility = new ExEventLog.EventTuple(3221225506U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UploadRequestStarted = new ExEventLog.EventTuple(1073741859U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UploadRequestCompleted = new ExEventLog.EventTuple(1073741860U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NewCanaryIssued = new ExEventLog.EventTuple(1073741861U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidCanaryDetected = new ExEventLog.EventTuple(3221225510U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResetCanaryInCookie = new ExEventLog.EventTuple(2147483687U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidCanaryInCookieDetected = new ExEventLog.EventTuple(3221225512U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidClientTokenDetected = new ExEventLog.EventTuple(3221225513U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MissingRequiredParameterDetected = new ExEventLog.EventTuple(3221225514U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_BadLinkedInConfiguration = new ExEventLog.EventTuple(3221225515U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LinkedInAuthorizationError = new ExEventLog.EventTuple(3221225516U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LoadReportingschemaFailed = new ExEventLog.EventTuple(3221225517U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_BadFacebookConfiguration = new ExEventLog.EventTuple(3221225518U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AsyncWebRequestStarted = new ExEventLog.EventTuple(1073741871U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AsyncWebRequestEnded = new ExEventLog.EventTuple(1073741872U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AsyncWebRequestFailed = new ExEventLog.EventTuple(3221225521U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AsyncWebRequestFailedInCancel = new ExEventLog.EventTuple(3221225522U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_Office365ShellServiceFailed = new ExEventLog.EventTuple(3221225523U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_Office365NavBarLoadConfigFailed = new ExEventLog.EventTuple(3221225524U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TooManyAsyncTaskInServer = new ExEventLog.EventTuple(1073741877U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TooManyAsyncTaskFromCurrentUser = new ExEventLog.EventTuple(1073741878U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LanguagePackIsNotInstalled = new ExEventLog.EventTuple(2147483703U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurableValueOutOfRange = new ExEventLog.EventTuple(2147483704U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ActivityContextError = new ExEventLog.EventTuple(3221225529U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_Office365NavBarCallServiceTimeout = new ExEventLog.EventTuple(3221225530U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			General = 1,
			Performance,
			Redirect,
			Proxy
		}

		internal enum Message : uint
		{
			EcpStarted = 1073741825U,
			EcpDisposed,
			EcpWebServiceStarted,
			RequestFailed = 3221225476U,
			WebServiceFailed,
			ADTransientFailure,
			EcpWebServiceRequestStarted = 1073741831U,
			EcpWebServiceRequestCompleted,
			EcpPowerShellInvoked,
			EcpPowerShellCompleted,
			EcpApplicationRequestStarted,
			EcpApplicationRequestEnded,
			EcpPerformanceConsoleEnabled = 2147483661U,
			EcpPerformanceIisLogEnabled,
			EcpPerformanceRecord = 1073741839U,
			EcpPerformanceEventLogMediumEnabled = 2147483664U,
			EcpPerformanceEventLogHighEnabled,
			EcpRedirectCasServer = 1073741842U,
			EcpRedirectCantFindCasServer = 2147483667U,
			EcpRedirectCantFindUserMailbox = 1073741844U,
			ScriptRequestFailed = 3221225493U,
			EcpProxyCantFindCasServer = 2147483670U,
			OutboundProxySessionInitialize = 1073741847U,
			InboundProxySessionInitialize,
			StandardSessionInitialize,
			ProxyErrorSslTrustFailure = 3221225498U,
			ProxyErrorUnauthorized,
			ProxyErrorForbidden,
			ProxyErrorServiceUnavailable,
			ProxyRequestFailed,
			UnableToDetectRbacRoleViaCmdlet,
			EsoStandardSessionInitialize = 1073741856U,
			PowershellExceptionTranslated,
			ProxyErrorCASCompatibility = 3221225506U,
			UploadRequestStarted = 1073741859U,
			UploadRequestCompleted,
			NewCanaryIssued,
			InvalidCanaryDetected = 3221225510U,
			ResetCanaryInCookie = 2147483687U,
			InvalidCanaryInCookieDetected = 3221225512U,
			InvalidClientTokenDetected,
			MissingRequiredParameterDetected,
			BadLinkedInConfiguration,
			LinkedInAuthorizationError,
			LoadReportingschemaFailed,
			BadFacebookConfiguration,
			AsyncWebRequestStarted = 1073741871U,
			AsyncWebRequestEnded,
			AsyncWebRequestFailed = 3221225521U,
			AsyncWebRequestFailedInCancel,
			Office365ShellServiceFailed,
			Office365NavBarLoadConfigFailed,
			TooManyAsyncTaskInServer = 1073741877U,
			TooManyAsyncTaskFromCurrentUser,
			LanguagePackIsNotInstalled = 2147483703U,
			ConfigurableValueOutOfRange,
			ActivityContextError = 3221225529U,
			Office365NavBarCallServiceTimeout
		}
	}
}
