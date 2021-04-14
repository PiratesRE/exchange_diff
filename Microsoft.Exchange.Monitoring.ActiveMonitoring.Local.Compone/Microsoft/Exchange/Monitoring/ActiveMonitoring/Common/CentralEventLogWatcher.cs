using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class CentralEventLogWatcher : IDisposable
	{
		private CentralEventLogWatcher()
		{
			TimerCallback callback = new TimerCallback(this.ProcessEvents);
			this.processorTimer = new Timer(callback, null, CentralEventLogWatcher.EventProcessorTimerInterval, CentralEventLogWatcher.EventProcessorTimerInterval);
		}

		private static void AddValueToDictionary<T>(Dictionary<string, HashSet<T>> dict, string key, T[] entriesToAdd)
		{
			if (entriesToAdd == null || entriesToAdd.Length < 1)
			{
				return;
			}
			HashSet<T> hashSet = null;
			if (!dict.TryGetValue(key, out hashSet))
			{
				hashSet = new HashSet<T>();
				dict.Add(key, hashSet);
			}
			foreach (T item in entriesToAdd)
			{
				hashSet.Add(item);
			}
		}

		private static TimeSpan EventProcessorTimerInterval
		{
			get
			{
				if (CentralEventLogWatcher.timerInterval == TimeSpan.Zero)
				{
					CentralEventLogWatcher.timerInterval = TimeSpan.FromSeconds((double)RegistryHelper.GetProperty<int>("ProcessorTimerIntervalInSecs", 300, "EventProcessor", null, false));
				}
				return CentralEventLogWatcher.timerInterval;
			}
		}

		public static CentralEventLogWatcher Instance
		{
			get
			{
				lock (CentralEventLogWatcher.singletonLock)
				{
					if (CentralEventLogWatcher.watcherInstance == null)
					{
						CentralEventLogWatcher.watcherInstance = new CentralEventLogWatcher();
					}
				}
				return CentralEventLogWatcher.watcherInstance;
			}
		}

		public event Action<EventRecord, CentralEventLogWatcher.EventRecordMini> BeforeEnqueueEvent;

		public void Dispose()
		{
			lock (this.disposeLock)
			{
				if (!this.isDisposed)
				{
					if (this.processorTimer != null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher:: Disposing ProcessorTimer...", null, "Dispose", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 306);
						this.processorTimer.Dispose();
						this.processorTimer = null;
					}
					this.isDisposed = true;
				}
			}
		}

		public bool IsEventWatchRuleExists(CentralEventLogWatcher.IEventLogWatcherRule rule)
		{
			return this.eventProbeRules.ContainsKey(rule.RuleName) && this.eventProbeRules[rule.RuleName].SameAs(rule);
		}

		public void AddEventWatchRule(CentralEventLogWatcher.IEventLogWatcherRule rule)
		{
			this.CheckIsDisposedAndThrow();
			if (string.IsNullOrWhiteSpace(rule.LogName) || string.IsNullOrWhiteSpace(rule.RuleName))
			{
				throw new CentralEventLogWatcher.InvalidRuleException(string.Format("Invalid EventWatchRule - LogName={0}, RuleName={1}", rule.LogName, rule.RuleName));
			}
			string logName = rule.LogName;
			string[] providerNames = rule.ProviderNames;
			long[] eventIds = rule.EventIds;
			if (!EventLog.Exists(logName) && !EventLogSession.GlobalSession.GetLogNames().Contains(logName))
			{
				throw new Exception(string.Format("LogName - {0} does not exists!", rule.LogName));
			}
			CentralEventLogWatcher.IEventLogWatcherRule eventLogWatcherRule = null;
			if (this.eventProbeRules.TryGetValue(rule.RuleName, out eventLogWatcherRule) && !this.eventProbeRules[rule.RuleName].GetType().Equals(rule.GetType()))
			{
				throw new CentralEventLogWatcher.InvalidRuleException(string.Format("Rule with name {0} is enrolled into watcher but not of the same type. You cannot overwrite existing rule with same name but of different type. RuleExistsType={1}, incoming type={2}", rule.RuleName, this.eventProbeRules[rule.RuleName].GetType(), rule.GetType()));
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher: Writing rule name={0}", rule.RuleName, null, "AddEventWatchRule", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 372);
			this.rwLockSubscriberRequests.EnterWriteLock();
			try
			{
				CentralEventLogWatcher.AddValueToDictionary<long>(this.subscriberRequestIds, logName, eventIds);
				CentralEventLogWatcher.AddValueToDictionary<string>(this.subscriberRequestProviders, logName, providerNames);
			}
			finally
			{
				this.rwLockSubscriberRequests.ExitWriteLock();
			}
			this.RebuildEventFilterList(rule);
			this.eventProbeRules.AddOrUpdate(rule.RuleName, rule, (string str, CentralEventLogWatcher.IEventLogWatcherRule currentRule) => rule);
		}

		public int PopLastEventRecordForRule(string ruleName, out CentralEventLogWatcher.EventRecordMini record)
		{
			this.CheckIsDisposedAndThrow();
			int result = 0;
			this.rwLockEventSorting.EnterUpgradeableReadLock();
			try
			{
				if (this.eventBuckets.ContainsKey(ruleName))
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher:: Clearing bucket {0}", ruleName, null, "PopLastEventRecordForRule", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 417);
					this.rwLockEventSorting.EnterWriteLock();
					try
					{
						if (!this.eventBuckets.TryRemove(ruleName, out record))
						{
							WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher:: Unable to clear bucket {0}.", ruleName, null, "PopLastEventRecordForRule", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 428);
						}
						goto IL_98;
					}
					finally
					{
						this.rwLockEventSorting.ExitWriteLock();
					}
				}
				record = null;
				IL_98:
				if (this.eventCount.ContainsKey(ruleName))
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher:: Clearing count bucket {0}", ruleName, null, "PopLastEventRecordForRule", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 447);
					this.rwLockEventSorting.EnterWriteLock();
					try
					{
						if (!this.eventCount.TryRemove(ruleName, out result))
						{
							WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher:: Unable to clear count bucket {0}.", ruleName, null, "PopLastEventRecordForRule", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 458);
						}
					}
					finally
					{
						this.rwLockEventSorting.ExitWriteLock();
					}
				}
			}
			finally
			{
				this.rwLockEventSorting.ExitUpgradeableReadLock();
			}
			return result;
		}

		public CentralEventLogWatcher.EventProcessorStatus EventProcessorCurrentStatus
		{
			get
			{
				this.rwLockEventProcessorStatus.EnterReadLock();
				CentralEventLogWatcher.EventProcessorStatus result;
				try
				{
					result = new CentralEventLogWatcher.EventProcessorStatus
					{
						LastEventProcessorRuntime = this.processorLastRunTime,
						EventsProcessedSinceInstanceStart = this.processorEventProcessedCount,
						TimerInterval = (int)CentralEventLogWatcher.EventProcessorTimerInterval.TotalSeconds,
						LastEventProcessorTimeSpentInMs = this.processorRuntimeDurationInMs,
						EventProcessorsRunningCount = this.eventProcessorsCount
					};
				}
				finally
				{
					this.rwLockEventProcessorStatus.ExitReadLock();
				}
				return result;
			}
		}

		private string ConstructXPathForLogName(string LogName, DateTime startTime, DateTime endTime)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			if (this.subscriberRequestIds.ContainsKey(LogName))
			{
				foreach (long num in this.subscriberRequestIds[LogName])
				{
					list.Add(string.Format("EventID={0}", num));
				}
			}
			if (this.subscriberRequestProviders.ContainsKey(LogName))
			{
				foreach (string arg in this.subscriberRequestProviders[LogName])
				{
					list2.Add(string.Format("@Name='{0}'", arg));
				}
			}
			if (list.Count < 1)
			{
				return string.Empty;
			}
			if (list2.Count < 1)
			{
				return string.Format("*[System[({0}) and TimeCreated[@SystemTime>='{1}' and @SystemTime<='{2}']]]", string.Join(" or ", list), startTime.ToString("o"), endTime.ToString("o"));
			}
			return string.Format("*[System[Provider[{0}] and ({1}) and TimeCreated[@SystemTime>='{2}' and @SystemTime<='{3}']]]", new object[]
			{
				string.Join(" or ", list2),
				string.Join(" or ", list),
				startTime.ToString("o"),
				endTime.ToString("o")
			});
		}

		private CentralEventLogWatcher.EventRecordMini PreProcessEvent(EventRecord e)
		{
			this.CheckIsDisposedAndThrow();
			if (e != null)
			{
				CentralEventLogWatcher.EventRecordMini eventRecordMini = CentralEventLogWatcher.EventRecordMini.ConstructFromEventRecord(e);
				if (this.BeforeEnqueueEvent != null)
				{
					this.BeforeEnqueueEvent(e, eventRecordMini);
				}
				return eventRecordMini;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher:: EventRecord is Empty", null, "PreProcessEvent", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 573);
			return null;
		}

		private void ProcessEvents(object stateInfo)
		{
			if (this.isDisposed)
			{
				WTFDiagnostics.TraceInformation<bool>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher:: Instance is already disposed. Skipping EventQuery... isDisposed={0}", this.isDisposed, null, "ProcessEvents", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 590);
				return;
			}
			DateTime dateTime = DateTime.UtcNow.AddHours(-1.0);
			DateTime utcNow = DateTime.UtcNow;
			this.rwLockEventProcessorStatus.EnterWriteLock();
			try
			{
				dateTime = ((this.processorLastRunTime == DateTime.MinValue) ? dateTime : this.processorLastRunTime);
				this.eventProcessorsCount++;
				this.processorLastRunTime = utcNow;
			}
			finally
			{
				this.rwLockEventProcessorStatus.ExitWriteLock();
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			CentralEventLogWatcher.IEventLogWatcherRule[] rulesArray = null;
			lock (this.eventFilterListLock)
			{
				if (this.eventFilterList != null && this.eventFilterList.Count > 0)
				{
					rulesArray = this.eventFilterList.ToArray();
				}
			}
			try
			{
				this.rwLockSubscriberRequests.EnterReadLock();
				try
				{
					foreach (string text in this.subscriberRequestIds.Keys)
					{
						dictionary.Add(text, this.ConstructXPathForLogName(text, dateTime, utcNow));
					}
				}
				finally
				{
					this.rwLockSubscriberRequests.ExitReadLock();
				}
				foreach (KeyValuePair<string, string> keyValuePair in dictionary)
				{
					try
					{
						using (EventLogReader eventLogReader = new EventLogReader(new EventLogQuery(keyValuePair.Key, PathType.LogName, keyValuePair.Value)))
						{
							for (EventRecord eventRecord = eventLogReader.ReadEvent(); eventRecord != null; eventRecord = eventLogReader.ReadEvent())
							{
								using (eventRecord)
								{
									this.PutEventsIntoBuckets(this.PreProcessEvent(eventRecord), rulesArray);
								}
							}
						}
					}
					catch (Exception ex)
					{
						WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher:: Exception logged when querying/processing events for log {0} - {1}", keyValuePair.Key, ex.ToString(), null, "ProcessEvents", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 667);
					}
				}
			}
			finally
			{
				this.rwLockEventProcessorStatus.EnterWriteLock();
				try
				{
					this.eventProcessorsCount--;
					this.processorRuntimeDurationInMs = (double)((int)(DateTime.UtcNow - utcNow).TotalMilliseconds);
				}
				finally
				{
					this.rwLockEventProcessorStatus.ExitWriteLock();
				}
			}
		}

		private void PutEventsIntoBuckets(CentralEventLogWatcher.EventRecordMini evtRecord, CentralEventLogWatcher.IEventLogWatcherRule[] rulesArray)
		{
			if (this.isDisposed)
			{
				WTFDiagnostics.TraceInformation<bool>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher:: Instance is already disposed. Skipping PutEventsIntoBuckets... isDisposed={0}", this.isDisposed, null, "PutEventsIntoBuckets", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 700);
				return;
			}
			if (rulesArray != null && rulesArray.Length > 0)
			{
				foreach (CentralEventLogWatcher.IEventLogWatcherRule eventLogWatcherRule in rulesArray)
				{
					if (eventLogWatcherRule.MatchRule(evtRecord))
					{
						string ruleName = eventLogWatcherRule.RuleName;
						this.rwLockEventSorting.EnterUpgradeableReadLock();
						try
						{
							if (!this.eventBuckets.ContainsKey(ruleName) || this.eventBuckets[ruleName] == null || !(this.eventBuckets[ruleName].TimeCreated > evtRecord.TimeCreated))
							{
								this.rwLockEventSorting.EnterWriteLock();
								try
								{
									this.eventBuckets[ruleName] = evtRecord;
									this.eventCount[ruleName] = 1 + this.eventCount.GetOrAdd(ruleName, 0);
								}
								finally
								{
									this.rwLockEventSorting.ExitWriteLock();
								}
							}
						}
						finally
						{
							this.rwLockEventSorting.ExitUpgradeableReadLock();
						}
					}
				}
			}
			this.rwLockEventProcessorStatus.EnterWriteLock();
			try
			{
				this.processorEventProcessedCount += 1L;
			}
			finally
			{
				this.rwLockEventProcessorStatus.ExitWriteLock();
			}
		}

		private void RebuildEventFilterList(CentralEventLogWatcher.IEventLogWatcherRule rule)
		{
			this.CheckIsDisposedAndThrow();
			lock (this.eventFilterListLock)
			{
				int num = -1;
				for (int i = 0; i < this.eventFilterList.Count; i++)
				{
					if (this.eventFilterList[i].RuleName.Equals(rule.RuleName))
					{
						num = i;
						break;
					}
				}
				if (num >= 0)
				{
					WTFDiagnostics.TraceInformation<int, string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher:: Replace rule in eventFilterList Index={0}, Rule={1}", num, rule.RuleName, null, "RebuildEventFilterList", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 782);
					this.eventFilterList[num] = rule;
				}
				else
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CommonComponentsTracer, this.traceContext, "CentralEventLogWatcher:: Adding rule in eventFilterList Rule={0}", rule.RuleName, null, "RebuildEventFilterList", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Probes\\CentralEventLogWatcher.cs", 793);
					this.eventFilterList.Add(rule);
				}
			}
		}

		private void CheckIsDisposedAndThrow()
		{
			lock (this.disposeLock)
			{
				if (this.isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().FullName);
				}
			}
		}

		private const string EventProcessorSubkey = "EventProcessor";

		private const string EventProcessorIntervalValueName = "ProcessorTimerIntervalInSecs";

		private static CentralEventLogWatcher watcherInstance = null;

		private static readonly object singletonLock = new object();

		private static TimeSpan timerInterval = TimeSpan.Zero;

		private TracingContext traceContext = TracingContext.Default;

		private readonly ReaderWriterLockSlim rwLockSubscriberRequests = new ReaderWriterLockSlim();

		private readonly Dictionary<string, HashSet<long>> subscriberRequestIds = new Dictionary<string, HashSet<long>>(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<string, HashSet<string>> subscriberRequestProviders = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

		private readonly ConcurrentDictionary<string, CentralEventLogWatcher.IEventLogWatcherRule> eventProbeRules = new ConcurrentDictionary<string, CentralEventLogWatcher.IEventLogWatcherRule>();

		private readonly ReaderWriterLockSlim rwLockEventSorting = new ReaderWriterLockSlim();

		private readonly ConcurrentDictionary<string, CentralEventLogWatcher.EventRecordMini> eventBuckets = new ConcurrentDictionary<string, CentralEventLogWatcher.EventRecordMini>();

		private readonly ConcurrentDictionary<string, int> eventCount = new ConcurrentDictionary<string, int>();

		private readonly object eventFilterListLock = new object();

		private readonly List<CentralEventLogWatcher.IEventLogWatcherRule> eventFilterList = new List<CentralEventLogWatcher.IEventLogWatcherRule>();

		private readonly ReaderWriterLockSlim rwLockEventProcessorStatus = new ReaderWriterLockSlim();

		private Timer processorTimer;

		private int eventProcessorsCount;

		private long processorEventProcessedCount;

		private DateTime processorLastRunTime = DateTime.MinValue;

		private double processorRuntimeDurationInMs;

		private bool isDisposed;

		private readonly object disposeLock = new object();

		public interface IEventLogWatcherRule
		{
			string RuleName { get; }

			string LogName { get; }

			long[] EventIds { get; }

			string[] ProviderNames { get; }

			bool SameAs(CentralEventLogWatcher.IEventLogWatcherRule rule);

			bool MatchRule(CentralEventLogWatcher.EventRecordMini evt);
		}

		public class EventRecordMini
		{
			public static CentralEventLogWatcher.EventRecordMini ConstructFromEventRecord(EventRecord evt)
			{
				CentralEventLogWatcher.EventRecordMini eventRecordMini = new CentralEventLogWatcher.EventRecordMini();
				eventRecordMini.LogName = evt.LogName;
				eventRecordMini.Source = evt.ProviderName;
				eventRecordMini.EventId = evt.Id;
				eventRecordMini.TimeCreated = evt.TimeCreated;
				eventRecordMini.CustomizedProperties = new List<string>();
				foreach (EventProperty eventProperty in evt.Properties)
				{
					eventRecordMini.CustomizedProperties.Add(eventProperty.Value.ToString());
				}
				return eventRecordMini;
			}

			public string LogName;

			public string Source;

			public int EventId;

			public DateTime? TimeCreated;

			public string WatsonProcessName;

			public string WatsonExtendedPropertyField1;

			public string WatsonExtendedPropertyField2;

			public string WatsonExtendedPropertyField3;

			public bool IsProcessTerminatingWatson;

			public string ExtendedPropertyField1;

			public List<string> CustomizedProperties;
		}

		public struct EventProcessorStatus
		{
			public long EventsProcessedSinceInstanceStart;

			public double LastEventProcessorTimeSpentInMs;

			public DateTime LastEventProcessorRuntime;

			public int TimerInterval;

			public int EventProcessorsRunningCount;
		}

		public class EventProbeRule : CentralEventLogWatcher.IEventLogWatcherRule
		{
			public EventProbeRule(string ruleName, string logName, string source, int[] greenEventIds, int[] redEventIds)
			{
				this.EventRuleName = ruleName;
				this.EventLogName = logName;
				this.Source = source;
				this.providerNames = new string[]
				{
					source
				};
				this.greenEventIds = greenEventIds;
				this.redEventIds = redEventIds;
				List<int> list = new List<int>();
				if (this.greenEventIds != null && this.greenEventIds.Length > 0)
				{
					list.AddRange(this.greenEventIds);
				}
				if (this.redEventIds != null && this.redEventIds.Length > 0)
				{
					list.AddRange(this.redEventIds);
				}
				this.allEventIds = (from id in list
				select (long)id).ToArray<long>();
			}

			public int[] GreenEventIds
			{
				get
				{
					if (this.greenEventIds == null)
					{
						return null;
					}
					return (int[])this.greenEventIds.Clone();
				}
			}

			public int[] RedEventIds
			{
				get
				{
					if (this.redEventIds == null)
					{
						return null;
					}
					return (int[])this.redEventIds.Clone();
				}
			}

			public string LogName
			{
				get
				{
					return this.EventLogName;
				}
			}

			public string RuleName
			{
				get
				{
					return this.EventRuleName;
				}
			}

			public long[] EventIds
			{
				get
				{
					return this.allEventIds;
				}
			}

			public string[] ProviderNames
			{
				get
				{
					return this.providerNames;
				}
			}

			public bool SameAs(CentralEventLogWatcher.IEventLogWatcherRule rule)
			{
				if (rule is CentralEventLogWatcher.EventProbeRule)
				{
					CentralEventLogWatcher.EventProbeRule eventProbeRule = (CentralEventLogWatcher.EventProbeRule)rule;
					return string.Equals(this.EventLogName, eventProbeRule.EventLogName, StringComparison.Ordinal) && string.Equals(this.Source, eventProbeRule.Source, StringComparison.Ordinal) && string.Equals(this.RuleName, eventProbeRule.RuleName, StringComparison.Ordinal) && CentralEventLogWatcher.EventProbeRule.IsArrayEqual<int>(this.greenEventIds, eventProbeRule.greenEventIds) && CentralEventLogWatcher.EventProbeRule.IsArrayEqual<int>(this.redEventIds, eventProbeRule.redEventIds);
				}
				return false;
			}

			public virtual bool MatchRule(CentralEventLogWatcher.EventRecordMini evt)
			{
				return (this.Source.Equals("*") || this.Source.Equals(evt.Source, StringComparison.OrdinalIgnoreCase)) && this.EventIds.Contains((long)evt.EventId);
			}

			private static bool IsArrayEqual<T>(T[] source, T[] target)
			{
				if (object.ReferenceEquals(source, target))
				{
					return true;
				}
				if (source == null || target == null)
				{
					return false;
				}
				if (source.Length != target.Length)
				{
					return false;
				}
				EqualityComparer<T> @default = EqualityComparer<T>.Default;
				for (int i = 0; i < source.Length; i++)
				{
					if (!@default.Equals(source[i], target[i]))
					{
						return false;
					}
				}
				return true;
			}

			public readonly string EventRuleName;

			public readonly string EventLogName;

			public readonly string Source;

			private int[] greenEventIds;

			private int[] redEventIds;

			private long[] allEventIds;

			private string[] providerNames;
		}

		public class ProcessCrashRule : CentralEventLogWatcher.IEventLogWatcherRule
		{
			public ProcessCrashRule(string ruleName, string serviceName, string moduleName = null, bool skipInformationalWatson = false)
			{
				this.EventRuleName = ruleName;
				this.ServiceName = serviceName;
				this.ModuleName = moduleName;
				this.SkipInformationalWatson = skipInformationalWatson;
			}

			public string RuleName
			{
				get
				{
					return this.EventRuleName;
				}
			}

			public string LogName
			{
				get
				{
					return "Application";
				}
			}

			public long[] EventIds
			{
				get
				{
					return CentralEventLogWatcher.ProcessCrashRule.eventIds;
				}
			}

			public string[] ProviderNames
			{
				get
				{
					return CentralEventLogWatcher.ProcessCrashRule.providerNames;
				}
			}

			public bool SameAs(CentralEventLogWatcher.IEventLogWatcherRule rule)
			{
				if (rule is CentralEventLogWatcher.ProcessCrashRule)
				{
					CentralEventLogWatcher.ProcessCrashRule processCrashRule = (CentralEventLogWatcher.ProcessCrashRule)rule;
					return string.Equals(processCrashRule.ServiceName, this.ServiceName) && string.Equals(rule.RuleName, this.RuleName) && string.Equals(processCrashRule.ModuleName, this.ModuleName) && processCrashRule.SkipInformationalWatson == this.SkipInformationalWatson;
				}
				return false;
			}

			public bool MatchRule(CentralEventLogWatcher.EventRecordMini evt)
			{
				if (string.IsNullOrEmpty(evt.Source) || string.IsNullOrEmpty(evt.WatsonProcessName))
				{
					return false;
				}
				bool flag = false;
				if (string.Equals(evt.Source, "MSExchange Common") && evt.EventId == 4999 && string.Compare(evt.WatsonProcessName, this.ServiceName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (string.IsNullOrEmpty(this.ModuleName))
					{
						flag = true;
					}
					else
					{
						string watsonExtendedPropertyField = evt.WatsonExtendedPropertyField1;
						flag = (!string.IsNullOrEmpty(watsonExtendedPropertyField) && watsonExtendedPropertyField.IndexOf(this.ModuleName, StringComparison.OrdinalIgnoreCase) != -1);
					}
					if (this.SkipInformationalWatson)
					{
						flag = (flag && evt.IsProcessTerminatingWatson);
					}
				}
				return flag;
			}

			public const string Source = "MSExchange Common";

			public const int EventId = 4999;

			public const string EventLogName = "Application";

			internal const int WatsonIssueTypeIndex = 1;

			internal const int WatsonProcessNameIndex = 4;

			internal const int WatsonAssemblyNameIndex = 5;

			internal const int WatsonMethodIndex = 6;

			internal const int WatsonExceptionTypeIndex = 7;

			internal const int WatsonTerminatingProcessIndex = 11;

			internal const string NativeCodeCrashTypeName = "E12N";

			private static readonly long[] eventIds = new long[]
			{
				4999L
			};

			private static readonly string[] providerNames = new string[]
			{
				"MSExchange Common"
			};

			public readonly string EventRuleName;

			public readonly string ServiceName;

			public readonly string ModuleName;

			public readonly bool SkipInformationalWatson;
		}

		[Serializable]
		public class InvalidRuleException : Exception
		{
			public InvalidRuleException(string exceptionMessage) : base(exceptionMessage)
			{
			}
		}
	}
}
