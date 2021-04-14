using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface ICollection : IEnumerable
	{
		[__DynamicallyInvokable]
		void CopyTo(Array array, int index);

		[__DynamicallyInvokable]
		int Count { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		object SyncRoot { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		bool IsSynchronized { [__DynamicallyInvokable] get; }
	}
}
