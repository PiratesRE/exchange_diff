using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[NativeCppClass]
[StructLayout(LayoutKind.Sequential, Size = 48)]
internal struct _SYSTEM_INFO
{
	private long <alignment\u0020member>;

	[NativeCppClass]
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	internal struct $UnnamedClass$0xf73bf4a3$135$
	{
		[FieldOffset(0)]
		private int <alignment\u0020member>;

		[NativeCppClass]
		[StructLayout(LayoutKind.Sequential, Size = 4)]
		internal struct $UnnamedClass$0xf73bf4a3$136$
		{
			private short <alignment\u0020member>;
		}
	}
}
