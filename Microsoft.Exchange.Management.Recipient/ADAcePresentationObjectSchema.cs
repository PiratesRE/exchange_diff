using System;
using System.DirectoryServices;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal class ADAcePresentationObjectSchema : AcePresentationObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition AccessRights = new SimpleProviderPropertyDefinition("AccessRights", ExchangeObjectVersion.Exchange2003, typeof(ActiveDirectoryRights[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExtendedRights = new SimpleProviderPropertyDefinition("ExtendedRights", ExchangeObjectVersion.Exchange2003, typeof(ExtendedRightIdParameter[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ChildObjectTypes = new SimpleProviderPropertyDefinition("ChildObjectTypes", ExchangeObjectVersion.Exchange2003, typeof(ADSchemaObjectIdParameter[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition InheritedObjectType = new SimpleProviderPropertyDefinition("InheritedObjectType", ExchangeObjectVersion.Exchange2003, typeof(ADSchemaObjectIdParameter), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Properties = new SimpleProviderPropertyDefinition("Properties", ExchangeObjectVersion.Exchange2003, typeof(ADSchemaObjectIdParameter[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
