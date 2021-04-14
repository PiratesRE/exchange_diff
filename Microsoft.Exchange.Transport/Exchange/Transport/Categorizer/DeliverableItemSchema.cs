using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class DeliverableItemSchema : CacheSchema
	{
		internal static CachedProperty[] CachedProperties
		{
			get
			{
				return DeliverableItemSchema.cachedProperties;
			}
		}

		public static void Set(ADRawEntry entry, MailRecipient recipient)
		{
			RestrictedItemSchema.Set(entry, recipient);
			CacheSchema.Set(DeliverableItemSchema.cachedProperties, entry, recipient);
		}

		public const string LegacyExchangeDN = "Microsoft.Exchange.Transport.DirectoryData.LegacyExchangeDN";

		private static CachedProperty[] cachedProperties = new CachedProperty[]
		{
			new CachedProperty(ADRecipientSchema.LegacyExchangeDN, "Microsoft.Exchange.Transport.DirectoryData.LegacyExchangeDN")
		};
	}
}
