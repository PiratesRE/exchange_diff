using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class GlobalLocatorServiceMsaUserSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition ExternalDirectoryOrganizationId = new SimpleProviderPropertyDefinition("ExternalDirectoryOrganizationId", ExchangeObjectVersion.Exchange2012, typeof(Guid?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MsaUserMemberName = new SimpleProviderPropertyDefinition("MsaUserMemberName", ExchangeObjectVersion.Exchange2012, typeof(SmtpAddress), PropertyDefinitionFlags.None, SmtpAddress.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition MsaUserNetId = new SimpleProviderPropertyDefinition("MsaUserNetId", ExchangeObjectVersion.Exchange2012, typeof(NetID), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ResourceForest = new SimpleProviderPropertyDefinition("ResourceForest", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition AccountForest = new SimpleProviderPropertyDefinition("AccountForest", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TenantContainerCN = new SimpleProviderPropertyDefinition("TenantContainerCN", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
