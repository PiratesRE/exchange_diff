using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.Tracing
{
	[FriendAccessAllowed]
	[__DynamicallyInvokable]
	public enum EventOpcode
	{
		[__DynamicallyInvokable]
		Info,
		[__DynamicallyInvokable]
		Start,
		[__DynamicallyInvokable]
		Stop,
		[__DynamicallyInvokable]
		DataCollectionStart,
		[__DynamicallyInvokable]
		DataCollectionStop,
		[__DynamicallyInvokable]
		Extension,
		[__DynamicallyInvokable]
		Reply,
		[__DynamicallyInvokable]
		Resume,
		[__DynamicallyInvokable]
		Suspend,
		[__DynamicallyInvokable]
		Send,
		[__DynamicallyInvokable]
		Receive = 240
	}
}
