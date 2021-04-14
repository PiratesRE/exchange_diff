using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum MethodImplOptions
	{
		Unmanaged = 4,
		ForwardRef = 16,
		[__DynamicallyInvokable]
		PreserveSig = 128,
		InternalCall = 4096,
		Synchronized = 32,
		[__DynamicallyInvokable]
		NoInlining = 8,
		[ComVisible(false)]
		[__DynamicallyInvokable]
		AggressiveInlining = 256,
		[__DynamicallyInvokable]
		NoOptimization = 64,
		SecurityMitigations = 1024
	}
}
