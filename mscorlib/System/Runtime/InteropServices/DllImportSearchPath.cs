using System;

namespace System.Runtime.InteropServices
{
	[Flags]
	[__DynamicallyInvokable]
	public enum DllImportSearchPath
	{
		[__DynamicallyInvokable]
		UseDllDirectoryForDependencies = 256,
		[__DynamicallyInvokable]
		ApplicationDirectory = 512,
		[__DynamicallyInvokable]
		UserDirectories = 1024,
		[__DynamicallyInvokable]
		System32 = 2048,
		[__DynamicallyInvokable]
		SafeDirectories = 4096,
		[__DynamicallyInvokable]
		AssemblyDirectory = 2,
		[__DynamicallyInvokable]
		LegacyBehavior = 0
	}
}
