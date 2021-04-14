using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Management.BackSync.Configuration
{
	internal abstract class PerformanceCounterSession
	{
		public PerformanceCounterSession(bool enablePerformanceCounters)
		{
			this.EnablePerformanceCounters = enablePerformanceCounters;
			this.stopwatch = new Stopwatch();
			this.stopwatch.Start();
		}

		protected abstract ExPerformanceCounter RequestTime { get; }

		protected abstract ExPerformanceCounter RequestCount { get; }

		protected abstract ExPerformanceCounter TimeSinceLast { get; }

		protected abstract PerformanceCounterSession.HitRatePerformanceCounters Success { get; }

		protected abstract PerformanceCounterSession.HitRatePerformanceCounters SystemError { get; }

		protected abstract PerformanceCounterSession.HitRatePerformanceCounters UserError { get; }

		private protected bool EnablePerformanceCounters { protected get; private set; }

		public void Initialize()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "EnablePerformanceCounters = {0}", this.EnablePerformanceCounters);
			if (this.EnablePerformanceCounters)
			{
				this.RequestCount.Increment();
				ExTraceGlobals.BackSyncTracer.TraceDebug<long>((long)SyncConfiguration.TraceId, "RequestCount = {0}", this.RequestCount.RawValue);
			}
		}

		public virtual void Finish()
		{
			if (this.EnablePerformanceCounters)
			{
				this.Success.RecordHit(this.userErrorCount == 0 && this.systemErrorCount == 0);
				this.UserError.RecordHit(this.userErrorCount > 0);
				this.SystemError.RecordHit(this.systemErrorCount > 0);
				this.TimeSinceLast.RawValue = Stopwatch.GetTimestamp();
				this.RequestTime.RawValue = (long)Math.Round(this.stopwatch.Elapsed.TotalSeconds, 0);
			}
		}

		public virtual void ReportChangeCount(int changeCount)
		{
		}

		public virtual void ReportSameCookie(bool sameCookie)
		{
		}

		public void IncrementUserError()
		{
			this.userErrorCount++;
		}

		public void IncrementSystemError()
		{
			this.systemErrorCount++;
		}

		private Stopwatch stopwatch;

		private int userErrorCount;

		private int systemErrorCount;

		public class HitRatePerformanceCounters
		{
			public HitRatePerformanceCounters(ExPerformanceCounter hit, ExPerformanceCounter rate, ExPerformanceCounter custom)
			{
				this.hit = hit;
				this.rate = rate;
				this.custom = custom;
			}

			public void RecordHit(bool hit)
			{
				this.hit.RawValue = (hit ? 100L : 0L);
				PerformanceCounterSession.HitRatePerformanceCounters.MostRecentHitCollection mostRecentHitCollection = new PerformanceCounterSession.HitRatePerformanceCounters.MostRecentHitCollection((ulong)this.custom.RawValue);
				mostRecentHitCollection.Add(hit);
				this.custom.RawValue = (long)mostRecentHitCollection.RawValue;
				this.rate.RawValue = (long)mostRecentHitCollection.HitRate;
			}

			private const long CounterValueHit = 100L;

			private const long CounterValueNoHit = 0L;

			private ExPerformanceCounter hit;

			private ExPerformanceCounter rate;

			private ExPerformanceCounter custom;

			public class MostRecentHitCollection
			{
				public MostRecentHitCollection(ulong value)
				{
					this.RawValue = value;
				}

				public ulong RawValue { get; private set; }

				public int HitRate
				{
					get
					{
						int count = this.Count;
						if (count == 0)
						{
							return 0;
						}
						return (this.HitCount * 1000 / count + 5) / 10;
					}
				}

				public int Count
				{
					get
					{
						return PerformanceCounterSession.HitRatePerformanceCounters.MostRecentHitCollection.CountBits(this.Mask);
					}
				}

				private int HitCount
				{
					get
					{
						return PerformanceCounterSession.HitRatePerformanceCounters.MostRecentHitCollection.CountBits(this.Samples);
					}
				}

				private ulong Mask
				{
					get
					{
						return PerformanceCounterSession.HitRatePerformanceCounters.MostRecentHitCollection.GetHigh(this.RawValue);
					}
				}

				private ulong Samples
				{
					get
					{
						return PerformanceCounterSession.HitRatePerformanceCounters.MostRecentHitCollection.GetLow(this.RawValue);
					}
				}

				public void Add(bool value)
				{
					ulong high = (this.Mask << 1) + 1UL;
					ulong low = (this.Samples << 1) + (ulong)(value ? 1L : 0L);
					this.RawValue = PerformanceCounterSession.HitRatePerformanceCounters.MostRecentHitCollection.Build(high, low);
				}

				private static ulong Build(ulong high, ulong low)
				{
					return (high << 32) + (low & (ulong)-1);
				}

				private static ulong GetHigh(ulong value)
				{
					return value >> 32;
				}

				private static ulong GetLow(ulong value)
				{
					return value & (ulong)-1;
				}

				private static int CountBits(ulong value)
				{
					int num = 0;
					while (value > 0UL)
					{
						if ((value & 1UL) == 1UL)
						{
							num++;
						}
						value >>= 1;
					}
					return num;
				}
			}
		}
	}
}
