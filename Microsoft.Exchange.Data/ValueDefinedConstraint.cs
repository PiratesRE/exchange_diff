using System;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class ValueDefinedConstraint<T> : PropertyDefinitionConstraint
	{
		public ValueDefinedConstraint(T[] valuesArray, bool specifyAllowedValues)
		{
			if (valuesArray == null)
			{
				throw new ArgumentNullException("valuesArray");
			}
			this.valuesArray = new T[valuesArray.Length];
			valuesArray.CopyTo(this.valuesArray, 0);
			this.specifyAllowedValues = specifyAllowedValues;
		}

		public ValueDefinedConstraint(T[] allowedValuesArray) : this(allowedValuesArray, true)
		{
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			T t;
			try
			{
				t = (T)((object)ValueConvertor.ConvertValue(value, typeof(T), null));
			}
			catch (NotImplementedException ex)
			{
				return new PropertyConstraintViolationError(new LocalizedString(ex.Message), propertyDefinition, value, this);
			}
			catch (TypeConversionException ex2)
			{
				return new PropertyConstraintViolationError(ex2.LocalizedString, propertyDefinition, value, this);
			}
			if (this.specifyAllowedValues)
			{
				foreach (T t2 in this.valuesArray)
				{
					if (object.Equals(t2, t))
					{
						return null;
					}
				}
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationValueIsNotAllowed(this.GetValuesString(this.valuesArray), t.ToString()), propertyDefinition, t, this);
			}
			foreach (T t3 in this.valuesArray)
			{
				if (object.Equals(t3, t))
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationValueIsDisallowed(this.GetValuesString(this.valuesArray), t.ToString()), propertyDefinition, t, this);
				}
			}
			return null;
		}

		private string GetValuesString(T[] values)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Format("\"{0}\"", values[0].ToString()));
			for (int i = 1; i < values.Length; i++)
			{
				stringBuilder.Append(string.Format(", \"{0}\"", values[i].ToString()));
			}
			return stringBuilder.ToString();
		}

		private T[] valuesArray;

		private bool specifyAllowedValues;
	}
}
