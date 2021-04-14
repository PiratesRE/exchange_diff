using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ForwardEmails : RecipientDataSourceService, IForwardEmails, IEditObjectService<ForwardEmailMailbox, SetForwardEmailMailbox>, IGetObjectService<ForwardEmailMailbox>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Mailbox?Identity@R:Self")]
		public PowerShellResults<ForwardEmailMailbox> GetObject(Identity identity)
		{
			identity = (identity ?? Identity.FromExecutingUserId());
			return base.GetObject<ForwardEmailMailbox>("Get-Mailbox", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Mailbox?Identity@R:Self+Set-Mailbox?Identity@W:Self")]
		public PowerShellResults<ForwardEmailMailbox> SetObject(Identity identity, SetForwardEmailMailbox properties)
		{
			identity = (identity ?? Identity.FromExecutingUserId());
			return base.SetObject<ForwardEmailMailbox, SetForwardEmailMailbox>("Set-Mailbox", identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Mailbox?Identity@R:Self+Set-Mailbox?Identity@W:Self")]
		public PowerShellResults<ForwardEmailMailbox> StopForward(Identity[] identities, BaseWebServiceParameters parameters)
		{
			SetForwardEmailMailbox setForwardEmailMailbox = new SetForwardEmailMailbox();
			setForwardEmailMailbox.ForwardingSmtpAddress = null;
			return this.SetObject(identities[0], setForwardEmailMailbox);
		}

		private const string GetObjectRole = "Get-Mailbox?Identity@R:Self";

		private const string SetObjectRole = "Get-Mailbox?Identity@R:Self+Set-Mailbox?Identity@W:Self";

		private const string StopForwardRole = "Get-Mailbox?Identity@R:Self+Set-Mailbox?Identity@W:Self";
	}
}
