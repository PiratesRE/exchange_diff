using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.HA.FailureItem;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FailedCopyWorkflow : AutoReseedWorkflow
	{
		public FailedCopyWorkflow(AutoReseedContext context, string workflowLaunchReason) : base(AutoReseedWorkflowType.FailedCopy, workflowLaunchReason, context)
		{
		}

		protected override bool IsDisabled
		{
			get
			{
				return RegistryParameters.AutoReseedDbFailedWorkflowDisabled;
			}
		}

		protected override TimeSpan GetThrottlingInterval(AutoReseedWorkflowState state)
		{
			return TimeSpan.Zero;
		}

		protected override LocalizedString RunPrereqs(AutoReseedWorkflowState state)
		{
			LocalizedString result = base.RunPrereqs(state);
			if (!result.IsEmpty)
			{
				return result;
			}
			result = FailedSuspendedCopyAutoReseedWorkflow.CheckExchangeVolumesPresent(base.Context);
			if (!result.IsEmpty)
			{
				return result;
			}
			return FailedSuspendedCopyAutoReseedWorkflow.CheckDatabaseLogPaths(base.Context);
		}

		protected override Exception ExecuteInternal(AutoReseedWorkflowState state)
		{
			RpcDatabaseCopyStatus2 copyStatus = base.Context.TargetCopyStatus.CopyStatus;
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedDbFailedPeriodicIntervalInSecs);
			base.TraceDebug("Calling SuspendAndFailLocalDatabaseCopy() ...", new object[0]);
			return DatabaseTasks.SuspendAndFailLocalDatabaseCopy(base.Context.Database, ReplayStrings.AutoReseedFailedCopyWorkflowSuspendedCopy(timeSpan.ToString()), copyStatus.ErrorMessage, copyStatus.ErrorEventId, copyStatus.ResumeBlocked, copyStatus.ReseedBlocked, copyStatus.InPlaceReseedBlocked);
		}
	}
}
