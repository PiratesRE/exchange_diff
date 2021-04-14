using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ActiveManagerCheck : ReplicationCheck
	{
		public ActiveManagerCheck(string serverName, IEventManager eventManager, string momeventsource) : base("ActiveManager", CheckId.ActiveManager, Strings.ActiveManagerCheckDesc, CheckCategory.SystemHighPriority, eventManager, momeventsource, serverName)
		{
		}

		public ActiveManagerCheck(string serverName, IEventManager eventManager, string momeventsource, uint? ignoreTransientErrorsThreshold) : base("ActiveManager", CheckId.ActiveManager, Strings.ActiveManagerCheckDesc, CheckCategory.SystemHighPriority, eventManager, momeventsource, serverName, ignoreTransientErrorsThreshold)
		{
		}

		protected override void InternalRun()
		{
			if (!ReplicationCheckGlobals.ActiveManagerCheckHasRun)
			{
				ReplicationCheckGlobals.ActiveManagerCheckHasRun = true;
			}
			else
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "ActiveManagerCheck skipping because it has already been run once.");
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
			string error = null;
			AmRole amRole = AmRole.Unknown;
			try
			{
				if (AmRpcVersionControl.IsGetAmRoleRpcSupported(ReplicationCheckGlobals.Server.AdminDisplayVersion))
				{
					if (ReplicationCheckGlobals.WriteVerboseDelegate != null)
					{
						ReplicationCheckGlobals.WriteVerboseDelegate(Strings.TestRHGetAmRoleRpc(ReplicationCheckGlobals.Server.AdminDisplayVersion.ToString(), AmRpcVersionControl.GetAMRoleSupportVersion.ToString()));
					}
					amRole = AmRpcClientHelper.GetActiveManagerRole(base.ServerName, out error);
					ReplicationCheckGlobals.ActiveManagerRole = amRole;
				}
				else
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "ActiveManagerCheck skipping because server {0} doesn't support the GetActiveManagerRole RPC.", base.ServerName);
					base.Skip();
				}
			}
			catch (AmServerException ex)
			{
				base.Fail(Strings.ErrorReadingAMRole(base.ServerName, ex.Message));
			}
			catch (AmServerTransientException ex2)
			{
				base.Fail(Strings.ErrorReadingAMRole(base.ServerName, ex2.Message));
			}
			ExTraceGlobals.HealthChecksTracer.TraceDebug<AmRole, string>((long)this.GetHashCode(), "AM has role {0} on server {1}.", amRole, base.ServerName);
			if (amRole == AmRole.Unknown)
			{
				base.Fail(Strings.AmUnknownRole(base.ServerName, error));
			}
			if ((ReplicationCheckGlobals.ServerConfiguration & ServerConfig.DagMember) == ServerConfig.DagMember)
			{
				if (amRole == AmRole.Standalone)
				{
					base.Fail(Strings.AmInvalidRoleDagServer(base.ServerName));
					return;
				}
			}
			else if (amRole != AmRole.Standalone)
			{
				base.Fail(Strings.AmInvalidRoleStandaloneServer(base.ServerName));
			}
		}
	}
}
