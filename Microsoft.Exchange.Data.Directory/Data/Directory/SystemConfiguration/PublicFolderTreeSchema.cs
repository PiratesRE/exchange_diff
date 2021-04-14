using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class PublicFolderTreeSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition PublicFolderTreeType = new ADPropertyDefinition("PublicFolderTreeType", ExchangeObjectVersion.Exchange2003, typeof(PublicFolderTreeType), "msExchPFTreeType", ADPropertyDefinitionFlags.PersistDefaultValue, Microsoft.Exchange.Data.Directory.SystemConfiguration.PublicFolderTreeType.Mapi, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(PublicFolderTreeType))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PublicFolderDefaultAdminAcl = SharedPropertyDefinitions.PublicFolderDefaultAdminAcl;

		public static readonly ADPropertyDefinition PublicDatabases = new ADPropertyDefinition("PublicDatabases", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchOwningPFTreeBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
