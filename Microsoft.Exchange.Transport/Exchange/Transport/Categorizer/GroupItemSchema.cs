using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class GroupItemSchema : CacheSchema
	{
		internal static CachedProperty[] CachedProperties
		{
			get
			{
				return GroupItemSchema.cachedProperties;
			}
		}

		public static void Set(ADRawEntry entry, MailRecipient recipient)
		{
			ReroutableItemSchema.Set(entry, recipient);
			CacheSchema.Set(GroupItemSchema.cachedProperties, entry, recipient);
		}

		public const string SendDeliveryReportsTo = "Microsoft.Exchange.Transport.DirectoryData.SendDeliveryReportsTo";

		public const string SendOofMessageToOriginator = "Microsoft.Exchange.Transport.DirectoryData.SendOofMessageToOriginator";

		public const string ManagedBy = "Microsoft.Exchange.Transport.DirectoryData.ManagedBy";

		private static CachedProperty[] cachedProperties = new CachedProperty[]
		{
			new CachedProperty(IADDistributionListSchema.SendDeliveryReportsTo, "Microsoft.Exchange.Transport.DirectoryData.SendDeliveryReportsTo"),
			new CachedProperty(ADGroupSchema.SendOofMessageToOriginatorEnabled, "Microsoft.Exchange.Transport.DirectoryData.SendOofMessageToOriginator"),
			new CachedProperty(ADGroupSchema.ManagedBy, "Microsoft.Exchange.Transport.DirectoryData.ManagedBy")
		};
	}
}
