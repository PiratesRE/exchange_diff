using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ATL._ATL_SAFE_ALLOCA_IMPL
{
	[NativeCppClass]
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	internal struct CAtlSafeAllocBufferManager<ATL::CCRTAllocator>
	{
		private long <alignment\u0020member>;

		[UnsafeValueType]
		[NativeCppClass]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		internal struct CAtlSafeAllocBufferNode
		{
			private long <alignment\u0020member>;
		}
	}
}
