using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class PublicFolderItemSchema : CacheSchema
	{
		internal static CachedProperty[] CachedProperties
		{
			get
			{
				return PublicFolderItemSchema.cachedProperties;
			}
		}

		public static void Set(ADRawEntry entry, MailRecipient recipient)
		{
			ForwardableItemSchema.Set(entry, recipient);
			CacheSchema.Set(PublicFolderItemSchema.cachedProperties, entry, recipient);
			if (PublicFolderItem.IsRemoteRecipient(recipient))
			{
				ContactItemSchema.Set(entry, recipient);
				recipient.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.IsRemoteRecipient", true);
			}
		}

		public const string IsOneOffRecipient = "Microsoft.Exchange.Transport.IsOneOffRecipient";

		public const string IsRemoteRecipient = "Microsoft.Exchange.Transport.IsRemoteRecipient";

		public const string EntryId = "Microsoft.Exchange.Transport.DirectoryData.EntryId";

		public const string ContentMailbox = "Microsoft.Exchange.Transport.DirectoryData.ContentMailbox";

		private static CachedProperty[] cachedProperties = new CachedProperty[]
		{
			new CachedProperty(ADPublicFolderSchema.EntryId, "Microsoft.Exchange.Transport.DirectoryData.EntryId"),
			new CachedProperty(ADRecipientSchema.DefaultPublicFolderMailbox, "Microsoft.Exchange.Transport.DirectoryData.ContentMailbox"),
			new CachedProperty(ADRecipientSchema.ExternalEmailAddress, "Microsoft.Exchange.Transport.DirectoryData.ExternalEmailAddress"),
			new CachedProperty(ADRecipientSchema.EmailAddresses, "Microsoft.Exchange.Transport.DirectoryData.EmailAddresses")
		};
	}
}
