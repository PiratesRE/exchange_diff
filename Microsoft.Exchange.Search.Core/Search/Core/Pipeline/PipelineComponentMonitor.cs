using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.AsyncTask;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	internal sealed class PipelineComponentMonitor : StartStopComponent, INotifyFailed
	{
		static PipelineComponentMonitor()
		{
			ComponentRegistry.Register<PipelineComponentMonitor>();
		}

		internal PipelineComponentMonitor(PipelineComponentList components)
		{
			Util.ThrowOnNullArgument(components, "components");
			base.DiagnosticsSession.ComponentName = "PipelineComponentMonitor";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.CorePipelineTracer;
			if (components.HasStartStopComponent)
			{
				List<IStartStopPipelineComponent> list = new List<IStartStopPipelineComponent>(components.Count);
				for (int i = 0; i < components.Count; i++)
				{
					IPipelineComponent pipelineComponent = components[i];
					if (pipelineComponent is IStartStopPipelineComponent)
					{
						list.Add((IStartStopPipelineComponent)pipelineComponent);
					}
				}
				base.DiagnosticsSession.TraceDebug<int>("Creating failure monitor to watch on {0} components that support start stop", list.Count);
				this.failureMonitor = new FailureMonitor(list.ToArray());
			}
			this.components = components;
		}

		protected sealed override void AtPrepareToStart(AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug("Preparing to start PipelineComponentMonitor", new object[0]);
			if (this.failureMonitor != null)
			{
				this.pendingAsyncResult = asyncResult;
				base.DiagnosticsSession.TraceDebug("Preparing to start failure monitor", new object[0]);
				new AsyncPrepareToStart(this.failureMonitor).Execute(new TaskCompleteCallback(this.AtPrepareToStartCallback));
				return;
			}
			base.DiagnosticsSession.TraceDebug("PipelineComponentMonitor is prepared to start.", new object[0]);
			base.AtPrepareToStart(asyncResult);
		}

		protected sealed override void AtStart(AsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug("Starting to hook failure on components for PipelineComponentMonitor", new object[0]);
			for (int i = 0; i < this.components.Count; i++)
			{
				IPipelineComponent pipelineComponent = this.components[i];
			}
			if (this.failureMonitor != null)
			{
				this.pendingAsyncResult = asyncResult;
				base.DiagnosticsSession.TraceDebug("Starting failure monitor", new object[0]);
				new AsyncStart(this.failureMonitor).Execute(new TaskCompleteCallback(this.AtStartCallback));
				return;
			}
			base.DiagnosticsSession.TraceDebug("PipelineComponentMonitor is started.", new object[0]);
			base.AtStart(asyncResult);
		}

		protected sealed override void AtStop(AsyncResult asyncResult)
		{
			this.UnhookComponents();
			this.InternalStop(asyncResult);
		}

		protected override void AtFail(ComponentFailedException reason)
		{
			this.UnhookComponents();
			base.AtFail(reason);
		}

		protected override void AtStopInFailed(AsyncResult asyncResult)
		{
			this.InternalStop(asyncResult);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.DisposeChildren();
			}
			base.InternalDispose(calledFromDispose);
		}

		private void DisposeChildren()
		{
			if (this.failureMonitor != null)
			{
				this.failureMonitor.Dispose();
				this.failureMonitor = null;
			}
		}

		private void UnhookComponents()
		{
			base.DiagnosticsSession.TraceDebug("Stopping hooking failure on components for PipelineComponentMonitor", new object[0]);
			for (int i = 0; i < this.components.Count; i++)
			{
				IPipelineComponent pipelineComponent = this.components[i];
			}
		}

		private void InternalStop(AsyncResult asyncResult)
		{
			if (this.failureMonitor != null)
			{
				this.pendingAsyncResult = asyncResult;
				base.DiagnosticsSession.TraceDebug("Stopping failure monitor", new object[0]);
				new AsyncStop(this.failureMonitor).Execute(new TaskCompleteCallback(this.AtStopCallback));
				return;
			}
			base.DiagnosticsSession.TraceDebug("PipelineComponentMonitor is stopped.", new object[0]);
			base.AtStop(asyncResult);
		}

		private void AtPipelineComponentFailed(IPipelineComponent component, ComponentFailedException reason)
		{
			base.DiagnosticsSession.TraceDebug<IPipelineComponent, ComponentFailedException>("Component {0} is failed with reason {1}.", component, reason);
			int num = this.components.IndexOf(component);
			base.DiagnosticsSession.TraceDebug<int>("Recreating a component and replace it at index of {0}.", num);
			this.components.Recreate(num);
		}

		private void PipelineComponentFailed(object sender, FailedEventArgs e)
		{
			if (sender is IStartStopPipelineComponent)
			{
				base.DiagnosticsSession.TraceDebug<object, ComponentFailedException>("Defer to failure monitor to revive the component {0} that failed with exception {1}", sender, e.Reason);
				return;
			}
			this.BeginDispatchPipelineComponentFailedSignal((IPipelineComponent)sender, e.Reason, new AsyncCallback(this.EndDispatchPipelineComponentFailedSignal), null);
		}

		private void AtPrepareToStartCallback(AsyncTask asyncTask)
		{
			base.DiagnosticsSession.TraceDebug("PipelineComponentMonitor is prepared to start.", new object[0]);
			if (asyncTask.Exception != null)
			{
				this.pendingAsyncResult.SetAsCompleted(asyncTask.Exception);
				return;
			}
			base.AtPrepareToStart(this.pendingAsyncResult);
		}

		private void AtStartCallback(AsyncTask asyncTask)
		{
			base.DiagnosticsSession.TraceDebug("PipelineComponentMonitor is started.", new object[0]);
			if (asyncTask.Exception != null)
			{
				this.pendingAsyncResult.SetAsCompleted(asyncTask.Exception);
				return;
			}
			base.AtStart(this.pendingAsyncResult);
		}

		private void AtStopCallback(AsyncTask asyncTask)
		{
			base.DiagnosticsSession.TraceDebug("PipelineComponentMonitor is stopped.", new object[0]);
			if (asyncTask.Exception != null)
			{
				this.pendingAsyncResult.SetAsCompleted(asyncTask.Exception);
				return;
			}
			base.AtStop(this.pendingAsyncResult);
		}

		private void OnFailureMonitorFailed(object sender, FailedEventArgs e)
		{
			ComponentFailedException reason = e.Reason;
			base.DiagnosticsSession.TraceDebug<ComponentFailedException>("The failure monitor failed to revive component. Exception=", reason);
			base.BeginDispatchFailSignal(reason, new AsyncCallback(base.EndDispatchFailSignal), null);
		}

		private static void RegisterComponent(ComponentInfo componentInfo)
		{
			componentInfo.RegisterSignal(PipelineComponentMonitor.Signal.PipelineComponentFailed, SignalPriority.Medium);
			componentInfo.RegisterTransition(6U, 9U, 6U, null, new ActionMethod(PipelineComponentMonitor.Transition_AtPipelineComponentFailed));
		}

		internal static void Transition_AtPipelineComponentFailed(object component, params object[] args)
		{
			((PipelineComponentMonitor)component).AtPipelineComponentFailed((IPipelineComponent)args[0], (ComponentFailedException)args[1]);
		}

		internal IAsyncResult BeginDispatchPipelineComponentFailedSignal(IPipelineComponent component, ComponentFailedException reason, AsyncCallback callback, object context)
		{
			return base.InternalBeginDispatchSignal(null, 9U, callback, context, TimeSpan.Zero, new object[]
			{
				component,
				reason
			});
		}

		internal IAsyncResult BeginDispatchPipelineComponentFailedSignal(IPipelineComponent component, ComponentFailedException reason, AsyncCallback callback, object context, WaitHandle waitHandle, TimeSpan delayInTimespan)
		{
			Util.ThrowOnNullArgument(waitHandle, "waitHandle");
			return base.InternalBeginDispatchSignal(waitHandle, 9U, callback, context, delayInTimespan, new object[]
			{
				component,
				reason
			});
		}

		internal void EndDispatchPipelineComponentFailedSignal(IAsyncResult asyncResult)
		{
			base.EndDispatchSignal(asyncResult);
		}

		private readonly PipelineComponentList components;

		private FailureMonitor failureMonitor;

		private AsyncResult pendingAsyncResult;

		internal new enum Signal : uint
		{
			PipelineComponentFailed = 9U,
			Max
		}

		internal new enum State : uint
		{
			Max = 10U
		}
	}
}
