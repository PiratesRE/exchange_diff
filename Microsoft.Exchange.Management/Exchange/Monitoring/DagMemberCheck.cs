using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Monitoring
{
	internal abstract class DagMemberCheck : ReplicationCheck
	{
		public DagMemberCheck(string serverName, string title, CheckId checkId, LocalizedString description, CheckCategory checkCategory, IEventManager eventManager, string momeventsource, IADDatabaseAvailabilityGroup dag, bool fClusterLevelCheck) : this(serverName, title, checkId, description, checkCategory, eventManager, momeventsource, null, dag, fClusterLevelCheck)
		{
		}

		public DagMemberCheck(string serverName, string title, CheckId checkId, LocalizedString description, CheckCategory checkCategory, IEventManager eventManager, string momeventsource, uint? ignoreTransientErrorsThreshold, IADDatabaseAvailabilityGroup dag, bool fClusterLevelCheck) : base(title, checkId, description, checkCategory, eventManager, momeventsource, serverName, ignoreTransientErrorsThreshold)
		{
			this.m_dag = dag;
			this.m_fClusterLevelCheck = fClusterLevelCheck;
		}

		public AmCluster Cluster
		{
			get
			{
				if (this.m_cluster == null)
				{
					AmServerName serverName = new AmServerName(base.ServerName);
					try
					{
						this.m_cluster = AmCluster.OpenByName(serverName);
						if (this.m_cluster == null)
						{
							base.Fail(Strings.CouldNotConnectToCluster(base.ServerName));
						}
					}
					catch (ClusterException ex)
					{
						base.Fail(Strings.CouldNotConnectToClusterError(base.ServerName, ex.Message));
					}
				}
				return this.m_cluster;
			}
		}

		protected override void InternalRun()
		{
			AmServerName serverName = new AmServerName(base.ServerName);
			if (DagHelper.IsNodeClustered(serverName))
			{
				if (!IgnoreTransientErrors.HasPassed(base.GetDefaultErrorKey(typeof(ClusterRpcCheck))))
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "ClusterRpcCheck didn't pass! Skipping check {0}.", base.Title);
					base.Skip();
				}
				if (this.m_fClusterLevelCheck)
				{
					this.RunCheck();
					return;
				}
				using (IEnumerator<AmServerName> enumerator = this.Cluster.EnumerateNodeNames().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AmServerName amServerName = enumerator.Current;
						try
						{
							if (this.IsNodeMemberOfDag(amServerName, this.m_dag))
							{
								using (IAmClusterNode amClusterNode = this.Cluster.OpenNode(amServerName))
								{
									if (this.RunIndividualCheck(amClusterNode))
									{
										base.ReportPassedInstance();
									}
									continue;
								}
							}
							ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Cluster node {0} is not a member of DAG {1} so check will not run here.", amServerName.NetbiosName, this.m_dag.Name);
						}
						finally
						{
							base.InstanceIdentity = null;
						}
					}
					return;
				}
			}
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "Local machine is not clustered! Skipping check {0}.", base.Title);
			base.Skip();
		}

		private bool IsNodeMemberOfDag(AmServerName node, IADDatabaseAvailabilityGroup dag)
		{
			foreach (ADObjectId adobjectId in dag.Servers)
			{
				if (SharedHelper.StringIEquals(node.NetbiosName, adobjectId.Name))
				{
					return true;
				}
			}
			return false;
		}

		protected bool IsNodeStopped(AmServerName node)
		{
			return this.m_dag.StoppedMailboxServers.Contains(node.Fqdn);
		}

		protected virtual void RunCheck()
		{
		}

		protected virtual bool RunIndividualCheck(IAmClusterNode node)
		{
			return true;
		}

		protected void SkipOnSamNodeIfMonitoringContext()
		{
			if (ReplicationCheckGlobals.RunningInMonitoringContext && (ReplicationCheckGlobals.ServerConfiguration & ServerConfig.DagMember) == ServerConfig.DagMember)
			{
				if (!IgnoreTransientErrors.HasPassed(base.GetDefaultErrorKey(typeof(ActiveManagerCheck))))
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "ActiveManagerCheck didn't pass! Skipping check {0}.", base.Title);
					base.Skip();
				}
				if (ReplicationCheckGlobals.ActiveManagerRole == AmRole.SAM)
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "SkipOnSAMNode(): Local machine is the SAM. Skipping check {0}.", base.Title);
					base.Skip();
					return;
				}
				if (ReplicationCheckGlobals.ActiveManagerRole == AmRole.PAM)
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "SkipOnSAMNode(): Local machine is the PAM. Checks will be run here.");
					return;
				}
				AmRole activeManagerRole = ReplicationCheckGlobals.ActiveManagerRole;
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			base.InternalDispose(calledFromDispose);
			if (this.m_cluster != null)
			{
				this.m_cluster.Dispose();
			}
		}

		protected readonly IADDatabaseAvailabilityGroup m_dag;

		private AmCluster m_cluster;

		private readonly bool m_fClusterLevelCheck;
	}
}
