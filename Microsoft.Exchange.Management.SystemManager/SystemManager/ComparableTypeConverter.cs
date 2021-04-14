using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ComparableTypeConverter : TypeConverter
	{
		public ComparableTypeConverter(IComparer comparer)
		{
			this.Comparer = comparer;
		}

		public IComparer Comparer { get; private set; }

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return new ComparableTypeConverter.ObjectWithComparerAdapter(value, this.Comparer);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			throw new NotSupportedException();
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			throw new NotSupportedException();
		}

		private class ObjectWithComparerAdapter : IComparable
		{
			public ObjectWithComparerAdapter(object item, IComparer comparer)
			{
				this.Item = item;
				this.Comparer = comparer;
			}

			public object Item { get; private set; }

			public IComparer Comparer { get; private set; }

			public int CompareTo(object obj)
			{
				if (obj == null || DBNull.Value.Equals(obj))
				{
					return 1;
				}
				return this.Comparer.Compare(this.Item, (obj as ComparableTypeConverter.ObjectWithComparerAdapter).Item);
			}
		}
	}
}
