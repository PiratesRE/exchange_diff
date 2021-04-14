using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class JunkEmailConfigurations : DataSourceService, IJunkEmailConfigurations, IEditObjectService<JunkEmailConfiguration, SetJunkEmailConfiguration>, IGetObjectService<JunkEmailConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxJunkEmailConfiguration?Identity@R:Self")]
		public PowerShellResults<JunkEmailConfiguration> GetObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			return base.GetObject<JunkEmailConfiguration>("Get-MailboxJunkEmailConfiguration", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxJunkEmailConfiguration?Identity@R:Self+Set-MailboxJunkEmailConfiguration?Identity@W:Self")]
		public PowerShellResults<JunkEmailConfiguration> SetObject(Identity identity, SetJunkEmailConfiguration properties)
		{
			identity = Identity.FromExecutingUserId();
			return base.SetObject<JunkEmailConfiguration, SetJunkEmailConfiguration>("Set-MailboxJunkEmailConfiguration", identity, properties);
		}

		internal const string GetCmdlet = "Get-MailboxJunkEmailConfiguration";

		internal const string SetCmdlet = "Set-MailboxJunkEmailConfiguration";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string GetObjectRole = "Get-MailboxJunkEmailConfiguration?Identity@R:Self";

		private const string SetObjectRole = "Get-MailboxJunkEmailConfiguration?Identity@R:Self+Set-MailboxJunkEmailConfiguration?Identity@W:Self";
	}
}
