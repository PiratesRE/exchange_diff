using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal class EnumConverter<T, S> where T : struct where S : struct
	{
		public static T Convert(S enumToConvert)
		{
			T result;
			if (!EnumConverter<T, S>.TryConvert(enumToConvert, out result))
			{
				throw new InvalidOperationException(string.Format("Enum {0} in type {1} cannot be converted to type {2}", enumToConvert, typeof(S), typeof(T)));
			}
			return result;
		}

		public static S Convert(T enumToConvert)
		{
			S result;
			if (!EnumConverter<T, S>.TryConvert(enumToConvert, out result))
			{
				throw new InvalidOperationException(string.Format("Enum {0} in type {1} cannot be converted to type {2}", enumToConvert, typeof(T), typeof(S)));
			}
			return result;
		}

		public static bool TryConvert(S enumToConvert, out T enumConverted)
		{
			EnumConverter<T, S>.InitializeIfNeccessary();
			return EnumConverter<T, S>.stMapping.TryGetValue(enumToConvert, out enumConverted);
		}

		public static bool TryConvert(T enumToConvert, out S enumConverted)
		{
			EnumConverter<T, S>.InitializeIfNeccessary();
			return EnumConverter<T, S>.tsMapping.TryGetValue(enumToConvert, out enumConverted);
		}

		private static void InitializeIfNeccessary()
		{
			if (EnumConverter<T, S>.tsMapping == null)
			{
				lock (EnumConverter<T, S>.staticInitLock)
				{
					if (EnumConverter<T, S>.tsMapping == null)
					{
						string[] names = Enum.GetNames(typeof(T));
						string[] names2 = Enum.GetNames(typeof(S));
						EnumConverter<T, S>.tsMapping = EnumConverter<T, S>.Populate<T, S>(names, names2);
						EnumConverter<T, S>.stMapping = EnumConverter<T, S>.Populate<S, T>(names2, names);
					}
				}
			}
		}

		private static Dictionary<U, V> Populate<U, V>(string[] stringNamesOfU, string[] stringNamesOfV) where U : struct where V : struct
		{
			Dictionary<U, V> dictionary = new Dictionary<U, V>(stringNamesOfU.Length);
			foreach (string value in stringNamesOfU)
			{
				V value2;
				if (EnumValidator<V>.TryParse(value, EnumParseOptions.Default, out value2))
				{
					U key = EnumValidator<U>.Parse(value, EnumParseOptions.Default);
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, value2);
					}
				}
			}
			return dictionary;
		}

		private static Dictionary<T, S> tsMapping;

		private static Dictionary<S, T> stMapping;

		private static object staticInitLock = new object();
	}
}
