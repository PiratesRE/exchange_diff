using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.EventLog;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal sealed class FailureMonitor : StartStopComponent, INotifyFailed
	{
		static FailureMonitor()
		{
			ComponentRegistry.Register<FailureMonitor>();
		}

		internal FailureMonitor(params INotifyFailed[] initialComponents) : this(FailureMonitor.DefaultReviveWait, initialComponents)
		{
		}

		internal FailureMonitor(TimeSpan reviveDueTime, params INotifyFailed[] initialComponents) : this(reviveDueTime, DeferredStarter.NoRetryPeriod, initialComponents)
		{
		}

		internal FailureMonitor(TimeSpan reviveDueTime, TimeSpan retryPeriod, params INotifyFailed[] initialComponents)
		{
			base.DiagnosticsSession.ComponentName = "FailureMonitor";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.CoreFailureMonitorTracer;
			this.reviveDueTime = reviveDueTime;
			this.retryPeriod = retryPeriod;
			this.isRetryMode = (retryPeriod != DeferredStarter.NoRetryPeriod);
			this.initialComponents = initialComponents;
		}

		public void AttachComponent(INotifyFailed component, bool startNow)
		{
			base.CheckDisposed();
			base.DiagnosticsSession.TraceDebug<INotifyFailed>("Attaching {0} to be monitored", component);
			lock (this.lockObject)
			{
				if (this.monitoredComponents == null)
				{
					throw new InvalidOperationException("Cannot attach component before monitoredComponents is initialized.");
				}
				this.monitoredComponents.Add(component);
			}
			if (startNow)
			{
				this.InternalReviveComponent((IStartStop)component, null);
			}
		}

		public void DetachComponent(INotifyFailed component)
		{
			base.CheckDisposed();
			base.DiagnosticsSession.TraceDebug<INotifyFailed>("Detaching {0} from being monitored", component);
			lock (this.lockObject)
			{
				if (this.monitoredComponents == null)
				{
					throw new InvalidOperationException("Cannot detach component before monitoredComponents is initialized.");
				}
				this.monitoredComponents.Remove(component);
				this.DisposeStarterNoLock((IStartStop)component);
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.componentStarters != null)
			{
				foreach (DeferredStarter deferredStarter in this.componentStarters.Values)
				{
					deferredStarter.Dispose();
				}
				this.componentStarters.Clear();
				this.componentStarters = null;
			}
			base.InternalDispose(calledFromDispose);
		}

		protected sealed override void AtStart(AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug("Starting hook failure on components", new object[0]);
			base.DiagnosticsSession.Assert(this.monitoredComponents == null, "monitoredComponents must be null", new object[0]);
			this.monitoredComponents = new List<INotifyFailed>(10);
			if (this.initialComponents != null)
			{
				foreach (INotifyFailed component in this.initialComponents)
				{
					this.AttachComponent(component, false);
				}
			}
			base.AtStart(asyncResult);
		}

		protected sealed override void AtStop(AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug("AtStop: Stopping hook failure on components", new object[0]);
			if (this.monitoredComponents != null)
			{
				this.DetachAllComponents();
			}
			base.AtStop(asyncResult);
		}

		protected override void AtFail(ComponentFailedException reason)
		{
			base.DiagnosticsSession.TraceDebug("AtFail: Stopping hook failure on components", new object[0]);
			if (this.monitoredComponents != null)
			{
				this.DetachAllComponents();
			}
			base.AtFail(reason);
		}

		private void InternalReviveComponent(IStartStop component, ComponentFailedException error)
		{
			base.CheckDisposed();
			TimeSpan dueTime = (error == null) ? TimeSpan.Zero : this.reviveDueTime;
			base.DiagnosticsSession.TraceDebug<IStartStop, double, object>("Reviving {0} in {1} ms due to {2}", component, dueTime.TotalMilliseconds, (error == null) ? "requested" : error);
			bool flag = false;
			try
			{
				object obj;
				Monitor.Enter(obj = this.lockObject, ref flag);
				if (this.monitoredComponents == null || !this.monitoredComponents.Contains((INotifyFailed)component))
				{
					base.DiagnosticsSession.TraceDebug<IStartStop>("Skip reviving {0} which was not attached.", component);
				}
				else
				{
					this.DisposeStarterNoLock(component);
					DeferredStarter starter = new DeferredStarter(component, dueTime, this.retryPeriod);
					this.componentStarters.Add(component, starter);
					starter.BeginInvoke(delegate(IAsyncResult ar)
					{
						ComponentException ex = null;
						try
						{
							starter.EndInvoke(ar);
						}
						catch (ComponentException ex2)
						{
							ex = ex2;
						}
						this.DiagnosticsSession.TraceDebug<IStartStop, object>("Reviving {0} is done. Error = {1}", component, (ex == null) ? "none" : error);
						if (ex != null && !this.isRetryMode)
						{
							this.BeginDispatchFailSignal(new ComponentFailedPermanentException(ex), new AsyncCallback(this.EndDispatchFailSignal), null);
						}
					}, null);
				}
			}
			finally
			{
				if (flag)
				{
					object obj;
					Monitor.Exit(obj);
				}
			}
		}

		private void DetachAllComponents()
		{
			base.CheckDisposed();
			lock (this.lockObject)
			{
				List<INotifyFailed> list = new List<INotifyFailed>(this.monitoredComponents.Count);
				list.AddRange(this.monitoredComponents);
				foreach (INotifyFailed component in list)
				{
					this.DetachComponent(component);
				}
				this.monitoredComponents = null;
			}
		}

		private void DisposeStarterNoLock(IStartStop component)
		{
			DeferredStarter deferredStarter;
			if (this.componentStarters.TryGetValue(component, out deferredStarter))
			{
				try
				{
					deferredStarter.Cancel();
				}
				finally
				{
					deferredStarter.Dispose();
					this.componentStarters.Remove(component);
				}
			}
		}

		private void ComponentFailed(object sender, FailedEventArgs e)
		{
			base.CheckDisposed();
			IDiagnosable diagnosable = sender as IDiagnosable;
			base.DiagnosticsSession.LogPeriodicEvent(MSExchangeFastSearchEventLogConstants.Tuple_ComponentFailed, (diagnosable == null) ? "DefaultComponentFailedEventPeriodKey" : diagnosable.GetDiagnosticComponentName(), new object[]
			{
				e.Reason
			});
			if (this.isRetryMode || e.Reason is ComponentFailedTransientException)
			{
				this.InternalReviveComponent((IStartStop)sender, e.Reason);
				return;
			}
			if (e.Reason is ComponentFailedPermanentException)
			{
				base.BeginDispatchFailSignal(e.Reason, new AsyncCallback(base.EndDispatchFailSignal), null);
				return;
			}
			base.DiagnosticsSession.Assert(false, "Unexpected exception type.", new object[0]);
		}

		private const string DefaultComponentFailedEventPeriodKey = "DefaultComponentFailedEventPeriodKey";

		private static readonly TimeSpan DefaultReviveWait = TimeSpan.FromSeconds(10.0);

		private readonly TimeSpan reviveDueTime;

		private readonly TimeSpan retryPeriod;

		private readonly bool isRetryMode;

		private readonly IEnumerable<INotifyFailed> initialComponents;

		private readonly object lockObject = new object();

		private volatile List<INotifyFailed> monitoredComponents;

		private Dictionary<IStartStop, DeferredStarter> componentStarters = new Dictionary<IStartStop, DeferredStarter>(10);
	}
}
