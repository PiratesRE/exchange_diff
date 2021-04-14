using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Inference.Performance;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal abstract class StatefulComponent : Disposable, IDiagnosable
	{
		internal StatefulComponent(uint initialState)
		{
			this.perfCounterInstance = StatefulComponentPerformanceCounters.GetInstance(base.GetType().ToString());
			this.diagnosticsSession = Microsoft.Exchange.Search.Core.Diagnostics.DiagnosticsSession.CreateComponentDiagnosticsSession("StatefulComponent", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.CoreComponentTracer, (long)this.GetHashCode());
			this.CurrentState = initialState;
			this.dispatchQueue = new Heap(StatefulComponent.DispatchPriorityComparer.Instance);
			this.transitionLog = new Queue<TransitionLogEntry>(10);
		}

		internal StatefulComponent() : this(1U)
		{
		}

		public IDiagnosticsSession DiagnosticsSession
		{
			get
			{
				return this.diagnosticsSession;
			}
		}

		internal TransitionLogEntry[] TransitionLog
		{
			get
			{
				TransitionLogEntry[] result;
				lock (this.transitionLog)
				{
					result = this.transitionLog.ToArray();
				}
				return result;
			}
		}

		internal uint CurrentState
		{
			get
			{
				return this.currentState;
			}
			private set
			{
				if (value == this.currentState)
				{
					if (this.DiagnosticsSession.IsTraceEnabled(TraceType.DebugTrace))
					{
						this.DiagnosticsSession.TraceDebug<StateInfo>("State already set to {0}.", this.CurrentStateInfo);
					}
					return;
				}
				if (this.DiagnosticsSession.IsTraceEnabled(TraceType.DebugTrace))
				{
					StateInfo arg = null;
					ComponentRegistry.TryGetStateInfo(base.GetType(), value, out arg);
					this.DiagnosticsSession.TraceDebug<StateInfo, StateInfo>("Changing state from {0} to {1}.", this.CurrentStateInfo, arg);
				}
				this.currentState = value;
				this.lastStateChangedTime = DateTime.UtcNow;
			}
		}

		protected StateInfo CurrentStateInfo
		{
			get
			{
				StateInfo result = null;
				ComponentRegistry.TryGetStateInfo(base.GetType(), this.CurrentState, out result);
				return result;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", base.GetType(), this.GetHashCode());
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return base.GetType().Name;
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return this.InternalGetDiagnosticInfo(parameters);
		}

		internal string GetSignalName(uint signal)
		{
			SignalInfo signalInfo = null;
			Type type = base.GetType();
			while (type != null && typeof(StatefulComponent).IsAssignableFrom(type))
			{
				if (ComponentRegistry.TryGetSignalInfo(type, signal, out signalInfo))
				{
					return signalInfo.Name;
				}
				type = type.BaseType;
			}
			return string.Format("Unknown Signal: {0}", signal);
		}

		internal string GetStateName(uint state)
		{
			StateInfo stateInfo = null;
			Type type = base.GetType();
			while (type != null && typeof(StatefulComponent).IsAssignableFrom(type))
			{
				if (ComponentRegistry.TryGetStateInfo(type, state, out stateInfo))
				{
					return stateInfo.Name;
				}
				type = type.BaseType;
			}
			return string.Format("Unknown State: {0}", state);
		}

		internal IAsyncResult InternalBeginDispatchSignal(WaitHandle waitHandle, uint signal, AsyncCallback callback, object context, TimeSpan delayInTimespan, params object[] arguments)
		{
			base.CheckDisposed();
			if (delayInTimespan != TimeSpan.Zero && waitHandle == null)
			{
				throw new ArgumentException("A value for delayInTimespan different than zero doesn't make sense if waitHandle is null");
			}
			IAsyncResult result;
			lock (this.dispatchQueue)
			{
				SignalInfo signalInfo = null;
				if (!ComponentRegistry.TryGetSignalInfo(base.GetType(), signal, out signalInfo))
				{
					throw new ArgumentException(string.Format("Unknown signal '{0}'", signal));
				}
				AsyncResult<bool> asyncResult = new AsyncResult<bool>(callback, context);
				StatefulComponent.QueueItem queueItem = new StatefulComponent.QueueItem(signalInfo, asyncResult, waitHandle, arguments);
				if (waitHandle == null)
				{
					this.DiagnosticsSession.TraceDebug<StatefulComponent.QueueItem>("Enqueuing {0}", queueItem);
					this.dispatchQueue.Push(queueItem);
					queueItem.MarkAsEnqueued();
					this.DispatchNextItemNoLock();
				}
				else
				{
					this.DiagnosticsSession.TraceDebug<StatefulComponent.QueueItem, double>("Delaying enqueuing {0} for {1} ms.", queueItem, delayInTimespan.TotalMilliseconds);
					RegisteredWaitHandleWrapper.RegisterWaitForSingleObject(waitHandle, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(this.DelayedDispatchSignalCallback)), queueItem, delayInTimespan, true);
				}
				result = asyncResult;
			}
			return result;
		}

		internal bool EndDispatchSignal(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			return ((AsyncResult<bool>)asyncResult).End();
		}

		protected sealed override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<StatefulComponent>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				lock (this.dispatchQueue)
				{
				}
				SpinWait spinWait = default(SpinWait);
				while (this.dispatchingItem != null)
				{
					spinWait.SpinOnce();
				}
			}
		}

		protected virtual XElement InternalGetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			xelement.Add(new XElement("State", this.GetStateName(this.CurrentState)));
			xelement.Add(new XElement("StateStartTime", this.lastStateChangedTime));
			return xelement;
		}

		protected virtual bool AtNoTransitionDefined(uint signal)
		{
			throw new NoTransitionException(this, this.CurrentState, signal);
		}

		private static void RegisterComponent(ComponentInfo componentInfo)
		{
			componentInfo.RegisterState(StatefulComponent.State.New);
		}

		private void DispatchNextItemNoLock()
		{
			this.DiagnosticsSession.TraceDebug<Heap>("DispatchNextItemNoLock, queue snapshot: {0}", this.dispatchQueue);
			if (this.dispatchingItem != null)
			{
				this.DiagnosticsSession.TraceDebug("Another thread already dispatching queue, exiting.", new object[0]);
				return;
			}
			IHeapItem heapItem = null;
			if (this.dispatchQueue.TryPop(out heapItem))
			{
				StatefulComponent.QueueItem queueItem = (StatefulComponent.QueueItem)heapItem;
				this.DiagnosticsSession.TraceDebug<StatefulComponent.QueueItem>("Dispatching: {0}", queueItem);
				this.dispatchingItem = queueItem;
				using (ActivityContext.SuppressThreadScope())
				{
					ThreadPool.QueueUserWorkItem(CallbackWrapper.WaitCallback(new WaitCallback(this.DispatchQueueItem)), queueItem);
				}
			}
		}

		private void DispatchQueueItem(object context)
		{
			base.CheckDisposed();
			try
			{
				ComponentException ex = null;
				StatefulComponent.QueueItem queueItem = context as StatefulComponent.QueueItem;
				bool asCompleted = true;
				long incrementValue = queueItem.MarkAsDispatched();
				this.DiagnosticsSession.IncrementCounterBy(this.perfCounterInstance.AverageSignalDispatchingLatency, incrementValue);
				this.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.AverageSignalDispatchingLatencyBase);
				queueItem.CheckForCancellation();
				List<TransitionInfo> list = null;
				if (!ComponentRegistry.TryGetTransitionInfo(base.GetType(), this.CurrentState, queueItem.SignalInfo.Value, out list))
				{
					try
					{
						asCompleted = this.AtNoTransitionDefined(queueItem.SignalInfo.Value);
						goto IL_190;
					}
					catch (ComponentException ex2)
					{
						ex = ex2;
						goto IL_190;
					}
				}
				TransitionInfo transitionInfo = null;
				foreach (TransitionInfo transitionInfo2 in list)
				{
					if (transitionInfo2.Condition == null)
					{
						transitionInfo = transitionInfo2;
						break;
					}
					if (transitionInfo2.Condition(this))
					{
						transitionInfo = transitionInfo2;
						break;
					}
				}
				if (transitionInfo == null)
				{
					try
					{
						asCompleted = this.AtNoTransitionDefined(queueItem.SignalInfo.Value);
						goto IL_190;
					}
					catch (ComponentException ex3)
					{
						ex = ex3;
						goto IL_190;
					}
				}
				if (transitionInfo.Action != null)
				{
					this.DiagnosticsSession.TraceDebug<MethodInfo>("Executing transition action: {0}", transitionInfo.Action.Method);
					try
					{
						transitionInfo.Action(this, queueItem.Arguments);
						goto IL_175;
					}
					catch (ComponentException ex4)
					{
						ex = ex4;
						goto IL_175;
					}
				}
				if (this.DiagnosticsSession.IsTraceEnabled(TraceType.DebugTrace))
				{
					this.DiagnosticsSession.TraceDebug<StateInfo, SignalInfo>("Action is not defined (noop) for this transition for state {0} and signal {1}", this.CurrentStateInfo, queueItem.SignalInfo);
				}
				asCompleted = false;
				IL_175:
				this.CurrentState = transitionInfo.TargetState;
				this.LogTransitionHistory(transitionInfo.TargetState, queueItem);
				IL_190:
				if (ex != null)
				{
					queueItem.AsyncResult.SetAsCompleted(ex);
				}
				else
				{
					queueItem.AsyncResult.SetAsCompleted(asCompleted);
				}
			}
			finally
			{
				this.dispatchingItem = null;
			}
			lock (this.dispatchQueue)
			{
				this.DispatchNextItemNoLock();
			}
		}

		private void DelayedDispatchSignalCallback(object context, bool isTimeout)
		{
			base.CheckDisposed();
			StatefulComponent.QueueItem queueItem = context as StatefulComponent.QueueItem;
			if (!isTimeout)
			{
				this.DiagnosticsSession.TraceDebug<StatefulComponent.QueueItem>("Event set, cancelling dispatch item: {0}", queueItem);
				queueItem.Cancel();
			}
			lock (this.dispatchQueue)
			{
				this.DiagnosticsSession.TraceDebug<StatefulComponent.QueueItem>("Enqueuing delayed dispatch item: {0}", queueItem);
				this.dispatchQueue.Push(queueItem);
				queueItem.MarkAsEnqueued();
				this.DispatchNextItemNoLock();
			}
		}

		private void LogTransitionHistory(uint currentState, StatefulComponent.QueueItem queueItem)
		{
			TransitionLogEntry item = new TransitionLogEntry(queueItem.Sequence, currentState, queueItem.SignalInfo.Value);
			lock (this.transitionLog)
			{
				if (this.transitionLog.Count == 10)
				{
					this.transitionLog.Dequeue();
				}
				this.transitionLog.Enqueue(item);
			}
		}

		private const int TransitionLogSize = 10;

		private readonly StatefulComponentPerformanceCountersInstance perfCounterInstance;

		private readonly Heap dispatchQueue;

		private readonly Queue<TransitionLogEntry> transitionLog;

		private readonly IDiagnosticsSession diagnosticsSession;

		private uint currentState;

		private DateTime lastStateChangedTime;

		private volatile StatefulComponent.QueueItem dispatchingItem;

		internal enum State : uint
		{
			New = 1U,
			Max,
			Any = 4294967295U
		}

		internal enum Signal : uint
		{
			Max = 1U,
			Cancelled = 4026531840U
		}

		private sealed class DispatchPriorityComparer : IComparer<IHeapItem>
		{
			internal static StatefulComponent.DispatchPriorityComparer Instance
			{
				get
				{
					return StatefulComponent.DispatchPriorityComparer.instance;
				}
			}

			public int Compare(IHeapItem left, IHeapItem right)
			{
				StatefulComponent.QueueItem queueItem = (StatefulComponent.QueueItem)left;
				StatefulComponent.QueueItem queueItem2 = (StatefulComponent.QueueItem)right;
				uint priority = (uint)queueItem.SignalInfo.Priority;
				uint priority2 = (uint)queueItem2.SignalInfo.Priority;
				if (priority > priority2)
				{
					return 1;
				}
				if (priority < priority2)
				{
					return -1;
				}
				if (queueItem.Sequence > queueItem2.Sequence)
				{
					return -1;
				}
				if (queueItem.Sequence < queueItem2.Sequence)
				{
					return 1;
				}
				return 0;
			}

			private static StatefulComponent.DispatchPriorityComparer instance = new StatefulComponent.DispatchPriorityComparer();
		}

		private sealed class QueueItem : IHeapItem
		{
			internal QueueItem(SignalInfo signalInfo, AsyncResult<bool> asyncResult, WaitHandle cancelItem, params object[] arguments)
			{
				this.signalInfo = signalInfo;
				this.asyncResult = asyncResult;
				this.cancelItem = cancelItem;
				this.arguments = arguments;
				this.sequence = Interlocked.Increment(ref StatefulComponent.QueueItem.globalSequence);
			}

			public int Handle
			{
				get
				{
					return this.heapHandle;
				}
				set
				{
					this.heapHandle = value;
				}
			}

			internal SignalInfo SignalInfo
			{
				get
				{
					return this.signalInfo;
				}
			}

			internal object[] Arguments
			{
				get
				{
					return this.arguments;
				}
			}

			internal AsyncResult<bool> AsyncResult
			{
				get
				{
					return this.asyncResult;
				}
			}

			internal long Sequence
			{
				get
				{
					return this.sequence;
				}
			}

			public override string ToString()
			{
				return string.Format("{0}, seq: {1}", this.SignalInfo.ToString(), this.Sequence);
			}

			internal void MarkAsEnqueued()
			{
				this.stopWatch = Stopwatch.StartNew();
			}

			internal long MarkAsDispatched()
			{
				this.stopWatch.Stop();
				return this.stopWatch.ElapsedMilliseconds;
			}

			internal void Cancel()
			{
				this.signalInfo = this.signalInfo.Clone();
				this.signalInfo.Value |= 4026531840U;
			}

			internal void CheckForCancellation()
			{
				if (this.cancelItem != null && this.cancelItem.WaitOne(0))
				{
					this.Cancel();
				}
			}

			private static long globalSequence;

			private readonly object[] arguments;

			private readonly AsyncResult<bool> asyncResult;

			private readonly long sequence;

			private readonly WaitHandle cancelItem;

			private int heapHandle;

			private SignalInfo signalInfo;

			private Stopwatch stopWatch;
		}
	}
}
