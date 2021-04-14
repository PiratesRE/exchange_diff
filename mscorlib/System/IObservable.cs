using System;

namespace System
{
	[__DynamicallyInvokable]
	public interface IObservable<out T>
	{
		[__DynamicallyInvokable]
		IDisposable Subscribe(IObserver<T> observer);
	}
}
