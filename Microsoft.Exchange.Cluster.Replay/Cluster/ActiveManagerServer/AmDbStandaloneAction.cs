using System;
using System.Diagnostics;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbStandaloneAction : AmDbAction
	{
		internal AmDbStandaloneAction(AmConfig cfg, IADDatabase db, AmDbActionCode actionCode, string uniqueOperationId) : base(cfg, db, actionCode, uniqueOperationId)
		{
		}

		protected override void MountInternal(MountFlags storeFlags, AmMountFlags amMountFlags, DatabaseMountDialOverride mountDialoverride, ref AmDbOperationDetailedStatus mountStatus)
		{
			Exception ex = null;
			bool isSuccess = false;
			AmServerName activeServer = base.State.ActiveServer;
			AmServerName serverToMount = AmServerName.LocalComputerName;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				ReplayCrimsonEvents.DirectMountInitiated.LogGeneric(base.PrepareSubactionArgs(new object[]
				{
					serverToMount,
					storeFlags,
					false,
					amMountFlags
				}));
				ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
				{
					this.ReportStatus(AmDbActionStatus.StoreMountInitiated);
					this.WriteStateMountStart(serverToMount);
					AmDbAction.MountDatabaseDirect(serverToMount, this.State.LastMountedServer, this.DatabaseGuid, storeFlags, amMountFlags, this.ActionCode);
					isSuccess = true;
				});
			}
			finally
			{
				stopwatch.Stop();
				if (isSuccess)
				{
					base.DbTrace.Debug("Database is now mounted on {0}", new object[]
					{
						serverToMount
					});
					SharedDependencies.WritableADHelper.ResetAllowFileRestoreDsFlag(base.DatabaseGuid, activeServer, serverToMount);
					ReplayCrimsonEvents.DirectMountSuccess.LogGeneric(base.PrepareSubactionArgs(new object[]
					{
						serverToMount,
						stopwatch.Elapsed
					}));
					base.WriteStateMountSuccess();
					base.ReportStatus(AmDbActionStatus.StoreMountSuccessful);
				}
				else
				{
					string text = (ex != null) ? ex.Message : ReplayStrings.UnknownError;
					ReplayCrimsonEvents.DirectMountFailed.LogGeneric(base.PrepareSubactionArgs(new object[]
					{
						serverToMount,
						stopwatch.Elapsed,
						text
					}));
					base.WriteStateMountFailed(true);
					base.ReportStatus(AmDbActionStatus.StoreMountFailed);
				}
			}
			AmHelper.ThrowDbActionWrapperExceptionIfNecessary(ex);
		}

		protected override void DismountInternal(UnmountFlags flags)
		{
			base.DismountCommon(flags);
		}

		protected override void MoveInternal(MountFlags mountFlags, UnmountFlags dismountFlags, DatabaseMountDialOverride mountDialoverride, AmServerName fromServer, AmServerName targetServer, bool tryOtherHealthyServers, AmBcsSkipFlags skipValidationChecks, string componentName, ref AmDbOperationDetailedStatus moveStatus)
		{
			throw new AmDbMoveOperationNotSupportedStandaloneException(base.DatabaseName);
		}

		protected override void RemountInternal(MountFlags mountFlags, DatabaseMountDialOverride mountDialoverride, AmServerName fromServer)
		{
			AmDbOperationDetailedStatus amDbOperationDetailedStatus = new AmDbOperationDetailedStatus(base.Database);
			this.DismountInternal(UnmountFlags.SkipCacheFlush);
			this.MountInternal(mountFlags, AmMountFlags.None, mountDialoverride, ref amDbOperationDetailedStatus);
		}
	}
}
