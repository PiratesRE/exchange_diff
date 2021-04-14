using System;

namespace System.Collections.Generic
{
	[__DynamicallyInvokable]
	public interface IEqualityComparer<in T>
	{
		[__DynamicallyInvokable]
		bool Equals(T x, T y);

		[__DynamicallyInvokable]
		int GetHashCode(T obj);
	}
}
