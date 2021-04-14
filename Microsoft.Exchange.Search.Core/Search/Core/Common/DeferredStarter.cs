using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.AsyncTask;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal sealed class DeferredStarter : Disposable
	{
		internal DeferredStarter(IStartStop component, TimeSpan dueTime) : this(component, dueTime, DeferredStarter.NoRetryPeriod)
		{
		}

		internal DeferredStarter(IStartStop component, TimeSpan dueTime, TimeSpan retryPeriod)
		{
			Util.ThrowOnNullArgument(component, "component");
			this.component = component;
			this.dueTime = dueTime;
			this.retryPeriod = retryPeriod;
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("DeferredStarter", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.CoreGeneralTracer, (long)this.GetHashCode());
		}

		internal IStartStop Component
		{
			get
			{
				return this.component;
			}
		}

		public IAsyncResult BeginInvoke(AsyncCallback callback, object context)
		{
			base.CheckDisposed();
			this.diagnosticsSession.TraceDebug<IStartStop>("DeferredStarter::BeginInvoke: Invoking start of component {0}", this.component);
			if (this.asyncTaskStarter == null)
			{
				this.diagnosticsSession.TraceDebug<IStartStop>("DeferredStarter::BeginInvoke: Create AsyncTaskSequence for component {0}", this.component);
				AsyncTaskSequence value = new AsyncTaskSequence(new AsyncTask[]
				{
					new AsyncStop(this.component),
					new AsyncPrepareToStart(this.component),
					new AsyncStart(this.component)
				});
				if (Interlocked.CompareExchange<AsyncTaskSequence>(ref this.asyncTaskStarter, value, null) != null)
				{
					throw new InvalidOperationException("Another thread has invoked.");
				}
			}
			AsyncResult asyncResult = new AsyncResult(callback, context);
			lock (this.cancelLocker)
			{
				if (this.isCancel)
				{
					asyncResult.SetAsCompleted();
				}
				else if (this.dueTime == TimeSpan.Zero)
				{
					this.InternalBeginInvoke(asyncResult);
				}
				else
				{
					this.diagnosticsSession.TraceDebug<IStartStop, double>("DeferredStarter::BeginInvoke: Deferred start of component {0} in {1} ms.", this.component, this.dueTime.TotalMilliseconds);
					RegisteredWaitHandleWrapper.RegisterWaitForSingleObject(this.cancelEvent, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(this.DeferredBeginInvoke)), asyncResult, this.dueTime, true);
					this.isDeferredInvokePending = true;
				}
			}
			return asyncResult;
		}

		public void EndInvoke(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			((AsyncResult)asyncResult).End();
		}

		public void Cancel()
		{
			base.CheckDisposed();
			this.diagnosticsSession.TraceDebug<IStartStop>("DeferredStarter::Cancel: Cancelling the start of component: {0}.", this.component);
			lock (this.cancelLocker)
			{
				if (this.isCancel)
				{
					throw new InvalidOperationException("Cannot cancel the starter more than once");
				}
				this.isCancel = true;
				this.cancelEvent.Set();
			}
			SpinWait spinWait = default(SpinWait);
			while (this.isDeferredInvokePending)
			{
				spinWait.SpinOnce();
			}
			if (this.asyncTaskStarter != null)
			{
				using (ManualResetEvent manualResetEvent = new ManualResetEvent(false))
				{
					this.asyncTaskStarter.Cancel(manualResetEvent);
					manualResetEvent.WaitOne();
				}
			}
			this.diagnosticsSession.TraceDebug<IStartStop>("DeferredStarter::Cancel: Cancelled the start of component {0}.", this.component);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DeferredStarter>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.cancelEvent != null)
			{
				this.cancelEvent.Close();
				this.cancelEvent = null;
			}
		}

		private void InternalBeginInvoke(AsyncResult asyncResult)
		{
			this.diagnosticsSession.TraceDebug<IStartStop>("DeferredStarter::InternalBeginInvoke for component {0}.", this.component);
			this.asyncTaskStarter.Execute(delegate(AsyncTask task)
			{
				this.CheckDisposed();
				if (!task.Cancelled && task.Exception != null)
				{
					this.diagnosticsSession.TraceDebug<IStartStop, ComponentException>("DeferredStarter::InternalBeginInvoke: failed to start component {0} due to {1}.", this.component, task.Exception);
					if (this.retryPeriod != DeferredStarter.NoRetryPeriod)
					{
						this.diagnosticsSession.TraceDebug<IStartStop, double>("DeferredStarter::InternalBeginInvoke: Restart of component {0} in {1} ms.", this.component, this.retryPeriod.TotalMilliseconds);
						lock (this.cancelLocker)
						{
							if (!this.isCancel)
							{
								this.diagnosticsSession.TraceDebug<IStartStop, double>("DeferredStarter::InternalBeginInvoke: Deferred start of component {0} in {1} ms.", this.component, this.retryPeriod.TotalMilliseconds);
								RegisteredWaitHandleWrapper.RegisterWaitForSingleObject(this.cancelEvent, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(this.DeferredBeginInvoke)), asyncResult, this.retryPeriod, true);
								this.isDeferredInvokePending = true;
							}
							return;
						}
					}
					asyncResult.SetAsCompleted(new ComponentFailedPermanentException(task.Exception));
					return;
				}
				asyncResult.SetAsCompleted();
			});
		}

		private void DeferredBeginInvoke(object state, bool timedOut)
		{
			base.CheckDisposed();
			try
			{
				AsyncResult asyncResult = (AsyncResult)state;
				if (timedOut)
				{
					this.diagnosticsSession.TraceDebug<IStartStop>("DeferredStarter::DeferredBeginInvoke: Time out for component {0}.", this.component);
					this.InternalBeginInvoke(asyncResult);
				}
				else
				{
					this.diagnosticsSession.TraceDebug<IStartStop>("DeferredStarter::DeferredBeginInvoke: Cancelled for component {0}.", this.component);
					asyncResult.SetAsCompleted();
				}
			}
			finally
			{
				this.isDeferredInvokePending = false;
			}
		}

		internal static readonly TimeSpan NoRetryPeriod = TimeSpan.Zero;

		private readonly IStartStop component;

		private readonly TimeSpan dueTime;

		private readonly TimeSpan retryPeriod;

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly object cancelLocker = new object();

		private AsyncTaskSequence asyncTaskStarter;

		private volatile bool isDeferredInvokePending;

		private ManualResetEvent cancelEvent = new ManualResetEvent(false);

		private bool isCancel;
	}
}
