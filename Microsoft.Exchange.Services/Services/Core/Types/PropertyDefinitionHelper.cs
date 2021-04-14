using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class PropertyDefinitionHelper
	{
		internal static PropertyDefinition GenerateInternetHeaderPropertyDefinition(string internetHeaderName)
		{
			return GuidNamePropertyDefinition.CreateCustom(internetHeaderName, typeof(string), PropertyIdGuids.PSETIDInternetHeaders, internetHeaderName, PropertyFlags.None);
		}
	}
}
