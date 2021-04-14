using System;
using System.Collections.Generic;
using Microsoft.Exchange.Net.DiagnosticsAggregation;

namespace Microsoft.Exchange.Data.QueueDigest
{
	[Serializable]
	public class QueueDigestPresentationObject : ConfigurableObject
	{
		public QueueDigestPresentationObject() : base(new SimpleProviderPropertyBag())
		{
		}

		public string GroupByValue { get; set; }

		public int MessageCount { get; set; }

		public int DeferredMessageCount { get; set; }

		public int LockedMessageCount { get; set; }

		public int StaleMessageCount { get; set; }

		public double IncomingRate { get; set; }

		public double OutgoingRate { get; set; }

		public List<QueueDigestDetails> Details { get; set; }

		internal static QueueDigestPresentationObject Create(AggregatedQueueInfo aggregatedQueueInfo)
		{
			QueueDigestPresentationObject queueDigestPresentationObject = new QueueDigestPresentationObject();
			queueDigestPresentationObject.GroupByValue = aggregatedQueueInfo.GroupByValue;
			queueDigestPresentationObject.MessageCount = aggregatedQueueInfo.MessageCount;
			queueDigestPresentationObject.DeferredMessageCount = aggregatedQueueInfo.DeferredMessageCount;
			queueDigestPresentationObject.LockedMessageCount = aggregatedQueueInfo.LockedMessageCount;
			queueDigestPresentationObject.StaleMessageCount = aggregatedQueueInfo.StaleMessageCount;
			queueDigestPresentationObject.IncomingRate = aggregatedQueueInfo.IncomingRate;
			queueDigestPresentationObject.OutgoingRate = aggregatedQueueInfo.OutgoingRate;
			queueDigestPresentationObject.Details = new List<QueueDigestDetails>();
			if (aggregatedQueueInfo.NormalDetails != null && aggregatedQueueInfo.NormalDetails.Count > 0)
			{
				using (List<AggregatedQueueNormalDetails>.Enumerator enumerator = aggregatedQueueInfo.NormalDetails.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AggregatedQueueNormalDetails details = enumerator.Current;
						queueDigestPresentationObject.Details.Add(new QueueDigestDetails(details));
					}
					goto IL_112;
				}
			}
			if (aggregatedQueueInfo.VerboseDetails != null && aggregatedQueueInfo.VerboseDetails.Count > 0)
			{
				foreach (AggregatedQueueVerboseDetails details2 in aggregatedQueueInfo.VerboseDetails)
				{
					queueDigestPresentationObject.Details.Add(new QueueDigestDetails(details2));
				}
			}
			IL_112:
			queueDigestPresentationObject.Details.Sort(new Comparison<QueueDigestDetails>(QueueDigestPresentationObject.CompareQueueDigestDetails));
			return queueDigestPresentationObject;
		}

		internal static QueueDigestPresentationObject Create(TransportQueueStatistics mtrtQueueAggregate, QueueDigestGroupBy groupBy)
		{
			QueueDigestPresentationObject queueDigestPresentationObject = new QueueDigestPresentationObject();
			queueDigestPresentationObject.DeferredMessageCount = mtrtQueueAggregate.DeferredMessageCount;
			queueDigestPresentationObject.IncomingRate = mtrtQueueAggregate.IncomingRate;
			queueDigestPresentationObject.LockedMessageCount = mtrtQueueAggregate.LockedMessageCount;
			queueDigestPresentationObject.MessageCount = mtrtQueueAggregate.MessageCount;
			queueDigestPresentationObject.OutgoingRate = mtrtQueueAggregate.OutgoingRate;
			QueueDigestPresentationObject.SetGroupByValue(queueDigestPresentationObject, mtrtQueueAggregate, groupBy);
			if (mtrtQueueAggregate.QueueLogs != null)
			{
				queueDigestPresentationObject.Details = new List<QueueDigestDetails>();
				foreach (TransportQueueLog details in mtrtQueueAggregate.QueueLogs)
				{
					queueDigestPresentationObject.Details.Add(new QueueDigestDetails(details));
				}
			}
			queueDigestPresentationObject.Details.Sort(new Comparison<QueueDigestDetails>(QueueDigestPresentationObject.CompareQueueDigestDetails));
			return queueDigestPresentationObject;
		}

		private static int CompareQueueDigestDetails(QueueDigestDetails lhs, QueueDigestDetails rhs)
		{
			return rhs.MessageCount - lhs.MessageCount;
		}

		private static void SetGroupByValue(QueueDigestPresentationObject result, TransportQueueStatistics mtrtResponse, QueueDigestGroupBy groupBy)
		{
			string groupByValue = string.Empty;
			switch (groupBy)
			{
			case QueueDigestGroupBy.NextHopDomain:
				groupByValue = mtrtResponse.NextHopDomain;
				break;
			case QueueDigestGroupBy.NextHopCategory:
				groupByValue = mtrtResponse.NextHopCategory;
				break;
			case QueueDigestGroupBy.NextHopKey:
				groupByValue = mtrtResponse.NextHopKey;
				break;
			case QueueDigestGroupBy.DeliveryType:
				groupByValue = mtrtResponse.DeliveryType;
				break;
			case QueueDigestGroupBy.Status:
				groupByValue = mtrtResponse.Status;
				break;
			case QueueDigestGroupBy.RiskLevel:
				groupByValue = mtrtResponse.RiskLevel;
				break;
			case QueueDigestGroupBy.LastError:
				groupByValue = mtrtResponse.LastError;
				break;
			case QueueDigestGroupBy.ServerName:
				groupByValue = mtrtResponse.ServerName;
				break;
			case QueueDigestGroupBy.OutboundIPPool:
				groupByValue = mtrtResponse.OutboundIPPool;
				break;
			default:
				throw new ArgumentOutOfRangeException("groupBy");
			}
			result.GroupByValue = groupByValue;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return QueueDigestPresentationObject.schema;
			}
		}

		private static QueueDigestPresentationObjectSchema schema = ObjectSchema.GetInstance<QueueDigestPresentationObjectSchema>();
	}
}
