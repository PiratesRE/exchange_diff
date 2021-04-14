using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[NativeCppClass]
[UnsafeValueType]
[StructLayout(LayoutKind.Sequential, Size = 152)]
internal struct tagRPC_EXTENDED_ERROR_INFO
{
	private long <alignment\u0020member>;

	[NativeCppClass]
	[UnsafeValueType]
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public struct <unnamed-type-u>
	{
		[FieldOffset(0)]
		private int <alignment\u0020member>;
	}
}
