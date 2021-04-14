using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ComponentFinder : IFindComponent
	{
		public MonitoredDatabase FindMonitoredDatabase(string nodeName, Guid dbGuid)
		{
			return RemoteDataProvider.GetMonitoredDatabase(dbGuid);
		}

		public LogCopier FindLogCopier(string nodeName, Guid dbGuid)
		{
			ReplicaInstanceManager replicaInstanceManager = Dependencies.ReplayCoreManager.ReplicaInstanceManager;
			ReplicaInstance replicaInstance = null;
			LogCopier logCopier = null;
			if (replicaInstanceManager.TryGetReplicaInstance(dbGuid, out replicaInstance))
			{
				logCopier = replicaInstance.GetComponent<LogCopier>();
				if (logCopier == null)
				{
					ComponentFinder.Tracer.TraceError<Guid>(0L, "FindLogCopier failed to find LogCopier for database {0}", dbGuid);
				}
			}
			else
			{
				ComponentFinder.Tracer.TraceError<Guid>(0L, "FindLogCopier failed to find RI database {0}", dbGuid);
			}
			return logCopier;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ReplayManagerTracer;
	}
}
