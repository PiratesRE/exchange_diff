using System;
using System.Collections;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class EnabledEmailAddressTemplatesConstraint : CollectionPropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(IEnumerable value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			ProxyAddressTemplateCollection proxyAddressTemplateCollection = (ProxyAddressTemplateCollection)value;
			if (proxyAddressTemplateCollection == null || proxyAddressTemplateCollection.FindPrimary(ProxyAddressPrefix.Smtp) == null)
			{
				return new PropertyConstraintViolationError(DataStrings.EapMustHaveOneEnabledPrimarySmtpAddressTemplate, propertyDefinition, value, this);
			}
			return null;
		}
	}
}
