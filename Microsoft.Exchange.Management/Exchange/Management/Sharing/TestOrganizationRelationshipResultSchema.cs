using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Sharing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TestOrganizationRelationshipResultSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Id = new SimpleProviderPropertyDefinition("Id", ExchangeObjectVersion.Exchange2007, typeof(TestOrganizationRelationshipResultId), PropertyDefinitionFlags.None, TestOrganizationRelationshipResultId.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Status = new SimpleProviderPropertyDefinition("Status", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Description = new SimpleProviderPropertyDefinition("Description", ExchangeObjectVersion.Exchange2007, typeof(LocalizedString), PropertyDefinitionFlags.None, LocalizedString.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
