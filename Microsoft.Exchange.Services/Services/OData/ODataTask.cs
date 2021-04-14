using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.OData
{
	internal class ODataTask : ITask
	{
		public ODataTask(ODataRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			this.Request = request;
			long num = 0L;
			if (request.ODataContext.RequestDetailsLogger.TryGetLatency(ServiceLatencyMetadata.HttpPipelineLatency, out num))
			{
				long num2 = (long)request.ODataContext.RequestDetailsLogger.ActivityScope.TotalMilliseconds - num;
				request.ODataContext.RequestDetailsLogger.UpdateLatency(ServiceLatencyMetadata.CheckAccessCoreLatency, (double)num2);
			}
			this.Description = request.GetOperationNameForLogging();
			this.WorkloadSettings = new WorkloadSettings(request.ODataContext.CallContext.WorkloadType, request.ODataContext.CallContext.BackgroundLoad);
		}

		public static Task<ODataResponse> CreateTask(ODataRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ODataTask wlmTask = new ODataTask(request);
			if (!request.ODataContext.CallContext.WorkloadManager.TrySubmitNewTask(wlmTask))
			{
				ThreadPool.QueueUserWorkItem(delegate(object o)
				{
					wlmTask.Complete(new ServerBusyException());
				}, null);
			}
			return wlmTask.Task;
		}

		public ODataRequest Request { get; private set; }

		public ODataResponse Response { get; private set; }

		public Task<ODataResponse> Task
		{
			get
			{
				return this.taskCompletionSource.Task;
			}
		}

		public IBudget Budget
		{
			get
			{
				return this.Request.ODataContext.CallContext.Budget;
			}
		}

		public string Description { get; set; }

		public virtual TimeSpan MaxExecutionTime
		{
			get
			{
				return TimeSpan.MaxValue;
			}
		}

		public object State { get; set; }

		public WorkloadSettings WorkloadSettings { get; set; }

		public void Cancel()
		{
			this.Complete(new OperationCanceledException(this.Description));
		}

		public TaskExecuteResult CancelStep(LocalizedException exception)
		{
			return TaskExecuteResult.ProcessingComplete;
		}

		public void Complete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			this.Complete(null);
		}

		public void Complete(Exception error = null)
		{
			Exception ex = error ?? this.executionException;
			if (ex != null)
			{
				this.taskCompletionSource.SetException(ex);
				return;
			}
			this.taskCompletionSource.SetResult(this.Response);
		}

		public TaskExecuteResult Execute(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			try
			{
				HttpContext.Current = this.Request.ODataContext.HttpContext;
				this.Request.Validate();
				using (ODataCommand odataCommand = this.Request.GetODataCommand())
				{
					this.Response = (ODataResponse)odataCommand.Execute();
				}
			}
			catch (Exception ex)
			{
				this.executionException = ex;
			}
			return TaskExecuteResult.ProcessingComplete;
		}

		public IActivityScope GetActivityScope()
		{
			return this.Request.ODataContext.RequestDetailsLogger.ActivityScope;
		}

		public ResourceKey[] GetResources()
		{
			return null;
		}

		public void Timeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
		}

		private TaskCompletionSource<ODataResponse> taskCompletionSource = new TaskCompletionSource<ODataResponse>();

		private Exception executionException;
	}
}
