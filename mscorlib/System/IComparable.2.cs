using System;

namespace System
{
	[__DynamicallyInvokable]
	public interface IComparable<in T>
	{
		[__DynamicallyInvokable]
		int CompareTo(T other);
	}
}
