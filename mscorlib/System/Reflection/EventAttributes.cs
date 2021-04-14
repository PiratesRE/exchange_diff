using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum EventAttributes
	{
		[__DynamicallyInvokable]
		None = 0,
		[__DynamicallyInvokable]
		SpecialName = 512,
		ReservedMask = 1024,
		[__DynamicallyInvokable]
		RTSpecialName = 1024
	}
}
