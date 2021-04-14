using System;
using System.Linq;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class HealthyCopyCompletedSeedWorkflow : AutoReseedWorkflow
	{
		public HealthyCopyCompletedSeedWorkflow(AutoReseedContext context) : base(AutoReseedWorkflowType.HealthyCopyCompletedSeed, string.Empty, context)
		{
		}

		protected override TimeSpan GetThrottlingInterval(AutoReseedWorkflowState state)
		{
			return TimeSpan.Zero;
		}

		protected override bool IsDisabled
		{
			get
			{
				return RegistryParameters.AutoReseedDbFailedSuspendedDisabled;
			}
		}

		protected override Exception ExecuteInternal(AutoReseedWorkflowState state)
		{
			AutoReseedWorkflowState autoReseedWorkflowState = new AutoReseedWorkflowState(base.Context.Database.Guid, AutoReseedWorkflowType.FailedSuspendedCopyAutoReseed);
			AutoReseedWorkflowState autoReseedWorkflowState2 = new AutoReseedWorkflowState(base.Context.Database.Guid, AutoReseedWorkflowType.ManualReseed);
			AutoReseedWorkflowState autoReseedWorkflowState3 = new AutoReseedWorkflowState(base.Context.Database.Guid, AutoReseedWorkflowType.ManualResume);
			AutoReseedWorkflowState[] source = new AutoReseedWorkflowState[]
			{
				autoReseedWorkflowState,
				autoReseedWorkflowState2,
				autoReseedWorkflowState3
			};
			AutoReseedWorkflowState[] array = (from s in source
			orderby s.WorkflowExecutionTime descending
			select s).ToArray<AutoReseedWorkflowState>();
			if (array[0].WorkflowType == AutoReseedWorkflowType.ManualResume)
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, string, Guid>((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: A manual resume outside of AR made the copy become Healthy.", base.WorkflowName, base.Context.Database.Name, base.Context.Database.Guid);
				ReplayCrimsonEvents.AutoReseedHealthyDueToManualResume.Log<string, Guid, int>(base.Context.Database.Name, base.Context.Database.Guid, 1);
			}
			else if (array[0].WorkflowType == AutoReseedWorkflowType.ManualReseed)
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, string, Guid>((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: A manual seed was kicked off that made the copy become Healthy.", base.WorkflowName, base.Context.Database.Name, base.Context.Database.Guid);
				ReplayCrimsonEvents.AutoReseedHealthyDueToManualReseed.Log<string, Guid, int>(base.Context.Database.Name, base.Context.Database.Guid, 1);
			}
			else if (autoReseedWorkflowState.LastReseedRecoveryAction == ReseedState.Resume)
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, string, Guid>((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: An automatic resume from AR made the copy become Healthy.", base.WorkflowName, base.Context.Database.Name, base.Context.Database.Guid);
				ReplayCrimsonEvents.AutoReseedHealthyDueToAutomaticResume.Log<string, Guid, int>(base.Context.Database.Name, base.Context.Database.Guid, autoReseedWorkflowState.ReseedRecoveryActionRetryCount);
			}
			else if (autoReseedWorkflowState.LastReseedRecoveryAction == ReseedState.InPlaceReseed)
			{
				if (!string.IsNullOrEmpty(autoReseedWorkflowState.AssignedVolumeName))
				{
					AutoReseedWorkflow.Tracer.TraceDebug<string, string, Guid>((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: An AutoReseed was performed to a spare volume.", base.WorkflowName, base.Context.Database.Name, base.Context.Database.Guid);
					ReplayCrimsonEvents.AutoReseedHealthyDueToReseedToSpare.Log<string, Guid, int>(base.Context.Database.Name, base.Context.Database.Guid, autoReseedWorkflowState.ReseedRecoveryActionRetryCount);
				}
				else
				{
					AutoReseedWorkflow.Tracer.TraceDebug<string, string, Guid>((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: An in-place AutoReseed was performed.", base.WorkflowName, base.Context.Database.Name, base.Context.Database.Guid);
					ReplayCrimsonEvents.AutoReseedHealthyDueToInPlaceReseed.Log<string, Guid, int>(base.Context.Database.Name, base.Context.Database.Guid, autoReseedWorkflowState.ReseedRecoveryActionRetryCount);
				}
			}
			if (autoReseedWorkflowState.IsLastReseedRecoveryActionPending())
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, string, Guid>((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: Previous auto-reseed had completed successfully. Resetting the AutoReseedWorkflowState.", base.WorkflowName, base.Context.Database.Name, base.Context.Database.Guid);
				autoReseedWorkflowState.LastReseedRecoveryAction = ReseedState.Completed;
				autoReseedWorkflowState.ReseedRecoveryActionRetryCount = 0;
				autoReseedWorkflowState.AssignedVolumeName = string.Empty;
				autoReseedWorkflowState.WriteWorkflowNextExecutionDueTime(TimeSpan.Zero);
			}
			if (autoReseedWorkflowState2.IsLastReseedRecoveryActionPending())
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, string, Guid>((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: Previous manual reseed action had completed successfully. Resetting the AutoReseedWorkflowState.", base.WorkflowName, base.Context.Database.Name, base.Context.Database.Guid);
				autoReseedWorkflowState2.LastReseedRecoveryAction = ReseedState.Completed;
			}
			if (autoReseedWorkflowState3.IsLastReseedRecoveryActionPending())
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, string, Guid>((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: Previous manual resume action had completed successfully. Resetting the AutoReseedWorkflowState.", base.WorkflowName, base.Context.Database.Name, base.Context.Database.Guid);
				autoReseedWorkflowState3.LastReseedRecoveryAction = ReseedState.Completed;
			}
			return null;
		}
	}
}
