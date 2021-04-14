using System;

namespace System.Collections
{
	[__DynamicallyInvokable]
	public interface IStructuralEquatable
	{
		[__DynamicallyInvokable]
		bool Equals(object other, IEqualityComparer comparer);

		[__DynamicallyInvokable]
		int GetHashCode(IEqualityComparer comparer);
	}
}
