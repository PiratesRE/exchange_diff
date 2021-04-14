using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetProvisioningReconciliationConfigCommand : SyntheticCommandWithPipelineInput<ProvisioningReconciliationConfig, ProvisioningReconciliationConfig>
	{
		private GetProvisioningReconciliationConfigCommand() : base("Get-ProvisioningReconciliationConfig")
		{
		}

		public GetProvisioningReconciliationConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}
	}
}
