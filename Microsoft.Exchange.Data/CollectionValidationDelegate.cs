using System;
using System.Collections;

namespace Microsoft.Exchange.Data
{
	internal delegate PropertyConstraintViolationError CollectionValidationDelegate(IEnumerable collection, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner);
}
