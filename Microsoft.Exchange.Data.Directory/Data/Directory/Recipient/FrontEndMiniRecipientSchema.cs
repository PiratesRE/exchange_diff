using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class FrontEndMiniRecipientSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition RecipientType = ADRecipientSchema.RecipientType;

		public static readonly ADPropertyDefinition Database = ADMailboxRecipientSchema.Database;

		public static readonly ADPropertyDefinition ExchangeGuid = ADMailboxRecipientSchema.ExchangeGuid;

		public static readonly ADPropertyDefinition ArchiveDatabase = IADMailStorageSchema.ArchiveDatabase;

		public static readonly ADPropertyDefinition ArchiveGuid = IADMailStorageSchema.ArchiveGuid;

		public static readonly ADPropertyDefinition LastExchangeChangedTime = IOriginatingTimestampSchema.LastExchangeChangedTime;
	}
}
