using System;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class EnumUtility
	{
		public static string GetString(EnumUtility.StringIntPair[] map, int value)
		{
			string result = string.Empty;
			for (int i = 0; i < map.Length; i++)
			{
				if (map[i].Int == value)
				{
					return map[i].String;
				}
				if (map[i].Int == 0)
				{
					result = map[i].String;
				}
			}
			return result;
		}

		public static bool TryGetInt(EnumUtility.StringIntPair[] map, string value, ref int result)
		{
			for (int i = 0; i < map.Length; i++)
			{
				if (map[i].String == null)
				{
					if (string.IsNullOrEmpty(value))
					{
						result = map[i].Int;
						return true;
					}
				}
				else if (map[i].String.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					result = map[i].Int;
					return true;
				}
			}
			return false;
		}

		public static EnumUtility.StringIntPair[] PriorityMap = new EnumUtility.StringIntPair[]
		{
			new EnumUtility.StringIntPair("normal", 0),
			new EnumUtility.StringIntPair("urgent", 1),
			new EnumUtility.StringIntPair("non-urgent", 2)
		};

		public static EnumUtility.StringIntPair[] XPriorityMap = new EnumUtility.StringIntPair[]
		{
			new EnumUtility.StringIntPair("1", 1),
			new EnumUtility.StringIntPair("2", 1),
			new EnumUtility.StringIntPair("3", 0),
			new EnumUtility.StringIntPair("5", 2),
			new EnumUtility.StringIntPair("4", 2)
		};

		public static EnumUtility.StringIntPair[] ImportanceMap = new EnumUtility.StringIntPair[]
		{
			new EnumUtility.StringIntPair("normal", 0),
			new EnumUtility.StringIntPair("high", 1),
			new EnumUtility.StringIntPair("low", 2)
		};

		public static EnumUtility.StringIntPair[] SensitivityMap = new EnumUtility.StringIntPair[]
		{
			new EnumUtility.StringIntPair(null, 0),
			new EnumUtility.StringIntPair("Personal", 1),
			new EnumUtility.StringIntPair("Private", 2),
			new EnumUtility.StringIntPair("Company-Confidential", 3)
		};

		public struct StringIntPair
		{
			public StringIntPair(string s, int i)
			{
				this.String = s;
				this.Int = i;
			}

			public string String;

			public int Int;
		}
	}
}
