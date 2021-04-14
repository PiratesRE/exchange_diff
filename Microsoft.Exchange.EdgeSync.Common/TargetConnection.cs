using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.MessageSecurity.EdgeSync;

namespace Microsoft.Exchange.EdgeSync
{
	internal abstract class TargetConnection : IDisposable
	{
		public TargetConnection(int localServerVersion, TargetServerConfig config)
		{
			this.localServerVersion = localServerVersion;
			this.host = config.Host;
			this.port = config.Port;
		}

		public int LocalServerVersion
		{
			get
			{
				return this.localServerVersion;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
		}

		public string Host
		{
			get
			{
				return this.host;
			}
		}

		public virtual bool SkipSyncCycle
		{
			get
			{
				return false;
			}
		}

		public abstract bool OnSynchronizing();

		public abstract void OnConnectedToSource(Connection sourceConnection);

		public abstract bool OnSynchronized();

		public abstract SyncResult OnAddEntry(ExSearchResultEntry entry, SortedList<string, DirectoryAttribute> sourceAttributes);

		public abstract SyncResult OnModifyEntry(ExSearchResultEntry entry, SortedList<string, DirectoryAttribute> sourceAttributes);

		public abstract SyncResult OnDeleteEntry(ExSearchResultEntry entry);

		public abstract SyncResult OnRenameEntry(ExSearchResultEntry entry);

		public abstract bool TryReadCookie(out Dictionary<string, Cookie> cookies);

		public abstract bool TrySaveCookie(Dictionary<string, Cookie> cookies);

		public abstract LeaseToken GetLease();

		public abstract bool CanTakeOverLease(bool force, LeaseToken lease, DateTime now);

		public abstract void SetLease(LeaseToken newLeaseToken);

		public virtual void Dispose()
		{
		}

		private string host;

		private int port;

		private int localServerVersion;
	}
}
