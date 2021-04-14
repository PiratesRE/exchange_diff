using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ReplicationCheck : DisposeTrackableBase, IReplicationCheck
	{
		protected virtual string ErrorKey
		{
			get
			{
				return this.m_ErrorKey = this.GetDefaultErrorKey(base.GetType());
			}
			set
			{
				this.m_ErrorKey = value;
			}
		}

		protected string InstanceIdentity
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_instanceIdentity))
				{
					return this.m_instanceIdentity = this.m_ServerName;
				}
				return this.m_instanceIdentity;
			}
			set
			{
				this.m_instanceIdentity = value;
			}
		}

		public string Title
		{
			get
			{
				return this.m_Title;
			}
		}

		public CheckId CheckId
		{
			get
			{
				return this.m_checkId;
			}
		}

		public LocalizedString Description
		{
			get
			{
				return this.m_Description;
			}
		}

		public CheckCategory Category
		{
			get
			{
				return this.m_Category;
			}
		}

		public string ServerName
		{
			get
			{
				return this.m_ServerName;
			}
		}

		public IEventManager EventManager
		{
			get
			{
				return this.m_EventManager;
			}
		}

		public uint? IgnoreTransientErrorsThreshold
		{
			get
			{
				return this.m_ignoreTransientErrorsThreshold;
			}
		}

		public bool HasRun
		{
			get
			{
				return this.m_checkResultInfo.HasRun;
			}
		}

		public bool HasError
		{
			get
			{
				return this.m_checkResultInfo.HasFailures || this.m_checkResultInfo.HasWarnings || this.m_checkResultInfo.HasTransientError;
			}
		}

		public bool HasPassed
		{
			get
			{
				return this.m_checkResultInfo.HasPassed;
			}
		}

		public ReplicationCheckOutcome Outcome
		{
			get
			{
				return this.GetCheckOutcome();
			}
		}

		public List<ReplicationCheckOutputObject> OutputObjects
		{
			get
			{
				return this.GetCheckOutputObjects();
			}
		}

		public ReplicationCheck(string title, CheckId checkId, LocalizedString description, CheckCategory category, IEventManager eventManager, string momEventSource, string serverName) : this(title, checkId, description, category, eventManager, momEventSource, serverName, null)
		{
		}

		public ReplicationCheck(string title, CheckId checkId, LocalizedString description, CheckCategory category, IEventManager eventManager, string momEventSource, string serverName, uint? ignoreTransientErrorsThreshold)
		{
			this.m_Title = title;
			this.m_checkId = checkId;
			this.m_Description = description;
			this.m_Category = category;
			this.m_ServerName = serverName;
			this.m_EventManager = eventManager;
			this.m_checkResultInfo = new ReplicationCheckResultInfo(this);
			uint? num = ignoreTransientErrorsThreshold;
			this.m_ignoreTransientErrorsThreshold = ((num != null) ? new uint?(num.GetValueOrDefault()) : new uint?(0U));
		}

		public virtual void Run()
		{
			try
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "Run(): Check {0} has started running.", this.Title);
				this.InternalRun();
				if (this.HasError)
				{
					if (this.m_checkResultInfo.HasFailures)
					{
						this.FailInternal();
					}
					else
					{
						if (this.m_checkResultInfo.HasWarnings)
						{
							throw new ReplicationCheckWarningException(this.m_Title, this.BuildErrorMessageForOutcome());
						}
						if (this.m_checkResultInfo.HasTransientError)
						{
							this.m_checkResultInfo.SetPassed();
						}
					}
				}
				else
				{
					this.MarkCheckAsPassed();
				}
			}
			catch (ReplicationCheckSkippedException)
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "Skip(): Check '{0}' is being skipped.", this.Title);
			}
			catch (DataSourceTransientException ex)
			{
				this.HandleKnownException(ex);
			}
			catch (ClusterException ex2)
			{
				this.HandleKnownException(ex2);
			}
			catch (DataSourceOperationException ex3)
			{
				this.HandleKnownException(ex3);
			}
			catch (AmServerException ex4)
			{
				this.HandleKnownException(ex4);
			}
			catch (AmServerTransientException ex5)
			{
				this.HandleKnownException(ex5);
			}
			finally
			{
				if (!this.m_hasSkipped)
				{
					this.m_checkResultInfo.HasRun = true;
				}
			}
		}

		private void HandleKnownException(Exception ex)
		{
			this.m_checkResultInfo.AddFailure(this.InstanceIdentity, ex.Message, true);
			string errorMessage = this.BuildErrorMessageForOutcome();
			if (this.m_Category == CheckCategory.SystemHighPriority)
			{
				throw new ReplicationCheckHighPriorityFailedException(this.m_Title, errorMessage);
			}
			throw new ReplicationCheckFailedException(this.m_Title, errorMessage);
		}

		public void Skip()
		{
			this.m_hasSkipped = true;
			if (!IgnoreTransientErrors.HasPassed(this.GetDefaultErrorKey(base.GetType())))
			{
				this.FailInternal();
			}
			throw new ReplicationCheckSkippedException();
		}

		private string BuildErrorMessageForOutcome()
		{
			return this.m_checkResultInfo.BuildErrorMessageForOutcome();
		}

		protected abstract void InternalRun();

		public void LogEvents()
		{
			this.m_checkResultInfo.LogEvents(this.m_EventManager);
		}

		public ReplicationCheckOutcome GetCheckOutcome()
		{
			return this.m_checkResultInfo.GetCheckOutcome();
		}

		public List<ReplicationCheckOutputObject> GetCheckOutputObjects()
		{
			return this.m_checkResultInfo.GetCheckOutputObjects();
		}

		private void FailInternal()
		{
			string text = this.BuildErrorMessageForOutcome();
			if (this.m_Category == CheckCategory.SystemHighPriority)
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Critical check {0} has failed!. Error: {1}", this.m_Title, text);
				throw new ReplicationCheckHighPriorityFailedException(this.m_Title, text);
			}
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Check {0} has failed. Error: {1}", this.m_Title, text);
			throw new ReplicationCheckFailedException(this.m_Title, text);
		}

		protected void Fail(LocalizedString error)
		{
			this.m_checkResultInfo.HasTransientError = true;
			bool isTransitioningState;
			if (!IgnoreTransientErrors.IgnoreTransientError(this.ErrorKey, this.IgnoreTransientErrorsThreshold.Value, ErrorType.Failure, out isTransitioningState))
			{
				this.m_checkResultInfo.AddFailure(this.InstanceIdentity, error, isTransitioningState);
				this.FailInternal();
				return;
			}
			this.m_checkResultInfo.AddSuccess(this.InstanceIdentity, isTransitioningState);
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Check '{0}' is logging treating the failure for instance '{1}' as transient and reporting a success record instead.", this.Title, this.InstanceIdentity);
		}

		protected void FailContinue(LocalizedString error)
		{
			this.FailContinue(error, 0U);
		}

		protected void FailContinue(LocalizedString error, uint dbFailureEventId)
		{
			this.m_checkResultInfo.HasTransientError = true;
			bool isTransitioningState;
			if (!IgnoreTransientErrors.IgnoreTransientError(this.ErrorKey, this.IgnoreTransientErrorsThreshold.Value, ErrorType.Failure, out isTransitioningState))
			{
				this.m_checkResultInfo.AddFailure(this.InstanceIdentity, error, false, (dbFailureEventId != 0U) ? new uint?(dbFailureEventId) : null, isTransitioningState);
				return;
			}
			this.m_checkResultInfo.AddSuccess(this.InstanceIdentity, isTransitioningState);
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Check '{0}' is logging treating the failure for instance '{1}' as transient and reporting a success record instead.", this.Title, this.InstanceIdentity);
		}

		protected void FailFatal(LocalizedString error)
		{
			this.m_checkResultInfo.HasTransientError = true;
			bool isTransitioningState;
			if (!IgnoreTransientErrors.IgnoreTransientError(this.ErrorKey, this.IgnoreTransientErrorsThreshold.Value, ErrorType.Failure, out isTransitioningState))
			{
				this.m_checkResultInfo.AddFailure(this.InstanceIdentity, Strings.ReplicationCheckFatalError(this.m_Title, error), isTransitioningState);
				throw new ReplicationCheckFatalException(this.m_Title, this.BuildErrorMessageForOutcome());
			}
			this.m_checkResultInfo.AddSuccess(this.InstanceIdentity, isTransitioningState);
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Check '{0}' is logging treating the failure for instance '{1}' as transient and reporting a success record instead.", this.Title, this.InstanceIdentity);
		}

		protected void WriteWarning(LocalizedString warning)
		{
			this.m_checkResultInfo.HasTransientError = true;
			bool isTransitioningState;
			if (!IgnoreTransientErrors.IgnoreTransientError(this.ErrorKey, this.IgnoreTransientErrorsThreshold.Value, ErrorType.Warning, out isTransitioningState))
			{
				this.m_checkResultInfo.AddWarning(this.InstanceIdentity, warning, isTransitioningState);
				string text = this.BuildErrorMessageForOutcome();
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Check {0} has issued a warning. Error: {1}", this.m_Title, text);
				throw new ReplicationCheckWarningException(this.m_Title, text);
			}
			this.m_checkResultInfo.AddSuccess(this.InstanceIdentity, isTransitioningState);
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Check '{0}' is logging treating the warning for instance '{1}' as transient and reporting a success record instead.", this.Title, this.InstanceIdentity);
		}

		protected void WriteWarningContinue(LocalizedString warning)
		{
			this.m_checkResultInfo.HasTransientError = true;
			bool isTransitioningState;
			if (!IgnoreTransientErrors.IgnoreTransientError(this.ErrorKey, this.IgnoreTransientErrorsThreshold.Value, ErrorType.Warning, out isTransitioningState))
			{
				this.m_checkResultInfo.AddWarning(this.InstanceIdentity, warning, isTransitioningState);
				return;
			}
			this.m_checkResultInfo.AddSuccess(this.InstanceIdentity, isTransitioningState);
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Check '{0}' is logging treating the warning for instance '{1}' as transient and reporting a success record instead.", this.Title, this.InstanceIdentity);
		}

		protected void ReportPassedInstance()
		{
			bool isTransitioningState = IgnoreTransientErrors.ResetError(this.ErrorKey);
			this.m_checkResultInfo.AddSuccess(this.InstanceIdentity, isTransitioningState);
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Check {0} is recording a pass entry for instance identity '{1}'.", this.Title, this.InstanceIdentity);
		}

		protected void MarkCheckAsPassed()
		{
			bool isTransitioningState = IgnoreTransientErrors.ResetError(this.ErrorKey);
			this.m_checkResultInfo.SetPassed(this.InstanceIdentity, isTransitioningState);
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "Check {0} has passed.", this.Title);
		}

		protected string GetDefaultErrorKey(Type type)
		{
			return string.Concat(new string[]
			{
				this.ServerName,
				"|",
				type.Name,
				"|",
				this.InstanceIdentity
			});
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ReplicationCheck>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
		}

		private readonly string m_Title;

		private readonly CheckId m_checkId;

		private readonly LocalizedString m_Description;

		private readonly CheckCategory m_Category;

		private readonly string m_ServerName;

		private readonly IEventManager m_EventManager;

		private readonly uint? m_ignoreTransientErrorsThreshold;

		private bool m_hasSkipped;

		private ReplicationCheckResultInfo m_checkResultInfo;

		protected string m_ErrorKey;

		protected string m_instanceIdentity;
	}
}
