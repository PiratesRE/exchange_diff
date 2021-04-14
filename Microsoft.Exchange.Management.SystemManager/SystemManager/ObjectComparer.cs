using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ObjectComparer : IObjectComparer, ISupportTextComparer
	{
		public ObjectComparer(ITextComparer textComparer)
		{
			if (textComparer == null)
			{
				throw new ArgumentNullException("textComparer");
			}
			this.TextComparer = textComparer;
		}

		public ITextComparer TextComparer { get; private set; }

		public SortMode GetSortMode(Type type)
		{
			if (typeof(IComparable).IsAssignableFrom(type) && !typeof(Enum).IsAssignableFrom(type))
			{
				return SortMode.Standard;
			}
			return SortMode.Custom;
		}

		public int Compare(object x, object y, ICustomFormatter customFormatter, IFormatProvider formatProvider, string formatString, string defaultEmptyText)
		{
			if (this.IsNullValue(x))
			{
				if (!this.IsNullValue(y))
				{
					return -1;
				}
				return 0;
			}
			else
			{
				if (this.IsNullValue(y))
				{
					return 1;
				}
				IComparable comparable = this.ConverToIComparable(x, customFormatter, formatProvider, formatString, defaultEmptyText);
				IComparable obj = this.ConverToIComparable(y, customFormatter, formatProvider, formatString, defaultEmptyText);
				return comparable.CompareTo(obj);
			}
		}

		private bool IsNullValue(object item)
		{
			return item == null || DBNull.Value.Equals(item);
		}

		private IComparable ConverToIComparable(object item, ICustomFormatter customFormatter, IFormatProvider formatProvider, string formatString, string defaultEmptyText)
		{
			IComparable comparable = this.ToComparable(item);
			if (comparable == null)
			{
				comparable = this.TextComparer.Format(item, customFormatter, formatProvider, formatString, string.Empty);
			}
			return comparable;
		}

		private IComparable ToComparable(object item)
		{
			if (this.GetSortMode(item.GetType()) == SortMode.Standard)
			{
				return item as IComparable;
			}
			if (item is Enum)
			{
				return new EnumObject(item as Enum);
			}
			if (item.GetType().IsGenericType && item.GetType().GetGenericTypeDefinition() == typeof(IComparable<>))
			{
				return new ObjectComparer.GenericComparableAdapter(item);
			}
			if (item.GetType().IsGenericType && item.GetType().GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return this.ToComparable(ObjectComparer.GetPropertyValue(typeof(Nullable<>), item, "Value"));
			}
			return null;
		}

		private static object GetPropertyValue(Type t, object obj, string propertyName)
		{
			return t.GetProperty(propertyName).GetValue(obj, null);
		}

		public static ObjectComparer DefaultObjectComparer = new ObjectComparer(Microsoft.Exchange.Management.SystemManager.TextComparer.DefaultTextComparer);

		private class GenericComparableAdapter : IComparable
		{
			public GenericComparableAdapter(object genericComparable)
			{
				if (genericComparable == null)
				{
					throw new ArgumentNullException();
				}
				this.genericComparable = genericComparable;
			}

			public int CompareTo(object obj)
			{
				if (DBNull.Value.Equals(obj))
				{
					obj = null;
				}
				if (obj != null && !obj.GetType().Equals(this.genericComparable.GetType()))
				{
					return 0;
				}
				return (int)typeof(IComparable<>).GetMethod("CompareTo").Invoke(this.genericComparable, new object[]
				{
					obj
				});
			}

			private object genericComparable;
		}
	}
}
