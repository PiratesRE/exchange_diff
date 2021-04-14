using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ProvisioningTasks
{
	[Cmdlet("New", "RecipientTemplateProvisioningPolicy", SupportsShouldProcess = true)]
	public sealed class NewRecipientTemplateProvisioningPolicy : NewTemplateProvisioningPolicyTaskBase<RecipientTemplateProvisioningPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewRecipientTemplateProvisioningPolicy(this.DataObject.Name.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.DataObject.DefaultDistributionListOU != null)
			{
				RecipientTaskHelper.ResolveOrganizationalUnitInOrganization(new OrganizationalUnitIdParameter(this.DataObject.DefaultDistributionListOU), (IConfigurationSession)base.DataSession, (base.CurrentOrganizationId != null) ? base.CurrentOrganizationId : OrganizationId.ForestWideOrgId, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ExchangeOrganizationalUnit>), ExchangeErrorCategory.Client, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
			}
			TaskLogger.LogExit();
		}
	}
}
