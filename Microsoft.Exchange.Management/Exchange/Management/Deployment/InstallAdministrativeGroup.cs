using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Install", "AdministrativeGroup")]
	public sealed class InstallAdministrativeGroup : NewFixedNameSystemConfigurationObjectTask<AdministrativeGroup>
	{
		protected override IConfigurable PrepareDataObject()
		{
			ADObjectId orgContainerId = ((IConfigurationSession)base.DataSession).GetOrgContainerId();
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, AdministrativeGroup.DefaultName);
			AdministrativeGroup[] array = ((IConfigurationSession)base.DataSession).Find<AdministrativeGroup>(null, QueryScope.SubTree, filter, null, 1);
			AdministrativeGroup administrativeGroup;
			if (array != null && array.Length > 0)
			{
				administrativeGroup = array[0];
				if (string.IsNullOrEmpty(administrativeGroup.LegacyExchangeDN))
				{
					administrativeGroup.StampLegacyExchangeDN(((IConfigurationSession)base.DataSession).GetOrgContainer().LegacyExchangeDN, administrativeGroup.Name);
				}
			}
			else
			{
				administrativeGroup = (AdministrativeGroup)base.PrepareDataObject();
				administrativeGroup.SetId(orgContainerId.GetChildId("Administrative Groups").GetChildId(AdministrativeGroup.DefaultName));
				administrativeGroup.StampLegacyExchangeDN(((IConfigurationSession)base.DataSession).GetOrgContainer().LegacyExchangeDN, AdministrativeGroup.DefaultName);
				filter = new ComparisonFilter(ComparisonOperator.Equal, AdministrativeGroupSchema.DefaultAdminGroup, true);
				AdministrativeGroup[] array2 = ((IConfigurationSession)base.DataSession).Find<AdministrativeGroup>(null, QueryScope.SubTree, filter, null, 0);
				administrativeGroup.DefaultAdminGroup = (array2 == null || array2.Length == 0);
			}
			return administrativeGroup;
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, PublicFolderTreeSchema.PublicFolderTreeType, PublicFolderTreeType.Mapi);
			PublicFolderTree[] array = ((IConfigurationSession)base.DataSession).Find<PublicFolderTree>(null, QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				PublicFolderTreeContainer publicFolderTreeContainer = this.InstallConfigurationObject<PublicFolderTreeContainer>(this.DataObject.Id.GetChildId(PublicFolderTreeContainer.DefaultName), false);
				this.InstallConfigurationObject<PublicFolderTree>(publicFolderTreeContainer.Id.GetChildId(PublicFolderTree.DefaultName), true);
			}
			this.InstallConfigurationObject<ServersContainer>(this.DataObject.Id.GetChildId(ServersContainer.DefaultName), false);
			this.InstallConfigurationObject<Container>(ClientAccessArray.GetParentContainer((ITopologyConfigurationSession)base.DataSession), false);
			this.InstallConfigurationObject<DatabasesContainer>(this.DataObject.Id.GetChildId(DatabasesContainer.DefaultName), false);
			this.InstallConfigurationObject<DatabaseAvailabilityGroupContainer>(this.DataObject.Id.GetChildId(DatabaseAvailabilityGroupContainer.DefaultName), false);
			AdvancedSecurityContainer advancedSecurityContainer = this.InstallConfigurationObject<AdvancedSecurityContainer>(this.DataObject.Id.GetChildId("Advanced Security"), false);
			Encryption encryption = new Encryption();
			encryption[EncryptionSchema.LegacyExchangeDN] = string.Format("{0}/cn=Configuration/cn={1}", this.DataObject.LegacyExchangeDN, Encryption.DefaultName);
			this.InstallConfigurationObject<Encryption>(advancedSecurityContainer.Id.GetChildId(Encryption.DefaultName), false, encryption);
			this.InstallConfigurationObject<LegacyGwart>(this.DataObject.Id.GetChildId(LegacyGwart.DefaultName), false);
			this.InstallConfigurationObject<RoutingGroupsContainer>(this.DataObject.Id.GetChildId(RoutingGroupsContainer.DefaultName), false);
			this.InstallConfigurationObject<RoutingGroup>(this.DataObject.Id.GetChildId(RoutingGroupsContainer.DefaultName).GetChildId(RoutingGroup.DefaultName), false);
			this.InstallConfigurationObject<ConnectorsContainer>(this.DataObject.Id.GetChildId(RoutingGroupsContainer.DefaultName).GetChildId(RoutingGroup.DefaultName).GetChildId("Connections"), false);
		}

		private TObject InstallConfigurationObject<TObject>(ADObjectId id, bool force) where TObject : ADConfigurationObject, new()
		{
			return this.InstallConfigurationObject<TObject>(id, force, default(TObject));
		}

		private TObject InstallConfigurationObject<TObject>(ADObjectId id, bool force, TObject instance) where TObject : ADConfigurationObject, new()
		{
			IEnumerator<TObject> enumerator = null;
			if (!force)
			{
				IEnumerable<TObject> enumerable = base.DataSession.FindPaged<TObject>(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, id), null, true, null, 0);
				enumerator = enumerable.GetEnumerator();
			}
			if (enumerator != null && enumerator.MoveNext())
			{
				return enumerator.Current;
			}
			TObject tobject = instance;
			if (instance == null)
			{
				tobject = Activator.CreateInstance<TObject>();
			}
			TObject tobject2 = tobject;
			tobject2.SetId(id);
			((IConfigurationSession)base.DataSession).Save(tobject2);
			return tobject2;
		}
	}
}
