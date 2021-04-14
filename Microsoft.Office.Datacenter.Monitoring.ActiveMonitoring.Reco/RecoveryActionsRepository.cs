using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class RecoveryActionsRepository
	{
		private RecoveryActionsRepository()
		{
		}

		public static RecoveryActionsRepository Instance
		{
			get
			{
				if (RecoveryActionsRepository.instance == null)
				{
					lock (RecoveryActionsRepository.initLock)
					{
						if (RecoveryActionsRepository.instance == null)
						{
							RecoveryActionsRepository.instance = new RecoveryActionsRepository();
						}
					}
				}
				return RecoveryActionsRepository.instance;
			}
		}

		internal string LocalServerName { get; set; }

		public void Initialize(Tuple<RecoveryActionEntry, RecoveryActionEntry> tuple, bool isCrimsonStoreEnabled = true, string localServerName = null)
		{
			if (this.isInitAttempted)
			{
				return;
			}
			lock (this)
			{
				if (!this.isInitAttempted)
				{
					this.isInitAttempted = true;
					this.isCrimsonStoreEnabled = isCrimsonStoreEnabled;
					this.LocalServerName = (localServerName ?? Dependencies.ThrottleHelper.Tunables.LocalMachineName);
					ThreadPool.QueueUserWorkItem(delegate(object o)
					{
						this.InitializeInternal(tuple);
					});
				}
			}
		}

		internal static RecoveryActionsRepository CreateTestInstance(bool isCrimsonStoreEnabled, string localServerName)
		{
			RecoveryActionsRepository recoveryActionsRepository = new RecoveryActionsRepository();
			recoveryActionsRepository.Initialize(null, isCrimsonStoreEnabled, localServerName);
			return recoveryActionsRepository;
		}

		internal static bool IsRecoveryInProgress(RpcGetThrottlingStatisticsImpl.ThrottlingStatistics ts)
		{
			if (ts.MostRecentEntry == null)
			{
				return false;
			}
			if (ts.MostRecentEntry.State != RecoveryActionState.Started)
			{
				return false;
			}
			if (ts.MostRecentEntry.LamProcessStartTime < ts.WorkerProcessStartTime)
			{
				return false;
			}
			if (ts.MostRecentEntry.LamProcessStartTime < ts.HostProcessStartTime)
			{
				return false;
			}
			DateTime localTime = ExDateTime.Now.LocalTime;
			return !(localTime > ts.MostRecentEntry.EndTime);
		}

		internal void PopulateEntriesFromCrimson(Tuple<RecoveryActionEntry, RecoveryActionEntry> tuple)
		{
			DateTime localTime = ExDateTime.Now.LocalTime;
			DateTime dateTime = localTime.AddHours(-25.0);
			DateTime dateTime2 = localTime.AddSeconds(2.0);
			int entriesCount = 0;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			TimeSpan timeSpan = TimeSpan.FromSeconds(300.0);
			try
			{
				bool flag;
				RecoveryActionHelper.ForeachMatching(delegate(RecoveryActionEntry entry, bool isNewestToOldest)
				{
					entriesCount++;
					this.AddEntryInternal(entry, false, isNewestToOldest);
					return false;
				}, RecoveryActionId.None, null, null, RecoveryActionState.None, RecoveryActionResult.None, dateTime, dateTime2, out flag, null, TimeSpan.FromSeconds(300.0), int.MaxValue);
				if (tuple != null)
				{
					RecoveryActionEntry item = tuple.Item1;
					if (item != null && !this.IsEntryExist(item))
					{
						this.AddEntryInternal(item, false, false);
						entriesCount++;
					}
					RecoveryActionEntry item2 = tuple.Item2;
					if (item2 != null && !this.IsEntryExist(item2))
					{
						this.AddEntryInternal(item2, false, false);
						entriesCount++;
					}
				}
				ManagedAvailabilityCrimsonEvents.RecoveryActionRepositoryInitSuccess.Log<string, int, DateTime, DateTime, TimeSpan, string>(stopwatch.Elapsed.ToString(), entriesCount, dateTime, dateTime2, timeSpan, "<none>");
			}
			catch (Exception ex)
			{
				ManagedAvailabilityCrimsonEvents.RecoveryActionRepositoryInitFailed.Log<TimeSpan, int, DateTime, DateTime, TimeSpan, string>(stopwatch.Elapsed, entriesCount, dateTime, dateTime2, timeSpan, ex.ToString());
			}
		}

		internal void WaitUntilInitializationComplete()
		{
			if (!this.isInitCompleted)
			{
				this.initCompleteEvent.WaitOne(TimeSpan.FromSeconds(300.0));
			}
		}

		internal void AddEntry(RecoveryActionEntry entry, bool isWritePersistentStore = true, bool isNewestToOldest = false)
		{
			this.WaitUntilInitializationComplete();
			this.AddEntryInternal(entry, this.isCrimsonStoreEnabled && isWritePersistentStore, isNewestToOldest);
		}

		internal void AddEntry(RecoveryActionHelper.RecoveryActionEntrySerializable serializedEntry, bool isWritePersistentStore = true, bool isNewestToOldest = false)
		{
			if (serializedEntry != null)
			{
				this.AddEntry(new RecoveryActionEntry(serializedEntry), this.isCrimsonStoreEnabled && isWritePersistentStore, isNewestToOldest);
			}
		}

		internal RpcGetThrottlingStatisticsImpl.ThrottlingStatistics GetThrottlingStatistics(RecoveryActionId actionId, string resourceName, int maxAllowedInOneHour, int maxAllowedInOneDay, bool isStopSearchWhenLimitExceeds = false, bool isCountFailedActions = false)
		{
			this.WaitUntilInitializationComplete();
			RpcGetThrottlingStatisticsImpl.ThrottlingStatistics throttlingStatistics = new RpcGetThrottlingStatisticsImpl.ThrottlingStatistics();
			throttlingStatistics.ServerName = this.LocalServerName;
			DateTime localTime = ExDateTime.Now.LocalTime;
			DateTime dayCutoffTime = localTime.AddDays(-1.0);
			DateTime hourCutoffTime = localTime.AddHours(-1.0);
			int totalActionsInDay = 0;
			int totalActionsInHour = 0;
			RecoveryActionEntry mostRecentEntry = null;
			RecoveryActionEntry entryExceedingOneHourLimit = null;
			RecoveryActionEntry entryExceedingOneDayLimit = null;
			int totalEntriesSearched = 0;
			this.SearchEntries(actionId, resourceName, dayCutoffTime, localTime, delegate(RecoveryActionEntry entry)
			{
				totalEntriesSearched++;
				if (entry.State == RecoveryActionState.Started && mostRecentEntry == null)
				{
					mostRecentEntry = entry;
				}
				if (entry.State == RecoveryActionState.Finished && (entry.Result == RecoveryActionResult.Succeeded || (isCountFailedActions && entry.Result == RecoveryActionResult.Failed)))
				{
					if (mostRecentEntry == null)
					{
						mostRecentEntry = entry;
					}
					if (entry.EndTime > hourCutoffTime)
					{
						totalActionsInHour++;
						if (maxAllowedInOneHour != -1 && totalActionsInHour == maxAllowedInOneHour)
						{
							if (entryExceedingOneHourLimit == null)
							{
								entryExceedingOneHourLimit = entry;
							}
							if (isStopSearchWhenLimitExceeds)
							{
								return false;
							}
						}
					}
					if (entry.EndTime > dayCutoffTime)
					{
						totalActionsInDay++;
						if (maxAllowedInOneDay != -1 && totalActionsInDay == maxAllowedInOneDay)
						{
							if (entryExceedingOneDayLimit == null)
							{
								entryExceedingOneDayLimit = entry;
							}
							if (isStopSearchWhenLimitExceeds)
							{
								return false;
							}
						}
					}
				}
				return true;
			});
			throttlingStatistics.TotalEntriesSearched = totalEntriesSearched;
			throttlingStatistics.NumberOfActionsInOneHour = totalActionsInHour;
			throttlingStatistics.NumberOfActionsInOneDay = totalActionsInDay;
			throttlingStatistics.MostRecentEntry = RecoveryActionHelper.CreateSerializableRecoveryActionEntry(mostRecentEntry);
			throttlingStatistics.EntryExceedingOneHourLimit = RecoveryActionHelper.CreateSerializableRecoveryActionEntry(entryExceedingOneHourLimit);
			throttlingStatistics.EntryExceedingOneDayLimit = RecoveryActionHelper.CreateSerializableRecoveryActionEntry(entryExceedingOneDayLimit);
			throttlingStatistics.WorkerProcessStartTime = WorkerProcessRepository.Instance.GetWorkerProcessStartTime();
			throttlingStatistics.HostProcessStartTime = this.localProcessTime;
			throttlingStatistics.SystemBootTime = RecoveryActionHelper.GetSystemBootTime();
			RpcSetThrottlingInProgressImpl.ThrottlingProgressInfo throttlingProgressInfo = this.throttlingProgressRepository.Get(actionId, resourceName);
			throttlingStatistics.ThrottleProgressInfo = throttlingProgressInfo;
			throttlingStatistics.IsThrottlingInProgress = (throttlingProgressInfo != null && throttlingProgressInfo.IsInProgress(throttlingStatistics.WorkerProcessStartTime));
			throttlingStatistics.IsRecoveryInProgress = RecoveryActionsRepository.IsRecoveryInProgress(throttlingStatistics);
			return throttlingStatistics;
		}

		internal RpcSetThrottlingInProgressImpl.Reply UpdateThrottlingProgress(RpcSetThrottlingInProgressImpl.Request request)
		{
			return this.throttlingProgressRepository.Update(request);
		}

		internal bool IsEntryExist(RecoveryActionEntry entry)
		{
			bool isFound = false;
			this.SearchEntries(entry.Id, null, DateTime.MinValue, DateTime.MaxValue, delegate(RecoveryActionEntry tmpEntry)
			{
				if (string.Equals(entry.InstanceId, tmpEntry.InstanceId, StringComparison.OrdinalIgnoreCase) && entry.State == tmpEntry.State)
				{
					isFound = true;
					return false;
				}
				return true;
			});
			return isFound;
		}

		internal void SearchEntries(RecoveryActionId actionId, string resourceName, DateTime startTime, DateTime endTime, Func<RecoveryActionEntry, bool> func)
		{
			RecoveryActionsRepository.RecoveryActionList recoveryActionList = null;
			if (this.map.TryGetValue(actionId, out recoveryActionList))
			{
				recoveryActionList.ForEachUntil(delegate(RecoveryActionEntry entry)
				{
					if (resourceName != null && !string.Equals(resourceName, entry.ResourceName))
					{
						return true;
					}
					DateTime t = (entry.State == RecoveryActionState.Finished) ? entry.EndTime : entry.StartTime;
					return t < startTime || t > endTime || func(entry);
				});
			}
		}

		private void InitializeInternal(Tuple<RecoveryActionEntry, RecoveryActionEntry> tuple)
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				this.localProcessTime = currentProcess.StartTime;
			}
			this.throttlingProgressRepository = new ThrottlingProgressRepository();
			if (this.isCrimsonStoreEnabled)
			{
				this.PopulateEntriesFromCrimson(tuple);
			}
			this.isInitCompleted = true;
			this.initCompleteEvent.Set();
		}

		private void AddEntryInternal(RecoveryActionEntry entry, bool isWritePersistentStore, bool isNewestToOldest)
		{
			RecoveryActionsRepository.RecoveryActionList orAdd = this.map.GetOrAdd(entry.Id, (RecoveryActionId id) => new RecoveryActionsRepository.RecoveryActionList());
			orAdd.Add(entry, this.isCrimsonStoreEnabled && isWritePersistentStore, isNewestToOldest);
		}

		internal const int HoursToKeepInMemory = 25;

		internal const int SecondsToCompleteInit = 300;

		private static readonly object initLock = new object();

		private static RecoveryActionsRepository instance = null;

		private readonly ManualResetEvent initCompleteEvent = new ManualResetEvent(false);

		private ConcurrentDictionary<RecoveryActionId, RecoveryActionsRepository.RecoveryActionList> map = new ConcurrentDictionary<RecoveryActionId, RecoveryActionsRepository.RecoveryActionList>();

		private DateTime localProcessTime;

		private bool isInitAttempted;

		private bool isInitCompleted;

		private bool isCrimsonStoreEnabled = true;

		private ThrottlingProgressRepository throttlingProgressRepository;

		internal class RecoveryActionList
		{
			internal RecoveryActionList()
			{
				this.rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
				this.Entries = new LinkedList<RecoveryActionEntry>();
			}

			internal LinkedList<RecoveryActionEntry> Entries { get; set; }

			internal DateTime GetEntryTime(RecoveryActionEntry entry)
			{
				return (entry.State == RecoveryActionState.Finished) ? entry.EndTime : entry.StartTime;
			}

			internal void Add(RecoveryActionEntry entryToInsert, bool isWritePersistentStore, bool isNewestToOldest = true)
			{
				try
				{
					this.rwLock.EnterWriteLock();
					if (isWritePersistentStore)
					{
						entryToInsert.Write(null);
					}
					DateTime entryTime = this.GetEntryTime(entryToInsert);
					if (isNewestToOldest)
					{
						LinkedListNode<RecoveryActionEntry> linkedListNode = this.Entries.Last;
						while (linkedListNode != null)
						{
							DateTime entryTime2 = this.GetEntryTime(linkedListNode.Value);
							if (entryTime <= entryTime2)
							{
								if (entryTime == entryTime2 && entryToInsert.LocalDataAccessMetaData != null && linkedListNode.Value.LocalDataAccessMetaData != null && linkedListNode.Value.LocalDataAccessMetaData.RecordId < entryToInsert.LocalDataAccessMetaData.RecordId)
								{
									this.Entries.AddBefore(linkedListNode, entryToInsert);
									break;
								}
								this.Entries.AddAfter(linkedListNode, entryToInsert);
								break;
							}
							else
							{
								linkedListNode = linkedListNode.Previous;
							}
						}
						if (linkedListNode == null)
						{
							this.Entries.AddFirst(entryToInsert);
						}
					}
					else
					{
						LinkedListNode<RecoveryActionEntry> linkedListNode = this.Entries.First;
						while (linkedListNode != null)
						{
							DateTime entryTime3 = this.GetEntryTime(linkedListNode.Value);
							if (entryTime >= entryTime3)
							{
								if (entryTime == entryTime3 && entryToInsert.LocalDataAccessMetaData != null && linkedListNode.Value.LocalDataAccessMetaData != null && linkedListNode.Value.LocalDataAccessMetaData.RecordId > entryToInsert.LocalDataAccessMetaData.RecordId)
								{
									this.Entries.AddAfter(linkedListNode, entryToInsert);
									break;
								}
								this.Entries.AddBefore(linkedListNode, entryToInsert);
								break;
							}
							else
							{
								linkedListNode = linkedListNode.Next;
							}
						}
						if (linkedListNode == null)
						{
							this.Entries.AddLast(entryToInsert);
						}
					}
					this.PurgeOldEntriesIfRequired();
				}
				finally
				{
					this.rwLock.ExitWriteLock();
				}
			}

			internal void ForEachUntil(Func<RecoveryActionEntry, bool> func)
			{
				try
				{
					this.rwLock.EnterReadLock();
					foreach (RecoveryActionEntry arg in this.Entries)
					{
						if (!func(arg))
						{
							break;
						}
					}
				}
				finally
				{
					this.rwLock.ExitReadLock();
				}
			}

			private void PurgeOldEntriesIfRequired()
			{
				LinkedListNode<RecoveryActionEntry> linkedListNode = this.Entries.Last;
				DateTime t = ExDateTime.Now.LocalTime.AddHours(-25.0);
				while (linkedListNode != null)
				{
					DateTime entryTime = this.GetEntryTime(linkedListNode.Value);
					if (entryTime >= t)
					{
						return;
					}
					LinkedListNode<RecoveryActionEntry> previous = linkedListNode.Previous;
					this.Entries.Remove(linkedListNode);
					linkedListNode = previous;
				}
			}

			private readonly ReaderWriterLockSlim rwLock;
		}
	}
}
