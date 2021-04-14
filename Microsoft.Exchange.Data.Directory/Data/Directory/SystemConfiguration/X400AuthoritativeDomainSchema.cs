using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class X400AuthoritativeDomainSchema : AcceptedDomainSchema
	{
		public new static readonly ADPropertyDefinition DomainName = new ADPropertyDefinition("DomainName", ExchangeObjectVersion.Exchange2007, typeof(X400Domain), "msExchAcceptedDomainName", ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
