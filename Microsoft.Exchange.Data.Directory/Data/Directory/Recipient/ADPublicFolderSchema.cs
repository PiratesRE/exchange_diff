using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADPublicFolderSchema : ADRecipientSchema
	{
		public static readonly ADPropertyDefinition Contacts = new ADPropertyDefinition("PublicFolderContacts", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "pFContacts", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Database = IADMailStorageSchema.Database;

		public static readonly ADPropertyDefinition DeliverToMailboxAndForward = IADMailStorageSchema.DeliverToMailboxAndForward;

		public static readonly ADPropertyDefinition EntryId = new ADPropertyDefinition("EntryId", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchPublicFolderEntryId", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
