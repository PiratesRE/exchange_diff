using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ProtocolSettings : DataSourceService, IProtocolSettings, IGetObjectService<ProtocolSettingsData>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-CasMailbox?Identity&ProtocolSettings@R:Self")]
		public PowerShellResults<ProtocolSettingsData> GetObject(Identity identity)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Get-CasMailbox");
			pscommand.AddParameter("ProtocolSettings", true);
			return base.GetObject<ProtocolSettingsData>(pscommand, Identity.FromExecutingUserId());
		}

		private const string GetObjectRole = "Get-CasMailbox?Identity&ProtocolSettings@R:Self";

		internal const string GetCmdlet = "Get-CasMailbox";

		internal const string ReadScope = "@R:Self";
	}
}
