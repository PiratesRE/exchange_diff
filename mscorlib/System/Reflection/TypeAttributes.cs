using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum TypeAttributes
	{
		[__DynamicallyInvokable]
		VisibilityMask = 7,
		[__DynamicallyInvokable]
		NotPublic = 0,
		[__DynamicallyInvokable]
		Public = 1,
		[__DynamicallyInvokable]
		NestedPublic = 2,
		[__DynamicallyInvokable]
		NestedPrivate = 3,
		[__DynamicallyInvokable]
		NestedFamily = 4,
		[__DynamicallyInvokable]
		NestedAssembly = 5,
		[__DynamicallyInvokable]
		NestedFamANDAssem = 6,
		[__DynamicallyInvokable]
		NestedFamORAssem = 7,
		[__DynamicallyInvokable]
		LayoutMask = 24,
		[__DynamicallyInvokable]
		AutoLayout = 0,
		[__DynamicallyInvokable]
		SequentialLayout = 8,
		[__DynamicallyInvokable]
		ExplicitLayout = 16,
		[__DynamicallyInvokable]
		ClassSemanticsMask = 32,
		[__DynamicallyInvokable]
		Class = 0,
		[__DynamicallyInvokable]
		Interface = 32,
		[__DynamicallyInvokable]
		Abstract = 128,
		[__DynamicallyInvokable]
		Sealed = 256,
		[__DynamicallyInvokable]
		SpecialName = 1024,
		[__DynamicallyInvokable]
		Import = 4096,
		[__DynamicallyInvokable]
		Serializable = 8192,
		[ComVisible(false)]
		[__DynamicallyInvokable]
		WindowsRuntime = 16384,
		[__DynamicallyInvokable]
		StringFormatMask = 196608,
		[__DynamicallyInvokable]
		AnsiClass = 0,
		[__DynamicallyInvokable]
		UnicodeClass = 65536,
		[__DynamicallyInvokable]
		AutoClass = 131072,
		[__DynamicallyInvokable]
		CustomFormatClass = 196608,
		[__DynamicallyInvokable]
		CustomFormatMask = 12582912,
		[__DynamicallyInvokable]
		BeforeFieldInit = 1048576,
		ReservedMask = 264192,
		[__DynamicallyInvokable]
		RTSpecialName = 2048,
		[__DynamicallyInvokable]
		HasSecurity = 262144
	}
}
