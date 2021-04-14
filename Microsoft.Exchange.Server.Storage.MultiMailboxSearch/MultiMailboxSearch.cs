using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.MultiMailboxSearch;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.FullTextIndex;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.MultiMailboxSearch
{
	[ComVisible(false)]
	internal class MultiMailboxSearch : IMultiMailboxSearch
	{
		internal MultiMailboxSearch(Guid databaseGuid, TimeSpan searchTimeOut)
		{
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.ctor");
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(databaseGuid != Guid.Empty, "Database guid cannot be empty.");
			this.searchTimeOut = searchTimeOut;
			this.databaseGuid = databaseGuid;
			if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.SearchTracer.TraceInformation<string, double>(36752, 0L, "Creating an instance of MultiMailboxSearch for database {0} with the time out interval {1} ms", databaseGuid.ToString(), this.searchTimeOut.TotalMilliseconds);
			}
			this.fullTextQueryManager = Hookable<IMultiMailboxSearchFullTextIndexQuery>.Create(true, new MultiMailboxSearchFullTextIndexQuery());
			MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.ctor");
		}

		public List<string> RefinersList
		{
			get
			{
				if (this.fullTextQueryManager.Value != null)
				{
					if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.SearchTracer.TraceInformation(53136, 0L, "Getting the required refiners list");
					}
					return this.fullTextQueryManager.Value.RefinersList;
				}
				return null;
			}
			set
			{
				if (this.fullTextQueryManager.Value != null)
				{
					if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.SearchTracer.TraceInformation(46992, 0L, "Setting the required refiners list");
					}
					this.fullTextQueryManager.Value.RefinersList = value;
				}
			}
		}

		public List<string> ExtraFieldsList
		{
			get
			{
				if (this.fullTextQueryManager.Value != null)
				{
					if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.SearchTracer.TraceInformation(63376, 0L, "Getting the required extra fields list");
					}
					return this.fullTextQueryManager.Value.ExtraFieldsList;
				}
				return null;
			}
			set
			{
				if (this.fullTextQueryManager.Value != null)
				{
					if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.SearchTracer.TraceInformation(38800, 0L, "Setting the required refiners list");
					}
					this.fullTextQueryManager.Value.ExtraFieldsList = value;
				}
			}
		}

		protected IMultiMailboxSearchFullTextIndexQuery FullTextIndexQuery
		{
			get
			{
				return this.fullTextQueryManager.Value;
			}
		}

		public ErrorCode GetKeywordStatistics(Context context, MultiMailboxSearchCriteria[] criteria, out IList<KeywordStatsResultRow> keywordStatsResult)
		{
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.GetKeywordStatistics");
			keywordStatsResult = new List<KeywordStatsResultRow>(0);
			if (context == null || criteria == null || criteria.Length <= 0)
			{
				if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.SearchTracer.TraceInformation<string>(55184, 0L, "Invalid eDiscovery keyword stats search criteria found on database instance {0}", this.databaseGuid.ToString());
				}
				MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.GetKeywordStatistics");
				return ErrorCode.CreateInvalidMultiMailboxKeywordStatsRequest((LID)46652U);
			}
			ErrorCode ec = ErrorCode.NoError;
			StorePerDatabasePerformanceCountersInstance perfInstance = PerformanceCounterFactory.GetDatabaseInstance(context.Database);
			if (perfInstance != null)
			{
				perfInstance.AverageMultiMailboxSearchKeywordCount.IncrementBy((long)criteria.Length);
				perfInstance.AverageMultiMailboxSearchKeywordCountBase.Increment();
			}
			IList<KeywordStatsResultRow> localResults = new List<KeywordStatsResultRow>(1);
			StopwatchStamp stamp = StopwatchStamp.GetStamp();
			bool isKeywordStatsSearch = true;
			TimeoutHandler.Execute(delegate()
			{
				ErrorCode ec = this.InternalKeywordStatsSearch(context, criteria, out localResults);
				if (ec == ErrorCode.NoError)
				{
					ec = ec;
				}
			}, this.searchTimeOut, delegate()
			{
				if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.SearchTracer.TraceInformation<int, string>(42896, 0L, "Keyword stats query {0} on database {1} timed out, aborting the query execution", (criteria != null) ? criteria.Length : 0, this.databaseGuid.ToString());
				}
				if (this.FullTextIndexQuery != null)
				{
					this.FullTextIndexQuery.Abort();
				}
				if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.SearchTracer.TraceInformation<int, string>(59280, 0L, "Keyword stats query {0} on database {1} aborted due to timeout, executing HandleTimeout", (criteria != null) ? criteria.Length : 0, this.databaseGuid.ToString());
				}
				ec = this.HandleTimeout(string.Empty, isKeywordStatsSearch, perfInstance, stamp);
			});
			keywordStatsResult = localResults;
			MultiMailboxSearch.UpdatePerformanceCounters(perfInstance, isKeywordStatsSearch, 0);
			MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.GetKeywordStatistics");
			return ec;
		}

		public ErrorCode Search(Context context, MultiMailboxSearchCriteria searchCriteria, out IList<FullTextIndexRow> results, out KeywordStatsResultRow keywordStatsResult, out Dictionary<string, List<RefinersResultRow>> refinersOutput)
		{
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.Search");
			ErrorCode ec = ErrorCode.NoError;
			results = new List<FullTextIndexRow>(0);
			keywordStatsResult = null;
			refinersOutput = new Dictionary<string, List<RefinersResultRow>>(0);
			if (context == null || searchCriteria == null || searchCriteria.SearchCriteria == null)
			{
				if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.SearchTracer.TraceInformation<string>(34704, 0L, "Invalid eDiscovery search criteria found on database instance {0}", this.databaseGuid.ToString());
				}
				ec = ErrorCode.CreateInvalidMultiMailboxSearchRequest((LID)47368U);
				MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.Search");
				return ec;
			}
			StorePerDatabasePerformanceCountersInstance perfInstance = PerformanceCounterFactory.GetDatabaseInstance(context.Database);
			StopwatchStamp stamp = StopwatchStamp.GetStamp();
			IList<FullTextIndexRow> localResults = new List<FullTextIndexRow>(searchCriteria.PageSize);
			Dictionary<string, List<RefinersResultRow>> localRefinersOutput = null;
			KeywordStatsResultRow localKeywordStatsResult = null;
			bool isKeywordStatsSearch = false;
			TimeoutHandler.Execute(delegate()
			{
				ErrorCode ec = this.InternalSearch(context, searchCriteria, out localResults, out localKeywordStatsResult, out localRefinersOutput);
				if (ec == ErrorCode.NoError)
				{
					ec = ec;
				}
			}, this.searchTimeOut, delegate()
			{
				if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.SearchTracer.TraceInformation<Guid, string, string>(51088, 0L, "Correlation Id:{0}. Query {1} on database {2} timed out, aborting the query execution", searchCriteria.QueryCorrelationId, searchCriteria.Query, this.databaseGuid.ToString());
				}
				this.FullTextIndexQuery.Abort();
				if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.SearchTracer.TraceInformation<Guid, string, string>(48016, 0L, "Correlation Id:{0}. Query {1} on database {2} aborted due to timeout, executing HandleTimeout", searchCriteria.QueryCorrelationId, searchCriteria.Query, this.databaseGuid.ToString());
				}
				ec = this.HandleTimeout(searchCriteria.Query, isKeywordStatsSearch, perfInstance, stamp);
			});
			int count = 0;
			if (results != null)
			{
				count = results.Count;
			}
			MultiMailboxSearch.UpdatePerformanceCounters(perfInstance, isKeywordStatsSearch, count);
			results = localResults;
			refinersOutput = localRefinersOutput;
			keywordStatsResult = localKeywordStatsResult;
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.Search");
			return ec;
		}

		internal ErrorCode ExecuteFullTextIndexQuery(Context context, Guid mailboxGuid, int mailboxNumber, string fulltextQuerystring, int pageSize, string sortSpec, out IList<FullTextIndexRow> resultList, out KeywordStatsResultRow keywordStatsResult, out Dictionary<string, List<RefinersResultRow>> refinersOutput)
		{
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.ExecuteFullTextIndexQuery");
			ErrorCode result = ErrorCode.NoError;
			resultList = new List<FullTextIndexRow>(0);
			refinersOutput = new Dictionary<string, List<RefinersResultRow>>(0);
			keywordStatsResult = null;
			StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(context.Database);
			StopwatchStamp stamp = StopwatchStamp.GetStamp();
			try
			{
				if (this.RefinersList != null)
				{
					int count = this.RefinersList.Count;
				}
				resultList = (List<FullTextIndexRow>)this.FullTextIndexQuery.ExecuteFullTextIndexQuery(this.databaseGuid, mailboxGuid, mailboxNumber, fulltextQuerystring, pageSize, sortSpec, out keywordStatsResult, out refinersOutput);
			}
			catch (TimeoutException exception)
			{
				context.OnExceptionCatch(exception);
				if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.SearchTracer.TraceError(56464, 0L, "Correlation Id:{0}. eDiscovery search for query {1} on database {2} timed out. The execution time was {3}", new object[]
					{
						this.FullTextIndexQuery.QueryCorrelationId,
						fulltextQuerystring,
						this.databaseGuid.ToString(),
						stamp.ElapsedTime
					});
				}
				result = this.HandleTimeout(fulltextQuerystring, false, databaseInstance, stamp);
			}
			catch (CommunicationException ex)
			{
				context.OnExceptionCatch(ex);
				if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.SearchTracer.TraceError(44176, 0L, "Correlation Id:{0}. eDiscovery search for query {1} on database {2} failed due to CommunicationException {3}.", new object[]
					{
						this.FullTextIndexQuery.QueryCorrelationId,
						fulltextQuerystring,
						this.databaseGuid.ToString(),
						ex.Message
					});
				}
				if (databaseInstance != null)
				{
					databaseInstance.MultiMailboxSearchesFTIndexFailed.Increment();
				}
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_FullTextIndexException, new object[]
				{
					string.Format("Correlation Id:{0}. eDiscovery search query execution on database {1} failed.", this.FullTextIndexQuery.QueryCorrelationId, this.databaseGuid.ToString()),
					ex.ToString()
				});
				result = ErrorCode.CreateMultiMailboxSearchFailed((LID)39176U);
			}
			finally
			{
				long incrementValue = (long)stamp.ElapsedTime.TotalMilliseconds;
				if (databaseInstance != null)
				{
					databaseInstance.AverageMultiMailboxSearchQueryLength.IncrementBy((long)fulltextQuerystring.Length);
					databaseInstance.AverageMultiMailboxSearchQueryLengthBase.Increment();
					databaseInstance.AverageMultiMailboxSearchTimeSpentInFTIndex.IncrementBy(incrementValue);
					databaseInstance.AverageMultiMailboxSearchTimeSpentInFTIndexBase.Increment();
				}
			}
			MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.ExecuteFullTextIndexQuery");
			return result;
		}

		internal ErrorCode ExecuteFullTextKeywordHitsQuery(Context context, Guid mailboxGuid, string fulltextQuerystring, out KeywordStatsResultRow keywordStatsResult)
		{
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.ExecuteFullTextKeywordHitsQuery");
			ErrorCode result = ErrorCode.NoError;
			keywordStatsResult = null;
			StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(context.Database);
			StopwatchStamp stamp = StopwatchStamp.GetStamp();
			try
			{
				keywordStatsResult = this.FullTextIndexQuery.ExecuteFullTextKeywordHitsQuery(this.databaseGuid, mailboxGuid, fulltextQuerystring);
			}
			catch (TimeoutException exception)
			{
				context.OnExceptionCatch(exception);
				if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.SearchTracer.TraceError<Guid, string, string>(60560, 0L, "Correlation Id:{0}. eDiscovery keyword stats query {1} on database {2} timed out", this.FullTextIndexQuery.QueryCorrelationId, fulltextQuerystring, this.databaseGuid.ToString());
				}
				result = this.HandleTimeout(fulltextQuerystring, true, databaseInstance, stamp);
			}
			catch (CommunicationException ex)
			{
				context.OnExceptionCatch(ex);
				if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.SearchTracer.TraceError(35984, 0L, "Correlation Id:{0}. eDiscovery keyword stats query {1} on database {2} failed due to CommunicationException {3}.", new object[]
					{
						this.FullTextIndexQuery.QueryCorrelationId,
						fulltextQuerystring,
						this.databaseGuid.ToString(),
						ex.Message
					});
				}
				if (databaseInstance != null)
				{
					databaseInstance.MultiMailboxSearchesFTIndexFailed.Increment();
				}
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_FullTextIndexException, new object[]
				{
					string.Format("eDiscovery search query execution on database {0} failed.", this.databaseGuid.ToString()),
					ex.ToString()
				});
				result = ErrorCode.CreateMultiMailboxSearchFailed((LID)55392U);
			}
			finally
			{
				long incrementValue = (long)stamp.ElapsedTime.TotalMilliseconds;
				if (databaseInstance != null)
				{
					databaseInstance.AverageMultiMailboxSearchQueryLength.IncrementBy((long)fulltextQuerystring.Length);
					databaseInstance.AverageMultiMailboxSearchQueryLengthBase.Increment();
					databaseInstance.AverageMultiMailboxSearchTimeSpentInFTIndex.IncrementBy(incrementValue);
					databaseInstance.AverageMultiMailboxSearchTimeSpentInFTIndexBase.Increment();
				}
			}
			MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.ExecuteFullTextKeywordHitsQuery");
			return result;
		}

		internal IDisposable SetFullTextIndexQueryTestHook(IMultiMailboxSearchFullTextIndexQuery fullTextQuery)
		{
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.SetFullTextIndexQueryTestHook");
			IDisposable result = this.fullTextQueryManager.SetTestHook(fullTextQuery);
			MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.SetFullTextIndexQueryTestHook");
			return result;
		}

		private static void TraceFunction(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}
			if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ExTraceGlobals.SearchTracer.TraceFunction(52752, 0L, message);
			}
		}

		private static void UpdatePerformanceCounters(StorePerDatabasePerformanceCountersInstance perfInstance, bool isKeywordStatsSearch, int count)
		{
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.UpdatePerformanceCounters");
			if (perfInstance != null)
			{
				if (count > 0 && !isKeywordStatsSearch)
				{
					perfInstance.AverageMultiMailboxPreviewSearchResultSize.IncrementBy((long)count);
					perfInstance.AverageMultiMailboxPreviewSearchResultSizeBase.Increment();
				}
				perfInstance.TotalMultiMailboxSearchFTIQueryExecution.Increment();
			}
			MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.UpdatePerformanceCounters");
		}

		private void TraceAndUpdateSearchTimedOutCounters(StorePerDatabasePerformanceCountersInstance perfInstance, string query, bool isKeywordStatsSearch, long timeTaken)
		{
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.TraceAndUpdateSearchTimedOutCounters");
			if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.SearchTracer.TraceInformation(64400, 0L, "MultiMailboxSearch call timeout.Query=\"{0}\", IsKeywordStats={1}, ElapsedTimeInMs={2}, TimeOutIntervalInMs={3}", new object[]
				{
					query,
					isKeywordStatsSearch,
					timeTaken,
					this.searchTimeOut.Milliseconds
				});
			}
			if (perfInstance == null)
			{
				return;
			}
			if (!isKeywordStatsSearch)
			{
				perfInstance.TotalMultiMailboxPreviewSearchesTimedOut.Increment();
			}
			else
			{
				perfInstance.TotalMultiMailboxKeywordStatsSearchesTimedOut.Increment();
			}
			MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.TraceAndUpdateSearchTimedOutCounters");
		}

		private ErrorCode InternalKeywordStatsSearch(Context context, MultiMailboxSearchCriteria[] criteria, out IList<KeywordStatsResultRow> keywordStatsResult)
		{
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.InternalKeywordStatsSearch");
			ErrorCode errorCode = ErrorCode.NoError;
			keywordStatsResult = new List<KeywordStatsResultRow>(criteria.Length);
			for (int i = 0; i < criteria.Length; i++)
			{
				if (criteria[i] == null || criteria[i].SearchCriteria == null)
				{
					if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
					{
						ExTraceGlobals.SearchTracer.TraceInformation<string, int>(39824, 0L, "eDiscovery keyword stats search on database {0}, invalid search criteria found at row {1}, returning InvalidMultiMailboxKeywordStatsRequest error", this.databaseGuid.ToString(), i + 1);
					}
					MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.InternalKeywordStatsSearch");
					errorCode = ErrorCode.CreateInvalidMultiMailboxKeywordStatsRequest((LID)63752U);
					break;
				}
				KeywordStatsResultRow keywordStatsResultRow = null;
				this.FullTextIndexQuery.QueryCorrelationId = criteria[i].QueryCorrelationId;
				string text = criteria[i].SearchCriteria.ToFqlString(FqlQueryGenerator.Options.LooseCheck, context.Culture);
				errorCode = this.ExecuteFullTextKeywordHitsQuery(context, criteria[i].MailboxGuid, text, out keywordStatsResultRow);
				if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.SearchTracer.TraceInformation(56208, 0L, "Correlation Id:{0}. eDiscovery keyword stats search on database {1}, executing query {2} at row {3} executed with the error code {4}", new object[]
					{
						this.FullTextIndexQuery.QueryCorrelationId,
						this.databaseGuid.ToString(),
						text,
						i + 1,
						errorCode.ToString()
					});
				}
				if (!(errorCode == ErrorCode.NoError))
				{
					break;
				}
				keywordStatsResult.Add(new KeywordStatsResultRow(criteria[i].Query, (keywordStatsResultRow != null) ? keywordStatsResultRow.Count : 0L, (keywordStatsResultRow != null) ? keywordStatsResultRow.Size : 0.0));
			}
			MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.InternalKeywordStatsSearch");
			return errorCode;
		}

		private ErrorCode InternalSearch(Context context, MultiMailboxSearchCriteria criteria, out IList<FullTextIndexRow> results, out KeywordStatsResultRow keywordStatsResult, out Dictionary<string, List<RefinersResultRow>> refinersOutput)
		{
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.InternalSearch");
			string text = criteria.SearchCriteria.ToFqlString(FqlQueryGenerator.Options.LooseCheck, context.Culture);
			if (!string.IsNullOrEmpty(criteria.PaginationClause) && !string.IsNullOrWhiteSpace(criteria.PaginationClause))
			{
				text = string.Format(criteria.PaginationClause, text);
			}
			if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.SearchTracer.TraceInformation(43920, 0L, "Correlation Id:{0}. Executing eDsicovery search query {1} on database {2}, page size:{3}, sort specification:{4}", new object[]
				{
					criteria.QueryCorrelationId,
					text,
					this.databaseGuid.ToString(),
					criteria.PageSize,
					criteria.SortSpecification
				});
			}
			this.FullTextIndexQuery.QueryCorrelationId = criteria.QueryCorrelationId;
			ErrorCode result = this.ExecuteFullTextIndexQuery(context, criteria.MailboxGuid, criteria.MailboxNumber, text, criteria.PageSize, criteria.SortSpecification, out results, out keywordStatsResult, out refinersOutput);
			MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.InternalSearch");
			return result;
		}

		private ErrorCode HandleTimeout(string fulltextQuerystring, bool isKeywordStatsSearch, StorePerDatabasePerformanceCountersInstance perfInstance, StopwatchStamp stamp)
		{
			MultiMailboxSearch.TraceFunction("Entering MultiMailboxSearch.HandleTimeout");
			ErrorCode result = isKeywordStatsSearch ? ErrorCode.CreateMultiMailboxKeywordStatsTimeOut((LID)51464U) : ErrorCode.CreateMultiMailboxSearchTimeOut((LID)35080U);
			this.TraceAndUpdateSearchTimedOutCounters(perfInstance, fulltextQuerystring, isKeywordStatsSearch, (long)stamp.ElapsedTime.TotalMilliseconds);
			MultiMailboxSearch.TraceFunction("Exiting MultiMailboxSearch.HandleTimeout");
			return result;
		}

		private readonly TimeSpan searchTimeOut;

		private readonly Guid databaseGuid;

		private Hookable<IMultiMailboxSearchFullTextIndexQuery> fullTextQueryManager;
	}
}
