using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class PropertyBagExtensions
	{
		public static IEnumerable<PropertyTag> WithNoValue(this IPropertyBag propertyBag, IEnumerable<PropertyTag> propertyTags)
		{
			foreach (PropertyTag propertyTag in propertyTags)
			{
				PropertyValue propertyValue = propertyBag.GetAnnotatedProperty(propertyTag).PropertyValue;
				if (!PropertyBagExtensions.IsValuePresent(propertyValue))
				{
					yield return propertyTag;
				}
			}
			yield break;
		}

		private static bool IsValuePresent(PropertyValue propertyValue)
		{
			return !propertyValue.IsError || (ErrorCode)propertyValue.Value == (ErrorCode)2147746565U;
		}
	}
}
