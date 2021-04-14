using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "EOPDistributionGroup", DefaultParameterSetName = "Identity")]
	public sealed class SetEOPDistributionGroup : EOPTask
	{
		[Parameter(Mandatory = false)]
		public DistributionGroupIdParameter Identity { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public string ExternalDirectoryObjectId { get; set; }

		[Parameter(Mandatory = false)]
		public string Alias { get; set; }

		[Parameter(Mandatory = false)]
		public string DisplayName { get; set; }

		[Parameter(Mandatory = false)]
		public SmtpAddress PrimarySmtpAddress { get; set; }

		[Parameter(Mandatory = false)]
		public string[] ManagedBy { get; set; }

		protected override void InternalProcessRecord()
		{
			try
			{
				SetDistributionGroupCmdlet setDistributionGroupCmdlet = new SetDistributionGroupCmdlet();
				ADObjectId executingUserId;
				base.ExchangeRunspaceConfig.TryGetExecutingUserId(out executingUserId);
				setDistributionGroupCmdlet.Authenticator = Authenticator.Create(base.CurrentOrganizationId, executingUserId);
				setDistributionGroupCmdlet.HostServerName = EOPRecipient.GetPsWsHostServerName();
				if (string.IsNullOrEmpty(this.ExternalDirectoryObjectId) && this.Identity == null)
				{
					base.ThrowTaskError(new ArgumentException(CoreStrings.MissingIdentityParameter.ToString()));
				}
				EOPRecipient.SetProperty(setDistributionGroupCmdlet, Parameters.Identity, string.IsNullOrEmpty(this.ExternalDirectoryObjectId) ? this.Identity.ToString() : this.ExternalDirectoryObjectId);
				EOPRecipient.SetProperty(setDistributionGroupCmdlet, Parameters.DisplayName, this.DisplayName);
				EOPRecipient.SetProperty(setDistributionGroupCmdlet, Parameters.Alias, this.Alias);
				EOPRecipient.SetProperty(setDistributionGroupCmdlet, Parameters.PrimarySmtpAddress, this.PrimarySmtpAddress);
				EOPRecipient.SetProperty(setDistributionGroupCmdlet, Parameters.ManagedBy, this.ManagedBy);
				EOPRecipient.SetProperty(setDistributionGroupCmdlet, Parameters.Organization, base.Organization);
				setDistributionGroupCmdlet.Run();
				EOPRecipient.CheckForError(this, setDistributionGroupCmdlet);
			}
			catch (Exception e)
			{
				base.ThrowAndLogTaskError(e);
			}
		}
	}
}
