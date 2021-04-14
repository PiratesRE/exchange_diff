using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class AddressBookMailboxPolicySchema : MailboxPolicySchema
	{
		public static readonly ADPropertyDefinition AssociatedUsers = new ADPropertyDefinition("AssociatedUsers", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchAddressBookPolicyBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AddressLists = new ADPropertyDefinition("AddressLists", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchAddressListsLink", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GlobalAddressList = new ADPropertyDefinition("GlobalAddressList", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchGlobalAddressListLink", ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RoomList = new ADPropertyDefinition("RoomList", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchAllRoomListLink", ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OfflineAddressBook = new ADPropertyDefinition("OfflineAddressBook", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchOfflineAddressBookLink", ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
