using System;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Data.Storage.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.PublicFolder;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderSynchronizer : DisposeTrackableBase
	{
		private PublicFolderSynchronizer(PublicFolderSynchronizerContext syncContext, IActivityScope activityScope, Action<Exception> onEndCallback)
		{
			this.syncContext = syncContext;
			this.activityScope = activityScope;
			this.onEndCallback = onEndCallback;
			bool isHierarchyReady = syncContext.IsHierarchyReady;
			this.transientExceptionHandler = new TransientExceptionHandler(PublicFolderSynchronizer.Tracer, isHierarchyReady ? 5 : PublicFolderSynchronizer.FibonacciSequence.Length, new FilterDelegate(null, (UIntPtr)ldftn(IsTransientException)), this.syncContext.CorrelationId, new Action<Exception>(this.ConnectionFailedHandler));
			this.maximumAllowedDelay = (isHierarchyReady ? TimeSpan.FromMinutes(2.0) : TimeSpan.FromMinutes(10.0));
			this.resourcesToAccess = new ResourceKey[]
			{
				ProcessorResourceKey.Local,
				new MdbReplicationResourceHealthMonitorKey(this.syncContext.ContentMailboxGuid),
				new MdbResourceHealthMonitorKey(this.syncContext.ContentMailboxGuid)
			};
		}

		public static void Begin(OrganizationId organizationId, Guid contentMailboxGuid, bool executeReconcileFolders, Action<Exception> onEndCallback)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ArgumentValidator.ThrowIfEmpty("contentMailboxGuid", contentMailboxGuid);
			ArgumentValidator.ThrowIfNull("onEndCallback", onEndCallback);
			PublicFolderSynchronizer.Tracer.TraceDebug<OrganizationId, Guid, bool>(0L, "PublicFolderSynchronizer.Begin() for organization {0}, mailbox {1}, executeReconcileFolders {2}", organizationId, contentMailboxGuid, executeReconcileFolders);
			ConfigBase<MRSConfigSchema>.InitializeConfigProvider(new Func<IConfigSchema, IConfigProvider>(ConfigProvider.CreateProvider));
			IActivityScope activityScope = ActivityContext.Start(null, ActivityType.System);
			activityScope.Component = "PublicFolderSynchronizer";
			try
			{
				PublicFolderSynchronizerContext publicFolderSynchronizerContext = new PublicFolderSynchronizerContext(organizationId, contentMailboxGuid, false, executeReconcileFolders, activityScope.ActivityId);
				PublicFolderSynchronizer publicFolderSynchronizer = new PublicFolderSynchronizer(publicFolderSynchronizerContext, activityScope, onEndCallback);
				PublicFolderSynchronizer.Tracer.TraceDebug<int>(0L, "Created PublicFolderSynchronizer={0}", publicFolderSynchronizer.GetHashCode());
				publicFolderSynchronizer.Execute();
			}
			finally
			{
				ActivityContext.ClearThreadScope();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderSynchronizer>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				PublicFolderSynchronizer.Tracer.TraceDebug((long)this.GetHashCode(), "Disposing");
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						if (this.timer != null)
						{
							this.timer.Dispose();
							this.timer = null;
						}
						if (this.syncContext != null)
						{
							this.syncContext.Dispose();
							this.syncContext = null;
						}
						if (this.budget != null)
						{
							this.budget.Dispose();
							this.budget = null;
						}
						if (this.activityScope != null)
						{
							this.activityScope.End();
							this.activityScope = null;
						}
					});
				}
				catch (GrayException ex)
				{
					PublicFolderSynchronizer.Tracer.TraceError<Exception>((long)this.GetHashCode(), "InternalDispose failed exception: {0}", ex.InnerException);
				}
				PublicFolderSynchronizer.Tracer.TraceDebug((long)this.GetHashCode(), "Disposed");
			}
		}

		private static Exception MapToReportableException(Exception exception)
		{
			if (exception is PublicFolderSyncTransientException || exception is PublicFolderSyncPermanentException)
			{
				return exception;
			}
			LocalizedString message = ServerStrings.PublicFolderSyncFolderHierarchyFailed(CommonUtils.FullExceptionMessage(exception, true));
			if (CommonUtils.IsTransientException(exception))
			{
				return new PublicFolderSyncTransientException(message);
			}
			return new PublicFolderSyncPermanentException(message);
		}

		private void Execute()
		{
			PublicFolderSynchronizer.Tracer.TraceDebug((long)this.GetHashCode(), "Execute() starting");
			Exception exception = null;
			bool complete = false;
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					CommonUtils.CatchKnownExceptions(delegate
					{
						complete = this.ExecuteWithinBudget();
					}, delegate(Exception e)
					{
						PublicFolderSynchronizer.Tracer.TraceError<Exception>((long)this.GetHashCode(), "ExecuteWithinBudget call failed with known exception: {0}", e);
						exception = e;
					});
				});
			}
			catch (GrayException ex)
			{
				if (ex.InnerException != null)
				{
					exception = ex.InnerException;
				}
				else
				{
					exception = ex;
				}
				PublicFolderSynchronizer.Tracer.TraceError<GrayException>((long)this.GetHashCode(), "ExecuteWithinBudget call failed with unknown exception: {0}", ex);
			}
			if (exception != null || complete)
			{
				if (exception != null)
				{
					exception = PublicFolderSynchronizer.MapToReportableException(exception);
				}
				this.ReportSynchronizerResult(exception, complete);
				this.ExecuteCallback(exception);
				this.Dispose();
			}
		}

		private void ReportSynchronizerResult(Exception exception, bool complete)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						if (exception != null)
						{
							this.syncContext.Logger.LogEvent(LogEventType.Error, "Failed: " + exception);
							if (this.syncExecutor != null)
							{
								this.syncExecutor.HandleException(exception);
							}
						}
						else if (complete)
						{
							this.syncContext.Logger.LogEvent(LogEventType.Verbose, "Completed");
						}
					}
					catch (StorageTransientException arg2)
					{
						PublicFolderSynchronizer.Tracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "Writing batch result to the mailbox log failed with exception: {0}", arg2);
						PublicFolderSynchronizerLogger.LogOnServer(string.Format("Exception while writing batch result to log. Exception: {0}", arg2), LogEventType.Warning, new Guid?(this.syncContext.CorrelationId));
					}
					catch (StoragePermanentException arg3)
					{
						PublicFolderSynchronizer.Tracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "Writing batch result to the mailbox log failed with exception: {0}", arg3);
						PublicFolderSynchronizerLogger.LogOnServer(string.Format("Exception while writing batch result to log. Exception: {0}", arg3), LogEventType.Warning, new Guid?(this.syncContext.CorrelationId));
					}
				});
			}
			catch (GrayException arg)
			{
				PublicFolderSynchronizer.Tracer.TraceError<GrayException>((long)this.GetHashCode(), "Unable to handle completion of the : {0}", arg);
			}
		}

		private void ExecuteCallback(Exception exception)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					PublicFolderSynchronizer.Tracer.TraceDebug((long)this.GetHashCode(), "Executing callback");
					this.onEndCallback(exception);
					PublicFolderSynchronizer.Tracer.TraceDebug((long)this.GetHashCode(), "Callback executed");
				});
			}
			catch (GrayException ex)
			{
				PublicFolderSynchronizer.Tracer.TraceError<Exception>((long)this.GetHashCode(), "Execcution of the callback failed with exception: {0}", ex.InnerException);
			}
		}

		private bool ExecuteWithinBudget()
		{
			if (this.budget == null)
			{
				PublicFolderSynchronizer.Tracer.TraceDebug((long)this.GetHashCode(), "Acquiring budget");
				this.budget = StandardBudget.Acquire(PublicFolderSynchronizer.BudgetKey);
			}
			else
			{
				PublicFolderSynchronizer.Tracer.TraceDebug((long)this.GetHashCode(), "Resetting budget");
				this.budget.ResetWorkAccomplished();
			}
			TimeSpan timeSpan;
			for (;;)
			{
				PublicFolderSynchronizer.<>c__DisplayClass11 CS$<>8__locals1 = new PublicFolderSynchronizer.<>c__DisplayClass11();
				CS$<>8__locals1.<>4__this = this;
				timeSpan = TimeSpan.Zero;
				CS$<>8__locals1.moreBatchesToProcess = false;
				bool flag = this.transientExceptionHandler.TryExecute(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ExecuteWithinBudget>b__10)));
				if (flag)
				{
					PublicFolderSynchronizer.Tracer.TraceDebug((long)this.GetHashCode(), "Batch executed successfully");
					if (!CS$<>8__locals1.moreBatchesToProcess)
					{
						break;
					}
				}
				else
				{
					PublicFolderSynchronizer.Tracer.TraceDebug((long)this.GetHashCode(), "Batch failed execution. Calculating Delays.");
					int num = Math.Min(this.transientExceptionHandler.TransientExceptionCount, PublicFolderSynchronizer.FibonacciSequence.Length) - 1;
					timeSpan = TimeSpan.FromMinutes((double)PublicFolderSynchronizer.FibonacciSequence[num]);
				}
				TimeSpan resourceMonitorDelay = this.GetResourceMonitorDelay();
				if (resourceMonitorDelay > timeSpan)
				{
					timeSpan = resourceMonitorDelay;
				}
				if (timeSpan > TimeSpan.Zero)
				{
					goto Block_5;
				}
			}
			PublicFolderSynchronizer.Tracer.TraceDebug((long)this.GetHashCode(), "No more batches to process, exiting");
			return true;
			Block_5:
			PublicFolderSynchronizerLogger.LogOnServer("Delay=" + timeSpan.TotalMilliseconds, LogEventType.Verbose, new Guid?(this.syncContext.CorrelationId));
			if (this.timer == null)
			{
				this.timer = new Timer(new TimerCallback(this.TimerExecute));
			}
			PublicFolderSynchronizer.Tracer.TraceDebug<TimeSpan>((long)this.GetHashCode(), "Setting timer to wake up in: {0}", timeSpan);
			this.timer.Change(timeSpan, TimeSpan.Zero);
			return false;
		}

		private TimeSpan GetResourceMonitorDelay()
		{
			DelayInfo delay = ResourceLoadDelayInfo.GetDelay(this.budget, PublicFolderSynchronizer.WorkloadSettings, this.resourcesToAccess, true);
			if (delay != null)
			{
				PublicFolderSynchronizer.Tracer.TraceDebug<TimeSpan>((long)this.GetHashCode(), "Resource load provided delay info: {0}", delay.Delay);
				if (delay.Delay > this.maximumAllowedDelay)
				{
					string text = string.Format("Delay suggested by ResourceMonitor of {0} ms has exceeded the maximum allowed delay of {1} ms", delay.Delay.TotalMilliseconds, this.maximumAllowedDelay.TotalMilliseconds);
					if (this.syncContext.IsHierarchyReady)
					{
						throw new StorageTransientException(new LocalizedString(text));
					}
					PublicFolderSynchronizerLogger.LogOnServer(text, LogEventType.Warning, new Guid?(this.syncContext.CorrelationId));
					return this.maximumAllowedDelay;
				}
			}
			if (delay == null)
			{
				return TimeSpan.Zero;
			}
			return delay.Delay;
		}

		private void TimerExecute(object stateNotUsed)
		{
			PublicFolderSynchronizer.Tracer.TraceDebug((long)this.GetHashCode(), "Waking up from timer");
			ActivityContext.SetThreadScope(this.activityScope);
			try
			{
				this.Execute();
			}
			finally
			{
				ActivityContext.ClearThreadScope();
			}
		}

		private void ConnectionFailedHandler(Exception exception)
		{
			if (exception != null && TransientExceptionHandler.IsConnectionFailure(exception))
			{
				if (exception is IMRSRemoteException || exception is CommunicationWithRemoteServiceFailedTransientException)
				{
					this.syncContext.ResetSourceMailboxConnection();
					this.syncExecutor = null;
					return;
				}
				this.syncContext.ResetDestinationMailboxConnection();
			}
		}

		private const int MaximumNumberOfTransientExceptionRetiresForMailboxWithHierarchyReady = 5;

		public static readonly string BudgetCallerInfo = "PublicFolderSynchronizer.Execute";

		private static readonly Trace Tracer = ExTraceGlobals.PublicFolderSynchronizerTracer;

		private static readonly WorkloadSettings WorkloadSettings = new WorkloadSettings(WorkloadType.PublicFolderMailboxSync, true);

		private static readonly int[] FibonacciSequence = new int[]
		{
			1,
			1,
			2,
			3,
			5,
			8,
			13,
			21
		};

		private static readonly BudgetKey BudgetKey = new UnthrottledBudgetKey("PublicFolderSync", BudgetType.ResourceTracking);

		private readonly TimeSpan maximumAllowedDelay;

		private readonly Action<Exception> onEndCallback;

		private readonly ResourceKey[] resourcesToAccess;

		private readonly TransientExceptionHandler transientExceptionHandler;

		private PublicFolderHierarchySyncExecutor syncExecutor;

		private Timer timer;

		private PublicFolderSynchronizerContext syncContext;

		private IBudget budget;

		private IActivityScope activityScope;
	}
}
