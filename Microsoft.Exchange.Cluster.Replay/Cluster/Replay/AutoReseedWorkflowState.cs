using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AutoReseedWorkflowState
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AutoReseedTracer;
			}
		}

		public AutoReseedWorkflowState(Guid dbGuid, AutoReseedWorkflowType workflowType)
		{
			this.m_dbGuid = dbGuid;
			this.m_workflowType = workflowType;
			this.m_workflowName = workflowType.ToString();
			this.m_fields = new AutoReseedWorkflowStateValues(dbGuid, workflowType);
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.m_dbGuid;
			}
		}

		public AutoReseedWorkflowType WorkflowType
		{
			get
			{
				return this.m_workflowType;
			}
		}

		public string WorkflowName
		{
			get
			{
				return this.m_workflowName;
			}
		}

		public bool WorkflowInfoIsValid
		{
			get
			{
				return this.m_fields.GetValue<bool>("WorkflowInfoIsValid");
			}
			set
			{
				this.m_fields.SetValue<bool>("WorkflowInfoIsValid", value);
			}
		}

		public AutoReseedWorkflowExecutionResult WorkflowExecutionResult
		{
			get
			{
				return this.m_fields.GetValue<AutoReseedWorkflowExecutionResult>("WorkflowExecutionResult2");
			}
			set
			{
				this.m_fields.SetValue<AutoReseedWorkflowExecutionResult>("WorkflowExecutionResult2", value);
			}
		}

		public string WorkflowExecutionError
		{
			get
			{
				return this.m_fields.GetValue<string>("WorkflowExecutionError");
			}
			set
			{
				this.m_fields.SetValue<string>("WorkflowExecutionError", value);
			}
		}

		public DateTime WorkflowExecutionTime
		{
			get
			{
				return this.m_fields.GetValue<DateTime>("WorkflowExecutionTime");
			}
			set
			{
				this.m_fields.SetValue<DateTime>("WorkflowExecutionTime", value);
			}
		}

		public DateTime WorkflowNextExecutionTime
		{
			get
			{
				return this.m_fields.GetValue<DateTime>("WorkflowNextExecutionTime");
			}
			set
			{
				this.m_fields.SetValue<DateTime>("WorkflowNextExecutionTime", value);
			}
		}

		public string AssignedVolumeName
		{
			get
			{
				return this.m_fields.GetValue<string>("AssignedVolumeName");
			}
			set
			{
				this.m_fields.SetValue<string>("AssignedVolumeName", value);
			}
		}

		public ReseedState LastReseedRecoveryAction
		{
			get
			{
				return this.m_fields.GetValue<ReseedState>("LastReseedRecoveryAction");
			}
			set
			{
				this.m_fields.SetValue<ReseedState>("LastReseedRecoveryAction", value);
			}
		}

		public int ReseedRecoveryActionRetryCount
		{
			get
			{
				return this.m_fields.GetValue<int>("ReseedRecoveryActionRetryCount");
			}
			set
			{
				this.m_fields.SetValue<int>("ReseedRecoveryActionRetryCount", value);
			}
		}

		public bool IgnoreInPlaceOverwriteDelay
		{
			get
			{
				return this.m_fields.GetValue<bool>("IgnoreInPlaceOverwriteDelay");
			}
			set
			{
				this.m_fields.SetValue<bool>("IgnoreInPlaceOverwriteDelay", value);
			}
		}

		public bool IsLastReseedRecoveryActionPending()
		{
			return this.LastReseedRecoveryAction != ReseedState.Unknown && this.LastReseedRecoveryAction != ReseedState.Completed;
		}

		public void WriteWorkflowExecutionState(Exception exception)
		{
			this.WorkflowInfoIsValid = false;
			this.WorkflowExecutionTime = DateTime.UtcNow;
			if (exception != null)
			{
				this.WorkflowExecutionResult = AutoReseedWorkflowExecutionResult.Failed;
				this.WorkflowExecutionError = exception.Message;
			}
			else
			{
				this.WorkflowExecutionError = string.Empty;
				this.WorkflowExecutionResult = AutoReseedWorkflowExecutionResult.Success;
			}
			this.WorkflowInfoIsValid = true;
		}

		public void WriteWorkflowNextExecutionDueTime(TimeSpan nextDueAfter)
		{
			DateTime workflowNextExecutionTime = DateTime.MinValue;
			if (nextDueAfter == InvokeWithTimeout.InfiniteTimeSpan)
			{
				workflowNextExecutionTime = DateTime.MaxValue;
			}
			else if (nextDueAfter != TimeSpan.Zero)
			{
				workflowNextExecutionTime = DateTime.UtcNow.Add(nextDueAfter);
			}
			this.WorkflowNextExecutionTime = workflowNextExecutionTime;
		}

		public void UpdateReseedRecoveryAction(ReseedState action)
		{
			this.WorkflowInfoIsValid = false;
			if (action != this.LastReseedRecoveryAction)
			{
				AutoReseedWorkflowState.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}': State is moving from '{2}' to '{3}'. ReseedRecoveryActionRetryCount is being reset to 0.", new object[]
				{
					this.WorkflowName,
					this.DatabaseGuid,
					this.LastReseedRecoveryAction,
					action
				});
				this.ResetReseedRecoveryActionInternal(action);
			}
			else
			{
				this.ReseedRecoveryActionRetryCount++;
				AutoReseedWorkflowState.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}': Retry count of state '{2}' is being incremented to: ReseedRecoveryActionRetryCount = {3}.", new object[]
				{
					this.WorkflowName,
					this.DatabaseGuid,
					this.LastReseedRecoveryAction,
					this.ReseedRecoveryActionRetryCount
				});
			}
			this.WorkflowInfoIsValid = true;
		}

		public void ResetReseedRecoveryAction()
		{
			this.WorkflowInfoIsValid = false;
			this.ResetReseedRecoveryActionInternal(ReseedState.Unknown);
			this.IgnoreInPlaceOverwriteDelay = false;
			this.WorkflowInfoIsValid = true;
		}

		public static Exception WriteManualWorkflowExecutionState(Guid dbGuid, AutoReseedWorkflowType manualAction)
		{
			Exception result = null;
			try
			{
				AutoReseedWorkflowState autoReseedWorkflowState = new AutoReseedWorkflowState(dbGuid, manualAction);
				if (manualAction == AutoReseedWorkflowType.ManualResume)
				{
					autoReseedWorkflowState.LastReseedRecoveryAction = ReseedState.Resume;
				}
				else
				{
					autoReseedWorkflowState.LastReseedRecoveryAction = ReseedState.InPlaceReseed;
				}
				autoReseedWorkflowState.WriteWorkflowExecutionState(null);
			}
			catch (RegistryParameterException ex)
			{
				result = ex;
			}
			return result;
		}

		public static Exception TriggerInPlaceReseed(Guid dbGuid, string dbName)
		{
			Exception result = null;
			try
			{
				AutoReseedWorkflowState autoReseedWorkflowState = new AutoReseedWorkflowState(dbGuid, AutoReseedWorkflowType.FailedSuspendedCopyAutoReseed);
				autoReseedWorkflowState.ResetReseedRecoveryAction();
				autoReseedWorkflowState.IgnoreInPlaceOverwriteDelay = true;
				autoReseedWorkflowState.UpdateReseedRecoveryAction(ReseedState.InPlaceReseed);
				ReplayCrimsonEvents.AutoReseedTriggerInPlaceReseed.LogPeriodic<string, Guid>(dbGuid, DiagCore.DefaultEventSuppressionInterval, dbName, dbGuid);
			}
			catch (RegistryParameterException ex)
			{
				result = ex;
			}
			return result;
		}

		private void ResetReseedRecoveryActionInternal(ReseedState action)
		{
			this.ReseedRecoveryActionRetryCount = 0;
			this.LastReseedRecoveryAction = action;
			this.WorkflowExecutionResult = AutoReseedWorkflowExecutionResult.Unknown;
			this.WorkflowExecutionError = string.Empty;
			this.WorkflowExecutionTime = DateTime.MinValue;
			this.WorkflowNextExecutionTime = DateTime.MinValue;
		}

		private readonly Guid m_dbGuid;

		private readonly string m_workflowName;

		private readonly AutoReseedWorkflowType m_workflowType;

		private readonly AutoReseedWorkflowStateValues m_fields;
	}
}
