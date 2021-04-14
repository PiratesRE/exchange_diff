using System;

namespace System.Runtime.CompilerServices
{
	[__DynamicallyInvokable]
	public interface IAsyncStateMachine
	{
		[__DynamicallyInvokable]
		void MoveNext();

		[__DynamicallyInvokable]
		void SetStateMachine(IAsyncStateMachine stateMachine);
	}
}
