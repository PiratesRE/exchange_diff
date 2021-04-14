using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum MethodImplAttributes
	{
		[__DynamicallyInvokable]
		CodeTypeMask = 3,
		[__DynamicallyInvokable]
		IL = 0,
		[__DynamicallyInvokable]
		Native,
		[__DynamicallyInvokable]
		OPTIL,
		[__DynamicallyInvokable]
		Runtime,
		[__DynamicallyInvokable]
		ManagedMask,
		[__DynamicallyInvokable]
		Unmanaged = 4,
		[__DynamicallyInvokable]
		Managed = 0,
		[__DynamicallyInvokable]
		ForwardRef = 16,
		[__DynamicallyInvokable]
		PreserveSig = 128,
		[__DynamicallyInvokable]
		InternalCall = 4096,
		[__DynamicallyInvokable]
		Synchronized = 32,
		[__DynamicallyInvokable]
		NoInlining = 8,
		[ComVisible(false)]
		[__DynamicallyInvokable]
		AggressiveInlining = 256,
		[__DynamicallyInvokable]
		NoOptimization = 64,
		SecurityMitigations = 1024,
		MaxMethodImplVal = 65535
	}
}
