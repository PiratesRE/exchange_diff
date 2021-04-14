using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.MessagingPolicies.AttachFilter
{
	internal static class Enum<EnumType>
	{
		private static string[] InitNames()
		{
			Type typeFromHandle = typeof(EnumType);
			FieldInfo[] fields = typeFromHandle.GetFields(BindingFlags.Static | BindingFlags.Public);
			List<string> list = new List<string>();
			foreach (FieldInfo fieldInfo in fields)
			{
				list.Add(fieldInfo.Name);
				Enum<EnumType>.innerDictionary.Add(fieldInfo.Name, (EnumType)((object)fieldInfo.GetRawConstantValue()));
			}
			return list.ToArray();
		}

		public static string ToString(int enumValue)
		{
			return Enum<EnumType>.names[enumValue];
		}

		public static EnumType Parse(string enumString)
		{
			return Enum<EnumType>.innerDictionary[enumString];
		}

		private static Dictionary<string, EnumType> innerDictionary = new Dictionary<string, EnumType>();

		private static string[] names = Enum<EnumType>.InitNames();
	}
}
