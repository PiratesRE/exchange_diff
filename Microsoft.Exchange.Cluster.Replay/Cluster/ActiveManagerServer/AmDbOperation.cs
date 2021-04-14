using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal abstract class AmDbOperation
	{
		internal AmDbOperation(IADDatabase db)
		{
			this.Database = db;
			this.CreationTime = ExDateTime.Now;
			this.Counter = (long)Interlocked.Increment(ref AmDbOperation.sm_operationCounter);
			this.UniqueId = AmDbOperation.GenerateUniqueId(this.Database.Guid, this.CreationTime, this.Counter);
		}

		internal IADDatabase Database { get; private set; }

		internal AmReportCompletionDelegate CompletionCallback { get; set; }

		internal AmDbActionStatus CustomStatus { get; set; }

		internal Exception LastException { get; set; }

		internal bool IsComplete { get; private set; }

		internal bool IsCancelled { get; set; }

		internal long Counter { get; private set; }

		internal string UniqueId { get; private set; }

		internal ExDateTime CreationTime { get; private set; }

		internal AmDbOperationDetailedStatus DetailedStatus { get; set; }

		internal static string GenerateUniqueId(Guid dbGuid, ExDateTime creationTime, long counter)
		{
			return string.Format("{0}#{1}#{2}#{3}", new object[]
			{
				creationTime.ToString("yyyy.MM.dd.hh.mm.ss.fff"),
				counter,
				AmServerName.LocalComputerName.NetbiosName,
				dbGuid
			});
		}

		internal static bool IsCompletionStatus(AmDbActionStatus status)
		{
			return status == AmDbActionStatus.Completed || status == AmDbActionStatus.Failed || status == AmDbActionStatus.Cancelled;
		}

		internal void Cancel()
		{
			this.IsCancelled = true;
			this.LastException = new AmDbActionCancelledException(this.Database.Name, base.GetType().Name);
			this.ReportStatus(this.Database, AmDbActionStatus.Cancelled);
		}

		internal void Enqueue()
		{
			AmDatabaseQueueManager databaseQueueManager = AmSystemManager.Instance.DatabaseQueueManager;
			databaseQueueManager.Enqueue(this);
		}

		internal AmDbCompletionReason Wait(TimeSpan timeout)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			AmDbCompletionReason amDbCompletionReason;
			while (!this.IsCancelled)
			{
				if (stopwatch.Elapsed > timeout)
				{
					amDbCompletionReason = AmDbCompletionReason.Timedout;
				}
				else
				{
					if (!this.IsComplete)
					{
						Thread.Sleep(50);
						continue;
					}
					amDbCompletionReason = AmDbCompletionReason.Finished;
				}
				IL_41:
				Exception ex = this.LastException;
				if (ex == null && amDbCompletionReason == AmDbCompletionReason.Timedout)
				{
					ex = new AmDbOperationTimedoutException(base.GetType().Name, this.Database.Name, timeout);
				}
				if (ex != null)
				{
					throw ex;
				}
				return amDbCompletionReason;
			}
			amDbCompletionReason = AmDbCompletionReason.Cancelled;
			goto IL_41;
		}

		internal AmDbCompletionReason Wait()
		{
			return this.Wait(TimeSpan.MaxValue);
		}

		internal bool IsStatusReached(AmDbActionStatus status)
		{
			lock (this.m_statusInfo)
			{
				ExDateTime exDateTime;
				if (this.m_statusInfo.TryGetValue(status, out exDateTime))
				{
					return true;
				}
			}
			return false;
		}

		internal void ReportStatus(IADDatabase db, AmDbActionStatus status)
		{
			lock (this.m_statusInfo)
			{
				this.m_statusInfo[status] = ExDateTime.Now;
			}
			if (AmDbOperation.IsCompletionStatus(status) || status == this.CustomStatus)
			{
				this.IsComplete = true;
			}
			if (this.CompletionCallback != null && this.IsComplete && !this.m_isCompletionCalled)
			{
				this.m_isCompletionCalled = true;
				this.CompletionCallback(db);
			}
			if (status == AmDbActionStatus.UpdateMasterServerInitiated)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2309369149U);
				return;
			}
			if (status == AmDbActionStatus.StoreMountInitiated)
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3383110973U);
			}
		}

		internal void Run()
		{
			this.RunInternal();
		}

		internal AmDatabaseMoveResult ConvertDetailedStatusToRpcMoveResult(AmDbOperationDetailedStatus detailedStatus)
		{
			Guid guid = detailedStatus.Database.Guid;
			string name = detailedStatus.Database.Name;
			string fromServerFqdn = string.Empty;
			string finalActiveServerFqdn = string.Empty;
			AmDbMountStatus dbMountStatusAtStart = AmDbMountStatus.Unknown;
			AmDbMountStatus dbMountStatusAtEnd = AmDbMountStatus.Unknown;
			if (detailedStatus.InitialDbState != null)
			{
				fromServerFqdn = detailedStatus.InitialDbState.ActiveServer.Fqdn;
				dbMountStatusAtStart = AmDbOperation.ConvertMountStatusToRpcMountStatus(detailedStatus.InitialDbState.MountStatus);
			}
			if (detailedStatus.FinalDbState != null)
			{
				finalActiveServerFqdn = detailedStatus.FinalDbState.ActiveServer.Fqdn;
				dbMountStatusAtEnd = AmDbOperation.ConvertMountStatusToRpcMountStatus(detailedStatus.FinalDbState.MountStatus);
			}
			Exception lastException = this.LastException;
			RpcErrorExceptionInfo errorInfo = AmRpcExceptionWrapper.Instance.ConvertExceptionToErrorExceptionInfo(lastException);
			AmDbMoveStatus dbMoveStatus = AmDbOperation.TranslateExceptionIntoMoveStatusEnum(lastException);
			List<AmDbRpcOperationSubStatus> attemptedServerSubStatuses = (from opSubStatus in detailedStatus.GetAllSubStatuses()
			select opSubStatus.ConvertToRpcSubStatus()).ToList<AmDbRpcOperationSubStatus>();
			return new AmDatabaseMoveResult(guid, name, fromServerFqdn, finalActiveServerFqdn, dbMoveStatus, dbMountStatusAtStart, dbMountStatusAtEnd, errorInfo, attemptedServerSubStatuses);
		}

		protected virtual void CheckIfOperationIsAllowedOnCurrentRole()
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (config.Role == AmRole.SAM)
			{
				AmReferralException ex = new AmReferralException(config.DagConfig.CurrentPAM.Fqdn);
				throw ex;
			}
			if (config.Role == AmRole.Unknown)
			{
				throw new AmInvalidConfiguration(config.LastError);
			}
		}

		protected abstract void RunInternal();

		protected AmDbAction PrepareDbAction(AmDbActionCode actionCode)
		{
			this.CheckIfOperationIsAllowedOnCurrentRole();
			AmConfig config = AmSystemManager.Instance.Config;
			AmDbAction amDbAction;
			if (config.IsPAM)
			{
				amDbAction = new AmDbPamAction(config, this.Database, actionCode, this.UniqueId);
			}
			else
			{
				amDbAction = new AmDbStandaloneAction(config, this.Database, actionCode, this.UniqueId);
			}
			AmDbAction amDbAction2 = amDbAction;
			amDbAction2.StatusCallback = (AmReportStatusDelegate)Delegate.Combine(amDbAction2.StatusCallback, new AmReportStatusDelegate(this.ReportStatus));
			return amDbAction;
		}

		private static AmDbMoveStatus TranslateExceptionIntoMoveStatusEnum(Exception lastException)
		{
			AmDbMoveStatus result;
			if (lastException == null)
			{
				result = AmDbMoveStatus.Succeeded;
			}
			else
			{
				result = AmDbMoveStatus.Failed;
				if (lastException is AmDbMoveOperationNotSupportedException)
				{
					result = AmDbMoveStatus.Warning;
				}
				else if (lastException is AmDbMoveOperationSkippedException)
				{
					result = AmDbMoveStatus.Warning;
				}
			}
			return result;
		}

		private static AmDbMountStatus ConvertMountStatusToRpcMountStatus(MountStatus mountStatus)
		{
			switch (mountStatus)
			{
			case MountStatus.Unknown:
				return AmDbMountStatus.Unknown;
			case MountStatus.Mounted:
				return AmDbMountStatus.Mounted;
			case MountStatus.Dismounted:
				return AmDbMountStatus.Dismounted;
			case MountStatus.Mounting:
				return AmDbMountStatus.Mounting;
			case MountStatus.Dismounting:
				return AmDbMountStatus.Dismounting;
			default:
				DiagCore.RetailAssert(false, "Unhandled case for mountStatus={0}", new object[]
				{
					mountStatus
				});
				return AmDbMountStatus.Unknown;
			}
		}

		private static int sm_operationCounter;

		private bool m_isCompletionCalled;

		private Dictionary<AmDbActionStatus, ExDateTime> m_statusInfo = new Dictionary<AmDbActionStatus, ExDateTime>();
	}
}
