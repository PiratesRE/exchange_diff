using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum ParameterAttributes
	{
		[__DynamicallyInvokable]
		None = 0,
		[__DynamicallyInvokable]
		In = 1,
		[__DynamicallyInvokable]
		Out = 2,
		[__DynamicallyInvokable]
		Lcid = 4,
		[__DynamicallyInvokable]
		Retval = 8,
		[__DynamicallyInvokable]
		Optional = 16,
		ReservedMask = 61440,
		[__DynamicallyInvokable]
		HasDefault = 4096,
		[__DynamicallyInvokable]
		HasFieldMarshal = 8192,
		Reserved3 = 16384,
		Reserved4 = 32768
	}
}
