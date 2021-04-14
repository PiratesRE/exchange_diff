using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ATL
{
	[NativeCppClass]
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	internal struct ClassesAllowedInStream
	{
		[FieldOffset(0)]
		private long <alignment\u0020member>;
	}
}
