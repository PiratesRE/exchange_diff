using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "EOPMailUser", DefaultParameterSetName = "Identity")]
	public sealed class NewEOPMailUser : EOPTask
	{
		[Parameter(Mandatory = false)]
		public string Alias { get; set; }

		[Parameter(Mandatory = false)]
		public string DisplayName { get; set; }

		[Parameter(Mandatory = false)]
		public ProxyAddress ExternalEmailAddress { get; set; }

		[Parameter(Mandatory = false)]
		public string FirstName { get; set; }

		[Parameter(Mandatory = false)]
		public string Initials { get; set; }

		[Parameter(Mandatory = false)]
		public string LastName { get; set; }

		[Parameter(Mandatory = true)]
		public WindowsLiveId MicrosoftOnlineServicesID { get; set; }

		[Parameter(Mandatory = true)]
		public string Name { get; set; }

		[Parameter(Mandatory = true)]
		public SecureString Password { get; set; }

		[Parameter(Mandatory = false)]
		public SmtpAddress PrimarySmtpAddress { get; set; }

		protected override void InternalProcessRecord()
		{
			try
			{
				NewMailUserCmdlet newMailUserCmdlet = new NewMailUserCmdlet();
				ADObjectId executingUserId;
				base.ExchangeRunspaceConfig.TryGetExecutingUserId(out executingUserId);
				newMailUserCmdlet.Authenticator = Authenticator.Create(base.CurrentOrganizationId, executingUserId);
				newMailUserCmdlet.HostServerName = EOPRecipient.GetPsWsHostServerName();
				EOPRecipient.SetProperty(newMailUserCmdlet, Parameters.FirstName, this.FirstName);
				EOPRecipient.SetProperty(newMailUserCmdlet, Parameters.Initials, this.Initials);
				EOPRecipient.SetProperty(newMailUserCmdlet, Parameters.LastName, this.LastName);
				EOPRecipient.SetProperty(newMailUserCmdlet, Parameters.ExternalEmailAddress, this.ExternalEmailAddress);
				EOPRecipient.SetProperty(newMailUserCmdlet, Parameters.Alias, this.Alias);
				EOPRecipient.SetProperty(newMailUserCmdlet, Parameters.PrimarySmtpAddress, this.PrimarySmtpAddress);
				EOPRecipient.SetProperty(newMailUserCmdlet, Parameters.Organization, base.Organization);
				EOPRecipient.SetProperty(newMailUserCmdlet, Parameters.MicrosoftOnlineServicesID, this.MicrosoftOnlineServicesID);
				EOPRecipient.SetProperty(newMailUserCmdlet, Parameters.DisplayName, this.DisplayName);
				EOPRecipient.SetProperty(newMailUserCmdlet, Parameters.Name, this.Name);
				EOPRecipient.SetProperty(newMailUserCmdlet, Parameters.Password, this.Password);
				newMailUserCmdlet.Run();
				EOPRecipient.CheckForError(this, newMailUserCmdlet);
			}
			catch (Exception e)
			{
				base.ThrowAndLogTaskError(e);
			}
		}
	}
}
