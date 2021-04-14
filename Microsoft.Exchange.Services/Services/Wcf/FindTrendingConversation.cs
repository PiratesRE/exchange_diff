using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Inference.Ranking;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class FindTrendingConversation
	{
		internal FindTrendingConversation(IdAndSession session, int pageSize)
		{
			if (FindTrendingConversation.FetchList == null)
			{
				FindTrendingConversation.Init();
			}
			this.session = session;
			this.pageSize = pageSize;
		}

		internal ServiceResult<ConversationType[]> Execute()
		{
			ConversationType[] array = null;
			ExDateTime utcNow = ExDateTime.UtcNow;
			MailboxSession mailboxSession = this.session.Session as MailboxSession;
			StorePerformanceCountersCapture storePerformanceCountersCapture = StorePerformanceCountersCapture.Start(mailboxSession);
			using (Folder folder = Folder.Bind(this.session.Session, this.session.Id))
			{
				using (IQueryResult queryResult = folder.AggregatedConversationQuery(null, FindTrendingConversation.Sort, FindTrendingConversation.FetchList, new ConversationAggregationExtension(mailboxSession)))
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(FindTrendingConversation.ConversationsToProcess);
					StorePerformanceCounters counters = storePerformanceCountersCapture.Stop();
					ExDateTime utcNow2 = ExDateTime.UtcNow;
					Stopwatch stopwatch = Stopwatch.StartNew();
					List<KeyValuePair<ConversationType, double>> list = new List<KeyValuePair<ConversationType, double>>(propertyBags.Count<IStorePropertyBag>());
					for (int i = 0; i < propertyBags.Count<IStorePropertyBag>(); i++)
					{
						ConversationType key = ConversationType.LoadFromAggregatedConversation(propertyBags[i], mailboxSession, FindTrendingConversation.ConversationPropertiesToReturn);
						double value = FindTrendingConversation.TrendingModel.Rank(propertyBags[i]);
						list.Add(new KeyValuePair<ConversationType, double>(key, value));
					}
					list.Sort((KeyValuePair<ConversationType, double> a, KeyValuePair<ConversationType, double> b) => b.Value.CompareTo(a.Value));
					list.Take(this.pageSize);
					array = list.ConvertAll<ConversationType>((KeyValuePair<ConversationType, double> a) => a.Key).ToArray();
					stopwatch.Stop();
					FindTrendingConversation.LogExecutionDetails(counters, utcNow, utcNow2, stopwatch.ElapsedMilliseconds, array);
				}
			}
			return new ServiceResult<ConversationType[]>(array);
		}

		private static void Init()
		{
			HashSet<PropertyDefinition> aggregatedConversationDependencies = AggregatedConversationLoader.GetAggregatedConversationDependencies(FindTrendingConversation.ConversationPropertiesToReturn);
			foreach (PropertyDefinition item in FindTrendingConversation.TrendingModel.Dependencies)
			{
				aggregatedConversationDependencies.Add(item);
			}
			FindTrendingConversation.FetchList = aggregatedConversationDependencies;
		}

		private static void LogExecutionDetails(StorePerformanceCounters counters, ExDateTime startUtcTime, ExDateTime endUtcTime, long processingTime, ConversationType[] returnedConversations)
		{
			RequestDetailsLogger.Current.Set(FindTrendingConversationMetadata.AggregatedConversationQueryTime, counters.ElapsedMilliseconds);
			RequestDetailsLogger.Current.Set(FindTrendingConversationMetadata.AggregatedConversationQueryCPUTime, counters.Cpu);
			RequestDetailsLogger.Current.Set(FindTrendingConversationMetadata.AggregatedConversationQueryRpcCount, counters.RpcCount);
			RequestDetailsLogger.Current.Set(FindTrendingConversationMetadata.AggregatedConversationQueryRpcLatency, counters.RpcLatency);
			RequestDetailsLogger.Current.Set(FindTrendingConversationMetadata.AggregatedConversationQueryRpcLatencyOnStore, counters.RpcLatencyOnStore);
			RequestDetailsLogger.Current.Set(FindTrendingConversationMetadata.AggregatedConversationQueryStartTimestamp, SearchUtil.FormatIso8601String(startUtcTime));
			RequestDetailsLogger.Current.Set(FindTrendingConversationMetadata.AggregatedConversationQueryEndTimestamp, SearchUtil.FormatIso8601String(endUtcTime));
			RequestDetailsLogger.Current.Set(FindTrendingConversationMetadata.AggregatedConversationRankingTime, processingTime);
			RequestDetailsLogger.Current.Set(FindTrendingConversationMetadata.TotalRowCount, returnedConversations.Length);
			foreach (ConversationType conversationType in returnedConversations)
			{
				RequestDetailsLogger.Current.SafeLogWithSeparator(FindTrendingConversationMetadata.ReturnedTrendingConversations, conversationType.ConversationId.ToString());
			}
		}

		private static readonly ConversationTrendingModel TrendingModel = new ConversationTrendingModel();

		private static readonly SortBy[] Sort = new SortBy[]
		{
			new SortBy(AggregatedConversationSchema.LastDeliveryTime, SortOrder.Descending)
		};

		private static readonly int ConversationsToProcess = 200;

		public static readonly PropertyUriEnum[] ConversationPropertiesToReturn = new PropertyUriEnum[]
		{
			PropertyUriEnum.ConversationId,
			PropertyUriEnum.Topic,
			PropertyUriEnum.ConversationLastDeliveryTime,
			PropertyUriEnum.ConversationLastModifiedTime,
			PropertyUriEnum.ConversationGlobalItemIds,
			PropertyUriEnum.ConversationGlobalMessageCount,
			PropertyUriEnum.ConversationUniqueSenders,
			PropertyUriEnum.ConversationInstanceKey,
			PropertyUriEnum.ConversationHasAttachments,
			PropertyUriEnum.ConversationHasIrm,
			PropertyUriEnum.ConversationPreview,
			PropertyUriEnum.ConversationImportance,
			PropertyUriEnum.ConversationUnreadCount,
			PropertyUriEnum.ConversationGlobalUnreadCount,
			PropertyUriEnum.ConversationItemClasses
		};

		private static HashSet<PropertyDefinition> FetchList;

		private readonly IdAndSession session;

		private readonly int pageSize;
	}
}
