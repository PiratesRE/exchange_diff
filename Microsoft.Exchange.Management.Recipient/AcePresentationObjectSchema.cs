using System;
using System.DirectoryServices;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal class AcePresentationObjectSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition IsInherited = new SimpleProviderPropertyDefinition("IsInherited", ExchangeObjectVersion.Exchange2003, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition User = new SimpleProviderPropertyDefinition("User", ExchangeObjectVersion.Exchange2003, typeof(SecurityPrincipalIdParameter), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition InheritanceType = new SimpleProviderPropertyDefinition("InheritanceType", ExchangeObjectVersion.Exchange2003, typeof(ActiveDirectorySecurityInheritance), PropertyDefinitionFlags.None, ActiveDirectorySecurityInheritance.All, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Deny = new SimpleProviderPropertyDefinition("Deny", ExchangeObjectVersion.Exchange2003, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
