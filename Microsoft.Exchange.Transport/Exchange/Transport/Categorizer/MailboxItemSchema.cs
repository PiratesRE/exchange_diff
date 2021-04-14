using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class MailboxItemSchema : CacheSchema
	{
		internal static CachedProperty[] CachedProperties
		{
			get
			{
				return MailboxItemSchema.cachedProperties;
			}
		}

		public static void Set(ADRawEntry entry, MailRecipient recipient)
		{
			DeliverableItemSchema.Set(entry, recipient);
			CacheSchema.Set(MailboxItemSchema.cachedProperties, entry, recipient);
		}

		public const string ServerName = "Microsoft.Exchange.Transport.DirectoryData.ServerName";

		private static CachedProperty[] cachedProperties = new CachedProperty[]
		{
			new CachedProperty(IADMailStorageSchema.Database, "Microsoft.Exchange.Transport.DirectoryData.Database"),
			new CachedProperty(IADMailStorageSchema.ExchangeGuid, "Microsoft.Exchange.Transport.DirectoryData.ExchangeGuid"),
			new CachedProperty(IADMailStorageSchema.ServerName, "Microsoft.Exchange.Transport.DirectoryData.ServerName"),
			new CachedProperty(ADRecipientSchema.OpenDomainRoutingDisabled, "Microsoft.Exchange.Transport.OpenDomainRoutingDisabled"),
			new CachedProperty(ADRecipientSchema.AddressBookPolicy, "Microsoft.Exchange.Transport.MailRecipient.AddressBookPolicy"),
			new CachedProperty(ADRecipientSchema.DisplayName, "Microsoft.Exchange.Transport.MailRecipient.DisplayName")
		};
	}
}
