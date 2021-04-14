using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Exchange.Services.Core
{
	internal class EnumValidator
	{
		[Conditional("DEBUG")]
		public static void AssertValid<ENUM_TYPE>(ENUM_TYPE valueToCheck)
		{
		}

		public static bool IsValidEnum<ENUM_TYPE>(ENUM_TYPE valueToCheck)
		{
			return EnumValidator.IsValidEnumIntValue<ENUM_TYPE>(EnumValidator.ToUInt64(valueToCheck));
		}

		public static bool IsValidEnumIntValue<ENUM_TYPE>(ulong valueToCheck)
		{
			EnumValidator.EnumStats enumStats = EnumValidator.GetEnumStats(typeof(ENUM_TYPE));
			if (enumStats.IsFlags)
			{
				return valueToCheck >= enumStats.LowVal && (valueToCheck & ~enumStats.AllFlags) == 0UL;
			}
			return valueToCheck >= enumStats.LowVal && valueToCheck <= enumStats.HighVal && enumStats.Values.ContainsKey(valueToCheck);
		}

		private static EnumValidator.EnumStats CreateEnumStats(Type enumType)
		{
			Array values = Enum.GetValues(enumType);
			ulong[] array = new ulong[values.Length];
			ulong num = ulong.MaxValue;
			ulong num2 = 0UL;
			Attribute customAttribute = enumType.GetTypeInfo().GetCustomAttribute(typeof(FlagsAttribute));
			for (int i = 0; i < values.Length; i++)
			{
				array[i] = EnumValidator.ToUInt64(values.GetValue(i));
				num = Math.Min(num, array[i]);
				num2 = Math.Max(num2, array[i]);
			}
			return new EnumValidator.EnumStats(num2, num, customAttribute != null, array);
		}

		private static EnumValidator.EnumStats GetEnumStats(Type enumType)
		{
			EnumValidator.EnumStats enumStats;
			if (!EnumValidator.enumStats.TryGetValue(enumType, out enumStats))
			{
				lock (EnumValidator.enumStatsLock)
				{
					if (!EnumValidator.enumStats.TryGetValue(enumType, out enumStats))
					{
						Dictionary<Type, EnumValidator.EnumStats> dictionary = new Dictionary<Type, EnumValidator.EnumStats>(EnumValidator.enumStats);
						enumStats = EnumValidator.CreateEnumStats(enumType);
						dictionary.Add(enumType, enumStats);
						EnumValidator.enumStats = dictionary;
					}
				}
			}
			return enumStats;
		}

		public static ulong ToUInt64(object value)
		{
			switch (Convert.GetTypeCode(value))
			{
			case TypeCode.SByte:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
				return (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);
			case TypeCode.Byte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
			default:
				throw new InvalidOperationException();
			}
		}

		private static Dictionary<Type, EnumValidator.EnumStats> enumStats = new Dictionary<Type, EnumValidator.EnumStats>();

		private static readonly object enumStatsLock = new object();

		private class EnumStats
		{
			public EnumStats(ulong highVal, ulong lowVal, bool isFlags, params ulong[] values)
			{
				this.HighVal = highVal;
				this.LowVal = lowVal;
				this.IsFlags = isFlags;
				this.Values = new Dictionary<ulong, ulong>();
				foreach (ulong num in values)
				{
					this.AllFlags |= num;
					if (!this.Values.ContainsKey(num))
					{
						this.Values.Add(num, num);
					}
				}
			}

			public readonly ulong HighVal;

			public readonly ulong LowVal;

			public readonly bool IsFlags;

			public readonly ulong AllFlags;

			public readonly Dictionary<ulong, ulong> Values;
		}
	}
}
