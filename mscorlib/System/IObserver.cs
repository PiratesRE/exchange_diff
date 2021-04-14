using System;

namespace System
{
	[__DynamicallyInvokable]
	public interface IObserver<in T>
	{
		[__DynamicallyInvokable]
		void OnNext(T value);

		[__DynamicallyInvokable]
		void OnError(Exception error);

		[__DynamicallyInvokable]
		void OnCompleted();
	}
}
