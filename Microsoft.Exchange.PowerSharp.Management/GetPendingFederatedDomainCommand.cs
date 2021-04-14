using System;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetPendingFederatedDomainCommand : SyntheticCommandWithPipelineInput<PendingFederatedDomain, PendingFederatedDomain>
	{
		private GetPendingFederatedDomainCommand() : base("Get-PendingFederatedDomain")
		{
		}

		public GetPendingFederatedDomainCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}
	}
}
