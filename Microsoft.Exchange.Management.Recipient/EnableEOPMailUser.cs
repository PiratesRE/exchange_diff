using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Enable", "EOPMailUser", DefaultParameterSetName = "Identity")]
	public sealed class EnableEOPMailUser : EOPTask
	{
		[Parameter(Mandatory = false)]
		public UserIdParameter Identity { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public string ExternalDirectoryObjectId { get; set; }

		protected override void InternalProcessRecord()
		{
			try
			{
				EnableMailUserCmdlet enableMailUserCmdlet = new EnableMailUserCmdlet();
				ADObjectId executingUserId;
				base.ExchangeRunspaceConfig.TryGetExecutingUserId(out executingUserId);
				enableMailUserCmdlet.Authenticator = Authenticator.Create(base.CurrentOrganizationId, executingUserId);
				enableMailUserCmdlet.HostServerName = EOPRecipient.GetPsWsHostServerName();
				if (string.IsNullOrEmpty(this.ExternalDirectoryObjectId) && this.Identity == null)
				{
					base.ThrowTaskError(new ArgumentException(CoreStrings.MissingIdentityParameter.ToString()));
				}
				EOPRecipient.SetProperty(enableMailUserCmdlet, Parameters.Identity, string.IsNullOrEmpty(this.ExternalDirectoryObjectId) ? this.Identity.ToString() : this.ExternalDirectoryObjectId);
				EOPRecipient.SetProperty(enableMailUserCmdlet, Parameters.Organization, base.Organization);
				enableMailUserCmdlet.Run();
				EOPRecipient.CheckForError(this, enableMailUserCmdlet);
			}
			catch (Exception e)
			{
				base.ThrowAndLogTaskError(e);
			}
		}
	}
}
