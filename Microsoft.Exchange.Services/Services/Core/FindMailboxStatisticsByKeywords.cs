using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class FindMailboxStatisticsByKeywords : MultiStepServiceCommand<FindMailboxStatisticsByKeywordsRequest, KeywordStatisticsSearchResult>, IDisposeTrackable, IDisposable
	{
		public FindMailboxStatisticsByKeywords(CallContext callContext, FindMailboxStatisticsByKeywordsRequest request) : base(callContext, request)
		{
			this.SaveRequestData(request);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FindMailboxStatisticsByKeywords>(this);
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

		internal override bool SupportsExternalUsers
		{
			get
			{
				return true;
			}
		}

		internal override Offer ExpectedOffer
		{
			get
			{
				return Offer.MailboxSearch;
			}
		}

		internal override TimeSpan? MaxExecutionTime
		{
			get
			{
				return FindMailboxStatisticsByKeywords.DefaultMaxExecutionTime;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			FindMailboxStatisticsByKeywordsResponse findMailboxStatisticsByKeywordsResponse = new FindMailboxStatisticsByKeywordsResponse();
			findMailboxStatisticsByKeywordsResponse.AddResponses(this.currentMailbox, base.Results);
			return findMailboxStatisticsByKeywordsResponse;
		}

		internal override void PreExecuteCommand()
		{
			this.ValidateRequestData();
			this.PerformAuthorization();
			try
			{
				this.MailboxSearch.CreateSearchMailboxCriteria(this.language, this.ConvertCollectionToMultiValuedProperty<string>(this.senders), this.ConvertCollectionToMultiValuedProperty<string>(this.recipients), this.fromDate, this.toDate, this.ConvertCollectionToMultiValuedProperty<KindKeyword>(this.messageTypes), this.userQuery, this.searchDumpster, this.includeUnsearchableItems, this.includePersonalArchive);
			}
			catch (ParserException ex)
			{
				ExTraceGlobals.SearchTracer.TraceError<string, string>((long)this.GetHashCode(), "Query: \"{0}\" returns ParserException: {1}", this.userQuery, ex.ToString());
				throw new ServiceArgumentException((CoreResources.IDs)3021008902U);
			}
			if (this.MailboxSearch.SearchCriteria.SubSearchFilters == null)
			{
				this.stepCount = 1;
			}
			else
			{
				if (this.MailboxSearch.SearchCriteria.SubSearchFilters.Count > 0)
				{
					this.actualKeywords = new string[this.MailboxSearch.SearchCriteria.SubSearchFilters.Count];
					int num = 0;
					foreach (string text in this.MailboxSearch.SearchCriteria.SubSearchFilters.Keys)
					{
						this.actualKeywords[num++] = text;
					}
				}
				this.stepCount = this.MailboxSearch.SearchCriteria.SubSearchFilters.Count + 1;
			}
			if (this.includeUnsearchableItems)
			{
				this.stepCount++;
			}
		}

		internal override ServiceResult<KeywordStatisticsSearchResult> Execute()
		{
			KeywordStatisticsSearchResult keywordStatisticsSearchResult;
			if (base.CurrentStep == 0)
			{
				this.mailboxSearch.CreateSearchFolderAndPerformInitialEstimation();
				keywordStatisticsSearchResult = this.GetSearchResult(this.mailboxSearch.SearchCriteria.UserQuery);
				if (keywordStatisticsSearchResult.ItemHits == 0)
				{
					this.skipProcessingKeywords = true;
				}
			}
			else if (this.includeUnsearchableItems && base.CurrentStep == this.stepCount - 1)
			{
				keywordStatisticsSearchResult = this.GetSearchResult("652beee2-75f7-4ca0-8a02-0698a3919cb9");
			}
			else
			{
				string keyword = this.actualKeywords[base.CurrentStep - 1];
				if (this.skipProcessingKeywords)
				{
					keywordStatisticsSearchResult = new KeywordStatisticsSearchResult
					{
						Keyword = keyword,
						ItemHits = 0,
						Size = 0UL
					};
				}
				else
				{
					this.mailboxSearch.PerformSingleKeywordSearch(keyword);
					keywordStatisticsSearchResult = this.GetSearchResult(keyword);
				}
			}
			return new ServiceResult<KeywordStatisticsSearchResult>(keywordStatisticsSearchResult);
		}

		private MailboxSearch MailboxSearch
		{
			get
			{
				return this.mailboxSearch;
			}
		}

		private void Dispose(bool fromDispose)
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			if (!this.disposed)
			{
				if (this.mailboxSearch != null)
				{
					this.mailboxSearch.Dispose();
					this.mailboxSearch = null;
				}
				this.disposed = true;
			}
		}

		private void SaveRequestData(FindMailboxStatisticsByKeywordsRequest request)
		{
			this.mailboxes = request.Mailboxes;
			this.keywords = request.Keywords;
			this.language = this.GetCultureInfo(request.Language);
			this.senders = request.Senders;
			this.recipients = request.Recipients;
			this.fromDate = (request.FromDateSpecified ? request.FromDate : DateTime.MinValue);
			this.toDate = (request.ToDateSpecified ? request.ToDate : DateTime.MinValue);
			this.messageTypes = this.GetMessageTypes(request.MessageTypes);
			this.searchDumpster = (request.SearchDumpsterSpecified && request.SearchDumpster);
			this.includePersonalArchive = (request.IncludePersonalArchiveSpecified && request.IncludePersonalArchive);
			this.includeUnsearchableItems = (request.IncludeUnsearchableItemsSpecified && request.IncludeUnsearchableItems);
			this.currentMailbox = this.mailboxes[0];
			this.userQuery = this.keywords[0];
		}

		private void ValidateRequestData()
		{
			if (this.mailboxes.Length > 1)
			{
				throw new ServiceArgumentException((CoreResources.IDs)3169826345U);
			}
			if (this.userQuery.Length > FindMailboxStatisticsByKeywords.MaxQueryLength)
			{
				throw new ServiceArgumentException(CoreResources.IDs.ErrorInvalidSearchQueryLength);
			}
			if ((!this.fromDate.Equals(DateTime.MinValue) && this.fromDate.Kind != DateTimeKind.Utc) || (!this.toDate.Equals(DateTime.MinValue) && this.toDate.Kind != DateTimeKind.Utc))
			{
				throw new ServiceArgumentException((CoreResources.IDs)2643283981U);
			}
			try
			{
				if (this.currentMailbox.IsArchive)
				{
					new Guid(this.currentMailbox.Id);
				}
				else
				{
					new SmtpAddress(this.currentMailbox.Id);
				}
			}
			catch (Exception)
			{
				throw new ServiceArgumentException(CoreResources.IDs.ErrorInvalidMailboxIdFormat);
			}
		}

		private MultiValuedProperty<T> ConvertCollectionToMultiValuedProperty<T>(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				return null;
			}
			MultiValuedProperty<T> multiValuedProperty = new MultiValuedProperty<T>();
			foreach (T item in collection)
			{
				if (!multiValuedProperty.Contains(item))
				{
					multiValuedProperty.Add(item);
				}
			}
			return multiValuedProperty;
		}

		private CultureInfo GetCultureInfo(string cultureInfoString)
		{
			CultureInfo cultureInfo = null;
			try
			{
				if (!string.IsNullOrEmpty(cultureInfoString))
				{
					cultureInfo = new CultureInfo(cultureInfoString);
				}
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.SearchTracer.TraceDebug<string>((long)this.GetHashCode(), "Exception trying to parse culture info string: {0}", ex.ToString());
			}
			if (cultureInfo == null)
			{
				ExTraceGlobals.SearchTracer.TraceDebug((long)this.GetHashCode(), "Invalid or no query language is specified, default to en-us");
				cultureInfo = new CultureInfo("en-us");
			}
			return cultureInfo;
		}

		private KindKeyword[] GetMessageTypes(SearchItemKind[] messageTypesFromRequest)
		{
			if (messageTypesFromRequest == null || messageTypesFromRequest.Length == 0)
			{
				return null;
			}
			KindKeyword[] array = new KindKeyword[messageTypesFromRequest.Length];
			int num = 0;
			foreach (SearchItemKind searchItemKind in messageTypesFromRequest)
			{
				array[num++] = (KindKeyword)Enum.Parse(typeof(KindKeyword), searchItemKind.ToString(), true);
			}
			return array;
		}

		private void PerformAuthorization()
		{
			try
			{
				bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Ews.ExternalUser.Enabled;
				if (base.CallContext.IsExternalUser)
				{
					ExternalCallContext externalCallContext = (ExternalCallContext)base.CallContext;
					SmtpAddress emailAddress = externalCallContext.EmailAddress;
					OrganizationId organizationId = DomainToOrganizationIdCache.Singleton.Get(new SmtpDomain(emailAddress.Domain));
					if (organizationId == null)
					{
						ExTraceGlobals.SearchTracer.TraceError((long)this.GetHashCode(), "Organization cannot be found.");
						throw new ServiceAccessDeniedException();
					}
					if (!enabled)
					{
						ExTraceGlobals.SearchTracer.TraceError((long)this.GetHashCode(), "External user is not supported in non datacenter scenario.");
						throw new ServiceAccessDeniedException();
					}
					OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
					OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(emailAddress.Domain);
					if (organizationRelationship == null)
					{
						ExTraceGlobals.SearchTracer.TraceError((long)this.GetHashCode(), "Organization relationship for the organization cannot be found.");
						throw new ServiceAccessDeniedException();
					}
					if (!organizationRelationship.Enabled)
					{
						ExTraceGlobals.SearchTracer.TraceError((long)this.GetHashCode(), "Organization relationship is not enabled.");
						throw new ServiceAccessDeniedException();
					}
					this.mailboxSearch = new MailboxSearch(this.currentMailbox.Id, this.currentMailbox.IsArchive, organizationId);
					if (!organizationId.Equals(this.MailboxSearch.OrganizationId))
					{
						ExTraceGlobals.SearchTracer.TraceError((long)this.GetHashCode(), "User does not belong to same organization.");
						throw new ServiceAccessDeniedException();
					}
				}
				else
				{
					if (enabled)
					{
						ExTraceGlobals.SearchTracer.TraceError((long)this.GetHashCode(), "Non external user is not supported in datacenter scenario.");
						throw new ServiceAccessDeniedException();
					}
					this.mailboxSearch = new MailboxSearch(this.currentMailbox.Id, this.currentMailbox.IsArchive, OrganizationId.ForestWideOrgId);
					ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = ExchangeRunspaceConfigurationCache.Singleton.Get(base.CallContext.EffectiveCaller, null, false);
					if (!exchangeRunspaceConfiguration.HasRoleOfType(RoleType.MailboxSearch))
					{
						ExTraceGlobals.SearchTracer.TraceError((long)this.GetHashCode(), "User does not have mailbox search role.");
						throw new ServiceAccessDeniedException();
					}
				}
			}
			catch (AuthzException innerException)
			{
				throw new ServiceAccessDeniedException(innerException);
			}
		}

		private KeywordStatisticsSearchResult GetSearchResult(string keyword)
		{
			KeywordStatisticsSearchResult keywordStatisticsSearchResult = new KeywordStatisticsSearchResult();
			keywordStatisticsSearchResult.Keyword = keyword;
			if (this.mailboxSearch.SearchResult != null && this.mailboxSearch.SearchResult.SubQueryResults.ContainsKey(keyword))
			{
				keywordStatisticsSearchResult.ItemHits = this.mailboxSearch.SearchResult.SubQueryResults[keyword].Count;
				keywordStatisticsSearchResult.Size = this.mailboxSearch.SearchResult.SubQueryResults[keyword].Size.ToBytes();
			}
			return keywordStatisticsSearchResult;
		}

		private static readonly TimeSpan? DefaultMaxExecutionTime = new TimeSpan?(TimeSpan.FromMinutes(10.0));

		private static readonly int MaxQueryLength = 10240;

		private readonly DisposeTracker disposeTracker;

		private int stepCount;

		private UserMailbox[] mailboxes;

		private string[] keywords;

		private string[] actualKeywords;

		private string userQuery;

		private CultureInfo language;

		private string[] senders;

		private string[] recipients;

		private DateTime fromDate;

		private DateTime toDate;

		private KindKeyword[] messageTypes;

		private bool searchDumpster;

		private bool includePersonalArchive;

		private bool includeUnsearchableItems;

		private UserMailbox currentMailbox;

		private MailboxSearch mailboxSearch;

		private bool skipProcessingKeywords;

		private bool disposed;
	}
}
