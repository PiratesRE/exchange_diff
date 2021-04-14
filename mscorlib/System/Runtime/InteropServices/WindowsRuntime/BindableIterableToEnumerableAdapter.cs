using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	internal sealed class BindableIterableToEnumerableAdapter
	{
		private BindableIterableToEnumerableAdapter()
		{
		}

		[SecurityCritical]
		internal IEnumerator GetEnumerator_Stub()
		{
			IBindableIterable bindableIterable = JitHelpers.UnsafeCast<IBindableIterable>(this);
			return new IteratorToEnumeratorAdapter<object>(new BindableIterableToEnumerableAdapter.NonGenericToGenericIterator(bindableIterable.First()));
		}

		private sealed class NonGenericToGenericIterator : IIterator<object>
		{
			public NonGenericToGenericIterator(IBindableIterator iterator)
			{
				this.iterator = iterator;
			}

			public object Current
			{
				get
				{
					return this.iterator.Current;
				}
			}

			public bool HasCurrent
			{
				get
				{
					return this.iterator.HasCurrent;
				}
			}

			public bool MoveNext()
			{
				return this.iterator.MoveNext();
			}

			public int GetMany(object[] items)
			{
				throw new NotSupportedException();
			}

			private IBindableIterator iterator;
		}
	}
}
