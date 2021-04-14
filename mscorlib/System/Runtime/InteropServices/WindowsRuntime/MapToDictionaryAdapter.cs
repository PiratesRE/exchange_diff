using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	internal sealed class MapToDictionaryAdapter
	{
		private MapToDictionaryAdapter()
		{
		}

		[SecurityCritical]
		internal V Indexer_Get<K, V>(K key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			IMap<K, V> this2 = JitHelpers.UnsafeCast<IMap<K, V>>(this);
			return MapToDictionaryAdapter.Lookup<K, V>(this2, key);
		}

		[SecurityCritical]
		internal void Indexer_Set<K, V>(K key, V value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			IMap<K, V> this2 = JitHelpers.UnsafeCast<IMap<K, V>>(this);
			MapToDictionaryAdapter.Insert<K, V>(this2, key, value);
		}

		[SecurityCritical]
		internal ICollection<K> Keys<K, V>()
		{
			IMap<K, V> map = JitHelpers.UnsafeCast<IMap<K, V>>(this);
			IDictionary<K, V> dictionary = (IDictionary<K, V>)map;
			return new DictionaryKeyCollection<K, V>(dictionary);
		}

		[SecurityCritical]
		internal ICollection<V> Values<K, V>()
		{
			IMap<K, V> map = JitHelpers.UnsafeCast<IMap<K, V>>(this);
			IDictionary<K, V> dictionary = (IDictionary<K, V>)map;
			return new DictionaryValueCollection<K, V>(dictionary);
		}

		[SecurityCritical]
		internal bool ContainsKey<K, V>(K key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			IMap<K, V> map = JitHelpers.UnsafeCast<IMap<K, V>>(this);
			return map.HasKey(key);
		}

		[SecurityCritical]
		internal void Add<K, V>(K key, V value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this.ContainsKey<K, V>(key))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_AddingDuplicate"));
			}
			IMap<K, V> this2 = JitHelpers.UnsafeCast<IMap<K, V>>(this);
			MapToDictionaryAdapter.Insert<K, V>(this2, key, value);
		}

		[SecurityCritical]
		internal bool Remove<K, V>(K key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			IMap<K, V> map = JitHelpers.UnsafeCast<IMap<K, V>>(this);
			if (!map.HasKey(key))
			{
				return false;
			}
			bool result;
			try
			{
				map.Remove(key);
				result = true;
			}
			catch (Exception ex)
			{
				if (-2147483637 != ex._HResult)
				{
					throw;
				}
				result = false;
			}
			return result;
		}

		[SecurityCritical]
		internal bool TryGetValue<K, V>(K key, out V value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			IMap<K, V> map = JitHelpers.UnsafeCast<IMap<K, V>>(this);
			if (!map.HasKey(key))
			{
				value = default(V);
				return false;
			}
			bool result;
			try
			{
				value = MapToDictionaryAdapter.Lookup<K, V>(map, key);
				result = true;
			}
			catch (KeyNotFoundException)
			{
				value = default(V);
				result = false;
			}
			return result;
		}

		private static V Lookup<K, V>(IMap<K, V> _this, K key)
		{
			V result;
			try
			{
				result = _this.Lookup(key);
			}
			catch (Exception ex)
			{
				if (-2147483637 == ex._HResult)
				{
					throw new KeyNotFoundException(Environment.GetResourceString("Arg_KeyNotFound"));
				}
				throw;
			}
			return result;
		}

		private static bool Insert<K, V>(IMap<K, V> _this, K key, V value)
		{
			return _this.Insert(key, value);
		}
	}
}
