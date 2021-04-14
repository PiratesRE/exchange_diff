using System;

namespace System
{
	[__DynamicallyInvokable]
	public interface IProgress<in T>
	{
		[__DynamicallyInvokable]
		void Report(T value);
	}
}
