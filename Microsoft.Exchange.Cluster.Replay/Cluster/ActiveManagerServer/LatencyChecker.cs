using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class LatencyChecker
	{
		private static List<AmServerName> DagServers
		{
			get
			{
				AmLastKnownGoodConfig lastKnownGoodConfig = AmSystemManager.Instance.LastKnownGoodConfig;
				List<AmServerName> result;
				if (lastKnownGoodConfig != null && lastKnownGoodConfig.Members != null)
				{
					result = lastKnownGoodConfig.Members.ToList<AmServerName>();
				}
				else
				{
					result = new List<AmServerName>();
				}
				return result;
			}
		}

		private static Dictionary<AmServerName, IEnumerable<IADDatabase>> DatabaseMap
		{
			get
			{
				return Dependencies.MonitoringADConfigProvider.GetRecentConfig(false).DatabaseMap;
			}
		}

		internal static bool EnableClusterKill { get; set; }

		internal static void WmiKillClussvc(AmServerName nodeName, ExDateTime apiInitiatedTime)
		{
			ObjectQuery query = new ObjectQuery("SELECT * From Win32_Process Where Name='clussvc.exe'");
			string arg = "root\\cimv2";
			ManagementScope managementScope = new ManagementScope(string.Format("\\\\{0}\\{1}", nodeName.NetbiosName, arg));
			managementScope.Connect();
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, query))
			{
				foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
				{
					ManagementObject managementObject = (ManagementObject)managementBaseObject;
					string text = (string)managementObject["CreationDate"];
					ExDateTime exDateTime = ExDateTime.Parse(string.Format("{0}/{1}/{2} {3}:{4}:{5}", new object[]
					{
						text.Substring(0, 4),
						text.Substring(4, 2),
						text.Substring(6, 2),
						text.Substring(8, 2),
						text.Substring(10, 2),
						text.Substring(12, 2)
					}));
					if (exDateTime <= apiInitiatedTime)
					{
						managementObject.InvokeMethod("Terminate", new object[]
						{
							0
						});
					}
					else
					{
						ReplayCrimsonEvents.GenericMessage.Log<string>(string.Format("Clussvc start time is greater than the requested time (ApiTime:{0} ProcessTime:{1}", apiInitiatedTime, exDateTime));
					}
				}
			}
		}

		internal static Dictionary<string, AmNodeState> QueryClusterNodeStatus(TimeSpan timeout, bool logEvent = true)
		{
			Dictionary<string, AmNodeState> states = new Dictionary<string, AmNodeState>(StringComparer.OrdinalIgnoreCase);
			Task task = new Task(delegate()
			{
				try
				{
					using (AmCluster amCluster = AmCluster.Open())
					{
						if (amCluster != null && amCluster.Nodes != null && amCluster.Nodes.Count<IAmClusterNode>() > 0)
						{
							foreach (IAmClusterNode amClusterNode in amCluster.Nodes)
							{
								states[amClusterNode.Name.NetbiosName] = amClusterNode.GetState(false);
							}
						}
					}
				}
				catch (Exception ex)
				{
					ExTraceGlobals.LatencyCheckerTracer.TraceDebug<string>(0L, "Exception caught in QueryClusterNodeStatus, ex={0}", ex.ToString());
				}
			});
			if (logEvent)
			{
				ReplayCrimsonEvents.HungNodeClusterInfoGatherStart.Log<string>(AmServerName.LocalComputerName.NetbiosName);
			}
			task.Start();
			task.Wait(timeout);
			return states;
		}

		internal static LatencyChecker.ClusDbHungInfo GatherHungNodesInformation(LatencyChecker.LatencyContext latencyContext)
		{
			LatencyChecker.ClusDbHungInfo clusDbHungInfo = new LatencyChecker.ClusDbHungInfo();
			TimeSpan timeSpan = ExDateTime.Now - latencyContext.StartTime;
			clusDbHungInfo.ApiName = latencyContext.ApiName;
			clusDbHungInfo.ApiHungStartTime = latencyContext.StartTime;
			ReplayCrimsonEvents.ClusApiOperationAppearsToBeHung.Log<string, ExDateTime, TimeSpan, string, TimeSpan>(latencyContext.ApiName, latencyContext.StartTime, timeSpan, latencyContext.HintStr, latencyContext.MaxAllowedLatency);
			clusDbHungInfo.HungNodeApiException = null;
			try
			{
				ReplayCrimsonEvents.AttemptingToGetHungNodes.Log<string, ExDateTime, LatencyChecker.LatencyContext>(latencyContext.ApiName, latencyContext.StartTime, latencyContext);
				HungNodesInfo nodesHungInClusDbUpdate = HungNodesInfo.GetNodesHungInClusDbUpdate();
				if (nodesHungInClusDbUpdate != null)
				{
					ReplayCrimsonEvents.HungNodeDetectionCompleted.Log<int, AmServerName, HungNodesInfo>(nodesHungInClusDbUpdate.CurrentGumId, nodesHungInClusDbUpdate.CurrentLockOwnerName, nodesHungInClusDbUpdate);
					clusDbHungInfo.CurrentGumId = nodesHungInClusDbUpdate.CurrentGumId;
					clusDbHungInfo.CurrentLockOwnerName = nodesHungInClusDbUpdate.CurrentLockOwnerName;
					clusDbHungInfo.HungNodes = nodesHungInClusDbUpdate.NodeMap.Values.ToArray<AmServerName>();
				}
			}
			catch (HungDetectionGumIdChangedException ex)
			{
				clusDbHungInfo.HungNodeApiException = ex;
				ReplayCrimsonEvents.HungActionSkippedSinceGumIdChanged.Log<int, int, string, long>(ex.LocalGumId, ex.RemoteGumId, ex.LockOwnerName, ex.HungNodesMask);
			}
			catch (OpenClusterTimedoutException ex2)
			{
				clusDbHungInfo.HungNodeApiException = ex2;
				clusDbHungInfo.HungNodes = new AmServerName[]
				{
					new AmServerName(ex2.ServerName)
				};
				ReplayCrimsonEvents.OpenClusterCallHung.Log<string, string, string>(ex2.ServerName, ex2.Message, ex2.Context);
			}
			catch (ClusterException ex3)
			{
				clusDbHungInfo.HungNodeApiException = ex3;
				ReplayCrimsonEvents.HungNodeDetectionFailed.Log<string, string>(ex3.Message, ex3.ToString());
			}
			List<AmServerName> dagServers = LatencyChecker.DagServers;
			ReplayCrimsonEvents.HungNodeRpcScanStart.Log<string>(LatencyChecker.ConvertAmServerNamesToString(dagServers));
			AmMultiNodeCopyStatusFetcher amMultiNodeCopyStatusFetcher = new AmMultiNodeCopyStatusFetcher(dagServers, LatencyChecker.DatabaseMap, RpcGetDatabaseCopyStatusFlags2.None, null, false, 60000);
			amMultiNodeCopyStatusFetcher.GetStatus();
			List<AmServerName> list = new List<AmServerName>();
			List<Exception> list2 = new List<Exception>();
			clusDbHungInfo.ClusterNodesStatus = LatencyChecker.QueryClusterNodeStatus(TimeSpan.FromSeconds(30.0), true);
			foreach (AmServerName amServerName in LatencyChecker.DagServers)
			{
				Exception possibleExceptionForServer = amMultiNodeCopyStatusFetcher.GetPossibleExceptionForServer(amServerName);
				if (possibleExceptionForServer != null)
				{
					if (possibleExceptionForServer is ReplayServiceDownException)
					{
						list.Add(amServerName);
					}
					list2.Add(possibleExceptionForServer);
				}
			}
			clusDbHungInfo.RpcFailedNodes = list.ToArray();
			clusDbHungInfo.RpcExceptions = list2.ToArray();
			ReplayCrimsonEvents.HungNodeInformationLog.Log<string>(clusDbHungInfo.ToString());
			return clusDbHungInfo;
		}

		internal static LatencyChecker.ClusDbHungAction AnalyzeAndSuggestActionForClusDbHang(LatencyChecker.ClusDbHungInfo hungInfo)
		{
			LatencyChecker.ClusDbHungAction clusDbHungAction = new LatencyChecker.ClusDbHungAction();
			clusDbHungAction.HungInfo = hungInfo;
			clusDbHungAction.TakeAction = false;
			clusDbHungAction.TargetNodes = hungInfo.HungNodes;
			clusDbHungAction.Reason = "If you see this message, something is wrong...";
			if (hungInfo.HungNodeApiException != null)
			{
				if (hungInfo.HungNodeApiException is HungDetectionGumIdChangedException)
				{
					clusDbHungAction.TakeAction = false;
					clusDbHungAction.Reason = "GumId changed.";
				}
				else if (hungInfo.HungNodeApiException is OpenClusterTimedoutException)
				{
					clusDbHungAction.TakeAction = true;
					OpenClusterTimedoutException ex = (OpenClusterTimedoutException)hungInfo.HungNodeApiException;
					clusDbHungAction.Reason = string.Format("OpenCluster timed-out for {0}", ex.ServerName);
				}
				else if (hungInfo.HungNodeApiException is ClusterException)
				{
					clusDbHungAction.TakeAction = false;
					clusDbHungAction.Reason = "ClusterException was caught.";
				}
			}
			else
			{
				clusDbHungAction.TakeAction = true;
				clusDbHungAction.Reason = "Hung node detected without any Exceptions caught.";
			}
			if (clusDbHungAction.TargetNodes == null || clusDbHungAction.TargetNodes.Length < 1)
			{
				clusDbHungAction.TakeAction = false;
				clusDbHungAction.Reason = "No hung node detected, and Rpc timeout did not catch anything.";
				if (hungInfo.RpcFailedNodes != null && hungInfo.RpcFailedNodes.Length > 0)
				{
					AmServerName amServerName = null;
					foreach (AmServerName amServerName2 in hungInfo.RpcFailedNodes)
					{
						AmNodeState amNodeState = AmNodeState.Unknown;
						if (hungInfo.ClusterNodesStatus.TryGetValue(amServerName2.NetbiosName, out amNodeState) && amNodeState != AmNodeState.Unknown && amNodeState != AmNodeState.Down)
						{
							amServerName = amServerName2;
							break;
						}
					}
					if (amServerName != null)
					{
						clusDbHungAction.TakeAction = true;
						clusDbHungAction.TargetNodes = new AmServerName[]
						{
							amServerName
						};
						clusDbHungAction.Reason = string.Format("Hung nodes detected via Rpc timeout. Node '{0}' chosen for action. Original list={1}", amServerName.NetbiosName, LatencyChecker.ConvertAmServerNamesToString(hungInfo.RpcFailedNodes));
					}
					else
					{
						clusDbHungAction.TakeAction = false;
						clusDbHungAction.TargetNodes = null;
						clusDbHungAction.Reason = string.Format("No nodes in Rpc non-responsive list are UP according to cluster. Skipping reboot. Original list={0}", LatencyChecker.ConvertAmServerNamesToString(hungInfo.RpcFailedNodes));
					}
				}
				if (!clusDbHungAction.TakeAction && !AmServerName.IsNullOrEmpty(hungInfo.CurrentLockOwnerName))
				{
					clusDbHungAction.TakeAction = true;
					clusDbHungAction.TargetNodes = new AmServerName[]
					{
						hungInfo.CurrentLockOwnerName
					};
					clusDbHungAction.Reason = string.Format("Could not find any hung nodes, so taking restart/reboot action for the lock owner '{0}'", hungInfo.CurrentLockOwnerName.NetbiosName);
				}
			}
			ReplayCrimsonEvents.HungNodeAnalysisResult.Log<string>(clusDbHungAction.ToString());
			return clusDbHungAction;
		}

		internal static void ActOnClusDbHang(LatencyChecker.ClusDbHungAction action)
		{
			if (action != null && action.TakeAction && action.HungInfo != null)
			{
				ReplayCrimsonEvents.HungNodeRecoveryActionStart.Log<string>(LatencyChecker.ConvertAmServerNamesToString(action.TargetNodes));
				bool flag;
				if (RegistryParameters.IsKillClusterServiceOnClusApiHang)
				{
					flag = true;
					if (action.TargetNodes != null && action.TargetNodes.Length > 0)
					{
						AmServerName amServerName = action.TargetNodes[0];
						RpcKillServiceImpl.Reply reply = RpcKillServiceImpl.SendKillRequest(amServerName.Fqdn, "Clussvc", action.HungInfo.ApiHungStartTime.LocalTime, false, RegistryParameters.RpcKillServiceTimeoutInMSec);
						flag = (reply != null && reply.IsSucceeded && reply.IsSucceeded);
					}
				}
				else
				{
					flag = false;
					ReplayCrimsonEvents.SkippedSendingClussvcKillRequest.LogPeriodic(action.HungInfo.ApiName, TimeSpan.FromMinutes(15.0));
				}
				if (!flag)
				{
					string text = LatencyChecker.ConvertAmServerNamesToString(action.TargetNodes);
					ReplayCrimsonEvents.HungNodeRebootRequested.Log<string>(text);
					LatencyChecker.TriggerNodeRestart(action.HungInfo.CurrentGumId.ToString(), (action.HungInfo.CurrentLockOwnerName != null) ? action.HungInfo.CurrentLockOwnerName.NetbiosName : "NULL", text, action.HungInfo, action);
					return;
				}
			}
			else if (action == null || action.HungInfo == null)
			{
				ReplayCrimsonEvents.GenericMessage.Log<string>("ActOnClusDbHang: Action is null or action.HungInfo is null");
			}
		}

		internal static void ReportClusApiHangLongLatency(object context)
		{
			LatencyChecker.LatencyContext latencyContext = (LatencyChecker.LatencyContext)context;
			TimeSpan timeSpan = ExDateTime.Now - latencyContext.StartTime;
			ReplayCrimsonEvents.ClusApiOperationAppearsToBeHungAlert.Log<string, ExDateTime, TimeSpan, string, TimeSpan>(latencyContext.ApiName, latencyContext.StartTime, timeSpan, latencyContext.HintStr, latencyContext.MaxAllowedLatency);
			ReplayEventLogConstants.Tuple_ClusterApiHungAlert.LogEvent(null, new object[]
			{
				latencyContext.ApiName,
				timeSpan.ToString()
			});
			LatencyChecker.RaiseRedEvent();
		}

		internal static void OnClusApiHang(object context)
		{
			LatencyChecker.LatencyContext latencyContext = (LatencyChecker.LatencyContext)context;
			LatencyChecker.ClusDbHungInfo clusDbHungInfo = LatencyChecker.GatherHungNodesInformation(latencyContext);
			LatencyChecker.LastKnownHungInfo = clusDbHungInfo;
			LatencyChecker.ClusDbHungAction action = LatencyChecker.AnalyzeAndSuggestActionForClusDbHang(clusDbHungInfo);
			LatencyChecker.ActOnClusDbHang(action);
		}

		internal static int MeasureClusApi(string apiName, string hintStr, Func<int> func)
		{
			TimeSpan timeSpan;
			return LatencyChecker.Measure(apiName, hintStr, TimeSpan.FromSeconds((double)RegistryParameters.ClusApiLatencyAllowedInSec), TimeSpan.MaxValue, null, func, out timeSpan);
		}

		internal static int MeasureClusApiAndKillIfExceeds(string apiName, string hintStr, Func<int> func)
		{
			TimerCallback latencyCallback = null;
			if (LatencyChecker.EnableClusterKill)
			{
				latencyCallback = new TimerCallback(LatencyChecker.OnClusApiHang);
			}
			TimeSpan currentLatency;
			int result = LatencyChecker.Measure(apiName, hintStr, TimeSpan.FromSeconds((double)RegistryParameters.ClusApiLatencyAllowedInSec), TimeSpan.FromSeconds((double)RegistryParameters.ClusApiHangActionLatencyAllowedInSec), latencyCallback, func, out currentLatency);
			LatencyChecker.RaiseGreenEventIfNeeded(currentLatency);
			return result;
		}

		internal static TimeSpan Measure(string apiName, string hintStr, TimeSpan maxAllowedLatency, Action action)
		{
			TimeSpan result;
			LatencyChecker.Measure(apiName, hintStr, maxAllowedLatency, TimeSpan.MaxValue, null, delegate()
			{
				action();
				return 0;
			}, out result);
			return result;
		}

		internal static int Measure(string apiName, string hintStr, TimeSpan maxAllowedLatency, TimeSpan maxAllowedLatencyForTimer, TimerCallback latencyCallback, Func<int> func, out TimeSpan elapsed)
		{
			ExDateTime now = ExDateTime.Now;
			int num = 0;
			bool flag = true;
			Timer timer = null;
			Timer timer2 = null;
			try
			{
				if (latencyCallback != null && maxAllowedLatencyForTimer.TotalSeconds > 0.0)
				{
					LatencyChecker.LatencyContext latencyContext = new LatencyChecker.LatencyContext(now, apiName, hintStr, maxAllowedLatencyForTimer);
					timer = new Timer(latencyCallback, latencyContext, -1, -1);
					latencyContext.Timer = timer;
					timer.Change(maxAllowedLatencyForTimer, TimeSpan.FromMilliseconds(-1.0));
					timer2 = new Timer(new TimerCallback(LatencyChecker.ReportClusApiHangLongLatency), latencyContext, -1, -1);
					TimeSpan dueTime = TimeSpan.FromSeconds((double)RegistryParameters.ClusApiHangReportLongLatencyDurationInSec);
					timer2.Change(dueTime, TimeSpan.FromMilliseconds(-1.0));
				}
				if (RegistryParameters.IsApiLatencyTestEnabled)
				{
					LatencyChecker.DelayApiIfRequired(apiName);
					num = RegistryParameters.GetApiSimulatedErrorCode(apiName);
					if (num == 0)
					{
						num = func();
					}
					else
					{
						NativeMethods.SetLastError(num);
					}
				}
				else
				{
					num = func();
				}
				flag = false;
			}
			finally
			{
				if (flag)
				{
					num = -1;
				}
				if (timer != null)
				{
					timer.Change(-1, -1);
					timer.Dispose();
				}
				if (timer2 != null)
				{
					timer2.Change(-1, -1);
					timer2.Dispose();
				}
				elapsed = ExDateTime.Now - now;
				ExTraceGlobals.LatencyCheckerTracer.TraceDebug(0L, "Api={0}, StartTime={1}, Elapsed={2}, Hint={3}, IsUnhandled={4}, RetCode={5}, MaxLatency={6}", new object[]
				{
					apiName,
					now,
					elapsed,
					hintStr,
					flag,
					num,
					maxAllowedLatency
				});
				if (elapsed > maxAllowedLatency || (num != 0 && RegistryParameters.GetIsLogApiLatencyFailure()))
				{
					ReplayCrimsonEvents.OperationTookVeryLongTimeToComplete.Log<string, ExDateTime, TimeSpan, string, bool, int, TimeSpan>(apiName, now, elapsed, hintStr, flag, num, maxAllowedLatency);
				}
			}
			return num;
		}

		private static void DelayApiIfRequired(string apiName)
		{
			if (RegistryParameters.IsApiLatencyTestEnabled)
			{
				int num = 0;
				while (!AmSystemManager.Instance.IsShutdown)
				{
					int apiLatencyInSec = RegistryParameters.GetApiLatencyInSec(apiName);
					if (num >= apiLatencyInSec)
					{
						break;
					}
					Thread.Sleep(1000);
					num++;
				}
			}
		}

		private static void TriggerNodeRestart(string currentGumId, string currentLockOwnerName, string hungNodeCsv, LatencyChecker.ClusDbHungInfo hungInfo, LatencyChecker.ClusDbHungAction hungAction)
		{
			EventNotificationItem eventNotificationItem = new EventNotificationItem("MSExchangeRepl", "Cluster", "ClusterNodeRestart", string.Format("Cluster Hung detected. GumId={0}, LockOwner={1}, HungNodes={2}, HungInfo={3}, Decision={4}", new object[]
			{
				currentGumId,
				currentLockOwnerName,
				hungNodeCsv,
				hungInfo.ToString(),
				hungAction.ToString()
			}), hungNodeCsv, ResultSeverityLevel.Critical);
			eventNotificationItem.Publish(false);
		}

		private static void RaiseRedEvent()
		{
			string arg = string.Empty;
			string arg2 = string.Empty;
			if (LatencyChecker.LastKnownHungInfo != null)
			{
				arg = LatencyChecker.ConvertAmServerNamesToString(LatencyChecker.LastKnownHungInfo.HungNodes);
				arg2 = LatencyChecker.ConvertAmServerNamesToString(LatencyChecker.LastKnownHungInfo.RpcFailedNodes);
			}
			new EventNotificationItem("MSExchangeRepl", "Cluster", "ClusterHung", string.Format("ClusDb write timed out. HungNodeInfo={0}", (LatencyChecker.LastKnownHungInfo == null) ? "NULL" : LatencyChecker.LastKnownHungInfo.ToString()), string.Format("HungNodeApi={0}", arg), ResultSeverityLevel.Critical)
			{
				StateAttribute2 = string.Format("RpcHungNode={0}", arg2)
			}.Publish(false);
		}

		private static void RaiseGreenEventIfNeeded(TimeSpan currentLatency)
		{
			if (currentLatency.TotalMilliseconds < 15000.0 && ExDateTime.Now - LatencyChecker.lastGreenEventRaisedTime > TimeSpan.FromMinutes(5.0))
			{
				EventNotificationItem eventNotificationItem = new EventNotificationItem("MSExchangeRepl", "Cluster", "ClusterHung", "ClusDb write completed normally", ((int)currentLatency.TotalMilliseconds).ToString(), ResultSeverityLevel.Informational);
				eventNotificationItem.Publish(false);
				eventNotificationItem = new EventNotificationItem("MSExchangeRepl", "Cluster", "ClusterNodeRestart", "ClusDb write completed normally", ((int)currentLatency.TotalMilliseconds).ToString(), ResultSeverityLevel.Informational);
				eventNotificationItem.Publish(false);
				eventNotificationItem = new EventNotificationItem("MSExchangeRepl", "Cluster", "HammerDown", "ClusDb write completed normally", ((int)currentLatency.TotalMilliseconds).ToString(), ResultSeverityLevel.Informational);
				eventNotificationItem.Publish(false);
				eventNotificationItem = new EventNotificationItem("ExCapacity", "NodeEvicted", "RepeatedlyOffendingNode", "ClusDb write completed normally", ((int)currentLatency.TotalMilliseconds).ToString(), ResultSeverityLevel.Informational);
				eventNotificationItem.Publish(false);
				LatencyChecker.lastGreenEventRaisedTime = ExDateTime.Now;
			}
		}

		private static string ConvertAmServerNamesToString(IEnumerable<AmServerName> servers)
		{
			if (servers != null && servers.Count<AmServerName>() > 0)
			{
				return string.Join(",", from s in servers
				select s.NetbiosName);
			}
			return string.Empty;
		}

		private const string NotificationItemServiceName = "MSExchangeRepl";

		private const string NotificationItemClusterComponentName = "Cluster";

		private const string NotificationItemClusNodeRestartTag = "ClusterNodeRestart";

		private const string NotificationItemClusDbHungTag = "ClusterHung";

		private const string NotificationItemClusHammerDownTag = "HammerDown";

		private const string NotificationItemCapacityServiceName = "ExCapacity";

		private const string NotificationItemCapacityComponentName = "NodeEvicted";

		private const string NotificationItemCapacityTagName = "RepeatedlyOffendingNode";

		private const string NotificationItemClusDbWriteSuccessMessage = "ClusDb write completed normally";

		private const string NotificationItemClusDbWriteFailureMessage = "Cluster Hung detected. GumId={0}, LockOwner={1}, HungNodes={2}, HungInfo={3}, Decision={4}";

		private const string NotificationItemClusDbWriteTimeoutFailureMessage = "ClusDb write timed out. HungNodeInfo={0}";

		private static readonly TimeSpan AdCacheTimeout = TimeSpan.FromMinutes(15.0);

		private static ExDateTime lastGreenEventRaisedTime = ExDateTime.MinValue;

		private static LatencyChecker.ClusDbHungInfo LastKnownHungInfo = null;

		private static ExDateTime adCacheLastUpdateTime = ExDateTime.MinValue;

		internal class LatencyContext
		{
			internal ExDateTime StartTime { get; set; }

			internal string ApiName { get; set; }

			internal string HintStr { get; set; }

			internal TimeSpan MaxAllowedLatency { get; set; }

			internal Timer Timer { get; set; }

			internal LatencyContext(ExDateTime startTime, string apiName, string hintStr, TimeSpan maxAllowedLatency)
			{
				this.StartTime = startTime;
				this.ApiName = apiName;
				this.HintStr = hintStr;
				this.MaxAllowedLatency = maxAllowedLatency;
				this.Timer = null;
			}

			public override string ToString()
			{
				return string.Format("StartTime: '{0}' ApiName: '{1}' HintStr: '{2}' MaxAllowedLatency: '{3}'", new object[]
				{
					this.StartTime.ToString("o"),
					this.ApiName,
					this.HintStr,
					this.MaxAllowedLatency
				});
			}
		}

		internal class ClusDbHungInfo
		{
			internal int CurrentGumId { get; set; }

			internal AmServerName CurrentLockOwnerName { get; set; }

			internal AmServerName[] HungNodes { get; set; }

			internal Exception HungNodeApiException { get; set; }

			internal Dictionary<string, AmNodeState> ClusterNodesStatus { get; set; }

			internal AmServerName[] RpcFailedNodes { get; set; }

			internal Exception[] RpcExceptions { get; set; }

			internal ExDateTime ApiHungStartTime { get; set; }

			internal string ApiName { get; set; }

			public override string ToString()
			{
				string format = "CurrentGumId: '{0}' LockOwner: '{1}' ApiName: '{2}' ApiHungStartTime: '{3}' HungNodes: '{4}' RpcFailedNodes: '{5}' ClusterStatus: '{6}' HungNodeApiEx: '{7}' RpcExs: '{8}'";
				object[] array = new object[9];
				array[0] = this.CurrentGumId;
				array[1] = ((this.CurrentLockOwnerName == null) ? "NULL" : this.CurrentLockOwnerName.NetbiosName);
				array[2] = (string.IsNullOrEmpty(this.ApiName) ? "NULL" : this.ApiName);
				array[3] = this.ApiHungStartTime.ToString("o");
				array[4] = LatencyChecker.ConvertAmServerNamesToString(this.HungNodes);
				array[5] = LatencyChecker.ConvertAmServerNamesToString(this.RpcFailedNodes);
				object[] array2 = array;
				int num = 6;
				string text;
				if (this.ClusterNodesStatus != null && this.ClusterNodesStatus.Count >= 1)
				{
					text = string.Join(",", this.ClusterNodesStatus.Select((KeyValuePair<string, AmNodeState> pair, int sel) => string.Format("{0}={1}", pair.Key, pair.Value)));
				}
				else
				{
					text = "NULL";
				}
				array2[num] = text;
				array[7] = ((this.HungNodeApiException == null) ? "NULL" : this.HungNodeApiException.Message);
				object[] array3 = array;
				int num2 = 8;
				string text2;
				if (this.RpcExceptions != null && this.RpcExceptions.Length >= 1)
				{
					text2 = string.Join(",", from e in this.RpcExceptions
					select e.Message);
				}
				else
				{
					text2 = "NULL";
				}
				array3[num2] = text2;
				return string.Format(format, array);
			}
		}

		internal class ClusDbHungAction
		{
			internal bool TakeAction { get; set; }

			internal AmServerName[] TargetNodes { get; set; }

			internal string Reason { get; set; }

			internal LatencyChecker.ClusDbHungInfo HungInfo { get; set; }

			public override string ToString()
			{
				return string.Format("TakeAction: '{0}' TargetNodes: '{1}' Reason: '{2}'", this.TakeAction, LatencyChecker.ConvertAmServerNamesToString(this.TargetNodes), string.IsNullOrEmpty(this.Reason) ? "NULL" : this.Reason);
			}
		}
	}
}
