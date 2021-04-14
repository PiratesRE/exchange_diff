using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PopBookmark
	{
		public PopBookmark(string encoded)
		{
			if (string.IsNullOrEmpty(encoded))
			{
				return;
			}
			string[] array = encoded.Split(PopBookmark.separators);
			int num = int.Parse(array[0]);
			if (num > 0)
			{
				this.items = new Dictionary<string, string>(num);
				for (int i = 1; i < array.Length; i++)
				{
					this.items[array[i]] = array[i];
				}
			}
		}

		internal ICollection<string> Values
		{
			get
			{
				if (this.items == null)
				{
					return PopBookmark.empty;
				}
				return this.items.Keys;
			}
		}

		internal bool HasChanged
		{
			get
			{
				return this.hasChanged;
			}
			set
			{
				this.hasChanged = false;
			}
		}

		public override string ToString()
		{
			if (this.items == null || this.items.Count == 0)
			{
				return string.Empty;
			}
			IEnumerator<string> enumerator = this.items.Keys.GetEnumerator();
			enumerator.MoveNext();
			int num = 4;
			int num2 = enumerator.Current.Length + 1;
			int num3 = this.items.Count * num2;
			int capacity = num + num3;
			StringBuilder stringBuilder = new StringBuilder(capacity);
			stringBuilder.Append(this.items.Count);
			do
			{
				stringBuilder.Append(PopBookmark.separator);
				stringBuilder.Append(enumerator.Current);
			}
			while (enumerator.MoveNext());
			return stringBuilder.ToString();
		}

		internal static PopBookmark Parse(string encoded)
		{
			return new PopBookmark(encoded);
		}

		internal bool SetCapacity(int capacity)
		{
			if (capacity != 0 && this.items == null)
			{
				this.items = new Dictionary<string, string>(capacity);
				return true;
			}
			return false;
		}

		internal bool Contains(string item)
		{
			return this.items != null && this.items.ContainsKey(item);
		}

		internal void Add(string item)
		{
			if (this.items == null)
			{
				this.items = new Dictionary<string, string>(1);
			}
			if (this.items.ContainsKey(item))
			{
				return;
			}
			this.items.Add(item, item);
			this.hasChanged = true;
		}

		internal bool Remove(string item)
		{
			if (this.items == null)
			{
				return false;
			}
			if (this.items.Remove(item))
			{
				this.hasChanged = true;
				return true;
			}
			return false;
		}

		internal void Clear()
		{
			if (this.items == null || this.items.Count == 0)
			{
				return;
			}
			this.items.Clear();
			this.hasChanged = true;
		}

		private const int MinEncodedLength = 4;

		private static char separator = ' ';

		private static char[] separators = new char[]
		{
			PopBookmark.separator
		};

		private static string[] empty = new string[0];

		private Dictionary<string, string> items;

		private bool hasChanged;
	}
}
