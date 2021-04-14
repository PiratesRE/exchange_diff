using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal class EnumAttributeInfo<T, A> where T : struct where A : Attribute
	{
		public static bool TryGetValue(int enumValue, out A attributeValue)
		{
			EnumAttributeInfo<T, A>.InitializeIfNeccessary();
			return EnumAttributeInfo<T, A>.attributeInfo.TryGetValue(Names<T>.Map[enumValue], out attributeValue);
		}

		private static void InitializeIfNeccessary()
		{
			if (EnumAttributeInfo<T, A>.attributeInfo == null)
			{
				lock (EnumAttributeInfo<T, A>.staticInitLock)
				{
					if (EnumAttributeInfo<T, A>.attributeInfo == null)
					{
						Dictionary<string, A> dictionary = new Dictionary<string, A>(EnumAttributeInfo<T, A>.Count, StringComparer.Ordinal);
						Type typeFromHandle = typeof(T);
						string[] map = Names<T>.Map;
						foreach (string name in map)
						{
							FieldInfo field = typeFromHandle.GetField(name);
							object[] customAttributes = field.GetCustomAttributes(typeof(A), false);
							foreach (object obj2 in customAttributes)
							{
								dictionary.Add(field.Name, (A)((object)obj2));
							}
							EnumAttributeInfo<T, A>.attributeInfo = dictionary;
						}
					}
				}
			}
		}

		private static readonly int Count = Enum.GetNames(typeof(T)).Length;

		private static object staticInitLock = new object();

		private static Dictionary<string, A> attributeInfo;
	}
}
