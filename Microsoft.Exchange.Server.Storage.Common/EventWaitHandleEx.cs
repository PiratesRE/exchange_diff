using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class EventWaitHandleEx : DisposableBase
	{
		public EventWaitHandleEx(bool manual, bool initialState)
		{
			if (manual)
			{
				this.waitHandle = new ManualResetEvent(initialState);
				return;
			}
			this.waitHandle = new AutoResetEvent(initialState);
		}

		public bool WaitOne(TimeSpan duration)
		{
			bool result;
			try
			{
				result = this.waitHandle.WaitOne(duration);
			}
			catch (ObjectDisposedException exception)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
				result = true;
			}
			return result;
		}

		public bool Set()
		{
			bool result;
			try
			{
				result = this.waitHandle.Set();
			}
			catch (ObjectDisposedException exception)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
				result = false;
			}
			return result;
		}

		public bool Reset()
		{
			bool result;
			try
			{
				result = this.waitHandle.Reset();
			}
			catch (ObjectDisposedException exception)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
				result = false;
			}
			return result;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EventWaitHandleEx>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.waitHandle != null)
			{
				this.waitHandle.Dispose();
			}
		}

		private EventWaitHandle waitHandle;
	}
}
