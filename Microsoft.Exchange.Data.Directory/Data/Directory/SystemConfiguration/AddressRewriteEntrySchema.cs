using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class AddressRewriteEntrySchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition InternalAddress = new ADPropertyDefinition("InternalAddress", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAddressRewriteInternalName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalAddress = new ADPropertyDefinition("ExternalAddress", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAddressRewriteExternalName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptionList = new ADPropertyDefinition("ExceptionList", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAddressRewriteExceptionList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Direction = new ADPropertyDefinition("Direction", ExchangeObjectVersion.Exchange2007, typeof(EntryDirection), "msExchAddressRewriteMappingType", ADPropertyDefinitionFlags.None, EntryDirection.Bidirectional, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
