using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal sealed class ADObjectNameStringLengthConstraint : MandatoryStringLengthConstraint
	{
		public ADObjectNameStringLengthConstraint(int minLength, int maxLength) : base(minLength, maxLength)
		{
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				return null;
			}
			if (ADObjectNameHelper.ReservedADNameStringRegex.IsMatch(value.ToString()))
			{
				return null;
			}
			return base.Validate(value, propertyDefinition, propertyBag);
		}
	}
}
