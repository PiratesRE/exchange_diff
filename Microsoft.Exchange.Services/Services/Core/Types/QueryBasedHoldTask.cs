using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class QueryBasedHoldTask : ITask, IDisposeTrackable, IDisposable
	{
		internal QueryBasedHoldTask(CallContext callContext, MailboxDiscoverySearch discoverySearch, DiscoverySearchDataProvider discoverySearchDataProvider, HoldAction actionType, IRecipientSession recipientSession)
		{
			this.CallContext = callContext;
			this.WorkloadSettings = new WorkloadSettings(WorkloadType.Ews, false);
			this.budget = EwsBudget.Acquire(callContext.Budget.Owner);
			this.DiscoverySearch = discoverySearch;
			this.DiscoverySearchDataProvider = discoverySearchDataProvider;
			this.ActionType = actionType;
			this.disposeTracker = this.GetDisposeTracker();
			if (!recipientSession.ReadOnly)
			{
				this.RecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, recipientSession.SessionSettings, 74, ".ctor", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\Types\\WorkloadManager\\QueryBasedHoldTask.cs");
				return;
			}
			this.RecipientSession = recipientSession;
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			this.Dispose(true);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<QueryBasedHoldTask>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal CallContext CallContext { get; private set; }

		internal MailboxDiscoverySearch DiscoverySearch { get; private set; }

		internal DiscoverySearchDataProvider DiscoverySearchDataProvider { get; private set; }

		internal HoldAction ActionType { get; private set; }

		internal IRecipientSession RecipientSession { get; private set; }

		public object State { get; set; }

		public string Description { get; set; }

		public void Cancel()
		{
			ExTraceGlobals.ThrottlingTracer.TraceDebug<string>((long)this.GetHashCode(), "[QueryBasedHoldTask.Cancel] Cancel called for task {0}", this.Description);
			this.Dispose();
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
			this.SendWatsonReportOnGrayException(delegate
			{
				this.DiscoverySearch.SynchronizeHoldSettings(this.DiscoverySearchDataProvider, this.RecipientSession, true);
				if (this.DiscoverySearch.Sources == null || this.DiscoverySearch.Sources.Count == 0)
				{
					this.DiscoverySearchDataProvider.Delete(this.DiscoverySearch);
				}
				result = TaskExecuteResult.ProcessingComplete;
			});
			return result;
		}

		public WorkloadSettings WorkloadSettings { get; private set; }

		public void Complete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			ExTraceGlobals.ThrottlingTracer.TraceDebug<string, TimeSpan, TimeSpan>((long)this.GetHashCode(), "[QueryBasedHoldTask.Complete] Complete with no exception called for task {0}.  Delay: {1}, Elapsed: {2}", this.Description, queueAndDelayTime, totalTime);
			this.Dispose();
		}

		public IBudget Budget
		{
			get
			{
				return this.budget;
			}
		}

		public TimeSpan MaxExecutionTime
		{
			get
			{
				return QueryBasedHoldTask.DefaultMaxExecutionTime;
			}
		}

		public void Timeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			ExTraceGlobals.ThrottlingTracer.TraceDebug<string, TimeSpan, TimeSpan>((long)this.GetHashCode(), "[QueryBasedHoldTask.Timeout] Timeout called for task {0}.  Delay: {1}, Elapsed: {2}", this.Description, queueAndDelayTime, totalTime);
			this.Dispose();
		}

		public TaskExecuteResult CancelStep(LocalizedException exception)
		{
			ExTraceGlobals.ThrottlingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[QueryBasedHoldTask.CancelStep] Current execution step for task {0} is cancelled with exception: {1}", this.Description, exception.ToString());
			return TaskExecuteResult.ProcessingComplete;
		}

		public ResourceKey[] GetResources()
		{
			return null;
		}

		private static bool GrayExceptionFilter(object exception)
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

		private void SendWatsonReportOnGrayException(QueryBasedHoldTask.GrayExceptionCallback callback)
		{
			Exception ex = null;
			string formatString = null;
			ServiceDiagnostics.RegisterAdditionalWatsonData();
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					callback();
				}, new GrayException.ExceptionFilterDelegate(QueryBasedHoldTask.GrayExceptionFilter));
			}
			catch (GrayException ex2)
			{
				ex = ex2;
				formatString = "Task {0} failed: {1}";
				if (this.Budget != null)
				{
					UserWorkloadManager.GetPerfCounterWrapper(this.Budget.Owner.BudgetType).UpdateTotalTaskExecutionFailuresCount();
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

		private void Dispose(bool suppressFinalize)
		{
			if (!this.disposed)
			{
				if (suppressFinalize)
				{
					GC.SuppressFinalize(this);
				}
				if (this.budget != null)
				{
					this.budget.LogEndStateToIIS();
					this.budget.Dispose();
					this.budget = null;
				}
				this.disposed = true;
			}
		}

		private static readonly TimeSpan DefaultMaxExecutionTime = TimeSpan.FromMinutes(1.0);

		private bool disposed;

		private IEwsBudget budget;

		private DisposeTracker disposeTracker;

		internal delegate void GrayExceptionCallback();
	}
}
