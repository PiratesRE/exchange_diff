﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ATL
{
	[NativeCppClass]
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	internal struct CComPtr<IUnknown>
	{
		private long <alignment\u0020member>;
	}
}