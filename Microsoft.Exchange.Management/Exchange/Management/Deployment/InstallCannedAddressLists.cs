using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Install", "CannedAddressLists")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class InstallCannedAddressLists : InstallAddressListBase
	{
		[Parameter]
		public SwitchParameter CreateModernGroupsAddressList { get; set; }

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (!base.IsContainerExisted)
			{
				base.PostExchange(this.DataObject.Id);
			}
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			Organization organization = (base.Organization != null) ? configurationSession.Read<ExchangeConfigurationUnit>(base.CurrentOrgContainerId) : configurationSession.Read<Organization>(configurationSession.GetOrgContainerId());
			if (organization.OrganizationId == OrganizationId.ForestWideOrgId || VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.InstallModernGroupsAddressList.Enabled)
			{
				this.RenameAllGroupsAddressListToAllDistributionLists();
				this.UpdateTheAllUsersAddressList();
			}
			foreach (KeyValuePair<string, QueryFilter> keyValuePair in CannedAddressListsFilterHelper.RecipientFiltersOfAddressList)
			{
				this.CreateCannedAddressList(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private void CreateCannedAddressList(string name, QueryFilter recipientFilter)
		{
			bool flag = string.Equals(name, CannedAddressListsFilterHelper.DefaultAllModernGroups, StringComparison.InvariantCulture);
			if (flag && (!this.CreateModernGroupsAddressList.IsPresent || !VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.InstallModernGroupsAddressList.Enabled))
			{
				return;
			}
			ADObjectId id = this.DataObject.Id;
			AddressBookBase[] array = this.FindAddressList(name, recipientFilter);
			if (array == null || array.Length == 0)
			{
				AddressBookBase addressBookBase = new AddressBookBase();
				string text = name;
				if (flag)
				{
					text = Strings.DefaultAllGroups;
					addressBookBase.IsModernGroupsAddressList = true;
				}
				ADObjectId childId = id.GetChildId(text);
				addressBookBase.SetId(childId);
				addressBookBase.DisplayName = text;
				addressBookBase.SetRecipientFilter(recipientFilter);
				RecipientFilterHelper.StampE2003FilterMetadata(addressBookBase, addressBookBase.LdapRecipientFilter, AddressBookBaseSchema.PurportedSearchUI);
				addressBookBase.OrganizationId = (base.CurrentOrganizationId ?? OrganizationId.ForestWideOrgId);
				base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(addressBookBase, base.DataSession, typeof(AddressBookBase)));
				try
				{
					base.DataSession.Save(addressBookBase);
					if (string.Equals(name, CannedAddressListsFilterHelper.DefaultAllRooms, StringComparison.InvariantCulture))
					{
						this.PostOrganization(childId);
					}
				}
				catch (ADObjectAlreadyExistsException)
				{
					base.WriteVerbose(Strings.VerboseCannedAddressListAlreadyExists(name));
				}
				finally
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
				}
				base.WriteObject(addressBookBase);
			}
		}

		private void UpdateTheAllUsersAddressList()
		{
			AddressBookBase[] array = this.FindAddressList(CannedAddressListsFilterHelper.DefaultAllUsers, CannedAddressListsFilterHelper.DefaultAllUsersFilter);
			if (array == null || array.Length == 0)
			{
				return;
			}
			AddressBookBase addressBookBase = array[0];
			if (!object.Equals(addressBookBase.RecipientFilter, CannedAddressListsFilterHelper.DefaultAllUsersFilter))
			{
				addressBookBase.SetRecipientFilter(CannedAddressListsFilterHelper.DefaultAllUsersFilter);
				try
				{
					base.DataSession.Save(addressBookBase);
				}
				finally
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
				}
			}
		}

		private void RenameAllGroupsAddressListToAllDistributionLists()
		{
			AddressBookBase[] array = this.FindAddressList(CannedAddressListsFilterHelper.DefaultAllDistributionLists, CannedAddressListsFilterHelper.DefaultAllDistributionListsFilter);
			if (array == null || array.Length == 0)
			{
				return;
			}
			AddressBookBase addressBookBase = array[0];
			bool flag = false;
			if (string.Equals(addressBookBase.DisplayName, Strings.DefaultAllGroups, StringComparison.CurrentCultureIgnoreCase))
			{
				addressBookBase.DisplayName = Strings.DefaultAllDistributionLists;
				flag = true;
			}
			if (string.Equals(addressBookBase.Name, Strings.DefaultAllGroups, StringComparison.CurrentCultureIgnoreCase))
			{
				addressBookBase.Name = Strings.DefaultAllDistributionLists;
				flag = true;
			}
			if (flag)
			{
				try
				{
					base.DataSession.Save(addressBookBase);
				}
				finally
				{
					base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
				}
			}
		}

		private AddressBookBase[] FindAddressList(string name, QueryFilter recipientFilter)
		{
			ADObjectId id = this.DataObject.Id;
			QueryFilter findFilterForCannedAddressLists = CannedAddressListsFilterHelper.GetFindFilterForCannedAddressLists(name, recipientFilter);
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(configurationSession, typeof(AddressBookBase), findFilterForCannedAddressLists, id, false));
			AddressBookBase[] result;
			try
			{
				AddressBookBase[] array = configurationSession.Find<AddressBookBase>(id, QueryScope.SubTree, findFilterForCannedAddressLists, null, 1);
				result = array;
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(configurationSession));
			}
			return result;
		}

		private void PostOrganization(ADObjectId resAl)
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			Organization organization = (base.Organization != null) ? configurationSession.Read<ExchangeConfigurationUnit>(base.CurrentOrgContainerId) : configurationSession.Read<Organization>(configurationSession.GetOrgContainerId());
			ADObjectId deletedObjectsContainer = configurationSession.DeletedObjectsContainer;
			List<ADObjectId> list = new List<ADObjectId>();
			foreach (ADObjectId adobjectId in organization.ResourceAddressLists)
			{
				if (adobjectId.Parent.Equals(deletedObjectsContainer))
				{
					list.Add(adobjectId);
				}
			}
			foreach (ADObjectId item in list)
			{
				organization.ResourceAddressLists.Remove(item);
			}
			organization.ResourceAddressLists.Add(resAl);
			base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(organization, configurationSession, typeof(Organization)));
			try
			{
				configurationSession.Save(organization);
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(configurationSession));
			}
		}
	}
}
