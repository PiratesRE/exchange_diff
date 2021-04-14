using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaNotificationManager : DisposeTrackableBase
	{
		internal OwaNotificationManager()
		{
			this.conditionAdvisorTable = new Dictionary<OwaStoreObjectId, OwaConditionAdvisor>();
		}

		public void CreateOwaConditionAdvisor(UserContext userContext, MailboxSession mailboxSession, OwaStoreObjectId folderId, EventObjectType objectType, EventType eventType)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (!userContext.IsWebPartRequest)
			{
				OwaConditionAdvisor value = new OwaConditionAdvisor(mailboxSession, folderId, objectType, eventType);
				if (this.conditionAdvisorTable == null)
				{
					this.conditionAdvisorTable = new Dictionary<OwaStoreObjectId, OwaConditionAdvisor>();
				}
				this.conditionAdvisorTable.Add(folderId, value);
			}
		}

		public Dictionary<OwaStoreObjectId, OwaConditionAdvisor> ConditionAdvisorTable
		{
			get
			{
				return this.conditionAdvisorTable;
			}
		}

		public void DeleteOwaConditionAdvisor(OwaStoreObjectId folderId)
		{
			OwaNotificationManager.DeleteAdvisorFromTable<OwaConditionAdvisor>(folderId, this.conditionAdvisorTable);
		}

		private static void DeleteAdvisorFromTable<T>(OwaStoreObjectId folderId, Dictionary<OwaStoreObjectId, T> advisorTable) where T : IDisposable
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (advisorTable == null || !advisorTable.ContainsKey(folderId))
			{
				return;
			}
			T t = advisorTable[folderId];
			advisorTable.Remove(folderId);
			t.Dispose();
		}

		public void DisposeOwaConditionAdvisorTable()
		{
			if (this.conditionAdvisorTable != null)
			{
				IDictionaryEnumerator dictionaryEnumerator = this.conditionAdvisorTable.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					OwaConditionAdvisor owaConditionAdvisor = (OwaConditionAdvisor)dictionaryEnumerator.Value;
					owaConditionAdvisor.Dispose();
				}
				this.conditionAdvisorTable.Clear();
				this.conditionAdvisorTable = null;
			}
		}

		public void CreateDelegateOwaFolderCountAdvisor(MailboxSession mailboxSession, OwaStoreObjectId folderId, EventObjectType objectType, EventType eventType)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (this.delegateFolderCountAdvisorTable == null)
			{
				this.delegateFolderCountAdvisorTable = new Dictionary<OwaStoreObjectId, OwaFolderCountAdvisor>();
			}
			OwaFolderCountAdvisor value;
			if (!this.delegateFolderCountAdvisorTable.TryGetValue(folderId, out value))
			{
				value = new OwaFolderCountAdvisor(mailboxSession, folderId, objectType, eventType);
				this.delegateFolderCountAdvisorTable.Add(folderId, value);
			}
		}

		public Dictionary<OwaStoreObjectId, OwaFolderCountAdvisor> DelegateFolderCountAdvisorTable
		{
			get
			{
				return this.delegateFolderCountAdvisorTable;
			}
		}

		public void DeleteDelegateFolderCountAdvisor(OwaStoreObjectId folderId)
		{
			OwaNotificationManager.DeleteAdvisorFromTable<OwaFolderCountAdvisor>(folderId, this.delegateFolderCountAdvisorTable);
		}

		public void DisposeDelegateOwaFolderCountAdvisorTable()
		{
			if (this.delegateFolderCountAdvisorTable != null)
			{
				IDictionaryEnumerator dictionaryEnumerator = this.delegateFolderCountAdvisorTable.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					OwaFolderCountAdvisor owaFolderCountAdvisor = (OwaFolderCountAdvisor)dictionaryEnumerator.Value;
					owaFolderCountAdvisor.Dispose();
				}
				this.delegateFolderCountAdvisorTable.Clear();
				this.delegateFolderCountAdvisorTable = null;
			}
		}

		public void CreateArchiveOwaFolderCountAdvisor(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (this.archiveFolderCountAdvisorTable == null)
			{
				this.archiveFolderCountAdvisorTable = new Dictionary<string, OwaFolderCountAdvisor>(StringComparer.InvariantCultureIgnoreCase);
			}
			OwaFolderCountAdvisor value;
			if (!this.archiveFolderCountAdvisorTable.TryGetValue(mailboxSession.MailboxOwnerLegacyDN, out value))
			{
				value = new OwaFolderCountAdvisor(mailboxSession, null, EventObjectType.Folder, EventType.ObjectModified);
				this.archiveFolderCountAdvisorTable.Add(mailboxSession.MailboxOwnerLegacyDN, value);
			}
		}

		public Dictionary<string, OwaFolderCountAdvisor> ArchiveFolderCountAdvisorTable
		{
			get
			{
				return this.archiveFolderCountAdvisorTable;
			}
		}

		public void DisposeArchiveOwaFolderCountAdvisorTable()
		{
			if (this.archiveFolderCountAdvisorTable != null)
			{
				IDictionaryEnumerator dictionaryEnumerator = this.archiveFolderCountAdvisorTable.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					OwaFolderCountAdvisor owaFolderCountAdvisor = (OwaFolderCountAdvisor)dictionaryEnumerator.Value;
					owaFolderCountAdvisor.Dispose();
				}
				this.archiveFolderCountAdvisorTable.Clear();
				this.archiveFolderCountAdvisorTable = null;
			}
		}

		public void CreateOwaFolderCountAdvisor(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (mailboxSession.LogonType != LogonType.Delegated)
			{
				this.folderCountAdvisor = new OwaFolderCountAdvisor(mailboxSession, null, EventObjectType.Folder, EventType.ObjectModified);
			}
		}

		public void DisposeOwaFolderCountAdvisor()
		{
			if (this.folderCountAdvisor != null)
			{
				this.folderCountAdvisor.Dispose();
				this.folderCountAdvisor = null;
			}
		}

		public OwaFolderCountAdvisor FolderCountAdvisor
		{
			get
			{
				return this.folderCountAdvisor;
			}
		}

		public void CreateOwaLastEventAdvisor(UserContext userContext, StoreObjectId folderId, EventObjectType objectType, EventType eventType)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (userContext.MailboxSession.LogonType != LogonType.Delegated)
			{
				this.lastEventAdvisor = new OwaLastEventAdvisor(userContext, folderId, objectType, eventType);
			}
		}

		public void DisposeOwaLastEventAdvisor()
		{
			if (this.lastEventAdvisor != null)
			{
				this.lastEventAdvisor.Dispose();
				this.lastEventAdvisor = null;
			}
		}

		public OwaLastEventAdvisor LastEventAdvisor
		{
			get
			{
				return this.lastEventAdvisor;
			}
			set
			{
				this.lastEventAdvisor = value;
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "OwaNotificationManager.Dispose. IsDisposing: {0}", isDisposing);
			if (isDisposing)
			{
				this.DisposeOwaConditionAdvisorTable();
				this.DisposeOwaFolderCountAdvisor();
				this.DisposeOwaLastEventAdvisor();
				this.DisposeDelegateOwaFolderCountAdvisorTable();
				this.DisposeArchiveOwaFolderCountAdvisorTable();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaNotificationManager>(this);
		}

		private Dictionary<OwaStoreObjectId, OwaConditionAdvisor> conditionAdvisorTable;

		private OwaFolderCountAdvisor folderCountAdvisor;

		private Dictionary<OwaStoreObjectId, OwaFolderCountAdvisor> delegateFolderCountAdvisorTable;

		private OwaLastEventAdvisor lastEventAdvisor;

		private Dictionary<string, OwaFolderCountAdvisor> archiveFolderCountAdvisorTable;
	}
}
