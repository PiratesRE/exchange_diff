using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	internal enum ADServerRole
	{
		None = 0,
		GlobalCatalog = 1,
		DomainController = 2,
		ConfigurationDomainController = 4
	}
}
