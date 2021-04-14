using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Provisioning;

namespace Microsoft.Exchange.Management.ProvisioningTasks
{
	public abstract class GetProvisioningPolicyBase<TIdentity, TDataObject> : GetMultitenancySystemConfigurationObjectTask<TIdentity, TDataObject> where TIdentity : ProvisioningPolicyIdParameter, new() where TDataObject : ADProvisioningPolicy, new()
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}
	}
}
