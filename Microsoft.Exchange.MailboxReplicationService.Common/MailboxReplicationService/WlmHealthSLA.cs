using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class WlmHealthSLA
	{
		public WlmHealthSLA() : this(TimeSpan.FromMinutes(30.0))
		{
		}

		public WlmHealthSLA(TimeSpan windowWidth)
		{
			this.avg5min = new WlmHealthSLA.Bucketizer(TimeSpan.FromMinutes(5.0));
			this.avg1hour = new WlmHealthSLA.Bucketizer(TimeSpan.FromHours(1.0));
			this.avg1day = new WlmHealthSLA.Bucketizer(TimeSpan.FromDays(1.0));
			this.CustomTimeInterval = windowWidth;
			this.avgCustom = new WlmHealthSLA.Bucketizer(this.CustomTimeInterval);
		}

		public TimeSpan CustomTimeInterval { get; private set; }

		public void AddSample(ResourceLoadState healthState, int currentCapacity)
		{
			this.avg5min.AddSample(healthState, currentCapacity);
			this.avg1hour.AddSample(healthState, currentCapacity);
			this.avg1day.AddSample(healthState, currentCapacity);
			this.avgCustom.AddSample(healthState, currentCapacity);
		}

		public WlmHealthStatistics GetStats()
		{
			return new WlmHealthStatistics
			{
				Avg5Min = this.avg5min.GetStats(),
				Avg1Hour = this.avg1hour.GetStats(),
				Avg1Day = this.avg1day.GetStats()
			};
		}

		public WlmHealthCounters GetCustomHealthCounters()
		{
			return this.avgCustom.GetHealthCounters();
		}

		private const ushort OneMinuteInMilliseconds = 60000;

		private WlmHealthSLA.Bucketizer avg5min;

		private WlmHealthSLA.Bucketizer avg1hour;

		private WlmHealthSLA.Bucketizer avg1day;

		private WlmHealthSLA.Bucketizer avgCustom;

		private class Bucketizer
		{
			public Bucketizer(TimeSpan windowWidth)
			{
				ushort numberOfBuckets = (ushort)windowWidth.TotalMinutes;
				this.healthValues = new Dictionary<ResourceLoadState, FixedTimeSum>();
				foreach (object obj in Enum.GetValues(typeof(ResourceLoadState)))
				{
					ResourceLoadState key = (ResourceLoadState)obj;
					this.healthValues[key] = new FixedTimeSum(60000, numberOfBuckets);
				}
				this.averageCapacity = new FixedTimeAverage(60000, numberOfBuckets, Environment.TickCount);
			}

			public void AddSample(ResourceLoadState healthState, int currentCapacity)
			{
				this.healthValues[healthState].Add(1U);
				this.averageCapacity.Add((uint)currentCapacity);
			}

			public WlmHealthStatistics.HealthAverages GetStats()
			{
				WlmHealthStatistics.HealthAverages healthAverages = new WlmHealthStatistics.HealthAverages();
				uint value = this.healthValues[ResourceLoadState.Underloaded].GetValue();
				uint value2 = this.healthValues[ResourceLoadState.Full].GetValue();
				uint value3 = this.healthValues[ResourceLoadState.Overloaded].GetValue();
				uint value4 = this.healthValues[ResourceLoadState.Critical].GetValue();
				uint value5 = this.healthValues[ResourceLoadState.Unknown].GetValue();
				uint total = value + value2 + value3 + value4 + value5;
				healthAverages.Full = WlmHealthSLA.Bucketizer.GetPercentage(value2, total);
				healthAverages.Overloaded = WlmHealthSLA.Bucketizer.GetPercentage(value3, total);
				healthAverages.Critical = WlmHealthSLA.Bucketizer.GetPercentage(value4, total);
				healthAverages.Unknown = WlmHealthSLA.Bucketizer.GetPercentage(value5, total);
				healthAverages.Underloaded = 100 - healthAverages.Full - healthAverages.Overloaded - healthAverages.Critical - healthAverages.Unknown;
				healthAverages.AverageCapacity = this.averageCapacity.GetValue();
				return healthAverages;
			}

			public WlmHealthCounters GetHealthCounters()
			{
				return new WlmHealthCounters
				{
					UnderloadedCounter = this.healthValues[ResourceLoadState.Underloaded].GetValue(),
					FullCounter = this.healthValues[ResourceLoadState.Full].GetValue(),
					OverloadedCounter = this.healthValues[ResourceLoadState.Overloaded].GetValue(),
					CriticalCounter = this.healthValues[ResourceLoadState.Critical].GetValue(),
					UnknownCounter = this.healthValues[ResourceLoadState.Unknown].GetValue()
				};
			}

			private static int GetPercentage(uint value, uint total)
			{
				if (total == 0U)
				{
					return 0;
				}
				return (int)(value * 100U / total);
			}

			private Dictionary<ResourceLoadState, FixedTimeSum> healthValues;

			private FixedTimeAverage averageCapacity;
		}
	}
}
