using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class CachedIterator<T> : IEnumerable<T>, IEnumerable
	{
		public CachedIterator(IEnumerator<T> enumerator)
		{
			if (enumerator == null)
			{
				throw new ArgumentNullException("enumerator");
			}
			this.wrappedEnumerator = enumerator;
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (T element in this.cachedResults)
			{
				yield return element;
			}
			while (this.wrappedEnumerator.MoveNext())
			{
				this.cachedResults.Add(this.wrappedEnumerator.Current);
				yield return this.wrappedEnumerator.Current;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private List<T> cachedResults = new List<T>();

		private IEnumerator<T> wrappedEnumerator;
	}
}
