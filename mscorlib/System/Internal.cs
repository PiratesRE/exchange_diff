using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using System.StubHelpers;

namespace System
{
	internal static class Internal
	{
		private static void CommonlyUsedGenericInstantiations()
		{
			Array.Sort<double>(null);
			Array.Sort<int>(null);
			Array.Sort<IntPtr>(null);
			new ArraySegment<byte>(new byte[1], 0, 0);
			new Dictionary<char, object>();
			new Dictionary<Guid, byte>();
			new Dictionary<Guid, object>();
			new Dictionary<Guid, Guid>();
			new Dictionary<short, IntPtr>();
			new Dictionary<int, byte>();
			new Dictionary<int, int>();
			new Dictionary<int, object>();
			new Dictionary<IntPtr, bool>();
			new Dictionary<IntPtr, short>();
			new Dictionary<object, bool>();
			new Dictionary<object, char>();
			new Dictionary<object, Guid>();
			new Dictionary<object, int>();
			new Dictionary<object, long>();
			new Dictionary<uint, WeakReference>();
			new Dictionary<object, uint>();
			new Dictionary<uint, object>();
			new Dictionary<long, object>();
			new Dictionary<MemberTypes, object>();
			new EnumEqualityComparer<MemberTypes>();
			new Dictionary<object, KeyValuePair<object, object>>();
			new Dictionary<KeyValuePair<object, object>, object>();
			Internal.NullableHelper<bool>();
			Internal.NullableHelper<byte>();
			Internal.NullableHelper<char>();
			Internal.NullableHelper<DateTime>();
			Internal.NullableHelper<decimal>();
			Internal.NullableHelper<double>();
			Internal.NullableHelper<Guid>();
			Internal.NullableHelper<short>();
			Internal.NullableHelper<int>();
			Internal.NullableHelper<long>();
			Internal.NullableHelper<float>();
			Internal.NullableHelper<TimeSpan>();
			Internal.NullableHelper<DateTimeOffset>();
			new List<bool>();
			new List<byte>();
			new List<char>();
			new List<DateTime>();
			new List<decimal>();
			new List<double>();
			new List<Guid>();
			new List<short>();
			new List<int>();
			new List<long>();
			new List<TimeSpan>();
			new List<sbyte>();
			new List<float>();
			new List<ushort>();
			new List<uint>();
			new List<ulong>();
			new List<IntPtr>();
			new List<KeyValuePair<object, object>>();
			new List<GCHandle>();
			new List<DateTimeOffset>();
			new KeyValuePair<char, ushort>('\0', 0);
			new KeyValuePair<ushort, double>(0, double.MinValue);
			new KeyValuePair<object, int>(string.Empty, int.MinValue);
			new KeyValuePair<int, int>(int.MinValue, int.MinValue);
			Internal.SZArrayHelper<bool>(null);
			Internal.SZArrayHelper<byte>(null);
			Internal.SZArrayHelper<DateTime>(null);
			Internal.SZArrayHelper<decimal>(null);
			Internal.SZArrayHelper<double>(null);
			Internal.SZArrayHelper<Guid>(null);
			Internal.SZArrayHelper<short>(null);
			Internal.SZArrayHelper<int>(null);
			Internal.SZArrayHelper<long>(null);
			Internal.SZArrayHelper<TimeSpan>(null);
			Internal.SZArrayHelper<sbyte>(null);
			Internal.SZArrayHelper<float>(null);
			Internal.SZArrayHelper<ushort>(null);
			Internal.SZArrayHelper<uint>(null);
			Internal.SZArrayHelper<ulong>(null);
			Internal.SZArrayHelper<DateTimeOffset>(null);
			Internal.SZArrayHelper<CustomAttributeTypedArgument>(null);
			Internal.SZArrayHelper<CustomAttributeNamedArgument>(null);
		}

		private static T NullableHelper<T>() where T : struct
		{
			Nullable.Compare<T>(null, null);
			Nullable.Equals<T>(null, null);
			return ((T?)null).GetValueOrDefault();
		}

		private static void SZArrayHelper<T>(SZArrayHelper oSZArrayHelper)
		{
			int count = oSZArrayHelper.get_Count<T>();
			T t = oSZArrayHelper.get_Item<T>(0);
			oSZArrayHelper.GetEnumerator<T>();
		}

		[SecurityCritical]
		private static void CommonlyUsedWinRTRedirectedInterfaceStubs()
		{
			Internal.WinRT_IEnumerable<byte>(null, null, null);
			Internal.WinRT_IEnumerable<char>(null, null, null);
			Internal.WinRT_IEnumerable<short>(null, null, null);
			Internal.WinRT_IEnumerable<ushort>(null, null, null);
			Internal.WinRT_IEnumerable<int>(null, null, null);
			Internal.WinRT_IEnumerable<uint>(null, null, null);
			Internal.WinRT_IEnumerable<long>(null, null, null);
			Internal.WinRT_IEnumerable<ulong>(null, null, null);
			Internal.WinRT_IEnumerable<float>(null, null, null);
			Internal.WinRT_IEnumerable<double>(null, null, null);
			Internal.WinRT_IEnumerable<string>(null, null, null);
			typeof(IIterable<string>).ToString();
			typeof(IIterator<string>).ToString();
			Internal.WinRT_IEnumerable<object>(null, null, null);
			typeof(IIterable<object>).ToString();
			typeof(IIterator<object>).ToString();
			Internal.WinRT_IList<int>(null, null, null, null);
			Internal.WinRT_IList<string>(null, null, null, null);
			typeof(IVector<string>).ToString();
			Internal.WinRT_IList<object>(null, null, null, null);
			typeof(IVector<object>).ToString();
			Internal.WinRT_IReadOnlyList<int>(null, null, null);
			Internal.WinRT_IReadOnlyList<string>(null, null, null);
			typeof(IVectorView<string>).ToString();
			Internal.WinRT_IReadOnlyList<object>(null, null, null);
			typeof(IVectorView<object>).ToString();
			Internal.WinRT_IDictionary<string, int>(null, null, null, null);
			typeof(IMap<string, int>).ToString();
			Internal.WinRT_IDictionary<string, string>(null, null, null, null);
			typeof(IMap<string, string>).ToString();
			Internal.WinRT_IDictionary<string, object>(null, null, null, null);
			typeof(IMap<string, object>).ToString();
			Internal.WinRT_IDictionary<object, object>(null, null, null, null);
			typeof(IMap<object, object>).ToString();
			Internal.WinRT_IReadOnlyDictionary<string, int>(null, null, null, null);
			typeof(IMapView<string, int>).ToString();
			Internal.WinRT_IReadOnlyDictionary<string, string>(null, null, null, null);
			typeof(IMapView<string, string>).ToString();
			Internal.WinRT_IReadOnlyDictionary<string, object>(null, null, null, null);
			typeof(IMapView<string, object>).ToString();
			Internal.WinRT_IReadOnlyDictionary<object, object>(null, null, null, null);
			typeof(IMapView<object, object>).ToString();
			Internal.WinRT_Nullable<bool>();
			Internal.WinRT_Nullable<byte>();
			Internal.WinRT_Nullable<int>();
			Internal.WinRT_Nullable<uint>();
			Internal.WinRT_Nullable<long>();
			Internal.WinRT_Nullable<ulong>();
			Internal.WinRT_Nullable<float>();
			Internal.WinRT_Nullable<double>();
		}

		[SecurityCritical]
		private static void WinRT_IEnumerable<T>(IterableToEnumerableAdapter iterableToEnumerableAdapter, EnumerableToIterableAdapter enumerableToIterableAdapter, IIterable<T> iterable)
		{
			iterableToEnumerableAdapter.GetEnumerator_Stub<T>();
			enumerableToIterableAdapter.First_Stub<T>();
		}

		[SecurityCritical]
		private static void WinRT_IList<T>(VectorToListAdapter vectorToListAdapter, VectorToCollectionAdapter vectorToCollectionAdapter, ListToVectorAdapter listToVectorAdapter, IVector<T> vector)
		{
			Internal.WinRT_IEnumerable<T>(null, null, null);
			vectorToListAdapter.Indexer_Get<T>(0);
			vectorToListAdapter.Indexer_Set<T>(0, default(T));
			vectorToListAdapter.Insert<T>(0, default(T));
			vectorToListAdapter.RemoveAt<T>(0);
			vectorToCollectionAdapter.Count<T>();
			vectorToCollectionAdapter.Add<T>(default(T));
			vectorToCollectionAdapter.Clear<T>();
			listToVectorAdapter.GetAt<T>(0U);
			listToVectorAdapter.Size<T>();
			listToVectorAdapter.SetAt<T>(0U, default(T));
			listToVectorAdapter.InsertAt<T>(0U, default(T));
			listToVectorAdapter.RemoveAt<T>(0U);
			listToVectorAdapter.Append<T>(default(T));
			listToVectorAdapter.RemoveAtEnd<T>();
			listToVectorAdapter.Clear<T>();
		}

		[SecurityCritical]
		private static void WinRT_IReadOnlyCollection<T>(VectorViewToReadOnlyCollectionAdapter vectorViewToReadOnlyCollectionAdapter)
		{
			Internal.WinRT_IEnumerable<T>(null, null, null);
			vectorViewToReadOnlyCollectionAdapter.Count<T>();
		}

		[SecurityCritical]
		private static void WinRT_IReadOnlyList<T>(IVectorViewToIReadOnlyListAdapter vectorToListAdapter, IReadOnlyListToIVectorViewAdapter listToVectorAdapter, IVectorView<T> vectorView)
		{
			Internal.WinRT_IEnumerable<T>(null, null, null);
			Internal.WinRT_IReadOnlyCollection<T>(null);
			vectorToListAdapter.Indexer_Get<T>(0);
			listToVectorAdapter.GetAt<T>(0U);
			listToVectorAdapter.Size<T>();
		}

		[SecurityCritical]
		private static void WinRT_IDictionary<K, V>(MapToDictionaryAdapter mapToDictionaryAdapter, MapToCollectionAdapter mapToCollectionAdapter, DictionaryToMapAdapter dictionaryToMapAdapter, IMap<K, V> map)
		{
			Internal.WinRT_IEnumerable<KeyValuePair<K, V>>(null, null, null);
			mapToDictionaryAdapter.Indexer_Get<K, V>(default(K));
			mapToDictionaryAdapter.Indexer_Set<K, V>(default(K), default(V));
			mapToDictionaryAdapter.ContainsKey<K, V>(default(K));
			mapToDictionaryAdapter.Add<K, V>(default(K), default(V));
			mapToDictionaryAdapter.Remove<K, V>(default(K));
			V v;
			mapToDictionaryAdapter.TryGetValue<K, V>(default(K), out v);
			mapToCollectionAdapter.Count<K, V>();
			mapToCollectionAdapter.Add<K, V>(new KeyValuePair<K, V>(default(K), default(V)));
			mapToCollectionAdapter.Clear<K, V>();
			dictionaryToMapAdapter.Lookup<K, V>(default(K));
			dictionaryToMapAdapter.Size<K, V>();
			dictionaryToMapAdapter.HasKey<K, V>(default(K));
			dictionaryToMapAdapter.Insert<K, V>(default(K), default(V));
			dictionaryToMapAdapter.Remove<K, V>(default(K));
			dictionaryToMapAdapter.Clear<K, V>();
		}

		[SecurityCritical]
		private static void WinRT_IReadOnlyDictionary<K, V>(IMapViewToIReadOnlyDictionaryAdapter mapToDictionaryAdapter, IReadOnlyDictionaryToIMapViewAdapter dictionaryToMapAdapter, IMapView<K, V> mapView, MapViewToReadOnlyCollectionAdapter mapViewToReadOnlyCollectionAdapter)
		{
			Internal.WinRT_IEnumerable<KeyValuePair<K, V>>(null, null, null);
			Internal.WinRT_IReadOnlyCollection<KeyValuePair<K, V>>(null);
			mapToDictionaryAdapter.Indexer_Get<K, V>(default(K));
			mapToDictionaryAdapter.ContainsKey<K, V>(default(K));
			V v;
			mapToDictionaryAdapter.TryGetValue<K, V>(default(K), out v);
			mapViewToReadOnlyCollectionAdapter.Count<K, V>();
			dictionaryToMapAdapter.Lookup<K, V>(default(K));
			dictionaryToMapAdapter.Size<K, V>();
			dictionaryToMapAdapter.HasKey<K, V>(default(K));
		}

		[SecurityCritical]
		private static void WinRT_Nullable<T>() where T : struct
		{
			T? t = null;
			NullableMarshaler.ConvertToNative<T>(ref t);
			NullableMarshaler.ConvertToManagedRetVoid<T>(IntPtr.Zero, ref t);
		}
	}
}
