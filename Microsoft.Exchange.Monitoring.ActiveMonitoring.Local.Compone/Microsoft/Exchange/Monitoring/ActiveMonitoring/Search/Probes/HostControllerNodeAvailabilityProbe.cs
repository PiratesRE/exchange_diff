using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Ceres.CoreServices.Services.HealthCheck;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Probes
{
	public class HostControllerNodeAvailabilityProbe : SearchProbeBase
	{
		protected override void InternalDoWork(CancellationToken cancellationToken)
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			List<HealthCheckInfo> list = null;
			Dictionary<string, int> dictionary = null;
			bool @bool = base.AttributeHelper.GetBool("CheckNodeMemory", true, false);
			try
			{
				List<HealthCheckInfo> nodeStates = RestartNodeResponder.GetNodeStates();
				list = RestartNodeResponder.GetUnhealthyNodes(nodeStates);
				this.LogNodeStateChanges(nodeStates, cancellationToken);
			}
			catch (Exception innerException)
			{
				this.LogNodeStateChanges(null, cancellationToken);
				throw new SearchProbeFailureException(Strings.SearchFailToCheckNodeState, innerException);
			}
			if (list.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (HealthCheckInfo healthCheckInfo in list)
				{
					if (!healthCheckInfo.Name.Equals("IndexNode1") || healthCheckInfo.State != 2)
					{
						stringBuilder.AppendFormat("Name: '{0}', State : '{1}', Description: '{2}'.", healthCheckInfo.Name, healthCheckInfo.State, healthCheckInfo.Description);
						stringBuilder.AppendLine();
					}
				}
				if (stringBuilder.Length > 0)
				{
					throw new SearchProbeFailureException(Strings.HostControllerServiceNodeUnhealthy(stringBuilder.ToString()));
				}
			}
			if (!@bool)
			{
				return;
			}
			double @double = attributeHelper.GetDouble("IndexNodePrivateByteLimitGB", true, 20.0, null, null);
			double double2 = attributeHelper.GetDouble("CtsNodePrivateByteLimitGB", true, 4.0, null, null);
			double double3 = attributeHelper.GetDouble("ImsNodePrivateByteLimitGB", true, 4.0, null, null);
			double double4 = attributeHelper.GetDouble("AdminNodePrivateByteLimitGB", true, 2.0, null, null);
			StringBuilder stringBuilder2 = new StringBuilder();
			if (dictionary == null)
			{
				dictionary = SearchMonitoringHelper.GetNodeProcessIds();
			}
			foreach (string text in dictionary.Keys)
			{
				int num = dictionary[text];
				long num2 = 0L;
				Process process = null;
				try
				{
					process = Process.GetProcessById(num);
					num2 = process.PrivateMemorySize64;
				}
				catch (ArgumentException)
				{
					WTFDiagnostics.TraceFunction<int>(ExTraceGlobals.SearchTracer, base.TraceContext, "HostControllerNodeAvailibilityProbe: Failed to find process with Id {0}.", num, null, "InternalDoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Search\\HostControllerAvailibilityProbe.cs", 139);
				}
				finally
				{
					if (process != null)
					{
						process.Dispose();
					}
				}
				string a;
				if (num2 != 0L && (a = text) != null)
				{
					double num3;
					if (!(a == "AdminNode1"))
					{
						if (!(a == "ContentEngineNode1"))
						{
							if (!(a == "IndexNode1"))
							{
								if (!(a == "InteractionEngineNode1"))
								{
									continue;
								}
								num3 = double3;
							}
							else
							{
								num3 = @double;
							}
						}
						else
						{
							num3 = double2;
						}
					}
					else
					{
						num3 = double4;
					}
					if ((double)num2 > num3 * 1024.0 * 1024.0 * 1024.0)
					{
						stringBuilder2.Append(Strings.HostControllerServiceNodeExcessivePrivateBytesDetails(text, num3, num2));
						stringBuilder2.AppendLine();
						ProbeResult result = base.Result;
						result.StateAttribute15 = result.StateAttribute15 + text + ",";
					}
				}
			}
			if (stringBuilder2.Length > 0)
			{
				throw new SearchProbeFailureException(Strings.HostControllerServiceNodeExcessivePrivateBytes(stringBuilder2.ToString()));
			}
		}

		private void LogNodeStateChanges(List<HealthCheckInfo> nodeStates, CancellationToken cancellationToken)
		{
			ProbeResult lastProbeResult = SearchMonitoringHelper.GetLastProbeResult(this, base.Broker, cancellationToken);
			if (nodeStates == null)
			{
				this.LogNodeStateChange(null, "IndexNode1", lastProbeResult);
				this.LogNodeStateChange(null, "ContentEngineNode1", lastProbeResult);
				this.LogNodeStateChange(null, "InteractionEngineNode1", lastProbeResult);
				this.LogNodeStateChange(null, "AdminNode1", lastProbeResult);
				return;
			}
			foreach (HealthCheckInfo healthCheckInfo in nodeStates)
			{
				this.LogNodeStateChange(healthCheckInfo, healthCheckInfo.Name, lastProbeResult);
			}
		}

		private void LogNodeStateChange(HealthCheckInfo currentNodeState, string nodeName, ProbeResult lastProbeResult)
		{
			string text = null;
			string text2 = (currentNodeState == null) ? "Unknown" : currentNodeState.State.ToString();
			string text3 = (currentNodeState == null) ? string.Empty : currentNodeState.Description;
			if (nodeName != null)
			{
				if (!(nodeName == "IndexNode1"))
				{
					if (!(nodeName == "ContentEngineNode1"))
					{
						if (!(nodeName == "InteractionEngineNode1"))
						{
							if (nodeName == "AdminNode1")
							{
								text = ((lastProbeResult == null) ? null : lastProbeResult.StateAttribute5);
								base.Result.StateAttribute5 = text2;
							}
						}
						else
						{
							text = ((lastProbeResult == null) ? null : lastProbeResult.StateAttribute4);
							base.Result.StateAttribute4 = text2;
						}
					}
					else
					{
						text = ((lastProbeResult == null) ? null : lastProbeResult.StateAttribute3);
						base.Result.StateAttribute3 = text2;
					}
				}
				else
				{
					text = ((lastProbeResult == null) ? null : lastProbeResult.StateAttribute2);
					base.Result.StateAttribute2 = text2;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				SearchMonitoringHelper.LogNodeStateChange("'{0}' now has State: '{1}', Description: '{2}'", new object[]
				{
					nodeName,
					text2,
					text3
				});
				return;
			}
			if (text2 != text)
			{
				SearchMonitoringHelper.LogNodeStateChange("'{0}' now has State: '{1}', Description: '{2}'. The previous State at {3} was: '{4}'", new object[]
				{
					nodeName,
					text2,
					text3,
					lastProbeResult.ExecutionStartTime.ToString("u"),
					text
				});
			}
		}

		private const double DefaultIndexNodePrivateByteLimitGB = 20.0;

		private const double DefaultCtsNodePrivateByteLimitGB = 4.0;

		private const double DefaultImsNodePrivateByteLimitGB = 4.0;

		private const double DefaultAdminNodePrivateByteLimitGB = 2.0;

		private const int DefaultIndexNodeLifeTimeMinuteThreshold = 15;
	}
}
