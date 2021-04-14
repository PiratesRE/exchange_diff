using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Update", "DistributionGroupMember", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class UpdateDistributionGroupMember : UpdateDistributionGroupMemberBase
	{
	}
}
