using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal abstract class WsAsyncProxyWrapper : SoapHttpClientProtocol, IDisposeTrackable, IDisposable
	{
		public WsAsyncProxyWrapper()
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<WsAsyncProxyWrapper>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		protected override void Dispose(bool disposing)
		{
			lock (this.syncRoot)
			{
				if (!this.disposed)
				{
					this.disposed = true;
					if (disposing)
					{
						if (this.disposeTracker != null)
						{
							this.disposeTracker.Dispose();
							this.disposeTracker = null;
						}
						if (this.timer != null)
						{
							this.timer.Dispose();
							this.timer = null;
						}
					}
					base.Dispose(disposing);
				}
			}
		}

		protected new IAsyncResult BeginInvoke(string methodName, object[] parameters, AsyncCallback callback, object asyncState)
		{
			IAsyncResult result;
			lock (this.syncRoot)
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("WsAsyncProxyWrapper");
				}
				if (this.asyncResult != null)
				{
					throw new InvalidOperationException("Single WsAsyncProxyWrapper does not support multiple async call in parallel!");
				}
				this.timerCallbackInvoked = false;
				this.actualCallback = callback;
				this.exceptionFromBeginInvoke = null;
				IAsyncResult asyncResult = null;
				try
				{
					asyncResult = base.BeginInvoke(methodName, parameters, new AsyncCallback(this.InternalWsCallback), asyncState);
				}
				catch (IOException ex)
				{
					this.exceptionFromBeginInvoke = ex;
				}
				catch (InvalidOperationException ex2)
				{
					this.exceptionFromBeginInvoke = ex2;
				}
				if (this.exceptionFromBeginInvoke != null)
				{
					asyncResult = new LazyAsyncResult(null, asyncState, callback);
					((LazyAsyncResult)asyncResult).InvokeCallback(this.exceptionFromBeginInvoke);
				}
				else if (!asyncResult.CompletedSynchronously)
				{
					this.asyncResult = asyncResult;
					this.StartTimer(asyncResult);
				}
				result = asyncResult;
			}
			return result;
		}

		protected new object[] EndInvoke(IAsyncResult result)
		{
			if (this.timerCallbackInvoked)
			{
				throw new WebException(string.Format(CultureInfo.InvariantCulture, "Async Request to {0} timed out", new object[]
				{
					base.Url
				}), WebExceptionStatus.Timeout);
			}
			if (this.exceptionFromBeginInvoke != null)
			{
				throw this.exceptionFromBeginInvoke;
			}
			return base.EndInvoke(result);
		}

		private void InternalWsCallback(IAsyncResult result)
		{
			lock (this.syncRoot)
			{
				if (!this.disposed)
				{
					if (result.CompletedSynchronously || result == this.asyncResult)
					{
						this.StopTimer();
						this.asyncResult = null;
						if (this.actualCallback != null)
						{
							AsyncCallback asyncCallback = this.actualCallback;
							this.actualCallback = null;
							asyncCallback(result);
						}
					}
				}
			}
		}

		private void InternalTimerCallback(object state)
		{
			lock (this.syncRoot)
			{
				if (!this.disposed)
				{
					this.Abort();
					this.timerCallbackInvoked = true;
					this.InternalWsCallback(state as IAsyncResult);
				}
			}
		}

		private void StartTimer(object state)
		{
			if (this.timer != null)
			{
				this.timer.Dispose();
			}
			this.timer = new Timer(new TimerCallback(this.InternalTimerCallback), state, base.Timeout, -1);
			if (this.stopwatch == null)
			{
				this.stopwatch = new Stopwatch();
			}
			else
			{
				this.stopwatch.Reset();
			}
			this.stopwatch.Start();
		}

		private void StopTimer()
		{
			if (this.timer != null)
			{
				this.timer.Change(-1, -1);
			}
			if (this.stopwatch != null && this.stopwatch.IsRunning)
			{
				this.stopwatch.Stop();
			}
		}

		public long GetElapsedMilliseconds()
		{
			if (this.stopwatch != null)
			{
				return this.stopwatch.ElapsedMilliseconds;
			}
			return 0L;
		}

		public static AsyncCallback WrapCallbackWithUnhandledExceptionHandlerAndCrash(AsyncCallback callback)
		{
			if (callback == null)
			{
				return null;
			}
			return delegate(IAsyncResult asyncResult)
			{
				try
				{
					callback(asyncResult);
				}
				catch (Exception exception)
				{
					ExWatson.SendReportAndCrashOnAnotherThread(exception);
				}
			};
		}

		private bool timerCallbackInvoked;

		private Timer timer;

		private Stopwatch stopwatch;

		private object syncRoot = new object();

		private AsyncCallback actualCallback;

		private IAsyncResult asyncResult;

		private Exception exceptionFromBeginInvoke;

		private DisposeTracker disposeTracker;

		private bool disposed;
	}
}
