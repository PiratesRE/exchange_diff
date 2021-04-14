using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IEqualityComparer
	{
		[__DynamicallyInvokable]
		bool Equals(object x, object y);

		[__DynamicallyInvokable]
		int GetHashCode(object obj);
	}
}
