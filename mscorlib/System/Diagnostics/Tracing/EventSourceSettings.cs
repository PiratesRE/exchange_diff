using System;

namespace System.Diagnostics.Tracing
{
	[Flags]
	[__DynamicallyInvokable]
	public enum EventSourceSettings
	{
		[__DynamicallyInvokable]
		Default = 0,
		[__DynamicallyInvokable]
		ThrowOnEventWriteErrors = 1,
		[__DynamicallyInvokable]
		EtwManifestEventFormat = 4,
		[__DynamicallyInvokable]
		EtwSelfDescribingEventFormat = 8
	}
}
