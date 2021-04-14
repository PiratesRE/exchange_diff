using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ServiceContextProvider;
using Microsoft.Forefront.RecoveryActionArbiter.Contract;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class ForceRebootServerResponder : ResponderWorkItem
	{
		internal string[] ServersInGroup { get; set; }

		internal int MinimumRequiredServers { get; set; }

		internal string RecoveryId { get; set; }

		internal string FailureReason { get; set; }

		internal string FfoArbitrationScope { get; set; }

		internal string FfoArbitrationSource { get; set; }

		internal string FfoRequestedAction { get; set; }

		internal bool IgnoreGroupThrottlingWhenMajorityNotSucceeded { get; set; }

		internal bool IsServersInGroupNullOrEmpty
		{
			get
			{
				return this.ServersInGroup == null || this.ServersInGroup.Length == 0;
			}
		}

		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, ServiceHealthStatus responderTargetState, string[] serversInGroup = null, int minimumRequiredServers = -1, string recoveryId = "", string failureReason = "", string arbitrationScope = "Datacenter, Stamp", string arbitrationSource = "RecoveryData", string requestedAction = "ArbitrationOnly", string serviceName = "Exchange", bool enabled = true, string throttleGroupName = null, bool isIgnoreGroupThrottlingWhenMajorityNotSucceeded = false)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = ForceRebootServerResponder.AssemblyPath;
			responderDefinition.TypeName = ForceRebootServerResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = monitorName;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = enabled;
			if (serversInGroup != null)
			{
				responderDefinition.Attributes["ServersInGroup"] = AttributeHelper.StringArrayToString(serversInGroup);
			}
			responderDefinition.Attributes["MinimumRequiredServers"] = minimumRequiredServers.ToString();
			responderDefinition.Attributes["RecoveryId"] = recoveryId;
			responderDefinition.Attributes["FailureReason"] = failureReason;
			responderDefinition.Attributes["ArbitrationScope"] = arbitrationScope;
			responderDefinition.Attributes["ArbitrationSource"] = arbitrationSource;
			responderDefinition.Attributes["RequestedAction"] = requestedAction;
			responderDefinition.Attributes["IgnoreGroupThrottling"] = isIgnoreGroupThrottlingWhenMajorityNotSucceeded.ToString();
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, throttleGroupName, RecoveryActionId.ForceReboot, Environment.MachineName, serversInGroup);
			return responderDefinition;
		}

		protected virtual void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			if (attributeHelper == null)
			{
				attributeHelper = new AttributeHelper(base.Definition);
			}
			this.IgnoreGroupThrottlingWhenMajorityNotSucceeded = attributeHelper.GetBool("IgnoreGroupThrottling", false, false);
			bool isMandatory = DatacenterRegistry.IsForefrontForOffice();
			if (this.IsServersInGroupNullOrEmpty)
			{
				this.ServersInGroup = attributeHelper.GetStrings("ServersInGroup", false, null, ';', true);
			}
			int num = this.IsServersInGroupNullOrEmpty ? 0 : this.ServersInGroup.Length;
			this.MinimumRequiredServers = attributeHelper.GetInt("MinimumRequiredServers", false, -1, null, null);
			if (num > 0 && this.MinimumRequiredServers == -1)
			{
				this.MinimumRequiredServers = num / 2 + 1;
			}
			this.RecoveryId = attributeHelper.GetString("RecoveryId", isMandatory, "");
			this.FailureReason = attributeHelper.GetString("FailureReason", isMandatory, "");
			this.FfoArbitrationScope = attributeHelper.GetString("ArbitrationScope", false, "Datacenter, Stamp");
			this.FfoArbitrationSource = attributeHelper.GetString("ArbitrationSource", false, "RecoveryData");
			this.FfoRequestedAction = attributeHelper.GetString("RequestedAction", false, "ArbitrationOnly");
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes(null);
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.ForceReboot, Environment.MachineName, this, true, cancellationToken, null);
			recoveryActionRunner.IgnoreGroupThrottlingWhenMajorityNotSucceeded = this.IgnoreGroupThrottlingWhenMajorityNotSucceeded;
			recoveryActionRunner.IsIgnoreResourceName = true;
			if (!this.IsServersInGroupNullOrEmpty)
			{
				recoveryActionRunner.SetServersInGroup(this.ServersInGroup);
			}
			string[] serversInGroup = recoveryActionRunner.GetServersInGroup();
			recoveryActionRunner.Execute(delegate(RecoveryActionEntry startEntry)
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (instance.WindowsServerRoleEndpoint != null && instance.WindowsServerRoleEndpoint.IsDirectoryServiceRoleInstalled)
				{
					DirectoryGeneralUtils.InternalPutDCInMM(DirectoryGeneralUtils.GetLocalFQDN(), this.TraceContext);
				}
				if (DatacenterRegistry.IsForefrontForOffice())
				{
					this.ArbitrateForFfo();
					return;
				}
				this.ReportBugcheck(startEntry, serversInGroup);
				ForceRebootHelper.PerformBugcheck();
			});
		}

		private void ArbitrateForFfo()
		{
			ArbitrationScope scope = (ArbitrationScope)Enum.Parse(typeof(ArbitrationScope), this.FfoArbitrationScope);
			ArbitrationSource source = (ArbitrationSource)Enum.Parse(typeof(ArbitrationSource), this.FfoArbitrationSource);
			RequestedAction requestedAction = (RequestedAction)Enum.Parse(typeof(RequestedAction), this.FfoRequestedAction);
			bool flag = ServiceContextProvider.Instance.RequestApprovalForRecovery(this.RecoveryId, scope, source, requestedAction, 2, this.FailureReason, "");
			if (flag)
			{
				ForceRebootHelper.PerformBugcheck();
			}
		}

		private void ReportBugcheck(RecoveryActionEntry startEntry, string[] serversInGroup)
		{
			PersistedGlobalActionEntry persistedGlobalActionEntry = new PersistedGlobalActionEntry(startEntry);
			if (!persistedGlobalActionEntry.WriteToFile(TimeSpan.FromSeconds(60.0)))
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.RecoveryActionTracer, base.TraceContext, "PersistedGlobalActionEntry.WriteToFile failed to complete in 60 seconds.", null, "ReportBugcheck", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\ForceRebootServerResponder.cs", 347);
			}
			if (!Utilities.IsSequenceNullOrEmpty<string>(serversInGroup))
			{
				string subkeyName = string.Format("RecoveryAction\\{0}", RecoveryActionId.ForceReboot);
				TimeSpan rpcTimeout = TimeSpan.FromMilliseconds((double)RegistryHelper.GetProperty<int>("LogCrimsonEventRpcTimeoutInMs", 25000, subkeyName, null, false));
				TimeSpan overallTimeout = TimeSpan.FromMilliseconds((double)RegistryHelper.GetProperty<int>("restartReportingTimeoutInMs", 30000, subkeyName, null, false));
				string timeStampStr = DateTime.UtcNow.ToString("o");
				ForceRebootHelper.ReportBugcheckToOtherServersInGroup(serversInGroup, Environment.MachineName, RecoveryActionId.ForceReboot, base.Definition.Name, startEntry.Context ?? string.Empty, timeStampStr, rpcTimeout, overallTimeout);
			}
		}

		internal readonly TimeSpan CoordinatedQueryDuration = TimeSpan.FromDays(1.0);

		internal readonly TimeSpan DurationToWaitToVerifyBugcheck = TimeSpan.FromSeconds(30.0);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ForceRebootServerResponder).FullName;

		internal static class AttributeNames
		{
			internal const string ServersInGroup = "ServersInGroup";

			internal const string MinimumRequiredServers = "MinimumRequiredServers";

			internal const string RecoveryId = "RecoveryId";

			internal const string FailureReason = "FailureReason";

			internal const string ArbitrationScope = "ArbitrationScope";

			internal const string ArbitrationSource = "ArbitrationSource";

			internal const string RequestedAction = "RequestedAction";

			internal const string IgnoreGroupThrottlingWhenMajorityNotSucceeded = "IgnoreGroupThrottling";
		}

		internal static class DefaultValues
		{
			internal const int MinimumRequiredServers = -1;

			internal const string RecoveryId = "";

			internal const string FailureReason = "";

			internal const string ArbitrationScope = "Datacenter, Stamp";

			internal const string ArbitrationSource = "RecoveryData";

			internal const string RequestedAction = "ArbitrationOnly";

			internal const bool IgnoreGroupThrottlingWhenMajorityNotSucceeded = false;

			internal const string ServiceName = "Exchange";

			internal const bool Enabled = true;
		}
	}
}
