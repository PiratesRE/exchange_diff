using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	internal class QueueAggregator
	{
		public QueueAggregator(QueueDigestGroupBy groupByKey, DetailsLevel detailsLevel) : this(groupByKey, detailsLevel, new NullQueueFilter(), null)
		{
		}

		public QueueAggregator(QueueDigestGroupBy groupByKey, DetailsLevel detailsLevel, IQueueFilter filter, TimeSpan? timeSpanForQueueDataBeingCurrent)
		{
			this.groupByKey = groupByKey;
			this.detailsLevel = detailsLevel;
			this.filter = filter;
			this.timeSpanForQueueDataBeingCurrent = timeSpanForQueueDataBeingCurrent;
			this.aggregationMap = new Dictionary<string, AggregatedQueueInfo>();
		}

		public void AddLocalQueue(LocalQueueInfo localQueue, DateTime timeStampOfQueue)
		{
			if (!this.filter.Match(localQueue))
			{
				return;
			}
			string groupByValue = QueueAggregator.GetGroupByValue(localQueue, this.groupByKey);
			AggregatedQueueInfo aggregatedQueueInfo;
			if (this.aggregationMap.TryGetValue(groupByValue, out aggregatedQueueInfo))
			{
				aggregatedQueueInfo.IncomingRate += localQueue.IncomingRate;
				aggregatedQueueInfo.OutgoingRate += localQueue.OutgoingRate;
			}
			else
			{
				aggregatedQueueInfo = new AggregatedQueueInfo
				{
					GroupByValue = groupByValue,
					IncomingRate = localQueue.IncomingRate,
					OutgoingRate = localQueue.OutgoingRate,
					NormalDetails = new List<AggregatedQueueNormalDetails>(),
					VerboseDetails = new List<AggregatedQueueVerboseDetails>()
				};
				this.aggregationMap[groupByValue] = aggregatedQueueInfo;
			}
			if (this.timeSpanForQueueDataBeingCurrent != null && timeStampOfQueue < DateTime.UtcNow - this.timeSpanForQueueDataBeingCurrent)
			{
				aggregatedQueueInfo.StaleMessageCount += localQueue.MessageCount;
			}
			else
			{
				aggregatedQueueInfo.MessageCount += localQueue.MessageCount;
				aggregatedQueueInfo.DeferredMessageCount += localQueue.DeferredMessageCount;
				aggregatedQueueInfo.LockedMessageCount += localQueue.LockedMessageCount;
			}
			QueueAggregator.UpdateAggregatedQueueDetails(aggregatedQueueInfo, localQueue, this.detailsLevel);
		}

		public void AddLocalQueues(IEnumerable<LocalQueueInfo> localQueues, DateTime timeStampOfQueues)
		{
			foreach (LocalQueueInfo localQueue in localQueues)
			{
				this.AddLocalQueue(localQueue, timeStampOfQueues);
			}
		}

		public void AddAggregatedQueue(AggregatedQueueInfo aggregatedQueue)
		{
			string groupByValue = aggregatedQueue.GroupByValue;
			AggregatedQueueInfo aggregatedQueueInfo;
			if (this.aggregationMap.TryGetValue(groupByValue, out aggregatedQueueInfo))
			{
				aggregatedQueueInfo.MessageCount += aggregatedQueue.MessageCount;
				aggregatedQueueInfo.DeferredMessageCount += aggregatedQueue.DeferredMessageCount;
				aggregatedQueueInfo.LockedMessageCount += aggregatedQueue.LockedMessageCount;
				aggregatedQueueInfo.StaleMessageCount += aggregatedQueue.StaleMessageCount;
				aggregatedQueueInfo.IncomingRate += aggregatedQueue.IncomingRate;
				aggregatedQueueInfo.OutgoingRate += aggregatedQueue.OutgoingRate;
				if (this.detailsLevel == DetailsLevel.Normal)
				{
					aggregatedQueueInfo.NormalDetails.AddRange(aggregatedQueue.NormalDetails);
					return;
				}
				if (this.detailsLevel == DetailsLevel.Verbose)
				{
					aggregatedQueueInfo.VerboseDetails.AddRange(aggregatedQueue.VerboseDetails);
					return;
				}
			}
			else
			{
				aggregatedQueueInfo = aggregatedQueue.Clone();
				this.aggregationMap[groupByValue] = aggregatedQueueInfo;
			}
		}

		public void AddAggregatedQueues(IEnumerable<AggregatedQueueInfo> aggregatedQueues)
		{
			foreach (AggregatedQueueInfo aggregatedQueue in aggregatedQueues)
			{
				this.AddAggregatedQueue(aggregatedQueue);
			}
		}

		public List<AggregatedQueueInfo> GetResultSortedByMessageCount(uint maxRecords)
		{
			List<AggregatedQueueInfo> list = new List<AggregatedQueueInfo>(from q in this.aggregationMap.Values
			orderby q.MessageCount descending
			select q);
			if ((long)list.Count > (long)((ulong)maxRecords))
			{
				int count = (int)Math.Min(2147483647L, (long)((ulong)maxRecords));
				list = list.GetRange(0, count);
			}
			return list;
		}

		private static string GetGroupByValue(LocalQueueInfo localQueue, QueueDigestGroupBy groupByKey)
		{
			string text;
			switch (groupByKey)
			{
			case QueueDigestGroupBy.NextHopDomain:
				text = localQueue.NextHopDomain;
				break;
			case QueueDigestGroupBy.NextHopCategory:
				text = localQueue.NextHopCategory;
				break;
			case QueueDigestGroupBy.NextHopKey:
				text = localQueue.NextHopKey;
				break;
			case QueueDigestGroupBy.DeliveryType:
				text = localQueue.DeliveryType;
				break;
			case QueueDigestGroupBy.Status:
				text = localQueue.Status;
				break;
			case QueueDigestGroupBy.RiskLevel:
				text = localQueue.RiskLevel;
				break;
			case QueueDigestGroupBy.LastError:
				if (localQueue.LastError == null)
				{
					text = string.Empty;
				}
				else if (localQueue.LastError.Length > 100)
				{
					text = localQueue.LastError.Substring(0, 100);
				}
				else
				{
					text = localQueue.LastError;
				}
				break;
			case QueueDigestGroupBy.ServerName:
				text = localQueue.ServerIdentity;
				break;
			case QueueDigestGroupBy.OutboundIPPool:
				text = localQueue.OutboundIPPool;
				break;
			default:
				throw new ArgumentException(string.Format("unsupported value for QueueDigestGroupBy: {0}", groupByKey));
			}
			if (text == null)
			{
				text = string.Empty;
			}
			return text;
		}

		private static void UpdateAggregatedQueueDetails(AggregatedQueueInfo aggregatedQueue, LocalQueueInfo localQueue, DetailsLevel detailsLevel)
		{
			if (detailsLevel == DetailsLevel.Normal)
			{
				AggregatedQueueNormalDetails item = new AggregatedQueueNormalDetails
				{
					QueueIdentity = localQueue.QueueIdentity,
					ServerIdentity = localQueue.ServerIdentity,
					MessageCount = localQueue.MessageCount
				};
				aggregatedQueue.NormalDetails.Add(item);
				return;
			}
			if (detailsLevel == DetailsLevel.Verbose)
			{
				AggregatedQueueVerboseDetails item2 = new AggregatedQueueVerboseDetails
				{
					QueueIdentity = localQueue.QueueIdentity,
					ServerIdentity = localQueue.ServerIdentity,
					MessageCount = localQueue.MessageCount,
					DeferredMessageCount = localQueue.DeferredMessageCount,
					LockedMessageCount = localQueue.LockedMessageCount,
					IncomingRate = localQueue.IncomingRate,
					OutgoingRate = localQueue.OutgoingRate,
					Velocity = localQueue.Velocity,
					NextHopDomain = localQueue.NextHopDomain,
					NextHopCategory = localQueue.NextHopCategory,
					NextHopConnector = localQueue.NextHopConnector,
					DeliveryType = localQueue.DeliveryType,
					Status = localQueue.Status,
					RiskLevel = localQueue.RiskLevel,
					OutboundIPPool = localQueue.OutboundIPPool,
					LastError = localQueue.LastError,
					TlsDomain = localQueue.TlsDomain
				};
				aggregatedQueue.VerboseDetails.Add(item2);
			}
		}

		private const int LastErrorPrefixLength = 100;

		private readonly QueueDigestGroupBy groupByKey;

		private readonly DetailsLevel detailsLevel;

		private readonly IQueueFilter filter;

		private readonly TimeSpan? timeSpanForQueueDataBeingCurrent;

		private IDictionary<string, AggregatedQueueInfo> aggregationMap;
	}
}
