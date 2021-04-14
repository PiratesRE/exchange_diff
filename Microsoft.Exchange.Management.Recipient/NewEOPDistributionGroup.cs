using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "EOPDistributionGroup", DefaultParameterSetName = "Identity")]
	public sealed class NewEOPDistributionGroup : EOPTask
	{
		[Parameter(Mandatory = false)]
		public string Alias { get; set; }

		[Parameter(Mandatory = false)]
		public string DisplayName { get; set; }

		[Parameter(Mandatory = false)]
		public string[] ManagedBy { get; set; }

		[Parameter(Mandatory = false)]
		public string[] Members { get; set; }

		[Parameter(Mandatory = true)]
		public string Name { get; set; }

		[Parameter(Mandatory = false)]
		public string Notes { get; set; }

		[Parameter(Mandatory = false)]
		public SmtpAddress PrimarySmtpAddress { get; set; }

		[Parameter(Mandatory = false)]
		public GroupType Type { get; set; }

		protected override void InternalProcessRecord()
		{
			try
			{
				NewDistributionGroupCmdlet newDistributionGroupCmdlet = new NewDistributionGroupCmdlet();
				ADObjectId executingUserId;
				base.ExchangeRunspaceConfig.TryGetExecutingUserId(out executingUserId);
				newDistributionGroupCmdlet.Authenticator = Authenticator.Create(base.CurrentOrganizationId, executingUserId);
				newDistributionGroupCmdlet.HostServerName = EOPRecipient.GetPsWsHostServerName();
				EOPRecipient.SetProperty(newDistributionGroupCmdlet, Parameters.Name, this.Name);
				EOPRecipient.SetProperty(newDistributionGroupCmdlet, Parameters.DisplayName, this.DisplayName);
				EOPRecipient.SetProperty(newDistributionGroupCmdlet, Parameters.Alias, this.Alias);
				EOPRecipient.SetProperty(newDistributionGroupCmdlet, Parameters.PrimarySmtpAddress, this.PrimarySmtpAddress);
				EOPRecipient.SetProperty(newDistributionGroupCmdlet, Parameters.Notes, this.Notes);
				EOPRecipient.SetProperty(newDistributionGroupCmdlet, Parameters.ManagedByForInput, this.ManagedBy);
				EOPRecipient.SetProperty(newDistributionGroupCmdlet, Parameters.Members, this.Members);
				EOPRecipient.SetProperty(newDistributionGroupCmdlet, Parameters.Type, this.Type.ToString());
				EOPRecipient.SetProperty(newDistributionGroupCmdlet, Parameters.Organization, base.Organization);
				newDistributionGroupCmdlet.Run();
				EOPRecipient.CheckForError(this, newDistributionGroupCmdlet);
			}
			catch (Exception e)
			{
				base.ThrowAndLogTaskError(e);
			}
		}
	}
}
