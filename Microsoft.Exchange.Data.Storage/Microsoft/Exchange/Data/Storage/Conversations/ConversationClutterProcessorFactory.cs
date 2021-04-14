using System;
using Microsoft.Exchange.Data.Storage.Clutter;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ConversationClutterProcessorFactory
	{
		public static IConversationClutterProcessor Create(IStoreSession session)
		{
			return ConversationClutterProcessorFactory.testHook.Value(session);
		}

		private static IConversationClutterProcessor CreateInternal(IStoreSession session)
		{
			IConversationClutterProcessor result = null;
			MailboxSession mailboxSession = session as MailboxSession;
			if (mailboxSession != null && ClutterUtilities.IsClutterEnabled(mailboxSession, mailboxSession.MailboxOwner.GetConfiguration()))
			{
				result = new FolderBasedConversationClutterProcessor(mailboxSession);
			}
			return result;
		}

		internal static readonly Hookable<Func<IStoreSession, IConversationClutterProcessor>> testHook = Hookable<Func<IStoreSession, IConversationClutterProcessor>>.Create(true, new Func<IStoreSession, IConversationClutterProcessor>(ConversationClutterProcessorFactory.CreateInternal));
	}
}
