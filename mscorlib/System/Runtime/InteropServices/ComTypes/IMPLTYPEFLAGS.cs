using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Flags]
	[__DynamicallyInvokable]
	[Serializable]
	public enum IMPLTYPEFLAGS
	{
		[__DynamicallyInvokable]
		IMPLTYPEFLAG_FDEFAULT = 1,
		[__DynamicallyInvokable]
		IMPLTYPEFLAG_FSOURCE = 2,
		[__DynamicallyInvokable]
		IMPLTYPEFLAG_FRESTRICTED = 4,
		[__DynamicallyInvokable]
		IMPLTYPEFLAG_FDEFAULTVTABLE = 8
	}
}
