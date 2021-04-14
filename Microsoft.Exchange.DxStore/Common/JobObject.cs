using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.DxStore.Common
{
	public class JobObject : DisposeTrackableBase
	{
		public JobObject(string name, long memorySizeLimitInMb = 0L)
		{
			this.safeJobHandle = NativeMethods.CreateJobObject(IntPtr.Zero, name);
			if (this.safeJobHandle.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				ExTraceGlobals.UtilsTracer.TraceError<string, int>((long)name.GetHashCode(), "{0}: Failed to create Job object. Win32 ErrorCode: {1}", name, lastWin32Error);
				throw new Win32Exception(lastWin32Error, "Failed to create Job object. Error code is {0}.");
			}
			NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION extendedLimits = default(NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION);
			extendedLimits.BasicLimitInformation.LimitFlags = 8192U;
			if (memorySizeLimitInMb > 0L)
			{
				extendedLimits.BasicLimitInformation.LimitFlags = (extendedLimits.BasicLimitInformation.LimitFlags | 256U);
				extendedLimits.ProcessMemoryLimit = new UIntPtr((uint)memorySizeLimitInMb * 1024U * 1024U);
			}
			this.safeJobHandle.SetExtendedLimits(extendedLimits);
		}

		public bool Add(Process process)
		{
			return process != null && this.safeJobHandle.Add(process);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.safeJobHandle != null)
			{
				this.safeJobHandle.Dispose();
				this.safeJobHandle = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JobObject>(this);
		}

		private const int Win32ErrorAccessDenied = 5;

		private const int Win32ErrorNotSupported = 50;

		private SafeJobHandle safeJobHandle;

		[Flags]
		private enum JobObjectExtendedLimit : uint
		{
			LimitProcessMemory = 256U,
			LimitKillOnJobClose = 8192U
		}
	}
}
