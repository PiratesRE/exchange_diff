using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum CompareOptions
	{
		[__DynamicallyInvokable]
		None = 0,
		[__DynamicallyInvokable]
		IgnoreCase = 1,
		[__DynamicallyInvokable]
		IgnoreNonSpace = 2,
		[__DynamicallyInvokable]
		IgnoreSymbols = 4,
		[__DynamicallyInvokable]
		IgnoreKanaType = 8,
		[__DynamicallyInvokable]
		IgnoreWidth = 16,
		[__DynamicallyInvokable]
		OrdinalIgnoreCase = 268435456,
		[__DynamicallyInvokable]
		StringSort = 536870912,
		[__DynamicallyInvokable]
		Ordinal = 1073741824
	}
}
