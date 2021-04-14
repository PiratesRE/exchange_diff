using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "EOPGroup", DefaultParameterSetName = "Identity")]
	public sealed class SetEOPGroup : EOPTask
	{
		[Parameter(Mandatory = false)]
		public GroupIdParameter Identity { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public string ExternalDirectoryObjectId { get; set; }

		[Parameter(Mandatory = false)]
		public string[] ManagedBy { get; set; }

		[Parameter(Mandatory = false)]
		public string Notes { get; set; }

		protected override void InternalProcessRecord()
		{
			try
			{
				SetGroupCmdlet setGroupCmdlet = new SetGroupCmdlet();
				ADObjectId executingUserId;
				base.ExchangeRunspaceConfig.TryGetExecutingUserId(out executingUserId);
				setGroupCmdlet.Authenticator = Authenticator.Create(base.CurrentOrganizationId, executingUserId);
				setGroupCmdlet.HostServerName = EOPRecipient.GetPsWsHostServerName();
				if (string.IsNullOrEmpty(this.ExternalDirectoryObjectId) && this.Identity == null)
				{
					base.ThrowTaskError(new ArgumentException(CoreStrings.MissingIdentityParameter.ToString()));
				}
				EOPRecipient.SetProperty(setGroupCmdlet, Parameters.Identity, string.IsNullOrEmpty(this.ExternalDirectoryObjectId) ? this.Identity.ToString() : this.ExternalDirectoryObjectId);
				EOPRecipient.SetProperty(setGroupCmdlet, Parameters.Notes, this.Notes);
				EOPRecipient.SetProperty(setGroupCmdlet, Parameters.ManagedBy, this.ManagedBy);
				EOPRecipient.SetProperty(setGroupCmdlet, Parameters.Organization, base.Organization);
				setGroupCmdlet.Run();
				EOPRecipient.CheckForError(this, setGroupCmdlet);
			}
			catch (Exception e)
			{
				base.ThrowAndLogTaskError(e);
			}
		}
	}
}
