using System;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AmRpcVersionControl
	{
		public static bool IsRemountRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.RemountRpcSupportedVersion);
		}

		public static bool IsReportSystemEventRpcSupported(ServerVersion serverVersion, AmSystemEventCode eventCode)
		{
			if (eventCode == AmSystemEventCode.StoreServiceStarted || eventCode == AmSystemEventCode.StoreServiceStopped)
			{
				return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.ReportSystemEventRpcSupportedVersion);
			}
			return eventCode == AmSystemEventCode.StoreServiceUnexpectedlyStopped && ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.UnexpectedStoreStopEventCodeSupportedVersion);
		}

		public static bool IsSwitchOverSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.SwitchOverSupportVersion);
		}

		public static bool IsGetAmRoleRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.GetAMRoleSupportVersion);
		}

		public static bool IsAttemptCopyLastLogsDirect2RpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.AttemptCopyLastLogsDirect2SupportVersion);
		}

		public static bool IsServerMoveAllDatabasesRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.ServerMoveAllDatabasesRpcSupportVersion);
		}

		public static bool IsMoveDatabaseEx2RpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.MoveDatabaseEx2RpcSupportVersion);
		}

		public static bool IsMoveDatabaseEx3RpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.MoveDatabaseEx3RpcSupportVersion);
		}

		public static bool IsCheckThirdPartyListenerSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.ThirdPartyReplListenerSupportedVersion);
		}

		public static bool IsAmRefreshConfigurationSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.AmRefreshConfigurationRpcSupportVersion);
		}

		public static bool IsAttemptCopyLastLogsDirect3RpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.AttemptCopyLastLogsDirect3SupportVersion);
		}

		public static bool IsMountWithAmFlagsRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.MountWithAmFlagsSupportedVersion);
		}

		public static bool IsMoveWithCatalogFailureReasonCodeSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.MoveWithCatalogFailureReasonCodeVersion);
		}

		public static bool IsReportServiceKillRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.ReportServiceKillSupportedVersion);
		}

		public static bool IsGetDeferredRecoveryEntriesRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.GetDeferredRecoveryEntriesRpcSupportedSupportedVersion);
		}

		public static bool IsServerMoveAllDatabases3RpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.AmServerMoveAllDatabases3RpcSupportedVersion);
		}

		public static bool IsServerMoveBackDatabasesRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, AmRpcVersionControl.AmServerMoveBackDatabasesRpcSupportedVersion);
		}

		public static readonly ServerVersion SwitchOverSupportVersion = new ServerVersion(14, 0, 244, 0);

		public static readonly ServerVersion GetAMRoleSupportVersion = new ServerVersion(14, 0, 288, 0);

		public static readonly ServerVersion ReportSystemEventRpcSupportedVersion = new ServerVersion(14, 0, 455, 0);

		public static readonly ServerVersion RemountRpcSupportedVersion = new ServerVersion(14, 0, 464, 0);

		public static readonly ServerVersion UnexpectedStoreStopEventCodeSupportedVersion = new ServerVersion(14, 0, 522, 0);

		public static readonly ServerVersion AttemptCopyLastLogsDirect2SupportVersion = new ServerVersion(14, 0, 572, 0);

		public static readonly ServerVersion ThirdPartyReplListenerSupportedVersion = new ServerVersion(14, 0, 572, 0);

		public static readonly ServerVersion ServerMoveAllDatabasesRpcSupportVersion = new ServerVersion(14, 0, 579, 0);

		public static readonly ServerVersion MoveDatabaseEx2RpcSupportVersion = new ServerVersion(14, 0, 579, 0);

		public static readonly ServerVersion AmRefreshConfigurationRpcSupportVersion = new ServerVersion(14, 0, 601, 0);

		public static readonly ServerVersion AttemptCopyLastLogsDirect3SupportVersion = new ServerVersion(14, 0, 636, 0);

		public static readonly ServerVersion MoveDatabaseEx3RpcSupportVersion = new ServerVersion(14, 1, 115, 0);

		public static readonly ServerVersion MountWithAmFlagsSupportedVersion = new ServerVersion(14, 1, 135, 0);

		public static readonly ServerVersion MoveWithCatalogFailureReasonCodeVersion = new ServerVersion(14, 2, 5038, 0);

		public static readonly ServerVersion ReportServiceKillSupportedVersion = new ServerVersion(14, 2, 5114, 0);

		public static readonly ServerVersion GetDeferredRecoveryEntriesRpcSupportedSupportedVersion = new ServerVersion(14, 2, 5114, 0);

		public static readonly ServerVersion AmServerMoveAllDatabases3RpcSupportedVersion = new ServerVersion(15, 0, 337, 0);

		public static readonly ServerVersion AmServerMoveBackDatabasesRpcSupportedVersion = new ServerVersion(15, 0, 997, 0);
	}
}
