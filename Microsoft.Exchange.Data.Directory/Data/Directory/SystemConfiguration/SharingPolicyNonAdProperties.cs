using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class SharingPolicyNonAdProperties
	{
		internal static object GetDefault(IPropertyBag properties)
		{
			return properties[SharingPolicyNonAdProperties.DefaultPropetyDefinition];
		}

		internal static void SetDefault(object value, IPropertyBag properties)
		{
			properties[SharingPolicyNonAdProperties.DefaultPropetyDefinition] = value;
		}

		internal static readonly ADPropertyDefinition DefaultPropetyDefinition = new ADPropertyDefinition("Default", ExchangeObjectVersion.Exchange2010, typeof(bool), "Default", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
