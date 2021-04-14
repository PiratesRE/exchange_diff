using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Update", "EOPDistributionGroupMember", DefaultParameterSetName = "Identity")]
	public sealed class UpdateEOPDistributionGroupMember : EOPTask
	{
		[Parameter(Mandatory = false)]
		public DistributionGroupIdParameter Identity { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public string ExternalDirectoryObjectId { get; set; }

		[Parameter(Mandatory = false)]
		public string[] Members { get; set; }

		protected override void InternalProcessRecord()
		{
			try
			{
				UpdateDistributionGroupMemberCmdlet updateDistributionGroupMemberCmdlet = new UpdateDistributionGroupMemberCmdlet();
				ADObjectId executingUserId;
				base.ExchangeRunspaceConfig.TryGetExecutingUserId(out executingUserId);
				updateDistributionGroupMemberCmdlet.Authenticator = Authenticator.Create(base.CurrentOrganizationId, executingUserId);
				updateDistributionGroupMemberCmdlet.HostServerName = EOPRecipient.GetPsWsHostServerName();
				if (string.IsNullOrEmpty(this.ExternalDirectoryObjectId) && this.Identity == null)
				{
					base.ThrowTaskError(new ArgumentException(CoreStrings.MissingIdentityParameter.ToString()));
				}
				EOPRecipient.SetProperty(updateDistributionGroupMemberCmdlet, Parameters.Identity, string.IsNullOrEmpty(this.ExternalDirectoryObjectId) ? this.Identity.ToString() : this.ExternalDirectoryObjectId);
				EOPRecipient.SetProperty(updateDistributionGroupMemberCmdlet, Parameters.Members, this.Members);
				EOPRecipient.SetProperty(updateDistributionGroupMemberCmdlet, Parameters.Organization, base.Organization);
				updateDistributionGroupMemberCmdlet.Run();
				EOPRecipient.CheckForError(this, updateDistributionGroupMemberCmdlet);
			}
			catch (Exception e)
			{
				base.ThrowAndLogTaskError(e);
			}
		}
	}
}
