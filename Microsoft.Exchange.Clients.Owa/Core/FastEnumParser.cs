using System;
using System.Collections;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class FastEnumParser : IComparer
	{
		public FastEnumParser(Type enumType, bool ignoreCase)
		{
			this.names = Enum.GetNames(enumType);
			Array values = Enum.GetValues(enumType);
			this.items = new FastEnumParser.EnumItem[this.names.Length];
			for (int i = 0; i < this.names.Length; i++)
			{
				this.items[i] = new FastEnumParser.EnumItem(this.names[i], values.GetValue(i));
			}
			if (ignoreCase)
			{
				this.stringComparer = StringComparer.OrdinalIgnoreCase;
			}
			else
			{
				this.stringComparer = StringComparer.Ordinal;
			}
			Array.Sort(this.items, this);
		}

		public FastEnumParser(Type enumType) : this(enumType, false)
		{
		}

		int IComparer.Compare(object x, object y)
		{
			FastEnumParser.EnumItem enumItem = x as FastEnumParser.EnumItem;
			FastEnumParser.EnumItem enumItem2 = y as FastEnumParser.EnumItem;
			string x2;
			if (enumItem != null)
			{
				x2 = enumItem.Name;
			}
			else
			{
				x2 = (string)x;
			}
			string y2;
			if (enumItem2 != null)
			{
				y2 = enumItem2.Name;
			}
			else
			{
				y2 = (string)y;
			}
			return this.stringComparer.Compare(x2, y2);
		}

		public string GetString(int value)
		{
			return this.names[value];
		}

		public object Parse(string value)
		{
			int num = Array.BinarySearch(this.items, value, this);
			if (num < 0)
			{
				return null;
			}
			return this.items[num].Value;
		}

		private FastEnumParser.EnumItem[] items;

		private string[] names;

		private StringComparer stringComparer;

		private class EnumItem
		{
			public EnumItem(string name, object value)
			{
				this.Name = name;
				this.Value = value;
			}

			public string Name;

			public object Value;
		}
	}
}
