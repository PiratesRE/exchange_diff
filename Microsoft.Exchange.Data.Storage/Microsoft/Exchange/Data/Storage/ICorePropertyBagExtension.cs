using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ICorePropertyBagExtension
	{
		internal static void SetOrDeleteProperty(this ICorePropertyBag propertyBag, PropertyDefinition property, object newValue)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (newValue == null || PropertyError.IsPropertyNotFound(newValue))
			{
				propertyBag.Delete(property);
				return;
			}
			propertyBag.SetProperty(property, newValue);
		}
	}
}
