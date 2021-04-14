using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Management.Tasks.UM;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class UMMailboxResetPin : DataSourceService, IUMMailboxResetPin, IEditObjectService<SetUMMailboxPinConfiguration, SetUMMailboxPinParameters>, IGetObjectService<SetUMMailboxPinConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Organization+Get-UMMailboxPin?Identity@R:Organization")]
		public PowerShellResults<SetUMMailboxPinConfiguration> GetObject(Identity identity)
		{
			PowerShellResults<SetUMMailboxPinConfiguration> @object = base.GetObject<SetUMMailboxPinConfiguration>("Get-UMMailbox", identity);
			if (@object.SucceededWithValue)
			{
				PowerShellResults<UMMailboxPin> powerShellResults = @object.MergeErrors<UMMailboxPin>(base.GetObject<UMMailboxPin>("Get-UMMailboxPin", identity));
				if (powerShellResults.SucceededWithValue)
				{
					@object.Value.UMMailboxPin = powerShellResults.Value;
				}
			}
			return @object;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Organization+Get-UMMailboxPin?Identity@R:Organization+Set-UMMailboxPin?Identity@W:Organization")]
		public PowerShellResults<SetUMMailboxPinConfiguration> SetObject(Identity identity, SetUMMailboxPinParameters properties)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Set-UMMailboxPin");
			pscommand.AddParameter("Identity", identity);
			pscommand.AddParameters(properties);
			PowerShellResults powerShellResults = base.Invoke(pscommand);
			PowerShellResults<SetUMMailboxPinConfiguration> powerShellResults2;
			if (powerShellResults.Succeeded)
			{
				powerShellResults2 = this.GetObject(identity);
			}
			else
			{
				powerShellResults2 = new PowerShellResults<SetUMMailboxPinConfiguration>();
				powerShellResults2.MergeErrors(powerShellResults);
			}
			return powerShellResults2;
		}

		private const string GetObjectRole = "Get-UMMailbox?Identity@R:Organization+Get-UMMailboxPin?Identity@R:Organization";

		private const string SetObjectRole = "Get-UMMailbox?Identity@R:Organization+Get-UMMailboxPin?Identity@R:Organization+Set-UMMailboxPin?Identity@W:Organization";
	}
}
