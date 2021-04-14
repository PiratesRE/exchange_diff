using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Win32
{
	internal sealed class IoCompletionPort : SafeHandleZeroOrMinusOneIsInvalid
	{
		private IoCompletionPort() : base(true)
		{
		}

		public IoCompletionPort(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		public bool GetQueuedCompletionStatus(out uint limitClass, out UIntPtr completionKey, out int processId, uint milliseconds)
		{
			processId = 0;
			IntPtr intPtr;
			if (NativeMethods.GetQueuedCompletionStatus(this, out limitClass, out completionKey, out intPtr, milliseconds))
			{
				processId = intPtr.ToInt32();
				return true;
			}
			NativeMethods.ErrorCode lastWin32Error = (NativeMethods.ErrorCode)Marshal.GetLastWin32Error();
			if (lastWin32Error == NativeMethods.ErrorCode.WaitTimeout)
			{
				return false;
			}
			throw new Win32Exception((int)lastWin32Error, "GetQueuedCompletionStatus failed with error code " + lastWin32Error);
		}

		public bool PostQueuedCompletionStatus(uint limitClass, uint completionKey)
		{
			IntPtr zero = IntPtr.Zero;
			if (!NativeMethods.PostQueuedCompletionStatus(this, limitClass, new UIntPtr(completionKey), zero))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32Exception(lastWin32Error, "PostQueuedCompletionStatus failed with error code " + lastWin32Error);
			}
			return true;
		}

		public bool PostQueuedCompletionStatus(uint limitClass, uint completionKey, IntPtr data)
		{
			if (!NativeMethods.PostQueuedCompletionStatus(this, limitClass, new UIntPtr(completionKey), data))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32Exception(lastWin32Error, "PostQueuedCompletionStatus failed with error code " + lastWin32Error);
			}
			return true;
		}

		public override bool IsInvalid
		{
			get
			{
				return this.handle == IntPtr.Zero;
			}
		}

		public static IoCompletionPort InvalidHandle
		{
			get
			{
				return new IoCompletionPort(IntPtr.Zero);
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			return IoCompletionPort.CloseHandle(this.handle);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("KERNEL32.DLL", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr handle);
	}
}
