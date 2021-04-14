using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum AssemblyNameFlags
	{
		[__DynamicallyInvokable]
		None = 0,
		[__DynamicallyInvokable]
		PublicKey = 1,
		EnableJITcompileOptimizer = 16384,
		EnableJITcompileTracking = 32768,
		[__DynamicallyInvokable]
		Retargetable = 256
	}
}
