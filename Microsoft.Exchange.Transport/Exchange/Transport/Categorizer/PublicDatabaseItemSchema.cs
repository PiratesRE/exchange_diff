using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class PublicDatabaseItemSchema : CacheSchema
	{
		internal static CachedProperty[] CachedProperties
		{
			get
			{
				return PublicDatabaseItemSchema.cachedProperties;
			}
		}

		public static void Set(ADRawEntry entry, MailRecipient recipient)
		{
			DeliverableItemSchema.Set(entry, recipient);
			CacheSchema.Set(PublicDatabaseItemSchema.cachedProperties, entry, recipient);
		}

		public const string DistinguishedName = "Microsoft.Exchange.Transport.DirectoryData.DistinguishedName";

		public const string Id = "Microsoft.Exchange.Transport.DirectoryData.Id";

		private static CachedProperty[] cachedProperties = new CachedProperty[]
		{
			new CachedProperty(ADObjectSchema.DistinguishedName, "Microsoft.Exchange.Transport.DirectoryData.DistinguishedName"),
			new CachedProperty(ADObjectSchema.Id, "Microsoft.Exchange.Transport.DirectoryData.Id")
		};
	}
}
