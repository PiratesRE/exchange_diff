using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxAssociationBaseSchema : ItemSchema
	{
		public new static MailboxAssociationBaseSchema Instance
		{
			get
			{
				if (MailboxAssociationBaseSchema.instance == null)
				{
					MailboxAssociationBaseSchema.instance = new MailboxAssociationBaseSchema();
				}
				return MailboxAssociationBaseSchema.instance;
			}
		}

		private static MailboxAssociationBaseSchema instance = null;

		[Autoload]
		public static readonly StorePropertyDefinition LegacyDN = InternalSchema.MailboxAssociationLegacyDN;

		[Autoload]
		public static readonly StorePropertyDefinition ExternalId = InternalSchema.MailboxAssociationExternalId;

		[Autoload]
		public static readonly StorePropertyDefinition IsMember = InternalSchema.MailboxAssociationIsMember;

		[Autoload]
		public static readonly StorePropertyDefinition ShouldEscalate = InternalSchema.MailboxAssociationShouldEscalate;

		[Autoload]
		public static readonly StorePropertyDefinition IsAutoSubscribed = InternalSchema.MailboxAssociationIsAutoSubscribed;

		[Autoload]
		public static readonly StorePropertyDefinition IsPin = InternalSchema.MailboxAssociationIsPin;

		[Autoload]
		public static readonly StorePropertyDefinition JoinDate = InternalSchema.MailboxAssociationJoinDate;

		[Autoload]
		public static readonly StorePropertyDefinition SmtpAddress = InternalSchema.MailboxAssociationSmtpAddress;

		[Autoload]
		public static readonly StorePropertyDefinition SyncedIdentityHash = InternalSchema.MailboxAssociationSyncedIdentityHash;

		[Autoload]
		public static readonly StorePropertyDefinition CurrentVersion = InternalSchema.MailboxAssociationCurrentVersion;

		[Autoload]
		public static readonly StorePropertyDefinition SyncedVersion = InternalSchema.MailboxAssociationSyncedVersion;

		[Autoload]
		public static readonly StorePropertyDefinition LastSyncError = InternalSchema.MailboxAssociationLastSyncError;

		[Autoload]
		public static readonly StorePropertyDefinition SyncAttempts = InternalSchema.MailboxAssociationSyncAttempts;

		[Autoload]
		public static readonly StorePropertyDefinition SyncedSchemaVersion = InternalSchema.MailboxAssociationSyncedSchemaVersion;
	}
}
