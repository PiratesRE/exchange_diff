using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct PerformanceData
	{
		public PerformanceData(int latency, uint count)
		{
			this = new PerformanceData(TimeSpan.FromMilliseconds((double)latency), count);
		}

		public PerformanceData(TimeSpan latency, uint count)
		{
			this = new PerformanceData(latency, count, 0, 0, 0);
		}

		public PerformanceData(TimeSpan latency, uint count, int counter1, int counter2, int counter3)
		{
			this.latency = latency;
			this.count = count;
			this.threadId = Environment.CurrentManagedThreadId;
			this.counter1 = counter1;
			this.counter2 = counter2;
			this.counter3 = counter3;
		}

		public static PerformanceData Zero
		{
			get
			{
				return PerformanceData.zero;
			}
		}

		public static PerformanceData Unknown
		{
			get
			{
				return PerformanceData.unknown;
			}
		}

		public int ThreadId
		{
			get
			{
				return this.threadId;
			}
		}

		public int Milliseconds
		{
			get
			{
				return (int)this.latency.TotalMilliseconds;
			}
		}

		public uint Count
		{
			get
			{
				return this.count;
			}
		}

		public TimeSpan Latency
		{
			get
			{
				return this.latency;
			}
		}

		public int Counter1
		{
			get
			{
				return this.counter1;
			}
		}

		public int Counter2
		{
			get
			{
				return this.counter2;
			}
		}

		public int Counter3
		{
			get
			{
				return this.counter3;
			}
		}

		public static bool operator ==(PerformanceData pd1, PerformanceData pd2)
		{
			return pd1.count == pd2.count && pd1.latency == pd2.latency && (pd1.Counter1 == pd2.Counter1 && pd1.Counter2 == pd2.Counter2) && pd1.Counter3 == pd2.Counter3;
		}

		public static bool Equals(PerformanceData pd1, PerformanceData pd2)
		{
			return pd1 == pd2;
		}

		public static bool operator !=(PerformanceData pd1, PerformanceData pd2)
		{
			return !(pd1 == pd2);
		}

		public static int Compare(PerformanceData pd1, PerformanceData pd2)
		{
			if (!(pd1 == pd2))
			{
				return 1;
			}
			return 0;
		}

		public static PerformanceData operator -(PerformanceData pd1, PerformanceData pd2)
		{
			return new PerformanceData(pd1.latency - pd2.latency, pd1.count - pd2.count, pd1.Counter1 - pd2.Counter1, pd1.Counter2 - pd2.Counter2, pd1.Counter3 - pd2.Counter3);
		}

		public static PerformanceData Subtract(PerformanceData pd1, PerformanceData pd2)
		{
			return pd1 - pd2;
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			if (obj != null && obj is PerformanceData)
			{
				result = ((PerformanceData)obj == this);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.count.GetHashCode() ^ this.latency.GetHashCode();
		}

		private static readonly PerformanceData zero = default(PerformanceData);

		private static readonly PerformanceData unknown = new PerformanceData(TimeSpan.FromMilliseconds(-1.0), 0U);

		private TimeSpan latency;

		private uint count;

		private int threadId;

		private int counter1;

		private int counter2;

		private int counter3;
	}
}
