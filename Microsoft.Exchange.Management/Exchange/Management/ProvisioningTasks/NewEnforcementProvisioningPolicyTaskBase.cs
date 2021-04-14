using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Provisioning;

namespace Microsoft.Exchange.Management.ProvisioningTasks
{
	public abstract class NewEnforcementProvisioningPolicyTaskBase<TDataObject> : NewProvisioningPolicyTaskBase<TDataObject> where TDataObject : EnforcementProvisioningPolicy, new()
	{
		protected override ADObjectId ContainerRdn
		{
			get
			{
				return EnforcementProvisioningPolicy.RdnEnforcementContainer;
			}
		}
	}
}
