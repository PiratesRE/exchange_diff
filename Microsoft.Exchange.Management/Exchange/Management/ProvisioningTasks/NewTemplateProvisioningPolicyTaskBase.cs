using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Provisioning;

namespace Microsoft.Exchange.Management.ProvisioningTasks
{
	public abstract class NewTemplateProvisioningPolicyTaskBase<TDataObject> : NewProvisioningPolicyTaskBase<TDataObject> where TDataObject : TemplateProvisioningPolicy, new()
	{
		protected override ADObjectId ContainerRdn
		{
			get
			{
				return TemplateProvisioningPolicy.RdnTemplateContainer;
			}
		}
	}
}
