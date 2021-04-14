using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class SearchMailboxExecuter
	{
		internal SearchMailboxExecuter(IRecipientSession recipientSession, IConfigurationSession configurationSession, SearchMailboxCriteria searchMailboxCriteria, ADRecipient targetUser)
		{
			this.RecipientSession = recipientSession;
			this.ConfigurationSession = configurationSession;
			this.searchMailboxCriteria = searchMailboxCriteria;
			this.TargetUser = targetUser;
			this.searchCommunicator = new SearchCommunicator();
			this.threadLimit = 1;
			this.threadLimitPerServer = 1;
		}

		internal SearchMailboxExecuter(IRecipientSession recipientSession, IConfigurationSession configurationSession, SearchMailboxCriteria searchMailboxCriteria, ADRecipient targetUser, HashSet<UniqueItemHash> processedMessages, HashSet<string> processedMessageIds, int maxThreadLimitPerSearch, int maxThreadLimitPerServer)
		{
			this.RecipientSession = recipientSession;
			this.ConfigurationSession = configurationSession;
			this.searchMailboxCriteria = searchMailboxCriteria;
			this.TargetUser = targetUser;
			this.searchCommunicator = new SearchCommunicator(processedMessages, processedMessageIds);
			this.threadLimit = maxThreadLimitPerSearch;
			this.threadLimitPerServer = maxThreadLimitPerServer;
		}

		internal GenericIdentity OwnerIdentity
		{
			get
			{
				return this.ownerIdentity;
			}
			set
			{
				this.ownerIdentity = value;
			}
		}

		internal ADRecipient TargetUser
		{
			get
			{
				return this.targetUser;
			}
			set
			{
				this.targetUser = value;
			}
		}

		internal string TargetFolder
		{
			get
			{
				return this.targetFolder;
			}
			set
			{
				this.targetFolder = value;
			}
		}

		internal ADRecipient[] ReviewRecipients
		{
			get
			{
				return this.reviewRecipients;
			}
			set
			{
				this.reviewRecipients = value;
			}
		}

		internal bool EstimationPhase
		{
			get
			{
				return this.TargetUser == null;
			}
		}

		internal IRecipientSession RecipientSession
		{
			get
			{
				return this.recipientSession;
			}
			set
			{
				this.recipientSession = value;
			}
		}

		internal IConfigurationSession ConfigurationSession
		{
			get
			{
				return this.configurationSession;
			}
			set
			{
				this.configurationSession = value;
			}
		}

		internal LoggingLevel LogLevel
		{
			get
			{
				return this.logLevel;
			}
			set
			{
				this.logLevel = value;
			}
		}

		internal SearchState SearchState
		{
			get
			{
				return this.searchState;
			}
			set
			{
				this.searchState = value;
			}
		}

		internal ExDateTime? SearchStartTime
		{
			get
			{
				return this.searchStartTime;
			}
		}

		internal EventHandler<SearchProgressEvent> ProgressHandler
		{
			get
			{
				return this.progressHandler;
			}
			set
			{
				this.progressHandler = value;
			}
		}

		internal EventHandler<SearchLoggingEvent> LoggingHandler
		{
			get
			{
				return this.loggingHandler;
			}
			set
			{
				this.loggingHandler = value;
			}
		}

		internal EventHandler<SearchExceptionEvent> SearchExceptionHandler
		{
			get
			{
				return this.searchExceptionHandler;
			}
			set
			{
				this.searchExceptionHandler = value;
			}
		}

		internal EventHandler<RequestLogBodyEvent> RequestLogBodyHandler
		{
			get
			{
				return this.requestBodyHandler;
			}
			set
			{
				this.requestBodyHandler = value;
			}
		}

		internal bool WaitForWorkersWhenAborted
		{
			get
			{
				return this.waitForWorkersWhenAborted;
			}
			set
			{
				this.waitForWorkersWhenAborted = value;
			}
		}

		internal SearchMailboxCriteria SearchMailboxCriteria
		{
			get
			{
				return this.searchMailboxCriteria;
			}
		}

		internal List<SearchMailboxAction> SearchActions
		{
			get
			{
				return this.searchActions;
			}
			set
			{
				this.searchActions = value;
			}
		}

		internal string Name { get; set; }

		internal StreamLogItem StreamLogItem { get; set; }

		internal StoreId LogMessageId { get; set; }

		internal int ThreadLimit
		{
			get
			{
				return this.threadLimit;
			}
		}

		internal int ThreadLimitPerServer
		{
			get
			{
				return this.threadLimitPerServer;
			}
		}

		internal bool ResumeSearch { get; set; }

		internal bool HasPreviousCompletedMailboxes { get; set; }

		internal MultiValuedProperty<string> SuccessfulMailboxes
		{
			get
			{
				return this.searchCommunicator.SuccessfulMailboxes;
			}
			set
			{
				this.searchCommunicator.SuccessfulMailboxes = value;
			}
		}

		internal MultiValuedProperty<string> UnsuccessfulMailboxes
		{
			get
			{
				return this.searchCommunicator.UnsuccessfulMailboxes;
			}
			set
			{
				this.searchCommunicator.UnsuccessfulMailboxes = value;
			}
		}

		internal SearchObject SearchObject { get; set; }

		internal ObjectId ExecutingUserIdentity { get; set; }

		internal IAsyncResult BeginExecuteSearch(AsyncCallback asyncCallback, object state, ExDateTime? searchStartTime)
		{
			SearchMailboxExecuter.Tracer.TraceFunction<SearchMailboxCriteria, ADRecipient, string>((long)this.GetHashCode(), "SearchMailboxExecuter::BeginExecuteSearch. SearchCriteria=[{0}], TargetUser={1}, TargetFolder={2}", this.searchMailboxCriteria, this.TargetUser, this.TargetFolder);
			this.PrepareSearch(searchStartTime);
			this.userCallback = asyncCallback;
			this.state = state;
			this.asyncResult = new SearchMailboxExecuter.SearchMailboxAsyncResult(this);
			Thread thread = new Thread(new ThreadStart(this.WatsonWrappedSearchEntry));
			thread.Start();
			return this.asyncResult;
		}

		internal void EndExecuteSearch(IAsyncResult asyncResult)
		{
			SearchMailboxExecuter.Tracer.TraceFunction((long)this.GetHashCode(), "SearchMailboxExecuter::EndExecuteSearch.");
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (this.asyncResult == null)
			{
				throw new InvalidOperationException("No search initialized");
			}
			if (this.asyncResult != asyncResult)
			{
				throw new ArgumentException("Invalidate async result", "asyncResult");
			}
			this.asyncResult.AsyncWaitHandle.WaitOne();
		}

		internal void ExecuteSearch()
		{
			SearchMailboxExecuter.Tracer.TraceFunction<SearchMailboxCriteria, ADRecipient, string>((long)this.GetHashCode(), "SearchMailboxExecuter::ExecuteSearch. SearchCriteria=[{0}], TargetUser={1}, TargetFolder={2}", this.searchMailboxCriteria, this.TargetUser, this.TargetFolder);
			this.PrepareSearch(new ExDateTime?(ExDateTime.UtcNow));
			this.SearchMainEntry();
		}

		internal void Abort()
		{
			SearchMailboxExecuter.Tracer.TraceFunction((long)this.GetHashCode(), "Entering Abort()");
			this.SearchState = (this.SearchMailboxCriteria.EstimateOnly ? SearchState.EstimateStopped : SearchState.Stopped);
			if (this.searchCommunicator != null)
			{
				SearchMailboxExecuter.Tracer.TraceDebug((long)this.GetHashCode(), "The search is aborted");
				lock (this.searchCommunicator)
				{
					this.searchCommunicator.Abort();
				}
			}
			SearchMailboxExecuter.Tracer.TraceFunction((long)this.GetHashCode(), "Leaving Abort()");
		}

		internal void ResetSearchStartTime()
		{
			this.searchStartTime = null;
		}

		internal SearchMailboxResult GetSearchResult(int sourceIndex)
		{
			return this.searchMailboxWorkers[sourceIndex].SearchResult;
		}

		internal SearchMailboxResult[] GetSearchResult()
		{
			List<SearchMailboxResult> list = new List<SearchMailboxResult>();
			foreach (SearchMailboxWorker searchMailboxWorker in this.searchMailboxWorkers)
			{
				list.Add(searchMailboxWorker.SearchResult);
			}
			return list.ToArray();
		}

		private void FireProgressEvent(LocalizedString activity, LocalizedString statusDescription, int percentCompleted, long resultItems, ByteQuantifiedSize resultSize)
		{
			if (this.ProgressHandler != null)
			{
				this.ProgressHandler(this, new SearchProgressEvent(activity, statusDescription, percentCompleted, resultItems, resultSize));
			}
		}

		private void FireLoggingEvent(LocalizedString loggingMessage)
		{
			if (this.LoggingHandler != null)
			{
				this.LoggingHandler(this, new SearchLoggingEvent(loggingMessage));
			}
		}

		private void FireExceptionEvent(SearchMailboxWorker worker, Exception exception)
		{
			SearchMailboxExecuter.Tracer.TraceError<SearchMailboxWorker, Exception>((long)this.GetHashCode(), "ExceptionEvent: worker {0} is aborted due to exception {1}", worker, exception);
			if (this.SearchExceptionHandler != null)
			{
				this.SearchExceptionHandler(this, new SearchExceptionEvent(new int?(worker.WorkerId), exception));
			}
		}

		private void FireExceptionEvent(Exception exception)
		{
			SearchMailboxExecuter.Tracer.TraceError<Exception>((long)this.GetHashCode(), "ExceptionEvent: the main thread is aborted due to exception {0}", exception);
			if (this.SearchExceptionHandler != null)
			{
				this.SearchExceptionHandler(this, new SearchExceptionEvent(null, exception));
			}
		}

		private void FireRequestLogBodyEvent(Body itemBody)
		{
			if (this.RequestLogBodyHandler != null)
			{
				this.RequestLogBodyHandler(this, new RequestLogBodyEvent(itemBody));
			}
		}

		private void SetReviewerPermission(Folder folder)
		{
			PermissionSet permissionSet = folder.GetPermissionSet();
			permissionSet.Clear();
			if (this.ReviewRecipients != null && this.ReviewRecipients.Length > 0)
			{
				foreach (ADRecipient adRecipient in this.ReviewRecipients)
				{
					PermissionSecurityPrincipal securityPrincipal = new PermissionSecurityPrincipal(adRecipient);
					if (permissionSet.GetEntry(securityPrincipal) == null)
					{
						permissionSet.AddEntry(securityPrincipal, PermissionLevel.Reviewer);
					}
				}
			}
		}

		private bool AddViewFolderPermission(Folder folder)
		{
			bool result = false;
			if (this.ReviewRecipients != null && this.ReviewRecipients.Length > 0)
			{
				PermissionSet permissionSet = folder.GetPermissionSet();
				foreach (ADRecipient adRecipient in this.ReviewRecipients)
				{
					PermissionSecurityPrincipal securityPrincipal = new PermissionSecurityPrincipal(adRecipient);
					if (permissionSet.GetEntry(securityPrincipal) == null)
					{
						Permission permission = permissionSet.AddEntry(securityPrincipal, PermissionLevel.None);
						permission.IsFolderVisible = true;
						result = true;
					}
				}
			}
			return result;
		}

		private StoreId GetTargetFolderId(MailboxSession targetMailbox)
		{
			StoreId result = null;
			using (Folder folder = Folder.Bind(targetMailbox, DefaultFolderType.Root))
			{
				if (string.IsNullOrEmpty(this.TargetFolder))
				{
					result = folder.Id;
				}
				else
				{
					using (Folder folder2 = Folder.Create(targetMailbox, folder.Id, StoreObjectType.Folder, this.TargetFolder, CreateMode.OpenIfExists))
					{
						FolderSaveResult folderSaveResult = folder2.Save();
						if (folderSaveResult != null && folderSaveResult.OperationResult != OperationResult.Succeeded)
						{
							SearchMailboxExecuter.Tracer.TraceError<OperationResult>((long)this.GetHashCode(), "SearchExecuter save target folder failed {0}", folderSaveResult.OperationResult);
						}
						folder2.Load();
						this.SetReviewerPermission(folder2);
						folderSaveResult = folder2.Save();
						if (folderSaveResult != null && folderSaveResult.OperationResult != OperationResult.Succeeded)
						{
							SearchMailboxExecuter.Tracer.TraceError<OperationResult>((long)this.GetHashCode(), "SearchExecuter save permission on target folder failed {0}", folderSaveResult.OperationResult);
						}
						folder2.Load();
						result = folder2.Id;
					}
				}
				if (this.AddViewFolderPermission(folder))
				{
					FolderSaveResult folderSaveResult2 = folder.Save();
					if (folderSaveResult2 != null && folderSaveResult2.OperationResult != OperationResult.Succeeded)
					{
						SearchMailboxExecuter.Tracer.TraceError<OperationResult>((long)this.GetHashCode(), "SearchExecuter save permission on RootFolder failed {0}", folderSaveResult2.OperationResult);
					}
				}
			}
			return result;
		}

		private void PrepareSearch(ExDateTime? searchStartTime)
		{
			if (this.searchMailboxCriteria == null)
			{
				throw new ArgumentNullException("searchMailboxCriteria");
			}
			Dictionary<ADObjectId, object> dictionary = new Dictionary<ADObjectId, object>();
			if (this.TargetUser != null)
			{
				if (this.TargetUser.RecipientType != RecipientType.UserMailbox)
				{
					throw new ArgumentException("Target use must be a mailbox user");
				}
				if (dictionary.ContainsKey(this.targetUser.Id))
				{
					throw new ArgumentException("target mailbox user can't be searched");
				}
			}
			this.searchMailboxCriteria.ResolveQueryFilter(this.RecipientSession, this.ConfigurationSession);
			if (this.EstimationPhase)
			{
				this.searchMailboxCriteria.GenerateSubQueryFilters(this.RecipientSession, this.ConfigurationSession);
			}
			this.searchMailboxWorkers.Clear();
			this.searchCompleteEvent.Reset();
			this.searchCommunicator.Reset(this.searchMailboxCriteria.SearchUserScope.Length);
			if (!this.searchCommunicator.IsAborted)
			{
				this.SearchState = (this.searchMailboxCriteria.EstimateOnly ? SearchState.EstimateInProgress : SearchState.InProgress);
			}
			if (this.searchStartTime == null)
			{
				this.searchStartTime = ((searchStartTime == null) ? new ExDateTime?(ExDateTime.UtcNow) : searchStartTime);
			}
			this.StreamLogItem = null;
		}

		private void PostProcessSearch()
		{
			bool flag = true;
			bool flag2 = true;
			foreach (SearchMailboxWorker searchMailboxWorker in this.searchMailboxWorkers)
			{
				SearchMailboxResult searchResult = searchMailboxWorker.SearchResult;
				searchResult.LastException = searchMailboxWorker.LastException;
				searchResult.TargetMailbox = (this.EstimationPhase ? null : this.TargetUser.Id);
				flag2 = (flag2 && searchResult.Success);
				flag = (flag && !searchResult.Success);
				if (!this.EstimationPhase)
				{
					if (!string.IsNullOrEmpty(this.TargetFolder))
					{
						searchResult.TargetFolder = string.Format("\\{0}\\{1}", this.TargetFolder, searchResult.TargetFolder);
					}
					else
					{
						searchResult.TargetFolder = string.Format("\\{0}", searchResult.TargetFolder);
					}
				}
			}
			if (this.SearchState == SearchState.InProgress || this.SearchState == SearchState.EstimateInProgress)
			{
				if (flag2)
				{
					this.SearchState = (this.searchMailboxCriteria.EstimateOnly ? SearchState.EstimateSucceeded : SearchState.Succeeded);
					return;
				}
				if (flag && (this.SuccessfulMailboxes == null || this.SuccessfulMailboxes.Count == 0) && !this.HasPreviousCompletedMailboxes)
				{
					this.SearchState = (this.searchMailboxCriteria.EstimateOnly ? SearchState.EstimateFailed : SearchState.Failed);
					return;
				}
				this.SearchState = (this.searchMailboxCriteria.EstimateOnly ? SearchState.EstimatePartiallySucceeded : SearchState.PartiallySucceeded);
			}
		}

		private void WatsonWrappedSearchEntry()
		{
			bool searchDone = false;
			SearchUtils.ExWatsonWrappedCall(delegate()
			{
				this.SearchMainEntry();
				searchDone = true;
			}, delegate()
			{
				if (!searchDone)
				{
					this.SearchState = (this.SearchMailboxCriteria.EstimateOnly ? SearchState.EstimateFailed : SearchState.Failed);
				}
			});
		}

		private void CatchKnownExceptions(Action tryDelegate, Action<Exception> finallyDelegate)
		{
			Exception obj = null;
			try
			{
				tryDelegate();
			}
			catch (AccessDeniedException ex)
			{
				obj = ex;
			}
			catch (ObjectNotFoundException ex2)
			{
				obj = ex2;
			}
			catch (StorageTransientException ex3)
			{
				obj = ex3;
			}
			catch (StoragePermanentException ex4)
			{
				obj = ex4;
			}
			finally
			{
				finallyDelegate(obj);
			}
		}

		private void SearchMainEntry()
		{
			this.CatchKnownExceptions(delegate
			{
				this.InternalSearchMainEntry();
			}, delegate(Exception exception)
			{
				if (exception != null)
				{
					this.SearchState = (this.SearchMailboxCriteria.EstimateOnly ? SearchState.EstimateFailed : SearchState.Failed);
					this.FireExceptionEvent(exception);
					SearchMailboxExecuter.Tracer.TraceError<Exception>((long)this.GetHashCode(), "SearchMainEntry throws {0}", exception);
				}
				StreamLogItem streamLogItem = this.StreamLogItem;
				this.CatchKnownExceptions(delegate
				{
					this.searchCompleteEvent.Set();
					if (this.userCallback != null)
					{
						this.userCallback(this.asyncResult);
					}
				}, delegate(Exception e)
				{
					if (e != null)
					{
						SearchMailboxExecuter.Tracer.TraceError<Exception>((long)this.GetHashCode(), "UserCallBack in search throws {0}", e);
					}
					if (streamLogItem != null)
					{
						streamLogItem.Dispose();
					}
				});
			});
		}

		private void ProcessCommunicatorEvents(SearchMailboxExecuter.ThreadThrottler threadThrottler, ref double searchProgress, ref int progressingWorkers, bool isAborted)
		{
			List<Pair<int, Exception>> list = new List<Pair<int, Exception>>();
			List<StreamLogItem.LogItem> logList = new List<StreamLogItem.LogItem>();
			List<SearchMailboxWorker> list2 = new List<SearchMailboxWorker>();
			double num = 0.0;
			lock (this.searchCommunicator)
			{
				num = this.searchCommunicator.OverallProgress;
				progressingWorkers = this.searchCommunicator.ProgressingWorkers;
				long overallResultItems = this.searchCommunicator.OverallResultItems;
				ByteQuantifiedSize overallResultSize = this.searchCommunicator.OverallResultSize;
				if (this.searchCommunicator.WorkerExceptions.Count > 0)
				{
					list.AddRange(this.searchCommunicator.WorkerExceptions);
					this.searchCommunicator.WorkerExceptions.Clear();
					List<int> list3 = new List<int>();
					List<Pair<int, Exception>> list4 = new List<Pair<int, Exception>>();
					foreach (Pair<int, Exception> pair in list)
					{
						SearchMailboxWorker searchMailboxWorker = this.searchMailboxWorkers[pair.First];
						Exception second = pair.Second;
						bool flag2 = false;
						if (list3.Contains(pair.First) || threadThrottler.WorkerQueue.Contains(searchMailboxWorker))
						{
							flag2 = true;
							list4.Add(pair);
						}
						if (!this.EstimationPhase && !this.searchMailboxCriteria.ExcludePurgesFromDumpster && !flag2 && this.IsRetryableException(second) && searchMailboxWorker.NumberOfRetry < this.retryThreshold)
						{
							list3.Add(pair.First);
							list4.Add(pair);
							if (!this.SearchMailboxCriteria.ExcludeDuplicateMessages || searchMailboxWorker.HasPendingHashSetVerification)
							{
								using (MailboxSession mailboxSession = SearchUtils.OpenMailboxSession(this.targetUser as ADUser, this.OwnerIdentity))
								{
									if (!this.SearchMailboxCriteria.ExcludeDuplicateMessages)
									{
										searchMailboxWorker.DeleteMailboxSearchResultFolder(mailboxSession);
									}
									else if (searchMailboxWorker.HasPendingHashSetVerification)
									{
										SearchMailboxExecuter.Tracer.TraceDebug<int, int>((long)this.GetHashCode(), "ProcessCommunicatorEvents -> Before rebuild hash set, ProcessedMessages={0}, ProcessedMessageIds={1}", this.searchCommunicator.ProcessedMessages.Count, this.searchCommunicator.ProcessedMessageIds.Count);
										this.BuildUniqueMessageHashSetBeforeRetry(mailboxSession, searchMailboxWorker.TargetFolderId);
										SearchMailboxExecuter.Tracer.TraceDebug<int, int>((long)this.GetHashCode(), "ProcessCommunicatorEvents -> After rebuild hash set, ProcessedMessages={0}, ProcessedMessageIds={1}", this.searchCommunicator.ProcessedMessages.Count, this.searchCommunicator.ProcessedMessageIds.Count);
										searchMailboxWorker.HasPendingHashSetVerification = false;
									}
								}
							}
							searchMailboxWorker.NumberOfRetry++;
							searchMailboxWorker.RunnableTime = this.GetWorkerRunnableTimeAfterRetry(searchMailboxWorker.NumberOfRetry);
							threadThrottler.WorkerQueue.Add(searchMailboxWorker);
							this.searchCommunicator.ResetWorker(searchMailboxWorker, false);
							num = this.searchCommunicator.OverallProgress;
							progressingWorkers = this.searchCommunicator.ProgressingWorkers;
							overallResultItems = this.searchCommunicator.OverallResultItems;
							overallResultSize = this.searchCommunicator.OverallResultSize;
						}
					}
					if (list4.Count > 0)
					{
						foreach (Pair<int, Exception> item in list4)
						{
							list.Remove(item);
						}
					}
				}
				if (this.searchCommunicator.WorkerLogs.Count > 0)
				{
					logList.AddRange(this.searchCommunicator.WorkerLogs);
					this.searchCommunicator.WorkerLogs.Clear();
				}
				if (this.searchCommunicator.CompletedWorkers.Count > 0)
				{
					list2.AddRange(this.searchCommunicator.CompletedWorkers);
					this.searchCommunicator.CompletedWorkers.Clear();
					if (this.LogLevel != LoggingLevel.Suppress)
					{
						using (MailboxSession targetMailbox = (!this.EstimationPhase) ? SearchUtils.OpenMailboxSession(this.targetUser as ADUser, this.OwnerIdentity) : null)
						{
							list2.ForEach(delegate(SearchMailboxWorker x)
							{
								bool flag3 = x.LastException == null;
								if (this.SearchMailboxCriteria.ExcludeDuplicateMessages || this.searchMailboxCriteria.ExcludePurgesFromDumpster)
								{
									flag3 = true;
								}
								if (!flag3)
								{
									x.DeleteMailboxSearchResultFolder(targetMailbox);
									if (x.NumberOfRetry >= this.retryThreshold)
									{
										this.searchCommunicator.ResetWorker(x, true);
									}
								}
								if (this.StreamLogItem != null)
								{
									this.FlushLogs(logList);
									try
									{
										this.StreamLogItem.ConsolidateLog(x.WorkerId, flag3);
									}
									catch (ArgumentException innerException)
									{
										this.SearchState = SearchState.Failed;
										this.FireExceptionEvent(new SearchLogFileCreateException(innerException));
									}
									catch (StoragePermanentException innerException2)
									{
										this.SearchState = SearchState.Failed;
										this.FireExceptionEvent(new SearchLogFileCreateException(innerException2));
									}
									catch (StorageTransientException innerException3)
									{
										this.SearchState = SearchState.Failed;
										this.FireExceptionEvent(new SearchLogFileCreateException(innerException3));
									}
								}
							});
						}
					}
				}
				if (searchProgress != num)
				{
					this.FireProgressEvent(Strings.ProgressSearching, Strings.ProgressSearchingInProgress, (int)num, overallResultItems, overallResultSize);
					searchProgress = num;
				}
			}
			if (!isAborted && list2.Count > 0)
			{
				list2.ForEach(delegate(SearchMailboxWorker x)
				{
					threadThrottler.OnEndWorker(x);
				});
			}
			if (threadThrottler.WorkerQueue.Count > 0)
			{
				int num2 = progressingWorkers - threadThrottler.WorkerQueue.Count;
				if (num2 < this.ThreadLimit)
				{
					threadThrottler.PumpWorkerThreads(this.ThreadLimit - num2);
				}
			}
			if (list.Count > 0)
			{
				foreach (Pair<int, Exception> pair2 in list)
				{
					this.FireExceptionEvent(this.searchMailboxWorkers[pair2.First], pair2.Second);
				}
				list.Clear();
			}
			this.FlushLogs(logList);
		}

		private void FlushLogs(List<StreamLogItem.LogItem> logList)
		{
			if (logList.Count > 0)
			{
				foreach (StreamLogItem.LogItem logItem in logList)
				{
					logItem.Logs.ForEach(delegate(LocalizedString x)
					{
						this.FireLoggingEvent(x);
					});
				}
				if (this.StreamLogItem != null)
				{
					this.StreamLogItem.WriteLogs(logList);
				}
				logList.Clear();
			}
		}

		private StoreId GetSubFolderId(MailboxSession mailboxSession, StoreId targetFolderId)
		{
			using (Folder folder = Folder.Bind(mailboxSession, targetFolderId))
			{
				List<Pair<StoreId, string>> subFoldersWithIdAndName = folder.GetSubFoldersWithIdAndName();
				if (subFoldersWithIdAndName != null && subFoldersWithIdAndName.Count > 0)
				{
					return subFoldersWithIdAndName[0].First;
				}
			}
			return null;
		}

		private string GetUniqueSubFolderName(int index, string[] uniqueSubFolderNames, MailboxSession mailboxSession, StoreId targetFolderId)
		{
			if (this.SearchMailboxCriteria.ExcludeDuplicateMessages)
			{
				string arg = uniqueSubFolderNames[0];
				if (mailboxSession != null && targetFolderId != null && this.ResumeSearch)
				{
					using (Folder folder = Folder.Bind(mailboxSession, targetFolderId))
					{
						List<Pair<StoreId, string>> subFoldersWithIdAndName = folder.GetSubFoldersWithIdAndName();
						if (subFoldersWithIdAndName != null && subFoldersWithIdAndName.Count > 0)
						{
							return subFoldersWithIdAndName[0].Second;
						}
					}
				}
				return string.Format("{0}-{1}", arg, this.SearchStartTime);
			}
			return string.Format("{0}-{1}", uniqueSubFolderNames[index], this.SearchStartTime);
		}

		private string[] CreateUniqueSubFolderNames(SearchUser[] searchUserScope)
		{
			if (this.SearchMailboxCriteria.ExcludeDuplicateMessages)
			{
				return new string[]
				{
					Strings.TargetFolder
				};
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			string[] array = new string[searchUserScope.Length];
			for (int i = 0; i < searchUserScope.Length; i++)
			{
				if (!hashSet.Contains(searchUserScope[i].DisplayName))
				{
					array[i] = searchUserScope[i].DisplayName;
				}
				else
				{
					string text = string.Format("{0}({1})", searchUserScope[i].DisplayName, searchUserScope[i].Id.DomainId.Name);
					if (!hashSet.Contains(text))
					{
						array[i] = text;
					}
					else
					{
						array[i] = string.Format("{0}({1})", searchUserScope[i].DisplayName, searchUserScope[i].Id.DistinguishedName);
					}
				}
				hashSet.Add(array[i]);
			}
			return array;
		}

		private void InternalSearchMainEntry()
		{
			try
			{
				MailboxDataProvider.IncrementDiscoveryMailboxSearchPerfCounters(this.SearchMailboxCriteria.SearchUserScope.Length);
				this.FireProgressEvent(Strings.ProgressOpening, Strings.ProgressOpeningTarget, 0, 0L, ByteQuantifiedSize.Zero);
				using (Referenced<MailboxSession> referenced = this.EstimationPhase ? null : Referenced<MailboxSession>.Acquire(SearchUtils.OpenMailboxSession(this.targetUser as ADUser, this.OwnerIdentity)))
				{
					StoreId storeId = null;
					if (!this.EstimationPhase)
					{
						if (referenced == null)
						{
							return;
						}
						storeId = this.GetTargetFolderId(referenced);
					}
					if (storeId != null && this.LogLevel != LoggingLevel.Suppress)
					{
						this.StreamLogItem = new StreamLogItem(referenced, this.LogMessageId, storeId, string.Format("{0}-{1}", this.Name ?? "Search Results", this.SearchStartTime), this.Name ?? "Search Results");
						this.FireRequestLogBodyEvent(this.StreamLogItem.MessageItem.Body);
						this.StreamLogItem.Save(true);
					}
					string[] uniqueSubFolderNames = this.CreateUniqueSubFolderNames(this.SearchMailboxCriteria.SearchUserScope);
					MailboxSession mailboxSession = (referenced == null) ? null : referenced.Value;
					if (!this.EstimationPhase && this.ResumeSearch && this.SearchMailboxCriteria.ExcludeDuplicateMessages)
					{
						this.BuildUniqueMessageHashSetFromExistingSearchFolder(mailboxSession, storeId);
					}
					for (int i = 0; i < this.searchMailboxCriteria.SearchUserScope.Length; i++)
					{
						SearchMailboxWorker searchMailboxWorker = new SearchMailboxWorker(this.searchMailboxCriteria, i);
						searchMailboxWorker.SubfolderName = this.GetUniqueSubFolderName(i, uniqueSubFolderNames, mailboxSession, storeId);
						searchMailboxWorker.OwnerIdentity = this.OwnerIdentity;
						searchMailboxWorker.RecipientSession = this.recipientSession;
						searchMailboxWorker.TargetUser = (this.EstimationPhase ? null : this.TargetUser);
						searchMailboxWorker.TargetFolderId = storeId;
						searchMailboxWorker.SearchCommunicator = this.searchCommunicator;
						searchMailboxWorker.LoggingLevel = this.LogLevel;
						searchMailboxWorker.RunnableTime = ExDateTime.Now;
						searchMailboxWorker.SearchObject = this.SearchObject;
						searchMailboxWorker.ExecutingUserIdentity = this.ExecutingUserIdentity;
						ADUser aduser = this.targetUser as ADUser;
						if (aduser != null)
						{
							searchMailboxWorker.TargetMailboxQuota = aduser.ProhibitSendReceiveQuota;
						}
						else
						{
							searchMailboxWorker.TargetMailboxQuota = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
						}
						if (this.searchActions != null && this.searchActions.Count > 0)
						{
							searchMailboxWorker.SearchActions = this.searchActions;
						}
						if (this.ResumeSearch && !this.SearchMailboxCriteria.ExcludeDuplicateMessages)
						{
							searchMailboxWorker.DeleteMailboxSearchResultFolder(mailboxSession);
						}
						this.searchMailboxWorkers.Add(searchMailboxWorker);
					}
					SearchMailboxExecuter.ThreadThrottler threadThrottler = new SearchMailboxExecuter.ThreadThrottler(this, this.searchMailboxWorkers);
					threadThrottler.PumpWorkerThreads(-1);
					this.FireProgressEvent(Strings.ProgressSearching, Strings.ProgressSearchingSources, 0, 0L, ByteQuantifiedSize.Zero);
					double num = 0.0;
					int count = this.searchMailboxWorkers.Count;
					WaitHandle[] waitHandles = new WaitHandle[]
					{
						this.searchCommunicator.AbortEvent,
						this.searchCommunicator.ProgressEvent
					};
					bool flag = count <= 0;
					while (!flag)
					{
						int num2 = WaitHandle.WaitAny(waitHandles, 5000);
						if (num2 == 0)
						{
							break;
						}
						this.ProcessCommunicatorEvents(threadThrottler, ref num, ref count, false);
						if (count == 0)
						{
							flag = true;
						}
					}
					if (!this.EstimationPhase && this.SearchMailboxCriteria.ExcludeDuplicateMessages)
					{
						StoreId subFolderId = this.GetSubFolderId(referenced, storeId);
						if (subFolderId != null)
						{
							this.DedupeAndCleanupMessageProperties(referenced, subFolderId);
							this.searchCommunicator.UpdateResults(referenced, subFolderId);
						}
					}
					while (count > threadThrottler.WorkerQueue.Count && this.WaitForWorkersWhenAborted)
					{
						this.searchCommunicator.ProgressEvent.WaitOne();
						this.ProcessCommunicatorEvents(threadThrottler, ref num, ref count, true);
					}
					if (flag)
					{
						this.PostProcessSearch();
						this.FireProgressEvent(Strings.ProgressCompleting, Strings.ProgressCompletingSearch, 100, this.searchCommunicator.OverallResultItems, this.searchCommunicator.OverallResultSize);
					}
				}
			}
			finally
			{
				if (this.searchState == SearchState.InProgress)
				{
					this.searchState = SearchState.Failed;
				}
				else if (this.searchState == SearchState.EstimateInProgress)
				{
					this.searchState = SearchState.EstimateFailed;
				}
				if (this.StreamLogItem != null)
				{
					this.StreamLogItem.CloseOpenedStream();
					this.FireRequestLogBodyEvent(this.StreamLogItem.MessageItem.Body);
					this.StreamLogItem.Save(true);
				}
				MailboxDataProvider.DecrementDiscoveryMailboxSearchPerfCounters(this.SearchMailboxCriteria.SearchUserScope.Length);
			}
		}

		private void DedupeAndCleanupMessageProperties(Referenced<MailboxSession> targetMailbox, StoreId targetFolderId)
		{
			using (Folder folder = Folder.Bind(targetMailbox, targetFolderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, new SortBy[]
				{
					new SortBy(ConversationItemSchema.ConversationId, SortOrder.Ascending)
				}, new PropertyDefinition[]
				{
					ConversationItemSchema.ConversationId,
					ItemSchema.Id
				}))
				{
					HashSet<StoreId> bccItemIds = SearchUtils.GetBccItemIds(folder);
					for (;;)
					{
						object[][] rows = queryResult.GetRows(ResponseThrottler.MaxBulkSize);
						if (rows == null || rows.Length <= 0)
						{
							break;
						}
						List<StoreObjectId> list = new List<StoreObjectId>();
						for (int i = 0; i < rows.Length; i++)
						{
							if (rows[i][0] != null && !(rows[i][0] is PropertyError))
							{
								ConversationId conversationId = (ConversationId)rows[i][0];
								StoreId storeId = (StoreId)rows[i][1];
								StoreObjectId storeObjectId = StoreId.GetStoreObjectId(storeId);
								Conversation conversation = Conversation.Load(targetMailbox, conversationId, null, true, new PropertyDefinition[0]);
								if (conversation != null)
								{
									IConversationTreeNode conversationTreeNode = null;
									bool flag = conversation.ConversationTree.TryGetConversationTreeNode(storeObjectId, out conversationTreeNode);
									if (flag)
									{
										bool flag2 = !conversationTreeNode.HasChildren || conversation.ConversationNodeContainedInChildren(conversationTreeNode);
										bool flag3 = bccItemIds.Contains(storeId);
										if (conversationTreeNode.HasChildren && flag2 && !flag3)
										{
											list.Add(storeObjectId);
										}
									}
								}
							}
						}
						folder.DeleteObjects(DeleteItemFlags.HardDelete, list.ToArray());
					}
				}
			}
			using (Folder folder2 = Folder.Bind(targetMailbox, targetFolderId))
			{
				using (QueryResult queryResult2 = folder2.ItemQuery(ItemQueryType.None, null, new SortBy[]
				{
					new SortBy(ItemSchema.ReceivedTime, SortOrder.Ascending)
				}, new PropertyDefinition[]
				{
					ItemSchema.Id
				}))
				{
					folder2.MarkAllAsRead(true);
					for (;;)
					{
						object[][] rows2 = queryResult2.GetRows(ResponseThrottler.MaxBulkSize);
						if (rows2 == null || rows2.Length <= 0)
						{
							break;
						}
						for (int j = 0; j < rows2.Length; j++)
						{
							if (rows2[j][0] != null && !(rows2[j][0] is PropertyError))
							{
								StoreId storeId2 = (StoreId)rows2[j][0];
								using (Item item = Item.Bind(targetMailbox, storeId2))
								{
									item.SetOrDeleteProperty(ItemSchema.ConversationIndexTracking, null);
									item.SetOrDeleteProperty(ItemSchema.Importance, null);
									item.Categories.Clear();
									if (!(item is CalendarItemBase) && !(item is Task))
									{
										item.ClearFlag();
									}
									item.Save(SaveMode.ResolveConflicts);
								}
							}
						}
					}
				}
			}
		}

		internal void BuildUniqueMessageHashSetFromExistingSearchFolder(MailboxSession mailboxSession, StoreId targetFolderId)
		{
			this.searchCommunicator.ProcessedMessages.Clear();
			this.searchCommunicator.ProcessedMessageIds.Clear();
			if (targetFolderId == null)
			{
				return;
			}
			using (Folder folder = Folder.Bind(mailboxSession, targetFolderId))
			{
				List<Pair<StoreId, string>> subFoldersWithIdAndName = folder.GetSubFoldersWithIdAndName();
				if (subFoldersWithIdAndName != null && subFoldersWithIdAndName.Count != 0)
				{
					StoreId first = subFoldersWithIdAndName[0].First;
					using (Folder folder2 = Folder.Bind(mailboxSession, first))
					{
						using (QueryResult queryResult = folder2.ItemQuery(ItemQueryType.None, null, null, (this.LogLevel == LoggingLevel.Full) ? SearchMailboxWorker.ItemPreloadPropertiesWithLogging : SearchMailboxWorker.ItemPreloadProperties))
						{
							StoreId[] array = new StoreId[ResponseThrottler.MaxBulkSize];
							ResponseThrottler responseThrottler = new ResponseThrottler(this.searchCommunicator.AbortEvent);
							while (!this.searchCommunicator.IsAborted)
							{
								responseThrottler.BackOffFromStore(mailboxSession);
								object[][] rows = queryResult.GetRows(array.Length);
								if (rows == null || rows.Length <= 0)
								{
									break;
								}
								for (int i = 0; i < rows.Length; i++)
								{
									StoreObjectId storeObjectId = null;
									UniqueItemHash uniqueItemHash = SearchMailboxWorker.BuildUniqueItemHash(mailboxSession, rows[i], null, out storeObjectId);
									if (uniqueItemHash != null)
									{
										if (!this.searchCommunicator.ProcessedMessages.Contains(uniqueItemHash))
										{
											this.searchCommunicator.ProcessedMessages.Add(uniqueItemHash);
										}
									}
									else
									{
										string internetMessageId = SearchMailboxWorker.GetInternetMessageId(rows[i]);
										if (internetMessageId != null && !this.searchCommunicator.ProcessedMessageIds.Contains(internetMessageId))
										{
											this.searchCommunicator.ProcessedMessageIds.Add(internetMessageId);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		internal void BuildUniqueMessageHashSetBeforeRetry(MailboxSession mailboxSession, StoreId targetFolderId)
		{
			lock (mailboxSession)
			{
				this.BuildUniqueMessageHashSetFromExistingSearchFolder(mailboxSession, targetFolderId);
			}
		}

		private bool IsRetryableException(Exception ex)
		{
			return ex is StorageTransientException || (ex is SearchMailboxException && ex.InnerException != null && ex.InnerException is PartialCompletionException);
		}

		private ExDateTime GetWorkerRunnableTimeAfterRetry(int retry)
		{
			if (retry <= 0)
			{
				return ExDateTime.Now;
			}
			int num = this.retryWaitInterval;
			for (int i = 2; i <= retry; i++)
			{
				num *= this.retryWaitFactor;
			}
			return ExDateTime.Now.AddMilliseconds((double)num);
		}

		internal static int GetSettingsValue(string regKey, int defaultValue)
		{
			int num = 0;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(SearchMailboxExecuter.discoveryRegistryPath))
			{
				if (registryKey != null && registryKey.GetValue(regKey) != null)
				{
					object value = registryKey.GetValue(regKey);
					if (value != null && value is int)
					{
						num = (int)value;
					}
				}
			}
			if (num <= 0)
			{
				return defaultValue;
			}
			return num;
		}

		private const int WaitEventTimeout = 5000;

		protected static readonly Trace Tracer = ExTraceGlobals.SearchTracer;

		private readonly ManualResetEvent searchCompleteEvent = new ManualResetEvent(false);

		private IRecipientSession recipientSession;

		private IConfigurationSession configurationSession;

		private List<SearchMailboxWorker> searchMailboxWorkers = new List<SearchMailboxWorker>();

		private SearchCommunicator searchCommunicator;

		private readonly int threadLimit;

		private readonly int threadLimitPerServer;

		private object state;

		private AsyncCallback userCallback;

		private SearchMailboxExecuter.SearchMailboxAsyncResult asyncResult;

		private bool waitForWorkersWhenAborted;

		private GenericIdentity ownerIdentity;

		private SearchMailboxCriteria searchMailboxCriteria;

		private List<SearchMailboxAction> searchActions;

		private ADRecipient targetUser;

		private string targetFolder;

		private ADRecipient[] reviewRecipients;

		private LoggingLevel logLevel;

		private SearchState searchState = SearchState.EstimateInProgress;

		private ExDateTime? searchStartTime;

		private EventHandler<SearchProgressEvent> progressHandler;

		private EventHandler<SearchLoggingEvent> loggingHandler;

		private EventHandler<SearchExceptionEvent> searchExceptionHandler;

		private EventHandler<RequestLogBodyEvent> requestBodyHandler;

		private static string discoveryRegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Discovery";

		private int retryThreshold = SearchMailboxExecuter.GetSettingsValue("RetryThreshold", 5);

		private int retryWaitInterval = SearchMailboxExecuter.GetSettingsValue("RetryWaitInterval", 5000);

		private int retryWaitFactor = SearchMailboxExecuter.GetSettingsValue("RetryWaitFactor", 2);

		internal class ThreadThrottler
		{
			internal ThreadThrottler(SearchMailboxExecuter searchExecuter, IEnumerable<SearchMailboxWorker> searchWorkers)
			{
				if (searchExecuter == null)
				{
					throw new ArgumentNullException("searchExecuter");
				}
				if (searchWorkers == null)
				{
					throw new ArgumentNullException("searchWorkers");
				}
				this.searchExecuter = searchExecuter;
				IEnumerable<IGrouping<string, SearchMailboxWorker>> source = searchWorkers.GroupBy((SearchMailboxWorker x) => x.SourceUser.ServerName, StringComparer.OrdinalIgnoreCase);
				this.serverThreadMap = source.ToDictionary((IGrouping<string, SearchMailboxWorker> x) => x.Key, (IGrouping<string, SearchMailboxWorker> x) => 0, StringComparer.OrdinalIgnoreCase);
				List<List<SearchMailboxWorker>> list = (from x in source
				select x.ToList<SearchMailboxWorker>()).ToList<List<SearchMailboxWorker>>();
				this.searchWorkerQueue = new List<SearchMailboxWorker>();
				while (list.Count > 0)
				{
					for (int i = list.Count - 1; i >= 0; i--)
					{
						this.searchWorkerQueue.Add(list[i][0]);
						list[i].RemoveAt(0);
						if (list[i].Count <= 0)
						{
							list.RemoveAt(i);
						}
					}
				}
			}

			internal List<SearchMailboxWorker> WorkerQueue
			{
				get
				{
					return this.searchWorkerQueue;
				}
			}

			internal void PumpWorkerThreads(int maxWorkers)
			{
				if (maxWorkers == -1)
				{
					int num;
					ThreadPool.GetAvailableThreads(out maxWorkers, out num);
					maxWorkers /= 2;
					maxWorkers = Math.Min(maxWorkers, this.searchExecuter.ThreadLimit);
					if (maxWorkers <= 0)
					{
						throw new ResourcesException(Strings.LowSystemResource);
					}
				}
				int num2 = this.searchWorkerQueue.Count - 1;
				while (num2 >= 0 && maxWorkers > 0)
				{
					SearchMailboxWorker searchMailboxWorker = this.searchWorkerQueue[num2];
					if (this.serverThreadMap[searchMailboxWorker.SourceUser.ServerName] < this.searchExecuter.ThreadLimitPerServer && ExDateTime.Compare(ExDateTime.Now, searchMailboxWorker.RunnableTime) >= 0)
					{
						this.searchWorkerQueue.RemoveAt(num2);
						this.OnBeginWorker(searchMailboxWorker);
						maxWorkers--;
					}
					num2--;
				}
			}

			internal void OnBeginWorker(SearchMailboxWorker worker)
			{
				Dictionary<string, int> dictionary;
				string serverName;
				(dictionary = this.serverThreadMap)[serverName = worker.SourceUser.ServerName] = dictionary[serverName] + 1;
				ThreadPool.QueueUserWorkItem(new WaitCallback(worker.ProcessMailbox));
			}

			internal void OnEndWorker(SearchMailboxWorker worker)
			{
				Dictionary<string, int> dictionary;
				string serverName;
				(dictionary = this.serverThreadMap)[serverName = worker.SourceUser.ServerName] = dictionary[serverName] - 1;
			}

			private SearchMailboxExecuter searchExecuter;

			private List<SearchMailboxWorker> searchWorkerQueue;

			private Dictionary<string, int> serverThreadMap;
		}

		private class SearchMailboxAsyncResult : IAsyncResult
		{
			internal SearchMailboxAsyncResult(SearchMailboxExecuter searchMailboxExecuter)
			{
				this.searchMailboxExecuter = searchMailboxExecuter;
			}

			public object AsyncState
			{
				get
				{
					return this.searchMailboxExecuter.state;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return this.searchMailboxExecuter.searchCompleteEvent;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return this.searchMailboxExecuter.searchCompleteEvent.WaitOne(0, false);
				}
			}

			private SearchMailboxExecuter searchMailboxExecuter;
		}
	}
}
