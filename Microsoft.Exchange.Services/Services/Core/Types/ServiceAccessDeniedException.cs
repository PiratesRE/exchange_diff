using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ServiceAccessDeniedException : ServicePermanentException
	{
		public ServiceAccessDeniedException() : base(ResponseCodeType.ErrorAccessDenied, (CoreResources.IDs)3579904699U)
		{
		}

		public ServiceAccessDeniedException(Enum messageId) : base(ResponseCodeType.ErrorAccessDenied, messageId)
		{
		}

		public ServiceAccessDeniedException(Exception innerException) : base(ResponseCodeType.ErrorAccessDenied, (CoreResources.IDs)3579904699U, innerException)
		{
		}

		public ServiceAccessDeniedException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorAccessDenied, messageId, innerException)
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
