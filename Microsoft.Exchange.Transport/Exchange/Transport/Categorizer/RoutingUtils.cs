using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal static class RoutingUtils
	{
		public static int CompareNames(string nameA, string nameB)
		{
			return string.Compare(nameA, nameB, StringComparison.OrdinalIgnoreCase);
		}

		public static bool NullMatch(object reference1, object reference2)
		{
			return reference1 == null == (reference2 == null);
		}

		public static bool MatchStrings(string s1, string s2)
		{
			return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
		}

		public static bool MatchNextHopServers(INextHopServer server1, INextHopServer server2)
		{
			if (server1.IsIPAddress != server2.IsIPAddress)
			{
				return false;
			}
			if (!server1.IsIPAddress)
			{
				return RoutingUtils.MatchStrings(server1.Fqdn, server2.Fqdn);
			}
			return server1.Address.Equals(server2.Address);
		}

		public static bool MatchLists<T>(List<T> l1, List<T> l2, Func<T, T, bool> matchValues)
		{
			return RoutingUtils.MatchLists<T>(l1, l2, matchValues, false);
		}

		public static bool MatchLists<T>(List<T> l1, List<T> l2, Func<T, T, bool> matchValues, bool ordered)
		{
			RoutingUtils.<>c__DisplayClass1<T> CS$<>8__locals1 = new RoutingUtils.<>c__DisplayClass1<T>();
			CS$<>8__locals1.l1 = l1;
			CS$<>8__locals1.matchValues = matchValues;
			if (!RoutingUtils.NullMatch(CS$<>8__locals1.l1, l2) || (CS$<>8__locals1.l1 != null && CS$<>8__locals1.l1.Count != l2.Count))
			{
				return false;
			}
			if (CS$<>8__locals1.l1 == null)
			{
				return true;
			}
			int i;
			for (i = 0; i < CS$<>8__locals1.l1.Count; i++)
			{
				if (!CS$<>8__locals1.matchValues(CS$<>8__locals1.l1[i], l2[i]))
				{
					if (!ordered)
					{
						if (l2.Exists((T element2) => CS$<>8__locals1.matchValues(CS$<>8__locals1.l1[i], element2)))
						{
							goto IL_A9;
						}
					}
					return false;
				}
				IL_A9:;
			}
			return true;
		}

		public static bool MatchOrderedLists<T>(List<T> l1, List<T> l2, Func<T, T, bool> matchValues)
		{
			return RoutingUtils.MatchLists<T>(l1, l2, matchValues, true);
		}

		public static bool MatchLists<T>(ListLoadBalancer<T> l1, ListLoadBalancer<T> l2, Func<T, T, bool> matchValues)
		{
			return RoutingUtils.NullMatch(l1, l2) && (l1 == null || RoutingUtils.MatchLists<T>(l1.NonLoadBalancedList, l2.NonLoadBalancedList, matchValues));
		}

		public static bool MatchDictionaries<TKey, TValue>(IDictionary<TKey, TValue> d1, IDictionary<TKey, TValue> d2, Func<TValue, TValue, bool> matchValues)
		{
			if (!RoutingUtils.NullMatch(d1, d2) || (d1 != null && d1.Count != d2.Count))
			{
				return false;
			}
			if (d1 == null)
			{
				return true;
			}
			foreach (KeyValuePair<TKey, TValue> keyValuePair in d1)
			{
				TValue arg;
				if (!d2.TryGetValue(keyValuePair.Key, out arg) || !matchValues(keyValuePair.Value, arg))
				{
					return false;
				}
			}
			return true;
		}

		public static bool MatchRouteDictionaries<TKey>(IDictionary<TKey, RouteInfo> d1, IDictionary<TKey, RouteInfo> d2, NextHopMatch nextHopMatch)
		{
			return RoutingUtils.MatchDictionaries<TKey, RouteInfo>(d1, d2, (RouteInfo routeInfo1, RouteInfo routeInfo2) => routeInfo1.Match(routeInfo2, nextHopMatch));
		}

		public static bool MatchRouteDictionaries<TKey>(IDictionary<TKey, RouteInfo> d1, IDictionary<TKey, RouteInfo> d2)
		{
			return RoutingUtils.MatchRouteDictionaries<TKey>(d1, d2, NextHopMatch.Full);
		}

		public static bool IsSmtpAddressType(string addressType)
		{
			return "smtp".Equals(addressType, StringComparison.OrdinalIgnoreCase);
		}

		public static bool TryConvertToRoutingAddress(SmtpAddress? smtpAddress, out RoutingAddress routingAddress)
		{
			routingAddress = RoutingAddress.Empty;
			if (smtpAddress != null && smtpAddress.Value.IsValidAddress && smtpAddress.Value != SmtpAddress.NullReversePath)
			{
				routingAddress = new RoutingAddress(smtpAddress.ToString());
				return true;
			}
			return false;
		}

		public static bool IsNullOrEmpty<T>(IList<T> list)
		{
			return list == null || list.Count == 0;
		}

		public static void AddItemToLazyList<ItemT>(ItemT item, ref List<ItemT> list)
		{
			if (list == null)
			{
				list = new List<ItemT>();
			}
			list.Add(item);
		}

		public static void AddItemToLazyList<ItemT>(ItemT item, bool randomLoadBalancingOffsetEnabled, ref ListLoadBalancer<ItemT> list)
		{
			if (list == null)
			{
				list = new ListLoadBalancer<ItemT>(randomLoadBalancingOffsetEnabled);
			}
			list.AddItem(item);
		}

		public static int GetRandomNumber(int maxValue)
		{
			int result;
			lock (RoutingUtils.random)
			{
				result = RoutingUtils.random.Next(maxValue);
			}
			return result;
		}

		public static void ShuffleList<T>(IList<T> list)
		{
			int i = list.Count;
			int randomNumber = RoutingUtils.GetRandomNumber(int.MaxValue);
			Random random = new Random(randomNumber);
			while (i > 1)
			{
				int index = random.Next(i);
				i--;
				T value = list[i];
				list[i] = list[index];
				list[index] = value;
			}
		}

		public static IEnumerable<T> RandomShuffleEnumerate<T>(IList<T> list)
		{
			RoutingUtils.ThrowIfNullOrEmpty<T>(list, "list");
			foreach (int index in RoutingUtils.ShuffleIndices(list.Count))
			{
				yield return list[index];
			}
			yield break;
		}

		public static IEnumerable<T> RandomShiftEnumerate<T>(IList<T> list)
		{
			return RoutingUtils.RandomShiftEnumerate<T>(list, 0);
		}

		public static IEnumerable<T> RandomShiftEnumerate<T>(IList<T> list, int offset)
		{
			int randomNumber = RoutingUtils.GetRandomNumber(list.Count - offset);
			return RoutingUtils.ShiftedEnumerate<T>(list, offset, randomNumber);
		}

		public static IEnumerable<T> ShiftedEnumerate<T>(IList<T> list, int shift)
		{
			return RoutingUtils.ShiftedEnumerate<T>(list, 0, shift);
		}

		public static IEnumerable<T> ShiftedEnumerate<T>(IList<T> list, int offset, int shift)
		{
			if (offset > list.Count || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", offset, "Offset cannot be negative or greater than the number of elements in the list");
			}
			if (shift < 0)
			{
				throw new ArgumentOutOfRangeException("shift", shift, "Shift cannot be negative");
			}
			int enumCount = list.Count - offset;
			for (int i = 0; i < enumCount; i++)
			{
				yield return list[(i + shift) % enumCount + offset];
			}
			yield break;
		}

		public static void ThrowIfNull(object value, string parameterName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		public static void ThrowIfNullOrEmpty(string value, string parameterName)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		public static void ThrowIfNullOrEmpty<T>(ICollection<T> collection, string parameterName)
		{
			if (collection == null || collection.Count == 0)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		public static void ThrowIfNullOrEmpty<T>(ListLoadBalancer<T> collection, string parameterName)
		{
			if (collection == null || collection.Count == 0)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		public static void ThrowIfEmpty(ADObjectId id, string parameterName)
		{
			if (string.IsNullOrEmpty(id.DistinguishedName))
			{
				throw new ArgumentException("Argument " + parameterName + " must have a non-empty DN", parameterName);
			}
			if (Guid.Empty.Equals(id.ObjectGuid))
			{
				throw new ArgumentException("Argument " + parameterName + " must have a non-empty ObjectGuid", parameterName);
			}
		}

		public static void ThrowIfNullOrEmptyObjectGuid(ADObjectId id, string parameterName)
		{
			RoutingUtils.ThrowIfNull(id, parameterName);
			if (Guid.Empty.Equals(id.ObjectGuid))
			{
				throw new ArgumentException("Argument " + parameterName + " must have a non-empty ObjectGuid", parameterName);
			}
		}

		public static void ThrowIfNullOrEmpty(ADObjectId id, string parameterName)
		{
			RoutingUtils.ThrowIfNull(id, parameterName);
			RoutingUtils.ThrowIfEmpty(id, parameterName);
		}

		public static void ThrowIfMissingDependency(object dependency, string dependencyName)
		{
			if (dependency == null)
			{
				throw new InvalidOperationException("Attempted to use missing external component: " + dependencyName);
			}
		}

		public static void LogErrorWhenAddToDictionaryFails<TKey, TValue>(TKey keyAdded, TValue valueAdded, string message)
		{
			ArgumentValidator.ThrowIfNull("keyAdded", keyAdded);
			RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingDictionaryInsertFailure, keyAdded.ToString(), new object[]
			{
				keyAdded,
				message
			});
		}

		private static IEnumerable<int> ShuffleIndices(int n)
		{
			if (n <= 0)
			{
				throw new ArgumentOutOfRangeException("n", n, "Count of indices should only be positive");
			}
			List<int> indices = Enumerable.Range(0, n).ToList<int>();
			int seed = RoutingUtils.GetRandomNumber(int.MaxValue);
			Random localRandom = new Random(seed);
			while (n > 0)
			{
				int i = localRandom.Next(n);
				yield return indices[i];
				n--;
				indices[i] = indices[n];
			}
			yield break;
		}

		private static readonly Random random = new Random((int)DateTime.UtcNow.Ticks);
	}
}
