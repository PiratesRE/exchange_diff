using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Common
{
	internal class AutoReadThroughCache<TKey, TValue>
	{
		public AutoReadThroughCache(Func<TKey, TValue> retrieveFunction)
		{
			if (retrieveFunction == null)
			{
				throw new ArgumentNullException();
			}
			this.retrieveFunction = retrieveFunction;
		}

		public TValue Get(TKey key)
		{
			Dictionary<TKey, TValue> dictionary = this.cache;
			TValue tvalue;
			if (!dictionary.TryGetValue(key, out tvalue))
			{
				tvalue = this.retrieveFunction(key);
				this.cache = new Dictionary<TKey, TValue>(dictionary)
				{
					{
						key,
						tvalue
					}
				};
			}
			return tvalue;
		}

		public void ForEach(Action<TKey, TValue> operation)
		{
			Dictionary<TKey, TValue> dictionary = this.cache;
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
			{
				operation(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private readonly Func<TKey, TValue> retrieveFunction;

		private Dictionary<TKey, TValue> cache = new Dictionary<TKey, TValue>();
	}
}
