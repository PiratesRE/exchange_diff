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
	[Cmdlet("Set", "RecipientTemplateProvisioningPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetRecipientTemplateProvisioningPolicy : SetProvisioningPolicyTaskBase<ProvisioningPolicyIdParameter, RecipientTemplateProvisioningPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetRecipientTemplateProvisioningPolicy(this.DataObject.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationalUnitIdParameter DefaultDistributionListOU
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields["DefaultDistributionListOU"];
			}
			set
			{
				base.Fields["DefaultDistributionListOU"] = value;
			}
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			if (this.DefaultDistributionListOU != null)
			{
				this.defaultOU = RecipientTaskHelper.ResolveOrganizationalUnitInOrganization(this.DefaultDistributionListOU, this.ConfigurationSession, null, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ExchangeOrganizationalUnit>), ExchangeErrorCategory.Client, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
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
			ExchangeOrganizationalUnit exchangeOrganizationalUnit = null;
			if (base.Fields.IsModified("DefaultDistributionListOU"))
			{
				this.DataObject.DefaultDistributionListOU = ((this.defaultOU == null) ? null : this.defaultOU.Id);
				exchangeOrganizationalUnit = this.defaultOU;
			}
			else if (this.DataObject.IsChanged(RecipientTemplateProvisioningPolicySchema.DefaultDistributionListOU) && this.DataObject.DefaultDistributionListOU != null)
			{
				exchangeOrganizationalUnit = RecipientTaskHelper.ResolveOrganizationalUnitInOrganization(new OrganizationalUnitIdParameter(this.DataObject.DefaultDistributionListOU), this.ConfigurationSession, null, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ExchangeOrganizationalUnit>), ExchangeErrorCategory.Client, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (exchangeOrganizationalUnit != null)
			{
				OrganizationId organizationId = OrganizationId.ForestWideOrgId;
				if (this.ConfigurationSession is ITenantConfigurationSession)
				{
					organizationId = TaskHelper.ResolveOrganizationId(this.DataObject.Id, ADProvisioningPolicy.RdnContainer, (ITenantConfigurationSession)this.ConfigurationSession);
				}
				RecipientTaskHelper.IsOrgnizationalUnitInOrganization(this.ConfigurationSession, organizationId, exchangeOrganizationalUnit, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
			}
			TaskLogger.LogExit();
		}

		private ExchangeOrganizationalUnit defaultOU;
	}
}
