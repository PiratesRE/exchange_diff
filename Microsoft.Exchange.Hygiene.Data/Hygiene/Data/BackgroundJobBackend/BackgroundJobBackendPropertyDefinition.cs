using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class BackgroundJobBackendPropertyDefinition : SimpleProviderPropertyDefinition
	{
		public BackgroundJobBackendPropertyDefinition(string name, Type type, PropertyDefinitionFlags flags, object defaultValue) : base(name, ExchangeObjectVersion.Exchange2012, type, flags, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None)
		{
		}
	}
}
