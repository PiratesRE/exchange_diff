using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("3c2925fe-8519-45c1-aa79-197b6718c1c1")]
	[ComImport]
	internal interface IMap<K, V> : IIterable<IKeyValuePair<K, V>>, IEnumerable<IKeyValuePair<K, V>>, IEnumerable
	{
		V Lookup(K key);

		uint Size { get; }

		bool HasKey(K key);

		IReadOnlyDictionary<K, V> GetView();

		bool Insert(K key, V value);

		void Remove(K key);

		void Clear();
	}
}
