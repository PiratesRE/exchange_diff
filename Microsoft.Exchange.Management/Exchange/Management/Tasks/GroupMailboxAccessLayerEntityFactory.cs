using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class GroupMailboxAccessLayerEntityFactory
	{
		private protected IRecipientSession AdSession { protected get; private set; }

		private protected ADUser CurrentMailbox { protected get; private set; }

		public abstract MailboxLocator CreateMasterLocator();

		protected GroupMailboxAccessLayerEntityFactory(IRecipientSession adSession, ADUser currentMailbox)
		{
			this.AdSession = adSession;
			this.CurrentMailbox = currentMailbox;
		}

		public static GroupMailboxAccessLayerEntityFactory Instantiate(IRecipientSession adSession, ADUser currentMailbox)
		{
			if (currentMailbox.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox)
			{
				return new GroupMailboxAccessLayerEntityFactory.GroupMailboxAccessLayerFactoryForGroupMailbox(adSession, currentMailbox);
			}
			if (currentMailbox.RecipientTypeDetails == RecipientTypeDetails.UserMailbox)
			{
				return new GroupMailboxAccessLayerEntityFactory.GroupMailboxAccessLayerFactoryForUserMailbox(adSession, currentMailbox);
			}
			throw new InvalidOperationException("Unsupported type of mailbox");
		}

		public IMailboxLocator CreateSlaveLocator(MailboxAssociationIdParameter mailboxAssociationIdParameter)
		{
			string externalId = null;
			string associationIdValue = mailboxAssociationIdParameter.AssociationIdValue;
			if (mailboxAssociationIdParameter.AssociationIdType == MailboxAssociationIdParameter.IdTypeExternalId)
			{
				externalId = mailboxAssociationIdParameter.AssociationIdValue;
			}
			return this.CreateSlaveLocator(externalId, associationIdValue);
		}

		protected abstract IMailboxLocator CreateSlaveLocator(string externalId, string legacyDn);

		public abstract BaseAssociationAdaptor CreateAssociationAdaptor(MailboxLocator master, IAssociationStore associationStore);

		private class GroupMailboxAccessLayerFactoryForGroupMailbox : GroupMailboxAccessLayerEntityFactory
		{
			public GroupMailboxAccessLayerFactoryForGroupMailbox(IRecipientSession adSession, ADUser currentMailbox) : base(adSession, currentMailbox)
			{
			}

			public override MailboxLocator CreateMasterLocator()
			{
				return GroupMailboxLocator.Instantiate(base.AdSession, base.CurrentMailbox);
			}

			protected override IMailboxLocator CreateSlaveLocator(string externalId, string legacyDn)
			{
				return new UserMailboxLocator(base.AdSession, externalId, legacyDn);
			}

			public override BaseAssociationAdaptor CreateAssociationAdaptor(MailboxLocator master, IAssociationStore associationStore)
			{
				return new UserAssociationAdaptor(associationStore, base.AdSession, (GroupMailboxLocator)master);
			}
		}

		private class GroupMailboxAccessLayerFactoryForUserMailbox : GroupMailboxAccessLayerEntityFactory
		{
			public GroupMailboxAccessLayerFactoryForUserMailbox(IRecipientSession adSession, ADUser currentMailbox) : base(adSession, currentMailbox)
			{
			}

			public override MailboxLocator CreateMasterLocator()
			{
				return UserMailboxLocator.Instantiate(base.AdSession, base.CurrentMailbox);
			}

			protected override IMailboxLocator CreateSlaveLocator(string externalId, string legacyDn)
			{
				return new GroupMailboxLocator(base.AdSession, externalId, legacyDn);
			}

			public override BaseAssociationAdaptor CreateAssociationAdaptor(MailboxLocator master, IAssociationStore associationStore)
			{
				return new GroupAssociationAdaptor(associationStore, base.AdSession, (UserMailboxLocator)master);
			}
		}
	}
}
