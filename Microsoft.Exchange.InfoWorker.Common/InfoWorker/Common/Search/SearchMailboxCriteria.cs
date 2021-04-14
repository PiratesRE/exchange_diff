using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Search.KqlParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class SearchMailboxCriteria
	{
		internal SearchMailboxCriteria(CultureInfo queryCulture, string searchQuery, SearchUser[] searchUserScope) : this(queryCulture, searchQuery, null, searchUserScope)
		{
		}

		internal SearchMailboxCriteria(CultureInfo queryCulture, string searchQuery, string userQuery, SearchUser[] searchUserScope)
		{
			if (searchUserScope == null)
			{
				throw new ArgumentNullException("searchUserScope");
			}
			if (queryCulture == null)
			{
				throw new ArgumentNullException("queryCulture");
			}
			if (!string.IsNullOrEmpty(searchQuery))
			{
				this.searchQuery = searchQuery;
			}
			if (!string.IsNullOrEmpty(userQuery))
			{
				this.userQuery = userQuery;
			}
			this.searchUserScope = searchUserScope;
			this.queryCulture = queryCulture;
		}

		internal SearchUser[] SearchUserScope
		{
			get
			{
				return this.searchUserScope;
			}
		}

		internal CultureInfo QueryCulture
		{
			get
			{
				return this.queryCulture;
			}
		}

		internal string SearchQuery
		{
			get
			{
				return this.searchQuery;
			}
		}

		internal QueryFilter SearchFilter
		{
			get
			{
				return this.searchFilter;
			}
		}

		internal IDictionary<string, QueryFilter> SubSearchFilters
		{
			get
			{
				return this.subfilters;
			}
		}

		internal bool SearchDumpster
		{
			get
			{
				return this.searchDumpster;
			}
			set
			{
				this.searchDumpster = value;
			}
		}

		internal string UserQuery
		{
			get
			{
				return this.userQuery;
			}
		}

		internal bool SearchDumpsterOnly
		{
			get
			{
				return this.searchDumpsterOnly;
			}
			set
			{
				this.searchDumpsterOnly = value;
			}
		}

		internal bool ExcludePurgesFromDumpster
		{
			get
			{
				return this.excludePurgesFromDumpster;
			}
			set
			{
				this.excludePurgesFromDumpster = value;
			}
		}

		internal bool IncludeUnsearchableItems { get; set; }

		internal bool IncludePersonalArchive { get; set; }

		internal bool IncludeRemoteAccounts { get; set; }

		internal bool EstimateOnly { get; set; }

		internal bool ExcludeDuplicateMessages { get; set; }

		public override string ToString()
		{
			return string.Format("SearchQuery={0}. Culture={1}. SearchUserScope count={2}. SearchDumpster={3}. IncludeUnsearchableItems={4}. IncludePersonalArchive={5}. IncludeRemoteAccounts={6}", new object[]
			{
				this.SearchQuery,
				this.QueryCulture,
				this.searchUserScope.Length,
				this.SearchDumpster,
				this.IncludeUnsearchableItems,
				this.IncludePersonalArchive,
				this.IncludeRemoteAccounts
			});
		}

		internal void ResolveQueryFilter(IRecipientSession recipientSession, IConfigurationSession configurationSession)
		{
			SearchMailboxCriteria.RecipientIdentityResolver recipientResolver = null;
			if (recipientSession != null)
			{
				recipientResolver = new SearchMailboxCriteria.RecipientIdentityResolver(recipientSession);
			}
			PolicyTagAdProvider policyTagProvider = null;
			if (configurationSession != null)
			{
				policyTagProvider = new PolicyTagAdProvider(configurationSession);
			}
			if (this.SearchQuery != null)
			{
				this.searchFilter = KqlParser.ParseAndBuildQuery(this.SearchQuery, KqlParser.ParseOption.ImplicitOr | KqlParser.ParseOption.UseCiKeywordOnly | KqlParser.ParseOption.DisablePrefixMatch | KqlParser.ParseOption.AllowShortWildcards | KqlParser.ParseOption.EDiscoveryMode, this.QueryCulture, RescopedAll.Default, recipientResolver, policyTagProvider);
				if (this.searchFilter == null)
				{
					throw new SearchQueryEmptyException();
				}
			}
			SearchMailboxCriteria.Tracer.TraceDebug<QueryFilter>((long)this.GetHashCode(), "SearchMailboxCriteria resolved QueryFilter:{0}", this.searchFilter);
		}

		internal StoreId[] GetFolderScope(MailboxSession mailbox)
		{
			StoreId defaultFolderId = mailbox.GetDefaultFolderId(DefaultFolderType.Root);
			StoreId defaultFolderId2 = mailbox.GetDefaultFolderId(DefaultFolderType.RecoverableItemsRoot);
			StoreId[] array;
			if (!this.SearchDumpsterOnly)
			{
				array = new StoreId[]
				{
					defaultFolderId
				};
			}
			else
			{
				array = new StoreId[0];
			}
			if ((this.SearchDumpsterOnly || this.SearchDumpster) && defaultFolderId2 != null)
			{
				QueryFilter queryFilter = DumpsterFolderHelper.ExcludeAuditFoldersFilter;
				if (this.excludePurgesFromDumpster && SearchUtils.LegalHoldEnabled(mailbox))
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.DisplayName, "DiscoveryHolds")
					});
					if (mailbox.COWSettings.HoldEnabled() && !mailbox.COWSettings.IsOnlyInPlaceHoldEnabled())
					{
						queryFilter = new AndFilter(new QueryFilter[]
						{
							queryFilter,
							new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.DisplayName, "Purges")
						});
					}
				}
				using (Folder folder = Folder.Bind(mailbox, defaultFolderId2))
				{
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, queryFilter, null, new PropertyDefinition[]
					{
						FolderSchema.Id
					}))
					{
						array = array.Concat(queryResult.Enumerator<StoreId>()).ToArray<StoreId>();
					}
				}
			}
			return array;
		}

		internal void GenerateSubQueryFilters(IRecipientSession recipientSession, IConfigurationSession configurationSession)
		{
			if (!string.IsNullOrEmpty(this.userQuery))
			{
				SearchMailboxCriteria.RecipientIdentityResolver recipientResolver = null;
				if (recipientSession != null)
				{
					recipientResolver = new SearchMailboxCriteria.RecipientIdentityResolver(recipientSession);
				}
				PolicyTagAdProvider policyTagProvider = null;
				if (configurationSession != null)
				{
					policyTagProvider = new PolicyTagAdProvider(configurationSession);
				}
				QueryFilter queryFilter = KqlParser.ParseAndBuildQuery(this.userQuery, KqlParser.ParseOption.ImplicitOr | KqlParser.ParseOption.UseCiKeywordOnly | KqlParser.ParseOption.DisablePrefixMatch | KqlParser.ParseOption.AllowShortWildcards | KqlParser.ParseOption.EDiscoveryMode, this.QueryCulture, RescopedAll.Default, recipientResolver, policyTagProvider);
				if (queryFilter == null)
				{
					throw new SearchQueryEmptyException();
				}
				ICollection<QueryFilter> collection = null;
				if (queryFilter != null && queryFilter.GetType() == typeof(OrFilter))
				{
					collection = AqsParser.FlattenQueryFilter(queryFilter);
				}
				if (collection != null && collection.Count > 1)
				{
					string text = this.searchQuery.Replace(this.userQuery, "");
					QueryFilter queryFilter2 = null;
					if (!string.IsNullOrEmpty(text))
					{
						queryFilter2 = KqlParser.ParseAndBuildQuery(text, KqlParser.ParseOption.ImplicitOr | KqlParser.ParseOption.UseCiKeywordOnly | KqlParser.ParseOption.DisablePrefixMatch | KqlParser.ParseOption.AllowShortWildcards | KqlParser.ParseOption.EDiscoveryMode, this.QueryCulture, RescopedAll.Default, recipientResolver, policyTagProvider);
					}
					this.subfilters = new Dictionary<string, QueryFilter>(collection.Count);
					int num = 0;
					foreach (QueryFilter queryFilter3 in collection)
					{
						string keywordPhrase = MailboxDiscoverySearch.GetKeywordPhrase(queryFilter3, this.userQuery, ref num);
						QueryFilter value;
						if (queryFilter2 == null)
						{
							value = queryFilter3;
						}
						else
						{
							value = new AndFilter(new QueryFilter[]
							{
								queryFilter2,
								queryFilter3
							});
						}
						if (!this.subfilters.ContainsKey(keywordPhrase))
						{
							this.subfilters.Add(keywordPhrase, value);
						}
					}
				}
			}
		}

		protected static readonly Trace Tracer = ExTraceGlobals.SearchTracer;

		private SearchUser[] searchUserScope;

		private CultureInfo queryCulture;

		private string searchQuery;

		private string userQuery = string.Empty;

		private bool searchDumpster;

		private bool excludePurgesFromDumpster;

		private bool searchDumpsterOnly;

		private QueryFilter searchFilter;

		private IDictionary<string, QueryFilter> subfilters;

		internal static readonly ADPropertyDefinition[] RecipientTypeDetailsProperty = new ADPropertyDefinition[]
		{
			ADRecipientSchema.RecipientTypeDetailsValue,
			ADRecipientSchema.RawCapabilities,
			IADSecurityPrincipalSchema.SamAccountName
		};

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
				List<string> list = new List<string>();
				foreach (ADRecipient adrecipient in objects)
				{
					list.Add(adrecipient.DisplayName);
					list.Add(adrecipient.Alias);
					list.Add(adrecipient.LegacyExchangeDN);
					list.Add(adrecipient.PrimarySmtpAddress.ToString());
				}
				SearchMailboxCriteria.Tracer.TraceDebug<string, List<string>>((long)this.GetHashCode(), "SearchMailboxCriteria resolving Identity {0} to {1}", identity, list);
				return list.ToArray();
			}

			private IRecipientSession recipientSession;
		}
	}
}
