using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class HotmailSubscriptions : DataSourceService, IHotmailSubscriptions, IEditObjectService<HotmailSubscription, SetHotmailSubscription>, IGetObjectService<HotmailSubscription>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-HotmailSubscription?Mailbox&Identity@R:Self")]
		public PowerShellResults<HotmailSubscription> GetObject(Identity identity)
		{
			PSCommand psCommand = new PSCommand().AddCommand("Get-HotmailSubscription");
			psCommand.AddParameters(new GetHotmailSubscription());
			return base.GetObject<HotmailSubscription>(psCommand, identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-HotmailSubscription?Mailbox&Identity@R:Self+Set-HotmailSubscription?Mailbox&Identity@W:Self")]
		public PowerShellResults<HotmailSubscription> SetObject(Identity identity, SetHotmailSubscription properties)
		{
			return base.SetObject<HotmailSubscription, SetHotmailSubscription>("Set-HotmailSubscription", identity, properties);
		}

		private const string Noun = "HotmailSubscription";

		internal const string GetCmdlet = "Get-HotmailSubscription";

		internal const string SetCmdlet = "Set-HotmailSubscription";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string GetObjectRole = "MultiTenant+Get-HotmailSubscription?Mailbox&Identity@R:Self";

		private const string SetObjectRole = "MultiTenant+Get-HotmailSubscription?Mailbox&Identity@R:Self+Set-HotmailSubscription?Mailbox&Identity@W:Self";
	}
}
