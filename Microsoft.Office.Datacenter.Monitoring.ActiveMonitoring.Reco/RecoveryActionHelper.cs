using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class RecoveryActionHelper
	{
		internal static DateTime CurrentProcessStartTime
		{
			get
			{
				if (RecoveryActionHelper.currentProcessStartTime == DateTime.MaxValue)
				{
					lock (RecoveryActionHelper.locker)
					{
						using (Process currentProcess = Process.GetCurrentProcess())
						{
							RecoveryActionHelper.currentProcessStartTime = currentProcess.StartTime;
						}
					}
				}
				return RecoveryActionHelper.currentProcessStartTime;
			}
		}

		public static bool CommunicateWorkerProcessInfoToHostProcess(bool isForce = false)
		{
			if (!isForce && RecoveryActionHelper.isWorkerProcessInfoSentToHost)
			{
				return true;
			}
			bool result = false;
			RpcSetWorkerProcessInfoImpl.WorkerProcessInfo info = new RpcSetWorkerProcessInfoImpl.WorkerProcessInfo
			{
				ProcessStartTime = RecoveryActionHelper.CurrentProcessStartTime
			};
			try
			{
				Dependencies.LamRpc.SetWorkerProcessInfo(Dependencies.ThrottleHelper.Tunables.LocalMachineName, info, 30000);
				result = true;
				RecoveryActionHelper.isWorkerProcessInfoSentToHost = true;
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.RecoveryActionTracer, RecoveryActionHelper.traceContext, "SetWorkerProcessInfo() failed with exception: {0})", ex.Message, null, "CommunicateWorkerProcessInfoToHostProcess", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\RecoveryActionHelper.cs", 118);
			}
			return result;
		}

		public static Exception RunAndMeasure(string operation, bool isRethrow, ManagedAvailabilityCrimsonEvent crimsonEvent, Func<string> action)
		{
			Stopwatch stopwatch = new Stopwatch();
			TimeSpan timeSpan = TimeSpan.MinValue;
			string text = string.Empty;
			Exception ex = null;
			try
			{
				stopwatch.Start();
				text = action();
				timeSpan = stopwatch.Elapsed;
			}
			catch (Exception ex2)
			{
				timeSpan = stopwatch.Elapsed;
				ex = ex2;
				if (isRethrow)
				{
					throw;
				}
			}
			finally
			{
				if (ex != null)
				{
					WTFDiagnostics.TraceError<string, string, string>(ExTraceGlobals.RecoveryActionTracer, RecoveryActionHelper.traceContext, "{0} failed with exception. (Duration: {1}, Exception: {2})", operation, timeSpan.ToString(), ex.ToString(), null, "RunAndMeasure", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\RecoveryActionHelper.cs", 169);
				}
				else
				{
					WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.RecoveryActionTracer, RecoveryActionHelper.traceContext, "{0} returned. (Duration: {1})", operation, timeSpan.ToString(), null, "RunAndMeasure", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\RecoveryActionHelper.cs", 179);
				}
				if (crimsonEvent != null)
				{
					crimsonEvent.LogGeneric(new object[]
					{
						operation,
						timeSpan.ToString(),
						text,
						(ex != null) ? ex.ToString() : "<none>"
					});
				}
			}
			return ex;
		}

		public static string ConstructInstanceId(RecoveryActionId id, DateTime startTime)
		{
			string str = startTime.ToString("yyMMdd.hhmmss.fffff.");
			int num = (int)id;
			return str + num.ToString("000");
		}

		public static RecoveryActionEntry ConstructStartActionEntry(RecoveryActionId id, string resourceName, string requesterName, DateTime expectedEndTime, string throttleIdentity, string throttleParametersXml, string context = null, string instanceId = null)
		{
			return RecoveryActionHelper.ConstructStartActionEntry(id, resourceName, requesterName, ExDateTime.Now.LocalTime, expectedEndTime, throttleIdentity, throttleParametersXml, context, instanceId, RecoveryActionHelper.CurrentProcessStartTime);
		}

		public static RecoveryActionEntry ConstructStartActionEntry(RecoveryActionId id, string resourceName, string requesterName, DateTime startTime, DateTime expectedEndTime, string throttleIdentity, string throttleParametersXml, string context, string instanceId, DateTime lamProcessStartTime)
		{
			return new RecoveryActionEntry
			{
				Id = id,
				ResourceName = resourceName,
				RequestorName = requesterName,
				State = RecoveryActionState.Started,
				Result = RecoveryActionResult.Succeeded,
				StartTime = startTime,
				EndTime = expectedEndTime,
				Context = context,
				InstanceId = (instanceId ?? RecoveryActionHelper.ConstructInstanceId(id, startTime)),
				LamProcessStartTime = lamProcessStartTime,
				ThrottleIdentity = throttleIdentity,
				ThrottleParametersXml = throttleParametersXml
			};
		}

		public static RecoveryActionEntry ConstructFinishActionEntry(RecoveryActionEntry startActionEntry, Exception exception = null, DateTime? finishTime = null, string finishContext = null)
		{
			RecoveryActionEntry recoveryActionEntry = new RecoveryActionEntry();
			recoveryActionEntry.Id = startActionEntry.Id;
			recoveryActionEntry.ResourceName = startActionEntry.ResourceName;
			recoveryActionEntry.RequestorName = startActionEntry.RequestorName;
			recoveryActionEntry.InstanceId = startActionEntry.InstanceId;
			recoveryActionEntry.CustomArg1 = startActionEntry.CustomArg1;
			recoveryActionEntry.CustomArg2 = startActionEntry.CustomArg2;
			recoveryActionEntry.CustomArg3 = startActionEntry.CustomArg3;
			recoveryActionEntry.Context = (finishContext ?? startActionEntry.Context);
			recoveryActionEntry.StartTime = startActionEntry.StartTime;
			recoveryActionEntry.EndTime = ((finishTime != null) ? finishTime.Value : ExDateTime.Now.LocalTime);
			recoveryActionEntry.State = RecoveryActionState.Finished;
			recoveryActionEntry.ThrottleIdentity = startActionEntry.ThrottleIdentity;
			recoveryActionEntry.ThrottleParametersXml = startActionEntry.ThrottleParametersXml;
			recoveryActionEntry.WriteRecordDelayInMilliSeconds = 1001;
			if (exception == null)
			{
				recoveryActionEntry.Result = RecoveryActionResult.Succeeded;
			}
			else
			{
				recoveryActionEntry.Result = RecoveryActionResult.Failed;
				recoveryActionEntry.ExceptionName = exception.GetType().Name;
				recoveryActionEntry.ExceptionMessage = exception.Message;
			}
			return recoveryActionEntry;
		}

		public static DateTime GetSystemBootTime()
		{
			Exception ex = null;
			DateTime dateTime = RecoveryActionHelper.GetSystemBootTime(out ex);
			if (dateTime == DateTime.MinValue)
			{
				dateTime = ExDateTime.Now.LocalTime;
			}
			return dateTime;
		}

		public static DateTime GetSystemBootTime(out Exception exception)
		{
			exception = null;
			DateTime dateTime = DateTime.MinValue;
			if (BugcheckSimulator.Instance.IsEnabled)
			{
				dateTime = BugcheckSimulator.Instance.SimulatedSystemBootTime;
			}
			if (dateTime == DateTime.MinValue)
			{
				if (RecoveryActionHelper.currentBootTime == DateTime.MinValue)
				{
					lock (RecoveryActionHelper.locker)
					{
						exception = WmiHelper.HandleWmiExceptions(delegate
						{
							RecoveryActionHelper.currentBootTime = WmiHelper.GetSystemBootTime();
						});
						if (exception != null)
						{
							WTFDiagnostics.TraceError<string>(ExTraceGlobals.RecoveryActionTracer, RecoveryActionHelper.traceContext, "GetSystemBootTime() failed with exception: {0})", exception.Message, null, "GetSystemBootTime", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\RecoveryActionHelper.cs", 388);
						}
					}
				}
				dateTime = RecoveryActionHelper.currentBootTime;
			}
			return dateTime;
		}

		public static string GetShortServerName(string fqdn)
		{
			string result = fqdn;
			if (fqdn != null)
			{
				string[] array = fqdn.Split(new char[]
				{
					'.'
				});
				if (array != null && array.Length > 0)
				{
					result = array[0];
				}
			}
			return result;
		}

		public static bool IsLocalServerName(string serverName)
		{
			bool result = false;
			if (serverName != null)
			{
				string shortServerName = RecoveryActionHelper.GetShortServerName(serverName);
				if (string.Equals(Dependencies.ThrottleHelper.Tunables.LocalMachineName, shortServerName, StringComparison.OrdinalIgnoreCase))
				{
					result = true;
				}
			}
			return result;
		}

		public static RecoveryActionEntry FindActionEntry(RecoveryActionId actionId, string resourceName, DateTime queryStartTime, DateTime queryEndTime)
		{
			RecoveryActionEntry result = null;
			bool flag = false;
			List<RecoveryActionEntry> list = RecoveryActionHelper.ReadEntries(actionId, null, null, RecoveryActionState.None, RecoveryActionResult.None, queryStartTime, queryEndTime, out flag, null, TimeSpan.MaxValue, 1);
			if (list.Count > 0)
			{
				result = list.First<RecoveryActionEntry>();
			}
			return result;
		}

		public static void Sleep(TimeSpan duration, CancellationToken cancellationToken)
		{
			if (cancellationToken.WaitHandle != null)
			{
				cancellationToken.WaitHandle.WaitOne(duration);
				cancellationToken.ThrowIfCancellationRequested();
				return;
			}
			Thread.Sleep(duration);
		}

		internal static string ConstructActionName(RecoveryActionId actionId, string resourceName)
		{
			return string.Format("{0}_{1}", actionId, resourceName);
		}

		internal static string ConstructCondition(List<string> conditions, bool condition, string propertyName, object propValue)
		{
			string text = string.Empty;
			if (condition)
			{
				text = string.Format("{0}='{1}'", propertyName, propValue);
				conditions.Add(text);
			}
			return text;
		}

		internal static string PrepareQueryConditionString(RecoveryActionId actionId, string resourceName, string instanceId, RecoveryActionState state, RecoveryActionResult result)
		{
			List<string> list = new List<string>();
			RecoveryActionHelper.ConstructCondition(list, actionId != RecoveryActionId.None, "Id", actionId);
			RecoveryActionHelper.ConstructCondition(list, !string.IsNullOrEmpty(resourceName), "ResourceName", resourceName);
			RecoveryActionHelper.ConstructCondition(list, !string.IsNullOrEmpty(instanceId), "InstanceId", instanceId);
			RecoveryActionHelper.ConstructCondition(list, state != RecoveryActionState.None, "State", state);
			RecoveryActionHelper.ConstructCondition(list, result != RecoveryActionResult.None, "Result", result);
			string result2 = null;
			if (list.Count > 0)
			{
				result2 = string.Format("({0})", string.Join(" and ", list.ToArray()));
			}
			return result2;
		}

		internal static bool ForeachMatching(Func<RecoveryActionEntry, bool, bool> action, RecoveryActionId actionId, string resourceName, string instanceId, RecoveryActionState state, RecoveryActionResult result, DateTime startTime, DateTime endTime, out bool isMoreRecordsAvailable, string xpathQueryString, TimeSpan timeout, int maxCount = 4096)
		{
			isMoreRecordsAvailable = false;
			Stopwatch stopwatch = Stopwatch.StartNew();
			CrimsonReader<RecoveryActionEntry> crimsonReader = RecoveryActionHelper.PrepareReader(actionId, resourceName, instanceId, state, result, startTime, endTime, xpathQueryString);
			bool isReverseDirection = crimsonReader.IsReverseDirection;
			int num = 0;
			bool flag = false;
			for (;;)
			{
				RecoveryActionEntry recoveryActionEntry = crimsonReader.ReadNext();
				if (recoveryActionEntry == null)
				{
					return flag;
				}
				num++;
				flag = action(recoveryActionEntry, isReverseDirection);
				if (flag)
				{
					return flag;
				}
				if (maxCount != -1 && num > maxCount)
				{
					break;
				}
				if (stopwatch.Elapsed > timeout)
				{
					goto Block_5;
				}
				if (num % 1000 == 0)
				{
					Thread.Sleep(10);
				}
			}
			isMoreRecordsAvailable = true;
			return flag;
			Block_5:
			isMoreRecordsAvailable = true;
			throw new TimeoutException();
		}

		internal static RecoveryActionHelper.RecoveryActionQuotaInfo GetRecoveryActionQuotaInfo(RecoveryActionId actionId, string resourceName, int maxAllowedQuota, DateTime queryStartTime, DateTime queryEndTime, TimeSpan timeout)
		{
			RecoveryActionHelper.RecoveryActionQuotaInfo quotaInfo = new RecoveryActionHelper.RecoveryActionQuotaInfo(actionId, resourceName, maxAllowedQuota);
			bool flag = false;
			quotaInfo.IsTimedout = RecoveryActionHelper.ForeachMatching((RecoveryActionEntry entry, bool isNewestToOldest) => quotaInfo.Update(entry), actionId, resourceName, null, RecoveryActionState.None, RecoveryActionResult.None, queryStartTime, queryEndTime, out flag, null, timeout, 4096);
			return quotaInfo;
		}

		internal static List<RecoveryActionEntry> ReadEntries(RecoveryActionId actionId, string resourceName, string instanceId, RecoveryActionState state, RecoveryActionResult result, DateTime startTime, DateTime endTime, out bool isMoreRecordsAvailable, string xpathQueryString, TimeSpan timeout, int maxCount)
		{
			List<RecoveryActionEntry> entries = new List<RecoveryActionEntry>();
			RecoveryActionHelper.ForeachMatching(delegate(RecoveryActionEntry entry, bool isNewestToOldest)
			{
				entries.Add(entry);
				return false;
			}, actionId, resourceName, instanceId, state, result, startTime, endTime, out isMoreRecordsAvailable, null, timeout, maxCount);
			return entries;
		}

		internal static CrimsonReader<RecoveryActionEntry> PrepareReader(RecoveryActionId actionId, string resourceName, string instanceId, RecoveryActionState state, RecoveryActionResult result, DateTime startTime, DateTime endTime, string xpathQueryString = null)
		{
			CrimsonReader<RecoveryActionEntry> crimsonReader = new CrimsonReader<RecoveryActionEntry>(null, null, "Microsoft-Exchange-ManagedAvailability/RecoveryActionResults");
			crimsonReader.IsReverseDirection = true;
			if (string.IsNullOrEmpty(xpathQueryString))
			{
				crimsonReader.QueryUserPropertyCondition = RecoveryActionHelper.PrepareQueryConditionString(actionId, resourceName, instanceId, state, result);
				crimsonReader.QueryStartTime = new DateTime?(startTime);
				crimsonReader.QueryEndTime = new DateTime?(endTime);
			}
			else
			{
				crimsonReader.ExplicitXPathQuery = xpathQueryString;
			}
			return crimsonReader;
		}

		internal static RecoveryActionThrottlingMode GetRecoveryActionLocalThrottlingMode(RecoveryActionId actionId, RecoveryActionThrottlingMode defaultMode = RecoveryActionThrottlingMode.None)
		{
			return RegistryHelper.GetProperty<RecoveryActionThrottlingMode>("LocalThrottlingMode", defaultMode, string.Format("RecoveryAction\\{0}", actionId), null, false);
		}

		internal static RecoveryActionThrottlingMode GetRecoveryActionDistributedThrottlingMode(RecoveryActionId actionId, RecoveryActionThrottlingMode defaultMode = RecoveryActionThrottlingMode.None)
		{
			return RegistryHelper.GetProperty<RecoveryActionThrottlingMode>("DistributedThrottlingMode", defaultMode, string.Format("RecoveryAction\\{0}", actionId), null, false);
		}

		internal static RecoveryActionHelper.RecoveryActionEntrySerializable CreateSerializableRecoveryActionEntry(RecoveryActionEntry entry)
		{
			if (entry == null)
			{
				return null;
			}
			return new RecoveryActionHelper.RecoveryActionEntrySerializable(entry);
		}

		internal const string RecoveryActionChannelName = "Microsoft-Exchange-ManagedAvailability/RecoveryActionResults";

		private static object locker = new object();

		private static DateTime currentProcessStartTime = DateTime.MaxValue;

		private static DateTime currentBootTime = DateTime.MinValue;

		private static TracingContext traceContext = TracingContext.Default;

		private static bool isWorkerProcessInfoSentToHost = false;

		[Serializable]
		public class RecoveryActionEntrySerializable
		{
			public RecoveryActionEntrySerializable(RecoveryActionEntry entry)
			{
				this.Id = entry.Id;
				this.InstanceId = entry.InstanceId;
				this.ResourceName = entry.ResourceName;
				this.StartTimeUtc = entry.StartTime.ToUniversalTime();
				this.EndTimeUtc = entry.EndTime.ToUniversalTime();
				this.State = entry.State;
				this.Result = entry.Result;
				this.RequestorName = entry.RequestorName;
				this.ExceptionName = entry.ExceptionName;
				this.ExceptionMessage = entry.ExceptionMessage;
				this.Context = entry.Context;
				this.CustomArg1 = entry.CustomArg1;
				this.CustomArg2 = entry.CustomArg2;
				this.CustomArg3 = entry.CustomArg3;
				this.LamProcessStartTime = entry.LamProcessStartTime;
				this.ThrottleIdentity = entry.ThrottleIdentity;
				this.ThrottleParametersXml = entry.ThrottleParametersXml;
				this.TotalLocalActionsInOneHour = entry.TotalLocalActionsInOneHour;
				this.TotalLocalActionsInOneDay = entry.TotalLocalActionsInOneDay;
				this.TotalGroupActionsInOneDay = entry.TotalGroupActionsInOneDay;
			}

			public RecoveryActionId Id { get; set; }

			public string InstanceId { get; set; }

			public string ResourceName { get; set; }

			public DateTime StartTimeUtc { get; set; }

			public DateTime StartTime
			{
				get
				{
					if (this.startTime == null)
					{
						this.startTime = new DateTime?(this.StartTimeUtc.ToLocalTime());
					}
					return this.startTime.Value;
				}
			}

			public DateTime EndTimeUtc { get; set; }

			public DateTime EndTime
			{
				get
				{
					if (this.endTime == null)
					{
						this.endTime = new DateTime?(this.EndTimeUtc.ToLocalTime());
					}
					return this.endTime.Value;
				}
			}

			public RecoveryActionState State { get; set; }

			public RecoveryActionResult Result { get; set; }

			public string RequestorName { get; set; }

			public string ExceptionName { get; set; }

			public string ExceptionMessage { get; set; }

			public string Context { get; set; }

			public string CustomArg1 { get; set; }

			public string CustomArg2 { get; set; }

			public string CustomArg3 { get; set; }

			public DateTime LamProcessStartTime { get; set; }

			public string ThrottleIdentity { get; set; }

			public string ThrottleParametersXml { get; set; }

			public int TotalLocalActionsInOneHour { get; set; }

			public int TotalLocalActionsInOneDay { get; set; }

			public int TotalGroupActionsInOneDay { get; set; }

			private DateTime? startTime;

			private DateTime? endTime;
		}

		[Serializable]
		public class RecoveryActionQuotaInfo
		{
			internal RecoveryActionQuotaInfo(RecoveryActionId actionId, string resourceName, int maxAllowedQuota)
			{
				this.ActionId = actionId;
				this.ResourceName = resourceName;
				this.MaxAllowedQuota = maxAllowedQuota;
				this.RemainingQuota = maxAllowedQuota;
				this.LastSystemBootTime = RecoveryActionHelper.GetSystemBootTime();
			}

			public RecoveryActionId ActionId { get; private set; }

			public string ResourceName { get; private set; }

			public int RemainingQuota { get; private set; }

			public int MaxAllowedQuota { get; private set; }

			public RecoveryActionHelper.RecoveryActionEntrySerializable LastStartedEntry { get; private set; }

			public RecoveryActionHelper.RecoveryActionEntrySerializable LastSucceededEntry { get; private set; }

			public DateTime LastSystemBootTime { get; private set; }

			public string CustomArg { get; set; }

			public bool IsTimedout { get; set; }

			public bool Update(RecoveryActionEntry entry)
			{
				bool result = false;
				switch (entry.State)
				{
				case RecoveryActionState.Started:
					if (this.LastStartedEntry == null || entry.StartTime > this.LastStartedEntry.StartTime)
					{
						this.LastStartedEntry = new RecoveryActionHelper.RecoveryActionEntrySerializable(entry);
					}
					break;
				case RecoveryActionState.Finished:
					if (entry.Result == RecoveryActionResult.Succeeded)
					{
						this.RemainingQuota--;
						if (this.RemainingQuota == 0)
						{
							result = true;
						}
						if (this.LastSucceededEntry == null || entry.StartTime > this.LastSucceededEntry.StartTime)
						{
							this.LastSucceededEntry = new RecoveryActionHelper.RecoveryActionEntrySerializable(entry);
						}
					}
					break;
				}
				return result;
			}
		}
	}
}
