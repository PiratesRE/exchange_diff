using System;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetAdServerSettingsCommand : SyntheticCommandWithPipelineInput<RunspaceServerSettingsPresentationObject, RunspaceServerSettingsPresentationObject>
	{
		private GetAdServerSettingsCommand() : base("Get-AdServerSettings")
		{
		}

		public GetAdServerSettingsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}
	}
}
