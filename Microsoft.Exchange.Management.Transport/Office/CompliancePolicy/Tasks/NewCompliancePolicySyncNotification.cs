using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "CompliancePolicySyncNotification", SupportsShouldProcess = true)]
	public sealed class NewCompliancePolicySyncNotification : DataAccessTask<UnifiedPolicySyncNotification>
	{
		[Parameter(Mandatory = true)]
		public string Identity
		{
			get
			{
				return (string)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string SyncSvcUrl
		{
			get
			{
				return (string)base.Fields["SyncSvcUrl"];
			}
			set
			{
				base.Fields["SyncSvcUrl"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter FullSync
		{
			get
			{
				return (SwitchParameter)(base.Fields["FullSync"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["FullSync"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SyncNow
		{
			get
			{
				return (SwitchParameter)(base.Fields["SyncNow"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SyncNow"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> SyncChangeInfos
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["SyncChangeInfos"];
			}
			set
			{
				base.Fields["SyncChangeInfos"] = value;
			}
		}

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

		protected override void InternalValidate()
		{
			if (this.SyncChangeInfos != null)
			{
				try
				{
					foreach (string input in this.SyncChangeInfos)
					{
						SyncChangeInfo.Parse(input);
					}
				}
				catch (FormatException ex)
				{
					base.WriteError(new InvalidOperationException("Invalid format for SyncChangeInfo paramemter. The detail of the error is " + ex.Message), (ErrorCategory)1000, this.SyncChangeInfos);
				}
			}
			try
			{
				if (!string.IsNullOrEmpty(this.SyncSvcUrl))
				{
					new Uri(this.SyncSvcUrl, UriKind.Absolute);
				}
			}
			catch (FormatException ex2)
			{
				base.WriteError(new InvalidOperationException("Invalid format for SyncSvcUrl parameter. The detail of the error is " + ex2.Message), (ErrorCategory)1000, this.SyncChangeInfos);
			}
			base.InternalValidate();
		}

		protected override IConfigDataProvider CreateSession()
		{
			this.ResolveCurrentOrganization();
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 141, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\UnifiedPolicy\\Tasks\\NewCompliancePolicySyncNotification.cs");
		}

		protected override void InternalProcessRecord()
		{
			ADUser discoveryMailbox = MailboxDataProvider.GetDiscoveryMailbox((IRecipientSession)base.DataSession);
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(discoveryMailbox, null);
			List<SyncChangeInfo> list = null;
			if (this.SyncChangeInfos != null && this.SyncChangeInfos.Count > 0)
			{
				list = new List<SyncChangeInfo>();
				foreach (string input in this.SyncChangeInfos)
				{
					list.Add(new SyncChangeInfo(input));
				}
			}
			SyncNotificationResult syncNotificationResult = RpcClientWrapper.NotifySyncChanges(this.Identity, exchangePrincipal.MailboxInfo.Location.ServerFqdn, new Guid(this.organizationId.ToExternalDirectoryOrganizationId()), this.SyncSvcUrl, this.FullSync, this.SyncNow, (list != null) ? list.ToArray() : null);
			if (!syncNotificationResult.Success)
			{
				base.WriteError(syncNotificationResult.Error, ErrorCategory.WriteError, syncNotificationResult);
			}
			base.WriteObject((UnifiedPolicySyncNotification)syncNotificationResult.ResultObject);
		}

		private void ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 200, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\UnifiedPolicy\\Tasks\\NewCompliancePolicySyncNotification.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ExchangeErrorCategory.Client);
				this.organizationId = adorganizationalUnit.OrganizationId;
				return;
			}
			Utils.ValidateNotForestWideOrganization(base.CurrentOrganizationId);
			this.organizationId = base.CurrentOrganizationId;
		}

		private OrganizationId organizationId;
	}
}
