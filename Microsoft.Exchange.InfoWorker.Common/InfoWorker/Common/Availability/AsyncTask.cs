using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class AsyncTask
	{
		public bool Aborted { get; private set; }

		public AsyncTask()
		{
			this.taskState = TaskState.NotStarted;
		}

		public virtual void BeginInvoke(TaskCompleteCallback callback)
		{
			AsyncTask.Tracer.TraceDebug<object, AsyncTask>((long)this.GetHashCode(), "{0}: BeginInvoke: {1}", TraceContext.Get(), this);
			if (this.taskState != TaskState.NotStarted)
			{
				throw new InvalidOperationException();
			}
			this.taskState = TaskState.Running;
			this.callback = callback;
		}

		public virtual void Abort()
		{
			AsyncTask.Tracer.TraceDebug<object, AsyncTask>((long)this.GetHashCode(), "{0}: Abort: {1}", TraceContext.Get(), this);
			this.Aborted = true;
			if (this.done != null)
			{
				this.SetDone(null);
			}
		}

		protected void Complete()
		{
			AsyncTask.Tracer.TraceDebug<object, AsyncTask>((long)this.GetHashCode(), "{0}: Complete: {1}", TraceContext.Get(), this);
			try
			{
				this.callback(this);
			}
			finally
			{
				this.taskState = TaskState.Completed;
			}
		}

		internal bool Invoke(DateTime deadline)
		{
			this.done = new ManualResetEvent(false);
			bool result;
			try
			{
				this.BeginInvoke(new TaskCompleteCallback(this.SetDone));
				DateTime utcNow = DateTime.UtcNow;
				TimeSpan timeout = (deadline > utcNow) ? (deadline - utcNow) : TimeSpan.Zero;
				bool flag = this.done.WaitOne(timeout, false);
				if (!flag)
				{
					this.Abort();
				}
				result = flag;
			}
			finally
			{
				this.done.Close();
			}
			return result;
		}

		private void SetDone(AsyncTask task)
		{
			try
			{
				this.done.Set();
			}
			catch (ObjectDisposedException)
			{
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.RequestRoutingTracer;

		private TaskState taskState;

		private TaskCompleteCallback callback;

		private ManualResetEvent done;
	}
}
