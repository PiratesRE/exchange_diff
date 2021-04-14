using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Logging;

namespace Microsoft.Exchange.EdgeSync
{
	internal abstract class SynchronizationProvider
	{
		public abstract string Identity { get; }

		public abstract List<TargetServerConfig> TargetServerConfigs { get; }

		public abstract int LeaseLockTryCount { get; }

		public abstract EnhancedTimeSpan RecipientSyncInterval { get; }

		public abstract EnhancedTimeSpan ConfigurationSyncInterval { get; }

		public abstract void Initialize(EdgeSyncConnector connector);

		public abstract List<TypeSynchronizer> CreateTypeSynchronizer(SyncTreeType type);

		public abstract TargetConnection CreateTargetConnection(TargetServerConfig targetServerConfig, SyncTreeType type, TestShutdownAndLeaseDelegate testShutdownAndLease, EdgeSyncLogSession logSession);
	}
}
