using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public static class EnumUtilities
	{
		public static T Parse<T>(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return default(T);
			}
			return EnumUtilities.enumTypeMap.Member.Parse<T>(value);
		}

		public static string ToString<T>(T value)
		{
			return EnumUtilities.enumTypeMap.Member.ToString<T>(value);
		}

		public static bool IsDefined<T>(T value)
		{
			return EnumUtilities.enumTypeMap.Member.IsDefined<T>(value);
		}

		public static string[] ToStringArray<T>(T[] enums)
		{
			string[] array = null;
			if (enums != null)
			{
				array = new string[enums.Length];
				for (int i = 0; i < enums.Length; i++)
				{
					array[i] = EnumUtilities.ToString<T>(enums[i]);
				}
			}
			return array;
		}

		public static T[] ParseStringArray<T>(string[] strings)
		{
			T[] array = null;
			if (strings != null)
			{
				array = new T[strings.Length];
				for (int i = 0; i < strings.Length; i++)
				{
					array[i] = EnumUtilities.Parse<T>(strings[i]);
				}
			}
			return array;
		}

		private static LazyMember<EnumTypeMap> enumTypeMap = new LazyMember<EnumTypeMap>(() => new EnumTypeMap());
	}
}
