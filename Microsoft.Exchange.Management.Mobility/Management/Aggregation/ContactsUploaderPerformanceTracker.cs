using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Win32;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Management.Aggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ContactsUploaderPerformanceTracker : IContactsUploaderPerformanceTracker, ILogEvent
	{
		public ContactsUploaderPerformanceTracker()
		{
			this.stopwatch = new Stopwatch();
			this.internalState = ContactsUploaderPerformanceTracker.InternalState.Stopped;
			this.OperationResult = "Succeded";
		}

		public int ReceivedContactsCount { get; set; }

		public double ExportedDataSize { get; set; }

		public string OperationResult { get; set; }

		public string EventId
		{
			get
			{
				return "PerformanceData";
			}
		}

		public void Start()
		{
			this.EnforceInternalState(ContactsUploaderPerformanceTracker.InternalState.Stopped, "Start");
			this.elapsedTimeBookmarks = new List<ContactsUploaderPerformanceTracker.ElapsedTimeBookmark>();
			this.stopwatch.Start();
			this.startThreadTimes = ThreadTimes.GetFromCurrentThread();
			this.startStorePerformanceData = RpcDataProvider.Instance.TakeSnapshot(true);
			this.internalState = ContactsUploaderPerformanceTracker.InternalState.Started;
		}

		public void IncrementContactsRead()
		{
			this.contactsRead++;
		}

		public void IncrementContactsExported()
		{
			this.contactsExported++;
		}

		public void AddTimeBookmark(ContactsUploaderPerformanceTrackerBookmarks activity)
		{
			this.EnforceInternalState(ContactsUploaderPerformanceTracker.InternalState.Started, "AddTimeBookmark");
			this.elapsedTimeBookmarks.Add(new ContactsUploaderPerformanceTracker.ElapsedTimeBookmark
			{
				Activity = activity,
				ElapsedTime = this.stopwatch.Elapsed
			});
		}

		public void Stop()
		{
			this.EnforceInternalState(ContactsUploaderPerformanceTracker.InternalState.Started, "Stop");
			this.stopwatch.Stop();
			ThreadTimes fromCurrentThread = ThreadTimes.GetFromCurrentThread();
			PerformanceData pd = RpcDataProvider.Instance.TakeSnapshot(false);
			this.internalState = ContactsUploaderPerformanceTracker.InternalState.Stopped;
			this.cpuTime += fromCurrentThread.Kernel - this.startThreadTimes.Kernel + (fromCurrentThread.User - this.startThreadTimes.User);
			PerformanceData performanceData = pd - this.startStorePerformanceData;
			this.storeRpcLatency += performanceData.Latency;
			this.storeRpcCount += (int)performanceData.Count;
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			this.EnforceInternalState(ContactsUploaderPerformanceTracker.InternalState.Stopped, "GetEventData");
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>
			{
				ContactsUploaderPerformanceTracker.CreateEventData(ContactsUploaderPerformanceTrackerSchema.ContactsRead, this.contactsRead),
				ContactsUploaderPerformanceTracker.CreateEventData(ContactsUploaderPerformanceTrackerSchema.ContactsExported, this.contactsExported),
				ContactsUploaderPerformanceTracker.CreateEventData(ContactsUploaderPerformanceTrackerSchema.ContactsReceived, this.ReceivedContactsCount),
				ContactsUploaderPerformanceTracker.CreateEventData(ContactsUploaderPerformanceTrackerSchema.DataSize, this.ExportedDataSize),
				ContactsUploaderPerformanceTracker.CreateEventData(ContactsUploaderPerformanceTrackerSchema.RpcCount, this.storeRpcCount),
				ContactsUploaderPerformanceTracker.CreateEventData(ContactsUploaderPerformanceTrackerSchema.RpcLatency, this.storeRpcLatency.TotalMilliseconds),
				ContactsUploaderPerformanceTracker.CreateEventData(ContactsUploaderPerformanceTrackerSchema.CpuTime, this.cpuTime.TotalMilliseconds),
				ContactsUploaderPerformanceTracker.CreateEventData(ContactsUploaderPerformanceTrackerSchema.Result, this.OperationResult)
			};
			this.AddBookmarkData(list);
			return list;
		}

		private static KeyValuePair<string, object> CreateEventData(ContactsUploaderPerformanceTrackerSchema field, object value)
		{
			return new KeyValuePair<string, object>(DisplayNameAttribute.GetEnumName(field), value);
		}

		private void AddBookmarkData(List<KeyValuePair<string, object>> eventData)
		{
			ArgumentValidator.ThrowIfNull("eventData", eventData);
			TimeSpan t = default(TimeSpan);
			foreach (ContactsUploaderPerformanceTracker.ElapsedTimeBookmark elapsedTimeBookmark in this.elapsedTimeBookmarks)
			{
				double totalMilliseconds = (elapsedTimeBookmark.ElapsedTime - t).TotalMilliseconds;
				eventData.Add(new KeyValuePair<string, object>(DisplayNameAttribute.GetEnumName(elapsedTimeBookmark.Activity), totalMilliseconds));
				t = elapsedTimeBookmark.ElapsedTime;
			}
		}

		private void EnforceInternalState(ContactsUploaderPerformanceTracker.InternalState expectedState, string action)
		{
			if (this.internalState != expectedState)
			{
				string message = string.Format("{0} can only be performed when state is {1}. Present state is {2}", action, expectedState, this.internalState);
				ExWatson.SendReport(new InvalidOperationException(message), ReportOptions.None, null);
			}
		}

		private const string DefaultOperationResult = "Succeded";

		private readonly Stopwatch stopwatch;

		private ThreadTimes startThreadTimes;

		private PerformanceData startStorePerformanceData;

		private int contactsRead;

		private int contactsExported;

		private List<ContactsUploaderPerformanceTracker.ElapsedTimeBookmark> elapsedTimeBookmarks;

		private TimeSpan cpuTime;

		private TimeSpan storeRpcLatency;

		private int storeRpcCount;

		private ContactsUploaderPerformanceTracker.InternalState internalState;

		private enum InternalState
		{
			Stopped,
			Started
		}

		private struct ElapsedTimeBookmark
		{
			public ContactsUploaderPerformanceTrackerBookmarks Activity;

			public TimeSpan ElapsedTime;
		}
	}
}
