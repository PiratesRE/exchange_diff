using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public static class Conversions
	{
		public static DateTime ConvertDoubleToDateTime(double d)
		{
			DateTime result;
			try
			{
				result = LibraryHelpers.FromOADate(d);
			}
			catch (ArgumentException)
			{
				result = ((d < 0.0) ? DateTime.MinValue : DateTime.MaxValue);
			}
			return result;
		}

		[CLSCompliant(false)]
		public static CompareOptions CompareOptionsFromLCMapFlags(uint lcmapFlags)
		{
			CompareOptions compareOptions = CompareOptions.None;
			foreach (uint num in Conversions.LcmapFlagsToCompareOptions.Keys)
			{
				if (num == (lcmapFlags & num))
				{
					compareOptions |= Conversions.LcmapFlagsToCompareOptions[num];
				}
			}
			return compareOptions;
		}

		[CLSCompliant(false)]
		public static uint LCMapFlagsFromCompareOptions(CompareOptions compareOptions)
		{
			uint num = 0U;
			foreach (CompareOptions compareOptions2 in Conversions.CompareOptionsToLcmapFlags.Keys)
			{
				if (compareOptions2 == (compareOptions & compareOptions2))
				{
					num |= Conversions.CompareOptionsToLcmapFlags[compareOptions2];
				}
			}
			return num;
		}

		private static IDictionary<TKey, TValue> InvertDictionary<TValue, TKey>(ICollection<KeyValuePair<TValue, TKey>> dict)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(dict.Count);
			foreach (KeyValuePair<TValue, TKey> keyValuePair in dict)
			{
				dictionary.Add(keyValuePair.Value, keyValuePair.Key);
			}
			return dictionary;
		}

		private static readonly IDictionary<CompareOptions, uint> CompareOptionsToLcmapFlags = new Dictionary<CompareOptions, uint>
		{
			{
				CompareOptions.IgnoreCase,
				1U
			},
			{
				CompareOptions.IgnoreKanaType,
				65536U
			},
			{
				CompareOptions.IgnoreNonSpace,
				2U
			},
			{
				CompareOptions.IgnoreSymbols,
				4U
			},
			{
				CompareOptions.IgnoreWidth,
				131072U
			},
			{
				CompareOptions.StringSort,
				4096U
			}
		};

		private static readonly IDictionary<uint, CompareOptions> LcmapFlagsToCompareOptions = Conversions.InvertDictionary<CompareOptions, uint>(Conversions.CompareOptionsToLcmapFlags);

		internal static class NativeMethods
		{
			public const uint NORM_IGNORECASE = 1U;

			public const uint NORM_IGNORENONSPACE = 2U;

			public const uint NORM_IGNORESYMBOLS = 4U;

			public const uint NORM_IGNOREKANATYPE = 65536U;

			public const uint NORM_IGNOREWIDTH = 131072U;

			public const uint SORT_STRINGSORT = 4096U;

			public const uint LCMAP_SORTKEY = 1024U;
		}
	}
}
