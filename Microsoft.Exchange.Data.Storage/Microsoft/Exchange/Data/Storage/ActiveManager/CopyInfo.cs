using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct CopyInfo
	{
		public DatabaseCopy Copy;

		public RpcDatabaseCopyStatus2 Status;
	}
}
