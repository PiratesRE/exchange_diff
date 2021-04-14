using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RestrictedItemSchema : CacheSchema
	{
		internal static CachedProperty[] CachedProperties
		{
			get
			{
				return RestrictedItemSchema.cachedProperties;
			}
		}

		public static void Set(ADRawEntry entry, MailRecipient recipient)
		{
			DirectoryItemSchema.Set(entry, recipient);
			CacheSchema.Set(RestrictedItemSchema.cachedProperties, entry, recipient);
		}

		public const string AcceptMessagesOnlyFrom = "Microsoft.Exchange.Transport.DirectoryData.AcceptMessagesOnlyFrom";

		public const string AcceptMessagesOnlyFromDLMembers = "Microsoft.Exchange.Transport.DirectoryData.AcceptMessagesOnlyFromDLMembers";

		public const string RejectMessagesFrom = "Microsoft.Exchange.Transport.DirectoryData.RejectMessagesFrom";

		public const string RejectMessagesFromDLMembers = "Microsoft.Exchange.Transport.DirectoryData.RejectMessagesFromDLMembers";

		public const string BypassModerationFrom = "Microsoft.Exchange.Transport.DirectoryData.BypassModerationFrom";

		public const string BypassModerationFromDLMembers = "Microsoft.Exchange.Transport.DirectoryData.BypassModerationFromDLMembers";

		public const string RequireAllSendersAreAuthenticated = "Microsoft.Exchange.Transport.DirectoryData.RequireAllSendersAreAuthenticated";

		public const string MaxReceiveSize = "Microsoft.Exchange.Transport.DirectoryData.MaxReceiveSize";

		public const string ModerationEnabled = "Microsoft.Exchange.Transport.DirectoryData.ModerationEnabled";

		public const string ModeratedBy = "Microsoft.Exchange.Transport.DirectoryData.ModeratedBy";

		public const string ArbitrationMailbox = "Microsoft.Exchange.Transport.DirectoryData.ArbitrationMailbox";

		public const string SendModerationNotifications = "Microsoft.Exchange.Transport.DirectoryData.SendModerationNotifications";

		public const string BypassNestedModerationEnabled = "Microsoft.Exchange.Transport.DirectoryData.BypassNestedModerationEnabled";

		private static CachedProperty[] cachedProperties = new CachedProperty[]
		{
			new CachedProperty(ADRecipientSchema.AcceptMessagesOnlyFrom, "Microsoft.Exchange.Transport.DirectoryData.AcceptMessagesOnlyFrom"),
			new CachedProperty(ADRecipientSchema.AcceptMessagesOnlyFromDLMembers, "Microsoft.Exchange.Transport.DirectoryData.AcceptMessagesOnlyFromDLMembers"),
			new CachedProperty(ADRecipientSchema.RejectMessagesFrom, "Microsoft.Exchange.Transport.DirectoryData.RejectMessagesFrom"),
			new CachedProperty(ADRecipientSchema.RejectMessagesFromDLMembers, "Microsoft.Exchange.Transport.DirectoryData.RejectMessagesFromDLMembers"),
			new CachedProperty(ADRecipientSchema.RequireAllSendersAreAuthenticated, "Microsoft.Exchange.Transport.DirectoryData.RequireAllSendersAreAuthenticated"),
			new CachedProperty(ADRecipientSchema.MaxReceiveSize, "Microsoft.Exchange.Transport.DirectoryData.MaxReceiveSize"),
			new CachedProperty(ADRecipientSchema.ModerationEnabled, "Microsoft.Exchange.Transport.DirectoryData.ModerationEnabled"),
			new CachedProperty(ADRecipientSchema.ModeratedBy, "Microsoft.Exchange.Transport.DirectoryData.ModeratedBy"),
			new CachedProperty(ADRecipientSchema.ArbitrationMailbox, "Microsoft.Exchange.Transport.DirectoryData.ArbitrationMailbox"),
			new CachedProperty(ADRecipientSchema.BypassModerationFrom, "Microsoft.Exchange.Transport.DirectoryData.BypassModerationFrom"),
			new CachedProperty(ADRecipientSchema.BypassModerationFromDLMembers, "Microsoft.Exchange.Transport.DirectoryData.BypassModerationFromDLMembers"),
			new CachedProperty(ADRecipientSchema.SendModerationNotifications, "Microsoft.Exchange.Transport.DirectoryData.SendModerationNotifications"),
			new CachedProperty(ADRecipientSchema.BypassNestedModerationEnabled, "Microsoft.Exchange.Transport.DirectoryData.BypassNestedModerationEnabled")
		};
	}
}
