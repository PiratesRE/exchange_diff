using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UserAssociationAdaptor : BaseAssociationAdaptor, IUserAssociationAdaptor, IAssociationAdaptor
	{
		public UserAssociationAdaptor(IAssociationStore associationStore, IRecipientSession adSession, GroupMailboxLocator currentGroup) : base(associationStore, adSession, currentGroup)
		{
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.UserAssociationAdaptorTracer;
			}
		}

		protected override string ItemClass
		{
			get
			{
				return "IPM.MailboxAssociation.User";
			}
		}

		protected override PropertyDefinition[] PropertiesToLoad
		{
			get
			{
				return UserAssociationAdaptor.AllProperties;
			}
		}

		public override MailboxLocator GetSlaveMailboxLocator(MailboxAssociation association)
		{
			ArgumentValidator.ThrowIfNull("association", association);
			return association.User;
		}

		public override MailboxAssociation GetAssociation(VersionedId itemId)
		{
			ArgumentValidator.ThrowIfNull("itemId", itemId);
			MailboxAssociation result = null;
			using (IMailboxAssociationBaseItem associationByItemId = this.GetAssociationByItemId(itemId))
			{
				if (associationByItemId != null)
				{
					this.Tracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "UserAssociationAdaptor.GetAssociation: Creating association from store item. itemId={0}", itemId);
					result = this.CreateMailboxAssociationFromItem(associationByItemId, true);
				}
			}
			return result;
		}

		protected override void UpdateStoreAssociationMasterData(MailboxAssociation association, IMailboxAssociationBaseItem item)
		{
			IMailboxAssociationUser mailboxAssociationUser = (IMailboxAssociationUser)item;
			BaseAssociationAdaptor.UpdateLocatorDataInStoreItem(association.User, mailboxAssociationUser);
			mailboxAssociationUser.SmtpAddress = association.UserSmtpAddress;
			mailboxAssociationUser.IsMember = association.IsMember;
			mailboxAssociationUser.ShouldEscalate = association.ShouldEscalate;
			mailboxAssociationUser.IsAutoSubscribed = association.IsAutoSubscribed;
			mailboxAssociationUser.JoinedBy = association.JoinedBy;
			mailboxAssociationUser.JoinDate = association.JoinDate;
			mailboxAssociationUser.LastVisitedDate = association.LastVisitedDate;
			if (!association.IsMember && mailboxAssociationUser.IsPin)
			{
				mailboxAssociationUser.IsPin = false;
			}
		}

		protected override void UpdateStoreAssociationSlaveData(MailboxAssociation association, IMailboxAssociationBaseItem item)
		{
			IMailboxAssociationUser mailboxAssociationUser = (IMailboxAssociationUser)item;
			BaseAssociationAdaptor.UpdateLocatorDataInStoreItem(association.User, mailboxAssociationUser);
			mailboxAssociationUser.SyncedIdentityHash = association.Group.IdentityHash;
			mailboxAssociationUser.IsPin = association.IsPin;
		}

		protected override void ValidateTargetLocatorType(IMailboxLocator locator)
		{
			ArgumentValidator.ThrowIfTypeInvalid<UserMailboxLocator>("locator", locator);
		}

		protected override MailboxAssociation CreateMailboxAssociationWithDefaultValues(IMailboxLocator user)
		{
			this.ValidateTargetLocatorType(user);
			MailboxAssociation mailboxAssociation = new MailboxAssociation
			{
				User = (user as UserMailboxLocator),
				Group = (base.MasterLocator as GroupMailboxLocator),
				UserSmtpAddress = SmtpAddress.Empty,
				IsMember = false,
				ShouldEscalate = false,
				IsAutoSubscribed = false,
				IsPin = false,
				JoinedBy = string.Empty,
				JoinDate = default(ExDateTime),
				LastVisitedDate = default(ExDateTime)
			};
			this.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "UserAssociationAdaptor.CreateMailboxAssociationWithDefaultValues: Creating new association with default values. Association={0}", mailboxAssociation);
			return mailboxAssociation;
		}

		protected override IMailboxAssociationBaseItem GetAssociationByItemId(VersionedId itemId)
		{
			return base.AssociationStore.GetUserAssociationByItemId(itemId);
		}

		protected override IMailboxAssociationBaseItem GetAssociationByIdProperty(PropertyDefinition idProperty, params object[] idValues)
		{
			return base.AssociationStore.GetUserAssociationByIdProperty(idProperty, idValues);
		}

		protected override IMailboxAssociationBaseItem CreateStoreItem(MailboxLocator locator)
		{
			IMailboxAssociationUser mailboxAssociationUser = base.AssociationStore.CreateUserAssociation();
			mailboxAssociationUser[MailboxAssociationBaseSchema.ExternalId] = (locator.ExternalId ?? string.Empty);
			mailboxAssociationUser[MailboxAssociationBaseSchema.LegacyDN] = locator.LegacyDn;
			mailboxAssociationUser[MailboxAssociationBaseSchema.SmtpAddress] = string.Empty;
			mailboxAssociationUser[MailboxAssociationBaseSchema.IsMember] = false;
			mailboxAssociationUser[MailboxAssociationBaseSchema.ShouldEscalate] = false;
			mailboxAssociationUser[MailboxAssociationBaseSchema.IsPin] = false;
			mailboxAssociationUser[MailboxAssociationBaseSchema.JoinDate] = default(ExDateTime);
			mailboxAssociationUser[MailboxAssociationUserSchema.LastVisitedDate] = default(ExDateTime);
			return mailboxAssociationUser;
		}

		protected override MailboxAssociation CreateMailboxAssociationFromItem(IPropertyBag item, bool setExtendedProperties = false)
		{
			UserMailboxLocator user = new UserMailboxLocator(base.AdSession, base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.ExternalId, string.Empty), base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.LegacyDN, string.Empty));
			string valueOrDefault = base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.SmtpAddress, null);
			MailboxAssociationFromStore mailboxAssociationFromStore = new MailboxAssociationFromStore
			{
				User = user,
				Group = (base.MasterLocator as GroupMailboxLocator),
				ItemId = base.AssociationStore.GetValueOrDefault<VersionedId>(item, ItemSchema.Id, null),
				UserSmtpAddress = (string.IsNullOrEmpty(valueOrDefault) ? SmtpAddress.Empty : new SmtpAddress(valueOrDefault)),
				IsMember = base.AssociationStore.GetValueOrDefault<bool>(item, MailboxAssociationBaseSchema.IsMember, false),
				ShouldEscalate = base.AssociationStore.GetValueOrDefault<bool>(item, MailboxAssociationBaseSchema.ShouldEscalate, false),
				IsAutoSubscribed = base.AssociationStore.GetValueOrDefault<bool>(item, MailboxAssociationBaseSchema.IsAutoSubscribed, false),
				IsPin = base.AssociationStore.GetValueOrDefault<bool>(item, MailboxAssociationBaseSchema.IsPin, false),
				JoinDate = base.AssociationStore.GetValueOrDefault<ExDateTime>(item, MailboxAssociationBaseSchema.JoinDate, default(ExDateTime)),
				LastVisitedDate = base.AssociationStore.GetValueOrDefault<ExDateTime>(item, MailboxAssociationUserSchema.LastVisitedDate, default(ExDateTime)),
				LastModified = base.AssociationStore.GetValueOrDefault<ExDateTime>(item, StoreObjectSchema.LastModifiedTime, default(ExDateTime)),
				SyncedVersion = base.AssociationStore.GetValueOrDefault<int>(item, MailboxAssociationBaseSchema.SyncedVersion, 0),
				CurrentVersion = base.AssociationStore.GetValueOrDefault<int>(item, MailboxAssociationBaseSchema.CurrentVersion, 0),
				SyncedIdentityHash = base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.SyncedIdentityHash, null)
			};
			if (setExtendedProperties)
			{
				mailboxAssociationFromStore.JoinedBy = base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationUserSchema.JoinedBy, string.Empty);
				mailboxAssociationFromStore.SyncAttempts = base.AssociationStore.GetValueOrDefault<int>(item, MailboxAssociationBaseSchema.SyncAttempts, 0);
				mailboxAssociationFromStore.SyncedSchemaVersion = base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.SyncedSchemaVersion, string.Empty);
				mailboxAssociationFromStore.LastSyncError = base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.LastSyncError, string.Empty);
			}
			this.Tracer.TraceDebug<bool, MailboxAssociationFromStore>((long)this.GetHashCode(), "UserAssociationAdaptor.CreateMailboxAssociationFromItem: Creating association from information found in store item. SetExtendedProperties={0}, Association={1}", setExtendedProperties, mailboxAssociationFromStore);
			return mailboxAssociationFromStore;
		}

		private static readonly PropertyDefinition[] PropertiesToMaster = new PropertyDefinition[]
		{
			MailboxAssociationBaseSchema.ExternalId,
			MailboxAssociationBaseSchema.LegacyDN,
			MailboxAssociationBaseSchema.SmtpAddress,
			MailboxAssociationBaseSchema.IsMember,
			MailboxAssociationBaseSchema.ShouldEscalate,
			MailboxAssociationBaseSchema.IsAutoSubscribed,
			MailboxAssociationBaseSchema.JoinDate,
			MailboxAssociationUserSchema.LastVisitedDate,
			MailboxAssociationBaseSchema.CurrentVersion,
			MailboxAssociationBaseSchema.SyncedVersion,
			MailboxAssociationBaseSchema.SyncedIdentityHash
		};

		private static readonly PropertyDefinition[] PropertiesToLoadForReadOnly = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.LastModifiedTime,
			StoreObjectSchema.ItemClass,
			MailboxAssociationBaseSchema.IsPin
		};

		private static readonly PropertyDefinition[] AllProperties = PropertyDefinitionCollection.Merge<PropertyDefinition>(UserAssociationAdaptor.PropertiesToMaster, UserAssociationAdaptor.PropertiesToLoadForReadOnly);
	}
}
