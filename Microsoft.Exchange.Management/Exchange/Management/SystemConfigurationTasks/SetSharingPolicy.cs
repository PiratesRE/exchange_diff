using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "SharingPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetSharingPolicy : SetSystemConfigurationObjectTask<SharingPolicyIdParameter, SharingPolicy>
	{
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SharingPolicyDomain> Domains
		{
			get
			{
				return this.domains;
			}
			set
			{
				this.domains = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return this.enabled != null && this.enabled.Value;
			}
			set
			{
				this.enabled = new bool?(value);
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Default
		{
			get
			{
				return this.isDefault;
			}
			set
			{
				this.isDefault = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetSharingPolicy(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			base.InternalValidate();
			if (this.isDefault)
			{
				this.ConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
				FederatedOrganizationId federatedOrganizationId = this.ConfigurationSession.GetFederatedOrganizationId(this.DataObject.OrganizationId);
				if (federatedOrganizationId != null)
				{
					this.defaultChanged = !this.DataObject.Id.Equals(federatedOrganizationId.DefaultSharingPolicyLink);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			if (this.isDefault)
			{
				SetSharingPolicy.SetDefaultSharingPolicy(this.ConfigurationSession, this.DataObject.OrganizationId, this.DataObject.Id);
			}
			if (((this.DataObject.Domains != null && this.DataObject.Domains.Count > 0) || this.enabled == true) && this.DataObject.IsAllowedForAnyAnonymousFeature() && !SetSharingPolicy.IsDatacenter)
			{
				this.WriteWarning(Strings.AnonymousSharingEnabledWarning);
			}
			TaskLogger.LogExit();
		}

		protected override bool IsObjectStateChanged()
		{
			return this.defaultChanged || base.IsObjectStateChanged();
		}

		protected override IConfigurable PrepareDataObject()
		{
			SharingPolicy sharingPolicy = (SharingPolicy)base.PrepareDataObject();
			if (this.domains != null)
			{
				if (sharingPolicy.Domains != null && this.domains.IsChangesOnlyCopy)
				{
					sharingPolicy.Domains.CopyChangesFrom(this.domains);
				}
				else
				{
					sharingPolicy.Domains = this.domains;
				}
			}
			if (this.enabled != null)
			{
				sharingPolicy.Enabled = this.enabled.Value;
			}
			return sharingPolicy;
		}

		internal static void SetDefaultSharingPolicy(IConfigurationSession session, OrganizationId organizationId, ADObjectId sharingPolicyId)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession;
			if (session.ConfigScope == ConfigScopes.TenantSubTree)
			{
				tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(session.DomainController, false, ConsistencyMode.FullyConsistent, session.NetworkCredential, session.SessionSettings, session.ConfigScope, 199, "SetDefaultSharingPolicy", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\SetSharingPolicy.cs");
			}
			else
			{
				tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(session.DomainController, false, ConsistencyMode.FullyConsistent, session.NetworkCredential, session.SessionSettings, 210, "SetDefaultSharingPolicy", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\SetSharingPolicy.cs");
			}
			tenantOrTopologyConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
			FederatedOrganizationId federatedOrganizationId = tenantOrTopologyConfigurationSession.GetFederatedOrganizationId(organizationId);
			if (federatedOrganizationId != null)
			{
				federatedOrganizationId.DefaultSharingPolicyLink = sharingPolicyId;
				tenantOrTopologyConfigurationSession.Save(federatedOrganizationId);
			}
		}

		internal static bool IsDatacenter
		{
			get
			{
				return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			}
		}

		private bool defaultChanged;

		private bool isDefault;

		private bool? enabled;

		private MultiValuedProperty<SharingPolicyDomain> domains;
	}
}
