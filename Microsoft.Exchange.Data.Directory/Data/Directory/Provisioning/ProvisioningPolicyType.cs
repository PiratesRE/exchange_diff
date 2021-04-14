using System;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[Flags]
	public enum ProvisioningPolicyType
	{
		Template = 1,
		Enforcement = 2
	}
}
