using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class InstallAddressBookContainer : NewMultitenancyFixedNameSystemConfigurationObjectTask<AddressBookBase>
	{
		protected bool IsContainerExisted
		{
			get
			{
				return this.isContainerExisted;
			}
		}

		protected abstract ADObjectId RdnContainerToOrganization { get; }

		protected sealed override IConfigurable PrepareDataObject()
		{
			ADObjectId descendantId = base.CurrentOrgContainerId.GetDescendantId(this.RdnContainerToOrganization);
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			AddressBookBase addressBookBase = configurationSession.Read<AddressBookBase>(descendantId);
			this.isContainerExisted = (null != addressBookBase);
			if (!this.IsContainerExisted)
			{
				addressBookBase = (AddressBookBase)base.PrepareDataObject();
				addressBookBase.SetId(descendantId);
				addressBookBase.DisplayName = this.RdnContainerToOrganization.Name;
				addressBookBase.OrganizationId = (base.CurrentOrganizationId ?? OrganizationId.ForestWideOrgId);
			}
			return addressBookBase;
		}

		protected override void InternalProcessRecord()
		{
			if (!this.IsContainerExisted)
			{
				base.InternalProcessRecord();
			}
		}

		internal IConfigurationSession CreateGlobalWritableConfigSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DataObject.OriginatingServer, false, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(this.DataObject.Id.GetPartitionId()), 90, "CreateGlobalWritableConfigSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\InstallAddressBookContainer.cs");
		}

		private bool isContainerExisted;
	}
}
