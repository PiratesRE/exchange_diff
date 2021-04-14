using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "DefaultSharingPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class InstallDefaultSharingPolicy : NewMultitenancySystemConfigurationObjectTask<SharingPolicy>
	{
		private new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.ConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
			FederatedOrganizationId federatedOrganizationId = this.ConfigurationSession.GetFederatedOrganizationId();
			string organization = (federatedOrganizationId.OrganizationId == null) ? "<No Org>" : federatedOrganizationId.OrganizationId.ToString();
			string cn;
			if (federatedOrganizationId.DefaultSharingPolicyLink != null)
			{
				this.skipProcessRecord = true;
				base.WriteVerbose(Strings.FoundDefaultSharingPolicy(federatedOrganizationId.DefaultSharingPolicyLink.DistinguishedName, organization));
				cn = Guid.NewGuid().ToString();
			}
			else
			{
				cn = this.GetDefaultSharingPolicyName();
				base.WriteVerbose(Strings.InstallDefaultSharingPolicy(organization));
			}
			SharingPolicy sharingPolicy = (SharingPolicy)base.PrepareDataObject();
			sharingPolicy.SetId(this.ConfigurationSession, cn);
			sharingPolicy.Enabled = true;
			sharingPolicy.Domains = new MultiValuedProperty<SharingPolicyDomain>(new SharingPolicyDomain[]
			{
				new SharingPolicyDomain("*", SharingPolicyAction.CalendarSharingFreeBusySimple)
			});
			sharingPolicy.Domains.Add(new SharingPolicyDomain("Anonymous", SharingPolicyAction.CalendarSharingFreeBusyReviewer));
			TaskLogger.LogExit();
			return sharingPolicy;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.skipProcessRecord)
			{
				base.InternalProcessRecord();
				SetSharingPolicy.SetDefaultSharingPolicy(this.ConfigurationSession, this.DataObject.OrganizationId, this.DataObject.Id);
			}
			TaskLogger.LogExit();
		}

		private string GetDefaultSharingPolicyName()
		{
			string text = Strings.DefaultSharingPolicyName.ToString();
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, text);
			SharingPolicy[] array = this.ConfigurationSession.Find<SharingPolicy>(null, QueryScope.SubTree, filter, null, 1);
			if (array != null && array.Length > 0)
			{
				return text + "_" + Guid.NewGuid().ToString();
			}
			return text;
		}

		private bool skipProcessRecord;
	}
}
