using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class RecurringTask<T> : Task<T>
	{
		public RecurringTask(Task<T>.TaskCallback callback, T context, TimeSpan interval) : this(callback, context, Task.NoDelay, interval, false)
		{
		}

		public RecurringTask(Task<T>.TaskCallback callback, T context, TimeSpan interval, bool autoStart) : this(callback, context, Task.NoDelay, interval, autoStart)
		{
		}

		public RecurringTask(Task<T>.TaskCallback callback, T context, TimeSpan initialDelay, TimeSpan interval, bool autoStart) : base(callback, context, ThreadPriority.Normal, 0, TaskFlags.UseThreadPoolThread)
		{
			bool flag = false;
			try
			{
				this.timer = new Timer(delegate(object unused)
				{
					base.Worker();
				}, null, -1, -1);
				this.initialDelay = initialDelay;
				this.interval = interval;
				if (autoStart)
				{
					this.Start();
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					base.Dispose();
				}
			}
		}

		protected TimeSpan InitialDelay
		{
			set
			{
				this.initialDelay = value;
			}
		}

		protected TimeSpan Interval
		{
			get
			{
				return this.interval;
			}
		}

		public override void Start()
		{
			base.CheckDisposed();
			using (LockManager.Lock(base.StateLock))
			{
				Task<T>.TaskState state = base.State;
				if (state == Task<T>.TaskState.Ready || state == Task<T>.TaskState.Complete)
				{
					base.State = Task<T>.TaskState.Starting;
					this.timer.Change(this.initialDelay, this.interval);
				}
			}
		}

		public override void Stop()
		{
			base.CheckDisposed();
			using (LockManager.Lock(base.StateLock))
			{
				if (this.timer != null)
				{
					this.timer.Change(-1, -1);
				}
				if (base.State == Task<T>.TaskState.Starting)
				{
					base.State = Task<T>.TaskState.Ready;
				}
				if (base.State == Task<T>.TaskState.Running)
				{
					base.State = Task<T>.TaskState.StopRequested;
				}
			}
		}

		protected override void Invoke()
		{
			using (LockManager.Lock(base.StateLock))
			{
				if (base.State == Task<T>.TaskState.Complete)
				{
					base.State = Task<T>.TaskState.Starting;
				}
				if (base.State == Task<T>.TaskState.StopRequested)
				{
					base.State = Task<T>.TaskState.Complete;
					return;
				}
			}
			base.Invoke();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RecurringTask<T>>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.Stop();
				base.WaitForCompletion();
				if (this.timer != null)
				{
					using (ManualResetEvent manualResetEvent = new ManualResetEvent(false))
					{
						if (this.timer.Dispose(manualResetEvent))
						{
							manualResetEvent.WaitOne();
						}
					}
					this.timer = null;
				}
			}
			base.InternalDispose(calledFromDispose);
		}

		protected static readonly TimeSpan RunOnce = TimeSpan.FromMilliseconds(-1.0);

		private Timer timer;

		private TimeSpan initialDelay;

		private TimeSpan interval;
	}
}
