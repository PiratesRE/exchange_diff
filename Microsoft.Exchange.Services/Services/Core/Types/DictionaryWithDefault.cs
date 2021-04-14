using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class DictionaryWithDefault<TKey, TValue>
	{
		public DictionaryWithDefault(TValue defaultValue)
		{
			this.specificValues = new Dictionary<TKey, TValue>();
			this.defaultValue = defaultValue;
		}

		public TValue this[TKey key]
		{
			get
			{
				TValue result = default(TValue);
				if (this.specificValues.TryGetValue(key, out result))
				{
					return result;
				}
				return this.defaultValue;
			}
			set
			{
				if (!value.Equals(this.defaultValue))
				{
					this.specificValues[key] = value;
					return;
				}
				if (this.specificValues.ContainsKey(key))
				{
					this.specificValues.Remove(key);
				}
			}
		}

		public TValue GetValue(TKey key)
		{
			return this[key];
		}

		public void Add(TKey key, TValue value)
		{
			if (this.specificValues.ContainsKey(key))
			{
				throw new ArgumentException();
			}
			this[key] = value;
		}

		public TValue DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		private Dictionary<TKey, TValue> specificValues;

		private TValue defaultValue;
	}
}
