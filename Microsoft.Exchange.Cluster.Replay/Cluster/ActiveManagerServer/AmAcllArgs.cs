using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmAcllArgs
	{
		public AmAcllArgs()
		{
			this.MountDialOverride = DatabaseMountDialOverride.Lossless;
			this.SkipValidationChecks = AmBcsSkipFlags.None;
			this.MountPending = true;
		}

		public int NumRetries { get; set; }

		public int E00TimeoutMs { get; set; }

		public int NetworkIOTimeoutMs { get; set; }

		public int NetworkConnectTimeoutMs { get; set; }

		public bool MountPending { get; set; }

		public AmServerName SourceServer { get; set; }

		public AmDbActionCode ActionCode { get; set; }

		public AmBcsSkipFlags SkipValidationChecks { get; set; }

		public DatabaseMountDialOverride MountDialOverride { get; set; }

		public string UniqueOperationId { get; set; }

		public int SubactionAttemptNumber { get; set; }
	}
}
