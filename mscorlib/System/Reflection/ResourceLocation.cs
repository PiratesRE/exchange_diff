using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum ResourceLocation
	{
		[__DynamicallyInvokable]
		Embedded = 1,
		[__DynamicallyInvokable]
		ContainedInAnotherAssembly = 2,
		[__DynamicallyInvokable]
		ContainedInManifestFile = 4
	}
}
