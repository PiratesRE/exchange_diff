using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Serializable]
	internal sealed class DictionaryKeyEnumerator<TKey, TValue> : IEnumerator<!0>, IDisposable, IEnumerator
	{
		public DictionaryKeyEnumerator(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			this.dictionary = dictionary;
			this.enumeration = dictionary.GetEnumerator();
		}

		void IDisposable.Dispose()
		{
			this.enumeration.Dispose();
		}

		public bool MoveNext()
		{
			return this.enumeration.MoveNext();
		}

		object IEnumerator.Current
		{
			get
			{
				return ((IEnumerator<!0>)this).Current;
			}
		}

		public TKey Current
		{
			get
			{
				KeyValuePair<TKey, TValue> keyValuePair = this.enumeration.Current;
				return keyValuePair.Key;
			}
		}

		public void Reset()
		{
			this.enumeration = this.dictionary.GetEnumerator();
		}

		private readonly IDictionary<TKey, TValue> dictionary;

		private IEnumerator<KeyValuePair<TKey, TValue>> enumeration;
	}
}
