using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class AvailabilityAddressSpaceSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition ForestName = new ADPropertyDefinition("ForestName", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAvailabilityForestName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UserName = new ADPropertyDefinition("UserName", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAvailabilityUserName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition Password = new ADPropertyDefinition("Password", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAvailabilityUserPassword", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UseServiceAccount = new ADPropertyDefinition("UseServiceAccount", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchAvailabilityUseServiceAccount", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AccessMethod = new ADPropertyDefinition("AccessMethod", ExchangeObjectVersion.Exchange2007, typeof(AvailabilityAccessMethod), "msExchAvailabilityAccessMethod", ADPropertyDefinitionFlags.None, AvailabilityAccessMethod.PublicFolder, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(AvailabilityAccessMethod))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProxyUrl = new ADPropertyDefinition("ProxyUrl", ExchangeObjectVersion.Exchange2007, typeof(Uri), "wWWHomePage", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition TargetAutodiscoverEpr = new ADPropertyDefinition("TargetAutodiscoverEpr", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchFedTargetAutodiscoverEPR", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);
	}
}
