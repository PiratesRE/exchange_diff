using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class FirstTimeSyncProviderFactory : MailboxSyncProviderFactory
	{
		public FirstTimeSyncProviderFactory(StoreSession storeSession) : base(storeSession)
		{
		}

		public bool UseNewProvider { get; set; }

		public override ISyncProvider CreateSyncProvider(ISyncLogger syncLogger = null)
		{
			Folder folder;
			if (this.folder != null)
			{
				folder = this.folder;
				this.folder = null;
			}
			else
			{
				folder = Folder.Bind(this.storeSession, this.folderId);
			}
			if (this.UseNewProvider)
			{
				return new FirstTimeSyncProvider(folder, this.trackReadFlagChanges, this.trackAssociatedMessageChanges, this.trackConversations, this.allowTableRestrict);
			}
			return new MailboxSyncProvider(folder, this.trackReadFlagChanges, this.trackAssociatedMessageChanges, false, this.trackConversations, this.allowTableRestrict, AirSyncDiagnostics.GetSyncLogger());
		}

		public override ISyncProvider CreateSyncProvider(Folder folder, ISyncLogger syncLogger = null)
		{
			if (this.UseNewProvider)
			{
				return new FirstTimeSyncProvider(folder, this.trackReadFlagChanges, this.trackAssociatedMessageChanges, this.trackConversations, this.allowTableRestrict, false);
			}
			return new MailboxSyncProvider(folder, this.trackReadFlagChanges, this.trackAssociatedMessageChanges, false, this.trackConversations, this.allowTableRestrict, false, AirSyncDiagnostics.GetSyncLogger());
		}
	}
}
