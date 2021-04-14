using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class RequestStreamTooBigException : ServicePermanentException
	{
		public RequestStreamTooBigException() : base(ResponseCodeType.ErrorRequestStreamTooBig, CoreResources.IDs.ErrorInvalidRequestQuotaExceeded)
		{
		}

		public RequestStreamTooBigException(Exception innerException) : base(ResponseCodeType.ErrorRequestStreamTooBig, CoreResources.IDs.ErrorInvalidRequestQuotaExceeded, innerException)
		{
		}

		public RequestStreamTooBigException(Enum messageId) : base(ResponseCodeType.ErrorRequestStreamTooBig, messageId)
		{
		}

		public RequestStreamTooBigException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorRequestStreamTooBig, messageId, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}
