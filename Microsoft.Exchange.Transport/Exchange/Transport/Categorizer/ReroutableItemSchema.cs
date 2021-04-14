using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ReroutableItemSchema : CacheSchema
	{
		internal static CachedProperty[] CachedProperties
		{
			get
			{
				return ReroutableItemSchema.cachedProperties;
			}
		}

		public static void Set(ADRawEntry entry, MailRecipient recipient)
		{
			RestrictedItemSchema.Set(entry, recipient);
			CacheSchema.Set(ReroutableItemSchema.cachedProperties, entry, recipient);
		}

		public const string HomeMtaServerId = "Microsoft.Exchange.Transport.DirectoryData.HomeMtaServerId";

		private static CachedProperty[] cachedProperties = new CachedProperty[]
		{
			new CachedProperty(ADGroupSchema.HomeMtaServerId, "Microsoft.Exchange.Transport.DirectoryData.HomeMtaServerId")
		};
	}
}
