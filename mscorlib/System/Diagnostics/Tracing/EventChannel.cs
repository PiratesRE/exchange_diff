using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.Tracing
{
	[FriendAccessAllowed]
	[__DynamicallyInvokable]
	public enum EventChannel : byte
	{
		[__DynamicallyInvokable]
		None,
		[__DynamicallyInvokable]
		Admin = 16,
		[__DynamicallyInvokable]
		Operational,
		[__DynamicallyInvokable]
		Analytic,
		[__DynamicallyInvokable]
		Debug
	}
}
