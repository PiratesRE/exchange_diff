using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PeopleConnectNotifier
	{
		public PeopleConnectNotifier(MailboxSession session)
		{
			Util.ThrowOnNullArgument(session, "session");
			this.session = session;
		}

		public void NotifyConnected(string provider)
		{
			Util.ThrowOnNullOrEmptyArgument(provider, "provider");
			this.Notify(provider, "IPM.Note.PeopleConnect.Notification.Connected");
		}

		public void NotifyDisconnected(string provider)
		{
			Util.ThrowOnNullOrEmptyArgument(provider, "provider");
			this.Notify(provider, "IPM.Note.PeopleConnect.Notification.Disconnected");
		}

		public void NotifyNewTokenNeeded(string provider)
		{
			Util.ThrowOnNullOrEmptyArgument(provider, "provider");
			this.Notify(provider, "IPM.Note.PeopleConnect.Notification.NewTokenNeeded");
		}

		public void NotifyInitialSyncCompleted(string provider)
		{
			Util.ThrowOnNullOrEmptyArgument(provider, "provider");
			this.Notify(provider, "IPM.Note.PeopleConnect.Notification.InitialSyncCompleted");
		}

		private void Notify(string provider, string connectionStatus)
		{
			StoreObjectId defaultFolderId = this.session.GetDefaultFolderId(DefaultFolderType.PeopleConnect);
			if (defaultFolderId == null)
			{
				return;
			}
			using (MessageItem messageItem = MessageItem.Create(this.session, defaultFolderId))
			{
				messageItem.ClassName = connectionStatus;
				messageItem.Subject = provider;
				messageItem.Save(SaveMode.ResolveConflicts);
			}
		}

		private readonly MailboxSession session;
	}
}
