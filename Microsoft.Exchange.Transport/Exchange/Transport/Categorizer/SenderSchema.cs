using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class SenderSchema : CacheSchema
	{
		internal static CachedProperty[] CachedProperties
		{
			get
			{
				return SenderSchema.cachedProperties;
			}
		}

		public static void Set(ADRawEntry entry, TransportMailItem mailItem)
		{
			CacheSchema.Set(SenderSchema.cachedProperties, entry, mailItem);
		}

		public const string RecipientType = "Microsoft.Exchange.Transport.DirectoryData.RecipientType";

		public const string Id = "Microsoft.Exchange.Transport.DirectoryData.Sender.Id";

		public const string DistinguishedName = "Microsoft.Exchange.Transport.DirectoryData.Sender.DistinguishedName";

		public const string MaxSendSize = "Microsoft.Exchange.Transport.DirectoryData.Sender.MaxSendSize";

		public const string RecipientLimits = "Microsoft.Exchange.Transport.DirectoryData.Sender.RecipientLimits";

		public const string ExternalOofOptions = "Microsoft.Exchange.Transport.DirectoryData.Sender.ExternalOofOptions";

		public const string Database = "Microsoft.Exchange.Transport.DirectoryData.Database";

		public const string ExternalEmailAddress = "Microsoft.Exchange.Transport.DirectoryData.ExternalEmailAddress";

		private static CachedProperty[] cachedProperties = new CachedProperty[]
		{
			new CachedProperty(ADRecipientSchema.RecipientType, "Microsoft.Exchange.Transport.DirectoryData.RecipientType"),
			new CachedProperty(ADObjectSchema.Id, "Microsoft.Exchange.Transport.DirectoryData.Sender.Id"),
			new CachedProperty(ADObjectSchema.DistinguishedName, "Microsoft.Exchange.Transport.DirectoryData.Sender.DistinguishedName"),
			new CachedProperty(ADRecipientSchema.MaxSendSize, "Microsoft.Exchange.Transport.DirectoryData.Sender.MaxSendSize"),
			new CachedProperty(ADRecipientSchema.RecipientLimits, "Microsoft.Exchange.Transport.DirectoryData.Sender.RecipientLimits"),
			new CachedProperty(IADMailStorageSchema.ExternalOofOptions, "Microsoft.Exchange.Transport.DirectoryData.Sender.ExternalOofOptions"),
			new CachedProperty(IADMailStorageSchema.Database, "Microsoft.Exchange.Transport.DirectoryData.Database"),
			new CachedProperty(ADRecipientSchema.ExternalEmailAddress, "Microsoft.Exchange.Transport.DirectoryData.ExternalEmailAddress")
		};
	}
}
