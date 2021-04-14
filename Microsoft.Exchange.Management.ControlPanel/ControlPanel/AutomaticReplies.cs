using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class AutomaticReplies : DataSourceService, IAutomaticReplies, IEditObjectService<AutoReplyConfiguration, SetAutoReplyConfiguration>, IGetObjectService<AutoReplyConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxAutoReplyConfiguration?Identity@R:Self")]
		public PowerShellResults<AutoReplyConfiguration> GetObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			return base.GetObject<AutoReplyConfiguration>("Get-MailboxAutoReplyConfiguration", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MailboxAutoReplyConfiguration?Identity@R:Self+Set-MailboxAutoReplyConfiguration?Identity@W:Self")]
		public PowerShellResults<AutoReplyConfiguration> SetObject(Identity identity, SetAutoReplyConfiguration properties)
		{
			identity = Identity.FromExecutingUserId();
			return base.SetObject<AutoReplyConfiguration, SetAutoReplyConfiguration>("Set-MailboxAutoReplyConfiguration", identity, properties);
		}

		internal const string GetCmdlet = "Get-MailboxAutoReplyConfiguration";

		internal const string SetCmdlet = "Set-MailboxAutoReplyConfiguration";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string GetObjectRole = "Get-MailboxAutoReplyConfiguration?Identity@R:Self";

		private const string SetObjectRole = "Get-MailboxAutoReplyConfiguration?Identity@R:Self+Set-MailboxAutoReplyConfiguration?Identity@W:Self";
	}
}
