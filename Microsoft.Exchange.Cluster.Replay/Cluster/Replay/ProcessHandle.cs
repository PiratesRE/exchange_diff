using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ProcessHandle : DisposeTrackableBase
	{
		public static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AmServiceMonitorTracer;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ProcessHandle>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this)
			{
				if (disposing)
				{
					if (this.eventWrapper != null)
					{
						this.eventWrapper.Dispose();
						this.eventWrapper = null;
					}
					if (this.safeWaitHandle != null)
					{
						this.safeWaitHandle.Dispose();
						this.safeWaitHandle = null;
					}
					if (this.process != null)
					{
						this.process = null;
					}
				}
			}
		}

		public WaitHandle WaitHandle
		{
			get
			{
				return this.eventWrapper;
			}
		}

		public bool TryGetWaitHandle(Process processToWatch, out Exception ex)
		{
			bool result = false;
			ex = null;
			if (this.process != null)
			{
				throw new InvalidOperationException("ProcessHandle is already in use");
			}
			this.process = processToWatch;
			try
			{
				IntPtr handle;
				try
				{
					handle = this.process.Handle;
				}
				catch (InvalidOperationException ex2)
				{
					ProcessHandle.Tracer.TraceError<InvalidOperationException>(0L, "ProcessHandle.TryGetWaitHandle failed to fetch process handle: {0}", ex2);
					ex = ex2;
					return false;
				}
				this.safeWaitHandle = new SafeWaitHandle(handle, false);
				this.eventWrapper = new ManualResetEvent(false);
				this.eventWrapper.SafeWaitHandle = this.safeWaitHandle;
				result = true;
			}
			catch (Win32Exception ex3)
			{
				ex = ex3;
				ProcessHandle.Tracer.TraceError<Win32Exception>(0L, "ProcessHandle.TryGetWaitHandle hit exception opening process handle: {0}", ex3);
				int hrforException = Marshal.GetHRForException(ex);
				if (hrforException != -2147467259)
				{
					throw;
				}
			}
			return result;
		}

		private Process process;

		private SafeWaitHandle safeWaitHandle;

		private ManualResetEvent eventWrapper;
	}
}
