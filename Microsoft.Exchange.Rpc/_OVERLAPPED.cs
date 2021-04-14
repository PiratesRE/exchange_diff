using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[NativeCppClass]
[StructLayout(LayoutKind.Sequential, Size = 32)]
internal struct _OVERLAPPED
{
	private long <alignment\u0020member>;

	[NativeCppClass]
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	internal struct $UnnamedClass$0xfb2f6e25$127$
	{
		[FieldOffset(0)]
		private long <alignment\u0020member>;

		[NativeCppClass]
		[StructLayout(LayoutKind.Sequential, Size = 8)]
		internal struct $UnnamedClass$0xfb2f6e25$128$
		{
			private int <alignment\u0020member>;
		}
	}
}
