using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.PushNotifications.Server.Core
{
	internal abstract class ServiceCommand<Request, Response> : IServiceCommand, ITask
	{
		public ServiceCommand(Request request, AsyncCallback asyncCallback, object asyncState)
		{
			this.Description = base.GetType().Name;
			this.asyncState = asyncState;
			this.asyncCallback = asyncCallback;
			this.RequestInstance = request;
		}

		public Request RequestInstance { get; private set; }

		public IAsyncResult CommandAsyncResult
		{
			get
			{
				return this.asyncResult;
			}
		}

		public IBudget Budget { get; private set; }

		public string Description { get; set; }

		public virtual TimeSpan MaxExecutionTime
		{
			get
			{
				return ServiceCommand<Request, Response>.DefaultMaxExecutionTime;
			}
		}

		public object State { get; set; }

		public WorkloadSettings WorkloadSettings
		{
			get
			{
				return ServiceCommand<Request, Response>.WorkloadSettingsInstance;
			}
		}

		private Exception CommandError { get; set; }

		public void Cancel()
		{
			ExTraceGlobals.PushNotificationServiceTracer.TraceDebug<string>((long)this.GetHashCode(), "ServiceCommand.Cancel: ServiceCommand cancelled for {0}.", this.Description);
			try
			{
				this.InternalCancel();
			}
			finally
			{
				this.Complete(new OperationCancelledException(this.Description));
			}
		}

		public TaskExecuteResult CancelStep(LocalizedException exception)
		{
			return this.InternalCancelStep(exception);
		}

		public void Complete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			ExTraceGlobals.PushNotificationServiceTracer.TraceDebug<string, TimeSpan, TimeSpan>((long)this.GetHashCode(), "ServiceCommand.Complete: Complete with no exception called for ServiceCommand {0}. Delay: {1}, Elapsed: {2}", this.Description, queueAndDelayTime, totalTime);
			try
			{
				this.InternalComplete(queueAndDelayTime, totalTime);
			}
			finally
			{
				this.Complete(null);
			}
		}

		public TaskExecuteResult Execute(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			TaskExecuteResult result = TaskExecuteResult.ProcessingComplete;
			try
			{
				this.asyncResult.Result = this.InternalExecute(queueAndDelayTime, totalTime);
			}
			catch (Exception ex)
			{
				this.CommandError = ex;
				ExTraceGlobals.PushNotificationServiceTracer.TraceError<string>((long)this.GetHashCode(), "ServiceCommand.Execute: An Exception was reported from the InternalExecute call {0}.", ex.ToTraceString());
			}
			return result;
		}

		public IActivityScope GetActivityScope()
		{
			return ActivityContext.GetCurrentActivityScope();
		}

		public ResourceKey[] GetResources()
		{
			return this.InternalGetResources();
		}

		public void Timeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			ExTraceGlobals.PushNotificationServiceTracer.TraceDebug<string, TimeSpan, TimeSpan>((long)this.GetHashCode(), "ServiceCommand.Timeout: Timeout called for ServiceCommoand {0}.  Delay: {1}, Elapsed: {2}", this.Description, queueAndDelayTime, totalTime);
			this.InternalTimeout(queueAndDelayTime, totalTime);
		}

		public void Initialize(IBudget budget)
		{
			ArgumentValidator.ThrowIfNull("budget", budget);
			this.Budget = budget;
			this.asyncResult = new ServiceCommandAsyncResult<Response>(this.asyncCallback, this.asyncState);
			this.activityScope = ActivityContext.Start(ActivityType.Request);
			this.activityScope.Action = this.Description;
			this.activityScope.Component = WorkloadType.PushNotificationService.ToString();
			this.InternalInitialize(budget);
		}

		public void Complete(Exception error = null)
		{
			if (!this.asyncResult.IsCompleted)
			{
				try
				{
					try
					{
						Exception ex = this.CommandError;
						if (error != null)
						{
							ex = ((ex != null) ? new AggregateException(new Exception[]
							{
								ex,
								error
							}) : error);
						}
						this.asyncResult.Complete(ex, false);
					}
					catch (InvalidOperationException exception)
					{
						ExTraceGlobals.PushNotificationServiceTracer.TraceError<string>((long)this.GetHashCode(), "ServiceCommand.Complete: WCF request was already completed on another worker thread. {0}", exception.ToTraceString());
					}
					return;
				}
				finally
				{
					this.Budget.Dispose();
					this.activityScope.End();
				}
			}
			ExTraceGlobals.PushNotificationServiceTracer.TraceDebug((long)this.GetHashCode(), "ServiceCommand.Complete: WCF request was already completed on another worker thread.");
		}

		protected abstract ResourceKey[] InternalGetResources();

		protected abstract Response InternalExecute(TimeSpan queueAndDelay, TimeSpan totalTime);

		protected virtual void InternalInitialize(IBudget budget)
		{
		}

		protected virtual void InternalCancel()
		{
		}

		protected virtual void InternalComplete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
		}

		protected virtual void InternalTimeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
		}

		protected virtual TaskExecuteResult InternalCancelStep(LocalizedException exception)
		{
			return TaskExecuteResult.ProcessingComplete;
		}

		private static readonly TimeSpan DefaultMaxExecutionTime = TimeSpan.FromMinutes(5.0);

		private static readonly WorkloadSettings WorkloadSettingsInstance = new WorkloadSettings(WorkloadType.PushNotificationService, false);

		private ServiceCommandAsyncResult<Response> asyncResult;

		private AsyncCallback asyncCallback;

		private object asyncState;

		private IActivityScope activityScope;
	}
}
