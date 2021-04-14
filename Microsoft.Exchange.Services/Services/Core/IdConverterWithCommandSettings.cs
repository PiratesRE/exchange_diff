using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class IdConverterWithCommandSettings
	{
		internal IdConverterWithCommandSettings(ToServiceObjectCommandSettingsBase commandSettings, CallContext callContext)
		{
			this.commandSettings = commandSettings;
			this.callContext = callContext;
		}

		internal ConcatenatedIdAndChangeKey GetConcatenatedId(StoreObjectId storeobjectId)
		{
			StoreSession storeSession = this.GetStoreSession();
			MailboxSession mailboxSession = storeSession as MailboxSession;
			if (mailboxSession != null)
			{
				MailboxId mailboxId = new MailboxId(mailboxSession);
				return IdConverter.GetConcatenatedId(storeobjectId, mailboxId, null);
			}
			if (!(storeSession is PublicFolderSession))
			{
				throw new NotImplementedException();
			}
			if (IdConverter.IsFolderObjectType(storeobjectId.ObjectType))
			{
				return IdConverter.GetConcatenatedIdForPublicFolder(storeobjectId);
			}
			StoreObjectId parentId = this.getParentId();
			return IdConverter.GetConcatenatedIdForPublicFolderItem(storeobjectId, parentId, null);
		}

		internal ItemId PersonaIdFromStoreId(StoreId storeId)
		{
			StoreSession storeSession = this.GetStoreSession();
			MailboxSession mailboxSession = storeSession as MailboxSession;
			if (mailboxSession != null)
			{
				MailboxId mailboxId = new MailboxId(mailboxSession);
				return IdConverter.PersonaIdFromStoreId(storeId, mailboxId);
			}
			if (storeSession is PublicFolderSession)
			{
				StoreObjectId parentId = this.getParentId();
				return IdConverter.PersonaIdFromPublicFolderItemId(storeId, parentId);
			}
			throw new NotImplementedException();
		}

		private StoreSession GetStoreSession()
		{
			MailboxSession mailboxSession = null;
			if (this.commandSettings.IdAndSession != null)
			{
				mailboxSession = (this.commandSettings.IdAndSession.Session as MailboxSession);
			}
			if (mailboxSession == null && this.callContext != null)
			{
				mailboxSession = this.callContext.SessionCache.GetMailboxIdentityMailboxSession();
			}
			if (mailboxSession != null)
			{
				return mailboxSession;
			}
			return this.commandSettings.IdAndSession.Session;
		}

		private StoreObjectId getParentId()
		{
			return StoreId.GetStoreObjectId(this.commandSettings.IdAndSession.ParentFolderId);
		}

		private readonly ToServiceObjectCommandSettingsBase commandSettings;

		private readonly CallContext callContext;
	}
}
