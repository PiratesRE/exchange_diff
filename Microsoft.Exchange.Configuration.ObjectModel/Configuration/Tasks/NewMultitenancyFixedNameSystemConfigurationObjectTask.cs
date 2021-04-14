using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class NewMultitenancyFixedNameSystemConfigurationObjectTask<TDataObject> : NewFixedNameSystemConfigurationObjectTask<TDataObject> where TDataObject : ADObject, new()
	{
		[Parameter]
		[ValidateNotNullOrEmpty]
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

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.SharedTenantConfigurationMode != SharedTenantConfigurationMode.NotShared)
			{
				base.OrgWideSessionSettings.IsSharedConfigChecked = true;
			}
		}

		protected override void InternalValidate()
		{
			if (!this.IgnoreDehydratedFlag)
			{
				SharedConfigurationTaskHelper.Validate(this, this.SharedTenantConfigurationMode, base.CurrentOrgState, typeof(TDataObject).ToString());
			}
			base.InternalValidate();
		}

		protected override IConfigDataProvider CreateSession()
		{
			base.SessionSettings.IsSharedConfigChecked = true;
			return base.CreateSession();
		}

		protected override void InternalProcessRecord()
		{
			if (!this.IgnoreDehydratedFlag && SharedConfigurationTaskHelper.ShouldPrompt(this, this.SharedTenantConfigurationMode, base.CurrentOrgState) && !base.InternalForce)
			{
				TDataObject dataObject = this.DataObject;
				if (!base.ShouldContinue(Strings.ConfirmSharedConfiguration(dataObject.OrganizationId.OrganizationalUnit.Name)))
				{
					return;
				}
			}
			base.InternalProcessRecord();
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 1082, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\NewAdObjectTask.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ExchangeErrorCategory.Client);
				return adorganizationalUnit.OrganizationId;
			}
			return base.ResolveCurrentOrganization();
		}

		protected virtual SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		public virtual SwitchParameter IgnoreDehydratedFlag
		{
			get
			{
				return true;
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
