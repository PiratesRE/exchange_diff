using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ParticipantSchema : Schema
	{
		public new static ParticipantSchema Instance
		{
			get
			{
				if (ParticipantSchema.instance == null)
				{
					ParticipantSchema.instance = new ParticipantSchema();
				}
				return ParticipantSchema.instance;
			}
		}

		protected ParticipantSchema()
		{
		}

		public static readonly StorePropertyDefinition DisplayName = InternalSchema.EmailDisplayName;

		public static readonly StorePropertyDefinition SimpleDisplayName = InternalSchema.DisplayName7Bit;

		public static readonly StorePropertyDefinition EmailAddress = InternalSchema.EmailAddress;

		public static readonly StorePropertyDefinition RoutingType = InternalSchema.EmailRoutingType;

		public static readonly StorePropertyDefinition OriginalDisplayName = InternalSchema.EmailOriginalDisplayName;

		public static readonly StorePropertyDefinition EmailAddressForDisplay = InternalSchema.EmailAddressForDisplay;

		public static readonly StorePropertyDefinition SmtpAddress = InternalSchema.SmtpAddress;

		internal static readonly StorePropertyDefinition LegacyExchangeDN = InternalSchema.LegacyExchangeDN;

		public static readonly StorePropertyDefinition OriginItemId = InternalSchema.ParticipantOriginItemId;

		public static readonly StorePropertyDefinition IsDistributionList = InternalSchema.IsDistributionList;

		public static readonly StorePropertyDefinition IsRoom = InternalSchema.IsRoom;

		public static readonly StorePropertyDefinition IsResource = InternalSchema.IsResource;

		public static readonly StorePropertyDefinition IsGroupMailbox = InternalSchema.IsGroupMailbox;

		public static readonly StorePropertyDefinition IsMailboxUser = InternalSchema.IsMailboxUser;

		[Autoload]
		public static readonly StorePropertyDefinition DisplayTypeEx = InternalSchema.DisplayTypeEx;

		[Autoload]
		public static readonly StorePropertyDefinition DisplayType = InternalSchema.DisplayType;

		public static readonly StorePropertyDefinition Alias = InternalSchema.Alias;

		internal static readonly StorePropertyDefinition SendRichInfo = InternalSchema.SendRichInfo;

		internal static readonly StorePropertyDefinition SendInternetEncoding = InternalSchema.SendInternetEncoding;

		public static readonly StorePropertyDefinition SipUri = InternalSchema.SipUri;

		public static readonly StorePropertyDefinition ParticipantSID = InternalSchema.ParticipantSID;

		public static readonly StorePropertyDefinition ParticipantGuid = InternalSchema.ParticipantGuid;

		private static ParticipantSchema instance = null;

		public static ReadOnlyCollection<ADPropertyDefinition> SupportedADProperties = new ReadOnlyCollection<ADPropertyDefinition>(new ADPropertyDefinition[]
		{
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.PrimarySmtpAddress,
			ADObjectSchema.ObjectClass,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.RecipientTypeDetails,
			ADRecipientSchema.RecipientDisplayType,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.SimpleDisplayName,
			ADMailboxRecipientSchema.Sid,
			ADObjectSchema.Guid,
			ADRecipientSchema.Alias
		});
	}
}
