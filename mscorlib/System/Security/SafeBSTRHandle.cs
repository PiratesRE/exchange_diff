using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace System.Security
{
	[SecurityCritical]
	[SuppressUnmanagedCodeSecurity]
	internal sealed class SafeBSTRHandle : SafeBuffer
	{
		internal SafeBSTRHandle() : base(true)
		{
		}

		internal static SafeBSTRHandle Allocate(string src, uint len)
		{
			SafeBSTRHandle safeBSTRHandle = SafeBSTRHandle.SysAllocStringLen(src, len);
			safeBSTRHandle.Initialize((ulong)(len * 2U));
			return safeBSTRHandle;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("oleaut32.dll", CharSet = CharSet.Unicode)]
		private static extern SafeBSTRHandle SysAllocStringLen(string src, uint len);

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			Win32Native.ZeroMemory(this.handle, (UIntPtr)(Win32Native.SysStringLen(this.handle) * 2U));
			Win32Native.SysFreeString(this.handle);
			return true;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal unsafe void ClearBuffer()
		{
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				base.AcquirePointer(ref ptr);
				Win32Native.ZeroMemory((IntPtr)((void*)ptr), (UIntPtr)(Win32Native.SysStringLen((IntPtr)((void*)ptr)) * 2U));
			}
			finally
			{
				if (ptr != null)
				{
					base.ReleasePointer();
				}
			}
		}

		internal int Length
		{
			get
			{
				return (int)Win32Native.SysStringLen(this);
			}
		}

		internal unsafe static void Copy(SafeBSTRHandle source, SafeBSTRHandle target)
		{
			byte* ptr = null;
			byte* ptr2 = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				source.AcquirePointer(ref ptr);
				target.AcquirePointer(ref ptr2);
				Buffer.Memcpy(ptr2, ptr, (int)(Win32Native.SysStringLen((IntPtr)((void*)ptr)) * 2U));
			}
			finally
			{
				if (ptr != null)
				{
					source.ReleasePointer();
				}
				if (ptr2 != null)
				{
					target.ReleasePointer();
				}
			}
		}
	}
}
