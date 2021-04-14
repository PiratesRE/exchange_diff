using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupAssociationAdaptor : BaseAssociationAdaptor, IGroupAssociationAdaptor, IAssociationAdaptor
	{
		public GroupAssociationAdaptor(IAssociationStore associationStore, IRecipientSession adSession, UserMailboxLocator currentUser) : base(associationStore, adSession, currentUser)
		{
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.GroupAssociationAdaptorTracer;
			}
		}

		protected override string ItemClass
		{
			get
			{
				return "IPM.MailboxAssociation.Group";
			}
		}

		protected override PropertyDefinition[] PropertiesToLoad
		{
			get
			{
				return GroupAssociationAdaptor.AllProperties;
			}
		}

		public override MailboxLocator GetSlaveMailboxLocator(MailboxAssociation association)
		{
			ArgumentValidator.ThrowIfNull("association", association);
			return association.Group;
		}

		public override MailboxAssociation GetAssociation(VersionedId itemId)
		{
			ArgumentValidator.ThrowIfNull("itemId", itemId);
			MailboxAssociation result = null;
			using (IMailboxAssociationBaseItem associationByItemId = this.GetAssociationByItemId(itemId))
			{
				if (associationByItemId != null)
				{
					this.Tracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "GroupAssociationAdaptor.GetAssociation: Creating association from store item. itemId={0}", itemId);
					result = this.CreateMailboxAssociationFromItem(associationByItemId, true);
				}
			}
			return result;
		}

		protected override void UpdateStoreAssociationMasterData(MailboxAssociation association, IMailboxAssociationBaseItem item)
		{
			IMailboxAssociationGroup mailboxAssociationGroup = (IMailboxAssociationGroup)item;
			BaseAssociationAdaptor.UpdateLocatorDataInStoreItem(association.Group, mailboxAssociationGroup);
			mailboxAssociationGroup.IsPin = association.IsPin;
			mailboxAssociationGroup.PinDate = association.PinDate;
		}

		protected override void UpdateStoreAssociationSlaveData(MailboxAssociation association, IMailboxAssociationBaseItem item)
		{
			IMailboxAssociationGroup mailboxAssociationGroup = (IMailboxAssociationGroup)item;
			BaseAssociationAdaptor.UpdateLocatorDataInStoreItem(association.Group, mailboxAssociationGroup);
			mailboxAssociationGroup.SyncedIdentityHash = association.User.IdentityHash;
			mailboxAssociationGroup.IsMember = association.IsMember;
			mailboxAssociationGroup.JoinDate = association.JoinDate;
			if (!association.IsMember)
			{
				mailboxAssociationGroup.IsPin = false;
			}
		}

		protected override void ValidateTargetLocatorType(IMailboxLocator locator)
		{
			ArgumentValidator.ThrowIfTypeInvalid<GroupMailboxLocator>("locator", locator);
		}

		protected override MailboxAssociation CreateMailboxAssociationWithDefaultValues(IMailboxLocator group)
		{
			this.ValidateTargetLocatorType(group);
			MailboxAssociation mailboxAssociation = new MailboxAssociation
			{
				User = (base.MasterLocator as UserMailboxLocator),
				Group = (group as GroupMailboxLocator),
				IsPin = false,
				IsMember = false,
				JoinDate = default(ExDateTime),
				PinDate = default(ExDateTime)
			};
			this.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "GroupAssociationAdaptor.CreateMailboxAssociationWithDefaultValues: Creating new association with default values. Association={0}", mailboxAssociation);
			return mailboxAssociation;
		}

		protected override IMailboxAssociationBaseItem GetAssociationByItemId(VersionedId itemId)
		{
			return base.AssociationStore.GetGroupAssociationByItemId(itemId);
		}

		protected override IMailboxAssociationBaseItem GetAssociationByIdProperty(PropertyDefinition idProperty, params object[] idValues)
		{
			return base.AssociationStore.GetGroupAssociationByIdProperty(idProperty, idValues);
		}

		protected override IMailboxAssociationBaseItem CreateStoreItem(MailboxLocator locator)
		{
			IMailboxAssociationGroup mailboxAssociationGroup = base.AssociationStore.CreateGroupAssociation();
			mailboxAssociationGroup[MailboxAssociationBaseSchema.ExternalId] = (locator.ExternalId ?? string.Empty);
			mailboxAssociationGroup[MailboxAssociationBaseSchema.LegacyDN] = locator.LegacyDn;
			mailboxAssociationGroup[MailboxAssociationBaseSchema.IsPin] = false;
			return mailboxAssociationGroup;
		}

		protected override MailboxAssociation CreateMailboxAssociationFromItem(IPropertyBag item, bool setExtendedProperties = false)
		{
			GroupMailboxLocator group = new GroupMailboxLocator(base.AdSession, base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.ExternalId, string.Empty), base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.LegacyDN, string.Empty));
			MailboxAssociationFromStore mailboxAssociationFromStore = new MailboxAssociationFromStore
			{
				User = (base.MasterLocator as UserMailboxLocator),
				Group = group,
				ItemId = base.AssociationStore.GetValueOrDefault<VersionedId>(item, ItemSchema.Id, null),
				IsPin = base.AssociationStore.GetValueOrDefault<bool>(item, MailboxAssociationBaseSchema.IsPin, false),
				IsMember = base.AssociationStore.GetValueOrDefault<bool>(item, MailboxAssociationBaseSchema.IsMember, false),
				JoinDate = base.AssociationStore.GetValueOrDefault<ExDateTime>(item, MailboxAssociationBaseSchema.JoinDate, default(ExDateTime)),
				PinDate = base.AssociationStore.GetValueOrDefault<ExDateTime>(item, MailboxAssociationGroupSchema.PinDate, default(ExDateTime)),
				LastModified = base.AssociationStore.GetValueOrDefault<ExDateTime>(item, StoreObjectSchema.LastModifiedTime, default(ExDateTime)),
				SyncedVersion = base.AssociationStore.GetValueOrDefault<int>(item, MailboxAssociationBaseSchema.SyncedVersion, 0),
				CurrentVersion = base.AssociationStore.GetValueOrDefault<int>(item, MailboxAssociationBaseSchema.CurrentVersion, 0),
				SyncedIdentityHash = base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.SyncedIdentityHash, null)
			};
			if (setExtendedProperties)
			{
				mailboxAssociationFromStore.SyncAttempts = base.AssociationStore.GetValueOrDefault<int>(item, MailboxAssociationBaseSchema.SyncAttempts, 0);
				mailboxAssociationFromStore.SyncedSchemaVersion = base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.SyncedSchemaVersion, string.Empty);
				mailboxAssociationFromStore.LastSyncError = base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.LastSyncError, string.Empty);
			}
			this.Tracer.TraceDebug<bool, MailboxAssociationFromStore>((long)this.GetHashCode(), "GroupAssociationAdaptor.CreateMailboxAssociationFromItem: Creating association from information found in store item. SetExtendedProperties={0}, Association={1}", setExtendedProperties, mailboxAssociationFromStore);
			return mailboxAssociationFromStore;
		}

		private static readonly PropertyDefinition[] PropertiesToMaster = new PropertyDefinition[]
		{
			MailboxAssociationBaseSchema.ExternalId,
			MailboxAssociationBaseSchema.LegacyDN,
			MailboxAssociationBaseSchema.IsPin,
			MailboxAssociationGroupSchema.PinDate,
			MailboxAssociationBaseSchema.CurrentVersion,
			MailboxAssociationBaseSchema.SyncedVersion,
			MailboxAssociationBaseSchema.SyncedIdentityHash
		};

		private static readonly PropertyDefinition[] PropertiesToLoadForReadOnly = new PropertyDefinition[]
		{
			ItemSchema.Id,
			MailboxAssociationBaseSchema.IsMember,
			MailboxAssociationBaseSchema.JoinDate,
			StoreObjectSchema.LastModifiedTime,
			StoreObjectSchema.ItemClass
		};

		private static readonly PropertyDefinition[] AllProperties = PropertyDefinitionCollection.Merge<PropertyDefinition>(GroupAssociationAdaptor.PropertiesToMaster, GroupAssociationAdaptor.PropertiesToLoadForReadOnly);
	}
}
