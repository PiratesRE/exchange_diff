using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData
{
	internal static class ODataComplexValueExtensions
	{
		public static T GetPropertyValue<T>(this ODataComplexValue odataComplexValue, string propertyName, T defaultValue = default(T))
		{
			ArgumentValidator.ThrowIfNull("odataComplexValue", odataComplexValue);
			ArgumentValidator.ThrowIfNullOrEmpty("propertyName", propertyName);
			ODataProperty odataProperty = odataComplexValue.Properties.SingleOrDefault((ODataProperty x) => x.Name.Equals(propertyName));
			if (odataProperty == null)
			{
				return defaultValue;
			}
			return (T)((object)odataProperty.Value);
		}
	}
}
