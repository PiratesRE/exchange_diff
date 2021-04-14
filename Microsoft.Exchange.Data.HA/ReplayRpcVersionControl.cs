using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ReplayRpcVersionControl
	{
		public static bool IsGetCopyStatusEx2RpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.GetCopyStatusEx2SupportVersion);
		}

		public static bool IsSuspendRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.SuspendRpcSupportVersion);
		}

		public static bool IsSeedRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.SeedRpcSupportVersion);
		}

		public static bool IsSeedRpcSafeDeleteSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.SeedRpcSafeDeleteSupportVersion);
		}

		public static bool IsSeedRpcV5Supported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.SeedRpcV5SupportVersion);
		}

		public static bool IsRunConfigUpdaterRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.RunConfigUpdaterRpcSupportVersion);
		}

		public static bool IsActivationRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.ActivationRpcSupportVersion);
		}

		public static bool IsNotifyChangedReplayConfigurationRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.NotifyChangedReplayConfigurationSupportVersion);
		}

		public static bool IsRequestSuspend3RpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.RequestSuspend3SupportVersion);
		}

		public static bool IsSeedFromPassiveSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.SeedFromPassiveSupportVersion);
		}

		public static bool IsGetCopyStatusEx4RpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.GetCopyStatusEx4RpcSupportVersion);
		}

		public static bool IsGetCopyStatusBasicRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.GetCopyStatusEx4RpcSupportVersion);
		}

		public static bool IsDisableReplayLagRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.GetCopyStatusEx4RpcSupportVersion);
		}

		public static bool IsDisableReplayLagRpcV2Supported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.DisableReplayLagRpcV2SupportVersion);
		}

		public static bool IsGetCopyStatusWithHealthStateRpcSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.IsGetCopyStatusWithHealthStateRpcSupportVersion);
		}

		public static bool IsServerLocatorServiceSupported(ServerVersion serverVersion)
		{
			return ReplayRpcVersionControl.IsVersionGreater(serverVersion, ReplayRpcVersionControl.IsServerLocatorServiceSupportVersion);
		}

		public static bool IsVersionGreater(ServerVersion a, ServerVersion b)
		{
			return ServerVersion.Compare(a, b) >= 0;
		}

		public static readonly ServerVersion SuspendRpcSupportVersion = new ServerVersion(14, 0, 145, 0);

		public static readonly ServerVersion SeedRpcSupportVersion = new ServerVersion(14, 0, 267, 0);

		public static readonly ServerVersion SeedRpcSafeDeleteSupportVersion = new ServerVersion(15, 0, 585, 0);

		public static readonly ServerVersion SeedRpcV5SupportVersion = new ServerVersion(15, 0, 645, 0);

		public static readonly ServerVersion DisableReplayLagRpcV2SupportVersion = new ServerVersion(15, 0, 691, 0);

		public static readonly ServerVersion GetCopyStatusEx2SupportVersion = new ServerVersion(14, 0, 288, 0);

		public static readonly ServerVersion RunConfigUpdaterRpcSupportVersion = new ServerVersion(14, 0, 324, 0);

		public static readonly ServerVersion ActivationRpcSupportVersion = new ServerVersion(14, 0, 408, 0);

		public static readonly ServerVersion GetCopyStatusEx3SupportVersion = new ServerVersion(14, 0, 455, 0);

		public static readonly ServerVersion NotifyChangedReplayConfigurationSupportVersion = new ServerVersion(14, 0, 572, 0);

		public static readonly ServerVersion RequestSuspend3SupportVersion = new ServerVersion(14, 0, 572, 0);

		public static readonly ServerVersion SeedFromPassiveSupportVersion = new ServerVersion(14, 0, 572, 0);

		public static readonly ServerVersion GetCopyStatusEx4RpcSupportVersion = new ServerVersion(15, 0, 202, 0);

		public static readonly ServerVersion IsGetCopyStatusWithHealthStateRpcSupportVersion = new ServerVersion(15, 0, 339, 0);

		public static readonly ServerVersion IsServerLocatorServiceSupportVersion = new ServerVersion(15, 0, 413, 0);
	}
}
