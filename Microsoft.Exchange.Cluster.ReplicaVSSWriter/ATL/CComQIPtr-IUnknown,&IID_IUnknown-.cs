using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ATL
{
	[NativeCppClass]
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	internal struct CComQIPtr<IUnknown,&IID_IUnknown>
	{
		private long <alignment\u0020member>;
	}
}
