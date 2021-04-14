using System;

namespace System.Collections
{
	[__DynamicallyInvokable]
	public interface IStructuralComparable
	{
		[__DynamicallyInvokable]
		int CompareTo(object other, IComparer comparer);
	}
}
