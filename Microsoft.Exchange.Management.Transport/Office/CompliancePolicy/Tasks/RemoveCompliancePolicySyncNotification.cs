using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Remove", "CompliancePolicySyncNotification")]
	public sealed class RemoveCompliancePolicySyncNotification : RemoveTenantADTaskBase<UnifiedPolicySyncNotificationIdParameter, UnifiedPolicySyncNotification>
	{
		[Parameter(Mandatory = false)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			this.ResolveCurrentOrganization();
			ADUser discoveryMailbox = MailboxDataProvider.GetDiscoveryMailbox(base.TenantGlobalCatalogSession);
			return new UnifiedPolicySyncNotificationDataProvider(base.SessionSettings, discoveryMailbox, "Remove-CompliancePolicySyncNotification");
		}

		protected override void InternalStateReset()
		{
			this.DisposeDataSession();
			base.InternalStateReset();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.DisposeDataSession();
			GC.SuppressFinalize(this);
		}

		private void ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 90, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\UnifiedPolicy\\Tasks\\RemoveCompliancePolicySyncNotification.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ExchangeErrorCategory.Client);
				base.CurrentOrganizationId = adorganizationalUnit.OrganizationId;
			}
			Utils.ValidateNotForestWideOrganization(base.CurrentOrganizationId);
		}

		private void DisposeDataSession()
		{
			UnifiedPolicySyncNotificationDataProvider unifiedPolicySyncNotificationDataProvider = (UnifiedPolicySyncNotificationDataProvider)base.DataSession;
			if (unifiedPolicySyncNotificationDataProvider != null)
			{
				unifiedPolicySyncNotificationDataProvider.Dispose();
			}
		}
	}
}
