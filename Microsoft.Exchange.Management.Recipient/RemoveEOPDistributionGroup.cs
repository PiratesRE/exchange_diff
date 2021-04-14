using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "EOPDistributionGroup", DefaultParameterSetName = "Identity")]
	public sealed class RemoveEOPDistributionGroup : EOPTask
	{
		[Parameter(Mandatory = false)]
		public DistributionGroupIdParameter Identity { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public string ExternalDirectoryObjectId { get; set; }

		protected override void InternalProcessRecord()
		{
			try
			{
				RemoveDistributionGroupCmdlet removeDistributionGroupCmdlet = new RemoveDistributionGroupCmdlet();
				ADObjectId executingUserId;
				base.ExchangeRunspaceConfig.TryGetExecutingUserId(out executingUserId);
				removeDistributionGroupCmdlet.Authenticator = Authenticator.Create(base.CurrentOrganizationId, executingUserId);
				removeDistributionGroupCmdlet.HostServerName = EOPRecipient.GetPsWsHostServerName();
				if (string.IsNullOrEmpty(this.ExternalDirectoryObjectId) && this.Identity == null)
				{
					base.ThrowTaskError(new ArgumentException(CoreStrings.MissingIdentityParameter.ToString()));
				}
				EOPRecipient.SetProperty(removeDistributionGroupCmdlet, Parameters.Identity, string.IsNullOrEmpty(this.ExternalDirectoryObjectId) ? this.Identity.ToString() : this.ExternalDirectoryObjectId);
				EOPRecipient.SetProperty(removeDistributionGroupCmdlet, Parameters.Organization, base.Organization);
				removeDistributionGroupCmdlet.Run();
				EOPRecipient.CheckForError(this, removeDistributionGroupCmdlet);
			}
			catch (Exception e)
			{
				base.ThrowAndLogTaskError(e);
			}
		}
	}
}
