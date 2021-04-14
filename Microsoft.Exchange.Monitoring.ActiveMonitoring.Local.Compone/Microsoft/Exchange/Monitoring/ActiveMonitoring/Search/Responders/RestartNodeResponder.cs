using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Ceres.CoreServices.Services.HealthCheck;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Responders
{
	public class RestartNodeResponder : ResponderWorkItem
	{
		internal string NodeNames { get; set; }

		internal static ResponderDefinition CreateDefinition(string responderName, string alertMask, ServiceHealthStatus responderTargetState, string nodeNames = "", int nodeStopTimeoutInSeconds = 0, string throttleGroupName = null)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = RestartNodeResponder.AssemblyPath;
			responderDefinition.TypeName = RestartNodeResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.ServiceName = ExchangeComponent.Search.Name;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = alertMask;
			responderDefinition.RecurrenceIntervalSeconds = 0;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.Enabled = true;
			responderDefinition.Attributes["NodeNames"] = ((nodeNames == null) ? string.Empty : nodeNames);
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, throttleGroupName, RecoveryActionId.RestartFastNode, "HostControllerService", null);
			return responderDefinition;
		}

		internal static List<HealthCheckInfo> GetUnhealthyNodes()
		{
			List<HealthCheckInfo> nodeStates = RestartNodeResponder.GetNodeStates();
			return RestartNodeResponder.GetUnhealthyNodes(nodeStates);
		}

		internal static List<HealthCheckInfo> GetUnhealthyNodes(List<HealthCheckInfo> nodeStates)
		{
			List<HealthCheckInfo> list = new List<HealthCheckInfo>();
			foreach (HealthCheckInfo healthCheckInfo in nodeStates)
			{
				if (!NodeManagementClient.Instance.IsNodeHealthy(healthCheckInfo.Name))
				{
					list.Add(healthCheckInfo);
				}
			}
			return list;
		}

		internal static List<HealthCheckInfo> GetNodeStates()
		{
			return new List<HealthCheckInfo>(NodeManagementClient.Instance.GetSystemInfo());
		}

		protected void InitializeServiceAttributes(AttributeHelper attributeHelper)
		{
			this.NodeNames = attributeHelper.GetString("NodeNames", false, string.Empty);
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			SearchMonitoringHelper.LogRecoveryAction(this, "Invoked.", new object[0]);
			Exception ex = null;
			bool flag = false;
			this.InitializeAttributes();
			this.CheckServiceRestartThrottling(cancellationToken);
			List<string> list = null;
			if (!string.IsNullOrWhiteSpace(this.NodeNames))
			{
				list = new List<string>(this.NodeNames.Split(new char[]
				{
					',',
					';'
				}));
			}
			else
			{
				list = new List<string>();
				ProbeResult probeResult = null;
				try
				{
					probeResult = WorkItemResultHelper.GetLastFailedProbeResult(this, base.Broker, cancellationToken);
				}
				catch (Exception ex2)
				{
					SearchMonitoringHelper.LogRecoveryAction(this, "Caught exception reading last failed probe result: '{0}'.", new object[]
					{
						ex2
					});
				}
				if (probeResult != null && !string.IsNullOrWhiteSpace(probeResult.StateAttribute15))
				{
					string[] array = probeResult.StateAttribute15.Split(new char[]
					{
						',',
						';'
					});
					foreach (string text in array)
					{
						string text2 = text.Trim();
						if (SearchMonitoringHelper.FastNodeNames.IsNodeNameValid(text2))
						{
							SearchMonitoringHelper.LogRecoveryAction(this, "Got unhealthy node '{0}' from last failed probe result.", new object[]
							{
								text2
							});
							list.Add(text2);
						}
					}
				}
				if (list.Count == 0)
				{
					try
					{
						List<HealthCheckInfo> unhealthyNodes = RestartNodeResponder.GetUnhealthyNodes();
						foreach (HealthCheckInfo healthCheckInfo in unhealthyNodes)
						{
							SearchMonitoringHelper.LogRecoveryAction(this, "Detected unhealthy node '{0}' with State: '{1}', Description: '{2}'.", new object[]
							{
								healthCheckInfo.Name,
								healthCheckInfo.State,
								healthCheckInfo.Description
							});
							if (healthCheckInfo.State == null)
							{
								flag = true;
							}
							list.Add(healthCheckInfo.Name);
						}
					}
					catch (PerformingFastOperationException ex3)
					{
						SearchMonitoringHelper.LogRecoveryAction(this, "Exception caught getting unhealthy nodes: '{0}'", new object[]
						{
							ex3
						});
						flag = true;
						list.Add("AdminNode1");
						ex = ex3;
					}
				}
			}
			if (flag)
			{
				try
				{
					SearchMonitoringHelper.CleanUpOrphanedWerProcesses();
				}
				catch (Exception ex4)
				{
					SearchMonitoringHelper.LogRecoveryAction(this, "Exception caught cleaning up orphaned WER processes: '{0}'", new object[]
					{
						ex4
					});
					ex = ex4;
				}
			}
			foreach (string text3 in list)
			{
				RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.RestartFastNode, text3, this, true, cancellationToken, null);
				string tmpNodeName = text3;
				try
				{
					recoveryActionRunner.Execute(delegate(RecoveryActionEntry startEntry)
					{
						this.InternalRestartNode(tmpNodeName, cancellationToken);
					});
					if (string.IsNullOrEmpty(base.Result.StateAttribute1))
					{
						base.Result.StateAttribute1 = tmpNodeName;
					}
				}
				catch (ThrottlingRejectedOperationException ex5)
				{
					ex = ex5;
					SearchMonitoringHelper.LogRecoveryAction(this, "Restarting '{0}' is throttled.", new object[]
					{
						text3
					});
				}
			}
			if (ex != null)
			{
				SearchMonitoringHelper.LogRecoveryAction(this, "Failed.", new object[0]);
				throw ex;
			}
			SearchMonitoringHelper.LogRecoveryAction(this, "Completed.", new object[0]);
		}

		protected virtual void InitializeAttributes()
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.InitializeServiceAttributes(attributeHelper);
		}

		private void InternalRestartNode(string nodeName, CancellationToken cancellationToken)
		{
			nodeName = nodeName.Trim();
			if (!SearchMonitoringHelper.FastNodeNames.IsNodeNameValid(nodeName))
			{
				throw new ArgumentException("nodeName");
			}
			SearchMonitoringHelper.LogRecoveryAction(this, "Restarting node '{0}'.", new object[]
			{
				nodeName
			});
			try
			{
				NodeManagementClient.Instance.KillAndRestartNode(nodeName);
			}
			catch (Exception ex)
			{
				SearchMonitoringHelper.LogRecoveryAction(this, "Restarting node '{0}' failed with exception: '{1}'.", new object[]
				{
					nodeName,
					ex
				});
				throw ex;
			}
			SearchMonitoringHelper.LogRecoveryAction(this, "Restarting node '{0}' completed.", new object[]
			{
				nodeName
			});
		}

		private void CheckServiceRestartThrottling(CancellationToken cancellationToken)
		{
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.RestartService, "HostControllerService", this, true, cancellationToken, null);
			try
			{
				recoveryActionRunner.VerifyThrottleLimitsNotExceeded();
			}
			catch (ThrottlingRejectedOperationException)
			{
				SearchMonitoringHelper.LogRecoveryAction(this, "Throttled by Host Controller Service restart.", new object[0]);
				throw;
			}
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(RestartNodeResponder).FullName;

		internal static class AttributeNames
		{
			internal const string NodeNames = "NodeNames";

			internal const string NodeStopTimeoutSeconds = "NodeStopTimeoutSeconds";

			internal const string throttleGroupName = "throttleGroupName";
		}

		internal static class DefaultValues
		{
			internal const string NodeNames = "";
		}
	}
}
