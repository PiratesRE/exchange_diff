using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal sealed class ScopedQueue : ISchedulerQueue
	{
		public ScopedQueue(IMessageScope scope, ISchedulerQueue queueInstance, Func<DateTime> timeProvider = null)
		{
			ArgumentValidator.ThrowIfNull("scope", scope);
			ArgumentValidator.ThrowIfNull("queueInstance", queueInstance);
			this.scope = scope;
			this.queueInstance = queueInstance;
			this.timeProvider = timeProvider;
			this.Locked = true;
			DateTime currentTime = this.GetCurrentTime();
			this.LastActivity = currentTime;
			this.LockDateTime = currentTime;
			this.createDateTime = currentTime;
			this.queueLog = new ScopedQueueLog(currentTime);
		}

		public DateTime LastActivity { get; private set; }

		public bool Locked { get; set; }

		public DateTime LockDateTime { get; private set; }

		public IMessageScope Scope
		{
			get
			{
				return this.scope;
			}
		}

		public DateTime CreateDateTime
		{
			get
			{
				return this.createDateTime;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.queueInstance.IsEmpty;
			}
		}

		public long Count
		{
			get
			{
				return this.queueInstance.Count;
			}
		}

		public void Enqueue(ISchedulableMessage message)
		{
			ArgumentValidator.ThrowIfNull("message", message);
			this.LastActivity = this.GetCurrentTime();
			this.queueInstance.Enqueue(message);
			this.queueLog.RecordEnqueue();
		}

		public bool TryDequeue(out ISchedulableMessage message)
		{
			if (this.queueInstance.TryDequeue(out message))
			{
				this.LastActivity = this.GetCurrentTime();
				this.queueLog.RecordDequeue();
				return true;
			}
			return false;
		}

		public bool TryPeek(out ISchedulableMessage message)
		{
			return this.queueInstance.TryPeek(out message);
		}

		public void Lock()
		{
			this.Locked = true;
			this.LockDateTime = this.GetCurrentTime();
			this.queueLog.Lock(this.LockDateTime);
		}

		public void Unlock()
		{
			this.Locked = false;
			this.queueLog.Unlock(this.GetCurrentTime());
		}

		public void Flush(DateTime timestamp, QueueLogInfo info)
		{
			info.Count = this.Count;
			this.queueLog.Flush(timestamp, info);
		}

		private DateTime GetCurrentTime()
		{
			if (this.timeProvider == null)
			{
				return DateTime.UtcNow;
			}
			return this.timeProvider();
		}

		private readonly ISchedulerQueue queueInstance;

		private readonly IMessageScope scope;

		private readonly Func<DateTime> timeProvider;

		private readonly DateTime createDateTime;

		private readonly ScopedQueueLog queueLog;
	}
}
