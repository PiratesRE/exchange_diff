using System;
using System.Runtime.InteropServices;

namespace System
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum AttributeTargets
	{
		[__DynamicallyInvokable]
		Assembly = 1,
		[__DynamicallyInvokable]
		Module = 2,
		[__DynamicallyInvokable]
		Class = 4,
		[__DynamicallyInvokable]
		Struct = 8,
		[__DynamicallyInvokable]
		Enum = 16,
		[__DynamicallyInvokable]
		Constructor = 32,
		[__DynamicallyInvokable]
		Method = 64,
		[__DynamicallyInvokable]
		Property = 128,
		[__DynamicallyInvokable]
		Field = 256,
		[__DynamicallyInvokable]
		Event = 512,
		[__DynamicallyInvokable]
		Interface = 1024,
		[__DynamicallyInvokable]
		Parameter = 2048,
		[__DynamicallyInvokable]
		Delegate = 4096,
		[__DynamicallyInvokable]
		ReturnValue = 8192,
		[__DynamicallyInvokable]
		GenericParameter = 16384,
		[__DynamicallyInvokable]
		All = 32767
	}
}
