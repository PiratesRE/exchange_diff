using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmBcsServerValidation
	{
		public AmBcsServerValidation(AmServerName serverToCheck, AmServerName sourceServer, IADDatabase database, AmConfig amConfig, IAmBcsErrorLogger errorLogger, IMonitoringADConfig dagConfig)
		{
			this.ServerToCheck = serverToCheck;
			this.SourceServer = sourceServer;
			this.Database = database;
			this.AmConfig = amConfig;
			this.ErrorLogger = errorLogger;
			this.DagConfig = dagConfig;
		}

		private AmServerName ServerToCheck { get; set; }

		private AmServerName SourceServer { get; set; }

		private IADDatabase Database { get; set; }

		private AmConfig AmConfig { get; set; }

		private IMonitoringADConfig DagConfig { get; set; }

		private IAmBcsErrorLogger ErrorLogger { get; set; }

		private string ErrorMessage { get; set; }

		public static AmBcsServerChecks GetServerValidationChecks(AmDbActionCode actionCode, bool isServerSpecifiedByAdmin)
		{
			AmBcsServerChecks result = AmBcsServerChecks.DebugOptionDisabled | AmBcsServerChecks.ClusterNodeUp | AmBcsServerChecks.DatacenterActivationModeStarted | AmBcsServerChecks.AutoActivationAllowed;
			if (actionCode.IsAdminMoveOperation)
			{
				result = (isServerSpecifiedByAdmin ? (AmBcsServerChecks.ClusterNodeUp | AmBcsServerChecks.DatacenterActivationModeStarted) : (AmBcsServerChecks.ClusterNodeUp | AmBcsServerChecks.DatacenterActivationModeStarted | AmBcsServerChecks.AutoActivationAllowed));
			}
			else if (actionCode.IsAdminMountOperation)
			{
				result = (AmBcsServerChecks.ClusterNodeUp | AmBcsServerChecks.DatacenterActivationModeStarted);
			}
			return result;
		}

		public bool RunChecks(AmBcsServerChecks checks, ref LocalizedString error)
		{
			bool flag = true;
			error = LocalizedString.Empty;
			if (flag && AmBcsServerValidation.ShouldRunCheck(checks, AmBcsServerChecks.DebugOptionDisabled))
			{
				flag = this.CheckDebugOption(ref error);
			}
			if (flag && AmBcsServerValidation.ShouldRunCheck(checks, AmBcsServerChecks.DatacenterActivationModeStarted))
			{
				flag = this.IsServerStartedForDACMode(ref error);
			}
			if (flag && AmBcsServerValidation.ShouldRunCheck(checks, AmBcsServerChecks.ClusterNodeUp))
			{
				flag = this.IsClusterNodeUp(ref error);
			}
			if (flag && AmBcsServerValidation.ShouldRunCheck(checks, AmBcsServerChecks.AutoActivationAllowed))
			{
				flag = this.IsAutoActivationAllowed(ref error);
			}
			return flag;
		}

		private static bool ShouldRunCheck(AmBcsServerChecks checksToRun, AmBcsServerChecks checkInQuestion)
		{
			return (checksToRun & checkInQuestion) == checkInQuestion;
		}

		private bool CheckDebugOption(ref LocalizedString error)
		{
			if (this.AmConfig.IsIgnoreServerDebugOptionEnabled(this.ServerToCheck))
			{
				string text = AmDebugOptions.IgnoreServerFromAutomaticActions.ToString();
				AmTrace.Error("AmBcsServerValidation: Rejecting server '{0}' for DB '{1}' because the node is marked in debug option. Debug options: {2}", new object[]
				{
					this.ServerToCheck,
					this.Database.Name,
					text
				});
				error = ReplayStrings.AmBcsTargetNodeDebugOptionEnabled(this.ServerToCheck.Fqdn, text);
				if (this.ErrorLogger != null)
				{
					this.ErrorLogger.ReportServerFailure(this.ServerToCheck, AmBcsServerChecks.DebugOptionDisabled.ToString(), error, ReplayCrimsonEvents.OperationNotPerformedDueToDebugOption, new object[]
					{
						this.ServerToCheck.NetbiosName,
						text,
						"Best copy selection"
					});
				}
				return false;
			}
			return true;
		}

		private bool IsClusterNodeUp(ref LocalizedString error)
		{
			if (this.AmConfig.DagConfig.IsNodePubliclyUp(this.ServerToCheck))
			{
				return true;
			}
			AmTrace.Error("AmBcsServerValidation: Rejecting server '{0}' for DB '{1}' because the node is down.", new object[]
			{
				this.ServerToCheck,
				this.Database.Name
			});
			error = ReplayStrings.AmBcsTargetNodeDownError(this.ServerToCheck.Fqdn);
			this.ReportServerBlocked(AmBcsServerChecks.ClusterNodeUp, error);
			return false;
		}

		private bool IsServerStartedForDACMode(ref LocalizedString error)
		{
			string adError = null;
			IADDatabaseAvailabilityGroup iaddatabaseAvailabilityGroup;
			if (this.DagConfig != null)
			{
				iaddatabaseAvailabilityGroup = this.DagConfig.Dag;
				if (iaddatabaseAvailabilityGroup == null)
				{
					CouldNotFindDagObjectForServer couldNotFindDagObjectForServer = new CouldNotFindDagObjectForServer(this.ServerToCheck.NetbiosName);
					adError = couldNotFindDagObjectForServer.Message;
				}
			}
			else
			{
				iaddatabaseAvailabilityGroup = AmBestCopySelectionHelper.GetLocalServerDatabaseAvailabilityGroup(out adError);
			}
			if (iaddatabaseAvailabilityGroup == null)
			{
				error = ReplayStrings.AmBcsDagNotFoundInAd(this.ServerToCheck.Fqdn, adError);
				this.ReportServerBlocked(AmBcsServerChecks.DatacenterActivationModeStarted, error);
				return false;
			}
			if (AmBestCopySelectionHelper.IsServerInDacAndStopped(iaddatabaseAvailabilityGroup, this.ServerToCheck))
			{
				AmTrace.Error("AmBcsServerValidation: Rejecting server '{0}' for DB '{1}' since it is stopped in the DAC mode.", new object[]
				{
					this.ServerToCheck,
					this.Database.Name
				});
				error = ReplayStrings.AmBcsTargetServerIsStoppedOnDAC(this.ServerToCheck.Fqdn);
				this.ReportServerBlocked(AmBcsServerChecks.DatacenterActivationModeStarted, error);
				return false;
			}
			return true;
		}

		private bool IsAutoActivationAllowed(ref LocalizedString error)
		{
			if (this.ServerToCheck.Equals(this.SourceServer))
			{
				AmTrace.Debug("AmBcsServerValidation: Skipping 'IsAutoActivationAllowed' check since source == target. TargetServer: {0}", new object[]
				{
					this.ServerToCheck
				});
				return true;
			}
			Exception ex;
			IADServer miniServer = AmBestCopySelectionHelper.GetMiniServer(this.ServerToCheck, out ex);
			if (miniServer == null)
			{
				AmTrace.Error("AmBcsServerValidation: Rejecting server '{0}' for DB '{1}' because target MiniServer could not be read.", new object[]
				{
					this.ServerToCheck,
					this.Database.Name
				});
				error = ReplayStrings.AmBcsTargetServerADError(this.ServerToCheck.Fqdn, ex.ToString());
				this.ReportServerBlocked(AmBcsServerChecks.AutoActivationAllowed, error);
				return false;
			}
			IADServer miniServer2 = AmBestCopySelectionHelper.GetMiniServer(this.SourceServer, out ex);
			if (miniServer2 == null)
			{
				AmTrace.Error("AmBcsServerValidation: Rejecting server '{0}' for DB '{1}' because source MiniServer '{2}' could not be read.", new object[]
				{
					this.ServerToCheck,
					this.Database.Name,
					this.SourceServer
				});
				error = ReplayStrings.AmBcsSourceServerADError(this.SourceServer.Fqdn, ex.ToString());
				this.ReportServerBlocked(AmBcsServerChecks.AutoActivationAllowed, error);
				return false;
			}
			if (!AmBestCopySelectionHelper.IsAutoActivationAllowed(miniServer2, miniServer, out error))
			{
				this.ReportServerBlocked(AmBcsServerChecks.AutoActivationAllowed, error);
				return false;
			}
			return true;
		}

		private void ReportServerBlocked(AmBcsServerChecks checkFailed, LocalizedString error)
		{
			if (this.ErrorLogger != null)
			{
				this.ErrorLogger.ReportServerFailure(this.ServerToCheck, checkFailed.ToString(), error);
			}
		}

		internal const string AmBcsCopyStatusRpcCheckName = "CopyStatusRpcCheck";

		internal const string AmBcsCopyHasBeenTriedCheckName = "CopyHasBeenTriedCheck";

		internal const AmBcsServerChecks AllChecks = AmBcsServerChecks.DebugOptionDisabled | AmBcsServerChecks.ClusterNodeUp | AmBcsServerChecks.DatacenterActivationModeStarted | AmBcsServerChecks.AutoActivationAllowed;

		internal const AmBcsServerChecks ChecksForAdminMoveWithTargetSpecified = AmBcsServerChecks.ClusterNodeUp | AmBcsServerChecks.DatacenterActivationModeStarted;

		internal const AmBcsServerChecks ChecksForAdminMoveWithTargetAutomaticallyChosen = AmBcsServerChecks.ClusterNodeUp | AmBcsServerChecks.DatacenterActivationModeStarted | AmBcsServerChecks.AutoActivationAllowed;

		internal const AmBcsServerChecks ChecksForAdminMount = AmBcsServerChecks.ClusterNodeUp | AmBcsServerChecks.DatacenterActivationModeStarted;
	}
}
