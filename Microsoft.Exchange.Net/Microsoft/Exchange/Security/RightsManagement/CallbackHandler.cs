using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[SecurityCritical(SecurityCriticalScope.Everything)]
	internal sealed class CallbackHandler : IDisposeTrackable, IDisposable
	{
		internal CallbackHandler()
		{
			this.resetEvent = new AutoResetEvent(false);
			this.callbackDelegate = new CallbackDelegate(this.OnStatus);
			this.disposeTracker = this.GetDisposeTracker();
		}

		internal CallbackDelegate CallbackDelegate
		{
			get
			{
				return this.callbackDelegate;
			}
		}

		internal string CallbackData
		{
			get
			{
				return this.callbackData;
			}
		}

		internal void WaitForCompletion()
		{
			this.resetEvent.WaitOne();
			if (this.exception != null)
			{
				throw this.exception;
			}
			Errors.ThrowOnErrorCode(this.hr);
		}

		internal bool WaitForCompletion(TimeSpan timeOut)
		{
			bool result = this.resetEvent.WaitOne(timeOut);
			if (this.exception != null)
			{
				throw this.exception;
			}
			Errors.ThrowOnErrorCode(this.hr);
			return result;
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private int OnStatus(StatusMessage status, int hr, IntPtr pvParam, IntPtr pvContext)
		{
			if (hr == 315140 || hr < 0)
			{
				this.exception = null;
				try
				{
					this.hr = hr;
					if (pvParam != IntPtr.Zero)
					{
						this.callbackData = Marshal.PtrToStringUni(pvParam);
					}
				}
				catch (Exception ex)
				{
					this.exception = ex;
				}
				finally
				{
					this.resetEvent.Set();
				}
			}
			return 0;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CallbackHandler>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed && disposing && this.resetEvent != null)
			{
				this.resetEvent.Set();
				((IDisposable)this.resetEvent).Dispose();
				this.resetEvent = null;
			}
			this.disposed = true;
		}

		private CallbackDelegate callbackDelegate;

		private AutoResetEvent resetEvent;

		private string callbackData;

		private int hr;

		private Exception exception;

		private DisposeTracker disposeTracker;

		private bool disposed;
	}
}
