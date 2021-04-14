using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.ApplicationLogic.Autodiscover;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.RpcClientAccess
{
	internal static class Extensions
	{
		public static ProbeDefinition ConfigureAuthenticationForBackendProbe(this ProbeDefinition definition, MailboxDatabaseInfo dbInfo, bool useServerAuthforBackEndProbes)
		{
			if (Extensions.ShouldUseLiveIdOnBackEnd && !useServerAuthforBackEndProbes)
			{
				return definition.AuthenticateAsUser(dbInfo).TargetBackEndPort();
			}
			return definition.AuthenticateAsCafeServer(dbInfo);
		}

		public static ProbeDefinition AuthenticateAsCafeServer(this ProbeDefinition definition, MailboxDatabaseInfo dbInfo)
		{
			if (Extensions.useBackendAuthenticationHook != null)
			{
				return Extensions.useBackendAuthenticationHook();
			}
			definition.SetAccountCommonAccessToken(Datacenter.IsLiveIDForExchangeLogin(true) ? dbInfo.MonitoringAccountExchangeLoginName : dbInfo.MonitoringAccountWindowsLoginName);
			definition.AccountDisplayName = dbInfo.MonitoringAccountExchangeLoginName;
			definition.SecondaryEndpoint = definition.Endpoint;
			return definition;
		}

		public static ProbeDefinition SetAccountCommonAccessToken(this ProbeDefinition definition, string tokenAccount)
		{
			CommonAccessToken commonAccessToken = null;
			if (Datacenter.IsLiveIDForExchangeLogin(true))
			{
				commonAccessToken = CommonAccessTokenHelper.CreateLiveIdBasic(tokenAccount);
			}
			else
			{
				using (WindowsIdentity windowsIdentity = new WindowsIdentity(tokenAccount))
				{
					commonAccessToken = CommonAccessTokenHelper.CreateWindows(windowsIdentity);
				}
			}
			definition.Account = commonAccessToken.Serialize();
			return definition;
		}

		public static TDefinition SuppressOnFreshBootUntilServiceIsStarted<TDefinition>(this TDefinition definition, string windowsServiceName) where TDefinition : WorkDefinition
		{
			StartupNotification.SetStartupNotificationDefinition(definition, windowsServiceName, Extensions.RoundInterval(Extensions.FreshBootServiceStartGracePeriod.TotalSeconds));
			return definition;
		}

		public static ProbeDefinition AuthenticateAsUser(this ProbeDefinition definition, MailboxDatabaseInfo dbInfo)
		{
			if (string.IsNullOrEmpty(dbInfo.MonitoringAccountPassword))
			{
				throw new InvalidOperationException("Cafe authentication requires a valid monitoring account password");
			}
			definition.Account = dbInfo.MonitoringAccountExchangeLoginName;
			definition.AccountPassword = dbInfo.MonitoringAccountPassword;
			definition.AccountDisplayName = dbInfo.MonitoringAccountExchangeLoginName;
			definition.SecondaryEndpoint = definition.Attributes["PersonalizedServerName"];
			return definition;
		}

		public static ProbeDefinition SetSecondaryEndpointAsPersonalizedServerName(this ProbeDefinition definition, MailboxDatabaseInfo dbInfo)
		{
			definition.SecondaryEndpoint = Extensions.GetPersonalizedServerName(dbInfo);
			return definition;
		}

		public static ProbeDefinition TargetPrimaryMailbox(this ProbeDefinition definition, MailboxDatabaseInfo dbInfo)
		{
			definition.Attributes["AccountLegacyDN"] = dbInfo.MonitoringAccountLegacyDN;
			definition.Attributes["PersonalizedServerName"] = Extensions.GetPersonalizedServerName(dbInfo);
			return definition;
		}

		private static string GetPersonalizedServerName(MailboxDatabaseInfo dbInfo)
		{
			return DirectoryAccessor.Instance.CreatePersonalizedServerName(dbInfo.MonitoringAccountMailboxGuid, dbInfo.MonitoringAccountDomain);
		}

		public static ProbeDefinition TargetArchiveMailbox(this ProbeDefinition definition, MailboxDatabaseInfo dbInfo)
		{
			definition.Attributes["AccountLegacyDN"] = dbInfo.MonitoringAccountLegacyDN;
			definition.Attributes["PersonalizedServerName"] = DirectoryAccessor.Instance.CreatePersonalizedServerName(dbInfo.MonitoringAccountMailboxArchiveGuid, dbInfo.MonitoringAccountDomain);
			string value = DirectoryAccessor.Instance.CreateAlternateMailboxLegDN(dbInfo.MonitoringAccountLegacyDN, dbInfo.MonitoringAccountMailboxArchiveGuid);
			definition.Attributes["MailboxLegacyDN"] = value;
			return definition;
		}

		public static ProbeDefinition ConfigureDeepTest(this ProbeDefinition definition, int numberOfResources)
		{
			TimeSpan timeout = TimeSpan.FromSeconds((double)(numberOfResources * Extensions.ProtocolDeepTestProbeIntervalInSeconds));
			definition.RecurrenceIntervalSeconds = Extensions.RoundInterval(timeout.TotalSeconds);
			definition.TimeoutSeconds = Extensions.RoundInterval(Extensions.AdjustTimeoutForSchedulingOverhead(timeout).TotalSeconds);
			return definition;
		}

		public static ProbeDefinition ConfigureSelfTest(this ProbeDefinition definition)
		{
			TimeSpan probeRecurrenceForConsecutiveFailuresMonitor = Extensions.GetProbeRecurrenceForConsecutiveFailuresMonitor(Extensions.ProtocolSelfTestFailureDetectionTime, 5);
			definition.RecurrenceIntervalSeconds = Extensions.RoundInterval(probeRecurrenceForConsecutiveFailuresMonitor.TotalSeconds);
			definition.TimeoutSeconds = Extensions.RoundInterval(Extensions.AdjustTimeoutForSchedulingOverhead(probeRecurrenceForConsecutiveFailuresMonitor).TotalSeconds);
			return definition;
		}

		public static ProbeDefinition ConfigureCtp(this ProbeDefinition definition, int numberOfResources)
		{
			TimeSpan probeRecurrenceIntervalForSmoothMonitor = Extensions.GetProbeRecurrenceIntervalForSmoothMonitor(Extensions.CtpFailureDetectionTime, numberOfResources);
			definition.RecurrenceIntervalSeconds = Extensions.RoundInterval(probeRecurrenceIntervalForSmoothMonitor.TotalSeconds);
			definition.TimeoutSeconds = Extensions.RoundInterval(Extensions.AdjustTimeoutForSchedulingOverhead(probeRecurrenceIntervalForSmoothMonitor).TotalSeconds);
			definition.Attributes["SiteName"] = DirectoryAccessor.Instance.Server.ServerSite.Name;
			return definition;
		}

		public static ProbeDefinition MakeTemplateForOnDemandExecution(this ProbeDefinition definition)
		{
			definition.RecurrenceIntervalSeconds = 0;
			definition.Enabled = false;
			definition.TimeoutSeconds = Extensions.RoundInterval(Extensions.OnDemandExecutionTimeout.TotalSeconds);
			return definition;
		}

		public static ProbeDefinition ConfigureCtpAuthenticationMethod(this ProbeDefinition definition, AutodiscoverRpcHttpSettings settings)
		{
			RpcProxyPort rpcProxyPort = settings.SslRequired ? RpcProxyPort.Default : RpcProxyPort.LegacyHttp;
			definition.Attributes["RpcProxyPort"] = rpcProxyPort.ToString();
			definition.Attributes["RpcProxyAuthenticationType"] = settings.AuthPackageString;
			return definition;
		}

		public static ProbeDefinition ForceSslCtpAuthenticationMethod(this ProbeDefinition definition)
		{
			definition.Attributes["RpcProxyPort"] = RpcProxyPort.Default.ToString();
			return definition;
		}

		public static ProbeDefinition TargetBackEndPort(this ProbeDefinition definition)
		{
			definition.Attributes["RpcProxyPort"] = RpcProxyPort.Backend.ToString();
			return definition;
		}

		public static MonitorDefinition DelayStateTransitions(this MonitorDefinition monitorDefinition, TimeSpan transitionDelay)
		{
			monitorDefinition.MonitorStateTransitions = (from transition in monitorDefinition.MonitorStateTransitions
			select new MonitorStateTransition(transition.ToState, transition.TransitionTimeout.Add(transitionDelay))).ToArray<MonitorStateTransition>();
			return monitorDefinition;
		}

		public static MonitorDefinition ConfigureMonitorStateTransitions(this MonitorDefinition monitorDefinition, params MonitorStateTransition[] monitorStateTransitions)
		{
			monitorDefinition.MonitorStateTransitions = monitorStateTransitions;
			return monitorDefinition;
		}

		public static MonitorDefinition LimitRespondersTo(this MonitorDefinition monitorDefinition, params ServiceHealthStatus[] supportedStates)
		{
			monitorDefinition.MonitorStateTransitions = (from state in monitorDefinition.MonitorStateTransitions
			where supportedStates.Contains(state.ToState)
			select state).ToArray<MonitorStateTransition>();
			return monitorDefinition;
		}

		private static TimeSpan GetProbeRecurrenceIntervalForSmoothMonitor(TimeSpan detectionTime, int numberOfResources)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(detectionTime.TotalSeconds / 10.0);
			return TimeSpan.FromSeconds((double)numberOfResources * timeSpan.TotalSeconds);
		}

		private static TimeSpan GetProbeRecurrenceForConsecutiveFailuresMonitor(TimeSpan detectionTime, int failureCount)
		{
			return TimeSpan.FromSeconds(detectionTime.TotalSeconds / (double)failureCount);
		}

		private static TimeSpan AdjustTimeoutForSchedulingOverhead(TimeSpan timeout)
		{
			if (timeout < Extensions.MaxAnticipatedSchedulingOverhead)
			{
				throw new ArgumentOutOfRangeException("timeout", "Must be greater than the anticipated scheduling overhead");
			}
			timeout -= Extensions.MaxAnticipatedSchedulingOverhead;
			if (!(timeout > Extensions.MaxTimeOut))
			{
				return timeout;
			}
			return Extensions.MaxTimeOut;
		}

		private static int RoundInterval(double interval)
		{
			return Math.Max(1, (int)interval);
		}

		public const int NumberOfChunksInASmoothMonitor = 5;

		public const int NumberOfProbesPerChunkInASmoothMonitor = 2;

		public static readonly TimeSpan ProtocolSelfTestFailureDetectionTime = TimeSpan.FromMinutes(5.0);

		public static readonly TimeSpan ProtocolDeepTestFailureDetectionTime = TimeSpan.FromMinutes(30.0);

		public static readonly TimeSpan CtpFailureDetectionTime = TimeSpan.FromMinutes(30.0);

		public static readonly TimeSpan OnDemandExecutionTimeout = TimeSpan.FromMinutes(1.0);

		public static readonly TimeSpan FreshBootServiceStartGracePeriod = TimeSpan.FromMinutes(2.0);

		public static readonly TimeSpan MaxAnticipatedSchedulingOverhead = TimeSpan.FromSeconds(2.0);

		public static readonly int ProtocolDeepTestProbeIntervalInSeconds = 60;

		public static double ProtocolDeepTestAvailabilityThreshold = 25.0;

		public static TimeSpan ProtocolDeepTestMonitorRecurrenceInterval = TimeSpan.FromMinutes(5.0);

		public static readonly string MomtComponentName = "RPCClientAccess";

		public static readonly bool IsDatacenter = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled;

		public static readonly bool ShouldUseLiveIdOnBackEnd = Extensions.IsDatacenter;

		private static Func<ProbeDefinition> useBackendAuthenticationHook = null;

		private static TimeSpan MaxTimeOut = TimeSpan.FromMinutes(3.0);
	}
}
