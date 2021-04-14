using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	internal sealed class ReadOnlyDictionaryValueCollection<TKey, TValue> : IEnumerable<!1>, IEnumerable
	{
		public ReadOnlyDictionaryValueCollection(IReadOnlyDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			this.dictionary = dictionary;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<!1>)this).GetEnumerator();
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return new ReadOnlyDictionaryValueEnumerator<TKey, TValue>(this.dictionary);
		}

		private readonly IReadOnlyDictionary<TKey, TValue> dictionary;
	}
}
