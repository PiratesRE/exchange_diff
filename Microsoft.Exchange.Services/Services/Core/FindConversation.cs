using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class FindConversation : SingleStepServiceCommand<FindConversationRequest, Microsoft.Exchange.Services.Core.Types.ConversationType[]>
	{
		public FindConversation(CallContext callContext, FindConversationRequest request) : base(callContext, request)
		{
			if (base.Request.ParentFolderId == null)
			{
				throw new ArgumentNullException("FindConversationRequest.ParentFolderId", "FindConversationRequest.ParentFolderId cannot be null.");
			}
			this.paging = base.Request.Paging;
			this.traversalType = base.Request.Traversal;
			this.viewFilter = base.Request.ViewFilter;
			this.fromFilter = base.Request.FromFilter;
			this.clutterFilter = base.Request.ClutterFilter;
			this.searchFolderId = (base.Request.SearchFolderId as FolderId);
			this.sortResults = base.Request.SortOrder;
			this.currentFolder = base.Request.ParentFolderId;
			this.queryString = ((base.Request.QueryString != null) ? base.Request.QueryString.Value : null);
			this.shape = this.GetResponseShape();
			this.searchScope = SearchScope.SelectedAndSubfolders;
			this.mailboxScope = request.MailboxScope;
			OwsLogRegistry.Register(FindConversation.FindConversationActionName, typeof(FindConversationAndItemMetadata), new Type[0]);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new FindConversationResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value, this.highlightTerms, this.totalConversationsInView, this.indexedOffset, this.isSearchInProgress)
			{
				SearchFolderId = this.searchFolderId
			};
		}

		internal override ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> Execute()
		{
			ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> result = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				UnifiedView unifiedView = UnifiedView.Create(ExTraceGlobals.SearchTracer, base.CallContext, base.Request.MailboxGuids, this.currentFolder);
				if (unifiedView != null)
				{
					disposeGuard.Add<UnifiedView>(unifiedView);
				}
				this.ValidateRequest();
				if (this.shape == null)
				{
					this.shape = new ConversationResponseShape(ShapeEnum.Default, null);
				}
				if (unifiedView != null)
				{
					UnifiedView.UpdateConversationResponseShape(this.shape);
				}
				this.determiner = PropertyListForViewRowDeterminer.BuildForConversation(this.shape);
				this.fetchList = new List<PropertyDefinition>(this.determiner.GetPropertiesToFetch());
				this.sortBy = SortResults.ToXsoSortBy(this.sortResults);
				IdAndSession idAndSession = this.GetIdAndSession(unifiedView);
				MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
				base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.QueryString, string.IsNullOrEmpty(this.queryString) ? string.Empty : this.queryString);
				base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.ViewFilter, this.viewFilter);
				if (!base.Request.MailboxScopeSpecified)
				{
					result = this.ExecuteSearch(idAndSession, unifiedView);
				}
				else
				{
					ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> serviceResult = null;
					ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> serviceResult2 = null;
					FindConversation.ExecuteArchiveSearchDelegate executeArchiveSearchDelegate = null;
					IAsyncResult asyncResult = null;
					if ((this.mailboxScope & MailboxSearchLocation.ArchiveOnly) == MailboxSearchLocation.ArchiveOnly)
					{
						if (mailboxSession.MailboxOwner.GetArchiveMailbox() == null)
						{
							throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorArchiveMailboxNotEnabled);
						}
						executeArchiveSearchDelegate = new FindConversation.ExecuteArchiveSearchDelegate(this.ExecuteArchiveSearch);
						asyncResult = executeArchiveSearchDelegate.BeginInvoke(mailboxSession.MailboxOwner, null, null);
					}
					if ((this.mailboxScope & MailboxSearchLocation.PrimaryOnly) == MailboxSearchLocation.PrimaryOnly)
					{
						serviceResult = this.ExecuteSearch(idAndSession, unifiedView);
						if (serviceResult.Value != null)
						{
							foreach (Microsoft.Exchange.Services.Core.Types.ConversationType conversationType in serviceResult.Value)
							{
								conversationType.MailboxScope = MailboxSearchLocation.PrimaryOnly;
							}
						}
					}
					if (asyncResult != null)
					{
						serviceResult2 = executeArchiveSearchDelegate.EndInvoke(asyncResult);
						if (serviceResult2.Value != null)
						{
							foreach (Microsoft.Exchange.Services.Core.Types.ConversationType conversationType2 in serviceResult2.Value)
							{
								conversationType2.MailboxScope = MailboxSearchLocation.ArchiveOnly;
							}
						}
					}
					result = this.MergeResults(serviceResult, serviceResult2);
				}
			}
			return result;
		}

		private void ValidateRequest()
		{
			int num;
			if (!CallContext.Current.Budget.CanAllocateFoundObjects(1U, out num))
			{
				ExceededFindCountLimitException.Throw();
			}
			Microsoft.Exchange.Services.Core.Search.BasePagingType.Validate(this.paging);
			if (base.Request.TraversalSpecified && string.IsNullOrEmpty(this.queryString))
			{
				ExTraceGlobals.ExceptionTracer.TraceError(0L, "[FindConversation::ValidateRequest] You can use traversal with a query string only.");
				throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorTraversalNotAllowedWithoutQueryString);
			}
			if (base.Request.MailboxScopeSpecified && string.IsNullOrEmpty(this.queryString))
			{
				ExTraceGlobals.ExceptionTracer.TraceError(0L, "[FindConversation::ValidateRequest] You can use mailbox scope with a query string only.");
				throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorMailboxScopeNotAllowedWithoutQueryString);
			}
		}

		private ConversationResponseShape GetResponseShape()
		{
			return Global.ResponseShapeResolver.GetResponseShape<ConversationResponseShape>(base.Request.ShapeName, base.Request.ConversationShape, base.CallContext.FeaturesManager);
		}

		private IdAndSession GetIdAndSession(UnifiedView unifiedView)
		{
			IdAndSession idAndSession = (unifiedView == null) ? base.IdConverter.ConvertFolderIdToIdAndSession(this.currentFolder.BaseFolderId, IdConverter.ConvertOption.IgnoreChangeKey) : unifiedView.CreateIdAndSessionUsingSessionCache();
			if (!(idAndSession.Session is MailboxSession))
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)3359997542U);
			}
			return idAndSession;
		}

		private void BuildQueryFilter(IdAndSession currentFolderIdAndSession)
		{
			MailboxSession mailboxSession = currentFolderIdAndSession.Session as MailboxSession;
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.JunkEmail);
			StoreObjectId defaultFolderId2 = mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems);
			bool includeJunkItems = currentFolderIdAndSession.Id.Equals(defaultFolderId) || this.searchScope == SearchScope.SelectedFolder;
			bool includeDeletedItems = currentFolderIdAndSession.Id.Equals(defaultFolderId2) || SearchUtil.ShouldReturnDeletedItems(base.Request.QueryString) || this.searchScope == SearchScope.SelectedFolder;
			this.queryFilter = SearchUtil.BuildQueryFilter(this.queryFilter, defaultFolderId, defaultFolderId2, includeJunkItems, includeDeletedItems, this.viewFilter, this.clutterFilter, base.Request.RefinerRestriction, this.fromFilter);
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> ExecuteSearch(IdAndSession idAndSession, UnifiedView unifiedView)
		{
			ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> result = null;
			MailboxSession session = idAndSession.Session as MailboxSession;
			if ((this.mailboxScope & MailboxSearchLocation.PrimaryOnly) == MailboxSearchLocation.PrimaryOnly && (this.mailboxScope & MailboxSearchLocation.ArchiveOnly) == MailboxSearchLocation.ArchiveOnly)
			{
				base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.MailboxTarget, MailboxTarget.PrimaryAndArchive.ToString());
			}
			else if (Util.IsArchiveMailbox(session))
			{
				base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.MailboxTarget, MailboxTarget.Archive.ToString());
			}
			else
			{
				base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.MailboxTarget, MailboxTarget.Primary.ToString());
			}
			if (SearchUtil.IsNormalView(base.Request.QueryString, this.viewFilter, this.clutterFilter))
			{
				QueryFilter queryFilter = null;
				SortBy[] conversationQuerySortBy = this.sortBy;
				if (!string.IsNullOrEmpty(this.fromFilter))
				{
					queryFilter = PeopleIKnowQuery.GetConversationQueryFilter(this.fromFilter);
					conversationQuerySortBy = PeopleIKnowQuery.GetConversationQuerySortBy(this.sortBy);
				}
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					Folder folder;
					if (unifiedView != null && unifiedView.SearchFolder != null)
					{
						folder = unifiedView.SearchFolder;
					}
					else
					{
						folder = Folder.Bind(session, idAndSession.Id);
						disposeGuard.Add<Folder>(folder);
					}
					using (QueryResult queryResult = folder.ConversationItemQuery(queryFilter, conversationQuerySortBy, this.fetchList))
					{
						BasePageResult basePageResult = Microsoft.Exchange.Services.Core.Search.BasePagingType.ApplyPostQueryPaging(queryResult, this.paging);
						Microsoft.Exchange.Services.Core.Types.ConversationType[] value = basePageResult.View.ConvertToConversationObjects(this.fetchList.ToArray(), this.determiner, idAndSession, (base.CallContext == null) ? null : base.CallContext.ProtocolLog);
						if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
						{
							this.totalConversationsInView = new int?(queryResult.EstimatedRowCount);
							base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.TotalRowCount, this.totalConversationsInView);
							IndexedPageResult indexedPageResult = basePageResult as IndexedPageResult;
							if (indexedPageResult != null)
							{
								this.indexedOffset = new int?(indexedPageResult.IndexedOffset);
							}
						}
						result = new ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]>(value);
					}
					return result;
				}
			}
			if (this.traversalType == ConversationQueryTraversal.Shallow)
			{
				this.searchScope = SearchScope.SelectedFolder;
			}
			if (SearchUtil.IsSearch(base.Request.QueryString))
			{
				this.queryFilter = SearchFilterGenerator.Execute(this.queryString, CallContext.Current.ClientCulture, null);
				if (base.Request.QueryString.ReturnHighlightTerms)
				{
					List<Microsoft.Exchange.Services.Core.Types.HighlightTermType> list = new List<Microsoft.Exchange.Services.Core.Types.HighlightTermType>();
					SearchUtil.BuildHighlightTerms(this.queryFilter, list);
					if (list.Count > 0)
					{
						this.highlightTerms = list.ToArray();
					}
				}
			}
			result = this.ExecuteFilteredViewOrFastSearch(idAndSession);
			return result;
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> ExecuteFilteredViewOrFastSearch(IdAndSession currentFolderIdAndSession)
		{
			MailboxSession mailboxSession = currentFolderIdAndSession.Session as MailboxSession;
			this.BuildQueryFilter(currentFolderIdAndSession);
			List<StoreId> list = new List<StoreId>();
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(currentFolderIdAndSession.Id);
			if (asStoreObjectId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.FromFavoriteSenders)))
			{
				list.Add(mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox));
			}
			else
			{
				list.Add(currentFolderIdAndSession.Id);
			}
			SearchFolderCriteria searchFolderCriteria = SearchUtil.BuildSearchCriteria(list, this.queryFilter, this.searchScope, base.Request.QueryString);
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.SearchFolders);
			ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> result;
			if (CallContext.Current.OwaCallback == null)
			{
				result = this.ExecuteFastSearch(currentFolderIdAndSession, defaultFolderId, searchFolderCriteria);
			}
			else
			{
				result = this.OwaExecuteFilteredViewOrFastSearch(currentFolderIdAndSession, defaultFolderId, searchFolderCriteria);
			}
			return result;
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> ExecuteFastSearch(IdAndSession currentFolderIdAndSession, StoreObjectId searchFoldersRootId, SearchFolderCriteria searchFolderCriteria)
		{
			MailboxSession mailboxSession = currentFolderIdAndSession.Session as MailboxSession;
			ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> result = null;
			ExTraceGlobals.SearchTracer.TraceDebug((long)this.GetHashCode(), "[FindConversation::ExecuteFastSearch] Executing non-OWA fast search.");
			using (SearchFolder searchFolder = SearchUtil.CreateStaticSearchFolder(mailboxSession, searchFoldersRootId, searchFolderCriteria))
			{
				if (searchFolder == null)
				{
					result = this.GetErrorServiceResult();
				}
				else
				{
					result = this.GetSearchServiceResult(searchFolder, mailboxSession);
					using (Folder folder = Folder.Bind(mailboxSession, searchFoldersRootId))
					{
						folder.DeleteObjects(DeleteItemFlags.HardDelete, new StoreId[]
						{
							searchFolder.Id.ObjectId
						});
					}
				}
			}
			return result;
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> OwaExecuteFilteredViewOrFastSearch(IdAndSession currentFolderIdAndSession, StoreObjectId searchFoldersRootId, SearchFolderCriteria searchFolderCriteria)
		{
			MailboxSession mailboxSession = currentFolderIdAndSession.Session as MailboxSession;
			ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> result = null;
			ExTraceGlobals.SearchTracer.TraceDebug((long)this.GetHashCode(), "[FindConversation::OwaExecuteFastSearch] Executing fast search for OWA.");
			OwaSearchContext owaSearchContext = new OwaSearchContext();
			owaSearchContext.SearchContextType = SearchContextType.ConversationSearch;
			owaSearchContext.ClientSearchFolderIdentity = base.Request.SearchFolderIdentity;
			owaSearchContext.IsFilteredView = SearchUtil.IsFilteredView(base.Request.QueryString, this.viewFilter, this.clutterFilter, this.fromFilter);
			owaSearchContext.IsResetCache = (base.Request.QueryString != null && base.Request.QueryString.ResetCache);
			owaSearchContext.WaitForSearchComplete = (base.Request.QueryString != null && base.Request.QueryString.WaitForSearchComplete);
			owaSearchContext.OptimizedSearch = (base.Request.QueryString != null && base.Request.QueryString.OptimizedSearch);
			owaSearchContext.SearchScope = this.searchScope;
			owaSearchContext.SearchSortBy = this.sortBy;
			owaSearchContext.SearchQueryFilter = this.queryFilter;
			owaSearchContext.SearchQueryFilterString = this.queryFilter.ToString();
			owaSearchContext.FromFilter = this.fromFilter;
			owaSearchContext.ViewFilter = (OwaViewFilter)SearchUtil.GetViewFilterForSearchFolder(this.viewFilter, this.clutterFilter);
			owaSearchContext.FolderIdToSearch = currentFolderIdAndSession.Id;
			owaSearchContext.SearchTimeoutInMilliseconds = Global.SearchTimeoutInMilliseconds;
			owaSearchContext.MaximumTemporaryFilteredViewPerUser = Global.MaximumTemporaryFilteredViewPerUser;
			SearchUtil.SetHighlightTerms(owaSearchContext, this.highlightTerms);
			owaSearchContext.RequestTimeZone = EWSSettings.RequestTimeZone;
			if (this.searchFolderId != null)
			{
				IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(this.searchFolderId);
				owaSearchContext.SearchFolderId = ((idAndSession == null) ? null : StoreId.GetStoreObjectId(idAndSession.Id));
			}
			SearchFolder searchFolder;
			if (owaSearchContext.IsFilteredView)
			{
				searchFolder = OwaFilterState.CreateOrOpenOwaFilteredViewSearchFolder(mailboxSession, owaSearchContext, searchFoldersRootId, searchFolderCriteria, true);
			}
			else
			{
				searchFolder = SearchUtil.CreateOrOpenStaticOwaSearchFolder(mailboxSession, owaSearchContext, searchFoldersRootId, searchFolderCriteria);
			}
			using (searchFolder)
			{
				this.searchFolderId = null;
				if (searchFolder == null)
				{
					result = this.GetErrorServiceResult();
				}
				else if (owaSearchContext.IsSearchFailed)
				{
					ServiceError error = new ServiceError(CoreResources.IDs.ErrorSearchFolderNotInitialized, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorSearchFolderNotInitialized, 0, ExchangeVersion.Exchange2012);
					result = new ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]>(error);
				}
				else
				{
					StorePerformanceCountersCapture storePerformanceCountersCapture = StorePerformanceCountersCapture.Start(mailboxSession);
					ExDateTime utcNow = ExDateTime.UtcNow;
					Stopwatch stopwatch = Stopwatch.StartNew();
					if (!owaSearchContext.IsFilteredView)
					{
						this.isSearchInProgress = owaSearchContext.IsSearchInProgress;
						if (this.paging != null)
						{
							this.paging.LoadPartialPageRows = this.isSearchInProgress;
						}
					}
					this.searchFolderId = IdConverter.GetFolderIdFromStoreId(searchFolder.StoreObjectId, new MailboxId(mailboxSession));
					result = this.GetSearchServiceResult(searchFolder, mailboxSession);
					ExDateTime dateTime = utcNow.AddTicks(stopwatch.Elapsed.Ticks);
					StorePerformanceCounters storePerformanceCounters = storePerformanceCountersCapture.Stop();
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.AggregateDataTime, storePerformanceCounters.ElapsedMilliseconds);
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.AggregateDataCPUTime, storePerformanceCounters.Cpu);
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.AggregateDataRpcCount, storePerformanceCounters.RpcCount);
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.AggregateDataRpcLatency, storePerformanceCounters.RpcLatency);
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.AggregateDataRpcLatencyOnStore, storePerformanceCounters.RpcLatencyOnStore);
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.AggregateDataStartTimestamp, SearchUtil.FormatIso8601String(utcNow));
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.AggregateDataEndTimestamp, SearchUtil.FormatIso8601String(dateTime));
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.OptimizedSearch, owaSearchContext.OptimizedSearch);
				}
			}
			return result;
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> GetErrorServiceResult()
		{
			ExTraceGlobals.SearchTracer.TraceDebug((long)this.GetHashCode(), "[FindConversation::GetErrorServiceResult] returning service error.");
			ServiceError error = new ServiceError(CoreResources.IDs.ErrorSearchTimeoutExpired, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorTimeoutExpired, 0, ExchangeVersion.Exchange2010);
			return new ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]>(error);
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> MergeResults(ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> primaryResults, ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> archiveResults)
		{
			if (archiveResults == null || archiveResults.Value == null)
			{
				return primaryResults;
			}
			if (primaryResults == null || primaryResults.Value == null)
			{
				return archiveResults;
			}
			IEnumerable<Microsoft.Exchange.Services.Core.Types.ConversationType> mergedConversations = from primaryResult in primaryResults.Value
			join archiveResult in archiveResults.Value on IdConverter.EwsIdToConversationId(primaryResult.ConversationId.Id) equals IdConverter.EwsIdToConversationId(archiveResult.ConversationId.Id)
			where primaryResult.ConversationId != null && archiveResult.ConversationId != null
			select Microsoft.Exchange.Services.Core.Types.ConversationType.MergeConversations(primaryResult, archiveResult);
			IEnumerable<Microsoft.Exchange.Services.Core.Types.ConversationType> second = from primaryResult in primaryResults.Value
			where primaryResult.ConversationId == null || !mergedConversations.Contains(primaryResult, ConversationHelper.ConversationTypeEqualityComparer)
			select primaryResult;
			IEnumerable<Microsoft.Exchange.Services.Core.Types.ConversationType> second2 = from archiveResult in archiveResults.Value
			where archiveResult.ConversationId == null || !mergedConversations.Contains(archiveResult, ConversationHelper.ConversationTypeEqualityComparer)
			select archiveResult;
			IEnumerable<Microsoft.Exchange.Services.Core.Types.ConversationType> source = mergedConversations.Concat(second).Concat(second2);
			this.totalConversationsInView = new int?(source.Count<Microsoft.Exchange.Services.Core.Types.ConversationType>());
			if (this.sortResults == null || this.sortResults.FirstOrDefault<SortResults>().Order == SortDirection.Descending)
			{
				return new ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]>(source.OrderByDescending((Microsoft.Exchange.Services.Core.Types.ConversationType c) => c.LastDeliveryTime, ConversationHelper.DateTimeStringComparer).ToArray<Microsoft.Exchange.Services.Core.Types.ConversationType>());
			}
			return new ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]>(source.OrderBy((Microsoft.Exchange.Services.Core.Types.ConversationType c) => c.LastDeliveryTime, ConversationHelper.DateTimeStringComparer).ToArray<Microsoft.Exchange.Services.Core.Types.ConversationType>());
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> GetSearchServiceResult(SearchFolder searchFolder, MailboxSession session)
		{
			QueryFilter queryFilter = null;
			if (SearchUtil.IsComplexClutterFilteredView(this.viewFilter, this.clutterFilter))
			{
				queryFilter = SearchUtil.GetViewQueryForComplexClutterFilteredView(this.clutterFilter, true);
			}
			ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> result;
			using (QueryResult queryResult = searchFolder.ConversationItemQuery(queryFilter, this.sortBy, this.fetchList))
			{
				ExTraceGlobals.SearchTracer.TraceDebug((long)this.GetHashCode(), "[FindConversation::GetSearchServiceResult] Start.");
				IdAndSession idAndSession = new IdAndSession(searchFolder.Id.ObjectId, session);
				BasePageResult basePageResult = Microsoft.Exchange.Services.Core.Search.BasePagingType.ApplyPostQueryPaging(queryResult, this.paging);
				Microsoft.Exchange.Services.Core.Types.ConversationType[] value = basePageResult.View.ConvertToConversationObjects(this.fetchList.ToArray(), this.determiner, idAndSession, (base.CallContext == null) ? null : base.CallContext.ProtocolLog);
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
				{
					this.totalConversationsInView = new int?(queryResult.EstimatedRowCount);
					ExTraceGlobals.SearchTracer.TraceDebug<int?>((long)this.GetHashCode(), "[FindConversation::GetSearchServiceResult] totalConversationsInView: {0}.", this.totalConversationsInView);
					base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.TotalRowCount, this.totalConversationsInView);
					IndexedPageResult indexedPageResult = basePageResult as IndexedPageResult;
					if (indexedPageResult != null)
					{
						this.indexedOffset = new int?(indexedPageResult.IndexedOffset);
					}
				}
				result = new ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]>(value);
			}
			return result;
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> ExecuteArchiveSearch(IExchangePrincipal primaryPrincipal)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			Stopwatch stopwatch = Stopwatch.StartNew();
			ExchangeServiceBinding archiveServiceBinding = EwsClientHelper.GetArchiveServiceBinding(base.CallContext.EffectiveCaller, primaryPrincipal);
			ExDateTime dateTime = utcNow.AddTicks(stopwatch.Elapsed.Ticks);
			IMailboxInfo archiveMailbox = primaryPrincipal.GetArchiveMailbox();
			base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.ArchiveState, (archiveMailbox != null) ? archiveMailbox.ArchiveState : ArchiveState.None);
			base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.ArchiveDiscoveryStartTimestamp, SearchUtil.FormatIso8601String(utcNow));
			base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.ArchiveDiscoveryEndTimestamp, SearchUtil.FormatIso8601String(dateTime));
			base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.ArchiveDiscoveryFailed, archiveServiceBinding == null);
			if (archiveServiceBinding != null)
			{
				return this.ExecuteRemoteArchiveSearch(archiveServiceBinding);
			}
			ExTraceGlobals.ExceptionTracer.TraceError(0L, "[FindConversation::ExecuteArchiveSearch] Unsupported archive state.");
			ServiceError error = new ServiceError((CoreResources.IDs)3156121664U, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorArchiveMailboxServiceDiscoveryFailed, 0, ExchangeVersion.Exchange2012);
			return new ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]>(error);
		}

		private ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> ExecuteRemoteArchiveSearch(ExchangeServiceBinding serviceBinding)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			Stopwatch stopwatch = Stopwatch.StartNew();
			FindConversationType findConversationType = EwsClientHelper.Convert<FindConversationRequest, FindConversationType>(new FindConversationRequest
			{
				ConversationShape = this.GetResponseShape(),
				QueryString = base.Request.QueryString,
				SortOrder = base.Request.SortOrder,
				ViewFilter = base.Request.ViewFilter,
				ParentFolderId = new TargetFolderId(new DistinguishedFolderId
				{
					Id = DistinguishedFolderIdName.archivemsgfolderroot
				}),
				Traversal = ConversationQueryTraversal.Deep
			});
			Exception ex = null;
			FindConversationResponseMessageType findConversationResponseMessageType = null;
			bool flag = EwsClientHelper.ExecuteEwsCall(delegate
			{
				findConversationResponseMessageType = serviceBinding.FindConversation(findConversationType);
			}, out ex);
			if (!flag)
			{
				base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.ExecuteRemoteArchiveSearchFailed, true);
				ServiceError error = new ServiceError((CoreResources.IDs)2535285679U, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2012);
				ExTraceGlobals.SearchTracer.TraceError<string, string>((long)this.GetHashCode(), "[FindConversation::ExecuteRemoteArchiveSearch] Search against URL {0} failed with error: {1}.", serviceBinding.Url, ex.Message);
				return new ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]>(error);
			}
			FindConversationResponseMessage findConversationResponseMessage = EwsClientHelper.Convert<FindConversationResponseMessageType, FindConversationResponseMessage>(findConversationResponseMessageType);
			base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.ExecuteRemoteArchiveSearchFailed, findConversationResponseMessage.ResponseClass != ResponseClass.Success);
			if (findConversationResponseMessage.ResponseClass == ResponseClass.Success)
			{
				ExDateTime dateTime = utcNow.AddTicks(stopwatch.Elapsed.Ticks);
				base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.ExecuteRemoteArchiveSearchStartTimestamp, SearchUtil.FormatIso8601String(utcNow));
				base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.ExecuteRemoteArchiveSearchEndTimestamp, SearchUtil.FormatIso8601String(dateTime));
				return new ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]>(findConversationResponseMessage.Conversations);
			}
			ServiceError error2 = new ServiceError((CoreResources.IDs)2535285679U, findConversationResponseMessage.ResponseCode, findConversationResponseMessage.DescriptiveLinkKey, ExchangeVersion.Exchange2012);
			ExTraceGlobals.SearchTracer.TraceError((long)this.GetHashCode(), "[FindConversation::ExecuteRemoteArchiveSearch] Search against URL failed {0} with response class {1}, code {2} and message {3}.", new object[]
			{
				serviceBinding.Url,
				findConversationResponseMessage.ResponseClass,
				findConversationResponseMessage.ResponseCode,
				findConversationResponseMessage.MessageText
			});
			return new ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]>(error2);
		}

		private static readonly string FindConversationActionName = typeof(FindConversation).Name;

		private readonly string fromFilter;

		private Microsoft.Exchange.Services.Core.Search.BasePagingType paging;

		private TargetFolderId currentFolder;

		private ConversationQueryTraversal traversalType;

		private ViewFilter viewFilter;

		private ClutterFilter clutterFilter;

		private SortResults[] sortResults;

		private SortBy[] sortBy;

		private PropertyListForViewRowDeterminer determiner;

		private QueryFilter queryFilter;

		private string queryString;

		private Microsoft.Exchange.Services.Core.Types.HighlightTermType[] highlightTerms;

		private List<PropertyDefinition> fetchList;

		private int? totalConversationsInView;

		private int? indexedOffset;

		private bool isSearchInProgress;

		private ConversationResponseShape shape;

		private SearchScope searchScope;

		private MailboxSearchLocation mailboxScope;

		private FolderId searchFolderId;

		private delegate ServiceResult<Microsoft.Exchange.Services.Core.Types.ConversationType[]> ExecuteArchiveSearchDelegate(IExchangePrincipal primaryPrincipal);
	}
}
