using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ForwardableItemSchema : CacheSchema
	{
		internal static CachedProperty[] CachedProperties
		{
			get
			{
				return ForwardableItemSchema.cachedProperties;
			}
		}

		public static void Set(ADRawEntry entry, MailRecipient recipient)
		{
			MailboxItemSchema.Set(entry, recipient);
			CacheSchema.Set(ForwardableItemSchema.cachedProperties, entry, recipient);
		}

		public const string ForwardingAddress = "Microsoft.Exchange.Transport.DirectoryData.ForwardingAddress";

		private static CachedProperty[] cachedProperties = new CachedProperty[]
		{
			new CachedProperty(ADRecipientSchema.ForwardingAddress, "Microsoft.Exchange.Transport.DirectoryData.ForwardingAddress"),
			new CachedProperty(IADMailStorageSchema.DeliverToMailboxAndForward, "Microsoft.Exchange.Transport.DirectoryData.DeliverToMailboxAndForward"),
			new CachedProperty(ADRecipientSchema.ThrottlingPolicy, "Microsoft.Exchange.Transport.DirectoryData.ThrottlingPolicy"),
			new CachedProperty(ADRecipientSchema.ForwardingSmtpAddress, "Microsoft.Exchange.Transport.DirectoryData.ForwardingSmtpAddress")
		};
	}
}
