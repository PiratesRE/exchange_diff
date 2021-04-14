using System;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities
{
	public static class ExceptionHelper
	{
		public static void AssertNotNull<T>(this T value, Action<T> consume, string name) where T : class
		{
			consume(value.AssertNotNull(name));
		}

		public static TResult AssertNotNull<T, TResult>(this T value, Func<T, TResult> consume, string name) where T : class
		{
			return consume(value.AssertNotNull(name));
		}

		public static T AssertNotNull<T>(this T value, string name) where T : class
		{
			return value;
		}

		public static void ThrowIfPropertyNotSet(this PropertyChangeTrackingObject container, PropertyDefinition property)
		{
			if (!container.IsPropertySet(property))
			{
				throw new InvalidRequestException(Strings.ErrorMissingRequiredParameter(property.Name));
			}
		}
	}
}
