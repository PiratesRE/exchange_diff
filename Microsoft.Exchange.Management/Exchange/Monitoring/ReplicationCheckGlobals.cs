using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ReplicationCheckGlobals
	{
		internal static bool RunningInMonitoringContext
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["RunningInMonitoringContext"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["RunningInMonitoringContext"] = value;
			}
		}

		internal static ServerConfig ServerConfiguration
		{
			get
			{
				return (ServerConfig)(ReplicationCheckGlobals.s_fields["ServerConfiguration"] ?? ServerConfig.Unknown);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["ServerConfiguration"] = value;
			}
		}

		internal static bool UsingReplayRpc
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["UsingReplayRpc"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["UsingReplayRpc"] = value;
			}
		}

		internal static bool ReplayServiceCheckHasRun
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["ReplayServiceCheckHasRun"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["ReplayServiceCheckHasRun"] = value;
			}
		}

		internal static bool ActiveManagerCheckHasRun
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["ActiveManagerCheckHasRun"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["ActiveManagerCheckHasRun"] = value;
			}
		}

		internal static bool ThirdPartyReplCheckHasRun
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["ThirdPartyReplCheckHasRun"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["ThirdPartyReplCheckHasRun"] = value;
			}
		}

		internal static bool TasksRpcListenerCheckHasRun
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["TasksRpcListenerCheckHasRun"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["TasksRpcListenerCheckHasRun"] = value;
			}
		}

		internal static bool TcpListenerCheckHasRun
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["TcpListenerCheckHasRun"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["TcpListenerCheckHasRun"] = value;
			}
		}

		internal static bool DatabaseRedundancyCheckHasRun
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["DatabaseRedundancyCheckHasRun"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["DatabaseRedundancyCheckHasRun"] = value;
			}
		}

		internal static bool DatabaseAvailabilityCheckHasRun
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["DatabaseAvailabilityCheckHasRun"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["DatabaseAvailabilityCheckHasRun"] = value;
			}
		}

		internal static bool ServerLocatorServiceCheckHasRun
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["ServerLocatorServiceCheckHasRun"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["ServerLocatorServiceCheckHasRun"] = value;
			}
		}

		internal static bool MonitoringServiceCheckHasRun
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["MonitoringServiceCheckHasRun"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["MonitoringServiceCheckHasRun"] = value;
			}
		}

		internal static bool IsReplayServiceDown
		{
			get
			{
				return (bool)(ReplicationCheckGlobals.s_fields["IsReplayServiceDown"] ?? false);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["IsReplayServiceDown"] = value;
			}
		}

		internal static AmRole ActiveManagerRole
		{
			get
			{
				return (AmRole)(ReplicationCheckGlobals.s_fields["ActiveManagerRole"] ?? AmRole.Unknown);
			}
			set
			{
				ReplicationCheckGlobals.s_fields["ActiveManagerRole"] = value;
			}
		}

		internal static IADServer Server
		{
			get
			{
				return (IADServer)ReplicationCheckGlobals.s_fields["Server"];
			}
			set
			{
				ReplicationCheckGlobals.s_fields["Server"] = value;
			}
		}

		internal static Dictionary<Guid, RpcDatabaseCopyStatus2> CopyStatusResults
		{
			get
			{
				return (Dictionary<Guid, RpcDatabaseCopyStatus2>)ReplicationCheckGlobals.s_fields["CopyStatusResults"];
			}
			set
			{
				ReplicationCheckGlobals.s_fields["CopyStatusResults"] = value;
			}
		}

		internal static Task.TaskVerboseLoggingDelegate WriteVerboseDelegate
		{
			get
			{
				return (Task.TaskVerboseLoggingDelegate)ReplicationCheckGlobals.s_fields["WriteVerboseDelegate"];
			}
			set
			{
				ReplicationCheckGlobals.s_fields["WriteVerboseDelegate"] = value;
			}
		}

		internal static void ResetState()
		{
			ReplicationCheckGlobals.s_fields.Clear();
		}

		private const int NumberOfFields = 16;

		private static HybridDictionary s_fields = new HybridDictionary(16, true);
	}
}
