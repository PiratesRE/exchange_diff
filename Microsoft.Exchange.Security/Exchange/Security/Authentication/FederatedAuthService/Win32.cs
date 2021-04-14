using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	[SuppressUnmanagedCodeSecurity]
	[ComVisible(false)]
	internal class Win32
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DuplicateHandle(SafeHandle sourceProcessHandle, IntPtr sourceHandle, SafeHandle targetProcessHandle, ref IntPtr targetHandle, uint desiredAccess, bool inheritHandle, Win32.DuplicateHandleOptions options);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern SafeProcessHandle GetCurrentProcess();

		[DllImport("msvcrt.dll")]
		internal static extern int memcmp(byte[] a, byte[] b, long count);

		internal enum DuplicateHandleOptions : uint
		{
			DUPLICATE_SAME_ACCESS = 2U
		}

		internal enum Win32ErrorCodes : uint
		{
			ERROR_LOGON_FAILURE = 1326U
		}
	}
}
