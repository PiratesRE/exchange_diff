using System;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class NonBlockingReader : DisposableBase
	{
		internal NonBlockingReader(NonBlockingReader.Operation operation, object state, TimeSpan timeout, NonBlockingReader.TimeoutCallback timeOutCallback)
		{
			this.operation = operation;
			this.userState = state;
			this.timeout = timeout;
			this.timeoutCallback = timeOutCallback;
			this.evt = new ManualResetEvent(false);
		}

		internal bool TimeOutExpired
		{
			get
			{
				return this.timedOut;
			}
		}

		internal void StartAsyncOperation()
		{
			base.CheckDisposed();
			if (!this.forciblyCompleted)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "NonBlockingReader starting async operation because it has not been forcibly completed", new object[0]);
				DisposableBase disposableBase = this.userState as DisposableBase;
				if (disposableBase != null)
				{
					disposableBase.AddReference();
				}
				base.AddReference();
				this.operation.BeginInvoke(this.userState, new AsyncCallback(this.CompleteOperation), null);
			}
		}

		internal bool WaitForCompletion()
		{
			base.CheckDisposed();
			bool flag = false;
			if (!this.timedOut)
			{
				flag = this.evt.WaitOne(this.timeout, false);
				if (!flag)
				{
					this.timedOut = true;
					if (this.timeoutCallback != null)
					{
						this.timeoutCallback(this.userState);
					}
				}
			}
			return flag;
		}

		internal void ForceCompletion()
		{
			base.CheckDisposed();
			this.forciblyCompleted = true;
			this.evt.Set();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.evt.Close();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<NonBlockingReader>(this);
		}

		private void CompleteOperation(IAsyncResult r)
		{
			try
			{
				this.operation.EndInvoke(r);
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "NonBlockingReader encountered an exception in the user callback '{0}'", new object[]
				{
					ex
				});
				if (!GrayException.IsGrayException(ex))
				{
					throw;
				}
			}
			finally
			{
				this.evt.Set();
				DisposableBase disposableBase = this.userState as DisposableBase;
				if (disposableBase != null)
				{
					disposableBase.ReleaseReference();
				}
				base.ReleaseReference();
			}
		}

		private bool timedOut;

		private ManualResetEvent evt;

		private NonBlockingReader.Operation operation;

		private NonBlockingReader.TimeoutCallback timeoutCallback;

		private object userState;

		private TimeSpan timeout;

		private bool forciblyCompleted;

		internal delegate void TimeoutCallback(object state);

		internal delegate void Operation(object state);
	}
}
