using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Collections
{
	[ComVisible(false)]
	internal class TypeDictionary<V>
	{
		internal TypeDictionary(IList<V> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			this.map = new Dictionary<Type, V>(list.Count);
			foreach (V value in list)
			{
				Type type = value.GetType();
				if (this.map.ContainsKey(type))
				{
					throw new InvalidOperationException("Elements in list with duplicated types are not supported by TypeDictionary.");
				}
				this.map[type] = value;
			}
		}

		internal T Lookup<T>() where T : V
		{
			V v;
			if (this.map.TryGetValue(typeof(T), out v))
			{
				return (T)((object)v);
			}
			return default(T);
		}

		private Dictionary<Type, V> map;
	}
}
