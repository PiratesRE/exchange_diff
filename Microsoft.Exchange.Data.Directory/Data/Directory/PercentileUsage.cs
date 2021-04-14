using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal class PercentileUsage
	{
		public static int FiveMinuteComparer(PercentileUsage x, PercentileUsage y)
		{
			return Comparer<int>.Default.Compare(x.FiveMinuteUsage, y.FiveMinuteUsage);
		}

		public static int OneHourComparer(PercentileUsage x, PercentileUsage y)
		{
			return Comparer<int>.Default.Compare(x.OneHourUsage, y.OneHourUsage);
		}

		public PercentileUsage()
		{
			this.CreationTime = TimeProvider.UtcNow;
		}

		public PercentileUsage(PercentileUsage source)
		{
			this.FiveMinuteUsage = source.FiveMinuteUsage;
			this.OneHourUsage = source.OneHourUsage;
			this.CreationTime = source.CreationTime;
		}

		public void AddUsage(int usage)
		{
			Interlocked.Add(ref this.fiveMinuteUsage, usage);
			Interlocked.Add(ref this.oneHourUsage, usage);
		}

		public int FiveMinuteUsage
		{
			get
			{
				return this.fiveMinuteUsage;
			}
			private set
			{
				Interlocked.Exchange(ref this.fiveMinuteUsage, value);
			}
		}

		public int OneHourUsage
		{
			get
			{
				return this.oneHourUsage;
			}
			private set
			{
				Interlocked.Exchange(ref this.oneHourUsage, value);
			}
		}

		public DateTime CreationTime { get; private set; }

		internal bool Expired { get; set; }

		public void Clear(bool oneHour)
		{
			if (oneHour)
			{
				this.OneHourUsage = 0;
			}
			this.FiveMinuteUsage = 0;
		}

		private int oneHourUsage;

		private int fiveMinuteUsage;
	}
}
