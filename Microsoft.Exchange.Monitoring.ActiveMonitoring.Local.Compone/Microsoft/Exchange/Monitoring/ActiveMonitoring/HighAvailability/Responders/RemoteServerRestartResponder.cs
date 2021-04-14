using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders
{
	public class RemoteServerRestartResponder : RemotePowerShellResponder
	{
		internal string ComponentName { get; set; }

		internal string[] ServersInGroup { get; set; }

		internal int MinimumRequiredServers { get; set; }

		internal string[] RemoteTargetServers { get; set; }

		internal int MaxNodesToReboot { get; set; }

		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, ServiceHealthStatus responderTargetState, string componentName, string targetServerName, string serviceName = "Exchange", bool enabled = true)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = RemoteServerRestartResponder.AssemblyPath;
			responderDefinition.TypeName = RemoteServerRestartResponder.TypeName;
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
			responderDefinition.Attributes["RemoteTargetServers"] = targetServerName;
			responderDefinition.Attributes["MinimumRequiredServers"] = -1.ToString();
			responderDefinition.Attributes["MaxNodeToReboot"] = "1";
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, "Dag", RecoveryActionId.RemoteForceReboot, Environment.MachineName, null);
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
			this.MaxNodesToReboot = attributeHelper.GetInt("MaxNodeToReboot", true, 1, null, null);
			this.MinimumRequiredServers = attributeHelper.GetInt("MinimumRequiredServers", false, -1, null, null);
			if (this.MinimumRequiredServers == -1)
			{
				this.MinimumRequiredServers = this.ServersInGroup.Length / 2 + 1;
			}
			string @string = attributeHelper.GetString("RemoteTargetServers", false, null);
			if (string.IsNullOrEmpty(@string))
			{
				this.RemoteTargetServers = null;
				return;
			}
			this.RemoteTargetServers = @string.Split(new string[]
			{
				","
			}, StringSplitOptions.RemoveEmptyEntries);
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes(null);
			if (this.RemoteTargetServers == null || this.RemoteTargetServers.Length < 1)
			{
				base.Result.StateAttribute1 = "RemoteTargetServer is NULL. Responder work is skipped.";
				return;
			}
			if (LocalEndpointManager.IsDataCenter)
			{
				try
				{
					Component component = Component.FindWellKnownComponent(this.ComponentName);
					if (component != null)
					{
						WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "RemoteServerRestartResponder.DoWork: Attempting to perform remote server force reboot (componentName={0})", component.ToString(), null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\RemoteServerRestartResponder.cs", 227);
						string arg = string.Join(",", this.RemoteTargetServers);
						this.RemoteTargetServers = this.RemoteTargetServers.Take(this.MaxNodesToReboot).ToArray<string>();
						base.Result.StateAttribute1 = string.Format("Responder is going to reboot on the following nodes: {0}, list received={1}, max nodes to reboot={2}", string.Join(",", this.RemoteTargetServers), arg, this.MaxNodesToReboot);
						new RecoveryActionRunner(RecoveryActionId.RemoteForceReboot, Environment.MachineName, this, true, cancellationToken, null)
						{
							IsIgnoreResourceName = true
						}.Execute(delegate()
						{
							this.PerformRemoteForceReboot(component.ToString());
						});
						if (this.errorMessage.Count > 0)
						{
							throw new HighAvailabilityMAResponderException(string.Format("Error occurred inside DoWork: {0}", string.Join(Environment.NewLine, this.errorMessage)));
						}
					}
					return;
				}
				catch (Exception ex)
				{
					RemoteServerRestartResponder.LogFailure((this.RemoteTargetServers != null && this.RemoteTargetServers.Length > 0) ? string.Join(",", this.RemoteTargetServers) : "NULL", ex.Message);
					throw ex;
				}
			}
			base.Result.StateAttribute1 = "Responder not running in Datacenter environment, ignored.";
		}

		private void PerformRemoteForceReboot(string componentName)
		{
			RecoveryActionHelper.RunAndMeasure(string.Format("RemoteForceReboot(WorkitemId={0}, ResultId={1}, Component={2}, TargetServer={3})", new object[]
			{
				base.Id,
				base.Result.ResultId,
				componentName,
				(this.RemoteTargetServers != null) ? string.Join(",", this.RemoteTargetServers) : "NULL"
			}), false, ManagedAvailabilityCrimsonEvents.MeasureOperation, delegate
			{
				base.CreateRunspace();
				int num = Math.Max(this.RemoteTargetServers.Length, this.MaxNodesToReboot);
				for (int i = 0; i < num; i++)
				{
					string text = this.RemoteTargetServers[i];
					if (!string.IsNullOrWhiteSpace(text))
					{
						PSCommand pscommand = new PSCommand();
						pscommand.AddScript(string.Format("Request-SetMachinePowerState.ps1 -MachineName {0} -Action Restart -Reason 'Active Monitoring RemoteServerRestartResponder from host {1}'", text, Environment.MachineName), false);
						base.Result.StateAttribute5 = string.Format("Request-SetMachinePowerState.ps1 -MachineName {0} -Action Restart -Reason 'Active Monitoring RemoteServerRestartResponder from host {1}'", text, Environment.MachineName);
						try
						{
							Collection<PSObject> collection = base.RemotePowerShell.InvokePSCommand(pscommand);
							StringBuilder stringBuilder = new StringBuilder();
							if (collection != null)
							{
								foreach (PSObject psobject in collection)
								{
									stringBuilder.AppendLine(psobject.ToString());
								}
							}
							base.Result.StateAttribute5 = "Execution Result=" + stringBuilder.ToString();
							RemoteServerRestartResponder.LogSuccess(text, stringBuilder.ToString());
						}
						catch (Exception ex)
						{
							this.errorMessage.Add(string.Format("Powershell Invoke exception: Command={0}, Ex={1}", pscommand.Commands[0].CommandText, ex));
							base.Result.StateAttribute5 = string.Format("Powershell Invoke exception: Command={0}, Ex={1}", pscommand.Commands[0].CommandText, ex);
							RemoteServerRestartResponder.LogFailure(text, ex.Message);
						}
					}
				}
				return string.Empty;
			});
		}

		private static void LogSuccess(string target, string additionalMessage)
		{
			int num = 1320;
			object[] eventData = new object[]
			{
				target,
				additionalMessage
			};
			HighAvailabilityUtility.LogEvent("Microsoft-Exchange-HighAvailability/Operational", "Microsoft-Exchange-HighAvailability", (long)num, 7, EventLogEntryType.Information, eventData);
		}

		private static void LogFailure(string target, string additionalMessage)
		{
			int num = 1321;
			object[] eventData = new object[]
			{
				target,
				additionalMessage
			};
			HighAvailabilityUtility.LogEvent("Microsoft-Exchange-HighAvailability/Operational", "Microsoft-Exchange-HighAvailability", (long)num, 7, EventLogEntryType.Warning, eventData);
		}

		private const string LogName = "Microsoft-Exchange-HighAvailability/Operational";

		private const string LogSource = "Microsoft-Exchange-HighAvailability";

		private const int TaskId = 7;

		protected const string SetMachinePowerstateScript = "Request-SetMachinePowerState.ps1 -MachineName {0} -Action Restart -Reason 'Active Monitoring RemoteServerRestartResponder from host {1}'";

		internal readonly TimeSpan CoordinatedQueryDuration = TimeSpan.FromDays(1.0);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(RemoteServerRestartResponder).FullName;

		private readonly List<string> errorMessage = new List<string>();

		internal static class AttributeNames
		{
			internal const string IsAutomaticallyDetectServers = "IsAutomaticallyDetectServers";

			internal const string ComponentName = "ComponentName";

			internal const string MinimumRequiredServers = "MinimumRequiredServers";

			internal const string RemoteTargetServers = "RemoteTargetServers";

			internal const string MaxNodeToReboot = "MaxNodeToReboot";
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
