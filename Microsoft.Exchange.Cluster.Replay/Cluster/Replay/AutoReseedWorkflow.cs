using System;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AutoReseedWorkflow
	{
		protected static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AutoReseedTracer;
			}
		}

		internal AutoReseedContext Context
		{
			get
			{
				return this.m_context;
			}
		}

		protected AutoReseedWorkflowType WorkflowType
		{
			get
			{
				return this.m_workflowType;
			}
		}

		protected string WorkflowName
		{
			get
			{
				return this.m_workflowName;
			}
		}

		protected string WorkflowLaunchReason
		{
			get
			{
				return this.m_workflowLaunchReason;
			}
		}

		protected abstract bool IsDisabled { get; }

		protected abstract TimeSpan GetThrottlingInterval(AutoReseedWorkflowState state);

		protected AutoReseedWorkflow(AutoReseedWorkflowType workflowType, string workflowLaunchReason, AutoReseedContext context)
		{
			this.m_workflowType = workflowType;
			this.m_workflowName = workflowType.ToString();
			this.m_workflowLaunchReason = workflowLaunchReason;
			this.m_context = context;
		}

		protected void TraceDebug(string formatString, params object[] args)
		{
			if (AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.DebugTrace) || AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				AutoReseedWorkflow.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: {3}", new object[]
				{
					this.WorkflowName,
					this.Context.Database.Name,
					this.Context.Database.Guid,
					string.Format(formatString, args)
				});
			}
		}

		protected void TraceError(string formatString, params object[] args)
		{
			if (AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.ErrorTrace) || AutoReseedWorkflow.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				AutoReseedWorkflow.Tracer.TraceError((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: {3}", new object[]
				{
					this.WorkflowName,
					this.Context.Database.Name,
					this.Context.Database.Guid,
					string.Format(formatString, args)
				});
			}
		}

		public Exception Execute()
		{
			Exception ex = null;
			if (this.IsDisabled)
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, string, Guid>((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}] will not be executed because it has been disabled via regkey.", this.WorkflowName, this.Context.Database.Name, this.Context.Database.Guid);
				return null;
			}
			try
			{
				AutoReseedWorkflowState autoReseedWorkflowState = new AutoReseedWorkflowState(this.Context.Database.Guid, this.WorkflowType);
				if (this.ShouldSkipWorkflowExecution(autoReseedWorkflowState))
				{
					return null;
				}
				this.LogWorkflowStarted();
				if (this.ArePrereqsSatisfied(autoReseedWorkflowState, out ex) && !this.ShouldThrottleExecution(autoReseedWorkflowState, out ex))
				{
					bool flag = true;
					try
					{
						ex = this.ExecuteInternal(autoReseedWorkflowState);
						flag = false;
					}
					catch (RegistryParameterException ex2)
					{
						flag = false;
						ex = new AutoReseedException(ex2.Message, ex2);
					}
					finally
					{
						if (flag)
						{
							ex = new AutoReseedUnhandledException(this.Context.Database.Name, this.Context.TargetServerName.NetbiosName);
						}
						autoReseedWorkflowState.WriteWorkflowExecutionState(ex);
					}
				}
			}
			catch (RegistryParameterException ex3)
			{
				ex = new AutoReseedException(ex3.Message, ex3);
			}
			this.LogWorkflowEnded(ex);
			return ex;
		}

		private bool ShouldSkipWorkflowExecution(AutoReseedWorkflowState state)
		{
			bool result = false;
			DateTime workflowNextExecutionTime = state.WorkflowNextExecutionTime;
			if (workflowNextExecutionTime != DateTime.MinValue && DateTime.UtcNow < workflowNextExecutionTime)
			{
				AutoReseedWorkflow.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: Skipping execution because WorkflowNextExecutionTime is set to '{3}'.", new object[]
				{
					this.WorkflowName,
					this.Context.Database.Name,
					this.Context.Database.Guid,
					workflowNextExecutionTime
				});
				result = true;
			}
			return result;
		}

		protected abstract Exception ExecuteInternal(AutoReseedWorkflowState state);

		protected virtual LocalizedString RunPrereqs(AutoReseedWorkflowState state)
		{
			return this.CheckThirdPartyReplicationEnabled();
		}

		protected LocalizedString CheckThirdPartyReplicationEnabled()
		{
			if (this.Context.Dag.ThirdPartyReplication == ThirdPartyReplicationMode.Enabled)
			{
				return ReplayStrings.AutoReseedWorkflowNotSupportedOnTPR(this.Context.Dag.Name);
			}
			return LocalizedString.Empty;
		}

		protected bool GetWorkflowElapsedExecutionTime(AutoReseedWorkflowState state, out TimeSpan elapsedExecutionTime)
		{
			elapsedExecutionTime = TimeSpan.Zero;
			if (state.WorkflowExecutionTime.Equals(DateTime.MinValue))
			{
				this.TraceDebug("GetWorkflowElapsedExecutionTime(): Returning 'false' since WorkflowExecutionTime is DateTime.MinValue.", new object[0]);
				return false;
			}
			elapsedExecutionTime = DateTime.UtcNow.Subtract(state.WorkflowExecutionTime);
			this.TraceDebug("GetWorkflowElapsedExecutionTime(): Returning elapsedExecutionTime = '{0}'", new object[]
			{
				elapsedExecutionTime
			});
			return true;
		}

		private bool ArePrereqsSatisfied(AutoReseedWorkflowState state, out Exception exception)
		{
			exception = null;
			LocalizedString value = this.RunPrereqs(state);
			if (!value.IsEmpty)
			{
				exception = new AutoReseedPrereqFailedException(this.Context.Database.Name, this.Context.TargetServerName.NetbiosName, value);
				return false;
			}
			return true;
		}

		private bool ShouldThrottleExecution(AutoReseedWorkflowState state, out Exception exception)
		{
			bool flag = false;
			exception = null;
			TimeSpan t;
			if (this.GetWorkflowElapsedExecutionTime(state, out t))
			{
				TimeSpan throttlingInterval = this.GetThrottlingInterval(state);
				if (t < throttlingInterval)
				{
					flag = true;
					exception = new AutoReseedThrottledException(this.Context.Database.Name, this.Context.TargetServerName.NetbiosName, throttlingInterval.ToString());
				}
				AutoReseedWorkflow.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}]: Throttling interval ({3}). Last execution time ({4}). Throttle = {5}.", new object[]
				{
					this.WorkflowName,
					this.Context.Database.Name,
					this.Context.Database.Guid,
					throttlingInterval,
					state.WorkflowExecutionTime,
					flag
				});
			}
			else
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, string, Guid>((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}] will not be throttled because this is the first time it is being executed.", this.WorkflowName, this.Context.Database.Name, this.Context.Database.Guid);
			}
			return flag;
		}

		private void LogWorkflowStarted()
		{
			AutoReseedWorkflow.Tracer.TraceDebug((long)this.GetHashCode(), "Starting AutoReseed workflow '{0}' for database '{1}' [{2}]. WorkflowLaunchReason: {3}", new object[]
			{
				this.WorkflowName,
				this.Context.Database.Name,
				this.Context.Database.Guid,
				this.m_workflowLaunchReason
			});
			ReplayCrimsonEvents.AutoReseedManagerBeginWorkflow.Log<string, Guid, string, string>(this.Context.Database.Name, this.Context.Database.Guid, this.WorkflowName, this.m_workflowLaunchReason);
		}

		private void LogWorkflowEnded(Exception exception)
		{
			if (exception == null)
			{
				AutoReseedWorkflow.Tracer.TraceDebug<string, string, Guid>((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}] completed successfully.", this.WorkflowName, this.Context.Database.Name, this.Context.Database.Guid);
				ReplayCrimsonEvents.AutoReseedManagerWorkflowCompletedSuccess.Log<string, Guid, string, string>(this.Context.Database.Name, this.Context.Database.Guid, this.WorkflowName, this.m_workflowLaunchReason);
				return;
			}
			if (exception is AutoReseedThrottledException)
			{
				AutoReseedWorkflow.Tracer.TraceError((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}] got throttled! Error: {3}", new object[]
				{
					this.WorkflowName,
					this.Context.Database.Name,
					this.Context.Database.Guid,
					AmExceptionHelper.GetExceptionMessageOrNoneString(exception)
				});
				ReplayCrimsonEvents.AutoReseedManagerWorkflowThrottled.LogPeriodic<string, Guid, string, string, string>(this.Context.Database.Guid, DiagCore.DefaultEventSuppressionInterval, this.Context.Database.Name, this.Context.Database.Guid, this.WorkflowName, this.m_workflowLaunchReason, AmExceptionHelper.GetExceptionMessageOrNoneString(exception));
				return;
			}
			AutoReseedWorkflow.Tracer.TraceError((long)this.GetHashCode(), "AutoReseed workflow '{0}' for database '{1}' [{2}] failed! Error: {3}", new object[]
			{
				this.WorkflowName,
				this.Context.Database.Name,
				this.Context.Database.Guid,
				AmExceptionHelper.GetExceptionMessageOrNoneString(exception)
			});
			ReplayCrimsonEvents.AutoReseedManagerWorkflowFailed.Log<string, Guid, string, string, string>(this.Context.Database.Name, this.Context.Database.Guid, this.WorkflowName, this.m_workflowLaunchReason, AmExceptionHelper.GetExceptionMessageOrNoneString(exception));
		}

		private readonly AutoReseedContext m_context;

		private readonly AutoReseedWorkflowType m_workflowType;

		private readonly string m_workflowName;

		private readonly string m_workflowLaunchReason;
	}
}
