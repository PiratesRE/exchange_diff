using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum FieldAttributes
	{
		[__DynamicallyInvokable]
		FieldAccessMask = 7,
		[__DynamicallyInvokable]
		PrivateScope = 0,
		[__DynamicallyInvokable]
		Private = 1,
		[__DynamicallyInvokable]
		FamANDAssem = 2,
		[__DynamicallyInvokable]
		Assembly = 3,
		[__DynamicallyInvokable]
		Family = 4,
		[__DynamicallyInvokable]
		FamORAssem = 5,
		[__DynamicallyInvokable]
		Public = 6,
		[__DynamicallyInvokable]
		Static = 16,
		[__DynamicallyInvokable]
		InitOnly = 32,
		[__DynamicallyInvokable]
		Literal = 64,
		[__DynamicallyInvokable]
		NotSerialized = 128,
		[__DynamicallyInvokable]
		SpecialName = 512,
		[__DynamicallyInvokable]
		PinvokeImpl = 8192,
		ReservedMask = 38144,
		[__DynamicallyInvokable]
		RTSpecialName = 1024,
		[__DynamicallyInvokable]
		HasFieldMarshal = 4096,
		[__DynamicallyInvokable]
		HasDefault = 32768,
		[__DynamicallyInvokable]
		HasFieldRVA = 256
	}
}
