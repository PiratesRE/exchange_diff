using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class DirectoryItemSchema : CacheSchema
	{
		internal static CachedProperty[] CachedProperties
		{
			get
			{
				return DirectoryItemSchema.cachedProperties;
			}
		}

		public static void Set(ADRawEntry entry, MailRecipient recipient)
		{
			CacheSchema.Set(DirectoryItemSchema.cachedProperties, entry, recipient);
		}

		public const string RecipientType = "Microsoft.Exchange.Transport.DirectoryData.RecipientType";

		public const string IsResource = "Microsoft.Exchange.Transport.DirectoryData.IsResource";

		public const string RecipientTypeDetailsRaw = "Microsoft.Exchange.Transport.DirectoryData.RecipientTypeDetailsRaw";

		public const string ObjectGuid = "Microsoft.Exchange.Transport.DirectoryData.ObjectGuid";

		private static CachedProperty[] cachedProperties = new CachedProperty[]
		{
			new CachedProperty(ADRecipientSchema.RecipientType, "Microsoft.Exchange.Transport.DirectoryData.RecipientType"),
			new CachedProperty(ADObjectSchema.Guid, "Microsoft.Exchange.Transport.DirectoryData.ObjectGuid"),
			new CachedProperty(ADRecipientSchema.IsResource, "Microsoft.Exchange.Transport.DirectoryData.IsResource"),
			new CachedProperty(ADRecipientSchema.RecipientTypeDetailsRaw, "Microsoft.Exchange.Transport.DirectoryData.RecipientTypeDetailsRaw")
		};
	}
}
