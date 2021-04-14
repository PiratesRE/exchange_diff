using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UnseenDataUserAssociationAdaptor : BaseAssociationAdaptor, IUserAssociationAdaptor, IAssociationAdaptor
	{
		public UnseenDataUserAssociationAdaptor(IAssociationStore associationStore, IRecipientSession adSession, GroupMailboxLocator currentGroup) : base(associationStore, adSession, currentGroup)
		{
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.UnseenDataUserAssociationAdaptorTracer;
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
				return UnseenDataUserAssociationAdaptor.AllProperties;
			}
		}

		public override MailboxAssociation GetAssociation(VersionedId itemId)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		public new IEnumerable<MailboxAssociation> GetAllAssociations()
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		public new IEnumerable<MailboxAssociation> GetEscalatedAssociations()
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		public new IEnumerable<MailboxAssociation> GetPinAssociations()
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		public override MailboxLocator GetSlaveMailboxLocator(MailboxAssociation association)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		public new void DeleteAssociation(MailboxAssociation association)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		public new void SaveAssociation(MailboxAssociation association, bool markForReplication)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		public new void SaveSyncState(MailboxAssociation association)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		public new void ReplicateAssociation(MailboxAssociation association)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		protected override void UpdateStoreAssociationMasterData(MailboxAssociation association, IMailboxAssociationBaseItem item)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		protected override void UpdateStoreAssociationSlaveData(MailboxAssociation association, IMailboxAssociationBaseItem item)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		protected override void ValidateTargetLocatorType(IMailboxLocator locator)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		protected override MailboxAssociation CreateMailboxAssociationWithDefaultValues(IMailboxLocator user)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		protected override IMailboxAssociationBaseItem GetAssociationByItemId(VersionedId itemId)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		protected override IMailboxAssociationBaseItem GetAssociationByIdProperty(PropertyDefinition idProperty, params object[] idValues)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		protected override IMailboxAssociationBaseItem CreateStoreItem(MailboxLocator locator)
		{
			throw new NotImplementedException("UnseenDataUserAssociationAdaptor should only be used to GetMembershipAssociations");
		}

		protected override MailboxAssociation CreateMailboxAssociationFromItem(IPropertyBag item, bool setExtendedProperties = false)
		{
			UserMailboxLocator user = new UserMailboxLocator(base.AdSession, base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.ExternalId, string.Empty), base.AssociationStore.GetValueOrDefault<string>(item, MailboxAssociationBaseSchema.LegacyDN, string.Empty));
			MailboxAssociationFromStore mailboxAssociationFromStore = new MailboxAssociationFromStore
			{
				User = user,
				Group = (base.MasterLocator as GroupMailboxLocator),
				IsMember = base.AssociationStore.GetValueOrDefault<bool>(item, MailboxAssociationBaseSchema.IsMember, false),
				JoinDate = base.AssociationStore.GetValueOrDefault<ExDateTime>(item, MailboxAssociationBaseSchema.JoinDate, default(ExDateTime)),
				LastVisitedDate = base.AssociationStore.GetValueOrDefault<ExDateTime>(item, MailboxAssociationUserSchema.LastVisitedDate, default(ExDateTime))
			};
			this.Tracer.TraceDebug<bool, MailboxAssociationFromStore>((long)this.GetHashCode(), "UnseenDataUserAssociationAdaptor .CreateMailboxAssociationFromItem: Creating association from information found in store item. SetExtendedProperties={0}, Association={1}", setExtendedProperties, mailboxAssociationFromStore);
			return mailboxAssociationFromStore;
		}

		private static readonly PropertyDefinition[] AllProperties = new PropertyDefinition[]
		{
			MailboxAssociationBaseSchema.ExternalId,
			MailboxAssociationBaseSchema.LegacyDN,
			MailboxAssociationBaseSchema.IsMember,
			MailboxAssociationBaseSchema.JoinDate,
			MailboxAssociationUserSchema.LastVisitedDate
		};
	}
}
