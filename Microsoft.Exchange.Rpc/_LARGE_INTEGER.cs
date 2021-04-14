using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[NativeCppClass]
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal struct _LARGE_INTEGER
{
	[FieldOffset(0)]
	private long <alignment\u0020member>;

	[NativeCppClass]
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	internal struct $UnnamedClass$0xf73bf4a3$2$
	{
		private int <alignment\u0020member>;
	}

	[NativeCppClass]
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	public struct <unnamed-type-u>
	{
		private int <alignment\u0020member>;
	}
}
