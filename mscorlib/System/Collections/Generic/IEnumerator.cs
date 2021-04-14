using System;

namespace System.Collections.Generic
{
	[__DynamicallyInvokable]
	public interface IEnumerator<out T> : IDisposable, IEnumerator
	{
		[__DynamicallyInvokable]
		T Current { [__DynamicallyInvokable] get; }
	}
}
