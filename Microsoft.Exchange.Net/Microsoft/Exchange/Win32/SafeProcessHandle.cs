using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Win32
{
	internal sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeProcessHandle() : base(true)
		{
		}

		internal bool HasExited
		{
			get
			{
				this.ValidateHandle();
				uint num = NativeMethods.WaitForSingleObject(this, 0U);
				if (num == 0U)
				{
					return true;
				}
				if (num == 258U)
				{
					return false;
				}
				throw new Win32Exception();
			}
		}

		internal static SafeProcessHandle CreateProcessAsUser(SafeUserTokenHandle hToken, string lpApplicationName, string lpCommandLine)
		{
			bool flag = false;
			SafeProcessHandle safeProcessHandle = new SafeProcessHandle();
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				NativeMethods.STARTUPINFO startupinfo = default(NativeMethods.STARTUPINFO);
				NativeMethods.PROCESS_INFORMATION process_INFORMATION = default(NativeMethods.PROCESS_INFORMATION);
				flag = NativeMethods.CreateProcessAsUser(hToken, lpApplicationName, lpCommandLine, IntPtr.Zero, IntPtr.Zero, true, 0U, IntPtr.Zero, null, ref startupinfo, ref process_INFORMATION);
				if (flag)
				{
					safeProcessHandle.SetHandle(process_INFORMATION.hProcess);
					SafeProcessHandle.CloseHandle(process_INFORMATION.hThread);
				}
			}
			if (!flag)
			{
				safeProcessHandle.Dispose();
				safeProcessHandle = null;
				throw new Win32Exception();
			}
			return safeProcessHandle;
		}

		internal static SafeProcessHandle CreateProcess(string lpApplicationName, string lpCommandLine)
		{
			bool flag = false;
			SafeProcessHandle safeProcessHandle = new SafeProcessHandle();
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				NativeMethods.STARTUPINFO startupinfo = default(NativeMethods.STARTUPINFO);
				NativeMethods.PROCESS_INFORMATION process_INFORMATION = default(NativeMethods.PROCESS_INFORMATION);
				flag = NativeMethods.CreateProcess(lpApplicationName, lpCommandLine, IntPtr.Zero, IntPtr.Zero, true, 0U, IntPtr.Zero, null, ref startupinfo, ref process_INFORMATION);
				if (flag)
				{
					safeProcessHandle.SetHandle(process_INFORMATION.hProcess);
					SafeProcessHandle.CloseHandle(process_INFORMATION.hThread);
				}
			}
			if (!flag)
			{
				safeProcessHandle.Dispose();
				safeProcessHandle = null;
				throw new Win32Exception();
			}
			return safeProcessHandle;
		}

		internal void TerminateProcess(uint exitCode)
		{
			this.ValidateHandle();
			if (!NativeMethods.TerminateProcess(this.handle, exitCode))
			{
				throw new Win32Exception();
			}
		}

		internal int GetProcessId()
		{
			this.ValidateHandle();
			uint processId = NativeMethods.GetProcessId(this.handle);
			if (processId == 0U)
			{
				throw new Win32Exception();
			}
			return (int)processId;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			return SafeProcessHandle.CloseHandle(this.handle);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("KERNEL32.DLL", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr handle);

		private void ValidateHandle()
		{
			if (this.IsInvalid)
			{
				throw new InvalidOperationException("Process handle is invalid.");
			}
		}
	}
}
