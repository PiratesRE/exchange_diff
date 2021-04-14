using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	[Serializable]
	public class LatencyInfo
	{
		[DataMember(Name = "Current", IsRequired = false)]
		[XmlElement(ElementName = "Current")]
		public int Current { get; set; }

		[XmlElement(ElementName = "Average")]
		[DataMember(Name = "Average", IsRequired = false)]
		public int Average { get; set; }

		[XmlElement(ElementName = "NumberOfLatencySamplingCalls")]
		[DataMember(Name = "NumberOfLatencySamplingCalls", IsRequired = false)]
		public int NumberOfLatencySamplingCalls { get; set; }

		[XmlElement(ElementName = "TotalNumberOfRemoteCalls")]
		[DataMember(Name = "TotalNumberOfRemoteCalls", IsRequired = false)]
		public int TotalNumberOfRemoteCalls { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan TotalRemoteCallDuration { get; set; }

		[XmlElement(ElementName = "TotalRemoteCallDurationTicks")]
		[DataMember(Name = "TotalRemoteCallDurationTicks", IsRequired = false)]
		public long TotalRemoteCallDurationTicks
		{
			get
			{
				return this.TotalRemoteCallDuration.Ticks;
			}
			set
			{
				this.TotalRemoteCallDuration = new TimeSpan(value);
			}
		}

		[XmlElement(ElementName = "Min")]
		[DataMember(Name = "Min", IsRequired = false)]
		public int Min { get; set; }

		[DataMember(Name = "Max", IsRequired = false)]
		[XmlElement(ElementName = "Max")]
		public int Max { get; set; }

		public LatencyInfo()
		{
			this.Min = int.MaxValue;
			this.Max = int.MinValue;
		}

		public static LatencyInfo operator +(LatencyInfo latencyInfo1, LatencyInfo latencyInfo2)
		{
			if (latencyInfo2.TotalNumberOfRemoteCalls == 0)
			{
				return latencyInfo1;
			}
			if (latencyInfo1.TotalNumberOfRemoteCalls == 0)
			{
				return latencyInfo2;
			}
			LatencyInfo latencyInfo3 = new LatencyInfo();
			latencyInfo3.NumberOfLatencySamplingCalls = latencyInfo1.NumberOfLatencySamplingCalls + latencyInfo2.NumberOfLatencySamplingCalls;
			latencyInfo3.Current = latencyInfo2.Current;
			latencyInfo3.TotalNumberOfRemoteCalls = latencyInfo1.TotalNumberOfRemoteCalls + latencyInfo2.TotalNumberOfRemoteCalls;
			latencyInfo3.TotalRemoteCallDuration = latencyInfo1.TotalRemoteCallDuration + latencyInfo2.TotalRemoteCallDuration;
			if (latencyInfo3.NumberOfLatencySamplingCalls != 0)
			{
				latencyInfo3.Average = (latencyInfo1.Average * latencyInfo1.NumberOfLatencySamplingCalls + latencyInfo2.Average * latencyInfo2.NumberOfLatencySamplingCalls) / latencyInfo3.NumberOfLatencySamplingCalls;
				latencyInfo3.Min = Math.Min(latencyInfo1.Min, latencyInfo2.Min);
				latencyInfo3.Max = Math.Max(latencyInfo1.Max, latencyInfo2.Max);
			}
			return latencyInfo3;
		}

		public void AddSample(int latency)
		{
			if (latency > this.Max)
			{
				this.Max = latency;
			}
			if (latency < this.Min)
			{
				this.Min = latency;
			}
			this.Current = latency;
			this.Average = (this.Average * this.NumberOfLatencySamplingCalls + latency) / (this.NumberOfLatencySamplingCalls + 1);
			this.NumberOfLatencySamplingCalls++;
		}

		public override string ToString()
		{
			return string.Format("Current: {0}ms, Avg:{1}ms, NumberOfLatencySamplingCalls:{2}, Min:{3}ms, Max: {4}ms, TotalNumberOfRemoteCalls:{5}, TotalRemoteCallDuration: {6}", new object[]
			{
				this.Current,
				this.Average,
				this.NumberOfLatencySamplingCalls,
				this.Min,
				this.Max,
				this.TotalNumberOfRemoteCalls,
				this.TotalRemoteCallDuration.TotalMilliseconds
			});
		}
	}
}
