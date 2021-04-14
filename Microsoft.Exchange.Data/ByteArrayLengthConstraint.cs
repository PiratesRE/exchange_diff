using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class ByteArrayLengthConstraint : PropertyDefinitionConstraint
	{
		public int MinLength
		{
			get
			{
				return this.minLength;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
		}

		public ByteArrayLengthConstraint(int minLength, int maxLength)
		{
			if (minLength > maxLength && maxLength != 0)
			{
				throw new ArgumentException(DataStrings.ConstraintViolationObjectIsBeyondRange(minLength.ToString()));
			}
			if (minLength < 0)
			{
				throw new ArgumentException(DataStrings.ConstraintViolationObjectIsBelowRange("0"));
			}
			this.minLength = minLength;
			this.maxLength = maxLength;
		}

		public ByteArrayLengthConstraint(int length) : this(length, length)
		{
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			byte[] array = (byte[])value;
			if (array != null)
			{
				if (array.Length < this.minLength)
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationByteArrayLengthTooShort(this.minLength, array.Length), propertyDefinition, value, this);
				}
				if (this.maxLength != 0 && array.Length > this.maxLength)
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationByteArrayLengthTooLong(this.maxLength, array.Length), propertyDefinition, value, this);
				}
			}
			return null;
		}

		private int minLength;

		private int maxLength;
	}
}
