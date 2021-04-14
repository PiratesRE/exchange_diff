using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[Serializable]
	public enum CALLCONV
	{
		[__DynamicallyInvokable]
		CC_CDECL = 1,
		[__DynamicallyInvokable]
		CC_MSCPASCAL,
		[__DynamicallyInvokable]
		CC_PASCAL = 2,
		[__DynamicallyInvokable]
		CC_MACPASCAL,
		[__DynamicallyInvokable]
		CC_STDCALL,
		[__DynamicallyInvokable]
		CC_RESERVED,
		[__DynamicallyInvokable]
		CC_SYSCALL,
		[__DynamicallyInvokable]
		CC_MPWCDECL,
		[__DynamicallyInvokable]
		CC_MPWPASCAL,
		[__DynamicallyInvokable]
		CC_MAX
	}
}
