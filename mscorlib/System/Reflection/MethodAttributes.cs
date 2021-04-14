using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum MethodAttributes
	{
		[__DynamicallyInvokable]
		MemberAccessMask = 7,
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
		Final = 32,
		[__DynamicallyInvokable]
		Virtual = 64,
		[__DynamicallyInvokable]
		HideBySig = 128,
		[__DynamicallyInvokable]
		CheckAccessOnOverride = 512,
		[__DynamicallyInvokable]
		VtableLayoutMask = 256,
		[__DynamicallyInvokable]
		ReuseSlot = 0,
		[__DynamicallyInvokable]
		NewSlot = 256,
		[__DynamicallyInvokable]
		Abstract = 1024,
		[__DynamicallyInvokable]
		SpecialName = 2048,
		[__DynamicallyInvokable]
		PinvokeImpl = 8192,
		[__DynamicallyInvokable]
		UnmanagedExport = 8,
		[__DynamicallyInvokable]
		RTSpecialName = 4096,
		ReservedMask = 53248,
		[__DynamicallyInvokable]
		HasSecurity = 16384,
		[__DynamicallyInvokable]
		RequireSecObject = 32768
	}
}
