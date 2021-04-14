using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.RPCHTTP.Messages
{
	public static class MSExchangeRPCHTTPAutoconfigEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransientException = new ExEventLog.EventTuple(3221227473U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PermanentException = new ExEventLog.EventTuple(3221227474U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyNotInstalled = new ExEventLog.EventTuple(3221227475U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WebSiteNotFound = new ExEventLog.EventTuple(3221227476U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WebSiteNotUnique = new ExEventLog.EventTuple(3221227477U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AppCmdNotFound = new ExEventLog.EventTuple(3221227478U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AppCmdRunFailure = new ExEventLog.EventTuple(3221227479U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyNotEnabled = new ExEventLog.EventTuple(3221227480U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FileNotAccessible = new ExEventLog.EventTuple(3221489625U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WebSitesNotConfigured = new ExEventLog.EventTuple(3221227482U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedValidPorts = new ExEventLog.EventTuple(1073744824U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DisabledValidPorts = new ExEventLog.EventTuple(1073744825U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedNSPIKey = new ExEventLog.EventTuple(2147486650U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_Obsolete_UpdatedAuthFlags = new ExEventLog.EventTuple(1073744827U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedHTTPErrors = new ExEventLog.EventTuple(1073744828U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedAllowAnon = new ExEventLog.EventTuple(1073744829U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EnabledValidPorts = new ExEventLog.EventTuple(1073744830U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedConcurrentRequestLimit = new ExEventLog.EventTuple(1073744831U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EnabledClientAccessServers = new ExEventLog.EventTuple(1073744832U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedClientAccessServers = new ExEventLog.EventTuple(1073744833U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DisabledClientAccessServers = new ExEventLog.EventTuple(1073744834U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EnabledClientAccessArray = new ExEventLog.EventTuple(1073744835U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedClientAccessArray = new ExEventLog.EventTuple(1073744836U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DisabledClientAccessArray = new ExEventLog.EventTuple(1073744837U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RpcHttpLBS_StartSuccess = new ExEventLog.EventTuple(1073744838U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RpcHttpLBS_StartFailure = new ExEventLog.EventTuple(3221228487U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RpcHttpLBS_NotInstalled = new ExEventLog.EventTuple(3221228488U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RpcHttpLBS_StopSuccess = new ExEventLog.EventTuple(1073744841U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RpcHttpLBS_StopFailure = new ExEventLog.EventTuple(2147486666U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EnabledValidPorts_AutoGCs = new ExEventLog.EventTuple(1073744843U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedValidPorts_AutoGCs = new ExEventLog.EventTuple(1073744844U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DisabledValidPorts_AutoGCs = new ExEventLog.EventTuple(1073744845U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidRegistryValueType = new ExEventLog.EventTuple(3221228494U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedAuthenticationProviders = new ExEventLog.EventTuple(1073744847U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedLBSArbitrationMode = new ExEventLog.EventTuple(1073744848U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedRpcVirtualDirectory = new ExEventLog.EventTuple(1073744849U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedRpcHttpGeneralSettings = new ExEventLog.EventTuple(1073744850U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_RunningIn2010RtmFunctionalMode = new ExEventLog.EventTuple(2147486678U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_RunningIn2010SP1FunctionalMode = new ExEventLog.EventTuple(1073744855U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_RunningIn2010DatacenterFunctionalMode = new ExEventLog.EventTuple(2147486680U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EnabledE15ClientAccessServers = new ExEventLog.EventTuple(1073744857U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedE15ClientAccessServers = new ExEventLog.EventTuple(1073744858U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DisabledE15ClientAccessServers = new ExEventLog.EventTuple(1073744859U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdatedAuthenticationSettings = new ExEventLog.EventTuple(1073744860U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RpcHttpServiceletSuccessfullyCheckedForUpdatedSettings = new ExEventLog.EventTuple(1073744861U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceAccountNoWorkingCredentials = new ExEventLog.EventTuple(3221229472U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceAccountKerberosError = new ExEventLog.EventTuple(3221229473U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceAccountCredentialException = new ExEventLog.EventTuple(3221229474U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceAccountCredentialsAdded = new ExEventLog.EventTuple(1073745827U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServiceAccountCredentialsRemoved = new ExEventLog.EventTuple(1073745828U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			General = 1,
			ServiceAccount
		}

		internal enum Message : uint
		{
			TransientException = 3221227473U,
			PermanentException,
			ProxyNotInstalled,
			WebSiteNotFound,
			WebSiteNotUnique,
			AppCmdNotFound,
			AppCmdRunFailure,
			ProxyNotEnabled,
			FileNotAccessible = 3221489625U,
			WebSitesNotConfigured = 3221227482U,
			UpdatedValidPorts = 1073744824U,
			DisabledValidPorts,
			UpdatedNSPIKey = 2147486650U,
			Obsolete_UpdatedAuthFlags = 1073744827U,
			UpdatedHTTPErrors,
			UpdatedAllowAnon,
			EnabledValidPorts,
			UpdatedConcurrentRequestLimit,
			EnabledClientAccessServers,
			UpdatedClientAccessServers,
			DisabledClientAccessServers,
			EnabledClientAccessArray,
			UpdatedClientAccessArray,
			DisabledClientAccessArray,
			RpcHttpLBS_StartSuccess,
			RpcHttpLBS_StartFailure = 3221228487U,
			RpcHttpLBS_NotInstalled,
			RpcHttpLBS_StopSuccess = 1073744841U,
			RpcHttpLBS_StopFailure = 2147486666U,
			EnabledValidPorts_AutoGCs = 1073744843U,
			UpdatedValidPorts_AutoGCs,
			DisabledValidPorts_AutoGCs,
			InvalidRegistryValueType = 3221228494U,
			UpdatedAuthenticationProviders = 1073744847U,
			UpdatedLBSArbitrationMode,
			UpdatedRpcVirtualDirectory,
			UpdatedRpcHttpGeneralSettings,
			RunningIn2010RtmFunctionalMode = 2147486678U,
			RunningIn2010SP1FunctionalMode = 1073744855U,
			RunningIn2010DatacenterFunctionalMode = 2147486680U,
			EnabledE15ClientAccessServers = 1073744857U,
			UpdatedE15ClientAccessServers,
			DisabledE15ClientAccessServers,
			UpdatedAuthenticationSettings,
			RpcHttpServiceletSuccessfullyCheckedForUpdatedSettings,
			ServiceAccountNoWorkingCredentials = 3221229472U,
			ServiceAccountKerberosError,
			ServiceAccountCredentialException,
			ServiceAccountCredentialsAdded = 1073745827U,
			ServiceAccountCredentialsRemoved
		}
	}
}
