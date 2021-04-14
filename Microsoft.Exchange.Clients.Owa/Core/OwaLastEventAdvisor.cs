using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaLastEventAdvisor : DisposeTrackableBase
	{
		internal OwaLastEventAdvisor(UserContext userContext, StoreObjectId folderId, EventObjectType objectType, EventType eventType)
		{
			this.eventCondition = new EventCondition();
			this.folderId = folderId;
			this.eventCondition.EventType = eventType;
			this.eventCondition.ObjectType = objectType;
			this.eventCondition.ContainerFolderIds.Add(folderId);
			this.lastEventAdvisor = LastEventAdvisor.Create(userContext.MailboxSession, this.eventCondition);
		}

		public StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public Event GetLastEvent(UserContext userContext)
		{
			Event result = null;
			try
			{
				result = this.lastEventAdvisor.GetLastEvent();
			}
			catch (StoragePermanentException)
			{
				this.AttemptToRestoreLastEventAdvisor(userContext);
			}
			catch (StorageTransientException)
			{
				this.AttemptToRestoreLastEventAdvisor(userContext);
			}
			return result;
		}

		private void AttemptToRestoreLastEventAdvisor(UserContext userContext)
		{
			LastEventAdvisor lastEventAdvisor = LastEventAdvisor.Create(userContext.MailboxSession, this.eventCondition);
			this.lastEventAdvisor.Dispose();
			this.lastEventAdvisor = lastEventAdvisor;
		}

		public void Flush()
		{
			try
			{
				this.lastEventAdvisor.GetLastEvent();
			}
			catch (StoragePermanentException)
			{
			}
			catch (StorageTransientException)
			{
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "OwaLastEventAdvisorCall.Dispose. IsDisposing: {0}", isDisposing);
			if (isDisposing && this.lastEventAdvisor != null)
			{
				this.lastEventAdvisor.Dispose();
				this.lastEventAdvisor = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaLastEventAdvisor>(this);
		}

		private LastEventAdvisor lastEventAdvisor;

		private EventCondition eventCondition;

		private StoreObjectId folderId;
	}
}
