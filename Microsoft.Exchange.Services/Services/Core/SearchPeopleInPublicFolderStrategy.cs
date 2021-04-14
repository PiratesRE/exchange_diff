using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SearchPeopleInPublicFolderStrategy : SearchPeopleStrategy
	{
		public SearchPeopleInPublicFolderStrategy(PublicFolderSession session, FindPeopleParameters parameters, StoreId searchScope, QueryFilter restrictionFilter) : base(parameters, restrictionFilter, searchScope)
		{
			this.publicFolderSession = session;
		}

		public override Persona[] Execute()
		{
			base.Log(FindPeopleMetadata.PersonalSearchMode, FindPeopleSearchFlavor.PublicFolderSearch);
			ExDateTime utcNow = ExDateTime.UtcNow;
			StorePerformanceCountersCapture storePerformanceCountersCapture = StorePerformanceCountersCapture.Start(this.publicFolderSession);
			FindPeopleResult findPeopleResult = null;
			QueryFilter queryFilter = new OrFilter(true, new QueryFilter[]
			{
				new TextFilter(StoreObjectSchema.DisplayName, base.QueryString, MatchOptions.PrefixOnWords, MatchFlags.IgnoreCase),
				new TextFilter(ContactSchema.CompanyName, base.QueryString, MatchOptions.PrefixOnWords, MatchFlags.IgnoreCase),
				new TextFilter(ContactSchema.GivenName, base.QueryString, MatchOptions.PrefixOnWords, MatchFlags.IgnoreCase),
				new TextFilter(ContactSchema.Surname, base.QueryString, MatchOptions.PrefixOnWords, MatchFlags.IgnoreCase)
			});
			QueryFilter queryFilter2 = (base.RestrictionFilter == null) ? queryFilter : new AndFilter(new QueryFilter[]
			{
				queryFilter,
				base.RestrictionFilter
			});
			using (Folder folder = Folder.Bind(this.publicFolderSession, base.SearchScope))
			{
				findPeopleResult = FindPeopleImplementation.QueryContactsInPublicFolder(this.publicFolderSession, folder, base.SortBy, (IndexedPageView)base.Paging, queryFilter2);
			}
			StorePerformanceCounters storePerformanceCounters = storePerformanceCountersCapture.Stop();
			ExDateTime utcNow2 = ExDateTime.UtcNow;
			base.Log(FindPeopleMetadata.PublicFolderSearchTime, storePerformanceCounters.ElapsedMilliseconds);
			base.Log(FindPeopleMetadata.PublicFolderSearchCPUTime, storePerformanceCounters.Cpu);
			base.Log(FindPeopleMetadata.PublicFolderSearchRpcCount, storePerformanceCounters.RpcCount);
			base.Log(FindPeopleMetadata.PublicFolderSearchRpcLatency, storePerformanceCounters.RpcLatency);
			base.Log(FindPeopleMetadata.PublicFolderSearchRpcLatencyOnStore, storePerformanceCounters.RpcLatencyOnStore);
			base.Log(FindPeopleMetadata.PublicFolderSearchStartTimestamp, SearchUtil.FormatIso8601String(utcNow));
			base.Log(FindPeopleMetadata.PublicFolderSearchEndTimestamp, SearchUtil.FormatIso8601String(utcNow2));
			return findPeopleResult.PersonaList;
		}

		private PublicFolderSession publicFolderSession;
	}
}
