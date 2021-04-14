using System;

namespace System.Diagnostics.Tracing
{
	[__DynamicallyInvokable]
	public enum EventFieldFormat
	{
		[__DynamicallyInvokable]
		Default,
		[__DynamicallyInvokable]
		String = 2,
		[__DynamicallyInvokable]
		Boolean,
		[__DynamicallyInvokable]
		Hexadecimal,
		[__DynamicallyInvokable]
		Xml = 11,
		[__DynamicallyInvokable]
		Json,
		[__DynamicallyInvokable]
		HResult = 15
	}
}
