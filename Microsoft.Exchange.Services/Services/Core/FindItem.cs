using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class FindItem : MultiStepServiceCommand<FindItemRequest, FindItemParentWrapper>
	{
		public FindItem(CallContext callContext, FindItemRequest request) : base(callContext, request)
		{
			OwsLogRegistry.Register(FindItem.FindItemActionName, typeof(FindConversationAndItemMetadata), new Type[0]);
			this.InitializeTracers();
		}

		internal override int StepCount
		{
			get
			{
				if (!string.IsNullOrEmpty(this.queryString))
				{
					return 1;
				}
				return this.rootFolders.Count;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return FindItemResponse.CreateResponseForFindItem(base.Results, new FindItemResponse.CreateFindItemResponse(FindItemResponse.CreateResponse), this.highlightTerms, this.isSearchInProgress, this.searchFolderId);
		}

		internal override void PreExecuteCommand()
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "FindItem.PreExecuteCommand called");
			this.itemShape = Global.ResponseShapeResolver.GetResponseShape<ItemResponseShape>(base.Request.ShapeName, base.Request.ItemShape, base.CallContext.FeaturesManager);
			this.paging = base.Request.Paging;
			this.grouping = base.Request.Grouping;
			this.restriction = base.Request.Restriction;
			this.queryString = ((base.Request.QueryString != null) ? base.Request.QueryString.Value : null);
			this.viewFilter = base.Request.ViewFilter;
			this.fromFilter = base.Request.FromFilter;
			this.clutterFilter = base.Request.ClutterFilter;
			this.searchFolderId = (base.Request.SearchFolderId as FolderId);
			this.sortResults = base.Request.SortOrder;
			this.rootFolders = base.Request.ParentFolderIds;
			this.traversalType = base.Request.Traversal;
			ServiceCommandBase.ThrowIfNull(this.itemShape, "itemShape", "FindItem::Execute");
			ServiceCommandBase.ThrowIfNullOrEmpty<BaseFolderId>(this.rootFolders, "rootFolders", "FindItem::Execute");
			this.ValidateRequest();
			this.sortBy = SortResults.ToXsoSortBy(this.sortResults);
			base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.QueryString, string.IsNullOrEmpty(this.queryString) ? string.Empty : this.queryString);
			base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.ViewFilter, this.viewFilter);
			if (SearchUtil.IsNormalView(base.Request.QueryString, this.viewFilter, this.clutterFilter))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "FindItem.PreExecuteCommand: IsNormalView is set to true.");
				ServiceObjectToFilterConverter serviceObjectToFilterConverter = new ServiceObjectToFilterConverter();
				this.queryFilter = ((this.restriction != null && this.restriction.Item != null) ? serviceObjectToFilterConverter.Convert(this.restriction.Item) : SearchUtil.GetViewQueryFilter(this.viewFilter));
				if (!string.IsNullOrEmpty(this.fromFilter))
				{
					this.tracer.TraceDebug<string>((long)this.GetHashCode(), "FindItem.PreExecuteCommand: FromFilter value is {0}.", this.fromFilter);
					QueryFilter itemQueryFilter = PeopleIKnowQuery.GetItemQueryFilter(this.fromFilter);
					this.queryFilter = SearchUtil.BuildAndFilter(this.queryFilter, itemQueryFilter);
					this.sortBy = PeopleIKnowQuery.GetItemQuerySortBy(this.sortBy);
				}
				this.queryFilter = BasePagingType.ApplyQueryAppend(this.queryFilter, this.paging);
			}
			else
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "FindItem.PreExecuteCommand: IsNormalView is set to false.");
				this.fastSearchIdAndSession = this.GetSessionFromFolder(this.rootFolders[0]);
				this.searchScope = SearchScope.SelectedAndSubfolders;
				if (this.traversalType == ItemQueryTraversal.Shallow)
				{
					this.searchScope = SearchScope.SelectedFolder;
				}
				if (SearchUtil.IsSearch(base.Request.QueryString))
				{
					QueryFilter filter = SearchFilterGenerator.Execute(this.queryString, CallContext.Current.ClientCulture, null);
					if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Ews.InstantSearchFoldersForPublicFolders.Enabled && this.fastSearchIdAndSession != null && this.fastSearchIdAndSession.Session is PublicFolderSession && this.fastSearchIdAndSession.Session.LogonType != LogonType.SystemService)
					{
						this.queryFilter = new CountFilter(200U, filter);
					}
					else
					{
						this.queryFilter = filter;
					}
					if (base.Request.QueryString.ReturnHighlightTerms)
					{
						List<HighlightTermType> list = new List<HighlightTermType>();
						SearchUtil.BuildHighlightTerms(this.queryFilter, list);
						if (list.Count > 0)
						{
							this.highlightTerms = list.ToArray();
						}
					}
				}
			}
			this.LogQueryFilter();
		}

		internal override ServiceResult<FindItemParentWrapper> Execute()
		{
			if (SearchUtil.IsNormalView(base.Request.QueryString, this.viewFilter, this.clutterFilter))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "FindItem.Execute: IsNormalView is set to true");
				IdAndSession idAndSession = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(this.rootFolders[base.CurrentStep]);
				base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.MailboxTarget, base.GetMailboxTarget(idAndSession.Session).ToString());
				if (this.paging is CalendarPageView && !ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
				{
					idAndSession.Session.ExTimeZone = ExTimeZone.CurrentTimeZone;
				}
				return this.FindItemsInParent(idAndSession);
			}
			this.tracer.TraceDebug((long)this.GetHashCode(), "FindItem.Execute: IsNormalView is set to false");
			return this.DoFastSearchOrFilteredView();
		}

		protected override void LogTracesForCurrentRequest()
		{
			ServiceCommandBase.TraceLoggerFactory.Create(base.CallContext.HttpContext.Response.Headers).LogTraces(this.requestTracer);
		}

		private void InitializeTracers()
		{
			ITracer tracer;
			if (!base.IsRequestTracingEnabled)
			{
				ITracer instance = NullTracer.Instance;
				tracer = instance;
			}
			else
			{
				tracer = new InMemoryTracer(ExTraceGlobals.FindItemCallTracer.Category, ExTraceGlobals.FindItemCallTracer.TraceTag);
			}
			this.requestTracer = tracer;
			this.tracer = ExTraceGlobals.FindItemCallTracer.Compose(this.requestTracer);
		}

		private ServiceResult<FindItemParentWrapper> DoFastSearchOrFilteredView()
		{
			if (!(this.grouping is NoGrouping))
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorNoGroupingForQueryString);
			}
			MailboxTarget mailboxTarget = base.GetMailboxTarget(this.fastSearchIdAndSession.Session);
			base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.MailboxTarget, mailboxTarget.ToString());
			ServiceResult<FindItemParentWrapper> result = null;
			if (mailboxTarget != MailboxTarget.PublicFolder && mailboxTarget != MailboxTarget.SharedFolder)
			{
				MailboxSession mailboxSession = this.fastSearchIdAndSession.Session as MailboxSession;
				try
				{
					if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
					{
						mailboxSession.EnablePrivateItemsFilter();
					}
					List<StoreId> folderScope = this.BuildQueryFilterAndFolderScope();
					SearchFolderCriteria searchFolderCriteria = SearchUtil.BuildSearchCriteria(folderScope, this.queryFilter, this.searchScope, base.Request.QueryString);
					StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.SearchFolders);
					if (CallContext.Current.OwaCallback == null)
					{
						result = this.DoFastSearch(defaultFolderId, searchFolderCriteria);
					}
					else
					{
						result = this.OwaDoFilteredViewOrFastSearch(this.fastSearchIdAndSession, defaultFolderId, this.queryFilter, searchFolderCriteria);
					}
				}
				finally
				{
					if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
					{
						mailboxSession.DisablePrivateItemsFilter();
					}
				}
				return result;
			}
			if (this.rootFolders.Count != 1)
			{
				CoreResources.IDs ds = (mailboxTarget == MailboxTarget.SharedFolder) ? ((CoreResources.IDs)2280668014U) : CoreResources.IDs.ErrorPublicFolderSearchNotSupportedOnMultipleFolders;
				throw new ServiceInvalidOperationException(ds);
			}
			return this.DoFastSearchForPublicOrSharedFolder();
		}

		private ServiceResult<FindItemParentWrapper> DoFastSearchForPublicOrSharedFolder()
		{
			ExTraceGlobals.SearchTracer.TraceDebug((long)this.GetHashCode(), "[FindItem::DoFastSearchForPublicOrSharedFolder] Executing fast search on Public Folder / shared folder.");
			ServiceResult<FindItemParentWrapper> result;
			using (Folder folder = Folder.Bind(this.fastSearchIdAndSession.Session, this.fastSearchIdAndSession.Id))
			{
				this.queryFilter = SearchUtil.BuildQueryFilter(this.queryFilter, this.viewFilter, this.clutterFilter, base.Request.RefinerRestriction, this.fromFilter);
				result = this.FindItemsInFolder(folder, this.queryFilter);
			}
			return result;
		}

		private ServiceResult<FindItemParentWrapper> DoFastSearch(StoreObjectId searchFoldersRootId, SearchFolderCriteria searchFolderCriteria)
		{
			MailboxSession mailboxSession = this.fastSearchIdAndSession.Session as MailboxSession;
			ServiceResult<FindItemParentWrapper> result = null;
			ExTraceGlobals.SearchTracer.TraceDebug((long)this.GetHashCode(), "[FindItem::DoFastSearch] Executing non-OWA fast search.");
			using (SearchFolder searchFolder = SearchUtil.CreateStaticSearchFolder(mailboxSession, searchFoldersRootId, searchFolderCriteria))
			{
				if (searchFolder == null)
				{
					result = this.GetErrorServiceResult();
				}
				else
				{
					result = this.FindItemsInSearchFolder(searchFolder);
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

		private ServiceResult<FindItemParentWrapper> OwaDoFilteredViewOrFastSearch(IdAndSession fastSearchIdAndSession, StoreObjectId searchFoldersRootId, QueryFilter queryFilter, SearchFolderCriteria searchFolderCriteria)
		{
			MailboxSession mailboxSession = fastSearchIdAndSession.Session as MailboxSession;
			ServiceResult<FindItemParentWrapper> result = null;
			ExTraceGlobals.SearchTracer.TraceDebug((long)this.GetHashCode(), "[FindItem::DoFastSearch] Executing OWA fast search.");
			OwaSearchContext owaSearchContext = new OwaSearchContext();
			owaSearchContext.SearchContextType = SearchContextType.ItemSearch;
			owaSearchContext.ClientSearchFolderIdentity = base.Request.SearchFolderIdentity;
			owaSearchContext.IsFilteredView = SearchUtil.IsFilteredView(base.Request.QueryString, this.viewFilter, this.clutterFilter, this.fromFilter);
			owaSearchContext.IsResetCache = (base.Request.QueryString != null && base.Request.QueryString.ResetCache);
			owaSearchContext.WaitForSearchComplete = (base.Request.QueryString != null && base.Request.QueryString.WaitForSearchComplete);
			owaSearchContext.SearchScope = this.searchScope;
			owaSearchContext.SearchSortBy = this.sortBy;
			owaSearchContext.SearchQueryFilter = this.queryFilter;
			owaSearchContext.SearchQueryFilterString = this.queryFilter.ToString();
			owaSearchContext.FromFilter = this.fromFilter;
			owaSearchContext.ViewFilter = (OwaViewFilter)SearchUtil.GetViewFilterForSearchFolder(this.viewFilter, this.clutterFilter);
			owaSearchContext.FolderIdToSearch = fastSearchIdAndSession.Id;
			owaSearchContext.SearchTimeoutInMilliseconds = Global.SearchTimeoutInMilliseconds;
			owaSearchContext.MaximumTemporaryFilteredViewPerUser = Global.MaximumTemporaryFilteredViewPerUser;
			owaSearchContext.IsWarmUpSearch = base.Request.IsWarmUpSearch;
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
					ServiceError error = new ServiceError(CoreResources.IDs.ErrorSearchFolderNotInitialized, ResponseCodeType.ErrorSearchFolderNotInitialized, 0, ExchangeVersion.Exchange2012);
					result = new ServiceResult<FindItemParentWrapper>(error);
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
					result = this.FindItemsInSearchFolder(searchFolder);
					ExDateTime dateTime = utcNow.AddTicks(stopwatch.Elapsed.Ticks);
					StorePerformanceCounters storePerformanceCounters = storePerformanceCountersCapture.Stop();
					base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.AggregateDataTime, storePerformanceCounters.ElapsedMilliseconds);
					base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.AggregateDataCPUTime, storePerformanceCounters.Cpu);
					base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.AggregateDataRpcCount, storePerformanceCounters.RpcCount);
					base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.AggregateDataRpcLatency, storePerformanceCounters.RpcLatency);
					base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.AggregateDataRpcLatencyOnStore, storePerformanceCounters.RpcLatencyOnStore);
					base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.AggregateDataStartTimestamp, SearchUtil.FormatIso8601String(utcNow));
					base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.AggregateDataEndTimestamp, SearchUtil.FormatIso8601String(dateTime));
				}
			}
			return result;
		}

		private ServiceResult<FindItemParentWrapper> GetErrorServiceResult()
		{
			ExTraceGlobals.SearchTracer.TraceDebug((long)this.GetHashCode(), "[FindItem::GetErrorServiceResult] returning service error.");
			ServiceError error = new ServiceError(CoreResources.IDs.ErrorSearchTimeoutExpired, ResponseCodeType.ErrorTimeoutExpired, 0, ExchangeVersion.Exchange2010);
			return new ServiceResult<FindItemParentWrapper>(error);
		}

		private List<StoreId> BuildQueryFilterAndFolderScope()
		{
			MailboxSession mailboxSession = this.fastSearchIdAndSession.Session as MailboxSession;
			List<StoreId> list = new List<StoreId>();
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.JunkEmail);
			StoreObjectId defaultFolderId2 = mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems);
			Guid mailboxGuid = mailboxSession.MailboxGuid;
			bool includeJunkItems = false;
			bool flag = false;
			foreach (BaseFolderId folderNode in this.rootFolders)
			{
				IdAndSession sessionFromFolder = this.GetSessionFromFolder(folderNode);
				MailboxSession mailboxSession2 = sessionFromFolder.Session as MailboxSession;
				if (!object.Equals(mailboxGuid, mailboxSession2.MailboxGuid))
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)2656700117U);
				}
				StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(sessionFromFolder.Id);
				if (asStoreObjectId.Equals(mailboxSession2.GetDefaultFolderId(DefaultFolderType.FromFavoriteSenders)))
				{
					list.Add(mailboxSession2.GetDefaultFolderId(DefaultFolderType.Inbox));
					this.queryFilter = SearchUtil.BuildAndFilter(this.queryFilter, FromFavoriteSendersFolderValidation.SearchQueryFilter);
				}
				else
				{
					list.Add(sessionFromFolder.Id);
				}
				if (sessionFromFolder.Id.Equals(defaultFolderId) || this.searchScope == SearchScope.SelectedFolder)
				{
					includeJunkItems = true;
				}
				if (sessionFromFolder.Id.Equals(defaultFolderId2) || this.searchScope == SearchScope.SelectedFolder)
				{
					flag = true;
				}
			}
			if (this.searchScope != SearchScope.SelectedFolder && flag && this.traversalType != ItemQueryTraversal.SoftDeleted)
			{
				flag = false;
			}
			if (SearchUtil.ShouldReturnDeletedItems(base.Request.QueryString))
			{
				flag = true;
			}
			this.queryFilter = SearchUtil.BuildQueryFilter(this.queryFilter, defaultFolderId, defaultFolderId2, includeJunkItems, flag, this.viewFilter, this.clutterFilter, base.Request.RefinerRestriction, this.fromFilter);
			return list;
		}

		private ServiceResult<FindItemParentWrapper> FindItemsInParent(IdAndSession folderIdAndSession)
		{
			Folder folder = null;
			bool flag = this.traversalType == ItemQueryTraversal.SoftDeleted && folderIdAndSession.Session is PublicFolderSession;
			ServiceResult<FindItemParentWrapper> result;
			try
			{
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012) && folderIdAndSession.Session is MailboxSession)
				{
					((MailboxSession)folderIdAndSession.Session).EnablePrivateItemsFilter();
				}
				using (Folder folder2 = Folder.Bind(folderIdAndSession.Session, folderIdAndSession.Id, null))
				{
					if (this.paging is SeekToConditionPageView && ObjectClass.IsCalendarFolder(folder2.ClassName))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "[FindItem::FindItemsInParent] SeekToConditionPageView is not supported for calendar items.");
						throw new CalendarExceptionSeekToConditionNotSupported();
					}
					folder = folder2;
					ItemQueryTraversal traversal = this.traversalType;
					if (flag)
					{
						StoreObjectId recoverableItemsDeletionsFolderId = PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId(folder2);
						if (recoverableItemsDeletionsFolderId == null)
						{
							this.tracer.TraceDebug((long)this.GetHashCode(), "[FindItem::FindItemsInParent] Dumpster folder not found.");
							throw new ServiceInvalidOperationException((CoreResources.IDs)3395659933U);
						}
						folder = Folder.Bind(folderIdAndSession.Session, recoverableItemsDeletionsFolderId, null);
						traversal = ItemQueryTraversal.Shallow;
					}
					this.DeterminePropertiesToFetch(folder);
					BasePageResult basePageResult = this.grouping.IssueQuery(this.queryFilter, folder, this.sortBy, this.paging, traversal, this.propsToFetch, (base.CallContext == null) ? null : base.CallContext.ProtocolLog);
					FindItemParentWrapper findItemParentWrapper = basePageResult.View.ConvertToFindItemParentWrapper(this.propsToFetch, this.classDeterminer, folderIdAndSession, basePageResult, this.grouping.QueryType);
					base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.TotalRowCount, findItemParentWrapper.TotalItemsInView);
					this.tracer.TraceDebug<int>((long)this.GetHashCode(), "[FindItem::FindItemsInParent] Total Number of items returned is {0}", findItemParentWrapper.TotalItemsInView);
					result = new ServiceResult<FindItemParentWrapper>(findItemParentWrapper);
				}
			}
			finally
			{
				if (flag && folder != null)
				{
					folder.Dispose();
				}
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012) && folderIdAndSession.Session is MailboxSession)
				{
					((MailboxSession)folderIdAndSession.Session).DisablePrivateItemsFilter();
				}
			}
			return result;
		}

		private ServiceResult<FindItemParentWrapper> FindItemsInSearchFolder(SearchFolder searchFolder)
		{
			QueryFilter filter;
			if (SearchUtil.IsComplexClutterFilteredView(this.viewFilter, this.clutterFilter))
			{
				filter = SearchUtil.GetViewQueryForComplexClutterFilteredView(this.clutterFilter, false);
			}
			else
			{
				filter = new TrueFilter();
			}
			return this.FindItemsInFolder(searchFolder, filter);
		}

		private ServiceResult<FindItemParentWrapper> FindItemsInFolder(Folder folder, QueryFilter filter)
		{
			this.DeterminePropertiesToFetch(folder);
			BasePageResult basePageResult = this.grouping.IssueQuery(filter, folder, this.sortBy, this.paging, ItemQueryTraversal.Shallow, this.propsToFetch, (base.CallContext == null) ? null : base.CallContext.ProtocolLog);
			IdAndSession idAndSession;
			if (this.fastSearchIdAndSession.Session is PublicFolderSession)
			{
				idAndSession = new IdAndSession(folder.Id.ObjectId, this.fastSearchIdAndSession.ParentFolderId, this.fastSearchIdAndSession.Session);
			}
			else
			{
				idAndSession = new IdAndSession(folder.Id.ObjectId, this.fastSearchIdAndSession.Session);
			}
			ItemType[] items = basePageResult.View.ConvertToItems(this.propsToFetch, this.classDeterminer, idAndSession);
			FindItemParentWrapper findItemParentWrapper = new FindItemParentWrapper(items, basePageResult);
			base.SafeSetProtocolLogMetadata(FindConversationAndItemMetadata.TotalRowCount, findItemParentWrapper.TotalItemsInView);
			return new ServiceResult<FindItemParentWrapper>(findItemParentWrapper);
		}

		private void DeterminePropertiesToFetch(Folder folder)
		{
			this.classDeterminer = PropertyListForViewRowDeterminer.BuildForItems(this.itemShape, folder);
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			list.AddRange(this.classDeterminer.GetPropertiesToFetch());
			PropertyDefinition[] additionalFetchProperties = this.grouping.GetAdditionalFetchProperties();
			if (additionalFetchProperties != null)
			{
				list.AddRange(additionalFetchProperties);
			}
			this.propsToFetch = list.ToArray();
			this.tracer.TraceDebug<Type, ItemResponseShape, int>((long)this.GetHashCode(), "FindItem.DeterminePropertiesToFetch: Folder.GetType() = {0}, this.itemShape = {1}, this.propsToFetch.Length = {2}", folder.GetType(), this.itemShape, this.propsToFetch.Length);
		}

		private void ValidateRequest()
		{
			int num;
			if ((this.paging == null || !this.paging.BudgetInducedTruncationAllowed) && !CallContext.Current.Budget.CanAllocateFoundObjects(1U, out num))
			{
				ExceededFindCountLimitException.Throw();
			}
			if (!string.IsNullOrEmpty(this.queryString))
			{
				if (this.restriction != null)
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "[FindItem::FindItems] You can not specify both restriction and query string.");
					throw new ServiceInvalidOperationException((CoreResources.IDs)2329210449U);
				}
				if (this.traversalType == ItemQueryTraversal.Associated)
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "[FindItem::FindItems] You cannot specify associated traversal when using a query string.");
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorAssociatedTraversalDisallowedWithQueryString);
				}
				if (CallContext.Current.OwaCallback != null && this.rootFolders.Count > 1)
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "[FindItem::FindItems] You cannot specify multiple search roots when using search context.");
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorMultipleSearchRootsDisallowedWithSearchContext);
				}
			}
			if (SearchUtil.IsFilteredView(base.Request.QueryString, this.viewFilter, this.clutterFilter, this.fromFilter))
			{
				if (this.restriction != null)
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "[FindItem::FindItems] You can not specify both view filter and restriction.");
					throw new ServiceInvalidOperationException((CoreResources.IDs)2675632227U);
				}
				if (this.traversalType == ItemQueryTraversal.Associated)
				{
					this.tracer.TraceDebug((long)this.GetHashCode(), "[FindItem::FindItems] You cannot specify associated traversal when using a view filter.");
					throw new ServiceInvalidOperationException((CoreResources.IDs)3735354645U);
				}
			}
			BasePagingType.Validate(this.paging);
			if (this.paging is CalendarPageView && (this.restriction != null || this.sortResults != null))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "[FindItem::FindItems] You cannot specify a restriction when using CalendarPageView");
				throw new ServiceInvalidOperationException((CoreResources.IDs)2358398289U);
			}
			if (this.traversalType == ItemQueryTraversal.Associated)
			{
				if (this.paging is CalendarPageView)
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)3789879302U);
				}
				if (this.paging is ContactsPageView)
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)3954262679U);
				}
				if (!(this.grouping is NoGrouping))
				{
					if (this.grouping is GroupByType)
					{
						throw new ServiceInvalidOperationException((CoreResources.IDs)3732945645U);
					}
					if (this.grouping is DistinguishedGroupByType)
					{
						throw new ServiceInvalidOperationException((CoreResources.IDs)3782996725U);
					}
				}
			}
		}

		private IdAndSession GetSessionFromFolder(BaseFolderId folderNode)
		{
			return base.IdConverter.ConvertFolderIdToIdAndSession(this.rootFolders[0], IdConverter.ConvertOption.IgnoreChangeKey);
		}

		private void LogQueryFilter()
		{
			string text = (this.queryFilter != null) ? this.queryFilter.ToString() : string.Empty;
			this.tracer.TraceDebug<string>((long)this.GetHashCode(), "FindItem.LogQueryFilter: this.filterString value is {0}", text);
			bool flag = !string.IsNullOrEmpty(this.queryString);
			base.SafeAppendLogGenericInfo("FindItemQueryFilter", text);
			base.SafeAppendLogGenericInfo("AQS", flag);
		}

		private static readonly string FindItemActionName = typeof(FindItem).Name;

		private ITracer tracer = ExTraceGlobals.FindItemCallTracer;

		private ITracer requestTracer = NullTracer.Instance;

		private ItemResponseShape itemShape;

		private BasePagingType paging;

		private BaseGroupByType grouping;

		private RestrictionType restriction;

		private string queryString;

		private HighlightTermType[] highlightTerms;

		private IList<BaseFolderId> rootFolders;

		private ItemQueryTraversal traversalType;

		private PropertyListForViewRowDeterminer classDeterminer;

		private PropertyDefinition[] propsToFetch;

		private QueryFilter queryFilter;

		private SortResults[] sortResults;

		private SortBy[] sortBy;

		private IdAndSession fastSearchIdAndSession;

		private SearchScope searchScope;

		private ViewFilter viewFilter;

		private bool isSearchInProgress;

		private FolderId searchFolderId;

		private string fromFilter;

		private ClutterFilter clutterFilter;
	}
}
