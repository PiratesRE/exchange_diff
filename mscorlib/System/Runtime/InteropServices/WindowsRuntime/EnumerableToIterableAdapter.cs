using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	internal sealed class EnumerableToIterableAdapter
	{
		private EnumerableToIterableAdapter()
		{
		}

		[SecurityCritical]
		internal IIterator<T> First_Stub<T>()
		{
			IEnumerable<T> enumerable = JitHelpers.UnsafeCast<IEnumerable<T>>(this);
			return new EnumeratorToIteratorAdapter<T>(enumerable.GetEnumerator());
		}
	}
}
