using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal class ScopeSettingsSchema : ObjectSchema
	{
		public static readonly AdminPropertyDefinition ForestViewEnabled = new AdminPropertyDefinition("ForestViewEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly AdminPropertyDefinition OrganizationalUnit = new AdminPropertyDefinition("OrganizationalUnit", ExchangeObjectVersion.Exchange2003, typeof(string), null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
