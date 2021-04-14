using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[NativeCppClass]
[StructLayout(LayoutKind.Sequential, Size = 64)]
internal struct EHExceptionRecord
{
	private long <alignment\u0020member>;

	[NativeCppClass]
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential, Size = 32)]
	public struct EHParameters
	{
		private long <alignment\u0020member>;
	}
}
