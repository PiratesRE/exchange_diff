using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	internal class CasTransactionResultSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Value = new SimpleProviderPropertyDefinition("Value", ExchangeObjectVersion.Exchange2010, typeof(CasTransactionResultEnum), PropertyDefinitionFlags.None, CasTransactionResultEnum.Success, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
