using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class FindMessageTrackingBaseQuery : BaseQuery
	{
		public new FindMessageTrackingBaseQueryResult Result
		{
			get
			{
				return (FindMessageTrackingBaseQueryResult)base.Result;
			}
		}

		public static FindMessageTrackingBaseQuery CreateFromUnknown(RecipientData recipientData, LocalizedException exception)
		{
			return new FindMessageTrackingBaseQuery(recipientData, new FindMessageTrackingBaseQueryResult(exception));
		}

		public static FindMessageTrackingBaseQuery CreateFromIndividual(RecipientData recipientData)
		{
			return new FindMessageTrackingBaseQuery(recipientData, null);
		}

		public static FindMessageTrackingBaseQuery CreateFromIndividual(RecipientData recipientData, LocalizedException exception)
		{
			return new FindMessageTrackingBaseQuery(recipientData, new FindMessageTrackingBaseQueryResult(exception));
		}

		private FindMessageTrackingBaseQuery(RecipientData recipientData, FindMessageTrackingBaseQueryResult result) : base(recipientData, result)
		{
		}
	}
}
