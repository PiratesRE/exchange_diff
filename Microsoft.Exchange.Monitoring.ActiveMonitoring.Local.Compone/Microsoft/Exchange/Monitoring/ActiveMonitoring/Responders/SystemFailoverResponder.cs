using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.HA.ManagedAvailability;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class SystemFailoverResponder : ResponderWorkItem
	{
		internal string ComponentName { get; set; }

		internal string[] ServersInGroup { get; set; }

		internal int MinimumRequiredServers { get; set; }

		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, ServiceHealthStatus responderTargetState, string componentName, string serviceName = "Exchange", bool enabled = true)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = SystemFailoverResponder.AssemblyPath;
			responderDefinition.TypeName = SystemFailoverResponder.TypeName;
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
			responderDefinition.Attributes["ComponentName"] = componentName;
			responderDefinition.Attributes["MinimumRequiredServers"] = -1.ToString();
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, "Dag", RecoveryActionId.ServerFailover, Environment.MachineName, null);
			return responderDefinition;
		}

		protected virtual void InitializeAttributes(AttributeHelper attributeHelper = null)
		{
			if (attributeHelper == null)
			{
				attributeHelper = new AttributeHelper(base.Definition);
			}
			this.ComponentName = attributeHelper.GetString("ComponentName", true, null);
			this.ServersInGroup = Dependencies.ThrottleHelper.Settings.GetServersInGroup("Dag");
			this.MinimumRequiredServers = attributeHelper.GetInt("MinimumRequiredServers", false, -1, null, null);
			if (this.MinimumRequiredServers == -1)
			{
				this.MinimumRequiredServers = this.ServersInGroup.Length / 2 + 1;
			}
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes(null);
			if (!RegistryHelper.GetPropertyIntBool("IsSystemFailoverEnabled", true, null, null, false))
			{
				base.Result.StateAttribute1 = "System failover disabled";
				return;
			}
			Component component = Component.FindWellKnownComponent(this.ComponentName);
			if (component != null)
			{
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "SystemFailoverResponder.DoWork: Attempting to perform system failover (componentName={0})", component.ToString(), null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\SystemFailoverResponder.cs", 163);
				new RecoveryActionRunner(RecoveryActionId.ServerFailover, Environment.MachineName, this, true, cancellationToken, null)
				{
					IsIgnoreResourceName = true
				}.Execute(delegate()
				{
					string comment = string.Format("Managed availability system failover initiated by Responder={0} Component={1}.", this.Definition.Name, component.Name);
					this.PerformSystemFailoverAsync(component.ToString(), comment);
				});
			}
		}

		private void PerformSystemFailoverAsync(string componentName, string comment)
		{
			ThreadPool.QueueUserWorkItem(delegate(object unused)
			{
				RecoveryActionHelper.RunAndMeasure(string.Format("SystemFailover(WorkitemId={0}, ResultId={1}, Component={2}, Comment{3})", new object[]
				{
					this.Id,
					this.Result.ResultId,
					componentName,
					comment
				}), false, ManagedAvailabilityCrimsonEvents.MeasureOperation, delegate
				{
					ManagedAvailabilityHelper.PerformSystemFailover(componentName, comment);
					return string.Empty;
				});
			});
		}

		internal readonly TimeSpan CoordinatedQueryDuration = TimeSpan.FromDays(1.0);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(SystemFailoverResponder).FullName;

		internal static class AttributeNames
		{
			internal const string IsAutomaticallyDetectServers = "IsAutomaticallyDetectServers";

			internal const string ComponentName = "ComponentName";

			internal const string MinimumRequiredServers = "MinimumRequiredServers";
		}

		internal static class DefaultValues
		{
			internal const string[] ServersInGroup = null;

			internal const bool IsAutomaticallyDetectServers = true;

			internal const int MinimumRequiredServers = -1;

			internal const string ServiceName = "Exchange";

			internal const bool Enabled = true;
		}
	}
}
