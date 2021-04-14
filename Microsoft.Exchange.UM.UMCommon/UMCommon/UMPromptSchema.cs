using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMPromptSchema : SimpleProviderObjectSchema
	{
		private static SimpleProviderPropertyDefinition CreatePropertyDefinition(string propertyName, Type propertyType, object defaultValue)
		{
			return new SimpleProviderPropertyDefinition(propertyName, ExchangeObjectVersion.Exchange2010, propertyType, PropertyDefinitionFlags.None, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		public static SimpleProviderPropertyDefinition AudioData = UMPromptSchema.CreatePropertyDefinition("AudioData", typeof(byte[]), null);

		public static SimpleProviderPropertyDefinition Name = UMPromptSchema.CreatePropertyDefinition("Name", typeof(string), null);
	}
}
