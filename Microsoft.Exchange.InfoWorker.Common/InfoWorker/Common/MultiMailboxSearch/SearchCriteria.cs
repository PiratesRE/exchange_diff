using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Search.KqlParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class SearchCriteria
	{
		public SearchCriteria(string queryString, string propertyFilterAqs, CultureInfo culture, SearchType type, IRecipientSession recipientSession, IConfigurationSession configurationSession, Guid queryCorrelationId, List<DefaultFolderType> excludedFolders)
		{
			Util.ThrowOnNull(queryString, "queryString");
			Util.ThrowOnNull(culture, "culture");
			this.queryString = queryString;
			this.queryCulture = culture;
			this.searchType = type;
			this.recipientSession = recipientSession;
			QueryFilter queryFilter = null;
			QueryFilter queryFilter2 = this.GetQueryFilter(queryString, this.recipientSession, configurationSession, out queryFilter);
			if (queryFilter2 == null)
			{
				throw new SearchQueryEmptyException();
			}
			if (propertyFilterAqs != null)
			{
				this.propertyFilterQuery = this.GetQueryFilter(propertyFilterAqs, this.recipientSession, configurationSession, out queryFilter);
				this.finalQueryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter2,
					this.propertyFilterQuery
				});
			}
			else
			{
				this.propertyFilterQuery = null;
				this.finalQueryFilter = queryFilter2;
			}
			int actualKeywordCount = queryFilter.Keywords().Count<string>();
			int maxAllowedKeywords = Factory.Current.GetMaxAllowedKeywords(this.RecipientSession);
			this.ValidateKeywordLimits(actualKeywordCount, maxAllowedKeywords);
			if (this.IsStatisticsSearch)
			{
				this.subFilters = this.GetSubFilters(queryFilter);
				if (this.subFilters != null)
				{
					this.ValidateKeywordLimits(this.subFilters.Count, Factory.Current.GetMaxAllowedKeywordsPerPage(this.RecipientSession));
					Dictionary<string, QueryFilter> dictionary = new Dictionary<string, QueryFilter>(this.subFilters.Count);
					foreach (KeyValuePair<string, QueryFilter> keyValuePair in this.subFilters)
					{
						dictionary.Add(keyValuePair.Key, Util.CreateNewQueryFilterForGroupExpansionRecipientsIfApplicable(keyValuePair.Value));
					}
					foreach (KeyValuePair<string, QueryFilter> keyValuePair2 in dictionary)
					{
						this.subFilters[keyValuePair2.Key] = keyValuePair2.Value;
					}
				}
			}
			this.queryCorrelationId = queryCorrelationId;
			this.excludedFolders = ((excludedFolders == null) ? new List<DefaultFolderType>() : excludedFolders);
		}

		public string QueryString
		{
			get
			{
				return this.queryString;
			}
		}

		public QueryFilter Query
		{
			get
			{
				return this.finalQueryFilter;
			}
		}

		public IDictionary<string, QueryFilter> SubFilters
		{
			get
			{
				return this.subFilters;
			}
		}

		public SearchType SearchType
		{
			get
			{
				return this.searchType;
			}
		}

		public bool IsPreviewSearch
		{
			get
			{
				return SearchType.Preview == (this.searchType & SearchType.Preview);
			}
		}

		public bool IsStatisticsSearch
		{
			get
			{
				return SearchType.Statistics == (this.searchType & SearchType.Statistics);
			}
		}

		public CultureInfo QueryCulture
		{
			get
			{
				return this.queryCulture;
			}
		}

		public List<DefaultFolderType> ExcludedFolders
		{
			get
			{
				return this.excludedFolders;
			}
		}

		private IDictionary<string, QueryFilter> GetSubFilters(QueryFilter filter)
		{
			if (filter == null)
			{
				return null;
			}
			if (!(filter.GetType() == typeof(OrFilter)))
			{
				return null;
			}
			IDictionary<string, QueryFilter> subQueryString = this.GetSubQueryString(AqsParser.FlattenQueryFilter(filter));
			if (subQueryString != null && subQueryString.Count == 1)
			{
				Factory.Current.LocalTaskTracer.TraceInformation<Guid, string>(this.GetHashCode(), 0L, "Correlation Id:{0}. The query filter {1} is an OrFilter, but it only returns one sub filter.", this.queryCorrelationId, filter.ToString());
				return null;
			}
			return subQueryString;
		}

		internal IRecipientSession RecipientSession
		{
			get
			{
				return this.recipientSession;
			}
		}

		private void ValidateKeywordLimits(int actualKeywordCount, int maxAllowedKeywords)
		{
			if (actualKeywordCount > maxAllowedKeywords)
			{
				Factory.Current.LocalTaskTracer.TraceInformation(this.GetHashCode(), 0L, "Correlation Id:{0}. Max keywords allowed per statistics search call is {1}, the request for the query:{2}, containted {3} keywords.Failing the search.", new object[]
				{
					this.queryCorrelationId,
					maxAllowedKeywords,
					this.queryString,
					actualKeywordCount
				});
				throw new TooManyKeywordsException(actualKeywordCount, maxAllowedKeywords);
			}
		}

		private IDictionary<string, QueryFilter> GetSubQueryString(ICollection<QueryFilter> filters)
		{
			int maxAllowedKeywords = Factory.Current.GetMaxAllowedKeywords(this.RecipientSession);
			if (filters.Count > maxAllowedKeywords)
			{
				throw new TooManyKeywordsException(filters.Count, maxAllowedKeywords);
			}
			int num = 0;
			Dictionary<string, QueryFilter> dictionary = new Dictionary<string, QueryFilter>(filters.Count);
			foreach (QueryFilter queryFilter in filters)
			{
				string keywordPhrase = MailboxDiscoverySearch.GetKeywordPhrase(queryFilter, this.queryString, ref num);
				QueryFilter value = queryFilter;
				if (this.propertyFilterQuery != null)
				{
					value = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						this.propertyFilterQuery
					});
				}
				if (!dictionary.ContainsKey(keywordPhrase))
				{
					dictionary.Add(keywordPhrase, value);
				}
			}
			return dictionary;
		}

		private QueryFilter GetQueryFilter(string queryString, IRecipientSession recipientSession, IConfigurationSession configurationSession, out QueryFilter nonDiscoveryQueryFilter)
		{
			SearchCriteria.RecipientIdentityResolver recipientResolver = null;
			if (recipientSession != null)
			{
				recipientResolver = new SearchCriteria.RecipientIdentityResolver(recipientSession);
			}
			PolicyTagAdProvider policyTagProvider = null;
			if (configurationSession != null)
			{
				policyTagProvider = new PolicyTagAdProvider(configurationSession);
			}
			KqlParser.ParseOption parseOption = KqlParser.ParseOption.ImplicitOr | KqlParser.ParseOption.UseCiKeywordOnly | KqlParser.ParseOption.DisablePrefixMatch | KqlParser.ParseOption.AllowShortWildcards;
			nonDiscoveryQueryFilter = KqlParser.ParseAndBuildQuery(queryString, parseOption, this.queryCulture, RescopedAll.Default, recipientResolver, policyTagProvider);
			parseOption |= KqlParser.ParseOption.EDiscoveryMode;
			return KqlParser.ParseAndBuildQuery(queryString, parseOption, this.queryCulture, RescopedAll.Default, recipientResolver, policyTagProvider);
		}

		private readonly string queryString;

		private readonly IRecipientSession recipientSession;

		private readonly CultureInfo queryCulture;

		private readonly QueryFilter propertyFilterQuery;

		private readonly QueryFilter finalQueryFilter;

		private readonly IDictionary<string, QueryFilter> subFilters;

		private readonly SearchType searchType;

		private readonly Guid queryCorrelationId;

		private readonly List<DefaultFolderType> excludedFolders;

		private class RecipientIdentityResolver : IRecipientResolver
		{
			internal RecipientIdentityResolver(IRecipientSession recipientSession)
			{
				this.recipientSession = recipientSession;
			}

			public string[] Resolve(string identity)
			{
				RecipientIdParameter recipientIdParameter = new RecipientIdParameter(identity);
				IEnumerable<ADRecipient> objects = recipientIdParameter.GetObjects<ADRecipient>(null, this.recipientSession);
				if (objects == null)
				{
					return null;
				}
				List<string> list = new List<string>(4);
				foreach (ADRecipient adrecipient in objects)
				{
					list.Add(adrecipient.DisplayName);
					list.Add(adrecipient.Alias);
					list.Add(adrecipient.LegacyExchangeDN);
					list.Add(adrecipient.PrimarySmtpAddress.ToString());
				}
				return list.ToArray();
			}

			private IRecipientSession recipientSession;
		}
	}
}
