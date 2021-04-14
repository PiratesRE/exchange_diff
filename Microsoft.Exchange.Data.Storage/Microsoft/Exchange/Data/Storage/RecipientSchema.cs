using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RecipientSchema : Schema
	{
		public new static RecipientSchema Instance
		{
			get
			{
				if (RecipientSchema.instance == null)
				{
					RecipientSchema.instance = new RecipientSchema();
				}
				return RecipientSchema.instance;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition EmailAddress = InternalSchema.EmailAddress;

		[Autoload]
		public static readonly StorePropertyDefinition EmailAddrType = InternalSchema.AddrType;

		[DetectCodepage]
		public static readonly StorePropertyDefinition EmailDisplayName = InternalSchema.DisplayName;

		[Autoload]
		public static readonly StorePropertyDefinition RecipientFlags = InternalSchema.RecipientFlags;

		public static readonly StorePropertyDefinition RecipientTrackStatus = InternalSchema.RecipientTrackStatus;

		[Autoload]
		public static readonly StorePropertyDefinition RecipientTrackStatusTime = InternalSchema.RecipientTrackStatusTime;

		public static readonly StorePropertyDefinition RecipientType = InternalSchema.RecipientType;

		[Autoload]
		internal static readonly StorePropertyDefinition RowId = InternalSchema.RowId;

		internal static readonly StorePropertyDefinition RecipientEntryId = InternalSchema.RecipientEntryId;

		internal static readonly StorePropertyDefinition DisplayTypeEx = InternalSchema.DisplayTypeEx;

		public static readonly StorePropertyDefinition DisplayType = InternalSchema.DisplayType;

		public static readonly StorePropertyDefinition DisplayName7Bit = InternalSchema.DisplayName7Bit;

		[Autoload]
		public static readonly StorePropertyDefinition EntryId = InternalSchema.EntryId;

		[Autoload]
		public static readonly StorePropertyDefinition SearchKey = InternalSchema.SearchKey;

		[Autoload]
		public static readonly StorePropertyDefinition SmtpAddress = InternalSchema.SmtpAddress;

		[Autoload]
		public static readonly StorePropertyDefinition SendRichInfo = InternalSchema.SendRichInfo;

		[Autoload]
		public static readonly StorePropertyDefinition Responsibility = InternalSchema.Responsibility;

		[Autoload]
		public static readonly StorePropertyDefinition SipUri = InternalSchema.SipUri;

		[Autoload]
		public static readonly StorePropertyDefinition ParticipantSID = InternalSchema.ParticipantSID;

		[Autoload]
		public static readonly StorePropertyDefinition ParticipantGuid = InternalSchema.ParticipantGuid;

		public static readonly StorePropertyDefinition TransmittableDisplayName = InternalSchema.TransmitableDisplayName;

		[Autoload]
		public static readonly StorePropertyDefinition RecipientOrder = InternalSchema.RecipientOrder;

		public static readonly StorePropertyDefinition OriginatorRequestedAlternateRecipientEntryId = InternalSchema.OriginatorRequestedAlternateRecipientEntryId;

		public static readonly StorePropertyDefinition RedirectionHistory = InternalSchema.RedirectionHistory;

		private static RecipientSchema instance = null;
	}
}
