using System;

namespace Microsoft.Exchange.Monitoring
{
	public enum TopologyServiceServerRole
	{
		None,
		GlobalCatalog,
		DomainController,
		ConfigurationDomainController = 4
	}
}
