using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "AddressBookPolicy", SupportsShouldProcess = true)]
	public sealed class NewAddressBookPolicy : NewMailboxPolicyBase<AddressBookMailboxPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewAddressBookPolicy(base.Name.ToString(), base.FormatMultiValuedProperty(this.AddressLists), this.GlobalAddressList.ToString(), this.RoomList.ToString(), this.OfflineAddressBook.ToString());
			}
		}

		private int MaxAddressBookPolicies
		{
			get
			{
				if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.MaxAddressBookPolicies.Enabled)
				{
					return int.MaxValue;
				}
				int? maxAddressBookPolicies = this.ConfigurationSession.GetOrgContainer().MaxAddressBookPolicies;
				if (maxAddressBookPolicies == null)
				{
					return 250;
				}
				return maxAddressBookPolicies.GetValueOrDefault();
			}
		}

		[Parameter(Mandatory = true)]
		public AddressListIdParameter[] AddressLists
		{
			get
			{
				return (AddressListIdParameter[])base.Fields[AddressBookMailboxPolicySchema.AddressLists];
			}
			set
			{
				base.Fields[AddressBookMailboxPolicySchema.AddressLists] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public GlobalAddressListIdParameter GlobalAddressList
		{
			get
			{
				return (GlobalAddressListIdParameter)base.Fields[AddressBookMailboxPolicySchema.GlobalAddressList];
			}
			set
			{
				base.Fields[AddressBookMailboxPolicySchema.GlobalAddressList] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public AddressListIdParameter RoomList
		{
			get
			{
				return (AddressListIdParameter)base.Fields[AddressBookMailboxPolicySchema.RoomList];
			}
			set
			{
				base.Fields[AddressBookMailboxPolicySchema.RoomList] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public OfflineAddressBookIdParameter OfflineAddressBook
		{
			get
			{
				return (OfflineAddressBookIdParameter)base.Fields[AddressBookMailboxPolicySchema.OfflineAddressBook];
			}
			set
			{
				base.Fields[AddressBookMailboxPolicySchema.OfflineAddressBook] = value;
			}
		}

		protected override void InternalValidate()
		{
			this.DataObject = (AddressBookMailboxPolicy)base.PrepareDataObject();
			this.DataObject.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
			this.DataObject.MinAdminVersion = new int?(this.DataObject.ExchangeVersion.ExchangeBuild.ToExchange2003FormatInt32());
			int maxAddressBookPolicies = this.MaxAddressBookPolicies;
			if (maxAddressBookPolicies < 2147483647)
			{
				IEnumerable<AddressBookMailboxPolicy> enumerable = base.DataSession.FindPaged<AddressBookMailboxPolicy>(null, ((IConfigurationSession)base.DataSession).GetOrgContainerId().GetDescendantId(this.DataObject.ParentPath), false, null, ADGenericPagedReader<AddressBookMailboxPolicy>.DefaultPageSize);
				int num = 0;
				foreach (AddressBookMailboxPolicy addressBookMailboxPolicy in enumerable)
				{
					num++;
					if (num >= maxAddressBookPolicies)
					{
						base.WriteError(new ManagementObjectAlreadyExistsException(Strings.ErrorTooManyItems(maxAddressBookPolicies)), ErrorCategory.LimitsExceeded, base.Name);
						break;
					}
				}
			}
			this.DataObject.AddressLists = AddressBookPolicyTaskUtility.ValidateAddressBook(base.DataSession, this.AddressLists, new AddressBookPolicyTaskUtility.GetUniqueObject(base.GetDataObject<AddressBookBase>), this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			AddressBookBase addressBookBase = (AddressBookBase)base.GetDataObject<AddressBookBase>(this.GlobalAddressList, base.DataSession, null, new LocalizedString?(Strings.ErrorAddressListOrGlobalAddressListNotFound(this.GlobalAddressList.ToString())), new LocalizedString?(Strings.ErrorAddressListOrGlobalAddressListNotUnique(this.GlobalAddressList.ToString())));
			if (addressBookBase != null && addressBookBase.IsGlobalAddressList)
			{
				this.DataObject.GlobalAddressList = (ADObjectId)addressBookBase.Identity;
			}
			else
			{
				base.WriteError(new ArgumentException(Strings.ErrorGlobalAddressListNotFound(this.GlobalAddressList.ToString())), ErrorCategory.InvalidArgument, base.Name);
				this.DataObject.GlobalAddressList = null;
			}
			AddressBookBase addressBookBase2 = (AddressBookBase)base.GetDataObject<AddressBookBase>(this.RoomList, base.DataSession, null, new LocalizedString?(Strings.ErrorAllRoomListNotFound(this.RoomList.ToString())), new LocalizedString?(Strings.ErrorAllRoomListNotUnique(this.RoomList.ToString())));
			if (addressBookBase2 != null)
			{
				this.DataObject.RoomList = (ADObjectId)addressBookBase2.Identity;
			}
			else
			{
				base.WriteError(new ArgumentException(Strings.ErrorAllRoomListNotFound(this.RoomList.ToString())), ErrorCategory.InvalidArgument, base.Name);
				this.DataObject.RoomList = null;
			}
			OfflineAddressBook offlineAddressBook = (OfflineAddressBook)base.GetDataObject<OfflineAddressBook>(this.OfflineAddressBook, base.DataSession, null, new LocalizedString?(Strings.ErrorOfflineAddressBookNotFound(this.OfflineAddressBook.ToString())), new LocalizedString?(Strings.ErrorOfflineAddressBookNotUnique(this.OfflineAddressBook.ToString())));
			if (offlineAddressBook != null)
			{
				this.DataObject.OfflineAddressBook = (ADObjectId)offlineAddressBook.Identity;
			}
			else
			{
				base.WriteError(new ArgumentException(Strings.ErrorOfflineAddressBookNotFound(this.OfflineAddressBook.ToString())), ErrorCategory.InvalidArgument, base.Name);
				this.DataObject.OfflineAddressBook = null;
			}
			ReadOnlyCollection<PropertyDefinition> allProperties = this.DataObject.Schema.AllProperties;
			base.InternalValidate();
		}
	}
}
