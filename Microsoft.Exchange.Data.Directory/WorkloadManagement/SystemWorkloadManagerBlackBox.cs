using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class SystemWorkloadManagerBlackBox
	{
		static SystemWorkloadManagerBlackBox()
		{
			string name = "WorkloadManager.MaxHistoryDepth";
			IntAppSettingsEntry intAppSettingsEntry = new IntAppSettingsEntry(name, SystemWorkloadManagerBlackBox.maxHistoryDepth, ExTraceGlobals.PoliciesTracer);
			SystemWorkloadManagerBlackBox.maxHistoryDepth = intAppSettingsEntry.Value;
		}

		public static void AddActiveClassification(WorkloadClassification classification)
		{
			lock (SystemWorkloadManagerBlackBox.activeLock)
			{
				if (SystemWorkloadManagerBlackBox.active == null || !SystemWorkloadManagerBlackBox.active.Contains(classification))
				{
					HashSet<WorkloadClassification> hashSet;
					if (SystemWorkloadManagerBlackBox.active == null)
					{
						hashSet = new HashSet<WorkloadClassification>();
					}
					else
					{
						hashSet = new HashSet<WorkloadClassification>(SystemWorkloadManagerBlackBox.active);
					}
					hashSet.Add(classification);
					SystemWorkloadManagerBlackBox.active = hashSet;
				}
			}
		}

		public static void RecordMonitorUpdate(ref SystemWorkloadManagerLogEntry lastEntry, ResourceKey resource, WorkloadClassification classification, ResourceLoad load)
		{
			SystemWorkloadManagerBlackBox.Record(ref lastEntry, SystemWorkloadManagerLogEntryType.Monitor, resource, classification, load, -1, false);
		}

		public static void RecordAdmissionUpdate(ref SystemWorkloadManagerLogEntry lastEntry, ResourceKey resource, WorkloadClassification classification, ResourceLoad load, int slots, bool delayed)
		{
			SystemWorkloadManagerBlackBox.Record(ref lastEntry, SystemWorkloadManagerLogEntryType.Admission, resource, classification, load, slots, delayed);
		}

		public static SystemWorkloadManagerLogEntry[] GetRecords(bool clear = false)
		{
			if (clear)
			{
				Queue<SystemWorkloadManagerLogEntry> queue = null;
				lock (SystemWorkloadManagerBlackBox.history)
				{
					queue = SystemWorkloadManagerBlackBox.history;
					SystemWorkloadManagerBlackBox.history = new Queue<SystemWorkloadManagerLogEntry>();
				}
				return queue.ToArray();
			}
			SystemWorkloadManagerLogEntry[] result;
			lock (SystemWorkloadManagerBlackBox.history)
			{
				result = SystemWorkloadManagerBlackBox.history.ToArray();
			}
			return result;
		}

		private static void Record(ref SystemWorkloadManagerLogEntry lastEntry, SystemWorkloadManagerLogEntryType type, ResourceKey resource, WorkloadClassification classification, ResourceLoad load, int slots, bool delayed)
		{
			if ((SystemWorkloadManagerBlackBox.active == null || SystemWorkloadManagerBlackBox.active.Contains(classification)) && (lastEntry == null || lastEntry.CurrentEvent.Load.State != load.State || lastEntry.CurrentEvent.Slots != slots || lastEntry.CurrentEvent.Delayed != delayed))
			{
				SystemWorkloadManagerEvent currentEvent = new SystemWorkloadManagerEvent(load, slots, delayed);
				lock (SystemWorkloadManagerBlackBox.history)
				{
					while (SystemWorkloadManagerBlackBox.history.Count >= SystemWorkloadManagerBlackBox.maxHistoryDepth)
					{
						SystemWorkloadManagerBlackBox.history.Dequeue();
					}
					if (lastEntry == null)
					{
						lastEntry = new SystemWorkloadManagerLogEntry(type, resource, classification, currentEvent, null);
						SystemWorkloadManagerBlackBox.history.Enqueue(lastEntry);
					}
					else
					{
						lastEntry = new SystemWorkloadManagerLogEntry(type, resource, classification, currentEvent, lastEntry.CurrentEvent);
						SystemWorkloadManagerBlackBox.history.Enqueue(lastEntry);
					}
				}
			}
		}

		private static int maxHistoryDepth = 1000;

		private static Queue<SystemWorkloadManagerLogEntry> history = new Queue<SystemWorkloadManagerLogEntry>();

		private static HashSet<WorkloadClassification> active = null;

		private static object activeLock = new object();
	}
}
