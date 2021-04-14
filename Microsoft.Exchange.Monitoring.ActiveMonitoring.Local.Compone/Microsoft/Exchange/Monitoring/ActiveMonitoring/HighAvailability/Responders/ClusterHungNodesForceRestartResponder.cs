using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders
{
	public class ClusterHungNodesForceRestartResponder : RemoteServerRestartResponder
	{
		private int MaxRebootsBeforeHammerDown { get; set; }

		private int SuppressionPeriodInSeconds { get; set; }

		private int HistoryLifeTimeInMinutes { get; set; }

		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, ServiceHealthStatus responderTargetState, string componentName, string serviceName = "Exchange", int suppressionPeriod = 60, int maxRebootBeforeHammerDown = 1, int maxHistoryLifeTimeInMins = 1440, int maxNodesToReboot = 1, bool enabled = true)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = ClusterHungNodesForceRestartResponder.AssemblyPath;
			responderDefinition.TypeName = ClusterHungNodesForceRestartResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = monitorName;
			responderDefinition.RecurrenceIntervalSeconds = 60;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = enabled;
			responderDefinition.Attributes["ComponentName"] = componentName;
			responderDefinition.Attributes["MinimumRequiredServers"] = -1.ToString();
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, "Dag", RecoveryActionId.RemoteForceReboot, Environment.MachineName, null);
			responderDefinition.Attributes["MaxNodeToReboot"] = maxNodesToReboot.ToString();
			responderDefinition.Attributes["SuppressionPeriod"] = suppressionPeriod.ToString();
			responderDefinition.Attributes["MaxTrialOfReboot"] = maxRebootBeforeHammerDown.ToString();
			responderDefinition.Attributes["HistoryLifeTime"] = maxHistoryLifeTimeInMins.ToString();
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.PopulateExtraAttributes();
			ProbeResult lastFailedProbeResult = WorkItemResultHelper.GetLastFailedProbeResult(this, base.Broker, cancellationToken);
			if (lastFailedProbeResult != null)
			{
				if (string.IsNullOrWhiteSpace(lastFailedProbeResult.StateAttribute1))
				{
					base.Result.StateAttribute2 = "RemoteTargetServer is null or empty. Not doing anything...";
					return;
				}
				string text = lastFailedProbeResult.StateAttribute1;
				if (text.Contains(','))
				{
					text = text.Split(new char[]
					{
						','
					}).FirstOrDefault<string>();
				}
				base.Definition.Attributes["RemoteTargetServers"] = text;
				base.Result.StateAttribute2 = string.Format("RemoteTargetServer (Failed node) = {0}. Server Passed in = {1}", lastFailedProbeResult.StateAttribute1, text);
				ClusterHungNodesForceRestartResponder.StrikeAction strikeAction = ClusterHungNodesForceRestartResponder.StrikeAction.Reboot;
				DateTime utcNow = DateTime.UtcNow;
				this.ClearStrikeHistory(text);
				List<ClusterHungNodesForceRestartResponder.StrikeHistory> list = this.RetrieveStrikeHistory(text);
				StringBuilder stringBuilder = new StringBuilder();
				if (list != null && list.Count > 0)
				{
					foreach (ClusterHungNodesForceRestartResponder.StrikeHistory strikeHistory in list)
					{
						stringBuilder.AppendLine(string.Format("{0}-{1}", strikeHistory.Timestamp, strikeHistory.Action));
					}
				}
				base.Result.StateAttribute4 = "Strike History - " + stringBuilder.ToString();
				if (list.Count > 0)
				{
					ClusterHungNodesForceRestartResponder.StrikeHistory strikeHistory2 = list.Last<ClusterHungNodesForceRestartResponder.StrikeHistory>();
					if (utcNow - strikeHistory2.Timestamp < TimeSpan.FromSeconds((double)this.SuppressionPeriodInSeconds))
					{
						base.Result.StateAttribute5 = string.Format("Last recorded strike for {0} was done at {1} (action={2}). Ignoring the current one since Current time is too close (Time={3}, SuppressionInSec={4})", new object[]
						{
							text,
							strikeHistory2.Timestamp.ToString("o"),
							strikeHistory2.Action,
							utcNow.ToString("o"),
							this.SuppressionPeriodInSeconds
						});
						return;
					}
					if (strikeHistory2.Action == ClusterHungNodesForceRestartResponder.StrikeAction.HammerDown)
					{
						base.Result.StateAttribute5 = string.Format("Last recorded strike was {0} at {1} for {2}. However, server came back and caused clusdb hang. Sending HammerDown signal again.", strikeHistory2.Action, strikeHistory2.Timestamp.ToString("o"), text);
						strikeAction = ClusterHungNodesForceRestartResponder.StrikeAction.HammerDown;
					}
				}
				if (list.Count >= this.MaxRebootsBeforeHammerDown)
				{
					base.Result.StateAttribute5 = string.Format("There are more than {0} strike counts for {1}. Initiating node evict for the cluster hung suspect. (Detail history = {2})", list.Count, text, stringBuilder.ToString());
					strikeAction = ClusterHungNodesForceRestartResponder.StrikeAction.HammerDown;
				}
				ResponderResult result = base.Result;
				result.StateAttribute5 = result.StateAttribute5 + " ACTION To TAKE = " + strikeAction.ToString();
				if (strikeAction == ClusterHungNodesForceRestartResponder.StrikeAction.Reboot)
				{
					try
					{
						base.DoResponderWork(cancellationToken);
						this.AppendStrikeHistory(text, ClusterHungNodesForceRestartResponder.StrikeAction.Reboot);
						return;
					}
					catch (Exception ex)
					{
						this.AppendStrikeHistory(text, ClusterHungNodesForceRestartResponder.StrikeAction.Throttled);
						throw ex;
					}
				}
				if (strikeAction == ClusterHungNodesForceRestartResponder.StrikeAction.HammerDown)
				{
					EventNotificationItem eventNotificationItem = new EventNotificationItem("MSExchangeRepl", "Cluster", "HammerDown", string.Format("HammerDown required for repeatedly offending node. Node='{0}'", text), text, ResultSeverityLevel.Critical);
					eventNotificationItem.Publish(false);
					return;
				}
			}
			else
			{
				base.Result.StateAttribute2 = "Last failed probe result does not exists. Ignored...";
			}
		}

		private List<ClusterHungNodesForceRestartResponder.StrikeHistory> RetrieveStrikeHistory(string serverName)
		{
			List<ClusterHungNodesForceRestartResponder.StrikeHistory> list = new List<ClusterHungNodesForceRestartResponder.StrikeHistory>();
			string value = HighAvailabilityUtility.NonCachedRegReader.GetValue<string>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\StrikeHistory", serverName, string.Empty);
			if (string.IsNullOrWhiteSpace(value))
			{
				return list;
			}
			string[] array = value.Split(new char[]
			{
				';'
			});
			if (array != null && array.Length > 0)
			{
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						'|'
					});
					if (array3 != null && array3.Length == 2)
					{
						list.Add(new ClusterHungNodesForceRestartResponder.StrikeHistory
						{
							Action = (ClusterHungNodesForceRestartResponder.StrikeAction)Enum.Parse(typeof(ClusterHungNodesForceRestartResponder.StrikeAction), array3[0]),
							Timestamp = DateTime.Parse(array3[1]).ToUniversalTime()
						});
					}
				}
			}
			return list;
		}

		private void ClearStrikeHistory(string serverName)
		{
			DateTime now = DateTime.UtcNow;
			List<ClusterHungNodesForceRestartResponder.StrikeHistory> list = this.RetrieveStrikeHistory(serverName);
			if (list != null && list.Count > 0)
			{
				IEnumerable<ClusterHungNodesForceRestartResponder.StrikeHistory> enumerable = from hist in list
				where now - hist.Timestamp <= TimeSpan.FromMinutes((double)this.HistoryLifeTimeInMinutes)
				select hist;
				if (enumerable != null && enumerable.Count<ClusterHungNodesForceRestartResponder.StrikeHistory>() > 0)
				{
					list = enumerable.ToList<ClusterHungNodesForceRestartResponder.StrikeHistory>();
				}
				else
				{
					list.Clear();
				}
			}
			if (list != null && list.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					stringBuilder.Append(list[i].Action.ToString());
					stringBuilder.Append('|');
					stringBuilder.Append(list[i].Timestamp.ToString("o"));
					if (i < list.Count - 1)
					{
						stringBuilder.Append(';');
					}
				}
				HighAvailabilityUtility.RegWriter.SetValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\StrikeHistory", serverName, stringBuilder.ToString());
				return;
			}
			HighAvailabilityUtility.RegWriter.SetValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\StrikeHistory", serverName, string.Empty);
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

		private void PopulateExtraAttributes()
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.MaxRebootsBeforeHammerDown = attributeHelper.GetInt("MaxTrialOfReboot", false, 1, null, null);
			this.SuppressionPeriodInSeconds = attributeHelper.GetInt("SuppressionPeriod", false, 120, null, null);
			this.HistoryLifeTimeInMinutes = attributeHelper.GetInt("HistoryLifeTime", false, 1440, null, null);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ClusterHungNodesForceRestartResponder).FullName;

		internal static class ExtraAttributeNames
		{
			internal const string SuppressionPeriodInSec = "SuppressionPeriod";

			internal const string MaximumTrialOfReboot = "MaxTrialOfReboot";

			internal const string HistoryLifeTime = "HistoryLifeTime";
		}

		public class StrikeHistory
		{
			public ClusterHungNodesForceRestartResponder.StrikeAction Action { get; set; }

			public DateTime Timestamp { get; set; }
		}

		public enum StrikeAction
		{
			Throttled,
			Reboot,
			HammerDown
		}
	}
}
