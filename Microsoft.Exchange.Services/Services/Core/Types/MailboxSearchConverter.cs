using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class MailboxSearchConverter
	{
		public static CommonAccessToken GetCommonAccessToken(CallContext context)
		{
			CommonAccessToken commonAccessToken = context.HttpContext.Items["Item-CommonAccessToken"] as CommonAccessToken;
			if (commonAccessToken == null && context.HttpContext.User != null)
			{
				OAuthIdentity oauthIdentity = context.HttpContext.User.Identity as OAuthIdentity;
				if (oauthIdentity != null)
				{
					int targetServerVersion = (ServerVersion.InstalledVersion != null) ? ServerVersion.InstalledVersion.ToInt() : OAuthTokenAccessor.MinVersion;
					commonAccessToken = oauthIdentity.ToCommonAccessToken(targetServerVersion);
				}
				else
				{
					LiveIDIdentity liveIDIdentity = context.HttpContext.User.Identity as LiveIDIdentity;
					if (liveIDIdentity != null)
					{
						LiveIdFbaTokenAccessor liveIdFbaTokenAccessor = LiveIdFbaTokenAccessor.Create(liveIDIdentity);
						return liveIdFbaTokenAccessor.GetToken();
					}
				}
			}
			return commonAccessToken;
		}

		public static CallerInfo GetCallerInfo(CallContext context, OrganizationId organizationId)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetCallerInfo OrgId:", organizationId);
			Guid queryCorrelationId = MailboxSearchHelper.GetQueryCorrelationId();
			return new CallerInfo(MailboxSearchHelper.IsOpenAsAdmin(context), MailboxSearchConverter.GetCommonAccessToken(context), context.EffectiveCaller.ClientSecurityContext, context.EffectiveCaller.PrimarySmtpAddress, organizationId, context.UserAgent, queryCorrelationId, MailboxSearchHelper.GetUserRolesFromAuthZClientInfo(context.EffectiveCaller), MailboxSearchHelper.GetApplicationRolesFromAuthZClientInfo(context.EffectiveCaller));
		}

		public static SearchMailboxesInputs GetSearchInputs(ISearchPolicy policy, SearchMailboxesRequest request)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchInputs Request:", request);
			SearchMailboxesInputs searchMailboxesInputs = new SearchMailboxesInputs();
			SortBy sortBy = MailboxSearchConverter.GetSortBy(request.SortBy);
			List<ExtendedPropertyInfo> additionalProperties = null;
			if (request.PreviewItemResponseShape != null && request.PreviewItemResponseShape.AdditionalProperties != null)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, new object[]
				{
					"MailboxSearchConverter.GetSearchInputs Load AdditionalProperites Shape:",
					request.PreviewItemResponseShape.BaseShape,
					"AdditionalProperties:",
					request.PreviewItemResponseShape.AdditionalProperties.Length
				});
				additionalProperties = MailboxSearchConverter.GetExtendedPropertyInfo(request.PreviewItemResponseShape.AdditionalProperties).ToList<ExtendedPropertyInfo>();
			}
			ReferenceItem referenceItem = null;
			if (!string.IsNullOrEmpty(request.PageItemReference))
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchInputs Load PageItemReference:", request.PageItemReference);
				referenceItem = ReferenceItem.Parse(sortBy, request.PageItemReference);
			}
			if (request.PreviewItemResponseShape == null)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchInputs No Shape, Defaulting");
				request.PreviewItemResponseShape = new PreviewItemResponseShape
				{
					BaseShape = PreviewItemBaseShape.Default
				};
			}
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchInputs Load PagingInfo");
			PagingInfo pagingInfo = new PagingInfo((request.PreviewItemResponseShape.BaseShape == PreviewItemBaseShape.Compact) ? MailboxSearchConverter.CompactPreviewDataProperties : MailboxSearchConverter.PreviewDataProperties, sortBy, (request.PageSize == 0) ? policy.ExecutionSettings.DiscoveryDefaultPageSize : request.PageSize, (request.PageDirection == SearchPageDirectionType.Next) ? PageDirection.Next : PageDirection.Previous, referenceItem, MailboxSearchHelper.GetTimeZone(), request.Deduplication, (request.PreviewItemResponseShape.BaseShape == PreviewItemBaseShape.Compact) ? PreviewItemBaseShape.Compact : PreviewItemBaseShape.Default, additionalProperties)
			{
				OriginalSortByReference = ((request.SortBy != null && request.SortBy.SortByProperty != null) ? request.SortBy.SortByProperty.ToString() : null)
			};
			searchMailboxesInputs.PagingInfo = pagingInfo;
			searchMailboxesInputs.RequestId = policy.CallerInfo.QueryCorrelationId;
			searchMailboxesInputs.CallerInfo = policy.CallerInfo;
			searchMailboxesInputs.Language = request.Language;
			searchMailboxesInputs.SearchType = ((request.ResultType == SearchResultType.PreviewOnly) ? SearchType.Preview : SearchType.Statistics);
			Recorder.Trace(1L, TraceType.InfoTrace, new object[]
			{
				"MailboxSearchConverter.GetSearchInputs Parameters Set Language:",
				request.Language,
				"SearchType:",
				searchMailboxesInputs.SearchType,
				"CorrelationId:",
				searchMailboxesInputs.RequestId
			});
			if (request.SearchQueries == null || request.SearchQueries.Length == 0)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchInputs No SearchQueries, Looking for SearchId");
				if (!string.IsNullOrEmpty(request.SearchId))
				{
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchInputs SearchId Found", request.SearchId);
					searchMailboxesInputs.SearchConfigurationId = request.SearchId;
					if (!string.IsNullOrEmpty(request.MailboxId))
					{
						Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchInputs MailboxId Found", request.MailboxId);
						searchMailboxesInputs.Sources = new List<SearchSource>
						{
							new SearchSource
							{
								SourceType = SourceType.AutoDetect,
								ReferenceId = request.MailboxId
							}
						};
					}
				}
			}
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchInputs Completed");
			return searchMailboxesInputs;
		}

		private static void MoveItemsWithBadSortValuesMidPage(SearchMailboxesRequest request, List<SearchPreviewItem> items)
		{
			string dateTimeAsString = MailboxSearchConverter.GetDateTimeAsString(ExDateTime.MinValue);
			if (MailboxSearchConverter.GetSortBy(request.SortBy).ColumnDefinition == ItemSchema.ReceivedTime && items[items.Count - 1].ReceivedTime.Equals(dateTimeAsString) && !items[0].ReceivedTime.Equals(dateTimeAsString))
			{
				int num = items.Count - 1;
				while (num >= 0 && items[num].ReceivedTime.Equals(dateTimeAsString))
				{
					num--;
				}
				SearchPreviewItem value = items[num];
				items[num] = items[items.Count - 1];
				items[items.Count - 1] = value;
			}
		}

		public static SearchMailboxesResponse GetSearchResponse(SearchMailboxesRequest request, List<ServiceResult<SearchMailboxesResults>> responses, SearchMailboxesInputs parameters)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, new object[]
			{
				"MailboxSearchConverter.GetSearchResponse Response:",
				responses,
				"Request:",
				request,
				"Parameters:",
				parameters
			});
			ISearchResult searchResult = new ResultAggregator();
			List<FailedSearchMailbox> list = new List<FailedSearchMailbox>();
			List<SearchSource> list2 = new List<SearchSource>();
			SearchMailboxesResult searchMailboxesResult = new SearchMailboxesResult();
			SearchMailboxesResponse searchMailboxesResponse = new SearchMailboxesResponse();
			ServiceResult<SearchMailboxesResult> serviceResult = new ServiceResult<SearchMailboxesResult>(searchMailboxesResult);
			searchMailboxesResult.SearchQueries = request.SearchQueries;
			searchMailboxesResult.ResultType = request.ResultType;
			searchMailboxesResult.Size = 0UL;
			searchMailboxesResult.ItemCount = 0UL;
			if (searchMailboxesResult.SearchQueries != null && searchMailboxesResult.SearchQueries.Length == 1 && parameters.SearchType == SearchType.ExpandSources)
			{
				searchMailboxesResult.SearchQueries[0].Query = parameters.SearchQuery;
			}
			bool flag = false;
			ServiceError serviceError = null;
			int num = 0;
			foreach (ServiceResult<SearchMailboxesResults> serviceResult2 in responses)
			{
				if (serviceResult2.Code != ServiceResultCode.Success || serviceResult2.Value == null)
				{
					Recorder.Trace(1L, TraceType.ErrorTrace, "MailboxSearchConverter.GetSearchResponse ErrorResponse:", serviceResult2.Error);
					serviceError = serviceResult2.Error;
					if (serviceError != null && num < request.SearchQueries.Length)
					{
						MailboxQuery mailboxQuery = request.SearchQueries[num];
						foreach (MailboxSearchScope mailboxSearchScope in mailboxQuery.MailboxSearchScopes)
						{
							list.Add(new FailedSearchMailbox
							{
								Mailbox = mailboxSearchScope.Mailbox,
								ErrorMessage = serviceError.MessageText
							});
						}
					}
				}
				else
				{
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchResponse SuccessfulResponse:", serviceResult2);
					searchResult.MergeSearchResult(serviceResult2.Value.SearchResult);
					list.AddRange(MailboxSearchConverter.GetFailures(serviceResult2.Value.Failures));
					list2.AddRange(serviceResult2.Value.Sources);
					flag = true;
				}
				num++;
			}
			if (flag)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchResponse Successful Responses Found");
				serviceResult.Code = ServiceResultCode.Success;
				searchMailboxesResult.ItemCount = searchResult.TotalResultCount;
				searchMailboxesResult.Size = searchResult.TotalResultSize.ToBytes();
				list.AddRange(MailboxSearchConverter.GetFailedMailboxes(searchResult.PreviewErrors));
				Recorder.Trace(1L, TraceType.InfoTrace, new object[]
				{
					"MailboxSearchConverter.GetSearchResponse Successful Responses Loaded Items:",
					searchMailboxesResult.ItemCount,
					"Size:",
					searchMailboxesResult.Size
				});
				KeywordStatisticsSearchResult[] array = MailboxSearchConverter.GetKeywordStatistics(searchResult.KeywordStatistics).ToArray<KeywordStatisticsSearchResult>();
				if (array.Length > 0)
				{
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchResponse Keyword Statistics Found Count:", array.Length);
					searchMailboxesResult.KeywordStats = array;
				}
				searchMailboxesResult.PageItemCount = 0;
				searchMailboxesResult.PageItemSize = 0UL;
				if (searchResult.PreviewResult != null && searchResult.PreviewResult.ResultRows != null && searchResult.PreviewResult.ResultRows.Length > 0)
				{
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchResponse Preview Results Found Count:", searchResult.PreviewResult.ResultRows.Length);
					List<SearchPreviewItem> list3 = new List<SearchPreviewItem>();
					foreach (PreviewItem item in searchResult.PreviewResult.ResultRows)
					{
						SearchPreviewItem previewItem = MailboxSearchConverter.GetPreviewItem(item, parameters.PagingInfo);
						searchMailboxesResult.PageItemCount++;
						searchMailboxesResult.PageItemSize += ((previewItem.Size != null) ? previewItem.Size.Value : 0UL);
						list3.Add(previewItem);
					}
					MailboxSearchConverter.MoveItemsWithBadSortValuesMidPage(request, list3);
					searchMailboxesResult.Items = list3.ToArray();
				}
				if (request.PreviewItemResponseShape.BaseShape == PreviewItemBaseShape.Default)
				{
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchResponse Default Shape");
					SearchRefinerItem[] array2 = MailboxSearchConverter.GetRefiners(searchResult.RefinersResult).ToArray<SearchRefinerItem>();
					if (array2.Length > 0)
					{
						Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchResponse Refiners Loaded, Count:", array2.Length);
						searchMailboxesResult.Refiners = array2;
					}
					MailboxStatisticsItem[] array3 = MailboxSearchConverter.GetMailboxStatistics(searchResult.MailboxStats).ToArray<MailboxStatisticsItem>();
					if (array3.Length > 0)
					{
						Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchResponse Statistics Loaded, Count:", array3.Length);
						searchMailboxesResult.MailboxStats = array3;
					}
				}
			}
			else
			{
				if (serviceError == null)
				{
					throw MailboxSearchConverter.GetException(new SearchException(KnownError.ErrorNoMailboxSpecifiedForSearchOperation));
				}
				Recorder.Trace(1L, TraceType.ErrorTrace, "MailboxSearchConverter.GetSearchResponse All Responses Failed, Error:", serviceError);
				serviceResult.Code = ServiceResultCode.Error;
				serviceResult.Error = serviceError;
			}
			if (list.Count > 0)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchResponse Failed Mailboxes Loaded, Count:", list.Count);
				searchMailboxesResult.FailedMailboxes = list.ToArray();
			}
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchResponse Completed");
			searchMailboxesResponse.AddResponses(new ServiceResult<SearchMailboxesResult>[]
			{
				serviceResult
			});
			return searchMailboxesResponse;
		}

		public static IEnumerable<SearchSource> GetSources(ISearchPolicy policy, IList<MailboxSearchScope> scopes)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSources Scopes:", scopes);
			foreach (MailboxSearchScope scope in scopes)
			{
				SearchSource source = new SearchSource
				{
					ReferenceId = scope.Mailbox,
					SourceLocation = (SourceLocation)scope.SearchScope,
					CanBeCrossPremise = (policy.CallerInfo != null && policy.CallerInfo.CommonAccessToken != null)
				};
				ExtendedAttribute[] extendedAttributes = scope.ExtendedAttributes;
				if (extendedAttributes != null && extendedAttributes.Length > 0)
				{
					foreach (ExtendedAttribute extendedAttribute in extendedAttributes)
					{
						if ("SearchScopeType".Equals(extendedAttribute.Name))
						{
							SourceType sourceType;
							if (Enum.TryParse<SourceType>(extendedAttribute.Value, out sourceType))
							{
								source.SourceType = sourceType;
							}
						}
						else if (!string.IsNullOrEmpty(extendedAttribute.Name))
						{
							source.ExtendedAttributes[extendedAttribute.Name] = extendedAttribute.Value;
						}
					}
				}
				source.TryLoadMailboxInfo();
				Recorder.Trace(1L, TraceType.InfoTrace, new object[]
				{
					"MailboxSearchConverter.GetSources Source Loaded, Reference:",
					source.ReferenceId,
					"Type:",
					source.SourceType,
					"Location:",
					source.SourceLocation,
					"MailboxInfo:",
					source.MailboxInfo
				});
				yield return source;
			}
			yield break;
		}

		public static SortBy GetSortBy(SortResults sortBy)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSortBy SortBy:", sortBy);
			PropertyDefinition propertyDefinition = ItemSchema.ReceivedTime;
			SortOrder sortOrder = SortOrder.Descending;
			if (sortBy != null && sortBy.SortByProperty != null)
			{
				PropertyUri propertyUri = sortBy.SortByProperty as PropertyUri;
				if (propertyUri == null)
				{
					Recorder.Trace(1L, TraceType.ErrorTrace, "MailboxSearchConverter.GetSortBy SortBy PropUri Invalid");
					throw new SearchException(KnownError.ErrorInvalidPropertyForSortBy);
				}
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSortBy PropUri:", propertyUri.UriString);
				propertyDefinition = MailboxSearchConverter.GetPropertyDefinition(propertyUri);
				if (propertyDefinition == null)
				{
					Recorder.Trace(1L, TraceType.ErrorTrace, "MailboxSearchConverter.GetSortBy SortBy Definition Invalid");
					throw new SearchException(KnownError.ErrorSortByPropertyIsNotFoundOrNotSupported);
				}
				sortOrder = ((sortBy.Order == SortDirection.Descending) ? SortOrder.Descending : SortOrder.Ascending);
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSortBy SortOrder:", sortOrder);
			}
			SortBy sortBy2 = new SortBy(propertyDefinition, sortOrder);
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSortBy SortedBy:", sortBy2);
			return sortBy2;
		}

		public static IEnumerable<ExtendedPropertyInfo> GetExtendedPropertyInfo(IList<ExtendedPropertyUri> propertyUriList)
		{
			Recorder.Trace(1L, TraceType.ErrorTrace, "MailboxSearchConverter.GetExtendedPropertyInfo PropertyList:", propertyUriList);
			foreach (ExtendedPropertyUri extendedPropertyUri in propertyUriList)
			{
				PropertyDefinition propertyDefinition = extendedPropertyUri.ToPropertyDefinition();
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
				yield return extendedPropertyInfo;
			}
			yield break;
		}

		public static PropertyDefinition GetPropertyDefinition(PropertyUri propUri)
		{
			Recorder.Trace(1L, TraceType.ErrorTrace, "MailboxSearchConverter.GetPropertyDefinition PropertyUri:", propUri);
			PropertyDefinition propertyDefinition = null;
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
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetPropertyDefinition Schema:", schema);
				PropertyInformation propertyInformation = null;
				if (schema.TryGetPropertyInformationByPath(propUri, out propertyInformation))
				{
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetPropertyDefinition PropInfo:", propertyInformation);
					PropertyDefinition[] propertyDefinitions = propertyInformation.GetPropertyDefinitions(null);
					if (propertyDefinitions != null && propertyDefinitions.Length > 0)
					{
						Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetPropertyDefinition PropDefinition:", propertyDefinition);
						propertyDefinition = propertyDefinitions[0];
					}
				}
			}
			return propertyDefinition;
		}

		public static IEnumerable<KeywordStatisticsSearchResult> GetKeywordStatistics(IDictionary<string, IKeywordHit> statistics)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetKeywordStatistics Statistics:", statistics);
			if (statistics != null && statistics.Count > 0)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetKeywordStatistics Count:", statistics.Count);
				foreach (KeyValuePair<string, IKeywordHit> kvp in statistics)
				{
					KeywordStatisticsSearchResult keywordStatisticsSearchResult = new KeywordStatisticsSearchResult();
					KeywordStatisticsSearchResult keywordStatisticsSearchResult2 = keywordStatisticsSearchResult;
					KeyValuePair<string, IKeywordHit> keyValuePair = kvp;
					keywordStatisticsSearchResult2.Keyword = keyValuePair.Value.Phrase;
					KeywordStatisticsSearchResult keywordStatisticsSearchResult3 = keywordStatisticsSearchResult;
					KeyValuePair<string, IKeywordHit> keyValuePair2 = kvp;
					keywordStatisticsSearchResult3.ItemHits = (int)keyValuePair2.Value.Count;
					KeywordStatisticsSearchResult keywordStatisticsSearchResult4 = keywordStatisticsSearchResult;
					KeyValuePair<string, IKeywordHit> keyValuePair3 = kvp;
					keywordStatisticsSearchResult4.Size = keyValuePair3.Value.Size.ToBytes();
					KeywordStatisticsSearchResult sr = keywordStatisticsSearchResult;
					yield return sr;
				}
			}
			yield break;
		}

		public static IEnumerable<MailboxStatisticsItem> GetMailboxStatistics(IList<MailboxStatistics> statistics)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetMailboxStatistics Statistics:", statistics);
			if (statistics != null && statistics.Count > 0)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetMailboxStatistics Count:", statistics.Count);
				foreach (MailboxStatistics resultMailboxStat in statistics)
				{
					SearchSource source = resultMailboxStat.MailboxInfo.SourceMailbox as SearchSource;
					yield return new MailboxStatisticsItem
					{
						MailboxId = ((source != null) ? (source.OriginalReferenceId ?? source.ReferenceId) : resultMailboxStat.MailboxInfo.LegacyExchangeDN),
						DisplayName = ((source != null && source.OriginalReferenceId != null) ? source.OriginalReferenceId : resultMailboxStat.MailboxInfo.DisplayName),
						ItemCount = (long)resultMailboxStat.Count,
						Size = resultMailboxStat.Size.ToBytes()
					};
				}
			}
			yield break;
		}

		public static IEnumerable<SearchRefinerItem> GetRefiners(IDictionary<string, List<IRefinerResult>> refiners)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetRefiners Refiners:", refiners);
			if (refiners != null && refiners.Count > 0)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetRefiners Count:", refiners.Count);
				foreach (KeyValuePair<string, List<IRefinerResult>> kvpRefiner in refiners)
				{
					KeyValuePair<string, List<IRefinerResult>> keyValuePair = kvpRefiner;
					if (keyValuePair.Value != null)
					{
						KeyValuePair<string, List<IRefinerResult>> keyValuePair2 = kvpRefiner;
						if (keyValuePair2.Value.Count > 0)
						{
							KeyValuePair<string, List<IRefinerResult>> keyValuePair3 = kvpRefiner;
							foreach (IRefinerResult refiner in keyValuePair3.Value)
							{
								SearchRefinerItem searchRefinerItem = new SearchRefinerItem();
								SearchRefinerItem searchRefinerItem2 = searchRefinerItem;
								KeyValuePair<string, List<IRefinerResult>> keyValuePair4 = kvpRefiner;
								searchRefinerItem2.Name = keyValuePair4.Key;
								searchRefinerItem.Value = refiner.Value;
								searchRefinerItem.Count = refiner.Count;
								searchRefinerItem.Token = string.Format(":{0}", refiner.Value);
								yield return searchRefinerItem;
							}
						}
					}
				}
			}
			yield break;
		}

		public static IEnumerable<FailedSearchMailbox> GetFailedMailboxes(IList<Pair<MailboxInfo, Exception>> errors)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetFailedMailboxes Errors:", errors);
			if (errors != null && errors.Count > 0)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetFailedMailboxes Count:", errors.Count);
				foreach (Pair<MailboxInfo, Exception> error in errors)
				{
					SearchSource source = error.First.SourceMailbox as SearchSource;
					yield return new FailedSearchMailbox
					{
						Mailbox = ((source != null) ? (source.OriginalReferenceId ?? source.ReferenceId) : error.First.LegacyExchangeDN),
						IsArchive = (error.First.Type == MailboxType.Archive),
						ErrorMessage = error.Second.Message
					};
				}
			}
			yield break;
		}

		public static SearchPreviewItem GetPreviewItem(PreviewItem item, PagingInfo pagingInfo)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, new object[]
			{
				"MailboxSearchConverter.GetPreviewItem Item:",
				item,
				"PagingInfo:",
				pagingInfo
			});
			if (item != null)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetPreviewItem Item:", item);
				SearchPreviewItem searchPreviewItem = new SearchPreviewItem();
				SearchSource searchSource = (item.MailboxInfo != null) ? (item.MailboxInfo.SourceMailbox as SearchSource) : null;
				if (item.Id != null)
				{
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetPreviewItem ItemId:", item.Id);
					searchPreviewItem.Id = MailboxSearchConverter.GetItemId(item.Id, item.MailboxGuid, searchSource, item.ParentItemId);
					string owaLink = string.Empty;
					if (item.OwaLink != null && !string.IsNullOrEmpty(item.OwaLink.AbsoluteUri))
					{
						Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetPreviewItem OWA:", item.OwaLink);
						owaLink = LinkUtils.UpdateOwaLinkToItem(item.OwaLink, searchPreviewItem.Id.Id).AbsoluteUri;
					}
					searchPreviewItem.OwaLink = owaLink;
				}
				if (item.ParentItemId != null)
				{
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetPreviewItem ParentId:", item.ParentItemId);
					searchPreviewItem.ParentId = MailboxSearchConverter.GetItemId(item.ParentItemId, item.MailboxGuid, searchSource, null);
				}
				searchPreviewItem.UniqueHash = ((item.ItemHash != null) ? item.ItemHash.ToString() : null);
				searchPreviewItem.SortValue = ((item.SortValue != null) ? item.SortValue.ToString() : null);
				searchPreviewItem.Size = new ulong?((ulong)((long)item.Size));
				if (pagingInfo.BaseShape == PreviewItemBaseShape.Default)
				{
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetPreviewItem Default Shape");
					searchPreviewItem.ItemClass = item.ItemClass;
					searchPreviewItem.Sender = item.Sender;
					searchPreviewItem.Subject = item.Subject;
					searchPreviewItem.ToRecipients = item.ToRecipients;
					searchPreviewItem.CcRecipients = item.CcRecipients;
					searchPreviewItem.BccRecipients = item.BccRecipients;
					searchPreviewItem.CreatedTime = MailboxSearchConverter.GetDateTimeAsString(item.CreationTime);
					searchPreviewItem.ReceivedTime = MailboxSearchConverter.GetDateTimeAsString(item.ReceivedTime);
					searchPreviewItem.SentTime = MailboxSearchConverter.GetDateTimeAsString(item.SentTime);
					searchPreviewItem.Preview = item.Preview;
					string primarySmtpAddress = (searchSource != null) ? searchSource.GetPrimarySmtpAddress() : null;
					searchPreviewItem.Mailbox = new PreviewItemMailbox
					{
						MailboxId = ((searchSource != null) ? (searchSource.OriginalReferenceId ?? searchSource.ReferenceId) : item.MailboxGuid.ToString()),
						PrimarySmtpAddress = primarySmtpAddress
					};
					string text = MailboxSearchConverter.GetPropertyValue(item.Importance, null) as string;
					string a;
					if (!string.IsNullOrEmpty(text) && (a = text.ToLower()) != null)
					{
						if (!(a == "low"))
						{
							if (a == "high")
							{
								searchPreviewItem.Importance = ImportanceType.High.ToString();
							}
						}
						else
						{
							searchPreviewItem.Importance = ImportanceType.Low.ToString();
						}
					}
					searchPreviewItem.Read = new bool?(item.Read);
					searchPreviewItem.HasAttachment = new bool?(item.HasAttachment);
				}
				if (pagingInfo.AdditionalProperties != null && pagingInfo.AdditionalProperties.Count > 0)
				{
					Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetPreviewItem AdditionalProperties:", pagingInfo.AdditionalProperties.Count);
					searchPreviewItem.ExtendedProperties = MailboxSearchConverter.GetExtendedProperties(pagingInfo, item.AdditionalPropertyValues).ToArray<ExtendedPropertyType>();
					if (searchPreviewItem.ExtendedProperties != null && searchPreviewItem.ExtendedProperties.Length == 0)
					{
						searchPreviewItem.ExtendedProperties = null;
					}
				}
				return searchPreviewItem;
			}
			return null;
		}

		public static IEnumerable<FailedSearchMailbox> GetFailures(IEnumerable<Exception> exceptions)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetFailures Exceptions:", exceptions);
			if (exceptions != null)
			{
				foreach (Exception exception in exceptions)
				{
					string mailbox = string.Empty;
					string errorMessage = string.Empty;
					int errorCode = 0;
					bool isArchve = false;
					SearchException searchException = exception as SearchException;
					if (searchException != null)
					{
						Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetFailures SearchException:", searchException.Error);
						if (searchException.ErrorSource != null)
						{
							mailbox = searchException.ErrorSource.ReferenceId;
							isArchve = (searchException.ErrorSource.MailboxInfo != null && searchException.ErrorSource.MailboxInfo.IsArchive);
						}
						switch (searchException.Error)
						{
						case KnownError.ErrorRecipientTypeNotSupported:
							errorMessage = CoreResources.GetLocalizedString((CoreResources.IDs)3611326890U);
							break;
						case KnownError.ErrorMailboxVersionNotSupported:
							errorMessage = CoreResources.GetLocalizedString(CoreResources.IDs.ErrorMailboxVersionNotSupported);
							break;
						case KnownError.ErrorNoPermissionToSearchOrHoldMailbox:
							errorMessage = CoreResources.GetLocalizedString((CoreResources.IDs)2354781453U);
							break;
						case KnownError.ErrorSearchableObjectNotFound:
							errorMessage = CoreResources.ErrorSearchableObjectNotFound;
							break;
						default:
							yield break;
						}
					}
					yield return new FailedSearchMailbox
					{
						Mailbox = mailbox,
						IsArchive = isArchve,
						ErrorCode = errorCode,
						ErrorMessage = errorMessage
					};
				}
			}
			yield break;
		}

		public static Exception GetException(Exception exception)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetException Exception:", exception);
			SearchException ex = exception as SearchException;
			if (ex != null)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetException SearchException:", ex.Error);
				switch (ex.Error)
				{
				case KnownError.ErrorDiscoverySearchesDisabledException:
					return new DiscoverySearchesDisabledException();
				case KnownError.ErrorSearchQueryCannotBeEmpty:
					return new ServiceArgumentException((CoreResources.IDs)2226875331U);
				case KnownError.ErrorNoMailboxSpecifiedForSearchOperation:
					return new ServiceInvalidOperationException(CoreResources.IDs.ErrorNoMailboxSpecifiedForSearchOperation);
				case KnownError.ErrorInvalidSearchQuerySyntax:
					return new ServiceArgumentException((CoreResources.IDs)3021008902U);
				case KnownError.ErrorQueryLanguageNotValid:
					return new ServiceArgumentException(CoreResources.IDs.ErrorQueryLanguageNotValid);
				case KnownError.ErrorSortByPropertyIsNotFoundOrNotSupported:
					return new ServiceArgumentException((CoreResources.IDs)2841035169U);
				case KnownError.ErrorInvalidPropertyForSortBy:
					return new ServiceArgumentException((CoreResources.IDs)2566235088U);
				case KnownError.ErrorInvalidSearchId:
					return new ServiceArgumentException((CoreResources.IDs)2179607746U);
				case KnownError.ErrorSearchTimedOut:
					return new ServiceInvalidOperationException((CoreResources.IDs)3285224352U, exception);
				case KnownError.TooManyMailboxQueryObjects:
					return new ServiceInvalidOperationException((CoreResources.IDs)3784063568U, new InvalidOperationException(CoreResources.TooManyMailboxQueryObjects((int)ex.Parameters[0], (int)ex.Parameters[1])));
				case KnownError.TooManyMailboxesException:
					return new TooManyMailboxesException((int)ex.Parameters[0], (int)ex.Parameters[1]);
				case KnownError.TooManyKeywordsException:
					return new TooManyKeywordsException(exception);
				case KnownError.ErrorWildcardAndGroupExpansionNotAllowed:
					return new ServiceArgumentException((CoreResources.IDs)4083587704U);
				case KnownError.ErrorSuffixSearchNotAllowed:
					return new ServiceArgumentException(CoreResources.IDs.ErrorSuffixSearchNotAllowed);
				}
				return new InternalServerErrorException(exception);
			}
			return exception;
		}

		public static IEnumerable<SearchableMailbox> GetSearchableMailboxes(GetSearchableMailboxesResults results)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchableMailboxes Results:", results);
			if (results != null && results.Sources != null)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetSearchableMailboxes Count:", results.Sources.Count);
				foreach (SearchSource source in results.Sources)
				{
					if (source != null)
					{
						string externalEmailAddress = (source.GetProperty(ADRecipientSchema.ExternalEmailAddress) == null) ? string.Empty : ((ProxyAddress)source.GetProperty(ADRecipientSchema.ExternalEmailAddress)).AddressString;
						SearchableMailbox searchableMailbox = new SearchableMailbox(((ADObjectId)source.GetProperty(ADObjectSchema.Id)).ObjectGuid, ((SmtpAddress)source.GetProperty(ADRecipientSchema.PrimarySmtpAddress)).ToString(), (RecipientType)source.GetProperty(ADRecipientSchema.RecipientType) == RecipientType.MailUser, externalEmailAddress, (string)source.GetProperty(ADRecipientSchema.DisplayName), SearchRecipient.IsMembershipGroupTypeDetail((RecipientTypeDetails)source.GetProperty(ADRecipientSchema.RecipientTypeDetails)), (string)source.GetProperty(ADRecipientSchema.LegacyExchangeDN));
						yield return searchableMailbox;
					}
				}
			}
			yield break;
		}

		private static IEnumerable<ExtendedPropertyType> GetExtendedProperties(PagingInfo pagingInfo, IDictionary<PropertyDefinition, object> propertyValues)
		{
			Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetExtendedProperties PropertyValues:", propertyValues);
			if (propertyValues != null && propertyValues.Count != 0)
			{
				Recorder.Trace(1L, TraceType.InfoTrace, "MailboxSearchConverter.GetExtendedProperties Count:", propertyValues.Count);
				foreach (KeyValuePair<PropertyDefinition, object> kvp in propertyValues)
				{
					KeyValuePair<PropertyDefinition, object> keyValuePair = kvp;
					if (keyValuePair.Value != null)
					{
						KeyValuePair<PropertyDefinition, object> keyValuePair2 = kvp;
						if (!(keyValuePair2.Value is PropertyError))
						{
							KeyValuePair<PropertyDefinition, object> keyValuePair3 = kvp;
							ExtendedPropertyUri extendedPropertyUri = new ExtendedPropertyUri(keyValuePair3.Key as NativeStorePropertyDefinition);
							KeyValuePair<PropertyDefinition, object> keyValuePair4 = kvp;
							ExtendedPropertyType extendedProperty;
							if (keyValuePair4.Value is Array)
							{
								ExtendedPropertyUri propertyUri = extendedPropertyUri;
								KeyValuePair<PropertyDefinition, object> keyValuePair5 = kvp;
								extendedProperty = new ExtendedPropertyType(propertyUri, (from t in (object[])keyValuePair5.Value
								select t as string).ToArray<string>());
							}
							else
							{
								ExtendedPropertyUri propertyUri2 = extendedPropertyUri;
								KeyValuePair<PropertyDefinition, object> keyValuePair6 = kvp;
								extendedProperty = new ExtendedPropertyType(propertyUri2, keyValuePair6.Value.ToString());
							}
							yield return extendedProperty;
						}
					}
				}
			}
			yield break;
		}

		private static ItemId GetItemId(StoreId itemId, Guid mailboxGuid, SearchSource source, StoreId parentId)
		{
			ConcatenatedIdAndChangeKey concatenatedIdAndChangeKey = default(ConcatenatedIdAndChangeKey);
			StoreId storeId = MailboxSearchConverter.GetPropertyValue(itemId, null) as StoreId;
			if (source != null && source.SourceType == SourceType.MailboxGuid && parentId != null && !string.IsNullOrEmpty(source.FolderSpec))
			{
				StoreObjectId parentFolderId = MailboxSearchConverter.GetPropertyValue(parentId, null) as StoreObjectId;
				concatenatedIdAndChangeKey = IdConverter.GetConcatenatedIdForPublicFolderItem(storeId, parentFolderId, null);
			}
			else
			{
				Guid mailboxGuid2 = (Guid)MailboxSearchConverter.GetPropertyValue(mailboxGuid, Guid.Empty);
				MailboxId mailboxId = new MailboxId(mailboxGuid2);
				concatenatedIdAndChangeKey = IdConverter.GetConcatenatedId(storeId, mailboxId, null);
			}
			return new ItemId(concatenatedIdAndChangeKey.Id, concatenatedIdAndChangeKey.ChangeKey);
		}

		private static object GetPropertyValue(object propertyValue, object defaultValueIfNull)
		{
			if (propertyValue != null && !(propertyValue is PropertyError))
			{
				return propertyValue;
			}
			return defaultValueIfNull;
		}

		private static string GetDateTimeAsString(object propertyValue)
		{
			object propertyValue2 = MailboxSearchConverter.GetPropertyValue(propertyValue, null);
			if (propertyValue2 != null)
			{
				return ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)propertyValue2);
			}
			return null;
		}

		internal static List<PropertyDefinition> CompactPreviewDataProperties = new List<PropertyDefinition>
		{
			StoreObjectSchema.ParentItemId,
			ItemSchema.Id,
			ItemSchema.Size
		};

		internal static List<PropertyDefinition> PreviewDataProperties = new List<PropertyDefinition>
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
	}
}
