using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class DisallowStarSmtpDomainWithSubdomainsConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			SmtpDomainWithSubdomains smtpDomainWithSubdomains = value as SmtpDomainWithSubdomains;
			if (smtpDomainWithSubdomains != null && smtpDomainWithSubdomains.IsStar)
			{
				return new PropertyConstraintViolationError(DataStrings.StarDomainNotAllowed(propertyDefinition.Name), propertyDefinition, value, this);
			}
			return null;
		}
	}
}
