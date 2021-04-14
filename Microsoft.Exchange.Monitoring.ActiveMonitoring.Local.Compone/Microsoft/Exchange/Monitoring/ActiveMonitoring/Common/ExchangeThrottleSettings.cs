using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class ExchangeThrottleSettings : ThrottleSettingsBase
	{
		internal static ExchangeThrottleSettings Instance
		{
			get
			{
				return ExchangeThrottleSettings.lazy.Value;
			}
		}

		private ExchangeThrottleSettings()
		{
			base.Initialize(ExchangeThrottleSettings.BaseSettings, ResponderDefinition.GlobalOverrides, ResponderDefinition.LocalOverrides);
			base.ReportAllThrottleEntriesToCrimson(true);
		}

		public override FixedThrottleEntry ConstructDefaultThrottlingSettings(RecoveryActionId recoveryActionId)
		{
			return new FixedThrottleEntry(recoveryActionId, 60, -1, 1, 60, 4);
		}

		private static bool IsDagGroup(string categoryName)
		{
			return string.Equals(categoryName, "Dag", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsCafeGroup(string categoryName)
		{
			return string.Equals(categoryName, "Cafe", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsDomainControllerGroup(string categoryName)
		{
			return string.Equals(categoryName, "DomainController", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsCentralAdminGroup(string categoryName)
		{
			return string.Equals(categoryName, "CentralAdmin", StringComparison.OrdinalIgnoreCase);
		}

		public override string[] GetServersInGroup(string categoryName)
		{
			return Dependencies.ThrottleHelper.GetServersInGroup(categoryName);
		}

		internal static string[] ResolveKnownExchangeGroupToServers(string categoryName)
		{
			if (ExchangeThrottleSettings.IsDagGroup(categoryName))
			{
				return DagUtils.GetMailboxServersInSameDag();
			}
			if (ExchangeThrottleSettings.IsCafeGroup(categoryName))
			{
				return CafeUtils.GetCafeServersInSameArray();
			}
			if (ExchangeThrottleSettings.IsDomainControllerGroup(categoryName))
			{
				return ADUtils.GetDomainControllersInSameSite();
			}
			if (ExchangeThrottleSettings.IsCentralAdminGroup(categoryName))
			{
				return ADUtils.GetCentralAdminSvrsInSameSite();
			}
			throw new InvalidOperationException(string.Format("Unknown group category '{0}' specified.", categoryName));
		}

		internal const int DefaultLocalMinimumMinutesBetweenAttempts = 60;

		internal const int DefaultLocalMaximumAllowedAttemptsInOneHour = -1;

		internal const int DefaultLocalMaximumAllowedAttemptsInADay = 1;

		internal const int DefaultGroupMinimumMinutesBetweenAttempts = 60;

		internal const int DefaultGroupMaximumAllowedAttemptsInADay = 4;

		private static readonly Lazy<ExchangeThrottleSettings> lazy = new Lazy<ExchangeThrottleSettings>(() => new ExchangeThrottleSettings());

		internal static readonly FixedThrottleEntry[] BaseSettings = new FixedThrottleEntry[]
		{
			new FixedThrottleEntry(RecoveryActionId.ForceReboot, 720, -1, 1, 600, 4),
			new FixedThrottleEntry(RecoveryActionId.ServerFailover, 60, -1, 1, 60, 4),
			new FixedThrottleEntry(RecoveryActionId.RestartService, 60, -1, 1, 60, 4),
			new FixedThrottleEntry(RecoveryActionId.RecycleApplicationPool, 60, -1, 1, 60, 4),
			new FixedThrottleEntry(RecoveryActionId.DatabaseFailover, 60, -1, 2, 60, 6),
			new FixedThrottleEntry(RecoveryActionId.TakeComponentOffline, 60, -1, 1, 60, 4),
			new FixedThrottleEntry(RecoveryActionId.MoveClusterGroup, 240, -1, 1, 480, 3),
			new FixedThrottleEntry(RecoveryActionId.ResumeCatalog, 5, 4, 8, 5, 12),
			new FixedThrottleEntry(RecoveryActionId.WatsonDump, 480, -1, 1, 720, 4),
			new FixedThrottleEntry(RecoveryActionId.ControlService, 60, -1, 1, 60, 4),
			new FixedThrottleEntry(RecoveryActionId.DeleteFile, 1, -1, 15, 1, 30),
			new FixedThrottleEntry(RecoveryActionId.PutDCInMM, 60, -1, 1, 60, 4),
			new FixedThrottleEntry(RecoveryActionId.KillProcess, 1, -1, 15, 1, 30),
			new FixedThrottleEntry(RecoveryActionId.RenameNTDSPowerOff, 60, -1, 1, 60, 4),
			new FixedThrottleEntry(RecoveryActionId.RpcClientAccessRestart, 4, -1, 2, 4, 8),
			new FixedThrottleEntry(RecoveryActionId.RemoteForceReboot, 5, -1, 2, 15, 7),
			new FixedThrottleEntry(RecoveryActionId.RestartService, "hostcontrollerservice", 60, -1, 6, -1, -1),
			new FixedThrottleEntry(RecoveryActionId.RestartService, "msexchangefastsearch", 60, -1, 4, -1, -1),
			new FixedThrottleEntry(RecoveryActionId.RestartFastNode, 60, -1, 6, -1, -1),
			new FixedThrottleEntry(RecoveryActionId.SetNetworkAdapter, 120, -1, 1, 60, 16),
			new FixedThrottleEntry(RecoveryActionId.AddRoute, 120, -1, 1, 60, 16),
			new FixedThrottleEntry(RecoveryActionId.ClusterNodeHammerDown, 720, -1, 2, 600, 7),
			new FixedThrottleEntry(RecoveryActionId.ClearLsassCache, 45, -1, 6, 3, 60),
			new FixedThrottleEntry(RecoveryActionId.Watson, 240, -1, 3, 120, 4),
			new FixedThrottleEntry(RecoveryActionId.RaiseFailureItem, 240, -1, 4, 5, 10),
			new FixedThrottleEntry(RecoveryActionId.PotentialOneCopyRemoteServerRestartResponder, 720, -1, 1, 720, 2),
			new FixedThrottleEntry(RecoveryActionId.RemoteForceReboot, ResponderCategory.Default, "Microsoft.Exchange.Monitoring.ActiveMonitoring.RemoteStore.Responders.RemoteStoreAdminRPCInterfaceForceRebootResponder", "RemoteStoreAdminRPCInterfaceKillServer", null, new ThrottleParameters(true, 60, -1, 2, 30, 7))
		};

		internal class WellKnownThrottleGroup
		{
			internal const string Dag = "Dag";

			internal const string Cafe = "Cafe";

			internal const string DomainController = "DomainController";

			internal const string CentralAdmin = "CentralAdmin";

			internal const string StampedGroup = "StampedGroup";
		}
	}
}
