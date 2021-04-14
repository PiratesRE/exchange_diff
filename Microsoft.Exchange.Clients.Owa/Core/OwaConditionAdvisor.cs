using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaConditionAdvisor : DisposeTrackableBase
	{
		internal OwaConditionAdvisor(MailboxSession mailboxSession, OwaStoreObjectId folderId, EventObjectType objectType, EventType eventType)
		{
			this.eventCondition = new EventCondition();
			this.folderId = folderId;
			this.eventCondition.ObjectType = objectType;
			this.eventCondition.EventType = eventType;
			this.eventCondition.ContainerFolderIds.Add(folderId.StoreObjectId);
			this.conditionAdvisor = ConditionAdvisor.Create(mailboxSession, this.eventCondition);
		}

		public OwaStoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public bool IsRecycled
		{
			get
			{
				return this.isRecycled;
			}
			set
			{
				this.isRecycled = value;
			}
		}

		public bool IsConditionTrue(MailboxSession mailboxSession)
		{
			bool result = false;
			try
			{
				result = this.conditionAdvisor.IsConditionTrue;
			}
			catch (StorageTransientException)
			{
				this.AttemptToRestoreConditionAdvisor(mailboxSession);
			}
			catch (StoragePermanentException)
			{
				this.AttemptToRestoreConditionAdvisor(mailboxSession);
			}
			return result;
		}

		public void ResetCondition(MailboxSession mailboxSession)
		{
			try
			{
				this.conditionAdvisor.ResetCondition();
			}
			catch (StorageTransientException)
			{
				this.AttemptToRestoreConditionAdvisor(mailboxSession);
			}
			catch (StoragePermanentException)
			{
				this.AttemptToRestoreConditionAdvisor(mailboxSession);
			}
		}

		private void AttemptToRestoreConditionAdvisor(MailboxSession mailboxSession)
		{
			ConditionAdvisor conditionAdvisor = ConditionAdvisor.Create(mailboxSession, this.eventCondition);
			this.conditionAdvisor.Dispose();
			this.conditionAdvisor = conditionAdvisor;
			this.isRecycled = true;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "OwaConditionAdvisor.InternalDispose. IsDisposing: {0}", isDisposing);
			if (this.isDisposed)
			{
				return;
			}
			if (isDisposing)
			{
				if (this.conditionAdvisor != null)
				{
					this.conditionAdvisor.Dispose();
					this.conditionAdvisor = null;
				}
				this.isDisposed = true;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaConditionAdvisor>(this);
		}

		private ConditionAdvisor conditionAdvisor;

		private EventCondition eventCondition;

		private OwaStoreObjectId folderId;

		private bool isRecycled;

		private bool isDisposed;
	}
}
