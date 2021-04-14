using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ContactItemSchema : CacheSchema
	{
		internal static CachedProperty[] CachedProperties
		{
			get
			{
				return ContactItemSchema.cachedProperties;
			}
		}

		public static void Set(ADRawEntry entry, MailRecipient recipient)
		{
			RestrictedItemSchema.Set(entry, recipient);
			CacheSchema.Set(ContactItemSchema.cachedProperties, entry, recipient);
		}

		public const string ExternalEmailAddress = "Microsoft.Exchange.Transport.DirectoryData.ExternalEmailAddress";

		public const string EmailAddresses = "Microsoft.Exchange.Transport.DirectoryData.EmailAddresses";

		public const string InternetEncoding = "Microsoft.Exchange.Transport.DirectoryData.InternetEncoding";

		public const string UseMapiRichTextFormat = "Microsoft.Exchange.Transport.DirectoryData.UseMapiRichTextFormat";

		private static CachedProperty[] cachedProperties = new CachedProperty[]
		{
			new CachedProperty(ADRecipientSchema.ExternalEmailAddress, "Microsoft.Exchange.Transport.DirectoryData.ExternalEmailAddress"),
			new CachedProperty(ADRecipientSchema.EmailAddresses, "Microsoft.Exchange.Transport.DirectoryData.EmailAddresses"),
			new CachedProperty(ADRecipientSchema.InternetEncoding, "Microsoft.Exchange.Transport.DirectoryData.InternetEncoding"),
			new CachedProperty(ADRecipientSchema.MapiRecipient, "Microsoft.Exchange.Transport.DirectoryData.UseMapiRichTextFormat")
		};
	}
}
