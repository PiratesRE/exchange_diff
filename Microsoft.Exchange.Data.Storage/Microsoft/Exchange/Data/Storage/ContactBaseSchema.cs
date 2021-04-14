using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContactBaseSchema : ItemSchema
	{
		public new static ContactBaseSchema Instance
		{
			get
			{
				if (ContactBaseSchema.instance == null)
				{
					ContactBaseSchema.instance = new ContactBaseSchema();
				}
				return ContactBaseSchema.instance;
			}
		}

		protected override void CoreObjectUpdateAllAttachmentsHidden(CoreItem coreItem)
		{
			Contact.CoreObjectUpdateAllAttachmentsHidden(coreItem);
		}

		private static ContactBaseSchema instance = null;

		[Autoload]
		public static readonly StorePropertyDefinition FileAs = InternalSchema.FileAsString;

		[DetectCodepage]
		[Autoload]
		public static readonly StorePropertyDefinition DisplayNameFirstLast = InternalSchema.DisplayNameFirstLast;

		[Autoload]
		[DetectCodepage]
		public static readonly StorePropertyDefinition DisplayNameLastFirst = InternalSchema.DisplayNameLastFirst;

		[DetectCodepage]
		[Autoload]
		public static readonly StorePropertyDefinition DisplayNamePriority = InternalSchema.DisplayNamePriority;

		public static readonly StorePropertyDefinition AnrViewParticipant = InternalSchema.AnrViewParticipant;

		public static readonly StorePropertyDefinition GALObjectId = InternalSchema.GALObjectId;

		public static readonly StorePropertyDefinition GALRecipientType = InternalSchema.GALRecipientType;

		public static readonly StorePropertyDefinition GALHiddenFromAddressListsEnabled = InternalSchema.GALHiddenFromAddressListsEnabled;

		public static readonly StorePropertyDefinition GALCurrentSpeechGrammarVersion = InternalSchema.GALCurrentSpeechGrammarVersion;

		public static readonly StorePropertyDefinition GALPreviousSpeechGrammarVersion = InternalSchema.GALPreviousSpeechGrammarVersion;

		public static readonly StorePropertyDefinition GALCurrentUMDtmfMapVersion = InternalSchema.GALCurrentUMDtmfMapVersion;

		public static readonly StorePropertyDefinition GALPreviousUMDtmfMapVersion = InternalSchema.GALPreviousUMDtmfMapVersion;

		public static readonly StorePropertyDefinition GALSpeechNormalizedNamesForDisplayName = InternalSchema.GALSpeechNormalizedNamesForDisplayName;

		public static readonly StorePropertyDefinition GALSpeechNormalizedNamesForPhoneticDisplayName = InternalSchema.GALSpeechNormalizedNamesForPhoneticDisplayName;
	}
}
