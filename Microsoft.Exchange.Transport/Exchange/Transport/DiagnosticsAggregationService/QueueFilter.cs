using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Net.DiagnosticsAggregation;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Transport.DiagnosticsAggregationService
{
	internal class QueueFilter : IQueueFilter
	{
		private QueueFilter(PagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema> pagingEngine)
		{
			this.pagingEngine = pagingEngine;
		}

		public static bool TryParse(string filterString, out IQueueFilter result)
		{
			ParsingException ex;
			return QueueFilter.TryParse(filterString, out result, out ex);
		}

		public static bool TryParse(string filterString, out IQueueFilter result, out ParsingException parsingException)
		{
			result = null;
			parsingException = null;
			if (string.IsNullOrEmpty(filterString))
			{
				result = new NullQueueFilter();
				return true;
			}
			QueryFilter filter;
			try
			{
				MonadFilter monadFilter = new MonadFilter(filterString, null, ObjectSchema.GetInstance<ExtensibleQueueInfoSchema>());
				filter = DateTimeConverter.ConvertQueryFilter(monadFilter.InnerFilter);
			}
			catch (ParsingException ex)
			{
				parsingException = ex;
				return false;
			}
			PagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema> pagingEngine = new PagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema>();
			pagingEngine.SetFilter(filter);
			result = new QueueFilter(pagingEngine);
			return true;
		}

		public bool Match(LocalQueueInfo localQueue)
		{
			ExtensibleQueueInfo dataObject;
			bool flag = QueueFilter.TryCreateExtensibleQueueInfo(localQueue, out dataObject);
			return flag && this.pagingEngine.ApplyFilterConditions(dataObject);
		}

		private static bool TryCreateExtensibleQueueInfo(LocalQueueInfo localQueue, out ExtensibleQueueInfo result)
		{
			result = null;
			ExtensibleQueueInfo extensibleQueueInfo = new PropertyBagBasedQueueInfo(new QueueIdentity(QueueType.Delivery, 1L, localQueue.NextHopDomain));
			extensibleQueueInfo.NextHopDomain = localQueue.NextHopDomain;
			extensibleQueueInfo.MessageCount = localQueue.MessageCount;
			extensibleQueueInfo.DeferredMessageCount = localQueue.DeferredMessageCount;
			extensibleQueueInfo.LockedMessageCount = localQueue.LockedMessageCount;
			extensibleQueueInfo.IncomingRate = localQueue.IncomingRate;
			extensibleQueueInfo.OutgoingRate = localQueue.OutgoingRate;
			extensibleQueueInfo.Velocity = localQueue.Velocity;
			extensibleQueueInfo.LastError = localQueue.LastError;
			extensibleQueueInfo.TlsDomain = localQueue.TlsDomain;
			extensibleQueueInfo.NextHopConnector = localQueue.NextHopConnector;
			NextHopCategory nextHopCategory;
			if (!Enum.TryParse<NextHopCategory>(localQueue.NextHopCategory, out nextHopCategory))
			{
				return false;
			}
			extensibleQueueInfo.NextHopCategory = nextHopCategory;
			DeliveryType deliveryType;
			if (!Enum.TryParse<DeliveryType>(localQueue.DeliveryType, out deliveryType))
			{
				return false;
			}
			extensibleQueueInfo.DeliveryType = deliveryType;
			RiskLevel riskLevel;
			if (!Enum.TryParse<RiskLevel>(localQueue.RiskLevel, out riskLevel))
			{
				return false;
			}
			extensibleQueueInfo.RiskLevel = riskLevel;
			QueueStatus status;
			if (!Enum.TryParse<QueueStatus>(localQueue.Status, out status))
			{
				return false;
			}
			extensibleQueueInfo.Status = status;
			result = extensibleQueueInfo;
			return true;
		}

		private PagingEngine<ExtensibleQueueInfo, ExtensibleQueueInfoSchema> pagingEngine;
	}
}
