using System;
using Microsoft.Exchange.Management.ForwardSyncTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetBposPlacementConfigurationCommand : SyntheticCommandWithPipelineInput<BposPlacementConfiguration, BposPlacementConfiguration>
	{
		private GetBposPlacementConfigurationCommand() : base("Get-BposPlacementConfiguration")
		{
		}

		public GetBposPlacementConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}
	}
}
