using System;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Transport.Sync.Worker.Health
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RemoteServerHealthData
	{
		internal RemoteServerHealthData(string serverName, TimeSpan slidingCounterWindowSize, TimeSpan slidingCounterBucketLength)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("serverName", serverName);
			this.serverName = serverName;
			this.slidingAverageLatencyCounter = new SlidingAverageCounter(slidingCounterWindowSize, slidingCounterBucketLength);
			this.Reset();
		}

		protected RemoteServerHealthData(string serverName, RemoteServerHealthState state, int backOffCount, ExDateTime lastUpdateTime, ExDateTime? lastBackOffStartTime, TimeSpan slidingCounterWindowSize, TimeSpan slidingCounterBucketLength)
		{
			this.serverName = serverName;
			this.state = state;
			this.backOffCount = backOffCount;
			this.lastUpdateTime = lastUpdateTime;
			this.lastBackOffStartTime = lastBackOffStartTime;
			this.slidingAverageLatencyCounter = new SlidingAverageCounter(slidingCounterWindowSize, slidingCounterBucketLength);
		}

		internal string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		internal RemoteServerHealthState State
		{
			get
			{
				return this.state;
			}
		}

		internal int BackOffCount
		{
			get
			{
				return this.backOffCount;
			}
		}

		internal ExDateTime LastUpdateTime
		{
			get
			{
				return this.lastUpdateTime;
			}
		}

		internal virtual ExDateTime? LastBackOffStartTime
		{
			get
			{
				return this.lastBackOffStartTime;
			}
		}

		internal static RemoteServerHealthData CreateRemoteServerHealthDataForViolatingServer(string serverName, RemoteServerHealthState state, int backOffCount, ExDateTime lastUpdateTime, ExDateTime lastBackOffStartTime, TimeSpan slidingCounterWindowSize, TimeSpan slidingCounterBucketLength)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("serverName", serverName);
			SyncUtilities.ThrowIfArgumentLessThanEqualToZero("backOffCount", backOffCount);
			SyncUtilities.ThrowIfArg1LessThenArg2("lastUpdateTime", lastUpdateTime, "lastBackOffStartTime", lastBackOffStartTime);
			return new RemoteServerHealthData(serverName, state, backOffCount, lastUpdateTime, new ExDateTime?(lastBackOffStartTime), slidingCounterWindowSize, slidingCounterBucketLength);
		}

		internal static bool TryCreateRemoteServerHealthDataForViolatingServer(string serverName, RemoteServerHealthState state, int backOffCount, ExDateTime lastUpdateTime, ExDateTime lastBackOffStartTime, TimeSpan slidingCounterWindowSize, TimeSpan slidingCounterBucketLength, out RemoteServerHealthData healthData, out Exception exception)
		{
			healthData = null;
			exception = null;
			bool result;
			try
			{
				healthData = RemoteServerHealthData.CreateRemoteServerHealthDataForViolatingServer(serverName, state, backOffCount, lastUpdateTime, lastBackOffStartTime, slidingCounterWindowSize, slidingCounterBucketLength);
				result = true;
			}
			catch (ArgumentException ex)
			{
				exception = ex;
				result = false;
			}
			return result;
		}

		internal TimeSpan TimeSinceLastUpdate()
		{
			return this.GetCurrentTime() - this.lastUpdateTime;
		}

		internal TimeSpan TimeSinceLastBackOff()
		{
			return this.GetCurrentTime() - this.LastBackOffStartTime.Value;
		}

		internal XElement GetDiagnosticInfo()
		{
			string content = (this.lastBackOffStartTime != null) ? this.TimeSinceLastBackOff().ToString() : null;
			long num = 0L;
			long averageLantency = this.GetAverageLantency(out num);
			return new XElement("HealthData", new object[]
			{
				new XElement("serverName", this.serverName),
				new XElement("state", this.state),
				new XElement("backOffCount", this.backOffCount),
				new XElement("lastUpdateTime", this.lastUpdateTime),
				new XElement("timeSinceLastUpdate", this.TimeSinceLastUpdate().ToString()),
				new XElement("lastBackOffStartTime", this.lastBackOffStartTime),
				new XElement("timeSinceLastBackOff", content),
				new XElement("averageLatency", averageLantency),
				new XElement("numberOfRoundtrips", num)
			});
		}

		internal virtual void RecordServerLatency(long latency)
		{
			this.slidingAverageLatencyCounter.AddValue(latency);
			this.lastUpdateTime = this.GetCurrentTime();
		}

		internal virtual long GetAverageLantency(out long numberOfRoundtrips)
		{
			return this.slidingAverageLatencyCounter.CalculateAverageAcrossAllSamples(out numberOfRoundtrips);
		}

		internal void MarkAsBackedOff()
		{
			this.state = RemoteServerHealthState.BackedOff;
			this.lastBackOffStartTime = new ExDateTime?(this.lastUpdateTime = this.GetCurrentTime());
			this.backOffCount++;
		}

		internal void MarkAsPoisonous()
		{
			this.state = RemoteServerHealthState.Poisonous;
			this.lastUpdateTime = this.GetCurrentTime();
		}

		internal void MarkAsClean()
		{
			this.state = RemoteServerHealthState.Clean;
			this.lastUpdateTime = this.GetCurrentTime();
		}

		internal void Reset()
		{
			this.state = RemoteServerHealthState.Clean;
			this.backOffCount = 0;
			this.lastBackOffStartTime = null;
			this.lastUpdateTime = this.GetCurrentTime();
		}

		protected virtual ExDateTime GetCurrentTime()
		{
			return ExDateTime.UtcNow;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "serverName:{0}, state:{1}, backOffCount:{2}, lastUpdateTime:{3}, lastBackOffStartTime:{4}", new object[]
			{
				this.serverName,
				this.state,
				this.backOffCount,
				this.lastUpdateTime,
				this.lastBackOffStartTime
			});
		}

		protected ExDateTime? lastBackOffStartTime;

		private readonly string serverName;

		private readonly SlidingAverageCounter slidingAverageLatencyCounter;

		private RemoteServerHealthState state;

		private int backOffCount;

		private ExDateTime lastUpdateTime;
	}
}
