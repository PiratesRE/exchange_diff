using System;
using System.Threading.Tasks;
using Microsoft.Exchange.Cluster.Replay.Monitoring;
using Microsoft.Exchange.Cluster.Replay.Monitoring.Client;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal class MonitoringServiceCheck : ReplicationCheck
	{
		public MonitoringServiceCheck(string serverName, IEventManager eventManager, string momeventsource) : base("MonitoringService", CheckId.MonitoringService, Strings.MonitoringServiceCheckDesc, CheckCategory.SystemMediumPriority, eventManager, momeventsource, serverName)
		{
		}

		public MonitoringServiceCheck(string serverName, IEventManager eventManager, string momeventsource, uint? ignoreTransientErrorsThreshold) : base("MonitoringService", CheckId.MonitoringService, Strings.MonitoringServiceCheckDesc, CheckCategory.SystemMediumPriority, eventManager, momeventsource, serverName, ignoreTransientErrorsThreshold)
		{
		}

		protected override void InternalRun()
		{
			if (!ReplicationCheckGlobals.MonitoringServiceCheckHasRun)
			{
				ReplicationCheckGlobals.MonitoringServiceCheckHasRun = true;
			}
			else
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "MonitoringServiceCheck skipping because it has already been run once.");
				base.Skip();
			}
			if ((ReplicationCheckGlobals.ServerConfiguration & ServerConfig.Stopped) == ServerConfig.Stopped)
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "Stopped server! Skipping {0}.", base.Title);
				base.Skip();
			}
			TimeSpan timeout = TimeSpan.FromSeconds(5.0);
			Exception ex = MonitoringServiceClient.HandleException(delegate
			{
				using (MonitoringServiceClient monitoringServiceClient = MonitoringServiceClient.Open(this.ServerName, timeout, timeout, timeout, timeout))
				{
					Task<ServiceVersion> versionAsync = monitoringServiceClient.GetVersionAsync();
					if (!versionAsync.Wait(timeout))
					{
						ExTraceGlobals.HealthChecksTracer.TraceError<string, TimeSpan>((long)this.GetHashCode(), "MonitoringServiceCheck: GetVersionAsync() call to server '{0}' timed out after '{1}'", this.ServerName, timeout);
						this.Fail(Strings.MonitoringServiceRequestTimedout(this.ServerName, timeout));
					}
					else
					{
						ExTraceGlobals.HealthChecksTracer.TraceDebug<string, long>((long)this.GetHashCode(), "MonitoringServiceCheck: GetVersionAsync() call to server '{0}' returned: {1}", this.ServerName, versionAsync.Result.Version);
					}
				}
			});
			if (ex != null)
			{
				ExTraceGlobals.HealthChecksTracer.TraceError<string, Exception>((long)this.GetHashCode(), "MonitoringServiceCheck: GetVersionAsync() call to server '{0}' failed with exception: {1}", base.ServerName, ex);
				base.Fail(Strings.MonitoringServiceRequestFailed(base.ServerName, ex.Message));
			}
		}
	}
}
