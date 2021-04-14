using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IAsyncResult
	{
		[__DynamicallyInvokable]
		bool IsCompleted { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		WaitHandle AsyncWaitHandle { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		object AsyncState { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		bool CompletedSynchronously { [__DynamicallyInvokable] get; }
	}
}
