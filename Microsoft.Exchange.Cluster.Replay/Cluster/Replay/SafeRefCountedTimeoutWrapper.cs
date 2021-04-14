using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class SafeRefCountedTimeoutWrapper : DisposeTrackableBase
	{
		private static Trace Tracer
		{
			get
			{
				return SafeRefCountedTimeoutWrapper.s_trace;
			}
		}

		protected SafeRefCountedTimeoutWrapper(string debugName) : this(debugName, null)
		{
		}

		protected SafeRefCountedTimeoutWrapper(string debugName, ManualOneShotEvent cancelEvent)
		{
			this.m_name = debugName;
			this.m_cancelEvent = cancelEvent;
		}

		protected string Name
		{
			get
			{
				return this.m_name;
			}
		}

		protected ManualOneShotEvent CancelEvent
		{
			get
			{
				return this.m_cancelEvent;
			}
		}

		protected abstract void InternalProtectedDispose();

		protected virtual Exception GetOperationTimedOutException(string operationName, TimeoutException timeoutEx)
		{
			return timeoutEx;
		}

		protected virtual Exception GetOperationCanceledException(string operationName, OperationAbortedException abortedEx)
		{
			return abortedEx;
		}

		protected void ProtectedCall(string operationName, Action operation)
		{
			this.ProtectedCallWithTimeout(operationName, InvokeWithTimeout.InfiniteTimeSpan, operation);
		}

		protected void ProtectedCallWithTimeout(string operationName, TimeSpan timeout, Action operation)
		{
			try
			{
				if (!this.IncrementRefCountIfNecessary())
				{
					throw this.GetOperationTimedOutException(operationName, new TimeoutException(string.Format("RefCount fails. Name={0}", this.Name)));
				}
				Action invokableAction = delegate()
				{
					try
					{
						operation();
					}
					finally
					{
						this.DecrementRefCountAndDisposeIfNecessary();
					}
				};
				InvokeWithTimeout.Invoke(invokableAction, timeout, this.m_cancelEvent);
			}
			catch (OperationAbortedException ex)
			{
				SafeRefCountedTimeoutWrapper.Tracer.TraceError<string, OperationAbortedException>((long)this.GetHashCode(), "SafeRefCountedTimeoutWrapper.ProtectedCallWithTimeout(): Operation '{0}' got canceled. Exception: {1}", operationName, ex);
				throw this.GetOperationCanceledException(operationName, ex);
			}
			catch (TimeoutException ex2)
			{
				SafeRefCountedTimeoutWrapper.Tracer.TraceError<string, TimeoutException>((long)this.GetHashCode(), "SafeRefCountedTimeoutWrapper.ProtectedCallWithTimeout(): Operation '{0}' timed out. Exception: {1}", operationName, ex2);
				throw this.GetOperationTimedOutException(operationName, ex2);
			}
		}

		public override void Dispose()
		{
			lock (this.m_lockObj)
			{
				if (this.m_disposeCompleted)
				{
					SafeRefCountedTimeoutWrapper.Tracer.TraceDebug<string>((long)this.GetHashCode(), "SafeRefCountedTimeoutWrapper.Dispose(): {0}: Object has already been disposed. Doing nothing.", this.Name);
					return;
				}
				this.m_disposeRequested = true;
				if (this.m_threadsInProtectedCalls > 0)
				{
					SafeRefCountedTimeoutWrapper.Tracer.TraceDebug<string, int>((long)this.GetHashCode(), "SafeRefCountedTimeoutWrapper.Dispose(): {0}: There are currently {1} threads in protected calls. Delaying the dispose until the last thread completes.", this.Name, this.m_threadsInProtectedCalls);
					return;
				}
			}
			base.Dispose();
		}

		protected override void InternalDispose(bool disposing)
		{
			bool flag = false;
			lock (this.m_lockObj)
			{
				if (!this.m_disposeCompleted && !this.m_disposeStarted)
				{
					flag = true;
					this.m_disposeStarted = true;
				}
			}
			if (flag)
			{
				if (disposing)
				{
					this.m_dbgDisposeCalledUtc = DateTimeHelper.ToPersistedString(DateTime.UtcNow);
					this.InternalProtectedDispose();
				}
				this.m_disposeCompleted = true;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SafeRefCountedTimeoutWrapper>(this);
		}

		protected bool IncrementRefCountIfNecessary()
		{
			bool result;
			lock (this.m_lockObj)
			{
				if (this.m_disposeRequested)
				{
					SafeRefCountedTimeoutWrapper.Tracer.TraceError<string>((long)this.GetHashCode(), "SafeRefCountedTimeoutWrapper.IncrementRefCountIfNecessary(): {0}: m_disposeRequested is true, so returning false to stop issuing new protected calls!", this.Name);
					result = false;
				}
				else
				{
					this.m_threadsInProtectedCalls++;
					SafeRefCountedTimeoutWrapper.Tracer.TraceDebug<string, int>((long)this.GetHashCode(), "SafeRefCountedTimeoutWrapper.IncrementRefCountIfNecessary(): {0}: Successfully recorded a new protected call. m_threadsInProtectedCalls = {1}", this.Name, this.m_threadsInProtectedCalls);
					result = true;
				}
			}
			return result;
		}

		protected void DecrementRefCountAndDisposeIfNecessary()
		{
			bool flag = false;
			lock (this.m_lockObj)
			{
				this.m_threadsInProtectedCalls--;
				if (this.m_disposeRequested)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.Dispose();
			}
		}

		private readonly string m_name;

		private readonly object m_lockObj = new object();

		private readonly ManualOneShotEvent m_cancelEvent;

		private static Trace s_trace = ExTraceGlobals.ReplayApiTracer;

		private bool m_disposeRequested;

		private bool m_disposeCompleted;

		private bool m_disposeStarted;

		private int m_threadsInProtectedCalls;

		private string m_dbgDisposeCalledUtc;
	}
}
