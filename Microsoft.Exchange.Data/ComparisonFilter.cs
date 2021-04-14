using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class ComparisonFilter : SinglePropertyFilter
	{
		public ComparisonFilter(ComparisonOperator comparisonOperator, PropertyDefinition property, object propertyValue) : base(property)
		{
			if (comparisonOperator != ComparisonOperator.Equal && ComparisonOperator.NotEqual != comparisonOperator)
			{
				Type type = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
				if (!typeof(IComparable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) && !typeof(string[]).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) && !typeof(byte[]).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
				{
					throw new ArgumentOutOfRangeException(DataStrings.ExceptionComparisonNotSupported(property.Name, property.Type, comparisonOperator));
				}
			}
			this.comparisonOperator = comparisonOperator;
			this.propertyValue = propertyValue;
		}

		public ComparisonOperator ComparisonOperator
		{
			get
			{
				return this.comparisonOperator;
			}
		}

		public object PropertyValue
		{
			get
			{
				return this.propertyValue;
			}
		}

		public override SinglePropertyFilter CloneWithAnotherProperty(PropertyDefinition property)
		{
			base.CheckClonable(property);
			return new ComparisonFilter(this.comparisonOperator, property, this.propertyValue);
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append(base.Property.Name);
			sb.Append(" ");
			sb.Append(this.comparisonOperator.ToString());
			sb.Append(" ");
			IFormattable formattable = this.propertyValue as IFormattable;
			if (formattable != null)
			{
				sb.Append(formattable.ToString(null, CultureInfo.InvariantCulture));
			}
			else
			{
				sb.Append(this.propertyValue ?? "<null>");
			}
			sb.Append(")");
		}

		public override bool Equals(object obj)
		{
			ComparisonFilter comparisonFilter = obj as ComparisonFilter;
			return comparisonFilter != null && this.comparisonOperator == comparisonFilter.comparisonOperator && comparisonFilter.GetType() == base.GetType() && ComparisonFilter.PropertyValueEquals(this.propertyValue, comparisonFilter.propertyValue) && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (int)this.comparisonOperator;
		}

		private static bool PropertyValueEquals(object value1, object value2)
		{
			if (object.Equals(value1, value2))
			{
				return true;
			}
			if (value1 is Enum || value2 is Enum)
			{
				IConvertible convertible = value1 as IConvertible;
				IConvertible convertible2 = value2 as IConvertible;
				if (convertible != null && convertible2 != null)
				{
					try
					{
						ulong num = convertible.ToUInt64(null);
						ulong num2 = convertible2.ToUInt64(null);
						return num == num2;
					}
					catch (OverflowException)
					{
					}
					catch (FormatException)
					{
						return false;
					}
					try
					{
						long num3 = convertible.ToInt64(null);
						long num4 = convertible2.ToInt64(null);
						return num3 == num4;
					}
					catch (OverflowException)
					{
						return false;
					}
					catch (FormatException)
					{
						return false;
					}
					return false;
				}
				return false;
			}
			IList list = value1 as IList;
			IList list2 = value2 as IList;
			if (list != null && list2 != null)
			{
				bool flag = list2.Count == list.Count;
				int num5 = 0;
				while (flag && num5 < list.Count)
				{
					flag = ComparisonFilter.PropertyValueEquals(list[num5], list2[num5]);
					num5++;
				}
				return flag;
			}
			return false;
		}

		private readonly ComparisonOperator comparisonOperator;

		private readonly object propertyValue;
	}
}
