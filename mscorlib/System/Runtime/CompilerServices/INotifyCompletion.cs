using System;

namespace System.Runtime.CompilerServices
{
	[__DynamicallyInvokable]
	public interface INotifyCompletion
	{
		[__DynamicallyInvokable]
		void OnCompleted(Action continuation);
	}
}
