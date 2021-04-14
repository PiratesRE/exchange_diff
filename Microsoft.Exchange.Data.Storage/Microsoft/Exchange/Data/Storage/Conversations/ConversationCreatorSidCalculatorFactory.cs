using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationCreatorSidCalculatorFactory
	{
		public ConversationCreatorSidCalculatorFactory(IXSOFactory xsoFactory)
		{
			this.xsoFactory = xsoFactory;
		}

		public bool TryCreate(IMailboxSession mailboxSession, IExchangePrincipal exchangePrincipal, out IConversationCreatorSidCalculator calculator)
		{
			calculator = null;
			if (!this.CanSetConversationCreatorProperty(mailboxSession))
			{
				return false;
			}
			if (exchangePrincipal != null && exchangePrincipal.GetContext(null) != null)
			{
				calculator = this.Create(mailboxSession, exchangePrincipal.GetContext(null));
			}
			else
			{
				calculator = new LegacyConversationCreatorSidCalculator(mailboxSession);
			}
			return true;
		}

		public bool TryCreate(IMailboxSession mailboxSession, MiniRecipient miniRecipient, out IConversationCreatorSidCalculator calculator)
		{
			calculator = null;
			if (!this.CanSetConversationCreatorProperty(mailboxSession))
			{
				return false;
			}
			if (miniRecipient != null && miniRecipient.GetContext(null) != null)
			{
				calculator = this.Create(mailboxSession, miniRecipient.GetContext(null));
			}
			else
			{
				calculator = new LegacyConversationCreatorSidCalculator(mailboxSession);
			}
			return true;
		}

		private IConversationCreatorSidCalculator Create(IMailboxSession mailboxSession, IConstraintProvider constraintProvider)
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(constraintProvider, null, null);
			if (snapshot.DataStorage.DeleteGroupConversation.Enabled)
			{
				ICoreConversationFactory<IConversation> conversationFactory = new CachedConversationFactory(mailboxSession);
				return new ConversationCreatorSidCalculator(this.xsoFactory, mailboxSession, conversationFactory);
			}
			return new LegacyConversationCreatorSidCalculator(mailboxSession);
		}

		private bool CanSetConversationCreatorProperty(IMailboxSession session)
		{
			return session != null && this.IsSessionLogonTypeSupported(session.LogonType) && session.IsGroupMailbox();
		}

		private bool IsSessionLogonTypeSupported(LogonType logonType)
		{
			switch (logonType)
			{
			case LogonType.Owner:
			case LogonType.Delegated:
			case LogonType.Transport:
				return true;
			}
			return false;
		}

		private readonly IXSOFactory xsoFactory;
	}
}
