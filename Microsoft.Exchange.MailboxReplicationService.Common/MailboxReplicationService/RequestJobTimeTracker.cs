using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class RequestJobTimeTracker : XMLSerializableBase
	{
		public RequestJobTimeTracker()
		{
			this.durations = new Dictionary<RequestState, RequestJobDurationData>();
			this.timestamps = new Dictionary<RequestJobTimestamp, DateTime>();
			this.curState = RequestState.None;
			this.stateChangeTimestamp = (DateTime)ExDateTime.UtcNow;
		}

		private RequestJobTimeTracker(SerializationInfo info, StreamingContext context)
		{
		}

		[XmlIgnore]
		public RequestState CurrentState
		{
			get
			{
				return this.curState;
			}
			set
			{
				this.SetState(value);
			}
		}

		internal void SetState(RequestState value)
		{
			if (this.curState == value)
			{
				return;
			}
			DateTime utcNow = TimeProvider.UtcNow;
			TimeSpan duration = utcNow.Subtract(this.stateChangeTimestamp);
			this.RefreshDurations();
			if (RequestJobTimeTracker.SupportDurationTracking(this.curState))
			{
				RequestJobDurationData requestJobDurationData;
				if (!this.durations.TryGetValue(this.curState, out requestJobDurationData))
				{
					requestJobDurationData = new RequestJobDurationData(this.curState);
				}
				requestJobDurationData.AddTime(duration);
				this.durations[this.curState] = requestJobDurationData;
			}
			this.stateChangeTimestamp = utcNow;
			this.curState = value;
			if (RequestJobTimeTracker.SupportDurationTracking(this.curState))
			{
				this.UpdateActiveCounts(this.curState, 1L);
				this.IncRateCounts(this.curState);
			}
		}

		[XmlElement("CurrentState")]
		public int CurrentStateInt
		{
			get
			{
				return (int)this.curState;
			}
			set
			{
				this.curState = (RequestState)value;
			}
		}

		[XmlElement("StateChangeTimestamp")]
		public long StateChangeTimestampInt
		{
			get
			{
				return this.stateChangeTimestamp.Ticks;
			}
			set
			{
				this.stateChangeTimestamp = new DateTime(value);
			}
		}

		[XmlArrayItem("D")]
		[XmlArray("Durations")]
		public RequestJobDurationData[] Durations
		{
			get
			{
				RequestJobDurationData[] array = new RequestJobDurationData[this.durations.Count];
				int num = 0;
				foreach (KeyValuePair<RequestState, RequestJobDurationData> keyValuePair in this.durations)
				{
					array[num++] = keyValuePair.Value;
				}
				return array;
			}
			set
			{
				this.durations.Clear();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						RequestJobDurationData requestJobDurationData = value[i];
						this.durations[requestJobDurationData.State] = requestJobDurationData;
					}
				}
			}
		}

		[XmlArrayItem("TS")]
		[XmlArray("Timestamps")]
		public RequestJobTimestampData[] Timestamps
		{
			get
			{
				RequestJobTimestampData[] array = new RequestJobTimestampData[this.timestamps.Count];
				int num = 0;
				foreach (KeyValuePair<RequestJobTimestamp, DateTime> keyValuePair in this.timestamps)
				{
					array[num++] = new RequestJobTimestampData(keyValuePair.Key, keyValuePair.Value);
				}
				return array;
			}
			set
			{
				this.timestamps.Clear();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						RequestJobTimestampData requestJobTimestampData = value[i];
						this.timestamps[requestJobTimestampData.Id] = requestJobTimestampData.Timestamp;
					}
				}
			}
		}

		public static bool SupportDurationTracking(RequestState requestState)
		{
			return requestState != RequestState.None && requestState != RequestState.InitialSeedingComplete && requestState != RequestState.Removed;
		}

		public static bool SupportTimestampTracking(RequestJobTimestamp timestamp)
		{
			if (timestamp != RequestJobTimestamp.None)
			{
				switch (timestamp)
				{
				case RequestJobTimestamp.DomainControllerUpdate:
				case RequestJobTimestamp.RequestCanceled:
				case RequestJobTimestamp.LastSuccessfulSourceConnection:
				case RequestJobTimestamp.LastSuccessfulTargetConnection:
				case RequestJobTimestamp.SourceConnectionFailure:
				case RequestJobTimestamp.TargetConnectionFailure:
				case RequestJobTimestamp.IsIntegStarted:
				case RequestJobTimestamp.LastServerBusyBackoff:
				case RequestJobTimestamp.ServerBusyBackoffUntil:
				case RequestJobTimestamp.MaxTimestamp:
					break;
				default:
					return true;
				}
			}
			return false;
		}

		public static void MergeTimestamps(RequestJobTimeTracker mrsTimeTracker, RequestJobTimeTracker other)
		{
			foreach (RequestJobTimestampData requestJobTimestampData in other.Timestamps)
			{
				if (!RequestJobTimeTracker.mrsMasteredTimestamps.Contains(requestJobTimestampData.Id))
				{
					if (RequestJobTimeTracker.nonMrsMasteredTimestamps.Contains(requestJobTimestampData.Id))
					{
						mrsTimeTracker.SetTimestamp(requestJobTimestampData.Id, new DateTime?(requestJobTimestampData.Timestamp));
					}
					else
					{
						DateTime? timestamp = mrsTimeTracker.GetTimestamp(requestJobTimestampData.Id);
						if (timestamp == null || timestamp.Value < requestJobTimestampData.Timestamp)
						{
							mrsTimeTracker.SetTimestamp(requestJobTimestampData.Id, new DateTime?(requestJobTimestampData.Timestamp));
						}
					}
				}
			}
			foreach (RequestJobTimestamp ts in RequestJobTimeTracker.nonMrsMasteredTimestamps)
			{
				if (other.GetTimestamp(ts) == null)
				{
					mrsTimeTracker.SetTimestamp(ts, null);
				}
			}
		}

		public void AddDurationToState(TimeSpan duration, RequestState requestState)
		{
			RequestJobDurationData requestJobDurationData;
			if (!this.durations.TryGetValue(requestState, out requestJobDurationData))
			{
				requestJobDurationData = new RequestJobDurationData(requestState);
			}
			requestJobDurationData.AddTime(duration);
			this.durations[requestState] = requestJobDurationData;
		}

		public TimeSpan GetCurrentDurationChunk()
		{
			return DateTime.UtcNow - this.stateChangeTimestamp;
		}

		public RequestJobDurationData GetDuration(RequestState state)
		{
			RequestJobDurationData requestJobDurationData = null;
			RequestJobDurationData requestJobDurationData2;
			if (this.durations.TryGetValue(state, out requestJobDurationData2))
			{
				requestJobDurationData = requestJobDurationData2;
			}
			if (state == this.curState)
			{
				DateTime utcNow = TimeProvider.UtcNow;
				TimeSpan duration = utcNow.Subtract(this.stateChangeTimestamp);
				if (requestJobDurationData == null)
				{
					requestJobDurationData = new RequestJobDurationData(state);
					requestJobDurationData.AddTime(duration);
					this.durations[this.curState] = requestJobDurationData;
				}
				else
				{
					requestJobDurationData.AddTime(duration);
				}
				this.stateChangeTimestamp = utcNow;
			}
			RequestJobStateNode state2 = RequestJobStateNode.GetState(state);
			if (state2 != null)
			{
				foreach (RequestJobStateNode requestJobStateNode in state2.Children)
				{
					RequestJobDurationData duration2 = this.GetDuration(requestJobStateNode.MRState);
					if (duration2 != null)
					{
						requestJobDurationData += duration2;
					}
				}
			}
			return requestJobDurationData;
		}

		public RequestJobDurationData GetDisplayDuration(RequestState state)
		{
			RequestJobDurationData duration = this.GetDuration(state);
			if (duration == null)
			{
				return new RequestJobDurationData(state);
			}
			return duration;
		}

		public EnhancedTimeSpan? GetDisplayDuration(RequestState[] states)
		{
			EnhancedTimeSpan? result = null;
			foreach (RequestState state in states)
			{
				RequestJobDurationData displayDuration = this.GetDisplayDuration(state);
				if (displayDuration != null)
				{
					result = new EnhancedTimeSpan?(displayDuration.Duration + ((result != null) ? result.Value : EnhancedTimeSpan.Zero));
				}
			}
			return result;
		}

		public void SetTimestamp(RequestJobTimestamp ts, DateTime? value)
		{
			if (value == null)
			{
				if (this.timestamps.ContainsKey(ts))
				{
					this.timestamps.Remove(ts);
					return;
				}
			}
			else
			{
				this.timestamps[ts] = value.Value;
			}
		}

		public DateTime? GetTimestamp(RequestJobTimestamp ts)
		{
			DateTime value;
			if (this.timestamps.TryGetValue(ts, out value))
			{
				return new DateTime?(value);
			}
			return null;
		}

		public void GetThrottledDurations(out ThrottleDurations sourceDurations, out ThrottleDurations targetDurations)
		{
			sourceDurations = new ThrottleDurations(this.GetNonEmptyDuration(RequestState.StalledDueToReadThrottle), this.GetNonEmptyDuration(RequestState.StalledDueToReadCpu), EnhancedTimeSpan.Zero, EnhancedTimeSpan.Zero, this.GetNonEmptyDuration(RequestState.StalledDueToReadUnknown));
			targetDurations = new ThrottleDurations(this.GetNonEmptyDuration(RequestState.StalledDueToWriteThrottle), this.GetNonEmptyDuration(RequestState.StalledDueToWriteCpu), this.GetNonEmptyDuration(RequestState.StalledDueToHA), this.GetNonEmptyDuration(RequestState.StalledDueToCI), this.GetNonEmptyDuration(RequestState.StalledDueToWriteUnknown));
		}

		public DateTime? GetDisplayTimestamp(RequestJobTimestamp ts)
		{
			DateTime? timestamp = this.GetTimestamp(ts);
			if (timestamp == null)
			{
				return null;
			}
			return new DateTime?(timestamp.Value.ToLocalTime());
		}

		public override string ToString()
		{
			this.RefreshDurations();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Current state: {0}\n", this.CurrentState);
			stringBuilder.AppendFormat("Last state change: {0}\n", this.stateChangeTimestamp);
			stringBuilder.AppendLine("Durations (per-slot data is in milliseconds):");
			foreach (RequestJobStateNode treeRoot in RequestJobStateNode.RootStates)
			{
				this.DumpDurationTree(stringBuilder, treeRoot, string.Empty);
			}
			stringBuilder.AppendLine("Timestamps:");
			for (RequestJobTimestamp requestJobTimestamp = RequestJobTimestamp.None; requestJobTimestamp < RequestJobTimestamp.MaxTimestamp; requestJobTimestamp++)
			{
				stringBuilder.AppendFormat("{0} {1}\n", requestJobTimestamp, this.GetDisplayTimestamp(requestJobTimestamp));
			}
			return stringBuilder.ToString();
		}

		internal RequestJobTimeTrackerXML GetDiagnosticInfo(RequestStatisticsDiagnosticArgument arguments)
		{
			bool showTimeSlots = arguments.HasArgument("showtimeslots");
			this.RefreshDurations();
			RequestJobTimeTrackerXML requestJobTimeTrackerXML = new RequestJobTimeTrackerXML();
			requestJobTimeTrackerXML.CurrentState = this.CurrentState.ToString();
			requestJobTimeTrackerXML.LastStateChangeTimeStamp = this.stateChangeTimestamp.ToString("O");
			for (RequestJobTimestamp requestJobTimestamp = RequestJobTimestamp.None; requestJobTimestamp < RequestJobTimestamp.MaxTimestamp; requestJobTimestamp++)
			{
				DateTime? timestamp = this.GetTimestamp(requestJobTimestamp);
				if (timestamp != null)
				{
					requestJobTimeTrackerXML.AddTimestamp(requestJobTimestamp, timestamp.Value);
				}
			}
			foreach (RequestJobStateNode treeRoot in RequestJobStateNode.RootStates)
			{
				RequestJobTimeTrackerXML.DurationRec durationRec = this.DumpDurationTreeForDiagnostics(treeRoot, showTimeSlots);
				if (durationRec != null && durationRec.Duration != TimeSpan.Zero.ToString())
				{
					if (requestJobTimeTrackerXML.Durations == null)
					{
						requestJobTimeTrackerXML.Durations = new List<RequestJobTimeTrackerXML.DurationRec>();
					}
					requestJobTimeTrackerXML.Durations.Add(durationRec);
				}
			}
			return requestJobTimeTrackerXML;
		}

		internal void AttachPerfCounters(MDBPerfCounterHelper pcHelper)
		{
			this.pcHelper = pcHelper;
			this.UpdateActiveCounts(this.CurrentState, 1L);
		}

		private void RefreshDurations()
		{
			DateTime utcNow = TimeProvider.UtcNow;
			if (utcNow <= this.stateChangeTimestamp)
			{
				return;
			}
			foreach (KeyValuePair<RequestState, RequestJobDurationData> keyValuePair in this.durations)
			{
				keyValuePair.Value.Refresh(this.stateChangeTimestamp);
			}
		}

		private TimeSpan GetNonEmptyDuration(RequestState state)
		{
			RequestJobDurationData duration = this.GetDuration(state);
			if (duration != null)
			{
				return duration.Duration;
			}
			return TimeSpan.Zero;
		}

		private void UpdateActiveCounts(RequestState rjState, long delta)
		{
			if (this.pcHelper == null)
			{
				return;
			}
			for (RequestJobStateNode requestJobStateNode = RequestJobStateNode.GetState(rjState); requestJobStateNode != null; requestJobStateNode = requestJobStateNode.Parent)
			{
				if (requestJobStateNode.GetActivePerfCounter != null)
				{
					requestJobStateNode.GetActivePerfCounter(this.pcHelper.PerfCounter).IncrementBy(delta);
				}
			}
		}

		private void IncRateCounts(RequestState rjState)
		{
			if (this.pcHelper == null)
			{
				return;
			}
			for (RequestJobStateNode requestJobStateNode = RequestJobStateNode.GetState(rjState); requestJobStateNode != null; requestJobStateNode = requestJobStateNode.Parent)
			{
				if (requestJobStateNode.GetCountRatePerfCounter != null)
				{
					requestJobStateNode.GetCountRatePerfCounter(this.pcHelper).IncrementBy(1L);
				}
			}
		}

		private void DumpDurationTree(StringBuilder strBuilder, RequestJobStateNode treeRoot, string indent)
		{
			strBuilder.AppendFormat("{0}{1}: {2}\n", indent, treeRoot.MRState.ToString(), this.GetDisplayDuration(treeRoot.MRState));
			indent += "  ";
			foreach (RequestJobStateNode treeRoot2 in treeRoot.Children)
			{
				this.DumpDurationTree(strBuilder, treeRoot2, indent);
			}
		}

		private RequestJobTimeTrackerXML.DurationRec DumpDurationTreeForDiagnostics(RequestJobStateNode treeRoot, bool showTimeSlots = false)
		{
			RequestJobDurationData displayDuration = this.GetDisplayDuration(treeRoot.MRState);
			if (displayDuration == null)
			{
				return null;
			}
			RequestJobTimeTrackerXML.DurationRec durationRec = displayDuration.GetDurationRec(treeRoot.MRState, showTimeSlots);
			foreach (RequestJobStateNode treeRoot2 in treeRoot.Children)
			{
				RequestJobTimeTrackerXML.DurationRec durationRec2 = this.DumpDurationTreeForDiagnostics(treeRoot2, showTimeSlots);
				if (durationRec2 != null && durationRec2.Duration != TimeSpan.Zero.ToString())
				{
					if (durationRec.ChildNodes == null)
					{
						durationRec.ChildNodes = new List<RequestJobTimeTrackerXML.DurationRec>();
					}
					durationRec.ChildNodes.Add(durationRec2);
				}
			}
			return durationRec;
		}

		private static readonly List<RequestJobTimestamp> mrsMasteredTimestamps = new List<RequestJobTimestamp>(1)
		{
			RequestJobTimestamp.MailboxLocked
		};

		private static readonly List<RequestJobTimestamp> nonMrsMasteredTimestamps = new List<RequestJobTimestamp>(2)
		{
			RequestJobTimestamp.CompleteAfter,
			RequestJobTimestamp.StartAfter
		};

		private Dictionary<RequestState, RequestJobDurationData> durations;

		private Dictionary<RequestJobTimestamp, DateTime> timestamps;

		private RequestState curState;

		private DateTime stateChangeTimestamp;

		[NonSerialized]
		private MDBPerfCounterHelper pcHelper;
	}
}
