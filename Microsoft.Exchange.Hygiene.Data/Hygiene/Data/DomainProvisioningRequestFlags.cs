using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	[Flags]
	public enum DomainProvisioningRequestFlags
	{
		Default = 0,
		Reporting = 1,
		GlobalLocator = 2,
		DNS = 4,
		RelayDomainUpdate = 8
	}
}
