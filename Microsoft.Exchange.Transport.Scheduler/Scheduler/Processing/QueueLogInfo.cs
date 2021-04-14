using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class QueueLogInfo
	{
		public QueueLogInfo(string display, DateTime timeStamp)
		{
			ArgumentValidator.ThrowIfNull("display", display);
			this.display = display;
			this.timeStamp = timeStamp;
		}

		public long Enqueues
		{
			get
			{
				return this.enqueues;
			}
			set
			{
				this.enqueues = value;
			}
		}

		public long Dequeues
		{
			get
			{
				return this.dequeues;
			}
			set
			{
				this.dequeues = value;
			}
		}

		public long Count
		{
			get
			{
				return this.count;
			}
			set
			{
				this.count = value;
			}
		}

		public UsageData UsageData
		{
			get
			{
				return this.usageData;
			}
			set
			{
				this.usageData = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return this.timeStamp;
			}
		}

		public string Display
		{
			get
			{
				return this.display;
			}
		}

		public TimeSpan TotalLockTime
		{
			get
			{
				return this.totalLockTime;
			}
			set
			{
				this.totalLockTime = value;
			}
		}

		private readonly DateTime timeStamp;

		private readonly string display;

		private UsageData usageData = UsageData.EmptyUsage;

		private long enqueues;

		private long dequeues;

		private long count;

		private TimeSpan totalLockTime = TimeSpan.Zero;
	}
}
