using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregatedAccountListConfigurationSchema : UserConfigurationObjectSchema
	{
		public static readonly SimplePropertyDefinition AggregatedAccountList = new SimplePropertyDefinition("AggregatedAccountList", ExchangeObjectVersion.Exchange2012, typeof(List<AggregatedAccountInfo>), PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
