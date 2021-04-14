using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CopyStatusClientCachedEntry : ICopyStatusCachedEntry
	{
		public RpcDatabaseCopyStatus2 CopyStatus { get; set; }

		internal Guid DbGuid { get; private set; }

		internal AmServerName ServerContacted { get; private set; }

		internal AmServerName ActiveServer { get; set; }

		internal DateTime CreateTimeUtc { get; private set; }

		internal TimeSpan RpcDuration { get; set; }

		internal Exception LastException { get; set; }

		internal CopyStatusRpcResult Result { get; set; }

		internal bool IsActive
		{
			get
			{
				return this.ServerContacted.Equals(this.ActiveServer);
			}
		}

		internal bool IsLocalCopy
		{
			get
			{
				return this.ServerContacted.IsLocalComputerName;
			}
		}

		internal CopyStatusClientCachedEntry(Guid dbGuid, AmServerName serverContacted)
		{
			this.CreateTimeUtc = DateTime.UtcNow;
			this.DbGuid = dbGuid;
			this.ServerContacted = serverContacted;
		}

		internal void TestSetCreateTimeUtc(DateTime time)
		{
			this.CreateTimeUtc = time;
		}
	}
}
