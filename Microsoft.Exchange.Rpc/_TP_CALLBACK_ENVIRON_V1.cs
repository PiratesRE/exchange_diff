using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[NativeCppClass]
[StructLayout(LayoutKind.Sequential, Size = 64)]
internal struct _TP_CALLBACK_ENVIRON_V1
{
	private long <alignment\u0020member>;

	[NativeCppClass]
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	public struct <unnamed-type-u>
	{
		[FieldOffset(0)]
		private int <alignment\u0020member>;

		[NativeCppClass]
		[CLSCompliant(false)]
		[StructLayout(LayoutKind.Sequential, Size = 4)]
		public struct <unnamed-type-s>
		{
			private int <alignment\u0020member>;
		}
	}
}
