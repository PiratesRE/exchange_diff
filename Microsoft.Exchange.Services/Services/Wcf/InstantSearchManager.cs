using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class InstantSearchManager : DisposeTrackableBase
	{
		public InstantSearchManager(Func<MailboxSession> createMailboxSession)
		{
			this.createMailboxSession = createMailboxSession;
		}

		public InstantSearchSession GetOrCreateSession(string sessionId, List<StoreId> folderIds, InstantSearchItemType itemType, long searchRequestId, SuggestionSourceType suggestionSource, out bool isNewSession)
		{
			isNewSession = false;
			InstantSearchSession result;
			lock (this.syncLock)
			{
				if (this.isDisposed)
				{
					result = null;
				}
				else if (this.currentSession == null)
				{
					this.CreateNewInstantSearchSession(sessionId, folderIds, itemType, searchRequestId, suggestionSource);
					isNewSession = true;
					result = this.currentSession;
				}
				else if (this.currentSession.SessionId == sessionId)
				{
					result = this.currentSession;
				}
				else
				{
					if (searchRequestId < this.currentSessionCreationRequestId)
					{
						throw new InstantSearchSessionExpiredException();
					}
					this.EndSearchSession(this.currentSession.SessionId, searchRequestId);
					this.CreateNewInstantSearchSession(sessionId, folderIds, itemType, searchRequestId, suggestionSource);
					isNewSession = true;
					result = this.currentSession;
				}
			}
			return result;
		}

		public EndInstantSearchSessionResponse EndSearchSession(string sessionId)
		{
			EndInstantSearchSessionResponse result;
			lock (this.syncLock)
			{
				result = this.EndSearchSession(sessionId, -1L);
			}
			return result;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (this.isDisposed)
			{
				return;
			}
			MailboxSession mailboxSession = null;
			try
			{
				lock (this.syncLock)
				{
					if (!this.isDisposed)
					{
						this.isDisposed = true;
						if (this.MailboxData != null)
						{
							mailboxSession = this.MailboxData.mailboxSession;
						}
						if (this.currentSession != null)
						{
							this.currentSession.BeginStopSession(-2L);
							this.currentSession = null;
						}
						this.MailboxData = null;
					}
				}
			}
			finally
			{
				if (mailboxSession != null)
				{
					lock (mailboxSession)
					{
						mailboxSession.Dispose();
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<InstantSearchManager>(this);
		}

		private void CreateNewInstantSearchSession(string sessionId, List<StoreId> folderIds, InstantSearchItemType itemType, long searchRequestId, SuggestionSourceType suggestionSourceType)
		{
			if (this.MailboxData == null)
			{
				MailboxSession mailboxSession = this.createMailboxSession();
				this.MailboxData = new InstantSearchMailboxDataSnapshot(mailboxSession);
			}
			this.currentSessionCreationRequestId = searchRequestId;
			InstantSearchSession instantSearchSession = new InstantSearchSession(sessionId, this.MailboxData, folderIds, itemType, suggestionSourceType);
			instantSearchSession.BeginStartSession(searchRequestId);
			this.currentSession = instantSearchSession;
		}

		private EndInstantSearchSessionResponse EndSearchSession(string sessionId, long searchRequestId)
		{
			EndInstantSearchSessionResponse result = null;
			if (this.currentSession != null && this.currentSession.SessionId == sessionId)
			{
				result = this.currentSession.BeginStopSession(searchRequestId);
				this.currentSession = null;
				this.currentSessionCreationRequestId = -1L;
			}
			return result;
		}

		public InstantSearchMailboxDataSnapshot MailboxData { get; set; }

		private readonly object syncLock = new object();

		private readonly Func<MailboxSession> createMailboxSession;

		private volatile bool isDisposed;

		private volatile InstantSearchSession currentSession;

		private long currentSessionCreationRequestId;
	}
}
