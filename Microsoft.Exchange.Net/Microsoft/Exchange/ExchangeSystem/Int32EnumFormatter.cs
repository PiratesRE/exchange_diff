using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal static class Int32EnumFormatter<T>
	{
		private static Dictionary<int, string> CreateValueNameMap()
		{
			Int32EnumFormatter<T>.enumType = typeof(T);
			if (!Int32EnumFormatter<T>.enumType.IsEnum)
			{
				throw new InvalidOperationException("Type is not enum");
			}
			if (Enum.GetUnderlyingType(Int32EnumFormatter<T>.enumType) != typeof(int))
			{
				throw new InvalidOperationException("Enum underlying type is not int");
			}
			Array values = Enum.GetValues(Int32EnumFormatter<T>.enumType);
			Dictionary<int, string> dictionary = new Dictionary<int, string>(values.Length);
			foreach (object obj in values)
			{
				string name = Enum.GetName(Int32EnumFormatter<T>.enumType, obj);
				dictionary[(int)obj] = name;
			}
			return dictionary;
		}

		public static string Format(int value)
		{
			string result;
			if (Int32EnumFormatter<T>.dictionary.TryGetValue(value, out result))
			{
				return result;
			}
			return value.ToString();
		}

		private static Type enumType;

		private static Dictionary<int, string> dictionary = Int32EnumFormatter<T>.CreateValueNameMap();
	}
}
