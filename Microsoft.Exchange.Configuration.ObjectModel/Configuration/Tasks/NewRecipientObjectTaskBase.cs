using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class NewRecipientObjectTaskBase<TDataObject> : NewADTaskBase<TDataObject> where TDataObject : ADRecipient, new()
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

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 355, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\NewAdObjectTask.cs");
			tenantOrRootOrgRecipientSession.LinkResolutionServer = ADSession.GetCurrentConfigDC(base.SessionSettings.GetAccountOrResourceForestFqdn());
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = false;
			return tenantOrRootOrgRecipientSession;
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			IConfigurationSession session = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 387, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\NewAdObjectTask.cs");
			session.UseConfigNC = false;
			session.UseGlobalCatalog = (base.ServerSettings.ViewEntireForest && null == base.DomainController);
			if (this.Organization != null)
			{
				base.CurrentOrganizationId = base.ProvisioningCache.TryAddAndGetGlobalDictionaryValue<OrganizationId, string>(CannedProvisioningCacheKeys.OrganizationIdDictionary, this.Organization.RawIdentity, delegate()
				{
					ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)this.GetDataObject<ADOrganizationalUnit>(this.Organization, session, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ExchangeErrorCategory.Client);
					return adorganizationalUnit.OrganizationId;
				});
			}
			return base.CurrentOrganizationId;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			TDataObject tdataObject = (TDataObject)((object)base.PrepareDataObject());
			if (tdataObject.ObjectCategory == null && this.ConfigurationSession.SchemaNamingContext != null)
			{
				tdataObject.ObjectCategory = this.ConfigurationSession.SchemaNamingContext.GetChildId(tdataObject.ObjectCategoryCN);
			}
			this.PrepareRecipientObject(tdataObject);
			RecipientTaskHelper.RemoveEmptyValueFromEmailAddresses(tdataObject);
			TaskLogger.LogExit();
			return tdataObject;
		}

		protected abstract void PrepareRecipientObject(TDataObject dataObject);

		protected override void WriteResult(IConfigurable result)
		{
			this.WriteResult((ADObject)result);
		}

		protected virtual void WriteResult(ADObject result)
		{
			base.WriteResult(result);
		}
	}
}
