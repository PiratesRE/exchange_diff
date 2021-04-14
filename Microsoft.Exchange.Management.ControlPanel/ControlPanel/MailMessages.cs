using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class MailMessages : DataSourceService, IMailMessages, INewObjectService<MailMessageRow, NewMailMessage>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Mailbox+New-MailMessage?Subject&Body&BodyFormat@W:Self")]
		public PowerShellResults<MailMessageRow> NewObject(NewMailMessage properties)
		{
			return base.NewObject<MailMessageRow, NewMailMessage>("New-MailMessage", properties);
		}

		internal const string NewCmdlet = "New-MailMessage";

		internal const string WriteScope = "@W:Self";

		private const string NewObjectRole = "Mailbox+New-MailMessage?Subject&Body&BodyFormat@W:Self";
	}
}
