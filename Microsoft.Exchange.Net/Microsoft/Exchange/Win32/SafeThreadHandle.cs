using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Win32
{
	internal sealed class SafeThreadHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeThreadHandle() : base(true)
		{
		}

		public static SafeThreadHandle GetCurrentThreadHandle()
		{
			SafeThreadHandle result;
			using (SafeProcessHandle currentProcess = NativeMethods.GetCurrentProcess())
			{
				using (SafeThreadHandle currentThread = NativeMethods.GetCurrentThread())
				{
					SafeThreadHandle safeThreadHandle;
					if (NativeMethods.DuplicateThreadHandle(currentProcess, currentThread, currentProcess, out safeThreadHandle, 0U, false, 2U))
					{
						result = safeThreadHandle;
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}

		public int CancelRpcRequest(int timeoutSeconds)
		{
			return NativeMethods.RpcCancelThreadEx(this, timeoutSeconds);
		}

		protected override bool ReleaseHandle()
		{
			return SafeThreadHandle.CloseHandle(this.handle);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("KERNEL32.DLL", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr handle);
	}
}
