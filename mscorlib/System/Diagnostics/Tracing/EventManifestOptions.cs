using System;

namespace System.Diagnostics.Tracing
{
	[Flags]
	[__DynamicallyInvokable]
	public enum EventManifestOptions
	{
		[__DynamicallyInvokable]
		None = 0,
		[__DynamicallyInvokable]
		Strict = 1,
		[__DynamicallyInvokable]
		AllCultures = 2,
		[__DynamicallyInvokable]
		OnlyIfNeededForRegistration = 4,
		[__DynamicallyInvokable]
		AllowEventSourceOverride = 8
	}
}
