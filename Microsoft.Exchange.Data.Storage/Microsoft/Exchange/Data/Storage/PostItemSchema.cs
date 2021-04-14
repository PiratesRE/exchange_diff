using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PostItemSchema : ItemSchema
	{
		public new static PostItemSchema Instance
		{
			get
			{
				if (PostItemSchema.instance == null)
				{
					PostItemSchema.instance = new PostItemSchema();
				}
				return PostItemSchema.instance;
			}
		}

		internal override void CoreObjectUpdate(CoreItem coreItem, CoreItemOperation operation)
		{
			base.CoreObjectUpdate(coreItem, operation);
			PostItem.CoreObjectUpdateConversationTopic(coreItem);
			PostItem.CoreObjectUpdateDraftFlag(coreItem);
		}

		[Autoload]
		public static readonly StorePropertyDefinition Flags = InternalSchema.Flags;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition SenderAddressType = InternalSchema.SenderAddressType;

		[DetectCodepage]
		[LegalTracking]
		public static readonly StorePropertyDefinition SenderDisplayName = InternalSchema.SenderDisplayName;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition SenderEmailAddress = InternalSchema.SenderEmailAddress;

		[LegalTracking]
		[Autoload]
		internal static readonly StorePropertyDefinition SenderEntryId = InternalSchema.SenderEntryId;

		[Autoload]
		internal static readonly StorePropertyDefinition AutoResponseSuppress = InternalSchema.AutoResponseSuppress;

		[LegalTracking]
		internal static readonly StorePropertyDefinition ExpiryTime = InternalSchema.ExpiryTime;

		public static readonly StorePropertyDefinition IsDraft = InternalSchema.IsDraft;

		public static readonly StorePropertyDefinition MapiHasAttachment = InternalSchema.MapiHasAttachment;

		internal static readonly StorePropertyDefinition MapiPriority = InternalSchema.MapiPriority;

		internal static readonly StorePropertyDefinition MapiReplyToBlob = InternalSchema.MapiReplyToBlob;

		internal static readonly StorePropertyDefinition MapiReplyToNames = InternalSchema.MapiReplyToNames;

		public static readonly StorePropertyDefinition MessageDraft = InternalSchema.MessageDraft;

		public static readonly StorePropertyDefinition MessageHidden = InternalSchema.MessageHidden;

		public static readonly StorePropertyDefinition MessageHighlighted = InternalSchema.MessageHighlighted;

		public static readonly StorePropertyDefinition MessageTagged = InternalSchema.MessageTagged;

		public static readonly StorePropertyDefinition MID = InternalSchema.MID;

		public static readonly StorePropertyDefinition OriginalAuthorName = InternalSchema.OriginalAuthorName;

		public static readonly StorePropertyDefinition ReceivedRepresentingAddressType = InternalSchema.ReceivedRepresentingAddressType;

		[DetectCodepage]
		public static readonly StorePropertyDefinition ReceivedRepresentingDisplayName = InternalSchema.ReceivedRepresentingDisplayName;

		public static readonly StorePropertyDefinition ReceivedRepresentingEmailAddress = InternalSchema.ReceivedRepresentingEmailAddress;

		public static readonly StorePropertyDefinition ReceivedRepresentingSearchKey = InternalSchema.ReceivedRepresentingSearchKey;

		internal static readonly StorePropertyDefinition TnefCorrelationKey = InternalSchema.TnefCorrelationKey;

		private static PostItemSchema instance = null;
	}
}
