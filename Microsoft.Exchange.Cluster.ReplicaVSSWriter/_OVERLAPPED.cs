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
	internal struct $UnnamedClass$0x8c154cf1$132$
	{
		[FieldOffset(0)]
		private long <alignment\u0020member>;

		[NativeCppClass]
		[StructLayout(LayoutKind.Sequential, Size = 8)]
		internal struct $UnnamedClass$0x8c154cf1$133$
		{
			private int <alignment\u0020member>;
		}
	}
}
