using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "EOPMailUser", DefaultParameterSetName = "Identity")]
	public sealed class SetEOPMailUser : EOPTask
	{
		[Parameter(Mandatory = false)]
		public MailUserIdParameter Identity { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public string ExternalDirectoryObjectId { get; set; }

		[Parameter(Mandatory = false)]
		public string Alias { get; set; }

		[Parameter(Mandatory = false)]
		public string DisplayName { get; set; }

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection EmailAddresses { get; set; }

		[Parameter(Mandatory = false)]
		public SmtpAddress MicrosoftOnlineServicesID { get; set; }

		[Parameter(Mandatory = false)]
		public SecureString Password { get; set; }

		protected override void InternalProcessRecord()
		{
			try
			{
				SetMailUserCmdlet setMailUserCmdlet = new SetMailUserCmdlet();
				ADObjectId executingUserId;
				base.ExchangeRunspaceConfig.TryGetExecutingUserId(out executingUserId);
				setMailUserCmdlet.Authenticator = Authenticator.Create(base.CurrentOrganizationId, executingUserId);
				setMailUserCmdlet.HostServerName = EOPRecipient.GetPsWsHostServerName();
				if (string.IsNullOrEmpty(this.ExternalDirectoryObjectId) && this.Identity == null)
				{
					base.ThrowTaskError(new ArgumentException(CoreStrings.MissingIdentityParameter.ToString()));
				}
				EOPRecipient.SetProperty(setMailUserCmdlet, Parameters.Identity, string.IsNullOrEmpty(this.ExternalDirectoryObjectId) ? this.Identity.ToString() : this.ExternalDirectoryObjectId);
				EOPRecipient.SetProperty(setMailUserCmdlet, Parameters.Alias, this.Alias);
				EOPRecipient.SetProperty(setMailUserCmdlet, Parameters.DisplayName, this.DisplayName);
				EOPRecipient.SetProperty(setMailUserCmdlet, Parameters.EmailAddresses, this.EmailAddresses);
				EOPRecipient.SetProperty(setMailUserCmdlet, Parameters.MicrosoftOnlineServicesID, this.MicrosoftOnlineServicesID);
				EOPRecipient.SetProperty(setMailUserCmdlet, Parameters.Password, this.Password);
				EOPRecipient.SetProperty(setMailUserCmdlet, Parameters.Organization, base.Organization);
				setMailUserCmdlet.Run();
				EOPRecipient.CheckForError(this, setMailUserCmdlet);
			}
			catch (Exception e)
			{
				base.ThrowAndLogTaskError(e);
			}
		}
	}
}
