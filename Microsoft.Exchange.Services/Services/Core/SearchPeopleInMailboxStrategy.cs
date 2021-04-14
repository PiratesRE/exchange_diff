using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SearchPeopleInMailboxStrategy : SearchPeopleStrategy
	{
		internal SearchPeopleInMailboxStrategy(FindPeopleParameters parameters, MailboxSession mailboxSession, StoreId searchScope, QueryFilter restrictionFilter, QueryFilter aggregationRestrictionFilter, AggregationExtension aggregationExtension) : base(parameters, restrictionFilter, searchScope)
		{
			this.mailboxSession = mailboxSession;
			this.AggregationRestrictionFilter = aggregationRestrictionFilter;
			this.aggregationExtension = aggregationExtension;
		}

		public override Persona[] Execute()
		{
			base.Log(FindPeopleMetadata.PersonalSearchMode, FindPeopleSearchFlavor.MailboxSearch);
			Persona[] result = null;
			using (SearchFolder searchFolder = this.CreateSearchFolder())
			{
				base.Log(FindPeopleMetadata.FolderId, searchFolder.Id);
				base.Log(FindPeopleMetadata.CorrelationId, searchFolder.GetValueOrDefault<Guid>(FolderSchema.CorrelationId, Guid.Empty));
				bool flag = this.SearchAndWait(searchFolder, FindPeopleConfiguration.FastSearchTimeoutInMilliseconds.Member);
				base.Log(FindPeopleMetadata.PersonalSearchSuccessful, flag);
				if (flag)
				{
					result = this.GetPageResult(searchFolder);
				}
				else
				{
					result = Array<Persona>.Empty;
				}
			}
			return result;
		}

		private SearchFolder CreateSearchFolder()
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			StorePerformanceCountersCapture storePerformanceCountersCapture = StorePerformanceCountersCapture.Start(this.mailboxSession);
			StoreObjectId defaultFolderId = this.mailboxSession.GetDefaultFolderId(DefaultFolderType.SearchFolders);
			if (defaultFolderId == null)
			{
				ExTraceGlobals.ExceptionTracer.TraceError(0L, "GetDefaultFolderId returned null for DefaultFolderType.SearchFolders. FolderNotFoundException will be thrown.");
				throw new FolderNotFoundException();
			}
			SearchFolder searchFolder = SearchFolder.Create(this.mailboxSession, defaultFolderId, "EWS People Search " + Guid.NewGuid().ToString(), CreateMode.InstantSearch);
			bool flag = false;
			try
			{
				searchFolder.Save();
				searchFolder.Load();
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					searchFolder.Dispose();
					searchFolder = null;
				}
			}
			StorePerformanceCounters storePerformanceCounters = storePerformanceCountersCapture.Stop();
			ExDateTime utcNow2 = ExDateTime.UtcNow;
			base.Log(FindPeopleMetadata.CreateSearchFolderTime, storePerformanceCounters.ElapsedMilliseconds);
			base.Log(FindPeopleMetadata.CreateSearchFolderCPUTime, storePerformanceCounters.Cpu);
			base.Log(FindPeopleMetadata.CreateSearchFolderRpcCount, storePerformanceCounters.RpcCount);
			base.Log(FindPeopleMetadata.CreateSearchFolderRpcLatency, storePerformanceCounters.RpcLatency);
			base.Log(FindPeopleMetadata.CreateSearchFolderRpcLatencyOnStore, storePerformanceCounters.RpcLatencyOnStore);
			base.Log(FindPeopleMetadata.CreateSearchFolderStartTimestamp, SearchUtil.FormatIso8601String(utcNow));
			base.Log(FindPeopleMetadata.CreateSearchFolderEndTimestamp, SearchUtil.FormatIso8601String(utcNow2));
			return searchFolder;
		}

		private bool SearchAndWait(SearchFolder searchFolder, int timeout)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			StorePerformanceCountersCapture storePerformanceCountersCapture = StorePerformanceCountersCapture.Start(this.mailboxSession);
			bool flag;
			using (SearchPeopleInMailboxStrategy.SearchCompletedNotificationHandler searchCompletedNotificationHandler = new SearchPeopleInMailboxStrategy.SearchCompletedNotificationHandler())
			{
				using (Subscription.Create(this.mailboxSession, new NotificationHandler(searchCompletedNotificationHandler.Complete), NotificationType.SearchComplete, searchFolder.StoreObjectId))
				{
					this.ApplySearchFolderCriteria(searchFolder);
					flag = searchCompletedNotificationHandler.Wait(timeout);
					base.Log(FindPeopleMetadata.PopulateSearchFolderNotificationQueueTime, searchCompletedNotificationHandler.NotificationQueueTime);
				}
			}
			if (flag)
			{
				StorePerformanceCounters storePerformanceCounters = storePerformanceCountersCapture.Stop();
				ExDateTime utcNow2 = ExDateTime.UtcNow;
				base.Log(FindPeopleMetadata.PopulateSearchFolderTime, storePerformanceCounters.ElapsedMilliseconds);
				base.Log(FindPeopleMetadata.PopulateSearchFolderCPUTime, storePerformanceCounters.Cpu);
				base.Log(FindPeopleMetadata.PopulateSearchFolderRpcCount, storePerformanceCounters.RpcCount);
				base.Log(FindPeopleMetadata.PopulateSearchFolderRpcLatency, storePerformanceCounters.RpcLatency);
				base.Log(FindPeopleMetadata.PopulateSearchFolderRpcLatencyOnStore, storePerformanceCounters.RpcLatencyOnStore);
				base.Log(FindPeopleMetadata.PopulateSearchFolderStartTimestamp, SearchUtil.FormatIso8601String(utcNow));
				base.Log(FindPeopleMetadata.PopulateSearchFolderEndTimestamp, SearchUtil.FormatIso8601String(utcNow2));
			}
			return flag;
		}

		private void ApplySearchFolderCriteria(SearchFolder searchFolder)
		{
			QueryFilter searchQuery = this.ComputeQueryFilter();
			StoreObjectId[] folderScope = this.ComputeSearchFolderScope();
			SearchFolderCriteria searchFolderCriteria = new SearchFolderCriteria(searchQuery, folderScope)
			{
				DeepTraversal = false
			};
			ContactsSearchFolderCriteria.ApplyOneTimeSearchFolderCriteria(XSOFactory.Default, this.mailboxSession, searchFolder, searchFolderCriteria);
		}

		private StoreObjectId[] ComputeSearchFolderScope()
		{
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(base.SearchScope);
			if (asStoreObjectId.Equals(this.mailboxSession.GetDefaultFolderId(DefaultFolderType.MyContacts)))
			{
				StoreObjectId[] value = this.mailboxSession.ContactFolders.MyContactFolders.Value;
				if (value != null && value.Length > 0)
				{
					ExTraceGlobals.FindPeopleCallTracer.TraceDebug(0L, "Search scope is MyContacts search folder, using cached folderIds");
					return value;
				}
			}
			if (asStoreObjectId.ObjectType == StoreObjectType.SearchFolder)
			{
				ExTraceGlobals.FindPeopleCallTracer.TraceDebug<StoreId>(0L, "Search scope is a search folder: {0}. Using its folder scope.", base.SearchScope);
				using (SearchFolder searchFolder = SearchFolder.Bind(this.mailboxSession, base.SearchScope))
				{
					List<StoreObjectId> list = new List<StoreObjectId>();
					foreach (StoreId storeId in searchFolder.GetSearchCriteria().FolderScope)
					{
						if (storeId != null)
						{
							list.Add((StoreObjectId)storeId);
						}
					}
					return list.ToArray();
				}
			}
			ExTraceGlobals.FindPeopleCallTracer.TraceDebug<StoreId>(0L, "Search scope is folder passed as param: {0}", base.SearchScope);
			return new StoreObjectId[]
			{
				(StoreObjectId)base.SearchScope
			};
		}

		private QueryFilter ComputeQueryFilter()
		{
			QueryFilter queryFilter = AqsParser.ParseAndBuildQuery(base.QueryString, AqsParser.ParseOption.SuppressError | AqsParser.ParseOption.UseBasicKeywordsOnly | AqsParser.ParseOption.AllowShortWildcards, this.parameters.CultureInfo, RescopedAll.Default, null, null);
			if (base.RestrictionFilter != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					base.RestrictionFilter,
					queryFilter
				});
			}
			return queryFilter;
		}

		private QueryFilter ComputeAggregationFilter()
		{
			QueryFilter queryFilter = SearchPeopleInMailboxStrategy.IsContactOrPDL;
			if (this.AggregationRestrictionFilter != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					this.AggregationRestrictionFilter,
					queryFilter
				});
			}
			return queryFilter;
		}

		private Persona[] GetPageResult(SearchFolder searchFolder)
		{
			PropertyListForViewRowDeterminer propertyListForViewRowDeterminer = PropertyListForViewRowDeterminer.BuildForPersonObjects(this.parameters.PersonaShape);
			List<PropertyDefinition> list = new List<PropertyDefinition>(propertyListForViewRowDeterminer.GetPropertiesToFetch());
			ExDateTime utcNow = ExDateTime.UtcNow;
			StorePerformanceCountersCapture storePerformanceCountersCapture = StorePerformanceCountersCapture.Start(this.mailboxSession);
			Persona[] result = null;
			using (IQueryResult queryResult = searchFolder.PersonItemQuery(null, this.ComputeAggregationFilter(), base.SortBy, list, this.aggregationExtension))
			{
				IdAndSession idAndSession = new IdAndSession(searchFolder.Id.ObjectId, this.mailboxSession);
				BasePageResult basePageResult = BasePagingType.ApplyPostQueryPaging(queryResult, base.Paging);
				Stopwatch stopwatch = Stopwatch.StartNew();
				result = basePageResult.View.ConvertPersonViewToPersonaObjects(list.ToArray(), propertyListForViewRowDeterminer, idAndSession);
				stopwatch.Stop();
				base.Log(FindPeopleMetadata.PersonalDataConversion, stopwatch.ElapsedMilliseconds);
			}
			StorePerformanceCounters storePerformanceCounters = storePerformanceCountersCapture.Stop();
			ExDateTime utcNow2 = ExDateTime.UtcNow;
			base.Log(FindPeopleMetadata.AggregateDataTime, storePerformanceCounters.ElapsedMilliseconds);
			base.Log(FindPeopleMetadata.AggregateDataCPUTime, storePerformanceCounters.Cpu);
			base.Log(FindPeopleMetadata.AggregateDataRpcCount, storePerformanceCounters.RpcCount);
			base.Log(FindPeopleMetadata.AggregateDataRpcLatency, storePerformanceCounters.RpcLatency);
			base.Log(FindPeopleMetadata.AggregateDataRpcLatencyOnStore, storePerformanceCounters.RpcLatencyOnStore);
			base.Log(FindPeopleMetadata.AggregateDataStartTimestamp, SearchUtil.FormatIso8601String(utcNow));
			base.Log(FindPeopleMetadata.AggregateDataEndTimestamp, SearchUtil.FormatIso8601String(utcNow2));
			return result;
		}

		private const string EwsPeopleSearchFolderPrefix = "EWS People Search ";

		private MailboxSession mailboxSession;

		private QueryFilter AggregationRestrictionFilter;

		private AggregationExtension aggregationExtension;

		private static readonly QueryFilter IsContactOrPDL = new OrFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, PersonSchema.PersonType, PersonType.Person),
			new ComparisonFilter(ComparisonOperator.Equal, PersonSchema.PersonType, PersonType.DistributionList)
		});

		private static readonly PropertyListForViewRowDeterminer IdOnlyDeterminer = PropertyListForViewRowDeterminer.BuildForPersonObjects(Persona.IdAndEmailPersonaShape);

		private static readonly List<PropertyDefinition> IdOnlyFetchList = new List<PropertyDefinition>(SearchPeopleInMailboxStrategy.IdOnlyDeterminer.GetPropertiesToFetch());

		private sealed class SearchCompletedNotificationHandler : IDisposable
		{
			public SearchCompletedNotificationHandler()
			{
				this.searchCompleted = new ManualResetEvent(false);
				this.syncRoot = new object();
			}

			internal long NotificationQueueTime { get; set; }

			public void Dispose()
			{
				lock (this.syncRoot)
				{
					if (this.searchCompleted != null)
					{
						this.searchCompleted.Dispose();
						this.searchCompleted = null;
					}
				}
			}

			public void Complete(Notification notification)
			{
				this.NotificationQueueTime = 0L;
				lock (this.syncRoot)
				{
					if (this.searchCompleted != null)
					{
						this.NotificationQueueTime = DateTime.UtcNow.Ticks - notification.CreateTime;
						this.searchCompleted.Set();
					}
				}
			}

			public bool Wait(int timeout)
			{
				return this.searchCompleted.WaitOne(timeout);
			}

			private ManualResetEvent searchCompleted;

			private object syncRoot;
		}
	}
}
