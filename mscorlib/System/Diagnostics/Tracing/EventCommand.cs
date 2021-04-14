using System;

namespace System.Diagnostics.Tracing
{
	[__DynamicallyInvokable]
	public enum EventCommand
	{
		[__DynamicallyInvokable]
		Update,
		[__DynamicallyInvokable]
		SendManifest = -1,
		[__DynamicallyInvokable]
		Enable = -2,
		[__DynamicallyInvokable]
		Disable = -3
	}
}
