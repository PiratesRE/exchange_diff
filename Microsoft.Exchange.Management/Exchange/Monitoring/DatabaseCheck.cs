using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Monitoring
{
	internal abstract class DatabaseCheck : ReplicationCheck
	{
		public DatabaseCheck(string serverName, string title, CheckId checkId, LocalizedString description, IEventManager eventManager, string momEventSource, List<ReplayConfiguration> replayConfigs, Dictionary<Guid, RpcDatabaseCopyStatus2> copyStatuses) : this(title, checkId, description, eventManager, momEventSource, replayConfigs, copyStatuses, serverName, new uint?(0U))
		{
		}

		public DatabaseCheck(string title, CheckId checkId, LocalizedString description, IEventManager eventManager, string momEventSource, List<ReplayConfiguration> replayConfigs, Dictionary<Guid, RpcDatabaseCopyStatus2> copyStatuses, string mailboxServer, uint? ignoreTransientErrorsThreshold) : base(title, checkId, description, CheckCategory.Database, eventManager, momEventSource, mailboxServer, ignoreTransientErrorsThreshold)
		{
			this.m_ReplayConfigs = replayConfigs;
			this.m_CopyStatuses = copyStatuses;
		}

		protected abstract bool RunIndividualCheck(ReplayConfiguration configToCheck, RpcDatabaseCopyStatus2 copyStatus);

		protected virtual bool ShouldCheckConfig(ReplayConfiguration replayconfig)
		{
			return replayconfig.Type == ReplayConfigType.RemoteCopyTarget;
		}

		internal virtual bool UseReplayRpc()
		{
			return ReplicationCheckGlobals.UsingReplayRpc;
		}

		protected override void InternalRun()
		{
			if (this.m_ReplayConfigs.Count == 0)
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "InternalRun(): m_ReplayConfigs has no entries. Returning from check.");
				return;
			}
			foreach (ReplayConfiguration replayConfiguration in this.m_ReplayConfigs)
			{
				base.InstanceIdentity = replayConfiguration.IdentityGuid.ToString().ToLowerInvariant();
				if (this.ShouldCheckConfig(replayConfiguration))
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "InternalRun(): ReplayConfiguration '{0}' of type '{1}' on server '{2}' will now be processed by check '{3}'.", new object[]
					{
						replayConfiguration.DisplayName,
						replayConfiguration.Type.ToString(),
						replayConfiguration.ServerName,
						base.Title
					});
					try
					{
						if (this.UseReplayRpc())
						{
							RpcDatabaseCopyStatus2 copyStatus = null;
							if ((this.m_CopyStatuses != null && this.m_CopyStatuses.TryGetValue(replayConfiguration.IdentityGuid, out copyStatus)) || replayConfiguration.Type == ReplayConfigType.RemoteCopySource)
							{
								if (this.RunIndividualCheck(replayConfiguration, copyStatus))
								{
									base.ReportPassedInstance();
								}
							}
							else
							{
								base.FailContinue(Strings.DatabaseCopyRpcResultNotFound(new LocalizedReplayConfigType(replayConfiguration.Type).ToString(), replayConfiguration.DisplayName));
							}
						}
						else if (this.RunIndividualCheck(replayConfiguration, null))
						{
							base.ReportPassedInstance();
						}
						continue;
					}
					finally
					{
						base.InstanceIdentity = null;
					}
				}
				ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "InternalRun(): ReplayConfiguration '{0}' of type '{1}' on server '{2}' is being skipped by check '{3}'.", new object[]
				{
					replayConfiguration.DisplayName,
					replayConfiguration.Type.ToString(),
					replayConfiguration.ServerName,
					base.Title
				});
			}
		}

		protected List<ReplayConfiguration> m_ReplayConfigs;

		protected Dictionary<Guid, RpcDatabaseCopyStatus2> m_CopyStatuses;
	}
}
