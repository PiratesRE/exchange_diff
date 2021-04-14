using System;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	[TypeDependency("System.SZArrayHelper")]
	[__DynamicallyInvokable]
	public interface IList<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		[__DynamicallyInvokable]
		T this[int index]
		{
			[__DynamicallyInvokable]
			get;
			[__DynamicallyInvokable]
			set;
		}

		[__DynamicallyInvokable]
		int IndexOf(T item);

		[__DynamicallyInvokable]
		void Insert(int index, T item);

		[__DynamicallyInvokable]
		void RemoveAt(int index);
	}
}
