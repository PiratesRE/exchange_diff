using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AttributedValue<T> : IEquatable<AttributedValue<T>>
	{
		internal AttributedValue(T value, string[] attributions)
		{
			this.value = value;
			this.attributions = attributions;
		}

		public T Value
		{
			get
			{
				return this.value;
			}
			internal set
			{
				this.value = value;
			}
		}

		public string[] Attributions
		{
			get
			{
				return this.attributions;
			}
			internal set
			{
				this.attributions = value;
			}
		}

		public static void AddToList(List<AttributedValue<T>> list, AttributedValue<T> attributedValue)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (attributedValue == null)
			{
				throw new ArgumentNullException("attributedValue");
			}
			bool flag = false;
			foreach (AttributedValue<T> attributedValue2 in list)
			{
				if (AttributedValue<T>.ValuesEqual(attributedValue2.Value, attributedValue.Value))
				{
					flag = true;
					List<string> list2 = new List<string>();
					foreach (string item in attributedValue2.Attributions)
					{
						if (!list2.Contains(item))
						{
							list2.Add(item);
						}
					}
					foreach (string item2 in attributedValue.Attributions)
					{
						if (!list2.Contains(item2))
						{
							list2.Add(item2);
						}
					}
					attributedValue2.Attributions = list2.ToArray();
					break;
				}
			}
			if (!flag)
			{
				list.Add(attributedValue);
			}
		}

		public static T GetValueFromAttribution(List<AttributedValue<T>> list, string attribution)
		{
			T result = default(T);
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (string.IsNullOrWhiteSpace(attribution))
			{
				throw new ArgumentNullException("attribution");
			}
			bool flag = false;
			foreach (AttributedValue<T> attributedValue in list)
			{
				foreach (string a in attributedValue.Attributions)
				{
					if (string.Equals(a, attribution, StringComparison.OrdinalIgnoreCase))
					{
						result = attributedValue.Value;
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			return result;
		}

		public bool Equals(AttributedValue<T> other)
		{
			return other != null && (object.ReferenceEquals(this, other) || (AttributedValue<T>.ArraysEqual(this.attributions, other.attributions) && AttributedValue<T>.ValuesEqual(this.Value, other.Value)));
		}

		private static bool ValuesEqual(object value1, object value2)
		{
			if (value1 == null && value2 == null)
			{
				return true;
			}
			if (value1 == null || value2 == null)
			{
				return false;
			}
			if (!value1.GetType().Equals(value2.GetType()))
			{
				return false;
			}
			if (value1 is string)
			{
				return ((string)value1).Equals((string)value2);
			}
			if (value1 is IEquatable<T>)
			{
				return ((IEquatable<T>)value1).Equals((T)((object)value2));
			}
			return value1.GetType().IsArray && AttributedValue<T>.ArraysEqual(value1, value2);
		}

		private static bool ArraysEqual(object value1, object value2)
		{
			Array array = value1 as Array;
			Array array2 = value2 as Array;
			if (array.Length != array2.Length)
			{
				return false;
			}
			bool[] array3 = new bool[array2.Length];
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < array2.Length; j++)
				{
					if (!array3[j] && AttributedValue<T>.ValuesEqual(array.GetValue(i), array2.GetValue(j)))
					{
						array3[j] = true;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		private T value;

		private string[] attributions;
	}
}
