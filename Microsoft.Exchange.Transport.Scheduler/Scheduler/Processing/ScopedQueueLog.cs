using System;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class ScopedQueueLog : QueueLog
	{
		public ScopedQueueLog(DateTime lockTime)
		{
			this.lastLockTime = lockTime;
			this.locked = true;
		}

		public void Lock(DateTime lockTime)
		{
			if (!this.locked)
			{
				this.locked = true;
				this.lastLockTime = lockTime;
			}
		}

		public void Unlock(DateTime unlockTime)
		{
			if (this.locked)
			{
				this.locked = false;
				this.lockDuration = this.lockDuration.Add(unlockTime - this.lastLockTime);
			}
		}

		protected override void FlushInternal(DateTime timestamp, QueueLogInfo info)
		{
			info.TotalLockTime = this.lockDuration;
			if (this.locked && timestamp > this.lastLockTime)
			{
				info.TotalLockTime = info.TotalLockTime.Add(timestamp - this.lastLockTime);
			}
		}

		protected override void ResetInternal(DateTime timetamp)
		{
			if (this.locked)
			{
				this.lastLockTime = timetamp;
			}
			this.lockDuration = TimeSpan.Zero;
		}

		private TimeSpan lockDuration = TimeSpan.Zero;

		private DateTime lastLockTime = DateTime.MinValue;

		private bool locked;
	}
}
