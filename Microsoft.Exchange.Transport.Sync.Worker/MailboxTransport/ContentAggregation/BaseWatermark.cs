using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class BaseWatermark
	{
		protected BaseWatermark(SyncLogSession syncLogSession, string mailboxServerSyncWatermark, ISimpleStateStorage stateStorage, bool loadedFromMailboxServer)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			if (loadedFromMailboxServer)
			{
				SyncUtilities.ThrowIfArgumentNull("mailboxServerSyncWatermark", mailboxServerSyncWatermark);
			}
			else
			{
				SyncUtilities.ThrowIfArgumentNull("stateStorage", stateStorage);
			}
			this.mailboxServerSyncWatermark = mailboxServerSyncWatermark;
			this.stateStorage = stateStorage;
			this.syncLogSession = syncLogSession;
			this.loadedFromMailboxServer = loadedFromMailboxServer;
		}

		protected ISimpleStateStorage StateStorage
		{
			get
			{
				return this.stateStorage;
			}
		}

		protected string MailboxServerSyncWatermark
		{
			get
			{
				return this.mailboxServerSyncWatermark;
			}
			set
			{
				this.mailboxServerSyncWatermark = value;
			}
		}

		protected SyncLogSession SyncLogSession
		{
			get
			{
				return this.syncLogSession;
			}
		}

		protected bool LoadedFromMailboxServer
		{
			get
			{
				return this.loadedFromMailboxServer;
			}
		}

		protected string StateStorageEncodedSyncWatermark
		{
			get
			{
				return this.stateStorageEncodedSyncWatermark;
			}
			set
			{
				SyncUtilities.ThrowIfArgumentNull("StateStorageEncodedSyncWatermark", value);
				this.isWatermarkUpdated = true;
				this.stateStorageEncodedSyncWatermark = value;
			}
		}

		public bool IsSyncWatermarkUpdated
		{
			get
			{
				return this.isWatermarkUpdated;
			}
		}

		public override string ToString()
		{
			if (this.LoadedFromMailboxServer)
			{
				return this.mailboxServerSyncWatermark;
			}
			return this.stateStorageEncodedSyncWatermark;
		}

		private readonly ISimpleStateStorage stateStorage;

		private readonly SyncLogSession syncLogSession;

		private readonly bool loadedFromMailboxServer;

		private string stateStorageEncodedSyncWatermark;

		private string mailboxServerSyncWatermark;

		private bool isWatermarkUpdated;
	}
}
