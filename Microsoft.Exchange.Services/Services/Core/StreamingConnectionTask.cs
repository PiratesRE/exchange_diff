using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core
{
	internal class StreamingConnectionTask : ITask, IDisposeTrackable, IDisposable
	{
		internal StreamingConnectionTask(StreamingConnection connection, CallContext callContext, StreamingConnectionTask.StreamingConnectionExecuteDelegate executeDelegate, string taskType)
		{
			this.connection = connection;
			this.callContext = callContext;
			this.executeDelegate = executeDelegate;
			this.Description = string.Format("StreamingConnectionTask: Connection: [{0}], Type:[{1}]", this.connection.GetHashCode(), taskType);
			this.WorkloadSettings = new WorkloadSettings(this.callContext.WorkloadType, this.callContext.BackgroundLoad);
			this.budget = EwsBudget.Acquire(this.callContext.Budget.Owner);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StreamingConnectionTask>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public IBudget Budget
		{
			get
			{
				return this.budget;
			}
		}

		public void Cancel()
		{
			ExTraceGlobals.ThrottlingTracer.TraceDebug<string>((long)this.GetHashCode(), "[StreamingConnectionTask.Cancel] Task.Cancel called for task {0}", this.Description);
			this.Dispose();
		}

		public void Complete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			this.Dispose();
		}

		public string Description { get; set; }

		public IActivityScope GetActivityScope()
		{
			IActivityScope result = null;
			if (this.callContext != null && this.callContext.ProtocolLog != null)
			{
				result = this.callContext.ProtocolLog.ActivityScope;
			}
			return result;
		}

		public TaskExecuteResult Execute(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			TaskExecuteResult result;
			try
			{
				CallContext.SetCurrent(this.callContext);
				result = this.executeDelegate();
			}
			finally
			{
				CallContext.SetCurrent(null);
			}
			return result;
		}

		public TimeSpan MaxExecutionTime
		{
			get
			{
				return StreamingConnectionTask.DefaultMaxExecutionTime;
			}
		}

		public object State { get; set; }

		public void Timeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			this.Dispose();
			this.connection.TryEndConnection(false);
		}

		public TaskExecuteResult CancelStep(LocalizedException exception)
		{
			this.connection.TryEndConnection(exception);
			return TaskExecuteResult.ProcessingComplete;
		}

		public ResourceKey[] GetResources()
		{
			return null;
		}

		public WorkloadSettings WorkloadSettings { get; private set; }

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
		}

		private void Dispose(bool suppressFinalize)
		{
			if (!this.isDisposed)
			{
				if (suppressFinalize)
				{
					GC.SuppressFinalize(this);
				}
				if (this.budget != null)
				{
					this.budget.LogEndStateToIIS();
					this.budget.Dispose();
				}
				this.isDisposed = true;
			}
		}

		private readonly DisposeTracker disposeTracker;

		private static readonly TimeSpan DefaultMaxExecutionTime = TimeSpan.FromMinutes(1.0);

		private StreamingConnection connection;

		private CallContext callContext;

		private StreamingConnectionTask.StreamingConnectionExecuteDelegate executeDelegate;

		private IEwsBudget budget;

		private bool isDisposed;

		internal delegate TaskExecuteResult StreamingConnectionExecuteDelegate();
	}
}
