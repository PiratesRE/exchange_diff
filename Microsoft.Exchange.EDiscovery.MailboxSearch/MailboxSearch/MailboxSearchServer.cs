using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.EDiscovery.Export;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.InfoWorker.Common.SearchService;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.MailboxSearch;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class MailboxSearchServer : MailboxSearchRpcServer
	{
		private bool IsShutDown
		{
			get
			{
				return this.shutdownEvent.WaitOne(0, false);
			}
		}

		public override SearchErrorInfo Start(SearchId searchId, Guid ownerGuid)
		{
			return this.StartEx(searchId, ownerGuid.ToString());
		}

		public override SearchErrorInfo StartEx(SearchId searchId, string ownerId)
		{
			MailboxSearchServer.Tracer.TraceFunction<string, string>((long)this.GetHashCode(), "MailboxSearchServer.Start {0}, {1}", searchId.SearchName, ownerId);
			SearchErrorInfo errorInfo = null;
			SearchUtils.ExWatsonWrappedCall(delegate()
			{
				lock (this)
				{
					if (this.IsShutDown)
					{
						errorInfo = new SearchErrorInfo(-2147220983, Strings.SearchServerShutdown);
					}
					else
					{
						bool flag2 = false;
						bool flag3 = this.searchWorkItemMap.ContainsKey(searchId);
						bool flag4 = this.pendingSearchIdMap.ContainsKey(searchId);
						if (flag3 || flag4)
						{
							bool flag5 = false;
							if (flag3)
							{
								MailboxSearchWorkItem mailboxSearchWorkItem = this.searchWorkItemMap[searchId];
								if (mailboxSearchWorkItem.Action != WorkItemAction.Remove)
								{
									flag5 = true;
								}
							}
							if (flag4)
							{
								flag5 = true;
							}
							if (flag5)
							{
								MailboxSearchServer.Tracer.TraceWarning<string>((long)this.GetHashCode(), "Search {0} is already started", searchId.SearchName);
								errorInfo = new SearchErrorInfo(262657, Strings.SearchAlreadStarted);
							}
							else
							{
								flag2 = true;
							}
						}
						if (errorInfo == null)
						{
							if (flag2)
							{
								this.pendingSearchIdMap.Add(searchId, ownerId);
								SearchEventLogger.Instance.LogDiscoverySearchPendingWorkItemsChangedEvent("AddedToPendingWorkItems", searchId.SearchName, searchId.MailboxDsName, this.pendingSearchIdMap.Count);
							}
							else
							{
								errorInfo = this.AddSearchToWorkItemQueue(searchId, ownerId);
							}
						}
					}
				}
			});
			if (errorInfo != null && errorInfo.Failed)
			{
				MailboxSearchServer.LogErrorInfo("Error occured when trying to start the search by adding it to queue", searchId, errorInfo);
			}
			return errorInfo;
		}

		public override SearchErrorInfo GetStatus(SearchId searchId, out SearchStatus searchStatus)
		{
			MailboxSearchServer.Tracer.TraceFunction<string>((long)this.GetHashCode(), "MailboxSearchServer.GetStatus {0}", searchId.SearchName);
			SearchErrorInfo errorInfo = null;
			searchStatus = null;
			SearchStatus theSearchStatus = null;
			SearchUtils.ExWatsonWrappedCall(delegate()
			{
				MailboxSearchWorkItem mailboxSearchWorkItem = null;
				if (!this.searchWorkItemMap.TryGetValue(searchId, out mailboxSearchWorkItem))
				{
					MailboxSearchServer.Tracer.TraceWarning<string>((long)this.GetHashCode(), "The search {0} is not started", searchId.SearchName);
					errorInfo = new SearchErrorInfo(262658, Strings.SearchNotStarted);
					return;
				}
				try
				{
					theSearchStatus = mailboxSearchWorkItem.GetStatus();
				}
				catch (ExportException ex)
				{
					MailboxSearchServer.Tracer.TraceError<ExportException>((long)this.GetHashCode(), "MailboxSearchServer.GetStatus error {0}", ex);
					errorInfo = new SearchErrorInfo(-2147220991, ex);
				}
			});
			searchStatus = theSearchStatus;
			if (errorInfo != null && errorInfo.Failed)
			{
				MailboxSearchServer.LogErrorInfo("Error occured when trying to get the status of the search workitem", searchId, errorInfo);
			}
			return errorInfo;
		}

		public override SearchErrorInfo Abort(SearchId searchId, Guid userGuid)
		{
			return this.AbortEx(searchId, userGuid.ToString());
		}

		public override SearchErrorInfo AbortEx(SearchId searchId, string userId)
		{
			MailboxSearchServer.Tracer.TraceFunction<string>((long)this.GetHashCode(), "MailboxSearchServer.Abort on {0}", searchId.SearchName);
			SearchErrorInfo errorInfo = null;
			SearchUtils.ExWatsonWrappedCall(delegate()
			{
				lock (this)
				{
					if (!this.searchWorkItemMap.ContainsKey(searchId) && !this.pendingSearchIdMap.ContainsKey(searchId))
					{
						MailboxSearchServer.Tracer.TraceWarning<string>((long)this.GetHashCode(), "The search {0} is not started", searchId.SearchName);
						try
						{
							ADObjectId discoverySystemMailboxId = new ADObjectId(searchId.MailboxDsName, searchId.MailboxGuid);
							IMailboxSearchConfigurationProvider mailboxSearchConfigurationProvider = new MailboxSearchConfigurationProvider(discoverySystemMailboxId, searchId.SearchName);
							if (MailboxDiscoverySearch.IsInProgressState(mailboxSearchConfigurationProvider.SearchObject.Status))
							{
								mailboxSearchConfigurationProvider.SearchObject.UpdateState(SearchStateTransition.Fail);
								mailboxSearchConfigurationProvider.UpdateSearchObject("AbortEx", 317);
							}
							errorInfo = new SearchErrorInfo(262658, Strings.SearchNotStarted);
							goto IL_3A4;
						}
						catch (FormatException exception)
						{
							errorInfo = new SearchErrorInfo(-2147220990, exception);
							goto IL_3A4;
						}
						catch (SearchObjectNotFoundException exception2)
						{
							errorInfo = new SearchErrorInfo(-2147220990, exception2);
							goto IL_3A4;
						}
						catch (DataSourceOperationException exception3)
						{
							errorInfo = new SearchErrorInfo(-2147220990, exception3);
							goto IL_3A4;
						}
					}
					if (this.searchWorkItemMap.ContainsKey(searchId))
					{
						MailboxSearchWorkItem searchWorkItem = this.searchWorkItemMap[searchId];
						if (searchWorkItem.IsCompleted)
						{
							goto IL_3A4;
						}
						try
						{
							searchWorkItem.Abort(userId);
						}
						catch (ExportException ex)
						{
							MailboxSearchServer.Tracer.TraceError<ExportException>((long)this.GetHashCode(), "MailboxSearchServer.Abort error {0}", ex);
							errorInfo = new SearchErrorInfo(-2147220991, ex);
						}
						catch (DataSourceOperationException ex2)
						{
							MailboxSearchServer.Tracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "MailboxSearchServer.Abort error {0}", ex2);
							errorInfo = new SearchErrorInfo(-2147220991, ex2);
						}
						lock (this.workItemQueue)
						{
							int num = this.workItemQueue.FindIndex((MailboxSearchWorkItem x) => x == searchWorkItem);
							if (num != -1)
							{
								if (num != 0)
								{
									this.workItemQueue.RemoveAt(num);
									this.workItemQueue.Insert(0, searchWorkItem);
								}
								int num2 = this.workItemSemaphore.Release();
								MailboxSearchServer.Tracer.TraceDebug<int, int>((long)this.GetHashCode(), "MailboxSearch is aborted with Queue Length {0} and semaphore {1}", this.workItemQueue.Count, num2 + 1);
								SearchEventLogger.Instance.LogDiscoverySearchWorkItemQueueChangedEvent("MovedToFrontOfQueueOnAbort", searchId.SearchName, searchId.MailboxDsName, searchWorkItem.Action.ToString(), searchWorkItem.IsEstimateOnly, this.workItemQueue.Count, this.searchWorkItemMap.Count, this.copySearchesInProgress, num2 + 1);
							}
							goto IL_3A4;
						}
					}
					if (this.pendingSearchIdMap.ContainsKey(searchId))
					{
						this.pendingSearchIdMap.Remove(searchId);
						SearchEventLogger.Instance.LogDiscoverySearchPendingWorkItemsChangedEvent("RemovedFromPendingWorkItemsOnAbort", searchId.SearchName, searchId.MailboxDsName, this.pendingSearchIdMap.Count);
					}
					IL_3A4:;
				}
			});
			if (errorInfo != null && errorInfo.Failed)
			{
				MailboxSearchServer.LogErrorInfo("Error occured when trying to abort the search workitem", searchId, errorInfo);
			}
			return errorInfo;
		}

		public override SearchErrorInfo UpdateStatus(SearchId searchId)
		{
			MailboxSearchServer.Tracer.TraceFunction<string>((long)this.GetHashCode(), "MailboxSearchServer.UpdateStatus on {0}", searchId.SearchName);
			SearchErrorInfo errorInfo = null;
			SearchUtils.ExWatsonWrappedCall(delegate()
			{
				lock (this)
				{
					if (this.searchWorkItemMap.ContainsKey(searchId) || this.pendingSearchIdMap.ContainsKey(searchId))
					{
						MailboxSearchServer.Tracer.TraceDebug<string>((long)this.GetHashCode(), "The search {0} is still in the queue. Not updating status", searchId.SearchName);
					}
					else
					{
						try
						{
							ADObjectId discoverySystemMailboxId = new ADObjectId(searchId.MailboxDsName, searchId.MailboxGuid);
							IMailboxSearchConfigurationProvider mailboxSearchConfigurationProvider = new MailboxSearchConfigurationProvider(discoverySystemMailboxId, searchId.SearchName);
							if (MailboxDiscoverySearch.IsInProgressState(mailboxSearchConfigurationProvider.SearchObject.Status))
							{
								mailboxSearchConfigurationProvider.SearchObject.UpdateState(SearchStateTransition.Fail);
								mailboxSearchConfigurationProvider.UpdateSearchObject("UpdateStatus", 452);
							}
							else if (MailboxDiscoverySearch.IsInDeletionState(mailboxSearchConfigurationProvider.SearchObject.Status))
							{
								this.QueueSearchForDeletion(searchId, true, ref errorInfo);
							}
						}
						catch (FormatException exception)
						{
							errorInfo = new SearchErrorInfo(-2147220990, exception);
						}
						catch (SearchObjectNotFoundException exception2)
						{
							errorInfo = new SearchErrorInfo(-2147220990, exception2);
						}
						catch (DataSourceOperationException exception3)
						{
							errorInfo = new SearchErrorInfo(-2147220990, exception3);
						}
						catch (NoInternalEwsAvailableException exception4)
						{
							errorInfo = new SearchErrorInfo(-2147220990, exception4);
						}
					}
				}
			});
			if (errorInfo != null && errorInfo.Failed)
			{
				MailboxSearchServer.LogErrorInfo("Error occured when trying to update the status of the search workitem", searchId, errorInfo);
			}
			return errorInfo;
		}

		public override SearchErrorInfo Remove(SearchId searchId, bool removeLogs)
		{
			MailboxSearchServer.Tracer.TraceFunction<string>((long)this.GetHashCode(), "MailboxSearchServer.Remove on {0}", searchId.SearchName);
			SearchErrorInfo errorInfo = null;
			SearchUtils.ExWatsonWrappedCall(delegate()
			{
				lock (this)
				{
					if (this.IsShutDown)
					{
						errorInfo = new SearchErrorInfo(-2147220983, Strings.SearchServerShutdown);
					}
					else if (this.searchWorkItemMap.ContainsKey(searchId))
					{
						MailboxSearchServer.Tracer.TraceWarning<string>((long)this.GetHashCode(), "The search {0} is already started", searchId.SearchName);
						errorInfo = new SearchErrorInfo(-2147220980, Strings.ErrorRemoveOngoingSearch);
					}
					else if (this.pendingSearchIdMap.ContainsKey(searchId))
					{
						this.pendingSearchIdMap.Remove(searchId);
						SearchEventLogger.Instance.LogDiscoverySearchPendingWorkItemsChangedEvent("RemovedFromPendingWorkItemsOnRemove", searchId.SearchName, searchId.MailboxDsName, this.pendingSearchIdMap.Count);
					}
					else
					{
						this.QueueSearchForDeletion(searchId, removeLogs, ref errorInfo);
					}
				}
			});
			if (errorInfo != null && errorInfo.Failed)
			{
				MailboxSearchServer.LogErrorInfo("Error occured when trying to remove the search workitem", searchId, errorInfo);
			}
			return errorInfo;
		}

		internal static void StartServer()
		{
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
			FileSystemAccessRule accessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.ReadData, AccessControlType.Allow);
			FileSecurity fileSecurity = new FileSecurity();
			fileSecurity.SetOwner(securityIdentifier);
			fileSecurity.SetAccessRule(accessRule);
			MailboxSearchServer.server = (MailboxSearchServer)RpcServerBase.RegisterServer(typeof(MailboxSearchServer), fileSecurity, 1);
			MailboxSearchServer.server.InternalStart();
		}

		internal static void StopServer()
		{
			MailboxSearchServer.server.InternalShutdown();
			RpcServerBase.StopServer(MailboxSearchRpcServer.RpcIntfHandle);
			MailboxSearchServer.server = null;
		}

		internal static void RestartServer()
		{
			MailboxSearchServer.StopServer();
			MailboxSearchServer.StartServer();
		}

		internal void RemoveWorkItem(SearchId searchId, bool isEstimateOnly)
		{
			this.RemoveWorkItem(searchId, true, isEstimateOnly);
		}

		internal void RemoveWorkItem(SearchId searchId, bool releaseSemaphore, bool isEstimateOnly)
		{
			lock (this)
			{
				MailboxSearchWorkItem mailboxSearchWorkItem;
				if (this.searchWorkItemMap.TryGetValue(searchId, out mailboxSearchWorkItem))
				{
					this.searchWorkItemMap.Remove(searchId);
					int num = -1;
					if (releaseSemaphore)
					{
						num = this.workItemSemaphore.Release();
					}
					if (!isEstimateOnly && mailboxSearchWorkItem.Action == WorkItemAction.Start)
					{
						this.copySearchesInProgress--;
					}
					MailboxSearchServer.Tracer.TraceDebug<int, int>((long)this.GetHashCode(), "A search job is removed, Queue Length {0} and semaphore {1}", this.workItemQueue.Count, num + 1);
					SearchEventLogger.Instance.LogDiscoverySearchWorkItemQueueChangedEvent("RemovedFromRunningWorkItems", searchId.SearchName, searchId.MailboxDsName, mailboxSearchWorkItem.Action.ToString(), mailboxSearchWorkItem.IsEstimateOnly, this.workItemQueue.Count, this.searchWorkItemMap.Count, this.copySearchesInProgress, num + 1);
					SearchUtils.ExWatsonWrappedCall(delegate()
					{
						string ownerId = null;
						if (this.pendingSearchIdMap.TryGetValue(searchId, out ownerId))
						{
							SearchErrorInfo searchErrorInfo = this.AddSearchToWorkItemQueue(searchId, ownerId);
							if (searchErrorInfo != null && searchErrorInfo.Failed)
							{
								MailboxSearchServer.LogErrorInfo("Error occured when try to Add a pending workitem to workitem queue", searchId, searchErrorInfo);
							}
							this.pendingSearchIdMap.Remove(searchId);
							MailboxSearchServer.Tracer.TraceDebug<int>((long)this.GetHashCode(), "Add pending search to the Queue, length = {0}", this.workItemQueue.Count);
							SearchEventLogger.Instance.LogDiscoverySearchPendingWorkItemsChangedEvent("RemovedFromPendingWorkItems", searchId.SearchName, searchId.MailboxDsName, this.pendingSearchIdMap.Count);
						}
					});
				}
				else
				{
					MailboxSearchServer.Tracer.TraceError<string>((long)this.GetHashCode(), "Removing search {0} failed becasue it is not in the work item map.", searchId.SearchName);
				}
			}
		}

		protected virtual void InternalStart()
		{
			this.workItemPumpingThread = new Thread(new ThreadStart(this.WorkItemPumpingThreadEntry));
			this.workItemPumpingThread.Start();
		}

		protected virtual void InternalShutdown()
		{
			this.shutdownEvent.Set();
			WaitHandle[] array = null;
			bool flag = false;
			try
			{
				Monitor.Enter(this, ref flag);
				MailboxSearchWorkItem currentItem = null;
				try
				{
					array = this.searchWorkItemMap.Values.Select(delegate(MailboxSearchWorkItem workitem)
					{
						currentItem = workitem;
						workitem.Abort(string.Empty);
						return workitem.WaitHandle;
					}).ToArray<WaitHandle>();
				}
				catch (DataSourceOperationException ex)
				{
					MailboxSearchServer.Tracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "MailboxSearchServer.InternalShutdown error {0}", ex);
					SearchEventLogger.Instance.LogDiscoverySearchServerErrorEvent("Exception occured when trying to abort search during shutdown", (currentItem != null) ? currentItem.SearchId.SearchName : null, (currentItem != null) ? currentItem.SearchId.MailboxDsName : null, ex);
				}
				this.pendingSearchIdMap.Clear();
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this);
				}
			}
			if (!array.IsNullOrEmpty<WaitHandle>())
			{
				WaitHandle.WaitAll(array);
			}
			if (this.workItemPumpingThread != null && this.workItemPumpingThread.IsAlive)
			{
				this.workItemPumpingThread.Join();
			}
		}

		private static void LogErrorInfo(string description, SearchId searchId, SearchErrorInfo errorInfo)
		{
			if (errorInfo.Exception == null)
			{
				SearchEventLogger.Instance.LogDiscoverySearchServerErrorEvent(description, searchId.SearchName, searchId.MailboxDsName, errorInfo.Message);
				return;
			}
			SearchEventLogger.Instance.LogDiscoverySearchServerErrorEvent(description, searchId.SearchName, searchId.MailboxDsName, errorInfo.Exception);
		}

		private SearchErrorInfo AddSearchToWorkItemQueue(SearchId searchId, string ownerId)
		{
			SearchErrorInfo result = null;
			try
			{
				MailboxSearchWorkItem mailboxSearchWorkItem = new MailboxSearchWorkItem(this, searchId);
				mailboxSearchWorkItem.PreStart(ownerId, null);
				this.searchWorkItemMap[searchId] = mailboxSearchWorkItem;
				lock (this.workItemQueue)
				{
					this.workItemQueue.Add(mailboxSearchWorkItem);
					int num = this.queueSemaphore.Release();
					MailboxSearchServer.Tracer.TraceDebug<SearchId, int, int>((long)this.GetHashCode(), "WorkItem '{0}' is enqueued, Queue length is {1}, Semaphore signals {2}", searchId, this.workItemQueue.Count, num + 1);
					SearchEventLogger.Instance.LogDiscoverySearchWorkItemQueueChangedEvent("EnqueuedAndAddedToRunningWorkItems", searchId.SearchName, searchId.MailboxDsName, mailboxSearchWorkItem.Action.ToString(), mailboxSearchWorkItem.IsEstimateOnly, this.workItemQueue.Count, this.searchWorkItemMap.Count, this.copySearchesInProgress, num + 1);
				}
			}
			catch (FormatException ex)
			{
				MailboxSearchServer.Tracer.TraceError<FormatException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex);
				result = new SearchErrorInfo(-2147220988, ex);
			}
			catch (ArgumentException ex2)
			{
				MailboxSearchServer.Tracer.TraceError<ArgumentException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex2);
				result = new SearchErrorInfo(-2147220988, ex2);
			}
			catch (ParserException ex3)
			{
				MailboxSearchServer.Tracer.TraceError<ParserException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex3);
				result = new SearchErrorInfo(-2147220984, ex3);
			}
			catch (SearchQueryEmptyException ex4)
			{
				MailboxSearchServer.Tracer.TraceError<SearchQueryEmptyException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex4);
				result = new SearchErrorInfo(-2147220981, ex4);
			}
			catch (SearchObjectNotFoundException ex5)
			{
				MailboxSearchServer.Tracer.TraceError<SearchObjectNotFoundException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex5);
				result = new SearchErrorInfo(-2147220990, ex5);
			}
			catch (SearchDisabledException ex6)
			{
				MailboxSearchServer.Tracer.TraceError<SearchDisabledException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex6);
				result = new SearchErrorInfo(-2147220978, ex6);
			}
			catch (SearchOverBudgetException ex7)
			{
				MailboxSearchServer.Tracer.TraceError<SearchOverBudgetException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex7);
				result = new SearchErrorInfo(-2147220979, ex7);
			}
			catch (ObjectNotFoundException ex8)
			{
				MailboxSearchServer.Tracer.TraceError<ObjectNotFoundException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex8);
				result = new SearchErrorInfo(-2147220987, ex8);
			}
			catch (UnauthorizedAccessException ex9)
			{
				MailboxSearchServer.Tracer.TraceError<UnauthorizedAccessException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex9);
				result = new SearchErrorInfo(-2147220982, ex9);
			}
			catch (AuthzException ex10)
			{
				MailboxSearchServer.Tracer.TraceError<AuthzException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex10);
				result = new SearchErrorInfo(-2147220982, ex10);
			}
			catch (StoragePermanentException ex11)
			{
				MailboxSearchServer.Tracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex11);
				result = new SearchErrorInfo(-2147220986, ex11);
			}
			catch (StorageTransientException ex12)
			{
				MailboxSearchServer.Tracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex12);
				result = new SearchErrorInfo(-2147220985, ex12);
			}
			catch (ExportException ex13)
			{
				MailboxSearchServer.Tracer.TraceError<ExportException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex13);
				result = new SearchErrorInfo(-2147220991, ex13);
			}
			catch (DataSourceOperationException ex14)
			{
				MailboxSearchServer.Tracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex14);
				result = new SearchErrorInfo(-2147220991, ex14);
			}
			catch (LocalizedException ex15)
			{
				MailboxSearchServer.Tracer.TraceError<LocalizedException>((long)this.GetHashCode(), "MailboxSearchServer.Start error {0}", ex15);
				result = new SearchErrorInfo(-2147220991, ex15);
			}
			return result;
		}

		private void QueueSearchForDeletion(SearchId searchId, bool removeLogs, ref SearchErrorInfo errorInfo)
		{
			try
			{
				MailboxSearchWorkItem mailboxSearchWorkItem = new MailboxSearchWorkItem(this, searchId);
				mailboxSearchWorkItem.PreRemove(removeLogs);
				if (!mailboxSearchWorkItem.IsCompleted)
				{
					this.searchWorkItemMap[mailboxSearchWorkItem.SearchId] = mailboxSearchWorkItem;
					lock (this.workItemQueue)
					{
						this.workItemQueue.Insert(0, mailboxSearchWorkItem);
						int num = this.queueSemaphore.Release();
						MailboxSearchServer.Tracer.TraceDebug<SearchId, int, int>((long)this.GetHashCode(), "WorkItem '{0}' is enqueued for remove, Queue length is {1}, Semaphore signals {2}", searchId, this.workItemQueue.Count, num + 1);
						SearchEventLogger.Instance.LogDiscoverySearchWorkItemQueueChangedEvent("EnqueuedAndAddedToRunningWorkItems", searchId.SearchName, searchId.MailboxDsName, mailboxSearchWorkItem.Action.ToString(), mailboxSearchWorkItem.IsEstimateOnly, this.workItemQueue.Count, this.searchWorkItemMap.Count, this.copySearchesInProgress, num + 1);
					}
				}
			}
			catch (FormatException ex)
			{
				MailboxSearchServer.Tracer.TraceError<FormatException>((long)this.GetHashCode(), "MailboxSearchServer.Remove error {0}", ex);
				errorInfo = new SearchErrorInfo(-2147220988, ex);
			}
			catch (ArgumentException ex2)
			{
				MailboxSearchServer.Tracer.TraceError<ArgumentException>((long)this.GetHashCode(), "MailboxSearchServer.Remove error {0}", ex2);
				errorInfo = new SearchErrorInfo(-2147220988, ex2);
			}
			catch (SearchObjectNotFoundException ex3)
			{
				MailboxSearchServer.Tracer.TraceError<SearchObjectNotFoundException>((long)this.GetHashCode(), "MailboxSearchServer.Remove error {0}", ex3);
				errorInfo = new SearchErrorInfo(-2147220990, ex3);
			}
			catch (ObjectNotFoundException ex4)
			{
				MailboxSearchServer.Tracer.TraceError<ObjectNotFoundException>((long)this.GetHashCode(), "MailboxSearchServer.Remove error {0}", ex4);
				errorInfo = new SearchErrorInfo(-2147220987, ex4);
			}
			catch (UnauthorizedAccessException ex5)
			{
				MailboxSearchServer.Tracer.TraceError<UnauthorizedAccessException>((long)this.GetHashCode(), "MailboxSearchServer.Remove error {0}", ex5);
				errorInfo = new SearchErrorInfo(-2147220982, ex5);
			}
			catch (StoragePermanentException ex6)
			{
				MailboxSearchServer.Tracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "MailboxSearchServer.Remove error {0}", ex6);
				errorInfo = new SearchErrorInfo(-2147220986, ex6);
			}
			catch (StorageTransientException ex7)
			{
				MailboxSearchServer.Tracer.TraceError<StorageTransientException>((long)this.GetHashCode(), "MailboxSearchServer.Remove error {0}", ex7);
				errorInfo = new SearchErrorInfo(-2147220985, ex7);
			}
			catch (ExportException ex8)
			{
				MailboxSearchServer.Tracer.TraceError<ExportException>((long)this.GetHashCode(), "MailboxSearchServer.Remove error {0}", ex8);
				errorInfo = new SearchErrorInfo(-2147220991, ex8);
			}
			catch (LocalizedException ex9)
			{
				MailboxSearchServer.Tracer.TraceError<LocalizedException>((long)this.GetHashCode(), "MailboxSearchServer.Remove error {0}", ex9);
				errorInfo = new SearchErrorInfo(-2147220991, ex9);
			}
			if (errorInfo != null && errorInfo.Failed)
			{
				MailboxSearchServer.LogErrorInfo("Error occured when trying to queue the search for deletion", searchId, errorInfo);
			}
		}

		private void WorkItemPumpingThreadEntry()
		{
			MailboxSearchWorkItem workItem = null;
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				WaitHandle[] waitHandles = new WaitHandle[]
				{
					this.shutdownEvent,
					this.queueSemaphore
				};
				WaitHandle[] waitHandles2 = new WaitHandle[]
				{
					this.shutdownEvent,
					this.workItemSemaphore
				};
				while (WaitHandle.WaitAny(waitHandles) != 0)
				{
					if (WaitHandle.WaitAny(waitHandles2) == 0)
					{
						MailboxSearchServer.Tracer.TraceDebug<ExDateTime>((long)this.GetHashCode(), "WorkItemPumpingThread received shutdown signal when waiting for work item resource, exiting at {0}", ExDateTime.Now);
						return;
					}
					workItem = this.GetNextWorkItem();
					if (workItem != null)
					{
						bool releaseSemaphore = !workItem.IsCompleted;
						try
						{
							GrayException.MapAndReportGrayExceptions(delegate()
							{
								bool flag = true;
								try
								{
									lock (this)
									{
										switch (workItem.Action)
										{
										case WorkItemAction.Start:
											workItem.Start();
											break;
										case WorkItemAction.Remove:
											workItem.Remove();
											break;
										}
										flag = workItem.IsCompleted;
									}
								}
								finally
								{
									if (flag)
									{
										this.RemoveWorkItem(workItem.SearchId, releaseSemaphore, workItem.IsEstimateOnly);
									}
								}
							});
						}
						catch (GrayException ex)
						{
							MailboxSearchServer.Tracer.TraceError<SearchId, Exception>((long)this.GetHashCode(), "WorkItem {0} has unhandled exception {1}, the work item is discarded!", workItem.SearchId, ex);
							SearchEventLogger.Instance.LogDiscoverySearchServerErrorEvent("An unhandled exception occurred while processing the work item, ignoring and continuing the processing of queue", workItem.SearchId.SearchName, workItem.SearchId.MailboxDsName, ex);
						}
					}
				}
				MailboxSearchServer.Tracer.TraceDebug<ExDateTime>((long)this.GetHashCode(), "WorkItemPumpingThread received shutdown signal when waiting for Queue, exiting at {0}", ExDateTime.Now);
			}, delegate(object exception)
			{
				MailboxSearchServer.Tracer.TraceError((long)this.GetHashCode(), "ExWatsonWrappedCall: Unhandled exception {0}", new object[]
				{
					exception
				});
				SearchEventLogger.Instance.LogDiscoverySearchServerErrorEvent("ExWatsonWrappedCall: Unhandled exception", (workItem != null) ? workItem.SearchId.SearchName : null, (workItem != null) ? workItem.SearchId.MailboxDsName : null, exception.ToString());
				return !(exception is GrayException);
			});
		}

		private MailboxSearchWorkItem GetNextWorkItem()
		{
			MailboxSearchWorkItem mailboxSearchWorkItem = null;
			lock (this.workItemQueue)
			{
				mailboxSearchWorkItem = this.workItemQueue.FirstOrDefault((MailboxSearchWorkItem x) => x.IsEstimateOnly);
				if (mailboxSearchWorkItem == null)
				{
					mailboxSearchWorkItem = this.workItemQueue.FirstOrDefault((MailboxSearchWorkItem x) => x.Action == WorkItemAction.Remove);
					if (mailboxSearchWorkItem == null)
					{
						MailboxSearchWorkItem mailboxSearchWorkItem2 = this.workItemQueue.First<MailboxSearchWorkItem>();
						if (this.copySearchesInProgress < ResponseThrottler.MaxRunningCopySearches)
						{
							mailboxSearchWorkItem = mailboxSearchWorkItem2;
						}
						else
						{
							SearchEventLogger.Instance.LogDiscoverySearchWorkItemQueueNotProcessedEvent(mailboxSearchWorkItem2.SearchId.SearchName, mailboxSearchWorkItem2.SearchId.MailboxDsName, mailboxSearchWorkItem2.Action.ToString(), mailboxSearchWorkItem2.IsEstimateOnly, this.workItemQueue.Count, this.searchWorkItemMap.Count, this.copySearchesInProgress, ResponseThrottler.MaxRunningCopySearches);
						}
					}
				}
				if (mailboxSearchWorkItem == null)
				{
					this.queueSemaphore.Release();
					this.workItemSemaphore.Release();
				}
				else
				{
					this.workItemQueue.Remove(mailboxSearchWorkItem);
					if (!mailboxSearchWorkItem.IsEstimateOnly && mailboxSearchWorkItem.Action == WorkItemAction.Start)
					{
						this.copySearchesInProgress++;
					}
					MailboxSearchServer.Tracer.TraceDebug<SearchId, ExDateTime>((long)this.GetHashCode(), "WorkItemPumpingThread start work item {0} at {1}", mailboxSearchWorkItem.SearchId, ExDateTime.Now);
					SearchEventLogger.Instance.LogDiscoverySearchWorkItemQueueChangedEvent("DequeuedForProcessing", mailboxSearchWorkItem.SearchId.SearchName, mailboxSearchWorkItem.SearchId.MailboxDsName, mailboxSearchWorkItem.Action.ToString(), mailboxSearchWorkItem.IsEstimateOnly, this.workItemQueue.Count, this.searchWorkItemMap.Count, this.copySearchesInProgress, 0);
				}
			}
			return mailboxSearchWorkItem;
		}

		protected static readonly Trace Tracer = ExTraceGlobals.SearchTracer;

		private static MailboxSearchServer server = null;

		private Dictionary<SearchId, MailboxSearchWorkItem> searchWorkItemMap = new Dictionary<SearchId, MailboxSearchWorkItem>();

		private Dictionary<SearchId, string> pendingSearchIdMap = new Dictionary<SearchId, string>();

		private List<MailboxSearchWorkItem> workItemQueue = new List<MailboxSearchWorkItem>();

		private Semaphore queueSemaphore = new Semaphore(0, int.MaxValue);

		private Semaphore workItemSemaphore = new Semaphore(ResponseThrottler.MaxRunningSearches, int.MaxValue);

		private int copySearchesInProgress;

		private Thread workItemPumpingThread;

		private ManualResetEvent shutdownEvent = new ManualResetEvent(false);

		private struct QueueEventDescriptions
		{
			public const string AddedToPendingWorkItems = "AddedToPendingWorkItems";

			public const string EnqueuedAndAddedToRunningWorkItems = "EnqueuedAndAddedToRunningWorkItems";

			public const string MovedToFrontOfQueueOnAbort = "MovedToFrontOfQueueOnAbort";

			public const string RemovedFromPendingWorkItemsOnAbort = "RemovedFromPendingWorkItemsOnAbort";

			public const string RemovedFromPendingWorkItemsOnRemove = "RemovedFromPendingWorkItemsOnRemove";

			public const string DequeuedForProcessing = "DequeuedForProcessing";

			public const string RemovedFromRunningWorkItems = "RemovedFromRunningWorkItems";

			public const string RemovedFromPendingWorkItems = "RemovedFromPendingWorkItems";
		}
	}
}
