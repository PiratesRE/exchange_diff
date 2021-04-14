using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class RuleDataConverter
	{
		public static string[] ToStringArray(this Array array)
		{
			if (array.IsNullOrEmpty())
			{
				return null;
			}
			string[] array2 = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array.GetValue(i).ToString();
			}
			return array2;
		}

		public static string[] ToStringArray(this MultiValuedProperty<string> stringProperty)
		{
			if (MultiValuedPropertyBase.IsNullOrEmpty(stringProperty))
			{
				return null;
			}
			return stringProperty.ToArray();
		}

		public static string[] ToStringArray<T>(this MultiValuedProperty<T> multiValuedProperty)
		{
			if (multiValuedProperty == null)
			{
				return new string[0];
			}
			string[] array = new string[multiValuedProperty.Count];
			for (int i = 0; i < multiValuedProperty.Count; i++)
			{
				string[] array2 = array;
				int num = i;
				T t = multiValuedProperty[i];
				array2[num] = t.ToString();
			}
			return array;
		}

		public static int ToKB(this ByteQuantifiedSize? size)
		{
			if (size != null)
			{
				return (int)size.Value.ToKB();
			}
			return 0;
		}

		public static ByteQuantifiedSize? ToByteSize(this int size)
		{
			ByteQuantifiedSize? result = new ByteQuantifiedSize?(default(ByteQuantifiedSize));
			if (size == 0)
			{
				result = null;
			}
			else
			{
				result = new ByteQuantifiedSize?(ByteQuantifiedSize.Parse(size.ToString() + ByteQuantifiedSize.Quantifier.KB.ToString()));
			}
			return result;
		}

		public static string ToCommaSeperatedString(this string[] stringArray)
		{
			if (stringArray.IsNullOrEmpty())
			{
				return null;
			}
			return string.Join(",", stringArray);
		}
	}
}
