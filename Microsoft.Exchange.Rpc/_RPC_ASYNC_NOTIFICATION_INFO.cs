using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[NativeCppClass]
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal struct _RPC_ASYNC_NOTIFICATION_INFO
{
	[FieldOffset(0)]
	private long <alignment\u0020member>;

	[CLSCompliant(false)]
	[NativeCppClass]
	[StructLayout(LayoutKind.Sequential, Size = 16)]
	public struct <unnamed-type-APC>
	{
		private long <alignment\u0020member>;
	}

	[CLSCompliant(false)]
	[NativeCppClass]
	[StructLayout(LayoutKind.Sequential, Size = 32)]
	public struct <unnamed-type-IOC>
	{
		private long <alignment\u0020member>;
	}

	[NativeCppClass]
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential, Size = 16)]
	public struct <unnamed-type-HWND>
	{
		private long <alignment\u0020member>;
	}
}
