using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal abstract class WorkloadManagerDispatchTask : DispatchTask, ITask
	{
		public WorkloadManagerDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
			this.workloadSettings = new WorkloadSettings(WorkloadType.Domt, false);
		}

		public WorkloadSettings WorkloadSettings
		{
			get
			{
				base.CheckDisposed();
				return this.workloadSettings;
			}
		}

		public object State
		{
			get
			{
				base.CheckDisposed();
				return this.state;
			}
			set
			{
				base.CheckDisposed();
				this.state = value;
			}
		}

		public string Description
		{
			get
			{
				base.CheckDisposed();
				return this.description;
			}
			set
			{
				base.CheckDisposed();
				this.description = value;
			}
		}

		public TimeSpan MaxExecutionTime
		{
			get
			{
				base.CheckDisposed();
				return Configuration.MaxExecutionTime;
			}
		}

		public abstract IBudget Budget { get; }

		public virtual IActivityScope GetActivityScope()
		{
			base.CheckDisposed();
			return null;
		}

		public TaskExecuteResult Execute(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			IActivityScope activityScope = this.GetActivityScope();
			TaskExecuteResult result;
			using (new ActivityScopeThreadGuard(activityScope))
			{
				if (activityScope != null)
				{
					activityScope.Action = this.TaskName;
				}
				result = this.ExecuteTask(queueAndDelayTime, totalTime, false);
			}
			return result;
		}

		public void Complete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			IActivityScope activityScope = this.GetActivityScope();
			using (new ActivityScopeThreadGuard(activityScope))
			{
				if (activityScope != null)
				{
					activityScope.Action = this.TaskName;
				}
				base.CheckDisposed();
				base.Completion();
			}
		}

		public void Cancel()
		{
			base.CheckDisposed();
			base.Completion();
		}

		public void Timeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			this.ExecuteTask(queueAndDelayTime, totalTime, true);
			this.Complete(queueAndDelayTime, totalTime);
		}

		public ResourceKey[] GetResources()
		{
			base.CheckDisposed();
			return null;
		}

		public TaskExecuteResult CancelStep(LocalizedException exception)
		{
			base.CheckDisposed();
			return TaskExecuteResult.ProcessingComplete;
		}

		protected abstract void InternalPreExecute();

		protected abstract void InternalExecute();

		protected abstract void InternalPostExecute(TimeSpan queueAndDelayTime, TimeSpan totalTime, bool calledFromTimeout);

		protected abstract bool TryHandleException(Exception exception);

		public TaskExecuteResult ExecuteTask(TimeSpan queueAndDelayTime, TimeSpan totalTime, bool calledFromTimeout)
		{
			base.CheckDisposed();
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				this.InternalPreExecute();
				try
				{
					if (!calledFromTimeout)
					{
						this.InternalExecute();
					}
				}
				catch (Exception exception)
				{
					if (!this.TryHandleException(exception))
					{
						if (Debugger.IsAttached)
						{
							Debugger.Break();
						}
						else
						{
							if (Configuration.CrashOnUnhandledException)
							{
								try
								{
									ExWatson.SendReportAndCrashOnAnotherThread(exception);
									goto IL_63;
								}
								finally
								{
									try
									{
										Process.GetCurrentProcess().Kill();
									}
									catch (Win32Exception)
									{
									}
									Environment.Exit(1);
								}
							}
							ExWatson.SendReport(exception, ReportOptions.DoNotFreezeThreads, null);
						}
					}
					IL_63:;
				}
				finally
				{
					stopwatch.Stop();
				}
				this.InternalPostExecute(queueAndDelayTime, stopwatch.Elapsed, calledFromTimeout);
			}
			finally
			{
				if (BaseTrace.CurrentThreadSettings.IsEnabled)
				{
					BaseTrace.CurrentThreadSettings.DisableTracing();
				}
			}
			return TaskExecuteResult.ProcessingComplete;
		}

		private WorkloadSettings workloadSettings;

		private object state;

		private string description;
	}
}
