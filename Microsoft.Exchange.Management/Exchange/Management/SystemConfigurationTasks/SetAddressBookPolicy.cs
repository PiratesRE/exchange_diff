using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "AddressBookPolicy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetAddressBookPolicy : SetMailboxPolicyBase<AddressBookMailboxPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAddressBookPolicy(this.Identity.ToString());
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
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

		[Parameter]
		[ValidateNotNullOrEmpty]
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

		[ValidateNotNullOrEmpty]
		[Parameter]
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

		[Parameter]
		[ValidateNotNullOrEmpty]
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
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			AddressBookMailboxPolicy dataObject = this.DataObject;
			if (this.AddressLists != null && this.AddressLists.Length > 0)
			{
				dataObject.AddressLists = AddressBookPolicyTaskUtility.ValidateAddressBook(base.DataSession, this.AddressLists, new AddressBookPolicyTaskUtility.GetUniqueObject(base.GetDataObject<AddressBookBase>), dataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(AddressBookMailboxPolicySchema.GlobalAddressList) && this.GlobalAddressList != null)
			{
				AddressBookBase addressBookBase = (AddressBookBase)base.GetDataObject<AddressBookBase>(this.GlobalAddressList, base.DataSession, null, new LocalizedString?(Strings.ErrorAddressListOrGlobalAddressListNotFound(this.GlobalAddressList.ToString())), new LocalizedString?(Strings.ErrorAddressListOrGlobalAddressListNotUnique(this.GlobalAddressList.ToString())));
				if (addressBookBase.IsGlobalAddressList)
				{
					dataObject.GlobalAddressList = (ADObjectId)addressBookBase.Identity;
				}
				else
				{
					base.WriteError(new ArgumentException(Strings.ErrorGlobalAddressListNotFound(this.GlobalAddressList.ToString())), ErrorCategory.InvalidArgument, this.Identity);
					dataObject.GlobalAddressList = null;
				}
			}
			if (base.Fields.IsModified(AddressBookMailboxPolicySchema.RoomList) && this.RoomList != null)
			{
				AddressBookBase addressBookBase2 = (AddressBookBase)base.GetDataObject<AddressBookBase>(this.RoomList, base.DataSession, null, new LocalizedString?(Strings.ErrorAllRoomListNotFound(this.RoomList.ToString())), new LocalizedString?(Strings.ErrorAllRoomListNotUnique(this.RoomList.ToString())));
				if (addressBookBase2 != null)
				{
					dataObject.RoomList = (ADObjectId)addressBookBase2.Identity;
				}
				else
				{
					base.WriteError(new ArgumentException(Strings.ErrorAllRoomListNotFound(this.RoomList.ToString())), ErrorCategory.InvalidArgument, this.Identity);
					dataObject.RoomList = null;
				}
			}
			if (base.Fields.IsModified(AddressBookMailboxPolicySchema.OfflineAddressBook))
			{
				if (this.OfflineAddressBook != null)
				{
					OfflineAddressBook offlineAddressBook = (OfflineAddressBook)base.GetDataObject<OfflineAddressBook>(this.OfflineAddressBook, base.DataSession, null, new LocalizedString?(Strings.ErrorOfflineAddressBookNotFound(this.OfflineAddressBook.ToString())), new LocalizedString?(Strings.ErrorOfflineAddressBookNotUnique(this.OfflineAddressBook.ToString())));
					dataObject.OfflineAddressBook = (ADObjectId)offlineAddressBook.Identity;
				}
				else
				{
					dataObject.OfflineAddressBook = null;
				}
			}
			TaskLogger.LogExit();
		}

		// Note: this type is marked as 'beforefieldinit'.
		static SetAddressBookPolicy()
		{
			ADPropertyDefinition[,] array = new ADPropertyDefinition[1, 2];
			array[0, 0] = ADConfigurationObjectSchema.AdminDisplayName;
			SetAddressBookPolicy.propertiesCannotBeSet = array;
		}

		private static readonly ADPropertyDefinition[,] propertiesCannotBeSet;
	}
}
