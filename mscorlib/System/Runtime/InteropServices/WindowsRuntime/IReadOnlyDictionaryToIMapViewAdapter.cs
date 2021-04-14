using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[DebuggerDisplay("Size = {Size}")]
	internal sealed class IReadOnlyDictionaryToIMapViewAdapter
	{
		private IReadOnlyDictionaryToIMapViewAdapter()
		{
		}

		[SecurityCritical]
		internal V Lookup<K, V>(K key)
		{
			IReadOnlyDictionary<K, V> readOnlyDictionary = JitHelpers.UnsafeCast<IReadOnlyDictionary<K, V>>(this);
			V result;
			if (!readOnlyDictionary.TryGetValue(key, out result))
			{
				Exception ex = new KeyNotFoundException(Environment.GetResourceString("Arg_KeyNotFound"));
				ex.SetErrorCode(-2147483637);
				throw ex;
			}
			return result;
		}

		[SecurityCritical]
		internal uint Size<K, V>()
		{
			IReadOnlyDictionary<K, V> readOnlyDictionary = JitHelpers.UnsafeCast<IReadOnlyDictionary<K, V>>(this);
			return (uint)readOnlyDictionary.Count;
		}

		[SecurityCritical]
		internal bool HasKey<K, V>(K key)
		{
			IReadOnlyDictionary<K, V> readOnlyDictionary = JitHelpers.UnsafeCast<IReadOnlyDictionary<K, V>>(this);
			return readOnlyDictionary.ContainsKey(key);
		}

		[SecurityCritical]
		internal void Split<K, V>(out IMapView<K, V> first, out IMapView<K, V> second)
		{
			IReadOnlyDictionary<K, V> readOnlyDictionary = JitHelpers.UnsafeCast<IReadOnlyDictionary<K, V>>(this);
			if (readOnlyDictionary.Count < 2)
			{
				first = null;
				second = null;
				return;
			}
			ConstantSplittableMap<K, V> constantSplittableMap = readOnlyDictionary as ConstantSplittableMap<K, V>;
			if (constantSplittableMap == null)
			{
				constantSplittableMap = new ConstantSplittableMap<K, V>(readOnlyDictionary);
			}
			constantSplittableMap.Split(out first, out second);
		}
	}
}
