using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Provisioning;

namespace Microsoft.Exchange.Management.ProvisioningTasks
{
	[Cmdlet("Get", "RecipientTemplateProvisioningPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetRecipientTemplateProvisioningPolicy : GetProvisioningPolicyBase<ProvisioningPolicyIdParameter, RecipientTemplateProvisioningPolicy>
	{
	}
}
