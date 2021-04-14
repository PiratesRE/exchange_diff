using System;

namespace Microsoft.Exchange.Data
{
	internal delegate PropertyConstraintViolationError ValidationDelegate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner);
}
