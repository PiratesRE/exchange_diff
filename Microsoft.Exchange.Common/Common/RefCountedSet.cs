using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Common
{
	public class RefCountedSet<T> : IEnumerable<T>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public void Add(T item)
		{
			if (!this.dictionary.ContainsKey(item))
			{
				this.dictionary[item] = 0;
			}
			Dictionary<T, int> dictionary;
			(dictionary = this.dictionary)[item] = dictionary[item] + 1;
		}

		public bool Remove(T item)
		{
			if (!this.dictionary.ContainsKey(item))
			{
				return false;
			}
			Dictionary<T, int> dictionary;
			(dictionary = this.dictionary)[item] = dictionary[item] - 1;
			return this.dictionary[item] == 0 && this.dictionary.Remove(item);
		}

		public void Clear()
		{
			this.dictionary.Clear();
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (KeyValuePair<T, int> kvp in this.dictionary)
			{
				KeyValuePair<T, int> keyValuePair = kvp;
				yield return keyValuePair.Key;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private Dictionary<T, int> dictionary = new Dictionary<T, int>();
	}
}
