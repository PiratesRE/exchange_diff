using System;
using System.Diagnostics;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal abstract class Executable : IExecutable, IDiagnosable, IDisposeTrackable, IDisposable
	{
		protected Executable(ISearchServiceConfig config)
		{
			this.config = config;
			this.diagnosticsSession = Microsoft.Exchange.Search.Core.Diagnostics.DiagnosticsSession.CreateComponentDiagnosticsSession("Executable", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.CoreComponentTracer, (long)this.GetHashCode());
			this.disposeTracker = this.GetDisposeTracker();
		}

		public IDiagnosticsSession DiagnosticsSession
		{
			[DebuggerStepThrough]
			get
			{
				return this.diagnosticsSession;
			}
		}

		public ISearchServiceConfig Config
		{
			[DebuggerStepThrough]
			get
			{
				return this.config;
			}
		}

		public string InstanceName
		{
			[DebuggerStepThrough]
			get
			{
				if (!string.IsNullOrEmpty(this.instanceName))
				{
					return this.instanceName;
				}
				return this.GetDiagnosticComponentName();
			}
			[DebuggerStepThrough]
			set
			{
				this.instanceName = value;
			}
		}

		public ICancelableAsyncResult AsyncResult
		{
			[DebuggerStepThrough]
			get
			{
				return this.executeAsyncResult;
			}
		}

		public WaitHandle StopEvent
		{
			[DebuggerStepThrough]
			get
			{
				return this.stopEvent;
			}
		}

		public bool Stopping
		{
			[DebuggerStepThrough]
			get
			{
				return this.stopping;
			}
		}

		public IAsyncResult BeginExecute(AsyncCallback callback, object state)
		{
			LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = new LazyAsyncResultWithTimeout(callback, state, new AsyncCallback(this.ExecuteComplete));
			if (Interlocked.CompareExchange<LazyAsyncResultWithTimeout>(ref this.executeAsyncResult, lazyAsyncResultWithTimeout, null) != null)
			{
				lazyAsyncResultWithTimeout.InvokeCallback(new InvalidOperationException("Only one execution allowed."));
				return lazyAsyncResultWithTimeout;
			}
			ThreadPool.QueueUserWorkItem(CallbackWrapper.WaitCallback(new WaitCallback(this.InternalExecutionStart)));
			return this.executeAsyncResult;
		}

		public void EndExecute(IAsyncResult asyncResult)
		{
			if (this.executeAsyncResult == null)
			{
				throw new InvalidOperationException("BeginExecute must be called before EndExecute.");
			}
			if (!object.ReferenceEquals(this.executeAsyncResult, asyncResult))
			{
				throw new InvalidOperationException("Passed in IAsyncResult does not match outstanding IASyncResult for this Executable.");
			}
			LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = LazyAsyncResult.EndAsyncOperation<LazyAsyncResultWithTimeout>(asyncResult);
			if (lazyAsyncResultWithTimeout.IsCanceled)
			{
				this.diagnosticsSession.TraceDebug("Execution complete. (Canceled)", new object[0]);
				return;
			}
			if (lazyAsyncResultWithTimeout.Result == null)
			{
				this.diagnosticsSession.TraceDebug("Execution complete.", new object[0]);
				return;
			}
			if (lazyAsyncResultWithTimeout.Result is Exception)
			{
				this.diagnosticsSession.TraceError("Execution complete with exception: {0}", new object[]
				{
					lazyAsyncResultWithTimeout.Result
				});
				throw new OperationFailedException((Exception)lazyAsyncResultWithTimeout.Result);
			}
		}

		public void CancelExecute()
		{
			LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = new LazyAsyncResultWithTimeout(null, null, null);
			lazyAsyncResultWithTimeout.Cancel();
			LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout2 = Interlocked.CompareExchange<LazyAsyncResultWithTimeout>(ref this.executeAsyncResult, lazyAsyncResultWithTimeout, null);
			if (lazyAsyncResultWithTimeout2 != null)
			{
				lazyAsyncResultWithTimeout2.Cancel();
			}
		}

		public string GetDiagnosticComponentName()
		{
			return base.GetType().Name;
		}

		public virtual XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			if (this.executeAsyncResult != null && this.executeAsyncResult.Result != null)
			{
				xelement.Add(new XElement("Error", this.executeAsyncResult.Result));
			}
			return xelement;
		}

		public void Dispose()
		{
			this.InternalDispose(true);
			GC.SuppressFinalize(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public abstract DisposeTracker GetDisposeTracker();

		protected abstract void InternalExecutionStart();

		protected abstract void InternalExecutionFinish();

		protected virtual void Dispose(bool calledFromDispose)
		{
		}

		protected bool TryRunUnderExceptionHandler<TReturnValue>(Func<TReturnValue> action, out TReturnValue returnValue, LocalizedString message)
		{
			TReturnValue tempReturnValue = default(TReturnValue);
			bool result = this.TryRunUnderExceptionHandler(delegate()
			{
				tempReturnValue = action();
			}, message);
			returnValue = tempReturnValue;
			return result;
		}

		protected bool TryRunUnderExceptionHandler(Action action, LocalizedString message)
		{
			ComponentFailedException value = null;
			try
			{
				action();
				return true;
			}
			catch (ComponentFailedPermanentException innerException)
			{
				value = new ComponentFailedPermanentException(message, innerException);
			}
			catch (ComponentFailedTransientException innerException2)
			{
				value = new ComponentFailedTransientException(message, innerException2);
			}
			catch (OperationFailedException innerException3)
			{
				value = new ComponentFailedTransientException(message, innerException3);
			}
			catch (Exception ex)
			{
				this.diagnosticsSession.SendWatsonReport(ex);
				value = new ComponentFailedPermanentException(message, ex);
			}
			this.executeAsyncResult.InvokeCallback(value);
			return false;
		}

		protected void CompleteExecute(object result)
		{
			this.DiagnosticsSession.TraceDebug("CompleteExecute: {0}", new object[]
			{
				result ?? "Success"
			});
			this.executeAsyncResult.InvokeCallback(result);
		}

		protected XElement BuildDiagnosticsErrorNode(string reason)
		{
			this.diagnosticsSession.TraceError<string>("Error executing Diagnostics command: {0}", reason);
			return new XElement("Error", reason);
		}

		private void ExecuteComplete(IAsyncResult passedAsyncResult)
		{
			this.stopping = true;
			this.stopEvent.Set();
			this.InternalExecutionFinish();
			LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (LazyAsyncResultWithTimeout)passedAsyncResult;
			AsyncCallback userCallback = (AsyncCallback)lazyAsyncResultWithTimeout.AsyncObject;
			if (userCallback != null)
			{
				ThreadPool.QueueUserWorkItem(CallbackWrapper.WaitCallback(delegate(object ar)
				{
					userCallback((IAsyncResult)ar);
				}), lazyAsyncResultWithTimeout);
			}
		}

		private void InternalExecutionStart(object state)
		{
			try
			{
				this.InternalExecutionStart();
			}
			catch (Exception result)
			{
				this.CompleteExecute(result);
			}
		}

		private void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.AsyncResult != null && !this.AsyncResult.IsCompleted)
			{
				this.CompleteExecute(null);
				this.AsyncResult.AsyncWaitHandle.WaitOne(this.Config.MaxOperationTimeout);
			}
			this.Dispose(calledFromDispose);
			if (calledFromDispose)
			{
				this.stopEvent.Dispose();
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
			}
		}

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly ISearchServiceConfig config;

		private DisposeTracker disposeTracker;

		private string instanceName;

		private LazyAsyncResultWithTimeout executeAsyncResult;

		private ManualResetEvent stopEvent = new ManualResetEvent(false);

		private bool stopping;
	}
}
