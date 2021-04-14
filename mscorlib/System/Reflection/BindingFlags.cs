using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum BindingFlags
	{
		Default = 0,
		[__DynamicallyInvokable]
		IgnoreCase = 1,
		[__DynamicallyInvokable]
		DeclaredOnly = 2,
		[__DynamicallyInvokable]
		Instance = 4,
		[__DynamicallyInvokable]
		Static = 8,
		[__DynamicallyInvokable]
		Public = 16,
		[__DynamicallyInvokable]
		NonPublic = 32,
		[__DynamicallyInvokable]
		FlattenHierarchy = 64,
		InvokeMethod = 256,
		CreateInstance = 512,
		GetField = 1024,
		SetField = 2048,
		GetProperty = 4096,
		SetProperty = 8192,
		PutDispProperty = 16384,
		PutRefDispProperty = 32768,
		[__DynamicallyInvokable]
		ExactBinding = 65536,
		SuppressChangeType = 131072,
		[__DynamicallyInvokable]
		OptionalParamBinding = 262144,
		IgnoreReturn = 16777216
	}
}
