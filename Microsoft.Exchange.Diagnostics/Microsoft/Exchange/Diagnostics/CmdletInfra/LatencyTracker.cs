using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class LatencyTracker
	{
		internal LatencyTracker(string name, Func<IActivityScope> activityScopeGetter)
		{
			this.Name = name;
			this.activityScopeGetter = activityScopeGetter;
		}

		internal string Name { get; private set; }

		internal bool IsRunning
		{
			get
			{
				return this.stopWatch.IsRunning;
			}
		}

		internal long ElapsedMilliseconds
		{
			get
			{
				return this.stopWatch.ElapsedMilliseconds;
			}
		}

		internal void Start()
		{
			this.stopWatch.Start();
		}

		internal long Stop()
		{
			this.stopWatch.Stop();
			return this.stopWatch.ElapsedMilliseconds;
		}

		internal bool StartInternalTracking(string funcName)
		{
			return this.StartInternalTracking(funcName, funcName, false);
		}

		internal bool StartInternalTracking(string groupName, string funcName, bool logDetailsAlways)
		{
			LatencyTracker.LatencyInfo currentLatencyInfo = this.GetCurrentLatencyInfo();
			currentLatencyInfo.LogDetailsAlways = logDetailsAlways;
			currentLatencyInfo.FuncName = funcName;
			currentLatencyInfo.GroupName = groupName;
			string key = LatencyTracker.LatencyInfo.GetKey(groupName, funcName);
			if (this.internalTrackingDic.ContainsKey(key))
			{
				return false;
			}
			if (!this.groupTrackingDic.ContainsKey(groupName))
			{
				this.groupTrackingDic.Add(groupName, funcName);
			}
			this.internalTrackingDic.Add(key, currentLatencyInfo);
			return true;
		}

		internal void EndInternalTracking(string funcName)
		{
			this.EndInternalTracking(funcName, funcName);
		}

		internal void EndInternalTracking(string groupName, string funcName)
		{
			string key = LatencyTracker.LatencyInfo.GetKey(groupName, funcName);
			if (!this.internalTrackingDic.ContainsKey(key))
			{
				return;
			}
			LatencyTracker.LatencyInfo latencyInfo = this.internalTrackingDic[key];
			this.internalTrackingDic.Remove(key);
			LatencyTracker.LatencyInfo latencyInfo2 = this.GetCurrentLatencyInfo() - latencyInfo;
			long elapsedTime = latencyInfo2.ElapsedTime;
			if (this.latencyBreakDowns.ContainsKey(groupName))
			{
				long value = 2L;
				string key2 = groupName + ".C";
				if (this.latencyBreakDowns.ContainsKey(key2))
				{
					value = this.latencyBreakDowns[key2] + 1L;
				}
				this.latencyBreakDowns[key2] = value;
			}
			long num = elapsedTime;
			string strA;
			if (this.groupTrackingDic.TryGetValue(groupName, out strA) && string.Compare(strA, funcName, true) == 0)
			{
				if (this.latencyBreakDowns.ContainsKey(groupName))
				{
					num = this.latencyBreakDowns[groupName] + num;
				}
				this.latencyBreakDowns[groupName] = num;
				this.groupTrackingDic.Remove(groupName);
			}
			string funcNameForDetailedLatencyLogging = this.GetFuncNameForDetailedLatencyLogging(funcName, latencyInfo);
			if (latencyInfo.LogDetailsAlways || elapsedTime >= (long)LoggerSettings.ThresholdToLogActivityLatency)
			{
				if (!string.Equals(funcNameForDetailedLatencyLogging, groupName))
				{
					this.latencyBreakDowns[funcNameForDetailedLatencyLogging] = elapsedTime;
				}
				if (latencyInfo2.ADLatency.Count > 0L)
				{
					this.latencyBreakDowns.Add(funcNameForDetailedLatencyLogging + ".ADC", latencyInfo2.ADLatency.Count);
					this.latencyBreakDowns.Add(funcNameForDetailedLatencyLogging + ".AD", (long)latencyInfo2.ADLatency.TotalMilliseconds);
				}
				if (latencyInfo2.RpcLatency.Count > 0L)
				{
					this.latencyBreakDowns.Add(funcNameForDetailedLatencyLogging + ".RpcC", latencyInfo2.RpcLatency.Count);
					this.latencyBreakDowns.Add(funcNameForDetailedLatencyLogging + ".Rpc", (long)latencyInfo2.RpcLatency.TotalMilliseconds);
				}
				if (latencyInfo2.ADObjToExchObjLatency.Count > 0L)
				{
					this.latencyBreakDowns.Add(funcNameForDetailedLatencyLogging + ".ATEC", latencyInfo2.ADObjToExchObjLatency.Count);
					this.latencyBreakDowns.Add(funcNameForDetailedLatencyLogging + ".ATE", (long)latencyInfo2.ADObjToExchObjLatency.TotalMilliseconds);
				}
			}
		}

		internal void PushLatencyDetailsToLog(Dictionary<string, Enum> knownFuncNameToLogMetadataDic, Action<Enum, double> updateLatencyToLogger, Action<string, string> defaultLatencyLogger)
		{
			if (this.latencyBreakDowns.Count != 0)
			{
				foreach (KeyValuePair<string, long> keyValuePair in this.latencyBreakDowns)
				{
					string key = keyValuePair.Key;
					long value = keyValuePair.Value;
					Enum arg;
					if (knownFuncNameToLogMetadataDic != null && updateLatencyToLogger != null && knownFuncNameToLogMetadataDic.TryGetValue(key, out arg))
					{
						updateLatencyToLogger(arg, (double)value);
					}
					else
					{
						defaultLatencyLogger(key, value.ToString());
					}
				}
				return;
			}
			if (defaultLatencyLogger != null)
			{
				defaultLatencyLogger("LatencyMissed", "latencyBreakDowns.Count is Zero");
				return;
			}
			if (updateLatencyToLogger != null)
			{
				updateLatencyToLogger(RpsCommonMetadata.GenericLatency, 0.0);
			}
		}

		private string GetFuncNameForDetailedLatencyLogging(string funcName, LatencyTracker.LatencyInfo latencyInfoAtStart)
		{
			string text = funcName;
			if (latencyInfoAtStart.GroupName != null && !string.Equals(funcName, latencyInfoAtStart.GroupName))
			{
				text = latencyInfoAtStart.GroupName + "." + text;
			}
			int num;
			if (this.funcNameTrackingDic.TryGetValue(text, out num))
			{
				num++;
			}
			else
			{
				num = 1;
			}
			this.funcNameTrackingDic[text] = num;
			if (num > 1)
			{
				text = text + "$" + num;
			}
			if (!string.Equals(text, funcName) && this.latencyBreakDowns.ContainsKey(text))
			{
				text = Guid.NewGuid().ToString();
			}
			return text;
		}

		private LatencyTracker.LatencyInfo GetCurrentLatencyInfo()
		{
			AggregatedOperationStatistics adlatency = new AggregatedOperationStatistics
			{
				Type = AggregatedOperationType.ADCalls,
				Count = 0L,
				TotalMilliseconds = 0.0
			};
			AggregatedOperationStatistics rpcLatency = new AggregatedOperationStatistics
			{
				Type = AggregatedOperationType.StoreRPCs,
				Count = 0L,
				TotalMilliseconds = 0.0
			};
			AggregatedOperationStatistics adobjToExchObjLatency = new AggregatedOperationStatistics
			{
				Type = AggregatedOperationType.ADObjToExchObjLatency,
				Count = 0L,
				TotalMilliseconds = 0.0
			};
			if (this.activityScopeGetter != null)
			{
				IActivityScope activityScope = this.activityScopeGetter();
				if (activityScope != null)
				{
					adlatency = activityScope.TakeStatisticsSnapshot(AggregatedOperationType.ADCalls);
					rpcLatency = activityScope.TakeStatisticsSnapshot(AggregatedOperationType.StoreRPCs);
					adobjToExchObjLatency = activityScope.TakeStatisticsSnapshot(AggregatedOperationType.ADObjToExchObjLatency);
				}
			}
			return new LatencyTracker.LatencyInfo
			{
				ElapsedTime = this.stopWatch.ElapsedMilliseconds,
				ADLatency = adlatency,
				RpcLatency = rpcLatency,
				ADObjToExchObjLatency = adobjToExchObjLatency
			};
		}

		internal const string LatencyMissed = "LatencyMissed";

		private readonly Stopwatch stopWatch = new Stopwatch();

		private readonly Dictionary<string, long> latencyBreakDowns = new Dictionary<string, long>();

		private readonly Dictionary<string, LatencyTracker.LatencyInfo> internalTrackingDic = new Dictionary<string, LatencyTracker.LatencyInfo>();

		private readonly Dictionary<string, string> groupTrackingDic = new Dictionary<string, string>();

		private readonly Dictionary<string, int> funcNameTrackingDic = new Dictionary<string, int>();

		private readonly Func<IActivityScope> activityScopeGetter;

		private struct LatencyInfo
		{
			public bool LogDetailsAlways { get; set; }

			public string GroupName { get; set; }

			public string FuncName { get; set; }

			public long ElapsedTime { get; set; }

			public AggregatedOperationStatistics ADLatency { get; set; }

			public AggregatedOperationStatistics RpcLatency { get; set; }

			public AggregatedOperationStatistics ADObjToExchObjLatency { get; set; }

			public static string GetKey(string groupName, string funcName)
			{
				return groupName + "." + funcName;
			}

			public static LatencyTracker.LatencyInfo operator -(LatencyTracker.LatencyInfo s1, LatencyTracker.LatencyInfo s2)
			{
				return new LatencyTracker.LatencyInfo
				{
					ElapsedTime = s1.ElapsedTime - s2.ElapsedTime,
					ADLatency = s1.ADLatency - s2.ADLatency,
					RpcLatency = s1.RpcLatency - s2.RpcLatency,
					ADObjToExchObjLatency = s1.ADObjToExchObjLatency - s2.ADObjToExchObjLatency
				};
			}
		}
	}
}
