using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders
{
	public class ClusterNodeShutdownEvictResponder : ResponderWorkItem
	{
		internal string ComponentName { get; set; }

		internal string[] ServersInGroup { get; set; }

		internal int MinimumRequiredServers { get; set; }

		internal string RemoteTargetServer { get; set; }

		internal int MaximumNumberOfNodesDown { get; set; }

		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, ServiceHealthStatus responderTargetState, string componentName, string serviceName = "Exchange", bool enabled = true)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = ClusterNodeShutdownEvictResponder.AssemblyPath;
			responderDefinition.TypeName = ClusterNodeShutdownEvictResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = monitorName;
			responderDefinition.RecurrenceIntervalSeconds = 900;
			responderDefinition.TimeoutSeconds = 600;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = enabled;
			responderDefinition.Attributes["ComponentName"] = componentName;
			responderDefinition.Attributes["MinimumRequiredServers"] = -1.ToString();
			responderDefinition.Attributes["MaxNodeDownAllowed"] = 3.ToString();
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, "Dag", RecoveryActionId.ClusterNodeHammerDown, Environment.MachineName, null);
			return responderDefinition;
		}

		internal static void SetActiveMonitoringCertificateSettings(ResponderDefinition definition)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(ClusterNodeShutdownEvictResponder.ActiveMonitoringRegistryPath, false))
			{
				if (registryKey != null)
				{
					string text;
					if (definition.Account == null && (text = (string)registryKey.GetValue("RPSCertificateSubject", null)) != null)
					{
						definition.Account = text;
					}
					if (definition.Endpoint == null && (text = (string)registryKey.GetValue("RPSEndpoint", null)) != null)
					{
						definition.Endpoint = text;
					}
				}
			}
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
				this.MinimumRequiredServers = this.ServersInGroup.Length - 2;
			}
			this.MaximumNumberOfNodesDown = attributeHelper.GetInt("MaxNodeDownAllowed", true, 3, null, null);
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes(null);
			this.RemoteTargetServer = null;
			ProbeResult lastFailedProbeResult = WorkItemResultHelper.GetLastFailedProbeResult(this, base.Broker, cancellationToken);
			if (lastFailedProbeResult == null)
			{
				base.Result.StateAttribute1 = "Unable to get Target server from last failed probe result.";
				return;
			}
			this.RemoteTargetServer = lastFailedProbeResult.StateAttribute1;
			if (this.RemoteTargetServer.Contains(','))
			{
				this.RemoteTargetServer = this.RemoteTargetServer.Split(new char[]
				{
					','
				}).FirstOrDefault<string>();
			}
			if (string.IsNullOrWhiteSpace(this.RemoteTargetServer))
			{
				base.Result.StateAttribute1 = "RemoteTargetServer is NULL. Responder work is skipped.";
				return;
			}
			if (base.Broker.IsLocal() && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.EscalateResponder.Enabled)
			{
				try
				{
					Component component = Component.FindWellKnownComponent(this.ComponentName);
					if (component != null)
					{
						WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "ClusterNodeShutdownEvictResponder.DoWork: Attempting to perform remote server shutdown (componentName={0})", component.ToString(), null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Responders\\ClusterNodeShutdownEvictResponder.cs", 272);
						base.Result.StateAttribute1 = string.Format("Responder is going to restart the following nodes: {0}", string.Join(",", new string[]
						{
							this.RemoteTargetServer
						}));
						new RecoveryActionRunner(RecoveryActionId.ClusterNodeHammerDown, Environment.MachineName, this, true, cancellationToken, null)
						{
							IsIgnoreResourceName = true
						}.Execute(delegate()
						{
							this.AppendStrikeHistory(this.RemoteTargetServer, ClusterHungNodesForceRestartResponder.StrikeAction.HammerDown);
							this.Result.StateAttribute5 = string.Format("Trying to restart node '{0}'...", this.RemoteTargetServer);
							this.PerformRemoteForceReboot(component.ToString());
							this.Result.StateAttribute5 = string.Format("Waiting till node '{0}' unpingable", this.RemoteTargetServer);
							this.WaitUntilPingFailed(this.RemoteTargetServer, TimeSpan.FromMinutes(5.0));
							this.Result.StateAttribute5 = "Checking if there are enough nodes up in the DAG...";
							this.CheckClusterNodeCount();
							this.Result.StateAttribute5 = string.Format("Trying to evict node '{0}' from cluster", this.RemoteTargetServer);
							this.EvictNode(this.RemoteTargetServer);
							this.Result.StateAttribute5 = string.Format("HammerDown completed for node '{0}'", this.RemoteTargetServer);
							EventNotificationItem eventNotificationItem = new EventNotificationItem("ExCapacity", "NodeEvicted", "RepeatedlyOffendingNode", string.Format("Node '{0}' is a repeated offender causing cluster hang. Node is evicted from cluster and require assistance from Capacity team to bring the server offline before root cause identified.", this.RemoteTargetServer), this.RemoteTargetServer, ResultSeverityLevel.Critical);
							eventNotificationItem.Publish(false);
							HighAvailabilityUtility.LogEvent("Microsoft-Exchange-HighAvailability/Operational", "Microsoft-Exchange-HighAvailability", 1322L, 7, EventLogEntryType.Information, new object[]
							{
								this.RemoteTargetServer
							});
						});
						if (!string.IsNullOrWhiteSpace(this.errorMessage))
						{
							throw new HighAvailabilityMAResponderException(string.Format("Error occurred inside DoWork: {0}", string.Join(Environment.NewLine, new string[]
							{
								this.errorMessage
							})));
						}
					}
					return;
				}
				catch (Exception ex)
				{
					HighAvailabilityUtility.LogEvent("Microsoft-Exchange-HighAvailability/Operational", "Microsoft-Exchange-HighAvailability", 1325L, 7, EventLogEntryType.Error, new object[]
					{
						this.RemoteTargetServer,
						ex.ToString()
					});
					throw ex;
				}
			}
			base.Result.StateAttribute1 = "Responder not running in Datacenter environment, ignored.";
		}

		private void CheckClusterNodeCount()
		{
			IADDatabaseAvailabilityGroup localDAG = CachedAdReader.Instance.LocalDAG;
			int count = localDAG.Servers.Count;
			Dictionary<string, AmNodeState> source = LatencyChecker.QueryClusterNodeStatus(TimeSpan.FromMinutes(2.0), false);
			IEnumerable<bool> enumerable = from node in source
			select node.Value != AmNodeState.Up && !node.Key.Equals(this.RemoteTargetServer, StringComparison.OrdinalIgnoreCase);
			int num = (enumerable == null) ? 0 : enumerable.Count<bool>();
			if (num < 1 || count - num > this.MaximumNumberOfNodesDown || num <= (int)Math.Round((double)count / 2.0))
			{
				string message = string.Format("Unable to evict node due to exceeding Maximum allowed downed nodes (Threshold={0}). Current Up Nodes = {1}, Total = {2}", this.MaximumNumberOfNodesDown, num, count);
				HighAvailabilityUtility.LogEvent("Microsoft-Exchange-HighAvailability/Operational", "Microsoft-Exchange-HighAvailability", 1323L, 7, EventLogEntryType.Error, new object[]
				{
					this.RemoteTargetServer,
					this.MaximumNumberOfNodesDown.ToString(),
					num.ToString(),
					count.ToString()
				});
				throw new HighAvailabilityMAResponderException(message);
			}
			string.Format("Cluster up node check passed. Threshold = {0}, Current Up Nodes = {1}, Total = {2}", this.MaximumNumberOfNodesDown, num, count);
			HighAvailabilityUtility.LogEvent("Microsoft-Exchange-HighAvailability/Operational", "Microsoft-Exchange-HighAvailability", 1324L, 7, EventLogEntryType.Information, new object[]
			{
				this.RemoteTargetServer,
				this.MaximumNumberOfNodesDown.ToString(),
				num.ToString(),
				count.ToString()
			});
		}

		private void EvictNode(string targetNetbiosName)
		{
			try
			{
				using (PowerShell powerShell = PowerShell.Create())
				{
					PSCommand pscommand = new PSCommand();
					pscommand.Clear();
					pscommand.AddCommand("Import-Module").AddParameter("Name", "FailoverClusters");
					this.RunPSCommand(powerShell, pscommand, false);
					pscommand.Clear();
					pscommand.AddCommand("Remove-ClusterNode").AddParameter("Name", targetNetbiosName).AddParameter("Force", true).AddParameter("Wait", 30).AddParameter("Confirm", false);
					this.RunPSCommand(powerShell, pscommand, false);
				}
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute4 = string.Format("Ex caught in Evict - {0}", ex.ToString());
			}
		}

		private Collection<PSObject> RunPSCommand(PowerShell ps, PSCommand cmd, bool asString)
		{
			ps.Streams.ClearStreams();
			ps.Commands = cmd;
			if (asString)
			{
				cmd.AddCommand("Out-String");
				ps.Commands = cmd;
			}
			Collection<PSObject> result = null;
			try
			{
				result = ps.Invoke();
			}
			catch (CommandNotFoundException ex)
			{
				throw new HighAvailabilityMAResponderException(ex.ToString());
			}
			if (ps.Streams.Error.Count > 0)
			{
				string psStream = this.GetPsStream<ErrorRecord>(ps.Streams.Error);
				throw new HighAvailabilityMAResponderException(string.Format("Powershell error - {0}", psStream));
			}
			return result;
		}

		private string GetPsStream<T>(PSDataCollection<T> collection)
		{
			string result = null;
			if (collection != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (T t in collection)
				{
					stringBuilder.AppendLine(t.ToString());
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		private void WaitUntilPingFailed(string targetNetbiosName, TimeSpan timeout)
		{
			DateTime utcNow = DateTime.UtcNow;
			PingReply pingReply = null;
			using (Ping ping = new Ping())
			{
				while (DateTime.UtcNow - utcNow < timeout)
				{
					try
					{
						pingReply = ping.Send(targetNetbiosName, 10000);
						if (pingReply.Status != IPStatus.Success)
						{
							break;
						}
						Thread.Sleep(5000);
					}
					catch
					{
					}
				}
			}
			if (pingReply == null || pingReply.Status == IPStatus.Success)
			{
				throw new HighAvailabilityMAResponderException(string.Format("Unable to wait till host '{0}' to be unpingable within {1} seconds.", targetNetbiosName, timeout.TotalSeconds));
			}
		}

		private void PerformRemoteForceReboot(string componentName)
		{
			RecoveryActionHelper.RunAndMeasure(string.Format("RemoteForceReboot(WorkitemId={0}, ResultId={1}, Component={2}, TargetServer={3})", new object[]
			{
				base.Id,
				base.Result.ResultId,
				componentName,
				(this.RemoteTargetServer != null) ? this.RemoteTargetServer : string.Empty
			}), false, ManagedAvailabilityCrimsonEvents.MeasureOperation, delegate
			{
				this.CreateRunspace();
				string remoteTargetServer = this.RemoteTargetServer;
				if (!string.IsNullOrWhiteSpace(remoteTargetServer))
				{
					PSCommand pscommand = new PSCommand();
					pscommand.AddScript(string.Format("Request-SetMachinePowerState.ps1 -MachineName {0} -Action Restart -Reason 'Active Monitoring Hammer Down for Cluster Db Hang'", remoteTargetServer), false);
					base.Result.StateAttribute5 = string.Format("Request-SetMachinePowerState.ps1 -MachineName {0} -Action Restart -Reason 'Active Monitoring Hammer Down for Cluster Db Hang'", remoteTargetServer);
					try
					{
						Collection<PSObject> collection = this.remotePowershell.InvokePSCommand(pscommand);
						StringBuilder stringBuilder = new StringBuilder();
						if (collection != null)
						{
							foreach (PSObject psobject in collection)
							{
								stringBuilder.AppendLine(psobject.ToString());
							}
						}
						base.Result.StateAttribute5 = "Execution Result=" + stringBuilder.ToString();
					}
					catch (Exception arg)
					{
						this.errorMessage = string.Format("Powershell Invoke exception: Command={0}, Ex={1}", pscommand.Commands[0].CommandText, arg);
						base.Result.StateAttribute5 = string.Format("Powershell Invoke exception: Command={0}, Ex={1}", pscommand.Commands[0].CommandText, arg);
					}
				}
				return string.Empty;
			});
		}

		private void CreateRunspace()
		{
			if (base.Definition.Account == null)
			{
				ClusterNodeShutdownEvictResponder.SetActiveMonitoringCertificateSettings(base.Definition);
				base.Result.StateAttribute5 = "No authentication values were defined in ClusterNodeShutdownEvictResponder. Certification settings have now been set.";
			}
			if (!string.IsNullOrWhiteSpace(base.Definition.AccountPassword))
			{
				this.remotePowershell = RemotePowerShell.CreateRemotePowerShellByCredential(new Uri(base.Definition.Endpoint), base.Definition.Account, base.Definition.AccountPassword, true);
				return;
			}
			if (base.Definition.Endpoint.Contains(";"))
			{
				this.remotePowershell = RemotePowerShell.CreateRemotePowerShellByCertificate(base.Definition.Endpoint.Split(new char[]
				{
					';'
				}), base.Definition.Account, true);
				return;
			}
			this.remotePowershell = RemotePowerShell.CreateRemotePowerShellByCertificate(new Uri(base.Definition.Endpoint), base.Definition.Account, true);
		}

		private void AppendStrikeHistory(string serverName, ClusterHungNodesForceRestartResponder.StrikeAction action)
		{
			HighAvailabilityUtility.RegWriter.CreateSubKey(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\StrikeHistory");
			string value = HighAvailabilityUtility.NonCachedRegReader.GetValue<string>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\StrikeHistory", serverName, string.Empty);
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(value))
			{
				stringBuilder.Append(value);
				stringBuilder.Append(';');
			}
			stringBuilder.Append(action.ToString());
			stringBuilder.Append('|');
			stringBuilder.Append(DateTime.UtcNow.ToString("o"));
			HighAvailabilityUtility.RegWriter.SetValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\StrikeHistory", serverName, stringBuilder.ToString());
		}

		private const string LogName = "Microsoft-Exchange-HighAvailability/Operational";

		private const string LogSource = "Microsoft-Exchange-HighAvailability";

		private const int TaskId = 7;

		private const string SetMachinePowerstateScript = "Request-SetMachinePowerState.ps1 -MachineName {0} -Action Restart -Reason 'Active Monitoring Hammer Down for Cluster Db Hang'";

		internal readonly TimeSpan CoordinatedQueryDuration = TimeSpan.FromDays(1.0);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ClusterNodeShutdownEvictResponder).FullName;

		private static readonly string ActiveMonitoringRegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\";

		private string errorMessage;

		private RemotePowerShell remotePowershell;

		internal static class AttributeNames
		{
			internal const string IsAutomaticallyDetectServers = "IsAutomaticallyDetectServers";

			internal const string ComponentName = "ComponentName";

			internal const string MinimumRequiredServers = "MinimumRequiredServers";

			internal const string MaxNodeDownAllowed = "MaxNodeDownAllowed";
		}

		internal static class DefaultValues
		{
			internal const string[] ServersInGroup = null;

			internal const bool IsAutomaticallyDetectServers = true;

			internal const int MinimumRequiredServers = -1;

			internal const int MaxNodeDownAllowed = 3;

			internal const string ServiceName = "Exchange";

			internal const bool Enabled = true;
		}
	}
}
