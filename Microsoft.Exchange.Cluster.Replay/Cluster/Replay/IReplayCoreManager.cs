using System;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IReplayCoreManager : IServiceComponent
	{
		ReplicaInstanceManager ReplicaInstanceManager { get; }

		ReplaySystemQueue SystemQueue { get; }

		IRunConfigurationUpdater ConfigurationUpdater { get; }

		DumpsterRedeliveryManager DumpsterRedeliveryManager { get; }

		SkippedLogsDeleter SkippedLogsDeleter { get; }

		AmSearchServiceMonitor SearchServiceMonitor { get; }
	}
}
