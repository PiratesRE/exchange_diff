using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SearchMailboxes : MultiStepServiceCommand<SearchMailboxesRequest, SearchMailboxesData>, IDisposeTrackable, IDisposable
	{
		public SearchMailboxes(CallContext callContext, SearchMailboxesRequest request) : base(callContext, request)
		{
			this.disposeTracker = this.GetDisposeTracker();
			if (MailboxSearchFlighting.IsFlighted(callContext, "SearchMailboxes", out this.policy))
			{
				CallContext.Current.ProtocolLog.AppendGenericInfo("SearchStartTime", ExDateTime.UtcNow);
				this.isFlighted = true;
				this.stepCount = 1;
				return;
			}
			this.requestId = MailboxSearchHelper.GetQueryCorrelationId();
			ExTraceGlobals.SearchTracer.TraceInformation<Guid, string>(this.GetHashCode(), 0L, "Correlation Id:{0}. Executing search with client id: {1}", this.requestId, ActivityContext.GetCurrentActivityScope().GetProperty(ActivityStandardMetadata.ClientRequestId));
			CallContext.Current.ProtocolLog.AppendGenericInfo("DiscoveryCorrelationId", this.requestId);
			this.SaveRequestData(request);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SearchMailboxes>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal override int StepCount
		{
			get
			{
				return this.stepCount;
			}
		}

		private bool HasAdditionalProperties
		{
			get
			{
				return this.additionalExtendedProperties != null && this.additionalExtendedProperties.Length > 0;
			}
		}

		internal override void PreExecuteCommand()
		{
			if (this.isFlighted)
			{
				return;
			}
			if (this.mailboxQueries != null && this.mailboxQueries.Length > Factory.Current.MaxAllowedMailboxQueriesPerRequest)
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)3784063568U, new InvalidOperationException(CoreResources.TooManyMailboxQueryObjects(this.mailboxQueries.Length, Factory.Current.MaxAllowedMailboxQueriesPerRequest)));
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			MailboxSearchHelper.PerformCommonAuthorization(base.CallContext.IsExternalUser, out this.runspaceConfig, out this.recipientSession);
			stopwatch.Stop();
			CallContext.Current.ProtocolLog.AppendGenericInfo("AuthorizationTimeTaken", stopwatch.ElapsedMilliseconds);
			if (this.recipientSession != null && !Factory.Current.IsDiscoverySearchEnabled(this.recipientSession))
			{
				ExTraceGlobals.SearchTracer.TraceInformation(this.GetHashCode(), 0L, "Discovery Searches are disabled.");
				throw new DiscoverySearchesDisabledException();
			}
			if (this.mailboxQueries == null)
			{
				this.SetMailboxQueryFromMailboxDiscoverySearch();
			}
		}

		internal override ServiceResult<SearchMailboxesData> Execute()
		{
			if (this.isFlighted)
			{
				this.response = MailboxSearchFlighting.SearchMailboxes(this.policy, base.Request);
				return new ServiceResult<SearchMailboxesData>(new SearchMailboxesData());
			}
			return this.ProcessRequest();
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			if (this.isFlighted)
			{
				if (this.response != null && this.response.ResponseMessages != null && this.response.ResponseMessages.Items != null && this.response.ResponseMessages.Items.Length > 0)
				{
					SearchMailboxesResponseMessage searchMailboxesResponseMessage = this.response.ResponseMessages.Items[0] as SearchMailboxesResponseMessage;
					if (searchMailboxesResponseMessage != null && searchMailboxesResponseMessage.SearchMailboxesResult != null)
					{
						CallContext.Current.ProtocolLog.AppendGenericInfo("ResultCount", searchMailboxesResponseMessage.SearchMailboxesResult.ItemCount);
						CallContext.Current.ProtocolLog.AppendGenericInfo("ResultSize", searchMailboxesResponseMessage.SearchMailboxesResult.Size);
						if (searchMailboxesResponseMessage.SearchMailboxesResult.MailboxStats != null)
						{
							CallContext.Current.ProtocolLog.AppendGenericInfo("MailboxesSearchedCount", searchMailboxesResponseMessage.SearchMailboxesResult.MailboxStats.Count<MailboxStatisticsItem>());
						}
					}
					CallContext.Current.ProtocolLog.AppendGenericInfo("SearchEndTime", ExDateTime.UtcNow);
				}
				return this.response;
			}
			List<ServiceResult<SearchMailboxesResult>> list = new List<ServiceResult<SearchMailboxesResult>>();
			List<MailboxQuery> list2 = new List<MailboxQuery>();
			List<MailboxQuery> list3 = new List<MailboxQuery>();
			ServiceError error = null;
			ResultAggregator resultAggregator = new ResultAggregator();
			Stopwatch stopwatch = Stopwatch.StartNew();
			List<FailedSearchMailbox> list4 = new List<FailedSearchMailbox>();
			int num = 0;
			ServiceResult<SearchMailboxesData>[] results = base.Results;
			for (int i = 0; i < results.Length; i++)
			{
				ServiceResult<SearchMailboxesData> sr = results[i];
				if (sr == null)
				{
					ExTraceGlobals.SearchTracer.TraceError<Guid>(this.GetHashCode(), 0L, "Correlation Id:{0}. The service result returned from Execute should not be null.", this.requestId);
					ServiceCommandBase.ThrowIfNull(sr, "sr", "SearchMailboxes::GetResponse");
				}
				else if (sr.Code == ServiceResultCode.Success)
				{
					if (sr.Value.ResultAggregator != null)
					{
						list2.Add(sr.Value.MailboxQuery);
						resultAggregator.MergeSearchResult(sr.Value.ResultAggregator);
					}
					if (sr.Value.NonSearchableMailboxes != null)
					{
						list4.AddRange(sr.Value.NonSearchableMailboxes);
					}
				}
				else
				{
					error = sr.Error;
					list3.Add(this.mailboxQueries[num]);
					MailboxSearchScope[] mailboxSearchScopes = this.mailboxQueries[num].MailboxSearchScopes;
					for (int j = 0; j < mailboxSearchScopes.Length; j++)
					{
						MailboxSearchScope scope = mailboxSearchScopes[j];
						if ((from x in list4
						where x.Mailbox.Equals(scope.Mailbox, StringComparison.OrdinalIgnoreCase) && x.ErrorMessage.Equals(sr.Error.MessageText, StringComparison.OrdinalIgnoreCase)
						select x).FirstOrDefault<FailedSearchMailbox>() == null)
						{
							list4.Add(new FailedSearchMailbox
							{
								Mailbox = scope.Mailbox,
								ErrorMessage = sr.Error.MessageText,
								IsArchive = (scope.SearchScope == MailboxSearchLocation.ArchiveOnly)
							});
						}
					}
				}
				num++;
			}
			if (list2.Count > 0)
			{
				SearchMailboxesResult value = this.CreateSearchMailboxesResult(list2.ToArray(), resultAggregator, list4);
				ServiceResult<SearchMailboxesResult> item = new ServiceResult<SearchMailboxesResult>(ServiceResultCode.Success, value, null);
				list.Insert(0, item);
			}
			else if (list3.Count > 0)
			{
				SearchMailboxesResult value2 = this.CreateSearchMailboxesResult(list3.ToArray(), null, list4);
				list.Add(new ServiceResult<SearchMailboxesResult>(ServiceResultCode.Error, value2, error));
			}
			SearchMailboxesResponse searchMailboxesResponse = new SearchMailboxesResponse();
			searchMailboxesResponse.AddResponses(list.ToArray());
			stopwatch.Stop();
			CallContext.Current.ProtocolLog.AppendGenericInfo("SearchResponseProcessingTime", stopwatch.ElapsedMilliseconds);
			return searchMailboxesResponse;
		}

		private void Dispose(bool fromDispose)
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			if (!this.disposed)
			{
				this.disposed = true;
			}
		}

		private void SaveRequestData(SearchMailboxesRequest request)
		{
			this.searchId = request.SearchId;
			this.mailboxId = request.MailboxId;
			this.mailboxQueries = request.SearchQueries;
			this.stepCount = ((this.mailboxQueries != null) ? this.mailboxQueries.Length : 1);
			this.resultType = request.ResultType;
			this.sortBy = request.SortBy;
			this.language = request.Language;
			this.performDeduplication = request.Deduplication;
			this.pageSize = ((request.PageSize <= 0) ? 100 : request.PageSize);
			this.pageItemReference = request.PageItemReference;
			this.pageDirection = ((request.PageDirection == SearchPageDirectionType.Previous) ? PageDirection.Previous : PageDirection.Next);
			this.baseShape = Microsoft.Exchange.Services.Core.Types.PreviewItemBaseShape.Default;
			if (request.PreviewItemResponseShape != null)
			{
				this.baseShape = request.PreviewItemResponseShape.BaseShape;
				this.additionalExtendedProperties = request.PreviewItemResponseShape.AdditionalProperties;
			}
		}

		private ServiceResult<SearchMailboxesData> ProcessRequest()
		{
			this.currentMailboxQuery = this.mailboxQueries[base.CurrentStep];
			if (string.IsNullOrEmpty(this.currentMailboxQuery.Query))
			{
				throw new ServiceArgumentException((CoreResources.IDs)2226875331U);
			}
			if (this.HasAdditionalProperties)
			{
				this.ProcessAdditionalExtendedProperties();
			}
			SearchType searchType = SearchType.Preview;
			if ((this.resultType & SearchResultType.StatisticsOnly) == SearchResultType.StatisticsOnly)
			{
				searchType = SearchType.Statistics;
			}
			else if ((this.resultType & SearchResultType.PreviewOnly) == SearchResultType.PreviewOnly)
			{
				searchType = SearchType.Preview;
			}
			int maxAllowedMailboxes = Factory.Current.GetMaxAllowedMailboxes(this.recipientSession, searchType);
			if (this.currentMailboxQuery.MailboxSearchScopes != null && !Factory.Current.IsSearchAllowed(this.recipientSession, searchType, this.currentMailboxQuery.MailboxSearchScopes.Length))
			{
				ExTraceGlobals.SearchTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. Max mailboxes allowed per search call is {1}, the {2} search request for the query:{3} contained {4} mailboxes to search on.", new object[]
				{
					this.requestId,
					maxAllowedMailboxes,
					(searchType == SearchType.Preview) ? "preview" : "statistics",
					this.currentMailboxQuery.Query,
					this.currentMailboxQuery.MailboxSearchScopes.Length
				});
				throw new TooManyMailboxesException(this.currentMailboxQuery.MailboxSearchScopes.Length, maxAllowedMailboxes);
			}
			int num = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			List<FailedSearchMailbox> nonSearchableMailboxes;
			List<MailboxInfo> list = this.ExpandAndFilterMailboxList(this.currentMailboxQuery, out num, out nonSearchableMailboxes);
			stopwatch.Stop();
			CallContext.Current.ProtocolLog.AppendGenericInfo("MailboxesSelectionTime", stopwatch.ElapsedMilliseconds);
			if (list == null || list.Count == 0)
			{
				ExTraceGlobals.SearchTracer.TraceError<Guid>((long)this.GetHashCode(), "Correlation Id:{0}. No mailbox to be searched after expanding and filtering the mailbox list.", this.requestId);
				throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorNoMailboxSpecifiedForSearchOperation);
			}
			if (!Factory.Current.IsSearchAllowed(this.recipientSession, searchType, num))
			{
				ExTraceGlobals.SearchTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. Max mailboxes allowed per search call is {1}, the {2} search request for the query:{3} contained {4} mailboxes to search on.", new object[]
				{
					this.requestId,
					maxAllowedMailboxes,
					(searchType == SearchType.Preview) ? "preview" : "statistics",
					this.currentMailboxQuery.Query,
					num
				});
				throw new TooManyMailboxesException(num, maxAllowedMailboxes);
			}
			CallContext.Current.ProtocolLog.AppendGenericInfo("QueryLength", this.currentMailboxQuery.Query.Length);
			CallContext.Current.ProtocolLog.AppendGenericInfo("MailboxesSearchedCount", list.Count);
			CallContext.Current.ProtocolLog.AppendGenericInfo("SearchType", this.resultType.ToString());
			CallContext.Current.ProtocolLog.AppendGenericInfo("PerformDeduplication", this.performDeduplication.ToString());
			CallContext.Current.ProtocolLog.AppendGenericInfo("SearchStartTime", ExDateTime.UtcNow);
			ResultAggregator resultAggregator = new ResultAggregator();
			stopwatch.Restart();
			ISearchResult searchResult = this.ExecuteSearch(list, this.currentMailboxQuery.Query, searchType);
			if (searchResult != null)
			{
				resultAggregator.MergeSearchResult(searchResult);
			}
			stopwatch.Stop();
			CallContext.Current.ProtocolLog.AppendGenericInfo("SearchEndTime", ExDateTime.UtcNow);
			CallContext.Current.ProtocolLog.AppendGenericInfo("SearchExecutionTime", stopwatch.ElapsedMilliseconds);
			foreach (KeyValuePair<string, object> keyValuePair in resultAggregator.ProtocolLog)
			{
				CallContext.Current.ProtocolLog.AppendGenericInfo(keyValuePair.Key, keyValuePair.Value.ToString());
			}
			return new ServiceResult<SearchMailboxesData>(new SearchMailboxesData
			{
				MailboxQuery = this.currentMailboxQuery,
				NonSearchableMailboxes = nonSearchableMailboxes,
				ResultAggregator = resultAggregator
			});
		}

		private List<MailboxInfo> ExpandAndFilterMailboxList(MailboxQuery mailboxQuery, out int totalMailboxesToSearch, out List<FailedSearchMailbox> nonSearchableMailboxes)
		{
			List<MailboxInfo> list = new List<MailboxInfo>();
			int num = 0;
			List<PropertyDefinition> list2 = new List<PropertyDefinition>(MailboxInfo.PropertyDefinitionCollection);
			list2.AddRange(MailboxSearchHelper.AdditionalProperties);
			PropertyDefinition[] properties = list2.ToArray();
			nonSearchableMailboxes = new List<FailedSearchMailbox>(1);
			ICollection<string> source = from m in mailboxQuery.MailboxSearchScopes
			select m.Mailbox;
			this.dictADRawEntries = MailboxSearchHelper.FindADEntriesByLegacyExchangeDNs(this.recipientSession, source.ToArray<string>(), properties);
			foreach (MailboxSearchScope mailboxSearchScope in mailboxQuery.MailboxSearchScopes)
			{
				ADRawEntry adrawEntry = null;
				if (this.dictADRawEntries.TryGetValue(mailboxSearchScope.Mailbox, out adrawEntry))
				{
					if (MailboxSearchHelper.IsMembershipGroup(adrawEntry))
					{
						Dictionary<ADObjectId, ADRawEntry> dictionary = MailboxSearchHelper.ProcessGroupExpansion(this.recipientSession, adrawEntry, this.recipientSession.SessionSettings.CurrentOrganizationId, properties);
						using (Dictionary<ADObjectId, ADRawEntry>.Enumerator enumerator = dictionary.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<ADObjectId, ADRawEntry> keyValuePair = enumerator.Current;
								if (keyValuePair.Value != null)
								{
									this.VerifyAndAddADObjectIdToCollection(list, keyValuePair.Value, mailboxSearchScope.SearchScope, nonSearchableMailboxes, ref num);
								}
							}
							goto IL_172;
						}
					}
					this.VerifyAndAddADObjectIdToCollection(list, adrawEntry, mailboxSearchScope.SearchScope, nonSearchableMailboxes, ref num);
				}
				else
				{
					ExTraceGlobals.SearchTracer.TraceError<string, Guid>((long)this.GetHashCode(), "Correlation Id:{0}. This mailbox is not found in AD: {1}", mailboxSearchScope.Mailbox, this.requestId);
					nonSearchableMailboxes.Add(new FailedSearchMailbox(mailboxSearchScope.Mailbox, 0, CoreResources.ErrorSearchableObjectNotFound));
				}
				IL_172:;
			}
			totalMailboxesToSearch = num;
			return list;
		}

		private void VerifyAndAddADObjectIdToCollection(List<MailboxInfo> mailboxes, ADRawEntry recipient, MailboxSearchLocation searchScope, List<FailedSearchMailbox> nonSearchableMailboxes, ref int totalMailboxCount)
		{
			bool flag = MailboxSearchHelper.HasPermissionToSearchMailbox(this.runspaceConfig, recipient);
			bool flag2 = MailboxSearchHelper.IsValidRecipientType(recipient);
			bool flag3 = MailboxSearchHelper.HasValidVersion(recipient);
			if (flag && flag2 && flag3)
			{
				MailboxInfo primaryMailbox = null;
				MailboxInfo archiveMailbox = null;
				if (searchScope == MailboxSearchLocation.ArchiveOnly)
				{
					totalMailboxCount++;
				}
				if (searchScope == MailboxSearchLocation.All || searchScope == MailboxSearchLocation.PrimaryOnly)
				{
					primaryMailbox = new MailboxInfo(recipient, MailboxType.Primary);
					totalMailboxCount++;
				}
				if ((searchScope == MailboxSearchLocation.All || searchScope == MailboxSearchLocation.ArchiveOnly) && !Guid.Empty.Equals((Guid)recipient[ADUserSchema.ArchiveGuid]))
				{
					archiveMailbox = new MailboxInfo(recipient, MailboxType.Archive);
				}
				if (primaryMailbox != null)
				{
					if (!mailboxes.Any((MailboxInfo m) => m.Type == primaryMailbox.Type && string.Compare(m.OwnerId.DistinguishedName, primaryMailbox.OwnerId.DistinguishedName, StringComparison.CurrentCultureIgnoreCase) == 0))
					{
						mailboxes.Add(primaryMailbox);
					}
				}
				if (archiveMailbox != null)
				{
					if (!mailboxes.Any((MailboxInfo m) => m.Type == archiveMailbox.Type && string.Compare(m.OwnerId.DistinguishedName, archiveMailbox.OwnerId.DistinguishedName, StringComparison.CurrentCultureIgnoreCase) == 0))
					{
						mailboxes.Add(archiveMailbox);
						return;
					}
				}
			}
			else
			{
				string errorMessage = string.Empty;
				if (!flag2)
				{
					errorMessage = CoreResources.GetLocalizedString((CoreResources.IDs)3611326890U);
				}
				else if (!flag3)
				{
					errorMessage = CoreResources.GetLocalizedString(CoreResources.IDs.ErrorMailboxVersionNotSupported);
				}
				else if (!flag)
				{
					errorMessage = CoreResources.GetLocalizedString((CoreResources.IDs)2354781453U);
				}
				string mailbox = (string)recipient[ADRecipientSchema.LegacyExchangeDN];
				nonSearchableMailboxes.Add(new FailedSearchMailbox
				{
					Mailbox = mailbox,
					ErrorMessage = errorMessage
				});
			}
		}

		private ISearchResult ExecuteSearch(List<MailboxInfo> mailboxes, string queryString, SearchType searchType)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, this.recipientSession.SessionSettings, 723, "ExecuteSearch", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\SearchMailboxes.cs");
			CultureInfo culture = CultureInfo.InvariantCulture;
			if (!string.IsNullOrEmpty(this.language))
			{
				try
				{
					culture = new CultureInfo(this.language);
				}
				catch (CultureNotFoundException)
				{
					ExTraceGlobals.SearchTracer.TraceError<string>((long)this.GetHashCode(), "Culture info: \"{0}\" returns CultureNotFoundException", this.language);
					throw new ServiceArgumentException(CoreResources.IDs.ErrorQueryLanguageNotValid);
				}
			}
			SearchCriteria criteria = null;
			try
			{
				criteria = new SearchCriteria(queryString, null, culture, searchType, this.recipientSession, tenantOrTopologyConfigurationSession, this.requestId, (this.policy != null && this.policy.ExecutionSettings != null) ? this.policy.ExecutionSettings.ExcludedFolders : new List<DefaultFolderType>());
			}
			catch (ParserException ex)
			{
				ExTraceGlobals.SearchTracer.TraceError<Guid, string, string>((long)this.GetHashCode(), "Correlation Id:{0}. Query: \"{1}\" returns ParserException: {2}", this.requestId, queryString, ex.ToString());
				throw new ServiceArgumentException((CoreResources.IDs)3021008902U);
			}
			catch (TooManyKeywordsException ex2)
			{
				ExTraceGlobals.SearchTracer.TraceError<Guid, string, string>((long)this.GetHashCode(), "Correlation Id:{0}. Query: \"{1}\" returns TooManyKeywordsException: {2}", this.requestId, queryString, ex2.ToString());
				throw new TooManyKeywordsException(ex2);
			}
			SortBy requestSortBy = this.GetRequestSortBy();
			ReferenceItem referenceItem = null;
			if (!string.IsNullOrEmpty(this.pageItemReference))
			{
				referenceItem = ReferenceItem.Parse(requestSortBy, this.pageItemReference);
			}
			PagingInfo pagingInfo = new PagingInfo(this.GetRequiredItemProperties(), requestSortBy, this.pageSize, this.pageDirection, referenceItem, MailboxSearchHelper.GetTimeZone(), this.performDeduplication, this.GetBaseShape(), this.GetExtendedPropertyInfoList());
			CallerInfo callerInfo = new CallerInfo(MailboxSearchHelper.IsOpenAsAdmin(base.CallContext), MailboxSearchConverter.GetCommonAccessToken(base.CallContext), base.CallContext.EffectiveCaller.ClientSecurityContext, base.CallContext.EffectiveCaller.PrimarySmtpAddress, this.recipientSession.SessionSettings.CurrentOrganizationId, base.CallContext.UserAgent, this.requestId, MailboxSearchHelper.GetUserRolesFromAuthZClientInfo(base.CallContext.EffectiveCaller), MailboxSearchHelper.GetApplicationRolesFromAuthZClientInfo(base.CallContext.EffectiveCaller));
			ISearchResult result;
			using (MultiMailboxSearch multiMailboxSearch = new MultiMailboxSearch(criteria, mailboxes, pagingInfo, callerInfo, this.recipientSession.SessionSettings.CurrentOrganizationId))
			{
				AsyncResult asyncResult = multiMailboxSearch.BeginSearch(null, null) as AsyncResult;
				TimeSpan defaultSearchTimeout = Factory.Current.GetDefaultSearchTimeout(this.recipientSession);
				CallContext.Current.ProtocolLog.AppendGenericInfo("SearchTimeoutInterval", defaultSearchTimeout.Minutes);
				bool flag = asyncResult.AsyncWaitHandle.WaitOne(defaultSearchTimeout);
				if (!flag)
				{
					multiMailboxSearch.AbortSearch();
				}
				CallContext.Current.ProtocolLog.AppendGenericInfo("SearchTimedOut", !flag);
				result = multiMailboxSearch.EndSearch(asyncResult);
			}
			return result;
		}

		private SearchPreviewItem ConvertPreviewItemToSearchPreviewItem(PreviewItem pi)
		{
			ItemId itemId = this.GetItemId(pi.Id, pi.MailboxGuid);
			SearchPreviewItem searchPreviewItem = new SearchPreviewItem
			{
				Id = itemId
			};
			if (this.baseShape == Microsoft.Exchange.Services.Core.Types.PreviewItemBaseShape.Compact || this.baseShape == Microsoft.Exchange.Services.Core.Types.PreviewItemBaseShape.Default)
			{
				searchPreviewItem.ParentId = this.GetItemId(pi.ParentItemId, pi.MailboxGuid);
				searchPreviewItem.Mailbox = this.GetMailbox(pi.MailboxGuid);
				searchPreviewItem.UniqueHash = ((pi.ItemHash != null) ? pi.ItemHash.ToString() : null);
				searchPreviewItem.SortValue = ((pi.SortValue != null) ? pi.SortValue.ToString() : null);
				searchPreviewItem.Size = new ulong?((ulong)((long)pi.Size));
			}
			if (this.baseShape == Microsoft.Exchange.Services.Core.Types.PreviewItemBaseShape.Default)
			{
				searchPreviewItem.ItemClass = pi.ItemClass;
				searchPreviewItem.Sender = pi.Sender;
				searchPreviewItem.Subject = pi.Subject;
				searchPreviewItem.ToRecipients = pi.ToRecipients;
				searchPreviewItem.CcRecipients = pi.CcRecipients;
				searchPreviewItem.BccRecipients = pi.BccRecipients;
				searchPreviewItem.CreatedTime = this.GetDateTimeAsString(pi.CreationTime);
				searchPreviewItem.ReceivedTime = this.GetDateTimeAsString(pi.ReceivedTime);
				searchPreviewItem.SentTime = this.GetDateTimeAsString(pi.SentTime);
				searchPreviewItem.Preview = pi.Preview;
				string owaLink = string.Empty;
				if (pi.OwaLink != null && !string.IsNullOrEmpty(pi.OwaLink.AbsoluteUri))
				{
					owaLink = LinkUtils.UpdateOwaLinkToItem(pi.OwaLink, itemId.Id).AbsoluteUri;
				}
				searchPreviewItem.OwaLink = owaLink;
				searchPreviewItem.Importance = this.GetImportance(pi.Importance).ToString();
				searchPreviewItem.Read = new bool?(pi.Read);
				searchPreviewItem.HasAttachment = new bool?(pi.HasAttachment);
			}
			if (this.HasAdditionalProperties)
			{
				searchPreviewItem.ExtendedProperties = this.ConvertPropertyValuesToExtendedProperties(pi.AdditionalPropertyValues);
			}
			return searchPreviewItem;
		}

		private SearchPreviewItem[] ProcessSearchResultRows(PreviewItem[] searchResultRows)
		{
			SearchPreviewItem[] array = new SearchPreviewItem[searchResultRows.Length];
			if (this.GetRequestSortBy().ColumnDefinition == ItemSchema.ReceivedTime && searchResultRows[searchResultRows.Length - 1].ReceivedTime == ExDateTime.MinValue && searchResultRows[0].ReceivedTime != ExDateTime.MinValue)
			{
				int num = 0;
				for (int i = 0; i < searchResultRows.Length; i++)
				{
					array[i] = this.ConvertPreviewItemToSearchPreviewItem(searchResultRows[i]);
					if (searchResultRows[i].ReceivedTime != ExDateTime.MinValue)
					{
						num = i;
					}
				}
				if (num != array.Length - 1)
				{
					SearchPreviewItem searchPreviewItem = array[num];
					array[num] = array[array.Length - 1];
					array[array.Length - 1] = searchPreviewItem;
				}
			}
			else
			{
				for (int j = 0; j < searchResultRows.Length; j++)
				{
					array[j] = this.ConvertPreviewItemToSearchPreviewItem(searchResultRows[j]);
				}
			}
			return array;
		}

		private List<PropertyDefinition> GetRequiredItemProperties()
		{
			if (this.itemProperties == null)
			{
				this.itemProperties = new List<PropertyDefinition>();
				if (this.baseShape == Microsoft.Exchange.Services.Core.Types.PreviewItemBaseShape.Compact)
				{
					this.itemProperties.AddRange(this.compactPreviewDataProperties);
				}
				else
				{
					this.itemProperties.AddRange(this.previewDataProperties);
				}
			}
			return this.itemProperties;
		}

		private void ProcessAdditionalExtendedProperties()
		{
			this.xsoEwsPropertiesMap = new Dictionary<PropertyDefinition, ExtendedPropertyUri>(this.additionalExtendedProperties.Count<ExtendedPropertyUri>());
			for (int i = 0; i < this.additionalExtendedProperties.Length; i++)
			{
				PropertyDefinition key = this.additionalExtendedProperties[i].ToPropertyDefinition();
				if (!this.xsoEwsPropertiesMap.ContainsKey(key))
				{
					this.xsoEwsPropertiesMap.Add(key, this.additionalExtendedProperties[i]);
				}
			}
		}

		private ExtendedPropertyType[] ConvertPropertyValuesToExtendedProperties(Dictionary<PropertyDefinition, object> propertyValues)
		{
			if (propertyValues == null)
			{
				return null;
			}
			List<ExtendedPropertyType> list = new List<ExtendedPropertyType>(propertyValues.Count);
			foreach (KeyValuePair<PropertyDefinition, object> keyValuePair in propertyValues)
			{
				if (keyValuePair.Value != null && !(keyValuePair.Value is PropertyError))
				{
					ExtendedPropertyUri propertyUri = this.xsoEwsPropertiesMap[keyValuePair.Key];
					ExtendedPropertyType item;
					if (keyValuePair.Value is Array)
					{
						item = new ExtendedPropertyType(propertyUri, this.ConvertObjectArrayToStringArray((object[])keyValuePair.Value));
					}
					else
					{
						item = new ExtendedPropertyType(propertyUri, keyValuePair.Value.ToString());
					}
					list.Add(item);
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			return list.ToArray();
		}

		private string[] ConvertObjectArrayToStringArray(object[] objs)
		{
			string[] array = new string[objs.Length];
			for (int i = 0; i < objs.Length; i++)
			{
				array[i] = (objs[i] as string);
			}
			return array;
		}

		private SortBy GetRequestSortBy()
		{
			PropertyDefinition propertyDefinition = ItemSchema.ReceivedTime;
			SortOrder sortOrder = SortOrder.Descending;
			if (this.sortBy != null && this.sortBy.SortByProperty != null)
			{
				PropertyUri propertyUri = this.sortBy.SortByProperty as PropertyUri;
				if (propertyUri == null)
				{
					throw new ServiceArgumentException((CoreResources.IDs)2566235088U);
				}
				propertyDefinition = this.GetPropertyDefinitionFromPropertyUri(propertyUri);
				if (propertyDefinition == null)
				{
					throw new ServiceArgumentException((CoreResources.IDs)2841035169U);
				}
				sortOrder = ((this.sortBy.Order == SortDirection.Descending) ? SortOrder.Descending : SortOrder.Ascending);
			}
			return new SortBy(propertyDefinition, sortOrder);
		}

		private Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch.PreviewItemBaseShape GetBaseShape()
		{
			if (this.baseShape == Microsoft.Exchange.Services.Core.Types.PreviewItemBaseShape.Compact)
			{
				return Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch.PreviewItemBaseShape.Compact;
			}
			return Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch.PreviewItemBaseShape.Default;
		}

		private List<ExtendedPropertyInfo> GetExtendedPropertyInfoList()
		{
			List<ExtendedPropertyInfo> list = null;
			if (this.xsoEwsPropertiesMap != null && this.xsoEwsPropertiesMap.Count > 0)
			{
				list = new List<ExtendedPropertyInfo>(this.xsoEwsPropertiesMap.Count);
				foreach (KeyValuePair<PropertyDefinition, ExtendedPropertyUri> keyValuePair in this.xsoEwsPropertiesMap)
				{
					list.Add(this.GetExtendedPropertyInfo(keyValuePair.Value, keyValuePair.Key));
				}
			}
			return list;
		}

		private ExtendedPropertyInfo GetExtendedPropertyInfo(ExtendedPropertyUri extendedPropertyUri, PropertyDefinition propertyDefinition)
		{
			ExtendedPropertyInfo extendedPropertyInfo = new ExtendedPropertyInfo();
			extendedPropertyInfo.XsoPropertyDefinition = propertyDefinition;
			extendedPropertyInfo.PropertySetId = extendedPropertyUri.PropertySetIdGuid;
			extendedPropertyInfo.PropertyName = extendedPropertyUri.PropertyName;
			extendedPropertyInfo.PropertyType = extendedPropertyUri.PropertyTypeString;
			if (extendedPropertyUri.PropertyIdSpecified)
			{
				extendedPropertyInfo.PropertyId = new int?(extendedPropertyUri.PropertyId);
			}
			if (!string.IsNullOrEmpty(extendedPropertyUri.PropertyTag))
			{
				extendedPropertyInfo.PropertyTagId = new int?((int)extendedPropertyUri.PropertyTagId);
			}
			return extendedPropertyInfo;
		}

		private PropertyDefinition GetPropertyDefinitionFromPropertyUri(PropertyUri propUri)
		{
			PropertyDefinition result = null;
			Schema schema = null;
			string uriString = propUri.UriString;
			if (uriString.StartsWith("folder:", StringComparison.OrdinalIgnoreCase))
			{
				schema = FolderSchema.GetSchema();
			}
			else if (uriString.StartsWith("item:", StringComparison.OrdinalIgnoreCase))
			{
				schema = ItemSchema.GetSchema();
			}
			else if (uriString.StartsWith("message:", StringComparison.OrdinalIgnoreCase))
			{
				schema = MessageSchema.GetSchema();
			}
			else if (uriString.StartsWith("meeting:", StringComparison.OrdinalIgnoreCase))
			{
				schema = MeetingMessageSchema.GetSchema();
			}
			else if (uriString.StartsWith("meetingRequest:", StringComparison.OrdinalIgnoreCase))
			{
				schema = MeetingRequestSchema.GetSchema();
			}
			else if (uriString.StartsWith("calendar:", StringComparison.OrdinalIgnoreCase))
			{
				schema = CalendarItemSchema.GetSchema(true);
			}
			else if (uriString.StartsWith("task:", StringComparison.OrdinalIgnoreCase))
			{
				schema = TaskSchema.GetSchema();
			}
			else if (uriString.StartsWith("contacts:", StringComparison.OrdinalIgnoreCase))
			{
				schema = ContactSchema.GetSchema();
			}
			else if (uriString.StartsWith("conversation:", StringComparison.OrdinalIgnoreCase))
			{
				schema = ConversationSchema.GetSchema();
			}
			else if (uriString.StartsWith("distributionlist:", StringComparison.OrdinalIgnoreCase))
			{
				schema = DistributionListSchema.GetSchema();
			}
			else if (uriString.StartsWith("postitem:", StringComparison.OrdinalIgnoreCase))
			{
				schema = PostItemSchema.GetSchema();
			}
			if (schema != null)
			{
				PropertyInformation propertyInformation = null;
				if (schema.TryGetPropertyInformationByPath(propUri, out propertyInformation))
				{
					PropertyDefinition[] propertyDefinitions = propertyInformation.GetPropertyDefinitions(null);
					if (propertyDefinitions != null && propertyDefinitions.Length > 0)
					{
						result = propertyDefinitions[0];
					}
				}
			}
			return result;
		}

		private ItemId GetItemId(StoreId itemId, Guid mailboxGuid)
		{
			StoreId storeId = this.GetPropertyValue(itemId, null) as StoreId;
			Guid mailboxGuid2 = (Guid)this.GetPropertyValue(mailboxGuid, Guid.Empty);
			MailboxId mailboxId = new MailboxId(mailboxGuid2);
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, mailboxId, null);
			return new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
		}

		private PreviewItemMailbox GetMailbox(Guid mailboxGuid)
		{
			Guid mailboxGuid2 = (Guid)this.GetPropertyValue(mailboxGuid, Guid.Empty);
			ADUser aduser = null;
			if (ADIdentityInformationCache.Singleton.TryGetADUser(mailboxGuid2, base.CallContext.ADRecipientSessionContext, out aduser))
			{
				PreviewItemMailbox previewItemMailbox = new PreviewItemMailbox();
				previewItemMailbox.MailboxId = aduser.LegacyExchangeDN;
				if (!string.IsNullOrEmpty(aduser.PrimarySmtpAddress.ToString()))
				{
					previewItemMailbox.PrimarySmtpAddress = aduser.PrimarySmtpAddress.ToString();
				}
				return previewItemMailbox;
			}
			return null;
		}

		private string GetDateTimeAsString(object propertyValue)
		{
			object propertyValue2 = this.GetPropertyValue(propertyValue, null);
			if (propertyValue2 != null)
			{
				return ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)propertyValue2);
			}
			return null;
		}

		private ImportanceType GetImportance(object propertyValue)
		{
			string text = this.GetPropertyValue(propertyValue, null) as string;
			string a;
			if (!string.IsNullOrEmpty(text) && (a = text.ToLower()) != null)
			{
				if (a == "low")
				{
					return ImportanceType.Low;
				}
				if (a == "high")
				{
					return ImportanceType.High;
				}
			}
			return ImportanceType.Normal;
		}

		private object GetPropertyValue(object propertyValue, object defaultValueIfNull)
		{
			if (propertyValue != null && !(propertyValue is PropertyError))
			{
				return propertyValue;
			}
			return defaultValueIfNull;
		}

		private SearchMailboxesResult CreateSearchMailboxesResult(MailboxQuery[] searchQueries, ResultAggregator resultAggregator, List<FailedSearchMailbox> nonSearchableMailboxes)
		{
			SearchMailboxesResult searchMailboxesResult = new SearchMailboxesResult();
			searchMailboxesResult.SearchQueries = searchQueries;
			searchMailboxesResult.ResultType = this.resultType;
			searchMailboxesResult.Size = 0UL;
			if (resultAggregator == null)
			{
				if (nonSearchableMailboxes != null && nonSearchableMailboxes.Count > 0)
				{
					CallContext.Current.ProtocolLog.AppendGenericInfo("FailedMailboxes", nonSearchableMailboxes.Count);
					searchMailboxesResult.FailedMailboxes = nonSearchableMailboxes.ToArray();
				}
				return searchMailboxesResult;
			}
			if (resultAggregator.KeywordStatistics != null && resultAggregator.KeywordStatistics.Count > 0)
			{
				KeywordStatisticsSearchResult[] array = new KeywordStatisticsSearchResult[resultAggregator.KeywordStatistics.Count];
				int num = 0;
				foreach (KeyValuePair<string, IKeywordHit> keyValuePair in resultAggregator.KeywordStatistics)
				{
					KeywordStatisticsSearchResult keywordStatisticsSearchResult = new KeywordStatisticsSearchResult
					{
						Keyword = keyValuePair.Value.Phrase,
						ItemHits = (int)keyValuePair.Value.Count,
						Size = keyValuePair.Value.Size.ToBytes()
					};
					array[num++] = keywordStatisticsSearchResult;
				}
				searchMailboxesResult.KeywordStats = array;
			}
			searchMailboxesResult.PageItemCount = 0;
			searchMailboxesResult.PageItemSize = 0UL;
			if (resultAggregator.PreviewResult != null && resultAggregator.PreviewResult.ResultRows != null && resultAggregator.PreviewResult.ResultRows.Length > 0)
			{
				searchMailboxesResult.Items = this.ProcessSearchResultRows(resultAggregator.PreviewResult.ResultRows);
				searchMailboxesResult.PageItemCount = searchMailboxesResult.Items.Length;
				foreach (SearchPreviewItem searchPreviewItem in searchMailboxesResult.Items)
				{
					searchMailboxesResult.PageItemSize += ((searchPreviewItem.Size != null) ? searchPreviewItem.Size.Value : 0UL);
				}
			}
			searchMailboxesResult.ItemCount = resultAggregator.TotalResultCount;
			searchMailboxesResult.Size = resultAggregator.TotalResultSize.ToBytes();
			CallContext.Current.ProtocolLog.AppendGenericInfo("ResultCount", searchMailboxesResult.ItemCount);
			CallContext.Current.ProtocolLog.AppendGenericInfo("ResultSize", searchMailboxesResult.Size);
			List<FailedSearchMailbox> list = new List<FailedSearchMailbox>();
			if (nonSearchableMailboxes != null)
			{
				list.AddRange(nonSearchableMailboxes);
			}
			if (resultAggregator.PreviewErrors != null && resultAggregator.PreviewErrors.Count > 0)
			{
				foreach (Pair<MailboxInfo, Exception> pair in resultAggregator.PreviewErrors)
				{
					FailedSearchMailbox item = new FailedSearchMailbox
					{
						Mailbox = pair.First.LegacyExchangeDN,
						IsArchive = (pair.First.Type == MailboxType.Archive),
						ErrorMessage = pair.Second.Message
					};
					list.Add(item);
				}
			}
			CallContext.Current.ProtocolLog.AppendGenericInfo("FailedMailboxes", list.Count);
			if (list.Count > 0)
			{
				searchMailboxesResult.FailedMailboxes = list.ToArray();
			}
			if (this.baseShape != Microsoft.Exchange.Services.Core.Types.PreviewItemBaseShape.Default)
			{
				return searchMailboxesResult;
			}
			if (resultAggregator.MailboxStats != null && resultAggregator.MailboxStats.Count > 0)
			{
				List<MailboxStatisticsItem> list2 = new List<MailboxStatisticsItem>(resultAggregator.MailboxStats.Count);
				foreach (MailboxStatistics mailboxStatistics in resultAggregator.MailboxStats)
				{
					list2.Add(new MailboxStatisticsItem
					{
						MailboxId = mailboxStatistics.MailboxInfo.LegacyExchangeDN,
						DisplayName = mailboxStatistics.MailboxInfo.DisplayName,
						ItemCount = (long)mailboxStatistics.Count,
						Size = mailboxStatistics.Size.ToBytes()
					});
				}
				searchMailboxesResult.MailboxStats = ((list2.Count > 0) ? list2.ToArray() : null);
			}
			if (resultAggregator.RefinersResult != null && resultAggregator.RefinersResult.Count > 0)
			{
				List<SearchRefinerItem> list3 = new List<SearchRefinerItem>(resultAggregator.RefinersResult.Count);
				foreach (KeyValuePair<string, List<IRefinerResult>> keyValuePair2 in resultAggregator.RefinersResult)
				{
					if (keyValuePair2.Value != null && keyValuePair2.Value.Count > 0)
					{
						foreach (IRefinerResult refinerResult in keyValuePair2.Value)
						{
							list3.Add(new SearchRefinerItem
							{
								Name = keyValuePair2.Key,
								Value = refinerResult.Value,
								Count = refinerResult.Count,
								Token = string.Format(":{0}", refinerResult.Value)
							});
						}
					}
				}
				searchMailboxesResult.Refiners = ((list3.Count > 0) ? list3.ToArray() : null);
			}
			return searchMailboxesResult;
		}

		private void SetMailboxQueryFromMailboxDiscoverySearch()
		{
			IDiscoverySearchDataProvider discoverySearchDataProvider = new DiscoverySearchDataProvider(this.recipientSession.SessionSettings.CurrentOrganizationId);
			if (!string.IsNullOrEmpty(this.searchId))
			{
				MailboxDiscoverySearch mailboxDiscoverySearch = discoverySearchDataProvider.Find<MailboxDiscoverySearch>(this.searchId);
				if (mailboxDiscoverySearch != null)
				{
					MailboxSearchLocation searchScope = MailboxSearchLocation.All;
					if (!string.IsNullOrEmpty(mailboxDiscoverySearch.Language))
					{
						this.language = mailboxDiscoverySearch.Language;
					}
					IEnumerable<MailboxSearchScope> source2;
					if (string.IsNullOrEmpty(this.mailboxId))
					{
						if (mailboxDiscoverySearch.Sources.Count == 0)
						{
							List<FailedSearchMailbox> list;
							List<SearchableMailbox> searchableMailboxes = MailboxSearchHelper.GetSearchableMailboxes(mailboxDiscoverySearch, true, this.recipientSession, this.runspaceConfig, out list);
							source2 = from mailbox in searchableMailboxes
							select new MailboxSearchScope
							{
								Mailbox = mailbox.ReferenceId,
								SearchScope = searchScope
							};
						}
						else
						{
							source2 = from source in mailboxDiscoverySearch.Sources
							select new MailboxSearchScope
							{
								Mailbox = source,
								SearchScope = searchScope
							};
						}
					}
					else
					{
						source2 = new MailboxSearchScope[]
						{
							new MailboxSearchScope
							{
								Mailbox = this.mailboxId,
								SearchScope = searchScope
							}
						};
					}
					this.mailboxQueries = new MailboxQuery[1];
					this.mailboxQueries[0] = new MailboxQuery();
					this.mailboxQueries[0].MailboxSearchScopes = source2.ToArray<MailboxSearchScope>();
					this.mailboxQueries[0].Query = mailboxDiscoverySearch.CalculatedQuery;
					this.performDeduplication = true;
				}
			}
			if (this.mailboxQueries == null)
			{
				throw new ServiceArgumentException((CoreResources.IDs)2179607746U);
			}
		}

		private const int DefaultPageSize = 100;

		private readonly DisposeTracker disposeTracker;

		private readonly PropertyDefinition[] compactPreviewDataProperties = new PropertyDefinition[]
		{
			StoreObjectSchema.ParentItemId,
			ItemSchema.Id,
			ItemSchema.Size
		};

		private readonly PropertyDefinition[] previewDataProperties = new PropertyDefinition[]
		{
			StoreObjectSchema.ParentItemId,
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			ItemSchema.HasAttachment,
			ItemSchema.Size,
			ItemSchema.BodyTag,
			ItemSchema.InternetMessageId,
			ItemSchema.Subject,
			MessageItemSchema.IsRead,
			ItemSchema.SentTime,
			ItemSchema.ReceivedTime,
			MessageItemSchema.SenderDisplayName,
			MessageItemSchema.SenderSmtpAddress,
			ItemSchema.Importance,
			ItemSchema.Categories,
			ItemSchema.DisplayCc,
			ItemSchema.DisplayBcc,
			ItemSchema.DisplayTo,
			StoreObjectSchema.CreationTime
		};

		private bool disposed;

		private string searchId;

		private string mailboxId;

		private MailboxQuery[] mailboxQueries;

		private MailboxQuery currentMailboxQuery;

		private SearchResultType resultType;

		private SortResults sortBy;

		private string language;

		private bool performDeduplication;

		private int pageSize;

		private string pageItemReference;

		private PageDirection pageDirection;

		private int stepCount;

		private ExchangeRunspaceConfiguration runspaceConfig;

		private IRecipientSession recipientSession;

		private Dictionary<string, ADRawEntry> dictADRawEntries;

		private List<PropertyDefinition> itemProperties;

		private Microsoft.Exchange.Services.Core.Types.PreviewItemBaseShape baseShape;

		private ExtendedPropertyUri[] additionalExtendedProperties;

		private Dictionary<PropertyDefinition, ExtendedPropertyUri> xsoEwsPropertiesMap;

		private readonly Guid requestId;

		private readonly bool isFlighted;

		private readonly ISearchPolicy policy;

		private SearchMailboxesResponse response;
	}
}
