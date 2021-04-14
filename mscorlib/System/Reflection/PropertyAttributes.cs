using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum PropertyAttributes
	{
		[__DynamicallyInvokable]
		None = 0,
		[__DynamicallyInvokable]
		SpecialName = 512,
		ReservedMask = 62464,
		[__DynamicallyInvokable]
		RTSpecialName = 1024,
		[__DynamicallyInvokable]
		HasDefault = 4096,
		Reserved2 = 8192,
		Reserved3 = 16384,
		Reserved4 = 32768
	}
}
