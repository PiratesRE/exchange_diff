using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class RemoveSystemConfigurationObjectTask<TIdentity, TDataObject> : RemoveADTaskBase<TIdentity, TDataObject> where TIdentity : IIdentityParameter where TDataObject : ADObject, new()
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
			TIdentity identity = this.Identity;
			SharedConfigurationTaskHelper.Validate(this, sharedTenantConfigurationMode, currentOrgState, identity.ToString());
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			LazilyInitialized<SharedTenantConfigurationState> currentOrgState = this.CurrentOrgState;
			OrganizationId currentOrganizationId = base.CurrentOrganizationId;
			TDataObject dataObject = base.DataObject;
			if (!currentOrganizationId.Equals(dataObject.OrganizationId))
			{
				currentOrgState = new LazilyInitialized<SharedTenantConfigurationState>(delegate()
				{
					TDataObject dataObject3 = base.DataObject;
					return SharedConfiguration.GetSharedConfigurationState(dataObject3.OrganizationId);
				});
			}
			if (SharedConfigurationTaskHelper.ShouldPrompt(this, this.SharedTenantConfigurationMode, currentOrgState) && !base.InternalForce)
			{
				TDataObject dataObject2 = base.DataObject;
				if (!base.ShouldContinue(Strings.ConfirmSharedConfiguration(dataObject2.OrganizationId.OrganizationalUnit.Name)))
				{
					TaskLogger.LogExit();
					return;
				}
			}
			base.InternalProcessRecord();
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 503, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\RemoveAdObjectTask.cs");
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADObject adobject = (ADObject)base.ResolveDataObject();
			if (TaskHelper.ShouldUnderscopeDataSessionToOrganization((IDirectorySession)base.DataSession, adobject))
			{
				base.UnderscopeDataSession(adobject.OrganizationId);
				base.CurrentOrganizationId = adobject.OrganizationId;
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
