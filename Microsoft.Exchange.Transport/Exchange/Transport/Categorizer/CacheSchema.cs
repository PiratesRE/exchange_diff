using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class CacheSchema
	{
		protected static void Set(CachedProperty[] properties, ADRawEntry entry, TransportMailItem mailItem)
		{
			foreach (CachedProperty cachedProperty in properties)
			{
				cachedProperty.Set(entry, mailItem);
			}
		}

		protected static void Set(CachedProperty[] properties, ADRawEntry entry, MailRecipient recipient)
		{
			foreach (CachedProperty cachedProperty in properties)
			{
				cachedProperty.Set(entry, recipient);
			}
		}

		public const string DirectoryPrefix = "Microsoft.Exchange.Transport.DirectoryData.";
	}
}
