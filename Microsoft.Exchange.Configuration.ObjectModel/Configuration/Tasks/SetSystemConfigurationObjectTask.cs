using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetSystemConfigurationObjectTask<TIdentity, TPublicObject, TDataObject> : SetADTaskBase<TIdentity, TPublicObject, TDataObject> where TIdentity : IIdentityParameter, new() where TPublicObject : IConfigurable, new() where TDataObject : ADObject, new()
	{
		internal LazilyInitialized<SharedTenantConfigurationState> CurrentOrgState { get; set; }

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.CurrentOrgState = new LazilyInitialized<SharedTenantConfigurationState>(() => SharedConfiguration.GetSharedConfigurationState(base.CurrentOrganizationId));
		}

		protected override void InternalValidate()
		{
			SharedTenantConfigurationMode sharedTenantConfigurationMode = this.SharedTenantConfigurationMode;
			LazilyInitialized<SharedTenantConfigurationState> currentOrgState = this.CurrentOrgState;
			string targetObject;
			if (this.Identity == null)
			{
				targetObject = null;
			}
			else
			{
				TIdentity identity = this.Identity;
				targetObject = identity.ToString();
			}
			SharedConfigurationTaskHelper.Validate(this, sharedTenantConfigurationMode, currentOrgState, targetObject);
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			OrganizationId currentOrganizationId = base.CurrentOrganizationId;
			TDataObject dataObject = this.DataObject;
			if (!currentOrganizationId.Equals(dataObject.OrganizationId))
			{
				this.CurrentOrgState = new LazilyInitialized<SharedTenantConfigurationState>(delegate()
				{
					TDataObject dataObject8 = this.DataObject;
					return SharedConfiguration.GetSharedConfigurationState(dataObject8.OrganizationId);
				});
			}
			if (SharedConfigurationTaskHelper.ShouldPrompt(this, this.SharedTenantConfigurationMode, this.CurrentOrgState) && !base.InternalForce)
			{
				TDataObject dataObject2 = this.DataObject;
				if (!base.ShouldContinue(Strings.ConfirmSharedConfiguration(dataObject2.OrganizationId.OrganizationalUnit.Name)))
				{
					TaskLogger.LogExit();
					return;
				}
			}
			TDataObject dataObject3 = this.DataObject;
			if (dataObject3.IsChanged(ADObjectSchema.Id))
			{
				IDirectorySession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.OrgWideSessionSettings, ConfigScopes.TenantSubTree, 702, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\SetAdObjectTask.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = ((IDirectorySession)base.DataSession).UseConfigNC;
				TDataObject dataObject4 = this.DataObject;
				ADObjectId parent = dataObject4.Id.Parent;
				ADRawEntry adrawEntry = tenantOrTopologyConfigurationSession.ReadADRawEntry(parent, new PropertyDefinition[]
				{
					ADObjectSchema.ExchangeVersion,
					ADObjectSchema.ObjectClass
				});
				ExchangeObjectVersion exchangeObjectVersion = (ExchangeObjectVersion)adrawEntry[ADObjectSchema.ExchangeVersion];
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)adrawEntry[ADObjectSchema.ObjectClass];
				TDataObject dataObject5 = this.DataObject;
				if (dataObject5.ExchangeVersion.IsOlderThan(exchangeObjectVersion) && !multiValuedProperty.Contains(Organization.MostDerivedClass))
				{
					TDataObject dataObject6 = this.DataObject;
					string name = dataObject6.Name;
					TDataObject dataObject7 = this.DataObject;
					base.WriteError(new TaskException(Strings.ErrorParentHasNewerVersion(name, dataObject7.ExchangeVersion.ToString(), exchangeObjectVersion.ToString())), (ErrorCategory)1004, null);
				}
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 750, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\SetAdObjectTask.cs");
		}

		protected override IConfigurable ResolveDataObject()
		{
			SharedConfigurationTaskHelper.Validate(this, this.SharedTenantConfigurationMode, this.CurrentOrgState, null);
			ADObject adobject = (ADObject)base.ResolveDataObject();
			if (TaskHelper.ShouldUnderscopeDataSessionToOrganization((IDirectorySession)base.DataSession, adobject))
			{
				base.UnderscopeDataSession(adobject.OrganizationId);
			}
			return adobject;
		}

		protected virtual SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.NotShared;
			}
		}
	}
}
