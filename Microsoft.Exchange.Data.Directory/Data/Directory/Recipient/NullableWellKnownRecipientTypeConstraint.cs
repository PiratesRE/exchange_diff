using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	internal class NullableWellKnownRecipientTypeConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			WellKnownRecipientType? wellKnownRecipientType = (WellKnownRecipientType?)value;
			if (wellKnownRecipientType != null && wellKnownRecipientType != WellKnownRecipientType.None && WellKnownRecipientType.AllRecipients != wellKnownRecipientType && (~(WellKnownRecipientType.MailboxUsers | WellKnownRecipientType.Resources | WellKnownRecipientType.MailContacts | WellKnownRecipientType.MailGroups | WellKnownRecipientType.MailUsers) & wellKnownRecipientType) != WellKnownRecipientType.None)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ConstraintViolationInvalidRecipientType(propertyDefinition.Name, value.ToString()), propertyDefinition, value, this);
			}
			return null;
		}
	}
}
