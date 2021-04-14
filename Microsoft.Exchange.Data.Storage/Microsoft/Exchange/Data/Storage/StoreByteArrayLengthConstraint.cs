using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class StoreByteArrayLengthConstraint : PropertyDefinitionConstraint
	{
		public int MaxLength
		{
			get
			{
				return this.maxLength;
			}
		}

		public StoreByteArrayLengthConstraint(int maxLength)
		{
			if (maxLength <= 0)
			{
				throw new ArgumentException("maxLength must be greater than 0", "maxLength");
			}
			this.maxLength = maxLength;
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			byte[] array = value as byte[];
			if (array != null && array.Length > this.maxLength)
			{
				return new PropertyConstraintViolationError(new LocalizedString(ServerStrings.ExConstraintViolationByteArrayLengthTooLong(propertyDefinition.Name, (long)this.maxLength, (long)array.Length)), propertyDefinition, value, this);
			}
			return null;
		}

		private int maxLength;
	}
}
