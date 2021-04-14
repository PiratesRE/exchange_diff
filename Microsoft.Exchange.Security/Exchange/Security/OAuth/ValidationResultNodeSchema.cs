using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.OAuth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ValidationResultNodeSchema : SimpleProviderObjectSchema
	{
		public static readonly ProviderPropertyDefinition Task = new SimpleProviderPropertyDefinition("Task", ExchangeObjectVersion.Exchange2010, typeof(LocalizedString), PropertyDefinitionFlags.None, LocalizedString.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition Detail = new SimpleProviderPropertyDefinition("Detail", ExchangeObjectVersion.Exchange2010, typeof(LocalizedString), PropertyDefinitionFlags.None, LocalizedString.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ResultType = new SimpleProviderPropertyDefinition("ResultType", ExchangeObjectVersion.Exchange2010, typeof(ResultType), PropertyDefinitionFlags.None, Microsoft.Exchange.Security.OAuth.ResultType.Success, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
