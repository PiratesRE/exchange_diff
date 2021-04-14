using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class GetMessageTrackingBaseQuery : BaseQuery
	{
		public new GetMessageTrackingBaseQueryResult Result
		{
			get
			{
				return (GetMessageTrackingBaseQueryResult)base.Result;
			}
		}

		public static GetMessageTrackingBaseQuery CreateFromUnknown(RecipientData recipientData, LocalizedException exception)
		{
			return new GetMessageTrackingBaseQuery(recipientData, new GetMessageTrackingBaseQueryResult(exception));
		}

		public static GetMessageTrackingBaseQuery CreateFromIndividual(RecipientData recipientData)
		{
			return new GetMessageTrackingBaseQuery(recipientData, null);
		}

		public static GetMessageTrackingBaseQuery CreateFromIndividual(RecipientData recipientData, LocalizedException exception)
		{
			return new GetMessageTrackingBaseQuery(recipientData, new GetMessageTrackingBaseQueryResult(exception));
		}

		private GetMessageTrackingBaseQuery(RecipientData recipientData, GetMessageTrackingBaseQueryResult result) : base(recipientData, result)
		{
		}
	}
}
