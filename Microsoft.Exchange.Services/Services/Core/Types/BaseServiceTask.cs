using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class BaseServiceTask<T> : ITask
	{
		internal BaseServiceTask(BaseRequest request, CallContext callContext, ServiceAsyncResult<T> serviceAsyncResult)
		{
			this.Request = request;
			this.CallContext = callContext;
			this.ServiceAsyncResult = serviceAsyncResult;
			this.WorkloadSettings = new WorkloadSettings(callContext.WorkloadType, callContext.BackgroundLoad);
		}

		public object State { get; set; }

		public string Description { get; set; }

		internal CallContext CallContext { get; private set; }

		internal BaseRequest Request { get; private set; }

		internal ServiceAsyncResult<T> ServiceAsyncResult { get; private set; }

		public void Cancel()
		{
			ExTraceGlobals.ThrottlingTracer.TraceDebug<string>((long)this.GetHashCode(), "[BaseServiceTask.Cancel] Task.Cancel called for task {0}", this.Description);
			this.InternalCancel();
		}

		public IActivityScope GetActivityScope()
		{
			IActivityScope result = null;
			if (this.CallContext != null && this.CallContext.ProtocolLog != null)
			{
				result = this.CallContext.ProtocolLog.ActivityScope;
			}
			return result;
		}

		public TaskExecuteResult Execute(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			TaskExecuteResult result = TaskExecuteResult.ProcessingComplete;
			this.ExecuteWithinCallContext(delegate
			{
				RequestDetailsLogger.LogEvent(this.CallContext.ProtocolLog, ServiceTaskMetadata.ServiceCommandBegin);
				using (CpuTracker.StartCpuTracking("CMD"))
				{
					IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
					Guid relatedActivityId;
					if (currentActivityScope != null)
					{
						if (string.IsNullOrEmpty(currentActivityScope.Component) && this.CallContext != null)
						{
							currentActivityScope.Component = this.CallContext.WorkloadType.ToString();
						}
						if (string.IsNullOrEmpty(currentActivityScope.Action) && this.Request.ServiceCommand != null)
						{
							currentActivityScope.Action = this.Request.ServiceCommand.GetType().Name;
						}
						relatedActivityId = currentActivityScope.ActivityId;
					}
					else
					{
						relatedActivityId = Guid.NewGuid();
					}
					using (ExPerfTrace.RelatedActivity(relatedActivityId))
					{
						((IEwsBudget)this.Budget).StartPerformanceContext();
						try
						{
							result = this.InternalExecute(queueAndDelayTime, totalTime);
						}
						finally
						{
							((IEwsBudget)this.Budget).StopPerformanceContext();
						}
					}
					RequestDetailsLogger.LogEvent(this.CallContext.ProtocolLog, ServiceTaskMetadata.ServiceCommandEnd);
				}
			});
			return result;
		}

		public WorkloadSettings WorkloadSettings { get; private set; }

		public void Complete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			ExTraceGlobals.ThrottlingTracer.TraceDebug<string, TimeSpan, TimeSpan>((long)this.GetHashCode(), "[BaseServiceTask.Complete] Complete with no exception called for task {0}.  Delay: {1}, Elapsed: {2}", this.Description, queueAndDelayTime, totalTime);
			this.InternalComplete(queueAndDelayTime, totalTime);
		}

		private void ExecuteWithinCallContext(Action action)
		{
			CallContext current = CallContext.Current;
			try
			{
				CallContext.SetCurrent(this.CallContext);
				if (ExchangeVersion.Current == null)
				{
					ExchangeVersion.Current = ExchangeVersion.Latest;
				}
				bool flag = false;
				if (this.CallContext.AccessingPrincipal != null && ExUserTracingAdaptor.Instance.IsTracingEnabledUser(this.CallContext.AccessingPrincipal.LegacyDn))
				{
					flag = true;
					BaseTrace.CurrentThreadSettings.EnableTracing();
				}
				try
				{
					action();
				}
				finally
				{
					if (flag)
					{
						BaseTrace.CurrentThreadSettings.DisableTracing();
					}
				}
			}
			finally
			{
				CallContext.SetCurrent(current);
			}
		}

		protected void CompleteWCFRequest(Exception exception)
		{
			if (exception != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(this.CallContext.ProtocolLog, exception, "BaseServiceTask_CompleteWCFRequest");
			}
			if (!this.ServiceAsyncResult.IsCompleted)
			{
				this.ServiceAsyncResult.Complete(exception);
				return;
			}
			ExTraceGlobals.ThrottlingTracer.TraceDebug<string>((long)this.GetHashCode(), "[BaseServiceTask::CompleteWCFRequest] WCF request was already complete on another worker thread.  Exception: {0}", (exception == null) ? "<NULL>" : exception.ToString());
		}

		protected virtual void FinishRequest(string logType, TimeSpan queueAndDelayTime, TimeSpan totalTime, Exception exception)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.CallContext.ProtocolLog, BudgetMetadata.ThrottlingDelay, queueAndDelayTime.TotalMilliseconds);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.CallContext.ProtocolLog, BudgetMetadata.ThrottlingRequestType, logType);
			if (exception != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(this.CallContext.ProtocolLog, exception, "BaseServiceTask_FinishRequest");
			}
			this.WriteThrottlingDiagnostics(logType, queueAndDelayTime, totalTime);
			this.CompleteWCFRequest(exception);
		}

		public virtual IBudget Budget
		{
			get
			{
				return this.CallContext.Budget;
			}
		}

		public virtual TimeSpan MaxExecutionTime
		{
			get
			{
				if (this.Request != null && this.Request.ServiceCommand != null && this.Request.ServiceCommand.MaxExecutionTime != null && this.Request.ServiceCommand.MaxExecutionTime != null)
				{
					return this.Request.ServiceCommand.MaxExecutionTime.Value;
				}
				return BaseServiceTask<T>.DefaultMaxExecutionTime;
			}
		}

		public void Timeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			ExTraceGlobals.ThrottlingTracer.TraceDebug<string, TimeSpan, TimeSpan>((long)this.GetHashCode(), "[BaseServiceTask.Timeout] Timeout called for task {0}.  Delay: {1}, Elapsed: {2}", this.Description, queueAndDelayTime, totalTime);
			try
			{
				CallContext.SetCurrent(this.CallContext);
				this.InternalTimeout(queueAndDelayTime, totalTime);
			}
			finally
			{
				CallContext.SetCurrent(null);
			}
		}

		public TaskExecuteResult CancelStep(LocalizedException exception)
		{
			TaskExecuteResult result;
			try
			{
				CallContext.SetCurrent(this.CallContext);
				if (exception != null)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(this.CallContext.ProtocolLog, exception, "BaseServiceTask_Cancel");
				}
				result = this.InternalCancelStep(exception);
			}
			finally
			{
				CallContext.SetCurrent(null);
			}
			return result;
		}

		public ResourceKey[] GetResources()
		{
			ResourceKey[] resourceKeys = null;
			try
			{
				CallContext.SetCurrent(this.CallContext);
				this.SendWatsonReportOnGrayException(delegate()
				{
					resourceKeys = this.InternalGetResources();
				}, null, false);
			}
			finally
			{
				CallContext.SetCurrent(null);
			}
			return resourceKeys;
		}

		protected virtual void SetResultData(T response)
		{
			this.ServiceAsyncResult.Data = response;
		}

		protected internal virtual void InternalCancel()
		{
		}

		protected string GetTimesLogString(string logType, TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			return string.Format("{0}Queues:{1}msec/Execute:{2}msec;", logType, queueAndDelayTime.TotalMilliseconds, (totalTime == TimeSpan.Zero) ? 0.0 : (totalTime - queueAndDelayTime).TotalMilliseconds);
		}

		protected void SendWatsonReportOnGrayException(BaseServiceTask<T>.GrayExceptionCallback callback)
		{
			this.SendWatsonReportOnGrayException(callback, null, true);
		}

		protected void SendWatsonReportOnGrayException(BaseServiceTask<T>.GrayExceptionCallback callback, BaseServiceTask<T>.GrayExceptionHandlerCallback exceptionHandlerCallback)
		{
			this.SendWatsonReportOnGrayException(callback, exceptionHandlerCallback, true);
		}

		private void SendWatsonReportOnGrayException(BaseServiceTask<T>.GrayExceptionCallback callback, BaseServiceTask<T>.GrayExceptionHandlerCallback exceptionHandlerCallback, bool isGrayExceptionTaskFailure)
		{
			Exception ex = null;
			string formatString = null;
			ServiceDiagnostics.RegisterAdditionalWatsonData();
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					callback();
				}, new GrayException.ExceptionFilterDelegate(BaseServiceTask<T>.GrayExceptionFilter));
			}
			catch (GrayException ex2)
			{
				ex = ex2;
				if (isGrayExceptionTaskFailure)
				{
					formatString = "Task {0} failed: {1}";
					if (this.Budget != null)
					{
						this.IncrementFailedCount();
					}
					this.executionException = ex2;
					if (ex2 != null)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(this.CallContext.ProtocolLog, ex2, "BaseServiceTask_SendWatsonReportOnGrayException");
					}
					if (exceptionHandlerCallback != null)
					{
						exceptionHandlerCallback();
					}
				}
				else
				{
					formatString = "Task {0} ignored exception: {1}";
				}
			}
			finally
			{
				ExWatson.ClearReportActions(WatsonActionScope.Thread);
			}
			if (ex != null)
			{
				ExTraceGlobals.ThrottlingTracer.TraceDebug<string, Exception>((long)this.GetHashCode(), formatString, this.Description, ex);
			}
		}

		protected static bool GrayExceptionFilter(object exception)
		{
			bool flag = false;
			Exception ex = exception as Exception;
			if (ex != null && ExWatson.IsWatsonReportAlreadySent(ex))
			{
				flag = true;
			}
			bool flag2 = GrayException.ExceptionFilter(exception);
			if (flag2 && !flag && ex != null)
			{
				ExWatson.SetWatsonReportAlreadySent(ex);
			}
			return flag2;
		}

		protected internal abstract TaskExecuteResult InternalExecute(TimeSpan queueAndDelay, TimeSpan totalTime);

		protected internal abstract void InternalComplete(TimeSpan queueAndDelayTime, TimeSpan totalTime);

		protected internal abstract void InternalTimeout(TimeSpan queueAndDelayTime, TimeSpan totalTime);

		protected internal abstract TaskExecuteResult InternalCancelStep(LocalizedException exception);

		protected internal abstract ResourceKey[] InternalGetResources();

		protected virtual void WriteThrottlingDiagnostics(string logType, TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			if (Global.WriteThrottlingDiagnostics)
			{
				string timesLogString = this.GetTimesLogString(logType, queueAndDelayTime, totalTime);
				this.CallContext.HttpContext.Response.AppendHeader("X-ThrottlingDiagnostics", timesLogString + this.Budget.ToString());
			}
		}

		private void IncrementFailedCount()
		{
			UserWorkloadManager.GetPerfCounterWrapper(this.Budget.Owner.BudgetType).UpdateTotalTaskExecutionFailuresCount();
		}

		private const string WlmQueueReSubmitKey = "WlmQueueReSubmitTime";

		private const string WlmQueueSubmitKey = "WlmQueueSubmitTime";

		private const string WlmLatencyKey = "WlmQueueingLatency";

		private static readonly TimeSpan DefaultMaxExecutionTime = TimeSpan.FromMinutes(1.0);

		protected Exception executionException;

		internal delegate void GrayExceptionCallback();

		internal delegate void GrayExceptionHandlerCallback();
	}
}
