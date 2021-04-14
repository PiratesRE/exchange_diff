using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IValidatablePropertyBag
	{
		bool IsPropertyDirty(PropertyDefinition propertyDefinition);

		object TryGetProperty(PropertyDefinition propertyDefinition);

		PropertyValueTrackingData GetOriginalPropertyInformation(PropertyDefinition propertyDefinition);
	}
}
