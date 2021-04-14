using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.EDiscovery.Export;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.InfoWorker.Common.SearchService;
using Microsoft.Exchange.Rpc.MailboxSearch;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class MailboxSearchWorkItem
	{
		internal MailboxSearchWorkItem(MailboxSearchServer server, SearchId searchId) : this(server, searchId, null)
		{
		}

		internal MailboxSearchWorkItem(MailboxSearchServer server, SearchId searchId, IMailboxSearchConfigurationProvider mailboxSearchConfiguration)
		{
			Util.ThrowIfNull(searchId, "searchId");
			this.server = server;
			this.SearchId = searchId;
			this.mailboxSearchConfiguration = mailboxSearchConfiguration;
			if (this.mailboxSearchConfiguration == null)
			{
				ADObjectId discoverySystemMailboxId = new ADObjectId(searchId.MailboxDsName, searchId.MailboxGuid);
				this.mailboxSearchConfiguration = new MailboxSearchConfigurationProvider(discoverySystemMailboxId, searchId.SearchName);
			}
		}

		internal SearchId SearchId { get; private set; }

		internal WorkItemAction Action { get; private set; }

		internal bool IsEstimateOnly
		{
			get
			{
				return this.mailboxSearchConfiguration.SearchObject.StatisticsOnly;
			}
		}

		internal bool IsSearchTaskCreated
		{
			get
			{
				return this.searchTask != null;
			}
		}

		internal bool IsCompleted
		{
			get
			{
				return this.WaitHandle.WaitOne(0, false);
			}
		}

		internal WaitHandle WaitHandle
		{
			get
			{
				return this.workItemCompletedEvent;
			}
		}

		internal void PreStart(string executingUserId, IMailboxSearchTask searchTask)
		{
			this.Action = WorkItemAction.Start;
			DateTime dateTime = (DateTime)ExDateTime.UtcNow;
			this.mailboxSearchConfiguration.ExecutingUserId = executingUserId;
			MailboxDiscoverySearch searchObject = this.mailboxSearchConfiguration.SearchObject;
			IDictionary<Uri, string> dictionary = new Dictionary<Uri, string>();
			VariantConfigurationSnapshot variantConfigurationSnapshot = this.UpdateFlightedFeatures(searchObject, false);
			bool flag = searchObject.IsFeatureFlighted("PublicFolderSearchFlighted");
			bool isDocIdHintFlightingEnabled = searchObject.IsFeatureFlighted("DocIdHintFlighted");
			Uri uri = null;
			ADUser discoverySystemMailboxUser = this.mailboxSearchConfiguration.DiscoverySystemMailboxUser;
			if (discoverySystemMailboxUser != null)
			{
				uri = BackEndLocator.GetBackEndWebServicesUrl(discoverySystemMailboxUser);
				if (variantConfigurationSnapshot != null)
				{
					ConstantProvider.RebindWithAutoDiscoveryEnabled = variantConfigurationSnapshot.Discovery.UrlRebind.Enabled;
					ConstantProvider.RebindAutoDiscoveryInternalUrlOnly = true;
					ConstantProvider.RebindAutoDiscoveryUrl = LinkUtils.AppendRelativePath(uri, "/autodiscover/autodiscover.svc", false);
				}
			}
			this.searchTask = searchTask;
			if (this.searchTask == null)
			{
				MailboxExportMetadata exportMetadata = new MailboxExportMetadata(searchObject.Identity.ToString(), searchObject.Name, !searchObject.ExcludeDuplicateMessages, searchObject.IncludeUnsearchableItems, dateTime, searchObject.Language);
				List<ISource> list = null;
				if (!searchObject.StatisticsOnly || !flag)
				{
					if (searchObject.Version != SearchObjectVersion.Original && flag)
					{
						list = (List<ISource>)this.mailboxSearchConfiguration.GetFinalSources(searchObject.Name, searchObject.CalculatedQuery, this.mailboxSearchConfiguration.ExecutingUserPrimarySmtpAddress, uri);
						searchObject.NumberOfMailboxes = list.Count;
					}
					else
					{
						list = (List<ISource>)this.mailboxSearchConfiguration.ValidateAndGetFinalSourceMailboxes(searchObject.CalculatedQuery, searchObject.Sources, this.notFoundMailboxes, this.versionSkippedMailboxes, this.rbacDeniedMailboxes, this.crossPremiseFailedMailboxes, dictionary);
						searchObject.NumberOfMailboxes = list.Count;
					}
				}
				else
				{
					searchObject.NumberOfMailboxes = 0;
				}
				if (flag || searchObject.NumberOfMailboxes > 0)
				{
					ITargetLocation targetLocation = new CopyTargetLocation(searchObject.Name, searchObject.Name + Constants.WorkingFolderSuffix);
					IExportContext exportContext = new MailboxExportContext(exportMetadata, list, targetLocation, searchObject.Resume, new Action<IEnumerable<ErrorRecord>>(this.WriteErrorLogEventHandler), new Action<IEnumerable<ExportRecord>>(this.WriteResultManifestEventHandler));
					if (searchObject.StatisticsOnly)
					{
						if (null != uri)
						{
							this.estimateEwsClient = new EwsClient(uri, new ServerToServerEwsCallingContext(dictionary));
						}
						else
						{
							this.estimateEwsClient = null;
						}
						string keywordStatisticsQuery = searchObject.IncludeKeywordStatistics ? searchObject.KeywordsQuery : null;
						this.searchTask = new MailboxSearchTask(this.estimateEwsClient, keywordStatisticsQuery, searchObject.UserKeywords, this.mailboxSearchConfiguration.RecipientSession, exportContext, this.mailboxSearchConfiguration.ExecutingUserPrimarySmtpAddress, (int)SearchUtils.GetDiscoveryMaxMailboxesForStatsSearch(this.mailboxSearchConfiguration.RecipientSession), flag, searchObject, this.mailboxSearchConfiguration.SearchDataProvider.OrganizationId);
					}
					else if (searchObject.NumberOfMailboxes > 0)
					{
						ITargetMailbox targetMailbox = Util.CreateTargetMailbox(this.mailboxSearchConfiguration.RecipientSession, searchObject, exportContext);
						this.searchTask = new MailboxSearchTask(targetMailbox, new ServerToServerCallingContextFactory(dictionary), this.mailboxSearchConfiguration.ExecutingUserPrimarySmtpAddress, this.mailboxSearchConfiguration.SearchDataProvider.OrganizationId, isDocIdHintFlightingEnabled);
					}
					else
					{
						Util.Tracer.TraceDebug((long)this.GetHashCode(), "Search not supported. Expanded NumberOfMailboxes == 0 and not a StatisticsOnly search");
					}
				}
			}
			else
			{
				List<ISource> list2 = (List<ISource>)this.mailboxSearchConfiguration.ValidateAndGetFinalSourceMailboxes(searchObject.CalculatedQuery, searchObject.Sources, this.notFoundMailboxes, this.versionSkippedMailboxes, this.rbacDeniedMailboxes, this.crossPremiseFailedMailboxes, dictionary);
				searchObject.NumberOfMailboxes = list2.Count;
			}
			if (this.searchTask != null)
			{
				this.exportContext = this.searchTask.ExportContext;
			}
			if (searchObject.Status == SearchState.Failed)
			{
				searchObject.UpdateState(SearchStateTransition.StartSearch);
			}
			this.mailboxSearchConfiguration.ResetSearchObject("PreStart", 437);
			this.noMailboxesToSearch = (searchObject.NumberOfMailboxes == 0 && !flag);
			searchObject.UpdateState(SearchStateTransition.MoveToNextState);
			if (this.noMailboxesToSearch)
			{
				lock (searchObject)
				{
					this.mailboxSearchConfiguration.UpdateSearchObject("PreStart", 449);
					goto IL_627;
				}
			}
			IMailboxSearchTask mailboxSearchTask = this.searchTask;
			mailboxSearchTask.OnReportStatistics = (EventHandler<ExportStatusEventArgs>)Delegate.Combine(mailboxSearchTask.OnReportStatistics, new EventHandler<ExportStatusEventArgs>(this.ExportStatusReportingEventHandler));
			IMailboxSearchTask mailboxSearchTask2 = this.searchTask;
			mailboxSearchTask2.OnEstimateCompleted = (Action<int, long, long, long, List<KeywordHit>>)Delegate.Combine(mailboxSearchTask2.OnEstimateCompleted, new Action<int, long, long, long, List<KeywordHit>>(this.OnEstimateCompleted));
			IMailboxSearchTask mailboxSearchTask3 = this.searchTask;
			mailboxSearchTask3.OnPrepareCompleted = (Action<ISearchResults>)Delegate.Combine(mailboxSearchTask3.OnPrepareCompleted, new Action<ISearchResults>(this.OnPrepareCompleted));
			IMailboxSearchTask mailboxSearchTask4 = this.searchTask;
			mailboxSearchTask4.OnExportCompleted = (Action)Delegate.Combine(mailboxSearchTask4.OnExportCompleted, new Action(this.OnSearchCompleted));
			searchObject.LastModifiedBy = this.mailboxSearchConfiguration.GetExecutingUserName();
			searchObject.LastStartTime = dateTime;
			if (!this.IsEstimateOnly)
			{
				searchObject.ResultsPath = searchObject.Target + Constants.ResultPathSeparator + searchObject.Name;
				searchObject.ResultsLink = this.mailboxSearchConfiguration.GenerateOWASearchResultsLink();
			}
			if (this.mailboxSearchConfiguration.IsPreviewAllowed())
			{
				searchObject.PreviewDisabled = false;
				searchObject.PreviewResultsLink = this.mailboxSearchConfiguration.GenerateOWAPreviewResultsLink();
			}
			else
			{
				searchObject.PreviewDisabled = true;
				string item = Strings.PreviewSearchDisabled((int)SearchUtils.GetDiscoveryMaxMailboxesForPreviewSearch(this.mailboxSearchConfiguration.RecipientSession), (int)this.mailboxSearchConfiguration.MaxMailboxesToSearch);
				if (!searchObject.Information.Contains(item))
				{
					searchObject.Information.Add(item);
				}
			}
			searchObject.KeywordStatisticsDisabled = false;
			if (!this.mailboxSearchConfiguration.IsKeywordStatsAllowed())
			{
				Util.Tracer.TraceDebug<int, uint>((long)this.GetHashCode(), "The keyword statistics search is disabled because number of mailboxes being searched ({0}) is greater than the allowed limit ({1}).", searchObject.NumberOfMailboxes, this.mailboxSearchConfiguration.MaxNumberOfMailboxesForKeywordStatistics);
				searchObject.KeywordStatisticsDisabled = true;
				string item2 = Strings.KeywordStatisticsSearchDisabled((int)this.mailboxSearchConfiguration.MaxNumberOfMailboxesForKeywordStatistics);
				if (!searchObject.Information.Contains(item2))
				{
					searchObject.Information.Add(item2);
				}
			}
			this.mailboxSearchConfiguration.UpdateSearchObject("PreStart", 509);
			try
			{
				this.mailboxSearchConfiguration.CheckDiscoveryBudget(this.IsEstimateOnly, this.server);
			}
			catch (LocalizedException ex)
			{
				lock (searchObject)
				{
					if (!searchObject.Errors.Contains(ex.Message))
					{
						searchObject.Errors.Add(ex.Message);
					}
					searchObject.UpdateState(SearchStateTransition.Fail);
					this.mailboxSearchConfiguration.UpdateSearchObject("PreStart", 529);
				}
				throw;
			}
			bool statisticsOnly = searchObject.StatisticsOnly;
			IL_627:
			if (this.server != null)
			{
				MailboxDataProvider.IncrementDiscoveryMailboxSearchQueuePerfCounters();
			}
		}

		internal void Start()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.InternalStart));
		}

		internal SearchStatus GetStatus()
		{
			MailboxDiscoverySearch searchObject = this.mailboxSearchConfiguration.SearchObject;
			SearchState searchState = searchObject.Status;
			if ((searchState == SearchState.InProgress || searchState == SearchState.EstimateInProgress) && this.IsSearchTaskCreated && (this.searchTask.CurrentState == SearchState.Stopped || this.searchTask.CurrentState == SearchState.EstimateStopped) && this.abortingUserId != null)
			{
				searchState = (this.IsEstimateOnly ? SearchState.EstimateStopping : SearchState.Stopping);
			}
			return new SearchStatus(this.SearchId.SearchName, null, (int)searchState, searchObject.PercentComplete, searchObject.ResultItemCountEstimate, (ulong)searchObject.ResultSizeEstimate, searchObject.ResultItemCountCopied, (ulong)searchObject.ResultSizeCopied, searchObject.ResultsLink);
		}

		internal void Abort(string userId)
		{
			this.abortingUserId = userId;
			if (this.Action == WorkItemAction.Remove)
			{
				Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Unable to abort a removing job '{0}'", this.mailboxSearchConfiguration.SearchName);
				return;
			}
			Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "The search {0} is aborted", this.mailboxSearchConfiguration.SearchName);
			if (!this.IsSearchTaskCreated)
			{
				Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Abort '{0}' without search task is running, the search is not really started", this.mailboxSearchConfiguration.SearchName);
				this.SetCompleted();
			}
			else
			{
				Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Abort running search '{0}'", this.mailboxSearchConfiguration.SearchName);
				this.searchTask.Abort();
			}
			lock (this.mailboxSearchConfiguration.SearchObject)
			{
				if (this.mailboxSearchConfiguration.RecipientSession != null && !string.IsNullOrEmpty(userId))
				{
					this.mailboxSearchConfiguration.SearchObject.LastModifiedBy = Util.GetUserNameFromUserId(this.mailboxSearchConfiguration.RecipientSession, userId);
				}
				this.mailboxSearchConfiguration.SearchObject.UpdateState(SearchStateTransition.StopSearch);
				this.mailboxSearchConfiguration.UpdateSearchObject("Abort", 631);
			}
		}

		internal void PreRemove(bool removeLogs)
		{
			Util.Tracer.TraceDebug<SearchId>((long)this.GetHashCode(), "PreRemove search '{0}'", this.SearchId);
			MailboxDiscoverySearch searchObject = this.mailboxSearchConfiguration.SearchObject;
			this.Action = WorkItemAction.Remove;
			this.UpdateFlightedFeatures(searchObject, true);
			string resultsPath = searchObject.ResultsPath;
			string targetLegacyDn;
			ITargetLocation targetLocation;
			if (!string.IsNullOrEmpty(resultsPath) && resultsPath.Contains(Constants.ResultPathSeparator))
			{
				int num = resultsPath.IndexOf(Constants.ResultPathSeparator);
				targetLegacyDn = resultsPath.Substring(0, num);
				string exportLocationName = resultsPath.Substring(num + Constants.ResultPathSeparator.Length);
				targetLocation = Util.CreateTargetLocation(exportLocationName);
			}
			else
			{
				targetLocation = Util.CreateTargetLocation(this.mailboxSearchConfiguration.SearchName);
				targetLegacyDn = this.mailboxSearchConfiguration.SearchObject.Target;
			}
			try
			{
				this.targetMailboxForRemoveTask = Util.CreateTargetMailbox(this.mailboxSearchConfiguration.RecipientSession, searchObject, targetLegacyDn, targetLocation);
			}
			catch (ObjectNotFoundException arg)
			{
				Util.Tracer.TraceError<ObjectNotFoundException>((long)this.GetHashCode(), "MailboxSearchWorkItem. PreRemove target mailbox not found {0}", arg);
			}
			this.hasSearchResultsToRemove = (this.targetMailboxForRemoveTask != null && (this.targetMailboxForRemoveTask.WorkingLocationExist || this.targetMailboxForRemoveTask.ExportLocationExist));
			if (this.hasSearchResultsToRemove)
			{
				this.targetMailboxForRemoveTask.PreRemoveSearchResults(removeLogs);
			}
			Util.Tracer.TraceDebug<SearchId, string>((long)this.GetHashCode(), "PreRemove search '{0}', Name '{1}' is successful", this.SearchId, this.mailboxSearchConfiguration.SearchName);
		}

		internal void Remove()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.RemoveResults));
		}

		internal void WriteErrorLogEventHandler(IEnumerable<ErrorRecord> errorRecords)
		{
			if (errorRecords != null)
			{
				lock (this.mailboxSearchConfiguration.SearchObject)
				{
					foreach (ErrorRecord errorRecord in errorRecords)
					{
						string item = Util.GenerateErrorMessageFromErrorRecord(errorRecord);
						if (!this.mailboxSearchConfiguration.SearchObject.Errors.Contains(item))
						{
							this.mailboxSearchConfiguration.SearchObject.Errors.Add(item);
						}
						if (!this.unsuccessfulMailboxes.Contains(errorRecord.SourceId))
						{
							this.unsuccessfulMailboxes.Add(errorRecord.SourceId);
						}
						if (this.successfulMailboxes.Contains(errorRecord.SourceId))
						{
							this.successfulMailboxes.Remove(errorRecord.SourceId);
						}
					}
					this.mailboxSearchConfiguration.UpdateSearchObject("WriteErrorLogEventHandler", 732);
				}
			}
		}

		internal void WriteResultManifestEventHandler(IEnumerable<ExportRecord> records)
		{
			if (records != null)
			{
				MailboxDiscoverySearch searchObject = this.mailboxSearchConfiguration.SearchObject;
				lock (searchObject)
				{
					(from x in records
					where string.IsNullOrEmpty(x.PrimaryIdOfDuplicates)
					select x).ForEach(delegate(ExportRecord x)
					{
						searchObject.ResultItemCountCopied += 1L;
						searchObject.ResultSizeCopied += (long)((ulong)x.Size);
					});
					if (this.searchStatsFlighted)
					{
						searchObject.UpdateSearchStats(this.GetSearchStatsFromSearchResults());
					}
				}
			}
			if (this.mailboxSearchConfiguration.SearchObject.LogLevel == LoggingLevel.Full && !this.IsEstimateOnly)
			{
				Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Log export records for search '{0}'", this.mailboxSearchConfiguration.SearchName);
				this.searchTask.TargetMailbox.WriteExportRecordLog(this.mailboxSearchConfiguration.SearchObject, records);
			}
		}

		private void InternalStart(object state)
		{
			if (this.server != null)
			{
				MailboxDataProvider.DecrementDiscoveryMailboxSearchQueuePerfCounters();
			}
			Exception ex = null;
			try
			{
				using (ScenarioData.Current)
				{
					if (this.mailboxSearchConfiguration != null && this.mailboxSearchConfiguration.SearchObject != null)
					{
						lock (this.mailboxSearchConfiguration.SearchObject)
						{
							this.mailboxSearchConfiguration.SearchObject.ScenarioId = ScenarioData.Current["SID"];
						}
					}
					Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "The search '{0}' is being started", this.mailboxSearchConfiguration.SearchName);
					if (this.noMailboxesToSearch)
					{
						Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "The search '{0}' has no mailboxes to search, complete immediately", this.mailboxSearchConfiguration.SearchName);
						if (this.IsEstimateOnly)
						{
							this.OnEstimateCompleted(0, 0L, 0L, 0L, null);
						}
						else
						{
							this.OnSearchCompleted();
						}
					}
					else if (this.IsCompleted)
					{
						Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Starting search {0} exits because it is completed.", this.mailboxSearchConfiguration.SearchName);
					}
					else
					{
						if (!this.mailboxSearchConfiguration.UserCanRunMailboxSearch)
						{
							throw new UnauthorizedAccessException(Strings.RbacTargetMailboxAuthorizationError);
						}
						if (!this.mailboxSearchConfiguration.ValidateKeywordsLimit())
						{
							throw new SearchTooManyKeywordsException((int)this.mailboxSearchConfiguration.MaxQueryKeywords);
						}
						SearchEventLogger.Instance.LogDiscoverySearchTaskStartedEvent(this.mailboxSearchConfiguration.SearchName, this.mailboxSearchConfiguration.SearchDataProvider.OrganizationId.ToString());
						Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "The search '{0}' was started", this.mailboxSearchConfiguration.SearchName);
						if (this.mailboxSearchConfiguration.SearchObject.LogLevel != LoggingLevel.Suppress && !this.IsEstimateOnly)
						{
							ScenarioData.Current["S"] = "CS";
							Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Create search log item email for search '{0}'", this.mailboxSearchConfiguration.SearchName);
							this.searchTask.TargetMailbox.CreateOrUpdateSearchLogEmail(this.mailboxSearchConfiguration.SearchObject, this.successfulMailboxes, this.unsuccessfulMailboxes);
						}
						this.searchTask.Start();
						SearchEventLogger.Instance.LogDiscoverySearchTaskCompletedEvent(this.mailboxSearchConfiguration.SearchName, this.mailboxSearchConfiguration.SearchDataProvider.OrganizationId.ToString(), this.searchTask.CurrentState.ToString());
						Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "The search '{0}' was completed", this.mailboxSearchConfiguration.SearchName);
					}
				}
			}
			catch (ParserException ex2)
			{
				ex = ex2;
			}
			catch (SearchQueryEmptyException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			catch (AuthzException ex5)
			{
				ex = ex5;
			}
			catch (DataSourceOperationException ex6)
			{
				ex = ex6;
			}
			catch (StoragePermanentException ex7)
			{
				ex = ex7;
			}
			catch (StorageTransientException ex8)
			{
				ex = ex8;
			}
			catch (LocalizedException ex9)
			{
				ex = ex9;
			}
			catch (ExportException ex10)
			{
				ex = ex10;
			}
			finally
			{
				if (ex != null)
				{
					SearchEventLogger.Instance.LogDiscoverySearchTaskErrorEvent(this.mailboxSearchConfiguration.SearchName, this.mailboxSearchConfiguration.SearchDataProvider.OrganizationId.ToString(), ex);
					this.SetCompleted();
					lock (this.mailboxSearchConfiguration.SearchObject)
					{
						this.mailboxSearchConfiguration.SearchObject.LastEndTime = (DateTime)ExDateTime.UtcNow;
						this.mailboxSearchConfiguration.SearchObject.UpdateState(SearchStateTransition.Fail);
						if (!this.mailboxSearchConfiguration.SearchObject.Errors.Contains(ex.Message))
						{
							this.mailboxSearchConfiguration.SearchObject.Errors.Add(ex.Message);
						}
						try
						{
							this.mailboxSearchConfiguration.UpdateSearchObject("InternalStart", 925);
						}
						catch (DataSourceOperationException ex11)
						{
							SearchEventLogger.Instance.LogDiscoverySearchServerErrorEvent("Failed to Update Search object", this.mailboxSearchConfiguration.SearchName, this.mailboxSearchConfiguration.SearchDataProvider.OrganizationId.ToString(), ex11.ToString());
						}
					}
				}
				if (this.IsSearchTaskCreated)
				{
					SearchState currentState = this.searchTask.CurrentState;
					this.searchTask.Dispose();
					this.searchTask = null;
					if (currentState == SearchState.Succeeded && this.mailboxSearchConfiguration.SearchObject.Status == SearchState.Succeeded)
					{
						MailboxStatusLog.DeleteStatusLog(this.mailboxSearchConfiguration.SearchDataProvider.OrganizationId, this.mailboxSearchConfiguration.SearchName);
					}
				}
				if (this.estimateEwsClient != null)
				{
					this.estimateEwsClient.Dispose();
					this.estimateEwsClient = null;
				}
			}
		}

		private void OnEstimateCompleted(int mailboxesSearchedCount, long itemCount, long totalSize, long unsearchableItemCount, List<KeywordHit> keywordHits)
		{
			try
			{
				lock (this.mailboxSearchConfiguration.SearchObject)
				{
					MailboxDiscoverySearch searchObject2 = this.mailboxSearchConfiguration.SearchObject;
					lock (searchObject2)
					{
						searchObject2.NumberOfMailboxes = mailboxesSearchedCount;
						searchObject2.ResultItemCountEstimate = itemCount + unsearchableItemCount;
						searchObject2.ResultUnsearchableItemCountEstimate = unsearchableItemCount;
						searchObject2.ResultSizeEstimate = totalSize;
						searchObject2.PercentComplete = 100;
						searchObject2.LastEndTime = (DateTime)ExDateTime.UtcNow;
						if (this.searchStatsFlighted)
						{
							searchObject2.UpdateSearchStats(this.GetSearchStatsFromSearchResults());
						}
						this.UpdateStateAfterSearchCompleted(searchObject2);
						searchObject2.KeywordHits = new MultiValuedProperty<KeywordHit>();
						if (keywordHits != null && keywordHits.Count > 0)
						{
							if (keywordHits.Count == 1)
							{
								if (string.Compare(keywordHits[0].Phrase, MailboxDiscoverySearch.EmptyQueryReplacement, StringComparison.OrdinalIgnoreCase) != 0)
								{
									this.AddKeywordHitToCollection(searchObject2.KeywordHits, keywordHits[0]);
								}
							}
							else
							{
								foreach (KeywordHit keywordHit in keywordHits)
								{
									if (string.Compare(keywordHit.Phrase, searchObject2.KeywordsQuery, StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(keywordHit.Phrase, MailboxDiscoverySearch.EmptyQueryReplacement, StringComparison.OrdinalIgnoreCase) != 0)
									{
										this.AddKeywordHitToCollection(searchObject2.KeywordHits, keywordHit);
									}
								}
							}
						}
						if (this.exportContext != null && this.exportContext.Sources != null)
						{
							List<string> list = (this.searchTask != null && this.searchTask.CurrentState == SearchState.EstimateSucceeded) ? this.successfulMailboxes : this.unsuccessfulMailboxes;
							list.AddRange((from src in this.exportContext.Sources
							select src.Id).ToArray<string>());
						}
						if (this.searchTask != null && this.searchTask.Errors != null)
						{
							foreach (string item in this.searchTask.Errors)
							{
								if (!this.mailboxSearchConfiguration.SearchObject.Errors.Contains(item))
								{
									this.mailboxSearchConfiguration.SearchObject.Errors.Add(item);
								}
							}
						}
						this.mailboxSearchConfiguration.UpdateSearchObject("OnEstimateCompleted", 1041);
					}
					this.SendStatusEmailIfRequired(this.estimateEwsClient, searchObject2);
				}
			}
			finally
			{
				this.SetCompleted();
			}
		}

		private void AddKeywordHitToCollection(MultiValuedProperty<KeywordHit> keywordHits, KeywordHit keywordHit)
		{
			KeywordHit keywordHit2 = Util.GetKeywordHit(keywordHits, keywordHit.Phrase);
			if (keywordHit2 == null)
			{
				keywordHit2 = new KeywordHit
				{
					Phrase = keywordHit.Phrase,
					Count = keywordHit.Count,
					Size = new ByteQuantifiedSize((ulong)keywordHit.Size)
				};
				keywordHits.Add(keywordHit2);
				return;
			}
			keywordHit2.Count += keywordHit.Count;
			keywordHit2.Size += new ByteQuantifiedSize((ulong)keywordHit.Size);
		}

		private void OnPrepareCompleted(ISearchResults searchResults)
		{
			this.searchResults = searchResults;
			if (searchResults != null)
			{
				lock (this.mailboxSearchConfiguration.SearchObject)
				{
					MailboxDiscoverySearch searchObject2 = this.mailboxSearchConfiguration.SearchObject;
					lock (searchObject2)
					{
						searchObject2.ResultItemCountEstimate = (long)(searchResults.SearchResultItemsCount + searchResults.UnsearchableItemsCount);
						searchObject2.ResultUnsearchableItemCountEstimate = (long)searchResults.UnsearchableItemsCount;
						searchObject2.ResultSizeEstimate = searchResults.TotalSize;
						if (this.searchStatsFlighted)
						{
							searchObject2.UpdateSearchStats(this.GetSearchStatsFromSearchResults());
						}
						this.mailboxSearchConfiguration.UpdateSearchObject("OnPrepareCompleted", 1103);
					}
				}
			}
		}

		private void OnSearchCompleted()
		{
			try
			{
				this.RemoveWorkItem();
				MailboxDiscoverySearch searchObject = this.mailboxSearchConfiguration.SearchObject;
				lock (searchObject)
				{
					searchObject.PercentComplete = 100;
					searchObject.LastEndTime = (DateTime)ExDateTime.UtcNow;
					this.UpdateStateAfterSearchCompleted(searchObject);
					if (this.searchResults != null)
					{
						foreach (ISource source in this.exportContext.Sources)
						{
							string id = source.Id;
							ISourceStatus sourceStatusBySourceId = this.searchResults.GetSourceStatusBySourceId(id);
							if (sourceStatusBySourceId.IsExportCompleted(true, this.GetIncludeUnsearchableItems(this.exportContext)) && !this.unsuccessfulMailboxes.Contains(id))
							{
								if (!this.successfulMailboxes.Contains(id))
								{
									this.successfulMailboxes.Add(id);
								}
								if (!searchObject.CompletedMailboxes.Contains(id))
								{
									searchObject.CompletedMailboxes.Add(id);
								}
							}
						}
						if (this.searchStatsFlighted)
						{
							searchObject.UpdateSearchStats(this.GetSearchStatsFromSearchResults());
						}
					}
					this.mailboxSearchConfiguration.UpdateSearchObject("OnSearchCompleted", 1167);
				}
				if (!this.noMailboxesToSearch)
				{
					if (this.mailboxSearchConfiguration.SearchObject.LogLevel != LoggingLevel.Suppress)
					{
						Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Create or update search log item email for search '{0}'", this.mailboxSearchConfiguration.SearchName);
						this.searchTask.TargetMailbox.CreateOrUpdateSearchLogEmail(this.mailboxSearchConfiguration.SearchObject, this.successfulMailboxes, this.unsuccessfulMailboxes);
						if (this.mailboxSearchConfiguration.SearchObject.LogLevel == LoggingLevel.Full)
						{
							this.searchTask.TargetMailbox.AttachDiscoveryLogFiles();
						}
					}
					this.SendStatusEmailIfRequired(this.searchTask.TargetMailbox.EwsClientInstance, searchObject);
				}
			}
			finally
			{
				this.SetCompleted();
			}
		}

		private void UpdateStateAfterSearchCompleted(MailboxDiscoverySearch searchObject)
		{
			if ((this.searchTask == null || this.searchTask.CurrentState != SearchState.Stopped) && searchObject.Status != SearchState.Stopped)
			{
				SearchStateTransition searchStateTransition = this.AddErrorsAndDetermineStateTransition();
				if (this.searchTask != null && this.searchTask.CurrentState == searchObject.Status && searchStateTransition == SearchStateTransition.MoveToNextState)
				{
					SearchEventLogger.Instance.LogDiscoverySearchServerErrorEvent("An attempt to Update the search object state to the same state cannot succeed, so we'll skip it.", this.mailboxSearchConfiguration.SearchName, this.mailboxSearchConfiguration.SearchDataProvider.OrganizationId.ToString(), this.searchTask.CurrentState.ToString());
					Util.Tracer.TraceWarning((long)this.GetHashCode(), string.Format("An attempt to Update the search object state to the same state cannot succeed, so we'll skip it. this.SearchTask.CurrentState: {0}", this.searchTask.CurrentState.ToString()));
					return;
				}
				searchObject.UpdateState(searchStateTransition);
			}
		}

		private SearchStateTransition AddErrorsAndDetermineStateTransition()
		{
			bool flag = false;
			MailboxDiscoverySearch searchObject = this.mailboxSearchConfiguration.SearchObject;
			if (searchObject.Errors != null && searchObject.Errors.Count > 0)
			{
				flag = true;
			}
			if (this.IsSearchTaskCreated && this.searchTask.Errors.Count > 0)
			{
				this.searchTask.Errors.ForEach(delegate(string x)
				{
					if (!searchObject.Errors.Contains(x))
					{
						searchObject.Errors.Add(x);
					}
				});
				flag = true;
			}
			(from x in this.notFoundMailboxes
			select Strings.SourceMailboxUserNotFoundInAD(x) into x
			where !searchObject.Errors.Contains(x)
			select x).ForEach(delegate(LocalizedString x)
			{
				searchObject.Errors.Add(x);
			});
			(from x in this.rbacDeniedMailboxes
			select string.Format("EDiscoveryError:E008::Mailbox:{0}::{1}", x, Strings.RbacSourceMailboxAuthorizationError(x)) into x
			where !searchObject.Errors.Contains(x)
			select x).ForEach(delegate(string x)
			{
				searchObject.Errors.Add(x);
			});
			(from x in this.versionSkippedMailboxes
			select Strings.SourceMailboxVersionError(x) into x
			where !searchObject.Errors.Contains(x)
			select x).ForEach(delegate(LocalizedString x)
			{
				searchObject.Errors.Add(x);
			});
			(from x in this.crossPremiseFailedMailboxes
			select Strings.SourceMailboxCrossPremiseError(x) into x
			where !searchObject.Errors.Contains(x)
			select x).ForEach(delegate(LocalizedString x)
			{
				searchObject.Errors.Add(x);
			});
			if (this.searchTask != null && (this.searchTask.CurrentState == SearchState.Stopped || this.searchTask.CurrentState == SearchState.EstimateStopped))
			{
				return SearchStateTransition.StopSearch;
			}
			if (flag && this.searchTask != null && (this.searchTask.CurrentState == SearchState.Succeeded || this.searchTask.CurrentState == SearchState.EstimateSucceeded))
			{
				if ((this.IsEstimateOnly && searchObject.ResultItemCountEstimate == 0L) || (!this.IsEstimateOnly && searchObject.ResultItemCountCopied == 0L))
				{
					return SearchStateTransition.Fail;
				}
				return SearchStateTransition.MoveToNextPartialSuccessState;
			}
			else
			{
				if (flag || this.searchTask == null || this.searchTask.CurrentState == SearchState.Failed || this.searchTask.CurrentState == SearchState.EstimateFailed)
				{
					return SearchStateTransition.Fail;
				}
				if ((this.noMailboxesToSearch || this.searchTask.CurrentState != SearchState.Failed || this.searchTask.CurrentState == SearchState.EstimateFailed) && (this.rbacDeniedMailboxes.Count > 0 || this.versionSkippedMailboxes.Count > 0 || this.notFoundMailboxes.Count > 0 || this.crossPremiseFailedMailboxes.Count > 0))
				{
					return SearchStateTransition.MoveToNextPartialSuccessState;
				}
				if (this.searchTask.CurrentState == SearchState.PartiallySucceeded || this.searchTask.CurrentState == SearchState.EstimatePartiallySucceeded)
				{
					return SearchStateTransition.MoveToNextPartialSuccessState;
				}
				return SearchStateTransition.MoveToNextState;
			}
		}

		private void RemoveResults(object state)
		{
			SearchUtils.ExWatsonWrappedCall(delegate()
			{
				Exception ex = null;
				try
				{
					if (this.hasSearchResultsToRemove)
					{
						this.targetMailboxForRemoveTask.RemoveSearchResults();
					}
					MailboxStatusLog.DeleteStatusLog(this.mailboxSearchConfiguration.SearchDataProvider.OrganizationId, this.mailboxSearchConfiguration.SearchName);
					if (this.mailboxSearchConfiguration.SearchObject.Status == SearchState.DeletionInProgress)
					{
						this.mailboxSearchConfiguration.SearchDataProvider.Delete<MailboxDiscoverySearch>(this.mailboxSearchConfiguration.SearchName);
					}
				}
				catch (StorageTransientException ex2)
				{
					ex = ex2;
				}
				catch (StoragePermanentException ex3)
				{
					ex = ex3;
				}
				catch (DataSourceOperationException ex4)
				{
					ex = ex4;
				}
				catch (ExportException ex5)
				{
					ex = ex5;
				}
				finally
				{
					if (ex != null)
					{
						Util.Tracer.TraceError<Exception>((long)this.GetHashCode(), "RemoveResults failed with exception {0}", ex);
						SearchEventLogger.Instance.LogDiscoverySearchTaskErrorEvent(this.mailboxSearchConfiguration.SearchName, this.mailboxSearchConfiguration.SearchDataProvider.OrganizationId.ToString(), ex);
					}
					if (this.targetMailboxForRemoveTask != null)
					{
						this.targetMailboxForRemoveTask.Dispose();
						this.targetMailboxForRemoveTask = null;
					}
					this.SetCompleted();
				}
			});
		}

		private void RemoveWorkItem()
		{
			if (this.server != null)
			{
				this.server.RemoveWorkItem(this.SearchId, this.IsEstimateOnly);
			}
		}

		private void SetCompleted()
		{
			this.workItemCompletedEvent.Set();
			this.RemoveWorkItem();
		}

		private void ExportStatusReportingEventHandler(object sender, ExportStatusEventArgs e)
		{
			if (e != null)
			{
				MailboxDiscoverySearch searchObject = this.mailboxSearchConfiguration.SearchObject;
				lock (searchObject)
				{
					if (searchObject.ResultItemCountEstimate > 0L)
					{
						searchObject.PercentComplete = (int)(searchObject.ResultItemCountCopied / searchObject.ResultItemCountEstimate) * 100;
						if (searchObject.PercentComplete > 100)
						{
							searchObject.PercentComplete = 100;
						}
					}
					if (this.searchStatsFlighted)
					{
						searchObject.UpdateSearchStats(this.GetSearchStatsFromSearchResults());
					}
					this.mailboxSearchConfiguration.UpdateSearchObject("ExportStatusReportingEventHandler", 1445);
				}
			}
		}

		private void SendStatusEmailIfRequired(IEwsClient ewsClient, MailboxDiscoverySearch searchObject)
		{
			if (searchObject.StatusMailRecipients != null && searchObject.StatusMailRecipients.Count > 0)
			{
				if (this.statusMailRecipients == null)
				{
					this.statusMailRecipients = (from r in searchObject.StatusMailRecipients.Select(delegate(ADObjectId recipientId)
					{
						ADRecipient adrecipient = this.mailboxSearchConfiguration.RecipientSession.Read(recipientId);
						if (adrecipient == null)
						{
							Util.Tracer.TraceWarning<ADObjectId>((long)this.GetHashCode(), "Unable to find status mail recipient '{0}'", recipientId);
							return null;
						}
						return adrecipient.PrimarySmtpAddress.ToString();
					})
					where r != null
					select r).ToArray<string>();
				}
				if (this.statusMailRecipients.Length > 0)
				{
					MessageType messageType = new MessageType();
					messageType.Subject = Strings.LogMailHeader(searchObject.Name, LocalizedDescriptionAttribute.FromEnum(typeof(SearchState), searchObject.Status));
					messageType.Body = new BodyType
					{
						BodyType1 = BodyTypeType.HTML,
						Value = Util.CreateStatusMailBody(searchObject, this.statusMailRecipients, this.successfulMailboxes, this.unsuccessfulMailboxes, this.exportContext.Sources)
					};
					messageType.ToRecipients = (from recipientEmailAddress in this.statusMailRecipients
					select new EmailAddressType
					{
						EmailAddress = recipientEmailAddress,
						RoutingType = "SMTP"
					}).ToArray<EmailAddressType>();
					MessageType messageType2 = messageType;
					ewsClient.SendEmails(this.mailboxSearchConfiguration.SearchDataProvider.PrimarySmtpAddress, new MessageType[]
					{
						messageType2
					});
				}
			}
		}

		private bool GetIncludeUnsearchableItems(IExportContext exportContext)
		{
			if (this.includeUnsearchableItems == null)
			{
				this.includeUnsearchableItems = new bool?(Util.IncludeUnsearchableItems(exportContext));
			}
			return this.includeUnsearchableItems.Value;
		}

		private DiscoverySearchStats GetSearchStatsFromSearchResults()
		{
			DiscoverySearchStats discoverySearchStats = new DiscoverySearchStats();
			if (this.searchResults != null)
			{
				discoverySearchStats.TotalDuplicateItems = this.searchResults.DuplicateItemCount + this.searchResults.UnsearchableDuplicateItemCount;
				discoverySearchStats.UnsearchableItemsAdded = (long)this.searchResults.UnsearchableItemsCount;
				discoverySearchStats.SkippedErrorItems = this.searchResults.ErrorItemCount;
			}
			return discoverySearchStats;
		}

		private VariantConfigurationSnapshot UpdateFlightedFeatures(MailboxDiscoverySearch searchObject, bool update)
		{
			ADUser aduser = this.mailboxSearchConfiguration.DiscoverySystemMailboxUser;
			MailboxSearchConfigurationProvider mailboxSearchConfigurationProvider = this.mailboxSearchConfiguration as MailboxSearchConfigurationProvider;
			if (mailboxSearchConfigurationProvider != null && mailboxSearchConfigurationProvider.ExecutingUser != null)
			{
				aduser = mailboxSearchConfigurationProvider.ExecutingUser;
			}
			if (aduser != null)
			{
				bool flag = false;
				VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(aduser.GetContext(null), null, null);
				if (snapshot.Eac.DiscoveryPFSearch.Enabled || snapshot.Discovery.SearchScale.Enabled)
				{
					searchObject.AddFlightedFeature("PublicFolderSearchFlighted");
					flag = true;
				}
				if (snapshot.Eac.DiscoveryDocIdHint.Enabled)
				{
					searchObject.AddFlightedFeature("DocIdHintFlighted");
					flag = true;
				}
				if (snapshot.Discovery.SearchScale.Enabled)
				{
					searchObject.AddFlightedFeature("SearchScaleFlighted");
					flag = true;
				}
				if (snapshot.Discovery.UrlRebind.Enabled)
				{
					searchObject.AddFlightedFeature("UrlRebindFlighted");
					flag = true;
				}
				bool flag2 = false;
				if (bool.TryParse(snapshot.Discovery.DiscoveryExcludedFoldersEnabled.Value, out flag2) && flag2)
				{
					searchObject.AddFlightedFeature("ExcludeFoldersFlighted");
					flag = true;
				}
				this.searchStatsFlighted = snapshot.Eac.DiscoverySearchStats.Enabled;
				if (this.searchStatsFlighted)
				{
					searchObject.AddFlightedFeature("SearchStatsFlighted");
					flag = true;
				}
				if (update && flag)
				{
					lock (searchObject)
					{
						this.mailboxSearchConfiguration.UpdateSearchObject("UpdateFlightedFeatures", 1602);
					}
				}
				return snapshot;
			}
			return null;
		}

		private const int DeleteSearchRetries = 3;

		private MailboxSearchServer server;

		private IMailboxSearchConfigurationProvider mailboxSearchConfiguration;

		private IMailboxSearchTask searchTask;

		private ITargetMailbox targetMailboxForRemoveTask;

		private List<string> rbacDeniedMailboxes = new List<string>(1);

		private List<string> versionSkippedMailboxes = new List<string>(1);

		private List<string> notFoundMailboxes = new List<string>(1);

		private List<string> crossPremiseFailedMailboxes = new List<string>(1);

		private ManualResetEvent workItemCompletedEvent = new ManualResetEvent(false);

		private string abortingUserId;

		private bool hasSearchResultsToRemove;

		private bool noMailboxesToSearch;

		private EwsClient estimateEwsClient;

		private List<string> successfulMailboxes = new List<string>(1);

		private List<string> unsuccessfulMailboxes = new List<string>(1);

		private string[] statusMailRecipients;

		private IExportContext exportContext;

		private ISearchResults searchResults;

		private bool? includeUnsearchableItems = null;

		private bool searchStatsFlighted;
	}
}
