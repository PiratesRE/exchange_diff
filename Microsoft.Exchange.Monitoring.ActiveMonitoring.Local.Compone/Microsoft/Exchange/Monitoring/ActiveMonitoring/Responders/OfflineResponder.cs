using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ServiceContextProvider;
using Microsoft.Forefront.RecoveryActionArbiter.Contract;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class OfflineResponder : ResponderWorkItem
	{
		internal string[] ServersInGroup { get; set; }

		internal int MinimumRequiredServers { get; set; }

		internal int MaximumConcurrentOfflines { get; set; }

		internal string FailureReason { get; set; }

		internal string FfoArbitrationScope { get; set; }

		internal string FfoArbitrationSource { get; set; }

		internal string FfoRequestedAction { get; set; }

		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, ServerComponentEnum componentToTakeOffline, ServiceHealthStatus responderTargetState, string serviceName, string[] serversInGroup, int minimumRequiredServers = -1, string failureReason = "", string arbitrationScope = "Datacenter", string arbitrationSource = "F5AvailabilityData", string requestedAction = "MachineOut")
		{
			if (!OfflineResponder.IsSupportedComponent(componentToTakeOffline))
			{
				throw new ArgumentException(string.Format("componentToTakeOffline was passed as '{0}'. This is not a supported componentId", componentToTakeOffline));
			}
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = OfflineResponder.AssemblyPath;
			responderDefinition.TypeName = OfflineResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = monitorName;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = true;
			responderDefinition.Attributes["ComponentToTakeOffline"] = componentToTakeOffline.ToString();
			if (serversInGroup != null)
			{
				responderDefinition.Attributes["ServersInGroup"] = string.Join(';'.ToString(), serversInGroup);
			}
			responderDefinition.Attributes["MinimumRequiredServers"] = minimumRequiredServers.ToString();
			responderDefinition.Attributes["FailureReason"] = failureReason;
			responderDefinition.Attributes["ArbitrationScope"] = arbitrationScope;
			responderDefinition.Attributes["ArbitrationSource"] = arbitrationSource;
			responderDefinition.Attributes["RequestedAction"] = requestedAction;
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, null, RecoveryActionId.TakeComponentOffline, componentToTakeOffline.ToString(), serversInGroup);
			return responderDefinition;
		}

		protected virtual void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			if (attributeHelper == null)
			{
				attributeHelper = new AttributeHelper(base.Definition);
			}
			bool isMandatory = DatacenterRegistry.IsForefrontForOffice();
			this.componentToTakeOffline = attributeHelper.GetEnum<ServerComponentEnum>("ComponentToTakeOffline", true, ServerComponentEnum.None);
			if (this.ServersInGroup == null || this.ServersInGroup.Length == 0)
			{
				this.ServersInGroup = attributeHelper.GetStrings("ServersInGroup", true, null, ';', true);
			}
			this.MinimumRequiredServers = attributeHelper.GetInt("MinimumRequiredServers", false, -1, null, null);
			if (this.MinimumRequiredServers == -1)
			{
				this.MinimumRequiredServers = (this.ServersInGroup.Length + 1) / 2;
			}
			this.FailureReason = attributeHelper.GetString("FailureReason", isMandatory, "");
			this.FfoArbitrationScope = attributeHelper.GetString("ArbitrationScope", false, "Datacenter");
			this.FfoArbitrationSource = attributeHelper.GetString("ArbitrationSource", false, "F5AvailabilityData");
			this.FfoRequestedAction = attributeHelper.GetString("RequestedAction", false, "MachineOut");
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes(null);
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.TakeComponentOffline, this.componentToTakeOffline.ToString(), this, true, cancellationToken, null);
			recoveryActionRunner.Execute(delegate()
			{
				this.PerformOffline();
			});
		}

		private static bool IsSupportedComponent(ServerComponentEnum compId)
		{
			switch (compId)
			{
			case ServerComponentEnum.None:
			case ServerComponentEnum.ServerWideOffline:
			case ServerComponentEnum.ServerWideSettings:
				return false;
			}
			return true;
		}

		private void PerformOffline()
		{
			bool flag = false;
			Exception ex = null;
			try
			{
				if (DatacenterRegistry.IsForefrontForOffice())
				{
					this.RequestFfoApprovalAndTakeOffline();
				}
				else
				{
					this.ArbitrateAcrossServersAndTakeOffline();
				}
				flag = true;
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			finally
			{
				if (!flag)
				{
					WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, string.Format("OfflineResponder: ForcedOnline: {0} (Reason: {1})", this.componentToTakeOffline.ToString(), (ex != null) ? ex.Message : "<none>"), null, "PerformOffline", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\OfflineResponder.cs", 311);
				}
			}
		}

		private void RequestFfoApprovalAndTakeOffline()
		{
			ArbitrationScope scope = (ArbitrationScope)Enum.Parse(typeof(ArbitrationScope), this.FfoArbitrationScope);
			ArbitrationSource source = (ArbitrationSource)Enum.Parse(typeof(ArbitrationSource), this.FfoArbitrationSource);
			RequestedAction requestedAction = (RequestedAction)Enum.Parse(typeof(RequestedAction), this.FfoRequestedAction);
			bool flag = ServiceContextProvider.Instance.RequestApprovalForRecovery(this.componentToTakeOffline.ToString(), scope, source, requestedAction, 0, this.FailureReason, "");
			if (flag)
			{
				ServerComponentStateManager.SetOffline(this.componentToTakeOffline);
				return;
			}
			throw new RequestForFfoApprovalToOfflineFailedException();
		}

		private void ArbitrateAcrossServersAndTakeOffline()
		{
			CoordinatedOfflineAction coordinatedOfflineAction = new CoordinatedOfflineAction(RecoveryActionId.TakeComponentOffline, this.QueryDuration, this.componentToTakeOffline, base.Definition.Name, this.MinimumRequiredServers, this.ServersInGroup);
			TimeSpan arbitrationTimeout = TimeSpan.FromMilliseconds(40000.0);
			coordinatedOfflineAction.InvokeOfflineOnMajority(arbitrationTimeout);
		}

		internal readonly TimeSpan QueryDuration = TimeSpan.FromMinutes(2.0);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(OfflineResponder).FullName;

		private ServerComponentEnum componentToTakeOffline;

		internal static class AttributeNames
		{
			internal const string ServersInGroup = "ServersInGroup";

			internal const string ComponentToTakeOffline = "ComponentToTakeOffline";

			internal const string MinimumRequiredServers = "MinimumRequiredServers";

			internal const string FailureReason = "FailureReason";

			internal const string ArbitrationScope = "ArbitrationScope";

			internal const string ArbitrationSource = "ArbitrationSource";

			internal const string RequestedAction = "RequestedAction";
		}

		internal static class DefaultValues
		{
			internal const int MinimumRequiredServers = -1;

			internal const string FailureReason = "";

			internal const string ArbitrationScope = "Datacenter";

			internal const string ArbitrationSource = "F5AvailabilityData";

			internal const string RequestedAction = "MachineOut";
		}
	}
}
