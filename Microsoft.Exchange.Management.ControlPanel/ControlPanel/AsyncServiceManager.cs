using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.DDIService;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AsyncServiceManager
	{
		public static PowerShellResults InvokeAsync(Func<PowerShellResults> callback, Action<PowerShellResults> onCompleted, string uniqueUserIdentity, AsyncTaskType taskType, string commandStringForTrace)
		{
			AsyncServiceManager.AsyncTaskBudgetManager asyncTaskBudget;
			ThrottlingType throttlingType;
			switch (taskType)
			{
			case AsyncTaskType.AsyncGetList:
				asyncTaskBudget = AsyncServiceManager.getListAsyncTaskBudget;
				throttlingType = ThrottlingType.Default;
				break;
			case AsyncTaskType.AsyncGetListPreLoad:
				asyncTaskBudget = AsyncServiceManager.getListAsyncTaskBudget;
				throttlingType = ThrottlingType.PerUser;
				break;
			default:
				asyncTaskBudget = AsyncServiceManager.defaultAsyncTaskBudget;
				throttlingType = ThrottlingType.Default;
				break;
			}
			return AsyncServiceManager.InvokeAsyncCore(callback, onCompleted, uniqueUserIdentity, asyncTaskBudget, commandStringForTrace, throttlingType);
		}

		private static PowerShellResults InvokeAsyncCore(Func<PowerShellResults> callback, Action<PowerShellResults> onCompleted, string uniqueUserIdentity, AsyncServiceManager.AsyncTaskBudgetManager asyncTaskBudget, string commandStringForTrace, ThrottlingType throttlingType)
		{
			if (string.IsNullOrEmpty(uniqueUserIdentity))
			{
				throw new ArgumentNullException("uniqueUserIdentity cannot be null.");
			}
			AsyncServiceManager.AsyncTaskThrottlingStatus asyncTaskThrottlingStatus = asyncTaskBudget.RegisterAsyncTask(uniqueUserIdentity, commandStringForTrace, throttlingType);
			if (asyncTaskThrottlingStatus != AsyncServiceManager.AsyncTaskThrottlingStatus.None)
			{
				LocalizedString value = (asyncTaskThrottlingStatus == AsyncServiceManager.AsyncTaskThrottlingStatus.PerAppThrottlingHit) ? Strings.LongRunPerAppThrottlingHit : Strings.LongRunPerUserThrottlingHit;
				asyncTaskBudget.UnregisterAsyncTask(uniqueUserIdentity, throttlingType);
				return new PowerShellResults
				{
					ErrorRecords = new ErrorRecord[]
					{
						new ErrorRecord(new InvalidOperationException(value))
					}
				};
			}
			AsyncServiceManager.WorkItem workItem = new AsyncServiceManager.WorkItem(Guid.NewGuid().ToString());
			AsyncServiceManager.workItems[workItem.Id] = workItem;
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			IPrincipal currentPrincipal = Thread.CurrentPrincipal;
			OperationContext currentOperationContext = OperationContext.Current;
			HttpContext currentHttpContext = HttpContext.Current;
			commandStringForTrace = ((commandStringForTrace == null) ? string.Empty : commandStringForTrace);
			RbacPrincipal rbacSession = RbacPrincipal.GetCurrent(false);
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				int managedThreadId = Thread.CurrentThread.ManagedThreadId;
				AsyncServiceManager.WorkItem workItem;
				AsyncServiceManager.workerThreads[managedThreadId] = workItem;
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				IPrincipal currentPrincipal = Thread.CurrentPrincipal;
				OperationContext value2 = OperationContext.Current;
				HttpContext value3 = HttpContext.Current;
				Thread.CurrentThread.CurrentCulture = (Thread.CurrentThread.CurrentUICulture = currentCulture);
				Thread.CurrentPrincipal = currentPrincipal;
				OperationContext.Current = currentOperationContext;
				HttpContext.Current = AsyncServiceManager.CloneHttpContextForLongRunningThread(currentHttpContext);
				ActivityContextManager.InitializeActivityContext(HttpContext.Current, ActivityContextLoggerId.LongRunning);
				PowerShellResults powerShellResults = null;
				try
				{
					EcpEventLogConstants.Tuple_AsyncWebRequestStarted.LogEvent(new object[]
					{
						uniqueUserIdentity,
						managedThreadId,
						commandStringForTrace
					});
					powerShellResults = callback();
					object obj = AsyncServiceManager.workerThreads[managedThreadId];
				}
				catch (Exception exception)
				{
					powerShellResults = new PowerShellResults();
					powerShellResults.ErrorRecords = new ErrorRecord[]
					{
						new ErrorRecord(exception)
					};
					EcpEventLogConstants.Tuple_AsyncWebRequestFailed.LogEvent(new object[]
					{
						uniqueUserIdentity,
						managedThreadId,
						exception.GetTraceFormatter(),
						commandStringForTrace
					});
					ErrorHandlingUtil.SendReportForCriticalException(currentHttpContext, exception);
					DDIHelper.Trace("Async work item {0}, Error: {1}", new object[]
					{
						workItem.Id,
						exception.GetTraceFormatter()
					});
				}
				finally
				{
					AsyncServiceManager.workerThreads.Remove(managedThreadId);
					lock (workItem)
					{
						workItem.Results = powerShellResults;
						ProgressRecord progressRecord = workItem.LegacyProgressRecord ?? ((workItem.ProgressCalculator == null) ? new ProgressRecord() : workItem.ProgressCalculator.ProgressRecord);
						powerShellResults.ProgressRecord = progressRecord;
						progressRecord.HasCompleted = true;
						progressRecord.IsCancelled = workItem.Cancelled;
						workItem.FinishedEvent.Set();
					}
					asyncTaskBudget.UnregisterAsyncTask(uniqueUserIdentity, throttlingType);
					if (onCompleted != null)
					{
						onCompleted(powerShellResults);
					}
					EcpEventLogConstants.Tuple_AsyncWebRequestEnded.LogEvent(new object[]
					{
						uniqueUserIdentity,
						managedThreadId,
						commandStringForTrace
					});
					ActivityContextManager.CleanupActivityContext(HttpContext.Current);
					Thread.CurrentThread.CurrentCulture = (Thread.CurrentThread.CurrentUICulture = currentCulture);
					Thread.CurrentPrincipal = currentPrincipal;
					OperationContext.Current = value2;
					HttpContext.Current = value3;
					GC.KeepAlive(rbacSession);
				}
			});
			return new PowerShellResults
			{
				ProgressId = workItem.Id
			};
		}

		public static bool IsCurrentWorkCancelled()
		{
			return AsyncServiceManager.CheckCurrentWorkItemThreadSafe((AsyncServiceManager.WorkItem x) => x.Cancelled) ?? false;
		}

		public static bool IsCurrentWorkBulkEdit()
		{
			return AsyncServiceManager.CheckCurrentWorkItemThreadSafe((AsyncServiceManager.WorkItem x) => x.ProgressCalculator is BulkEditProgressCalculator || x.ProgressCalculator is MaximumCountProgressCalculator) ?? false;
		}

		private static bool? CheckCurrentWorkItemThreadSafe(Predicate<AsyncServiceManager.WorkItem> condition)
		{
			AsyncServiceManager.WorkItem currentThreadWorkItem = AsyncServiceManager.GetCurrentThreadWorkItem();
			bool? result = null;
			if (currentThreadWorkItem != null)
			{
				lock (currentThreadWorkItem)
				{
					result = new bool?(condition(currentThreadWorkItem));
				}
			}
			return result;
		}

		private static AsyncServiceManager.WorkItem GetCurrentThreadWorkItem()
		{
			return AsyncServiceManager.workerThreads[Thread.CurrentThread.ManagedThreadId] as AsyncServiceManager.WorkItem;
		}

		public static PowerShellResults<JsonDictionary<object>> GetProgress(string progressId)
		{
			AsyncServiceManager.WorkItem workItem = null;
			return AsyncServiceManager.GetProgressImpl<JsonDictionary<object>>(progressId, out workItem);
		}

		private static PowerShellResults<T> GetProgressImpl<T>(string progressId, out AsyncServiceManager.WorkItem workItem)
		{
			PowerShellResults<T> powerShellResults = new PowerShellResults<T>();
			workItem = null;
			if (!string.IsNullOrEmpty(progressId) && AsyncServiceManager.workItems.TryGetValue(progressId, out workItem))
			{
				lock (workItem)
				{
					if (workItem.Results != null)
					{
						PowerShellResults<T> powerShellResults2 = workItem.Results as PowerShellResults<T>;
						if (powerShellResults2 != null)
						{
							powerShellResults = powerShellResults2;
						}
						else
						{
							powerShellResults.MergeErrors(workItem.Results);
						}
						AsyncServiceManager.workItems.Remove(progressId);
						workItem.FinishedEvent.Close();
					}
					else
					{
						ProgressRecord progressRecord = null;
						if (workItem.LegacyProgressRecord != null)
						{
							progressRecord = workItem.LegacyProgressRecord;
						}
						else
						{
							ProgressRecord progressRecord2 = (workItem.ProgressCalculator == null) ? new ProgressRecord() : workItem.ProgressCalculator.ProgressRecord;
							lock (progressRecord2.SyncRoot)
							{
								progressRecord = new ProgressRecord
								{
									Errors = progressRecord2.Errors,
									FailedCount = progressRecord2.FailedCount,
									MaxCount = progressRecord2.MaxCount,
									Percent = progressRecord2.Percent,
									Status = progressRecord2.Status,
									SucceededCount = progressRecord2.SucceededCount,
									HasCompleted = progressRecord2.HasCompleted,
									IsCancelled = progressRecord2.IsCancelled
								};
								progressRecord2.Errors = null;
							}
						}
						powerShellResults.ProgressRecord = progressRecord;
					}
				}
			}
			DDIHelper.Trace("GetProgress: {0}, Results: {1}", new object[]
			{
				progressId,
				powerShellResults
			});
			return powerShellResults;
		}

		public static PowerShellResults<PSObject> GetPreLoadResult(string progressId)
		{
			AsyncServiceManager.WorkItem workItem = null;
			PowerShellResults<PSObject> powerShellResults = new PowerShellResults<PSObject>();
			if (AsyncServiceManager.workItems.TryGetValue(progressId, out workItem) && workItem != null)
			{
				workItem.FinishedEvent.WaitOne();
				lock (workItem)
				{
					if (workItem.Results != null)
					{
						if (workItem.AsyncGetListContext.PsObjectCollection != null)
						{
							powerShellResults.Output = workItem.AsyncGetListContext.PsObjectCollection.ToArray();
						}
						powerShellResults.MergeErrors(workItem.Results);
						AsyncServiceManager.workItems.Remove(progressId);
						workItem.FinishedEvent.Close();
					}
				}
			}
			return powerShellResults;
		}

		public static PowerShellResults<PSObject> GetCurrentResult(string progressId)
		{
			AsyncServiceManager.WorkItem workItem = null;
			PowerShellResults<PSObject> progressImpl = AsyncServiceManager.GetProgressImpl<PSObject>(progressId, out workItem);
			if (!string.IsNullOrEmpty(progressId) && workItem != null)
			{
				List<PSObject> list = null;
				AsyncGetListContext asyncGetListContext = workItem.AsyncGetListContext;
				List<string> unicodeOutputColumnNames = asyncGetListContext.UnicodeOutputColumnNames;
				bool flag = unicodeOutputColumnNames != null && unicodeOutputColumnNames.Count > 0;
				int num = 0;
				int num2 = 0;
				lock (workItem)
				{
					list = asyncGetListContext.PsObjectCollection;
					if (list != null)
					{
						num = asyncGetListContext.NextFetchStartAt;
						num2 = list.Count;
						asyncGetListContext.NextFetchStartAt = num2;
						if (flag && asyncGetListContext.UnicodeColumns == null)
						{
							asyncGetListContext.UnicodeColumns = new List<Tuple<int, string[], string>>(num2);
						}
						progressImpl.AsyncGetListContext = asyncGetListContext;
						if (progressImpl.ProgressRecord == null || progressImpl.ProgressRecord.HasCompleted)
						{
							asyncGetListContext.Completed = true;
							workItem.AsyncGetListContext = null;
						}
					}
				}
				progressImpl.StartIndex = num;
				progressImpl.EndIndex = num2;
				progressImpl.Output = new PSObject[num2 - num];
				if (list != null)
				{
					int i = num;
					int num3 = 0;
					while (i < num2)
					{
						progressImpl.Output[num3] = list[i];
						list[i] = null;
						i++;
						num3++;
					}
				}
			}
			return progressImpl;
		}

		public static PowerShellResults Cancel(string progressId)
		{
			AsyncServiceManager.WorkItem workItem = null;
			if (!string.IsNullOrEmpty(progressId) && AsyncServiceManager.workItems.TryGetValue(progressId, out workItem))
			{
				lock (workItem)
				{
					if (!workItem.Cancelled)
					{
						if (workItem.PowerShell != null)
						{
							try
							{
								workItem.PowerShell.BeginStop(new AsyncCallback(AsyncServiceManager.AsyncStop), workItem.PowerShell);
							}
							catch (ObjectDisposedException)
							{
							}
						}
						workItem.Cancelled = true;
					}
				}
			}
			return new PowerShellResults();
		}

		private static void AsyncStop(IAsyncResult asyncResult)
		{
			PowerShell powerShell = asyncResult.AsyncState as PowerShell;
			if (powerShell != null)
			{
				try
				{
					powerShell.EndStop(asyncResult);
				}
				catch (Exception ex)
				{
					EcpEventLogConstants.Tuple_AsyncWebRequestFailedInCancel.LogEvent(new object[]
					{
						ex.Message
					});
				}
			}
		}

		private static HttpContext CloneHttpContextForLongRunningThread(HttpContext context)
		{
			return new HttpContext(context.Request, context.Response)
			{
				User = context.User
			};
		}

		internal static bool TestCancel(string progressId)
		{
			bool result = false;
			AsyncServiceManager.WorkItem workItem = null;
			if (!string.IsNullOrEmpty(progressId) && AsyncServiceManager.workItems.TryGetValue(progressId, out workItem))
			{
				result = workItem.Cancelled;
			}
			return result;
		}

		public static void RegisterPowerShell(PowerShell powerShell)
		{
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			if (AsyncServiceManager.workerThreads.Contains(managedThreadId))
			{
				AsyncServiceManager.WorkItem workItem = (AsyncServiceManager.WorkItem)AsyncServiceManager.workerThreads[managedThreadId];
				workItem.LegacyProgressRecord = new ProgressRecord();
				powerShell.Streams.Progress.DataAdded += delegate(object sender, DataAddedEventArgs e)
				{
					AsyncServiceManager.OnProgress(workItem, ((PSDataCollection<ProgressRecord>)sender)[e.Index]);
				};
			}
		}

		public static void RegisterPowerShellToActivity(PowerShell powerShell, CmdletActivity activity, IEnumerable pipelineInput, out List<PSObject> psDataCollection, bool isGetListAsync)
		{
			psDataCollection = null;
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			if (AsyncServiceManager.workerThreads.Contains(managedThreadId))
			{
				AsyncServiceManager.WorkItem workItem = (AsyncServiceManager.WorkItem)AsyncServiceManager.workerThreads[managedThreadId];
				lock (workItem)
				{
					workItem.ProgressCalculator.SetPipelineInput(pipelineInput);
					workItem.PowerShell = powerShell;
					if (isGetListAsync && workItem.AsyncGetListContext.PsObjectCollection == null)
					{
						psDataCollection = new List<PSObject>(DDIHelper.GetListDefaultResultSize * 2);
						workItem.AsyncGetListContext.PsObjectCollection = psDataCollection;
					}
				}
				if (powerShell != null)
				{
					powerShell.Streams.Progress.DataAdded += delegate(object sender, DataAddedEventArgs e)
					{
						ProgressRecord progressRecord = ((PSDataCollection<ProgressRecord>)sender)[e.Index];
						if (isGetListAsync)
						{
							AsyncServiceManager.OnProgress(workItem, progressRecord);
							return;
						}
						List<ErrorRecord> list = new List<ErrorRecord>();
						if (AsyncServiceManager.IsCurrentWorkBulkEdit())
						{
							Collection<ErrorRecord> collection = powerShell.Streams.Error.ReadAll();
							foreach (ErrorRecord errorRecord in collection)
							{
								list.Add(new ErrorRecord(errorRecord));
							}
						}
						string status = (progressRecord.RecordType != ProgressRecordType.Completed) ? progressRecord.StatusDescription : string.Empty;
						ProgressReportEventArgs progressReportEventArgs = new ProgressReportEventArgs(list, progressRecord.PercentComplete, status);
						DDIHelper.Trace("Async Progress Id: {0},  ProgressReportEventArgs: {1}", new object[]
						{
							workItem.Id,
							progressReportEventArgs
						});
						activity.OnPSProgressReport(progressReportEventArgs);
					};
				}
			}
		}

		public static void RegisterWorkflow(Workflow workflow, AsyncGetListContext asyncGetListContext = null)
		{
			if (workflow == null)
			{
				throw new InvalidOperationException("Only BulkEditWorkflow is allowed to do async execution.");
			}
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			if (AsyncServiceManager.workerThreads.Contains(managedThreadId))
			{
				AsyncServiceManager.WorkItem workItem = (AsyncServiceManager.WorkItem)AsyncServiceManager.workerThreads[managedThreadId];
				workItem.ProgressCalculator = (ProgressCalculatorBase)Activator.CreateInstance(workflow.ProgressCalculator);
				workflow.ProgressCalculatorInstance = workItem.ProgressCalculator;
				workItem.AsyncGetListContext = asyncGetListContext;
				workflow.ProgressChanged += delegate(object sender, ProgressReportEventArgs e)
				{
					workItem.ProgressCalculator.CalculateProgress(e);
				};
			}
		}

		public static AsyncGetListContext GetRegisteredContext(string progressId)
		{
			AsyncGetListContext result = null;
			AsyncServiceManager.WorkItem workItem = null;
			if (!string.IsNullOrEmpty(progressId) && AsyncServiceManager.workItems.TryGetValue(progressId, out workItem))
			{
				result = workItem.AsyncGetListContext;
			}
			return result;
		}

		private static void OnProgress(AsyncServiceManager.WorkItem workItem, ProgressRecord pspRecord)
		{
			workItem.LegacyProgressRecord = new ProgressRecord(pspRecord);
		}

		private static readonly Hashtable workerThreads = Hashtable.Synchronized(new Hashtable());

		private static readonly int asyncWorkItemExpirationTimeInMinutes = ConfigUtil.ReadInt("AsyncWorkItemExpirationTimeInMinutes", 2);

		private static readonly MruDictionaryCache<string, AsyncServiceManager.WorkItem> workItems = new MruDictionaryCache<string, AsyncServiceManager.WorkItem>(10, int.MaxValue, AsyncServiceManager.asyncWorkItemExpirationTimeInMinutes);

		private static readonly AsyncServiceManager.AsyncTaskBudgetManager defaultAsyncTaskBudget = new AsyncServiceManager.AsyncTaskBudgetManager(ConfigUtil.ReadInt("PerServerMaxConcurrentAsyncTaskCount", 5), ConfigUtil.ReadInt("PerUserMaxConcurrentAsyncTaskCount", 2));

		private static readonly AsyncServiceManager.AsyncTaskBudgetManager getListAsyncTaskBudget = new AsyncServiceManager.AsyncTaskBudgetManager(ConfigUtil.ReadInt("PerServerMaxConcurrentAsyncGetListTaskCount", 8), ConfigUtil.ReadInt("PerUserMaxConcurrentAsyncGetListTaskCount", 2));

		private class WorkItem
		{
			public WorkItem(string id)
			{
				this.Id = id;
			}

			public readonly string Id;

			public ProgressRecord LegacyProgressRecord;

			public ProgressCalculatorBase ProgressCalculator;

			public PowerShellResults Results;

			public PowerShell PowerShell;

			public bool Cancelled;

			public AsyncGetListContext AsyncGetListContext;

			public ManualResetEvent FinishedEvent = new ManualResetEvent(false);
		}

		private enum AsyncTaskThrottlingStatus
		{
			None,
			PerAppThrottlingHit,
			PerUserThrottlingHit
		}

		private class AsyncTaskBudgetManager
		{
			public AsyncTaskBudgetManager(int perServerQuota, int perUserQuota)
			{
				this._perServerMaxConcurrentAsyncTaskCount = perServerQuota;
				this._perUserMaxConcurrentAsyncTaskCount = perUserQuota;
			}

			public AsyncServiceManager.AsyncTaskThrottlingStatus RegisterAsyncTask(string uniqueUserIdentity, string commandStringForTrace, ThrottlingType throttlingType)
			{
				AsyncServiceManager.AsyncTaskThrottlingStatus asyncTaskThrottlingStatus = AsyncServiceManager.AsyncTaskThrottlingStatus.None;
				lock (this.dictionarySynchronizationObject)
				{
					if ((throttlingType & ThrottlingType.PerUser) != (ThrottlingType)0)
					{
						int num = 0;
						if (this._perUserBudget.TryGetValue(uniqueUserIdentity, out num) && num >= this._perUserMaxConcurrentAsyncTaskCount)
						{
							asyncTaskThrottlingStatus = AsyncServiceManager.AsyncTaskThrottlingStatus.PerUserThrottlingHit;
						}
						this._perUserBudget[uniqueUserIdentity] = num + 1;
					}
					if ((throttlingType & ThrottlingType.PerServer) != (ThrottlingType)0)
					{
						if (asyncTaskThrottlingStatus == AsyncServiceManager.AsyncTaskThrottlingStatus.None && this._currentConcurrentTaskCount >= this._perServerMaxConcurrentAsyncTaskCount)
						{
							asyncTaskThrottlingStatus = AsyncServiceManager.AsyncTaskThrottlingStatus.PerAppThrottlingHit;
						}
						this._currentConcurrentTaskCount++;
					}
				}
				if (asyncTaskThrottlingStatus == AsyncServiceManager.AsyncTaskThrottlingStatus.PerAppThrottlingHit)
				{
					EcpEventLogConstants.Tuple_TooManyAsyncTaskInServer.LogEvent(new object[]
					{
						this._currentConcurrentTaskCount,
						this._perServerMaxConcurrentAsyncTaskCount,
						commandStringForTrace
					});
				}
				else if (asyncTaskThrottlingStatus == AsyncServiceManager.AsyncTaskThrottlingStatus.PerUserThrottlingHit)
				{
					EcpEventLogConstants.Tuple_TooManyAsyncTaskFromCurrentUser.LogEvent(new object[]
					{
						this._perUserBudget[uniqueUserIdentity],
						this._perUserMaxConcurrentAsyncTaskCount,
						commandStringForTrace
					});
				}
				return asyncTaskThrottlingStatus;
			}

			public void UnregisterAsyncTask(string uniqueUserIdentity, ThrottlingType throttlingType)
			{
				lock (this.dictionarySynchronizationObject)
				{
					if ((throttlingType & ThrottlingType.PerServer) != (ThrottlingType)0)
					{
						this._currentConcurrentTaskCount--;
					}
					Dictionary<string, int> perUserBudget;
					if ((throttlingType & ThrottlingType.PerUser) != (ThrottlingType)0 && ((perUserBudget = this._perUserBudget)[uniqueUserIdentity] = perUserBudget[uniqueUserIdentity] - 1) == 0)
					{
						this._perUserBudget.Remove(uniqueUserIdentity);
					}
				}
			}

			private readonly int _perServerMaxConcurrentAsyncTaskCount;

			private readonly int _perUserMaxConcurrentAsyncTaskCount;

			private Dictionary<string, int> _perUserBudget = new Dictionary<string, int>();

			private int _currentConcurrentTaskCount;

			private readonly object dictionarySynchronizationObject = new object();
		}
	}
}
