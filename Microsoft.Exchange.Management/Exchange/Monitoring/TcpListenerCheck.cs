using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal class TcpListenerCheck : ReplicationCheck
	{
		public TcpListenerCheck(string serverName, IEventManager eventManager, string momeventsource, IADDatabaseAvailabilityGroup dag) : base("TcpListener", CheckId.TcpListener, Strings.TcpListenerCheckDesc, CheckCategory.SystemHighPriority, eventManager, momeventsource, serverName)
		{
			if (dag != null)
			{
				this.replicationPort = dag.ReplicationPort;
			}
		}

		public TcpListenerCheck(string serverName, IEventManager eventManager, string momeventsource, uint? ignoreTransientErrorsThreshold, IADDatabaseAvailabilityGroup dag) : base("TcpListener", CheckId.TcpListener, Strings.TcpListenerCheckDesc, CheckCategory.SystemHighPriority, eventManager, momeventsource, serverName, ignoreTransientErrorsThreshold)
		{
			if (dag != null)
			{
				this.replicationPort = dag.ReplicationPort;
			}
		}

		protected override void InternalRun()
		{
			if (!ReplicationCheckGlobals.TcpListenerCheckHasRun)
			{
				ReplicationCheckGlobals.TcpListenerCheckHasRun = true;
			}
			else
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "TcpListenerCheck skipping because it has already been run once.");
				base.Skip();
			}
			if (!IgnoreTransientErrors.HasPassed(base.GetDefaultErrorKey(typeof(ReplayServiceCheck))))
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "ReplayServiceCheck didn't pass! Skipping {0}.", base.Title);
				base.Skip();
			}
			if ((ReplicationCheckGlobals.ServerConfiguration & ServerConfig.Stopped) == ServerConfig.Stopped)
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "Stopped server! Skipping {0}.", base.Title);
				base.Skip();
			}
			string text = null;
			bool flag = TcpHealthCheck.TestHealth(base.ServerName, (int)this.replicationPort, 5000, out text);
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string, bool, string>((long)this.GetHashCode(), "TcpListenerCheck: TestHealth() for server '{0}' returned: healthy={1}, errMsg='{2}'", base.ServerName, flag, text);
			if (!flag)
			{
				base.Fail(Strings.TcpListenerRequestFailed(base.ServerName, text));
			}
		}

		private const int TimeOutMs = 5000;

		private readonly ushort replicationPort = 64327;
	}
}
