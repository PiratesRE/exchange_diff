using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum CallingConvention
	{
		[__DynamicallyInvokable]
		Winapi = 1,
		[__DynamicallyInvokable]
		Cdecl,
		[__DynamicallyInvokable]
		StdCall,
		[__DynamicallyInvokable]
		ThisCall,
		FastCall
	}
}
