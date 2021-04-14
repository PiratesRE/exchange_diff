using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ExchangeConfigurationContainerSchemaWithAddressLists : ADContainerSchema
	{
		public static readonly ADPropertyDefinition AddressBookRoots = new ADPropertyDefinition("AddressBookRoots", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "addressBookRoots", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultGlobalAddressList = new ADPropertyDefinition("GlobalAddressList", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "globalAddressList", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AddressBookRoots2 = new ADPropertyDefinition("AddressBookRoots2", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "addressBookRoots2", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultGlobalAddressList2 = new ADPropertyDefinition("GlobalAddressList2", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "globalAddressList2", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
