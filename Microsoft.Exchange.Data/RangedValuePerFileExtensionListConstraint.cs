using System;

namespace Microsoft.Exchange.Data
{
	internal class RangedValuePerFileExtensionListConstraint : PropertyDefinitionConstraint
	{
		public RangedValuePerFileExtensionListConstraint(int minValue, int maxValue, int maxLength, char extensionValueDelimiter, char pairDelimiter)
		{
			if (minValue > maxValue)
			{
				throw new ArgumentException("minValue > maxValue");
			}
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.maxLength = maxLength;
			this.extensionValueDelimiter = extensionValueDelimiter;
			this.pairDelimiter = pairDelimiter;
		}

		public int MinValue
		{
			get
			{
				return this.minValue;
			}
		}

		public int MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
		}

		public char ExtensionValueDelimiter
		{
			get
			{
				return this.extensionValueDelimiter;
			}
		}

		public char PairDelimiter
		{
			get
			{
				return this.pairDelimiter;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			string text = (string)value;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			if (text.Length > this.maxLength)
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationObjectIsBeyondRange(this.maxLength.ToString()), propertyDefinition, value, this);
			}
			int num = 0;
			if (!int.TryParse(text, out num))
			{
				char[] separator = new char[]
				{
					this.pairDelimiter
				};
				char[] separator2 = new char[]
				{
					this.extensionValueDelimiter
				};
				string[] array = text.Split(separator);
				string[] array2 = array;
				int i = 0;
				while (i < array2.Length)
				{
					string text2 = array2[i];
					string[] array3 = text2.Split(separator2);
					PropertyConstraintViolationError result;
					if (array3.Length != 2)
					{
						result = new PropertyConstraintViolationError(DataStrings.ConstraintViolationMalformedExtensionValuePair(text2), propertyDefinition, value, this);
					}
					else if (array3[0].Length == 0)
					{
						result = new PropertyConstraintViolationError(DataStrings.ConstraintViolationMalformedExtensionValuePair(text2), propertyDefinition, value, this);
					}
					else
					{
						int num2 = 0;
						if (!int.TryParse(array3[1], out num2))
						{
							result = new PropertyConstraintViolationError(DataStrings.ConstraintViolationMalformedExtensionValuePair(text2), propertyDefinition, value, this);
						}
						else
						{
							if (num2 >= this.minValue && num2 <= this.maxValue)
							{
								i++;
								continue;
							}
							result = new PropertyConstraintViolationError(DataStrings.ConstraintViolationValueOutOfRange(this.minValue.ToString(), this.maxValue.ToString(), num2.ToString()), propertyDefinition, value, this);
						}
					}
					return result;
				}
				return null;
			}
			if (num < this.minValue)
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationObjectIsBelowRange(this.minValue.ToString()), propertyDefinition, value, this);
			}
			if (num > this.maxValue)
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationObjectIsBeyondRange(this.maxValue.ToString()), propertyDefinition, value, this);
			}
			return null;
		}

		private int minValue;

		private int maxValue;

		private int maxLength;

		private char extensionValueDelimiter;

		private char pairDelimiter;
	}
}
