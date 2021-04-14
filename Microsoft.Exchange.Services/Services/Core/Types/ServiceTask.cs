using System;
using System.ServiceModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ServiceTask<T> : BaseServiceTask<T>
	{
		internal ServiceTask(BaseRequest request, CallContext callContext, ServiceAsyncResult<T> serviceAsyncResult) : base(request, callContext, serviceAsyncResult)
		{
			base.Request.InitializeServiceCommand(callContext);
			OwsLogRegistry.Register(ServiceTask<T>.ServiceTaskActionName, typeof(ServiceTaskMetadata), new Type[0]);
		}

		protected internal override TaskExecuteResult InternalExecute(TimeSpan queueAndDelay, TimeSpan totalTime)
		{
			RequestDetailsLogger requestDetailsLogger = RequestDetailsLogger.Current;
			double num = -1.0;
			long watsonReportCount = ExWatson.WatsonReportCount;
			AggregatedOperationStatistics s = requestDetailsLogger.ActivityScope.TakeStatisticsSnapshot(AggregatedOperationType.ADCalls);
			AggregatedOperationStatistics s2 = requestDetailsLogger.ActivityScope.TakeStatisticsSnapshot(AggregatedOperationType.StoreRPCs);
			TaskExecuteResult taskExecuteResult = TaskExecuteResult.Undefined;
			try
			{
				taskExecuteResult = requestDetailsLogger.TrackLatency<TaskExecuteResult>(ServiceLatencyMetadata.CoreExecutionLatency, delegate()
				{
					T resultData;
					if (this.TryGetResponse(out resultData))
					{
						this.SetResultData(resultData);
						return TaskExecuteResult.ProcessingComplete;
					}
					TaskExecuteResult result;
					try
					{
						TaskExecuteResult taskExecuteResult2 = this.ExecuteHelper(() => base.Request.ServiceCommand.ExecuteStep());
						if (taskExecuteResult2 == TaskExecuteResult.ProcessingComplete)
						{
							this.SetResponse(base.ServiceAsyncResult.Data, base.ServiceAsyncResult.CompletionState as Exception);
						}
						result = taskExecuteResult2;
					}
					catch (FaultException error)
					{
						this.SetResponse(default(T), error);
						throw;
					}
					catch (LocalizedException error2)
					{
						this.SetResponse(default(T), error2);
						throw;
					}
					return result;
				}, out num);
			}
			finally
			{
				AggregatedOperationStatistics aggregatedOperationStatistics = requestDetailsLogger.ActivityScope.TakeStatisticsSnapshot(AggregatedOperationType.ADCalls) - s;
				this.totalADCount += aggregatedOperationStatistics.Count;
				this.totalADLatency += aggregatedOperationStatistics.TotalMilliseconds;
				AggregatedOperationStatistics aggregatedOperationStatistics2 = requestDetailsLogger.ActivityScope.TakeStatisticsSnapshot(AggregatedOperationType.StoreRPCs) - s2;
				this.totalRpcCount += aggregatedOperationStatistics2.Count;
				this.totalRpcLatency += aggregatedOperationStatistics2.TotalMilliseconds;
				long num2 = ExWatson.WatsonReportCount - watsonReportCount;
				this.totalWatsonCount += num2;
				if (taskExecuteResult == TaskExecuteResult.ProcessingComplete || taskExecuteResult == TaskExecuteResult.Undefined)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(base.CallContext.ProtocolLog, ServiceLatencyMetadata.CoreExecutionLatency, num);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(base.CallContext.ProtocolLog, ServiceTaskMetadata.WatsonReportCount, this.totalWatsonCount);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(base.CallContext.ProtocolLog, ServiceTaskMetadata.ADCount, this.totalADCount);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(base.CallContext.ProtocolLog, ServiceTaskMetadata.ADLatency, this.totalADLatency);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(base.CallContext.ProtocolLog, ServiceTaskMetadata.RpcCount, this.totalRpcCount);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(base.CallContext.ProtocolLog, ServiceTaskMetadata.RpcLatency, this.totalRpcLatency);
				}
			}
			return taskExecuteResult;
		}

		private bool TryGetResponse(out T response)
		{
			try
			{
				return base.CallContext.TryGetResponse<T>(out response);
			}
			catch (FaultException executionException)
			{
				this.executionException = executionException;
			}
			catch (LocalizedException executionException2)
			{
				this.executionException = executionException2;
			}
			response = default(T);
			return true;
		}

		private void SetResponse(T response, Exception error)
		{
			try
			{
				base.CallContext.SetResponse<T>(response, error);
			}
			catch (FaultException executionException)
			{
				this.executionException = executionException;
			}
			catch (LocalizedException executionException2)
			{
				this.executionException = executionException2;
			}
		}

		protected internal override TaskExecuteResult InternalCancelStep(LocalizedException exception)
		{
			return this.ExecuteHelper(() => this.Request.ServiceCommand.CancelStep(exception));
		}

		private TaskExecuteResult ExecuteHelper(Func<TaskExecuteResult> multiStepAction)
		{
			TaskExecuteResult result = TaskExecuteResult.ProcessingComplete;
			base.SendWatsonReportOnGrayException(delegate()
			{
				try
				{
					bool flag = true;
					if (this.initial)
					{
						flag = this.Request.ServiceCommand.PreExecute();
						this.initial = false;
					}
					TaskExecuteResult taskExecuteResult;
					if (!flag)
					{
						taskExecuteResult = TaskExecuteResult.ProcessingComplete;
					}
					else
					{
						taskExecuteResult = multiStepAction();
					}
					if (taskExecuteResult == TaskExecuteResult.ProcessingComplete)
					{
						this.SetResultData((T)((object)this.Request.ServiceCommand.PostExecute()));
					}
					result = taskExecuteResult;
				}
				catch (FaultException ex)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(this.CallContext.ProtocolLog, ex, "ServiceTask_ExecuteHelper");
					this.executionException = ex;
				}
				catch (BailOutException executionException)
				{
					this.executionException = executionException;
				}
			});
			return result;
		}

		protected internal override void InternalComplete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			this.FinishRequest("[C]", queueAndDelayTime, totalTime, this.executionException);
		}

		protected internal override void InternalTimeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			this.ExecuteHelper(delegate
			{
				base.Request.ServiceCommand.CancelStep(new RequestTimeoutException());
				return TaskExecuteResult.ProcessingComplete;
			});
			this.FinishRequest("[T]", queueAndDelayTime, totalTime, this.executionException);
		}

		protected internal override ResourceKey[] InternalGetResources()
		{
			ResourceKey[] resources = base.Request.ServiceCommand.GetResources();
			if (resources == null || resources.Length == 0)
			{
				return ServiceTask<T>.ResourceKeysWithLocalCPUOnly;
			}
			ResourceKey[] array = new ResourceKey[resources.Length + 1];
			array[0] = ProcessorResourceKey.Local;
			Array.Copy(resources, 0, array, 1, resources.Length);
			return array;
		}

		private const string CompleteLog = "[C]";

		private const string CompleteWithExceptionLog = "[CWE]";

		private const string CanceledLog = "[X]";

		private const string TimedOutLog = "[T]";

		private static readonly string ServiceTaskActionName = "ServiceTask";

		private static readonly ResourceKey[] ResourceKeysWithLocalCPUOnly = new ResourceKey[]
		{
			ProcessorResourceKey.Local
		};

		private bool initial = true;

		private long totalADCount;

		private long totalRpcCount;

		private long totalWatsonCount;

		private double totalADLatency;

		private double totalRpcLatency;
	}
}
