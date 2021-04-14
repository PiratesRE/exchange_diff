using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class RangedValueConstraint<T> : PropertyDefinitionConstraint where T : struct, IComparable
	{
		public RangedValueConstraint(T minValue, T maxValue)
		{
			if (minValue.CompareTo(maxValue) > 0)
			{
				throw new ArgumentException("minValue > maxValue");
			}
			this.minValue = minValue;
			this.maxValue = maxValue;
		}

		public RangedValueConstraint(T minValue, T maxValue, LocalizedString customErrorMessage) : this(minValue, maxValue)
		{
			this.customErrorMessage = customErrorMessage;
		}

		public T MinimumValue
		{
			get
			{
				return this.minValue;
			}
		}

		public T MaximumValue
		{
			get
			{
				return this.maxValue;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (Comparer<T>.Default.Compare(this.MinimumValue, (T)((object)value)) > 0 || Comparer<T>.Default.Compare(this.MaximumValue, (T)((object)value)) < 0)
			{
				string arg;
				string arg2;
				string text;
				if (typeof(T).GetTypeInfo().IsEnum)
				{
					if (Enum.IsDefined(typeof(T), this.MinimumValue))
					{
						arg = string.Format("{0}:{1}", (this.MinimumValue as Enum).ToString("G"), (this.MinimumValue as Enum).ToString("D"));
					}
					else
					{
						arg = (this.MinimumValue as Enum).ToString("D");
					}
					if (Enum.IsDefined(typeof(T), this.MaximumValue))
					{
						arg2 = string.Format("{0}:{1}", (this.MaximumValue as Enum).ToString("G"), (this.MaximumValue as Enum).ToString("D"));
					}
					else
					{
						arg2 = (this.MaximumValue as Enum).ToString("D");
					}
					if (Enum.IsDefined(typeof(T), value))
					{
						text = string.Format("{0}:{1}", (value as Enum).ToString("G"), (value as Enum).ToString("D"));
					}
					else
					{
						text = (((T)((object)value)) as Enum).ToString("D");
					}
				}
				else
				{
					T minimumValue = this.MinimumValue;
					arg = minimumValue.ToString();
					T maximumValue = this.MaximumValue;
					arg2 = maximumValue.ToString();
					text = value.ToString();
				}
				LocalizedString empty = LocalizedString.Empty;
				if (!this.customErrorMessage.IsEmpty)
				{
					empty = new LocalizedString(string.Format(this.customErrorMessage.ToString(), arg, arg2, text));
				}
				return new PropertyConstraintViolationError((empty == LocalizedString.Empty) ? DataStrings.ConstraintViolationValueOutOfRange(arg, arg2, text) : empty, propertyDefinition, value, this);
			}
			return null;
		}

		public override string ToString()
		{
			return string.Format("[{0},{1}]", this.MinimumValue, this.MaximumValue);
		}

		private T minValue;

		private T maxValue;

		private LocalizedString customErrorMessage = LocalizedString.Empty;
	}
}
