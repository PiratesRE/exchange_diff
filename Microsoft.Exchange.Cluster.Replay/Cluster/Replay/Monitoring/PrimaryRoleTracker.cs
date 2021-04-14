using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.DagManagement;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	internal class PrimaryRoleTracker
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.DatabaseHealthTrackerTracer;
			}
		}

		public bool HasBecomePrimary { get; private set; }

		public bool WasPAM { get; private set; }

		public bool IsPAM { get; private set; }

		public bool IsAMRoleChanged
		{
			get
			{
				return this.IsPAM != this.WasPAM;
			}
		}

		private bool ShouldBecomePrimary
		{
			get
			{
				return this.IsPAM && !this.HasBecomePrimary;
			}
		}

		public void ReportPAMStatus(IEnumerable<CopyStatusClientCachedEntry> localStatuses)
		{
			this.WasPAM = this.IsPAM;
			if (localStatuses.Any((CopyStatusClientCachedEntry status) => status.Result == CopyStatusRpcResult.Success && status.CopyStatus.IsPrimaryActiveManager))
			{
				this.IsPAM = true;
			}
			else
			{
				this.IsPAM = false;
				this.HasBecomePrimary = false;
			}
			PrimaryRoleTracker.Tracer.TraceDebug<bool, bool, bool>((long)this.GetHashCode(), "ReportPAMStatus() found: IsPAM = '{0}', IsAMRoleChanged = '{1}', HasBecomePrimary = '{2}'", this.IsPAM, this.IsAMRoleChanged, this.HasBecomePrimary);
		}

		public bool ShouldTryToBecomePrimary()
		{
			bool result = false;
			TransientErrorInfo.ErrorType errorType;
			if (!this.IsPAM)
			{
				PrimaryRoleTracker.Tracer.TraceDebug((long)this.GetHashCode(), "ShouldTryToBecomePrimary(): The node is *NOT* the PAM! Returning 'false'.");
				this.m_primaryTransitionSuppression.ReportFailurePeriodic(out errorType);
				return false;
			}
			if (!this.ShouldBecomePrimary)
			{
				PrimaryRoleTracker.Tracer.TraceDebug((long)this.GetHashCode(), "ShouldTryToBecomePrimary(): The node has already become the primary. Returning 'false'.");
				this.m_primaryTransitionSuppression.ReportFailurePeriodic(out errorType);
				return false;
			}
			PrimaryRoleTracker.Tracer.TraceDebug((long)this.GetHashCode(), "ShouldTryToBecomePrimary(): The node is PAM but hasn't become the primary yet.");
			if (this.m_primaryTransitionSuppression.ReportSuccessPeriodic(out errorType) && errorType == TransientErrorInfo.ErrorType.Success)
			{
				result = true;
				PrimaryRoleTracker.Tracer.TraceDebug((long)this.GetHashCode(), "ShouldTryToBecomePrimary(): Returning 'true' after periodic suppression.");
			}
			else
			{
				PrimaryRoleTracker.Tracer.TraceDebug((long)this.GetHashCode(), "ShouldTryToBecomePrimary(): Returning 'false' because of periodic suppression.");
			}
			return result;
		}

		public void BecomePrimary()
		{
			this.HasBecomePrimary = true;
			TransientErrorInfo.ErrorType errorType;
			this.m_primaryTransitionSuppression.ReportFailurePeriodic(out errorType);
		}

		private static readonly TimeSpan PrimaryTransitionSuppressionWindow = TimeSpan.FromSeconds((double)RegistryParameters.MonitoringDHTPrimaryTransitionSuppressionInSec);

		private static readonly TimeSpan PrimaryPeriodicSuppressionWindow = TimeSpan.FromSeconds((double)RegistryParameters.MonitoringDHTPrimaryPeriodicSuppressionInSec);

		private TransientErrorInfoPeriodic m_primaryTransitionSuppression = new TransientErrorInfoPeriodic(PrimaryRoleTracker.PrimaryTransitionSuppressionWindow, PrimaryRoleTracker.PrimaryPeriodicSuppressionWindow, TimeSpan.Zero, TransientErrorInfoPeriodic.InfiniteTimeSpan, TransientErrorInfo.ErrorType.Failure);
	}
}
