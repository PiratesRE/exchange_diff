using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class AdministrativeGroupSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition PublicFolderDatabase = SharedPropertyDefinitions.SitePublicFolderDatabase;

		public static readonly ADPropertyDefinition LegacyExchangeDN = SharedPropertyDefinitions.LegacyExchangeDN;

		public static readonly ADPropertyDefinition PublicFolderDefaultAdminAcl = SharedPropertyDefinitions.PublicFolderDefaultAdminAcl;

		public static readonly ADPropertyDefinition DisplayName = SharedPropertyDefinitions.MandatoryDisplayName;

		public static readonly ADPropertyDefinition AdminGroupMode = new ADPropertyDefinition("AdminGroupMode", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchAdminGroupMode", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DefaultAdminGroup = new ADPropertyDefinition("DefaultAdminGroup", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchDefaultAdminGroup", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
