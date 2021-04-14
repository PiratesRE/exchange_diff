using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.UserPhotos
{
	internal sealed class UserPhotoQuery : BaseQuery
	{
		public new UserPhotoQueryResult Result
		{
			get
			{
				return (UserPhotoQueryResult)base.Result;
			}
		}

		public static UserPhotoQuery CreateFromUnknown(RecipientData data, LocalizedException exception, ITracer upstreamTracer)
		{
			return new UserPhotoQuery(data, new UserPhotoQueryResult(exception, upstreamTracer));
		}

		public static UserPhotoQuery CreateFromIndividual(RecipientData data, ITracer upstreamTracer)
		{
			return new UserPhotoQuery(data, null);
		}

		public static UserPhotoQuery CreateFromIndividual(RecipientData data, LocalizedException exception, ITracer upstreamTracer)
		{
			return new UserPhotoQuery(data, new UserPhotoQueryResult(exception, upstreamTracer));
		}

		private UserPhotoQuery(RecipientData recipientData, UserPhotoQueryResult result) : base(recipientData, result)
		{
		}
	}
}
