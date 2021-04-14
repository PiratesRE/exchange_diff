using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Serializable]
	internal sealed class DictionaryValueEnumerator<TKey, TValue> : IEnumerator<TValue>, IDisposable, IEnumerator
	{
		public DictionaryValueEnumerator(IDictionary<TKey, TValue> dictionary)
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
				return ((IEnumerator<TValue>)this).Current;
			}
		}

		public TValue Current
		{
			get
			{
				KeyValuePair<TKey, TValue> keyValuePair = this.enumeration.Current;
				return keyValuePair.Value;
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
