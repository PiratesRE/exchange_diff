using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaFolderCountAdvisor : DisposeTrackableBase
	{
		internal OwaFolderCountAdvisor(MailboxSession mailboxSession, OwaStoreObjectId folderId, EventObjectType objectType, EventType eventType)
		{
			this.folderId = folderId;
			this.mailboxOwner = mailboxSession.MailboxOwner;
			this.eventCondition = new EventCondition();
			this.eventCondition.EventType = eventType;
			this.eventCondition.ObjectType = objectType;
			if (folderId != null)
			{
				this.eventCondition.ContainerFolderIds.Add(folderId.StoreObjectId);
				if (mailboxSession.LogonType == LogonType.Delegated)
				{
					this.eventCondition.ObjectIds.Add(folderId.StoreObjectId);
				}
			}
			this.countAdvisor = ItemCountAdvisor.Create(mailboxSession, this.eventCondition);
		}

		public OwaStoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public IExchangePrincipal MailboxOwner
		{
			get
			{
				return this.mailboxOwner;
			}
		}

		public Dictionary<StoreObjectId, ItemCountPair> GetItemCounts(MailboxSession mailboxSession)
		{
			Dictionary<StoreObjectId, ItemCountPair> result = null;
			try
			{
				result = this.countAdvisor.GetItemCounts();
			}
			catch (StorageTransientException)
			{
				this.AttemptToRestoreFolderCountAdvisor(mailboxSession);
			}
			catch (StoragePermanentException)
			{
				this.AttemptToRestoreFolderCountAdvisor(mailboxSession);
			}
			return result;
		}

		private void AttemptToRestoreFolderCountAdvisor(MailboxSession mailboxSession)
		{
			ItemCountAdvisor itemCountAdvisor = ItemCountAdvisor.Create(mailboxSession, this.eventCondition);
			this.countAdvisor.Dispose();
			this.countAdvisor = itemCountAdvisor;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "OwaFolderCountAdvisor.Dispose. IsDisposing: {0}", isDisposing);
			if (isDisposing && this.countAdvisor != null)
			{
				this.countAdvisor.Dispose();
				this.countAdvisor = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaFolderCountAdvisor>(this);
		}

		private ItemCountAdvisor countAdvisor;

		private EventCondition eventCondition;

		private OwaStoreObjectId folderId;

		private IExchangePrincipal mailboxOwner;
	}
}
