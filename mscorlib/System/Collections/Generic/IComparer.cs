using System;

namespace System.Collections.Generic
{
	[__DynamicallyInvokable]
	public interface IComparer<in T>
	{
		[__DynamicallyInvokable]
		int Compare(T x, T y);
	}
}
